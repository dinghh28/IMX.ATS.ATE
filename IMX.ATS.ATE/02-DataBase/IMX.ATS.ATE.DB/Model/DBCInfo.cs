using FreeSql.DataAnnotations;
using FreeSql;
using IMX.ATE.Common;
using Piggy.VehicleBus.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace IMX.DB.Model
{
    #region DBC
    /// <summary>
    /// DBC文件数据库列表
    /// </summary>
    public class Test_DBCFileInfo : BaseEntity<Test_DBCFileInfo, int>
    {
        /// <summary>
        /// 文件名称
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// 文件说明
        /// </summary>
        public string FileDescription { get; set; }

        /// <summary>
        /// 文件大小
        /// </summary>
        public int FileSize { get; set; }

        /// <summary>
        /// 文件后缀
        /// </summary>
        public string FileExtension { get; set; }

        /// <summary>
        /// 文本内容
        /// </summary>
        [Column(StringLength = -1)]
        public byte[] FileContent { get; set; }

        /// <summary>
        /// 文件上传人员
        /// </summary>
        public string Operator { get; set; }

        ///// <summary>
        ///// DBC配置内容(子项)
        ///// </summary>
        //[Navigate(nameof(Test_DBCConfig.Id))]
        //public List<Test_DBCConfig> DBCConfigs { get; set;}
    }

    /// <summary>
    /// DBC配置数据库列表
    /// </summary>
    public class Test_DBCConfig : BaseEntity<Test_DBCConfig, int>
    {
        /// <summary>
        /// 配置名称
        /// </summary>
        public string ConfigName { get; set; }

        /// <summary>
        /// DBC配置说明
        /// </summary>
        public string Describe { get; set; }

        /// <summary>
        /// 对应DBC文件ID
        /// </summary>
        public int DBCFileID { get; set; }

        /// <summary>
        /// 对应DBC文件名称
        /// </summary>
        public string DBCFileName { get; set; }

        /// <summary>
        /// 电能类型
        /// </summary>
        public Electricity Electricity { get; set; }

        /// <summary>
        /// 当前配置使用使能
        /// </summary>
        public bool EnableUse { get; set; } = false;

        /// <summary>
        /// 当前配置下发信号使能
        /// </summary>
        public bool EnableUseSend { get; set; } = false;

        /// <summary>
        /// 当前配置上报信号使能
        /// </summary>
        public bool EnableUseReceive { get; set; } = false;

        /// <summary>
        /// 对应项目ID
        /// </summary>
        public int ProjectID { get; set; }

        /// <summary>
        /// DBC下发信号配置
        /// </summary>
        [JsonMap]
        public List<Test_DBCInfo> Test_DBCSendSignals { get; set; } = new List<Test_DBCInfo>();

        /// <summary>
        /// DBC下发消息配置
        /// </summary>
        [JsonMap]
        public List<Test_DBCMessageInfo> Test_DBCSendMessages { get; set; } = new List<Test_DBCMessageInfo>();


        /// <summary>
        /// DBC上报信号配置
        /// </summary>
        [JsonMap]
        public List<Test_DBCInfo> Test_DBCReceiveSignals { get; set; } = new List<Test_DBCInfo>();

        /// <summary>
        /// 上次更新人员
        /// </summary>
        public string UpdateOperator { get; set; }

        ///// <summary>
        ///// DBC文件(父项)
        ///// </summary>
        //[Navigate(nameof(DBCFileID))]
        //public Test_DBCFileInfo FileInfo { get; set; }
    }

    #region 下发帧
    /// <summary>
    /// DBC下发帧配置
    /// </summary>
    public class Test_DBCMessageInfo
    {
        /// <summary>
        /// 帧ID
        /// </summary>
        public uint Message_ID { get; set; }

        /// <summary>
        /// 帧名
        /// </summary>
        public string MessageName { get; set; }

        /// <summary>
        /// 消息发送周期（毫秒）
        /// </summary>
        public uint CycleTime { get; set; }

        /// <summary>
        /// 消息报文格式
        /// </summary>
        public FrameFormat FrameFormat { get; set; }
    }
    #endregion

    /// <summary>
    /// DBC存储信息
    /// </summary>
    public class Test_DBCInfo
    {
        /// <summary>
        /// 帧ID
        /// </summary>
        public uint Message_ID { get; set; }

        /// <summary>
        /// 帧名
        /// </summary>
        public string MessageName { get; set; }

        /// <summary>
        /// 信号名
        /// </summary>
        public string Signal_Name { get; set; }

        /// <summary>
        /// 用户自定义名称
        /// </summary>
        public string Custom_Name { get; set; }

        /// <summary>
        /// 信号初始值
        /// </summary>
        public string SignalInitValue { get; set; }

        /// <summary>
        /// 是否为固定信号
        /// </summary>
        public bool IsRegular { get; set; }
    }

    #region 用户自定义帧
    /// <summary>
    /// 用户自定义下发帧数据库列表
    /// </summary>
    public class Test_CustomMessageInfo : BaseEntity<Test_CustomMessageInfo, int>
    {
        /// <summary>
        /// 对应DBC配置ID
        /// </summary>
        public int DBCConfigID { get; set; } = -1;

        /// <summary>
        /// 配置名称
        /// </summary>
        public string ConfigName { get; set; }

        /// <summary>
        /// 自定义消息列表
        /// </summary>
        [JsonMap]
        public List<Test_CustomMessage> CustomMessages { get; set; } = new List<Test_CustomMessage>();

        /// <summary>
        /// 上次更新人员
        /// </summary>
        public string UpdateOperator { get; set; }
    }

    /// <summary>
    /// 自定义消息存储信息
    /// </summary>
    public class Test_CustomMessage
    {
        /// <summary>
        /// 消息ID
        /// </summary>
        public uint Message_ID { get; set; }

        /// <summary>
        /// 消息发送周期（毫秒）
        /// </summary>
        public uint CycleTime { get; set; }

        /// <summary>
        /// 消息报文格式
        /// </summary>
        public FrameFormat FrameFormat { get; set; }

        /// <summary>
        /// 发送次数
        /// </summary>
        public int SendCount { get; set; }

        /// <summary>
        /// 发送十六进制数据
        /// </summary>
        public byte[] Data { get; set; } = new byte[0];
    }
    #endregion

    #endregion
}
