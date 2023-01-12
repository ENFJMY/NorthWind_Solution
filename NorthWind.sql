-- View 생성 쿼리
	-- 필요한 데이터를 뷰로 생성하여 한번에 저장하여 필요할때 호출하여 사용하기 위한 작업
create view VW_NorthwindCode
as
select 'Customer' Category, CustomerID Code, CompanyName Name from Customers
union
select 'Employee' Category, cast(EmployeeID as nvarchar) Code, concat(FirstName,' ',LastName) Name from Employees
union
select 'Category' Category, cast(CategoryID as nvarchar) Code, CategoryName Name from Categories

-- 생성된 View에 추가하는 쿼리
alter view VW_NorthwindCode
as
select 'Customer' Category, CustomerID Code, CompanyName Name from Customers
union
select 'Employee' Category, cast(EmployeeID as nvarchar) Code, concat(FirstName,' ',LastName) Name from Employees
union
select 'Category' Category, cast(CategoryID as nvarchar) Code, CategoryName Name from Categories
union
select 'Shipper' Category, cast(ShipperID as nvarchar) Code, CompanyName Name from Shippers

-- View 출력하는 쿼리
select Category, Code , Name
from VW_NorthwindCode
where Category in ('Customer','Employee','Category')

-- cboProducts에 바인딩할 GetProductAllList()쿼리
	-- 콤보박스를 별도로 하기 위하여
select ProductID, ProductName, CategoryID, QuantityPerUnit, UnitPrice, UnitsOnOrder
from Products

-- 장바구니
select OrderID, ProductID, UnitPrice, Quantity
from [Order Details]

-- 주문하기 쿼리
insert into Orders(CustomerID, EmployeeID,　OrderDate, RequiredDate)
values (@CustomerID, @EmployeeID,　@OrderDate, @RequiredDate);select @@IDENTITY

select @@IDENTITY
insert into [Order Details](OrderID, ProductID, UnitPrice, Quantity)
values(@OrderID, @ProductID, @UnitPrice, @Quantity)

select distinct category from VW_NorthwindCode

select count(*) ProdutCNT from Products

select * from VW_NorthwindCode

select * from Products