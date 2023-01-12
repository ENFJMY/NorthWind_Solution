select OrderID, OD.ProductID, ProductName, CategoryName, OD.UnitPrice, Quantity
from [Order Details] OD inner join Products P on OD.ProductID = P.ProductID
						inner join Categories C on P.CategoryID = C.CategoryID
where OrderID = 10666

select * from [Order Details]

select top 10 * from Orders order by orderID desc

update Orders set ShipVia = , Freight = , ShippedDate = 
where OrderID = 

delete from Orders where OrderID = 11078

delete from [Order Details] where OrderID = 11078

select * from Employees

cast(EmployeeID as nvarchar) Code

select EmployeeID, FirstName, LastName, Title, 
		convert(varchar(10), BirthDate, 23) BirthDate, 
		convert(varchar(10), HireDate, 23) HireDate 
from Employees

LastName, FirstName, Title, BirthDate, HireDate, Notes, Photo

insert into Employees(LastName, FirstName, Title, BirthDate, HireDate, Notes, Photo)
values (@LastName, @FirstName, @Title, @BirthDate, @HireDate, @Notes, @Photo) 