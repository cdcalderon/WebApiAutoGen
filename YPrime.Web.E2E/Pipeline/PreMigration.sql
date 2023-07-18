USE [YPrime_eCOA-e2e];

DECLARE @name VARCHAR(128)
DECLARE @schema VARCHAR(128)
DECLARE @constraint VARCHAR(254)
DECLARE @SQL VARCHAR(254)

/* Drop all non-system stored procs */
PRINT 'Dropping Stored Procs'
SELECT @name=null, @schema=null, @constraint=null, @sql=null;

SELECT @name = (SELECT TOP 1 [name] FROM sysobjects WHERE [type] = 'P' AND category = 0 ORDER BY [name])

WHILE @name is not null
BEGIN
    SELECT @SQL = 'DROP PROCEDURE [dbo].[' + RTRIM(@name) +']'
    EXEC (@SQL)
    PRINT 'Dropped Procedure: ' + @name
	SELECT @name=null, @schema=null, @constraint=null, @sql=null;
    SELECT @name = (SELECT TOP 1 [name] FROM sysobjects WHERE [type] = 'P' AND category = 0 AND [name] > @name ORDER BY [name])
END

/* Drop all views */
PRINT 'Dropping Views'
SELECT @name=null, @schema=null, @constraint=null, @sql=null;

SELECT TOP 1 @name = sys.objects.name, @schema = sys.schemas.name
	FROM sys.objects 
	INNER JOIN sys.schemas ON sys.objects.schema_id = sys.schemas.schema_id
	WHERE sys.objects.type='V' and sys.schemas.name <> 'sys'
	order by sys.objects.name

WHILE @name IS NOT NULL
BEGIN
    SELECT @SQL = 'DROP VIEW [' + RTRIM(@schema) + '].[' + RTRIM(@name) +']'
    EXEC (@SQL)
    PRINT 'Dropped View: ' + @schema + '.' + @name
	SELECT @name=null, @schema=null, @constraint=null, @sql=null;
    SELECT @name = (SELECT TOP 1 [name] FROM sysobjects WHERE [type] = 'V' AND category = 0 AND [name] > @name ORDER BY [name])
END

/* Drop all functions */
PRINT 'Dropping Functions'
SELECT @name=null, @schema=null, @constraint=null, @sql=null;

SELECT @name = (SELECT TOP 1 [name] FROM sysobjects WHERE [type] IN (N'FN', N'IF', N'TF', N'FS', N'FT') AND category = 0 ORDER BY [name])

WHILE @name IS NOT NULL
BEGIN
    SELECT @SQL = 'DROP FUNCTION [dbo].[' + RTRIM(@name) +']'
    EXEC (@SQL)
    PRINT 'Dropped Function: ' + @name
	SELECT @name=null, @schema=null, @constraint=null, @sql=null;
    SELECT @name = (SELECT TOP 1 [name] FROM sysobjects WHERE [type] IN (N'FN', N'IF', N'TF', N'FS', N'FT') AND category = 0 AND [name] > @name ORDER BY [name])
END

/* Drop all Foreign Key constraints */
PRINT 'Dropping FKs'
SELECT @name=null, @schema=null, @constraint=null, @sql=null;

SELECT TOP 1 @constraint=sys.objects.name, @schema=sys.schemas.name, @name=parent_object.name
	FROM sys.objects 
	INNER JOIN sys.schemas ON sys.objects.schema_id = sys.schemas.schema_id
	INNER JOIN sys.objects parent_object ON sys.objects.parent_object_id=parent_object.object_id and parent_object.type='U'
	WHERE sys.objects.type='F' and sys.schemas.name <> 'sys'
	order by sys.objects.name

WHILE @constraint is not null
BEGIN

    SELECT @SQL = 'ALTER TABLE [' + RTRIM(@schema) + '].[' + RTRIM(@name) +'] DROP CONSTRAINT [' + RTRIM(@constraint) +']'
    EXEC (@SQL)
    PRINT 'Dropped FK Constraint: ' + @constraint + ' on ' + @schema + '.' + @name
	SELECT @name=null, @schema=null, @constraint=null, @sql=null;
    
	SELECT TOP 1 @constraint=sys.objects.name, @schema=sys.schemas.name, @name=parent_object.name
		FROM sys.objects 
		INNER JOIN sys.schemas ON sys.objects.schema_id = sys.schemas.schema_id
		INNER JOIN sys.objects parent_object ON sys.objects.parent_object_id=parent_object.object_id and parent_object.type='U'
		WHERE sys.objects.type='F' and sys.schemas.name <> 'sys'
		order by sys.objects.name
END

/* Drop all Primary Key constraints */
PRINT 'Dropping PKs'
SELECT @name=null, @schema=null, @constraint=null, @sql=null;

SELECT TOP 1 @constraint=sys.objects.name, @schema=sys.schemas.name, @name=parent_object.name
	FROM sys.objects 
	INNER JOIN sys.schemas ON sys.objects.schema_id = sys.schemas.schema_id
	INNER JOIN sys.objects parent_object ON sys.objects.parent_object_id=parent_object.object_id and parent_object.type='U'
	WHERE sys.objects.type='PK' and sys.schemas.name <> 'sys'
	order by sys.objects.name

WHILE @constraint IS NOT NULL
BEGIN
    SELECT @SQL = 'ALTER TABLE [' + RTRIM(@schema) + '].[' + RTRIM(@name) +'] DROP CONSTRAINT [' + RTRIM(@constraint)+']'
    EXEC (@SQL)
    PRINT 'Dropped PK Constraint: ' + @constraint + ' on ' + @schema + '.' + @name
	SELECT @name=null, @schema=null, @constraint=null, @sql=null;

	SELECT TOP 1 @constraint=sys.objects.name, @schema=sys.schemas.name, @name=parent_object.name
		FROM sys.objects 
		INNER JOIN sys.schemas ON sys.objects.schema_id = sys.schemas.schema_id
		INNER JOIN sys.objects parent_object ON sys.objects.parent_object_id=parent_object.object_id and parent_object.type='U'
		WHERE sys.objects.type='PK' and sys.schemas.name <> 'sys'
		order by sys.objects.name
END

/* Drop all tables */
PRINT 'Dropping Tables'
SELECT @name=null, @schema=null, @constraint=null, @sql=null;

SELECT TOP 1 @name = sys.objects.name, @schema = sys.schemas.name
	FROM sys.objects 
	INNER JOIN sys.schemas ON sys.objects.schema_id = sys.schemas.schema_id
	WHERE sys.objects.type='U' and sys.schemas.name <> 'sys'
	order by sys.objects.name

WHILE @name IS NOT NULL
BEGIN
    SELECT @SQL = 'DROP TABLE [' + @schema + '].[' + RTRIM(@name) +']'
    EXEC (@SQL)
    PRINT 'Dropped Table: ' + @schema + '.' + @name
	SELECT @name=null, @schema=null, @constraint=null, @sql=null;
    
	SELECT TOP 1 @name = sys.objects.name, @schema = sys.schemas.name
		FROM sys.objects 
		INNER JOIN sys.schemas ON sys.objects.schema_id = sys.schemas.schema_id
		WHERE sys.objects.type='U' and sys.schemas.name <> 'sys'
		order by sys.objects.name
END