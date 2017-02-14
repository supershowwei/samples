SELECT
    r.scheduler_id AS 排程器識別碼
   ,status AS 要求的狀態
   ,r.session_id AS SPID
   ,r.blocking_session_id AS BlkBy
   ,SUBSTRING(
    LTRIM(q.text),
    r.statement_start_offset / 2 + 1,
    (CASE
        WHEN r.statement_end_offset = -1 THEN LEN(CONVERT(NVARCHAR(MAX), q.text)) * 2
        ELSE r.statement_end_offset
    END - r.statement_start_offset) / 2)
    AS [正在執行的 T-SQL 命令]
   ,r.cpu_time AS [CPU Time(ms)]
   ,r.start_time AS [開始時間]
   ,r.total_elapsed_time AS [執行總時間]
   ,r.reads AS [讀取數]
   ,r.writes AS [寫入數]
   ,r.logical_reads AS [邏輯讀取數]
   ,q.text /* 完整的 T-SQL 指令碼 */
   ,d.name AS [資料庫名稱]
FROM sys.dm_exec_requests r
CROSS APPLY sys.dm_exec_sql_text(sql_handle) AS q
LEFT JOIN sys.databases d
    ON (r.database_id = d.database_id)
WHERE r.session_id > 50
AND r.session_id <> @@spid
ORDER BY r.total_elapsed_time DESC