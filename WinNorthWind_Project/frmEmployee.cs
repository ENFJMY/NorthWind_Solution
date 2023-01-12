using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.Configuration;
using WinNorthWind_Project.Services;
using NorthWindDTO;
using WinNorthWind_Project.Utils;
using System.IO; // 이미지 관련

namespace WinNorthWind_Project
{
    public partial class frmEmployee : Form
    {
        EmployeeService srv;
        List<EmployeeDTO> allList;
        string saveFileName; // Excel Export 할때 사용하기 위한 전역변수


        public frmEmployee()
        {
            InitializeComponent();
        }

        private void frmEmployee_Load(object sender, EventArgs e)
        {
            picEmp.ImageLocation = "image/noimage.png"; // Image 파일 -> noimage 속성 -> 출력디렉토리에 복사 -> <항상 복사>

            srv = new EmployeeService();
            DataGridViewUtil.SetInitDataGridView(dgvEmp);
            DataGridViewUtil.AddGridTextBoxColumn(dgvEmp,"직원ID", "EmployeeID", 80, frosen: true, align:DataGridViewContentAlignment.MiddleCenter, visible: false);
            DataGridViewUtil.AddGridTextBoxColumn(dgvEmp, "직원 이름", "FirstName", frosen:true);
            DataGridViewUtil.AddGridTextBoxColumn(dgvEmp, "직원 성", "LastName", frosen: true);
            DataGridViewUtil.AddGridTextBoxColumn(dgvEmp, "직책", "Title", 200);
            DataGridViewUtil.AddGridTextBoxColumn(dgvEmp, "생년월일", "BirthDate", align: DataGridViewContentAlignment.MiddleCenter);
            DataGridViewUtil.AddGridTextBoxColumn(dgvEmp, "취직일", "HireDate", align: DataGridViewContentAlignment.MiddleCenter);
            DataGridViewUtil.AddGridTextBoxColumn(dgvEmp, "전화번호", "HomePhone", 150);
            DataGridViewUtil.AddGridTextBoxColumn(dgvEmp, "비고", "Notes", visible:false);
            DataGridViewUtil.AddGridTextBoxColumn(dgvEmp, "사진", "Photo", visible: false);

            // 폼이 로드될때 전체 직원정보를 조회해서 바인딩을 하고,
            // 조회버튼을 클릭할때 전체 직원정보에서 필터링해서 바인딩
            allList = srv.GetAllEmployees();
            dgvEmp.DataSource = allList;

            tabControl1.SelectedIndex = 1;
        }

        private void btnAddPic_Click(object sender, EventArgs e)
        { // 사진등록
            // 파일열기 다이얼로그를 보여주고, 선택한 파일의 이미지를 pictureBox컨트롤에 바인딩
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "Image Files(*.gif;*.jpg;*.png;*.bmp;*.jfif)|*.gif;*.jpg;*.png;*.bmp;*.jfif";
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                picEmp.ImageLocation = dlg.FileName;
            }
        }

        private void btnAddEmployee_Click(object sender, EventArgs e)
        {
            // 유효성 체크 (입력여부체크, 정규화체크)
            if (! CommonUtil.IsPhoneNumber(txtHomePhone.Text))
            {
                MessageBox.Show("전화번호 형식이 아닙니다.");
                return;
            }
            string errMsg = CommonUtil.InputTextCheck(panel1); 
            if (errMsg.Length > 0)
            {
                MessageBox.Show(errMsg);
                return;
            }

            // 처리
            // 1. 이미지를 byte[]로 변환 (방법2 사용)
            FileStream fs = new FileStream(picEmp.ImageLocation, FileMode.Open, FileAccess.Read);
            byte[] bImage = new byte[fs.Length]; // 이미지를 담을 배열
            fs.Read(bImage, 0, (int)fs.Length);  // 이미지가 바이트배열로 변환이 되어 배열에 담긴다.
            #region 이미지를 DB에 저장하는 방법 
            // 방법1. 경로를 저장 varchar(50) => 실행프로그램 exe로 부터의 상대경로 저장
            //      -- 이미지를 서버에 저장(파일을 서버에 업로드)하고, 서버의 경로를 상대경로로 DB에 저장
            // 방법2. 이미지를 저장 BLOB, IMAGE (위 사용)
            //      -- 이미지를 byte[]로 저장 
            //      -- DB에 저장되는 사이즈가 크다는 단점 (사원사진, 서명이미지와 같은 작은 사이즈의 이미지 사용 O)
            #endregion

            // 2. DTO를 생성
            EmployeeDTO newEmp = new EmployeeDTO()
            {
                FirstName = txtFirstName.Text,
                LastName= txtLastName.Text,
                Title = txtTitle.Text,
                BirthDate = dtpBirthDate.Value.ToShortDateString(),
                HireDate = dtpHireDate.Value.ToString("yyyy-MM-dd"), // ToString("yyyy-MM-dd") == ToShortDateString()
                HomePhone = txtHomePhone.Text,
                Notes = txtNotes.Text,
                Photo = bImage // 위 생성한 bImage 배열
            };

            // 3. 서비스 등록 메서드 호출
            bool result = srv.AddEmployee(newEmp);
            if (result)
            {
                MessageBox.Show("직원 등록이 완료되었습니다.");
                return;
            }
            else
            {
                MessageBox.Show("다시 시도하여 주십시오.", "등록 에러");
            }
        }

        private void dgvEmp_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            // ==> 유효성 체크 (GridView의 기본 유효성체크)
            if (e.RowIndex < 0) return;

            // ==> 선택한 사원의 EmployeeID 값을 얻어와서
            int empID = Convert.ToInt32(dgvEmp[0, e.RowIndex].Value); // 더블클릭한 EmployeeID값을 변수에 담는다.
            //int empID = Convert.ToInt32(dgvEmp["EmployeeID", e.RowIndex].Value); // 위와 같은 코딩

            // ==> DB에서 EmployeeID 값의 상제정보를 조회해서 EmployeeDTO로 전달받는다.
            EmployeeDTO emp = srv.GetEmployeeInfo(empID);

            // ==> EmployeeDTO의 속성을 각 컨트롤에 바인딩
            if (emp != null)
            {
                lblEmployeeID.Text = emp.EmployeeID.ToString(); // 삭제하기 위하여 숨겨놓은 라벨에 ID 값을 입력해준다.
                txtFirstName2.Text = emp.FirstName;
                txtLastName2.Text = emp.LastName;
                txtTitle2.Text = emp.Title;
                dtpBirthDate2.Value = Convert.ToDateTime(emp.BirthDate);
                dtpHiredDate2.Value = Convert.ToDateTime(emp.HireDate);
                txtHomePhone2.Text = emp.HomePhone;
                txtNotes2.Text = emp.Notes;

                // ==> byte[] -> Image로 변환해서 picturebox에 바인딩
                MemoryStream ms = new MemoryStream(emp.Photo);
                picEmp2.Image = Image.FromStream(ms); // 에러발생 시 아래 참조
                // 바이트배열로부터 스트림을 얻어내면 Image.FromStream()을 사용할 수 있다. Image.FromStream() 사용하기 위해서 MemoryStream에 담아준다.

                #region Image.FromStream(ms)에서 오류가 발생하는 경우 아래 코드 사용
                // 정상적인 이미지 포맷이 아니거나, 적절한 메타정보가 없는 경우에 오류가 발생할 수 있다.

                //TypeConverter tc = TypeDescriptor.GetConverter(typeof(Bitmap));
                //picEmp2.Image = (Bitmap)tc.ConvertFrom(emp.Photo);

                // CommonUtil에 메서드를 생성하여 사용하는 경우 (위 코드를 Util에 함수로 정의)
                // picEmp2.Image = CommonUtil.ByteToImage(emp.Photo);
                #endregion
            }
            #region 위와 같은 코드
            //if (e.RowIndex < 0) return;
            //int empID = Convert.ToInt32(dgvEmp[0, e.RowIndex].Value);
            //List<EmployeeDTO> list = srv.GetAllEmployees();
            //EmployeeDTO empList = list.Find((emp) => emp.EmployeeID == empID);
            //if (empList != null)
            //{
            //    txtFirstName2.Text = empList.FirstName;
            //    txtLastName2.Text = empList.LastName;
            //    txtTitle2.Text = empList.Title;
            //    dtpBirthDate2.Value = Convert.ToDateTime(empList.BirthDate);
            //    dtpHiredDate2.Value = Convert.ToDateTime(empList.HireDate);
            //    txtHomePhone2.Text = empList.HomePhone;
            //}
            #endregion
        }

        // ===================>  BtnSearch 로딩 팝업 사용안 할 경우  <=====================================================================
        private void btnSearch_Click(object sender, EventArgs e) // button3
        {
            string search = txtKeyword.Text.Trim(); // 입력된 [검색어]를 변수처리

            // 로직1 : 검색어가 없는 경우 전체 사원목록을 보여주고
            if (search.Length < 1)
            {
                dgvEmp.DataSource = null;   // 가끔 바인딩이 안될 경우 대비
                dgvEmp.DataSource = allList;
            }

            // 로직2 : 검색어가 있는 경우 검색 조건에 따라 전체 사원목록을 필터링해서 보여준다.
            else
            {
                List<EmployeeDTO> list = null; // 초기 빈 값의 직원 목록
                if (rdoName.Checked)
                {
                    // Linq 사용
                    list = (from emp in allList
                            where emp.FirstName.Contains(search) || emp.LastName.Contains(search)
                            select emp).ToList();
                    // 직원 전체목록에서 검색된 FirstName 이나 LastName이 있으면 검색된 직원들을 list로 담는다.
                }
                else if (rdoTitle.Checked)
                {
                    // 확장메서드 사용
                    list = allList.FindAll((emp) => emp.Title.ToLower().Contains(search.ToLower()));
                }
                else
                {
                    list = allList.Where((emp) => emp.HireDate.Contains(search)).ToList();
                }
                dgvEmp.DataSource = null;
                dgvEmp.DataSource = list; // 검색어가 있는 경우 list에 담은 직원 목록
            }
        }
        // ===================>  BtnSearch 로딩 팝업 사용할 경우  <========================================================================
        #region BtnSearch 직원 검색 로딩 팝업 사용 할 경우
        //private void btnSearch_Click(object sender, EventArgs e) // button3
        //{
        //    // 로직1 : 검색어가 없는 경우 전체 사원목록을 보여주고
        //    if (string.IsNullOrWhiteSpace(txtKeyword.Text))
        //    {
        //        dgvEmp.DataSource = null;   // 가끔 바인딩이 안될 경우 대비
        //        dgvEmp.DataSource = allList;

        //        MessageBox.Show("검색 조건을 입력해주세요.");
        //    }

        //    // 로직2 : 검색어가 있는 경우 검색 조건에 따라 전체 사원목록을 필터링해서 보여준다.
        //    else
        //    {
        //        frmPopUp_LoadingAsync pop = new frmPopUp_LoadingAsync(EmployeeDataBinding);
        //        pop.Show();
        //    }
        //}
        //private void EmployeeDataBinding()
        //{
        //    string search = txtKeyword.Text.Trim(); // 입력된 [검색어]를 변수처리

        //    List<EmployeeDTO> list = null; // 초기 빈 값의 직원 목록
        //    if (rdoName.Checked)
        //    {
        //        // Linq 사용
        //        list = (from emp in allList
        //                where emp.FirstName.Contains(search) || emp.LastName.Contains(search)
        //                select emp).ToList();
        //        // 직원 전체목록에서 검색된 FirstName 이나 LastName이 있으면 검색된 직원들을 list로 담는다.
        //    }
        //    else if (rdoTitle.Checked)
        //    {
        //        // 확장메서드 사용
        //        list = allList.FindAll((emp) => emp.Title.ToLower().Contains(search.ToLower()));
        //    }
        //    else
        //    {
        //        list = allList.Where((emp) => emp.HireDate.Contains(search)).ToList();
        //    }

        //    //Task.Delay(5000).Wait(); // 테스트 해보기 위해 5초동안 팝업 띄우기
        //    this.Invoke(new Action(() => dgvEmp.DataSource = null));
        //    this.Invoke(new Action(() => dgvEmp.DataSource = list)); // 검색어가 있는 경우 list에 담은 직원 목록
        //}
        #endregion
        // ===============================================================================================================================
        private void btnRefresh_Click(object sender, EventArgs e)
        {
            txtKeyword.Text = "";
            rdoName.Checked = true;

            btnSearch.PerformClick();
        }
        private void btnEmpUpdate_Click(object sender, EventArgs e)
        {

        }
        private void btnEmpDel_Click(object sender, EventArgs e)
        {
            // delete from Employees where empID = '' 쿼리로 삭제(아이디 정보만으로 삭제처리)
        }
        private void btnExcel_Click(object sender, EventArgs e)
        {
            SaveFileDialog dlg = new SaveFileDialog();
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                saveFileName = dlg.FileName; // 전역변수에 string saveFileName; 선언하여 사용
                //ExcelEmployeeExport();
                frmPopUp_LoadingAsync pop = new frmPopUp_LoadingAsync(ExcelEmployeeExport);
                pop.ShowDialog();
            }
        }
        private void ExcelEmployeeExport()
        {
            // 현재 그리드뷰에 직원 목록 excel export
            List<EmployeeDTO> list = (List<EmployeeDTO>)dgvEmp.DataSource;
            // 전체 목록 있는 직원 목록 excel export
            //List<EmployeeDTO> allList = (List<EmployeeDTO>)dgvEmp.DataSource;

            string errMsg = ExcelUtil.ExcelExportListDTO<EmployeeDTO>(list, saveFileName, "Notes|Photo"); // btnExcel_Click에서 선언한 saveFileName 사용
            if (errMsg != null)
            {
                MessageBox.Show(errMsg);
            }
            else
            {
                MessageBox.Show("엑셀 다운로드 완료");
            }
        }
    }
}
