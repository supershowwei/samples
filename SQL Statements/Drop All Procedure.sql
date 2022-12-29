
DECLARE @sql NVARCHAR(MAX) = N'';

SELECT
    @sql +=
    N'DROP PROCEDURE ' +
    QUOTENAME(OBJECT_SCHEMA_NAME(p.[object_id])) + N'.' +
    QUOTENAME(p.[name]) + N'; ' + NCHAR(13)
FROM sys.procedures AS p
WHERE OBJECT_SCHEMA_NAME(p.[object_id]) = 'DataSync'
--WHERE p.[name] LIKE '%_dss_%_trigger'

PRINT @sql;