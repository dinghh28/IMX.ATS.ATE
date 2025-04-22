using IMX.ATS.ATEConfig.Function;
using IMX.Function.Base;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using IMX.Device.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using DriveType = IMX.Device.Common.DriveType;
using FreeSql.DataAnnotations;
using IMX.DB.Model;
using IMX.Common;

namespace IMX.ATS.ATEConfig
{
    /// <summary>
    /// 当前项目支持配置
    /// </summary>
    public static class SupportConfig
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

        /// <summary>
        /// 项目支持试验项名称
        /// </summary>
        public static List<string> TestTestProcess => DicProcessConfig.Keys.ToList();

        /// <summary>
        /// 支持的试验项配置字典[试验项名称, 数据库存储信息]
        /// </summary>
        public static Dictionary<string, ProcessConfig_EX> DicProcessConfig => new()
        {
            {"输入电压范围", new ProcessConfig_EX
                                {
                                    Name = "输入电压范围",
                                    UseCustomData = false,
                                    Test_ReadData = new List<ModTestDataInfo>
                                    {
                                        new() { Name="瞬时电压",},
                                        new() { Name="瞬时电流",},
                                        new() { Name="A相瞬时电压",},
                                        new() { Name="A相瞬时电流",},
                                        new() { Name="AC频率",},
                                        new() { Name="HVDC瞬时电压",},
                                        new() { Name="HVDC瞬时电流",},
                                        new() { Name="HVDC瞬时功率",},
                                        new() { Name="OBC输出瞬时电压",},
                                        new() { Name="OBC输出瞬时电流",},
                                        new() { Name="OBC输出瞬时功率",},
                                    },
                                    Test_SetData = new List<ModTestDataInfo>
                                    {
                                        new() { Name="稳压源输出电压",},
                                        new() { Name="交流源输出电压",},
                                        new() { Name="交流源输出频率",},
                                        new() { Name="高压源输出电压",},
                                        new() { Name="负载电压",},
                                        new() { Name="负载电流",},
                                        new() { Name="OBC输出电压",},
                                        new() { Name="OBC输出最大电流",},
                                    },
                                    Test_CustomData = new List<ModTestDataInfo>(),
                                }
            },
            {"输入频率范围", new ProcessConfig_EX
                                {
                                    Name = "输入频率范围",
                                    UseCustomData = false,
                                    Test_ReadData = new List<ModTestDataInfo>
                                    {
                                        new() { Name="瞬时电压",},
                                        new() { Name="瞬时电流",},
                                        new() { Name="A相瞬时电压",},
                                        new() { Name="A相瞬时电流",},
                                        new() { Name="AC频率",},
                                        new() { Name="HVDC瞬时电压",},
                                        new() { Name="HVDC瞬时电流",},
                                        new() { Name="HVDC瞬时功率",},
                                        new() { Name="OBC输出瞬时电压",},
                                        new() { Name="OBC输出瞬时电流",},
                                        new() { Name="OBC输出瞬时功率",},
                                    },
                                    Test_SetData = new List<ModTestDataInfo>
                                    {
                                        new() { Name="稳压源输出电压",},
                                        new() { Name="交流源输出电压",},
                                        new() { Name="交流源输出频率",},
                                        new() { Name="高压源输出电压",},
                                        new() { Name="负载电压",},
                                        new() { Name="负载电流",},
                                        new() { Name="OBC输出电压",},
                                        new() { Name="OBC输出最大电流",},
                                    },
                                    Test_CustomData = new List<ModTestDataInfo>(),
                                }
            },
            {"输入电流", new ProcessConfig_EX
                                {
                                    Name = "输入频率范围",
                                    UseCustomData = false,
                                    Test_ReadData = new List<ModTestDataInfo>
                                    {
                                        new() { Name="稳压源输出电压",},
                                        new() { Name="交流源输出电压",},
                                        new() { Name="交流源输出频率",},
                                        new() { Name="高压源输出电压",},
                                        new() { Name="负载电压",},
                                        new() { Name="负载电流",},
                                        new() { Name="OBC输出电压",},
                                        new() { Name="OBC输出最大电流",},
                                        new() { Name="OBC最大电流",},
                                    }
                                }
            },
        };
    }

    /// <summary>
    /// 试验项配置额外信息
    /// </summary>
    public class ProcessConfig_EX
    {
        /// <summary>
        /// 试验项名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 试验存储读取数据列表
        /// </summary>
        public List<ModTestDataInfo> Test_ReadData { get; set; }

        /// <summary>
        /// 试验存储设置数据列表
        /// </summary>
        public List<ModTestDataInfo> Test_SetData { get; set; }

        /// <summary>
        /// 是否允许使用自定义数据
        /// </summary>
        public bool UseCustomData { get; set; } = false;

        /// <summary>
        /// 试验存储用户自定义数据列表
        /// </summary>
        public List<ModTestDataInfo> Test_CustomData { get; set; }
    }
}
