SELECT
    s.[host_name]
   ,s.program_name
   ,c.client_net_address
   ,c.client_tcp_port
   ,c.local_net_address
   ,c.local_tcp_port
   ,d.[name] AS local_database_name
   ,s.login_name
   ,(SELECT
            q.[text]
        FROM sys.dm_exec_sql_text(r.sql_handle) q)
    AS sql_text
FROM sys.dm_exec_connections c
INNER JOIN sys.dm_exec_sessions s
    ON c.session_id = s.session_id
INNER JOIN sys.dm_exec_requests r
    ON c.session_id = r.session_id
INNER JOIN sys.databases d
    ON r.database_id = d.database_id
WHERE c.session_id = @@spid;