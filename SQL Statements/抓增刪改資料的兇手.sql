SELECT
    s.[host_name]
   ,s.program_name
   ,c.client_net_address
   ,c.client_tcp_port
   ,c.local_net_address
   ,c.local_tcp_port
   ,d.[Name] AS local_database_name
   ,s.login_name
   ,(SELECT
            ib.event_info
        FROM sys.dm_exec_input_buffer(r.session_id, r.request_id) ib)
    AS sql_text
FROM sys.dm_exec_sessions s
INNER JOIN sys.dm_exec_connections c
    ON s.session_id = c.session_id
INNER JOIN sys.dm_exec_requests r
    ON s.session_id = r.session_id
INNER JOIN sys.databases d
    ON r.database_id = d.database_id
WHERE s.session_id = @@spid;