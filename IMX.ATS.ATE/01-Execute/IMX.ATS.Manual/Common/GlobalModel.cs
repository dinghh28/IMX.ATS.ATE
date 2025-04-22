using IMX.DB.Model;
using IMX.Device.Base;
using IMX.Device.Base.DriveOperate;
using IMX.Device.Common;
using IMX.Function.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMX.ATS.Manual
{
    internal static class GlobalModel
    {        /// <summary>
             /// 用户信息
             /// </summary>
        public static UserInfo UserInfo { get; set; } = new UserInfo();

        /// <summary>
        /// 工装初始化状态
        /// </summary>
        public static bool CabinetSate { get; set; } = true;

        /// <summary>
        /// 设备操作句柄字典[设备操作句柄, 驱动接口]
        /// </summary>
        public static Dictionary<IDeviceOperate, DriveOperate> DicDeviceOperate { get; set; } = new Dictionary<IDeviceOperate, DriveOperate>();

        /// <summary>
        /// 设备配置字典[设备名称, 配置参数]
        /// </summary>
        public static Dictionary<string, DeviceArgs> DicDeviceArgs { get; set; } = new Dictionary<string, DeviceArgs>();

        /// <summary>
        /// 设备相关信息字典[设备名称, 设备所有配置]
        /// </summary>
        public static Dictionary<string, DeviceInfo_ALL> DicDeviceInfo { get; set; } = new Dictionary<string, DeviceInfo_ALL>();

        /// <summary>
        /// 设备驱动字典[驱动资源字符, 驱动接口]
        /// </summary>
        public static Dictionary<string, DriveOperate> DicDeviceDrives { get; set; } = new Dictionary<string, DriveOperate>();
    }

    public class DeviceInfo_ALL
    {
        public DeviceArgs Args { get; set; }

        public IDeviceOperate DeviceOperate { get; set; }

        public DriveOperate Drive { get; set; }

        public SysteamSupportDeviceConfigInfo Config { get; set; }
    }

    /// <summary>
    /// 系统支持设备配置信息模型
    /// </summary>
    public class SysteamSupportDeviceConfigInfo
    {
        /// <summary>
        /// 设备类型
        /// </summary>
        public EDeviceType DeviceType { get; set; } = EDeviceType.Unknow;

        /// <summary>
        /// 设备允许连接数量
        /// </summary>
        public int DeviceNum { get; set; } = 0;

        /// <summary>
        /// 设备名称
        /// </summary>
        public string TypeName { get; set; } = "未知设备";

        /// <summary>
        /// 设备描述
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// 设备型号
        /// </summary>
        public string DeviceModel { get; set; }

        /// <summary>
        /// 自动配置模板
        /// </summary>
        public FuncitonType FuncitonType { get; set; }

        /// <summary>
        /// 是否允许设备驱动初始化
        /// </summary>
        public bool EnableDriveInit { get; set; } = false;

        /// <summary>
        /// 是否允许自动化流程操作
        /// </summary>
        public bool EnableFlow { get; set; } = false;

        /// <summary>
        /// 是否允许手动操作
        /// </summary>
        public bool EnableManual { get; set; } = false;

        /// <summary>
        /// 是否允许上报监控
        /// </summary>
        public bool EnableMonitor { get; set; } = false;
    }
}
