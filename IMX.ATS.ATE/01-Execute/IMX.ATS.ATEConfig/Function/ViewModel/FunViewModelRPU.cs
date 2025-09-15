#region << 版 本 注 释 >>
/*----------------------------------------------------------------
 * 版权所有 (c) 2024   保留所有权利。
 * CLR版本：4.0.30319.42000
 * 机器名称：LAPTOP-9Q9TTD5V
 * 公司名称：
 * 命名空间：IMX.ATS.ATEConfig.Function
 * 唯一标识：83c9a238-d623-44d3-9537-a4083b9520b1
 * 文件名：FunViewModelAPU
 * 当前用户域：LAPTOP-9Q9TTD5V
 * 
 * 创建者：58274
 * 创建时间：2024/10/17 20:17:57
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
using IMX.Common;
using IMX.Function;
using IMX.Function.Base;
using IMX.Function.ViewModel;
using IMX.Function.ViewModel.Model;
using Super.Zoo.Framework;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMX.ATS.ATEConfig.Function
{
    /// <summary>
    /// 倒灌电源流程配置模板类
    /// </summary>
    public class FunViewModelRPU : SteppingFunViewModel
    {

        #region 公共属性
        private TestFunction func = TestFunction.Create(FuncitonType.RPU);
        public override TestFunction Func
        {
            get => func;
            set
            {
                func = value;
                FunConfig_RPU config = value.Config as FunConfig_RPU;

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
                            DeviceTypename = "Acquisition",
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
                                DeviceTypename = "Acquisition",
                            });
                        }
                    }

                    for (int i = 0; i < GlobalModel.TestDBCconfig.Test_DBCReceiveSignals.Count; i++)
                    {
                        var data = GlobalModel.TestDBCconfig.Test_DBCReceiveSignals[i];
                        ConditionValues.Add(new ModDeviceReadData
                        {
                            DataInfo = new ModTestDataInfo { Name = data.Custom_Name },
                            DeviceTypename = "Product",
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

        public override FuncitonType SupportFuncitonType =>  FuncitonType.RPU;

        public override string SupportFuncitonString => "RPU";


        #region 界面绑定属性

        private double setOutputVol = 0;
        /// <summary>
        /// 设置拉载电压
        /// </summary>
        public double SetOutputVol
        {
            get => setOutputVol = (Func.Config as FunConfig_RPU).Set_Vol;
            set
            {
                if (Set(nameof(SetOutputVol), ref setOutputVol, value))
                {
                    (Func.Config as FunConfig_RPU).Set_Vol = value;
                }
            }
        }

        private double setOutputCur = 0;
        /// <summary>
        /// 设置拉载电流
        /// </summary>
        public double SetOutputCur
        {
            get => setOutputCur = (Func.Config as FunConfig_RPU).Set_Cur;
            set
            {
                if (Set(nameof(SetOutputCur), ref setOutputCur, value))
                {
                    (Func.Config as FunConfig_RPU).Set_Cur = value;

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
        public FunViewModelRPU() { }
        #endregion

    }
}
