using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// 추가
using System.Data;              
using System.Configuration;     
using System.Data.SqlClient;    
using NorthWindDTO;             // 다른 네임스페이스의 DTO를 사용하기 위해서
// NorthWindDAO -> 참조추가 -> System.Configuration

namespace NorthWindDAO
{
    public class CommonDAO : IDisposable // public 이어야 어셈블리가 달라도 참조가 가능하다.
    {
        // WinNorthWind_Project -> App.Config -> <Connectionstrings> 작성
        SqlConnection conn;
        public CommonDAO()
        {
            string connStr = ConfigurationManager.ConnectionStrings["MyDB"].ConnectionString;
            conn = new SqlConnection(connStr);
        }
        public void Dispose() 
        {
            if (conn != null && conn.State == ConnectionState.Open)
                conn.Close();
        }

        public void CustomerList()
        { // 고객 정보를 DB에서 가져오는 메서드(다양한 테이블에서 정보들을 모아서 저장하여 필요할 때 호출하여 사용하는 메서드)

        }

        public List<ComboItemDTO> GetCodeByCategory(string[] category)
        { // using NorthWindDTO; 추가(다른 네임스페이스의 DTO를 사용하기 위해서)
            #region SQL 쿼리작성
            // ==> 쿼리 작성시 
            // Union으로 쿼리를 합칠 때 합칠 데이터들에게 동일한 알리아스를 준다.
            // Customer, Employee, Category를 구분하기 위해서 Category라는 알리아스로 통일해준다.
            // 3개의 쿼리를 합칠 때 같은 알리아스를 준 데이터의 타입이 같아야 하므로 cast를 통해서 동일한 타입으로 만들어준다.
            // 컬럼별로 동일한 타입으로 변경해줘야 한다. (nchar => nvarchar 같은 경우는 가능하다. 둘다 문자열)

            // ==> View 생성 쿼리
            //--필요한 데이터를 뷰로 생성하여 한번에 저장하여 필요할때 호출하여 사용하기 위한 작업
            //create view VW_NorthwindCode
            //as
            //select 'Customer' Category, CustomerID Code, CompanyName Name from Customers
            //union
            //select 'Employee' Category, cast(EmployeeID as nvarchar) Code, concat(FirstName, ' ', LastName) Name from Employees
            //  union
            //select 'Category' Category, cast(CategoryID as nvarchar) Code, CategoryName Name from Categories
            //union
            //select 'Shipper' Category, cast(ShipperID as nvarchar) Code, CompanyName Name from Shippers

            // ==>View 출력하는 쿼리
            //select Category, Code, Name
            //from VW_NorthwindCode
            //where Category in ('Customer', 'Employee', 'Category')
            #endregion
            string search = "'" + string.Join("','", category) + "'"; // 배열로 받아온 데이터를 구분 지어주기 위한 코드
            string sql = @"select Category, Code , Name
                           from VW_NorthwindCode
                           where Category in (" + search + ")";

            #region 평소 사용하던 코드
            //List<ComboItemDTO> list = new List<ComboItemDTO>();

            //SqlCommand cmd = new SqlCommand(sql, conn);
            //conn.Open();
            //SqlDataReader reader = cmd.ExecuteReader();
            //while (reader.Read())
            //{
            //    list.Add(new ComboItemDTO()
            //    {
            //        Category = reader["Category"].ToString(),
            //        Code = reader["Code"].ToString(),
            //        Name = reader["Name"].ToString()
            //    });
            //}
            //conn.Close();
            //return list; 
            #endregion

            // 유틸[Helper.cs]을 활용하여 코드 간소화 (helper 프로젝트에 포함하여 namespace를 맞춰준다.) 
            SqlCommand cmd = new SqlCommand(sql, conn);
            conn.Open();
            List<ComboItemDTO> list = Helper.DataReaderMapToList<ComboItemDTO>(cmd.ExecuteReader());
            conn.Close();
            return list;
            #region 유틸사용법 설명
            // 커멘드를 오픈하여 실행한다.
            // cmd.ExecuteReader()를 Helper.DataReaderMapToList 함수에 준다.
            // 제네릭 메서드이기 때문에 <T>를 사용한다.
            // 내가 원하는 DTO의 list로 반환해준다.
            // 순서 상관없이 이름으로 찾는다.

            // --> 흔한 에러
            // return 할때 list가 null이면 cmd.ExecuteReader()가 Helper.DataReaderMapToList로 변환하는 부분에서 에러가 난것이다.
            // SetValue(helper.cs)를 해줄때 DB에서의 Type이 다르기 때문이다.
            #endregion
        }
    }
}
