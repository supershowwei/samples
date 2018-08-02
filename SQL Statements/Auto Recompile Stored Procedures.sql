-- 1 - Declaration statements for all variables
DECLARE @TableName NVARCHAR(128)
DECLARE @OwnerName NVARCHAR(128)
DECLARE @CMD1 NVARCHAR(4000)
DECLARE @TableListLoop INT
DECLARE @TableListTable TABLE (
    UIDTableList INT IDENTITY (1, 1)
   ,OwnerName NVARCHAR(128)
   ,TableName NVARCHAR(128)
)

-- 2 - Outer loop for populating the database names
INSERT INTO @TableListTable(OwnerName
                           ,TableName)
    SELECT
        u.[name]
       ,o.[name]
    FROM sys.objects o
    INNER JOIN sys.schemas u
        ON o.schema_id = u.schema_id
    WHERE o.type = 'U'
    ORDER BY o.[name]

-- 3 - Determine the highest UIDDatabaseList to loop through the records
SELECT
    @TableListLoop = MAX(UIDTableList)
FROM @TableListTable

-- 4 - While condition for looping through the database records
WHILE @TableListLoop > 0
BEGIN

-- 5 - Set the @DatabaseName parameter
SELECT
    @TableName = TableName
   ,@OwnerName = OwnerName
FROM @TableListTable
WHERE UIDTableList = @TableListLoop

-- 6 - String together the final backup command
SELECT
    @CMD1 = 'EXEC sp_recompile ' + '[' + @OwnerName + '.' + @TableName + ']' + CHAR(13)

-- 7 - Execute the final string to complete the backups
PRINT @CMD1
--EXEC (@CMD1)


-- 8 - Descend through the database list
SELECT
    @TableListLoop = @TableListLoop - 1
END