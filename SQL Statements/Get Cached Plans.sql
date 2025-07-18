SELECT
    decp.usecounts AS [UserCounts]
   ,size_in_bytes / 1024.0 AS [SizeKB]
   ,decp.cacheobjtype AS [CacheObject]
   ,decp.objtype AS [ObjectType]
   ,DB_NAME(dest.[dbid]) AS [DBName]
   ,dest.[text] AS [TSQL]
   ,CONVERT(VARCHAR(512), HASHBYTES('SHA2_256', dest.[text]), 2) AS [TSQLHash]
   ,decp.plan_handle AS [PlanHandle]
   ,'DBCC FREEPROCCACHE (0x' + CONVERT(VARCHAR(512), decp.plan_handle, 2) + ') WITH NO_INFOMSGS' AS [DBCCFREEPROCCACHECommand]
   ,deqp.query_plan AS [Plan]
FROM sys.dm_exec_cached_plans decp
CROSS APPLY sys.dm_exec_query_plan(plan_handle) deqp
CROSS APPLY sys.dm_exec_sql_text(plan_handle) dest
--WHERE dest.dbid = DB_ID('WantGoo')
--WHERE dest.dbid = DB_ID('twStocks')
--WHERE dest.[text] LIKE ''


