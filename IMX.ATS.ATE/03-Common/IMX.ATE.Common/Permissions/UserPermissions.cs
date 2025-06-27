using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMX.ATS.Common
{
    [Flags]
    public enum UserPermissions
    {

        /// <summary>
        /// 测试平台
        /// </summary>
        [UserPermissionsAttribute("测试平台", "QVRF")]
        ATE = 1 << 0,
        /// <summary>
        /// 测试项目配置
        /// </summary>
        [UserPermissionsAttribute("测试项目配置", "QVRFQ29uZmln")]
        ATEConfig = 1 << 1,
        /// <summary>
        /// 数据管理
        /// </summary>
        [UserPermissionsAttribute("数据管理", "")]
        DIOS = 1 << 2,
        /// <summary>
        /// DBC配置
        /// </summary>
        [UserPermissionsAttribute("DBC配置", "REJDQ29uZmln")]
        DBCConfig = 1 << 3,
        /// <summary>
        /// 手动操作平台
        /// </summary>
        [UserPermissionsAttribute("手动操作平台", "TWFudWFs")]
        Manual = 1<< 4,
        /// <summary>
        /// 设备配置平台
        /// </summary>
        [UserPermissionsAttribute("设备配置平台", "RGV2aWNlQ29uZmln")]
        DeviceConfig = 1<<5,
        /// <summary>
        /// 用户管理
        /// </summary>
        [UserPermissionsAttribute("用户管理", "VXNlck1hbmFnZQ==")]
        UserManage = 1 << 6,

        ALL = ATE| ATEConfig| DIOS| DBCConfig| Manual| DeviceConfig|UserManage

    }
}
