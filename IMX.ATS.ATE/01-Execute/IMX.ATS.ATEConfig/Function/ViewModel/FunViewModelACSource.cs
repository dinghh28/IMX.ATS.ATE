using IMX.Function.Base;
using IMX.Function.ViewModel;
using IMX.Function;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace IMX.ATS.ATEConfig.Function
{
    /// <summary>
    /// 交流源配置模板
    /// </summary>
    public class FunViewModelACSource : FunViewModel
    {

        #region 公共属性

        public override TestFunction Func { get; set; } = TestFunction.Create(FuncitonType.ACSource);

        public override FuncitonType SupportFuncitonType => FuncitonType.ACSource;

        public override string SupportFuncitonString => "ACSource";

        #region 界面绑定属性

        private double setOutputVol = 0;
        /// <summary>
        /// 设置拉载电压
        /// </summary>
        public double SetOutputVol
        {
            get => setOutputVol = (Func.Config as FunConfig_ACSource).Set_Vol;
            set
            {
                if (Set(nameof(SetOutputVol), ref setOutputVol, value))
                {
                    (Func.Config as FunConfig_ACSource).Set_Vol = value;
                    OperateType = SetOutPutState.ON;
                }
            }
        }

        private double setOutputFrq = 0;
        /// <summary>
        /// 设置拉载电流
        /// </summary>
        public double SetOutputFrq
        {
            get => setOutputFrq = (Func.Config as FunConfig_ACSource).Set_Frequency;
            set
            {
                if (Set(nameof(SetOutputFrq), ref setOutputFrq, value))
                {
                    (Func.Config as FunConfig_ACSource).Set_Frequency = value;

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
        public FunViewModelACSource() { }
        #endregion
    }
}
