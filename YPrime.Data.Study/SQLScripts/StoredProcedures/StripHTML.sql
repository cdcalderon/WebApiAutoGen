/****** Object:  UserDefinedFunction [dbo].[StripHTML]    Script Date: 7/24/2019 8:40:14 AM ******/
if exists(select * from information_schema.routines where specific_name = 'StripHTML')
	DROP FUNCTION [dbo].[StripHTML]
GO

/****** Object:  UserDefinedFunction [dbo].[StripHTML]    Script Date: 7/24/2019 8:40:14 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE FUNCTION [dbo].[StripHTML] (@HTMLText VARCHAR(MAX))
RETURNS VARCHAR(MAX) AS
BEGIN
    DECLARE @Start INT
    DECLARE @End INT
    DECLARE @Length INT

	SET @HTMLText = replace(@HTMLText,'&nbsp;',' ')

    SET @Start = CHARINDEX('<',@HTMLText)
    SET @End = CHARINDEX('>',@HTMLText,CHARINDEX('<',@HTMLText))
    SET @Length = (@End - @Start) + 1
    WHILE @Start > 0 AND @End > 0 AND @Length > 0
    BEGIN
        SET @HTMLText = STUFF(@HTMLText,@Start,@Length,'')
        SET @Start = CHARINDEX('<',@HTMLText)
        SET @End = CHARINDEX('>',@HTMLText,CHARINDEX('<',@HTMLText))
        SET @Length = (@End - @Start) + 1
    END
	SET @HTMLText = replace(@HTMLText, '&lt;', '<')
	SET @HTMLText = replace(@HTMLText, '&gt;', '>')
	SET @HTMLText = replace(@HTMLText, '&amp;', '&')
    RETURN LTRIM(RTRIM(@HTMLText))
END
GO


