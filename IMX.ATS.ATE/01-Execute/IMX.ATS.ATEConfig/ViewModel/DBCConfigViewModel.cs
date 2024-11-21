#region << 版 本 注 释 >>
/*----------------------------------------------------------------
 * 版权所有 (c) 2024   保留所有权利。
 * CLR版本：4.0.30319.42000
 * 机器名称：LAPTOP-9Q9TTD5V
 * 公司名称：
 * 命名空间：IMX.ATS.ATEConfig.ViewModel
 * 唯一标识：5fcdf42b-83a5-4ec1-91d6-ee33d79ab2ab
 * 文件名：DBCConfigViewModel
 * 当前用户域：LAPTOP-9Q9TTD5V
 * 
 * 创建者：58274
 * 创建时间：2024/9/13 15:12:35
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

namespace IMX.ATS.ATEConfig
{
    public class DBCConfigViewModel : ExtendViewModelBase
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

        #region DBC信号配置
        private ObservableCollection<SignalConfigPage> signalconfigpages = new ObservableCollection<SignalConfigPage>();
        //{
        //    new SignalConfigPage
        //    {
        //        PageName = "上报信号"
        //    },
        //    new SignalConfigPage{ PageName = "下发信号"},
        //};
        /// <summary>
        /// 信号配置页面
        /// </summary>
        public ObservableCollection<SignalConfigPage> SignalConfigPages
        {
            get => signalconfigpages;
            set => Set(nameof(SignalConfigPages), ref signalconfigpages, value);
        }

        private SignalConfigPage selectedpage;
        /// <summary>
        /// 当前选中配置页面
        /// </summary>
        public SignalConfigPage SelectedPage
        {
            get => selectedpage;
            set => Set(nameof(SelectedPage), ref selectedpage, value);
        }

        #endregion

        private string dbcfilename;
        /// <summary>
        /// DBC文件名称
        /// </summary>
        public string DBCFileName
        {
            get => dbcfilename;
            set => Set(nameof(DBCFileName), ref dbcfilename, value);
        }

        #endregion

        #region 界面绑定指令

        /// <summary>
        /// DBC文件变更
        /// </summary>
        public RelayCommand<object> ChangeDBCFile => new RelayCommand<object>(ChangedDBCFile);

        ///// <summary>
        ///// 信号选中变更信息
        ///// </summary>
        //public RelayCommand SignalSelectChang => new RelayCommand(SignalSelectChanged);
        #endregion

        /// <summary>
        /// 项目ID
        /// </summary>
        public int ProjectID { get; set; } = -1;

        /// <summary>
        /// DBC文件是否发生编辑
        /// </summary>
        public bool IsEdit { get; set; } = true;

        public bool IsloadFile = false;
        #endregion

        #region 私有变量
        /// <summary>
        /// 是否为新建项目
        /// </summary>
        private bool isnew = false;


        /// <summary>
        /// 数据库DBC配置信息
        /// </summary>
        public Test_DBCConfig DBCConfig = null;

        /// <summary>
        /// 数据库DBC文件配置信息
        /// </summary>
        public Test_DBCFileInfo DBCFileInfo = null;

        /// <summary>
        /// 当前选中信号
        /// </summary>
        private DBCMessageTreeNode selectedsignal;

        /// <summary>
        /// 消息文件加载器
        /// </summary>
        private IMessageFileLoader messageFileLoader;

        private MainViewModel mainviewmodel;

        #endregion

        #region 私有方法
        private void ChangedDBCFile(object obj)
        {
            try
            {
                string viewstr = obj.ToString().ToUpper();

                if (viewstr == "UPLOAD")
                {
                    Application.Current.Dispatcher.Invoke(new Action(() =>
                    {
                        if (((ViewModelLocator)Application.Current.FindResource("Locator")).DBCFileUpload.IsOpen) { MessageBox.Show($"界面已打，请勿重复操作！", "界面提示", MessageBoxButton.OK, MessageBoxImage.Information); return; }

                        Window DBCfilewindow = ContentControlManager.GetWindow<DBCFileUploadView>(((ViewModelLocator)Application.Current.FindResource("Locator")).DBCFileUpload);
                        DBCfilewindow.Show();
                    }));
                }
                else if (viewstr == "CHANGE")
                {
                    var result = MessageBox.Show("是否切换当前项目DBC文件，若切换将自动清空当前配置信号", "DBC文件变更", MessageBoxButton.YesNo);

                    if (result == MessageBoxResult.No) { return; }

                    if (((ViewModelLocator)Application.Current.FindResource("Locator")).DBCFileChange.IsOpen) { MessageBox.Show($"界面已打，请勿重复操作！", "界面提示", MessageBoxButton.OK, MessageBoxImage.Information); return; }

                    Window mainwindow = ContentControlManager.GetWindow<DBCFileChangeView>(((ViewModelLocator)Application.Current.FindResource("Locator")).DBCFileChange);
                    mainwindow.Show();

                    ////清空上报配置
                    //SignalConfigPages[0].SignalConfigs.Clear();
                    ////清空下发配置
                    //SignalConfigPages[1].SignalConfigs.Clear();
                    ////清空信号列表
                    //DBCMessages.Clear();

                    ////重新导入DBC文件信号
                    //IsloadFile = true;
                    //重新加载DBC配置信号
                    //AddFixedSignal();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"变更DBC文件异常：{ex.Message}", "异常", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        #region DBC文件操作
        /// <summary>
        /// 加载DBC文件
        /// </summary>
        /// <param name="filepath">DBC文件地址</param>
        public void LoadFile(string filepath)
        {
            try
            {
                OperateResult<IMessageFileLoader> rltCreate = MessageFileLoader.Create(Path.GetExtension(filepath), SuperDHHLoggerManager.DeviceLogger);
                if (!rltCreate)
                {
                    MessageBox.Show($"DBC文件加载失败:{rltCreate.Message}", "DBC文件解析异常", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                messageFileLoader = rltCreate.Data;

                var rltLoad = rltCreate.Data.Paser(filepath);
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
        #endregion

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

        #region 信号操作

        /// <summary>
        /// 添加系统固定信号
        /// </summary>
        private void AddFixedSignal()
        {
            try
            {
                //if (!IsloadFile)//打开项目正常加载信号
                //{
                    SignalConfigPages[0].SignalConfigs.Clear();
                    DBCConfig.Test_DBCReceiveSignals.ForEach(signal =>
                    {
                        SignalConfigPages[0].SignalConfigs.Add(new DBCSignalConfig
                        {
                            Config = new SignalConfig
                            {
                                Signal_Name = signal.Signal_Name,
                                Message_ID = signal.Message_ID,
                                MessageName = DBCMessages.ToList().First(x => x.Tag == signal.Message_ID).Signals.ToList().First(y => y.Name == signal.Signal_Name).TagText,
                                Info = new Test_DBCInfo { Custom_Name = signal.Custom_Name, Signal_Name = signal.Signal_Name, Message_ID = signal.Message_ID }

                            },
                            IsRegularConfig = true,
                            AddSignal = new RelayCommand(SelectedSignal),
                            RemoveSignal = new RelayCommand(RevomeSignal)
                        });
                    });
                    SignalConfigPages[1].SignalConfigs.Clear();
                    DBCConfig.Test_DBCSendSignals.ForEach(signal =>
                    {
                        SignalConfigPages[1].SignalConfigs.Add(new DBCSignalConfig
                        {
                            Config = new SignalConfig
                            {
                                Signal_Name = signal.Signal_Name,
                                Message_ID = signal.Message_ID,
                                MessageName = DBCMessages.ToList().First(x => x.Tag == signal.Message_ID).Signals.ToList().First(y => y.Name == signal.Signal_Name).TagText,
                                Info = new Test_DBCInfo { Custom_Name = signal.Custom_Name, Signal_Name = signal.Signal_Name, Message_ID = signal.Message_ID }

                            },
                            IsRegularConfig = true,
                            AddSignal = new RelayCommand(SelectedSignal),
                            RemoveSignal = new RelayCommand(RevomeSignal)
                        });
                    });
                //}
                //else//新建项目或变更DBC文件加载格式
                //{
                //    SignalConfigPages[0].SignalConfigs.Clear();

                //    SignalConfigPages[0].SignalConfigs.Add(new DBCSignalConfig
                //    {
                //        Config = new SignalConfig
                //        {
                //            Info = new Test_DBCInfo { Custom_Name = "OBC电压" }

                //        },
                //        IsRegularConfig = true,
                //        AddSignal = new RelayCommand(SelectedSignal),
                //        RemoveSignal = new RelayCommand(RevomeSignal)
                //    });
                //    SignalConfigPages[0].SignalConfigs.Add(new DBCSignalConfig
                //    {
                //        Config = new SignalConfig
                //        {
                //            Info = new Test_DBCInfo { Custom_Name = "OBC电流" }
                //        },
                //        IsRegularConfig = true,
                //        AddSignal = new RelayCommand(SelectedSignal),
                //        RemoveSignal = new RelayCommand(RevomeSignal)
                //    });
                //    SignalConfigPages[0].SignalConfigs.Add(new DBCSignalConfig
                //    {
                //        Config = new SignalConfig
                //        {
                //            Info = new Test_DBCInfo { Custom_Name = "OBC工作状态" }
                //        },
                //        IsRegularConfig = true,
                //        AddSignal = new RelayCommand(SelectedSignal),
                //        RemoveSignal = new RelayCommand(RevomeSignal)
                //    });

                //}
            }
            catch (Exception ex)
            {
                MessageBox.Show($"添加系统固定信号异常：{ex.GetMessage()}", "异常", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
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
            if (SelectedPage == null)
            {
                MessageBox.Show($"请选择配置页面！", "提示", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }

            if (SelectedPage.SelectedSignalConfig == null)
            {
                MessageBox.Show($"请选择配置信号！", "提示", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }

            SelectedPage.SelectedSignalConfig.Config.MessageName = selectedsignal.TagText;
            SelectedPage.SelectedSignalConfig.Config.Signal_Name = selectedsignal.Name;
            SelectedPage.SelectedSignalConfig.Config.Message_ID = selectedsignal.Tag;
            SelectedPage.SelectedSignalConfig.Config.SignalValue = selectedsignal.InitValue.ToString();
        }

        /// <summary>
        /// 配置信号移除
        /// </summary>
        private void RevomeSignal()
        {
            if (SelectedPage == null)
            {
                MessageBox.Show($"请选择需要配置页面！", "提示", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }

            if (SelectedPage.SelectedSignalConfig == null)
            {
                MessageBox.Show($"请选择需要配置的信号！", "提示", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }

            SelectedPage.SelectedSignalConfig.Config.MessageName = string.Empty;
            SelectedPage.SelectedSignalConfig.Config.Signal_Name = string.Empty;
            SelectedPage.SelectedSignalConfig.Config.Message_ID = 0;
        }


        #endregion

        private void SignalConfigAdd()
        {
            if (SelectedPage == null)
            {
                MessageBox.Show($"请选择需要配置页面！", "提示", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }

            SelectedPage.SignalConfigs.Add(new DBCSignalConfig
            {
                Config = new SignalConfig
                {
                    Info = new Test_DBCInfo(),
                    CustomName = $"新建信号{SelectedPage.SignalConfigs.Count}",
                },
                AddSignal = new RelayCommand(SelectedSignal),
                RemoveSignal = new RelayCommand(RevomeSignal)
            });
        }

        private void SignalConfigDelete()
        {
            if (SelectedPage == null)
            {
                MessageBox.Show($"请选择需要配置页面！", "提示", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }

            if (SelectedPage.SelectedSignalConfig == null)
            {
                MessageBox.Show($"请选择需要配置的信号！", "提示", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }

            if (SelectedPage.SelectedSignalConfig.IsRegularConfig) { MessageBox.Show($"此信号为系统固定配置，无法删除！", "提示", MessageBoxButton.OK, MessageBoxImage.Exclamation); return; }
            SelectedPage.SignalConfigs.Remove(SelectedPage.SelectedSignalConfig);
        }

        #endregion
        /// <summary>
        /// 保存DBC信号配置信息
        /// </summary>
        private void SaveSignalConfig()
        {

            if (string.IsNullOrEmpty(DBCFileName))
            {
                MessageBox.Show("DBC文件名为空，请先选择DBC文件", "DBC信号配置保存失败", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }

            if (SelectedPage == null)
            {
                MessageBox.Show($"请选择需要配置的信号！", "提示", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }

            try
            {
                var repeatsn = SelectedPage?.SignalConfigs.ToList()
                .GroupBy(x => x.Config.CustomName)
                .Where(x => x.Count() > 1)
                .Select(x => x.Key)
                .ToList();

                if (repeatsn.Count > 0)
                {
                    StringBuilder stringBuilder = new StringBuilder();
                    stringBuilder.AppendLine("自定义信号名称");
                    for (int i = 0; i < repeatsn.Count; i++)
                    {
                        stringBuilder.AppendLine(repeatsn[i]);
                    }
                    stringBuilder.AppendLine("存在重复");
                    MessageBox.Show(stringBuilder.ToString(), "DBC配置保存异常");
                    return;
                }
            }
            catch (Exception ex)
            {
                SuperDHHLoggerManager.Exception( LoggerType.FROMLOG, nameof(DBCConfigViewModel), nameof(SaveSignalConfig), ex);
            }

            try
            {
                if (SelectedPage?.PageName == "上报信号配置")
                {
                    DBCConfig.Test_DBCReceiveSignals = new List<Test_DBCInfo>();
                    for (int i = 0; i < SelectedPage?.SignalConfigs?.Count; i++)
                    {
                        var config = SelectedPage.SignalConfigs[i];
                        DBCConfig.Test_DBCReceiveSignals.Add(config.Config.Info);
                    }
                }
                else if (SelectedPage?.PageName == "下发信号配置")
                {
                    DBCConfig.Test_DBCSendSignals = new List<Test_DBCInfo>();
                    for (int i = 0; i < SelectedPage?.SignalConfigs?.Count; i++)
                    {
                        var config = SelectedPage.SignalConfigs[i];
                        DBCConfig.Test_DBCSendSignals.Add(config.Config.Info);
                    }
                }

                DBCConfig.UpdateOperator = GlobalModel.UserInfo.UserName;

                if (isnew)
                {
                    DBOperate.Default.InsertDBCConfig(DBCConfig).AttachIfSucceed(result =>
                    {
                        MessageBox.Show($"DBC{SelectedPage?.PageName}保存成功！");
                    }).AttachIfFailed(result =>
                    {
                        MessageBox.Show($"{SelectedPage?.PageName}保存失败：{result.Message}", "信号保存失败", MessageBoxButton.OK, MessageBoxImage.Question);
                    });
                    isnew = false;
                }
                else
                {
                    DBOperate.Default.UpdateDBCConfig(DBCConfig).AttachIfSucceed(result =>
                    {
                        MessageBox.Show($"DBC{SelectedPage?.PageName}保存成功！");
                    }).AttachIfFailed(result =>
                    {
                        MessageBox.Show($"{SelectedPage?.PageName}保存失败：{result.Message}", "信号保存失败", MessageBoxButton.OK, MessageBoxImage.Question);
                    });
                }


            }
            catch (Exception ex)
            {
                MessageBox.Show($"保存异常：{ex.Message}", "信号保存异常", MessageBoxButton.OK, MessageBoxImage.Question);
            }

        }

        #endregion

        #region 保护方法
        protected override void WindowLoadedExecute(object obj)
        {
            //mainviewmodel = ((ViewModelLocator)Application.Current.FindResource("Locator")).Main;
            DBCConfig = GlobalModel.TestDBCconfig;
            DBCFileInfo = GlobalModel.TestDBCFileInfo;

            DBCFileName = DBCFileInfo?.FileName ?? string.Empty;


            //DBOperate.Default.GetDBCConfig_ByProjectID(mainviewmodel.ProjectInfo.Id).AttachIfSucceed(result =>
            //{
            //    if (result.Data == null)
            //    {
            //        isnew = true;
            //    }
            //});
#if DEBUG
            //DBCFileName = "BEV_E0X_OT_Car RMCU V3.72 Draft_202311220825";
            //string path = Path.Combine(@"C:\Users\Administrator\Desktop", DBCFileName + ".dbc");
            //LoadFile(path);
#endif
            if (IsEdit)
            {
                if (!GlobalModel.IsNewProject && DBCFileInfo.Id != 0)
                {
                    DBCFileName = DBCFileInfo.FileName;

                    if (!string.IsNullOrEmpty(DBCFileName))
                    {
                        if (!Directory.Exists(SupportConfig.DBCFileDownPath))
                        {
                            Directory.CreateDirectory(SupportConfig.DBCFileDownPath);
                        }

                        string path = Path.Combine(SupportConfig.DBCFileDownPath, DBCFileInfo.FileName + DBCFileInfo.FileExtension);

                        File.WriteAllBytes(path, DBCFileInfo.FileContent);
                        //string path = Path.Combine(SupportConfig.DBCFileDownPath, DBCFileName + ".dbc");
                        LoadFile(path);

                        AddFixedSignal();
                    }

                    IsEdit = false;
                }
            }

            
            base.WindowLoadedExecute(obj);
        }

        protected override void WindowClosedExecute(object obj)
        {
            base.WindowClosedExecute(obj);
        }
        #endregion

        #region 构造方法
        public DBCConfigViewModel()
        {
            SignalConfigPages.Add(new SignalConfigPage
            {
                PageName = "上报信号配置",
                AddNewConfig = new RelayCommand(SignalConfigAdd),
                DeleteConfig = new RelayCommand(SignalConfigDelete),
                SaveConfig = new RelayCommand(SaveSignalConfig),
            });

            SignalConfigPages.Add(new SignalConfigPage
            {
                PageName = "下发信号配置",
                AddNewConfig = new RelayCommand(SignalConfigAdd),
                DeleteConfig = new RelayCommand(SignalConfigDelete),
                SaveConfig = new RelayCommand(SaveSignalConfig),
            });
        }


        #endregion

    }

    public class DBCMessageTreeNode : ViewModelBase
    {
        private string name;
        /// <summary>
        /// 名称
        /// </summary>
        public string Name
        {
            get => name;
            set => Set(nameof(Name), ref name, value);
        }

        /// <summary>
        /// 保存消息ID信息
        /// </summary>
        public uint Tag { get; set; }

        /// <summary>
        /// 保存消息名称信息
        /// </summary>
        public string TagText { get; set; }

        /// <summary>
        /// 信号初始值
        /// </summary>
        public double InitValue { get; set; }

        private string toolTipText;
        /// <summary>
        /// 提示信息
        /// </summary>
        public string ToolTipText
        {
            get => toolTipText;
            set => Set(nameof(ToolTipText), ref toolTipText, value);
        }

        private SolidColorBrush backColor;
        public SolidColorBrush BackColor
        {
            get => backColor;
            set => Set(nameof(BackColor), ref backColor, value);
        }

        private ObservableCollection<DBCMessageTreeNode> signals;

        public ObservableCollection<DBCMessageTreeNode> Signals
        {
            get => signals;
            set => Set(nameof(Signals), ref signals, value);
        }

        private bool isselected;
        /// <summary>
        /// 是否为选中信号
        /// </summary>
        public bool IsSelected
        {
            get => isselected;
            set
            {
                if (Set(nameof(IsSelected), ref isselected, value))
                {
                    if (value)
                    {
                        SelectChange?.Execute(GetSelectedSignal());
                    }
                    else
                    {
                        SelectChange?.Execute(null);
                    }
                }
            }
        }

        /// <summary>
        /// 选中提示信息
        /// </summary>
        public string Info => $"{Name} - {TagText}({Tag})";

        public RelayCommand SelectSignal { get; set; }

        public RelayCommand<object> SelectChange { get; set; }

        #region 公共方法
        /// <summary>
        /// 获取选中信号
        /// </summary>
        /// <returns>信号信息</returns>
        private DBCMessageTreeNode GetSelectedSignal()
        {
            if (Signals == null || Signals?.Count < 1)
            {
                return this;
            }

            return null;

            //var signal = signals.ToList().Find(x => IsSelected);
            //if (signal == null)
            //{
            //    return string.Empty;
            //}

            //return signal.Info;
        }
        #endregion
    }

    /// <summary>
    /// 信号配置页面模型
    /// </summary>
    public class SignalConfigPage : ViewModelBase
    {
        private string pagename;
        /// <summary>
        /// 页面标签名称
        /// </summary>
        public string PageName
        {
            get => pagename;
            set => Set(nameof(PageName), ref pagename, value);
        }

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

        /// <summary>
        /// 新增配置
        /// </summary>
        public RelayCommand AddNewConfig { get; set; }

        /// <summary>
        /// 保存配置
        /// </summary>
        public RelayCommand SaveConfig { get; set; }

        /// <summary>
        /// 删除配置
        /// </summary>
        public RelayCommand DeleteConfig { get; set; }
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
        /// 添加信号配置
        /// </summary>
        public RelayCommand AddSignal { get; set; }

        /// <summary>
        /// 清除当前配置
        /// </summary>
        public RelayCommand RemoveSignal { get; set; }

        //private string messagename;
        ///// <summary>
        ///// 帧名
        ///// </summary>
        //public string MessageName 
        //{
        //    get => messagename;
        //    set => Set(nameof(MessageName), ref messagename, value);
        //}

        //private uint messageid;
        ///// <summary>
        ///// 帧ID
        ///// </summary>
        //public uint Message_ID
        //{
        //    get => messageid;
        //    set
        //    {
        //        if (Set(nameof(Message_ID), ref messageid, value))
        //        {
        //            if (Info != null)
        //            {
        //                Info.Message_ID = value;
        //            }
        //        }
        //    }
        //}

        //private string signalname;
        ///// <summary>
        ///// 信号名
        ///// </summary>
        //public string Signal_Name
        //{
        //    get => signalname;
        //    set 
        //    {
        //        if (Set(nameof(Signal_Name), ref signalname, value))
        //        {
        //            if (Info!=null)
        //            {
        //                Info.Signal_Name = value;
        //            }
        //        }
        //    }
        //}

        ///// <summary>
        ///// 数据库配置信息
        ///// </summary>
        //public Test_DBCInfo Info { get; set; }

        //private string signalvalue = "NULL";
        ///// <summary>
        ///// 信号值
        ///// </summary>
        //public string SignalValue
        //{
        //    get => signalvalue;
        //    set => Set(nameof(SignalValue), ref signalvalue, value);
        //}
    }
}
