using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WinNorthWind_Project
{
    static class Program
    {
        // ====================== Log 활용 =============================
        // 개발시에는 주석처리하여 개발
        // LoggingUtility.cs 프로젝트에 복사하여 붙혀넣기
        // 로그를 남기고자 하는 폼에 가서 frm___.Load 에
        //      예시) Program.Log.WriteInfo("frmOrder 공통코드 조회 : " + list.Count);  작성
        static LoggingUtility _log = new LoggingUtility("WinNorthWind_Project", log4net.Core.Level.Info, 30);
        static public LoggingUtility Log { get { return _log; } }

        // =============== DAO 로그 작성 시 (OrderDAO 확인) ===============
        // 1. DAO에 Log4net nuget 설치 -> 참조추가 
        // 2. LoggingUtility.cs 복사하여 붙혀넣기
        // 3. static LoggingUtility _log = new LoggingUtility("WinNorthWind_Project", log4net.Core.Level.Info, 30); 작성
        // 4. public List<ProductInfoDTO> GetProductAllList() 참고 
        // 5. 아래 [로그 찍기 위한 코드], [로그 찍기 위한 이벤트 작성]
        // ==============================================================


        /// <summary>
        /// 해당 애플리케이션의 주 진입점입니다.
        /// </summary>
        [STAThread]
        static void Main()
        {   // ================================ 로그를 찍기 위한 코드 ===============================
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException; 
            Application.ThreadException += Application_ThreadException; 
            // ====================================================================================
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new frmMain());
        }


        //  =====================================  로그를 찍기 위한 이벤트  =====================================
        private static void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            Program.Log.WriteError("처리되지 않은 오류발생", e.Exception);
            MessageBox.Show(e.Exception.Message);
        }
        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e) 
        {
            Program.Log.WriteError("처리되지 않은 오류발생", (Exception)e.ExceptionObject);
            MessageBox.Show(((Exception)e.ExceptionObject).Message);
        }
        // =====================================================================================================
    }
}
