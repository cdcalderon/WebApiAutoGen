
if exists(select * from information_schema.routines where specific_name = 'CheckAuditTableColumns')
	DROP PROCEDURE [dbo].[CheckAuditTableColumns]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[CheckAuditTableColumns] (@debug tinyint = 0)
AS
BEGIN
	
	declare @auditSchema nvarchar(max);
	declare @table_name nvarchar(max);
	declare @column_name nvarchar(max);
	declare @data_type nvarchar(max);
	declare @sql nvarchar(max);

	set @auditSchema = 'ypaudit';

	--use this table to check for audit columns
	declare @AuditColumn table
	(
	  COLUMN_NAME nvarchar(max),
	  DATA_TYPE nvarchar(max)
	)
	insert into @AuditColumn (COLUMN_NAME, DATA_TYPE) values ('ModifiedBy','nvarchar');
	insert into @AuditColumn (COLUMN_NAME, DATA_TYPE) values ('ModifiedDate','datetimeoffset');
	insert into @AuditColumn (COLUMN_NAME, DATA_TYPE) values ('AuditCorrectionId','uniqueidentifier');
	insert into @AuditColumn (COLUMN_NAME, DATA_TYPE) values ('AuditAction','nvarchar');

	-- Two new colmns (#59586)
	insert into @AuditColumn (COLUMN_NAME, DATA_TYPE) values ('AuditSource','nvarchar');
	insert into @AuditColumn (COLUMN_NAME, DATA_TYPE) values ('AuditDeviceId','uniqueidentifier');

	declare @col_Name nvarchar(max)
	declare @additionalColumns nvarchar(max)=''

	declare cols cursor for 
	select * from @auditColumn
	open cols
	fetch cols into @col_name,@data_Type
	while @@fetch_status = 0
	begin
		if (@data_type='nvarchar') 
			begin
				set @additionalColumns += '['+@col_name+'] ['+@data_Type+'](255) NULL,'
			end
		else
			begin
				set @additionalColumns += '['+@col_name+'] ['+@data_Type+'] NULL,'
			end
		fetch cols into @col_name,@data_Type
	end
	set @additionalColumns = substring(@AdditionalColumns,1,len(@additionalColumns)-1)


	--******************************************************
	--CHECK FOR SCHEMA
	if not exists (
		select  schema_name
		from    information_schema.schemata
		where   schema_name = @auditSchema ) 

		begin
			set @sql = 'create schema ' + @auditSchema;

			print @sql
			if @Debug = 0 exec sp_executesql @sql;
		end

	--******************************************************
	--CHECK FOR MISSING TABLES
	declare curTables cursor for  
	select 
		mainTables.TABLE_NAME 
	from 
		(select 
				* 
			from
				information_schema.tables
			where 
				table_schema in ('dbo','config')
				) mainTables
	left join
		(select 
				* 
			from
				information_schema.tables
			where 
				table_schema = @auditSchema
				) auditTables
	on auditTables.TABLE_NAME = mainTables.TABLE_NAME
	where auditTables.TABLE_NAME is null
		and mainTables.TABLE_NAME not in('__MigrationHistory');

	print '-------------------------------------------------------------'
	print '-- Check for missng '+@AuditSchema+' tables '
	print '-------------------------------------------------------------'

	open curTables;
	fetch curTables into @table_name;
	while @@fetch_status = 0
	begin
		print '-- Creating table ' + @table_name;
		print '-- '
		set @sql = 'CREATE TABLE [ypaudit].' + quotename(@table_name) + '('+@AdditionalColumns+')';

		print @sql
		if @debug = 0 exec sp_executesql @sql;
		
		fetch curTables into @table_name;
	end

	close curTables;
	deallocate curTables;

	--******************************************************
	--CHECK FOR COLUMNS
	print ' '
	print '-------------------------------------------------------------'
	print '-- Check for missng columns'
	print '-------------------------------------------------------------'


	declare curColumns cursor for
		select 
			mainTableColumns.TABLE_NAME
			, mainTableColumns.COLUMN_NAME
			, mainTableColumns.DATA_TYPE
		from 
			(select 
				* 
			from
				information_schema.columns
			where 
				table_schema in ('dbo','config')
				) mainTableColumns
		left join
			(select 
				* 
			from
				information_schema.columns
			where 
				table_schema = @auditSchema
			) auditTableColumns
		on 
			mainTableColumns.TABLE_NAME = auditTableColumns.TABLE_NAME
			and mainTableColumns.COLUMN_NAME = auditTableColumns.COLUMN_NAME
		left join
			(select
				*
			from 
				information_schema.tables
			where 
				table_schema = @auditSchema
			) auditTables
		on auditTables.TABLE_NAME = mainTableColumns.TABLE_NAME
		where 
			auditTableColumns.COLUMN_NAME is null
			and auditTables.TABLE_NAME is not null
		union --union in any missing audit columns
		select 
			t.TABLE_NAME
			, ac.COLUMN_NAME
			, ac.DATA_TYPE
		from 
			information_schema.tables t
		left join @AuditColumn ac 
			on 1 = 1
		left join information_schema.columns c
			on	c.TABLE_SCHEMA = t.TABLE_SCHEMA 
				and c.TABLE_NAME = t.TABLE_NAME 
				and c.COLUMN_NAME = ac.COLUMN_NAME
		where 
			t.TABLE_SCHEMA = @auditSchema
			and c.COLUMN_NAME is null	
		;
	
	open curColumns;
	fetch curColumns into @table_Name, @column_Name, @data_type;
	while @@fetch_status = 0
	begin
		set @sql = N'alter table ' + quotename(@auditSchema) + '.' + quotename(@table_name) + ' add ' + quotename(@column_name) 
			+ ' ' +
			case when @data_type = 'nvarchar' or @data_type = 'varchar'
				then @data_type + '(max)'
				else @data_type
				end
			+ ';'
			;
		--add the column
		print @sql;
		if @debug = 0 exec sp_executesql @sql;
	
		fetch curColumns into @table_name, @column_Name, @data_type;
	end

	close curColumns;
	deallocate curColumns;

	--******************************************************
	--make audit columns nullable
	print ' '
	print '-------------------------------------------------------------'
	print '-- Make audit columns nullable '
	print '-------------------------------------------------------------'

	declare curAuditColumns cursor for
		select 
			auditTableColumns.TABLE_NAME
			, auditTableColumns.COLUMN_NAME
			, auditTableColumns.DATA_TYPE
		from 
		(select 
				* 
			from
				information_schema.columns
			where 
				table_schema = @auditSchema
			) auditTableColumns
			
		left join
			(select 
				* 
			from
				information_schema.columns
			where 
				table_schema in ('dbo','config')
				) mainTableColumns
		on 
			auditTableColumns.TABLE_NAME = mainTableColumns.TABLE_NAME 
			and auditTableColumns.COLUMN_NAME = mainTableColumns.COLUMN_NAME
		left join
			(select
				*
			from 
				information_schema.tables
			where 
				table_schema in ('dbo','config')
			) mainTables
		on mainTables.TABLE_NAME = auditTableColumns.TABLE_NAME
		where 
			mainTableColumns.COLUMN_NAME is null
			and auditTableColumns.IS_NULLABLE = 'NO'
			and mainTables.TABLE_NAME is not null;

	open curAuditColumns;
		fetch curAuditColumns into @table_Name, @column_Name, @data_type;
	while @@fetch_status = 0
	begin
		set @sql = N'alter table ' + quotename(@auditSchema) + '.' + quotename(@table_name) + ' alter column ' + quotename(@column_name)
			+ ' ' +
			case when @data_type = 'nvarchar' or @data_type = 'varchar'
				then @data_type + '(max)'
				else @data_type
				end
			+ ' null;'
			;
		--make the column nullable
		print @sql;
		if @debug=0 exec sp_executesql @sql;
	
		fetch curAuditColumns into @table_name, @column_Name, @data_type;
	end

	close curAuditColumns;
	deallocate curAuditColumns;

END
GO
