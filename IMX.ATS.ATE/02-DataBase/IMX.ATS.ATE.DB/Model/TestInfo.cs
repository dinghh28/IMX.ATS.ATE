using FreeSql.DataAnnotations;
using FreeSql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IMX.Common;

namespace IMX.DB.Model
{
    /// <summary>
    /// 实验结果条目信息
    /// </summary>
    [Table(Name = "Test_ItemInfo_{yyyy}", AsTable = "createtime=2023-1-1(1 year)")]
    public class Test_ItemInfo : BaseEntity<Test_ItemInfo, int>
    {
        /// <summary>
        /// 项目ID
        /// </summary>
        public int ProjectID { get; set; }

        /// <summary>
        /// 产品编号
        /// </summary>
        [Column(IsNullable = false)]
        public string ProductSN { get; set; }

        /// <summary>
        /// 项目名称
        /// </summary>
        [Column(IsNullable = false)]
        public string ProjectName { get; set; }

        ///// <summary>
        ///// 试验工序
        ///// </summary>
        //[Column(IsNullable = false)]
        //public ProcessType ProcessType { get; set; }

        ///// <summary>
        ///// 老化标定总时长
        ///// </summary>
        //public uint CalibrationTime { get; set; }

        /// <summary>
        /// 实际运行时间
        /// </summary>
        public long ActualRunTime { get; set; }

        /// <summary>
        /// 试验结果
        /// </summary>
        public ResultState Result { get; set; } = ResultState.UNACCOMPLISHED;

        /// <summary>
        /// 操作人员
        /// </summary>
        public string Operator { get; set; } = "amdin";

        /// <summary>
        /// 故障信息
        /// </summary>
        public string ErrorInfo { get; set; } = string.Empty;
    }

    /// <summary>
    /// 实验结果数据信息
    /// </summary>
    public class Test_DataInfo : BaseEntity<Test_DataInfo, int>
    {
        /// <summary>
        /// 结果条目ID
        /// </summary>
        public int TestItemID { get; set; }

        /// <summary>
        /// 样品编号
        /// </summary>
        public string ProductSN { get; set; }

        /// <summary>
        /// 流程名称
        /// </summary>
        public string FlowName { get; set; }

        /// <summary>
        /// 步骤名称
        /// </summary>
        public string StepName { get; set; }

        /// <summary>
        /// 步骤序号
        /// </summary>
        public int StepIndex { get; set; }

        /// <summary>
        /// 产品数据
        /// </summary>
        [JsonMap]
        public List<ModTestDataInfo> Pro_Data { get; set; }

        /// <summary>
        /// 产品详细数据
        /// </summary>
        [JsonMap]
        public List<ModDeviceReadData> Pro_DeviceRead { get; set; }

        /// <summary>
        /// 设备数据
        /// </summary>
        [JsonMap]
        public List<ModTestDataInfo> Euq_Data { get; set; }

        /// <summary>
        /// 设备数据
        /// </summary>
        [JsonMap]
        public List<ModDeviceReadData> Euq_DeviceRead { get; set; }

        /// <summary>
        /// 试验结果
        /// </summary>
        public ResultState Result { get; set; } = ResultState.SUCCESS;

        /// <summary>
        /// 故障信息
        /// </summary>
        public string ErrorInfo { get; set; }
    }
}
