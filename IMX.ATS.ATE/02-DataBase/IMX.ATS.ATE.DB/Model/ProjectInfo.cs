using FreeSql.DataAnnotations;
using FreeSql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMX.DB.Model
{
    /// <summary>
    /// 项目信息库表
    /// </summary>
    public class Test_ProjectInfo : BaseEntity<Test_ProjectInfo, int>
    {
        /// <summary>
        /// 项目编号
        /// </summary>
        [Column(IsNullable = true)]
        public string ProjectSN { get; set; }

        /// <summary>
        /// 项目名称
        /// </summary>
        [Column(IsNullable = true)]
        public string ProjectName { get; set; }

        /// <summary>
        /// 标定电压
        /// </summary>
        public uint RatedVol { get; set; }

        /// <summary>
        /// 标定电流
        /// </summary>
        public uint RatedCur { get; set; }

        /// <summary>
        /// 标定功率
        /// </summary>
        public uint RatedPow { get; set; }

        /// <summary>
        /// 是否使用DBC
        /// </summary>
        public bool IsUseDDBC { get; set; } = false;

        /// <summary>
        /// 仲裁波特率
        /// </summary>
        public string BaudRate { get; set; } = "500Kbps";

        /// <summary>
        /// 数据域波特率
        /// </summary>
        public string DataBaudrate { get; set; } = "500Kbps";
    }

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
    }

    /// <summary>
    /// DBC配置数据库列表
    /// </summary>
    public class Test_DBCConfig : BaseEntity<Test_DBCConfig, int>
    {
        /// <summary>
        /// 对应DBC文件ID
        /// </summary>
        public int DBCFileID { get; set; }

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
        /// DBC上报信号配置
        /// </summary>
        [JsonMap]
        public List<Test_DBCInfo> Test_DBCReceiveSignals { get; set; } = new List<Test_DBCInfo>();

        /// <summary>
        /// 上次更新人员
        /// </summary>
        public string UpdateOperator { get; set; }
    }

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
    }
    #endregion

    public class Test_Process : BaseEntity<Test_Process, int>
    {

        //public int FunctionID { get; set; }

        /// <summary>
        /// 对应项目ID
        /// </summary>
        public int ProjectID { get; set; }

        /// <summary>
        /// 流程名称
        /// </summary>
        public string FunctionName { get; set; }

        /// <summary>
        /// 试验流程
        /// </summary>
        [JsonMap]
        public List<ModTestProcess> Test_Flows { get; set; }

        /// <summary>
        /// 上次更新人员
        /// </summary>
        public string UpdateOperator { get; set; }
    }

    /// <summary>
    /// 实验流程操作存储模版类
    /// </summary>
    public class ModTestProcess
    {
        /// <summary>
        /// 步骤
        /// </summary>
        public int Step { get; set; }

        /// <summary>
        /// 模板名称
        /// </summary>
        public string FuntionName { get; set; }
        /// <summary>
        /// 自定义模板名称
        /// </summary>
        public string CustomName { get; set; }

        /// <summary>
        /// 试验描述
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 模板类型
        /// </summary>
        public string Type { get; set; }

        ///// <summary>
        ///// 操作设备
        ///// </summary>
        //public string DeviceName { get; set; }

        /// <summary>
        /// 模板内容
        /// </summary>
        public string Funtion { get; set; }
    }
}
