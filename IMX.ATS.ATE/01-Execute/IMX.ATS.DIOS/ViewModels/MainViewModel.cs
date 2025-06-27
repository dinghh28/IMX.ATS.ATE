#region << 版 本 注 释 >>
/*----------------------------------------------------------------
 * 版权所有 (c) 2025   保留所有权利。
 * CLR版本：4.0.30319.42000
 * 机器名称：LAPTOP-9Q9TTD5V
 * 公司名称：
 * 命名空间：IMX.ATS.DIOS.ViewModels
 * 唯一标识：3f3b4b4a-51d6-4a39-bdff-7cc1c3befb69
 * 文件名：MainViewModel
 * 当前用户域：LAPTOP-9Q9TTD5V
 * 
 * 创建者：58274
 * 创建时间：2025/6/26 16:15:52
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

using GalaSoft.MvvmLight.Command;
using H.WPF.Framework;
using IMX.ATE.Common;
using IMX.DB;
using IMX.WPF.Resource;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace IMX.ATS.DIOS
{
    public class MainViewModel : WindowViewModelBaseEx
    {
        #region 公共属性
        /// <summary>
        /// 软件版本信息
        /// </summary>
        public string SoftwareVersion => SysteamInfo.SoftwareVersion;

        private Visibility leftbtnvis = Visibility.Collapsed;
        /// <summary>
        /// 左侧导航按钮显示
        /// </summary>
        public Visibility LeftBtnVis
        {
            get => leftbtnvis;
            set => Set(nameof(LeftBtnVis), ref leftbtnvis, value);
        }

        private Visibility rightbtnvis = Visibility.Collapsed;
        /// <summary>
        /// 右侧导航按钮显示
        /// </summary>
        public Visibility RightBtnVis
        {
            get => rightbtnvis;
            set => Set(nameof(RightBtnVis), ref rightbtnvis, value);
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
        #region 界面绑定属性
        #endregion

        #region 界面绑定指令
        public RelayCommand<object> Change => new RelayCommand<object>((obj) => 
        {
            ChangePage(obj.ToString());
        });


        #endregion

        #endregion

        #region 公共方法
        public void ChangePage(string name) 
        {
            switch (name)
            {
                case "Project":
                    MainContent = ContentControlManager.GetControl<TestProjectItemView>(((ViewModelLocator)Application.Current.FindResource("Locator")).Project);
                    break;
                case "Item":
                    MainContent = ContentControlManager.GetControl<TestItemView>(((ViewModelLocator)Application.Current.FindResource("Locator")).TestItem);
                    break;
                default:
                    break;
            }
        }
        #endregion

        #region 私有变量
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

            WindowLeftDown_MoveEvent.LeftDown_MoveEventRegister(win);
            WindowMax.Execute(win);
           
            if (!DBOperate.Default.IsInitOK)
            {
                DBOperate.Default.Init();
            }

            MainContent = ContentControlManager.GetControl<TestProjectItemView>(((ViewModelLocator)Application.Current.FindResource("Locator")).Project);

            //base.WindowLoadedExecute(obj);
        }

        protected override void WindowClosedExecute(object obj)
        {
            base.WindowClosedExecute(obj);
        }
        #endregion


        #region 构造方法
        public MainViewModel() { }
        #endregion

    }
}
