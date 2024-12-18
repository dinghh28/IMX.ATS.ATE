﻿#region << 版 本 注 释 >>
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
using IMX.ATE.Common;
using IMX.Common;
using IMX.DB;
using IMX.DB.Model;
using IMX.Device.Base.DriveOperate;
using IMX.Device.Base;
using IMX.Function;
using IMX.Function.Base;
using IMX.Logger;
using IMX.WPF.Resource;
using Super.Zoo.Framework;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;
using IMX.Device.Common;
using IMX.Device.Base.DeviceInerfaces;
using System.Xml.Linq;
using IMX.Device.Product;
using IMX.Device.DCLoad;
using IMX.Device.Common.Enumerations;
using IMX.Function.Base.Enumerations;
using System.Windows.Documents;
using System.Reflection.Emit;
using Newtonsoft.Json.Linq;
using System.Windows.Ink;
using IMX.Device.Relay;
using System.Windows.Input;

namespace IMX.ATS.ATE
{
    public class MainViewModel : WindowViewModelBaseEx
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

        private SolidColorBrush contentcolor = Brushes.Green;

        public SolidColorBrush ContentColor
        {
            get => contentcolor;
            set => Set(nameof(ContentColor), ref contentcolor, value);
        }


        private ObservableCollection<string> prodectnames = new ObservableCollection<string>();
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

        private bool isfocuse = false;
        /// <summary>
        /// 当前SN码输入框是否获取焦点
        /// </summary>
        public bool IsFocuse
        {
            get => isfocuse;
            set => Set(nameof(IsFocuse), ref isfocuse, value);
        }

        private bool winfocuse = true;
        /// <summary>
        /// 当前窗口焦点
        /// </summary>
        public bool WinFocuse
        {
            get => winfocuse;
            set => Set(nameof(WinFocuse), ref winfocuse, value);
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

        #region 试验结果
        private string testresult;
        /// <summary>
        /// 试验结果
        /// </summary>
        public string TestResult
        {
            get => testresult;
            set => Set(nameof(TestResult), ref testresult, value);
        }

        private SolidColorBrush testresultcolor = Brushes.Yellow;
        /// <summary>
        /// 试验结果颜色
        /// </summary>
        public SolidColorBrush TestResultColor
        {
            get => testresultcolor;
            set => Set(nameof(TestResultColor), ref testresultcolor, value);
        }
        #endregion

        private bool enabletestbtn = true;
        /// <summary>
        /// 试验按钮使能
        /// </summary>
        public bool EnableTestBtn
        {
            get => enabletestbtn;
            set => Set(nameof(EnableTestBtn), ref enabletestbtn, value);
        }

        private bool prodectnamechangelock = true;
        /// <summary>
        /// 项目锁（锁定后无法变更试验项目）
        /// </summary>
        public bool ProdectNameChangeLock
        {
            get => prodectnamechangelock;
            set => Set(nameof(ProdectNameChangeLock), ref prodectnamechangelock, value);
        }


        /// <summary>
        /// 当前登陆用户
        /// </summary>
        public string UserName => GlobalModel.UserInfo?.UserName;

        /// <summary>
        /// 软件版本信息
        /// </summary>
        public string SoftwareVersion => SysteamInfo.SoftwareVersion;

        /// <summary>
        /// 是否允许运行试验
        /// </summary>
        public bool EnableRunTest => GlobalModel.CabinetSate;

        #region 准备阶段提示
        private string stepstr;
        /// <summary>
        /// 阶段信息
        /// </summary>
        public string StepStr
        {
            get => stepstr;
            set => Set(nameof(StepStr), ref stepstr, value);
        }

        private Visibility stepstrshow = Visibility.Collapsed;
        /// <summary>
        /// 阶段信息展示
        /// </summary>
        public Visibility StepStrShow
        {
            get => stepstrshow;
            set => Set(nameof(StepStrShow), ref stepstrshow, value);
        }
        #endregion

        #endregion

        #region 界面绑定指令
        /// <summary>
        /// 试验指令
        /// </summary>
        public RelayCommand Test => new RelayCommand(DoTest);

        /// <summary>
        /// 清除故障指示灯
        /// </summary>
        public RelayCommand ClearError => new RelayCommand(ClearErrorLED);


        /// <summary>
        /// 扫码枪回车事件
        /// </summary>
        public RelayCommand<object> EnterKeyboard => new RelayCommand<object>(FocuseChanged);

        #region 系统窗口指令
        ///// <summary>
        ///// 窗口最大化指令
        ///// </summary>
        //public RelayCommand<object> WindowMax => new RelayCommand<object>((o) =>
        //{
        //    if (!(o is Window win))
        //    {
        //        return;
        //    }
        //    if (win.WindowState == WindowState.Maximized)
        //    {
        //        win.WindowState = WindowState.Normal;
        //        win.Width = 1000;
        //        win.Height = 600;
        //    }
        //    else if (win.WindowState == WindowState.Normal)
        //    {
        //        //double screenWidth = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width;
        //        //double screenHeight = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height;

        //        win.WindowState = WindowState.Maximized;

        //        //win.Height = SystemParameters.WorkArea.Size.Height;
        //        //win.Width = SystemParameters.WorkArea.Size.Width;


        //        //this.MaxWidth = screenWidth * 0.5; // 设置为屏幕宽度的80%
        //        //this.MaxHeight = screenHeight * 0.48; // 设置为屏幕高度的80%
        //        //this.MaxWidth = screenWidth; // 设置为屏幕宽度的80%
        //        //this.MaxHeight = screenHeight; // 设置为屏幕高度的80%


        //        //SystemCommands.MaximizeWindow(win);
        //    }
        //});

        ///// <summary>
        ///// 窗口最小化指令
        ///// </summary>
        //public RelayCommand<object> WindowMin => new RelayCommand<object>((o) =>
        //{
        //    if (!(o is Window win))
        //    {
        //        return;
        //    }
        //    win.WindowState = WindowState.Minimized;
        //});
        #endregion

        #endregion

        #endregion

        #region 私有变量

        /// <summary>
        /// 试验监控线程
        /// </summary>
        private MonitorThread monitor = null;

        /// <summary>
        /// 继电器操作句柄
        /// </summary>
        private Relay_ZS4Bit_Operate relayoperate_4 = null;
        // private Window window;
        #endregion

        #region 私有方法

        /// <summary>
        /// 指示灯清除
        /// </summary>
        private void ClearErrorLED()
        {
            if (IsTestRuning)
            {
                relayoperate_4?.SateLedContrcl(LightType.RUNING);
            }
            else
            {
                relayoperate_4?.SateLedContrcl(LightType.DEFALT);
            }

            
            Thread.Sleep(100);
            IsFocuse = true;
            //MessageBox.Show($"故障指示灯已清除");
        }

        private int productsnlenth = 0;
        /// <summary>
        /// 产品SN回车焦点事件
        /// </summary>
        /// <param name="obj"></param>
        private void FocuseChanged(object obj)
        {
            //焦点获取判定，非textbox不处理事件
            if (!IsFocuse) { return; }

            if (!(obj is KeyDownArgs args))
            {
                return;
            }

            if (args.Args.Key == Key.Back) 
            {
                productsnlenth = ProductSN.Length;
                return;
            }
            else if (args.Args.Key != Key.Enter)
            {
                return;
            }

           if (IsTestRuning)
            {
                return;
            }

            Thread.Sleep(20);
            //if (!string.IsNullOrEmpty(ProductSN)) 
            //{

            //}

            ProductSN = ProductSN.Remove(0, productsnlenth);

            

            TestStartReady();

            //if (args.Parameters.Length < 1)
            //{
            //    return;
            //}
        }

        #region 试验操作
        private void DoTest()
        {
            if (!GlobalModel.CabinetSate)
            {
                MessageBox.Show("当前工装存在故障设备，不支持开启试验", "工装异常");
                return;
            }

            if (IsTestRuning)
            {
                if (MessageBox.Show("是否暂停试验?", "试验停止确认", MessageBoxButton.OKCancel, MessageBoxImage.Question) == MessageBoxResult.Cancel)
                {
                    return;
                }
                Task.Run(()=>{ EnableTestBtn = false; });
                TestStop();
                EnableTestBtn = true;
            }
            else
            {
                TestStartReady();
                //if (string.IsNullOrEmpty(SelectedProductName))
                //{
                //    MessageBox.Show("试验项目不可为空，请选择需运行项目", "试验开启失败");
                //    return;
                //}

                //if (string.IsNullOrEmpty(ProductSN))
                //{
                //    MessageBox.Show("项目编码不可为空，请填写产品对应编码", "试验开启失败");
                //    return;
                //}

                //if (MessageBox.Show("是否开始试验?", "试验开始确认", MessageBoxButton.OKCancel, MessageBoxImage.Question) == MessageBoxResult.Cancel)
                //{
                //    return;
                //}

                //Task.Run(() => { EnableTestBtn = false; });

                //ATEExecuteInfos.Clear();

                //TestStart();
                //EnableTestBtn = true;
            }
            //EnableTestInfoChange =! EnableTestInfoChange;
        }

        /// <summary>
        /// 开始试验准备动作
        /// </summary>
        private void TestStartReady() 
        {
            if (string.IsNullOrEmpty(SelectedProductName))
            {
                MessageBox.Show("试验项目不可为空，请选择需运行项目", "试验开启失败");
                return;
            }

            if (string.IsNullOrEmpty(ProductSN))
            {
                MessageBox.Show("项目编码不可为空，请填写产品对应编码", "试验开启失败");
                return;
            }

            productsnlenth = ProductSN.Length;
            //if (MessageBox.Show("是否开始试验?", "试验开始确认", MessageBoxButton.OKCancel, MessageBoxImage.Question) == MessageBoxResult.Cancel)
            //{
            //    return;
            //}

            Task.Run(() => { EnableTestBtn = false; });

            ATEExecuteInfos.Clear();

            TestStart();

            Task.Run(() => {
                EnableTestBtn = true;
            });
        }
        /// <summary>
        /// 开始试验
        /// </summary>
        private void TestStart()
        {
            var rlt = DBOperate.Default.GetProjectInfo_ByName(SelectedProductName);
            if (!rlt)
            {
                MessageBox.Show($"项目信息获取失败:{rlt.Message}", "试验配置获取异常");
                return;
            }

            bool issucced = true;
            Test_DBCFileInfo dbcfileinfo = null;
            Test_DBCConfig dbcconfig = null;
            Test_Programme programme = null;
            Dictionary<string, List<TestFunction>> process = new Dictionary<string, List<TestFunction>>();
            Dictionary<string, string> dicreadsignals = new Dictionary<string, string>();
            List<string> sendsignals_can = new List<string>();
            string error = string.Empty;

            Test_ProjectInfo data = rlt.Data;

            #region DBC配置获取
            if (data.IsUseDDBC)
            {
                DBOperate.Default.GetDBCConfig_ByProjectID(data.Id)
                .ThenAnd(dbcresult => DBOperate.Default.GetFile_ByID(dbcresult.Data.DBCFileID)
                .AttachIfSucceed(result =>
                {
                    dbcfileinfo = result.Data;
                    //monitor.DicMonitorPage[CabinetID].DBCFileInfo = result.Data;
                })
                .ConvertTo(dbcresult.Data))
                .AttachIfSucceed(result =>
                {
                    dbcconfig = result.Data;
                    //MessageBox.Show("DBC配置获取成功");
                    //monitor.DicMonitorPage[CabinetID].DBCConfig = result.Data;
                })
                .AttachIfFailed(result =>
                {
                    issucced = false;
                    error = $"DBC配置获取失败:{result.Message}\r\n";
                    //MessageBox.Show($"DBC配置获取失败:{result.Message}", "试验配置保存异常");
                })
                .AttachIfExcepted(result =>
                {
                    issucced = false;
                    error = $"DBC配置获取异常:{result.Message}\r\n";
                    //MessageBox.Show($"DBC配置获取异常,请联系上位机工程师:\r\n{result.Message}", "试验配置保存异常"); 
                });

                if (dbcfileinfo == null || dbcconfig == null)
                {
                    issucced = false;
                    error = "无DBC文件或未配置DBC，请确认配置信息";
                }

                try
                {
                    if (issucced)
                    {
                        for (int i = 0; i < dbcconfig?.Test_DBCReceiveSignals?.Count; i++)
                        {
                            var signal = dbcconfig.Test_DBCReceiveSignals[i];
                            dicreadsignals.Add(signal.Signal_Name, signal.Custom_Name);
                        }

                        for (int i = 0; i < dbcconfig?.Test_DBCSendSignals?.Count; i++) 
                        {
                            sendsignals_can.Add(dbcconfig?.Test_DBCSendSignals[i].Signal_Name);
                        }
                    }
                }
                catch (Exception ex)
                {
                    error = $"DBC上报列表配置获取失败：\r\n{ex.GetMessage()}";
                    issucced = false;
                }
            }
            #endregion

            List<string> funtionnames = new List<string>();

            if (issucced)
            {
                ////加载循环方案信息
                DBOperate.Default.GetProgramme(data.Id)
                .AttachIfSucceed(result =>
                {
                    programme = result.Data;
                }).
                And(DBOperate.Default.GetFlowsByNameID(data.Id, programme.Test_FlowNames.Distinct().ToList())
                .AttachIfSucceed(result =>
                {
                    var keys = result.ConvertTo(result.Data).Data.Keys.ToList();
                    for (Int32 i = 0; i < result.Data.Count; i++)
                    {
                        List<TestFunction> functions = new List<TestFunction>();
                        for (int j = 0; j < result.Data[keys[i]].Count; j++)
                        {
                            var value = result.Data[keys[i]];
                            var test = TestFunction
                            .Create(Function_Config.DeJson((FuncitonType)Enum.Parse(typeof(FuncitonType), value[j].Type), value[j].Funtion).Data);
                            test.Step = (uint)value[j].Step;
                            test.Comments = value[j].CustomName;
                            functions.Add(test);
                        }
                        process.Add(keys[i], functions);
                    }

                })).AttachIfFailed(result =>
                {
                    error = $"试验流程获取失败：{result.Message}";
                });
            }

            if (!string.IsNullOrEmpty(error))
            {
                MessageBox.Show(error, "项目信息获取失败");
                return;
            }

            //线程参数初始化
            monitor = new MonitorThread
            {
                ThreadName = $"{data.ProjectName}_{ProductSN}",
                ThreadID = Guid.NewGuid().ToString(),
                OprateThread = new Thread(TestRun) { IsBackground = true },
                ProjectInfo = data,
                DBCFile = dbcfileinfo,
                DicReadSignals_CAN = dicreadsignals,
                SendSignals_CAN = sendsignals_can,
                Programme = programme,
                TestFlowsFunction = process,
            };

            Thread.Sleep(1000);

            TestResult = "进行中";
            TestResultColor = Brushes.Blue;
#if DEBUG
#else
            try
            {
                relayoperate_4?.SateLedContrcl(LightType.RUNING);
                //(GlobalModel.DicDeviceInfo["Relay"].DeviceOperate as Relay_ZS4Bit_Operate).SateLedContrcl(LightType.RUNING);
            }
            catch (Exception ex)
            {
                SuperDHHLoggerManager.Exception(LoggerType.FROMLOG, nameof(MainViewModel), nameof(TestStart), ex);
            }
            Thread.Sleep(1000);
#endif


            monitor.OprateThread.Start(monitor);

            
            ContentName = "运行试验中...";
            ContentColor = Brushes.Green;
            IsTestRuning = true;
        }


        private string TestErrorString = string.Empty;

        private void TestRun(object sender)
        {
            if (sender == null)
            {
                SuperDHHLoggerManager.Error(LoggerType.THREAD, nameof(MainViewModel), nameof(TestRun), $"线程参数不可为空");
                return;
            }

            if (!(sender is MonitorThread thread))
            {
                SuperDHHLoggerManager.Error(LoggerType.THREAD, nameof(MainViewModel), nameof(TestRun), $"线程传参异常:{sender.GetType()}");
                return;
            }

            if (string.IsNullOrEmpty(thread.ThreadID))
            {
                SuperDHHLoggerManager.Error(LoggerType.THREAD, nameof(MainViewModel), nameof(TestRun), $"试验流程线程【{thread.ThreadName}】不存在");
                return;
            }

            thread.IsRunning = true;
            TestErrorString = string.Empty;
            Test_DataInfo test_Data = new Test_DataInfo
            {
                Euq_Data = new List<ModTestDataInfo>(),
                Pro_Data = new List<ModTestDataInfo>(),
            };

            //CAN通讯初始化
            if (thread.ProjectInfo.IsUseDDBC)
            {
                Application.Current.Dispatcher.Invoke(new Action(() =>
                {
                    StepStr = "正在初始化CAN";
                    StepStrShow = Visibility.Visible;
                }));

                Thread.Sleep(1000);
                try
                {
                    GlobalModel.DicDeviceInfo["Product"].Args.DriveConfig.BaudRate = thread.ProjectInfo.BaudRate;
                    var canoprate = CANInt(GlobalModel.DicDeviceInfo["Product"].Args);
                    if (!canoprate)
                    {
                        GlobalModel.IsTestThreadRun = false;
                        thread.IsRunning = false;
                        SuperDHHLoggerManager.Fatal(LoggerType.THREAD, nameof(MainViewModel), nameof(TestRun), canoprate.Message);

                        Thread.Sleep(200);

                        Application.Current.Dispatcher.Invoke(new Action(() =>
                        {
                            ContentName = "CAN通讯初始化失败";
                            ContentColor = Brushes.Red;
                            StepStrShow = Visibility.Collapsed;
                            IsTestRuning = false;
                            TestResult = "FAIL";
                            TestResultColor = Brushes.Red;
                            //IsFocuse = true;
                        }));

                        try
                        {
                            relayoperate_4?.SateLedContrcl(LightType.ERROR);
                            //(GlobalModel.DicDeviceInfo["Relay"].DeviceOperate as Relay_ZS4Bit_Operate).SateLedContrcl(LightType.ERROR);
                        }
                        catch (Exception ex)
                        {
                            SuperDHHLoggerManager.Exception(LoggerType.FROMLOG, nameof(MainViewModel), nameof(TestStart), ex);
                        }

                        MessageBox.Show(canoprate.Message, "CAN初始化失败");
                        return;
                    }
                   
                    GlobalModel.DicDeviceInfo["Product"].DeviceOperate = canoprate.Data;
                    Product_CAN_Operate operate = (canoprate.Data as Product_CAN_Operate);

                    var cansetrlt =operate.LoadMessageFile(thread.DBCFile.FileContent, thread.DBCFile.FileExtension)
                        .And(operate.SetReadSignal(thread.DicReadSignals_CAN))
                        .And(operate.SetSendSignal(thread.SendSignals_CAN));

                    if (!cansetrlt) 
                    {
                        GlobalModel.IsTestThreadRun = false;
                        thread.IsRunning = false;
                        SuperDHHLoggerManager.Fatal(LoggerType.THREAD, nameof(MainViewModel), nameof(TestRun), canoprate.Message);

                        Thread.Sleep(200);
                        Application.Current.Dispatcher.Invoke(new Action(() =>
                        {
                            ContentName = "CAN配置参数初始化失败";
                            ContentColor = Brushes.Red;
                            StepStrShow = Visibility.Collapsed;
                            IsTestRuning = false;
                            TestResult = "FAIL";
                            TestResultColor = Brushes.Red;
                            //ProductSN = string.Empty;
                            //productsnlenth = 0;
                        }));

                        CANUnint(operate);

                        MessageBox.Show(canoprate.Message, "CAN参数初始化失败");
                        return;
                    }

                    //    .ThenAnd(result =>
                    //{
                    //    Product_CAN_Operate operate = (result.Data as Product_CAN_Operate);
                    //    return operate
                    //     .SetReadSignal(thread.DicReadSignals_CAN)
                    //     .And(operate.SetSendSignal(thread.SendSignals_CAN))
                    //     .ConvertTo(result.Data);
                    //});
                    for (int i = 0; i < GlobalModel.DicDeviceInfo["Product"].DeviceOperate.ReadInfos.Count; i++) 
                    {
                        test_Data.Pro_Data.Add(GlobalModel.DicDeviceInfo["Product"].DeviceOperate.ReadInfos[i].DataInfo);
                    }
                    
                    //test_Data.Pro_DeviceRead = GlobalModel.DicDeviceInfo["Product"].DeviceOperate.ReadInfos;
                }
                catch (Exception ex)
                {
                    GlobalModel.IsTestThreadRun = false;
                    thread.IsRunning = false;
                    SuperDHHLoggerManager.Exception(LoggerType.THREAD, nameof(MainViewModel), nameof(TestRun), ex);
                    MessageBox.Show(ex.GetMessage(), "CAN初始化异常");

                    Application.Current.Dispatcher.Invoke(new Action(() =>
                    {
                        ContentName = "CAN通讯初始化失败";
                        ContentColor = Brushes.Red;
                        StepStrShow = Visibility.Collapsed;
                        TestResult = "FAIL";
                        TestResultColor = Brushes.Red;
                        //ProductSN = string.Empty;
                        //productsnlenth = 0;
                        //IsFocuse = true;
                    }));

                    return;
                }
            }

            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                StepStrShow = Visibility.Collapsed;
            }));
#if DEBUG
#else
            for (int i = 0; i < GlobalModel.DicDeviceInfo["Acquisition"].DeviceOperate.ReadInfos.Count; i++)
            {
                test_Data.Euq_Data.Add(GlobalModel.DicDeviceInfo["Acquisition"].DeviceOperate.ReadInfos[i].DataInfo);
            }
#endif

            //test_Data.Euq_DeviceRead = GlobalModel.DicDeviceInfo["Acquisition"].DeviceOperate.ReadInfos;
            bool testresult = true;
            string errorstr = string.Empty;

            Test_ItemInfo testinfo = new Test_ItemInfo
            {
                ProductSN = ProductSN,
                ProjectName = SelectedProductName,
                ProjectID = thread.ProjectInfo.Id,
                Operator = UserName,
            };

            
            SaveItem(true, testinfo);
            test_Data.TestItemID = testinfo.Id;
            test_Data.ProductSN = ProductSN;
            test_Data.ProjectName = SelectedProductName;

            #region 试验方案执行
            for (int i = 0; i < thread.Programme.Test_FlowNames?.Count; i++)
            {

                if (!thread.IsStartThread)
                {
                    break;
                }

                string flowname = thread.Programme.Test_FlowNames[i];


                if (!thread.TestFlowsFunction.TryGetValue(flowname, out List<TestFunction> flows))
                {
                    Application.Current.Dispatcher.Invoke(new Action(() =>
                    {
                        ContentName = "试验方案获取异常";
                        ContentColor = Brushes.Red;
                        StepStrShow = Visibility.Collapsed;
                        TestResult = "FAIL";
                        TestResultColor = Brushes.Red;
                        //IsFocuse = true;
                    }));

                    TestErrorString = ContentName;
                    break;
                }

                //步骤信息记录
                ObservableCollection<ExecuteStepInfo> stepinfo = new ObservableCollection<ExecuteStepInfo>();

                ExecuteInfo functioninfo = new ExecuteInfo
                {
                    Index = i + 1,
                    Result = ResultState.UNACCOMPLISHED,
                    StartTime = DateTime.Now.ToString("HH:mm:ss"),
                    StepInfos = stepinfo,
                    FunctionName = flowname
                };


                Application.Current.Dispatcher.Invoke(new Action(() =>
                {
                    ATEExecuteInfos.Add(functioninfo);
                }));
                #region 试验项执行
                //TODO 试验项执行
                for (int j = 0; j < flows?.Count; j++)
                {
                    bool isoutfunc = false;
                    if (!thread.IsStartThread)
                    {
                        break;
                    }

                    if (isoutfunc) 
                    {
                        break;
                    }

                    try
                    {
                        IFuntionConfig config = flows[j].Config;

                        IDeviceOperate operate = null;
                        //if (config.DeviceAddress != null)
                        //{
                        if (GlobalModel.DicDeviceInfo.TryGetValue(config.SupportFuncitonType.ToString(), out DeviceInfo_ALL info))
                        {
                            operate = info.DeviceOperate;
                        }
                        //}

                        ExecuteStepInfo step = new ExecuteStepInfo
                        {
                            Result = ResultState.UNACCOMPLISHED,
                            ExecuteTime = DateTime.Now.ToString("HH:mm:ss"),
                            StepName = config.SupportFuncitonType.GetDescription()
                        };

                        #region 试验步骤执行
                        //TODO 试验步骤执行
                        if (config.SupportFuncitonType == FuncitonType.ProductResult)
                        {
                            if (!thread.ProjectInfo.IsUseDDBC)
                            {
                                continue;
                            }
                            try
                            {
                                if (GlobalModel.DicDeviceInfo.TryGetValue("Product", out DeviceInfo_ALL caninfo))
                                {
                                    operate = caninfo.DeviceOperate;
                                }
                                FunConfig_ProductResult resultconfig = config as FunConfig_ProductResult;

                                var result = ProductResultExecute(j + 1, flowname, stepinfo, resultconfig, operate as Product_CAN_Operate);

                                test_Data.StepName = config.SupportFuncitonType.GetDescription();
                                test_Data.FlowName = flowname;
                                test_Data.StepIndex = j + 1;

                                test_Data.Pro_DeviceRead = new List<ModDeviceReadData>();
                                for (int k = 0; k < resultconfig?.Datas?.Count; k++)
                                {
                                    test_Data.Pro_DeviceRead.Add(resultconfig.Datas[k]);
                                }
                                test_Data.Id = 0;
                                test_Data.ErrorInfo = result? string.Empty : result.Message;
                                test_Data.Result = result? ResultState.SUCCESS : ResultState.FAIL;
                                Task.Run(() => { DBOperate.Default.InserTestData(test_Data); });

                                if (!result)
                                {
                                    relayoperate_4.SateLedContrcl(LightType.ERROR);

                                    testresult = false;
                                    TestErrorString += $"{result.Message}\r\n";
                                    if (resultconfig.ResultOpereate == ResultOpereateType.OUTTEST)
                                    {
                                        thread.IsStartThread = false;
                                        break;
                                    }
                                    else if (resultconfig.ResultOpereate == ResultOpereateType.OUTFUNCTION)
                                    {
                                        isoutfunc = true;
                                        //thread.IsStartThread = true;
                                        break;
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                relayoperate_4.SateLedContrcl(LightType.ERROR);
                                TestErrorString += $"{ex.GetMessage()}\r\n";
                                thread.IsStartThread = false;
                                ProductSN = string.Empty;
                                productsnlenth = 0;
                                //IsFocuse = true;
                                break;
                            }

                        }
                        else if (config.SupportFuncitonType == FuncitonType.EquipmentResult)
                        {
#if DEBUG
                            continue;
#endif

                            if (GlobalModel.DicDeviceInfo.TryGetValue("Acquisition", out DeviceInfo_ALL acqinfo))
                            {
                                operate = acqinfo.DeviceOperate;
                            }

                            FunConfig_EquipmentResult resultconfig = config as FunConfig_EquipmentResult;
                            var result = EquipmentResultExecute(j+1, flowname, stepinfo, resultconfig, operate as IAcquisition);

                            test_Data.StepName = config.SupportFuncitonType.GetDescription();
                            test_Data.FlowName = flowname;
                            test_Data.StepIndex = j + 1;
                            test_Data.Euq_DeviceRead = new List<ModDeviceReadData>();
                            for (int k = 0; k < resultconfig?.Datas?.Count; k++)
                            {
                                test_Data.Euq_DeviceRead.Add(resultconfig.Datas[k]);
                            }
                            test_Data.Id = 0;
                            test_Data.ErrorInfo = result ? string.Empty : result.Message;
                            test_Data.Result = result ? ResultState.SUCCESS : ResultState.FAIL;
                            Task.Run(() => { DBOperate.Default.InserTestData(test_Data); });

                            if (!result)
                            {
                                relayoperate_4.SateLedContrcl(LightType.ERROR);
                                testresult = false;
                                TestErrorString += $"{result.Message}\r\n";
                                if (resultconfig.ResultOpereate == ResultOpereateType.OUTTEST)
                                {
                                    thread.IsStartThread = false;
                                    break;
                                }
                                else if (resultconfig.ResultOpereate == ResultOpereateType.OUTFUNCTION)
                                {
                                    isoutfunc = true;
                                    //thread.IsStartThread = true;
                                    break;
                                }
                            }
                        }
                        else if (config.SupportFuncitonType == FuncitonType.DCLoad)
                        {
#if DEBUG
                            continue;
#endif
                            Application.Current.Dispatcher.Invoke(new Action(() =>
                            {
                                stepinfo.Add(step);
                            }));

                            Product_CAN_Operate product_CAN = null;
                            IAcquisition acquisition = null;
                            if (thread.ProjectInfo.IsUseDDBC)
                            {
                                if (GlobalModel.DicDeviceInfo.TryGetValue("Product", out DeviceInfo_ALL infopro))
                                {
                                    product_CAN = infopro.DeviceOperate as Product_CAN_Operate;
                                }
                            }
                            if (GlobalModel.DicDeviceInfo.TryGetValue("Acquisition", out DeviceInfo_ALL infoac))
                            {
                                acquisition = infoac.DeviceOperate as IAcquisition;
                            }

                          var  result =  DCLoadExecute(config as FunConfig_DCLoad, 
                                                      operate as IDCLoad, 
                                                      product_CAN, 
                                                      acquisition);
                            if (!result)
                            {
                                relayoperate_4.SateLedContrcl(LightType.ERROR);
                                thread.IsStartThread = false;
                                Application.Current.Dispatcher.Invoke(new Action(() =>
                                {
                                    step.Result = ResultState.FAIL;
                                }));
                                TestErrorString += result.Message;
                                break;
                            }

                            Application.Current.Dispatcher.Invoke(new Action(() =>
                            {
                                step.Result = ResultState.SUCCESS;
                            }));
                        }
                        else if (config.SupportFuncitonType == FuncitonType.ACSource)
                        {
#if DEBUG
                            continue;
#endif
                            Application.Current.Dispatcher.Invoke(new Action(() =>
                            {
                                stepinfo.Add(step);
                            }));

                            Product_CAN_Operate product_CAN = null;
                            IAcquisition acquisition = null;
                            if (thread.ProjectInfo.IsUseDDBC)
                            {
                                if (GlobalModel.DicDeviceInfo.TryGetValue("Product", out DeviceInfo_ALL infopro))
                                {
                                    product_CAN = infopro.DeviceOperate as Product_CAN_Operate;
                                }
                            }
                            if (GlobalModel.DicDeviceInfo.TryGetValue("Acquisition", out DeviceInfo_ALL infoac))
                            {
                                acquisition = infoac.DeviceOperate as IAcquisition;
                            }

                            var result = ACScourceExecute(config as FunConfig_ACSource,
                                                        operate as IACSource,
                                                        product_CAN,
                                                        acquisition);
                            if (!result)
                            {
                                relayoperate_4.SateLedContrcl(LightType.ERROR);
                                thread.IsStartThread = false;
                                Application.Current.Dispatcher.Invoke(new Action(() =>
                                {
                                    step.Result = ResultState.FAIL;
                                }));
                                TestErrorString += result.Message;


                                break;
                            }

                            Application.Current.Dispatcher.Invoke(new Action(() =>
                            {
                                step.Result = ResultState.SUCCESS;
                            }));
                        }
                        else
                        {
                            Application.Current.Dispatcher.Invoke(new Action(() =>
                            {
                                stepinfo.Add(step);
                            }));

                            //防止调试过程中频繁勾选DBC使用情况，试验项步骤未变更
                            if (config.SupportFuncitonType == FuncitonType.Product && !thread.ProjectInfo.IsUseDDBC)
                            {
                                step.Result = ResultState.SUCCESS;
                                continue;
                            }

#if DEBUG
                            if (config.SupportFuncitonType != FuncitonType.Product) 
                            {
                                continue;
                            }
#endif

                            var result =  config.Execute(operate);
                            if (!result)
                            {
                                relayoperate_4.SateLedContrcl(LightType.ERROR);
                                thread.IsStartThread = false;
                                Application.Current.Dispatcher.Invoke(new Action(() =>
                                {
                                    step.Result = ResultState.FAIL;
                                }));
                                TestErrorString += result.Message;
                                break;
                            }

                            Application.Current.Dispatcher.Invoke(new Action(() =>
                            {
                                step.Result = ResultState.SUCCESS;
                            }));
                        }
                        Thread.Sleep(0);
                        #endregion
                    }
                    catch (Exception ex)
                    {
                        relayoperate_4.SateLedContrcl(LightType.ERROR);
                        thread.IsStartThread = false;
                        //ProductSN = string.Empty;
                        //productsnlenth = 0;
                        //IsFocuse = true;
                        SuperDHHLoggerManager.Exception(LoggerType.FROMLOG, nameof(MainViewModel), nameof(TestRun), ex);
                        break;
                    }
                }
                #endregion
            }
            #endregion

            if (!string.IsNullOrEmpty(TestErrorString))
            {
                testinfo.ErrorInfo = TestErrorString;
                testinfo.Result = ResultState.FAIL;
                Application.Current.Dispatcher.Invoke(new Action(() =>
                {
                    
                    TestResult = "FAIL";
                    TestResultColor = Brushes.Red;
                    //ProductSN = string.Empty;
                    //productsnlenth = 0;
                    //IsFocuse = true;
                }));
                //MessageBox.Show(TestErrorString, "试验失败");
            }
            else
            {
                testinfo.Result = ResultState.SUCCESS;
                Application.Current.Dispatcher.Invoke(new Action(() =>
                {
                    //ContentColor = Brushes.Red;
                    TestResult = "PASS";
                    TestResultColor = Brushes.Green;

                }));
                relayoperate_4?.SateLedContrcl(LightType.DEFALT);
                //MessageBox.Show("试验运行完成", "试验成功");
            }

            testinfo.ActualRunTime = DateTime.Now.Ticks - testinfo.CreateTime.Ticks;
            SaveItem(false, testinfo);

            ShutDown(thread.Programme.TestOff_FlowNames, thread.ProjectInfo.IsUseDDBC);
            if (thread.ProjectInfo.IsUseDDBC)
            {
                CANUnint(GlobalModel.DicDeviceInfo["Product"].DeviceOperate);
            }

            thread.IsRunning = false;
            GlobalModel.IsTestThreadRun = false;
            IsTestRuning = false;

            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                ContentName = string.Empty;
                StepStrShow = Visibility.Collapsed;
                //ProductSN = string.Empty;
                //productsnlenth = 0;
                //IsFocuse = true;
            }));
            //try
            //{
            //    //(GlobalModel.DicDeviceInfo["Relay"].DeviceOperate as Relay_ZS4Bit_Operate).SateLedContrcl(LightType.DEFALT);
            //}
            //catch (Exception ex)
            //{
            //    SuperDHHLoggerManager.Exception(LoggerType.FROMLOG, nameof(MainViewModel), nameof(TestRun), ex);
            //}
        }

        #region 特殊步骤操作
        /// <summary>
        /// 产品试验结果执行
        /// </summary>
        /// <returns></returns>
        private OperateResult ProductResultExecute(int index, string flowname, ObservableCollection<ExecuteStepInfo> infos, FunConfig_ProductResult config, Product_CAN_Operate operate)
        {
            string errorstring = string.Empty;

            try
            {
                if (operate == null)
                {
                    errorstring = "设备类型不存在";
                    SuperDHHLoggerManager.Error(LoggerType.TESTLOG, nameof(MainViewModel), nameof(ProductResultExecute), errorstring);
                    return OperateResult.Failed(errorstring);
                }

                if (config == null)
                {
                    errorstring = "产品结果读取配置不可为空";
                    SuperDHHLoggerManager.Error(LoggerType.TESTLOG, nameof(MainViewModel), nameof(ProductResultExecute), errorstring);
                    return OperateResult.Failed(errorstring);
                }

                operate.Device_ReadAll();

                for (int i = 0; i < config.Datas?.Count; i++)
                {
                    ModDeviceReadData data = config.Datas[i];
                    ExecuteStepInfo step = new ExecuteStepInfo
                    {
                        StepName = config.Datas[i].DataInfo.Name,
                        ExecuteTime = DateTime.Now.ToString("HH:mm:ss"),
                        //NowVlaue=data.DataInfo.Value.ToString(),
                        Limit_Lower = data.Limits_Lower.ToString(),
                        Limit_Upper = data.Limits_Upper.ToString(),
                        ValueConditions = data.Judgment.GetDescription(),
                    };
                    config.Datas[i].DataInfo.Value = operate.DicReadInfo[config.Datas[i].DataInfo.Name].DataInfo.Value;
                    
                    step.NowVlaue = data.DataInfo.Value.ToString();

                    SuperDHHLoggerManager.Info(LoggerType.TESTLOG, nameof(MainViewModel), nameof(ProductResultExecute),
                         $"测试项目：【{flowname}】\r\n步骤：【{index}】【{data.DataInfo.Name}】\r\n 当前读取值【{data.DataInfo.Value}】（判定条件[{data.Judgment.GetDescription()}]）要求：{data.Limits_Lower}- {data.Limits_Upper}");
                    if (!config.Datas[i].IsInRange)
                    {
                        step.Result = ResultState.FAIL;
                        errorstring += $"测试项目：【{flowname}】\r\n步骤：【{index}】【{data.DataInfo.Name}】\r\n 当前读取值【{data.DataInfo.Value}】（判定条件[{data.Judgment.GetDescription()}]）要求：{data.Limits_Lower}- {data.Limits_Upper}\r\n";
                    }
                    else
                    {
                        step.Result = ResultState.SUCCESS;
                    }

                    Application.Current.Dispatcher.Invoke(new Action(() =>
                    {
                        infos.Add(step);
                    }));
                }

                Thread.Sleep(config.DelayAfterRun > 0 ? config.DelayAfterRun : 0);

                if (!string.IsNullOrEmpty(errorstring))
                {
                    return OperateResult.Failed(errorstring);
                }

                return OperateResult.Succeed();
            }
            catch (Exception)
            {

                throw;
            }
        }

        /// <summary>
        /// 工装试验结果执行
        /// </summary>
        /// <returns></returns>
        private OperateResult EquipmentResultExecute(int index, string flowname, ObservableCollection<ExecuteStepInfo> infos, FunConfig_EquipmentResult config, IAcquisition operate)
        {
            string errorstring = string.Empty;
            try
            {
                if (operate == null)
                {
                    errorstring = "设备类型不存在";
                    SuperDHHLoggerManager.Error(LoggerType.TESTLOG, nameof(MainViewModel), nameof(EquipmentResultExecute), errorstring);
                    return OperateResult.Failed(errorstring);
                }

                if (config == null)
                {
                    errorstring = "工装结果读取配置不可为空";
                    SuperDHHLoggerManager.Error(LoggerType.TESTLOG, nameof(MainViewModel), nameof(EquipmentResultExecute), errorstring);
                    return OperateResult.Failed(errorstring);
                }

                operate.Device_ReadAll();

                for (int i = 0; i < config.Datas?.Count; i++)
                {
                    ModDeviceReadData data = config.Datas[i];
                    ExecuteStepInfo step = new ExecuteStepInfo
                    {
                        StepName = config.Datas[i].DataInfo.Name,
                        ExecuteTime = DateTime.Now.ToString("HH:mm:ss"),
                        //NowVlaue = data.DataInfo.Value.ToString(),
                        Limit_Lower = data.Limits_Lower.ToString(),
                        Limit_Upper = data.Limits_Upper.ToString(),
                        ValueConditions = data.Judgment.GetDescription(),
                    };

                    config.Datas[i].DataInfo.Value = operate.DicReadInfo[config.Datas[i].DataInfo.Name].DataInfo.Value;
                    
                    step.NowVlaue = data.DataInfo.Value.ToString();
                    //test_Data.StepName = config.SupportFuncitonType.GetDescription();
                    //test_Data.FlowName = flowname;
                    //test_Data.StepIndex = j + 1;

                    SuperDHHLoggerManager.Info(LoggerType.TESTLOG, nameof(MainViewModel), nameof(EquipmentResultExecute),
                        $"测试项目：【{flowname}】\r\n步骤：【{index}】【{ data.DataInfo.Name}】\r\n 当前读取值【{ data.DataInfo.Value}】（判定条件[{ data.Judgment.GetDescription()}]）要求：{ data.Limits_Lower}- { data.Limits_Upper}");
                    if (!config.Datas[i].IsInRange)
                    {
                        step.Result = ResultState.FAIL;
                        errorstring += $"测试项目：【{flowname}】\r\n步骤：【{index}】【{data.DataInfo.Name}】\r\n 当前读取值【{data.DataInfo.Value}】（判定条件[{data.Judgment.GetDescription()}]）要求：{data.Limits_Lower}- {data.Limits_Upper}\r\n";
                    }
                    else
                    {
                        step.Result = ResultState.SUCCESS;
                    }

                    Application.Current.Dispatcher.Invoke(new Action(() =>
                    {
                        infos.Add(step);
                    }));
                }

                Thread.Sleep(config.DelayAfterRun > 0 ? config.DelayAfterRun : 0);

                if (!string.IsNullOrEmpty(errorstring))
                {
                    return OperateResult.Failed(errorstring);
                }

                return OperateResult.Succeed();
            }
            catch (Exception)
            {

                throw;
            }
        }

        /// <summary>
        /// 负载试验执行
        /// </summary>
        /// <returns></returns>
        private OperateResult DCLoadExecute(FunConfig_DCLoad config, IDCLoad device, Product_CAN_Operate canoperate, IAcquisition aqcoperate)
        {
            string errorstring = string.Empty;
            try
            {
                if (device == null)
                {
                    errorstring = "设备类型不存在";
                    SuperDHHLoggerManager.Error(LoggerType.TESTLOG, nameof(MainViewModel), nameof(DCLoadExecute), errorstring);
                    return OperateResult.Failed(errorstring);
                }

                if (config == null)
                {
                    errorstring = "直流负载配置不可为空";
                    SuperDHHLoggerManager.Error(LoggerType.TESTLOG, nameof(MainViewModel), nameof(DCLoadExecute), errorstring);
                    return OperateResult.Failed(errorstring);
                }
                if (aqcoperate == null)
                {
                    errorstring = "采样系统不存在";
                    SuperDHHLoggerManager.Error(LoggerType.TESTLOG, nameof(MainViewModel), nameof(DCLoadExecute), errorstring);
                    return OperateResult.Failed(errorstring);
                }

                string InfoString = string.Empty;


                //短路模式
                if (config.Set_ShortState == DeviceOutPutState.ON)
                {
                    OperateResult result = device.SetShort(config.Set_ShortState);
                    InfoString = $"设置\n[短路模式] {config.Set_ShortState}成功";
                    SuperDHHLoggerManager.Info(LoggerType.TESTLOG, nameof(MainViewModel), nameof(DCLoadExecute), InfoString);
                }
                else//非短路模式
                {
                    #region 启动拉载
                    OperateResult result = device.SetShort(config.Set_ShortState);
                    if (!result)
                    {
                        errorstring = $"设置\n[短路模式]异常：【{result.Message}】";
                        SuperDHHLoggerManager.Error(LoggerType.TESTLOG, nameof(MainViewModel), nameof(DCLoadExecute), errorstring);
                        return OperateResult.Failed(errorstring);
                    }
                    Thread.Sleep(200);

                    double loadvalue = config.EnableStepping ? config.StartLoadValue : config.Set_LoadValue;

                    OperateResult SetRlt = null;
                    if (string.IsNullOrEmpty(config.Set_Model))
                    {
                        SetRlt = device.SetLoadValue(loadvalue);
                    }
                    else
                    {
                        SetRlt = device.SetModelAndValue(config.Set_Model, loadvalue);
                    }

                    if (!SetRlt)
                    {
                        errorstring = $"设备参数设置异常：【{SetRlt.Message}】";
                        SuperDHHLoggerManager.Error(LoggerType.TESTLOG, nameof(MainViewModel), nameof(DCLoadExecute), errorstring);
                        return OperateResult.Failed(errorstring);
                    }

                    InfoString = $"设置\n[模式]{config.Set_Model}\n[拉载值]{loadvalue}成功";
                    SuperDHHLoggerManager.Info(LoggerType.TESTLOG, nameof(MainViewModel), nameof(DCLoadExecute), InfoString);

                    OperateResult SetParamRlt = device.SetParameters(config.Set_ParamValue1, config.Set_ParamValue2);

                    if (!SetParamRlt)
                    {
                        errorstring = $"设备拉载参数设置异常：【{SetParamRlt.Message}】";
                        SuperDHHLoggerManager.Error(LoggerType.TESTLOG, nameof(MainViewModel), nameof(DCLoadExecute), errorstring);

                        //Logger.Error(nameof(FunConfig_DCLoad), nameof(Execute), LastError);
                        return OperateResult.Failed(errorstring);
                    }

                    InfoString = $"设置\n[模式]{config.Set_Model}\n[参数1]{config.Set_ParamValue1}\n[参数2]{config.Set_ParamValue2}成功";
                    SuperDHHLoggerManager.Info(LoggerType.TESTLOG, nameof(MainViewModel), nameof(DCLoadExecute), InfoString);

                    if (config.OperateType != SetOutPutState.Null)
                    {
                        Thread.Sleep(200);

                        device.SetOnOff(config.OperateType == SetOutPutState.ON ? DeviceOutPutState.ON : DeviceOutPutState.OFF);

                        InfoString = config.OperateType == SetOutPutState.ON ? "打开" : "关闭";
                        SuperDHHLoggerManager.Info(LoggerType.TESTLOG, nameof(MainViewModel), nameof(DCLoadExecute), InfoString);

                        //Logger.Info(nameof(FunConfig_DCLoad), nameof(Execute), $"设备已{InfoString}");
                    }

                    #endregion

                    //步进模式
                    if (config.EnableStepping)
                    {
                        Thread.Sleep(config.StepFrequency);

                        Dictionary<string, ModDeviceReadData> data = new Dictionary<string, ModDeviceReadData>();

                        if (canoperate != null)
                        {
                            for (int i = 0; i < canoperate?.ReadInfos.Count; i++)
                            {
                                data.Add(canoperate.ReadInfos[i].DataInfo.Name, canoperate.ReadInfos[i]);
                            }
                        }

                        for (int i = 0; i < aqcoperate?.ReadInfos.Count; i++)
                        {
                            data.Add(aqcoperate.ReadInfos[i].DataInfo.Name, aqcoperate.ReadInfos[i]);
                        }

                        if (Math.Abs(config.EndLoadValue - config.StartLoadValue) >= config.Stride)
                        {

                            int count = Convert.ToInt16(Math.Abs((((config.EndLoadValue - config.StartLoadValue) % config.Stride) == 0) ? ((config.EndLoadValue - config.StartLoadValue) / config.Stride) : ((config.EndLoadValue - config.StartLoadValue) / config.Stride + 1)));
                            for (int i = 0; i < count; i++)
                            {
                                if (monitor != null && (!monitor.IsStartThread))
                                {
                                    SuperDHHLoggerManager.Info(LoggerType.TESTLOG, nameof(MainViewModel), nameof(ACScourceExecute), "手动退出步进");
                                    break;
                                }

                                double setpvol = config.StartLoadValue + config.Stride * (i + 1) * (config.EndLoadValue < config.StartLoadValue ? -1 : 1);

                                setpvol = config.EndLoadValue > config.StartLoadValue ? Math.Min(setpvol, config.EndLoadValue) : Math.Max(setpvol, config.EndLoadValue);

                                device.SetLoadValue(setpvol);

                                InfoString = $"设备步进设置拉载值{setpvol}成功";

                                SuperDHHLoggerManager.Info(LoggerType.TESTLOG, nameof(MainViewModel), nameof(DCLoadExecute), InfoString);

                                Thread.Sleep(config.StepFrequency);
                                
                                if (canoperate!=null)
                                {
                                    canoperate?.Device_ReadAll();
                                }
                                
                                aqcoperate?.Device_ReadAll();

                                //跳出步进条件
                                if (config.Values != null && config.Values.Count > 0)
                                {
                                    //if (config.StepCondition == StepConditions.OR)
                                    //{
                                    List<bool> results = new List<bool>();
                                    for (int j = 0; j < config.Values.Count; j++)
                                    {
                                        if (config.Values[j].Value == null)
                                        {
                                            continue;
                                        }
                                        double readdata = data[config.Values[j].Value.DataInfo.Name].DataInfo.Value;
                                        switch (config.Values[j].StepValueCondition.ToString())
                                        {
                                            case "GREATERTHAN"://大于

                                                results.Add(readdata > config.Values[j].ConditionValue ? true : false);
                                                //if (config.Values[j].Value.DataInfo.Value > config.Values[j].ConditionValue)
                                                //{
                                                //    InfoString = $"跳出步进：[{config.Values[j].Value.DataInfo.Name}][{config.Values[j].Value.DataInfo.Value}]大于[{Values[j].ConditionValue}]";
                                                //    SuperDHHLoggerManager.Info(LoggerType.TESTLOG, nameof(MainViewModel), nameof(DCLoadExecute), InfoString);
                                                //    return OperateResult.Succeed();
                                                //}
                                                break;
                                            case "LESSTHAN"://小于
                                                results.Add(readdata < config.Values[j].ConditionValue ? true : false);
                                                //if (config.Values[j].Value.DataInfo.Value < config.Values[j].ConditionValue)
                                                //    {
                                                //        InfoString = $"跳出步进：[{config.Values[j].Value.DataInfo.Name}][{config.Values[j].Value.DataInfo.Value}]小于[{config.Values[j].ConditionValue}]";
                                                //        SuperDHHLoggerManager.Info(LoggerType.TESTLOG, nameof(MainViewModel), nameof(DCLoadExecute), InfoString);
                                                //        return OperateResult.Succeed();
                                                //    }
                                                break;
                                            case "EQUALTO"://等于
                                                results.Add(readdata == config.Values[j].ConditionValue ? true : false);
                                                //if (config.Values[j].Value.DataInfo.Value == config.Values[j].ConditionValue)
                                                //    {
                                                //        InfoString = $"跳出步进：[{config.Values[j].Value.DataInfo.Name}][{config.Values[j].Value.DataInfo.Value}]等于[{config.Values[j].ConditionValue}]";
                                                //        SuperDHHLoggerManager.Info(LoggerType.TESTLOG, nameof(MainViewModel), nameof(DCLoadExecute), InfoString);
                                                //        return OperateResult.Succeed();
                                                //    }
                                                break;
                                            case "NOTEQUALTO"://不等于
                                                results.Add(readdata != config.Values[j].ConditionValue ? true : false);
                                                //if (config.Values[j].Value.DataInfo.Value != config.Values[j].ConditionValue)
                                                //    {
                                                //        InfoString = $"跳出步进：[{config.Values[j].Value.DataInfo.Name}][{config.Values[j].Value.DataInfo.Value}]不等于[{config.Values[j].ConditionValue}]";
                                                //        SuperDHHLoggerManager.Info(LoggerType.TESTLOG, nameof(MainViewModel), nameof(DCLoadExecute), InfoString);
                                                //        return OperateResult.Succeed();
                                                //    }
                                                break;
                                            default:
                                                break;
                                        }
                                    }
                                    //}

                                    if (results.All(x => x == true))
                                    {
                                        InfoString = $"达成条件，跳出步进";
                                        SuperDHHLoggerManager.Info(LoggerType.TESTLOG, nameof(MainViewModel), nameof(DCLoadExecute), InfoString);
                                        return OperateResult.Succeed();
                                    }
                                    else if (results.Any(x => x == true) && config.StepCondition == StepConditions.OR)
                                    {
                                        InfoString = $"达成条件，跳出步进";
                                        SuperDHHLoggerManager.Info(LoggerType.TESTLOG, nameof(MainViewModel), nameof(DCLoadExecute), InfoString);
                                        return OperateResult.Succeed();
                                    }
                                }


                            }
                        }
                        else
                        {
                            InfoString = $"步进操作：步进目标值和初始值相同，无需进行步进操作";
                            SuperDHHLoggerManager.Info(LoggerType.TESTLOG, nameof(MainViewModel), nameof(DCLoadExecute), InfoString);
                        }
                    }
                    //else
                    //{
                    //    OperateResult SetRlt = operate.SetModelAndValue(Set_Model, Set_LoadValue);

                    //    if (!SetRlt)
                    //    {
                    //        LastError = $"设备参数设置异常：【{SetRlt.Message}】";
                    //        Logger.Error(ClassType.Name, FuntionName, LastError);

                    //        //Logger.Error(nameof(FunConfig_DCLoad), nameof(Execute), LastError);
                    //        return OperateResult.Failed(LastError);
                    //    }

                    //    InfoString = $"设置\n[模式]{Set_Model}\n[拉载值]{Set_LoadValue}成功";
                    //    Logger.Info(ClassType.Name, FuntionName, InfoString);

                    //    //Logger.Info(nameof(FunConfig_DCLoad), nameof(Execute), InfoString);
                    //}

                    //if (OperateType != SetOutPutState.Null)
                    //{
                    //    operate.SetOnOff(OperateType == SetOutPutState.ON ? DeviceOutPutState.ON : DeviceOutPutState.OFF);

                    //    InfoString = OperateType == SetOutPutState.ON ? "打开" : "关闭";
                    //    Logger.Info(ClassType.Name, FuntionName, $"设备已{InfoString}");

                    //    //Logger.Info(nameof(FunConfig_DCLoad), nameof(Execute), $"设备已{InfoString}");
                    //}

                }
                Thread.Sleep(config.DelayAfterRun > 0 ? config.DelayAfterRun : 0);
                return OperateResult.Succeed();

            }
            catch (Exception ex)
            {
                SuperDHHLoggerManager.Exception(LoggerType.TESTLOG, nameof(MainViewModel), nameof(DCLoadExecute), ex);
                return OperateResult.Excepted(ex);
            }
        }

        /// <summary>
        /// 交流源试验执行
        /// </summary>
        /// <param name="config"></param>
        /// <param name="device"></param>
        /// <param name="canoperate"></param>
        /// <param name="aqcoperate"></param>
        /// <returns></returns>
        private OperateResult ACScourceExecute(FunConfig_ACSource config, IACSource device, Product_CAN_Operate canoperate, IAcquisition aqcoperate) 
        {
            string errorstring = string.Empty;
            try
            {
                if (device == null)
                {
                    errorstring = "设备类型不存在";
                    SuperDHHLoggerManager.Error(LoggerType.TESTLOG, nameof(MainViewModel), nameof(ACScourceExecute), errorstring);
                    return OperateResult.Failed(errorstring);
                }

                if (config == null)
                {
                    errorstring = "交流源配置不可为空";
                    SuperDHHLoggerManager.Error(LoggerType.TESTLOG, nameof(MainViewModel), nameof(ACScourceExecute), errorstring);
                    return OperateResult.Failed(errorstring);
                }
                if (aqcoperate == null)
                {
                    errorstring = "采样系统不存在";
                    SuperDHHLoggerManager.Error(LoggerType.TESTLOG, nameof(MainViewModel), nameof(ACScourceExecute), errorstring);
                    return OperateResult.Failed(errorstring);
                }

                string InfoString = string.Empty;

                double loadvalue = config.EnableStepping ? config.StartLoadValue : config.Set_Vol;

                OperateResult SetRlt = device.SetValue_Singel(loadvalue, config.Set_Frequency);

                if (!SetRlt)
                {
                    errorstring = $"设备参数设置异常：【{SetRlt.Message}】";
                    SuperDHHLoggerManager.Error(LoggerType.TESTLOG, nameof(MainViewModel), nameof(ACScourceExecute), errorstring);
                    return OperateResult.Failed(errorstring);
                }

                InfoString = $"设置\n[电压]{config.Set_Vol}\n[频率]{config.Set_Frequency}成功";
                //Logger.Info(ClassType.Name, FuntionName, InfoString);
                SuperDHHLoggerManager.Info(LoggerType.TESTLOG, nameof(MainViewModel), nameof(ACScourceExecute), InfoString);

                if (config.OperateType != SetOutPutState.Null)
                {
                    device.SetOnOff(config.OperateType == SetOutPutState.ON ? DeviceOutPutState.ON : DeviceOutPutState.OFF);

                    InfoString = config.OperateType == SetOutPutState.ON ? "打开" : "关闭";
                    SuperDHHLoggerManager.Info(LoggerType.TESTLOG, nameof(MainViewModel), nameof(ACScourceExecute), $"设备已{InfoString}");

                    //Logger.Info(ClassType.Name, Executename, $"设备已{InfoString}");
                }

                //步进模式
                if (config.EnableStepping)
                {
                    Thread.Sleep(config.StepFrequency);

                    Dictionary<string, ModDeviceReadData> data = new Dictionary<string, ModDeviceReadData>();

                    if (canoperate != null)
                    {
                        for (int i = 0; i < canoperate?.ReadInfos?.Count; i++)
                        {
                            data.Add(canoperate.ReadInfos[i].DataInfo.Name, canoperate.ReadInfos[i]);
                        }
                    }

                    for (int i = 0; i < aqcoperate?.ReadInfos?.Count; i++)
                    {
                        data.Add(aqcoperate.ReadInfos[i].DataInfo.Name, aqcoperate.ReadInfos[i]);
                    }

                    if (Math.Abs(config.EndLoadValue - config.StartLoadValue) >= config.Stride)
                    {
                        int count = Convert.ToInt16(Math.Abs((((config.EndLoadValue - config.StartLoadValue) % config.Stride) == 0) ? ((config.EndLoadValue - config.StartLoadValue) / config.Stride) : ((config.EndLoadValue - config.StartLoadValue) / config.Stride + 1)));
                        for (int i = 0; i < count; i++)
                        {
                            if (monitor!= null && (!monitor.IsStartThread))
                            {
                                SuperDHHLoggerManager.Info(LoggerType.TESTLOG, nameof(MainViewModel), nameof(ACScourceExecute), "手动退出步进");
                                break;
                            }
                            double setpvol = config.StartLoadValue + config.Stride * (i + 1) * (config.EndLoadValue < config.StartLoadValue ? -1 : 1);

                            setpvol = config.EndLoadValue > config.StartLoadValue ? Math.Min(setpvol, config.EndLoadValue) : Math.Max(setpvol, config.EndLoadValue);

                            device.SetValue_Singel(setpvol ,config.Set_Frequency);

                            InfoString = $"设备步进设置拉载值{setpvol}成功";

                            SuperDHHLoggerManager.Info(LoggerType.TESTLOG, nameof(MainViewModel), nameof(ACScourceExecute), InfoString);

                            Thread.Sleep(config.StepFrequency);

                            if (canoperate != null)
                            {
                                canoperate?.Device_ReadAll();
                            }

                            aqcoperate?.Device_ReadAll();

                            //跳出步进条件
                            if (config.Values  != null&& config.Values.Count > 0)
                            {
                                //if (config.StepCondition == StepConditions.OR)
                                //{
                                List<bool> results = new List<bool>();
                                for (int j = 0; j < config.Values.Count; j++)
                                {
                                    if (config.Values[j].Value == null) 
                                    {
                                        continue;
                                    }
                                    double readdata = data[config.Values[j].Value.DataInfo.Name].DataInfo.Value;
                                    switch (config.Values[j].StepValueCondition.ToString())
                                    {
                                        case "GREATERTHAN"://大于

                                            results.Add(readdata > config.Values[j].ConditionValue ? true : false);
                                            break;
                                        case "LESSTHAN"://小于
                                            results.Add(readdata < config.Values[j].ConditionValue ? true : false);
                                            break;
                                        case "EQUALTO"://等于
                                            results.Add(readdata == config.Values[j].ConditionValue ? true : false);
                                            break;
                                        case "NOTEQUALTO"://不等于
                                            results.Add(readdata != config.Values[j].ConditionValue ? true : false);
                                            break;
                                        default:
                                            break;
                                    }
                                }
                                //}

                                if (results.All(x => x == true))
                                {
                                    InfoString = $"达成条件，跳出步进";
                                    SuperDHHLoggerManager.Info(LoggerType.TESTLOG, nameof(MainViewModel), nameof(ACScourceExecute), InfoString);
                                    return OperateResult.Succeed();
                                }
                                else if (results.Any(x => x == true) && config.StepCondition == StepConditions.OR)
                                {
                                    InfoString = $"达成条件，跳出步进";
                                    SuperDHHLoggerManager.Info(LoggerType.TESTLOG, nameof(MainViewModel), nameof(ACScourceExecute), InfoString);
                                    return OperateResult.Succeed();
                                }
                            }


                        }
                    }
                    else
                    {
                        InfoString = $"步进操作：步进目标值和初始值相同，无需进行步进操作";
                        SuperDHHLoggerManager.Info(LoggerType.TESTLOG, nameof(MainViewModel), nameof(ACScourceExecute), InfoString);
                    }
                }
                Thread.Sleep(config.DelayAfterRun > 0 ? config.DelayAfterRun : 0);
                return OperateResult.Succeed();
            }
            catch (Exception ex)
            {
                SuperDHHLoggerManager.Exception(LoggerType.TESTLOG, nameof(MainViewModel), nameof(ACScourceExecute), ex);
                return OperateResult.Excepted(ex);
            }

        }
        #endregion

        #region 试验条目存储

        private readonly object objLock = new object();
        /// <summary>
        /// 数据条目存储
        /// </summary>
        private OperateResult<string> SaveItem(bool isstart, Test_ItemInfo info)
        {
            lock (objLock)
            {
                if (isstart)
                {
                    return DBOperate.Default.InserTestItem(info);
                }
                else
                {
                    var rlt = DBOperate.Default.UpdateTetsItem(info);
                    if (!rlt)
                    {
                        return OperateResult<string>.Failed(string.Empty, rlt.Message);
                    }
                    return OperateResult<string>.Succeed(string.Empty);
                }
            }
        }
        #endregion

        #region CAN通道操作
        /// <summary>
        /// CAN通道初始化
        /// </summary>
        /// <param name="config">配置文件</param>
        /// <returns></returns>
        private OperateResult<IDeviceOperate> CANInt(DeviceArgs config)
        {
            //lock (GlobalModel.DicDeviceDrives)
            //{
            if (!GlobalModel.DicDeviceDrives.TryGetValue(config.DriveConfig.ResourceString, out DriveOperate drive))
            {
                DriveOperate operate = DriveOperate.Creat();
                OperateResult result = operate.Open(config.DriveConfig);
                if (!result)
                {
                    return OperateResult<IDeviceOperate>.Failed(null, result.Message);
                }
                GlobalModel.DicDeviceDrives.Add(config.DriveConfig.ResourceString, operate);
                drive = operate;
                GlobalModel.DicDeviceInfo["Product"].Drive = drive;
            }

            return drive.RegisterDevice(config)
                     .ThenAnd(result => result.Data.Init(config, drive.Drive).ConvertTo(result.Data))
                     .AttachIfSucceed(result =>
                     {
                         GlobalModel.DicDeviceOperate.Add(result.Data, drive);

                     });
        }

        /// <summary>
        /// CAN通道反初始化
        /// </summary>
        /// <param name="operate">采样系统操作句柄</param>
        /// <returns></returns>
        private OperateResult CANUnint(IDeviceOperate operate)
        {
            try
            {
                //lock (GlobalModel.DicDeviceOperate)
                //{
                OperateResult result = GlobalModel.DicDeviceOperate[operate].UnregisterDevice(operate);

                if (GlobalModel.DicDeviceOperate[operate].CanRemove)
                {
                    GlobalModel.DicDeviceDrives.Remove(operate.DeviceConfig.DriveConfig.ResourceString);
                }


                //if (GlobalModel.DicDeviceOperate[operate].Devices.Count < 1)
                //{
                //    GlobalModel.DicDeviceOperate[operate].Close();
                //    GlobalModel.DicDeviceDrives.Remove(operate.DeviceConfig.DriveConfig.ResourceString);
                //}

                GlobalModel.DicDeviceOperate.Remove(operate);
                //operate?.Dispose();

                return result;

                //return GlobalModel.DicDeviceOperate[operate].UnregisterDevice(operate)
                //    .AttachIfSucceed(result =>
                //    {
                //        if (GlobalModel.DicDeviceOperate[operate].CanRemove)
                //        {
                //            GlobalModel.DicDeviceDrives.Remove(operate.DeviceConfig.DriveConfig.ResourceString);
                //        }
                //        GlobalModel.DicDeviceOperate.Remove(operate);
                //        operate?.Dispose();
                //        //operate = null;
                //    });
                //}

            }
            catch (Exception ex)
            {
                SuperDHHLoggerManager.Exception(LoggerType.FROMLOG, nameof(MainViewModel), nameof(CANUnint), ex);
                return OperateResult.Failed("CAN通道卸载失败");
            }
        }
        #endregion

        /// <summary>
        /// 下电操作
        /// </summary>
        private void ShutDown(List<string> steps, bool isusedbc) 
        {
            for (int i = 0; i < steps?.Count; i++) 
            {
                DeviceInfo_ALL info = null;
                switch (steps[i])
                {
                    case "直流负载下电关机":
                    {
                        GlobalModel.DicDeviceInfo.TryGetValue("DCLoad", out info);
                    }
                        break;
                    case "高压直流源下电关机":
                    {
                        GlobalModel.DicDeviceInfo.TryGetValue("HVDCSource", out info);
                    }
                        break;

                    case "交流源下电关机":
                    {
                        GlobalModel.DicDeviceInfo.TryGetValue("ACSource", out info);
                    }
                        break;
                    case "稳压直流源下电关机":
                    {
                        GlobalModel.DicDeviceInfo.TryGetValue("APU", out info);
                    }
                        break;
                    case "产品通讯卸载":
                        if (!isusedbc) 
                        {
                            GlobalModel.DicDeviceInfo.TryGetValue("Product", out info);
                        }
                        break;
                    default:
                        break;
                }

                if (info == null || info.DeviceOperate == null) 
                {
                    continue;
                }

                info.DeviceOperate.Close();

            }
        }

        /// <summary>
        /// 结束试验
        /// </summary>

        private void TestStop()
        {
            TestErrorString += "手动停止试验";
            if (monitor != null)
            {
                monitor.IsStartThread = false;
                int timeOut = 5 * 60;
                while (timeOut-- > 0)
                {
                    if (!monitor.IsRunning)
                    {
                        break;
                    }

                    Thread.Sleep(100);
                }
            }

            ContentName = "手动停止试验";
            ContentColor = Brushes.Red;
            TestResult = "FAIL";
            TestResultColor = Brushes.Red;
            GlobalModel.IsTestThreadRun = false;
            IsTestRuning = false;
            IsFocuse = true;
        }

        #endregion

        #region 界面测试
        private void TestThread() 
        {   
            for (int i = 0; i < 10; i++) 
            {
                ObservableCollection<ExecuteStepInfo> stepinfo = new ObservableCollection<ExecuteStepInfo>();

                ExecuteInfo functioninfo = new ExecuteInfo
                {
                    Index = i + 1,
                    Result = ResultState.UNACCOMPLISHED,
                    StartTime = DateTime.Now.ToString("HH:mm:ss"),
                    StepInfos = stepinfo,
                    FunctionName = "test1",
                };


                Application.Current.Dispatcher.Invoke(new Action(() =>
                {
                    ATEExecuteInfos.Add(functioninfo);
                }));
                Thread.Sleep(100);
                for (int k = 0; k < 5; k++)
                {
                    ExecuteStepInfo step = new ExecuteStepInfo
                    {
                        Result = ResultState.UNACCOMPLISHED,
                        ExecuteTime = DateTime.Now.ToString("HH:mm:ss"),
                        StepName = "KL30上电",
                    };

                    Application.Current.Dispatcher.Invoke(new Action(() =>
                    {
                        stepinfo.Add(step);
                    }));
                    Thread.Sleep(500);
                    Application.Current.Dispatcher.Invoke(new Action(() =>
                    {
                        step.Result = ResultState.SUCCESS;
                    }));
                }
                

                //Application.Current.Dispatcher.Invoke(new Action(() =>
                //{
                //    ATEExecuteInfos.Add(new ExecuteInfo {
                //    Index = i + 1,
                //    FunctionName = "硬件唤醒测试",
                //    Result = ResultState.FAIL,
                //    StartTime = DateTime.Now.ToString("HH:mm:ss"),
                //    StepInfos = new ObservableCollection<ExecuteStepInfo> {
                //                new ExecuteStepInfo {
                //                    StepName = "KL30上电",
                //                    ExecuteTime = DateTime.Now.ToString("HH:mm:ss"),
                //                    Limit_Lower = "11",
                //                    Limit_Upper = "12.5",
                //                    Result = ResultState.FAIL,
                //                },
                //                new ExecuteStepInfo {
                //                    StepName = "KL15导通",
                //                    ExecuteTime = DateTime.Now.AddMinutes(1).ToString("HH:mm:ss"),
                //                    Limit_Lower = "1",
                //                    Limit_Upper = "1",
                //                    Result = ResultState.FAIL,
                //                },
                //                 new ExecuteStepInfo {
                //                    StepName = "样品通讯上报",
                //                    ExecuteTime = DateTime.Now.AddMinutes(2).ToString("HH:mm:ss"),
                //                    Limit_Lower = string.Empty,
                //                    Limit_Upper = string.Empty,
                //                    Result = ResultState.FAIL,
                //                },
                //            }

                //    });
                //}));

                //Thread.Sleep(1000);
            }

        }
        #endregion

#endregion

        #region 保护方法
        protected override void WindowMaxExecute(object obj)
        {
            base.WindowMaxExecute(obj);
        }
        protected override void WindowLoadedExecute(object obj)
        {
            if (!(obj is Window win))
            {
                return;
            }


            WindowLeftDown_MoveEvent.LeftDown_MoveEventRegister(win);

            if (!DBOperate.Default.IsInitOK)
            {
                DBOperate.Default.Init();
            }

            DBOperate.Default.GetProjectNames()
                .AttachIfSucceed(result =>
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        ProdectNames.Clear();

                        for (int i = 0; i < result.Data.Count; i++)
                        {
                            string name = result.Data[i];
                            ProdectNames.Add(name);
                        }
                    });
                })
                .AttachIfFailed(result =>
                {
                    MessageBox.Show($"项目信息获取失败,请重启操作平台\r\n{result.Message}", "项目信息");
                });

            if (GlobalModel.CabinetSate)
            {
                relayoperate_4 = GlobalModel.DicDeviceInfo["Relay"].DeviceOperate as Relay_ZS4Bit_Operate;
            }
#if DEBUG
            //new Thread(TestThread) { IsBackground = true }.Start();
#endif
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

            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                Window mainwindow = ContentControlManager.GetWindow<DeviceInitView>(((ViewModelLocator)Application.Current.FindResource("Locator")).Init);
                mainwindow.Show();
            }));

            base.WindowClosedExecute(obj);
        }
        #endregion


        #region 构造方法

        public MainViewModel()
        {
#if DEBUG
            GlobalModel.CabinetSate = true;
#endif
            if (!GlobalModel.CabinetSate)
            {
                ContentName = "系统设备存在故障，无法运行";
                ContentColor = Brushes.Red;
            }
            //#if DEBUG
            //SelectedProductName = "OBC测试800W";
            //ProductSN = "35L2024091101020S";
            //ATEExecuteInfos.Add(new ExecuteInfo
            //{
            //    Index = 1,
            //    FunctionName = "硬件唤醒测试",
            //    Result = ResultState.FAIL,
            //    StartTime = DateTime.Now.ToString("HH:mm:ss"),
            //    StepInfos = new ObservableCollection<ExecuteStepInfo> {
            //                    new ExecuteStepInfo {
            //                        StepName = "KL30上电",
            //                        ExecuteTime = DateTime.Now.ToString("HH:mm:ss"),
            //                        Limit_Lower = "11",
            //                        Limit_Upper = "12.5",
            //                        Result = ResultState.FAIL,
            //                    },
            //                    new ExecuteStepInfo {
            //                        StepName = "KL15导通",
            //                        ExecuteTime = DateTime.Now.AddMinutes(1).ToString("HH:mm:ss"),
            //                        Limit_Lower = "1",
            //                        Limit_Upper = "1",
            //                        Result = ResultState.FAIL,
            //                    },
            //                     new ExecuteStepInfo {
            //                        StepName = "样品通讯上报",
            //                        ExecuteTime = DateTime.Now.AddMinutes(2).ToString("HH:mm:ss"),
            //                        Limit_Lower = string.Empty,
            //                        Limit_Upper = string.Empty,
            //                        Result = ResultState.FAIL,
            //                    },
            //                }

            //});

            //ATEExecuteInfos.Add(new ExecuteInfo
            //{
            //    Index = 2,
            //    FunctionName = "CC唤醒测试",
            //    Result = ResultState.SUCCESS,
            //    StartTime = DateTime.Now.ToString("HH:mm:ss"),
            //    StepInfos = new ObservableCollection<ExecuteStepInfo> {
            //                    new ExecuteStepInfo {
            //                        StepName = "test1",
            //                        ExecuteTime = DateTime.Now.ToString("HH:mm:ss"),
            //                        Limit_Lower = string.Empty,
            //                        Limit_Upper = string.Empty,
            //                        Result = ResultState.SUCCESS,
            //                    },
            //                    new ExecuteStepInfo {
            //                        StepName = "test2",
            //                        ExecuteTime = DateTime.Now.AddMinutes(1).ToString("HH:mm:ss"),
            //                        Limit_Lower = "1",
            //                        Limit_Upper = "3",
            //                        Result = ResultState.SUCCESS,
            //                    },
            //                     new ExecuteStepInfo {
            //                        StepName = "test3",
            //                        ExecuteTime = DateTime.Now.AddMinutes(2).ToString("HH:mm:ss"),
            //                        Limit_Lower = string.Empty,
            //                        Limit_Upper = string.Empty,
            //                        Result = ResultState.FAIL,
            //                    },
            //                }
            //});

            //ATEExecuteInfos.Add(new ExecuteInfo
            //{
            //    Index = 2,
            //    FunctionName = "CP唤醒测试",
            //    Result = ResultState.UNACCOMPLISHED,
            //    StartTime = DateTime.Now.ToString("HH:mm:ss"),
            //    StepInfos = new ObservableCollection<ExecuteStepInfo> {
            //                    new ExecuteStepInfo {
            //                        StepName = "test1",
            //                        ExecuteTime = DateTime.Now.ToString("HH:mm:ss"),
            //                        Limit_Lower = string.Empty,
            //                        Limit_Upper = string.Empty,
            //                        Result = ResultState.SUCCESS,
            //                    },
            //                    new ExecuteStepInfo {
            //                        StepName = "test2",
            //                        ExecuteTime = DateTime.Now.AddMinutes(1).ToString("HH:mm:ss"),
            //                        Limit_Lower = "1",
            //                        Limit_Upper = "3",
            //                        Result = ResultState.SUCCESS,
            //                    },
            //                     new ExecuteStepInfo {
            //                        StepName = "test3",
            //                        ExecuteTime = DateTime.Now.AddMinutes(2).ToString("HH:mm:ss"),
            //                        Limit_Lower = string.Empty,
            //                        Limit_Upper = string.Empty,
            //                        Result = ResultState.FAIL,
            //                    },
            //                }
            //});

            //ATEExecuteInfos.Add(new ExecuteInfo
            //{
            //    Index = 2,
            //    FunctionName = "额定功率",
            //    Result = ResultState.UNACCOMPLISHED,
            //    StartTime = DateTime.Now.ToString("HH:mm:ss"),
            //    StepInfos = new ObservableCollection<ExecuteStepInfo> {
            //                    new ExecuteStepInfo {
            //                        StepName = "test1",
            //                        ExecuteTime = DateTime.Now.ToString("HH:mm:ss"),
            //                        Limit_Lower = string.Empty,
            //                        Limit_Upper = string.Empty,
            //                        Result = ResultState.SUCCESS,
            //                    },
            //                    new ExecuteStepInfo {
            //                        StepName = "test2",
            //                        ExecuteTime = DateTime.Now.AddMinutes(1).ToString("HH:mm:ss"),
            //                        Limit_Lower = "1",
            //                        Limit_Upper = "3",
            //                        Result = ResultState.SUCCESS,
            //                    },
            //                     new ExecuteStepInfo {
            //                        StepName = "test3",
            //                        ExecuteTime = DateTime.Now.AddMinutes(2).ToString("HH:mm:ss"),
            //                        Limit_Lower = string.Empty,
            //                        Limit_Upper = string.Empty,
            //                        Result = ResultState.FAIL,
            //                    },
            //                }
            //});

            //ATEExecuteInfos.Add(new ExecuteInfo
            //{
            //    Index = 2,
            //    FunctionName = "输出电压过压保护",
            //    Result = ResultState.UNACCOMPLISHED,
            //    StartTime = DateTime.Now.ToString("HH:mm:ss"),
            //    StepInfos = new ObservableCollection<ExecuteStepInfo> {
            //                    new ExecuteStepInfo {
            //                        StepName = "test1",
            //                        ExecuteTime = DateTime.Now.ToString("HH:mm:ss"),
            //                        Limit_Lower = string.Empty,
            //                        Limit_Upper = string.Empty,
            //                        Result = ResultState.SUCCESS,
            //                    },
            //                    new ExecuteStepInfo {
            //                        StepName = "test2",
            //                        ExecuteTime = DateTime.Now.AddMinutes(1).ToString("HH:mm:ss"),
            //                        Limit_Lower = "1",
            //                        Limit_Upper = "3",
            //                        Result = ResultState.SUCCESS,
            //                    },
            //                     new ExecuteStepInfo {
            //                        StepName = "test3",
            //                        ExecuteTime = DateTime.Now.AddMinutes(2).ToString("HH:mm:ss"),
            //                        Limit_Lower = string.Empty,
            //                        Limit_Upper = string.Empty,
            //                        Result = ResultState.FAIL,
            //                    },
            //                }
            //});

            //ATEExecuteInfos.Add(new ExecuteInfo
            //{
            //    Index = 2,
            //    FunctionName = "输出电压欠压保护",
            //    Result = ResultState.UNACCOMPLISHED,
            //    StartTime = DateTime.Now.ToString("HH:mm:ss"),
            //    StepInfos = new ObservableCollection<ExecuteStepInfo> {
            //                    new ExecuteStepInfo {
            //                        StepName = "test1",
            //                        ExecuteTime = DateTime.Now.ToString("HH:mm:ss"),
            //                        Limit_Lower = string.Empty,
            //                        Limit_Upper = string.Empty,
            //                        Result = ResultState.SUCCESS,
            //                    },
            //                    new ExecuteStepInfo {
            //                        StepName = "test2",
            //                        ExecuteTime = DateTime.Now.AddMinutes(1).ToString("HH:mm:ss"),
            //                        Limit_Lower = "1",
            //                        Limit_Upper = "3",
            //                        Result = ResultState.SUCCESS,
            //                    },
            //                     new ExecuteStepInfo {
            //                        StepName = "test3",
            //                        ExecuteTime = DateTime.Now.AddMinutes(2).ToString("HH:mm:ss"),
            //                        Limit_Lower = string.Empty,
            //                        Limit_Upper = string.Empty,
            //                        Result = ResultState.FAIL,
            //                    },
            //                }
            //});
            //#endif

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
            set
            {
                if (Set(nameof(Result), ref result, value))
                {
                    //Forecolor = Result == ResultState.SUCCESS ? new SolidColorBrush(Colors.Green) : new SolidColorBrush(Colors.Red);
                }
            }
        }

        //private Brush forecolor;
        ///// <summary>
        ///// 结果字体颜色
        ///// </summary>
        //public Brush Forecolor
        //{
        //    get => forecolor;
        //    set
        //    {
        //        Set(nameof(Forecolor), ref forecolor, value);
        //    }
        //}

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
        /// 当前值
        /// </summary>
        public string NowVlaue {  get; set; }

        /// <summary>
        /// 下限值
        /// </summary>
        public string Limit_Lower { get; set; }

        /// <summary>
        /// 上限值
        /// </summary>
        public string Limit_Upper { get; set; }

        /// <summary>
        /// 判定条件
        /// </summary>
        public string ValueConditions { get; set; }

        private ResultState result;
        /// <summary>
        /// 执行结果
        /// </summary>
        public ResultState Result
        {
            get => result;
            set
            {
                if (Set(nameof(Result), ref result, value))
                {
                    //switch (value)
                    //{
                    //    case ResultState.UNACCOMPLISHED:
                    //        Forecolor = new SolidColorBrush(Colors.Black);
                    //        break;
                    //    case ResultState.SUCCESS:
                    //        Forecolor = new SolidColorBrush(Colors.Green);
                    //        break;
                    //    case ResultState.FAIL:
                    //        Forecolor = new SolidColorBrush(Colors.Red);
                    //        break;
                    //        default:
                    //        break;
                    //}
                    Forecolor = value== ResultState.SUCCESS?new SolidColorBrush(Colors.Green) :new SolidColorBrush(Colors.Red);
                }

            }
        }

        /// <summary>
        /// 开始时间
        /// </summary>
        public string ExecuteTime { get; set; }

        private Brush forecolor = new SolidColorBrush(Colors.Black);
        /// <summary>
        /// 结果字体颜色
        /// </summary>
        public Brush Forecolor
        {
            get => forecolor;
            set => Set(nameof(Forecolor), ref forecolor, value);
        }
    }


    public class MonitorThread
    {
        /// <summary>
        /// 试验名称
        /// </summary>
        public string ThreadName { get; set; }

        /// <summary>
        /// 线程Guid
        /// </summary>
        public string ThreadID { get; set; }

        /// <summary>
        /// 运行线程
        /// </summary>
        public Thread OprateThread { get; set; }

        /// <summary>
        /// 线程是否继续运行
        /// </summary>
        public bool IsStartThread { get; set; } = true;

        /// <summary>
        /// 线程运行状态
        /// </summary>
        public bool IsRunning { get; set; } = false;

        /// <summary>
        /// 试验运行流程
        /// </summary>

        /// <summary>
        /// 项目信息
        /// </summary>
        public Test_ProjectInfo ProjectInfo { get; set; }

        /// <summary>
        /// DBC文件
        /// </summary>
        public Test_DBCFileInfo DBCFile { get; set; }

        /// <summary>
        /// CAN上报列表
        /// </summary>
        public Dictionary<string, string> DicReadSignals_CAN { get; set; }

        /// <summary>
        /// CAN下发列表
        /// </summary>
        public List<string> SendSignals_CAN { get; set; }

        /// <summary>
        /// 试验运行流程
        /// </summary>
        public Test_Programme Programme { get; set; }

        /// <summary>
        /// 试验运行流程
        /// </summary>
        public Dictionary<string, List<TestFunction>> TestFlowsFunction = new Dictionary<string, List<TestFunction>>();


    }
}
