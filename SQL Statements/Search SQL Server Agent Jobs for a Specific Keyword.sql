SELECT
    SERVERPROPERTY('SERVERNAME') AS [instance]
   ,jobs.job_id
   ,jobs.[name]
   ,jobs.[description]
   ,jobsteps.step_id
   ,jobsteps.step_name
   ,jobsteps.command
   ,jobsteps.[database_name]
   ,jobsteps.last_run_date
   ,jobsteps.last_run_time
   ,jobs.originating_server_id
   ,jobs.[enabled]
   ,jobs.start_step_id
   ,jobs.category_id
   ,jobs.owner_sid
   ,jobs.notify_level_eventlog
   ,jobs.notify_level_email
   ,jobs.notify_level_netsend
   ,jobs.notify_level_page
   ,jobs.notify_email_operator_id
   ,jobs.notify_netsend_operator_id
   ,jobs.notify_page_operator_id
   ,jobs.delete_level
   ,jobs.date_created
   ,jobs.date_modified
   ,jobs.version_number
   ,jobsteps.job_id
   ,jobsteps.subsystem
   ,jobsteps.flags
   ,jobsteps.additional_parameters
   ,jobsteps.cmdexec_success_code
   ,jobsteps.on_success_action
   ,jobsteps.on_success_step_id
   ,jobsteps.on_fail_action
   ,jobsteps.on_fail_step_id
   ,jobsteps.[server]
   ,jobsteps.database_user_name
   ,jobsteps.retry_attempts
   ,jobsteps.retry_interval
   ,jobsteps.os_run_priority
   ,jobsteps.output_file_name
   ,jobsteps.last_run_outcome
   ,jobsteps.last_run_duration
   ,jobsteps.last_run_retries
   ,jobsteps.proxy_id
   ,jobsteps.step_uid
FROM msdb.dbo.sysjobs jobs
JOIN msdb.dbo.sysjobsteps jobsteps
    ON jobs.job_id = jobsteps.job_id
WHERE jobsteps.command LIKE '%KEYWORD%'
    OR jobs.job_id IN ('JOB_ID')


--
SELECT
    *
FROM msdb.dbo.sysschedules schecules
LEFT JOIN msdb.dbo.sysjobschedules jobschedules
    ON schecules.schedule_id = jobschedules.schedule_id
WHERE schecules.active_start_time = '30000'


--
SELECT
    *
FROM msdb.dbo.sysjobactivity s WITH (NOLOCK)
WHERE s.start_execution_date >= '2018-01-02 11:10:00'
    AND DATEDIFF(SECOND, s.start_execution_date, s.stop_execution_date) > 60
ORDER BY s.job_id