using H.WPF.Framework;
using IMX.Device.Base.DeviceInerfaces;
using IMX.Device.Common.Enumerations;
using IMX.Device.Common;
using IMX.Function.Base;
using IMX.Logger;
using Super.Zoo.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using GalaSoft.MvvmLight.Command;
using System.Collections.ObjectModel;
using System.CodeDom.Compiler;

namespace IMX.ATS.Manual
{
    /// <summary>
    /// 水浴设备操作
    /// </summary>
    public class ManualViewModelWaterBox : ExtendViewModelBase
    {
        #region 公有变量

        //设备开关操作
        public List<SetOutPutState> OperateTypeList { get; set; } = new List<SetOutPutState>
        {
            SetOutPutState.ON,
            SetOutPutState.OFF,
            SetOutPutState.Null
        };

        #endregion

        #region 界面绑定

        #region 属性

        private SetOutPutState operateType;
        /// <summary>
        /// 当前动作
        /// </summary>
        public SetOutPutState OperateType
        {
            get => operateType;
            set => Set(nameof(OperateType), ref operateType, value);
        }

        private ObservableCollection<WaterBoxModel> waterBoxSetValues = new ObservableCollection<WaterBoxModel>
        {
            new WaterBoxModel{ Channel= "通道1" },
            new WaterBoxModel{ Channel= "通道2" },
            new WaterBoxModel{ Channel= "通道3" },
        };
        /// <summary>
        /// 设置参数
        /// </summary>
        public ObservableCollection<WaterBoxModel> WaterBoxSetValues
        {
            get => waterBoxSetValues;
            set => Set(nameof(WaterBoxSetValues), ref waterBoxSetValues, value);
        }

        #endregion

        #region 指令

        public RelayCommand SetValues => new RelayCommand(SetedValues);

        #endregion
        #endregion

        #region 私有方法

        private void SetedValues()
        {
            try
            {
                if (!(GlobalModel.DicDeviceInfo.TryGetValue(EDeviceType.DCLoad.ToString(), out DeviceInfo_ALL deviceInfo)))
                {
                    MessageBox.Show($"设备初始化异常：【{deviceInfo.GetType()}】");
                    return;
                }

                if  (!(deviceInfo.DeviceOperate is IDCLoad operate))
                {
                    MessageBox.Show($"设备类型异常：【{deviceInfo.DeviceOperate.GetType()}】");
                    return;
                }

                string InfoString = string.Empty;


                List<(double temp, double tempslope, double flow, double pressure)> values = new List<(double temp, double tempslope, double flow, double pressure)> { };
                for (int i = 0; i < 3; i++)
                {
                    values.Add((WaterBoxSetValues[i].SetValue.Set_Temperature, WaterBoxSetValues[i].SetValue.Set_TemperatureSlope, WaterBoxSetValues[i].SetValue.Set_Flow, WaterBoxSetValues[i].SetValue.Set_Pressure));
                }

                //OperateResult SetRtl = operate.Set_Value(values);
                OperateResult SetRtl = OperateResult.Succeed();
                if (!SetRtl)
                {
                    MessageBox.Show($"水浴设备参数设置失败：{SetRtl.Message}");
                    return;
                }

                InfoString = $"所有通道设置\n[水浴温度]-[通道1：]{WaterBoxSetValues[0].SetValue.Set_Temperature}-[通道2：]{WaterBoxSetValues[1].SetValue.Set_Temperature}-[通道3：]{WaterBoxSetValues[2].SetValue.Set_Temperature}\n" +
                    $"[水浴流速]-[通道1：]{WaterBoxSetValues[0].SetValue.Set_Flow}-[通道2：]{WaterBoxSetValues[1].SetValue.Set_Flow}-[通道3：]{WaterBoxSetValues[2].SetValue.Set_Flow}\n" +
                    $"[水浴流速]-[通道1：]{WaterBoxSetValues[0].SetValue.Set_Pressure}-[通道2：]{WaterBoxSetValues[1].SetValue.Set_Pressure}-[通道3：]{WaterBoxSetValues[2].SetValue.Set_Pressure}\n";

                SuperDHHLoggerManager.Info(LoggerType.TESTLOG, nameof(ManualViewModelWaterBox), nameof(SetedValues), InfoString);

                if (OperateType != SetOutPutState.Null)
                {
                    for (int i = 0; i < 3; i++)
                    {
                        //OperateResult OnOffRlt = operate.SetOnOff(i, OperateType == SetOutPutState.ON);
                        operate.SetOnOff(OperateType == SetOutPutState.ON ? DeviceOutPutState.ON : DeviceOutPutState.OFF);

                        InfoString = OperateType == SetOutPutState.ON ? "打开" : "关闭";

                        SuperDHHLoggerManager.Info(LoggerType.TESTLOG, nameof(ManualViewModelWaterBox), nameof(SetedValues), $"设备通道{i}已{InfoString}");
                    }

                    SuperDHHLoggerManager.Info(LoggerType.TESTLOG, nameof(ManualViewModelWaterBox), nameof(SetedValues), $"所有通道{OperateType}");

                }

                MessageBox.Show($"水浴参数设置成功：\r\n{InfoString}");
            }
            catch (Exception ex)
            {
                SuperDHHLoggerManager.Exception(LoggerType.TESTLOG, nameof(ManualViewModelWaterBox), nameof(SetedValues), ex);

                MessageBox.Show("设置失败", "参数设置", MessageBoxButton.OK);
            }
        }

        #endregion

        #region 公有方法

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

        #region 构造函数

        #endregion
    }

    /// <summary>
    /// 水浴试验配置界面模型类
    /// </summary>
    public class WaterBoxModel : ExtendViewModelBase
    {
        /// <summary>
        /// 通道名称
        /// </summary>
        public string Channel { get; set; } = "通道1";

        private ModWaterBoxSetValue setValue = new ModWaterBoxSetValue();
        /// <summary>
        /// 设置参数
        /// </summary>
        public ModWaterBoxSetValue SetValue
        {
            get => setValue;
            set => Set(nameof(setValue), ref setValue, value);
        }

    }

    /// <summary>
    /// 水浴设置参数模型类
    /// </summary>
    public class ModWaterBoxSetValue
    {
        /// <summary>
        /// 设置温度
        /// </summary>
        public double Set_Temperature { get; set; } = 0;

        /// <summary>
        /// 设置温度速率
        /// </summary>
        public double Set_TemperatureSlope { get; set; } = 0;

        /// <summary>
        /// 设置流速
        /// </summary>
        public double Set_Flow { get; set; } = 0;

        /// <summary>
        /// 设置压力
        /// </summary>
        public double Set_Pressure { get; set; } = 0;
    }
}
