using FreeSql.DataAnnotations;
using FreeSql;
using System.Collections.Generic;
using IMX.ATE.Common;
using IMX.Common;

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

        ///// <summary>
        ///// 标定电压
        ///// </summary>
        //public uint RatedVol { get; set; }

        ///// <summary>
        ///// 标定电流
        ///// </summary>
        //public uint RatedCur { get; set; }

        ///// <summary>
        ///// 标定功率
        ///// </summary>
        //public uint RatedPow { get; set; }

        /// <summary>
        /// 是否使用DBC
        /// </summary>
        public bool IsUseDDBC { get; set; } = false;

        /// <summary>
        /// 对应DBC配置ID
        /// </summary>
        public int DBCConfigID { get; set; } = -1;

        /// <summary>
        /// 对应DBC配置名称
        /// </summary>
        public string DBCConfigName { get; set; }

        /// <summary>
        /// 供电电能
        /// </summary>
        public Electricity Electricity { get; set; } = Electricity.Single;

        /// <summary>
        /// 仲裁波特率
        /// </summary>
        public string BaudRate { get; set; } = "500Kbps";

        /// <summary>
        /// 数据域波特率
        /// </summary>
        public string DataBaudrate { get; set; } = "500Kbps";

        /// <summary>
        /// 试验流程
        /// </summary>
        [JsonMap]
        public List<ModTestProcess> Test_OpenFlows { get; set; }

        /// <summary>
        /// 试验流程
        /// </summary>
        [JsonMap]
        public List<ModTestProcess> Test_ShutFlows { get; set; }
    }

    /// <summary>
    /// 测试流程
    /// </summary>
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
        /// 试验存储读取数据列表
        /// </summary>
        [JsonMap]
        public List<ModTestDataInfo> Test_ReadData_Pro { get; set; }

        /// <summary>
        /// 试验存储设置数据列表
        /// </summary>
        [JsonMap]
        public List<ModTestDataInfo> Test_SetData_Pro { get; set; }

        /// <summary>
        /// 试验存储读取数据列表
        /// </summary>
        [JsonMap]
        public List<ModTestDataInfo> Test_ReadData_Euq { get; set; }

        /// <summary>
        /// 试验存储设置数据列表
        /// </summary>
        [JsonMap]
        public List<ModTestDataInfo> Test_SetData_Euq { get; set; }

        /// <summary>
        /// 使用计算值作为数据
        /// </summary>
        public bool UseCalculateData { get; set; } = false;

        /// <summary>
        /// 试验存储计算值数据列表
        /// </summary>
        [JsonMap]
        public List<ModTestDataInfo> Test_CalculateData { get; set; }

        /// <summary>
        /// 是否允许使用自定义数据
        /// </summary>
        public bool UseCustomData { get; set; } = false;

        /// <summary>
        /// 试验存储用户自定义数据列表
        /// </summary>
        [JsonMap]
        public List<ModTestDataInfo> Test_CustomData { get; set; }

        /// <summary>
        /// 上次更新人员
        /// </summary>
        public string UpdateOperator { get; set; }

        /// <summary>
        /// 描述/备注
        /// </summary>
        public string Description { get; set; }
    }

    /// <summary>
    /// 试验方案信息库表
    /// </summary>
    public class Test_Programme : BaseEntity<Test_Programme, int>
    {
        /// <summary>
        /// 对应项目ID
        /// </summary>
        public int ProjectID { get; set; }

        ///// <summary>
        ///// 对应项目名称
        ///// </summary>
        //public int ProjectName{ get; set; }

        /// <summary>
        /// 试验流程名称
        /// </summary>
        [JsonMap]
        public List<string> Test_FlowNames { get; set; }

        /// <summary>
        /// 下电试验流程名称
        /// </summary>
        [JsonMap]
        public List<string> TestOff_FlowNames { get; set; }

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
