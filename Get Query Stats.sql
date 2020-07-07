SELECT TOP 100
    SUBSTRING(text, qs.statement_start_offset / 2
    , (CASE
        WHEN qs.statement_end_offset = -1 THEN LEN(CONVERT(NVARCHAR(MAX),
            text)) * 2
        ELSE qs.statement_end_offset
    END - qs.statement_start_offset) / 2)
   ,text
   ,qs.plan_generation_num AS recompiles
   ,qs.execution_count AS execution_count
   ,qs.total_elapsed_time - qs.total_worker_time AS total_wait_time
   ,qs.total_worker_time AS cpu_time
   ,qs.total_logical_reads AS reads
   ,qs.total_logical_writes AS writes
   ,last_execution_time
FROM sys.dm_exec_query_stats qs
CROSS APPLY sys.dm_exec_sql_text(qs.sql_handle) st
LEFT JOIN sys.dm_exec_requests r
    ON qs.sql_handle = r.sql_handle
WHERE qs.last_execution_time >= '2019-08-27 08:00:00'
ORDER BY qs.total_worker_time / qs.execution_count DESC
--ORDER BY qs.last_execution_time DESC