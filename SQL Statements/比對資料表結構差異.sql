create or alter proc dbo.uspCompareTableSchema
(
@sourceDb nvarchar(100)
,@sourceSchema nvarchar(100)
,@sourceTable nvarchar(100)
,@targetDb nvarchar(100)
,@targetSchema nvarchar(100)
,@targetTable nvarchar(100)
)
as
begin
set nocount on;

declare @sourceSql nvarchar(max)='SELECT * FROM '+@sourceDb+'.'+@sourceSchema+'.'+@sourceTable;
declare @targetSql nvarchar(max)='SELECT * FROM '+@targetDb+'.'+@targetSchema+'.'+@targetTable;


SELECT isnull([source].source_database,@sourceDb) as source_database,
isnull([target].source_database,@targetDb) as target_database,
isnull([source].source_schema,@sourceSchema) as source_schema, 
isnull([target].source_schema,@targetSchema) as target_schema, 
isnull([source].source_table,@sourceTable) as source_table, 
isnull([target].source_table,@targetTable) as target_table, 
[source].name as source_ColumnName, 
[target].name as target_ColumnName, 
case 
 when isnull([source].name,'') != isnull([target].name,'') then 'error'
 else ''
end as ColumnName_Result,
[source].collation_name as source_Collation,
[target].collation_name as target_Collation,
case 
 when isnull([source].collation_name,'') != isnull([target].collation_name,'') then 'error'
 else ''
end as Collation_Result,
[source].is_nullable as source_is_nullable, 
[target].is_nullable as target_is_nullable,
case 
 when isnull([source].is_nullable,-1) != isnull([target].is_nullable,-1) then 'error'
 else ''
end as Nullable_Result,
[source].system_type_name as source_Datatype, 
[target].system_type_name as target_Datatype,
case 
 when isnull([source].system_type_name,'') != isnull([target].system_type_name,'') then 'error'
 else ''
end as Datatype_Result,
[source].is_identity_column as source_is_identity, 
[target].is_identity_column as target_is_identity,
case 
 when isnull([source].is_identity_column,-1) != isnull([target].is_identity_column,-1) then 'error'
 else ''
end as Identity_Result
FROM sys.dm_exec_describe_first_result_set (@sourceSql, NULL, 0) as [source] 
FULL OUTER JOIN  sys.dm_exec_describe_first_result_set (@targetSql, NULL, 0) as [target] 
ON [source].name = [target].name 
where [source].name is null 
or [target].name  is null
or [source].system_type_name != [target].system_type_name
or [source].is_identity_column !=[target].is_identity_column

end
GO