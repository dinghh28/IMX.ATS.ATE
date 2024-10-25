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
using IMX.ATS.ATE.Common;
using IMX.Common;
using IMX.Device.Base;
using IMX.Device.Base.DriveOperate;
using IMX.Device.Common;
using IMX.Function.Base;
using Super.Zoo.Framework;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
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
        #endregion

        #region 私有方法
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
                    if (device.EnableDriveInit)
                    {
                        //SupportConfig.DicSupportDevice.Add(device.DeviceType, device.TypeName);
                        SupportConfig.DicSysteamDeviceConfigs.Add(device.Description, device);
                    }
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

            return SupportDeviceConfigXml.WriteXml(new List<SysteamSupportDeviceConfigInfo>
            {
               new SysteamSupportDeviceConfigInfo { DeviceType = EDeviceType.Product,  DeviceNum = 1, Description = "Product",DeviceModel = "Product_CAN", TypeName = "Product_CAN", FuncitonType = FuncitonType.Product, EnableFlow = true, EnableDriveInit = true, EnableManual = false, EnableMonitor = true},
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
        private void ReadConfig()
        {
            string folderPath = $@"{AppDomain.CurrentDomain.BaseDirectory}\Config\ConfigDevice";
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            FileInfo[] files = new DirectoryInfo(folderPath).GetFiles();

            foreach (FileInfo file in files)
            {
                //    var device = DeviceTypes.Find(x => file.Name.Split('_')[0].Contains(x.Type.ToString()));
                //    if (device != null)
                //    {
                string pathname = file.Name.Split('.')[0];
                string num = pathname[pathname.Length - 1].ToString();

                //系统支持当前设备数量判断
                if (Convert.ToInt32(num) >= SupportConfig.DicSysteamDeviceConfigs[pathname].DeviceNum)
                {
                    continue;
                }

                //        Device_Config deviceconfig = new Device_Config(pathname);
                //        var config = deviceconfig.DeviceConfig;

                //        DeviceConfigInfo deviceinfo = new DeviceConfigInfo
                //        {
                //            Config = config,
                //            Drive = config.DriveConfig.CommunicationType,
                //            PathName = pathname,
                //            Name = $"{config.DeviceType.GetDescription()}-{num}",
                //            BaudRate = SupportConfig.DicSupportBaudRate[config.DriveConfig.CommunicationType],
                //        };

                //        if (deviceinfo.Drive == DriveType.CAN)
                //        {
                //            DriveType eType = DriveType.NULL;
                //            string strParam1 = string.Empty;
                //            string strParam2 = string.Empty;
                //            string strParam3 = "0";

                //            DeviceResourceHelper
                //                .DecodeResourceString(deviceinfo.Config.DriveConfig.ResourceString, ref eType, ref strParam1, ref strParam2, ref strParam3)
                //                .And(DeviceResourceHelper
                //                .DecodeConfigString(deviceinfo.Config.DriveConfig.ConfigString,
                //             out string canfdacceler, out string databaudrate, out uint channelindex));

                //            //OperateResult rlt = DeviceResourceHelper.DecodeConfigString(deviceinfo.Config.DriveConfig.ConfigString,
                //            // out string canfdacceler, out string databaudrate, out uint channelindex);
                //            deviceinfo.CANDrive = strParam2;
                //            deviceinfo.DriveIndex = Convert.ToUInt32(strParam3);
                //            deviceinfo.SelectedCANFDacceler = canfdacceler;
                //            deviceinfo.SelectedDatabaudRate = databaudrate;
                //            deviceinfo.ChanneIndex = channelindex;
                //        }

                //        if (deviceinfo.Drive == DriveType.ASRL)
                //        {
                //            SerialParity ASRL_Parity = SerialParity.None;                               // 串口通讯_校验位;
                //            int ASRL_DataBits = 8;                                                      // 串口通讯_数据位;
                //            SerialStopBitsMode ASRL_StopBits = SerialStopBitsMode.One;                  // 串口通讯_停止位;
                //            SerialFlowControlModes ASRL_FlowControl = SerialFlowControlModes.None;   // 串口通讯_流控制; 
                //            int ASRL_Address = 0;
                //            bool isuser = false;
                //            DeviceResourceHelper.DecodeConfigString(deviceinfo.Config.DriveConfig.ConfigString, ref ASRL_Parity, ref ASRL_DataBits, ref ASRL_StopBits, ref ASRL_FlowControl, ref ASRL_Address, ref isuser);
                //            deviceinfo.SelectSerialParitys = ASRL_Parity.ToString();
                //            deviceinfo.DataBits = (uint)ASRL_DataBits;
                //            deviceinfo.SelectStopBitsMode = ASRL_StopBits.ToString();
                //            deviceinfo.SelectFlowControl = ASRL_FlowControl.ToString();
                //            deviceinfo.UseSerial = isuser;
                //        }

                //        device.DevieConfigs.Add(deviceinfo);

                //    }
            }
        }


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
        public DeviceInitViewModel() { }
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
