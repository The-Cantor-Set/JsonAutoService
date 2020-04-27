/* Sql with framework */

/* delete all records */
--set nocount on;
delete jas.product_options;
dbcc checkident ('jas.product_options', reseed, 0) with no_infomsgs 
go
delete jas.options;
dbcc checkident ('jas.options', reseed, 0) with no_infomsgs 
go
delete jas.products;
dbcc checkident ('jas.products', reseed, 0) with no_infomsgs 
go

/* CRUD CREATE (INSERT PRODUCTS) -- 3 SINGLE-ROW TRANSACTIONS -- */
/* jas.products sample records */
    /* transaction 1 */
/*
/* ---------- Example nonframework sql ---------- */
begin transaction
    insert jas.products values('Phone 1', sysutcdatetime());
commit;
/* ---------- ---------- ---------- ---------- */
*/
/* ---------- Example FRAMEWORK sql ---------- */
declare
  @out_test_id      bigint,
  @out_response     nvarchar(max);

exec jas.api_product_post 
    @headers = '{ "check_isjson": 1 }',
    @params = null,
    @body='{ "product_name": "Phone 1" }',
    @test_id=@out_test_id output,
    @response=@out_response output;

print (cast(@out_test_id as varchar(12)));
print (@out_response);

    /* transaction 2 */
/*
/* ---------- Example nonframework sql ---------- */
begin transaction
    insert jas.products values('Phone 2', sysutcdatetime());
commit;
/* ---------- ---------- ---------- ---------- */
*/
/* ---------- Example FRAMEWORK sql ---------- */
declare
  @out_test_id      bigint,
  @out_response     nvarchar(max);

exec jas.api_product_post 
    @headers = '{ "check_isjson": 1 }',
    @params = null,
    @body='{ "product_name": "Phone 2" }',
    @test_id=@out_test_id output,
    @response=@out_response output;

print (cast(@out_test_id as varchar(12)));
print (@out_response);

    /* transaction 3 */
/*
/* ---------- Example nonframework sql ---------- */
begin transaction
    insert jas.products values('Phone 3', sysutcdatetime());
commit;
/* ---------- ---------- ---------- ---------- */
*/
/* ---------- Example FRAMEWORK sql ---------- */
declare
  @out_test_id      bigint,
  @out_response      nvarchar(max);

exec jas.api_product_post 
    @headers = '{ "check_isjson": 1 }',
    @params = null,
    @body='{ "product_name": "Phone 3" }',
    @test_id=@out_test_id output,
    @response=@out_response output;

print (cast(@out_test_id as varchar(12)));
print (@out_response);

/* CRUD CREATE (INSERT OPTIONS) -- 1 MULTI-ROW TRANSACTION -- */
/* jas.options sample records */
/*
/* ---------- Example nonframework sql ---------- */
begin transaction
    insert jas.options values
    ('Screen cover', sysutcdatetime()), 
    ('Outer case 1', sysutcdatetime()), 
    ('Outer case 2', sysutcdatetime());
commit;
/* ---------- ---------- ---------- ---------- */
*/
/* ---------- Example FRAMEWORK sql ---------- */
declare
  @out_test_id      bigint,
  @out_response      nvarchar(max);

exec jas.api_options_post 
    @headers = '{ "check_isjson": 0 }',
    @params = null,
    @body='{ "options": [ 
                { "option_name": "Screen cover" },
                { "option_name": "Outer case 1" },
                { "option_name": "Outer case 2" } 
                ]
           }',
    @test_id=@out_test_id output,
    @response=@out_response output;

print (cast(@out_test_id as varchar(12)));
print (@out_response);

/* CRUD CREATE (INSERT PRODUCT_OPTIONS) -- 3 MULTI-ROW TRANSACTIONS -- */
/* jas.product_options same records */
    /* transaction 1 */
/*
/* ---------- Example nonframework sql ---------- */
begin transaction
    insert jas.product_options(p_id, o_id, product_option, created_dt) values 
    (1, 1, null, sysutcdatetime()),
    (1, 2, null, sysutcdatetime());
commit;
/* ---------- ---------- ---------- ---------- */
*/
/* ---------- Example FRAMEWORK sql ---------- */
declare
  @out_test_id      bigint,
  @out_response      nvarchar(max);

exec jas.api_product_options_post 
    @headers = '{ "check_isjson": 1 }',
    @params = null,
    @body='{ "p_id": 1, 
             "product_options": [ 
                    { "o_id": 1 },
                    { "o_id": 2 } ] }',
    @test_id=@out_test_id output,
    @response=@out_response output;

print (cast(@out_test_id as varchar(12)));
print (@out_response);

    /* transaction 2 */
/*
/* ---------- Example nonframework sql ---------- */
begin transaction
    insert jas.product_options(p_id, o_id, product_option, created_dt) values
    (2, 1, null, sysutcdatetime()),
    (2, 3, null, sysutcdatetime());
commit;
/* ---------- ---------- ---------- ---------- */
*/
/* ---------- Example FRAMEWORK sql ---------- */
declare
  @out_test_id      bigint,
  @out_response      nvarchar(max);

exec jas.api_product_options_post 
    @headers = '{ "check_isjson": 1 }',
    @params = null,
    @body='{ "p_id": 2, 
             "product_options": [ 
                    { "o_id": 1 },
                    { "o_id": 3 } ] }',
    @test_id=@out_test_id output,
    @response=@out_response output;

print (cast(@out_test_id as varchar(12)));
print (@out_response);

    /* transaction 3 */
/*
/* ---------- Example nonframework sql ---------- */
begin transaction
    insert jas.product_options(p_id, o_id, product_option, created_dt) values 
    (3, 1, null, sysutcdatetime());
commit;
/* ---------- ---------- ---------- ---------- */
*/
/* ---------- Example FRAMEWORK sql ---------- */
declare
  @out_test_id      bigint,
  @out_response      nvarchar(max);

exec jas.api_product_options_post 
    @headers = '{ "check_isjson": 1 }',
    @params = null,
    @body='{ "p_id": 3, 
             "product_options": [ 
                    { "o_id": 1 } ] }',
    @test_id=@out_test_id output,
    @response=@out_response output;

print (cast(@out_test_id as varchar(12)));
print (@out_response);



/* CRUD UPDATE -- 1 SINGLE-ROW -- */
/*
/* ---------- Example nonframework sql ---------- */
begin transaction
    update jas.product_options set o_id=2 where po_id=4;
commit;
/* ---------- ---------- ---------- ---------- */
*/
/* ---------- Example FRAMEWORK sql ---------- */
declare
  @out_test_value    bit,
  @out_response      nvarchar(max);

exec jas.api_product_option_put
    @headers = '{ "check_isjson": 1 }',
    @params = null,
    @body='{ "o_id": 2, "po_id": 4 }',
    @test_value=@out_test_value output,
    @response=@out_response output;

print (cast(@out_test_value as varchar(12)));
print (@out_response);


/* CRUD DELETE -- 1 SINGLE-ROW -- */
/*
/* ---------- Example nonframework sql ---------- */
begin transaction
    delete jas.product_options where po_id=5;
commit;
/* ---------- ---------- ---------- ---------- */
*/
/* ---------- Example FRAMEWORK sql ---------- */
declare
  @out_test_value    bit,
  @out_response      nvarchar(max);

exec jas.api_product_option_delete
    @headers = '{ "check_isjson": 1 }',
    @params = '{ "id": 4 }',
    @body=null,
    @test_value=@out_test_value output,
    @response=@out_response output;

print (cast(@out_test_value as varchar(12)));
print (@out_response);



/* CRUD READ -- 5 QUERIES / 4 MULTI-ROW AND 1 SINGLE-ROW -- */
/* Read record(s) */
/* Query 1 */
--select * from jas.products;
exec jas.api_products_get
    @headers = null,
    @params = null;

/* Query 2 */
--select * from jas.options;
exec jas.api_options_get
    @headers = null,
    @params = null;

/* Query 3 */
--select * from jas.product_options where po_id=1;
exec jas.api_product_option_get
    @headers = null,
    @params = '{ "id": 1 }';

/* Query 4 */
--select * from jas.product_options where p_id=1;
exec jas.api_pos_by_option_get
    @headers = null,
    @params = '{ "id": 1 }';

/* Query 5 */
                --select * from jas.product_options where o_id=1;
exec jas.api_pos_by_product_get
    @headers = null,
    @params = '{ "id": 1 }';
