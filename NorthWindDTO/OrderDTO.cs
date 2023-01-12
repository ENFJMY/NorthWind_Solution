using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NorthWindDTO
{
    public class OrderDTO
    {
        public int      OrderID         { get; set; }
        public string   CustomerID      { get; set; }
        public int      EmployeeID      { get; set; }
        public string   OrderDate       { get; set; }       // DB에는 datetime 타입으로 되어있다. 
        public string   RequiredDate    { get; set; }       // DB에는 datetime 타입으로 되어있다. 

        // DTO는 알리아스를 준 값으로 입력해야 한다.
        // DB에서 DateTime으로 정의된 컬럼을 string 속성으로 가지고 오려면...
        // => select 쿼리문에서 Datetime 컬럼을 string 으로 형변환 해서 쿼리문 작성
        public string       CompanyName         { get; set; }
        public string       EmpName             { get; set; }
        public string       ShippedDate         { get; set; }       // DB에는 datetime 타입으로 되어있다. 
        public int?         ShipVia             { get; set; }       // int? 사용한 이유 : DB에서 Null 이기때문에 Null 대신 빈값을 주기 위해서
        public string       ShipCompanyName     { get; set; }
        public decimal?     Freight             { get; set; }       // decimal? 사용한 이유 : DB에서 Null 이기때문에 Null 대신 빈값을 주기 위해서
    }
}
