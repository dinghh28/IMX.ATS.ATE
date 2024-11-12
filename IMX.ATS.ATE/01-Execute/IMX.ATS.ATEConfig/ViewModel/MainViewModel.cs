#region << 版 本 注 释 >>
/*----------------------------------------------------------------
 * 版权所有 (c) 2024   保留所有权利。
 * CLR版本：4.0.30319.42000
 * 机器名称：LAPTOP-9Q9TTD5V
 * 公司名称：
 * 命名空间：IMX.ATS.BIS.ProjectConfig.ViewModel
 * 唯一标识：e3e9a201-0e38-4706-842a-4988b1bd0a66
 * 文件名：MainViewModel
 * 当前用户域：LAPTOP-9Q9TTD5V
 * 
 * 创建者：58274
 * 创建时间：2024/9/13 11:25:43
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

using GalaSoft.MvvmLight.CommandWpf;
using H.WPF.Framework;
using IMX.ATE.Common;
using IMX.DB.Model;
using IMX.WPF.Resource;
using Super.Zoo.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace IMX.ATS.ATEConfig
{
    public class MainViewModel : WindowViewModelBaseEx
    {


        #region 公共属性

        #region 界面绑定属性
        private string projectname;
        /// <summary>
        /// 当前配置项目名称
        /// </summary>
        public string ProjectName
        {
            get => projectname;
            set => Set(nameof(ProjectName), ref projectname, value);
        }

        private FrameworkElement _mainContent;
        /// <summary>
        /// 功能界面
        /// </summary>
        public FrameworkElement MainContent
        {
            get => _mainContent;
            set => Set(nameof(MainContent), ref _mainContent, value);
        }

        private Visibility dbcconfigvisbility;
        /// <summary>
        /// DBC配置显示
        /// </summary>
        public Visibility DBCConfigVisbility
        {
            get => dbcconfigvisbility;
            set => Set(nameof(DBCConfigVisbility), ref dbcconfigvisbility, value);
        }

        private Visibility flowconfigvisbility;
        /// <summary>
        /// 试验步骤配置显示
        /// </summary>
        public Visibility FlowConfigVisbility
        {
            get => flowconfigvisbility;
            set => Set(nameof(FlowConfigVisbility), ref flowconfigvisbility, value);
        }

        /// <summary>
        /// 当前登陆用户
        /// </summary>
        public string UserName=>GlobalModel.UserInfo?.UserName;

        /// <summary>
        /// 软件版本信息
        /// </summary>
        public string SoftwareVersion => SysteamInfo.SoftwareVersion;
        #endregion

        #region 界面绑定指令

        public RelayCommand<object> NavChangeCommand => new RelayCommand<object>(DoNavChanged);

        //public RelayCommand<object> WindowMin => new RelayCommand<object>((object o) =>
        //{
        //    if (!(o is Window win))
        //    {
        //        return;
        //    }

        //    win.WindowState = WindowState.Minimized;
        //});

        //public RelayCommand<object> WindowMax => new RelayCommand<object>((object o) =>
        //{
        //    if (!(o is Window win))
        //    {
        //        return;
        //    }

        //    if (win.WindowState == WindowState.Normal)
        //    {
        //        win.WindowState = WindowState.Maximized;
        //        win.Width = 900;
        //        win.Height = 600;
        //    }
        //    else
        //    {
        //        win.WindowState = WindowState.Normal;
        //        win.MaxHeight = SystemParameters.WorkArea.Size.Height;
        //        win.MaxWidth = SystemParameters.WorkArea.Size.Width;
        //    }
        //    //win.WindowState = win.WindowState == WindowState.Normal ? WindowState.Maximized : WindowState.Normal;
        //});
        #endregion

        /// <summary>
        /// 是否为新建项目
        /// </summary>
        public bool IsNewProject { get; set; } = false;

        /// <summary>
        /// 项目信息
        /// </summary>
        public Test_ProjectInfo ProjectInfo { get; set; } = null;

        /// <summary>
        /// DBC文件信息
        /// </summary>
        public Test_DBCFileInfo DBCFileInfo { get; set; } = null;

        /// <summary>
        /// DBC配置信息：下发/上报指令
        /// </summary>
        public Test_DBCConfig DBCConfig { get; set; } = null;

        ///// <summary>
        ///// 试验流程
        ///// </summary>
        //public Test_Function FunctionInfo { get; set; } = null;
        #endregion

        #region 私有变量
        private string LastView = string.Empty;
        #endregion

        #region 私有方法
        /// <summary>
        /// 导航功能界面切换
        /// </summary>
        /// <param name="obj">功能界面名称</param>
        /// <exception cref="NotImplementedException"></exception>
        public void DoNavChanged(object obj)
        {
            if (LastView.Equals(obj))
            {
                return;
            }

            // TODO 此处需增加方案配置内容的判断：如设备初始化、试验循环保存等操作

            try
            {

                Type win = Type.GetType($"{SupportConfig.SystemName}.{obj}View");
                Type model = Type.GetType($"{SupportConfig.SystemName}.{obj}ViewModel");
                MainContent = ContentControlManager.GetControl(win, ((ViewModelLocator)Application.Current.FindResource("Locator")).GetModel(model));
            }
            catch (Exception ex)
            {
                MessageBox.Show($"界面跳转失败:{ex.GetMessage()}");
                return;
            }

            LastView = obj.ToString();
        }
        #endregion

        #region 保护方法
        protected override void WindowLoadedExecute(object obj)
        {
            if (!(obj is Window win))
            {
                return;
            }

            WindowLeftDown_MoveEvent.LeftDown_MoveEventRegister(win);
            //base.WindowLoadedExecute(obj);
        }

        protected override void WindowClosedExecute(object obj)
        {
            if (!(obj is Window win))
            {
                return;
            }

            WindowLeftDown_MoveEvent.LeftDown_MoveEventUnRegister(win);

            base.WindowClosedExecute(obj);
        }
        #endregion


        #region 构造方法
        public MainViewModel() { }
        #endregion

    }
}
