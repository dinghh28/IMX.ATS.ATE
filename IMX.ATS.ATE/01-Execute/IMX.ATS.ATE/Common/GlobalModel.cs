

using IMX.ATS.ATE.Common;
using IMX.DB.Model;
using IMX.Device.Base;
using IMX.Device.Base.DriveOperate;
using IMX.Device.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public static Dictionary<string, DriveOperate> DicDeviceDrives = new Dictionary<string, DriveOperate>();

        /// <summary>
        /// 设备操作句柄字典[设备操作句柄, 驱动接口]
        /// </summary>
        public static Dictionary<IDeviceOperate, DriveOperate> DicDeviceOperate = new Dictionary<IDeviceOperate, DriveOperate>();
    }
}
