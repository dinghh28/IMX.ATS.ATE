
using IMX.DB.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMX.ATS.DBCConfig
{
    public class GlobalModel
    {
        /// <summary>
        /// 用户信息
        /// </summary>
        public static UserInfo UserInfo { get; set; } = new UserInfo();

        /// <summary>
        /// DBC配置信息
        /// </summary>
        public static Test_DBCConfig Test_DBC { get; set;} = new Test_DBCConfig();

        /// <summary>
        /// DBC文件信息
        /// </summary>
        public static Test_DBCFileInfo Test_DBCFileInfo { get; set; } = null;


        /// <summary>
        /// DBC文件配置变更后信息
        /// </summary>
        public static Test_DBCFileInfo TestDBCFileInfo_Change { get; set; } = null;

        /// <summary>
        /// 是否为新建配置
        /// </summary>
        public static bool IsNew { get; set; } = false;
    }
}
