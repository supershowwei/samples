IF OBJECT_ID('dbo.StatsUpdateLog', 'U') IS NULL
    BEGIN
        CREATE TABLE dbo.StatsUpdateLog (
            Id INT IDENTITY (1, 1) PRIMARY KEY
           ,RunId UNIQUEIDENTIFIER NOT NULL
           ,SchemaName SYSNAME NOT NULL
           ,ObjectName SYSNAME NOT NULL
           ,ObjectType CHAR(2) NULL
           ,[RowCount] BIGINT NULL
           ,ReservedMB DECIMAL(18, 2) NULL
           ,StatsCount INT NULL
           ,MaxModCount BIGINT NULL  -- 更新前的最大異動筆數
           ,SumModCount BIGINT NULL
           ,SamplePercent INT NULL   -- 100 代表 FULLSCAN
           ,StartTime DATETIME2(3) NOT NULL
           ,EndTime DATETIME2(3) NULL
           ,DurationMs AS DATEDIFF(MILLISECOND, StartTime, EndTime)
           ,ErrorMessage NVARCHAR(4000) NULL
        );
        CREATE INDEX IX_StatsUpdateLog_RunId ON dbo.StatsUpdateLog (RunId) INCLUDE (ObjectName);
        CREATE INDEX IX_StatsUpdateLog_StartTime ON dbo.StatsUpdateLog (StartTime);
    END

-- 索引維護紀錄表
IF OBJECT_ID('dbo.IndexMaintenanceLog', 'U') IS NULL
    BEGIN
        CREATE TABLE dbo.IndexMaintenanceLog (
            Id INT IDENTITY (1, 1) PRIMARY KEY
           ,RunId UNIQUEIDENTIFIER NOT NULL
           ,SchemaName SYSNAME NOT NULL
           ,ObjectName SYSNAME NOT NULL
           ,IndexName SYSNAME NOT NULL
           ,IndexType NVARCHAR(60) NULL
           ,PageCount BIGINT NULL
           ,Fragmentation DECIMAL(5, 2) NULL  -- 處理前的碎片率
           ,Operation VARCHAR(10) NOT NULL    -- REBUILD / REORGANIZE
           ,StartTime DATETIME2(3) NOT NULL
           ,EndTime DATETIME2(3) NULL
           ,DurationMs AS DATEDIFF(MILLISECOND, StartTime, EndTime)
           ,ErrorMessage NVARCHAR(4000) NULL
        );
        CREATE INDEX IX_IndexMaintenanceLog_RunId ON dbo.IndexMaintenanceLog (RunId) INCLUDE (ObjectName, IndexName);
        CREATE INDEX IX_IndexMaintenanceLog_StartTime ON dbo.IndexMaintenanceLog (StartTime);
    END
GO

SET NOCOUNT ON;

DECLARE @SamplePercent INT = NULL;  -- 全域預設；NULL = FULLSCAN
DECLARE @Verbose BIT = 0;
DECLARE @RetentionDays INT = 7;     -- 紀錄表保留天數
DECLARE @TestObject SYSNAME = NULL;  -- 測試完改回 NULL 才會跑全庫

-- 索引維護參數
DECLARE @IndexMaintenance BIT = 1;               -- 0 = 只更新統計，不做索引維護
DECLARE @ReorgThreshold DECIMAL(5, 2) = 5.0;     -- 碎片率 >= 此值才 REORGANIZE
DECLARE @RebuildThreshold DECIMAL(5, 2) = 30.0;  -- 碎片率 >= 此值改為 REBUILD
DECLARE @MinPageCount INT = 1000;                -- 頁數低於此值的索引不處理（約 8MB）
DECLARE @OnlineRebuild BIT = 0;                  -- 1 = REBUILD 加 ONLINE（需 Enterprise/Developer）

IF @SamplePercent IS NOT NULL
AND @SamplePercent NOT BETWEEN 1 AND 100
    BEGIN
        RAISERROR (N'@SamplePercent 必須為 NULL（FULLSCAN）或 1~100。', 16, 1);
        RETURN;
    END

IF @ReorgThreshold > @RebuildThreshold
    BEGIN
        RAISERROR (N'@ReorgThreshold 不可大於 @RebuildThreshold。', 16, 1);
        RETURN;
    END

/* ============================================================================
   個別物件取樣率覆寫（精確名稱比對）
   ----------------------------------------------------------------------------
   SchemaName   : 填 NULL 表示不限 schema；同名物件存在於多個 schema 時請明確指定
   ObjectName   : 完整物件名稱，不支援萬用字元
   SamplePercent: 1~100
   未列於此的物件一律套用 @SamplePercent（預設 FULLSCAN）
   ============================================================================ */
DECLARE @SampleOverride TABLE (
    SchemaName SYSNAME NULL
   ,ObjectName SYSNAME NOT NULL
   ,SamplePercent INT NOT NULL
);

INSERT @SampleOverride (SchemaName, ObjectName, SamplePercent) VALUES
    (N'dbo', N'DailyCandlestickTechnicalIndicator', NULL),
    (N'dbo', N'SkyrocketTechnicalIndicator',        NULL),
    (N'dbo', N'MinuteTradeStatistics',              NULL),
    (N'dbo', N'FiveMinutesTradeStatistics',         NULL),
    (N'dbo', N'QuoteAccumulation',                  NULL);

IF EXISTS (SELECT
            1
        FROM @SampleOverride
        WHERE SamplePercent NOT BETWEEN 1 AND 100)
    BEGIN
        RAISERROR (N'@SampleOverride 中的 SamplePercent 必須為 1~100。', 16, 1);
        RETURN;
    END

-- 覆寫清單中若有名稱對不到任何物件（例如打錯字），發出警告避免默默套用預設值
DECLARE @orphan NVARCHAR(MAX);

SELECT @orphan = STRING_AGG(CONVERT(NVARCHAR(MAX), ISNULL(ov.SchemaName, N'(any)') + N'.' + ov.ObjectName), N', ')
FROM @SampleOverride ov
WHERE NOT EXISTS (SELECT
            1
        FROM sys.objects o
        JOIN sys.schemas s
            ON s.schema_id = o.schema_id
        WHERE o.name = ov.ObjectName
        AND (ov.SchemaName IS NULL
            OR s.name = ov.SchemaName)
);

IF @orphan IS NOT NULL
    RAISERROR (N'警告：以下覆寫項目找不到對應物件：%s', 0, 1, @orphan) WITH NOWAIT;

/* ============================================================================
   清除逾期紀錄（分批刪除，避免單一交易過大）
   ============================================================================ */
DECLARE @cutoff DATETIME2(3) = DATEADD(DAY, -@RetentionDays, SYSDATETIME());
DECLARE @purged INT = 1;

WHILE @purged > 0
BEGIN
    DELETE TOP (5000)
    FROM dbo.StatsUpdateLog
    WHERE StartTime < @cutoff;

    SET @purged = @@ROWCOUNT;
END

-- 清除索引維護紀錄
SET @purged = 1;

WHILE @purged > 0
BEGIN
    DELETE TOP (5000)
    FROM dbo.IndexMaintenanceLog
    WHERE StartTime < @cutoff;

    SET @purged = @@ROWCOUNT;
END

/* ============================================================================
   更新統計資訊 + 索引維護
   ============================================================================ */
DECLARE @RunId UNIQUEIDENTIFIER = NEWID();
DECLARE @schema SYSNAME
       ,@object SYSNAME
       ,@type CHAR(2)
       ,@objId INT
       ,@effSample INT
       ,@rowCount BIGINT
       ,@reservedMB DECIMAL(18, 2)
       ,@statsCount INT
       ,@maxMod BIGINT
       ,@sumMod BIGINT
       ,@sql NVARCHAR(MAX)
       ,@logId INT
       ,@msg NVARCHAR(300)
       -- 索引維護用變數
       ,@idxName SYSNAME
       ,@idxType NVARCHAR(60)
       ,@idxPages BIGINT
       ,@idxFrag DECIMAL(5, 2)
       ,@op VARCHAR(10)
       ,@idxLogId INT
       ,@sqlIdx NVARCHAR(MAX);

DECLARE c CURSOR LOCAL FAST_FORWARD FOR SELECT
    s.name
   ,o.name
   ,o.type
   ,o.object_id
FROM sys.objects o
JOIN sys.schemas s
    ON s.schema_id = o.schema_id
WHERE o.is_ms_shipped = 0
AND o.type IN ('U', 'V')     -- 檢視表只有建了索引才有統計資訊可更新
AND (@TestObject IS NULL OR o.name = @TestObject)
AND (o.type = 'U'
    OR EXISTS (SELECT
            1
        FROM sys.indexes i
        WHERE i.object_id = o.object_id)
)
ORDER BY s.name, o.name;

OPEN c;
FETCH NEXT FROM c INTO @schema, @object, @type, @objId;

WHILE @@FETCH_STATUS = 0
BEGIN
    SELECT
        @rowCount = SUM(CASE
            WHEN ps.index_id IN (0, 1) THEN ps.row_count
            ELSE 0
        END)
       ,@reservedMB = CAST(SUM(ps.reserved_page_count) * 8.0 / 1024 AS DECIMAL(18, 2))
    FROM sys.dm_db_partition_stats ps
    WHERE ps.object_id = @objId;

    SELECT
        @statsCount = COUNT(*)
       ,@maxMod = MAX(sp.modification_counter)
       ,@sumMod = SUM(sp.modification_counter)
    FROM sys.stats s
    OUTER APPLY sys.dm_db_stats_properties(s.object_id, s.stats_id) sp
    WHERE s.object_id = @objId;

    -- 完全沒有統計資訊的物件，呼叫 UPDATE STATISTICS 是空轉，直接跳過
    IF @statsCount = 0
        BEGIN
            FETCH NEXT FROM c INTO @schema, @object, @type, @objId;
            CONTINUE;
        END

    /* 決定本次取樣率：先回復為全域預設，再查覆寫清單。
       若要區分大小寫，於下方兩個名稱比對條件各加上
       COLLATE Latin1_General_100_BIN2 */
    SET @effSample = @SamplePercent;

    SELECT TOP 1
        @effSample = ov.SamplePercent
    FROM @SampleOverride ov
    WHERE ov.ObjectName = @object
    AND (ov.SchemaName IS NULL
        OR ov.SchemaName = @schema)
    ORDER BY CASE
        WHEN ov.SchemaName IS NULL THEN 1
        ELSE 0
    END;  -- 明確指定 schema 者優先

    INSERT dbo.StatsUpdateLog (RunId
                              ,SchemaName
                              ,ObjectName
                              ,ObjectType
                              ,[RowCount]
                              ,ReservedMB
                              ,StatsCount
                              ,MaxModCount
                              ,SumModCount
                              ,SamplePercent
                              ,StartTime)
        VALUES (@RunId
               ,@schema
               ,@object
               ,@type
               ,@rowCount
               ,@reservedMB
               ,@statsCount
               ,@maxMod
               ,@sumMod
               ,ISNULL(@effSample, 100)
               ,SYSDATETIME());

    SET @logId = SCOPE_IDENTITY();

    IF @Verbose = 1
        BEGIN
            SET @msg = CONVERT(VARCHAR(8), SYSDATETIME(), 108) + '  '
                     + QUOTENAME(@schema) + '.' + QUOTENAME(@object)
                     + '  ' + CASE
                                  WHEN @effSample IS NULL THEN 'FULLSCAN'
                                  ELSE 'SAMPLE ' + CONVERT(VARCHAR(10), @effSample) + '%'
                              END;
            RAISERROR (N'%s', 0, 1, @msg) WITH NOWAIT;  -- 參數化，避免名稱含 % 被當格式符號
        END

    SET @sql = N'UPDATE STATISTICS ' + QUOTENAME(@schema) + N'.' + QUOTENAME(@object)
             + CASE
                   WHEN @effSample IS NULL THEN N' WITH FULLSCAN;'
                   ELSE N' WITH SAMPLE ' + CONVERT(NVARCHAR(10), @effSample) + N' PERCENT;'
               END;

    BEGIN TRY
        EXEC sp_executesql @sql;
    END TRY
    BEGIN CATCH
        UPDATE dbo.StatsUpdateLog
        SET ErrorMessage = LEFT(ERROR_MESSAGE(), 4000)
        WHERE Id = @logId;
    END CATCH

    /* 失敗的列同樣會寫入 EndTime，故 DurationMs 代表「失敗前耗時」。
       查詢成功記錄時請加上 ErrorMessage IS NULL。 */
    UPDATE dbo.StatsUpdateLog
    SET EndTime = SYSDATETIME()
    WHERE Id = @logId;

    /* ========================================================================
       本張物件統計更新完後，處理它的索引
       判斷規則（以 LIMITED 模式取得的平均碎片率為準）：
           碎片率 <  @ReorgThreshold                     → 不處理
           @ReorgThreshold ~ @RebuildThreshold           → REORGANIZE
           碎片率 >= @RebuildThreshold                   → REBUILD
       另外排除：heap、頁數過小、停用中、hypothetical、
                 非 rowstore（columnstore / XML / 空間索引）
       分割資料表以「整個索引」為單位處理：頁數取總和、碎片率取各分割區最大值
       ======================================================================== */
    IF @IndexMaintenance = 1
        BEGIN
            DECLARE ci CURSOR LOCAL FAST_FORWARD FOR SELECT
                i.name
               ,i.type_desc
               ,SUM(ps.page_count)
               ,CAST(MAX(ps.avg_fragmentation_in_percent) AS DECIMAL(5, 2))
            FROM sys.dm_db_index_physical_stats(DB_ID(), @objId, NULL, NULL, 'LIMITED') ps
            JOIN sys.indexes i
                ON i.object_id = ps.object_id
                AND i.index_id = ps.index_id
            WHERE ps.index_id > 0                        -- 排除 heap
            AND ps.index_level = 0
            AND ps.alloc_unit_type_desc = 'IN_ROW_DATA'
            AND i.type IN (1, 2)                         -- 僅 rowstore 叢集／非叢集索引
            AND i.is_disabled = 0
            AND i.is_hypothetical = 0
            AND i.name IS NOT NULL
            GROUP BY i.name, i.type_desc
            HAVING SUM(ps.page_count) >= @MinPageCount
            AND MAX(ps.avg_fragmentation_in_percent) >= @ReorgThreshold
            ORDER BY i.name;

            OPEN ci;
            FETCH NEXT FROM ci INTO @idxName, @idxType, @idxPages, @idxFrag;

            WHILE @@FETCH_STATUS = 0
            BEGIN
                SET @op = CASE
                    WHEN @idxFrag >= @RebuildThreshold THEN 'REBUILD'
                    ELSE 'REORGANIZE'
                END;

                INSERT dbo.IndexMaintenanceLog (RunId
                                               ,SchemaName
                                               ,ObjectName
                                               ,IndexName
                                               ,IndexType
                                               ,PageCount
                                               ,Fragmentation
                                               ,Operation
                                               ,StartTime)
                    VALUES (@RunId
                           ,@schema
                           ,@object
                           ,@idxName
                           ,@idxType
                           ,@idxPages
                           ,@idxFrag
                           ,@op
                           ,SYSDATETIME());

                SET @idxLogId = SCOPE_IDENTITY();

                IF @Verbose = 1
                    BEGIN
                        SET @msg = CONVERT(VARCHAR(8), SYSDATETIME(), 108) + '    -> '
                                 + QUOTENAME(@idxName) + '  ' + @op
                                 + '  (frag ' + CONVERT(VARCHAR(10), @idxFrag)
                                 + '%, pages ' + CONVERT(VARCHAR(20), @idxPages) + ')';
                        RAISERROR (N'%s', 0, 1, @msg) WITH NOWAIT;
                    END

                SET @sqlIdx = N'ALTER INDEX ' + QUOTENAME(@idxName)
                            + N' ON ' + QUOTENAME(@schema) + N'.' + QUOTENAME(@object)
                            + CASE
                                  WHEN @op = 'REBUILD' THEN N' REBUILD'
                                                          + CASE
                                                                WHEN @OnlineRebuild = 1 THEN N' WITH (ONLINE = ON)'
                                                                ELSE N''
                                                            END
                                  ELSE N' REORGANIZE'
                              END + N';';

                BEGIN TRY
                    EXEC sp_executesql @sqlIdx;
                END TRY
                BEGIN CATCH
                    UPDATE dbo.IndexMaintenanceLog
                    SET ErrorMessage = LEFT(ERROR_MESSAGE(), 4000)
                    WHERE Id = @idxLogId;
                END CATCH

                UPDATE dbo.IndexMaintenanceLog
                SET EndTime = SYSDATETIME()
                WHERE Id = @idxLogId;

                FETCH NEXT FROM ci INTO @idxName, @idxType, @idxPages, @idxFrag;
            END

            CLOSE ci;
            DEALLOCATE ci;
        END

    FETCH NEXT FROM c INTO @schema, @object, @type, @objId;
END

CLOSE c;
DEALLOCATE c;