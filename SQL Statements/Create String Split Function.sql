-- ================================================
-- Template generated from Template Explorer using:
-- Create Inline Function (New Menu).SQL
--
-- Use the Specify Values for Template Parameters 
-- command (Ctrl-Shift-M) to fill in the parameter 
-- values below.
--
-- This block of comments will not be included in
-- the definition of the function.
-- ================================================
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- Prefix must be 'sp_'
CREATE FUNCTION sp_string_split
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
EXEC sys.sp_MS_marksystemobject 'sp_string_split'
GO