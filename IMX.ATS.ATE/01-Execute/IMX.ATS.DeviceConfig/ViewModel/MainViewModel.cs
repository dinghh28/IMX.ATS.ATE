#region << 版 本 注 释 >>
/*----------------------------------------------------------------
 * 版权所有 (c) 2025   保留所有权利。
 * CLR版本：4.0.30319.42000
 * 机器名称：LAPTOP-9Q9TTD5V
 * 公司名称：
 * 命名空间：IMX.ATS.DeviceConfig.ViewModel
 * 唯一标识：2ea0b7fa-8946-4ded-9024-056a6c78761f
 * 文件名：MainViewModel
 * 当前用户域：LAPTOP-9Q9TTD5V
 * 
 * 创建者：58274
 * 创建时间：2025/2/20 15:25:34
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
using GalaSoft.MvvmLight.CommandWpf;
using H.WPF.Framework;
using IMX.ATE.Common;
using IMX.ATE.Common.SupportDevice;
using IMX.Device.Base;
using IMX.Device.Base.DriveOperate;
using IMX.Device.Common;
using IMX.Function.Base;
using IMX.Logger;
using IMX.WPF.Resource;
using Ivi.Visa;
using Newtonsoft.Json.Linq;
using Super.Zoo.Framework;
using Super.Zoo.Framework.Logger;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using DriveType = IMX.Device.Common.DriveType;

namespace IMX.ATS.DeviceConfig
{
    public class MainViewModel : WindowViewModelBaseEx
    {

        #region 公共属性

        #region 界面绑定属性
        /// <summary>
        /// 当前登陆用户
        /// </summary>
        public string UserName => GlobalModel.UserInfo?.UserName;

        /// <summary>
        /// 软件版本信息
        /// </summary>
        public string SoftwareVersion => SysteamInfo.SoftwareVersion;

        private Visibility deviceshow = Visibility.Hidden;
        /// <summary>
        /// 配置界面显示
        /// </summary>
        public Visibility DeviceShow
        {
            get => deviceshow;
            set => Set(nameof(DeviceShow), ref deviceshow, value);
        }


        #region 通讯自检提示
        private string contentname = string.Empty;
        /// <summary>
        /// 通讯自检提示
        /// </summary>
        public string ContentName
        {
            get => contentname;
            set => Set(nameof(ContentName), ref contentname, value);
        }

        private SolidColorBrush contentcolor = Brushes.Green;
        /// <summary>
        /// 通讯自检提示颜色
        /// </summary>
        public SolidColorBrush ContentColor
        {
            get => contentcolor;
            set => Set(nameof(ContentColor), ref contentcolor, value);
        }
        #endregion

        #region 设备信息
        /// <summary>
        /// 设备列表
        /// </summary>
        public List<DeviceTypeInfo> Devices { get; set; } = new List<DeviceTypeInfo>();
        //{
        //    new DeviceTypeInfo {  Type= EDeviceType.DCLoad, DeviceName = "1111", Info = new DeviceConfigInfo(){ Config = new DeviceArgs()} },
        //    new DeviceTypeInfo {  Type= EDeviceType.Acquisition, DeviceName = "1111", Info = new DeviceConfigInfo(){ Config = new DeviceArgs()}},
        //    new DeviceTypeInfo {  Type= EDeviceType.HVDCSource, DeviceName = "1111", Info = new DeviceConfigInfo(){ Config = new DeviceArgs()}},
        //    new DeviceTypeInfo {  Type= EDeviceType.ACSource, DeviceName = "1111", Info = new DeviceConfigInfo(){ Config = new DeviceArgs()}},
        //};

        private DeviceTypeInfo selectedDevie = new DeviceTypeInfo();
        /// <summary>
        /// 当前选择设备配置信息
        /// </summary>
        public DeviceTypeInfo SelectedDevie
        {
            get => selectedDevie;
            set => Set(nameof(SelectedDevie), ref selectedDevie, value);
        }
        #endregion

        private ObservableCollection<ViewLogger> logger = new ObservableCollection<ViewLogger>();
        /// <summary>
        /// 日志界面显示记录
        /// </summary>
        public ObservableCollection<ViewLogger> Logger
        {
            get => logger;
            set => Set(nameof(Logger), ref logger, value);
        }

        //private List<string> devices;
        ///// <summary>
        ///// 设备列表
        ///// </summary>
        //public List<string> Devices
        //{
        //    get => devices;
        //    set => Set(nameof(Devices), ref devices, value);
        //}

        #endregion

        #region 界面绑定指令

        /// <summary>
        /// 保存配置
        /// </summary>
        public RelayCommand Save => new RelayCommand(SaveConfig);

        /// <summary>
        /// 通讯连接测试
        /// </summary>
        public RelayCommand Link => new RelayCommand(LinkTest);

        /// <summary>
        /// 清除日志内容
        /// </summary>
        public RelayCommand ClearLogger => new(() => { Logger.Clear(); });
        #endregion

        #endregion

        #region 私有变量

        /// <summary>
        /// 系统支持设备列表
        /// </summary>
        private BaseConfig SupportDeviceConfigXml = new BaseConfig(BaseConfig.StartupPath + string.Format("Config\\Systeam\\SupportDeviceConfig.xml"));
        #endregion

        #region 私有方法

        /// <summary>
        /// 获取设备配置信息
        /// </summary>
        /// <param name="obj">设备类型</param>
        private void GetedDeviceConfigs(object obj)
        {
            SelectedDevie = Devices.Find(x => x.Type == (EDeviceType)Enum.Parse(typeof(EDeviceType), value: obj.ToString()));

            if (SelectedDevie == null && SelectedDevie.Info == null)
            {
                DeviceShow = Visibility.Hidden;
                return;
            }

            DeviceShow = Visibility.Visible;
            var value = SelectedDevie.Info;

            if (!SupportConfig.DicSupportBaudRate.TryGetValue(value.Drive, out ObservableCollection<string> baudrates))
            {
                baudrates = new ObservableCollection<string>();
            }
            value.BaudRate = baudrates;
            value.DriveResources.Clear();
            try
            {
                value.ExtShow = value.Drive == DriveType.ASRL ? Visibility.Visible : value.Drive == DriveType.VehicleBus ? Visibility.Visible : Visibility.Collapsed;
                value.ASRLMegShow = value.Drive == DriveType.ASRL ? Visibility.Visible : Visibility.Collapsed;
                value.CANMegShow = value.Drive == DriveType.VehicleBus ? Visibility.Visible : Visibility.Collapsed;
                value.NormalShow = value.Drive != DriveType.VehicleBus ? Visibility.Visible : Visibility.Collapsed;
                int index = -1;
                switch (value.Drive)
                {
                    //case DriveType.CAN:
                    //    DriveResources.Add(Config.DriveConfig.ResourceString);
                    //    break;
                    case DriveType.TCPIP:
                        var tcpresources = new NationalInstruments.Visa.ResourceManager().Find($"{value.Drive.ToString().ToUpper()}?*SOCKET").ToList();
                        
                        for (int i = 0; i < tcpresources?.Count; i++)
                        {
                            value.DriveResources.Add(tcpresources[i]);
                            if (tcpresources[i] == value.Config.DriveConfig.ResourceString)
                            {
                                index = i;
                            }
                        }

                        value.SelectedDriveResourceIndex = index;

                        value.ExtShow = Visibility.Collapsed;
                        value.ASRLMegShow = Visibility.Collapsed;
                        value.CANMegShow = Visibility.Collapsed;
                        value.NormalShow = Visibility.Visible;
                        //tcpresources.ForEach(x =>
                        //{
                        //    DriveResources.Add(x);
                        //});
                        break;
                    case DriveType.VehicleBus:
                        value.ExtShow = Visibility.Visible;
                        value.ASRLMegShow = Visibility.Collapsed;
                        value.CANMegShow = Visibility.Visible;
                        value.NormalShow = Visibility.Collapsed;
                        break;
                    case DriveType.USB:
                    case DriveType.ASRL:
                        var resources = new NationalInstruments.Visa.ResourceManager().Find($"{value.ToString().ToUpper()}?*INSTR").ToList();
                        for (int i = 0; i < resources?.Count; i++)
                        {
                            value.DriveResources.Add(resources[i]);
                            if (resources[i] == value.Config.DriveConfig.ResourceString)
                            {
                                index = i;
                            }
                        }

                        value.SelectedDriveResourceIndex = index;

                        value.ExtShow = value.Drive == DriveType.ASRL ? Visibility.Visible : Visibility.Collapsed;
                        value.ASRLMegShow = value.Drive == DriveType.ASRL ? Visibility.Visible : Visibility.Collapsed;
                        value.CANMegShow = Visibility.Collapsed;
                        value.NormalShow = Visibility.Visible;
                        //resources.ForEach(x =>
                        //{
                        //    DriveResources.Add(x);
                        //});
                        //DriveResources = new ResourceManager().Find($"{Config.DriveConfig.CommunicationType.ToString().ToUpper()}?*INSTR").ToList();
                        break;
                }
            }
            catch (Exception ex)
            {
                value.DriveResources.Add($"{ex.Message}");
            }
        }

        /// <summary>
        /// 保存当前设备配置信息
        /// </summary>
        private void SaveConfig()
        {
            if (SelectedDevie == null)
            {
                return;
            }


            Device_Config devicedonfig = new Device_Config(SelectedDevie.PathName);
            var config = SelectedDevie.Info.Config;
            var item = SelectedDevie.Info;
            config.DriveConfig.CommunicationType = item.Drive;

            if (item.SelectedDriveResourceIndex == -1)
            {
                Logger.Add(new ViewLogger
                {
                    RecordTime = DateTime.Now,
                    Level = LoggerLevel.WARN,
                    Content = $"{item.Config.Name}[{item.Config.DeviceType.GetDescription()}]配置保存失败",
                });
                MessageBox.Show($"设备[{item.PathName}]配置保存异常:未配置驱动通讯参数");
                return;
            }

            if (SelectedDevie.Info.Drive == DriveType.VehicleBus)
            {
                string configstring = string.Empty;
                string resourcestring = string.Empty;
                DriveHelper
                    .EncryptedResourceString(SelectedDevie.Info.CANDrive, SelectedDevie.Info.DriveIndex, ref resourcestring)
                    .And(
                    DriveHelper
                    .EncryptedConfigString(item.SelectedCANFDacceler, item.SelectedDatabaudRate, item.ChanneIndex, ref configstring))
                    .AttachIfSucceed(result =>
                    {
                        item.Config.DriveConfig.ConfigString = configstring;
                        item.Config.DriveConfig.ResourceString = resourcestring;
                    });
            }

            if (item.Drive == DriveType.ASRL)
            {
                string configstring = string.Empty;

                DriveHelper
                .EncryptedConfigString(item.SelectSerialParitys, item.DataBits, item.SelectStopBitsMode, item.SelectFlowControl, item.UseSerial, ref configstring)
                .AttachIfSucceed(result =>
                {
                    item.Config.DriveConfig.ConfigString = configstring;
                    
                });
            }
            //devicedonfig.WriteXml(item.Config).AttachIfFailed(result =>
            //{
            //    MessageBox.Show($"设备[{item.PathName}]配置保存异常:{result.Message}");
            //    return;
            //});
            item.Config.DriveConfig.ResourceString = item.DriveResources[item.SelectedDriveResourceIndex];
            item.Config.Name = SelectedDevie.DeviceName;
            item.Config.DeviceType = SelectedDevie.Type;

            var rlt = devicedonfig.WriteXml(item.Config);
            if (!rlt)
            {
                Logger.Add(new ViewLogger
                {
                    RecordTime = DateTime.Now,
                    Level = LoggerLevel.WARN,
                    Content = $"{item.Config.Name}[{item.Config.DeviceType.GetDescription()}]配置保存失败",
                });
                MessageBox.Show($"设备[{item.PathName}]配置保存异常:{rlt.Message}");
                return;
            }

            Logger.Add(new ViewLogger
            {
                RecordTime = DateTime.Now,
                Level = LoggerLevel.INFO,
                Content = $"{item.Config.Name}[{item.Config.DeviceType.GetDescription()}]配置保存成功",
            }) ;
        }

        /// <summary>
        /// 通讯测试
        /// </summary>
        private void LinkTest()
        {
            if (SelectedDevie == null)
            {
                return;
            }
            DeviceArgs config = SelectedDevie.Info.Config;
            var item = SelectedDevie.Info;

            string content = $"{config.DeviceType.GetDescription()}[{config.Name}]";

            DriveOperate operate = DriveOperate.Creat();
            OperateResult driveresult = operate.Open(config.DriveConfig);
            if (!driveresult)
            {
                //content = $"{config.Name}[{config.DeviceType.GetDescription()}]驱动打开失败";
                ContentName = content+ "驱动打开失败";
                ContentColor = Brushes.Green;

                Logger.Add(new ViewLogger
                {
                    RecordTime = DateTime.Now,
                    Level = LoggerLevel.WARN,
                    Content = ContentName,
                });

                SuperDHHLoggerManager.Warn(LoggerType.FROMLOG, "接口配置", "通讯测试", content);
                return;
                //return OperateResult.Failed(result.Message);
            }

            operate.RegisterDevice(config)
            .ThenAnd(result => result.Data.Init(config, operate.Drive).ConvertTo(result.Data))
            .ThenAnd(result=> result.Data.Device_ReadAll().ConvertTo(result.Data))
            .AttachIfSucceed(result => 
            {
                ContentName = content+"通讯连接成功";
                ContentColor = Brushes.Green;

                Logger.Add(new ViewLogger
                {
                    RecordTime = DateTime.Now,
                    Level = LoggerLevel.INFO,
                    Content = ContentName,
                });
            })
            .AttachIfFailed(result => 
            {
                //content = $"{config.DeviceType.GetDescription()}[{config.Name}]通讯连接失败";
                ContentName = content + "通讯连接失败";
                ContentColor = Brushes.Red;

                Logger.Add(new ViewLogger
                {
                    RecordTime = DateTime.Now,
                    Level = LoggerLevel.ERROR,
                    Content = ContentName,
                });

                SuperDHHLoggerManager.Warn(LoggerType.FROMLOG, "接口配置", "通讯测试", content);
            });

            operate.Dispose();
            //if (!devicerlt) 
            //{
            //    content = $"{config.Name}[{config.DeviceType.GetDescription()}]通讯连接失败";
            //    ContentName = content;
            //    ContentColor = Brushes.Green;

            //    Logger.Add(new ViewLogger
            //    {
            //        RecordTime = DateTime.Now,
            //        Level = LoggerLevel.INFO,
            //        Content = content,
            //    });

            //    SuperDHHLoggerManager.Warn(LoggerType.FROMLOG, "接口配置", "通讯测试", content);
            //}
            
            

            //ContentName = content;
            //ContentColor = Brushes.Green;



            //Logger.Add(new ViewLogger
            //{
            //    RecordTime = DateTime.Now,
            //    Level = LoggerLevel.INFO,
            //    Content = content,
            //});
        }


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

            return SupportDeviceConfigXml.GetSections(out List<SysteamSupportDeviceConfigInfo> config)
                .AttachIfSucceed(result =>
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

                    //if (GlobalModel.DicDeviceArgs.ContainsKey(name))
                    //{
                    //    GlobalModel.DicDeviceArgs.Remove(name);
                    //}

                    //deviceconfig.GetSections<DeviceArgs>();
                    var x = deviceconfig.DeviceConfig;

                    //GlobalModel.DicDeviceArgs.Add(name, deviceconfig.DeviceConfig);

                    //if (GlobalModel.DicDeviceInfo.ContainsKey(name))
                    //{
                    //    GlobalModel.DicDeviceInfo[name].Args = deviceconfig.DeviceConfig;
                    //}


                    if (!deviceconfiginfo.EnableDriveInit)
                    {
                        continue;
                    }

                    var config = deviceconfig.DeviceConfig;
                    //if (!SupportConfig.DicSupportBaudRate.TryGetValue(config.DriveConfig.CommunicationType, out ObservableCollection<string> baudrates))
                    //{
                    //    baudrates = new ObservableCollection<string>();
                    //}

                    DeviceConfigInfo deviceinfo = new DeviceConfigInfo
                    {
                        Config = config,
                        Drive = config.DriveConfig.CommunicationType,
                        PathName = file.Name.Split('.')[0],
                        //BaudRate = baudrates,
                    };

                    if (deviceinfo.Drive == DriveType.VehicleBus)
                    {
                        DriveType eType = DriveType.NULL;
                        string strParam1 = string.Empty;
                        string strParam2 = string.Empty;
                        string strParam3 = "0";

                        DriveHelper
                            .DecodeResourceString(deviceinfo.Config.DriveConfig.ResourceString, ref eType, ref strParam1, ref strParam2, ref strParam3)
                            .And(DriveHelper
                            .DecodeConfigString(deviceinfo.Config.DriveConfig.ConfigString,
                         out string canfdacceler, out string databaudrate, out uint channelindex));

                        //OperateResult rlt = DeviceResourceHelper.DecodeConfigString(deviceinfo.Config.DriveConfig.ConfigString,
                        // out string canfdacceler, out string databaudrate, out uint channelindex);
                        deviceinfo.CANDrive = strParam2;
                        deviceinfo.DriveIndex = Convert.ToUInt32(strParam3);
                        deviceinfo.SelectedCANFDacceler = canfdacceler;
                        deviceinfo.SelectedDatabaudRate = databaudrate;
                        deviceinfo.ChanneIndex = channelindex;
                    }

                    if (deviceinfo.Drive == DriveType.ASRL)
                    {
                        SerialParity ASRL_Parity = SerialParity.None;                               // 串口通讯_校验位;
                        int ASRL_DataBits = 8;                                                      // 串口通讯_数据位;
                        SerialStopBitsMode ASRL_StopBits = SerialStopBitsMode.One;                  // 串口通讯_停止位;
                        SerialFlowControlModes ASRL_FlowControl = SerialFlowControlModes.None;   // 串口通讯_流控制; 
                        int ASRL_Address = 0;
                        bool isuser = false;
                        DriveHelper.DecodeConfigString(deviceinfo.Config.DriveConfig.ConfigString, ref ASRL_Parity, ref ASRL_DataBits, ref ASRL_StopBits, ref ASRL_FlowControl, ref ASRL_Address, ref isuser);
                        deviceinfo.SelectSerialParitys = ASRL_Parity.ToString();
                        deviceinfo.DataBits = (uint)ASRL_DataBits;
                        deviceinfo.SelectStopBitsMode = ASRL_StopBits.ToString();
                        deviceinfo.SelectFlowControl = ASRL_FlowControl.ToString();
                        deviceinfo.UseSerial = isuser;
                    }

                    SupportConfig.DicSupportDeviceModel.TryGetValue(SupportConfig.DicSysteamDeviceConfigs[name].DeviceType, out ObservableCollection<string> devicemodels);

                    Devices.Add(new DeviceTypeInfo 
                    {
                         Type = SupportConfig.DicSysteamDeviceConfigs[name].DeviceType,
                         PathName = pathname,
                         DeviceName = SupportConfig.DicSysteamDeviceConfigs[name].DeviceModel,
                         DeviceNames = devicemodels,
                         Info = deviceinfo,
                         GetDeviceConfigs = new RelayCommand<object>(GetedDeviceConfigs),
                    });

                    Thread.Sleep(100);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.GetMessage(), "系统配置文件获取异常");
                SuperDHHLoggerManager.Exception(LoggerType.FROMLOG, nameof(IMX.ATS.DeviceConfig.MainViewModel), nameof(ReadConfig), ex);
                return OperateResult.Excepted(ex);
            }

            return OperateResult.Succeed();
        }

        #region  调试写入
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

            return SupportDeviceConfigXml.WriteXml(new List<SysteamSupportDeviceConfigInfo>
            {
               new SysteamSupportDeviceConfigInfo { DeviceType = EDeviceType.Product,  DeviceNum = 1, Description = "Product",DeviceModel = "Product_CAN", TypeName = "Product_CAN", FuncitonType = FuncitonType.Product, EnableFlow = true, EnableDriveInit = false, EnableManual = false, EnableMonitor = true},
               new SysteamSupportDeviceConfigInfo { DeviceType = EDeviceType.Unknow,  DeviceNum = 0, Description = "未知设备",DeviceModel = "未知设备", TypeName = "未知设备", FuncitonType = FuncitonType.ProductResult, EnableFlow = true, EnableDriveInit = true, EnableManual = false, EnableMonitor = true},
               new SysteamSupportDeviceConfigInfo { DeviceType = EDeviceType.Acquisition,  DeviceNum = 1, Description = "Acquisition", DeviceModel = "MCx", TypeName = "MCx", FuncitonType = FuncitonType.EquipmentResult, EnableFlow = true, EnableDriveInit = true, EnableManual = false, EnableMonitor = true},
               new SysteamSupportDeviceConfigInfo { DeviceType = EDeviceType.DCLoad,  DeviceNum = 1, Description = "DCLoad", DeviceModel = "AN23600E", TypeName = "AN23600E", FuncitonType = FuncitonType.DCLoad, EnableFlow = true, EnableDriveInit = true, EnableManual = false, EnableMonitor = true},
               new SysteamSupportDeviceConfigInfo { DeviceType = EDeviceType.HVDCSource,  DeviceNum = 1, Description = "HVDCSource", DeviceModel = "AN50300", TypeName = "AN50300", FuncitonType = FuncitonType.DCSource, EnableFlow = true, EnableDriveInit = true, EnableManual = false, EnableMonitor = true},
               new SysteamSupportDeviceConfigInfo { DeviceType = EDeviceType.APU,  DeviceNum = 1, Description = "APU", DeviceModel = "IT6800", TypeName = "IT6800", FuncitonType = FuncitonType.APU, EnableFlow = true, EnableDriveInit = true, EnableManual = false, EnableMonitor = true},
               new SysteamSupportDeviceConfigInfo { DeviceType = EDeviceType.ACSource,  DeviceNum = 1, Description = "ACSource", DeviceModel = "ANFH010S", TypeName = "ANFH010S", FuncitonType = FuncitonType.ACSource, EnableFlow = true, EnableDriveInit = true, EnableManual = false, EnableMonitor = true},
               new SysteamSupportDeviceConfigInfo { DeviceType = EDeviceType.Relay,  DeviceNum = 1, Description = "Relay", DeviceModel = "ZS4Bit", TypeName = "ZS4Bit", FuncitonType = FuncitonType.Relay, EnableFlow = true, EnableDriveInit = true, EnableManual = false, EnableMonitor = true},
            });
        }
        #endregion

        #endregion
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
            base.WindowClosedExecute(obj);
        }
        #endregion


        #region 构造方法
        public MainViewModel() 
        {
            OperateResult result = DeviceConfigInit().And(ReadConfig());

            //Devices.ForEach(x => 
            //{
            //    x.GetDeviceConfigs = new RelayCommand<object>(GetedDeviceConfigs);
            //});
        }
        #endregion
    }

    /// <summary>
    /// 设备信息
    /// </summary>
    public class DeviceTypeInfo : ViewModelBase
    {
        /// <summary>
        /// 设备类型
        /// </summary>
        public EDeviceType Type { get; set; }

        /// <summary>
        /// 配置文件地址
        /// </summary>
        public string PathName { get; set; }

        private ObservableCollection<string> devicenames;
        /// <summary>
        /// 设备型号列表
        /// </summary>
        public ObservableCollection<string> DeviceNames
        {
            get => devicenames;
            set => Set(nameof(DeviceNames), ref devicenames, value);
        }

        private string deviceName;
        /// <summary>
        /// 当前设备型号
        /// </summary>
        public string DeviceName
        {
            get => deviceName;
            set => Set(nameof(DeviceName), ref deviceName, value);
        }

        /// <summary>
        /// 获取设备配置信息
        /// </summary>
        public RelayCommand<object> GetDeviceConfigs { get; set; }

        private DeviceConfigInfo info;
        /// <summary>
        /// 设备配置信息
        /// </summary>
        public DeviceConfigInfo Info
        {
            get => info;
            set => Set(nameof(Info), ref info, value);
        }

    }


    /// <summary>
    /// 设备配置信息
    /// </summary>
    public class DeviceConfigInfo : ViewModelBase
    {
        private DriveType dirve = DriveType.ASRL;
        /// <summary>
        /// 设备类型
        /// </summary>
        public DriveType Drive
        {
            get => dirve;
            set
            {
                if (Set(nameof(Drive), ref dirve, value))
                {
                    ExtShow = value == DriveType.ASRL ? Visibility.Visible : Drive == DriveType.VehicleBus ? Visibility.Visible : Visibility.Collapsed;
                    ASRLMegShow = value == DriveType.ASRL ? Visibility.Visible : Visibility.Collapsed;
                    CANMegShow = value == DriveType.VehicleBus ? Visibility.Visible : Visibility.Collapsed;
                    NormalShow = value != DriveType.VehicleBus ? Visibility.Visible : Visibility.Collapsed;

                    if (!SupportConfig.DicSupportBaudRate.TryGetValue(value, out ObservableCollection<string> baudrates))
                    {
                        baudrates = new ObservableCollection<string>();
                    }
                    BaudRate = baudrates;
                    DriveResources.Clear();
                    try
                    {
                        int index = -1;
                        switch (value)
                        {
                            //case DriveType.CAN:
                            //    DriveResources.Add(Config.DriveConfig.ResourceString);
                            //    break;
                            case DriveType.TCPIP:
                                
                                var tcpresources = new NationalInstruments.Visa.ResourceManager().Find($"{value.ToString().ToUpper()}?*SOCKET").ToList();
                                for (int i = 0; i < tcpresources?.Count; i++) 
                                {
                                    DriveResources.Add(tcpresources[i]);
                                    if (tcpresources[i] == Config.DriveConfig.ResourceString)
                                    {
                                        index = i;
                                    }
                                }

                                SelectedDriveResourceIndex = index;

                                ExtShow = Visibility.Collapsed;
                                ASRLMegShow = Visibility.Collapsed;
                                CANMegShow = Visibility.Collapsed;
                                NormalShow = Visibility.Visible;
                                //tcpresources.ForEach(x =>
                                //{
                                //    DriveResources.Add(x);
                                //});
                                break;
                            case DriveType.VehicleBus:
                                ExtShow =  Visibility.Visible;
                                ASRLMegShow = Visibility.Collapsed;
                                CANMegShow = Visibility.Visible ;
                                NormalShow = Visibility.Collapsed;
                                break;
                            case DriveType.USB:
                            case DriveType.ASRL:
                                var resources = new NationalInstruments.Visa.ResourceManager().Find($"{value.ToString().ToUpper()}?*INSTR").ToList();
                                //int index = -1;
                                for (int i = 0; i < resources?.Count; i++)
                                {
                                    DriveResources.Add(resources[i]);
                                    if (resources[i] == Config.DriveConfig.ResourceString)
                                    {
                                        index = i;
                                    }
                                }

                                SelectedDriveResourceIndex = index;

                                ExtShow = value == DriveType.ASRL ? Visibility.Visible : Visibility.Collapsed;
                                ASRLMegShow = value == DriveType.ASRL ? Visibility.Visible : Visibility.Collapsed;
                                CANMegShow = Visibility.Collapsed;
                                NormalShow = Visibility.Visible;
                                //resources.ForEach(x =>
                                //{
                                //    DriveResources.Add(x);
                                //});
                                //DriveResources = new ResourceManager().Find($"{Config.DriveConfig.CommunicationType.ToString().ToUpper()}?*INSTR").ToList();
                                break;
                            default:
                                CANMegShow = Visibility.Collapsed;
                                NormalShow = Visibility.Visible;
                                ExtShow = Visibility.Collapsed;
                                break;

                        }
                    }
                    catch (Exception ex)
                    {
                        DriveResources.Add($"{ex.Message}");
                    }

                }
            }
        }

        /// <summary>
        /// 文件名称
        /// </summary>
        public string PathName { get; set; }

        private Visibility normalshow = Visibility.Visible;
        /// <summary>
        /// 常规配置显示
        /// </summary>
        public Visibility NormalShow
        {
            get => normalshow;
            set => Set(nameof(NormalShow), ref normalshow, value);
        }


        private ObservableCollection<string> driveResources = new ObservableCollection<string>();
        /// <summary>
        /// 驱动资源字符
        /// </summary>
        public ObservableCollection<string> DriveResources
        {
            get => driveResources;
            set => Set(nameof(DriveResources), ref driveResources, value);
        }

        private int selecteddriveresourceindex = -1;
        /// <summary>
        /// 当前选择资源字符串序号
        /// </summary>
        public int SelectedDriveResourceIndex
        {
            get
            {
                return selecteddriveresourceindex;
            } 
            set => Set(nameof(SelectedDriveResourceIndex), ref selecteddriveresourceindex, value);
        }

        #region 额外配置
        private Visibility extshow = Visibility.Collapsed;
        /// <summary>
        /// 额外配置显示
        /// </summary>
        public Visibility ExtShow
        {
            get => extshow;
            set => Set(nameof(ExtShow), ref extshow, value);
        }

        private string selectBaudRate;

        public string SelectBaudRate
        {
            get => selectBaudRate;
            set => Set(nameof(SelectBaudRate), ref selectBaudRate, value);
        }

        private ObservableCollection<string> baudRate = new ObservableCollection<string>();
        /// <summary>
        /// 波特率列表
        /// </summary>
        public ObservableCollection<string> BaudRate
        {
            get => baudRate;
            set => Set(nameof(BaudRate), ref baudRate, value);
        }

        #region CAN/CANFD额外配置

        private string candrive;
        /// <summary>
        /// 当前选择CAN驱动
        /// </summary>
        public string CANDrive
        {
            get => candrive;
            set => Set(nameof(CANDrive), ref candrive, value);
        }

        /// <summary>
        /// 支持CAN驱动列表
        /// </summary>
        public List<string> CANDrives { get; set; } = new List<string>
        {
            "USBCANFD_200U",
            "USBCANFD_400U",
            "USBCANFD_800U",
        };

        private uint driveIndex;
        /// <summary>
        /// CAN驱动编号
        /// </summary>
        public uint DriveIndex
        {
            get => driveIndex;
            set => Set(nameof(DriveIndex), ref driveIndex, value);
        }


        private Visibility canmesgshow = Visibility.Collapsed;
        /// <summary>
        /// CAN配置信息显示
        /// </summary>
        public Visibility CANMegShow
        {
            get => canmesgshow;
            set => Set(nameof(CANMegShow), ref canmesgshow, value);
        }

        private uint channelIndex;
        /// <summary>
        /// 通道号
        /// </summary>
        public uint ChanneIndex
        {
            get => channelIndex;
            set => Set(nameof(ChanneIndex), ref channelIndex, value);
        }

        /// <summary>
        /// 数据域波特率
        /// </summary>
        public List<string> DatabaudRate { get; set; } = new List<string> { "50Kbps", "100Kbps", "250Kbps", "500Kbps", "800Kbps", "1000Kbps", "2000Kbps", "4000Kbps", "5000Kbps" };


        private string selectedDatabaudRate;
        /// <summary>
        /// 当前选择数据波特率
        /// </summary>
        public string SelectedDatabaudRate
        {
            get => selectedDatabaudRate;
            set => Set(nameof(SelectedDatabaudRate), ref selectedDatabaudRate, value);
        }

        /// <summary>
        /// CANFD加速器
        /// </summary>
        public List<string> CANFDacceler { get; set; } = new List<string> { "是", "否" };

        private string selectedCANFDacceler;
        /// <summary>
        /// 当前选择数据波特率
        /// </summary>
        public string SelectedCANFDacceler
        {
            get => selectedCANFDacceler;
            set => Set(nameof(SelectedCANFDacceler), ref selectedCANFDacceler, value);
        }
        #endregion

        #region ASRL额外配置

        private Visibility asrlmesgshow = Visibility.Collapsed;
        /// <summary>
        /// CAN配置信息显示
        /// </summary>
        public Visibility ASRLMegShow
        {
            get => asrlmesgshow;
            set => Set(nameof(ASRLMegShow), ref asrlmesgshow, value);
        }

        /// <summary>
        /// 校验位
        /// </summary>
        public List<string> SerialParitys => new List<string> { "None", "Odd", "Even", "Mark", "Space" };

        private string selectserialparitys = "None";
        /// <summary>
        /// 当前选择校验位
        /// </summary>
        public string SelectSerialParitys
        {
            get => selectserialparitys;
            set => Set(nameof(SelectSerialParitys), ref selectserialparitys, value);
        }

        private uint databits = 8;
        /// <summary>
        /// 数据位
        /// </summary>
        public uint DataBits
        {
            get => databits;
            set => Set(nameof(DataBits), ref databits, value);
        }


        /// <summary>
        /// 停止位
        /// </summary>
        public List<string> StopBitsModes => new List<string> { "One", "OneAndOneHalf", "Two" };

        private string selectstopbitsmode;
        /// <summary>
        /// 当前选择停止位
        /// </summary>
        public string SelectStopBitsMode
        {
            get => selectstopbitsmode;
            set => Set(nameof(SelectStopBitsMode), ref selectstopbitsmode, value);
        }

        /// <summary>
        /// 控制流
        /// </summary>
        public List<string> FlowControl => new List<string> { "None", "XOnXOff", "RtsCts", "DtrDsr" };

        private string selectflowcontrol = "None";
        /// <summary>
        /// 当前选择控制流
        /// </summary>
        public string SelectFlowControl
        {
            get => selectflowcontrol;
            set => Set(nameof(SelectFlowControl), ref selectflowcontrol, value);
        }

        /// <summary>
        /// 是否使用原生接口
        /// </summary>
        public bool UseSerial { get; set; } = false;

        //public List<string> UseSerials => new List<string> { "true", "false" };

        //private string selectuseserial;
        ///// <summary>
        ///// 是否使用原生接口
        ///// </summary>
        //public string SelectUseSerial
        //{
        //    get => selectuseserial;
        //    set => Set(nameof(SelectUseSerial), ref selectuseserial, value);
        //}

        #endregion

        #endregion

        private DeviceArgs config = new DeviceArgs();
        /// <summary>
        /// 设备配置
        /// </summary>
        public DeviceArgs Config
        {
            get => config;
            set
            {
                if (Set(nameof(Config), ref config, value))
                {


                    //: DeviceResourceHelper.DecodeConfigString(Config.DriveConfig.ConfigString,
                    //                     ref  ASRL_Parity,
                    //                     ref  ASRL_DataBits,
                    //                     ref  ASRL_StopBits,
                    //                     ref  ASRL_FlowControl,
                    //                     ref  ASRL_Address,
                    //                     ref  IsUseSerial);

                }
            }
        }
    }

    /// <summary>
    /// 界面日志记录内容
    /// </summary>
    public class ViewLogger : ViewModelBase
    {
        private DateTime recordtime;
        /// <summary>
        /// 记录时间
        /// </summary>
        public DateTime RecordTime
        {
            get => recordtime;
            set => Set(nameof(RecordTime), ref recordtime, value);
        }

        private LoggerLevel level;
        /// <summary>
        /// 日志等级
        /// </summary>
        public LoggerLevel Level
        {
            get => level;
            set => Set(nameof(Level), ref level, value);
        }


        private string content;
        /// <summary>
        /// 日志内容
        /// </summary>
        public string Content
        {
            get => content;
            set => Set(nameof(Content), ref content, value);
        }

    }

}
