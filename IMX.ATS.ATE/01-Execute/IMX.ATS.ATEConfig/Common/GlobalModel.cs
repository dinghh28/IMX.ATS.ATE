
using IMX.DB.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMX.ATS.ATEConfig
{
    public class GlobalModel
    {
        /// <summary>
        /// 用户信息
        /// </summary>
        public static UserInfo UserInfo { get; set; } = new UserInfo();

        /// <summary>
        /// 项目信息
        /// </summary>
        public static Test_ProjectInfo Test_ProjectInfo { get; set; } = new Test_ProjectInfo();

        /// <summary>
        /// DBC配置信息
        /// </summary>
        public static Test_DBCConfig TestDBCconfig { get; set; } = new Test_DBCConfig();

        /// <summary>
        /// DBC文件配置信息
        /// </summary>
        public static Test_DBCFileInfo TestDBCFileInfo { get; set; } = new Test_DBCFileInfo();

        /// <summary>
        /// 是否为新建项目
        /// </summary>
        public static bool IsNewProject { get; set; } = false;
    }
}
