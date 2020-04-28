/* JsonAutoService */
/* Example 1: Sql Server Compatibility Level 140 */
/* DDL */
/* WHAT THIS CODE DOES: */
/*          1. Drop Resources */
/*              1) Drop all tables */
/*              2) Drop error meta data function */
/*              3) Drop all stored procedures */
/*              4) Drop 'jas' schema */
/*          2. Create JsonAutoService Example Resources */
/*              1) Create 'jas' schema */
/*              2) Create 3 tables */
/*              3) Create error meta data function */
/*              4) Create 11 stored procedures */

/* drop tables */
drop table if exists jas.product_options;
drop table if exists jas.options;
drop table if exists jas.products;
go

/* drop function(s) */
drop function if exists jas.error_metadata;
go

/* drop procedures */
drop proc if exists jas.api_product_post;
drop proc if exists jas.api_options_post;
drop proc if exists jas.api_product_options_post;
drop proc if exists jas.api_products_get;
drop proc if exists jas.api_options_get;
drop proc if exists jas.api_product_option_get;
drop proc if exists jas.api_pos_by_product_get;
drop proc if exists jas.api_pos_by_option_get;
drop proc if exists jas.api_product_option_put;
drop proc if exists jas.api_product_option_delete;
drop proc if exists dbo.api_product_exists_head;
go

/* sample schema */
drop schema if exists jas;
go
create schema jas;
go

/* sample tables */
drop table if exists jas.products;
go
create table jas.products(
  p_id                   bigint identity(1,1) constraint pk_products unique not null,
  product_name           nvarchar(20) unique not null,
  created_dt             datetime2 not null);
go

drop table if exists jas.options;
go
create table jas.options(
  o_id                bigint identity(1,1) constraint pk_ingredients unique not null,
  option_name         nvarchar(20) unique not null,
  created_dt          datetime2 not null);
go

drop table if exists jas.product_options;
go
create table jas.product_options(
  po_id                 bigint identity(1,1) constraint pk_products_options unique not null,
  p_id                  bigint constraint fk_products references jas.products(p_id) not null,
  o_id                  bigint constraint fk_options references jas.options(o_id) not null,
  product_option        nvarchar(20) null,
  created_dt            datetime2 not null,
  constraint
    unq_product_options_p_o unique (p_id, o_id));
go

/* framework functions */
drop function if exists jas.error_metadata;
go
create function jas.error_metadata()
returns table with schemabinding
as
return
	(select
	  error_number() error_number,
	  error_severity() error_severity,
	  error_state() error_state,
	  error_line () error_line,
	  error_procedure() error_procedure,
	  error_message() error_message,
	  xact_state() xact_state);
go

/* CRUD+H procedures to service jas.product_options table */
/* CRUD+H [Create, Read, Update, Delete, Head] stored procedures with framework */
/* targets RESTful .NET Core Web Api controllers */
/* proc 1 Create new records */
drop proc if exists jas.api_product_post;
go
create proc jas.api_product_post
  @headers					nvarchar(max)=null,
  @params					nvarchar(max)=null,
  @body						nvarchar(max)=null,
  @test_id					bigint output,
  @response					nvarchar(max) output
as
set nocount on;
set xact_abort on;

begin transaction
begin try
    declare
      @check_isjson         int=nullif(json_value(@headers, N'$.check_isjson'), 0);

    if @check_isjson=1
        begin
            if isjson(@headers)=0
                throw 50000, 'The headers json is invalid', 1;

            if isjson(@params)=0
                throw 50000, 'The parameters json is invalid', 1;

            if isjson(@body)=0
                throw 50000, 'The request body json is invalid', 1;
        end

    declare
      @p_id                 bigint,
      @p_count              bigint;

    insert jas.products(product_name, created_dt) values(json_value(@body, N'strict $.product_name'), sysutcdatetime())
	select @p_count=rowcount_big();
    select @p_id=cast(scope_identity() as bigint);

    if @p_count<>1
        throw 50000, 'No rows inserted', 1;

    select 
      @test_id=@p_id,
      @response=(select N'Ok' reply_message, @p_id p_id for json path, without_array_wrapper);		
	commit transaction;
end try
begin catch
	select 
      @test_id=cast(0 as bigint),
      @response=(select * from jas.error_metadata() for json path, without_array_wrapper);

    rollback transaction;  
end catch

set xact_abort off;
set nocount off;
go

drop proc if exists jas.api_options_post;
go
create proc jas.api_options_post
  @headers					nvarchar(max)=null,
  @params					nvarchar(max)=null,
  @body						nvarchar(max)=null,
  @test_id					bigint output,
  @response					nvarchar(max) output
as
set nocount on;
set xact_abort on;

begin transaction
begin try
    declare
      @check_isjson         int=nullif(json_value(@headers, N'$.check_isjson'), 0);

    if @check_isjson=1
        begin
            if isjson(@headers)=0
                throw 50000, 'The headers json is invalid', 1;

            if isjson(@params)=0
                throw 50000, 'The parameters json is invalid', 1;

            if isjson(@body)=0
                throw 50000, 'The request body json is invalid', 1;
        end

    declare
      @o                   table(o_id               bigint unique not null,
                                 product_name       nvarchar(20));
    declare
      @o_count             bigint;

    insert jas.options(option_name, created_dt) 
    output inserted.o_id, inserted.option_name into @o
    select
      o.option_name,
      sysutcdatetime()
    from
      openjson(@body, N'strict $.options')
      with
        (option_name       nvarchar(20)) o;
	select @o_count=rowcount_big();

    if @o_count<1
        throw 50000, 'No rows inserted', 1;

    select 
      @test_id=cast(1 as bigint),
      @response=(select 
                   N'Ok' reply_message, 
                   (select * from @o for json path) options
                 for json path, without_array_wrapper);		
	commit transaction;
end try
begin catch
	select 
      @test_id=cast(0 as bigint),
      @response=(select * from jas.error_metadata() for json path, without_array_wrapper);

    rollback transaction;  
end catch

set xact_abort off;
set nocount off;
go

drop proc if exists jas.api_product_options_post;
go
create proc jas.api_product_options_post
  @headers					nvarchar(max)=null,
  @params					nvarchar(max)=null,
  @body						nvarchar(max)=null,
  @test_id					bigint output,
  @response					nvarchar(max) output
as
set nocount on;
set xact_abort on;

begin transaction
begin try
    declare
      @check_isjson         int=nullif(json_value(@headers, N'$.check_isjson'), 0);

    if @check_isjson=1
        begin
            if isjson(@headers)=0
                throw 50000, 'The headers json is invalid', 1;

            if isjson(@params)=0
                throw 50000, 'The parameters json is invalid', 1;

            if isjson(@body)=0
                throw 50000, 'The request body json is invalid', 1;
        end

    declare
      @po                   table(po_id             bigint unique not null,
                                  p_id              bigint,
                                  o_id              int,
                                  product_option    nvarchar(20),
                                  created_dt        datetime2);
    declare
      @p_id                 bigint=json_value(@body, N'strict $.p_id'),
      @po_count             bigint;

    insert jas.product_options(p_id, o_id, product_option, created_dt)
    output inserted.* into @po
    select
      @p_id,
      po.*,
      sysutcdatetime()
    from
      openjson(@body, N'strict $.product_options')
      with
        (o_id   			bigint N'strict $.o_id',
		 product_option		nvarchar(20) N'$.product_option') po
	select @po_count=rowcount_big();

    if @po_count<=0
        throw 50000, 'No rows inserted', 1;

    select 
      @test_id=cast(1 as bigint),
      @response=(select 
                   N'Ok' reply_message,
                   (select * from @po for json path) product_options
                 for json path, without_array_wrapper);		
	commit transaction;
end try
begin catch
	select 
      @test_id=cast(0 as bigint),
      @response=(select * from jas.error_metadata() for json path, without_array_wrapper);

	--if xact_state()=-1
    rollback transaction;  
end catch

set xact_abort off;
set nocount off;
go


/* proc 2 Read record(s) */
drop proc if exists jas.api_products_get;
go
create proc jas.api_products_get
  @headers					nvarchar(max)=null,
  @params					nvarchar(max)=null
as
set nocount on;

select
  *
from
  jas.products
for json path;

set nocount off;
go

drop proc if exists jas.api_options_get;
go
create proc jas.api_options_get
  @headers					nvarchar(max)=null,
  @params					nvarchar(max)=null
as
set nocount on;

select
  *
from
  jas.options
for json path;

set nocount off;
go

drop proc if exists jas.api_product_option_get;
go
create proc jas.api_product_option_get
  @headers					nvarchar(max)=null,
  @params					nvarchar(max)=null
as
set nocount on;

select
  *
from
  jas.product_options
where
  po_id=json_value(@params, N'strict $.id')
for json path;

set nocount off;
go

drop proc if exists jas.api_pos_by_option_get;
go
create proc jas.api_pos_by_option_get
  @headers					nvarchar(max)=null,
  @params					nvarchar(max)=null
as
set nocount on;

select
  *
from
  jas.product_options
where
  o_id=json_value(@params, N'strict $.id')
for json path;

set nocount off;
go

drop proc if exists jas.api_pos_by_product_get;
go
create proc jas.api_pos_by_product_get
  @headers					nvarchar(max)=null,
  @params					nvarchar(max)=null
as
set nocount on;

select
  *
from
  jas.product_options
where
  p_id=json_value(@params, N'strict $.id')
for json path;

set nocount off;
go


drop proc if exists jas.api_product_option_put;
go
create proc jas.api_product_option_put
  @headers					nvarchar(max)=null,
  @params					nvarchar(max)=null,
  @body						nvarchar(max)=null,
  @test_value				bit output,
  @response					nvarchar(max) output
as
set nocount on;
set xact_abort on;

begin transaction
begin try
    declare
      @check_isjson         int=nullif(json_value(@headers, N'$.check_isjson'), 0);
    if @check_isjson=1
        begin
            if isjson(@headers)=0
                throw 50000, 'The headers json is invalid', 1;

            if isjson(@params)=0
                throw 50000, 'The parameters json is invalid', 1;

            if isjson(@body)=0
                throw 50000, 'The request body json is invalid', 1;
        end

	/* interpret json request */
    declare
      @o_id                     bigint=json_value(@body, N'strict $.o_id'),
      @po_id                    bigint=json_value(@body, N'strict $.po_id'),
      @po_count                 int;

	update jas.product_options set o_id=@o_id where po_id=@po_id;
	select @po_count=@@rowcount;

    if @po_count<>1
		throw 51000, N'No records upated', 1;

	select @test_value=cast(1 as bit), @response=(select N'Ok' reply_message for json path, without_array_wrapper);		
	commit transaction;
end try
begin catch
	select @test_value=cast(0 as bit), @response=(select * from jas.error_metadata() for json path, without_array_wrapper);

	if xact_state()=-1  /* this is pedantic and unnecessary, by definition xact_state=-1 when in catch block */
		rollback transaction;  
end catch

set xact_abort off;
set nocount off;
go

drop proc if exists jas.api_product_option_delete;
go
create proc jas.api_product_option_delete
  @headers					nvarchar(max)=null,
  @params					nvarchar(max)=null,
  @body						nvarchar(max)=null,
  @test_value				bit output,
  @response					nvarchar(max) output
as
set nocount on;
set xact_abort on;

begin transaction
begin try
    declare
      @check_isjson         int=nullif(json_value(@headers, N'$.check_isjson'), 0);
    if @check_isjson=1
        begin
            if isjson(@headers)=0
                throw 50000, 'The headers json is invalid', 1;

            if isjson(@params)=0
                throw 50000, 'The parameters json is invalid', 1;

            if isjson(@body)=0
                throw 50000, 'The request body json is invalid', 1;
        end

	/* interpret json request */
    declare
      @po_count                 int;

    delete jas.product_options where po_id=json_value(@params, N'$.id');
	select @po_count=@@rowcount;

    if @po_count=0
		throw 51000, N'No records deleted', 1;

	select @test_value=cast(1 as bit), @response=(select N'Ok' reply_message for json path, without_array_wrapper);		
	commit transaction;
end try
begin catch
	select @test_value=cast(0 as bit), @response=(select * from jas.error_metadata() for json path, without_array_wrapper);

	--if xact_state()=-1  /* this is pedantic and unnecessary, by definition xact_state=-1 when in catch block */
	rollback transaction;  
end catch

set xact_abort off;
set nocount off;
go

drop proc if exists dbo.api_product_exists_head;
go
create proc dbo.api_product_exists_head
  @headers							nvarchar(max)=null,
  @params							nvarchar(max)=null
as
set nocount on;
declare
  @product_name				nvarchar(20)=json_value(@params, N'strict $.product_name');

select count(*) from jas.products where product_name=@product_name;
set nocount off;
go

