USE [master]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE FUNCTION fn_string_split
(
    @string NVARCHAR(MAX)
   ,@separator NVARCHAR(MAX)
)
RETURNS @result TABLE (
    [value] NVARCHAR(MAX) NULL
)
BEGIN
    DECLARE @xml XML;

    SET @xml = CAST('<d>' + REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(@string, '&', '&amp;'), '<', '&lt;'), '>', '&gt;'), '"', '&quot;'), '''', '&apos;'), @separator, '</d><d>') + '</d>' AS XML);

    INSERT INTO @result([value])
        SELECT
            REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(LTRIM(RTRIM(document.element.value('.', 'nvarchar(max)'))), '&amp;', '&'), '&lt;', '<'), '&gt;', '>'), '&quot;', '"'), '&apos;', '''')
        FROM @xml.nodes('/d') document (element)

    RETURN
END
GO

-- Mark as system object
EXEC sp_MS_marksystemobject 'fn_string_split'
GO