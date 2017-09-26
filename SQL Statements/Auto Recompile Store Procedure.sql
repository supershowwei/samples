DECLARE @CompilationSql NVARCHAR(MAX)
DECLARE CompilationCursor CURSOR LOCAL STATIC FORWARD_ONLY READ_ONLY FOR SELECT
    [name]
FROM dbo.sysobjects
WHERE [Type] = 'P'
ORDER BY [name]

OPEN CompilationCursor
FETCH NEXT FROM CompilationCursor INTO @CompilationSql
WHILE @@fetch_status = 0
BEGIN

--EXEC sys.sp_recompile @CompilationSql
PRINT @CompilationSql

FETCH NEXT FROM CompilationCursor INTO @CompilationSql
END

CLOSE CompilationCursor
DEALLOCATE CompilationCursor