SELECT
    decp.usecounts 'User Counts'
   ,size_in_bytes / 1024.0 'Size(KB)'
   ,decp.cacheobjtype 'Cache Object'
   ,decp.objtype 'Obj Type'
   ,DB_NAME(dest.[dbid]) 'DB'
   ,dest.[text] 'T-SQL'
   ,HASHBYTES('SHA2_256', dest.[text]) AS [T-SQL_Hash]
   ,decp.plan_handle
   ,deqp.query_plan 'Query Plan'
   ,'DBCC FREEPROCCACHE (0x' + CONVERT(VARCHAR(512), decp.plan_handle, 2) + ')' AS [DBCC FREEPROCCACHE Command]
FROM sys.dm_exec_cached_plans decp
CROSS APPLY sys.dm_exec_query_plan(plan_handle) deqp
CROSS APPLY sys.dm_exec_sql_text(plan_handle) dest
--WHERE st.dbid = DB_ID()
--WHERE dest.[text] LIKE ''

