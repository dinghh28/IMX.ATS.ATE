using IMX.DB.Model;
using IMX.Device.Base;
using IMX.Device.Base.DriveOperate;
using IMX.Device.Common;
using System.Collections.Generic;

namespace IMX.ATS.ATE
{
    public class GlobalModel
    {
        /// <summary>
        /// 用户信息
        /// </summary>
        public static UserInfo UserInfo { get; set; } = new UserInfo();

        /// <summary>
        /// 设备驱动字典[驱动资源字符, 驱动接口]
        /// </summary>
        public static Dictionary<string, DriveOperate> DicDeviceDrives { get; set; } = new Dictionary<string, DriveOperate>();

        /// <summary>
        /// 设备操作句柄字典[设备操作句柄, 驱动接口]
        /// </summary>
        public static Dictionary<IDeviceOperate, DriveOperate> DicDeviceOperate { get; set; } = new Dictionary<IDeviceOperate, DriveOperate>();

        /// <summary>
        /// 设备配置字典[设备名称, 配置参数]
        /// </summary>
        public static Dictionary<string, DeviceArgs> DicDeviceArgs { get; set; } =  new Dictionary<string, DeviceArgs>();

        /// <summary>
        /// 设备相关信息字典[设备名称, 设备所有配置]
        /// </summary>
        public static Dictionary<string, DeviceInfo_ALL> DicDeviceInfo { get; set; } = new Dictionary<string, DeviceInfo_ALL>();

        /// <summary>
        /// 工装初始化状态
        /// </summary>
        public static bool CabinetSate { get; set; } = true;

        /// <summary>
        /// 试验线程运行状态
        /// </summary>
        public static bool IsTestThreadRun { get; set; } = true;
    }

    public class DeviceInfo_ALL
    {
        public DeviceArgs Args { get; set; }

        public IDeviceOperate DeviceOperate { get; set; }

        public DriveOperate Drive { get; set; }

        public SysteamSupportDeviceConfigInfo Config { get; set; }
    }
}
