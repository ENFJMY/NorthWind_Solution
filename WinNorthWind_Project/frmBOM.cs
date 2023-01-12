using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WinNorthWind_Project.Services;
using NorthWindDTO;
using WinNorthWind_Project.Utils;

namespace WinNorthWind_Project
{
    public partial class frmBOM : Form
    {
        ItemBOMService srv = null;
        public frmBOM()
        {
            InitializeComponent();
        }

        private void frmBOM_Load(object sender, EventArgs e)
        {
            srv = new ItemBOMService();

            CommonUtil.ComboBinding(cboProduct, srv.GetProductList(), "BOM");
            srv.GetProductList();
        }

        private void btnSearch2_Click(object sender, EventArgs e)
        {
            // 선택된 완제품ID를 DB에 전달해서 BOM정보를 조회해서 DataGridView에 바인딩
            string itemCD = cboProduct.SelectedValue.ToString();
            List<ItemBOMDTO> list = srv.GetBOMList(itemCD);

            // DataGridView 바인딩
            dataGridView2.DataSource = list;

            // TreeView 컨트롤에 바인딩
            treeView1.Nodes.Clear(); // 초기화 하고 TreeView 그리기

            var level0 = list.FindAll((b) => b.levels == 0);
            foreach (ItemBOMDTO bom0 in level0)
            {
                TreeNode node = new TreeNode(bom0.ITEM_NM2); // 루트 노드(대메뉴) 출력

                var level1 = list.FindAll((b) => b.levels == 1 && b.sortOrder.Contains(bom0.ITEM_CD)).OrderBy((b) => b.sortOrder);

                foreach (ItemBOMDTO bom1 in level1)
                {
                    TreeNode node1 = new TreeNode(bom1.ITEM_NM2); // 자식1 노드 
                    var level2 = list.FindAll((b) => b.levels == 2 && b.sortOrder.Contains(bom1.ITEM_CD)).OrderBy((b) => b.sortOrder);

                    foreach (ItemBOMDTO bom2 in level2)
                    {
                        TreeNode node2 = new TreeNode(bom2.ITEM_NM2); // 자식2 노트

                        node1.Nodes.Add(node2);
                    }
                    node.Nodes.Add(node1);
                }
                treeView1.Nodes.Add(node);
            }
            treeView1.ExpandAll();
        }
    }
}
