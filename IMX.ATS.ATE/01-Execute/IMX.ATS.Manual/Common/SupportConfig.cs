using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMX.ATS.Manual
{
    internal static class SupportConfig
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
}
