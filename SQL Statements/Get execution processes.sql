SELECT
    r.scheduler_id AS �Ƶ{���ѧO�X
   ,status AS �n�D�����A
   ,r.session_id AS SPID
   ,r.blocking_session_id AS BlkBy
   ,SUBSTRING(
    LTRIM(q.text),
    r.statement_start_offset / 2 + 1,
    (CASE
        WHEN r.statement_end_offset = -1 THEN LEN(CONVERT(NVARCHAR(MAX), q.text)) * 2
        ELSE r.statement_end_offset
    END - r.statement_start_offset) / 2)
    AS [���b���檺 T-SQL �R�O]
   ,r.cpu_time AS [CPU Time(ms)]
   ,r.start_time AS [�}�l�ɶ�]
   ,r.total_elapsed_time AS [�����`�ɶ�]
   ,r.reads AS [Ū����]
   ,r.writes AS [�g�J��]
   ,r.logical_reads AS [�޿�Ū����]
   ,q.text /* ���㪺 T-SQL ���O�X */
   ,d.name AS [��Ʈw�W��]
FROM sys.dm_exec_requests r
CROSS APPLY sys.dm_exec_sql_text(sql_handle) AS q
LEFT JOIN sys.databases d
    ON (r.database_id = d.database_id)
WHERE r.session_id > 50
AND r.session_id <> @@spid
ORDER BY r.cpu_time DESC