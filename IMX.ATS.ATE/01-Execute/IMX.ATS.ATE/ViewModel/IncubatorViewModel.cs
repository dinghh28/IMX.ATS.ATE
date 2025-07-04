#region << 版 本 注 释 >>
/*----------------------------------------------------------------
 * 版权所有 (c) 2025   保留所有权利。
 * CLR版本：4.0.30319.42000
 * 机器名称：LAPTOP-9Q9TTD5V
 * 公司名称：
 * 命名空间：IMX.ATS.ATE.ViewModel
 * 唯一标识：79ebe197-aa82-4c17-a0cb-360f94556645
 * 文件名：IncubatorViewModel
 * 当前用户域：LAPTOP-9Q9TTD5V
 * 
 * 创建者：58274
 * 创建时间：2025/4/22 16:58:18
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

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using H.WPF.Framework;
using IMX.Device.Base.DeviceInerfaces;
using IMX.Function.Base;
using Super.Zoo.Framework;
using Super.Zoo.Framework.Debugger;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace IMX.ATS.ATE
{
    /// <summary>
    /// 温箱操作界面类
    /// </summary>
    public class IncubatorViewModel : ViewModelBase
    {

        #region 公共属性

        #region 界面绑定属性

        #region 设置参数
        private double temperature = 25;
        /// <summary>
        /// 设置温度
        /// </summary>
        public double Temperature
        {
            get => temperature;
            set => Set(nameof(Temperature), ref temperature, value);
        }

        private double humid = 0;
        /// <summary>
        /// 设置湿度
        /// </summary>
        public double Humid
        {
            get => humid;
            set => Set(nameof(Humid), ref humid, value);
        }


        private uint tempslope = 1;
        /// <summary>
        /// 温度速率
        /// </summary>
        public uint TempSlope
        {
            get => tempslope;
            set => Set(nameof(TempSlope), ref tempslope, value);
        }

        /// <summary>
        /// 设备状态操作列表
        /// </summary>
        public List<SetOutPutState> OperateTypeList { get; set; } = new List<SetOutPutState>
        {
            SetOutPutState.ON,
            SetOutPutState.OFF,
            SetOutPutState.Null
        };

        private SetOutPutState operatetype = SetOutPutState.Null;
        /// <summary>
        /// 设备启停状态
        /// </summary>
        public SetOutPutState OperateType
        {
            get => operatetype;
            set => Set(nameof(OperateType), ref operatetype, value);
        }
        #endregion

        #region 读取参数
        //private double actualtemperature;

        //public double ActualTemperature
        //{
        //    get => temperature;
        //    set => Set(nameof(Temperature), ref temperature, value);
        //}

        private ObservableCollection<EnvironmentReadData> actualtdatas = new ObservableCollection<EnvironmentReadData>();
        /// <summary>
        /// 实际值
        /// </summary>
        public ObservableCollection<EnvironmentReadData> ActualtDatas
        {
            get => actualtdatas;
            set => Set(nameof(ActualtDatas), ref actualtdatas, value);
        }


        #endregion

        #endregion

        #region 界面绑定指令
        public RelayCommand SetValue => new RelayCommand(Set);
        #endregion

        #endregion

        #region 私有变量
        private IIncubator Incubator;
        #endregion

        #region 公共方法
        public void IncubatorThread()
        {
            int reflashcount = 0;
            while (GlobalModel.IsWinOpen)
            {
                Incubator.Device_ReadAll();

                if (++reflashcount >= Incubator.DeviceConfig.RefreshTime)
                {
                    Application.Current.Dispatcher.Invoke(new Action(() => 
                    {
                        for (int i = 0; i < Incubator.ReadInfos.Count; i++)
                        {
                            ActualtDatas[i].Value = Incubator.ReadInfos[i].DataInfo.Value;
                        }
                    }));
                }
            }
        }
        #endregion

        #region 私有方法

        private void Set()
        {
            Incubator.Device_SetTemperature(Temperature)
                .And(Incubator.Device_SetTempSlope(TempSlope))
                .And(Incubator.Device_SetHumid(Humid))
                .ThenAnd(result => 
                {
                    if (OperateType!= SetOutPutState.Null) 
                    {
                        return Incubator.SetOnOff(OperateType == SetOutPutState.ON);
                    }

                    return OperateResult.Succeed();
                })
                .AttachIfFailed(result => { MessageBox.Show(result.Message, "温箱参数设置失败"); });
        }

        #endregion


        #region 构造方法
        public IncubatorViewModel()
        {
            if (GlobalModel.DicDeviceInfo.TryGetValue("Incubator", out DeviceInfo_ALL deviceInfo))
            {
                Incubator = deviceInfo.DeviceOperate as IIncubator;
                for (int i = 0; i < Incubator.ReadInfos.Count; i++)
                {
                    ActualtDatas.Add(new EnvironmentReadData 
                    {
                        InfoName = Incubator.ReadInfos[i].DataInfo.Name,
                    });
                }
            }
        }
        #endregion
    }

    public class EnvironmentReadData : ViewModelBase 
    {
        private string infoname;
        /// <summary>
        /// 信号名称
        /// </summary>
        public string InfoName
        {
            get => infoname;
            set => Set(nameof(InfoName), ref infoname, value);
        }


        private double value = 0;
        /// <summary>
        /// 读取值
        /// </summary>
        public double Value
        {
            get => value;
            set => Set(nameof(Value), ref value, value);
        }

    }
}
