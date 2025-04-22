using GalaSoft.MvvmLight;
using IMX.DB.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMX.ATE.Framework
{
    /// <summary>
    /// DBC配置信息
    /// </summary>
    public class DBCConfigInfo : ViewModelBase
    {
        /// <summary>
        /// DBC配置
        /// </summary>
        public Test_DBCConfig Config { get; set; }

        /// <summary>
        /// DBC文件配置
        /// </summary>
        public Test_DBCFileInfo FileInfo { get; set; }
    }
}
