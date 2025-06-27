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

using GalaSoft.MvvmLight.CommandWpf;
using H.WPF.Framework;
using IMX.Common;
using IMX.Device.Common;
using IMX.Function;
using IMX.Function.Base;
using IMX.Function.Base.Enumerations;
using IMX.Function.ViewModel;
using IMX.Function.ViewModel.Model;
using Super.Zoo.Framework;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IMX.ATS.ATEConfig.Function
{
    /// <summary>
    /// 高压直流源流程配置模板类
    /// </summary>
    public class FunViewModelHVDCSource : SteppingFunViewModel
    {
        #region 公共属性
        private TestFunction func = TestFunction.Create(FuncitonType.HVDCSource);
        /// <inheritdoc/>
        public override TestFunction Func
        {
            get => func;
            set
            {
                func = value;
                FunConfig_HVDCSource config = value.Config as FunConfig_HVDCSource;

                StepValues.Clear();

                ObservableCollection<string> CondNames = new ObservableCollection<string>();
                ObservableCollection<ModDeviceReadData> CondValues = new ObservableCollection<ModDeviceReadData>();

                if (ConditionValues.Count < 1)
                {
                    if (!SupportConfig.DicProcessConfig.TryGetValue(GlobalModel.NowProcessName, out ProcessConfig_EX processconfig))
                    {
                        return;
                    }


                    for (int i = 0; i < processconfig.Test_ReadData_Euq.Count; i++)
                    {
                        var data = processconfig.Test_ReadData_Euq[i];
                        ConditionValues.Add(new ModDeviceReadData
                        {
                            DataInfo = data,
                        });
                    }

                    if (GlobalModel.NowElectricity == ATE.Common.Electricity.Three
                        || GlobalModel.NowElectricity == ATE.Common.Electricity.ThreeANDInversion)
                    {
                        for (int i = 0; i < processconfig.Test_ReadData_EX.Count; i++)
                        {
                            var data = processconfig.Test_ReadData_EX[i];
                            ConditionValues.Add(new ModDeviceReadData
                            {
                                DataInfo = data,
                            });
                        }
                    }

                    for (int i = 0; i < GlobalModel.TestDBCconfig.Test_DBCReceiveSignals.Count; i++)
                    {
                        var data = GlobalModel.TestDBCconfig.Test_DBCReceiveSignals[i];
                        ConditionValues.Add(new ModDeviceReadData
                        {
                            DataInfo = new ModTestDataInfo { Name = data.Custom_Name },
                        });
                    }
                }

                for (int i = 0; i < ConditionValues.Count; i++)
                {
                    CondNames.Add(ConditionValues[i].DataInfo.Name);
                    CondValues.Add(ConditionValues[i]);
                }

                config.Values ??= [];

                for (int i = 0; i < config.Values.Count; i++)
                {
                    int findindex = CondNames.ToList().FindIndex(n => n == config.Values[i].Value.DataInfo.Name);
                    if (findindex == -1)
                    {
                        continue;
                    }

                    var configvalue = new StepValue
                    {
                        ConditionValue = config.Values[i],
                        ConditionValues = CondValues,
                        ConditionNames = CondNames,
                        ConditionIndex = findindex,
                    };

                    StepValues.Add(configvalue);
                }

            }
        }

        public override FuncitonType SupportFuncitonType => FuncitonType.HVDCSource;

        public override string SupportFuncitonString => "HVDCSource";

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
                    OperateType = SetOutPutState.ON;
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

        //#region 步进拉载切换控制
        //private bool enableSetValue = true;
        ///// <summary>
        ///// 拉载设置标志位
        ///// </summary>
        //public bool EnableSetValue
        //{
        //    get => enableSetValue;
        //    set => Set(nameof(EnableSetValue), ref enableSetValue, value);
        //}

        //private bool enablesetstepvalue;
        ///// <summary>
        ///// 允许步进参数设置标志位
        ///// </summary>
        //public bool EnableSetStepValue
        //{
        //    get => enablesetstepvalue;
        //    set
        //    {
        //        if (Set(nameof(EnableSetStepValue), ref enablesetstepvalue, value))
        //        {
        //            EnableSetValue = value ? false : true;
        //        }

        //    }
        //}

        //private bool set_StepModel = false;
        ///// <summary>
        ///// 允许步进设置标志位
        ///// </summary>
        //public bool Set_StepModel
        //{
        //    get
        //    {
        //        EnableSetStepValue = (Func.Config as FunConfig_HVDCSource).EnableStepping;
        //        return set_StepModel = (Func.Config as FunConfig_HVDCSource).EnableStepping;
        //    }
        //    set
        //    {
        //        if (Set(nameof(Set_StepModel), ref set_StepModel, value))
        //        {
        //            (Func.Config as FunConfig_HVDCSource).EnableStepping = value;

        //            EnableSetStepValue = value;
        //            EnableSetValue = !value;

        //        }
        //    }
        //}
        //#endregion

        //#region 步进参数
        //private double stride;
        ///// <summary>
        ///// 步进步幅
        ///// </summary>
        //public double Stride
        //{
        //    get => stride = (Func.Config as FunConfig_HVDCSource).Stride;
        //    set
        //    {
        //        if (Set(nameof(Stride), ref stride, value))
        //        {
        //            (Func.Config as FunConfig_HVDCSource).Stride = value;
        //        }
        //    }
        //}

        //private int stepfrequency;
        ///// <summary>
        ///// 步进步频
        ///// </summary>
        //public int StepFrequency
        //{
        //    get => stepfrequency = (Func.Config as FunConfig_HVDCSource).StepFrequency;
        //    set
        //    {
        //        if (Set(nameof(StepFrequency), ref stepfrequency, value))
        //        {
        //            (Func.Config as FunConfig_HVDCSource).StepFrequency = value;
        //        }
        //    }
        //}

        //private double startloadvalue;
        ///// <summary>
        ///// 起始拉载值
        ///// </summary>
        //public double StartLoadValue
        //{
        //    get => startloadvalue = (Func.Config as FunConfig_HVDCSource).StartLoadValue;
        //    set
        //    {
        //        if (Set(nameof(StartLoadValue), ref startloadvalue, value))
        //        {
        //            (Func.Config as FunConfig_HVDCSource).StartLoadValue = value;
        //        }
        //    }
        //}

        //private double endloadvalue;
        ///// <summary>
        ///// 结束拉载值
        ///// </summary>
        //public double EndLoadValue
        //{
        //    get => endloadvalue = (Func.Config as FunConfig_HVDCSource).EndLoadValue;
        //    set
        //    {
        //        if (Set(nameof(EndLoadValue), ref endloadvalue, value))
        //        {
        //            (Func.Config as FunConfig_HVDCSource).EndLoadValue = value;
        //        }
        //    }
        //}
        //#endregion

        //#region 步进判定参数
        ///// <summary>
        ///// 步进各参变综合判定条件
        ///// </summary>
        //public List<StepConditions> Conditions { get; } = new List<StepConditions> { StepConditions.AND, StepConditions.OR };

        //private StepConditions condition;
        ///// <summary>
        ///// 当前选择综合判定条件
        ///// </summary>
        //public StepConditions Condition
        //{
        //    get => condition = (Func.Config as FunConfig_HVDCSource).StepCondition;
        //    set
        //    {
        //        if (Set(nameof(Condition), ref condition, value))
        //        {
        //            (Func.Config as FunConfig_HVDCSource).StepCondition = value;
        //        }
        //    }
        //}

        //private int selectedvalueindex;
        ///// <summary>
        ///// 当前选择步进条件地址
        ///// </summary>
        //public int SelectedValueIndex
        //{
        //    get => selectedvalueindex;
        //    set => Set(nameof(SelectedValueIndex), ref selectedvalueindex, value);
        //}


        //private ObservableCollection<StepValue> stepvalues = new ObservableCollection<StepValue>();
        ///// <summary>
        ///// 步进条件列表
        ///// </summary>
        //public ObservableCollection<StepValue> StepValues
        //{
        //    get
        //    {
        //        return stepvalues;
        //    }
        //    set => Set(nameof(StepValues), ref stepvalues, value);
        //}
        //#endregion

        #endregion

        #region 界面绑定指令
        ///// <summary>
        ///// 新增条件
        ///// </summary>
        //public RelayCommand AddCondition => new RelayCommand(Add);

        ///// <summary>
        ///// 删除条件
        ///// </summary>
        //public RelayCommand DeletCondition => new RelayCommand(Delet);

        #endregion

        #endregion

        #region 私有变量
        #endregion

        #region 私有方法
        ///// <summary>
        ///// 添加步进跳出条件
        ///// </summary>
        //private void Add()
        //{
        //    try
        //    {
        //        StepValue data = new StepValue
        //        {
        //            ConditionValue = new StepConditionValue(),
        //            //SelectConditionName = new RelayCommand<object>(StepValuesADD),
        //        };
        //        //获取功率计设备高压直流侧电压电流数据
        //        //SupportDeviceInfo.DeviceRecInfo["AN87330"]
        //        //for (int i = 0; i < (Func.Config as FunConfig_DCLoad)?.ConditionalValues.Count; i++)
        //        for (int i = 0; i < SupportDeviceInfo.DeviceRecInfo["AN87330"].Count; i++)
        //        {
        //            data.ConditionNames.Add(SupportDeviceInfo.DeviceRecInfo["AN87330"][i].DataInfo.Name);
        //            data.ConditionValues.Add(SupportDeviceInfo.DeviceRecInfo["AN87330"][i]);
        //            //data.ConditionValues.Add((Func.Config as FunConfig_DCLoad).ConditionalValues[i]);
        //        }

        //        data.ConditionValue.Value = data.ConditionValues[0];

        //        if ((Func.Config as FunConfig_HVDCSource).Values == null)
        //        {
        //            (Func.Config as FunConfig_HVDCSource).Values = new List<StepConditionValue>();
        //        }
        //        (Func.Config as FunConfig_HVDCSource)?.Values.Add(data.ConditionValue);

        //        StepValues.Add(data);
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(ex.GetMessage(), "条件添加异常");
        //        return;
        //    }

        //}


        ///// <summary>
        ///// 删除步进跳出条件
        ///// </summary>
        //private void Delet()
        //{
        //    if (SelectedValueIndex == -1)
        //    {
        //        MessageBox.Show("请选择要删除的条件", "条件删除异常");
        //        return;
        //    }

        //    if (StepValues.Count < 1)
        //    {
        //        MessageBox.Show("暂无步进条件，请添加后再选择删除", "条件删除异常");
        //        return;
        //    }

        //    try
        //    {
        //        (Func.Config as FunConfig_HVDCSource)?.Values.RemoveAt(SelectedValueIndex);
        //        StepValues.RemoveAt(SelectedValueIndex);

        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(ex.GetMessage(), "条件删除异常");
        //        return;
        //    }
        //}

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
        public FunViewModelHVDCSource() { }
        #endregion

    }
}
