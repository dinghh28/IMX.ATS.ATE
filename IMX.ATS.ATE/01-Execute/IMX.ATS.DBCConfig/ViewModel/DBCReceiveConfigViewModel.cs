#region << 版 本 注 释 >>
/*----------------------------------------------------------------
 * 版权所有 (c) 2025   保留所有权利。
 * CLR版本：4.0.30319.42000
 * 机器名称：LAPTOP-9Q9TTD5V
 * 公司名称：
 * 命名空间：IMX.ATS.DBCConfig.ViewModel
 * 唯一标识：d0f3c873-e5ec-4e8c-b40c-4784d60138b4
 * 文件名：DBCReceiveConfigViewModel
 * 当前用户域：LAPTOP-9Q9TTD5V
 * 
 * 创建者：58274
 * 创建时间：2025/3/10 16:56:09
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
using IMX.ATE.Common;
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
using System.Windows.Media;

namespace IMX.ATS.DBCConfig
{
    /// <summary>
    /// DBC上报信号配置模型类
    /// </summary>
    public class DBCReceiveConfigViewModel : ExtendViewModelBase
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


        private DBCSignalConfig selectedsignalconfig;
        /// <summary>
        /// 当前选中配置信号
        /// </summary>
        public DBCSignalConfig SelectedSignalConfig
        {
            get => selectedsignalconfig;
            set => Set(nameof(SelectedSignalConfig), ref selectedsignalconfig, value);
        }

        #endregion

        #region 界面绑定指令

        /// <summary>
        /// 保存配置
        /// </summary>
        public RelayCommand SaveConfig => new RelayCommand(SaveSignalConfig);

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

        private Electricity electricity;

        private int fileid = -1;
        #endregion

        #region 私有方法

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
                    MessageBox.Show($"DBC文件加载失败:{rltCreate.Message}", "DBC文件解析异常", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                messageFileLoader = rltCreate.Data;

                var rltLoad = rltCreate.Data.Paser(fileData);
                if (!rltLoad)
                {
                    MessageBox.Show($"DBC解析失败:{rltLoad.Message}", "DBC文件解析异常", MessageBoxButton.OK, MessageBoxImage.Error);
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
                MessageBox.Show($"DBC解析异常:{ex.Message}", "DBC文件解析异常", MessageBoxButton.OK, MessageBoxImage.Error);
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
        private DBCMessageTreeNode CreateMessageNode(Message message)
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

        /// <summary>
        /// 保存DBC信号配置信息
        /// </summary>
        private void SaveSignalConfig() 
        {
            try
            {
                List<Test_DBCInfo> dbcconfigs = new List<Test_DBCInfo> ();

                for (int i = 0; i < SignalConfigs?.Count; i++)
                {
                    var signalConfig = SignalConfigs[i];
                    if (!signalConfig.IsConfiged)
                    {
                        //MessageBox.Show($"【{signalConfig.Config.CustomName}】\r\n未配置信号，请完成配置后再保存","上报信号保存失败");
                        //return;
                        continue;
                    }

                    var config = signalConfig.Config;

                    dbcconfigs.Add(new Test_DBCInfo
                    {
                          Custom_Name = config.CustomName,
                          MessageName = config.MessageName,
                          Message_ID = config.Message_ID,
                          Signal_Name = config.Signal_Name,
                          SignalInitValue = config.SignalValue,
                    });
                }

                DBOperate.Default.UpdateReceiveSignals(GlobalModel.Test_DBC.Id, dbcconfigs)
                    .AttachIfSucceed(result => MessageBox.Show("上报信号保存成功!"))
                    .AttachIfFailed(result => MessageBox.Show("上报信号保存失败!"));
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{ex.GetMessage()}","上报信号保存异常");
                SuperDHHLoggerManager.Exception( LoggerType.FROMLOG, nameof(DBCReceiveConfigViewModel), nameof(SaveSignalConfig), ex);
            }
        }

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
            if (SelectedSignalConfig == null)
            {
                MessageBox.Show($"请选择需要配置上报信号！", "提示", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }

            if (selectedsignal == null)
            {
                MessageBox.Show($"请选择配置信号！", "提示", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }

            SelectedSignalConfig.Config.MessageName = selectedsignal.TagText;
            SelectedSignalConfig.Config.Signal_Name = selectedsignal.Name;
            SelectedSignalConfig.Config.Message_ID = selectedsignal.Tag;
            SelectedSignalConfig.Config.SignalValue = selectedsignal.InitValue.ToString();
            SelectedSignalConfig.IsConfiged = true;
        }

        /// <summary>
        /// 配置信号移除
        /// </summary>
        private void RevomeSignal()
        {
            if (SelectedSignalConfig == null)
            {
                MessageBox.Show($"请选择需要配置的信号！", "提示", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }

            SelectedSignalConfig.Config.MessageName = string.Empty;
            SelectedSignalConfig.Config.Signal_Name = string.Empty;
            SelectedSignalConfig.Config.Message_ID = 0;
            SelectedSignalConfig.IsConfiged = false;
        }
        #endregion

        /// <summary>
        /// 添加系统固定信号
        /// </summary>
        private void AddFixedSignal() 
        {
            SignalConfigs.Clear();

            if (!GlobalModel.IsNew && GlobalModel.Test_DBC != null && GlobalModel.Test_DBC.Test_DBCReceiveSignals.Count > 0)
            {
                SupportConfig.LisRegularSignals.ForEach(signal =>
                {

                    var config = new DBCSignalConfig
                    {
                        Config = new SignalConfig
                        {
                            Info = new Test_DBCInfo { Custom_Name = signal, IsRegular = true }
                        },
                        IsRegularConfig = true,
                        AddSignal = new RelayCommand(SelectedSignal),
                        RemoveSignal = new RelayCommand(RevomeSignal)
                    };

                    var info = GlobalModel.Test_DBC.Test_DBCReceiveSignals.Find(x => x.Custom_Name == signal);
                    if (info != null)
                    {
                        config.Config.Info.Signal_Name = info.Signal_Name;
                        config.Config.Info.Message_ID = info.Message_ID;
                        config.Config.Signal_Name = info.Signal_Name;
                        config.Config.Message_ID = info.Message_ID;
                        config.Config.MessageName = info.MessageName;
                        config.IsConfiged = true;
                    }

                    SignalConfigs.Add(config);
                });

                if (GlobalModel.Test_DBC.Electricity == Electricity.SingleANDInversion)
                {
                    SupportConfig.LisRegularSignals_In.ForEach(signal =>
                    {
                        var config = new DBCSignalConfig
                        {
                            Config = new SignalConfig
                            {
                                Info = new Test_DBCInfo { Custom_Name = signal, IsRegular = true }
                            },
                            IsRegularConfig = true,
                            AddSignal = new RelayCommand(SelectedSignal),
                            RemoveSignal = new RelayCommand(RevomeSignal)
                        };

                        var info = GlobalModel.Test_DBC.Test_DBCReceiveSignals.Find(x => x.Custom_Name == signal);
                        if (info != null)
                        {
                            config.Config.Info.Signal_Name = info.Signal_Name;
                            config.Config.Info.Message_ID = info.Message_ID;
                            config.Config.Signal_Name = info.Signal_Name;
                            config.Config.Message_ID = info.Message_ID;
                            config.Config.MessageName = info.MessageName;
                        }
                        SignalConfigs.Add(config);
                    });
                }
                else if (GlobalModel.Test_DBC.Electricity == Electricity.Three)
                {
                    SupportConfig.LisRegularSignals_Three.ForEach(signal =>
                    {
                        var config = new DBCSignalConfig
                        {
                            Config = new SignalConfig
                            {
                                Info = new Test_DBCInfo { Custom_Name = signal, IsRegular = true }
                            },
                            IsRegularConfig = true,
                            AddSignal = new RelayCommand(SelectedSignal),
                            RemoveSignal = new RelayCommand(RevomeSignal)
                        };
                        var info = GlobalModel.Test_DBC.Test_DBCReceiveSignals.Find(x => x.Custom_Name == signal);
                        if (info != null)
                        {
                            config.Config.Info.Signal_Name = info.Signal_Name;
                            config.Config.Info.Message_ID = info.Message_ID;
                            config.Config.Signal_Name = info.Signal_Name;
                            config.Config.Message_ID = info.Message_ID;
                            config.Config.MessageName = info.MessageName;
                        }
                        SignalConfigs.Add(config);
                    });
                }
                else if (GlobalModel.Test_DBC.Electricity == Electricity.ThreeANDInversion)
                {
                    SupportConfig.LisRegularSignals_Three.ForEach(signal =>
                    {
                        var config = new DBCSignalConfig
                        {
                            Config = new SignalConfig
                            {
                                Info = new Test_DBCInfo { Custom_Name = signal, IsRegular = true }
                            },
                            IsRegularConfig = true,
                            AddSignal = new RelayCommand(SelectedSignal),
                            RemoveSignal = new RelayCommand(RevomeSignal)
                        };

                        var info = GlobalModel.Test_DBC.Test_DBCReceiveSignals.Find(x => x.Custom_Name == signal);
                        if (info != null)
                        {
                            config.Config.Info.Signal_Name = info.Signal_Name;
                            config.Config.Info.Message_ID = info.Message_ID;
                            config.Config.Signal_Name = info.Signal_Name;
                            config.Config.Message_ID = info.Message_ID;
                            config.Config.MessageName = info.MessageName;
                        }
                        SignalConfigs.Add(config);
                    });

                    SupportConfig.LisRegularSignals_ThreeIn.ForEach(signal =>
                    {
                        var config = new DBCSignalConfig
                        {
                            Config = new SignalConfig
                            {
                                Info = new Test_DBCInfo { Custom_Name = signal, IsRegular = true }
                            },
                            IsRegularConfig = true,
                            AddSignal = new RelayCommand(SelectedSignal),
                            RemoveSignal = new RelayCommand(RevomeSignal)
                        };
                        var info = GlobalModel.Test_DBC.Test_DBCReceiveSignals.Find(x => x.Custom_Name == signal);
                        if (info != null)
                        {
                            config.Config.Info.Signal_Name = info.Signal_Name;
                            config.Config.Info.Message_ID = info.Message_ID;
                            config.Config.Signal_Name = info.Signal_Name;
                            config.Config.Message_ID = info.Message_ID;
                            config.Config.MessageName = info.MessageName;
                        }
                        SignalConfigs.Add(config);
                    });
                }
            }
            else
            {
                SupportConfig.LisRegularSignals.ForEach(signal =>
                {
                    var config = new DBCSignalConfig
                    {
                        Config = new SignalConfig
                        {
                            Info = new Test_DBCInfo { Custom_Name = signal, IsRegular = true }
                        },
                        IsRegularConfig = true,
                        AddSignal = new RelayCommand(SelectedSignal),
                        RemoveSignal = new RelayCommand(RevomeSignal)
                    };

                    SignalConfigs.Add(config);
                });

                if (GlobalModel.Test_DBC.Electricity == Electricity.SingleANDInversion)
                {
                    SupportConfig.LisRegularSignals_In.ForEach(signal => 
                    {
                        var config = new DBCSignalConfig
                        {
                            Config = new SignalConfig
                            {
                                Info = new Test_DBCInfo { Custom_Name = signal, IsRegular = true }
                            },
                            IsRegularConfig = true,
                            AddSignal = new RelayCommand(SelectedSignal),
                            RemoveSignal = new RelayCommand(RevomeSignal)
                        };

                        SignalConfigs.Add(config);
                    });
                }
                else if (GlobalModel.Test_DBC.Electricity == Electricity.Three)
                {
                    SupportConfig.LisRegularSignals_Three.ForEach(signal =>
                    {
                        var config = new DBCSignalConfig
                        {
                            Config = new SignalConfig
                            {
                                Info = new Test_DBCInfo { Custom_Name = signal, IsRegular = true }
                            },
                            IsRegularConfig = true,
                            AddSignal = new RelayCommand(SelectedSignal),
                            RemoveSignal = new RelayCommand(RevomeSignal)
                        };

                        SignalConfigs.Add(config);
                    });
                }
                else if (GlobalModel.Test_DBC.Electricity == Electricity.ThreeANDInversion)
                {
                    SupportConfig.LisRegularSignals_Three.ForEach(signal =>
                    {
                        var config = new DBCSignalConfig
                        {
                            Config = new SignalConfig
                            {
                                Info = new Test_DBCInfo { Custom_Name = signal, IsRegular = true }
                            },
                            IsRegularConfig = true,
                            AddSignal = new RelayCommand(SelectedSignal),
                            RemoveSignal = new RelayCommand(RevomeSignal)
                        };

                        SignalConfigs.Add(config);
                    });

                    SupportConfig.LisRegularSignals_ThreeIn.ForEach(signal =>
                    {
                        var config = new DBCSignalConfig
                        {
                            Config = new SignalConfig
                            {
                                Info = new Test_DBCInfo { Custom_Name = signal, IsRegular = true }
                            },
                            IsRegularConfig = true,
                            AddSignal = new RelayCommand(SelectedSignal),
                            RemoveSignal = new RelayCommand(RevomeSignal)
                        };

                        SignalConfigs.Add(config);
                    });
                }
            }
        }
        #endregion

        #region 保护方法
        protected override void WindowLoadedExecute(object obj)
        {
            //if (GlobalModel.Test_DBC.EnableUse)
            //{

            if (fileid == GlobalModel.Test_DBCFileInfo.Id 
                && electricity == GlobalModel.Test_DBC.Electricity 
                && GlobalModel.Test_DBC.EnableUse)
            {
                return;
            }

            electricity = GlobalModel.Test_DBC.Electricity;
            fileid = GlobalModel.Test_DBCFileInfo.Id;

            //}
            //if (!GlobalModel.Test_DBC.EnableUseReceive && GlobalModel.Test_DBC.EnableUse)
            //{
                //fileid = GlobalModel.Test_DBCFileInfo.Id;

                //if (GlobalModel.Test_DBC.EnableUse)
                //{
                    //if (!Directory.Exists(SupportConfig.DBCFileDownPath))
                    //{
                    //    Directory.CreateDirectory(SupportConfig.DBCFileDownPath);
                    //}

                    //string path = Path.Combine(SupportConfig.DBCFileDownPath, GlobalModel.Test_DBCFileInfo.FileName + GlobalModel.Test_DBCFileInfo.FileExtension);

                    //File.WriteAllBytes(path, GlobalModel.Test_DBCFileInfo.FileContent);
                    //string path = Path.Combine(SupportConfig.DBCFileDownPath, DBCFileName + ".dbc");
                    LoadFile(GlobalModel.Test_DBCFileInfo.FileContent, GlobalModel.Test_DBCFileInfo.FileExtension);

                    AddFixedSignal();
                //}
            //}
            //DBCConfig = GlobalModel.TestDBCconfig;
            //DBCFileInfo = GlobalModel.TestDBCFileInfo;
            //base.WindowLoadedExecute(obj);
        }

        protected override void WindowClosedExecute(object obj)
        {
            base.WindowClosedExecute(obj);
        }
        #endregion


        #region 构造方法
        public DBCReceiveConfigViewModel() 
        {
            electricity = GlobalModel.Test_DBC.Electricity == Electricity.Single ? Electricity.Three : Electricity.Single;
        }
        #endregion
    }

    public class SignalConfig : ViewModelBase
    {
        private string messagename;
        /// <summary>
        /// 帧名
        /// </summary>
        public string MessageName
        {
            get => messagename;
            set
            {
                if (Set(nameof(MessageName), ref messagename, value))
                {
                    if (Info != null)
                    {
                        Info.MessageName = value;
                    }
                }
            }
        }

        private uint messageid;
        /// <summary>
        /// 帧ID
        /// </summary>
        public uint Message_ID
        {
            get => messageid;
            set
            {
                if (Set(nameof(Message_ID), ref messageid, value))
                {
                    if (Info != null)
                    {
                        Info.Message_ID = value;
                    }
                }
            }
        }

        private string signalname;
        /// <summary>
        /// 信号名
        /// </summary>
        public string Signal_Name
        {
            get => signalname;
            set
            {
                if (Set(nameof(Signal_Name), ref signalname, value))
                {
                    if (Info != null)
                    {
                        Info.Signal_Name = value;
                    }
                }
            }
        }

        private string customname;
        /// <summary>
        /// 用户自定义名称
        /// </summary>
        public string CustomName
        {
            get => Info?.Custom_Name ?? customname;
            set
            {
                if (Set(nameof(CustomName), ref customname, value))
                {
                    if (Info != null)
                    {
                        Info.Custom_Name = value;
                    }
                }
            }
        }
        /// <summary>
        /// 数据库配置信息
        /// </summary>
        public Test_DBCInfo Info { get; set; }

        private string signalvalue = "NULL";
        /// <summary>
        /// 信号值
        /// </summary>
        public string SignalValue
        {
            get => signalvalue;
            set
            {
                if (Set(nameof(SignalValue), ref signalvalue, value))
                {
                    if (Info != null)
                    {
                        Info.SignalInitValue = value;
                    }
                }
            }
        }

        private bool isselected = false;
        /// <summary>
        /// 是否被选中
        /// </summary>
        public bool IsSelected
        {
            get => isselected;
            set => Set(nameof(IsSelected), ref isselected, value);
        }

        public int Index { get; set; }
    }

    public class DBCSignalConfig : ViewModelBase
    {
        private SignalConfig config;
        /// <summary>
        /// 信号配置
        /// </summary>
        public SignalConfig Config
        {
            get => config;
            set => Set(nameof(Config), ref config, value);
        }

        /// <summary>
        /// 是否为系统固定配置
        /// </summary>
        public bool IsRegularConfig { get; set; } = false;

        /// <summary>
        /// 已完成配置
        /// </summary>
        public bool IsConfiged { get; set; } = false;

        /// <summary>
        /// 添加信号配置
        /// </summary>
        public RelayCommand AddSignal { get; set; }

        /// <summary>
        /// 清除当前配置
        /// </summary>
        public RelayCommand RemoveSignal { get; set; }
    }
}
