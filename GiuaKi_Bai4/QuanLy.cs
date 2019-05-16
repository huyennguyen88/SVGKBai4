using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;
namespace GiuaKi_Bai4
{
    public class QuanLy
    {
        SqlConnection cnn;
        public QuanLy(string s)
        {
            cnn = new SqlConnection(s);
        }
        public DataTable GetData(string query)
        {
            SqlDataAdapter ds = new SqlDataAdapter(query,cnn);
            DataTable dt = new DataTable();
            if(cnn.State == ConnectionState.Closed) cnn.Open();

            ds.Fill(dt);
            this.cnn.Close();
            return dt;
        }
        public void ExecuteNonQuery(string query)
        {
            SqlCommand cmd = new SqlCommand(query,cnn);
            if (cnn.State == ConnectionState.Closed) cnn.Open();
            cmd.ExecuteNonQuery();
            this.cnn.Close();
        }
        public string runExecuteScaSt(string query)
        {
            SqlCommand cmd = new SqlCommand(query, cnn);
            if (cnn.State == ConnectionState.Closed) cnn.Open();
            string st;
            st = (string) cmd.ExecuteScalar();
            this.cnn.Close();
            return st;
        }
        public int runExecuteScaInt(string query)
        {
            SqlCommand cmd = new SqlCommand(query, cnn);
            if (cnn.State == ConnectionState.Closed) cnn.Open();
            int st;
            st = (int)cmd.ExecuteScalar();
            this.cnn.Close();
            return st;
        }
        public string getPath()
        {
            return cnn.ConnectionString;
        }
    }
}
