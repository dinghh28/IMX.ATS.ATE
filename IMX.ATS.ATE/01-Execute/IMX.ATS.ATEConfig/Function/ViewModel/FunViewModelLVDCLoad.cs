#region << 版 本 注 释 >>
/*----------------------------------------------------------------
 * 版权所有 (c) 2025   保留所有权利。
 * CLR版本：4.0.30319.42000
 * 机器名称：LAPTOP-9Q9TTD5V
 * 公司名称：
 * 命名空间：IMX.ATS.ATEConfig.Function.ViewModel
 * 唯一标识：848773db-0dcd-40dd-8537-136e44079d6c
 * 文件名：FunViewModelLVDCLoad
 * 当前用户域：LAPTOP-9Q9TTD5V
 * 
 * 创建者：58274
 * 创建时间：2025/5/21 17:28:59
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

using IMX.Common;
using IMX.Device.Common;
using IMX.Device.Common.Enumerations;
using IMX.Function;
using IMX.Function.Base;
using IMX.Function.ViewModel;
using IMX.Function.ViewModel.Model;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace IMX.ATS.ATEConfig.Function
{
    /// <summary>
    /// 低压直流负载配置模板
    /// </summary>
    public class FunViewModelLVDCLoad : SteppingFunViewModel
    {
        #region 公共属性

        private TestFunction func = TestFunction.Create(FuncitonType.DCLoad);
        public override TestFunction Func
        {
            get => func;
            set
            {
                func = value;
                FunConfig_LVDCLoad config = value.Config as FunConfig_LVDCLoad;

                StepValues.Clear();

                ObservableCollection<string> CondNames = new ObservableCollection<string>();
                ObservableCollection<ModDeviceReadData> CondValues = new ObservableCollection<ModDeviceReadData>();

                for (int i = 0; i < SupportDeviceInfo.DeviceRecInfo["AN87330"].Count; i++)
                {
                    CondNames.Add(SupportDeviceInfo.DeviceRecInfo["AN87330"][i].DataInfo.Name);
                    CondValues.Add(SupportDeviceInfo.DeviceRecInfo["AN87330"][i]);
                    //data.ConditionValues.Add((Func.Config as FunConfig_ACSource).ConditionalValues[i]);
                }

                config.Values ??= [];

                for (int i = 0; i < config.Values.Count; i++)
                {
                    StepValues.Add(new StepValue
                    {
                        ConditionValue = config.Values[i],
                        ConditionValues = CondValues,
                        ConditionNames = CondNames,
                        ConditionIndex = CondNames.ToList().FindIndex(n => n == config.Values[i].Value.DataInfo.Name),
                    });
                }

                if (config.Set_Model == Opaerate_Mode.NULL)
                {
                    config.Set_Model = Opaerate_Mode.CC;
                }
                else
                {
                    config.Set_Model = SetModel;
                }
            }
        }


        public override FuncitonType SupportFuncitonType => FuncitonType.LVDCLoad;

        public override string SupportFuncitonString => "LVDCLoad";
        #region 界面绑定属性
        #endregion

        #region 界面绑定指令
        private bool enableSetValue = true;
        /// <summary>
        /// 拉载设置标志位
        /// </summary>
        public override bool EnableSetValue
        {
            get => enableSetValue;
            set => Set(nameof(EnableSetValue), ref enableSetValue, value);
        }

        private bool enablesetloadvalue;
        /// <summary>
        /// 允许拉载设置标志位
        /// </summary>
        public bool EnableSetLoadValue
        {
            get => enablesetloadvalue;
            set => Set(nameof(EnableSetLoadValue), ref enablesetloadvalue, value);
        }

        private bool enablesetstepvalue;
        /// <summary>
        /// 允许步进参数设置标志位
        /// </summary>
        public override bool EnableSetStepValue
        {
            get => enablesetstepvalue;
            set
            {
                if (Set(nameof(EnableSetStepValue), ref enablesetstepvalue, value))
                {
                    EnableSetValue = value ? false : true;
                }

            }
        }

        private bool set_StepModel = false;
        /// <summary>
        /// 允许步进设置标志位
        /// </summary>
        public override bool Set_StepModel
        {
            get
            {
                EnableSetStepValue = (Func.Config as FunConfig_LVDCLoad).EnableStepping;
                return set_StepModel = (Func.Config as FunConfig_LVDCLoad).EnableStepping;
            }
            set
            {
                if (Set(nameof(Set_StepModel), ref set_StepModel, value))
                {
                    (Func.Config as FunConfig_LVDCLoad).EnableStepping = value;

                    EnableSetStepValue = value;
                    EnableSetValue = value ? false : true;

                }
            }
        }

        private DeviceOutPutState set_shortstate = DeviceOutPutState.OFF;
        /// <summary>
        /// 短路模式设置
        /// </summary>
        public DeviceOutPutState Set_ShortState
        {
            get
            {
                if ((Func.Config as FunConfig_LVDCLoad).Set_ShortState == DeviceOutPutState.ON)
                {
                    EnableSetLoadValue = false;
                    EnableSetStepValue = false;
                    Set_StepModel = false;
                    EnableSetValue = false;
                }
                else
                {
                    EnableSetLoadValue = true;
                }
                return set_shortstate = (Func.Config as FunConfig_LVDCLoad).Set_ShortState;
            }
            set
            {
                if (Set(nameof(Set_ShortState), ref set_shortstate, value))
                {
                    (Func.Config as FunConfig_LVDCLoad).Set_ShortState = value;

                    if (value == DeviceOutPutState.ON)
                    {
                        EnableSetLoadValue = false;
                        EnableSetStepValue = false;
                        Set_StepModel = false;
                        EnableSetValue = false;
                    }
                    else
                    {
                        EnableSetLoadValue = true;
                    }
                }
            }
        }

        public List<Opaerate_Mode> Models { get; } = new List<Opaerate_Mode> { Opaerate_Mode.CC, Opaerate_Mode.CV, Opaerate_Mode.CR };

        private Opaerate_Mode setmodel = Opaerate_Mode.CC;
        /// <summary>
        /// 设置运行模式
        /// </summary>
        public Opaerate_Mode SetModel
        {
            get
            {
                return setmodel;
            }
            set
            {
                if (Set(nameof(SetModel), ref setmodel, value))
                {
                    (Func.Config as FunConfig_LVDCLoad).Set_Model = value;
                }
            }
        }

        private double set_loadvalue;
        /// <summary>
        /// 设置拉载值
        /// </summary>
        public double Set_LoadValue
        {
            get => set_loadvalue = (Func.Config as FunConfig_LVDCLoad).Set_LoadValue;
            set
            {
                if (Set(nameof(Set_LoadValue), ref set_loadvalue, value))
                {
                    (Func.Config as FunConfig_LVDCLoad).Set_LoadValue = value;
                }
            }
        }

        private double setlimit;
        /// <summary>
        /// 限制值
        /// </summary>
        public double SetLimit
        {
            get => setlimit = (Func.Config as FunConfig_LVDCLoad).SetLimtValue;
            set
            {
                if (Set(nameof(SetLimit), ref setlimit, value))
                {
                    (Func.Config as FunConfig_LVDCLoad).SetLimtValue = value;
                }
            }
        }


        private double set_POSitiveValue;
        /// <summary>
        /// 设置上升斜率
        /// </summary>
        public double Set_POSitiveValue
        {
            get => set_POSitiveValue = (Func.Config as FunConfig_LVDCLoad).Set_ParamValue1;
            set
            {
                if (Set(nameof(Set_POSitiveValue), ref set_POSitiveValue, value))
                {
                    (Func.Config as FunConfig_LVDCLoad).Set_ParamValue1 = value;
                }
            }
        }

        private double set_EGativeValue;
        /// <summary>
        /// 设置下升斜率
        /// </summary>
        public double Set_EGativeValue
        {
            get => set_EGativeValue = (Func.Config as FunConfig_LVDCLoad).Set_ParamValue2;
            set
            {
                if (Set(nameof(Set_EGativeValue), ref set_EGativeValue, value))
                {
                    (Func.Config as FunConfig_LVDCLoad).Set_ParamValue2 = value;
                }
            }
        }
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
        public FunViewModelLVDCLoad() { }
        #endregion

    }
}
