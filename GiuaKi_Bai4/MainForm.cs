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
//Bai nay ap dung cho bang: 2 ma so giang vien khac nhau khong duoc trung ten
namespace GiuaKi_Bai4
{
    public partial class MainForm : Form
    {
        QuanLy ql;
        public MainForm()
        {
            InitializeComponent();
            string path = @"Data Source=NGUYENTHANHHUYEN;Initial Catalog=GiangVien;Integrated Security=True";
            ql = new QuanLy(path);

            //Load CBB
            loadKhoacbb(path);
            loadHPhancbb(path);
        }
        private void MainForm_Load(object sender, EventArgs e)
        {

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
        public void upDataGridView(string query)
        {
            dataGridView1.DataSource = ql.GetData(query);
            int i = 1;
            int n = dataGridView1.RowCount;
            for (i = 0; i < n; i++)
                dataGridView1.Rows[i].Cells["STT"].Value = i + 1;
        }
        public void ShowDataGridView()
        {
            string query = "SELECT  GiangVienThongTin.Ten  , " +
                " NgaySinh, GioiTinh, HocHam, HocVi, Khoa," +
                "HocPhanThongTin.Ten AS 'HocPhan' " +
                "FROM GiangVienThongTin " +
                "INNER JOIN GV_HP ON GiangVienThongTin.MSGV = GV_HP.MSGV " +
                "INNER JOIN HocPhanThongTin ON HocPhanThongTin.MSHP = GV_HP.MSHP";
            upDataGridView(query);
        }
        private void btShow_Click(object sender, EventArgs e)
        {
            ShowDataGridView();
        }
        private void btDel_Click(object sender, EventArgs e)
        {
            List<string> msList = new List<string>();
            List<string> HPList = new List<string>();
            DataGridViewSelectedRowCollection tap;
            tap = dataGridView1.SelectedRows;
            foreach (DataGridViewRow hang in tap)
            {
                msList.Add(hang.Cells["Ten"].Value.ToString());
                HPList.Add(hang.Cells["HocPhan"].Value.ToString());
            }
            int i = 0;
            foreach (string tengv in msList)
            {
                string query = "SELECT MSGV FROM GiangVienThongTin  " +
            "WHERE Ten = N'" + tengv + "'"; // ko trung ten
                string magv = ql.runExecuteScaSt(query);

                query = "DELETE w  FROM GV_HP w  " +
                    "INNER JOIN HocPhanThongTin t ON w.MSHP = t.MSHP   " +
                    "WHERE w.MSGV = '" + magv + "' AND t.Ten = N'" + HPList[i] + "'";
                ql.ExecuteNonQuery(query);
                i++;
            }
            this.ShowDataGridView();
        }
        private void dataGridView1_RowHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            DataGridViewRow r;
            r = dataGridView1.SelectedRows[0];
            txtName.Text = r.Cells["Ten"].Value.ToString();
            cbbKhoa.SelectedIndex = cbbKhoa.FindStringExact(r.Cells["Khoa"].Value.ToString());
            dateTimePicker1.Value = Convert.ToDateTime(r.Cells["NgaySinh"].Value.ToString());
            cbbHH.SelectedIndex = cbbHH.FindStringExact(r.Cells["HocHam"].Value.ToString());
            cbbHV.SelectedIndex = cbbHV.FindStringExact(r.Cells["HocVi"].Value.ToString());
            cbbHP.SelectedIndex = cbbHP.FindStringExact(r.Cells["HocPhan"].Value.ToString());
            if (r.Cells["GioiTinh"].Value.ToString() == "True")
            {
                rdMale.Checked = true;
            }
            else rdFemale.Checked = true;
        }
        private void btUpdate_Click(object sender, EventArgs e)
        {
            string tenGV;
            tenGV = txtName.Text;
            if(tenGV=="")
            {
                MessageBox.Show("Khong co du lieu");
                return;
            }
            string query = "SELECT MSGV FROM GiangVienThongTin  " +
                "WHERE Ten = N'" + tenGV + "'";
            string magv = ql.runExecuteScaSt(query);
            string mahpCu;
            try
            {
                query = "SELECT MSHP FROM HocPhanThongTin  " +
    "WHERE Ten = N'" + dataGridView1.SelectedRows[0].Cells["HocPhan"].Value.ToString() + "'";
                mahpCu = ql.runExecuteScaSt(query);
                string cbbtemp = cbbHP.SelectedItem.ToString();
                query = "SELECT MSHP FROM HocPhanThongTin  " +
                    "WHERE Ten = N'" + cbbtemp + "'";
                string mahpMoi = ql.runExecuteScaSt(query);

                query = "UPDATE GiangVienThongTin  " +
                    "SET NgaySinh = '" + String.Format("{0:yyyy-MM-dd}", dateTimePicker1.Value) + "', GioiTinh = '" + rdMale.Checked + "',  " +
                    "HocHam = N'" + cbbHH.SelectedItem.ToString() + "', HocVi = N'" + cbbHV.SelectedItem.ToString() + "', Khoa = N'" + cbbKhoa.SelectedItem.ToString() + "'  " +
                    "WHERE MSGV = '" + magv + "'    " +
                    "UPDATE GV_HP  SET MSGV = '" + magv + "', MSHP = '" + mahpMoi + "'  " +
                    "WHERE MSGV = '" + magv + "' AND MSHP = '" + mahpCu + "'";
                ql.ExecuteNonQuery(query);
                ShowDataGridView();
            }
            catch(Exception a)
            {
                MessageBox.Show("Khong co du lieu");
            }
        }
        private void btSearch_Click(object sender, EventArgs e)
        {
            string query;
            string st;
            st = txtSearch.Text;
            query = "SELECT  GiangVienThongTin.Ten, NgaySinh, GioiTinh, HocHam, HocVi, Khoa," +
                "HocPhanThongTin.Ten AS 'HocPhan' " +
                "FROM GiangVienThongTin  " +
                "INNER JOIN GV_HP ON GiangVienThongTin.MSGV = GV_HP.MSGV  " +
                "INNER JOIN HocPhanThongTin ON HocPhanThongTin.MSHP = GV_HP.MSHP  " +
                "WHERE GiangVienThongTin.Ten LIKE '%" + st + "%'";
            upDataGridView(query);
        }

        private void btSort_Click(object sender, EventArgs e)
        {
            string query;
            query = "SELECT  GiangVienThongTin.Ten, NgaySinh, GioiTinh, HocHam, HocVi, Khoa, " +
                "HocPhanThongTin.Ten AS 'HocPhan' FROM GiangVienThongTin " +
                "INNER JOIN GV_HP ON GiangVienThongTin.MSGV = GV_HP.MSGV " +
                "INNER JOIN HocPhanThongTin ON HocPhanThongTin.MSHP = GV_HP.MSHP " +
                "ORDER BY Khoa, HocPhanThongTin.Ten ";
            upDataGridView(query);

        }
        private void btAdd_Click(object sender, EventArgs e)
        {
            AddForm af = new AddForm();
            af.On_OKclick += ShowDataGridView;
            af.Show();
        }
    }
}
