using IMX.ATS.ATEConfig.Function;
using IMX.Function.Base;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMX.ATS.ATEConfig
{
    /// <summary>
    /// 当前项目支持配置
    /// </summary>
    public static  class SupportConfig
    {
        /// <summary>
        /// 系统空间名
        /// </summary>
        public static string SystemName => "IMX.ATS.ATEConfig";

        /// <summary>
        /// 项目DBC文件下载至本地地址
        /// </summary>
        public static string DBCFileDownPath => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "DBCFile");

        /// <summary>
        /// 支持的自动试验配置字典[设备类型, 设备型号]
        /// </summary>
        public static Dictionary<FuncitonType, Type> DicTestFlowItems { get; } = new Dictionary<FuncitonType, Type>
        {
            {FuncitonType.APU, typeof(FunViewModelAPU) },
            {FuncitonType.HVDCSource, typeof(FunViewModelHVDCSource) },
            {FuncitonType.POPUP, typeof(FunViewModelPOPUP) },
            {FuncitonType.Product , typeof(FunViewModelProduct)},
            {FuncitonType.ACSource , typeof(FunViewModelACSource)},
            {FuncitonType.DCLoad , typeof(FunViewModelDCLoad)},
            {FuncitonType.EquipmentResult, typeof(FunViewModelEquipmentResult)},
            {FuncitonType.ProductResult, typeof(FunViewModelProductResult)},
        };
    }
}
