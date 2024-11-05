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
using IMX.Function.Config;
using System.Windows.Documents;

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
        public RelayCommand Test => new RelayCommand(DoTest);

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
        // private Window window;
        #endregion

        #region 私有方法

        private void DoTest()
        {
            if (!GlobalModel.CabinetSate)
            {
                MessageBox.Show("当前工装存在故障设备，不支持开启试验","工装异常");
                return;
            }

            if (IsTestRuning)
            {
                if (MessageBox.Show("是否暂停试验?", "试验停止确认", MessageBoxButton.OKCancel, MessageBoxImage.Question) == MessageBoxResult.Cancel)
                {
                    return;
                }
                TestStop();
            }
            else
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

                if (MessageBox.Show("是否开始试验?", "试验开始确认", MessageBoxButton.OKCancel, MessageBoxImage.Question) == MessageBoxResult.Cancel)
                {
                    return;
                }

                TestStart();
            }
            //EnableTestInfoChange =! EnableTestInfoChange;
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
            Dictionary<string, List<TestFunction>> process = null;
            Dictionary<string, string> dicreadsignals = new Dictionary<string, string>();
            string error = string.Empty;

            Test_ProjectInfo data = rlt.Data;
            

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

                try
                {
                    if (issucced)
                    {
                        for (int i = 0; i < dbcconfig?.Test_DBCReceiveSignals?.Count; i++)
                        {
                            var signal = dbcconfig.Test_DBCReceiveSignals[i];
                            dicreadsignals.Add(signal.Signal_Name, signal.Custom_Name);
                        }
                    }
                }
                catch (Exception ex)
                {
                    error = $"DBC上报列表配置获取失败：\r\n{ex.GetMessage()}";
                    issucced = false;
                }
            }

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


            //线程参数初始化
            monitor = new MonitorThread
            {
                ThreadName = $"{data.ProjectName}_{ProductSN}",
                ThreadID = Guid.NewGuid().ToString(),
                OprateThread = new Thread(TestRun) { IsBackground = true },
                ProjectInfo = data,
                DBCFile = dbcfileinfo,
                DicReadSignals_CAN = dicreadsignals,
                Programme = programme,
                TestFlowsFunction = process,
            };

            monitor.OprateThread.Start(monitor);

            ContentName = "运行试验中...";
            ContentColor = Brushes.LightGreen;
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
            Test_DataInfo test_Data = new Test_DataInfo();

            //CAN通讯初始化
            if (thread.ProjectInfo.IsUseDDBC)
            {
                Application.Current.Dispatcher.Invoke(new Action(() =>
                {
                    StepStr = "正在初始化CAN";
                    StepStrShow = Visibility.Visible;
                }));

                Thread.Sleep(100);
                try
                {
                    GlobalModel.DicDeviceInfo["Product"].Args.DriveConfig.BaudRate = thread.ProjectInfo.BaudRate;
                    CANInt(GlobalModel.DicDeviceInfo["Product"].Args)
                        .ThenAnd(result =>
                    {
                        Product_CAN_Operate operate = (result.Data as Product_CAN_Operate);
                       return operate
                        .SetReadSignal(thread.DicReadSignals_CAN)
                        .And(operate.SetSendSignal(thread.SendSignals_CAN))
                        .ConvertTo(result.Data);
                    });

                    test_Data.Pro_DeviceRead = GlobalModel.DicDeviceInfo["Product"].DeviceOperate.ReadInfos;
                }
                catch (Exception ex)
                {
                    GlobalModel.IsTestThreadRun = false;
                    thread.IsRunning = false;
                    SuperDHHLoggerManager.Exception(LoggerType.THREAD, nameof(MainViewModel), nameof(TestRun), ex);
                    MessageBox.Show(ex.GetMessage(),"CAN初始化异常");

                    Application.Current.Dispatcher.Invoke(new Action(() =>
                    {
                        ContentName = "CAN通讯初始化失败";
                        ContentColor = Brushes.Red;
                        StepStrShow = Visibility.Collapsed;
                    }));

                    return;
                }
            }

            test_Data.Euq_DeviceRead = GlobalModel.DicDeviceInfo["Acquisition"].DeviceOperate.ReadInfos;
            bool testresult = true;
            string errorstr = string.Empty;

            Test_ItemInfo testinfo = new Test_ItemInfo 
            {
                 ProductSN = ProductSN,
                 ProjectID = thread.ProjectInfo.Id,
                 Operator = UserName,
            };

            SaveItem(true, testinfo);

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
                    StepInfos = stepinfo
                };


                Application.Current.Dispatcher.Invoke(new Action(() =>
                {
                    ATEExecuteInfos.Add(functioninfo);
                }));
                
                for (int j = 0; j < flows?.Count; j++)
                {
                    if (!thread.IsStartThread)
                    {
                        break;
                    }


                    try
                    {
                        IFuntionConfig config = flows[j].Config;

                        IDeviceOperate operate = null;
                        if (config.DeviceAddress != null)
                        {
                            if (GlobalModel.DicDeviceInfo.TryGetValue(config.DeviceAddress.ToUpper(), out DeviceInfo_ALL info))
                            {
                                operate = info.DeviceOperate;
                            }
                        }

                        ExecuteStepInfo step = new ExecuteStepInfo 
                        {
                            Result = ResultState.UNACCOMPLISHED,
                            ExecuteTime = DateTime.Now.ToString("HH:mm:ss"),
                            StepName = config.SupportFuncitonType.GetDescription()
                        };

                        if (config.SupportFuncitonType == FuncitonType.ProductResult)
                        {
                            operate.Device_ReadAll();
                        }
                        else if (config.SupportFuncitonType == FuncitonType.EquipmentResult)
                        {
                            FunConfig_EquipmentResult resultconfig = config as FunConfig_EquipmentResult;
                            var result = EquipmentResultExecute(stepinfo, resultconfig, operate as IAcquisition);

                            test_Data.StepName = config.SupportFuncitonType.GetDescription();
                            test_Data.FlowName = flowname;

                            Task.Run(() => { DBOperate.Default.InserTestData(test_Data); });

                            if (!result)
                            {
                                TestErrorString += $"result.Message\r\n";
                                if (resultconfig.ResultOpereate != ResultOpereateType.IGNORE)
                                {
                                    testresult = false;
                                    thread.IsStartThread = resultconfig.ResultOpereate == ResultOpereateType.OUTFUNCTION;
                                    break;
                                }
                            }
                        }
                        else if (config.SupportFuncitonType == FuncitonType.DCLoad)
                        {
                            Application.Current.Dispatcher.Invoke(new Action(() =>
                            {
                                stepinfo.Add(step);
                            }));
                        }
                        else
                        {
                            config.Execute(operate);
                        }
                        Thread.Sleep(0);
                    }
                    catch (Exception ex)
                    {
                        thread.IsStartThread = false;
                        SuperDHHLoggerManager.Exception(LoggerType.FROMLOG, nameof(MainViewModel), nameof(TestRun), ex);
                        break;
                    }
                }
            }

            if (!string.IsNullOrEmpty(TestErrorString))
            {
                testinfo.ErrorInfo = TestErrorString;
                testinfo.Result = ResultState.FAIL;
                MessageBox.Show(TestErrorString, "试验失败");
            }
            else
            {
                testinfo.Result = ResultState.SUCCESS;
                Application.Current.Dispatcher.Invoke(new Action(() =>
                {
                    ContentName = string.Empty;
                    ContentColor = Brushes.Red;
                    StepStrShow = Visibility.Collapsed;
                }));
                MessageBox.Show("试验运行完成","试验成功");
            }

            SaveItem(false, testinfo);

            if (thread.ProjectInfo.IsUseDDBC)
            {
                CANUnint(GlobalModel.DicDeviceInfo["Product"].DeviceOperate);
            }
            thread.IsRunning = false;
        }

        #region 特殊步骤操作
        /// <summary>
        /// 产品试验结果执行
        /// </summary>
        /// <returns></returns>
        private OperateResult ProductResultExecute(ObservableCollection<ExecuteStepInfo> infos, FunConfig_ProductResult config, Product_CAN_Operate operate) 
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
                        Limit_Lower = data.Limits_Lower.ToString(),
                        Limit_Upper = data.Limits_Upper.ToString(),
                    };
                    config.Datas[i].DataInfo.Value = operate.DicReadInfo[config.Datas[i].DataInfo.Name].DataInfo.Value;

                    if (!config.Datas[i].IsInRange)
                    {
                        step.Result = ResultState.FAIL;
                        errorstring += $"{data.DataInfo.Name}当前读取值【{data.DataInfo.Value}】（判定条件{data.DataInfo}），不符合要求：{data.Limits_Lower} - {data.Limits_Upper}\r\n";
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
        private OperateResult EquipmentResultExecute(ObservableCollection<ExecuteStepInfo> infos, FunConfig_EquipmentResult config, IAcquisition operate)
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
                        Limit_Lower = data.Limits_Lower.ToString(),
                        Limit_Upper = data.Limits_Upper.ToString(),
                    };

                    config.Datas[i].DataInfo.Value = operate.DicReadInfo[config.Datas[i].DataInfo.Name].DataInfo.Value;

                    if (!config.Datas[i].IsInRange)
                    {
                        step.Result = ResultState.FAIL;
                        errorstring += $"{data.DataInfo.Name}当前读取值【{data.DataInfo.Value}】（判定条件{data.DataInfo}），不符合要求：{data.Limits_Lower} - {data.Limits_Upper}\r\n";
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
                    SuperDHHLoggerManager.Error( LoggerType.TESTLOG,nameof(MainViewModel), nameof(DCLoadExecute), errorstring);
                    return OperateResult.Failed(errorstring);
                }

                if (config == null)
                {
                    errorstring = "直流负载配置不可为空";
                    SuperDHHLoggerManager.Error(LoggerType.TESTLOG, nameof(MainViewModel), nameof(DCLoadExecute), errorstring);
                    return OperateResult.Failed(errorstring);
                }


                string InfoString = string.Empty;

                //短路模式
                if (config.Set_ShortState == DeviceOutPutState.ON)
                {
                    OperateResult result = device.SetShort(config.Set_ShortState);
                }
                else//非短路模式
                {
                    #region 启动拉载

                    double loadvalue = config.EnableStepping ? config.StartLoadValue : config.Set_LoadValue;

                    OperateResult SetRlt = device.SetModelAndValue(config.Set_Model, loadvalue);

                    if (!SetRlt)
                    {
                        errorstring = $"设备参数设置异常：【{SetRlt.Message}】";
                        SuperDHHLoggerManager.Error(LoggerType.TESTLOG, nameof(MainViewModel), nameof(DCLoadExecute), errorstring);
                        return OperateResult.Failed(errorstring);
                    }

                    InfoString = $"设置\n[模式]{config.Set_Model}\n[拉载值]{loadvalue}成功";
                    SuperDHHLoggerManager.Info(LoggerType.TESTLOG, nameof(MainViewModel), nameof(DCLoadExecute), InfoString);

                    if (config.OperateType != SetOutPutState.Null)
                    {
                        device.SetOnOff(config.OperateType == SetOutPutState.ON ? DeviceOutPutState.ON : DeviceOutPutState.OFF);

                        InfoString = config.OperateType == SetOutPutState.ON ? "打开" : "关闭";
                        SuperDHHLoggerManager.Info(LoggerType.TESTLOG, nameof(MainViewModel), nameof(DCLoadExecute), InfoString);

                        //Logger.Info(nameof(FunConfig_DCLoad), nameof(Execute), $"设备已{InfoString}");
                    }

                    #endregion

                    //步进模式
                    if (config.EnableStepping)
                    {
                        Dictionary<string, ModDeviceReadData> data = new Dictionary<string, ModDeviceReadData>();

                        for (int i = 0; i < canoperate?.ReadInfos.Count; i++)
                        {
                            data.Add(canoperate.ReadInfos[i].DataInfo.Name, canoperate.ReadInfos[i]);
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
                                double setpvol = config.StartLoadValue + config.Stride * (i + 1) * (config.EndLoadValue < config.StartLoadValue ? -1 : 1);

                                setpvol = config.EndLoadValue > config.StartLoadValue ? Math.Min(setpvol, config.EndLoadValue) : Math.Max(setpvol, config.EndLoadValue);

                                device.SetLoadValue((uint)setpvol * 10);

                                InfoString = $"设备设置电压{setpvol}成功";

                                SuperDHHLoggerManager.Info(LoggerType.TESTLOG, nameof(MainViewModel), nameof(DCLoadExecute), InfoString);

                                canoperate?.Device_ReadAll();
                                aqcoperate?.Device_ReadAll();
                                
                                //跳出步进条件
                                if (config.Values.Count > 0)
                                {
                                    //if (config.StepCondition == StepConditions.OR)
                                    //{
                                    List<bool> results = new List<bool>();
                                        for (int j = 0; j < config.Values.Count; j++)
                                        {
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

                                Thread.Sleep(config.StepFrequency);
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
        /// 结束试验
        /// </summary>
        private void TestStop() 
        {
            TestErrorString = "手动停止试验";
            if (monitor!=null)
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
            GlobalModel.IsTestThreadRun = false;
            IsTestRuning = false;

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

            DBOperate.Default.GetProjectNames()
                .AttachIfSucceed(result =>
                {
                    ProdectNames.Clear();

                    for (int i = 0; i < result.Data.Count; i++)
                    {
                        string name = result.Data[i];
                        ProdectNames.Add(name);
                    }
                })
                .AttachIfFailed(result => {
                    MessageBox.Show($"项目信息获取失败,请重启操作平台\r\n{result.Message}","项目信息");
                });
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
            if (!GlobalModel.CabinetSate)
            {
                ContentName = "系统设备存在故障，无法运行";
                ContentColor = Brushes.Red;
            }
            //ATEExecuteInfos.Add(new ExecuteInfo 
            //{
            //    Index = 1,
            //    FunctionName = "TEST1",
            //    Result = ResultState.UNACCOMPLISHED,
            //    StartTime = DateTime.Now.ToString("HH:mm:ss"),
            //    StepInfos = new ObservableCollection<ExecuteStepInfo> { 
            //        new ExecuteStepInfo {
            //            StepName = "test1",
            //            ExecuteTime = DateTime.Now.ToString("HH:mm:ss"),
            //            Limit_Lower = string.Empty,
            //            Limit_Upper = string.Empty,
            //            Result = ResultState.SUCCESS,
            //        },
            //        new ExecuteStepInfo {
            //            StepName = "test2",
            //            ExecuteTime = DateTime.Now.AddMinutes(1).ToString("HH:mm:ss"),
            //            Limit_Lower = "1",
            //            Limit_Upper = "3",
            //            Result = ResultState.SUCCESS,
            //        },
            //         new ExecuteStepInfo {
            //            StepName = "test3",
            //            ExecuteTime = DateTime.Now.AddMinutes(2).ToString("HH:mm:ss"),
            //            Limit_Lower = string.Empty,
            //            Limit_Upper = string.Empty,
            //            Result = ResultState.FAIL,
            //        },
            //    }
            //});

            //ATEExecuteInfos.Add(new ExecuteInfo
            //{
            //    Index = 2,
            //    FunctionName = "TEST2",
            //    Result = ResultState.UNACCOMPLISHED,
            //    StartTime = DateTime.Now.ToString("HH:mm:ss"),
            //    StepInfos = new ObservableCollection<ExecuteStepInfo> {
            //        new ExecuteStepInfo {
            //            StepName = "test1",
            //            ExecuteTime = DateTime.Now.ToString("HH:mm:ss"),
            //            Limit_Lower = string.Empty,
            //            Limit_Upper = string.Empty,
            //            Result = ResultState.SUCCESS,
            //        },
            //        new ExecuteStepInfo {
            //            StepName = "test2",
            //            ExecuteTime = DateTime.Now.AddMinutes(1).ToString("HH:mm:ss"),
            //            Limit_Lower = "1",
            //            Limit_Upper = "3",
            //            Result = ResultState.SUCCESS,
            //        },
            //         new ExecuteStepInfo {
            //            StepName = "test3",
            //            ExecuteTime = DateTime.Now.AddMinutes(2).ToString("HH:mm:ss"),
            //            Limit_Lower = string.Empty,
            //            Limit_Upper = string.Empty,
            //            Result = ResultState.FAIL,
            //        },
            //    }
            //});
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
            set => Set(nameof(Result), ref result, value);
        }

        /// <summary>
        /// 开始时间
        /// </summary>
        public string ExecuteTime { get; set; }
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
