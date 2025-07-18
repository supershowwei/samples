-- 一支執行計劃只有一個 plan_handle，但執行次數很多；max 與 min 差距 > 10 倍，就值得懷疑：
DECLARE @DBName VARCHAR(MAX) = 'WantGoo'

;
WITH stats
AS
(SELECT
        qs.plan_handle
       ,qs.query_hash
       ,qs.statement_start_offset
       ,qs.statement_end_offset
       ,qs.execution_count
       ,qs.min_logical_reads
       ,qs.max_logical_reads
       ,qs.min_elapsed_time
       ,qs.max_elapsed_time
       ,qs.total_logical_reads / qs.execution_count AS avg_reads
       ,qs.total_elapsed_time / qs.execution_count AS avg_ms
       ,qs.last_execution_time
    FROM sys.dm_exec_query_stats qs)
SELECT TOP (50)
    DB_NAME(st.dbid) AS db_name
   ,OBJECT_NAME(st.objectid, st.dbid) AS obj_name
   ,SUBSTRING(st.text,
    (stats.statement_start_offset / 2) + 1,
    ((CASE stats.statement_end_offset
        WHEN -1 THEN DATALENGTH(st.text)
        ELSE stats.statement_end_offset
    END)
    - stats.statement_start_offset) / 2 + 1) AS stmt
   ,stats.*
   ,qp.query_plan
FROM stats
CROSS APPLY sys.dm_exec_sql_text(stats.plan_handle) AS st
CROSS APPLY sys.dm_exec_query_plan(stats.plan_handle) AS qp
WHERE stats.execution_count >= 5                               -- 篩掉偶發
    AND stats.min_logical_reads > 0
    AND stats.max_logical_reads > stats.min_logical_reads * 10   -- 10 倍落差
    AND DB_NAME(st.dbid) = @DBName
ORDER BY stats.max_logical_reads DESC;
