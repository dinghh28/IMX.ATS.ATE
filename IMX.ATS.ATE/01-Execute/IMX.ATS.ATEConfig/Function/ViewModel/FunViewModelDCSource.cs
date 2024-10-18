#region << 版 本 注 释 >>
/*----------------------------------------------------------------
 * 版权所有 (c) 2024   保留所有权利。
 * CLR版本：4.0.30319.42000
 * 机器名称：LAPTOP-9Q9TTD5V
 * 公司名称：
 * 命名空间：IMX.ATS.ATEConfig.Function.ViewModel
 * 唯一标识：7cde0006-1a25-49a9-a3c0-64bcaf66e035
 * 文件名：FunViewModelDCSource
 * 当前用户域：LAPTOP-9Q9TTD5V
 * 
 * 创建者：58274
 * 创建时间：2024/10/17 20:37:52
 * 版本：V1.0.0
 * 描述：
 *
 * ----------------------------------------------------------------
 * 修改人：
 * 时间：
 * 修改说明：
 *
 * 版本：V1.0.1
 *----------------------------------------------------------------*/
#endregion << 版 本 注 释 >>

using H.WPF.Framework;
using IMX.Function;
using IMX.Function.Base;
using IMX.Function.Config;
using IMX.Function.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMX.ATS.ATEConfig.Function
{
    /// <summary>
    /// 高压直流源流程配置模板类
    /// </summary>
    public class FunViewModelDCSource : FunViewModel
    {
        #region 公共属性

        public override TestFunction Func { get; set; } = TestFunction.Create(FuncitonType.DCSOURCE);

        public override FuncitonType SupportFuncitonType => FuncitonType.DCSOURCE;

        public override string SupportFuncitonString => "DCSource";

        #region 界面绑定属性

        private double setOutputVol = 0;
        /// <summary>
        /// 设置拉载电压
        /// </summary>
        public double SetOutputVol
        {
            get => setOutputVol = (Func.Config as FunConfig_HVDCSource).Set_Vol;
            set
            {
                if (Set(nameof(SetOutputVol), ref setOutputVol, value))
                {
                    (Func.Config as FunConfig_HVDCSource).Set_Vol = value;
                }
            }
        }

        private double setOutputCur = 0;
        /// <summary>
        /// 设置拉载电流
        /// </summary>
        public double SetOutputCur
        {
            get => setOutputCur = (Func.Config as FunConfig_HVDCSource).Set_Cur;
            set
            {
                if (Set(nameof(SetOutputCur), ref setOutputCur, value))
                {
                    (Func.Config as FunConfig_HVDCSource).Set_Cur = value;

                }
            }
        }

        #endregion

        #region 界面绑定指令
        #endregion

        #endregion

        #region 私有变量
        #endregion

        #region 私有方法
        #endregion

        #region 保护方法
        protected override void WindowLoadedExecute(object obj)
        {
            //base.WindowLoadedExecute(obj);
        }

        protected override void WindowClosedExecute(object obj)
        {
            base.WindowClosedExecute(obj);
        }
        #endregion


        #region 构造方法
        public FunViewModelDCSource() { }
        #endregion

    }
}
