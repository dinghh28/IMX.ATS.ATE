using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMX.ATS.DBCConfig
{
    internal static class SupportConfig
    {
        /// <summary>
        /// 系统空间名
        /// </summary>
        public static string SystemName => "IMX.ATS.DBCConfig";

        /// <summary>
        /// 项目DBC文件下载至本地地址
        /// </summary>
        public static string DBCFileDownPath => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "DBCFile");

        /// <summary>
        /// 固定上报信号列表
        /// </summary>
        public static List<string> LisRegularSignals =>
        [
            "产品HVDC电压",
            "产品HVDC电流",
            //"产品HVDC功率",
            "产品LVDC电压",
            "产品LVDC电流",
            //"产品LVDC功率",
            "KL30电压",
            "OBC工作模式",
            "DCDC工作模式",
            "OBC温度",
            "DCDC温度",
            "OBC故障状态",
            "DCDC故障状态",

            #region CP信号
            "产品CP幅值",
            "产品CP频率",
            "产品CP占空比",
            #endregion

            "产品CC阻值",

            #region 交流输入
            "产品输入A相电压",
            "产品输入A相电流",
            "产品输入频率",
            #endregion
        ];

        /// <summary>
        /// 逆变固定上报信号列表
        /// </summary>
        public static List<string> LisRegularSignals_In =>
        [
            //#region 输入
            //"输入A相功率因素",
            //"输入A相有功功率",
            //#endregion

            #region 输出
            "产品输出A相电压",
            "产品输出A相电流",
            "产品输出频率",
            //"输出A相功率因素",
            //"输出A相有功功率",
            #endregion
        ];

        /// <summary>
        /// 三相固定上报信号列表
        /// </summary>
        public static List<string> LisRegularSignals_Three =>
        [
            #region 输入
            //"输入A相电压",
            "产品输入B相电压",
            "产品输入C相电压",
            //"输入A相电流",
            "产品输入B相电流",
            "产品输入C相电流",
            //"输入频率",
            //"输入A相功率因素",
            //"输入B相功率因素",
            //"输入C相功率因素",
            //"输入A相有功功率",
            //"输入B相有功功率",
            //"输入C相有功功率",
            #endregion


        ];

        /// <summary>
        /// 三相逆变固定上报信号列表
        /// </summary>
        public static List<string> LisRegularSignals_ThreeIn=>
        [
                        #region 输出
            //"输出A相电压",
            "产品输出B相电压",
            "产品输出C相电压",
            //"输出A相电流",
            "产品输出B相电流",
            "产品输出C相电流",
            //"产品输出频率",
            //"输出A相功率因素",
            //"输出B相功率因素",
            //"输出C相功率因素",
            //"输出A相有功功率",
            //"输出B相有功功率",
            //"输出C相有功功率",
            #endregion
        ];

        /// <summary>
        /// 产品固定下发信号列表
        /// </summary>
        public static List<string> LisRegularSendSignals => 
        [
            #region OBC
            "OBC设置输出电流",
            "OBC设置输出电压",
            #endregion

            #region DCDC
            "DCDC设置输出电流",
            "DCDC设置输出电压",
            //"DCDC设置输入电流",
            //"DCDC设置输入电压",
            #endregion

            #region 工作模式
            "OBC设置工作模式",
            "DCDC设置工作模式",
            #endregion
        ];

        /// <summary>
        /// 产品逆变固定下发列表
        /// </summary>
        public static List<string> LisRegularSendSignals_In =>
        [
            #region BOBC
            "BOBC设置输出电流",
            "BOBC设置输出电压",
            //"BOBC设置输出频率",
            //"BOBC设置输入电流",
            //"BOBC设置输入电压",
            #endregion
        ];
    }
}
