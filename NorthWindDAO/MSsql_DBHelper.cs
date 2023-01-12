using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;

namespace NorthWindDAO
{
    class MSsql_DBHelper
    {
        public int ExecuteNonQuerySP(SqlConnection conn, string sp_nm, List<SqlParameter> pParams, ref string pMsg, ref int pDataCnt)
        {
            try
            {
                if (conn == null)
                    throw new Exception("Connection Error");
                //else if (conn.State != ConnectionState.Open)
                //    throw new Exception("Connection Open Error");

                SqlCommand cmd = new SqlCommand(sp_nm, conn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddRange(pParams.ToArray());
                cmd.Parameters.Add(new SqlParameter("@PO_CD", SqlDbType.Int)).Direction = ParameterDirection.Output;
                cmd.Parameters.Add(new SqlParameter("@PO_MSG", SqlDbType.NVarChar, 1000)).Direction = ParameterDirection.Output;

                conn.Open();
                int iRowAffect = cmd.ExecuteNonQuery();
                conn.Close();

                // SP가 성공했을 경우, pMsg = 빈문자열, pDataCnt = 적용된 행수, 반환값 = 1
                // SP가 실패했을 경우, pMsg = 에러메세지, pDataCnt = 에러코드, 반환값 = -1
                pMsg = cmd.Parameters["@PO_MSG"].Value.ToString();

                if (pMsg.Length < 1)
                {
                    pDataCnt = iRowAffect;
                    return 1;
                }
                else
                {
                    pDataCnt = Convert.ToInt32(cmd.Parameters["@PO_CD"].Value);
                    return -1;
                }
            }
            catch (Exception err)
            {
                pDataCnt = -1;
                pMsg = err.Message;

                return -1;
            }
        }
    }
}
