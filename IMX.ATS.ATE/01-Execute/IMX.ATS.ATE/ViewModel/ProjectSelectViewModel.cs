#region << 版 本 注 释 >>
/*----------------------------------------------------------------
 * 版权所有 (c) 2024   保留所有权利。
 * CLR版本：4.0.30319.42000
 * 机器名称：LAPTOP-9Q9TTD5V
 * 公司名称：
 * 命名空间：IMX.ATS.ATEConfig.ViewModel
 * 唯一标识：af41d760-0f35-4734-89dd-c4c59ae464cd
 * 文件名：ProjectSelectViewModel
 * 当前用户域：LAPTOP-9Q9TTD5V
 * 
 * 创建者：58274
 * 创建时间：2024/9/13 14:47:03
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

using H.WPF.Framework;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Super.Zoo.Framework;
using System.Windows;
using GalaSoft.MvvmLight.CommandWpf;
using IMX.DB.Model;
using IMX.DB;
using IMX.WPF.Resource;
using System.Threading;

namespace IMX.ATS.ATE
{
    public class ProjectSelectViewModel : ExtendViewModelBase
    {

        #region 公共属性

        #region 界面绑定属性
        private ObservableCollection<ProjectData> searchprojectinfos = new ObservableCollection<ProjectData>();
        /// <summary>
        /// 检索获取的项目信息
        /// </summary>
        public ObservableCollection<ProjectData> SearchProjectInfos
        {
            get => searchprojectinfos;
            set => Set(nameof(SearchProjectInfos), ref searchprojectinfos, value);
        }

        private ProjectData selectedinfo;
        /// <summary>
        /// 当前选择项目
        /// </summary>
        public ProjectData SelectedInfo
        {
            get => selectedinfo;
            set => Set(nameof(SelectedInfo), ref selectedinfo, value);
        }

        private string searchstr;
        /// <summary>
        /// 检索字符串
        /// </summary>
        public string SearchStr
        {
            get => searchstr;
            set => Set(nameof(SearchStr), ref searchstr, value);
        }

        private int selectedinfoindex = -1;
        /// <summary>
        /// 当前选中的项目
        /// </summary>
        public int SelectedInfoIndex
        {
            get => selectedinfoindex;
            set
            {
                if (Set(nameof(SelectedInfoIndex), ref selectedinfoindex, value))
                {
                    if (value == -1)
                    {
                        SelectedInfo = null;
                        return;
                    }
                    SelectedInfo = SearchProjectInfos[value];
                }
            }
        }

        #endregion

        #region 界面绑定指令
        /// <summary>
        /// 页面跳转指令
        /// </summary>
        public RelayCommand<object> ReturnConfig => new RelayCommand<object>((object obj) =>
        {

            if (SelectedInfoIndex == -1)
            {
                MessageBox.Show("请选择项目");
                return;
            }
            

            if (string.IsNullOrEmpty(SelectedInfo.Tag))
            {
                MessageBox.Show("请选择项目");
                return;
            }

            var result = DBOperate.Default.GetProgramme(SelectedInfo.Info.Id);

            if (!result)
            {
                MessageBox.Show("项目试验阶段获取失败");
                return;
            }
            if (result.Data.Test_FlowNames == null || result.Data.Test_FlowNames.Count<1)
            {
                MessageBox.Show("当前项目未配置试验项或未完成试验方案配置，无法进行自动测试");
                return;
            }
            MainViewModel viewmodel = ((ViewModelLocator)Application.Current.FindResource("Locator")).Main;
            
            viewmodel.SelectedProductName = SelectedInfo.Info.ProjectName;
            viewmodel.ProcessInfos.Clear();
            viewmodel.IsSelectAll = false;
            Thread.Sleep(10);
            for (int i = 0; i < result.Data.Test_FlowNames?.Count; i++)
            {
                viewmodel.ProcessInfos.Add(new ProcessInfo { ProcessName = result.Data.Test_FlowNames[i] });
            }
            GlobalModel.TestOff_FlowNames.Clear();
            GlobalModel.TestOff_FlowNames = result.Data.TestOff_FlowNames;
            GlobalModel.ProjectInfo = SelectedInfo.Info;

            Win.Close();
        });

        /// <summary>
        /// 项目信息检索指令
        /// </summary>
        public RelayCommand Search => new RelayCommand(() =>
        {
            SearchProjectInfos.Clear();
            selectedinfoindex = -1;
            List<ProjectData> data = new List<ProjectData>();

            if (string.IsNullOrEmpty(SearchStr))
            {
                data = LsProductInfos;
            }
            else
            {
                data = LsProductInfos.FindAll(x => x.Tag.Contains(SearchStr));
            }

            for (int i = 0; i < data.Count; i++)
            {
                SearchProjectInfos.Add(data[i]);
            }
        });
        #endregion

        #endregion

        #region 私有变量
        /// <summary>
        /// 数据库包含所有项目信息
        /// </summary>
        private List<ProjectData> LsProductInfos = new List<ProjectData>();

        /// <summary>
        /// 当前窗口
        /// </summary>
        private Window Win = null;

        #endregion

        #region 私有方法
        #endregion

        #region 保护方法
        protected override void WindowLoadedExecute(object obj)
        {
            //Window win = new MouseLeftButtonDown().MouseLeft(obj);
            //if (win != null) {Win = win;}
            if (!(obj is Window win))
            {
                return;
            }

            //获取当前窗口
            Win = win;
            WindowLeftDown_MoveEvent.LeftDown_MoveEventRegister(Win);

            //Win.MouseLeftButtonDown += (s, e) =>
            //{
            //    if (e.LeftButton == MouseButtonState.Pressed)
            //        Win.DragMove();
            //};

            SearchStr = string.Empty;

            if (!DBOperate.Default.IsInitOK)
            {
                DBOperate.Default.Init();
                DBOperate.Default.UpdateOperator = GlobalModel.UserInfo.UserName;
            }

            DBOperate.Default.SelectedProjectInfo_All().AttachIfSucceed(result =>
            {
                LsProductInfos.Clear();
                result.Data.ForEach(item => { LsProductInfos.Add(new ProjectData { Info = item }); });
            })
                .AttachIfFailed(result =>
                { MessageBox.Show($"{result.Message}", "数据库查询", MessageBoxButton.OK, MessageBoxImage.Error); return; })
               .AttachIfExcepted(result =>
               {
                   MessageBox.Show($"{result.Message}", "数据库查询", MessageBoxButton.OK, MessageBoxImage.Error); return;
               });

            SearchProjectInfos.Clear();
            for (int i = 0; i < LsProductInfos.Count; i++)
            {
                SearchProjectInfos.Add(LsProductInfos[i]);
            }
        }

        //private void Win_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        //{
        //    if (e.LeftButton == MouseButtonState.Pressed)
        //        Win.DragMove();
        //}

        protected override void WindowClosedExecute(object obj)
        {
            WindowLeftDown_MoveEvent.LeftDown_MoveEventUnRegister(Win);

            base.WindowClosedExecute(obj);
        }
        #endregion


        #region 构造方法
        public ProjectSelectViewModel()
        {

        }
        #endregion

    }

    public class ProjectData
    {
        /// <summary>
        /// 项目信息
        /// </summary>
        public Test_ProjectInfo Info { get; set; }

        /// <summary>
        /// 项目显示信息
        /// </summary>
        public string Tag => $"[{Info?.ProjectSN}-{Info?.ProjectName}]";
    }
}
