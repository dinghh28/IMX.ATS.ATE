using IMX.Device.Common;
using IMX.Function.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMX.ATS.ATE.Common
{
    /// <summary>
    /// 系统支持配置类
    /// </summary>
    public static class SupportConfig
    {
        /// <summary>
        /// 系统支持设备配置列表
        /// </summary>
        public static List<SysteamSupportDeviceConfigInfo> SysteamDeviceConfigs { get; set; } = new List<SysteamSupportDeviceConfigInfo>();

        /// <summary>
        /// 系统支持设备配置字典[设备类型, 设备配置信息]
        /// </summary>
        public static Dictionary<string, SysteamSupportDeviceConfigInfo> DicSysteamDeviceConfigs { get; set; } = new Dictionary<string, SysteamSupportDeviceConfigInfo>();
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
        public FuncitonType FuncitonType { get; set; } = FuncitonType.NONE;

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
