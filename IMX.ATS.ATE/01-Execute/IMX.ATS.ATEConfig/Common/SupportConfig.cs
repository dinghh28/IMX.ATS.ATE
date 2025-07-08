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
            {FuncitonType.RPU, typeof(FunViewModelRPU) },
            {FuncitonType.HVDCSource, typeof(FunViewModelHVDCSource) },
            {FuncitonType.POPUP, typeof(FunViewModelPOPUP) },
            {FuncitonType.Product , typeof(FunViewModelProduct)},
            //{FuncitonType.ACSource , typeof(FunViewModelACSource)},
            {FuncitonType.ACSourceLoad , typeof(FunViewModelACSourceLoad)},
            {FuncitonType.DCLoad , typeof(FunViewModelDCLoad)},
            {FuncitonType.LVDCLoad , typeof(FunViewModelLVDCLoad)},
            {FuncitonType.EquipmentResult, typeof(FunViewModelEquipmentResult)},
            {FuncitonType.ProductResult, typeof(FunViewModelProductResult)},
            { FuncitonType.Shutdown, null},
            { FuncitonType.Startup, null},
            { FuncitonType.CustomMessage, typeof(FunViewModelCustomMessage)}, 
            { FuncitonType.CustomRevData, typeof(FunViewModelCustomRevData)},
        };

        /// <summary>
        /// 项目支持试验项名称
        /// </summary>
        public static List<string> TestTestProcess { get; set; } = new List<string>();

        /// <summary>
        /// 支持的试验项配置字典[试验项名称, 数据库存储信息]
        /// </summary>
        public static Dictionary<string, ProcessConfig_EX> DicProcessConfig => new()
        {
            #region OBC
            #region 输入电压范围
            {"输入电压范围", new ProcessConfig_EX
                                {
                                    Name = "输入电压范围",
                                    UseCustomData = false,
                                    Test_ReadData_Euq = new List<ModTestDataInfo>
                                    {
                                        new() { Name="辅电电压",},
                                        new() { Name="辅电电流",},
                                        new() { Name="A相电压",},
                                        new() { Name="A相电流",},
                                        new() { Name="A相频率",},
                                        new() { Name="HVDC电压",},
                                        new() { Name="HVDC电流",},
                                        new() { Name="HVDC功率",},
                                    },
                                    //Test_ReadData_Pro = new List<ModTestDataInfo>()
                                    //{
                                    //    new() { Name="OBC输出瞬时电压",},
                                    //    new() { Name="OBC输出瞬时电流",},
                                    //    new() { Name="OBC输出瞬时功率",},
                                    //},
                                    Test_SetData_Euq = new List<ModTestDataInfo>
                                    {
                                        new() { Name="辅电设置电压",},
                                        new() { Name="交流源A相设置电压",},
                                        new() { Name="交流源设置频率",},
                                        new() { Name="高压源设置电压",},
                                        new() { Name="负载拉载模式",},
                                        new() { Name="负载拉载值",},
                                        new() { Name="负载限制值",},
                                    },
                                    //Test_SetData_Pro = new List<ModTestDataInfo>()
                                    //{
                                    //    new() { Name="OBC输出电压",},
                                    //    new() { Name="OBC输出最大电流",},
                                    //},
                                    Test_CustomData = new List<ModTestDataInfo>(),
                                    Test_ReadData_EX = new List<ModTestDataInfo>
                                    {
                                        new() { Name="B相电压",},
                                        new() { Name="B相电流",},
                                        new() { Name="B相频率",},
                                        new() { Name="C相电压",},
                                        new() { Name="C相电流",},
                                        new() { Name="C相频率",},
                                    },
                                    Test_SetData_Ex = new List<ModTestDataInfo>()
                                    {
                                        new() { Name="交流源B相设置电压",},
                                        new() { Name="交流源C相设置电压",},
                                    },
                                }
            },
            #endregion
            #region 输入频率范围
            {"输入频率范围", new ProcessConfig_EX
                                {
                                    Name = "输入频率范围",
                                    UseCustomData = false,
                                    Test_ReadData_Euq = new List<ModTestDataInfo>
                                    {
                                        new() { Name="辅电电压",},
                                        new() { Name="辅电电流",},
                                        new() { Name="A相电压",},
                                        new() { Name="A相电流",},
                                        new() { Name="A相频率",},
                                        new() { Name="HVDC电压",},
                                        new() { Name="HVDC电流",},
                                        new() { Name="HVDC功率",},
                                    },
                                    Test_SetData_Euq = new List<ModTestDataInfo>
                                    {
                                        new() { Name="辅电设置电压",},
                                        new() { Name="交流源A相设置电压",},
                                        new() { Name="交流源设置频率",},
                                        new() { Name="高压源设置电压",},
                                        new() { Name="负载拉载模式",},
                                        new() { Name="负载拉载值",},
                                        new() { Name="负载限制值",},
                                    },

                                    //Test_ReadData_Pro = new List<ModTestDataInfo>
                                    //{
                                    //    new() { Name="OBC输出瞬时电压",},
                                    //    new() { Name="OBC输出瞬时电流",},
                                    //    new() { Name="OBC输出瞬时功率",},
                                    //},
                                    //Test_SetData_Pro = new List<ModTestDataInfo>
                                    //{
                                    //    new() { Name="OBC输出电压",},
                                    //    new() { Name="OBC输出最大电流",},
                                    //},
                                    Test_CustomData = new List<ModTestDataInfo>(),
                                    Test_ReadData_EX = new List<ModTestDataInfo>
                                    {
                                        new() { Name="B相电压",},
                                        new() { Name="B相电流",},
                                        new() { Name="B相频率",},
                                        new() { Name="C相电压",},
                                        new() { Name="C相电流",},
                                        new() { Name="C相频率",},
                                    },
                                    Test_SetData_Ex = new List<ModTestDataInfo>()
                                    {
                                        new() { Name="交流源B相设置电压",},
                                        new() { Name="交流源C相设置电压",},
                                    },
                                }
            },
            #endregion
            #region 输入电流
            {"输入电流", new ProcessConfig_EX
                                {
                                    Name = "输入电流",
                                    UseCustomData = false,
                                    Test_ReadData_Euq = new List<ModTestDataInfo>
                                    {
                                        new() { Name="辅电电压",},
                                        new() { Name="辅电电流",},
                                        new() { Name="A相电压",},
                                        new() { Name="A相电流",},
                                        new() { Name="A相频率",},
                                        new() { Name="HVDC电压",},
                                        new() { Name="HVDC电流",},
                                        new() { Name="HVDC功率",},
                                    },
                                    //Test_ReadData_Pro = new List<ModTestDataInfo>()
                                    //{
                                    //    new() { Name="OBC输出瞬时电压",},
                                    //    new() { Name="OBC输出瞬时电流",},
                                    //    new() { Name="OBC输出瞬时功率",},
                                    //},
                                    Test_SetData_Euq = new List<ModTestDataInfo>
                                    {
                                        new() { Name="辅电设置电压",},
                                        new() { Name="交流源A相设置电压",},
                                        new() { Name="交流源设置频率",},
                                        new() { Name="高压源设置电压",},
                                        new() { Name="负载拉载模式",},
                                        new() { Name="负载拉载值",},
                                        new() { Name="负载限制值",},
                                    },
                                    //Test_SetData_Pro = new List<ModTestDataInfo>()
                                    //{
                                    //    new() { Name="OBC输出电压",},
                                    //    new() { Name="OBC输出最大电流",},
                                    //},
                                    Test_CustomData = new List<ModTestDataInfo>(),
                                    Test_ReadData_EX = new List<ModTestDataInfo>
                                    {
                                        new() { Name="B相电压",},
                                        new() { Name="B相电流",},
                                        new() { Name="B相频率",},
                                        new() { Name="C相电压",},
                                        new() { Name="C相电流",},
                                        new() { Name="C相频率",},
                                    },
                                    Test_SetData_Ex = new List<ModTestDataInfo>()
                                    {
                                        new() { Name="交流源B相设置电压",},
                                        new() { Name="交流源C相设置电压",},
                                    },
                                }
            },
            #endregion
            #region 启动冲击电流
            {"启动冲击电流", new ProcessConfig_EX
                {
                    Name = "启动冲击电流",
                    UseCustomData = true,
                    Test_ReadData_Euq = new List<ModTestDataInfo>
                    {
                                        new() { Name="辅电电压",},
                                        new() { Name="辅电电流",},
                                        new() { Name="A相电压",},
                                        new() { Name="A相电流",},
                                        new() { Name="A相频率",},
                                        new() { Name="HVDC电压",},
                                        new() { Name="HVDC电流",},
                                        new() { Name="HVDC功率",},
                    },
                    Test_SetData_Euq = new List<ModTestDataInfo>
                    {
                        new() { Name="稳压源输出电压",},
                        new() { Name="交流源A相输出电压",},
                        new() { Name="交流源输出频率",},
                        new() { Name="交流源启动相位角",},
                        new() { Name="高压源输出电压",},
                        new() { Name="负载电压",},
                        new() { Name="负载电流",},
                    },
                    Test_CustomData = new List<ModTestDataInfo>()
                    {
                        new ModTestDataInfo{ Name = "过冲电流峰值"},
                        new ModTestDataInfo{ Name = "稳定后交流电力峰值"},
                    },
                    Test_ReadData_EX = new List<ModTestDataInfo>
                    {
                        new() { Name="B相电压",},
                        new() { Name="B相电流",},
                        new() { Name="B相频率",},
                        new() { Name="C相电压",},
                        new() { Name="C相电流",},
                        new() { Name="C相频率",},
                    },
                    Test_SetData_Ex = new List<ModTestDataInfo>()
                    {
                        new() { Name="交流源B相输出电压",},
                        new() { Name="交流源C相输出电压",},
                    },
                }
            },
            #endregion
            #region 功率因素
            {"功率因素", new ProcessConfig_EX
                                {
                                    Name = "功率因素",
                                    UseCustomData = false,
                                    Test_ReadData_Euq = new List<ModTestDataInfo>
                                    {
                                        new() { Name="辅电电压",},
                                        new() { Name="辅电电流",},
                                        new() { Name="A相电压",},
                                        new() { Name="A相电流",},
                                        new() { Name="A相频率",},
                                        new() { Name="A相有功功率" },
                                        new() { Name="A相功率因数" },
                                        new() { Name="HVDC电压",},
                                        new() { Name="HVDC电流",},
                                        new() { Name="HVDC功率",},
                                    },
                                    //Test_ReadData_Pro = new List<ModTestDataInfo>()
                                    //{
                                    //    new() { Name="OBC输出瞬时电压",},
                                    //    new() { Name="OBC输出瞬时电流",},
                                    //    new() { Name="OBC输出瞬时功率",},
                                    //},
                                    Test_SetData_Euq = new List<ModTestDataInfo>
                                    {
                                        new() { Name="稳压源输出电压",},
                                        new() { Name="交流源A相输出电压",},
                                        new() { Name="交流源输出频率",},
                                        new() { Name="高压源输出电压",},
                                        new() { Name="负载电压",},
                                        new() { Name="负载电流",},
                                    },
                                    //Test_SetData_Pro = new List<ModTestDataInfo>()
                                    //{
                                    //    new() { Name="OBC输出电压",},
                                    //    new() { Name="OBC输出最大电流",},
                                    //},
                                    Test_CustomData = new List<ModTestDataInfo>(),
                                    Test_ReadData_EX = new List<ModTestDataInfo>
                                    {
                                        new() { Name="B相电压",},
                                        new() { Name="B相电流",},
                                        new() { Name="B相频率",},
                                        new() { Name="B相有功功率" },
                                        new() { Name="B相功率因数" },
                                        new() { Name="C相电压",},
                                        new() { Name="C相电流",},
                                        new() { Name="C相频率",},
                                        new() { Name="C相有功功率" },
                                        new() { Name="C相功率因数" },
                                    },
                                    Test_SetData_Ex = new List<ModTestDataInfo>()
                                    {
                                        new() { Name="交流源B相设置电压",},
                                        new() { Name="交流源C相设置电压",},
                                    },
                                }
            },
            #endregion
            #region AC侧电压检测
            {"AC侧电压检测", new ProcessConfig_EX
                                {
                                    Name = "AC侧电压检测",
                                    UseCustomData = false,
                                    UseCalculate = true,
                                    Test_ReadData_Euq = new List<ModTestDataInfo>
                                    {
                                        new() { Name="辅电电压",},
                                        new() { Name="辅电电流",},
                                        new() { Name="A相电压",},
                                        new() { Name="A相电流",},
                                        new() { Name="A相频率",},
                                        new() { Name="HVDC电压",},
                                        new() { Name="HVDC电流",},
                                        new() { Name="HVDC功率",},
                                    },
                                    //Test_ReadData_Pro = new List<ModTestDataInfo>()
                                    //{
                                    //    new() { Name="OBC输出瞬时电压",},
                                    //    new() { Name="OBC输出瞬时电流",},
                                    //    new() { Name="OBC输出瞬时功率",},
                                    //},
                                    Test_SetData_Euq = new List<ModTestDataInfo>
                                    {
                                        new() { Name="辅电设置电压",},
                                        new() { Name="交流源A相设置电压",},
                                        new() { Name="交流源设置频率",},
                                        new() { Name="高压源设置电压",},
                                        new() { Name="负载拉载模式",},
                                        new() { Name="负载拉载值",},
                                        new() { Name="负载限制值",},
                                    },
                                    //Test_SetData_Pro = new List<ModTestDataInfo>()
                                    //{
                                    //    new() { Name="OBC输出电压",},
                                    //    new() { Name="OBC输出最大电流",},
                                    //},
                                    Test_CustomData = new List<ModTestDataInfo>(),
                                    Test_ReadData_EX = new List<ModTestDataInfo>
                                    {
                                        new() { Name="B相电压",},
                                        new() { Name="B相电流",},
                                        new() { Name="B相频率",},
                                        new() { Name="C相电压",},
                                        new() { Name="C相电流",},
                                        new() { Name="C相频率",},
                                    },
                                    Test_SetData_Ex = new List<ModTestDataInfo>()
                                    {
                                        new() { Name="交流源B相输出电压",},
                                        new() { Name="交流源C相输出电压",},
                                    },
                                    Test_CalculateData = new List<ModTestDataInfo>()
                                    {
                                        new() { Name="A相输入电压精度",},
                                    },
                                    Test_CalculateDataEX = new List<ModTestDataInfo>()
                                    {
                                        new() { Name="B相输入电压精度",},
                                        new() { Name="C相输入电压精度",},
                                    },
                                }
            },
            #endregion·
            #region AC侧电流检测
            {"AC侧电流检测", new ProcessConfig_EX
                                {
                                    Name = "AC侧电流检测",
                                    UseCustomData = false,
                                    UseCalculate = true,
                                    Test_ReadData_Euq = new List<ModTestDataInfo>
                                    {
                                        new() { Name="辅电电压",},
                                        new() { Name="辅电电流",},
                                        new() { Name="A相电压",},
                                        new() { Name="A相电流",},
                                        new() { Name="A相频率",},
                                        new() { Name="HVDC电压",},
                                        new() { Name="HVDC电流",},
                                        new() { Name="HVDC功率",},
                                    },
                                    //Test_ReadData_Pro = new List<ModTestDataInfo>()
                                    //{
                                    //    new() { Name="OBC输出瞬时电压",},
                                    //    new() { Name="OBC输出瞬时电流",},
                                    //    new() { Name="OBC输出瞬时功率",},
                                    //},
                                    Test_SetData_Euq = new List<ModTestDataInfo>
                                    {
                                        new() { Name="辅电设置电压",},
                                        new() { Name="交流源A相设置电压",},
                                        new() { Name="交流源设置频率",},
                                        new() { Name="高压源设置电压",},
                                        new() { Name="负载拉载模式",},
                                        new() { Name="负载拉载值",},
                                        new() { Name="负载限制值",},
                                    },
                                    //Test_SetData_Pro = new List<ModTestDataInfo>()
                                    //{
                                    //    new() { Name="OBC输出电压",},
                                    //    new() { Name="OBC输出最大电流",},
                                    //},
                                    Test_CustomData = new List<ModTestDataInfo>(),
                                    Test_ReadData_EX = new List<ModTestDataInfo>
                                    {
                                        new() { Name="B相电压",},
                                        new() { Name="B相电流",},
                                        new() { Name="B相频率",},
                                        new() { Name="C相电压",},
                                        new() { Name="C相电流",},
                                        new() { Name="C相频率",},
                                    },
                                    Test_SetData_Ex = new List<ModTestDataInfo>()
                                    {
                                        new() { Name="交流源B相输出电压",},
                                        new() { Name="交流源C相输出电压",},
                                    },
                                    Test_CalculateData = new List<ModTestDataInfo>()
                                    {
                                        new() { Name="A相输入电流精度",},
                                        new() { Name="A相输入电流误差",},
                                    },
                                    Test_CalculateDataEX = new List<ModTestDataInfo>()
                                    {
                                        new() { Name="B相输入电流精度",},
                                        new() { Name="B相输入电流误差",},
                                        new() { Name="C相输入电流精度",},
                                        new() { Name="C相输入电流误差",},
                                    },
                                }
            },
            #endregion
            #region HVDC电压检测
            {"HVDC电压检测", new ProcessConfig_EX
                                {
                                    Name = "HVDC电压检测",
                                    UseCustomData = false,
                                    UseCalculate = true,
                                    Test_ReadData_Euq = new List<ModTestDataInfo>
                                    {
                                        new() { Name="辅电电压",},
                                        new() { Name="辅电电流",},
                                        new() { Name="A相电压",},
                                        new() { Name="A相电流",},
                                        new() { Name="A相频率",},
                                        new() { Name="HVDC电压",},
                                        new() { Name="HVDC电流",},
                                        new() { Name="HVDC功率",},
                                    },
                                    //Test_ReadData_Pro = new List<ModTestDataInfo>()
                                    //{
                                    //    new() { Name="OBC输出瞬时电压",},
                                    //    new() { Name="OBC输出瞬时电流",},
                                    //    new() { Name="OBC输出瞬时功率",},
                                    //},
                                    Test_SetData_Euq = new List<ModTestDataInfo>
                                    {
                                        new() { Name="辅电设置电压",},
                                        new() { Name="交流源A相设置电压",},
                                        new() { Name="交流源设置频率",},
                                        new() { Name="高压源设置电压",},
                                        new() { Name="负载拉载模式",},
                                        new() { Name="负载拉载值",},
                                        new() { Name="负载限制值",},
                                    },
                                    //Test_SetData_Pro = new List<ModTestDataInfo>()
                                    //{
                                    //    new() { Name="OBC输出电压",},
                                    //    new() { Name="OBC输出最大电流",},
                                    //},
                                    Test_CustomData = new List<ModTestDataInfo>(),
                                    Test_ReadData_EX = new List<ModTestDataInfo>
                                    {
                                        new() { Name="B相电压",},
                                        new() { Name="B相电流",},
                                        new() { Name="B相频率",},
                                        new() { Name="C相电压",},
                                        new() { Name="C相电流",},
                                        new() { Name="C相频率",},
                                    },
                                    Test_SetData_Ex = new List<ModTestDataInfo>()
                                    {
                                        new() { Name="交流源B相输出电压",},
                                        new() { Name="交流源C相输出电压",},
                                    },
                                    Test_CalculateData = new List<ModTestDataInfo>()
                                    {
                                        new() { Name="产品HVDC电压精度",},
                                    },
                                    Test_CalculateDataEX = new List<ModTestDataInfo>()
                                    {
                                    },
                                }
            },
            #endregion
            #region HVDC电流检测
            {"HVDC电流检测", new ProcessConfig_EX
                                {
                                    Name = "HVDC电流检测",
                                    UseCustomData = false,
                                    UseCalculate = true,
                                    Test_ReadData_Euq = new List<ModTestDataInfo>
                                    {
                                        new() { Name="辅电电压",},
                                        new() { Name="辅电电流",},
                                        new() { Name="A相电压",},
                                        new() { Name="A相电流",},
                                        new() { Name="A相频率",},
                                        new() { Name="HVDC电压",},
                                        new() { Name="HVDC电流",},
                                        new() { Name="HVDC功率",},
                                    },
                                    //Test_ReadData_Pro = new List<ModTestDataInfo>()
                                    //{
                                    //    new() { Name="OBC输出瞬时电压",},
                                    //    new() { Name="OBC输出瞬时电流",},
                                    //    new() { Name="OBC输出瞬时功率",},
                                    //},
                                    Test_SetData_Euq = new List<ModTestDataInfo>
                                    {
                                        new() { Name="辅电设置电压",},
                                        new() { Name="交流源A相设置电压",},
                                        new() { Name="交流源设置频率",},
                                        new() { Name="高压源设置电压",},
                                        new() { Name="负载拉载模式",},
                                        new() { Name="负载拉载值",},
                                        new() { Name="负载限制值",},
                                    },
                                    //Test_SetData_Pro = new List<ModTestDataInfo>()
                                    //{
                                    //    new() { Name="OBC输出电压",},
                                    //    new() { Name="OBC输出最大电流",},
                                    //},
                                    Test_CustomData = new List<ModTestDataInfo>(),
                                    Test_ReadData_EX = new List<ModTestDataInfo>
                                    {
                                        new() { Name="B相电压",},
                                        new() { Name="B相电流",},
                                        new() { Name="B相频率",},
                                        new() { Name="C相电压",},
                                        new() { Name="C相电流",},
                                        new() { Name="C相频率",},
                                    },
                                    Test_SetData_Ex = new List<ModTestDataInfo>()
                                    {
                                        new() { Name="交流源B相输出电压",},
                                        new() { Name="交流源C相输出电压",},
                                    },
                                    Test_CalculateData = new List<ModTestDataInfo>()
                                    {
                                        new() { Name="产品HVDC电流精度",},
                                        new() { Name="产品HVDC电流误差",},
                                    },
                                    Test_CalculateDataEX = new List<ModTestDataInfo>()
                                    {
                                    },
                                }
            },
            #endregion
            #region 限压特性
            {"限压特性", new ProcessConfig_EX
                                {
                                    Name = "限压特性",
                                    UseCustomData = false,
                                    UseCalculate = true,
                                    Test_ReadData_Euq = new List<ModTestDataInfo>
                                    {
                                        new() { Name="辅电电压",},
                                        new() { Name="辅电电流",},
                                        new() { Name="A相电压",},
                                        new() { Name="A相电流",},
                                        new() { Name="A相频率",},
                                        new() { Name="HVDC电压",},
                                        new() { Name="HVDC电流",},
                                        new() { Name="HVDC功率",},
                                    },
                                    //Test_ReadData_Pro = new List<ModTestDataInfo>()
                                    //{
                                    //    new() { Name="OBC输出瞬时电压",},
                                    //    new() { Name="OBC输出瞬时电流",},
                                    //    new() { Name="OBC输出瞬时功率",},
                                    //},
                                    Test_SetData_Euq = new List<ModTestDataInfo>
                                    {
                                        new() { Name="辅电设置电压",},
                                        new() { Name="交流源A相设置电压",},
                                        new() { Name="交流源设置频率",},
                                        new() { Name="高压源设置电压",},
                                        new() { Name="负载拉载模式",},
                                        new() { Name="负载拉载值",},
                                        new() { Name="负载限制值",},
                                    },
                                    //Test_SetData_Pro = new List<ModTestDataInfo>()
                                    //{
                                    //    new() { Name="OBC输出电压",},
                                    //    new() { Name="OBC输出最大电流",},
                                    //},
                                    Test_CustomData = new List<ModTestDataInfo>(),
                                    Test_ReadData_EX = new List<ModTestDataInfo>
                                    {
                                        new() { Name="B相电压",},
                                        new() { Name="B相电流",},
                                        new() { Name="B相频率",},
                                        new() { Name="C相电压",},
                                        new() { Name="C相电流",},
                                        new() { Name="C相频率",},
                                    },
                                    Test_SetData_Ex = new List<ModTestDataInfo>()
                                    {
                                        new() { Name="交流源B相输出电压",},
                                        new() { Name="交流源C相输出电压",},
                                    },
                                    //Test_CalculateData = new List<ModTestDataInfo>()
                                    //{
                                    //    new() { Name="产品HVDC电流精度",},
                                    //    new() { Name="产品HVDC电流误差",},
                                    //},
                                    //Test_CalculateDataEX = new List<ModTestDataInfo>()
                                    //{
                                    //},
                                }
            },
            #endregion
            #region 输出电压范围
            {"输出电压范围", new ProcessConfig_EX
                                {
                                    Name = "输出电压范围",
                                    UseCustomData = false,
                                    UseCalculate = true,
                                    Test_ReadData_Euq = new List<ModTestDataInfo>
                                    {
                                        new() { Name="辅电电压",},
                                        new() { Name="辅电电流",},
                                        new() { Name="A相电压",},
                                        new() { Name="A相电流",},
                                        new() { Name="A相频率",},
                                        new() { Name="HVDC电压",},
                                        new() { Name="HVDC电流",},
                                        new() { Name="HVDC功率",},
                                    },
                                    //Test_ReadData_Pro = new List<ModTestDataInfo>()
                                    //{
                                    //    new() { Name="OBC输出瞬时电压",},
                                    //    new() { Name="OBC输出瞬时电流",},
                                    //    new() { Name="OBC输出瞬时功率",},
                                    //},
                                    Test_SetData_Euq = new List<ModTestDataInfo>
                                    {
                                        new() { Name="辅电设置电压",},
                                        new() { Name="交流源A相设置电压",},
                                        new() { Name="交流源设置频率",},
                                        new() { Name="高压源设置电压",},
                                        new() { Name="负载拉载模式",},
                                        new() { Name="负载拉载值",},
                                        new() { Name="负载限制值",},
                                    },
                                    //Test_SetData_Pro = new List<ModTestDataInfo>()
                                    //{
                                    //    new() { Name="OBC输出电压",},
                                    //    new() { Name="OBC输出最大电流",},
                                    //},
                                    Test_CustomData = new List<ModTestDataInfo>(),
                                    Test_ReadData_EX = new List<ModTestDataInfo>
                                    {
                                        new() { Name="B相电压",},
                                        new() { Name="B相电流",},
                                        new() { Name="B相频率",},
                                        new() { Name="C相电压",},
                                        new() { Name="C相电流",},
                                        new() { Name="C相频率",},
                                    },
                                    Test_SetData_Ex = new List<ModTestDataInfo>()
                                    {
                                        new() { Name="交流源B相输出电压",},
                                        new() { Name="交流源C相输出电压",},
                                    },
                                    //Test_CalculateData = new List<ModTestDataInfo>()
                                    //{
                                    //    new() { Name="产品HVDC电流精度",},
                                    //    new() { Name="产品HVDC电流误差",},
                                    //},
                                    //Test_CalculateDataEX = new List<ModTestDataInfo>()
                                    //{
                                    //},
                                }
            },
            #endregion
            #region 输出电流范围
            {"输出电流范围", new ProcessConfig_EX
                                {
                                    Name = "输出电流范围",
                                    UseCustomData = false,
                                    UseCalculate = true,
                                    Test_ReadData_Euq = new List<ModTestDataInfo>
                                    {
                                        new() { Name="辅电电压",},
                                        new() { Name="辅电电流",},
                                        new() { Name="A相电压",},
                                        new() { Name="A相电流",},
                                        new() { Name="A相频率",},
                                        new() { Name="HVDC电压",},
                                        new() { Name="HVDC电流",},
                                        new() { Name="HVDC功率",},
                                    },
                                    //Test_ReadData_Pro = new List<ModTestDataInfo>()
                                    //{
                                    //    new() { Name="OBC输出瞬时电压",},
                                    //    new() { Name="OBC输出瞬时电流",},
                                    //    new() { Name="OBC输出瞬时功率",},
                                    //},
                                    Test_SetData_Euq = new List<ModTestDataInfo>
                                    {
                                        new() { Name="辅电设置电压",},
                                        new() { Name="交流源A相设置电压",},
                                        new() { Name="交流源设置频率",},
                                        new() { Name="高压源设置电压",},
                                        new() { Name="负载拉载模式",},
                                        new() { Name="负载拉载值",},
                                        new() { Name="负载限制值",},
                                    },
                                    //Test_SetData_Pro = new List<ModTestDataInfo>()
                                    //{
                                    //    new() { Name="OBC输出电压",},
                                    //    new() { Name="OBC输出最大电流",},
                                    //},
                                    Test_CustomData = new List<ModTestDataInfo>(),
                                    Test_ReadData_EX = new List<ModTestDataInfo>
                                    {
                                        new() { Name="B相电压",},
                                        new() { Name="B相电流",},
                                        new() { Name="B相频率",},
                                        new() { Name="C相电压",},
                                        new() { Name="C相电流",},
                                        new() { Name="C相频率",},
                                    },
                                    Test_SetData_Ex = new List<ModTestDataInfo>()
                                    {
                                        new() { Name="交流源B相输出电压",},
                                        new() { Name="交流源C相输出电压",},
                                    },
                                    Test_CalculateData = new List<ModTestDataInfo>()
                                    {
                                        new() { Name="HVDC电流控制误差",},
                                        new() { Name="HVDC电流控制精度",},
                                    },
                                    Test_CalculateDataEX = new List<ModTestDataInfo>()
                                    {
                                    },
                                }
            },
            #endregion
            #region 充电效率
            {"充电效率", new ProcessConfig_EX
                                {
                                    Name = "充电效率",
                                    UseCustomData = false,
                                    UseCalculate = true,
                                    Test_ReadData_Euq = new List<ModTestDataInfo>
                                    {
                                        new() { Name="辅电电压",},
                                        new() { Name="辅电电流",},
                                        new() { Name="A相电压",},
                                        new() { Name="A相电流",},
                                        new() { Name="A相功率",},
                                        new() { Name="A相频率",},
                                        new() { Name="交流测总功率",},
                                        new() { Name="HVDC电压",},
                                        new() { Name="HVDC电流",},
                                        new() { Name="HVDC功率",},
                                    },
                                    //Test_ReadData_Pro = new List<ModTestDataInfo>()
                                    //{
                                    //    new() { Name="OBC输出瞬时电压",},
                                    //    new() { Name="OBC输出瞬时电流",},
                                    //    new() { Name="OBC输出瞬时功率",},
                                    //},
                                    Test_SetData_Euq = new List<ModTestDataInfo>
                                    {
                                        new() { Name="辅电设置电压",},
                                        new() { Name="交流源A相设置电压",},
                                        new() { Name="交流源设置频率",},
                                        new() { Name="高压源设置电压",},
                                        new() { Name="负载拉载模式",},
                                        new() { Name="负载拉载值",},
                                        new() { Name="负载限制值",},
                                    },
                                    //Test_SetData_Pro = new List<ModTestDataInfo>()
                                    //{
                                    //    new() { Name="OBC输出电压",},
                                    //    new() { Name="OBC输出最大电流",},
                                    //},
                                    Test_CustomData = new List<ModTestDataInfo>(),
                                    Test_ReadData_EX = new List<ModTestDataInfo>
                                    {
                                        new() { Name="B相电压",},
                                        new() { Name="B相电流",},
                                        new() { Name="B相功率",},
                                        new() { Name="B相频率",},
                                        new() { Name="C相电压",},
                                        new() { Name="C相电流",},
                                        new() { Name="C相功率",},
                                        new() { Name="C相频率",},
                                    },
                                    Test_SetData_Ex = new List<ModTestDataInfo>()
                                    {
                                        new() { Name="交流源B相输出电压",},
                                        new() { Name="交流源C相输出电压",},
                                    },
                                    Test_CalculateData = new List<ModTestDataInfo>()
                                    {
                                        new() { Name="OBC充电效率",},
                                    },
                                    Test_CalculateDataEX = new List<ModTestDataInfo>()
                                    {
                                    },
                                }
            },
            #endregion
            #region 三相
            #region 三相交流相位不平衡
            {"三相交流相位不平衡", new ProcessConfig_EX
                                {
                                    Name = "三相交流相位不平衡",
                                    UseCustomData = false,
                                    UseCalculate = false,
                                    IsThree = true,
                                    Test_ReadData_Euq = new List<ModTestDataInfo>
                                    {
                                        new() { Name="辅电电压",},
                                        new() { Name="辅电电流",},
                                        new() { Name="A相电压",},
                                        new() { Name="A相电流",},
                                        new() { Name="A相频率",},
                                        new() { Name="A相电压相间角度",},
                                        new() { Name="HVDC电压",},
                                        new() { Name="HVDC电流",},
                                        new() { Name="HVDC功率",},
                                    },
                                    //Test_ReadData_Pro = new List<ModTestDataInfo>()
                                    //{
                                    //    new() { Name="OBC输出瞬时电压",},
                                    //    new() { Name="OBC输出瞬时电流",},
                                    //    new() { Name="OBC输出瞬时功率",},
                                    //},
                                    Test_SetData_Euq = new List<ModTestDataInfo>
                                    {
                                        new() { Name="辅电设置电压",},
                                        new() { Name="交流源A相设置电压",},
                                        new() { Name="交流源设置频率",},
                                        new() { Name="高压源设置电压",},
                                        new() { Name="负载拉载模式",},
                                        new() { Name="负载拉载值",},
                                        new() { Name="负载限制值",},
                                    },
                                    //Test_SetData_Pro = new List<ModTestDataInfo>()
                                    //{
                                    //    new() { Name="OBC输出电压",},
                                    //    new() { Name="OBC输出最大电流",},
                                    //},
                                    Test_CustomData = new List<ModTestDataInfo>(),
                                    Test_ReadData_EX = new List<ModTestDataInfo>
                                    {
                                        new() { Name="B相电压",},
                                        new() { Name="B相电流",},
                                        new() { Name="B相频率",},
                                        new() { Name="B相电压相间角度",},
                                        new() { Name="C相电压",},
                                        new() { Name="C相电流",},
                                        new() { Name="C相频率",},
                                        new() { Name="C相电压相间角度",},
                                    },
                                    Test_SetData_Ex = new List<ModTestDataInfo>()
                                    {
                                        new() { Name="交流源B相输出电压",},
                                        new() { Name="交流源C相输出电压",},
                                        new() { Name ="交流源AB设置相角" },
                                        new() { Name ="交流源AC设置相角" }
                                    },
                                    //Test_CalculateData = new List<ModTestDataInfo>()
                                    //{
                                    //    new() { Name="产品HVDC电流精度",},
                                    //    new() { Name="产品HVDC电流误差",},
                                    //},
                                    //Test_CalculateDataEX = new List<ModTestDataInfo>()
                                    //{
                                    //},
                                }
            },
            #endregion
            #region 三相交流不平衡
            {"三相交流不平衡", new ProcessConfig_EX
                                {
                                    Name = "三相交流不平衡",
                                    UseCustomData = false,
                                    IsThree = true,
                                    Test_ReadData_Euq = new List<ModTestDataInfo>
                                    {
                                        new() { Name="辅电电压",},
                                        new() { Name="辅电电流",},
                                        new() { Name="A相电压",},
                                        new() { Name="A相电流",},
                                        new() { Name="A相频率",},
                                        new() { Name="HVDC电压",},
                                        new() { Name="HVDC电流",},
                                        new() { Name="HVDC功率",},
                                    },
                                    //Test_ReadData_Pro = new List<ModTestDataInfo>()
                                    //{
                                    //    new() { Name="OBC输出瞬时电压",},
                                    //    new() { Name="OBC输出瞬时电流",},
                                    //    new() { Name="OBC输出瞬时功率",},
                                    //},
                                    Test_SetData_Euq = new List<ModTestDataInfo>
                                    {
                                        new() { Name="辅电设置电压",},
                                        new() { Name="交流源A相设置电压",},
                                        new() { Name="交流源设置频率",},
                                        new() { Name="高压源设置电压",},
                                        new() { Name="负载拉载模式",},
                                        new() { Name="负载拉载值",},
                                        new() { Name="负载限制值",},
                                    },
                                    //Test_SetData_Pro = new List<ModTestDataInfo>()
                                    //{
                                    //    new() { Name="OBC输出电压",},
                                    //    new() { Name="OBC输出最大电流",},
                                    //},
                                    Test_CustomData = new List<ModTestDataInfo>(),
                                    Test_ReadData_EX = new List<ModTestDataInfo>
                                    {
                                        new() { Name="B相电压",},
                                        new() { Name="B相电流",},
                                        new() { Name="B相频率",},
                                        new() { Name="C相电压",},
                                        new() { Name="C相电流",},
                                        new() { Name="C相频率",},
                                    },
                                    Test_SetData_Ex = new List<ModTestDataInfo>()
                                    {
                                        new() { Name="交流源B相设置电压",},
                                        new() { Name="交流源C相设置电压",},
                                    },
                                }
            },
            #endregion
            #endregion
            #endregion

            #region 逆变
            #region 输入电压范围（逆变）
            {"输入电压范围（逆变）", new ProcessConfig_EX
                                {
                                    Name = "输入电压范围（逆变）",
                                    UseCustomData = false,
                                    IsInversion = true,
                                    Test_ReadData_Euq = new List<ModTestDataInfo>
                                    {
                                        new() { Name="辅电电压",},
                                        new() { Name="辅电电流",},
                                        new() { Name="A相电压",},
                                        new() { Name="A相电流",},
                                        new() { Name="A相频率",},
                                        new() { Name="A相有功功率"},
                                        new() { Name="A相功率因数"},
                                        new() { Name="HVDC电压",},
                                        new() { Name="HVDC电流",},
                                        new() { Name="HVDC功率",},
                                    },
                                    //Test_ReadData_Pro = new List<ModTestDataInfo>()
                                    //{
                                    //    new() { Name="OBC输出瞬时电压",},
                                    //    new() { Name="OBC输出瞬时电流",},
                                    //    new() { Name="OBC输出瞬时功率",},
                                    //},
                                    Test_SetData_Euq = new List<ModTestDataInfo>
                                    {
                                        new() { Name="辅电设置电压",},
                                        new() { Name="交流负载设置拉载模式",},
                                        new() { Name="交流负载A相设置拉载值",},
                                        new() { Name="高压源设置电压",},
                                    },
                                    //Test_SetData_Pro = new List<ModTestDataInfo>()
                                    //{
                                    //    new() { Name="OBC输出电压",},
                                    //    new() { Name="OBC输出最大电流",},
                                    //},
                                    Test_CustomData = new List<ModTestDataInfo>(),
                                    Test_ReadData_EX = new List<ModTestDataInfo>
                                    {
                                        new() { Name="B相电压",},
                                        new() { Name="B相电流",},
                                        new() { Name="B相频率",},
                                        new() { Name="B相有功功率"},
                                        new() { Name="B相功率因数"},
                                        new() { Name="C相电压",},
                                        new() { Name="C相电流",},
                                        new() { Name="C相频率",},                                        
                                        new() { Name="C相有功功率"},
                                        new() { Name="C相功率因数"},
                                    },
                                    Test_SetData_Ex = new List<ModTestDataInfo>()
                                    {
                                        new() { Name="交流负载B相设置拉载值",},
                                        new() { Name="交流负载C相设置拉载值",},
                                    },
                                }
            },
            #endregion
            #endregion

            #region DCDC
            #region 输入电压范围（DCDC）
            {"输入电压范围（DCDC）", new ProcessConfig_EX
                                {
                                    Name = "输入电压范围（DCDC）",
                                    UseCustomData = false,
                                    Test_ReadData_Euq = new List<ModTestDataInfo>
                                    {
                                        new() { Name="辅电电压",},
                                        new() { Name="辅电电流",},
                                        new() { Name="A相电压",},
                                        new() { Name="A相电流",},
                                        new() { Name="A相频率",},
                                        new() { Name="HVDC电压",},
                                        new() { Name="HVDC电流",},
                                        new() { Name="HVDC功率",},
                                        new() { Name="LVDC电压",},
                                        new() { Name="LVDC电流",},
                                        new() { Name="LVDC功率",},
                                    },
                                    //Test_ReadData_Pro = new List<ModTestDataInfo>()
                                    //{
                                    //    new() { Name="OBC输出瞬时电压",},
                                    //    new() { Name="OBC输出瞬时电流",},
                                    //    new() { Name="OBC输出瞬时功率",},
                                    //},
                                    Test_SetData_Euq = new List<ModTestDataInfo>
                                    {
                                        new() { Name="辅电设置电压",},
                                        new() { Name="交流源A相设置电压",},
                                        new() { Name="交流源设置频率",},
                                        new() { Name="高压源设置电压",},
                                        new() { Name="低压负载设置模式",},
                                        new() { Name="低压负载设置拉载值",},
                                        new() { Name="低压负载设置限制值",},
                                    },
                                    //Test_SetData_Pro = new List<ModTestDataInfo>()
                                    //{
                                    //    new() { Name="OBC输出电压",},
                                    //    new() { Name="OBC输出最大电流",},
                                    //},
                                    Test_CustomData = new List<ModTestDataInfo>(),
                                    Test_ReadData_EX = new List<ModTestDataInfo>
                                    {
                                        new() { Name="B相电压",},
                                        new() { Name="B相电流",},
                                        new() { Name="B相频率",},
                                        new() { Name="C相电压",},
                                        new() { Name="C相电流",},
                                        new() { Name="C相频率",},
                                    },
                                    Test_SetData_Ex = new List<ModTestDataInfo>()
                                    {
                                        new() { Name="交流源B相设置电压",},
                                        new() { Name="交流源C相设置电压",},
                                    },
                                }
            },
            #endregion
            #region 输出电压范围（DCDC）
            {"输出电压范围（DCDC）", new ProcessConfig_EX
                                {
                                    Name = "输出电压范围（DCDC）",
                                    UseCustomData = false,
                                    Test_ReadData_Euq = new List<ModTestDataInfo>
                                    {
                                        new() { Name="辅电电压",},
                                        new() { Name="辅电电流",},
                                        new() { Name="A相电压",},
                                        new() { Name="A相电流",},
                                        new() { Name="A相频率",},
                                        new() { Name="HVDC电压",},
                                        new() { Name="HVDC电流",},
                                        new() { Name="HVDC功率",},
                                        new() { Name="LVDC电压",},
                                        new() { Name="LVDC电流",},
                                        new() { Name="LVDC功率",},
                                    },
                                    //Test_ReadData_Pro = new List<ModTestDataInfo>()
                                    //{
                                    //    new() { Name="OBC输出瞬时电压",},
                                    //    new() { Name="OBC输出瞬时电流",},
                                    //    new() { Name="OBC输出瞬时功率",},
                                    //},
                                    Test_SetData_Euq = new List<ModTestDataInfo>
                                    {
                                        new() { Name="辅电设置电压",},
                                        new() { Name="交流源A相设置电压",},
                                        new() { Name="交流源设置频率",},
                                        new() { Name="高压源设置电压",},
                                        new() { Name="低压负载设置模式",},
                                        new() { Name="低压负载设置拉载值",},
                                        new() { Name="低压负载设置限制值",},
                                    },
                                    //Test_SetData_Pro = new List<ModTestDataInfo>()
                                    //{
                                    //    new() { Name="OBC输出电压",},
                                    //    new() { Name="OBC输出最大电流",},
                                    //},
                                    Test_CustomData = new List<ModTestDataInfo>(),
                                    Test_ReadData_EX = new List<ModTestDataInfo>
                                    {
                                        new() { Name="B相电压",},
                                        new() { Name="B相电流",},
                                        new() { Name="B相频率",},
                                        new() { Name="C相电压",},
                                        new() { Name="C相电流",},
                                        new() { Name="C相频率",},
                                    },
                                    Test_SetData_Ex = new List<ModTestDataInfo>()
                                    {
                                        new() { Name="交流源B相设置电压",},
                                        new() { Name="交流源C相设置电压",},
                                    },
                                }
            },
            #endregion
            #region 额定输出电流（DCDC）
            {"额定输出电流（DCDC）", new ProcessConfig_EX
                                {
                                    Name = "额定输出电流（DCDC）",
                                    UseCustomData = false,
                                    Test_ReadData_Euq = new List<ModTestDataInfo>
                                    {
                                        new() { Name="辅电电压",},
                                        new() { Name="辅电电流",},
                                        new() { Name="A相电压",},
                                        new() { Name="A相电流",},
                                        new() { Name="A相频率",},
                                        new() { Name="HVDC电压",},
                                        new() { Name="HVDC电流",},
                                        new() { Name="HVDC功率",},
                                        new() { Name="LVDC电压",},
                                        new() { Name="LVDC电流",},
                                        new() { Name="LVDC功率",},
                                    },
                                    //Test_ReadData_Pro = new List<ModTestDataInfo>()
                                    //{
                                    //    new() { Name="OBC输出瞬时电压",},
                                    //    new() { Name="OBC输出瞬时电流",},
                                    //    new() { Name="OBC输出瞬时功率",},
                                    //},
                                    Test_SetData_Euq = new List<ModTestDataInfo>
                                    {
                                        new() { Name="辅电设置电压",},
                                        new() { Name="交流源A相设置电压",},
                                        new() { Name="交流源设置频率",},
                                        new() { Name="高压源设置电压",},
                                        new() { Name="低压负载设置模式",},
                                        new() { Name="低压负载设置拉载值",},
                                        new() { Name="低压负载设置限制值",},
                                    },
                                    //Test_SetData_Pro = new List<ModTestDataInfo>()
                                    //{
                                    //    new() { Name="OBC输出电压",},
                                    //    new() { Name="OBC输出最大电流",},
                                    //},
                                    Test_CustomData = new List<ModTestDataInfo>(),
                                    Test_ReadData_EX = new List<ModTestDataInfo>
                                    {
                                        new() { Name="B相电压",},
                                        new() { Name="B相电流",},
                                        new() { Name="B相频率",},
                                        new() { Name="C相电压",},
                                        new() { Name="C相电流",},
                                        new() { Name="C相频率",},
                                    },
                                    Test_SetData_Ex = new List<ModTestDataInfo>()
                                    {
                                        new() { Name="交流源B相设置电压",},
                                        new() { Name="交流源C相设置电压",},
                                    },
                                }
            },
            #endregion
            #region 输出效率（DCDC）
            {"输出效率（DCDC））", new ProcessConfig_EX
                                {
                                    Name = "输出效率（DCDC）",
                                    UseCalculate = true,
                                    Test_ReadData_Euq = new List<ModTestDataInfo>
                                    {
                                        new() { Name="辅电电压",},
                                        new() { Name="辅电电流",},
                                        new() { Name="A相电压",},
                                        new() { Name="A相电流",},
                                        new() { Name="A相频率",},
                                        new() { Name="HVDC电压",},
                                        new() { Name="HVDC电流",},
                                        new() { Name="HVDC功率",},
                                        new() { Name="LVDC电压",},
                                        new() { Name="LVDC电流",},
                                        new() { Name="LVDC功率",},
                                    },
                                    Test_SetData_Euq = new List<ModTestDataInfo>
                                    {
                                        new() { Name="辅电设置电压",},
                                        new() { Name="交流源A相设置电压",},
                                        new() { Name="交流源设置频率",},
                                        new() { Name="高压源设置电压",},
                                        new() { Name="低压负载设置模式",},
                                        new() { Name="低压负载设置拉载值",},
                                        new() { Name="低压负载设置限制值",},
                                    },
                                    Test_CustomData = new List<ModTestDataInfo>(),
                                    Test_ReadData_EX = new List<ModTestDataInfo>
                                    {
                                        new() { Name="B相电压",},
                                        new() { Name="B相电流",},
                                        new() { Name="B相频率",},
                                        new() { Name="C相电压",},
                                        new() { Name="C相电流",},
                                        new() { Name="C相频率",},
                                    },
                                    Test_SetData_Ex = new List<ModTestDataInfo>()
                                    {
                                        new() { Name="交流源B相设置电压",},
                                        new() { Name="交流源C相设置电压",},
                                    },
                                    Test_CalculateData = new List<ModTestDataInfo>()
                                    {
                                        new() { Name="输出效率(DCDC)",},
                                    },
                                }
            },
            #endregion
            #region 输入电压精度（DCDC）
            {"输入电压精度（DCDC）", new ProcessConfig_EX
                                {
                                    Name = "输入电压精度（DCDC）",
                                    UseCalculate = true,
                                    Test_ReadData_Euq = new List<ModTestDataInfo>
                                    {
                                        new() { Name="辅电电压",},
                                        new() { Name="辅电电流",},
                                        new() { Name="A相电压",},
                                        new() { Name="A相电流",},
                                        new() { Name="A相频率",},
                                        new() { Name="HVDC电压",},
                                        new() { Name="HVDC电流",},
                                        new() { Name="HVDC功率",},
                                        new() { Name="LVDC电压",},
                                        new() { Name="LVDC电流",},
                                        new() { Name="LVDC功率",},
                                    },
                                    Test_SetData_Euq = new List<ModTestDataInfo>
                                    {
                                        new() { Name="辅电设置电压",},
                                        new() { Name="交流源A相设置电压",},
                                        new() { Name="交流源设置频率",},
                                        new() { Name="高压源设置电压",},
                                        new() { Name="低压负载设置模式",},
                                        new() { Name="低压负载设置拉载值",},
                                        new() { Name="低压负载设置限制值",},
                                    },
                                    Test_CustomData = new List<ModTestDataInfo>(),
                                    Test_ReadData_EX = new List<ModTestDataInfo>
                                    {
                                        new() { Name="B相电压",},
                                        new() { Name="B相电流",},
                                        new() { Name="B相频率",},
                                        new() { Name="C相电压",},
                                        new() { Name="C相电流",},
                                        new() { Name="C相频率",},
                                    },
                                    Test_SetData_Ex = new List<ModTestDataInfo>()
                                    {
                                        new() { Name="交流源B相设置电压",},
                                        new() { Name="交流源C相设置电压",},
                                    },
                                    Test_CalculateData = new List<ModTestDataInfo>()
                                    {
                                        new() { Name="输入电压精度(DCDC)",},
                                    },
                                }
            },
            #endregion
            #region 输出电压精度（DCDC）
            {"输出电压精度（DCDC）", new ProcessConfig_EX
                                {
                                    Name = "输出电压精度（DCDC）",
                                    UseCalculate = true,
                                    Test_ReadData_Euq = new List<ModTestDataInfo>
                                    {
                                        new() { Name="辅电电压",},
                                        new() { Name="辅电电流",},
                                        new() { Name="A相电压",},
                                        new() { Name="A相电流",},
                                        new() { Name="A相频率",},
                                        new() { Name="HVDC电压",},
                                        new() { Name="HVDC电流",},
                                        new() { Name="HVDC功率",},
                                        new() { Name="LVDC电压",},
                                        new() { Name="LVDC电流",},
                                        new() { Name="LVDC功率",},
                                    },
                                    Test_SetData_Euq = new List<ModTestDataInfo>
                                    {
                                        new() { Name="辅电设置电压",},
                                        new() { Name="交流源A相设置电压",},
                                        new() { Name="交流源设置频率",},
                                        new() { Name="高压源设置电压",},
                                        new() { Name="低压负载设置模式",},
                                        new() { Name="低压负载设置拉载值",},
                                        new() { Name="低压负载设置限制值",},
                                    },
                                    Test_CustomData = new List<ModTestDataInfo>(),
                                    Test_ReadData_EX = new List<ModTestDataInfo>
                                    {
                                        new() { Name="B相电压",},
                                        new() { Name="B相电流",},
                                        new() { Name="B相频率",},
                                        new() { Name="C相电压",},
                                        new() { Name="C相电流",},
                                        new() { Name="C相频率",},
                                    },
                                    Test_SetData_Ex = new List<ModTestDataInfo>()
                                    {
                                        new() { Name="交流源B相设置电压",},
                                        new() { Name="交流源C相设置电压",},
                                    },
                                    Test_CalculateData = new List<ModTestDataInfo>()
                                    {
                                        new() { Name="输出电压精度(DCDC)",},
                                    },
                                }
            },
            #endregion
            #region 输入电流精度（DCDC）
            {"输入电流精度（DCDC）", new ProcessConfig_EX
                                {
                                    Name = "输入电流精度（DCDC）",
                                    UseCalculate = true,
                                    Test_ReadData_Euq = new List<ModTestDataInfo>
                                    {
                                        new() { Name="辅电电压",},
                                        new() { Name="辅电电流",},
                                        new() { Name="A相电压",},
                                        new() { Name="A相电流",},
                                        new() { Name="A相频率",},
                                        new() { Name="HVDC电压",},
                                        new() { Name="HVDC电流",},
                                        new() { Name="HVDC功率",},
                                        new() { Name="LVDC电压",},
                                        new() { Name="LVDC电流",},
                                        new() { Name="LVDC功率",},
                                    },
                                    Test_SetData_Euq = new List<ModTestDataInfo>
                                    {
                                        new() { Name="辅电设置电压",},
                                        new() { Name="交流源A相设置电压",},
                                        new() { Name="交流源设置频率",},
                                        new() { Name="高压源设置电压",},
                                        new() { Name="低压负载设置模式",},
                                        new() { Name="低压负载设置拉载值",},
                                        new() { Name="低压负载设置限制值",},
                                    },
                                    Test_CustomData = new List<ModTestDataInfo>(),
                                    Test_ReadData_EX = new List<ModTestDataInfo>
                                    {
                                        new() { Name="B相电压",},
                                        new() { Name="B相电流",},
                                        new() { Name="B相频率",},
                                        new() { Name="C相电压",},
                                        new() { Name="C相电流",},
                                        new() { Name="C相频率",},
                                    },
                                    Test_SetData_Ex = new List<ModTestDataInfo>()
                                    {
                                        new() { Name="交流源B相设置电压",},
                                        new() { Name="交流源C相设置电压",},
                                    },
                                    Test_CalculateData = new List<ModTestDataInfo>()
                                    {
                                        new() { Name="输入电流精度(DCDC)",},
                                        new() { Name="输入电流误差(DCDC)",},
                                    },
                                }
            },
            #endregion
            #region 输出电流精度（DCDC）
            {"输出电流精度（DCDC）", new ProcessConfig_EX
                                {
                                    Name = "输出电流精度（DCDC）",
                                    UseCalculate = true,
                                    Test_ReadData_Euq = new List<ModTestDataInfo>
                                    {
                                        new() { Name="辅电电压",},
                                        new() { Name="辅电电流",},
                                        new() { Name="A相电压",},
                                        new() { Name="A相电流",},
                                        new() { Name="A相频率",},
                                        new() { Name="HVDC电压",},
                                        new() { Name="HVDC电流",},
                                        new() { Name="HVDC功率",},
                                        new() { Name="LVDC电压",},
                                        new() { Name="LVDC电流",},
                                        new() { Name="LVDC功率",},
                                    },
                                    Test_SetData_Euq = new List<ModTestDataInfo>
                                    {
                                        new() { Name="辅电设置电压",},
                                        new() { Name="交流源A相设置电压",},
                                        new() { Name="交流源设置频率",},
                                        new() { Name="高压源设置电压",},
                                        new() { Name="低压负载设置模式",},
                                        new() { Name="低压负载设置拉载值",},
                                        new() { Name="低压负载设置限制值",},
                                    },
                                    Test_CustomData = new List<ModTestDataInfo>(),
                                    Test_ReadData_EX = new List<ModTestDataInfo>
                                    {
                                        new() { Name="B相电压",},
                                        new() { Name="B相电流",},
                                        new() { Name="B相频率",},
                                        new() { Name="C相电压",},
                                        new() { Name="C相电流",},
                                        new() { Name="C相频率",},
                                    },
                                    Test_SetData_Ex = new List<ModTestDataInfo>()
                                    {
                                        new() { Name="交流源B相设置电压",},
                                        new() { Name="交流源C相设置电压",},
                                    },
                                    Test_CalculateData = new List<ModTestDataInfo>()
                                    {
                                        new() { Name="输出电流精度(DCDC)",},
                                        new() { Name="输出电流误差(DCDC)",},
                                    },
                                }
            },
            #endregion
            #endregion
           
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
        /// 是否为逆变模式
        /// </summary>
        public bool IsInversion {  get; set; } = false;

        /// <summary>
        /// 是否为三相模式特有
        /// </summary>
        public bool IsThree { get; set; } = false;

        /// <summary>
        /// 试验存储读取数据列表
        /// </summary>
        public List<ModTestDataInfo> Test_ReadData_Pro { get; set; }

        /// <summary>
        /// 试验存储设置数据列表
        /// </summary>
        public List<ModTestDataInfo> Test_SetData_Pro { get; set; }

        /// <summary>
        /// 试验存储读取数据列表
        /// </summary>
        public List<ModTestDataInfo> Test_ReadData_Euq { get; set; }

        /// <summary>
        /// 试验存储设置数据列表
        /// </summary>
        public List<ModTestDataInfo> Test_SetData_Euq { get; set; }

        /// <summary>
        /// 三相额外数据
        /// </summary>
        public List<ModTestDataInfo> Test_ReadData_EX { get; set; }

        /// <summary>
        /// 三相额外设置参数
        /// </summary>
        public List<ModTestDataInfo> Test_SetData_Ex { get; set; }

        /// <summary>
        /// 是否允许使用自定义数据
        /// </summary>
        public bool UseCustomData { get; set; } = false;

        /// <summary>
        /// 试验存储用户自定义数据列表
        /// </summary>
        public List<ModTestDataInfo> Test_CustomData { get; set; } = new List<ModTestDataInfo>();

        /// <summary>
        /// 是否使用计算值
        /// </summary>
        public bool UseCalculate { get; set; } = false;

        /// <summary>
        /// 试验存储计算值数据列表
        /// </summary>
        public List<ModTestDataInfo> Test_CalculateData { get; set; } = new List<ModTestDataInfo>();

        /// <summary>
        /// 试验存储计算值额外数据列表
        /// </summary>
        public List<ModTestDataInfo> Test_CalculateDataEX { get; set; } = new List<ModTestDataInfo>();
    }
}
