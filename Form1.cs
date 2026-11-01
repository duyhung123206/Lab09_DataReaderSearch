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

            // Khởi tạo danh sách các Khoa mẫu cho ComboBox
            LoadDepartmentsToComboBox();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // Tự động giãn các cột để lấp đầy bảng DataGridView
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            // Đăng ký sự kiện chọn dòng trên bảng
            dataGridView1.SelectionChanged += dataGridView1_SelectionChanged;
        }

        // 1. Hàm tạo bảng dữ liệu mẫu ban đầu (đã bổ sung thêm nhiều khoa và điểm số để test lọc)
        private void LoadMockDataToGrid()
        {
            DataTable table = GetSampleDataTable();
            dataGridView1.DataSource = table;
        }

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

            // Thêm dữ liệu mẫu
            table.Rows.Add("24001296", "Nguyễn Duy Hưng", "2006-01-01", "Nam", "Công nghệ Thông tin", 8.5, "nguyenduyhung@gmail.com");
            table.Rows.Add("24001297", "Nguyễn Văn Hiếu", "2006-05-12", "Nam", "Công nghệ Thông tin", 8.0, "hieu@gmail.com");
            table.Rows.Add("24001298", "Trần Văn An", "2006-03-20", "Nam", "Điện tử Viễn thông", 6.5, "an@gmail.com");
            table.Rows.Add("24001299", "Lê Thị Mai", "2006-08-15", "Nữ", "Quản trị Kinh doanh", 7.5, "mai@gmail.com");

            return table;
        }

        // Đổ dữ liệu mẫu vào ComboBox Khoa
        private void LoadDepartmentsToComboBox()
        {
            if (cbKhoa != null)
            {
                cbKhoa.Items.Clear();
                cbKhoa.Items.Add("Tất cả");
                cbKhoa.Items.Add("Công nghệ Thông tin");
                cbKhoa.Items.Add("Điện tử Viễn thông");
                cbKhoa.Items.Add("Quản trị Kinh doanh");
                cbKhoa.SelectedIndex = 0;
            }
        }

        // 2. Hàm tìm kiếm gần đúng theo họ tên[cite: 13]
        private DataTable SearchByName(string keyword)
        {
            DataTable table = GetSampleDataTable();
            DataView dv = table.DefaultView;

            if (!string.IsNullOrEmpty(keyword))
            {
                dv.RowFilter = $"HoTen LIKE '%{keyword.Trim()}%'";
            }

            return dv.ToTable();
        }

        // 3. Hàm lọc theo khoa và khoảng điểm (BETWEEN Min AND Max)
        private DataTable FilterByDepartmentAndScore(string tenKhoa, double minScore, double maxScore)
        {
            DataTable table = GetSampleDataTable();
            DataView dv = table.DefaultView;

            // Xây dựng điều kiện lọc khoảng điểm
            string filterExpression = $"DiemTB >= {minScore} AND DiemTB <= {maxScore}";

            // Nếu có chọn khoa cụ thể (khác "Tất cả")
            if (!string.IsNullOrEmpty(tenKhoa) && tenKhoa != "Tất cả")
            {
                filterExpression += $" AND TenKhoa = '{tenKhoa}'";
            }

            dv.RowFilter = filterExpression;
            return dv.ToTable();
        }

        // Sự kiện nút Tìm kiếm theo tên (button1)
        private void button1_Click(object sender, EventArgs e)
        {
            string keyword = txtSearchKeyword.Text;
            DataTable dtResult = SearchByName(keyword);
            dataGridView1.DataSource = dtResult;
        }

        // Sự kiện nút Lọc theo khoa và khoảng điểm (btnFilter hoặc button2)
        private void btnFilter_Click(object sender, EventArgs e)
        {
            string khoa = cbKhoa != null && cbKhoa.SelectedItem != null ? cbKhoa.SelectedItem.ToString() : "Tất cả";

            // Đọc giá trị điểm min và max (mặc định từ 0 đến 10 nếu người dùng bỏ trống)
            double min = double.TryParse(txtMinScore?.Text, out double minVal) ? minVal : 0.0;
            double max = double.TryParse(txtMaxScore?.Text, out double maxVal) ? maxVal : 10.0;

            DataTable dtResult = FilterByDepartmentAndScore(khoa, min, max);
            dataGridView1.DataSource = dtResult;
        }

        // 4. Sự kiện đồng bộ dòng chọn trên DataGridView với các ô TextBox thông tin chi tiết[cite: 1]
        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentRow != null && !dataGridView1.CurrentRow.IsNewRow)
            {
                DataGridViewRow row = dataGridView1.CurrentRow;

                txtMaSV.Text = row.Cells["MaSV"].Value?.ToString() ?? "";
                txtHoTen.Text = row.Cells["HoTen"].Value?.ToString() ?? "";
                txtEmail.Text = row.Cells["Email"].Value?.ToString() ?? "";
            }
        }
    }
}