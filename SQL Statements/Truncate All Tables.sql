
DECLARE @sql NVARCHAR(MAX) = N'';

SELECT
    @sql +=
    N'TRUNCATE TABLE ' +
    QUOTENAME(OBJECT_SCHEMA_NAME(t.[object_id])) + N'.' +
    QUOTENAME(t.[name]) + N'; ' + NCHAR(13)
FROM sys.tables AS t
WHERE OBJECT_SCHEMA_NAME(t.[object_id]) = 'DataSync'
--WHERE t.[name] NOT LIKE '%_dss%'

PRINT @sql;