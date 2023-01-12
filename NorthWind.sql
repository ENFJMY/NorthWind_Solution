-- View ���� ����
	-- �ʿ��� �����͸� ��� �����Ͽ� �ѹ��� �����Ͽ� �ʿ��Ҷ� ȣ���Ͽ� ����ϱ� ���� �۾�
create view VW_NorthwindCode
as
select 'Customer' Category, CustomerID Code, CompanyName Name from Customers
union
select 'Employee' Category, cast(EmployeeID as nvarchar) Code, concat(FirstName,' ',LastName) Name from Employees
union
select 'Category' Category, cast(CategoryID as nvarchar) Code, CategoryName Name from Categories

-- ������ View�� �߰��ϴ� ����
alter view VW_NorthwindCode
as
select 'Customer' Category, CustomerID Code, CompanyName Name from Customers
union
select 'Employee' Category, cast(EmployeeID as nvarchar) Code, concat(FirstName,' ',LastName) Name from Employees
union
select 'Category' Category, cast(CategoryID as nvarchar) Code, CategoryName Name from Categories
union
select 'Shipper' Category, cast(ShipperID as nvarchar) Code, CompanyName Name from Shippers

-- View ����ϴ� ����
select Category, Code , Name
from VW_NorthwindCode
where Category in ('Customer','Employee','Category')

-- cboProducts�� ���ε��� GetProductAllList()����
	-- �޺��ڽ��� ������ �ϱ� ���Ͽ�
select ProductID, ProductName, CategoryID, QuantityPerUnit, UnitPrice, UnitsOnOrder
from Products

-- ��ٱ���
select OrderID, ProductID, UnitPrice, Quantity
from [Order Details]

-- �ֹ��ϱ� ����
insert into Orders(CustomerID, EmployeeID,��OrderDate, RequiredDate)
values (@CustomerID, @EmployeeID,��@OrderDate, @RequiredDate);select @@IDENTITY

select @@IDENTITY
insert into [Order Details](OrderID, ProductID, UnitPrice, Quantity)
values(@OrderID, @ProductID, @UnitPrice, @Quantity)

select distinct category from VW_NorthwindCode

select count(*) ProdutCNT from Products

select * from VW_NorthwindCode

select * from Products