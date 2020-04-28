/* Sql without framework */

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
begin transaction
    insert jas.products values('Phone 1', sysutcdatetime());
commit;
    /* transaction 2 */
begin transaction
    insert jas.products values('Phone 2', sysutcdatetime());
commit;
    /* transaction 3 */
begin transaction
    insert jas.products values('Phone 3', sysutcdatetime());
commit;

/* CRUD CREATE (INSERT OPTIONS) -- 1 MULTI-ROW TRANSACTION -- */
/* jas.options sample records */
begin transaction
    insert jas.options values
    ('Screen cover', sysutcdatetime()), 
    ('Outer case 1', sysutcdatetime()), 
    ('Outer case 2', sysutcdatetime());
commit;

/* CRUD CREATE (INSERT PRODUCT_OPTIONS) -- 3 MULTI-ROW TRANSACTIONS -- */
/* jas.product_options same records */
    /* transaction 1 */
begin transaction
    insert jas.product_options(p_id, o_id, product_option, created_dt) values 
    (1, 1, null, sysutcdatetime()),
    (1, 2, null, sysutcdatetime());
commit;
    /* transaction 2 */
begin transaction
    insert jas.product_options(p_id, o_id, product_option, created_dt) values
    (2, 1, null, sysutcdatetime()),
    (2, 3, null, sysutcdatetime());
commit;
    /* transaction 3 */
begin transaction
    insert jas.product_options(p_id, o_id, product_option, created_dt) values (3, 1, null, sysutcdatetime());
commit;



/* CRUD UPDATE -- 1 SINGLE-ROW -- */
begin transaction
    update jas.product_options set o_id=2 where po_id=4;
commit;


/* CRUD DELETE -- 1 SINGLE-ROW -- */
begin transaction
    delete jas.product_options where po_id=5;
commit;



/* CRUD READ -- 5 QUERIES / 4 MULTI-ROW AND 1 SINGLE-ROW -- */
/* Read record(s) */
/* Query 1 */
select * from jas.products;
/* Query 2 */
select * from jas.options;
/* Query 3 */
select * from jas.product_options where po_id=1;
/* Query 4 */
select * from jas.product_options where p_id=1;
/* Query 5 */
select * from jas.product_options where o_id=1;
