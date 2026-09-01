DECLARE @StartDate datetime = '2026-09-01 00:00:00';
DECLARE @EndDate   datetime = '2026-09-02 00:00:00';   -- 不含端點，建議用「小於」的寫法

SELECT
    j.name                AS job_name,
    h.step_id,
    h.step_name,
    msdb.dbo.agent_datetime(h.run_date, h.run_time) AS start_time,
    CASE h.run_status
        WHEN 0 THEN N'失敗'
        WHEN 1 THEN N'成功'
        WHEN 2 THEN N'重試'
        WHEN 3 THEN N'取消'
        WHEN 4 THEN N'進行中'
    END                   AS run_status,
    -- run_duration 是 HHMMSS 整數，轉成秒數方便排序／加總
    (h.run_duration / 10000) * 3600
      + (h.run_duration / 100 % 100) * 60
      + (h.run_duration % 100)                      AS duration_sec,
    STUFF(STUFF(RIGHT('000000' + CAST(h.run_duration AS varchar(6)), 6), 5, 0, ':'), 3, 0, ':')
                                                    AS duration_hhmmss,
    h.sql_message_id,
    h.sql_severity,
    h.retries_attempted,
    h.message             AS output_message,
    s.subsystem,
    s.database_name,
    s.command             AS step_command
FROM msdb.dbo.sysjobhistory h
JOIN msdb.dbo.sysjobs j
     ON j.job_id = h.job_id
LEFT JOIN msdb.dbo.sysjobsteps s
     ON s.job_id = h.job_id
    AND s.step_id = h.step_id
WHERE
    -- 先用 int 粗篩，能吃到 run_date 的索引
    h.run_date BETWEEN CONVERT(char(8), @StartDate, 112)
                   AND CONVERT(char(8), @EndDate,   112)
    -- 再用實際時間精算邊界
    AND msdb.dbo.agent_datetime(h.run_date, h.run_time) >= @StartDate
    AND msdb.dbo.agent_datetime(h.run_date, h.run_time) <  @EndDate
ORDER BY start_time DESC, h.step_id;