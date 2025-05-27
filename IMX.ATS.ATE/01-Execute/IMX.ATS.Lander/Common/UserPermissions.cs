using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMX.ATS.Lander.Common
{
    [Flags]
    internal enum UserPermissions
    {

        /// <summary>
        /// 测试平台
        /// </summary>
        [Description("测试平台")]
        ATE = 1 << 0,
        /// <summary>
        /// 测试项目配置
        /// </summary>
        [Description("测试项目配置")]
        ATECONFIG = 1 << 1,
        /// <summary>
        /// 数据管理
        /// </summary>
        [Description("数据管理")]
        DIOS = 1 << 2,
    }
}
