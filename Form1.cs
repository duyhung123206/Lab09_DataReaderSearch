using System;
using System.Data;
using System.Windows.Forms;

namespace Lab09_DataReaderSearch
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            // Nạp dữ liệu toàn bộ sinh viên khi khởi chạy Form
            LoadMockDataToGrid();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // Tự động giãn các cột để lấp đầy bảng DataGridView
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            // Đăng ký sự kiện chọn dòng trên bảng
            dataGridView1.SelectionChanged += dataGridView1_SelectionChanged;
        }

        // 1. Hàm tạo bảng dữ liệu mẫu ban đầu
        private void LoadMockDataToGrid()
        {
            DataTable table = GetSampleDataTable();
            dataGridView1.DataSource = table;
        }

        // Tạo cấu trúc và dữ liệu bảng chung
        private DataTable GetSampleDataTable()
        {
            DataTable table = new DataTable();
            table.Columns.Add("MaSV", typeof(string));
            table.Columns.Add("HoTen", typeof(string));
            table.Columns.Add("NgaySinh", typeof(string));
            table.Columns.Add("GioiTinh", typeof(string));
            table.Columns.Add("TenKhoa", typeof(string));
            table.Columns.Add("DiemTB", typeof(double));
            table.Columns.Add("Email", typeof(string));

            // Thêm dữ liệu mẫu vào bảng
            table.Rows.Add("24001296", "Nguyễn Duy Hưng", "2006-01-01", "Nam", "Công nghệ Thông tin", 8.5, "nguyenduyhung@gmail.com");
            table.Rows.Add("24001297", "Nguyễn Văn Hiếu", "2006-05-12", "Nam", "Công nghệ Thông tin", 8.0, "hieu@gmail.com");

            return table;
        }

        // 2. Hàm tìm kiếm gần đúng theo họ tên (Yêu cầu 2 của bài Lab)[cite: 13]
        private DataTable SearchByName(string keyword)
        {
            DataTable table = GetSampleDataTable();
            DataView dv = table.DefaultView;

            if (!string.IsNullOrEmpty(keyword))
            {
                // Lọc gần đúng theo từ khóa họ tên[cite: 13]
                dv.RowFilter = $"HoTen LIKE '%{keyword.Trim()}%'";
            }

            return dv.ToTable();
        }

        // 3. Sự kiện khi người dùng bấm nút button1 trên giao diện
        private void button1_Click(object sender, EventArgs e)
        {
            string keyword = txtSearchKeyword.Text; // Ô TextBox nhập từ khóa tìm kiếm
            DataTable dtResult = SearchByName(keyword);
            dataGridView1.DataSource = dtResult;
        }

        // 4. Sự kiện đồng bộ dòng chọn trên DataGridView với các ô TextBox thông tin chi tiết[cite: 1]
        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentRow != null && !dataGridView1.CurrentRow.IsNewRow)
            {
                DataGridViewRow row = dataGridView1.CurrentRow;

                // Gán giá trị từ các cột lên các TextBox tương ứng
                txtMaSV.Text = row.Cells["MaSV"].Value?.ToString() ?? "";
                txtHoTen.Text = row.Cells["HoTen"].Value?.ToString() ?? "";
                txtEmail.Text = row.Cells["Email"].Value?.ToString() ?? "";
            }
        }
    }
}