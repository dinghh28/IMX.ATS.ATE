using H.Maths.Encryption.AES;
using IMX.DB.Model;
using Newtonsoft.Json;
using Super.Zoo.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using MessageBox = System.Windows.Forms.MessageBox;
using MessageBoxButtons = System.Windows.Forms.MessageBoxButtons;
using MessageBoxIcon = System.Windows.Forms.MessageBoxIcon;

namespace IMX.ATS.UserManage
{
    internal class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            //MessageBox.Show("进入Main函数体了（参数：" + args.Length + "）");
#if DEBUG
            
           var info = JsonConvert.DeserializeObject<UserInfo>("""{"UserName":"admin","Password":null,"Privilege":15,"Id":1,"CreateTime":"2024-09-03T18:13:07.1761652+08:00","UpdateTime":"0001-01-01T00:00:00","IsDeleted":false,"Sort":0}""");

#else

            if (args.Length < 1)
            {
                MessageBox.Show("进入Main函数体了（参数不可为空）");
                return;
            }

            string arg = AES.Decrypt(args[0], "VXNlck1hbmFnZQ==");
            var info = JsonConvert.DeserializeObject<UserInfo>(arg);
#endif


            if (info == null)
            {
                MessageBox.Show("当前用户权限异常，无法使用该功能");
                return;
            }

            GlobalModel.UserInfo = info;

            string strProcessName = Process.GetCurrentProcess().ProcessName;
            //检查进程是否已经启动，已经启动则退出程序，显示已启动的程序。 
            if (Process.GetProcessesByName(strProcessName).Length > 1)
            {
                RaiseOtherProcess();
                Application.Current.Shutdown();
                return;
            }
            else
            {
                //MessageBox.Show("进入Main函数体了（参数：" + args.Length + "）");
                var app = new App();
                app.InitializeComponent();
                //CustomApplication app = new CustomApplication();
                app.Run();
            }
            ////方法一 打开指定窗口
            ////MainWindow window = new MainWindow();
            ////var app = new App();
            ////app.Run(window);
            ////方法二 程序设置的那个窗口就打开那个窗口
            ////var app = new App();
            ////app.InitializeComponent();
            ////方法三
            //CustomApplication app = new CustomApplication();
            //app.Run();
        }

        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);
        [DllImport("user32.dll")]
        private static extern bool ShowWindowAsync(IntPtr hWnd, int nCmdShow);
        [DllImport("user32.dll")]
        private static extern bool IsIconic(IntPtr hWnd);
        private const int SW_RESTORE = 9;

        /// <summary>
        /// 激活已打开窗口
        /// </summary>
        public static void RaiseOtherProcess()
        {
            Process proc = Process.GetCurrentProcess();
            foreach (Process otherProc in
                Process.GetProcessesByName(Process.GetCurrentProcess().ProcessName))
            {
                if (proc.Id != otherProc.Id)
                {
                    IntPtr hWnd = otherProc.MainWindowHandle;
                    if (IsIconic(hWnd))
                    {
                        ShowWindowAsync(hWnd, 9);
                    }
                    SetForegroundWindow(hWnd);
                    break;
                }
            }
        }
    }

    public class CustomApplication : Application
    {
        private Mutex mutex;

        public CustomApplication()
        {
            System.AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            DispatcherUnhandledException += App_DispatcherUnhandledException;

            System.Threading.Tasks.TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;

            ResigerGetMessageEvent();
        }

        private void Current_Exit(object sender, ExitEventArgs e)
        {
            mutex?.Close();
            mutex?.Dispose();
        }

        private void TaskScheduler_UnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            //ResigerGetMessageEvent();
            //MessageBox.Show(e.Exception.GetMessage(), "软件异常 - 异步线程", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //ExceptionExtends.ResetGetMessageEvent();
            return;
        }

        private void App_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            ResigerGetMessageEvent();
            MessageBox.Show(e.Exception.GetMessage(), "软件异常 - UI线程", MessageBoxButtons.OK, MessageBoxIcon.Error);
            ExceptionExtends.ResetGetMessageEvent();
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            ResigerGetMessageEvent();
            MessageBox.Show((e.ExceptionObject as Exception).GetMessage(), "软件异常 - 非UI线程", MessageBoxButtons.OK, MessageBoxIcon.Error);
            ExceptionExtends.ResetGetMessageEvent();
        }

        private static void ResigerGetMessageEvent()
        {
            ExceptionExtends.ResigerGetMessageEvent(
                ex =>
                $"<| 异常方法 |> {ex.TargetSite}{Environment.NewLine}" +
                $"<| 异常来源 |> {ex.Source}{Environment.NewLine}" +
                $"<| 异常类型 |> {ex.GetType().Name}{Environment.NewLine}" +
                $"<| 异常信息 |> {ex.Message}{Environment.NewLine}" +
                $"<| 堆栈调用 |> {Environment.NewLine}{ex.StackTrace}"
            );
        }

        //StartupEventArgs     引用：System.Windows
        protected override void OnStartup(StartupEventArgs e)
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            string assemblyName = assembly.GetName().Name;

            mutex = new Mutex(true, assemblyName, out bool createdNew);

            if (!createdNew)
            {
                // 如果已经存在另一个实例，则退出
                MessageBox.Show("应用程序已经在运行中,无法再次打开！");
                Current.Shutdown();
                return;
            }

            // 注册退出事件，确保在退出时释放互斥锁
            Current.Exit += Current_Exit;

            UserMainView window = new UserMainView(e.Args);
            window.Show();

            base.OnStartup(e);
        }
    }
}
