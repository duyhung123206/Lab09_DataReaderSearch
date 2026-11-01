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

            // Gọi hàm khởi tạo dữ liệu mẫu trực tiếp khi mở Form
            LoadMockDataToGrid();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // Tự động giãn các cột để lấp đầy bảng DataGridView
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            // Đăng ký sự kiện chọn dòng
            dataGridView1.SelectionChanged += dataGridView1_SelectionChanged;
        }

        // Tạo bảng dữ liệu mẫu trực tiếp không cần phụ thuộc vào SQL Server
        private void LoadMockDataToGrid()
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

            // Đưa dữ liệu lên DataGridView
            dataGridView1.DataSource = table;
        }

        // Sự kiện đồng bộ dòng chọn trên DataGridView với các ô TextBox thông tin chi tiết[cite: 1]
        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentRow != null && !dataGridView1.CurrentRow.IsNewRow)
            {
                DataGridViewRow row = dataGridView1.CurrentRow;

                // Lấy giá trị từ các cột gán lên các TextBox tương ứng
                txtMaSV.Text = row.Cells["MaSV"].Value?.ToString() ?? "";
                txtHoTen.Text = row.Cells["HoTen"].Value?.ToString() ?? "";
                txtEmail.Text = row.Cells["Email"].Value?.ToString() ?? "";
            }
        }
    }
}