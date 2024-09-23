#region << 版 本 注 释 >>
/*----------------------------------------------------------------
 * 版权所有 (c) 2024   保留所有权利。
 * CLR版本：4.0.30319.42000
 * 机器名称：LAPTOP-9Q9TTD5V
 * 公司名称：
 * 命名空间：IMX.ATS.ATE.ViewModel
 * 唯一标识：445796a9-744d-4692-bef1-06b3e6faec46
 * 文件名：MainViewModel
 * 当前用户域：LAPTOP-9Q9TTD5V
 * 
 * 创建者：58274
 * 创建时间：2024/9/9 16:19:41
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
using IMX.Common;
using IMX.WPF.Resource;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace IMX.ATS.ATE
{
    public class MainViewModel : ExtendViewModelBase
    {

        #region 公共属性

        #region 界面绑定属性

        //private bool enabletestinfochange = true;
        ///// <summary>
        ///// 试验信息允许变更使能
        ///// </summary>
        //public bool EnableTestInfoChange
        //{
        //    get => enabletestinfochange;
        //    set => Set(nameof(EnableTestInfoChange), ref enabletestinfochange, value);
        //}

        private bool istestruning = false;
        /// <summary>
        /// 试验是否正在运行中
        /// </summary>
        public bool IsTestRuning
        {
            get => istestruning;
            set => Set(nameof(IsTestRuning), ref istestruning, value);
        }

        private string contentname = string.Empty;
        /// <summary>
        /// 试验运行提示
        /// </summary>
        public string ContentName
        {
            get => contentname;
            set => Set(nameof(ContentName), ref contentname, value);
        }

        private ObservableCollection<string> prodectnames;
        /// <summary>
        /// 项目名称列表
        /// </summary>
        public ObservableCollection<string> ProdectNames
        {
            get => prodectnames;
            set => Set(nameof(ProdectNames), ref prodectnames, value);
        }

        private string selectedproductname;
        /// <summary>
        /// 当前选择项目名称
        /// </summary>
        public string SelectedProductName
        {
            get => selectedproductname;
            set => Set(nameof(SelectedProductName), ref selectedproductname, value);
        }

        private string productsn;
        /// <summary>
        /// 产品SN码
        /// </summary>
        public string ProductSN
        {
            get => productsn;
            set => Set(nameof(ProductSN), ref productsn, value);
        }

        private ObservableCollection<ExecuteInfo> ateexecuteinfos = new ObservableCollection<ExecuteInfo>();
        /// <summary>
        /// 试验执行信息展示
        /// </summary>
        public ObservableCollection<ExecuteInfo> ATEExecuteInfos
        {
            get => ateexecuteinfos;
            set => Set(nameof(ATEExecuteInfos), ref ateexecuteinfos, value);
        }


        #endregion

        #region 界面绑定指令
        public RelayCommand StartTest => new RelayCommand(StartedTest);

        #region 系统窗口指令
        /// <summary>
        /// 窗口最大化指令
        /// </summary>
        public RelayCommand<object> WindowMax => new RelayCommand<object>((o) =>
        {
            if (!(o is Window win))
            {
                return;
            }
            if (win.WindowState == WindowState.Maximized)
            {
                win.WindowState = WindowState.Normal;
                win.Width = 1000;
                win.Height = 600;
            }
            else if (win.WindowState == WindowState.Normal)
            {
                //double screenWidth = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width;
                //double screenHeight = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height;

                win.WindowState = WindowState.Maximized;

                //win.Height = SystemParameters.WorkArea.Size.Height;
                //win.Width = SystemParameters.WorkArea.Size.Width;


                //this.MaxWidth = screenWidth * 0.5; // 设置为屏幕宽度的80%
                //this.MaxHeight = screenHeight * 0.48; // 设置为屏幕高度的80%
                //this.MaxWidth = screenWidth; // 设置为屏幕宽度的80%
                //this.MaxHeight = screenHeight; // 设置为屏幕高度的80%


                //SystemCommands.MaximizeWindow(win);
            }
        });

        /// <summary>
        /// 窗口最小化指令
        /// </summary>
        public RelayCommand<object> WindowMin => new RelayCommand<object>((o) =>
        {
            if (!(o is Window win))
            {
                return;
            }
            win.WindowState = WindowState.Minimized;
        });
        #endregion

        #endregion

        #endregion

        #region 私有变量
        // private Window window;
        #endregion

        #region 私有方法

        private void StartedTest()
        {
            IsTestRuning = !IsTestRuning;

            //EnableTestInfoChange =! EnableTestInfoChange;
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
            //window = win;
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
        public MainViewModel() 
        {
            ATEExecuteInfos.Add(new ExecuteInfo 
            {
                Index = 1,
                FunctionName = "TEST1",
                Result = ResultState.UNACCOMPLISHED,
                StartTime = DateTime.Now.ToString("HH:mm:ss"),
                StepInfos = new ObservableCollection<ExecuteStepInfo> { 
                    new ExecuteStepInfo {
                        StepName = "test1",
                        ExecuteTime = DateTime.Now.ToString("HH:mm:ss"),
                        Limit_Lower = string.Empty,
                        Limit_Upper = string.Empty,
                        Result = ResultState.SUCCESS,
                    },
                    new ExecuteStepInfo {
                        StepName = "test2",
                        ExecuteTime = DateTime.Now.AddMinutes(1).ToString("HH:mm:ss"),
                        Limit_Lower = "1",
                        Limit_Upper = "3",
                        Result = ResultState.SUCCESS,
                    },
                     new ExecuteStepInfo {
                        StepName = "test3",
                        ExecuteTime = DateTime.Now.AddMinutes(2).ToString("HH:mm:ss"),
                        Limit_Lower = string.Empty,
                        Limit_Upper = string.Empty,
                        Result = ResultState.FAIL,
                    },
                }
            });

            ATEExecuteInfos.Add(new ExecuteInfo
            {
                Index = 2,
                FunctionName = "TEST2",
                Result = ResultState.UNACCOMPLISHED,
                StartTime = DateTime.Now.ToString("HH:mm:ss"),
                StepInfos = new ObservableCollection<ExecuteStepInfo> {
                    new ExecuteStepInfo {
                        StepName = "test1",
                        ExecuteTime = DateTime.Now.ToString("HH:mm:ss"),
                        Limit_Lower = string.Empty,
                        Limit_Upper = string.Empty,
                        Result = ResultState.SUCCESS,
                    },
                    new ExecuteStepInfo {
                        StepName = "test2",
                        ExecuteTime = DateTime.Now.AddMinutes(1).ToString("HH:mm:ss"),
                        Limit_Lower = "1",
                        Limit_Upper = "3",
                        Result = ResultState.SUCCESS,
                    },
                     new ExecuteStepInfo {
                        StepName = "test3",
                        ExecuteTime = DateTime.Now.AddMinutes(2).ToString("HH:mm:ss"),
                        Limit_Lower = string.Empty,
                        Limit_Upper = string.Empty,
                        Result = ResultState.FAIL,
                    },
                }
            });
        }
        #endregion

    }

    /// <summary>
    /// ATE执行信息
    /// </summary>
    public class ExecuteInfo : ViewModelBase
    {
        /// <summary>
        /// 序号
        /// </summary>
        public int Index { get; set; }

        /// <summary>
        /// 方案名称
        /// </summary>
        public string FunctionName { get; set; }


        private ResultState result;
        /// <summary>
        /// 执行结果
        /// </summary>
        public ResultState Result
        {
            get => result;
            set => Set(nameof(Result), ref result, value);
        }

        /// <summary>
        /// 开始时间
        /// </summary>
        public string StartTime { get; set; }

        private ObservableCollection<ExecuteStepInfo> stepinfos;

        public ObservableCollection<ExecuteStepInfo> StepInfos
        {
            get => stepinfos;
            set => Set(nameof(StepInfos), ref stepinfos, value);
        }

        /// <summary>
        /// 步骤信息展示
        /// </summary>
        public RelayCommand Show => new RelayCommand(() => { });
    }

    public class ExecuteStepInfo : ViewModelBase
    {
        /// <summary>
        /// 执行步骤名称
        /// </summary>
        public string StepName { get; set; }

        /// <summary>
        /// 下限值
        /// </summary>
        public string Limit_Lower { get; set; }

        /// <summary>
        /// 上限值
        /// </summary>
        public string Limit_Upper { get; set;}

        private ResultState result;
        /// <summary>
        /// 执行结果
        /// </summary>
        public ResultState Result
        {
            get => result;
            set => Set(nameof(Result), ref result, value);
        }

        /// <summary>
        /// 开始时间
        /// </summary>
        public string ExecuteTime { get; set; }
    }
}
