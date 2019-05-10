DECLARE @OptimizationSql NVARCHAR(MAX)
DECLARE OptimizedCursor CURSOR LOCAL STATIC FORWARD_ONLY READ_ONLY FOR SELECT
    'ALTER INDEX [' + ix.[name] + '] ON [' + s.[name] + '].[' + t.[name] + '] ' +
    CASE
        WHEN ps.avg_fragmentation_in_percent > 15 THEN 'REBUILD'
        ELSE 'REORGANIZE'
    END +
    CASE
        WHEN pc.partition_count > 1 THEN ' PARTITION = ' + CAST(ps.partition_number AS NVARCHAR(MAX))
        ELSE ''
    END
FROM sys.indexes AS ix
INNER JOIN sys.tables t
    ON t.object_id = ix.object_id
INNER JOIN sys.schemas s
    ON t.schema_id = s.schema_id
INNER JOIN (SELECT
        object_id
       ,index_id
       ,avg_fragmentation_in_percent
       ,partition_number
    FROM sys.dm_db_index_physical_stats(DB_ID(), NULL, NULL, NULL, NULL)) ps
    ON t.object_id = ps.object_id
    AND ix.index_id = ps.index_id
INNER JOIN (SELECT
        object_id
       ,index_id
       ,COUNT(DISTINCT partition_number) AS partition_count
    FROM sys.partitions
    GROUP BY object_id
            ,index_id) pc
    ON t.object_id = pc.object_id
    AND ix.index_id = pc.index_id
WHERE ps.avg_fragmentation_in_percent > 10
AND ix.[name] IS NOT NULL
--AND t.[name] = 'ClubChatroom'

OPEN OptimizedCursor
FETCH NEXT FROM OptimizedCursor INTO @OptimizationSql
WHILE @@fetch_status = 0
BEGIN

--EXEC sys.sp_executesql @OptimizationSql
PRINT @OptimizationSql

FETCH NEXT FROM OptimizedCursor INTO @OptimizationSql
END

CLOSE OptimizedCursor
DEALLOCATE OptimizedCursor