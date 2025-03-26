#region << 版 本 注 释 >>
/*----------------------------------------------------------------
 * 版权所有 (c) 2025   保留所有权利。
 * CLR版本：4.0.30319.42000
 * 机器名称：LAPTOP-9Q9TTD5V
 * 公司名称：
 * 命名空间：IMX.ATS.DBCConfig.ViewModel
 * 唯一标识：faeaba18-094f-4db0-8683-2a39204be61d
 * 文件名：CustomConfigViewModel
 * 当前用户域：LAPTOP-9Q9TTD5V
 * 
 * 创建者：58274
 * 创建时间：2025/3/18 11:33:53
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
using GalaSoft.MvvmLight.Command;
using H.WPF.Framework;
using IMX.DB;
using IMX.DB.Model;
using Piggy.VehicleBus.Common;
using Super.Zoo.Framework;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using H.Maths;
using System.Windows.Documents;
using IMX.Logger;
using FastDeepCloner;

namespace IMX.ATS.DBCConfig
{
    public class CustomConfigViewModel : ExtendViewModelBase
    {


        #region 公共属性

        #region 界面绑定属性

        private ObservableCollection<Test_CustomMessageInfo> customconfigs = [];
        /// <summary>
        /// 配置名称列表
        /// </summary>
        public ObservableCollection<Test_CustomMessageInfo> CustomConfigs
        {
            get => customconfigs;
            set => Set(nameof(CustomConfigs), ref customconfigs, value);
        }

        private int selectedconfigindex = -1;
        /// <summary>
        /// 当前选择配置序号
        /// </summary>
        public int SelectedConfigIndex
        {
            get => selectedconfigindex;
            set 
            {
                if (Set(nameof(SelectedConfigIndex), ref selectedconfigindex, value))
                {
                    if (value < 0)
                    {
                        return;
                    }

                    GetConfig(value);
                }
            }
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


        private int selectedmessageindex;
        /// <summary>
        /// 当前选择消息序号
        /// </summary>
        public int SelectedMessageIndex
        {
            get => selectedmessageindex;
            set => Set(nameof(SelectedMessageIndex), ref selectedmessageindex, value);
        }

        #endregion

        #region 界面绑定指令

        /// <summary>
        /// 保存自定义帧配置
        /// </summary>
        public RelayCommand Save => new RelayCommand(() => 
        {
            try
            {
                List<Test_CustomMessage> messages = [];
                for (int i = 0; i < CustomMessages.Count; i++)
                {
                    var message = CustomMessages[i];
                    if (!message.IDRegex)
                    {
                        MessageBox.Show($"第 【{i+1}】 帧自定义帧 帧ID 不符合格式要求", "帧ID异常");
                        return;
                    }
                    if (!message.DataRegex)
                    {
                        MessageBox.Show($"自定义帧【{message.MessageID}】数据不符合格式要求","数据异常");
                        return;
                    }
                    messages.Add(CustomMessages[i].CustomMessage);
                }
                DBOperate.Default.UpdateCustomMessage(CustomConfigs[SelectedConfigIndex].Id, messages)
                .AttachIfFailed(result=>MessageBox.Show(result.Message,"自定义帧保存失败"))
                .AttachIfSucceed(result=>MessageBox.Show("自定义帧保存成功!"));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.GetMessage(),"自定义帧保存异常");
                SuperDHHLoggerManager.Exception( LoggerType.FROMLOG, nameof(CustomConfigViewModel), nameof(Save), ex);
            }

        });

        /// <summary>
        /// 插入新配置
        /// </summary>
        public RelayCommand Insert => new RelayCommand(() =>
        {
            Test_CustomMessageInfo config = new Test_CustomMessageInfo { ConfigName = $"自定义消息组{CustomConfigs.Count + 1}", DBCConfigID = dbcid };
            if (CustomConfigs.Count >= 1)
            {
                if (MessageBox.Show("是否复制当前自定义配置内容", "新建自定义配置", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    config = DeepCloner.Clone(CustomConfigs[SelectedConfigIndex]);
                    config.Id = 0;
                }
            }

#if DEBUG_VIEW
            DBOperate.Default.InsertCustomConfig(config)
                       .AttachIfFailed(result => MessageBox.Show(result.Message, "新建配置失败"))
                       .AttachIfSucceed(result =>
                       {
                           CustomConfigs.Add(config);
                           SelectedMessageIndex = CustomConfigs.Count - 1;
                       });
#else
            CustomConfigs.Add(config);
#endif

            SelectedConfigIndex = CustomConfigs.Count - 1;
            if (SelectedConfigIndex == 0)
            {
                GetConfig(0);
            }
        });

        /// <summary>
        /// 添加消息
        /// </summary>
        public RelayCommand AddMessage => new RelayCommand(() => 
        {
            CustomMessages.Add(new CustomConfig() { CustomMessage = new Test_CustomMessage { FrameFormat = FrameFormat.StandardCAN } });
        });

        /// <summary>
        /// 删除消息
        /// </summary>
        public RelayCommand DeletMessage => new RelayCommand(() => 
        {
            if (SelectedMessageIndex < 0)
            {
                return;
            }

            if (MessageBox.Show($"是否删除【{CustomMessages[SelectedMessageIndex].MessageID}】消息？", "删除提示", MessageBoxButton.YesNo) != MessageBoxResult.Yes)
            {
                return;
            }
            CustomMessages.RemoveAt(SelectedMessageIndex);

        });
        #endregion

        #endregion

        #region 私有变量

        private int dbcid = -1;
        #endregion

        #region 私有方法

        /// <summary>
        /// 获取当前配置
        /// </summary>
        /// <param name="index"></param>
        private void GetConfig(int index) 
        {
            if (CustomConfigs.Count < 1)
            {
                return;
            }

            CustomMessages.Clear();

            try
            {
                for (int i = 0; i < CustomConfigs[index]?.CustomMessages?.Count; i++)
                {
                    var config = CustomConfigs[index].CustomMessages[i];
                    CustomConfig custom = new()
                    {
                        MessageID = config.Message_ID.ToString("X"),
                        CycleTime = config.CycleTime,
                        Data = config.Data.ToString(" ", "X2"),
                        SendCount = config.SendCount,
                        SelectedFrameFormat = config.FrameFormat switch
                        {
                            FrameFormat.StandardCAN => 0,
                            FrameFormat.ExtendedCAN => 1,
                            FrameFormat.StandardCANFD => 2,
                            FrameFormat.ExtendedCANFD => 3,
                            _ => -1,
                        },

                        CustomMessage = config
                    };

                    CustomMessages.Add(custom);
                }
            }
            catch (Exception ex)
            {
                SuperDHHLoggerManager.Exception(LoggerType.FROMLOG, nameof(CustomConfigViewModel), nameof(GetConfig), ex);
            }
            

            //DBOperate.Default.GetCustomMessages(GlobalModel.Test_DBC.Id, CustomConfigNames[index])
            //    .AttachIfFailed(result => MessageBox.Show(result.Message, "配置获取失败"))
            //    .AttachIfSucceed(result => 
            //    {
            //        for (int i = 0; i < result.Data?.Count; i++)
            //        {
            //            var config = result.Data[i];
            //            CustomConfig custom = new CustomConfig 
            //            {
            //                MessageID = config.Message_ID,
            //                CycleTime = config.CycleTime,
            //                Data = config.Data.ToString(" ", "X2"),
            //                SendCount = config.SendCount
            //            };

            //            custom.SelectedFrameFormat = config.FrameFormat switch
            //            {
            //                FrameFormat.StandardCAN => 0,
            //                FrameFormat.ExtendedCAN => 1,
            //                FrameFormat.StandardCANFD => 2,
            //                FrameFormat.ExtendedCANFD => 3,
            //                _ => -1,
            //            };

            //            custom.CustomMessage = config;

            //            CustomMessages.Add(custom);
            //        }

            //    }) ;
        }
        #endregion

        #region 保护方法
        protected override void WindowLoadedExecute(object obj)
        {
#if !DEBUG_VIEW
            //for (int i = 1; i <= 5; i++)
            //{
            //    CustomConfigs.Add(new Test_CustomMessageInfo
            //    {
            //        ConfigName = $"自定义帧{i}",
            //        CustomMessages = new List<Test_CustomMessage> { new Test_CustomMessage { Message_ID = 0xA, CycleTime = 10, FrameFormat = FrameFormat.StandardCAN, SendCount = -1, Data = [(byte)i, 0x01, 0x02, 0x03, 0x0A, 0x0B, 0x0C, 0x1D] } }
            //    });

            //}
#else
            if (dbcid == GlobalModel.Test_DBC.Id)
            {
                return;
            }
            
            dbcid = GlobalModel.Test_DBC.Id;

            CustomConfigs.Clear();
            OperateResult<List<Test_CustomMessageInfo>> result =  DBOperate.Default.GetCustomConfig(dbcid);
            if (!result)
            {
                MessageBox.Show(result.Message,"配置加载失败");
                return;
            }

            for (int i = 0; i < result.Data?.Count; i++)
            {
                CustomConfigs.Add(result.Data[i]);
            }

            if (CustomConfigs.Count > 0)
            {
                SelectedConfigIndex = 0;
                //GetConfig(0);
            }
#endif

            //GetConfig(SelectedMessageIndex);
            //base.WindowLoadedExecute(obj);
        }

        protected override void WindowClosedExecute(object obj)
        {
            base.WindowClosedExecute(obj);
        }
        #endregion


        #region 构造方法
        public CustomConfigViewModel() { }
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

        private int selectedframeformat;
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
                            0=> FrameFormat.StandardCAN,
                            1=> FrameFormat.ExtendedCAN,
                            2=> FrameFormat.ExtendedCANFD,
                            3=> FrameFormat.ExtendedCANFD,
                            _=> FrameFormat.Unknow,
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
        public Test_CustomMessage CustomMessage { get; set; } = new Test_CustomMessage();
    }
}
