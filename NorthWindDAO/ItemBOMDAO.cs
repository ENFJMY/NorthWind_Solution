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
    public class ItemBOMDAO : IDisposable
    {
        SqlConnection conn;
        public ItemBOMDAO()
        {
            string connStr = ConfigurationManager.ConnectionStrings["BOMDB"].ConnectionString;
            conn = new SqlConnection(connStr);
        }

        public void Dispose()
        {
            if (conn.State == ConnectionState.Open)
                conn.Close();
        }

        public List<ComboItemDTO> GetProductList()
        {   // 완제품 목록을 ComboItemDTO 조회
            string sql = @"SELECT I.ITEM_CD Code, I.ITEM_NM Name, 'BOM' Category
                           FROM ITEM I inner join MYBOM B on I.ITEM_CD = B.ITEM_CD
                           WHERE B.PRNT_ITEM_CD = '*'";
                           // Code, Name, Category로 알리아스르 준 것은 DB가 다르지만 
                           // 현재 프로젝트에서 만들기 위해서 동일한 DTO 프로퍼티로 맞추기 위해서이다.
            SqlCommand cmd = new SqlCommand(sql, conn);
            conn.Open();
            List<ComboItemDTO> list = Helper.DataReaderMapToList<ComboItemDTO>(cmd.ExecuteReader());
            conn.Close();

            return list;
        }

        public List<ItemBOMDTO> GetBOMList(string itemCD)
        {
            SqlCommand cmd = new SqlCommand("SP_GETBOMList", conn); // sql 쿼리문 대신 프로시져명을 입력해준다.
            cmd.CommandType = CommandType.StoredProcedure;          // CommandType 정의 필수
            cmd.Parameters.AddWithValue("@ITEM_CD", itemCD);        // 프로시져 내에 선언된 파라미터 값을 준다.

            conn.Open();
            List<ItemBOMDTO> list = Helper.DataReaderMapToList<ItemBOMDTO>(cmd.ExecuteReader());
            conn.Close();

            return list;
        }
    }
}
