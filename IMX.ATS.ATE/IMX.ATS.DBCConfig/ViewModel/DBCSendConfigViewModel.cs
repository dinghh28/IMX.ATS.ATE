#region << 版 本 注 释 >>
/*----------------------------------------------------------------
 * 版权所有 (c) 2025   保留所有权利。
 * CLR版本：4.0.30319.42000
 * 机器名称：LAPTOP-9Q9TTD5V
 * 公司名称：
 * 命名空间：IMX.ATS.DBCConfig.ViewModel
 * 唯一标识：aed0324f-c91a-4cee-90b7-44d60176e8a6
 * 文件名：DBCSendConfigViewModel
 * 当前用户域：LAPTOP-9Q9TTD5V
 * 
 * 创建者：58274
 * 创建时间：2025/3/13 17:16:39
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
using GalaSoft.MvvmLight.Messaging;
using H.WPF.Framework;
using IMX.DB;
using IMX.DB.Model;
using IMX.Logger;
using Piggy.VehicleBus.Common;
using Piggy.VehicleBus.MessageProcess;
using Super.Zoo.Framework;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media;
using MessageBox = System.Windows.Forms.MessageBox;

namespace IMX.ATS.DBCConfig
{
    public class DBCSendConfigViewModel : ExtendViewModelBase
    {


        #region 公共属性

        #region 界面绑定属性

        #region DBC消息/信号列表
        private ObservableCollection<DBCMessageTreeNode> dbcMessages = new ObservableCollection<DBCMessageTreeNode>();
        /// <summary>
        /// DBC帧列表
        /// </summary>
        public ObservableCollection<DBCMessageTreeNode> DBCMessages
        {
            get => dbcMessages;
            set => Set(nameof(DBCMessages), ref dbcMessages, value);
        }

        private string selectedsignalinfo = string.Empty;
        /// <summary>
        /// 当前选中信号信息
        /// </summary>
        public string SelectedSignalInfo
        {
            get => selectedsignalinfo;
            set => Set(nameof(SelectedSignalInfo), ref selectedsignalinfo, value);
        }
        #endregion


        private ObservableCollection<DBCSignalConfig> signalconfigs = new ObservableCollection<DBCSignalConfig>();
        /// <summary>
        /// 信号配置列表
        /// </summary>
        public ObservableCollection<DBCSignalConfig> SignalConfigs
        {
            get => signalconfigs;
            set => Set(nameof(SignalConfigs), ref signalconfigs, value);
        }

        private ObservableCollection<SendSignalConfig> sendsignalconfigs = new ObservableCollection<SendSignalConfig>();
        /// <summary>
        /// 下发消息配置列表
        /// </summary>
        public ObservableCollection<SendSignalConfig> SendSignalConfigs
        {
            get => sendsignalconfigs;
            set => Set(nameof(SendSignalConfigs), ref sendsignalconfigs, value);
        }


        private DBCSignalConfig selectedsignalconfig;
        /// <summary>
        /// 当前选中配置信号
        /// </summary>
        public DBCSignalConfig SelectedSignalConfig
        {
            get => selectedsignalconfig;
            set => Set(nameof(SelectedSignalConfig), ref selectedsignalconfig, value);
        }

        #region 固定信号绑定
        private ObservableCollection<SendFixedSignalConfig> fixedsignalconfigs = new ObservableCollection<SendFixedSignalConfig>();
        /// <summary>
        /// 固定信号配置列表
        /// </summary>
        public ObservableCollection<SendFixedSignalConfig> FixedSignalConfigs
        {
            get => fixedsignalconfigs;
            set => Set(nameof(FixedSignalConfigs), ref fixedsignalconfigs, value);
        }

        //private ObservableCollection<SignalConfig> fixedsignals = [];

        //public ObservableCollection<SignalConfig> FixedSignals
        //{
        //    get => fixedsignals;
        //    set => Set(nameof(FixedSignals), ref fixedsignals, value);
        //}

        #endregion
        #endregion

        #region 界面绑定指令
        /// <summary>
        /// 保存配置
        /// </summary>
        public RelayCommand SaveConfig => new RelayCommand(SaveSignalConfig);

        public RelayCommand DeletSignal => new RelayCommand(DeletedSignal);
        #endregion

        #endregion

        #region 私有变量

        /// <summary>
        /// 当前选中信号
        /// </summary>
        private DBCMessageTreeNode selectedsignal;


        /// <summary>
        /// 消息文件加载器
        /// </summary>
        private IMessageFileLoader messageFileLoader;

        /// <summary>
        /// 下发信号配置列表
        /// </summary>
        private ObservableCollection<SignalConfig> lis_signal = [];

        /// <summary>
        /// 下发信号配置列表
        /// </summary>
        private Dictionary<uint, SendSignalConfig> dic_messageconfig = [];

        private int fileid = -1;
        #endregion

        #region 私有方法

        /// <summary>
        /// 保存DBC信号配置信息
        /// </summary>
        private void SaveSignalConfig()
        {
            try
            {
                List<Test_DBCInfo> dbcconfigs = new List<Test_DBCInfo>();
                List<Test_DBCMessageInfo> messageinfos = new List<Test_DBCMessageInfo>();

                #region 下发信号
                for (int i = 0; i < SendSignalConfigs?.Count; i++)
                {
                    var signalConfig = SendSignalConfigs[i];
                    //if (!signalConfig.IsConfiged)
                    //{
                    //    MessageBox.Show($"【{signalConfig.Config.CustomName}】\r\n未配置信号，请完成配置后再保存", "下发信号保存失败");
                    //    return;
                    //}

                    //var config = signalConfig.Config;

                    //dbcconfigs.Add(new Test_DBCInfo
                    //{
                    //    Custom_Name = config.CustomName,
                    //    MessageName = config.MessageName,
                    //    Signal_Name = config.Signal_Name,
                    //    SignalInitValue = config.SignalValue,
                    //});

                    messageinfos.Add(new Test_DBCMessageInfo 
                    {
                        MessageName = signalConfig.MessageName,
                        Message_ID = signalConfig.MessageID,
                        CycleTime = signalConfig.CycleTime,
                        FrameFormat = signalConfig.FrameFormat,
                    });
                }

                for (int i = 0; i < lis_signal.Count; i++)
                {
                    var config = lis_signal[i];
                    dbcconfigs.Add(new Test_DBCInfo
                    {
                        Custom_Name = config.Signal_Name,
                        Message_ID = config.Message_ID,
                        MessageName = config.MessageName,
                        Signal_Name = config.Signal_Name,
                        SignalInitValue = config.SignalValue,
                    });
                }
                #endregion
                #region 下发信号绑定系统变量
                Dictionary<int, string> dicfixedsignals = new Dictionary<int, string>();

                for (int i = 0; i < FixedSignalConfigs.Count; i++)
                {
                    var cfg = FixedSignalConfigs[i];
                    int index = cfg.SelectedSignalIndex;
                    string systeamname = cfg.SysteamName;

                    if (index == -1)
                    {
                        MessageBox.Show($"变量 【{systeamname}】 未绑定信号，请绑定后再保存");
                        return;
                    }

                    if (dicfixedsignals.TryGetValue(index, out string signalname))
                    {
                        MessageBox.Show($"变量 【{systeamname}】 与变量 【{signalname}】 重复绑定信号，请确认后再保存");
                        return;
                    }
                    dicfixedsignals.Add(index, systeamname);

                    lis_signal[index].CustomName = systeamname;
                }
                #endregion



                DBOperate.Default.UpdateSendSignals(GlobalModel.Test_DBC.Id, dbcconfigs,messageinfos)
                    .AttachIfSucceed(result => MessageBox.Show("下发信号保存成功!"))
                    .AttachIfFailed(result => MessageBox.Show("下发信号保存失败!"));
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{ex.GetMessage()}", "上报信号保存异常");
                SuperDHHLoggerManager.Exception(LoggerType.FROMLOG, nameof(DBCReceiveConfigViewModel), nameof(SaveSignalConfig), ex);
            }
        }

        #region 加载DBC文件

        /// <summary>
        /// 加载DBC文件
        /// </summary>
        /// <param name="fileData">文件内容</param>
        /// <param name="Extension">文件扩展</param>
        public void LoadFile(byte[] fileData, string Extension)
        {
            try
            {
                OperateResult<IMessageFileLoader> rltCreate = MessageFileLoader.Create(Extension, SuperDHHLoggerManager.DeviceLogger);
                if (!rltCreate)
                {
                    MessageBox.Show($"DBC文件加载失败:{rltCreate.Message}", "DBC文件解析异常", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                messageFileLoader = rltCreate.Data;

                var rltLoad = rltCreate.Data.Paser(fileData);
                if (!rltLoad)
                {
                    MessageBox.Show($"DBC解析失败:{rltLoad.Message}", "DBC文件解析异常", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // 加载项目
                if (rltLoad)
                {
                    LoadItems(messageFileLoader);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"DBC解析异常:{ex.Message}", "DBC文件解析异常", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }

        /// <summary>
        /// 获取文件信息
        /// </summary>
        /// <param name="loader"></param>
        private void LoadItems(IMessageFileLoader loader)
        {
            DBCMessages.Clear();

            foreach (var item in loader.MessageList.OrderBy(x => x.ID))
            {
                var message = CreateMessageNode(item);
                foreach (var signal in item.Signals)
                {
                    message.Signals.Add(CreateSignalNode(signal));
                }
                DBCMessages.Add(message);
            }
        }

        #region 消息操作
        /// <summary>
        /// 创建帧节点
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        private DBCMessageTreeNode CreateMessageNode(Piggy.VehicleBus.Common.Message message)
        {
            //bool isFD =
            //    message.FrameFormat == FrameFormat.StandardCANFD ||
            //    message.FrameFormat == FrameFormat.ExtendedCANFD;

            string extraMessage =
                message.CycleTime == 0 ? $" 《 发送周期为 0，无法发送消息 》 {Environment.NewLine}" :
                message.Grouped ? $" 《 该消息存在分组信号 》 {Environment.NewLine}" :
                "";

            DBCMessageTreeNode showmessage = new DBCMessageTreeNode
            {
                Name = $"{message.ID_HEX} - {message.Name}({message.FrameFormat})",
                Tag = message.ID,
                ToolTipText =
                    $"{(string.IsNullOrEmpty(extraMessage) ? "" : Environment.NewLine + extraMessage + Environment.NewLine)}" +
                    $"Length (byte): {message.Length}{Environment.NewLine}" +
                    $"Cycle Time: {message.CycleTime}{Environment.NewLine}" +
                    $"Frame Format: {message.FrameFormat}{Environment.NewLine}" +
                    $"Comment: {message.Comment}",
                // 额外显示
                BackColor =
                    message.CycleTime == 0 ? new SolidColorBrush(Colors.LightPink) :
                    message.Grouped ? new SolidColorBrush(Colors.LightSkyBlue) :
                     new SolidColorBrush(Colors.Transparent),
                Signals = new ObservableCollection<DBCMessageTreeNode>(),
            };

            //message.Signals.ForEach(signal =>
            //{
            //    showmessage.Signals.Add(CreateSignalNode(signal));
            //});

            return showmessage;
        }

        /// <summary>
        /// 创建信号节点
        /// </summary>
        /// <param name="signal">信号对象</param>
        /// <returns>信号节点</returns>
        private DBCMessageTreeNode CreateSignalNode(Signal signal)
        {
            string extraMessage =
                signal.Message.CycleTime == 0 ? $" 《 发送周期为 0，无法发送消息 》 {Environment.NewLine}" :
                signal.Grouped && signal.GroupID < 0 ? $" 《 Multiplexor 》 {Environment.NewLine}" :
                signal.Grouped && signal.GroupID >= 0 ? $" 《 {signal.Message.Signals.FirstOrDefault(x => x.Grouped && x.GroupID == -1)?.Name ?? "Unknow"} = 0x{signal.GroupID:X} 》{Environment.NewLine}" :
                "";

            return new DBCMessageTreeNode()
            {
                Name = signal.Name,
                Tag = signal.Message.ID,
                TagText = signal.Message.Name,
                InitValue = signal.InitValue,
                CycleTime = signal.Message.CycleTime,
                FrameFormat = signal.Message.FrameFormat,
                ToolTipText =
                    $"{(string.IsNullOrEmpty(extraMessage) ? "" : Environment.NewLine + extraMessage + Environment.NewLine)}" +
                    $"Start Bit: {signal.StartBit}{Environment.NewLine}" +
                    $"Length (bit): {signal.Length}{Environment.NewLine}" +
                    $"Byte Order: {signal.ByteOrder}{Environment.NewLine}" +
                    $"Data Type: {signal.DataType}{Environment.NewLine}" +
                    $"Initial Value: {signal.InitValue}{Environment.NewLine}" +
                    $"Factor: {signal.Factor}{Environment.NewLine}" +
                    $"Offset: {signal.Offset}{Environment.NewLine}" +
                    $"Range: [{signal.MinValue}, {signal.MaxValue}]{Environment.NewLine}" +
                    $"Unit: {signal.Unit}{Environment.NewLine}" +
                    $"Comment: {signal.Comment}",
                // 额外显示
                BackColor =
                    signal.Message.CycleTime == 0 ? new SolidColorBrush(Colors.LightPink) :
                    signal.Grouped && signal.GroupID < 0 ? new SolidColorBrush(Colors.Orange) :
                    signal.Grouped && signal.GroupID >= 0 ? new SolidColorBrush(Colors.LightGoldenrodYellow) :
                    new SolidColorBrush(Colors.Transparent),
                SelectSignal = new RelayCommand(SelectedSignal),
                SelectChange = new RelayCommand<object>(SignalSelectChanged),
            };
        }

        #endregion

        #endregion

        #region 信号选中处理
        /// <summary>
        /// 选择信号变更事件
        /// </summary>
        /// <param name="info"></param>
        private void SignalSelectChanged(object info)
        {
            if (info == null)
            {
                SelectedSignalInfo = string.Empty;
                selectedsignal = null;
                return;
            }

            if (!(info is DBCMessageTreeNode node))
            {
                SelectedSignalInfo = string.Empty;
                selectedsignal = null;
                return;
            }

            SelectedSignalInfo = node.Info.ToString();
            selectedsignal = node;
        }

        /// <summary>
        /// 信号选择配置
        /// </summary>
        private void SelectedSignal()
        {
            //if (SelectedSignalConfig == null)
            //{
            //    MessageBox.Show($"请选择需要配置上报信号！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            //    return;
            //}

            if (selectedsignal == null)
            {
                MessageBox.Show($"请选择配置信号！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }





            //SelectedSignalConfig.Config = config;
            

            if (!dic_messageconfig.TryGetValue(selectedsignal.Tag, out SendSignalConfig message))
            {
                message = new SendSignalConfig
                {
                    CycleTime = selectedsignal.CycleTime,
                    FrameFormat = selectedsignal.FrameFormat,
                    MessageID = selectedsignal.Tag,
                    MessageName = selectedsignal.TagText,
                };

                dic_messageconfig.Add(selectedsignal.Tag, message);

                SendSignalConfigs.Add(message);
            }
            else 
            {
                if (message.Signals.Count > 0)
                {
                    if (message.Signals.ToList().Exists(x => x.Signal_Name == selectedsignal.Name))
                    {
                        MessageBox.Show("此信号已存在下发帧列表中,请勿重复添加！", "提示！", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }
                }
            }

            SignalConfig config = new()
            {
                MessageName = selectedsignal.TagText,
                Signal_Name = selectedsignal.Name,
                Message_ID = selectedsignal.Tag,
                SignalValue = selectedsignal.InitValue.ToString(),
            };

            lis_signal.Add(config);
            //FixedSignals.Add(config);
            dic_messageconfig[selectedsignal.Tag].Signals.Add(config);


            //SelectedSignalConfig.Config.MessageName = selectedsignal.TagText;
            //SelectedSignalConfig.Config.Signal_Name = selectedsignal.Name;
            //SelectedSignalConfig.Config.Message_ID = selectedsignal.Tag;
            //SelectedSignalConfig.Config.SignalValue = selectedsignal.InitValue.ToString();
            //SelectedSignalConfig.IsConfiged = true;
        }

        /// <summary>
        /// 配置信号移除
        /// </summary>
        //private void RevomeSignal()
        //{
        //    if (SelectedSignalConfig == null)
        //    {
        //        MessageBox.Show($"请选择需要删除的信号！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        //        return;
        //    }

        //    try
        //    {
        //        lis_signal.Remove(SelectedSignalConfig.Config);
        //        dic_messageconfig[selectedsignal.Tag].Signals.Remove(SelectedSignalConfig.Config);
        //        SelectedSignalConfig = null;
        //        if (dic_messageconfig[selectedsignal.Tag].Signals.Count < 1)
        //        {
        //            //var message = SendSignalConfigs.
        //            SendSignalConfigs.RemoveAt(dic_messageconfig[selectedsignal.Tag].Index);
        //            dic_messageconfig.Remove(selectedsignal.Tag);
        //        }
        //    }
        //    catch (Exception ex)
        //    {

        //    }
            

        //    //if (!dic_messageconfig.TryGetValue(selectedsignal.Tag, out SendSignalConfig message))
        //    //{
        //    //    dic_messageconfig.Add(selectedsignal.Tag, new SendSignalConfig
        //    //    {
        //    //        CycleTime = selectedsignal.CycleTime,
        //    //        FrameFormat = selectedsignal.FrameFormat,
        //    //        MessageID = selectedsignal.Tag,
        //    //        MessageName = selectedsignal.TagText,
        //    //    });
        //    //}
        //    //dic_messageconfig[selectedsignal.Tag].Signals.Add(config);
        //    //SelectedSignalConfig.Config.MessageName = string.Empty;
        //    //SelectedSignalConfig.Config.Signal_Name = string.Empty;
        //    //SelectedSignalConfig.Config.Message_ID = 0;
        //    //SelectedSignalConfig.IsConfiged = false;
        //}

        /// <summary>
        /// 移除信号
        /// </summary>
        private void DeletedSignal() 
        {
            for (int i = 0; i < lis_signal.Count; i++)
            {
                if (!lis_signal[i].IsSelected)
                {
                    continue;
                }

                if (MessageBox.Show($"是否删除消息【{lis_signal[i].Message_ID}】下的【{lis_signal[i].Signal_Name}】信号", "删除信号", MessageBoxButtons.YesNo) != DialogResult.Yes)
                {
                    return;
                }

                var signal = lis_signal[i];
                var message = dic_messageconfig[signal.Message_ID];

                var dSignal = message.Signals
                    .Where(x => x.Message_ID == signal.Message_ID && x.Signal_Name == signal.Signal_Name)
                    .FirstOrDefault();

               var a =  message.Signals.Remove(dSignal);
                
                if (message.Signals.Count < 1)
                {
                    SendSignalConfigs.Remove(message);
                    dic_messageconfig.Remove(signal.Message_ID);
                }
                lis_signal.RemoveAt(i);

                return;
            }
        }
        #endregion

        /// <summary>
        /// 添加系统固定信号
        /// </summary>
        private void AddFixedSignal()
        {
            InitSignal();
            if (!GlobalModel.IsNew && GlobalModel.Test_DBC != null && GlobalModel.Test_DBC.Test_DBCSendSignals.Count > 0) 
            {
                var sendsignals = GlobalModel.Test_DBC.Test_DBCSendSignals;
                var sendmessages = GlobalModel.Test_DBC.Test_DBCSendMessages;
                try
                {
                    for (int i = 0; i < sendmessages.Count; i++)
                    {
                        var sendmessage = sendmessages[i];
                        if (!dic_messageconfig.TryGetValue(sendmessage.Message_ID, out SendSignalConfig message))
                        {
                            message = new SendSignalConfig
                            {
                                CycleTime = sendmessage.CycleTime,
                                FrameFormat = sendmessage.FrameFormat,
                                MessageID = sendmessage.Message_ID,
                                MessageName = sendmessage.MessageName,
                            };

                            dic_messageconfig.Add(sendmessage.Message_ID, message);

                            SendSignalConfigs.Add(message);
                        }
                    }

                    Dictionary<string, int> fixindex = new Dictionary<string, int>();

                    for (int i = 0; i < sendsignals.Count; i++)
                    {
                        var sendsignal = sendsignals[i];
                        SignalConfig config = new()
                        {
                            MessageName = sendsignal.MessageName,
                            CustomName = sendsignal.Custom_Name,
                            Signal_Name = sendsignal.Signal_Name,
                            Message_ID = sendsignal.Message_ID,
                            SignalValue = sendsignal.SignalInitValue,
                        };

                        lis_signal.Add(config);
                        dic_messageconfig[sendsignal.Message_ID].Signals.Add(config);
                        if (sendsignal.Signal_Name!= sendsignal.Custom_Name)
                        {
                            fixindex.Add(sendsignal.Custom_Name, i);
                            //SendFixedSignalConfig fix = new SendFixedSignalConfig 
                            //{
                            //    Signals = lis_signal,
                            //    SysteamName = sendsignal.Custom_Name,
                            //    Signal = lis_signal[i],
                            //};
                            //FixedSignalConfigs.Add(fix);
                        }
                    }

                    foreach (var item in fixindex)
                    {
                        FixedSignalConfigs.Add(new SendFixedSignalConfig
                        {
                            Signals = lis_signal,
                            SysteamName = item.Key,
                            SelectedSignalValue = lis_signal[item.Value].Signal_Name,
                        });
                    }
                }
                catch (Exception ex)
                {
                    SignalConfigs.Clear();
                    lis_signal.Clear();
                    dic_messageconfig.Clear();
                    SendSignalConfigs.Clear();
                    MessageBox.Show(ex.GetMessage(), "下发信号加载异常");
                    return;
                }
            }
            else
            {
                for (int i = 0; i < SupportConfig.LisRegularSendSignals?.Count; i++)
                {
                    FixedSignalConfigs.Add(new SendFixedSignalConfig
                    {
                        Signals = lis_signal,
                        SysteamName = SupportConfig.LisRegularSendSignals[i],
                    });
                }
            }
        }

        private void InitSignal() 
        {
            SignalConfigs.Clear();
            lis_signal.Clear();
            dic_messageconfig.Clear();
            FixedSignalConfigs.Clear();
            SendSignalConfigs.Clear();

            //for (int i = 0; i < SupportConfig.LisRegularSendSignals?.Count; i++)
            //{
            //    FixedSignalConfigs.Add(new SendFixedSignalConfig 
            //    {
            //         Signals = lis_signal,
            //         SysteamName = SupportConfig.LisRegularSendSignals[i],
            //    });
            //}

            //FixedSignals = lis_signal;
        }
        #endregion

            #region 保护方法
        protected override void WindowLoadedExecute(object obj)
        {
            if (fileid != GlobalModel.Test_DBCFileInfo.Id && GlobalModel.Test_DBCFileInfo.Id != 0)
            {
                fileid = GlobalModel.Test_DBCFileInfo.Id;

                if (!string.IsNullOrEmpty(GlobalModel.Test_DBCFileInfo.FileName))
                {
                    if (!Directory.Exists(SupportConfig.DBCFileDownPath))
                    {
                        Directory.CreateDirectory(SupportConfig.DBCFileDownPath);
                    }

                    string path = Path.Combine(SupportConfig.DBCFileDownPath, GlobalModel.Test_DBCFileInfo.FileName + GlobalModel.Test_DBCFileInfo.FileExtension);

                    File.WriteAllBytes(path, GlobalModel.Test_DBCFileInfo.FileContent);
                    //string path = Path.Combine(SupportConfig.DBCFileDownPath, DBCFileName + ".dbc");
                    LoadFile(GlobalModel.Test_DBCFileInfo.FileContent, GlobalModel.Test_DBCFileInfo.FileExtension);

                    AddFixedSignal();
                }
            }
            //base.WindowLoadedExecute(obj);
        }

        protected override void WindowClosedExecute(object obj)
        {
            base.WindowClosedExecute(obj);
        }
        #endregion


        #region 构造方法
        public DBCSendConfigViewModel() { }
        #endregion

    }

    /// <summary>
    /// 下发消息配置
    /// </summary>
    public class SendSignalConfig:ViewModelBase
    {
        private ObservableCollection<SignalConfig> signals = [];
        /// <summary>
        /// 配置信号
        /// </summary>
        public ObservableCollection<SignalConfig> Signals
        {
            get => signals;
            set => Set(nameof(Signals), ref signals, value);
        }

        private uint messageid;
        /// <summary>
        /// 消息ID
        /// </summary>
        public uint MessageID
        {
            get => messageid;
            set
            {
                if (Set(nameof(MessageID), ref messageid, value))
                {
                    if (MessageInfo != null)
                    {
                        MessageInfo.Message_ID = value;
                    }
                }
            }
        }


        private string messagename;
        /// <summary>
        /// 消息名称
        /// </summary>
        public string MessageName
        {
            get => messagename;
            set
            {
                if (Set(nameof(MessageName), ref messagename, value))
                {
                    if (MessageInfo != null)
                    {
                        MessageInfo.MessageName = value;
                    }
                }
            }
        }

        private uint cycletime;
        /// <summary>
        /// 消息发送周期（毫秒）
        /// </summary>
        public uint CycleTime
        {
            get => cycletime;
            set
            {
                if (Set(nameof(CycleTime), ref cycletime, value))
                {
                    if (MessageInfo != null)
                    {
                        MessageInfo.CycleTime = value;
                    }
                }
            }
        }

        private FrameFormat frameformat;
        /// <summary>
        /// 消息报文格式
        /// </summary>
        public FrameFormat FrameFormat
        {
            get => frameformat;
            set
            {
                if (Set(nameof(FrameFormat), ref frameformat, value))
                {
                    if (MessageInfo != null)
                    {
                        MessageInfo.FrameFormat = value;
                    }
                }
            }
        }

        /// <summary>
        /// 展示信息
        /// </summary>
        public string Info => $"{MessageID:X}({MessageName})";

        /// <summary>
        /// 消息配置信息
        /// </summary>
        public Test_DBCMessageInfo MessageInfo { get; set; } = new Test_DBCMessageInfo();

        private bool isselected = false;
        /// <summary>
        /// 是否被选中
        /// </summary>
        public bool IsSelected
        {
            get => isselected;
            set => Set(nameof(IsSelected), ref isselected, value);
        }

        /// <summary>
        /// 消息序号
        /// </summary>
        public int Index { get; set; }
    }

    /// <summary>
    /// 固定下发信号配置
    /// </summary>
    public class SendFixedSignalConfig : ViewModelBase
    {
        private ObservableCollection<SignalConfig> signals = [];
        /// <summary>
        /// 配置信号列表
        /// </summary>
        public ObservableCollection<SignalConfig> Signals
        {
            get => signals;
            set => Set(nameof(Signals), ref signals, value);
        }

        private SignalConfig signal;
        /// <summary>
        /// 当前选中信号配置
        /// </summary>
        public SignalConfig Signal
        {
            get => signal;
            set => Set(nameof(Signal), ref signal, value);
        }

        private int selectedsignalindex = -1;
        /// <summary>
        /// 当前选中信号序号
        /// </summary>
        public int SelectedSignalIndex
        {
            get => selectedsignalindex;
            set => Set(nameof(SelectedSignalIndex), ref selectedsignalindex, value);
        }

        private string selectedsignalvalue = string.Empty;
        /// <summary>
        /// 当前选中信号
        /// </summary>
        public string SelectedSignalValue
        {
            get => selectedsignalvalue;
            set => Set(nameof(SelectedSignalValue), ref selectedsignalvalue, value);
        }

        private string systeamname;
        /// <summary>
        /// 系统变量名称
        /// </summary>
        public string SysteamName
        {
            get => systeamname;
            set => Set(nameof(SysteamName), ref systeamname, value);
        }
    }
}
