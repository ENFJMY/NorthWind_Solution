using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Configuration;
using System.Data.SqlClient;
using NorthWindDTO;

namespace NorthWindDAO
{
    public class EmployeeDAO : IDisposable
    {
        LoggingUtility _log = new LoggingUtility("WinNorthWind_Project", log4net.Core.Level.Info, 30);
        SqlConnection conn;
        public EmployeeDAO()
        {
            string connStr = ConfigurationManager.ConnectionStrings["MyDB"].ConnectionString;
            conn = new SqlConnection(connStr);
        }

        public void Dispose()
        {
            if (conn.State == ConnectionState.Open)
                conn.Close();
        }

        public List<EmployeeDTO> GetAllEmployees()
        {
            string sql = @"select EmployeeID, FirstName, LastName, Title, HomePhone,
                           		  convert(varchar(10), BirthDate, 23) BirthDate, 
                           		  convert(varchar(10), HireDate, 23) HireDate 
                           from Employees
                           order by FirstName, LastName"; // order by FirstName, LastName 필수 X
            SqlCommand cmd = new SqlCommand(sql, conn);

            conn.Open();
            List<EmployeeDTO> list = Helper.DataReaderMapToList<EmployeeDTO>(cmd.ExecuteReader());
            conn.Close();

            return list;
        }
        
        public EmployeeDTO GetEmployeeInfo(int empID)
        {
            string sql = @"select EmployeeID, LastName, FirstName, Title, 
                           	      convert(varchar(10),BirthDate, 23) BirthDate, 
                           	      convert(varchar(10),HireDate, 23) HireDate,
                                  HomePhone, Notes, Photo 
                           from Employees 
                           where EmployeeID = @empID";
            SqlCommand cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@empID", empID);

            conn.Open();
            List<EmployeeDTO> list = Helper.DataReaderMapToList<EmployeeDTO>(cmd.ExecuteReader());
            conn.Close();

            return (list == null) ? null : list[0];
            #region Helper.cs의 DataReaderMapToDTO 함수 사용할 경우
            //EmployeeDTO emp = Helper.DataReaderMapToDTO<EmployeeDTO>(cmd.ExecuteReader());
            //conn.Close();

            //return emp;
            #endregion
        }

        public bool AddEmployee(EmployeeDTO emp)
        {
            //> MSsql_DBHelper 유틸을 사용하여 프로시저에 파라미터 전달 
            MSsql_DBHelper db = new MSsql_DBHelper();

            List<SqlParameter> param = new List<SqlParameter>();
            param.Add(new SqlParameter("@LastName", emp.LastName));
            param.Add(new SqlParameter("@FirstName", emp.FirstName));
            param.Add(new SqlParameter("@Title", emp.Title));
            param.Add(new SqlParameter("@BirthDate", emp.BirthDate));
            param.Add(new SqlParameter("@HireDate", emp.HireDate));
            param.Add(new SqlParameter("@HomePhone", emp.HomePhone));
            param.Add(new SqlParameter("@Notes", emp.Notes));
            param.Add(new SqlParameter("@Photo", emp.Photo));

            param.Add(new SqlParameter("@EmployeeID", emp.EmployeeID));

            string pMsg = "";
            int pCode = 0;
            if (db.ExecuteNonQuerySP(conn, "SP_UpdateEmployees", param, ref pMsg, ref pCode) < 1)
                throw new Exception(pMsg);
            else
                return true;

            #region Store Procedure 생성하고 쿼리 작성 후 파라미터 전달
            //string sql = @"insert into Employees(LastName, FirstName, Title, BirthDate, HireDate, HomePhone, Notes, Photo)
            //               values (@LastName, @FirstName, @Title, @BirthDate, @HireDate, @HomePhone, @Notes, @Photo)";

            //SqlCommand cmd = new SqlCommand("SP_UpdateEmployee", conn);
            //cmd.CommandType = CommandType.StoredProcedure;

            //cmd.Parameters.AddWithValue("@LastName", emp.LastName);
            //cmd.Parameters.AddWithValue("@FirstName", emp.FirstName);
            //cmd.Parameters.AddWithValue("@Title", emp.Title);
            //cmd.Parameters.AddWithValue("@BirthDate", emp.BirthDate);
            //cmd.Parameters.AddWithValue("@HireDate", emp.HireDate);
            //cmd.Parameters.AddWithValue("@HomePhone", emp.HomePhone);
            //cmd.Parameters.AddWithValue("@Notes", emp.Notes);
            //cmd.Parameters.AddWithValue("@Photo", emp.Photo);
            //cmd.Parameters.AddWithValue("@EmployeeID", emp.EmployeeID);

            //cmd.Parameters.Add(new SqlParameter("@PO_CD", SqlDbType.Int)).Direction = ParameterDirection.Output;
            //cmd.Parameters.Add(new SqlParameter("@PO_MSG", SqlDbType.NVarChar, 1000)).Direction = ParameterDirection.Output;

            //conn.Open();
            //int iRowAffect = cmd.ExecuteNonQuery();
            //// == int iRowAffect = Convert.ToInt32(cmd.ExecuteScalar()); => Procedure에서 set @EmployeeID = (select @@IDENTITY)를 사용하지 않고
            //// set = @@IDENTITY 를 사용 할 경우

            //if (Convert.ToInt32(cmd.Parameters["@PO_CD"].Value) < 0)
            //{
            //    _log.WriteError(cmd.Parameters["@PO_MSG"].Value.ToString());
            //    return false;
            //}
            //else
            //    return (iRowAffect > 0);
            #endregion
        }



        //public List<EmployeeDTO> GetEmployeeSearchList(string name, string title, string hireDate)
        //{

        //}
    }
}
