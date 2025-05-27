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
using IMX.Device.Base;

namespace IMX.ATS.Manual
{
    public class ManualViewModelTempBox: ExtendViewModelBase
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


        private double set_Temperature;
        /// <summary>
        /// 温箱温度
        /// </summary>
        public double Set_Temperature
        {
            get => set_Temperature;
            set => Set(nameof(Set_Temperature), ref set_Temperature, value);
        }


        private double set_TempSlope;
        /// <summary>
        /// 温度斜率
        /// </summary>
        public double Set_TempSlope
        {
            get => set_TempSlope;
            set => Set(nameof(Set_TempSlope), ref set_TempSlope, value);
        }

        private double set_Humidity;
        /// <summary>
        /// 温箱湿度
        /// </summary>
        public double Set_Humidity
        {
            get => set_Humidity;
            set => Set(nameof(Set_Humidity), ref set_Humidity, value);
        }

        private double set_HumidSlope;
        /// <summary>
        /// 湿度斜率
        /// </summary>
        public double Set_HumidSlope
        {
            get => set_HumidSlope;
            set => Set(nameof(Set_HumidSlope), ref set_HumidSlope, value);
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

                if (!(deviceInfo.DeviceOperate is IDCLoad operate))
                {
                    MessageBox.Show($"设备类型异常：【{deviceInfo.DeviceOperate.GetType()}】");
                    return;
                }

                string InfoString = string.Empty;

                OperateResult result = operate.SetLoadValue(Set_Temperature);//, Set_TempSlope, Set_Humidity);

                if (!result)
                {
                    MessageBox.Show($"设备参数设置异常：【{result.Message}】");
                    return;
                }
                InfoString = $"设置\n [温度]{Set_Temperature}\n [温度斜率]{Set_TempSlope}\n [湿度]{Set_Humidity}\n [湿度斜率]{Set_HumidSlope}成功";

                SuperDHHLoggerManager.Info(LoggerType.TESTLOG, nameof(ManualViewModelTempBox), nameof(SetedValues), InfoString);

                if (OperateType != SetOutPutState.Null)
                {
                    operate.SetOnOff(OperateType == SetOutPutState.ON ? DeviceOutPutState.ON : DeviceOutPutState.OFF);

                    InfoString = OperateType == SetOutPutState.ON ? "打开" : "关闭";

                    SuperDHHLoggerManager.Info(LoggerType.TESTLOG, nameof(ManualViewModelTempBox), nameof(SetedValues), $"设备已{InfoString}");
                }
            }
            catch (Exception ex)
            {
                SuperDHHLoggerManager.Exception(LoggerType.TESTLOG, nameof(ManualViewModelTempBox), nameof(SetedValues), ex);

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
}
