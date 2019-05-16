using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
namespace GiuaKi_Bai4
{
    public partial class AddForm : Form
    {
        QuanLy ql;
        public AddForm()
        {
            InitializeComponent();
            string path = @"Data Source=NGUYENTHANHHUYEN;Initial Catalog=GiangVien;Integrated Security=True";
            ql = new QuanLy(path);
            //Load CBB
            loadKhoacbb(path);
            loadHPhancbb(path);
        }
        public void loadKhoacbb(string path)
        {
            string query = "SELECT * FROM GiangVienThongTin";
            SqlConnection cn = new SqlConnection(path);
            SqlCommand cmd = new SqlCommand(query, cn);
            SqlDataReader r;
            cn.Open();
            r = cmd.ExecuteReader();
            while (r.Read())
            {
                string k = r["Khoa"].ToString();
                if (cbbKhoa.FindStringExact(k) < 0)
                {
                    cbbKhoa.Items.Add(k);
                }
            }
            r.Close();
            cn.Close();
        }
        public void loadHPhancbb(string path)
        {
            string query = "SELECT * FROM HocPhanThongTin";
            SqlConnection cn = new SqlConnection(path);
            SqlCommand cmd = new SqlCommand(query, cn);
            SqlDataReader r;
            cn.Open();
            r = cmd.ExecuteReader();
            while (r.Read())
            {
                string k = r["Ten"].ToString();
                if (cbbHP.FindStringExact(k) < 0)
                {
                    cbbHP.Items.Add(k);
                }
            }
            r.Close();
            cn.Close();
        }
        public delegate void deleOK();
        public event deleOK On_OKclick;
        private void btOK_Click(object sender, EventArgs e)
        {
            string khoa = "";
            string ngaysinh = "";
            string hocham = "";
            string hocvi = "";
            string hocphan = "";
            string tengv = txtName.Text;
            string MS = txtMaso.Text;
            bool gioitinh = false;
            if (rdMale.Checked) gioitinh = true;
            else if (rdFemale.Checked) gioitinh = false;
  
            if(tengv=="" || MS=="")
            {
                MessageBox.Show("Khong co du lieu");
                return;
            }
            try
            {
                    khoa = cbbKhoa.SelectedItem.ToString();
                    ngaysinh = String.Format("{0:yyyy-MM-dd}", dateTimePicker1.Value);
                    hocham = cbbHH.SelectedItem.ToString();
                    hocvi = cbbHV.SelectedItem.ToString();
                    hocphan = cbbHP.SelectedItem.ToString();
                }
            catch (Exception ex)
            {
                MessageBox.Show("Khong co du lieu");
                return;
            }
            addBt(tengv, khoa, ngaysinh, hocham, hocvi, hocphan, gioitinh, MS);
            if (On_OKclick != null)
            {
                On_OKclick();
            }
        }
        public void addBt(string tengv, string khoa, string ngaysinh,
      string hocham, string hocvi, string hocphan, bool gioitinh, string MS)
        {     
            string query;
            //Lay ma hoc phan
            string mahp, magv;
            query = "SELECT MSHP FROM HocPhanThongTin  " +
                "WHERE Ten = N'" + hocphan + "'";
            mahp = ql.runExecuteScaSt(query);
            //Kiem tra trung ten
            query = "SELECT COUNT (*) FROM GiangVienThongTin  " +
                "WHERE Ten = N'" + tengv + "'";
            int dem = ql.runExecuteScaInt(query);
            //Kiem tra trung ma
            query = "SELECT COUNT (*) FROM GiangVienThongTin  " +
     "WHERE MSGV = N'" + MS + "'";
             dem = ql.runExecuteScaInt(query);
            if(dem>0)
            {
                MessageBox.Show("Trung ma");
                return;
            }
            if (dem > 0) //Ten giang vien da co
            {
                //Ma gv
                query = "SELECT MSGV FROM GiangVienThongTin  " +
              "WHERE Ten = N'" + tengv + "'";
                magv = ql.runExecuteScaSt(query);
                //Insert bang trung gian
                query = "INSERT INTO GV_HP (MSGV, MSHP)  " +
                    "VALUES('" + magv + "', '" + mahp + "')";
                try
                {
                    ql.ExecuteNonQuery(query);
                }
                catch (Exception e)
                {
                    MessageBox.Show("Cặp dữ liệu giảng viên và học phần này đã tồn tại");
                    return;
                }
            }
            else //Chua co
            {
                //Kiem tra trung ma giang vien
                query = "SELECT COUNT (*) FROM GiangVienThongTin  WHERE MSGV = '" + MS + "'";
                dem = ql.runExecuteScaInt(query);
                if (dem > 0)
                {
                    MessageBox.Show("Mã số giảng viên này đã tồn tại");
                    return;
                }
                query = "INSERT INTO GiangVienThongTin (MSGV,Ten, NgaySinh,GioiTinh,HocHam,HocVi,Khoa)  " +
                    "VALUES('" + MS + "', N'" + tengv + "', '" + ngaysinh + "', '" + gioitinh + "', N'" + hocham + "', N'" + hocvi + "', N'" + khoa + "')  " +
                    "INSERT INTO GV_HP(MSGV, MSHP) VALUES('" + MS + "', '" + mahp + "')";
                ql.ExecuteNonQuery(query);
            }

        }
        private void btCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private void AddForm_Load(object sender, EventArgs e)
        {

        }
    }
}
