#region << 版 本 注 释 >>
/*----------------------------------------------------------------
 * 版权所有 (c) 2024   保留所有权利。
 * CLR版本：4.0.30319.42000
 * 机器名称：LAPTOP-9Q9TTD5V
 * 公司名称：
 * 命名空间：IMX.ATS.ATEConfig.Function.ViewModel
 * 唯一标识：92fd0f54-4644-4e71-b89d-ad4b1c8b6bd3
 * 文件名：FunViewModelDCLoad
 * 当前用户域：LAPTOP-9Q9TTD5V
 * 
 * 创建者：58274
 * 创建时间：2024/10/24 11:27:42
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
using IMX.Function.Base;
using IMX.Function.ViewModel;
using IMX.Function;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IMX.Device.Common.Enumerations;
using IMX.Function.Base.Enumerations;
using GalaSoft.MvvmLight;
using System.Windows;
using System.Collections.ObjectModel;
using IMX.Common;
using GalaSoft.MvvmLight.Command;
using Super.Zoo.Framework;

namespace IMX.ATS.ATEConfig.Function
{
    public class FunViewModelDCLoad : FunViewModel
    {
        #region 公共属性

        public override TestFunction Func { get; set; } = TestFunction.Create(FuncitonType.DCLoad);

        public override FuncitonType SupportFuncitonType => FuncitonType.DCLoad;

        public override string SupportFuncitonString => "DCLoad";

        #region 界面绑定属性

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
        /// 允许步进设置标志位
        /// </summary>
        public bool EnableSetStepValue
        {
            get => enablesetstepvalue = (Func.Config as FunConfig_DCLoad).EnableStepping;
            set
            {
                if (Set(nameof(EnableSetStepValue), ref enablesetstepvalue, value))
                {
                    (Func.Config as FunConfig_DCLoad).EnableStepping = value;
                }
            }
        }

        private DeviceOutPutState set_shortstate = DeviceOutPutState.OFF;
        /// <summary>
        /// 短路模式设置
        /// </summary>
        public DeviceOutPutState Set_ShortState
        {
            get => set_shortstate = (Func.Config as FunConfig_DCLoad).Set_ShortState;
            set
            {
                if (Set(nameof(Set_ShortState), ref set_shortstate, value))
                {
                    (Func.Config as FunConfig_DCLoad).Set_ShortState = value;

                    if (value == DeviceOutPutState.ON)
                    {
                        EnableSetLoadValue = false;
                        EnableSetStepValue = false;
                    }
                    else
                    {
                        EnableSetLoadValue = true;
                    }
                }
            }
        }

        public List<string> Models { get; } = new List<string> { "CCL", "CCH", "CVL", "CVH", "CRL", "CRH" };

        private string set_model;
        /// <summary>
        /// 运行模式设置
        /// </summary>
        public string Set_Model
        {
            get => set_model = (Func.Config as FunConfig_DCLoad).Set_Model;
            set
            {
                if (Set(nameof(Set_Model), ref set_model, value))
                {
                    (Func.Config as FunConfig_DCLoad).Set_Model = value;
                }
            }
        }

        private double set_loadvalue;
        /// <summary>
        /// 设置拉载值
        /// </summary>
        public double Set_LoadValue
        {
            get => set_loadvalue = (Func.Config as FunConfig_DCLoad).Set_LoadValue;
            set
            {
                if (Set(nameof(Set_LoadValue), ref set_loadvalue, value))
                {
                    (Func.Config as FunConfig_DCLoad).Set_LoadValue = value;
                }
            }
        }

        #region 步进参数
        private double stride;
        /// <summary>
        /// 步进步幅
        /// </summary>
        public double Stride
        {
            get => stride = (Func.Config as FunConfig_DCLoad).Stride;
            set
            {
                if (Set(nameof(Stride), ref stride, value))
                {
                    (Func.Config as FunConfig_DCLoad).Stride = value;
                }
            }
        }

        private int stepfrequency;
        /// <summary>
        /// 步进步频
        /// </summary>
        public int StepFrequency
        {
            get => stepfrequency = (Func.Config as FunConfig_DCLoad).StepFrequency;
            set
            {
                if (Set(nameof(StepFrequency), ref stepfrequency, value))
                {
                    (Func.Config as FunConfig_DCLoad).StepFrequency = value;
                }
            }
        }

        private double startloadvalue;
        /// <summary>
        /// 结束拉载值
        /// </summary>
        public double StartLoadValue
        {
            get => startloadvalue = (Func.Config as FunConfig_DCLoad).StartLoadValue;
            set
            {
                if (Set(nameof(StartLoadValue), ref startloadvalue, value))
                {
                    (Func.Config as FunConfig_DCLoad).StartLoadValue = value;
                }
            }
        }

        private double endloadvalue;
        /// <summary>
        /// 结束拉载值
        /// </summary>
        public double EndLoadValue
        {
            get => endloadvalue = (Func.Config as FunConfig_DCLoad).EndLoadValue;
            set
            {
                if (Set(nameof(EndLoadValue), ref endloadvalue, value))
                {
                    (Func.Config as FunConfig_DCLoad).EndLoadValue = value;
                }
            }
        }
        #endregion

        #region 步进判定参数
        /// <summary>
        /// 步进各参变综合判定条件
        /// </summary>
        public List<StepConditions> Conditions { get; } = new List<StepConditions> { StepConditions.AND, StepConditions.OR };

        private StepConditions condition;
        /// <summary>
        /// 当前选择综合判定条件
        /// </summary>
        public StepConditions Condition
        {
            get => condition = (Func.Config as FunConfig_DCLoad).StepCondition;
            set
            {
                if (Set(nameof(Condition), ref condition, value))
                {
                    (Func.Config as FunConfig_DCLoad).StepCondition = value;
                }
            }
        }

        private int selectedvalueindex;
        /// <summary>
        /// 当前选择步进条件地址
        /// </summary>
        public int SelectedValueIndex
        {
            get => selectedvalueindex;
            set => Set(nameof(SelectedValueIndex), ref selectedvalueindex, value);
        }


        private ObservableCollection<StepValue> stepvalues = new ObservableCollection<StepValue>();
        /// <summary>
        /// 步进条件列表
        /// </summary>
        public ObservableCollection<StepValue> StepValues
        {
            get => stepvalues;
            set => Set(nameof(StepValues), ref stepvalues, value);
        }
        #endregion

        #endregion

        #region 界面绑定指令
        /// <summary>
        /// 新增条件
        /// </summary>
        public RelayCommand AddCondition => new RelayCommand(Add);

        /// <summary>
        /// 删除条件
        /// </summary>
        public RelayCommand DeletCondition => new RelayCommand(Delet);

        #endregion

        #endregion

        #region 私有变量
        #endregion

        #region 私有方法
        /// <summary>
        /// 添加步进跳出条件
        /// </summary>
        private void Add()
        {
            try
            {
                StepValue data = new StepValue
                {
                    ConditionValue = new StepConditionValue(),
                };

                for (int i = 0; i < (Func.Config as FunConfig_DCLoad)?.ConditionalValues.Count; i++)
                {
                    data.ConditionValues.Add((Func.Config as FunConfig_DCLoad).ConditionalValues[i]);
                }

                (Func.Config as FunConfig_DCLoad)?.Values.Add(data.ConditionValue);

                StepValues.Add(data);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.GetMessage(), "条件添加异常");
                return;
            }

        }

        /// <summary>
        /// 删除步进跳出条件
        /// </summary>
        private void Delet()
        {
            if (SelectedValueIndex == -1)
            {
                MessageBox.Show("请选择要删除的条件","条件删除异常");
                return;
            }

            if (StepValues.Count < 1)
            {
                MessageBox.Show("暂无步进条件，请添加后再选择删除", "条件删除异常");
                return;
            }

            try
            {
                StepValues.RemoveAt(SelectedValueIndex);
                (Func.Config as FunConfig_DCLoad)?.Values.RemoveAt(SelectedValueIndex);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.GetMessage(), "条件删除异常");
                return;
            }
        }
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
        public FunViewModelDCLoad() { }
        #endregion

    }

    /// <summary>
    /// 步进跳出条件变量参数界面类
    /// </summary>
    public class StepValue : ViewModelBase
    {
        /// <summary>
        /// 变量判定条件列表
        /// </summary>
        public List<StepValueConditions> ValueConditions { get; } = new List<StepValueConditions> 
        {  
            StepValueConditions.GREATERTHAN, StepValueConditions.LESSTHAN, StepValueConditions.EQUALTO, StepValueConditions.NOTEQUALTO
        };

        private ObservableCollection<ModDeviceReadData> conditionvalues = new ObservableCollection<ModDeviceReadData>();
        /// <summary>
        /// 变量列表
        /// </summary>
        public ObservableCollection<ModDeviceReadData> ConditionValues
        {
            get => conditionvalues;
            set => Set(nameof(ConditionValues), ref conditionvalues, value);
        }


        private StepConditionValue conditionvalue;
        /// <summary>
        /// 变量信息
        /// </summary>
        public StepConditionValue ConditionValue
        {
            get => conditionvalue;
            set => Set(nameof(ConditionValue), ref conditionvalue, value);
        }

    }
}
