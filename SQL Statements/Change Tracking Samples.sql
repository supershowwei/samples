-- Enable Change Tracking
ALTER DATABASE mydatabase SET CHANGE_TRACKING = ON(CHANGE_RETENTION = 2 DAYS, AUTO_CLEANUP = ON);
ALTER TABLE dbo.MyDependencyTable ENABLE CHANGE_TRACKING WITH(TRACK_COLUMNS_UPDATED = ON);

-- Disable Change Tracking
ALTER TABLE dbo.MyDependencyTable DISABLE CHANGE_TRACKING;
ALTER DATABASE mydatabase SET CHANGE_TRACKING = OFF;

-- Script for find CT enabled database.
SELECT *
FROM sys.change_tracking_databases;

-- Script for find CT enabled table.
SELECT *
FROM sys.change_tracking_tables;

--
SELECT *
FROM CHANGETABLE(CHANGES MyDependencyTable, 1) AS CT
ORDER BY SYS_CHANGE_VERSION;

--
SELECT *
FROM CHANGETABLE(CHANGES MyDependencyTable, 1) AS ct
     INNER JOIN MyDependencyTable pn ON pn.Id = CT.Id
WHERE SYS_CHANGE_VERSION > 1
      AND CT.Sys_Change_Operation <> 'D';

-- Obtain the current synchronization version. This will be used the next time CHANGETABLE(CHANGES...) is called.
SET @synchronization_version = CHANGE_TRACKING_CURRENT_VERSION();

-- Obtain incremental changes by using the synchronization version obtained the last time the data was synchronized.
SELECT
    MDT.*,
    CT.SYS_CHANGE_OPERATION, CT.SYS_CHANGE_COLUMNS,
    CT.SYS_CHANGE_CONTEXT
FROM
    dbo.MyDependencyTable AS MDT
RIGHT OUTER JOIN
    CHANGETABLE(CHANGES dbo.MyDependencyTable, @last_synchronization_version) AS CT
ON
    MDT.Id = CT.Id
