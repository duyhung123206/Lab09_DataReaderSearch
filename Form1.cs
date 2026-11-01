using System;
using System.Data;
using System.Windows.Forms;

namespace Lab09_DataReaderSearch
{
    public partial class Form1 : Form
    {
        // Khai báo biến toàn cục quản lý phân trang
        private int currentPage = 1;
        private int pageSize = 10; // Mỗi trang hiển thị 10 sinh viên

        public Form1()
        {
            InitializeComponent();

            // Nạp dữ liệu phân trang khi khởi chạy Form
            LoadDataWithPagination();

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

        // 1. Hàm tạo bảng dữ liệu mẫu ban đầu
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

        // 2. Hàm tìm kiếm gần đúng theo họ tên
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

            string filterExpression = $"DiemTB >= {minScore} AND DiemTB <= {maxScore}";

            if (!string.IsNullOrEmpty(tenKhoa) && tenKhoa != "Tất cả")
            {
                filterExpression += $" AND TenKhoa = '{tenKhoa}'";
            }

            dv.RowFilter = filterExpression;
            return dv.ToTable();
        }

        // 6. Hàm tìm kiếm đa tiêu chí (kết hợp mã, họ tên, khoa, giới tính, khoảng điểm; cho phép bỏ trống)
        private DataTable MultiCriteriaSearch(string maSV, string hoTen, string tenKhoa, string gioiTinh, double minScore, double maxScore)
        {
            DataTable table = GetSampleDataTable();
            DataView dv = table.DefaultView;

            string filterExpression = $"DiemTB >= {minScore} AND DiemTB <= {maxScore}";

            if (!string.IsNullOrEmpty(maSV))
            {
                filterExpression += $" AND MaSV LIKE '%{maSV.Trim()}%'";
            }

            if (!string.IsNullOrEmpty(hoTen))
            {
                filterExpression += $" AND HoTen LIKE '%{hoTen.Trim()}%'";
            }

            if (!string.IsNullOrEmpty(tenKhoa) && tenKhoa != "Tất cả")
            {
                filterExpression += $" AND TenKhoa = '{tenKhoa}'";
            }

            if (!string.IsNullOrEmpty(gioiTinh) && gioiTinh != "Tất cả")
            {
                filterExpression += $" AND GioiTinh = '{gioiTinh}'";
            }

            dv.RowFilter = filterExpression;
            return dv.ToTable();
        }

        // 4. Hàm phân trang dữ liệu (Mô phỏng OFFSET-FETCH)
        private DataTable GetPagedStudents(int pageIndex)
        {
            DataTable table = GetSampleDataTable();
            int totalRows = table.Rows.Count;
            int startIndex = (pageIndex - 1) * pageSize; 

            if (startIndex >= totalRows && totalRows > 0)
            {
                currentPage = totalRows / pageSize + (totalRows % pageSize == 0 ? 0 : 1);
                startIndex = (currentPage - 1) * pageSize;
            }
            if (startIndex < 0) startIndex = 0;

            DataTable pagedTable = table.Clone();
            int endIndex = Math.Min(startIndex + pageSize, totalRows);
            for (int i = startIndex; i < endIndex; i++)
            {
                pagedTable.ImportRow(table.Rows[i]);
            }

            return pagedTable;
        }

        private void LoadDataWithPagination()
        {
            dataGridView1.DataSource = GetPagedStudents(currentPage);
        }

        // Sự kiện nút Tìm kiếm theo tên (button1)
        private void button1_Click(object sender, EventArgs e)
        {
            string keyword = txtSearchKeyword.Text;
            DataTable dtResult = SearchByName(keyword);
            dataGridView1.DataSource = dtResult;
        }

        // Sự kiện nút Lọc theo khoa và khoảng điểm
        private void btnFilter_Click(object sender, EventArgs e)
        {
            string khoa = cbKhoa != null && cbKhoa.SelectedItem != null ? cbKhoa.SelectedItem.ToString() : "Tất cả";

            double min = double.TryParse(txtMinScore?.Text, out double minVal) ? minVal : 0.0;
            double max = double.TryParse(txtMaxScore?.Text, out double maxVal) ? maxVal : 10.0;

            DataTable dtResult = FilterByDepartmentAndScore(khoa, min, max);
            dataGridView1.DataSource = dtResult;
        }

        // Sự kiện nút Trước (Previous Page)
        private void btnPrevious_Click(object sender, EventArgs e)
        {
            if (currentPage > 1)
            {
                currentPage--;
                LoadDataWithPagination();
            }
        }

        // Sự kiện nút Sau (Next Page)
        private void btnNext_Click(object sender, EventArgs e)
        {
            currentPage++;
            LoadDataWithPagination();
        }

        // 5. Sự kiện hiển thị chi tiết dòng được chọn chuẩn theo bài Lab
        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentRow == null || dataGridView1.CurrentRow.IsNewRow) return;

            DataGridViewRow row = dataGridView1.CurrentRow;

            // Đưa dữ liệu từ các cột lên các control chi tiết tương ứng
            txtMaSV.Text = Convert.ToString(row.Cells["MaSV"].Value);
            txtHoTen.Text = Convert.ToString(row.Cells["HoTen"].Value);
            txtEmail.Text = Convert.ToString(row.Cells["Email"].Value);
        }
    }
}