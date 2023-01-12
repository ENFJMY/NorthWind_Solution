using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

// 추가
using WinNorthWind_Project.Services; // using을 추가하지 않으려면 해당 클래스에서 .Services를 지워서 namespace를 통일 해준다.
using WinNorthWind_Project.Utils;
using NorthWindDTO;

namespace WinNorthWind_Project
{
    // 1. WinNorthWind_Project 참조추가 -> 프로젝트 -> 솔루션 -> NorthWindDAO, NorthWindDTO 체크 추가 
    // 2. NorthWindDAO 참조추가 -> 프로젝트 -> 솔루션 -> NorthWindDTO 체크 추가 

    // 프로그램 개발 순서 : 쿼리문작성 --> DAO-DTO작성 --> Service 작성 --> 폼에서 호출
    public partial class frmOrder : Form
    {
        OrderService srv;
        List<ProductInfoDTO> productList = null;  // 카테고리를 처음으로 선택할 때 new가 된다.
        List<OrderDetailDTO> cartList = null;     // 장바구니 추가를 처음 클릭할 때 new가 된다.
        string saveFileName;
        public frmOrder()
        {
            InitializeComponent();
        }

        private void frmOrder_Load(object sender, EventArgs e)
        {
            // 3티어 프로그램을 위해 Service 폴더에 OrderService.cs 클래스를 만들어준다.
            string[] category = { "Customer", "Employee", "Category", "Shipper" }; // 배열 초기화


            srv = new OrderService();
            List<ComboItemDTO> list = srv.GetCommonCode(category);

            Program.Log.WriteInfo("frmOrder 공통코드 조회 : " + list.Count); // 로그 작성 (Program.cs 로그 작성 후 작성)

            CommonUtil.ComboBinding(cboCustomer, list, "Customer", blankText: " == Select Customer == "); // blackText: ""를 주면 콤보박스 기본값이 빈값이 된다.
            CommonUtil.ComboBinding(cboEmployee, list, "Employee"); // blankText를 입력하지 않으면 CommonUtil에 기본값으로 설정한 " == Select == "가 입력된다.
            CommonUtil.ComboBinding(cboCategory, list, "Category");

            dtpRequiredDate.Value = DateTime.Now.AddDays(7);

            // DataGridViewUtil을 Utils에 추가하여 메서드 활용
            DataGridViewUtil.SetInitDataGridView(dgvCart);
            DataGridViewUtil.AddGridTextBoxColumn(dgvCart, "카테고리", "CategoryName", 150);
            DataGridViewUtil.AddGridTextBoxColumn(dgvCart, "제품명", "ProductName", 150);
            DataGridViewUtil.AddGridTextBoxColumn(dgvCart, "단가", "UnitPrice", align: DataGridViewContentAlignment.MiddleLeft);
            DataGridViewUtil.AddGridTextBoxColumn(dgvCart, "수량", "Quantity", align: DataGridViewContentAlignment.MiddleLeft);

            DataGridViewUtil.AddGridTextBoxColumn(dgvCart, "OrderID", "OrderID", visible: false);
            DataGridViewUtil.AddGridTextBoxColumn(dgvCart, "ProductID", "ProductID", visible: false);

            // TabPage2 (주문 조회/관리) 로드
            //periodDateTime1.Period = PeriodType.Week1;
            CommonUtil.ComboBinding(cboCustomer2, list, "Customer");
            CommonUtil.ComboBinding(cboEmployee2, list, "Employee");
            CommonUtil.ComboBinding(cboShipper, list, "Shipper");

            //주문목록, 주문상세목록 데이터 그리드 뷰의 컬럼추가
            DataGridViewUtil.SetInitDataGridView(dgvOrder);
            DataGridViewUtil.AddGridTextBoxColumn(dgvOrder, "주문ID", "OrderID");
            DataGridViewUtil.AddGridTextBoxColumn(dgvOrder, "거래처명", "CompanyName");
            DataGridViewUtil.AddGridTextBoxColumn(dgvOrder, "담당직원", "EmpName");
            DataGridViewUtil.AddGridTextBoxColumn(dgvOrder, "주문일", "OrderDate");
            DataGridViewUtil.AddGridTextBoxColumn(dgvOrder, "요청일", "RequiredDate");
            DataGridViewUtil.AddGridTextBoxColumn(dgvOrder, "배송일", "ShippedDate");
            DataGridViewUtil.AddGridTextBoxColumn(dgvOrder, "배송업체", "ShipCompanyName");
            DataGridViewUtil.AddGridTextBoxColumn(dgvOrder, "배송료", "Freight");

            DataGridViewUtil.SetInitDataGridView(dgvOrderDetail);
            DataGridViewUtil.AddGridTextBoxColumn(dgvOrderDetail, "카테고리", "CategoryName");
            DataGridViewUtil.AddGridTextBoxColumn(dgvOrderDetail, "제품명", "ProductName");
            DataGridViewUtil.AddGridTextBoxColumn(dgvOrderDetail, "제품단가", "UnitPrice");
            DataGridViewUtil.AddGridTextBoxColumn(dgvOrderDetail, "주문수량", "Quantity");

            tabControl1.SelectedTab = tabPage2; // 폼이 로드될때 tabPage2(주문 조회/관리)부터 로드

            btnRefresh.PerformClick(); // 조회버튼 이벤트를 줘서 로드될때 자동으로 조회버튼 클릭한 효과
        }

        private void cboCategory_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboCategory.SelectedIndex < 1) return;

            if (productList == null)
            {
                productList = srv.GetProductAllList();
            }

            // 전체 제품중에서 선택된 카테고리에 해당하는 제품목록을 바인딩
            #region 아래 코드 설명
            // selProdList에 전역변수에 선언한 List<ProductInfoDTO> productList를
            // CategoryID를 int로 형변환하여 cboCategory의 선택값으로 하여
            // ComboItemDTO에 code = db의 ProductID로 저장하고 Name = ProductName, ______ 하여 리스트로 저장한다.
            // CommonUtil의 ComboBinding 메서드를 기준으로 cboProducts에 저장해둔 selProdList
            #endregion
            var selProdList = (from prod in productList
                               where prod.CategoryID == Convert.ToInt32(cboCategory.SelectedValue)
                               select new ComboItemDTO()
                               {
                                   Code = prod.ProductID.ToString(),
                                   Name = prod.ProductName,
                                   Category = "Product"
                               }).ToList();
            CommonUtil.ComboBinding(cboProducts, selProdList, "Product");

            // ComboBox의 값이 바뀌면 단위,수량 textbox Clear()
        }

        private void cboProducts_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboProducts.SelectedIndex < 1) return;

            //productList.FindAll((product) => product.ProductID == Convert.ToInt32(cboProducts.SelectedValue)).FirstOrDefault();
            int selProdID = Convert.ToInt32(cboProducts.SelectedValue); // 선택한 제품ID

            #region 아래 코드 설명
            // 첫번째 찾은(FirstOrDefault) ProductID 없으면 null로 반환한다.
            // if() 찾은 ProductID가 null이 아니라면 
            // 주문 단위, 금액, 수량을 증가시킨다.
            // else() 찾은 ProductID가 null이라면 빈값과 0으로 초기화한다.
            #endregion
            ProductInfoDTO selProduct = productList.FirstOrDefault((product) => product.ProductID == selProdID);
            if (selProduct != null)
            {
                txtQuantityPerUnit.Text = selProduct.QuantityPerUnit;
                txtUnitPrice.Text = selProduct.UnitPrice.ToString();
                nuQuantity.Value = selProduct.UnitsOnOrder; // 주문단위 수량
                if (selProduct.UnitsOnOrder == 0)           // 주문단위 수량이 0이면 
                    nuQuantity.Value = 1;                   // 기본값을 1로 설정
                nuQuantity.Increment = (selProduct.UnitsOnOrder > 0) ? selProduct.UnitsOnOrder : 1;
                // UnitsOnOrder의 값이 있으면 그 값만큼 추가 ? 0이면 1씩 증가하게 한다.
            }
            else
            {
                txtQuantityPerUnit.Text = txtUnitPrice.Text = "";
                nuQuantity.Value = 0;
            }
        }

        private void btnCartAdd_Click(object sender, EventArgs e)
        {
            //List<OrderDetailDTO> cartList = null; 전역변수 추가

            // 유효성 체크(수량, 제품선택)
            if (cboProducts.SelectedIndex < 1 || nuQuantity.Value < 1)
            {
                MessageBox.Show("장바구니에 추가할 제품,수량을 선택하여 주십시오.", "선택");
                return;
            }

            if (cartList == null)
            {
                cartList = new List<OrderDetailDTO>(); // 처음에만 빈 리스트이고 상품이 존재한다면 추가
            }

            // 장바구니에 담겨진 제품이면 수량 변경, 없으면 추가
            int selProdID = Convert.ToInt32(cboProducts.SelectedValue); // 선택된 제품ID

            int idx = cartList.FindIndex((product) => product.ProductID == selProdID); // 장바구니에 추가했던 상품을 찾는다.
            if (idx >= 0) // 장바구니에 동일한 상품이 있다면
            {
                cartList[idx].Quantity += (int)nuQuantity.Value; // 장바구니에서 수량을 증가시킨다.
            }
            else // 장바구니에 존재하지 않는 상품이라면
            {
                OrderDetailDTO newItem = new OrderDetailDTO()
                {
                    CategoryName = cboCategory.Text,
                    ProductID = Convert.ToInt32(cboProducts.SelectedValue),
                    ProductName = cboProducts.Text,
                    UnitPrice = Convert.ToDecimal(txtUnitPrice.Text),
                    Quantity = (int)nuQuantity.Value // decimal을 int로 형변환
                };
                cartList.Add(newItem);
            }

            dgvCart.DataSource = null; // 처음에 null 값을 주고 cartList에 DataSource를 주면 원활하게 실행된다.
            dgvCart.DataSource = cartList;

            dgvCart.ClearSelection();
        }

        private void btnCartDel_Click(object sender, EventArgs e)
        {
            // 유효성체크(삭제할 제품을 선택했는지 체크)
            if (dgvCart.SelectedRows.Count < 1)
            {
                MessageBox.Show("삭제하실 제품 선택하여 주십시오.");
                return;
            }

            string msg = $"{dgvCart["ProductName", dgvCart.SelectedRows[0].Index].Value} 제품을 삭제하시겠습니까?";
            if (MessageBox.Show(msg, "장바구니 삭제", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                // 처리(CartList에서 해당 제품을 선택해서 삭제)
                //int prodID = Convert.ToInt32(dgvCart["ProductID", dgvCart.SelectedRows[0].Index].Value);
                int prodID = Convert.ToInt32(dgvCart.SelectedRows[0].Cells["ProductID"].Value);

                int index = cartList.FindIndex((product) => product.ProductID == prodID);
                if (index >= 0) // 찾으면 0보다 큰값이 나온다.
                {
                    cartList.RemoveAt(index);
                    dgvCart.DataSource = null;
                    dgvCart.DataSource = dgvCart;
                }
            }
        }

        private void btnOrder_Click(object sender, EventArgs e)
        {
            // 유효성 체크(장바구니 제품, 고객, 직원 선택했는지 체크)
            if (cboCustomer.SelectedIndex < 1 || cboEmployee.SelectedIndex < 1)
            {
                MessageBox.Show("주문정보를 입력하여 주십시오.", "주문 에러");
                return;
            }
            if (cartList == null || cartList.Count < 1)
            {
                MessageBox.Show("장바구니에 주문하실 제품을 추가하여 주십시오.", "주문 에러");
                return;
            }

            OrderDTO order = new OrderDTO()
            {
                CustomerID = cboCustomer.SelectedValue.ToString(),
                EmployeeID = Convert.ToInt32(cboEmployee.SelectedValue),
                RequiredDate = dtpRequiredDate.Value.ToString("yyyy-MM-dd")
            };
            // 처리 (DB => Order, OrderDetails에 저장) => ( order, cartList)
            bool result = srv.AddOrder(order, cartList);
            if (result)
            {
                MessageBox.Show("신규 주문등록이 완료되었습니다.", "주문 완료");
                InitControl();
            }
            else
            {
                MessageBox.Show("다시 시도하여 주십시오.", "주문 에러");
            }
        }
        private void InitControl()
        {
            // 주문하기 후 폼의 입력된 값을 초기화하는 메서드
            cartList.Clear();
            dgvCart.DataSource = null;
            cboCustomer.SelectedIndex = cboEmployee.SelectedIndex = cboCategory.SelectedIndex = cboProducts.SelectedIndex = 0; // 콤보박스의 선택값을 0번째것으로 초기화해준다.
            dtpRequiredDate.Value = DateTime.Now.AddDays(7);

            txtQuantityPerUnit.Text = txtUnitPrice.Text = "";
            nuQuantity.Value = 0;
        }

        private void btnSearch_Click(object sender, EventArgs e) // 주문/조회 관리 페이지
        {
            string fromDt = periodDateTime1.dtFrom;
            string toDt = periodDateTime1.dtTo;
            string customerID = cboCustomer2.SelectedValue.ToString();
            int employeeID = (cboEmployee2.SelectedIndex < 1) ? 0 : Convert.ToInt32(cboEmployee2.SelectedValue);

            List<OrderDTO> list = srv.GetOrderSearchList(fromDt, toDt, customerID, employeeID);
            dgvOrder.DataSource = list;

            // 주문내역, 주문상세내역 컨트롤 초기화 속성
            dgvOrderDetail.DataSource = null;
            foreach (Label lbl in panel3.Controls)
            {
                lbl.Text = "";
            }
            cboShipper.SelectedIndex = 0;
            dtpShippedDate.Value = DateTime.Now;
            txtFreight.Text = "";
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            cboCustomer2.SelectedIndex = 0;
            cboEmployee2.SelectedIndex = 0;
            //periodDateTime1.dtFrom = DateTime.Now.ToString();
            periodDateTime1.dtFrom = DateTime.Now.AddDays(-7).ToString();
            periodDateTime1.dtTo = DateTime.Now.AddDays(7).ToString();
            btnSearch.PerformClick();
        }

        private void dgvOrder_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            // 유효성 체크(컬럼의 헤더부분을 클릭한 경우 무시)
            if (e.RowIndex < 0) return; // e.RowIndex -1은 컬럼 헤더를 클릭한 경우이다. => 0보다 작으면 컬럼 헤더를 클릭했기 때문에 return한다.

            // ==> 처리 <== 
            // -- 주문번호를 얻어와서,
            int orderID = Convert.ToInt32(dgvOrder[0, e.RowIndex].Value); // 내가 지금 클릭한 행의 0번째 행에 있는 값

            // -- 주문번호의 상세주문내역을 조회하고 그 결과를 데이터그리드뷰에 바인딩 (DAO 작성 -> DTO 작성 -> 서비스 작성 -> frmOrder에서 호출)
            List<OrderDetailDTO> list = srv.GetOrderDetail(orderID);
            dgvOrderDetail.DataSource = list;

            // -- 주문번호의 내용을 리스트에서 찾아서 오른쪽 컨트롤들에 출력(바인딩)
            List<OrderDTO> orderList = (List<OrderDTO>)dgvOrder.DataSource;
            OrderDTO curOrder = orderList.Find((order) => order.OrderID == orderID); // Find는 제일 앞에 것을 찾는다. 있으면 값을 반환하고 없으면 null을 반환한다.
            if (curOrder != null) //curOrder가 null이 아니라면 첫번째 값을 찾은 것이다.
            {
                lblOrderID.Text = curOrder.OrderID.ToString();
                lblCustomer.Text = curOrder.CompanyName;
                lblEmployee.Text = curOrder.EmpName;
                lblOrderDate.Text = curOrder.OrderDate;
                lblRequiredDate.Text = curOrder.RequiredDate;

             // -- 주문번호의 배송내역이 있는 경우(배송중, 배송완료) 주문삭제버튼 비활성화 처리
             // -- 아직 배송처리가 되지 않은 경우는 배송처리 또는 주문삭제 모두 활성화 처리
                if (curOrder.ShipVia == null || curOrder.Freight == null) // 배송전
                {
                    cboShipper.SelectedIndex = 0;   // 다른 데이터 선택후 shipper가 없는 데이터를 선택했을때 combobox 0번째 값으로 초기화 
                    txtFreight.Text = "";           // 다른 데이터 선택후 Freight가 없는 데이터를 선택했을때 빈 문자열로 값으로 초기화 

                    dtpShippedDate.Format = DateTimePickerFormat.Short; 
                    dtpShippedDate.Value = DateTime.Now;

                    btnDelete.Enabled = true;
                    btnShip.Enabled = true;
                }
                else // 배송중 , 배송완료
                { // 배송정보를 바인딩

                    cboShipper.SelectedValue = curOrder.ShipVia.ToString();
                    txtFreight.Text = curOrder.Freight.ToString();
                    if (curOrder.ShippedDate == null)
                    {
                        dtpShippedDate.Format = DateTimePickerFormat.Custom; // DateTimePicker를 빈값으로 주기 위해서 포맷을 custom으로 변경 후 
                        dtpRequiredDate.CustomFormat = "";                   // 빈값으로 설정
                    }
                    else
                    {
                        dtpShippedDate.Format = DateTimePickerFormat.Short;              // DateTimePicker에 값을 줘야 하기 때문에 포맷을 Short으로 변경 한후
                        dtpShippedDate.Value = Convert.ToDateTime(curOrder.ShippedDate); // 배송일을 값으로 준다.
                    }
                    
                    btnDelete.Enabled = false;
                    btnShip.Enabled = false;
                }
            }
        }

        private void btnShip_Click(object sender, EventArgs e)
        {
            // 유효성 체크
            if (string.IsNullOrWhiteSpace(lblOrderID.Text))
            {
                MessageBox.Show("배송 처리할 주문 정보를 선택하여 주십시오.");
                return;
            }

            if (cboShipper.SelectedIndex < 1 == string.IsNullOrWhiteSpace(txtFreight.Text))
            {
                MessageBox.Show("배송정보를 입력하세요");
                return;
            }

            // ==> 처리 <==
            OrderDTO order = new OrderDTO()
            {
                OrderID = int.Parse(lblOrderID.Text),
                ShipVia = Convert.ToInt32(cboShipper.SelectedValue),
                ShippedDate = dtpShippedDate.Value.ToShortDateString(), //ToShortDateString() == ToString("yyyy-MM-dd")
                Freight = Convert.ToDecimal(txtFreight.Text)
            };
            bool result = srv.UpdateOrder(order);
            if (result)
            {
                MessageBox.Show("배송처리가 완료되었습니다.");
                btnSearch.PerformClick();
            }
            else
            {
                MessageBox.Show("배송처리 중 오류가 발생하였습니다.\n다시 시도하여 주십시오.");
            }

        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            // 유효성 체크 
            if (string.IsNullOrWhiteSpace(lblOrderID.Text))
            {
                MessageBox.Show("삭제할 주문 정보를 선택하여 주십시오.");
                return;
            }
            
            if (MessageBox.Show("주문정보를 삭제하시겠습니까?", "삭제확인", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                // Master - Detail 테이블 관계에서 delete는 detail을 삭제하고 master 삭제
                // 자식 테이블 먼저 삭제하고 부모 테이블을 삭제해야 한다.
                // 트랙잭션이 필수로 필요하다.
                bool result = srv.DeleteOrder(int.Parse(lblOrderID.Text));
                if (result)
                {
                    MessageBox.Show("주문이 삭제되었습니다.");
                    btnSearch.PerformClick(); 
                }
                else
                {
                    MessageBox.Show("주문 삭제 중 오류가 발생하였습니다.\n다시 시도하여 주십시오.");
                }
            }
        }

        private void btnExcel_Click(object sender, EventArgs e)
        {
            SaveFileDialog dlg = new SaveFileDialog();
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                saveFileName = dlg.FileName;
                frmPopUp_LoadingAsync pop = new frmPopUp_LoadingAsync(ExcelOrderExport);
                pop.ShowDialog();
            }
        }
        private void ExcelOrderExport()
        {
            // 현재 그리드뷰에 직원 목록 excel export
            List<OrderDTO> list = (List<OrderDTO>)dgvOrder.DataSource;
            // 전체 목록 있는 직원 목록 excel export
            //List<EmployeeDTO> allList = (List<EmployeeDTO>)dgvEmp.DataSource;

            string errMsg = ExcelUtil.ExcelExportListDTO<OrderDTO>(list, saveFileName, "");
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
