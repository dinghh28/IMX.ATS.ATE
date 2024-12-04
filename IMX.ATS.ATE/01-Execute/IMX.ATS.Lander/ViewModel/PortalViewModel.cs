#region << 版 本 注 释 >>
/*----------------------------------------------------------------
 * 版权所有 (c) $year$ $registeredorganization$  保留所有权利。
 * CLR版本：$clrversion$
 * 机器名称：$machinename$
 * 公司名称：$registeredorganization$
 * 命名空间：$rootnamespace$
 * 唯一标识：$guid10$
 * 文件名：$safeitemname$
 * 当前用户域：$userdomain$
 * 
 * 创建者：$username$
 * 创建时间：$time$
 * 版本：V1.0.0
 * 描述：
 *
 * ----------------------------------------------------------------
 * 修改人：
 * 时间：
 * 修改说明：
 *
 * 版本：V1.0.1
 *----------------------------------------------------------------*/
#endregion << 版 本 注 释 >>

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using H.Maths.Encryption.AES;
using H.WPF.Framework;
using IMX.ATS.Lander.Common;
using IMX.Logger;
using IMX.WPF.Resource;
using Newtonsoft.Json;
using Super.Zoo.Framework;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;

namespace IMX.ATS.Lander
{
    /// <summary>
    /// 门户界面：功能模块选择界面
    /// </summary>
    public class PortalViewModel : ExtendViewModelBase
    {
        #region 公共属性

        #region 界面绑定属性
        private ObservableCollection<UserPrivilege> privilege = new ObservableCollection<UserPrivilege>();
        /// <summary>
        /// 当前用户拥有权限
        /// </summary>
        public ObservableCollection<UserPrivilege> Privilege
        {
            get => privilege;
            set => Set(nameof(Privilege), ref privilege, value);
        }

        private ObservableCollection<FunModule> funModules = new ObservableCollection<FunModule>() 
        {
           //new FunModule{ FunModuleName = "项目信息配置", ViewmodeName = "ProjectConfig"},
        };

        /// <summary>
        /// 门户模块
        /// </summary>
        public ObservableCollection<FunModule> FunModules
        {
            get => funModules;
            set => Set(nameof(FunModules), ref funModules, value);
        }


        #endregion

        #region 界面绑定指令
        #endregion

        #endregion

        #region 私有变量

        private Window Win = null;

        //public Dictionary<string, bool> dicViewopen = new Dictionary<string, bool>()
        //{
        //    //{ "ProjectConfig.PCProjectSelectView", false },
        //    //{ "UserManage.UserMainView",false },
        //    //{ "TestOperate.TODeviceInitView", false },
        //    //{ "DataOperate.ADOMainView", false},
        //    { "ProjectConfig", false },
        //    { "UserManage",false },
        //    { "BIS", false },
        //    { "DIOS", false},
        //};

        private Dictionary<string, string> dicViewname = new Dictionary<string, string>()
        {
            { "ATEConfig", "项目信息配置" },
            { "ATE", "测试管理" },
            { "UserManage", "用户管理" },
            { "DIOS", "数据管理" },
        };


        //private Dictionary<string, string> dicIconURL = new Dictionary<string, string>()
        //{
        //    { "ATEConfig", "软件升级.png" },
        //    { "UserManage", "无权限.png" },
        //    { "ATE", "错误.png" },
        //    { "DIOS", "统计.png" },
        //};

        /// <summary>
        /// 软件AESKey字典
        /// </summary>
        private Dictionary<string, string> dicAESKey = new Dictionary<string, string>() 
        {
            { "ATEConfig", "QVRFQ29uZmln" },
            { "UserManage", "VXNlck1hbmFnZQ==" },
            { "ATE", "QVRF" },
            { "DIOS", "" },
        };

        private Dictionary<string, Visibility> dicVisibility = new Dictionary<string, Visibility>()
        {
            //{ "ProjectConfig.PCProjectSelectView", (GlobalModel.UserInfo.Privilege & 2) == 2? Visibility.Visible : Visibility.Collapsed },
            //{ "UserManage.UserMainView",Visibility.Visible },// (GlobalModel.UserInfo.Privilege & 8) == 8 ? Visibility.Visible : Visibility.Collapsed },
            //{ "TestOperate.TODeviceInitView", Visibility.Visible },
            //{ "DataOperate.ADOMainView",(GlobalModel.UserInfo.Privilege & 4) == 4? Visibility.Visible : Visibility.Collapsed },

            { "ATEConfig", (GlobalModel.UserInfo.Privilege & 2) == 2? Visibility.Visible : Visibility.Collapsed },
            { "UserManage",Visibility.Visible },// (GlobalModel.UserInfo.Privilege & 8) == 8 ? Visibility.Visible : Visibility.Collapsed },
            { "ATE", Visibility.Visible },
            { "DIOS",(GlobalModel.UserInfo.Privilege & 4) == 4? Visibility.Visible : Visibility.Collapsed },
        };


        #endregion

        #region 私有方法
        #endregion

        #region 保护方法
        protected override void WindowLoadedExecute(object obj)
        {
            if (!(obj is Window win))
            {
                return;
            }

            Win = win;
            WindowLeftDown_MoveEvent.LeftDown_MoveEventRegister(Win);
            
            foreach (var item in dicViewname)
            {
                FunModules.Add(new FunModule 
                { 
                    ViewmodeName = item.Key,
                    FunModuleName = item.Value, 
                    FunModuleVisibility = dicVisibility[item.Key], 
                    ExeAESKey = dicAESKey[item.Key],
                    //FunModuleIconURL = $"pack://application:,,,/{SupportConfig.SystemName};component/Resource/Image/{dicIconURL[item.Key]}"
                    /*, OpenFunmodule = new RelayCommand<object>(DoViewChange)*/ 
                });
            }
        }

        //[DllImport("kernel32.dll")]
        //public static extern int WinExec(string programPath, int operType);

        private void DoViewChange(object obj)
        {
            //GC.Collect();
            //GC.WaitForPendingFinalizers();
            //GC.Collect();

            try
            {
                //if (dicViewopen[obj.ToString()]) { MessageBox.Show($"界面已打开，请勿重复打开界面！", "提示", MessageBoxButton.OK, MessageBoxImage.Information); return; }

                if (obj.ToString() == "DataOperate")
                {
                    string pathStr = AppDomain.CurrentDomain.BaseDirectory + string.Format("IMX.ATS.DIOS.exe");
                    //var result = WinExec(pathStr, 7);
                    Process p = Process.Start(pathStr);
                    //dicViewopen[obj.ToString()] = true;
                    return;
                }

                if (obj.ToString() == "UserManage")
                {
                    string pathStr = AppDomain.CurrentDomain.BaseDirectory + string.Format("IMX.ATS.UserManage.exe");
                    string info = """{"UserName":"丁慧慧","Password":null,"Privilege":15,"Id":1}""";
                    //string info = "1111";
                    string arg = AES.Encrypt(info, "VXNlck1hbmFnZQ==");

                    //string enco = AES.Decrypt(arg, "UserManage");

                    //var result = WinExec(pathStr, 7);
                    Process p = Process.Start(pathStr, arg);
                    //dicViewopen[obj.ToString()] = true;
                    return;
                }

                if (obj.ToString() == "ProjectConfig")
                {
                    string pathStr = AppDomain.CurrentDomain.BaseDirectory + string.Format("IMX.ATS.ProjectConfig.exe");
                    string info = """{"UserName":"丁慧慧","Password":null,"Privilege":15,"Id":1}""";
                    //string info = "1111";
                    string arg = AES.Encrypt(info, "UHJvamVjdENvbmZpZw ==");

                    //string enco = AES.Decrypt(arg, "UserManage");

                    //var result = WinExec(pathStr, 7);
                    Process p = Process.Start(pathStr, arg);
                    //dicViewopen[obj.ToString()] = true;
                    return;
                }

                
                //Type win = Type.GetType($"IMX.ATS.BIS.{obj}");
                //Type model = Type.GetType($"IMX.ATS.BIS.{obj}Model");
                //Window Content = ContentControlManager.GetWindow(win, ((ViewModelLocator)Application.Current.FindResource("Locator")).GetModel(model));
                //dicViewopen[obj.ToString()] = true;
                //Content.Show();
                
            }
            catch (Exception ex)
            {
                //dicViewopen[obj.ToString()] = false;
                SuperDHHLoggerManager.Exception(LoggerType.FROMLOG, nameof(PortalViewModel), nameof(DoViewChange), ex);
                MessageBox.Show($"打开功能模块异常:{ex.GetMessage()}");
            }
        }

        protected override void WindowClosedExecute(object obj)
        {
            WindowLeftDown_MoveEvent.LeftDown_MoveEventUnRegister(Win);
            (obj as Window).Close();
            base.WindowClosedExecute(obj);
        }
        #endregion

        #region 构造方法
        public PortalViewModel()
        {
            
        }
        #endregion
    }

    /// <summary>
    /// 门户模块
    /// </summary>
    public class FunModule : ViewModelBase
    {
        private string viewmodeName;

        public string ViewmodeName
        {
            get => viewmodeName;
            set => Set(nameof(ViewmodeName), ref viewmodeName, value);
        }

        private string funModuleName;

        /// <summary>
        /// 功能模块名称
        /// </summary>
        public string FunModuleName
        {
            get => funModuleName;
            set => Set(nameof(FunModuleName), ref funModuleName, value);
        }

        /// <summary>
        /// 模块显示
        /// </summary>
        public Visibility FunModuleVisibility { get; set; }

        /// <summary>
        /// 模块图标路径
        /// </summary>
        public string FunModuleIconURL => $"pack://application:,,,/{SupportConfig.SystemName};component/Resource/Image/{ViewmodeName}.png";

        /// <summary>
        /// 功能AES密匙
        /// </summary>
        public string ExeAESKey { get; set; }

        //public RelayCommand<object> OpenFunmodule { get; set; }



        public RelayCommand<object> OpenFunmodule => new RelayCommand<object>((objWindow) =>
        {
            try
            {
                string pathStr = AppDomain.CurrentDomain.BaseDirectory + string.Format($"IMX.ATS.{ViewmodeName}.exe");

                if (ViewmodeName == "DIOS")
                {
                    Process p = Process.Start(pathStr);
                    return;
                }
                else
                {
                    string info = JsonConvert.SerializeObject(GlobalModel.UserInfo);
                    string arg = AES.Encrypt(info, ExeAESKey);
                    Process p = Process.Start(pathStr, arg);
                }



                //string enco = AES.Decrypt(arg, "UserManage");

                //var result = WinExec(pathStr, 7);
                //Process p = Process.Start(pathStr, arg);
                //dicViewopen[obj.ToString()] = true;
                

                //Type win = Type.GetType($"IMX.ATS.BIS.Main.{viewmodeName}");
                //Type model = Type.GetType($"IMX.ATS.BIS.Main.{viewmodeName}");
                //Content = ContentControlManager.GetControl(win, ((ViewModelLocator)Application.Current.FindResource("Locator")).GetModel(model));
            }
            catch (Exception ex)
            {
                SuperDHHLoggerManager.Exception(LoggerType.FROMLOG, nameof(FunModule), nameof(OpenFunmodule), ex);
                MessageBox.Show($"界面切换异常:{ex.GetMessage()}");
            }
        });

    }


    /// <summary>
    /// 用户权限
    /// </summary>
    public class UserPrivilege : ViewModelBase
    {
        private string privilegename;

        /// <summary>
        /// 权限名称
        /// </summary>
        public string PrivilegeName
        {
            get => privilegename;
            set => Set(nameof(PrivilegeName), ref privilegename, value);
        }

        /// <summary>
        /// 权限对应界面字符串
        /// </summary>
        public string PrivilegeTag { get; set; }

        /// <summary>
        /// 权限显示标识
        /// </summary>
        public string PrivilegeIcon { get; set; }
    }
}
