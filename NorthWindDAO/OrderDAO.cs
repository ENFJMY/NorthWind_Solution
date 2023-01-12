using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// 추가
using System.Data;              
using System.Configuration;
using System.Data.SqlClient;
using NorthWindDTO;
// NorthWindDAO -> 참조추가 -> System.Configuration

namespace NorthWindDAO
{
    public class OrderDAO : IDisposable // public 이어야 어셈블리가 달라도 참조가 가능하다. 
    {
        // WinNorthWind_Project -> App.Config -> <Connectionstrings> 작성
        SqlConnection conn;

        // ======= Log 출력 ========= 예시. public List<ProductInfoDTO> GetProductAllList() ======
        static LoggingUtility _log = new LoggingUtility("WinNorthWind_Project", log4net.Core.Level.Info, 30);
        // ======================================================================================

        public OrderDAO()
        {
            string connStr = ConfigurationManager.ConnectionStrings["MyDB"].ConnectionString;
            conn = new SqlConnection(connStr);
        }
        public void Dispose()
        {
            if (conn.State == ConnectionState.Open)
                conn.Close();
        }

        public List<ProductInfoDTO> GetProductAllList()
        { // 전체 제품을 조회해서 List<ProductInfoDTO>로 반환하는 함수

            //// =================== 로그 안찍고 실행 시 코드 ====================
            //string sql = @"select ProductID, ProductName, CategoryID, QuantityPerUnit, UnitPrice, UnitsOnOrder
            //               from Products";
            //SqlCommand cmd = new SqlCommand(sql, conn);
            //conn.Open();
            //List<ProductInfoDTO> list = Helper.DataReaderMapToList<ProductInfoDTO>(cmd.ExecuteReader());
            //conn.Close();
            //return list;
            //// ==============================================================

            // ==================== 로그 파일 작성 시 코드 ======================
            try
            {
                string sql = @"select ProductID, ProductName, CategoryID, QuantityPerUnit, UnitPrice, UnitsOnOrder
                           from Products";
                SqlCommand cmd = new SqlCommand(sql, conn);
                conn.Open();
                List<ProductInfoDTO> list = Helper.DataReaderMapToList<ProductInfoDTO>(cmd.ExecuteReader());
                if (list == null)
                {
                    throw new Exception("Helper가 null 반환");
                }
                return list;
            }
            catch (Exception err)
            {
                string msg = $"GetProductAllList() =>" + err.Message;
                _log.WriteError(msg);
                return null;
            }
            finally
            {
                if (conn != null && conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
            }
            // ===============================================================
        }


        //> AddOrder 사용자 정의 테이블(dbo.OrderDetailDataType) + SP_SaveOrder 프로시져 사용
        #region AddOrder 사용자 정의 테이블(dbo.OrderDetailDataType) + SP_SaveOrder 프로시져 사용
        //public bool AddOrder(OrderDTO order, List<OrderDetailDTO> cartList)
        //{
        //    //> 사용자 정의 테이블(dbo.OrderDetailDataType) + SP_SaveOrder 프로시져 사용
        //    try
        //    {
        //        using (SqlCommand cmd = new SqlCommand("SP_SaveOrderTable", conn))
        //        {
        //            cmd.CommandType = CommandType.StoredProcedure;

        //            cmd.Parameters.AddWithValue("@CustomerID", order.CustomerID);
        //            cmd.Parameters.AddWithValue("@EmployeeID", order.EmployeeID);
        //            cmd.Parameters.AddWithValue("@RequiredDate", order.RequiredDate);

        //            cmd.Parameters.Add("@OrderDetails", SqlDbType.Structured);
        //            cmd.Parameters["OrderDetails"].TypeName = "dbo.OrderDetailDataType";
        //            cmd.Parameters["OrderDetails"].Value = cartList;

        //            conn.Open();
        //            cmd.ExecuteNonQuery();
        //            conn.Close();
        //        }
        //        return true;
        //    }
        //    catch
        //    {
        //        return false;
        //    }
        //}
        #endregion
        public bool AddOrder(OrderDTO order, List<OrderDetailDTO> cartList)
        {
            #region 프로시저 사용하지 않는 코드
            //public bool AddOrder(OrderDTO order, List<OrderDetailDTO> cartList)
            //{ // 장바구니에서 주문하여 DB에 구매 목록을 저장하는 메서드
            //    #region DB Transaction 기본 틀
            //    //public bool AddOrder(OrderDTO order, List<OrderDetailDTO> cartList)
            //    //{ 
            //    //    conn.Open();
            //    //    SqlTransaction trans = conn.BeginTransaction();
            //    //    try
            //    //    {
            //    //        trans.Commit();
            //    //        return true;
            //    //    }
            //    //    catch
            //    //    {
            //    //        trans.Rollback();
            //    //        return false;
            //    //    }
            //    //    finally
            //    //    {
            //    //        conn.Close();
            //    //    }
            //    //}
            //    #endregion
            //    conn.Open();
            //    SqlTransaction trans = conn.BeginTransaction();
            //    try
            //    {
            //        string sql = @"insert into Orders(CustomerID, EmployeeID,　OrderDate, RequiredDate)
            //                       values (@CustomerID, @EmployeeID,　getdate(), @RequiredDate);select @@IDENTITY"; //select @@IDENTITY = 이제 막 들어간 정보를 가져온다.
            //        SqlCommand cmd = new SqlCommand();
            //        cmd.CommandText = sql;
            //        cmd.Connection = conn;
            //        cmd.Parameters.AddWithValue("@CustomerID", order.CustomerID);
            //        cmd.Parameters.AddWithValue("@EmployeeID", order.EmployeeID);
            //        //cmd.Parameters.AddWithValue("@OrderDate", DateTime.Now); 쿼리에 getdate()를 작성하였기 때문에 파라미터를 주지 않아도 된다. (DB의 시간으로 자동입력)
            //        cmd.Parameters.AddWithValue("@RequiredDate", order.RequiredDate);
            //        cmd.Transaction = trans;
            //        int newOrderID = Convert.ToInt32(cmd.ExecuteScalar()); // 값이 하나기 때문에 ExecuteScalar를 사용한다.

            //        cmd.Parameters.Clear(); // 위에서 선언된 3개의 파라미터값이 사라진다.(하나의 SqlCommand를 아래에서도 계속 사용하기 때문에)

            //        cmd.CommandText = @"insert into [Order Details](OrderID, ProductID, UnitPrice, Quantity)
            //                            values(@OrderID, @ProductID, @UnitPrice, @Quantity)";
            //        // 반복문을 돌기 위해서는 AddWithValue 대신 Add 를 사용하고 new SqlParameter을 추가하고 파라미터 뒤에 SqlDbType을 정의해준다.
            //        // ==> 속성을 등록해놓고 값은 foreach문에서 저장한다.
            //        cmd.Parameters.Add(new SqlParameter("@OrderID", SqlDbType.Int));
            //        cmd.Parameters.Add(new SqlParameter("@ProductID", SqlDbType.Int));
            //        cmd.Parameters.Add(new SqlParameter("@UnitPrice", SqlDbType.Money));
            //        cmd.Parameters.Add(new SqlParameter("@Quantity", SqlDbType.Int));

            //        foreach (OrderDetailDTO item in cartList)
            //        {
            //            cmd.Parameters["@OrderID"].Value = newOrderID;
            //            cmd.Parameters["@ProductID"].Value = item.ProductID;
            //            cmd.Parameters["@UnitPrice"].Value = item.UnitPrice;
            //            cmd.Parameters["@Quantity"].Value = item.Quantity;

            //            cmd.ExecuteNonQuery();
            //        }

            //        trans.Commit();
            //        return true;
            //    }
            //    catch(Exception err)
            //    {
            //        trans.Rollback();
            //        return false;
            //    }
            //    finally
            //    {
            //        conn.Close();
            //    }
            //}
            #endregion

            //> SP_SaveOrder 프로시져 사용
            StringBuilder sbProductID = new StringBuilder();
            StringBuilder sbQty = new StringBuilder();
            StringBuilder sbPrice = new StringBuilder();

            foreach (var item in cartList)
            {
                sbProductID.Append(item.ProductID + "@");
                sbQty.Append(item.Quantity + "@");
                sbPrice.Append(item.UnitPrice.ToString().Substring(0, item.UnitPrice.ToString().IndexOf('.')) + "@");
            }
            List<SqlParameter> param = new List<SqlParameter>();
            param.Add(new SqlParameter("@CustomerID", order.CustomerID));
            param.Add(new SqlParameter("@EmployeeID", order.EmployeeID));
            param.Add(new SqlParameter("@RequiredDate", order.RequiredDate));
            param.Add(new SqlParameter("@productID", sbProductID.ToString().TrimEnd('@')));
            param.Add(new SqlParameter("@Quantity", sbQty.ToString().TrimEnd('@')));
            param.Add(new SqlParameter("@UnitPrice", sbPrice.ToString().TrimEnd('@')));

            MSsql_DBHelper db = new MSsql_DBHelper();
            string pMsg = "";
            int pDataCnt = 0;

            if (db.ExecuteNonQuerySP(conn, "SP_SaveOrder", param, ref pMsg, ref pDataCnt) < 1)
                throw new Exception(pMsg);
            else
                return true;
            // 주문목록과 주문상세목록을 DB에 저장하는 방법
            // 문자열로 전체 주문 값을 받은 다음에 @를 이용하여 문자열을 잘라서 각각의 값으로 변환하여 DB로 전달한다.
        }

        public List<OrderDTO> GetOrderSearchList(string fromDt, string toDt, string customerID, int employeeID) // 작성 완료 후 Service로 복사해간다.
        {   // 직원이 많은 경우 이 메서드를 참고한다.

            StringBuilder sb = new StringBuilder(); // StringBuilder를 사용해서 문자열을 보관하고 추가하기 위해서
            SqlCommand cmd = new SqlCommand(); // SqlCommand를 먼저 선언한 뒤 값은 아래에서 채워준다.

            sb.Append(@"select OrderID, O.CustomerID, C.CompanyName ,
	                           O.EmployeeID, concat(FirstName, ' ', LastName) EmpName,
	                           convert(varchar(10), OrderDate, 23) OrderDate, 
	                           convert(varchar(10), RequiredDate,23) RequiredDate, 
                               convert(varchar(10), ShippedDate, 23) ShippedDate,
	                           ShipVia, S.CompanyName ShipCompanyName, 
	                           case when Freight = 0.00 then null else Freight end Freight
                         from Orders O inner join Customers C on O.CustomerID = C.CustomerID
                         			   inner join Employees E on O.EmployeeID = E.EmployeeID
                         		       left outer join Shippers S on O.ShipVia = S.ShipperID
                         where OrderDate >= @fromDt and OrderDate < @toDt ");
            #region MSSQL 쿼리문
            //==>검색 조건
            //====>그리드뷰에 보여줄 정보를 모두 가져오는 쿼리
            //select OrderID, O.CustomerID, C.CompanyName ,
            //	     O.EmployeeID, concat(FirstName, ' ', LastName) EmpName,
            //	     convert(varchar(10), OrderDate, 23) OrderDate, 
            //	     convert(varchar(10), RequiredDate, 23) RequiredDate, 
            //       convert(varchar(10), ShippedDate, 23) ShippedDate,
            //	     ShipVia, S.CompanyName ShipCompanyName,
            //	     case when Freight = 0.00 then null else Freight end Freight
            //from Orders O inner join Customers C on O.CustomerID = C.CustomerID
            //              inner join Employees E on O.EmployeeID = E.EmployeeID
            //              left outer join Shippers S on O.ShipVia = S.ShipperID
            //where OrderDate >= '1996-01-01' and OrderDate< '1997-01-01'               =>    //where OrderDate >= @fromDt and OrderDate< @toDt
            //and O.CustomerID = 'ROMEY'                                                =>    //and O.CustomerID = @customerID                 
            //and O.EmployeeID = 4                                                      =>    //and O.EmployeeID = @employeeID

            //==> CONVERT 사용법:  
            //====> convert(varchar(10), [DateTime을 string으로 바꿀 컬럼명], 23)
            //====> 23 = yyyy - MM - dd 형식으로 변환하는 내장함수 번호
            //
            //-- case when Freight = 0.00 then null else Freight end Freight
            //====> Freight 값이 0.00이면 null을 반환하고 값이 있으면 그 값을 출력
            #endregion

            cmd.Parameters.AddWithValue("@fromDt", fromDt);
            cmd.Parameters.AddWithValue("@toDt", toDt);

            if (!string.IsNullOrWhiteSpace(customerID)) // customerID가 있으면 sql문 뒤에 연결되어 작성된다.
            {
                sb.Append(" and O.CustomerID = @customerID"); // " 뒤에 한칸 띄어쓰기 필수
                cmd.Parameters.AddWithValue("@CustomerID", customerID); // 파라미터 값을 customerID가 존재할 때만 추가하도록 한다.
            }
            if (employeeID > 0) // employeeID가 있으면 sql문 뒤에 연결되어 작성된다.
            {
                sb.Append(" and O.EmployeeID = @employeeID");
                cmd.Parameters.AddWithValue("@employeeID", employeeID); // 파라미터 값을 employeeID가 존재할 때만 추가하도록 한다.
            }
            cmd.Connection = conn;
            cmd.CommandText = sb.ToString();

            conn.Open();
            List<OrderDTO> list = Helper.DataReaderMapToList<OrderDTO>(cmd.ExecuteReader());
            conn.Close();

            return list;
        }

        public List<OrderDetailDTO> GetOrderDetail(int orderID)
        {
            string sql = @"select OrderID, OD.ProductID, ProductName, CategoryName, OD.UnitPrice, Quantity
                           from [Order Details] OD inner join Products P on OD.ProductID = P.ProductID
                           						   inner join Categories C on P.CategoryID = C.CategoryID
                           where OrderID = @orderID";
            // --> DTO확인 (내가 필요한 정보를 담을 DTO가 존재하는지 확인, 없으면 DTO에 정보를 추가하거나 새로운 DTO 작성) 
            // --> 처음에 public void GetOrderDetail(int orderID) 작성한 메서드의 return 타입을 List<OrderDetailDTO> 변경

            SqlCommand cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@orderID", orderID);

            conn.Open();
            List<OrderDetailDTO> list = Helper.DataReaderMapToList<OrderDetailDTO>(cmd.ExecuteReader());
            conn.Close();
            return list;

            // --> public List<OrderDetailDTO> GetOrderDetail(int orderID) 메서드를 복사해서 Service에 붙혀넣는다

        }

        public bool UpdateOrder(OrderDTO order)
        {// OrderDTO에 아래 파라미터에 전달해야 하는 값들이 다 있기 때문에 order 하나로 값을 전부 전달한다.

            string sql = @"update Orders set ShipVia = @ShipVia
                                           , Freight = @Freight
                                           , ShippedDate = @ShippedDate
                           where OrderID = @OrderID";

            #region Try, Catch, Finally 사용안하는 코드(상관X)
            //SqlCommand cmd = new SqlCommand();
            //cmd.Parameters.AddWithValue("@ShipVia", order.ShipVia);
            //cmd.Parameters.AddWithValue("@Freight", order.Freight);
            //cmd.Parameters.AddWithValue("@ShippedDate", order.ShippedDate);
            //cmd.Parameters.AddWithValue("@OrderID", order.OrderID);

            //conn.Open();
            //int iRowAffect = cmd.ExecuteNonQuery(); // update이기 때문에 ExecuteNonQuery를 사용한다.
            //conn.Close();

            //return iRowAffect > 0;
            #endregion
            try
            {
                SqlCommand cmd = new SqlCommand();
                cmd.Parameters.AddWithValue("@ShipVia", order.ShipVia);
                cmd.Parameters.AddWithValue("@Freight", order.Freight);
                cmd.Parameters.AddWithValue("@ShippedDate", order.ShippedDate);
                cmd.Parameters.AddWithValue("@OrderID", order.OrderID);

                conn.Open();
                int iRowAffect = cmd.ExecuteNonQuery(); // update이기 때문에 ExecuteNonQuery를 사용한다.

                return iRowAffect > 0;
            }
            catch
            {
                return false;
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
            }
        }

        public bool DeleteOrder(int orderID)
        {
            // 트랜젝션 사용
            conn.Open();
            SqlTransaction trans = conn.BeginTransaction();

            try
            {
                string sql = @"delete from [Order Details] where OrderID = @OrderID";

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@OrderID", orderID);
                cmd.Transaction = trans;
                cmd.ExecuteNonQuery();

                cmd.CommandText = "delete from Orders where OrderID = @OrderID";
                cmd.ExecuteNonQuery();

                trans.Commit();
                return true;
            }
            catch
            {
                trans.Rollback();
                return false;
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
            }
        }
    }
}
