#region << 版 本 注 释 >>
/*----------------------------------------------------------------
 * 版权所有 (c) 2025   保留所有权利。
 * CLR版本：4.0.30319.42000
 * 机器名称：LAPTOP-9Q9TTD5V
 * 公司名称：
 * 命名空间：IMX.ATS.DeviceConfig.Common
 * 唯一标识：552b322e-219b-41e7-abda-bf0bde1ad365
 * 文件名：SupportConfig
 * 当前用户域：LAPTOP-9Q9TTD5V
 * 
 * 创建者：58274
 * 创建时间：2025/2/25 17:18:55
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
using IMX.ATE.Common.SupportDevice;
using IMX.Device.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMX.ATS.DeviceConfig
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

        /// <summary>
        /// 驱动支持波特率字典[驱动类型, 波特率]
        /// </summary>
        public static Dictionary<DriveType, ObservableCollection<string>> DicSupportBaudRate => new Dictionary<DriveType, ObservableCollection<string>>
        {
            { DriveType.ASRL, new ObservableCollection<string>{ "300", "1200", "2400", "9600", "19200", "38400", "115200" } },
            { DriveType.VehicleBus, new ObservableCollection<string>{ "5Kbps", "10Kbps", "20Kbps", "50Kbps", "100Kbps", "125Kbps", "250Kbps", "500Kbps", "800Kbps", "1000Kbps" }},
            { DriveType.USB, new ObservableCollection<string>()},
            { DriveType.TCPIP, new ObservableCollection<string>()},
            { DriveType.NULL, new ObservableCollection<string>()},
        };

        /// <summary>
        /// 设备支持型号字典[设备类型, 设备型号]
        /// </summary>
        public static Dictionary<EDeviceType, ObservableCollection<string>> DicSupportDeviceModel => new Dictionary<EDeviceType, ObservableCollection<string>>
        {

        };
    }
}
