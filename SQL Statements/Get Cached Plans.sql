SELECT TOP 100
    cp.usecounts 'User Counts'
   ,size_in_bytes / 1024.0 'Size(KB)'
   ,cp.cacheobjtype 'Cache Object'
   ,cp.objtype 'Obj Type'
   ,DB_NAME(st.dbid) 'DB'
   ,st.text 'T-SQL'
   ,cp.plan_handle
   ,qp.query_plan 'Query Plan'
FROM sys.dm_exec_cached_plans cp
CROSS APPLY sys.dm_exec_sql_text(plan_handle) st
CROSS APPLY sys.dm_exec_query_plan(plan_handle) qp
WHERE st.dbid = DB_ID()
ORDER BY 1 DESC;