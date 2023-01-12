using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WinNorthWind_Project
{
    public partial class frmPopUp_LoadingAsync : Form
    {
        public Action Worker { get; set; } // 2. property 추가
        public frmPopUp_LoadingAsync(Action worker) // 1. (Action worker) 추가
        {
            InitializeComponent();
            this.Worker = worker; // 3. property 선언
        }

        private void frmPopUp_LoadingAsync_Load(object sender, EventArgs e) // 4. 로드 이벤트 생성
        {
            Task t1 = Task.Factory.StartNew(this.Worker); 
            t1.ContinueWith((t) => this.Close(), TaskScheduler.FromCurrentSynchronizationContext()); // 로딩이 끝나면 팝업창을 닫는다.
        }
    }
}
