#region << 版 本 注 释 >>
/*----------------------------------------------------------------
 * 版权所有 (c) 2025   保留所有权利。
 * CLR版本：4.0.30319.42000
 * 机器名称：LAPTOP-9Q9TTD5V
 * 公司名称：
 * 命名空间：IMX.ATS.ATE.ViewModel
 * 唯一标识：79a87ef1-6fbd-40fb-99e5-01b57b67bfd3
 * 文件名：WaterBathViewModel
 * 当前用户域：LAPTOP-9Q9TTD5V
 * 
 * 创建者：58274
 * 创建时间：2025/4/22 16:57:16
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
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IMX.ATS.ATE
{
    /// <summary>
    /// 水浴操作界面类
    /// </summary>
    public class WaterBathViewModel : ViewModelBase
    {
        #region 公共属性

        #region 界面绑定属性

        #region 设置参数

        private uint tempslope = 1;
        /// <summary>
        /// 水浴温度速率
        /// </summary>
        public uint TempSlope
        {
            get => tempslope;
            set => Set(nameof(TempSlope), ref tempslope, value);
        }

        private double flow = 25;
        /// <summary>
        /// 设置流量
        /// </summary>
        public double Flow
        {
            get => flow;
            set => Set(nameof(Flow), ref flow, value);
        }

        private double pressure = 25;
        /// <summary>
        /// 设置压力
        /// </summary>
        public double Pressure
        {
            get => pressure;
            set => Set(nameof(Pressure), ref pressure, value);
        }

        private double temperature = 25;
        /// <summary>
        /// 设置温度
        /// </summary>
        public double Temperature
        {
            get => temperature;
            set => Set(nameof(Temperature), ref temperature, value);
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

        #region 界面绑定指令
        public RelayCommand SetValue => new RelayCommand(Set);
        #endregion

        #endregion

        #region 私有变量
        private IWaterBath WaterBath;
        #endregion

        #region 私有方法
        private void Set()
        {
            WaterBath.Device_SetTemp(0, Temperature)
                .And(WaterBath.Device_SetFlow(0,Flow))
                .And(WaterBath.Device_SetPressure(0, Pressure))
                .ThenAnd(result =>
                {
                    if (OperateType != SetOutPutState.Null)
                    {
                        return WaterBath.Device_SetOnOff(0, OperateType == SetOutPutState.ON);
                    }

                    return OperateResult.Succeed();
                })
                .AttachIfFailed(result => { MessageBox.Show(result.Message, "温箱参数设置失败"); });
        }
        #endregion


        #region 构造方法
        public WaterBathViewModel() 
        {
            if (GlobalModel.DicDeviceInfo.TryGetValue("WaterBath", out DeviceInfo_ALL deviceInfo))
            {
                WaterBath = deviceInfo.DeviceOperate as IWaterBath;

                for (int i = 0; i < WaterBath.ReadInfos.Count; i++)
                {
                    ActualtDatas.Add(new EnvironmentReadData
                    {
                        InfoName = WaterBath.ReadInfos[i].DataInfo.Name,
                    });
                }
            }
        }
        #endregion

    }
}
