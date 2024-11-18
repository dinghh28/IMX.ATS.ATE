using IMX.Function.Base;
using IMX.Function.ViewModel;
using IMX.Function;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using IMX.Function.Base.Enumerations;
using System.Collections.ObjectModel;
using GalaSoft.MvvmLight.Command;
using IMX.Device.Common;
using Super.Zoo.Framework;
using System.Windows.Forms;
using System.Windows.Markup;
using IMX.Common;

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

        private bool enablesetloadvalue = false;
        /// <summary>
        /// 允许拉载设置标志位
        /// </summary>
        public bool EnableSetLoadValue
        {
            get => enablesetloadvalue;
            set => Set(nameof(EnableSetLoadValue), ref enablesetloadvalue, value);
        }

        private bool enablesetstepvalue = false;
        /// <summary>
        /// 允许步进参数设置标志位
        /// </summary>
        public bool EnableSetStepValue
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


        private bool enableSetValue = true;
        /// <summary>
        /// 非步进模式下的参数设置标志位
        /// </summary>
        public bool EnableSetValue
        {
            get => enableSetValue;
            set => Set(nameof(EnableSetValue), ref enableSetValue, value);
        }

        private bool set_StepModel = false;
        /// <summary>
        /// 允许步进设置标志位
        /// </summary>
        public bool Set_StepModel
        {
            get
            {
                EnableSetStepValue= (Func.Config as FunConfig_ACSource).EnableStepping;
                return set_StepModel = (Func.Config as FunConfig_ACSource).EnableStepping;
            }
            set
            {
                if (Set(nameof(Set_StepModel), ref set_StepModel, value))
                {
                    (Func.Config as FunConfig_ACSource).EnableStepping = value;

                    EnableSetStepValue = value;

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
            get => stride = (Func.Config as FunConfig_ACSource).Stride;
            set
            {
                if (Set(nameof(Stride), ref stride, value))
                {
                    (Func.Config as FunConfig_ACSource).Stride = value;
                }
            }
        }

        private int stepfrequency;
        /// <summary>
        /// 步进步频
        /// </summary>
        public int StepFrequency
        {
            get => stepfrequency = (Func.Config as FunConfig_ACSource).StepFrequency;
            set
            {
                if (Set(nameof(StepFrequency), ref stepfrequency, value))
                {
                    (Func.Config as FunConfig_ACSource).StepFrequency = value;
                }
            }
        }

        private double startloadvalue;
        /// <summary>
        /// 起始拉载值
        /// </summary>
        public double StartLoadValue
        {
            get => startloadvalue = (Func.Config as FunConfig_ACSource).StartLoadValue;
            set
            {
                if (Set(nameof(StartLoadValue), ref startloadvalue, value))
                {
                    (Func.Config as FunConfig_ACSource).StartLoadValue = value;
                }
            }
        }

        private double endloadvalue;
        /// <summary>
        /// 结束拉载值
        /// </summary>
        public double EndLoadValue
        {
            get => endloadvalue = (Func.Config as FunConfig_ACSource).EndLoadValue;
            set
            {
                if (Set(nameof(EndLoadValue), ref endloadvalue, value))
                {
                    (Func.Config as FunConfig_ACSource).EndLoadValue = value;
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
            get => condition = (Func.Config as FunConfig_ACSource).StepCondition;
            set
            {
                if (Set(nameof(Condition), ref condition, value))
                {
                    (Func.Config as FunConfig_ACSource).StepCondition = value;
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
            get
            {
                stepvalues = new ObservableCollection<StepValue>();
                ObservableCollection<string> CondNames = new ObservableCollection<string>();
                ObservableCollection<ModDeviceReadData> CondValues = new ObservableCollection<ModDeviceReadData>();

                for (int i = 0; i < SupportDeviceInfo.DeviceRecInfo["AN87330"].Count; i++)
                {
                    CondNames.Add(SupportDeviceInfo.DeviceRecInfo["AN87330"][i].DataInfo.Name);
                    CondValues.Add(SupportDeviceInfo.DeviceRecInfo["AN87330"][i]);
                    //data.ConditionValues.Add((Func.Config as FunConfig_ACSource).ConditionalValues[i]);
                }
                if ((Func.Config as FunConfig_ACSource).Values == null)
                {
                    (Func.Config as FunConfig_ACSource).Values = new List<StepConditionValue>();
                }

                (Func.Config as FunConfig_ACSource).Values.ForEach(x =>
                {
                    stepvalues.Add(new StepValue
                    {
                        ConditionValue = x,
                        ConditionValues = CondValues,
                        ConditionNames = CondNames,
                    });
                });

                return stepvalues;
            }
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
                    //SelectConditionName = new RelayCommand<object>(StepValuesADD),
                };
                //获取功率计设备高压直流侧电压电流数据

                //SupportDeviceInfo.DeviceRecInfo["AN87330"]
                //for (int i = 0; i < (Func.Config as FunConfig_ACSource)?.ConditionalValues.Count; i++)
                for (int i = 0; i < SupportDeviceInfo.DeviceRecInfo["AN87330"].Count; i++)
                {
                    data.ConditionNames.Add(SupportDeviceInfo.DeviceRecInfo["AN87330"][i].DataInfo.Name);
                    data.ConditionValues.Add(SupportDeviceInfo.DeviceRecInfo["AN87330"][i]);
                    //data.ConditionValues.Add((Func.Config as FunConfig_ACSource).ConditionalValues[i]);
                }
                if ((Func.Config as FunConfig_ACSource).Values == null)
                {
                    (Func.Config as FunConfig_ACSource).Values = new List<StepConditionValue>();
                }

                (Func.Config as FunConfig_ACSource)?.Values.Add(data.ConditionValue);

                StepValues.Add(data);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.GetMessage(), "条件添加异常");
                return;
            }

        }
        //private void StepValuesADD(object index)
        //{
        //    StepConditionValue stepCondition = (Func.Config as FunConfig_ACSource)?.Values[SelectedValueIndex];
        //    StepValues[SelectedValueIndex].ConditionValue.Value = StepValues[SelectedValueIndex].ConditionValues[(int)index];
        //    stepCondition = StepValues[SelectedValueIndex].ConditionValue;
        //}

        /// <summary>
        /// 删除步进跳出条件
        /// </summary>
        private void Delet()
        {
            if (SelectedValueIndex == -1)
            {
                MessageBox.Show("请选择要删除的条件", "条件删除异常");
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
                (Func.Config as FunConfig_ACSource)?.Values.RemoveAt(SelectedValueIndex);
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
        public FunViewModelACSource() { }
        #endregion
    }
}
