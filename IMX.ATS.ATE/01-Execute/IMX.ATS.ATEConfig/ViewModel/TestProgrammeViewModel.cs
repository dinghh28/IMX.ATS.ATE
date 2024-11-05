#region << 版 本 注 释 >>
/*----------------------------------------------------------------
 * 版权所有 (c) 2024   保留所有权利。
 * CLR版本：4.0.30319.42000
 * 机器名称：LAPTOP-9Q9TTD5V
 * 公司名称：
 * 命名空间：IMX.ATS.ATEConfig.ViewModel
 * 唯一标识：36105c5c-afa3-4c40-ad50-971579bd9e71
 * 文件名：TestProgrammeViewModel
 * 当前用户域：LAPTOP-9Q9TTD5V
 * 
 * 创建者：58274
 * 创建时间：2024/10/28 11:10:07
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
using H.WPF.Framework;
using IMX.DB;
using IMX.DB.Model;
using IMX.Logger;
using Super.Zoo.Framework;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace IMX.ATS.ATEConfig
{
    public class TestProgrammeViewModel : ExtendViewModelBase
    {


        #region 公共属性

        #region 界面绑定属性
        private ObservableCollection<TestProcessNameModel> selectedprocessnames = new ObservableCollection<TestProcessNameModel>();
        /// <summary>
        /// 添加到方案的流程列表
        /// </summary>
        public ObservableCollection<TestProcessNameModel> SelectedProcessNames
        {
            get => selectedprocessnames;
            set => Set(nameof(SelectedProcessNames), ref selectedprocessnames, value);
        }

        private int selectedindex;
        /// <summary>
        /// 当前选择流程Index
        /// </summary>
        public int SelectedIndex
        {
            get => selectedindex;
            set => Set(nameof(SelectedIndex), ref selectedindex, value);
        }

        #endregion

        #region 界面绑定指令
        /// <summary>
        /// 添加流程
        /// </summary>
        public RelayCommand AddProcess => new RelayCommand(Add);

        /// <summary>
        /// 删除流程
        /// </summary>
        public RelayCommand DeletProcess => new RelayCommand(Delet);

        /// <summary>
        /// 保存流程
        /// </summary>
        public RelayCommand SaveProgramme => new RelayCommand(Save);
        #endregion

        public ObservableCollection<string> ProcessNames { get; set; } =  new ObservableCollection<string>();

        #endregion

        #region 私有变量
        #endregion

        #region 私有方法
        private void Add()
        {
            SelectedProcessNames.Add(new TestProcessNameModel 
            {
                ProcessNames = ProcessNames
            });
        }

        private void Delet()
        {
            if (SelectedIndex == -1)
            {
                MessageBox.Show("请选择要删除的流程","流程删除异常");
                return;
            }

            try
            {
                SelectedProcessNames.RemoveAt(SelectedIndex);
            }
            catch (Exception ex)
            {
                SuperDHHLoggerManager.Exception( LoggerType.FROMLOG, nameof(TestProgrammeViewModel), nameof(Delet), ex);
                MessageBox.Show(ex.GetMessage(), "流程删除异常");
                return;
            }
        }

        private void Save() 
        {
            if (MessageBox.Show("是否要保存当前方案","方案保存", MessageBoxButton.OKCancel)== MessageBoxResult.Cancel)
            {
                return;
            }

            try
            {
                if (SelectedProcessNames.Count < 1)
                {
                    MessageBox.Show("请添加试验流程后再进行保存","方案保存失败");
                    return;
                }

                Test_Programme programme = new Test_Programme();
                programme.ProjectID = GlobalModel.Test_ProjectInfo.Id;
                programme.Test_FlowNames = new List<string>();
                for (int i = 0; i < SelectedProcessNames.Count(); i++)
                {
                    programme.Test_FlowNames.Add(SelectedProcessNames[i].SelectedName);
                }
                programme.UpdateOperator = GlobalModel.UserInfo.UserName;

                DBOperate.Default.UpdateProgramme(programme)
                    .AttachIfFailed(result => { MessageBox.Show(result.Message, "方案保存失败"); })
                    .AttachIfSucceed(result => 
                    {
                        MessageBox.Show(result.Message, "方案保存");
                    });
            }
            catch (Exception ex)
            {
                SuperDHHLoggerManager.Exception(LoggerType.FROMLOG, nameof(TestProgrammeViewModel), nameof(Save), ex);
                MessageBox.Show(ex.GetMessage(),"方案保存异常");
            }

            



        }
        #endregion

        #region 保护方法
        protected override void WindowLoadedExecute(object obj)
        {
            ProcessNames.Clear();
            DBOperate.Default.GetProcessName(GlobalModel.Test_ProjectInfo.Id)
                .AttachIfSucceed(result => {
                    for (int i = 0; i < result.Data.Count; i++)
                    {
                        ProcessNames.Add(result.Data[i]);
                    }
                }).AttachIfFailed(result => 
                {
                    MessageBox.Show($"无法获取当前项目流程：\r\n{result.Message}","项目流程获取异常");
                });
            //base.WindowLoadedExecute(obj);
        }

        protected override void WindowClosedExecute(object obj)
        {
            base.WindowClosedExecute(obj);
        }
        #endregion


        #region 构造方法
        public TestProgrammeViewModel() { }
        #endregion
    }

    public class TestProcessNameModel : ViewModelBase 
    {
        private string selectedname;

        /// <summary>
        /// 当前选择的流程名称
        /// </summary>
        public string SelectedName
        {
            get => selectedname;
            set => Set(nameof(SelectedName), ref selectedname, value);
        }

        private ObservableCollection<string> processnames;
        /// <summary>
        /// 流程列表
        /// </summary>
        public ObservableCollection<string> ProcessNames
        {
            get => processnames;
            set => Set(nameof(ProcessNames), ref processnames, value);
        }

    }
}
