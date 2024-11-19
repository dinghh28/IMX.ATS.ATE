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
using IMX.Function.ViewModel.Model;
using IMX.Logger;
using Super.Zoo.Framework;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media;
using Application = System.Windows.Application;
using MessageBox = System.Windows.Forms.MessageBox;

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

        //private ObservableCollection<TestoffProcessNameModel> epowerOffProcessNames = new ObservableCollection<TestoffProcessNameModel>();
        ///// <summary>
        ///// 添加到方案的流程列表
        ///// </summary>
        //public ObservableCollection<TestoffProcessNameModel> EPowerOffProcessNames
        //{
        //    get => epowerOffProcessNames;
        //    set => Set(nameof(EPowerOffProcessNames), ref epowerOffProcessNames, value);
        //}

        private ObservableCollection<string> programNames = new ObservableCollection<string>();

        /// <summary>
        /// 项目所有试验方案
        /// </summary>
        public ObservableCollection<string> ProgramNames
        {
            get => programNames;
            set => Set(nameof(ProgramNames), ref programNames, value);
        }

        private ObservableCollection<string> selectedprogramName = new ObservableCollection<string>();

        /// <summary>
        /// 项目选择的试验方案
        /// </summary>
        public ObservableCollection<string> SelectedProgramName
        {
            get => selectedprogramName;
            set => Set(nameof(SelectedProgramName), ref selectedprogramName, value);
        }

        private string newProgramName;

        /// <summary>
        /// 另存为方案名
        /// </summary>
        public string NewProgramName
        {
            get => newProgramName;
            set => Set(nameof(NewProgramName), ref newProgramName, value);
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



        #region 下电配置

        private int powerOffIndex;
        /// <summary>
        /// 当前选择下电流程Index
        /// </summary>
        public int PowerOffIndex
        {
            get => powerOffIndex;
            set => Set(nameof(PowerOffIndex), ref powerOffIndex, value);
        }

        private bool isExpanded;
        /// <summary>
        /// 下电配置展开收起
        /// </summary>
        public bool IsExpanded
        {
            get => isExpanded;
            set
            {
                if (Set(nameof(IsExpanded), ref isExpanded, value))
                {
                    ExpanderAngle = IsExpanded ? 0 : 90;
                    ExpanderBrush = IsExpanded ? new SolidColorBrush(Colors.Transparent) : new SolidColorBrush(Colors.Gray);
                }
            }
        }

        private double expanderAngle = 90;
        /// <summary>
        /// 下电配置旋转
        /// </summary>
        public double ExpanderAngle
        {
            get => expanderAngle;
            set => Set(nameof(ExpanderAngle), ref expanderAngle, value);
        }

        private Brush expanderBrush = new SolidColorBrush(Colors.Gray);
        /// <summary>
        /// 下电配置背景色
        /// </summary>
        public Brush ExpanderBrush
        {
            get => expanderBrush;
            set => Set(nameof(ExpanderBrush), ref expanderBrush, value);
        }

        #endregion


        #endregion

        #region 界面绑定指令
        /// <summary>
        /// 添加试验流程
        /// </summary>
        public RelayCommand AddProcess => new RelayCommand(Add);

        /// <summary>
        /// 删除试验流程
        /// </summary>
        public RelayCommand DeletProcess => new RelayCommand(Delet);

        /// <summary>
        /// 上移下电试验流程
        /// </summary>
        public RelayCommand UpProcess => new RelayCommand(Up);

        /// <summary>
        /// 下移下电试验流程
        /// </summary>
        public RelayCommand DownProcess => new RelayCommand(Down);

        /// <summary>
        /// 保存方案
        /// </summary>
        public RelayCommand SaveProgramme => new RelayCommand(Save);

        ///// <summary>
        ///// 另存为方案
        ///// </summary>
        //public RelayCommand SaveAsProgramme => new RelayCommand(SaveAs);


        #endregion

        public ObservableCollection<string> ProcessNames { get; set; } = new ObservableCollection<string>();

        public ObservableCollection<string> EPowerOffProcessNames { get; set; } = new ObservableCollection<string>
        {
            "直流负载下电关机",
            "高压直流源下电关机",
            "交流源下电关机",
            "稳压直流源下电关机",
            "产品通讯卸载",
        };

        #endregion

        #region 私有变量

        private int projectid = -1;

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
                MessageBox.Show("请选择要删除的流程", "流程删除异常");
                return;
            }

            try
            {
                SelectedProcessNames.RemoveAt(SelectedIndex);
            }
            catch (Exception ex)
            {
                SuperDHHLoggerManager.Exception(LoggerType.FROMLOG, nameof(TestProgrammeViewModel), nameof(Delet), ex);
                MessageBox.Show(ex.GetMessage(), "流程删除异常");
                return;
            }
        }

        private void Up()
        {
            if (PowerOffIndex == 0)
            { MessageBox.Show("已到达最高点，无法上移！", "提示！", MessageBoxButtons.OK, MessageBoxIcon.Information); return; }

            EPowerOffProcessNames.Insert(PowerOffIndex - 1, EPowerOffProcessNames[PowerOffIndex]);
            EPowerOffProcessNames.RemoveAt(PowerOffIndex);
            //PowerOffIndex = PowerOffIndex - 1;
        }

        private void Down()
        {
            if (PowerOffIndex == EPowerOffProcessNames.Count - 1)
            { MessageBox.Show("已到达最低点，无法下移!", "提示！", MessageBoxButtons.OK, MessageBoxIcon.Information); return; }
            EPowerOffProcessNames.Insert(PowerOffIndex + 2, EPowerOffProcessNames[PowerOffIndex]);
            EPowerOffProcessNames.RemoveAt(PowerOffIndex);
            //PowerOffIndex = PowerOffIndex + 1;
        }

        private void Save()
        {
            List<string> listProcessNames = new List<string>();
            foreach (var item in SelectedProcessNames)
            {
                listProcessNames.Add(item.SelectedName);
            }

            if (SelectedProcessNames.Count == 0)
            {
                if (MessageBox.Show($"试验方案保存异常：\r\n试验方案流程为空，是否继续保存方案", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Information) == DialogResult.Cancel)
                {
                    return;
                }
            }
            Test_Programme program = new Test_Programme()
            {
                ProjectID = projectid,
                //ProjectName = NewProgramName,
                Test_FlowNames = listProcessNames,
                TestOff_FlowNames = EPowerOffProcessNames.ToList(),
                UpdateOperator = GlobalModel.UserInfo.UserName,
            };
            DBOperate.Default.ExistProgram(projectid)
                .AttachIfSucceed(result =>
                {
                    if (result.Data == null)
                    {
                        DBOperate.Default.InsertTestProgramme(program)
                        .AttachIfSucceed(result =>{ MessageBox.Show($"试验方案保存成功", "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);})
                        .AttachIfFailed(result =>{MessageBox.Show($"试验方案保存失败：\r\n{result.Message}", "失败", MessageBoxButtons.OK, MessageBoxIcon.Information);});
                    }
                    else
                    {
                        DBOperate.Default.UpdateProgram(projectid, listProcessNames, EPowerOffProcessNames.ToList())
                        .AttachIfSucceed(result => { MessageBox.Show($"试验方案保存成功", "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);  })
                        .AttachIfFailed(result => { MessageBox.Show($"试验方案保存失败：\r\n{result.Message}", "失败", MessageBoxButtons.OK, MessageBoxIcon.Information); });
                    }

                });


        }

        //private void SaveAs()
        //{
        //    List<string> listProcessNames = new List<string>();
        //    foreach (var item in SelectedProcessNames)
        //    {
        //        listProcessNames.Add(item.SelectedName);
        //    }

        //    if (SelectedProcessNames.Count == 0)
        //    {
        //        if (MessageBox.Show($"试验方案另存为异常：\r\n试验方案为空，是否继续保存方案", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Information) == DialogResult.Cancel)
        //        {
        //            return;
        //        }
        //    }
        //    Test_Programme program = new Test_Programme()
        //    {
        //        ProjectID = projectid,
        //        ProjectName = NewProgramName,
        //        Test_FlowNames = listProcessNames,
        //        TestOff_FlowNames = EPowerOffProcessNames.ToList(),
        //        UpdateOperator = GlobalModel.UserInfo.UserName,
        //    };

        //    DBOperate.Default.InsertTestProgram(program)
        //        .AttachIfSucceed(result =>
        //        {
        //            MessageBox.Show($"试验方案另存为成功", "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
        //        })
        //        .AttachIfFailed(result =>
        //        {
        //            MessageBox.Show($"试验方案另存为失败：\r\n{result.Message}", "失败", MessageBoxButtons.OK, MessageBoxIcon.Information);
        //        });
        //}

        #endregion

        #region 保护方法
        protected override void WindowLoadedExecute(object obj)
        {
            //MainViewModel viewmodel = ((ViewModelLocator)Application.Current.FindResource("Locator")).Main;
            projectid = GlobalModel.Test_ProjectInfo.Id;
            // = ((ViewModelLocator)System.Windows.Application.Current.FindResource("Locator")).Main.ProjectInfo.Id;
            ProcessNames.Clear();
            DBOperate.Default.GetProcessName(projectid)
            .AttachIfSucceed(result =>
            {
                for (int i = 0; i < result.Data.Count; i++)
                {
                    ProcessNames.Add(result.Data[i]);
                }
            }).AttachIfFailed(result =>
            {
                MessageBox.Show($"无法获取当前项目流程：\r\n{result.Message}", "项目流程获取异常");
            });

            if (!GlobalModel.IsNewProject)
            {
                //获取试验方案阶段
                DBOperate.Default.GetProgrammeName(projectid)
                   .AttachIfSucceed(result =>
                   {
                       if (result.Data != null)
                       {
                           Test_Programme test_Programme = result.Data;
                           if (test_Programme.Test_FlowNames.Count > 0)
                           {
                               SelectedProcessNames.Clear();
                               for (int i = 0; i < test_Programme.Test_FlowNames.Count; i++)
                               {
                                   SelectedProcessNames.Add(new TestProcessNameModel
                                   {
                                       SelectedName = result.Data.Test_FlowNames[i],
                                       ProcessNames = ProcessNames
                                   });
                               }
                           }
                           if (test_Programme.TestOff_FlowNames.Count > 0)
                           {
                               EPowerOffProcessNames.Clear();
                               for (int i = 0; i < test_Programme.TestOff_FlowNames.Count; i++)
                               {
                                   EPowerOffProcessNames.Add(test_Programme.TestOff_FlowNames[i]);
                               }
                           }
                       }

                   }).AttachIfFailed(result =>
                   {
                       MessageBox.Show($"无法获取当前项目阶段：\r\n{result.Message}", "项目阶段获取异常");
                   });

                ////获取下电试验方案阶段
                //DBOperate.Default.GetOffProgrammeName(GlobalModel.Test_ProjectInfo.Id)
                // .AttachIfSucceed(result =>
                // {
                //     if (result.Data.Count > 0)
                //     {
                //         EPowerOffProcessNames.Clear();
                //         for (int i = 0; i < result.Data.Count; i++)
                //         {
                //             EPowerOffProcessNames.Add(result.Data[i]);
                //         }
                //     }

                // }).AttachIfFailed(result =>
                // {
                //     MessageBox.Show($"无法获取下电试验阶段：\r\n{result.Message}", "下电试验阶段获取异常");
                // });

            }

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

    public class TestoffProcessNameModel : ViewModelBase
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
        public ObservableCollection<string> ProcessNames = new ObservableCollection<string>
        {
            "直流负载下电关机",
            "高压直流源下电关机",
            "交流源下电关机",
            "稳压直流源下电关机",
            "产品通讯卸载",
        };

    }

}
