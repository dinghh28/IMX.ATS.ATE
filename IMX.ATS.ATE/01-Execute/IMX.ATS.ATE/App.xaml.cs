﻿using Super.Zoo.Framework;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Threading;
using MessageBox = System.Windows.Forms.MessageBox;

namespace IMX.ATS.ATE
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : System.Windows.Application
    {
        public App()
        {
            System.AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            DispatcherUnhandledException += App_DispatcherUnhandledException;

            System.Threading.Tasks.TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;

            ResigerGetMessageEvent();
        }

        private Mutex mutex;

        protected override void OnStartup(StartupEventArgs e)
        {
            //const string mutexName = "IMX.AutomationTestSystem";
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

            // 执行应用程序的初始化逻辑
            base.OnStartup(e);
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
            //ResigerGetMessageEvent();
            MessageBox.Show(e.Exception.GetMessage(), "软件异常 - UI线程", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //ExceptionExtends.ResetGetMessageEvent();
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            // ResigerGetMessageEvent();
            MessageBox.Show((e.ExceptionObject as Exception).GetMessage(), "软件异常 - 非UI线程", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //ExceptionExtends.ResetGetMessageEvent();
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
    }
}
