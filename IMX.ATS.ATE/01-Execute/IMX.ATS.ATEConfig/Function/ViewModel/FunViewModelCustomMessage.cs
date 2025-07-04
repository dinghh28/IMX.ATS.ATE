#region << 版 本 注 释 >>
/*----------------------------------------------------------------
 * 版权所有 (c) 2025   保留所有权利。
 * CLR版本：4.0.30319.42000
 * 机器名称：LAPTOP-9Q9TTD5V
 * 公司名称：
 * 命名空间：IMX.ATS.ATEConfig.Function.ViewModel
 * 唯一标识：88b4f602-b7da-41b8-a93e-271f545df723
 * 文件名：FunViewModelCustomMessage
 * 当前用户域：LAPTOP-9Q9TTD5V
 * 
 * 创建者：58274
 * 创建时间：2025/6/30 16:13:56
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
using H.Maths;
using H.WPF.Framework;
using IMX.Common;
using IMX.DB.Model;
using IMX.Device.Common;
using IMX.Function;
using IMX.Function.Base;
using IMX.Function.ViewModel;
using IMX.Function.ViewModel.Model;
using IMX.Logger;
using Piggy.VehicleBus.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;

namespace IMX.ATS.ATEConfig.Function
{
    public class FunViewModelCustomMessage : FunViewModel
    {

        #region 公共属性
        private TestFunction func = TestFunction.Create(FuncitonType.CustomMessage);
        public override TestFunction Func
        {
            get => func;
            set
            {
                func = value;
                FunConfig_CustomMessage config = value.Config as FunConfig_CustomMessage;
                funconfig = config;

                config.CustomSendMessages ??= [];

                for (int i = 0; i < config.CustomSendMessages.Count; i++)
                {
                    var message = config.CustomSendMessages[i];
                    CustomMessages.Add(new CustomConfig 
                    {
                        MessageID = message.Message_ID.ToString("X"),
                        CycleTime = message.CycleTime,
                        Data = message.Data.ToString(" ", "X2"),
                        SendCount = message.SendCount,
                        SelectedFrameFormat = message.FrameFormat switch
                        {
                            FrameFormat.StandardCAN => 0,
                            FrameFormat.ExtendedCAN => 1,
                            FrameFormat.StandardCANFD => 2,
                            FrameFormat.ExtendedCANFD => 3,
                            _ => -1,
                        },

                        CustomMessage = message,
                    });
                }
                //config.Values.ForEach(x =>
                //{
                //    StepValues.Add(new StepValue
                //    {
                //        ConditionValue = x,
                //        ConditionValues = CondValues,
                //        ConditionNames = CondNames,
                //        ConditionIndex = CondNames.ToList().FindIndex(n => n == x.Value.DataInfo.Name),
                //    });
                //});
            }
        }

        public override FuncitonType SupportFuncitonType => FuncitonType.CustomMessage;

        public override string SupportFuncitonString => SupportFuncitonType.ToString();
        #region 界面绑定属性

        private int selectedmessageindex;
        /// <summary>
        /// 当前选择消息序号
        /// </summary>
        public int SelectedMessageIndex
        {
            get => selectedmessageindex;
            set => Set(nameof(SelectedMessageIndex), ref selectedmessageindex, value);
        }

        private ObservableCollection<CustomConfig> custommessages = [];
        /// <summary>
        /// 自定义下发消息列表
        /// </summary>
        public ObservableCollection<CustomConfig> CustomMessages
        {
            get => custommessages;
            set => Set(nameof(CustomMessages), ref custommessages, value);
        }
        #endregion

        #region 界面绑定指令
        /// <summary>
        /// 添加自定义消息
        /// </summary>
        public RelayCommand AddMessage => new RelayCommand(() => 
        {
            try
            {
                CustomSendMessage config = new CustomSendMessage()
                {
                    CycleTime = 100,
                    SendCount = -1,
                    FrameFormat = FrameFormat.StandardCAN,
                };
                funconfig.CustomSendMessages.Add(config);
                CustomMessages.Add(new CustomConfig { CustomMessage = config });
            }
            catch (Exception ex)
            {
                MessageBox.Show("自定义消息添加异常");
                SuperDHHLoggerManager.Exception(LoggerType.FROMLOG, nameof(FunViewModelCustomMessage), "自定义消息添加异常", ex);
            }

        });

        /// <summary>
        /// 删除自定义消息
        /// </summary>
        public RelayCommand DeletMessage => new RelayCommand(() => 
        {
            if (SelectedMessageIndex < 0)
            {
                return;
            }
            try
            {
                funconfig.CustomSendMessages.RemoveAt(SelectedMessageIndex);
                CustomMessages.RemoveAt(SelectedMessageIndex);
            }
            catch (Exception ex)
            {
                MessageBox.Show("自定义消息删除异常");
                SuperDHHLoggerManager.Exception(LoggerType.FROMLOG, nameof(FunViewModelCustomMessage), "自定义消息删除异常", ex);
            }


        });
        #endregion

        #endregion

        #region 私有变量
        FunConfig_CustomMessage funconfig;
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
        public FunViewModelCustomMessage() { }
        #endregion

    }

    /// <summary>
    /// 自定义消息配置
    /// </summary>
    public class CustomConfig : ViewModelBase
    {
        private string messageid;
        /// <summary>
        /// 消息ID
        /// </summary>
        public string MessageID
        {
            get => messageid;
            set
            {
                if (Set(nameof(MessageID), ref messageid, value))
                {
                    string str = value.Replace("0x", "").Replace("0X", "").Replace(" ", "");
                    MatchCollection match = Regex.Matches(str, @"\b[0-9A-Fa-f]+\b");
                    if (match.Count != 1)
                    {
                        IDRegex = false;
                        return;
                    }

                    if (CustomMessage != null)
                    {
                        CustomMessage.Message_ID = Convert.ToUInt32(value, 16);
                    }

                    IDRegex = true;
                }
            }
        }

        private uint cycletime = 100;
        /// <summary>
        /// 消息发送周期(毫秒)
        /// </summary>
        public uint CycleTime
        {
            get => cycletime;
            set
            {
                if (Set(nameof(CycleTime), ref cycletime, value))
                {
                    if (CustomMessage != null)
                    {
                        CustomMessage.CycleTime = value;
                    }
                }
            }
        }

        private int sendcount = -1;
        /// <summary>
        /// 发送次数
        /// </summary>
        public int SendCount
        {
            get => sendcount;
            set
            {
                if (Set(nameof(SendCount), ref sendcount, value))
                {
                    if (CustomMessage != null)
                    {
                        CustomMessage.SendCount = value;
                    }
                }
            }
        }

        /// <summary>
        /// 报文格式列表
        /// </summary>
        public List<string> FrameFormats => ["CAN 标准帧", "CAN 扩展帧", "CAN FD 标准帧", "CAN FD 扩展帧"];

        private int selectedframeformat = 0;
        /// <summary>
        /// 当前选择报文格式序号
        /// </summary>
        public int SelectedFrameFormat
        {
            get => selectedframeformat;
            set
            {
                if (Set(nameof(SelectedFrameFormat), ref selectedframeformat, value))
                {
                    if (value < 0)
                    {
                        return;
                    }
                    if (CustomMessage != null)
                    {
                        CustomMessage.FrameFormat = value switch
                        {
                            0 => FrameFormat.StandardCAN,
                            1 => FrameFormat.ExtendedCAN,
                            2 => FrameFormat.ExtendedCANFD,
                            3 => FrameFormat.ExtendedCANFD,
                            _ => FrameFormat.Unknow,
                        };
                    }
                }
            }
        }

        private string data;

        public string Data
        {
            get => data;
            set
            {
                if (Set(nameof(Data), ref data, value))
                {
                    string str = value.Replace("0x", "").Replace("0X", "").Replace(" ", "");
                    if (str.Length % 2 > 0)
                    {
                        DataRegex = false;
                        return;
                    }
                    List<byte> bytes = [];
                    MatchCollection match = Regex.Matches(str, @"\b[0-9A-Fa-f]+\b");
                    if (match.Count != 1)
                    {
                        DataRegex = false;
                        return;
                    }

                    if (CustomMessage != null)
                    {
                        try
                        {
                            for (int i = 0; i < str.Length; i += 2)
                            {
                                bytes.Add(Convert.ToByte(str.Substring(i, 2), 16));
                            }

                            CustomMessage.Data = bytes.ToArray();

                        }
                        catch (Exception ex)
                        {
                            SuperDHHLoggerManager.Exception(LoggerType.FROMLOG, nameof(CustomConfig),"自定义帧转换", ex);
                            DataRegex = false;
                            return;
                        }
                    }

                    DataRegex = true;
                }
            }
        }

        /// <summary>
        /// 数据是否符合要求
        /// </summary>
        public bool DataRegex { get; private set; } = true;

        /// <summary>
        /// 消息ID是否符合要求
        /// </summary>
        public bool IDRegex { get; private set; } = true;

        /// <summary>
        /// 数据库存储信息
        /// </summary>
        public CustomSendMessage CustomMessage { get; set; } = new CustomSendMessage();
    }
}
