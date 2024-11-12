#region << 版 本 注 释 >>
/*----------------------------------------------------------------
 * 版权所有 (c) 2024   保留所有权利。
 * CLR版本：4.0.30319.42000
 * 机器名称：LAPTOP-9Q9TTD5V
 * 公司名称：
 * 命名空间：IMX.ATS.ATE.ViewModel
 * 唯一标识：0b3e3f5f-f3e0-46a4-b983-8af4c5539e5a
 * 文件名：DeviceInitViewModel
 * 当前用户域：LAPTOP-9Q9TTD5V
 * 
 * 创建者：58274
 * 创建时间：2024/9/10 15:37:57
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
using H.WPF.Framework;
using IMX.Common;
using IMX.Device.Base;
using IMX.Device.Base.DriveOperate;
using IMX.Device.Common;
using IMX.Function.Base;
using IMX.Logger;
using Super.Zoo.Framework;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace IMX.ATS.ATE
{
    /// <summary>
    /// 设备初始化界面模型类
    /// </summary>
    public class DeviceInitViewModel : ExtendViewModelBase
    {

        #region 公共属性

        #region 界面绑定属性
        private string title = "功能测试平台上电初始化";

        public string Title
        {
            get => title;
            set => Set(nameof(Title), ref title, value);
        }

        private ObservableCollection<InitInfo> initinfos = new ObservableCollection<InitInfo>();

        public ObservableCollection<InitInfo> InitInfos
        {
            get => initinfos;
            set => Set(nameof(InitInfos), ref initinfos, value);
        }

        #endregion

        #region 界面绑定指令
        #endregion

        /// <summary>
        /// 是否为初始化
        /// </summary>
        public bool IsInit { get; set; } = true;
        #endregion

        #region 私有变量
        /// <summary>
        /// 首次进入状态
        /// </summary>
        private bool IsFrist = true;

        /// <summary>
        /// 故障信息
        /// </summary>
        private string ErrorStr = string.Empty;

        /// <summary>
        /// 界面窗口句柄
        /// </summary>
        private Window window;

        /// <summary>
        /// 系统支持设备列表
        /// </summary>
        private BaseConfig SupportDeviceConfigXml = new BaseConfig(BaseConfig.StartupPath + string.Format("Config\\Systeam\\SupportDeviceConfig.xml"));

        /// <summary>
        /// 初始化信息字典[设备名称, 初始化参数信息]
        /// </summary>
        private Dictionary<string, InitInfo> dicInitInfo = new Dictionary<string, InitInfo>();
        #endregion

        #region 私有方法

        #region 设备信息读取
        /// <summary>
        /// 设备配置初始化
        /// </summary>
        /// <returns></returns>
        private OperateResult DeviceConfigInit() 
        {
            if (!Directory.Exists(BaseConfig.StartupPath + string.Format("Config\\Systeam")))
            {
                Directory.CreateDirectory(BaseConfig.StartupPath + string.Format("Config\\Systeam"));
                return OperateResult.Failed("系统配置文件缺失，请确认Config文件夹下的Systeam文件夹内容");
            }

            return SupportDeviceConfigXml.GetSections(out List<SysteamSupportDeviceConfigInfo> config).AttachIfSucceed(result =>
            {
                //SupportConfig.DicSupportDevice.Clear();
                //SupportConfig.DicTestFlowItems.Clear();
                //SupportConfig.DicManualItems.Clear();
                for (int i = 0; i < config?.Count; i++)
                {
                    var device = config[i];
                    //if (device.EnableDriveInit)
                    //{
                        //SupportConfig.DicSupportDevice.Add(device.DeviceType, device.TypeName);
                        SupportConfig.DicSysteamDeviceConfigs.Add(device.Description, device);
                        GlobalModel.DicDeviceInfo.Add(device.Description, new DeviceInfo_ALL { Config = device });
                    //}
                    //if (device.EnableFlow)
                    //{
                    //    SupportConfig.DicTestFlowItems.Add(device.Description, device.FuncitonType.GetDescription());
                    //}

                    //if (device.EnableManual)
                    //{
                    //    SupportConfig.DicManualItems.Add(device.DeviceType.GetDescription(), device.Description);
                    //}
                }

                SupportConfig.SysteamDeviceConfigs = config;
            });
        }

        
        /// <summary>
        /// 系统设备配置文件写入
        /// </summary>
        /// <returns></returns>
        private OperateResult WriteSupportDeviceConfig()
        {
            if (!Directory.Exists(BaseConfig.StartupPath + string.Format("Config\\Systeam")))
            {
                Directory.CreateDirectory(BaseConfig.StartupPath + string.Format("Config\\Systeam"));
            }
            
            new BaseConfig(BaseConfig.StartupPath + string.Format("Config\\ConfigDevice\\ACSource_0.xml"))
                .WriteXml(new DeviceArgs 
                {
                    Address = 1,
                    DeviceType = EDeviceType.ACSource,
                    Name = "ANFH010S",
                    DriveConfig = new DriveArgs 
                    {
                         BaudRate = "38400",
                         ResourceString = "ASRL2::INSTR",
                         CommunicationType = Device.Common.DriveType.ASRL,
                         ConfigString = "Parity=None;DataBits=8;StopBits=One;FlowControl=XOnXOff;Address=1;UseSerial=false",
                         TimeoutMS = 500,
                         BeforeReadDelayMS = 200,
                         TerminationCharacterEnabled = false,
                         BusType = Piggy.VehicleBus.Common.VehicleBusType.Unknow,
                    },
                });

            new BaseConfig(BaseConfig.StartupPath + string.Format("Config\\ConfigDevice\\APU_0.xml"))
                .WriteXml(new DeviceArgs
                {
                    Address = 1,
                    DeviceType = EDeviceType.APU,
                    Name = "IT6832",
                    DriveConfig = new DriveArgs
                    {
                        BaudRate = "9600",
                        ResourceString = "USB0::0x2A8D::0x0101::MY57501899::INSTR",
                        CommunicationType = Device.Common.DriveType.USB,
                        ConfigString = "Parity=None;DataBits=8;StopBits=One;FlowControl=XOnXOff;Address=1;UseSerial=false",
                        TimeoutMS = 500,
                        BeforeReadDelayMS = 200,
                        TerminationCharacterEnabled = true,
                        BusType = Piggy.VehicleBus.Common.VehicleBusType.Unknow,
                    },
                });

            new BaseConfig(BaseConfig.StartupPath + string.Format("Config\\ConfigDevice\\HVDCSource_0.xml"))
            .WriteXml(new DeviceArgs
            {
                Address = 1,
                DeviceType = EDeviceType.HVDCSource,
                Name = "AN50300",
                DriveConfig = new DriveArgs
                {
                    BaudRate = "38400",
                    ResourceString = "ASRL3::INSTR",
                    CommunicationType = Device.Common.DriveType.ASRL,
                    ConfigString = "Parity=None;DataBits=8;StopBits=One;FlowControl=XOnXOff;Address=1;UseSerial=false",
                    TimeoutMS = 500,
                    BeforeReadDelayMS = 200,
                    TerminationCharacterEnabled = true,
                    BusType = Piggy.VehicleBus.Common.VehicleBusType.Unknow,
                },
            });

            new BaseConfig(BaseConfig.StartupPath + string.Format("Config\\ConfigDevice\\DCLoad_0.xml"))
            .WriteXml(new DeviceArgs
            {
                Address = 1,
                DeviceType = EDeviceType.DCLoad,
                Name = "AN23600E",
                DriveConfig = new DriveArgs
                {
                    ResourceString = "TCPIP0::192.168.0.10::2101::SOCKET",
                    CommunicationType = Device.Common.DriveType.LAN,
                    ConfigString = "Parity=None;DataBits=8;StopBits=One;FlowControl=XOnXOff;Address=1;UseSerial=false",
                    TimeoutMS = 500,
                    BeforeReadDelayMS = 200,
                    TerminationCharacterEnabled = true,
                    BusType = Piggy.VehicleBus.Common.VehicleBusType.Unknow,
                },
            });

            new BaseConfig(BaseConfig.StartupPath + string.Format("Config\\ConfigDevice\\Acquisition_0.xml"))
            .WriteXml(new DeviceArgs
            {
                Address = 1,
                DeviceType = EDeviceType.Acquisition,
                Name = "AN87330",
                DriveConfig = new DriveArgs
                {
                    ResourceString = "TCPIP0::192.168.0.10:520::SOCKET",
                    CommunicationType = Device.Common.DriveType.LAN,
                    ConfigString = "Parity=None;DataBits=8;StopBits=One;FlowControl=XOnXOff;Address=1;UseSerial=false",
                    TimeoutMS = 500,
                    BeforeReadDelayMS = 200,
                    TerminationCharacterEnabled = true,
                    BusType = Piggy.VehicleBus.Common.VehicleBusType.Unknow,
                },
            });

            new BaseConfig(BaseConfig.StartupPath + string.Format("Config\\ConfigDevice\\Product_0.xml"))
           .WriteXml(new DeviceArgs
           {
               Address = 1,
               DeviceType = EDeviceType.Product,
               Name = "CAN",
               DriveConfig = new DriveArgs
               {
                   BaudRate = "500Kbps",
                   ResourceString = "ZLG_CANFD::USBCANFD_200U::0::INSTR",
                   CommunicationType = Device.Common.DriveType.LAN,
                   ConfigString = "canfdspeed=否;DataBaudrate=500Kbps;ChannelIndex=0",
                   TimeoutMS = 100,
                   BeforeReadDelayMS = 20,
                   TerminationCharacterEnabled = true,
                   BusType = Piggy.VehicleBus.Common.VehicleBusType.CAN,
               },
           });

            return SupportDeviceConfigXml.WriteXml(new List<SysteamSupportDeviceConfigInfo>
            {
               new SysteamSupportDeviceConfigInfo { DeviceType = EDeviceType.Product,  DeviceNum = 1, Description = "Product",DeviceModel = "Product_CAN", TypeName = "Product_CAN", FuncitonType = FuncitonType.Product, EnableFlow = true, EnableDriveInit = false, EnableManual = false, EnableMonitor = true},
               new SysteamSupportDeviceConfigInfo { DeviceType = EDeviceType.Unknow,  DeviceNum = 0, Description = "未知设备",DeviceModel = "未知设备", TypeName = "未知设备", FuncitonType = FuncitonType.ProductResult, EnableFlow = true, EnableDriveInit = true, EnableManual = false, EnableMonitor = true},
               new SysteamSupportDeviceConfigInfo { DeviceType = EDeviceType.Acquisition,  DeviceNum = 1, Description = "Acquisition", DeviceModel = "MCx", TypeName = "MCx", FuncitonType = FuncitonType.EquipmentResult, EnableFlow = true, EnableDriveInit = true, EnableManual = false, EnableMonitor = true},
               new SysteamSupportDeviceConfigInfo { DeviceType = EDeviceType.DCLoad,  DeviceNum = 1, Description = "DCLoad", DeviceModel = "AN23600E", TypeName = "AN23600E", FuncitonType = FuncitonType.DCLoad, EnableFlow = true, EnableDriveInit = true, EnableManual = false, EnableMonitor = true},
               new SysteamSupportDeviceConfigInfo { DeviceType = EDeviceType.HVDCSource,  DeviceNum = 1, Description = "HVDCSource", DeviceModel = "AN50300", TypeName = "AN50300", FuncitonType = FuncitonType.DCSource, EnableFlow = true, EnableDriveInit = true, EnableManual = false, EnableMonitor = true},
               new SysteamSupportDeviceConfigInfo { DeviceType = EDeviceType.APU,  DeviceNum = 1, Description = "APU", DeviceModel = "IT6800", TypeName = "IT6800", FuncitonType = FuncitonType.APU, EnableFlow = true, EnableDriveInit = true, EnableManual = false, EnableMonitor = true},
               new SysteamSupportDeviceConfigInfo { DeviceType = EDeviceType.ACSource,  DeviceNum = 1, Description = "ACSource", DeviceModel = "ANFH010S", TypeName = "ANFH010S", FuncitonType = FuncitonType.ACSource, EnableFlow = true, EnableDriveInit = true, EnableManual = false, EnableMonitor = true},
            });
        }

        /// <summary>
        /// 读取配置信息
        /// </summary>
        private OperateResult ReadConfig()
        {
            string folderPath = $@"{AppDomain.CurrentDomain.BaseDirectory}\Config\ConfigDevice";
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            try
            {
                FileInfo[] files = new DirectoryInfo(folderPath).GetFiles();

                foreach (FileInfo file in files)
                {
                    string pathname = file.Name.Split('.')[0];
                    string name = file.Name.Split('_')[0];
                    string num = pathname[pathname.Length - 1].ToString();

                    if (!SupportConfig.DicSysteamDeviceConfigs.TryGetValue(name, out SysteamSupportDeviceConfigInfo deviceconfiginfo))
                    {
                        continue;
                    }

                    //系统支持当前设备数量判断
                    if (Convert.ToInt32(num) >= deviceconfiginfo.DeviceNum)
                    {
                        continue;
                    }

                    Device_Config deviceconfig = new Device_Config(pathname);

                    if (GlobalModel.DicDeviceArgs.ContainsKey(name))
                    {
                        GlobalModel.DicDeviceArgs.Remove(name);
                    }

                    deviceconfig.GetSections<DeviceArgs>();
                   var x =  deviceconfig.DeviceConfig;

                    GlobalModel.DicDeviceArgs.Add(name, deviceconfig.DeviceConfig);
                    
                    if (GlobalModel.DicDeviceInfo.ContainsKey(name))
                    {
                        GlobalModel.DicDeviceInfo[name].Args = deviceconfig.DeviceConfig;
                    }
                    

                    if (!deviceconfiginfo.EnableDriveInit)
                    {
                        continue;
                    }

                    if (dicInitInfo.ContainsKey(name))
                    {
                        continue;
                    }

                    InitInfo info = new InitInfo
                    {
                        Describe = deviceconfig.DeviceConfig.DeviceType.GetDescription(),
                        DeviceModel = deviceconfig.DeviceConfig.Name,
                        DeviceSate = ResultState.UNACCOMPLISHED,
                    };

                    dicInitInfo.Add(name, info);
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        InitInfos.Add(info);
                    });

                    Thread.Sleep(100);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.GetMessage(),"系统配置文件获取异常");
                SuperDHHLoggerManager.Exception( LoggerType.FROMLOG, nameof(DeviceInitViewModel), nameof(ReadConfig), ex);
                return OperateResult.Excepted(ex);
            }

            return OperateResult.Succeed();
        }
        #endregion



        private readonly object objLock = new object();
        /// <summary>
        /// 设备初始化
        /// </summary>
        /// <param name="config">设备配置信息</param>
        /// <returns></returns>
        private OperateResult<IDeviceOperate> DeviceInit(DeviceArgs config)
        {
            lock (objLock)
            {
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
                     .ThenAnd(result => result.Data.Init(config, drive.Drive).ConvertTo(result.Data));
            }
        }

        /// <summary>
        /// 工装初始化
        /// </summary>

        private void CabinetInit()
        {
            if (GlobalModel.DicDeviceInfo.Count<1)
            {
                return;
            }

            try
            {
                foreach (var item in GlobalModel.DicDeviceInfo)
                {
                    if (item.Value.Config.EnableDriveInit)
                    {
                        var result = DeviceInit(item.Value.Args);
                        if (!result) 
                        {
                            ErrorStr += $"{dicInitInfo[item.Key].Describe}设备初始化失败\r\n{result.Message}\r\n";
                            GlobalModel.CabinetSate = false;
                            Thread.Sleep(10);
                            Application.Current.Dispatcher.Invoke(() =>
                           {
                               dicInitInfo[item.Key].DeviceSate = ResultState.FAIL;
                           });
                            continue;
                        }

                        result.Data.Close()
                            .AttachIfSucceed(result1 => 
                            {
                                item.Value.DeviceOperate = result.Data;
                                item.Value.Drive = GlobalModel.DicDeviceDrives[item.Value.Args.DriveConfig.ResourceString];
                                Thread.Sleep(10);
                                Application.Current.Dispatcher.Invoke(() =>
                                {
                                    dicInitInfo[item.Key].DeviceSate = ResultState.SUCCESS;
                                });
                            })
                            .AttachIfFailed(result1 =>
                            {
                                ErrorStr += $"{dicInitInfo[item.Key].Describe}设备初始化失败\r\n{result1.Message}\r\n";
                                GlobalModel.CabinetSate = false;
                                Thread.Sleep(10);
                                Application.Current.Dispatcher.Invoke(() =>
                                {
                                    dicInitInfo[item.Key].DeviceSate = ResultState.FAIL;
                                });
                            });

                        //.ThenAnd(result => result.Data.Device_ReadAll()
                        //.ConvertTo(result.Data))
                        //.AttachIfSucceed(result => {
                        //    item.Value.DeviceOperate = result.Data;
                        //    item.Value.Drive = GlobalModel.DicDeviceDrives[item.Value.Args.DriveConfig.ResourceString];
                        //    Thread.Sleep(10);
                        //    Application.Current.Dispatcher.Invoke(() =>
                        //    {
                        //        dicInitInfo[item.Key].DeviceSate = ResultState.SUCCESS;
                        //    });
                        //})
                        //.AttachIfFailed(result =>
                        //{
                        //    ErrorStr += $"{dicInitInfo[item.Key].Describe}设备初始化失败\r\n{result.Message}\r\n";
                        //    GlobalModel.CabinetSate = false;
                        //    Thread.Sleep(10);
                        //    Application.Current.Dispatcher.Invoke(() =>
                        //    {
                        //        dicInitInfo[item.Key].DeviceSate = ResultState.FAIL;
                        //    });
                        //});
                    }
                }

            }
            catch (Exception ex)
            {
                GlobalModel.CabinetSate = false;
                ErrorStr += ex.GetMessage();
            }

            Thread.Sleep(1000);

            WindowClosedExecute(window);
        }

        /// <summary>
        /// 工装反初始化
        /// </summary>
        private void CabinetUnInit()
        {
            foreach (var item in dicInitInfo)
            {
                if (item.Value.DeviceSate == ResultState.FAIL)
                {
                    continue;
                }


                try
                {
                    var deviceinfo = GlobalModel.DicDeviceInfo[item.Key];

                    deviceinfo.DeviceOperate.Close().And(deviceinfo.DeviceOperate.UnInit())
                        .And(deviceinfo.Drive.UnregisterDevice(deviceinfo.DeviceOperate))
                        .AttachIfSucceed(result => 
                        {
                            Application.Current.Dispatcher.Invoke(() =>
                            {
                                dicInitInfo[item.Key].DeviceSate = ResultState.SUCCESS;
                            });
                            Thread.Sleep(100);
                        })
                        .AttachIfFailed(result =>
                        {
                            Application.Current.Dispatcher.Invoke(() =>
                            {
                                dicInitInfo[item.Key].DeviceSate = ResultState.FAIL;
                            });
                            Thread.Sleep(100);
                        });
                }
                catch (Exception ex)
                {
                    SuperDHHLoggerManager.Exception( LoggerType.FROMLOG, nameof(DeviceInitViewModel), nameof(CabinetUnInit), ex);
                }
            }
            Thread.Sleep(1000);
            WindowClosedExecute(window);
        }
        #endregion

        #region 保护方法
        protected override void WindowLoadedExecute(object obj)
        {
            if (!(obj is Window win))
            {
                return;
            }

            window = win;

            Title = IsInit ? "功能测试平台上电初始化" : "功能测试平台下电复原";

            if (IsInit)
            {
                InitInfos.Clear();
                dicInitInfo.Clear();

                OperateResult result =  DeviceConfigInit().And(ReadConfig());
                if (!result)
                {

                    ErrorStr = result.Message;
                    GlobalModel.CabinetSate=false;
                    WindowClosedExecute(obj);
                    return;
                }
                new Thread(CabinetInit) { IsBackground = true }.Start();
            }
            else
            {
                foreach (var item in dicInitInfo) 
                {
                    if (item.Value.DeviceSate == ResultState.FAIL)
                    {
                        continue;
                    }

                    item.Value.DeviceSate = ResultState.UNACCOMPLISHED;
                }
                new Thread(CabinetUnInit) { IsBackground = true }.Start();
            }


            //base.WindowLoadedExecute(obj);

        }

        protected override void WindowClosedExecute(object obj)
        {
            if (IsInit)
            {
                if (!string.IsNullOrEmpty(ErrorStr))
                { System.Windows.Forms.MessageBox.Show(ErrorStr, "设备初始化异常"); }

                ErrorStr = string.Empty;
                Application.Current.Dispatcher.Invoke(new Action(() =>
                {
                    Window mainwindow = ContentControlManager.GetWindow<MainView>(((ViewModelLocator)Application.Current.FindResource("Locator")).Main);
                    mainwindow.Show();
                    //base.WindowClosedExecute(obj);
                }));
            }

            IsInit = !IsInit;

            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                base.WindowClosedExecute(obj);
            }));
        }
        #endregion


        #region 构造方法
        public DeviceInitViewModel() 
        {
            //WriteSupportDeviceConfig();
        }
        #endregion

    }

    /// <summary>
    /// 初始化信息
    /// </summary>
    public class InitInfo : ViewModelBase
    {
        private string describe;
        /// <summary>
        /// 初始化设备概况描述
        /// </summary>
        public string Describe
        {
            get => describe;
            set => Set(nameof(Describe), ref describe, value);
        }

        private string devicemodel;
        /// <summary>
        /// 初始化设备型号
        /// </summary>
        public string DeviceModel
        {
            get => devicemodel;
            set => Set(nameof(DeviceModel), ref devicemodel, value);
        }

        private ResultState devicesate;
        /// <summary>
        /// 设备初始化状态
        /// </summary>
        public ResultState DeviceSate
        {
            get => devicesate;
            set => Set(nameof(DeviceSate), ref devicesate, value);
        }
    }
}
