#region << 版 本 注 释 >>
/*----------------------------------------------------------------
 * 版权所有 (c) 2025   保留所有权利。
 * CLR版本：4.0.30319.42000
 * 机器名称：LAPTOP-9Q9TTD5V
 * 公司名称：
 * 命名空间：IMX.ATS.DBCConfig.ViewModel
 * 唯一标识：0fb2cd23-0c22-4f7c-b104-c3c836f2ecb4
 * 文件名：MainViewModel
 * 当前用户域：LAPTOP-9Q9TTD5V
 * 
 * 创建者：58274
 * 创建时间：2025/3/3 17:53:59
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
using IMX.DB;
using IMX.DB.Model;
using IMX.WPF.Resource;
using Super.Zoo.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace IMX.ATS.DBCConfig
{
    public class MainViewModel : WindowViewModelBaseEx
    {
        #region 公共属性
        private string dbcconfigname;
        /// <summary>
        /// 当前配置项目名称
        /// </summary>
        public string DBCConfigName
        {
            get => dbcconfigname;
            set => Set(nameof(DBCConfigName), ref dbcconfigname, value);
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

        private Visibility messageconfigvisbility;
        /// <summary>
        /// DBC配置显示
        /// </summary>
        public Visibility MessageConfigVisbility
        {
            get => messageconfigvisbility;
            set => Set(nameof(MessageConfigVisbility), ref messageconfigvisbility, value);
        }


        /// <summary>
        /// 当前登陆用户
        /// </summary>
        public string UserName => GlobalModel.UserInfo?.UserName;

        /// <summary>
        /// 软件版本信息
        /// </summary>
        public string SoftwareVersion => SysteamInfo.SoftwareVersion;
        #region 界面绑定属性


        #endregion

        #region 界面绑定指令
        public RelayCommand<object> NavChangeCommand => new RelayCommand<object>(DoNavChanged);
        #endregion

        #endregion

        #region 私有变量

        /// <summary>
        /// 当前窗口
        /// </summary>
        private Window Win = null;

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


            //获取当前窗口
            Win = win;
            WindowLeftDown_MoveEvent.LeftDown_MoveEventRegister(Win);

            if (GlobalModel.IsNew)
            {
                MessageConfigVisbility = Visibility.Collapsed;
                DBCConfigName = "未命名新配置";
            }
            else if (!GlobalModel.Test_DBC.EnableUse)
            {
                MessageConfigVisbility = Visibility.Collapsed;
                DBCConfigName = GlobalModel.Test_DBC.ConfigName;
            }
            else
            {
                MessageConfigVisbility = Visibility.Visible;
                DBCConfigName = GlobalModel.Test_DBC.ConfigName;
                DBOperate.Default.GetFile_ByID(GlobalModel.Test_DBC.DBCFileID)
                    .AttachIfSucceed(result => GlobalModel.Test_DBCFileInfo = result.Data ?? new Test_DBCFileInfo())
                    .AttachIfFailed(result => MessageBox.Show($"DBC文件获取失败：{result.Message}", "DBC通讯配置获取异常"));
            }

            DoNavChanged("DBCConfig");
            base.WindowMaxExecute(obj);
        }

        protected override void WindowClosedExecute(object obj)
        {
            WindowLeftDown_MoveEvent.LeftDown_MoveEventUnRegister(Win);

            base.WindowClosedExecute(obj);
        }
        #endregion


        #region 构造方法
        public MainViewModel() { }
        #endregion

    }
}
