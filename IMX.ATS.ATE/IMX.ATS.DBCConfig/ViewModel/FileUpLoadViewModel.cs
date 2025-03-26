#region << 版 本 注 释 >>
/*----------------------------------------------------------------
 * 版权所有 (c) 2025   保留所有权利。
 * CLR版本：4.0.30319.42000
 * 机器名称：LAPTOP-9Q9TTD5V
 * 公司名称：
 * 命名空间：IMX.ATS.DBCConfig.ViewModel
 * 唯一标识：6f58544f-8873-4dfa-8364-055fddb13c0a
 * 文件名：FileUpLoadViewModel
 * 当前用户域：LAPTOP-9Q9TTD5V
 * 
 * 创建者：58274
 * 创建时间：2025/3/5 17:47:57
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
using IMX.Logger;
using IMX.WPF.Resource;
using Piggy.VehicleBus.Common;
using Piggy.VehicleBus.MessageProcess;
using Super.Zoo.Framework;
using Super.Zoo.Framework.Logger;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media;
using MessageBox = System.Windows.Forms.MessageBox;

namespace IMX.ATS.DBCConfig
{
    public class FileUpLoadViewModel : WindowViewModelBaseEx
    {
        #region 公共属性

        #region 界面绑定属性

        #region 文件
        private string dbcfilepath = "";
        /// <summary>
        /// 文件地址
        /// </summary>
        public string DBCFilePath
        {
            get => dbcfilepath;
            set => Set(nameof(DBCFilePath), ref dbcfilepath, value);
        }

        private string dbcfilename = "";
        /// <summary>
        /// 文件名称
        /// </summary>
        public string DBCFileName
        {
            get => dbcfilename;
            set => Set(nameof(DBCFileName), ref dbcfilename, value);
        }

        private string dbcfiledescription = "";
        /// <summary>
        /// 文件说明
        /// </summary>
        public string DBCFileDescription
        {
            get => dbcfiledescription;
            set => Set(nameof(DBCFileDescription), ref dbcfiledescription, value);
        }
        #endregion

        private ObservableCollection<DBCMessageTreeNode> dbcMessages = new ObservableCollection<DBCMessageTreeNode>();
        /// <summary>
        /// DBC帧列表
        /// </summary>
        public ObservableCollection<DBCMessageTreeNode> DBCMessages
        {
            get => dbcMessages;
            set => Set(nameof(DBCMessages), ref dbcMessages, value);
        }

        private ObservableCollection<ViewLogger> logger = new ObservableCollection<ViewLogger>();
        /// <summary>
        /// 日志界面显示记录
        /// </summary>
        public ObservableCollection<ViewLogger> Logger
        {
            get => logger;
            set => Set(nameof(Logger), ref logger, value);
        }
        #endregion

        #region 界面绑定指令
        /// <summary>
        /// 文件选择指令
        /// </summary>
        public RelayCommand SelectFile => new RelayCommand(SelectedFile);

        /// <summary>
        /// 加载文件指令
        /// </summary>
        public RelayCommand LoadFile => new RelayCommand(LoadedFile);

        /// <summary>
        /// 文件上传指令
        /// </summary>
        public RelayCommand UploadFile => new(UploadedFile);


        public RelayCommand ClearLogger => new(() => { Logger.Clear(); });
        #endregion

        /// <summary>
        /// 窗口打开状态
        /// </summary>
        public bool IsOpen { get; set; } =  false;
        #endregion

        #region 私有变量
        /// <summary>
        /// 当前窗口
        /// </summary>
        private Window Win = null;

        /// <summary>
        /// 文件扩展名
        /// </summary>
        private string dbcfileextension = ".dbc";

        /// <summary>
        /// txt文件读取内容
        /// </summary>
        private Byte[] fileContent;

        /// <summary>
        /// 加载已文件标志位
        /// </summary>
        private bool IsLoadedFile = false;
        #endregion

        #region 私有方法
        /// <summary>
        /// 选择DBC文件
        /// </summary>
        private void SelectedFile()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Title = "请选择消息文件",
                Filter = "DBC文件|*.dbc",
            };

            if (openFileDialog.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            DBCFilePath = openFileDialog.FileName;
            DBCFileName = Path.GetFileNameWithoutExtension(DBCFilePath);
            dbcfileextension = Path.GetExtension(DBCFilePath);
            IsLoadedFile = false;
            //DBCFileName = openFileDialog.SafeFileName;
        }

        /// <summary>
        /// 加载DBC文件
        /// </summary>
        private void LoadedFile()
        {
            if (string.IsNullOrEmpty(DBCFilePath))
            {
                MessageBox.Show("请先选择文件");
                return;
            }

            fileContent = File.ReadAllBytes(DBCFilePath);
            OperateResult<IMessageFileLoader> rltCreate = MessageFileLoader.Create(dbcfileextension, SuperDHHLoggerManager.DeviceLogger)
                .ThenAnd(result => result.Data.Paser(fileContent).ConvertTo(result.Data))
                .AttachIfFailed(result => 
                {
                    Logger.Add(new ViewLogger 
                    {
                         RecordTime = DateTime.Now,
                         Level = LoggerLevel.ERROR,
                         Content = result.Message,
                    });

                    MessageBox.Show("DBC文件解析异常");
                })
                .AttachIfSucceed(result => 
                {
                    Logger.Add(new ViewLogger
                    {
                        RecordTime = DateTime.Now,
                        Level = LoggerLevel.INFO,
                        Content = "DBC文件加载成功",
                    });
                    Thread.Sleep(100);
                    if (!string.IsNullOrEmpty(result.Message))
                    {
                        Logger.Add(new ViewLogger
                        {
                            RecordTime = DateTime.Now,
                            Level = LoggerLevel.WARN,
                            Content = result.Message.Replace("\r\n", ";"),
                        });
                    }
                    IsLoadedFile = true;
                    LoadItems(result.Data);
                });
        }

        /// <summary>
        /// 上传DBC文件
        /// </summary>
        private void UploadedFile() 
        {
            if (string.IsNullOrEmpty(DBCFilePath) || string.IsNullOrEmpty(DBCFileName))
            {
                MessageBox.Show("请选择上传文件", "文件上传异常");
                return;
            }

            if (!IsLoadedFile)
            {
                MessageBox.Show("请先加载需上传文件,或确认文件已正常加载", "文件上传异常");
                return;
            }

            DBOperate.Default.InsertDBCFile(new DB.Model.Test_DBCFileInfo
                {
                    FileName = DBCFileName,
                    FileContent = fileContent,
                    FileSize = fileContent.Length,
                    FileExtension = dbcfileextension,
                    FileDescription = DBCFileDescription,
                    Operator = GlobalModel.UserInfo.UserName,
                })
                .AttachIfSucceed(result =>
                {
                    Logger.Add(new ViewLogger 
                    {
                        RecordTime = DateTime.Now,
                        Level = LoggerLevel.INFO,
                        Content = $"文件 [{DBCFileName}] 上传成功",
                    });
                    MessageBox.Show($"文件上传成功！", "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
                })
               .AttachIfFailed(result => 
               {
                   Logger.Add(new ViewLogger
                   {
                       RecordTime = DateTime.Now,
                       Level = LoggerLevel.ERROR,
                       Content = $"文件 [{DBCFileName}] 上传失败：{result.Message}",
                   });
                   MessageBox.Show($"文件上传失败！", "失败", MessageBoxButtons.OK, MessageBoxIcon.Error); 
               })
                .AttachIfExcepted(result => 
                {
                    Logger.Add(new ViewLogger
                    {
                        RecordTime = DateTime.Now,
                        Level = LoggerLevel.ERROR,
                        Content = $"文件 [{DBCFileName}] 上传异常：{result.Message}",
                    });
                    MessageBox.Show($"文件上传异常！", "异常", MessageBoxButtons.OK, MessageBoxIcon.Error); 
                });
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
            };
        }

        #endregion

        /// <summary>
        /// 拖拽事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ExecuteDrop(object sender, System.Windows.DragEventArgs e)
        {
            if (!e.Data.GetDataPresent(System.Windows.DataFormats.FileDrop))
            {
                return;
            }

            string[] files = (string[])e.Data.GetData(System.Windows.DataFormats.FileDrop);
            string extension = Path.GetExtension(files[0]);

            if (extension.ToUpper() != ".DBC")
            {
                MessageBox.Show("当前仅接收 .dbc 后缀文件，请确认文件格式", "文件异常");
                return;
            }

            DBCFilePath = files[0];
            DBCFileName = Path.GetFileNameWithoutExtension(DBCFilePath);
            dbcfileextension = extension;
        }
        #endregion

        #region 保护方法
        protected override void WindowLoadedExecute(object obj)
        {
            DBCFilePath = "";
            DBCFileName = "";
            DBCFileDescription = "";

            dbcfileextension = string.Empty;

            if (!(obj is Window win))
            {
                return;
            }

            WindowLeftDown_MoveEvent.LeftDown_MoveEventRegister(win);
            if (win != null) { Win = win; Win.Drop += ExecuteDrop; }
            IsOpen = true;

            //WindowMaxExecute(win);
        }

        protected override void WindowClosedExecute(object obj)
        {
            #region 保证再次开启MinLines属性生效
            DBCFilePath = "";
            DBCFileName = "";
            DBCFileDescription = "";
            #endregion

            DBCMessages.Clear();
            Logger.Clear();
            IsOpen = false;

            base.WindowClosedExecute(obj);
        }
        #endregion


        #region 构造方法
        public FileUpLoadViewModel() { }
        #endregion
    }

    /// <summary>
    /// 界面日志记录内容
    /// </summary>
    public class ViewLogger : ViewModelBase
    {
        private DateTime recordtime;
        /// <summary>
        /// 记录时间
        /// </summary>
        public DateTime RecordTime
        {
            get => recordtime;
            set => Set(nameof(RecordTime), ref recordtime, value);
        }

        private LoggerLevel level;
        /// <summary>
        /// 日志等级
        /// </summary>
        public LoggerLevel Level
        {
            get => level;
            set => Set(nameof(Level), ref level, value);
        }


        private string content;
        /// <summary>
        /// 日志内容
        /// </summary>
        public string Content
        {
            get => content;
            set => Set(nameof(Content), ref content, value);
        }

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
        /// 保存消息周期
        /// </summary>
        public uint CycleTime { get; set; }

        /// <summary>
        /// 保存消息格式
        /// </summary>
        public FrameFormat FrameFormat { get; set; }

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
        }
        #endregion
    }
}
