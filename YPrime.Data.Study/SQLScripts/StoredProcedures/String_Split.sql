/****** Object:  UserDefinedFunction [dbo].[String_Split]    Script Date: 7/24/2019 8:51:47 AM ******/
if exists(select * from information_schema.routines where specific_name = 'String_Split')
	DROP FUNCTION [dbo].[String_Split]
GO

/****** Object:  UserDefinedFunction [dbo].[String_Split]    Script Date: 7/24/2019 8:51:47 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE FUNCTION [dbo].[String_Split]
(
    @String NVARCHAR(4000),
    @Delimiter NCHAR(1)
)
RETURNS TABLE
AS
RETURN
(
    WITH Split(stpos,endpos)
    AS(
        SELECT 0 AS stpos, CHARINDEX(@Delimiter,@String) AS endpos
        UNION ALL
        SELECT endpos+1, CHARINDEX(@Delimiter,@String,endpos+1)
            FROM Split
            WHERE endpos > 0
    )
    SELECT 'Id' = ROW_NUMBER() OVER (ORDER BY (SELECT 1)),
        'Data' = SUBSTRING(@String,stpos,COALESCE(NULLIF(endpos,0),LEN(@String)+1)-stpos)
    FROM Split
)
GO



