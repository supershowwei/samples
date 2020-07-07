SELECT TOP 20
    *
FROM (SELECT
        (total_worker_time / execution_count) / 1000 AS [AvgCPUTime (ms)]
       ,qs.last_execution_time AS [LastExecutionTime]
       ,st.text AS [SQL Statement]
       ,qp.query_plan AS [QueryPlan]
       ,qs.last_logical_reads AS [LastLogicalReads]
       ,qs.last_logical_writes AS [LastLogicalWrites]
    FROM sys.dm_exec_query_stats AS qs
    CROSS APPLY sys.dm_exec_sql_text(qs.sql_handle) st
    CROSS APPLY sys.dm_exec_query_plan(qs.plan_handle) qp
    WHERE qs.last_execution_time >= '2020-07-07 07:00:00') [stats]
ORDER BY [stats].[AvgCPUTime (ms)] DESC