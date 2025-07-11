using GalaSoft.MvvmLight.Command;
using H.WPF.Framework;
using IMX.ATE.Framework;
using IMX.DB;
using IMX.DB.Model;
using IMX.Device.Common;
using IMX.WPF.Resource;
using Piggy.VehicleBus.Common;
using Super.Zoo.Framework;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace IMX.ATS.Manual
{
    public class SelectDBCViewModel : ExtendViewModelBase
    {

        #region 公共属性

        #region 界面绑定属性
        private string searchstr;
        /// <summary>
        /// 检索字符串
        /// </summary>
        public string SearchStr
        {
            get => searchstr;
            set => Set(nameof(SearchStr), ref searchstr, value);
        }

        private ObservableCollection<DBCConfigInfo> searchdbcconfiginfos = new ObservableCollection<DBCConfigInfo>();
        /// <summary>
        /// DBC配置列表
        /// </summary>
        public ObservableCollection<DBCConfigInfo> SearchDBCConfigInfos
        {
            get => searchdbcconfiginfos;
            set => Set(nameof(SearchDBCConfigInfos), ref searchdbcconfiginfos, value);
        }

        private int selectedindex;
        /// <summary>
        /// 当前选中配置序号
        /// </summary>
        public int SelectedIndex
        {
            get => selectedindex;
            set
            {
                if (Set(nameof(SelectedIndex), ref selectedindex, value))
                {
                    if (value == -1)
                    {
                        SelectedInfo = null;
                        return;
                    }
                    SelectedInfo = SearchDBCConfigInfos[value];
                }
            }
        }

        private DBCConfigInfo selectedinfo;
        /// <summary>
        /// 当前选中配置
        /// </summary>
        public DBCConfigInfo SelectedInfo
        {
            get => selectedinfo;
            set => Set(nameof(SelectedInfo), ref selectedinfo, value);
        }

        #endregion

        #region 界面绑定指令
        /// <summary>
        /// 配置信息检索指令
        /// </summary>
        public RelayCommand Search => new RelayCommand(() =>
        {
            SearchDBCConfigInfos.Clear();
            SelectedIndex = -1;
            List<DBCConfigInfo> data = new List<DBCConfigInfo>();

            if (string.IsNullOrEmpty(SearchStr))
            {
                data = LsDBCConfigInfos;
            }
            else
            {
                data = LsDBCConfigInfos.FindAll(x => x.Config.ConfigName.Contains(SearchStr));
            }

            for (int i = 0; i < data.Count; i++)
            {
                SearchDBCConfigInfos.Add(data[i]);
            }
        });

        /// <summary>
        /// 提交变更
        /// </summary>
        public RelayCommand Submit => new RelayCommand(SubmitChange);
        #endregion

        /// <summary>
        /// 当前界面打开状态
        /// </summary>
        public bool IsOpen = false;
        #endregion

        #region 私有变量
        /// <summary>
        /// 当前窗口
        /// </summary>
        private Window Win = null;

        /// <summary>
        /// 数据库包含所有配置信息
        /// </summary>
        private List<DBCConfigInfo> LsDBCConfigInfos = new List<DBCConfigInfo>();
        #endregion

        #region 私有方法 
        private void SubmitChange()
        {
            if (SearchDBCConfigInfos.Count == 0)
            {
                MessageBox.Show("库中不存在DBC配置，无法进行变更操作！请前去DBC配置模块完成配置后再进行操作", "变更提交异常");
                return;
            }

            if (SelectedIndex == -1)
            {
                MessageBox.Show("请先选择DBC配置", "变更提交异常");
                return;
            }
            try
            {
                DBOperate.Default.GetFile_ByID(SearchDBCConfigInfos[SelectedIndex].Config.DBCFileID).AttachIfSucceed(result =>
                {
                    SearchDBCConfigInfos[SelectedIndex].FileInfo = result.Data;
                })
                    .AttachIfFailed(resuly =>
                    {
                        MessageBox.Show("获取DBC文件信息失败", "变更提交异常"); 
                        return;
                    });

                GlobalModel.CANThread.TestDBCconfig = SearchDBCConfigInfos[SelectedIndex].Config;
                GlobalModel.CANThread.TestDBCFile = SearchDBCConfigInfos[SelectedIndex].FileInfo;

                List<CANDataInfo> lisreadsignals = new List<CANDataInfo>();
                Dictionary<CANDataInfo, double> dicsendsignal = new Dictionary<CANDataInfo, double>();
                List<CANDataInfo> lissendsignals = new List<CANDataInfo>();
                //Dictionary<uint, uint> dicsendmessages = new Dictionary<uint, uint>();
                Dictionary<uint, (uint, FrameFormat)> dicsendmessages = new Dictionary<uint, (uint, FrameFormat)>();

                for (int i = 0; i < SearchDBCConfigInfos[SelectedIndex].Config?.Test_DBCReceiveSignals?.Count; i++)
                {
                    Test_DBCInfo signal = SearchDBCConfigInfos[SelectedIndex].Config.Test_DBCReceiveSignals[i];
                    lisreadsignals.Add(new CANDataInfo { CustomName = signal.Custom_Name, SignalInfo = new CANSignalInfo { MessageID = signal.Message_ID, SignalName = signal.Signal_Name } });
                    //dicreadsignals.Add(signal.Signal_Name, signal.Custom_Name);
                }

                for (int i = 0; i < SearchDBCConfigInfos[SelectedIndex].Config?.Test_DBCSendSignals?.Count; i++)
                {
                    var signal = SearchDBCConfigInfos[SelectedIndex].Config.Test_DBCSendSignals[i];
                    lissendsignals.Add(new CANDataInfo { CustomName = signal.Custom_Name, SignalInfo = new CANSignalInfo { MessageID = signal.Message_ID, SignalName = signal.Signal_Name } });

                    dicsendsignal.Add(new CANDataInfo { CustomName = signal.Custom_Name, SignalInfo = new CANSignalInfo { MessageID = signal.Message_ID, SignalName = signal.Signal_Name } }, Convert.ToDouble(signal.SignalInitValue));
                    //sendsignals_can.Add(dbcconfig?.Test_DBCSendSignals[i].Signal_Name);
                }

                for (int i = 0; i < SearchDBCConfigInfos[SelectedIndex].Config?.Test_DBCSendMessages?.Count; i++)
                {
                    var message = SearchDBCConfigInfos[SelectedIndex].Config.Test_DBCSendMessages[i];
                    dicsendmessages.Add(message.Message_ID, (message.CycleTime, message.FrameFormat));
                }

                GlobalModel.CANThread.LisReadSignals_CAN = lisreadsignals;
                GlobalModel.CANThread.DicMessageSet = dicsendmessages;
                GlobalModel.CANThread.DicSendSignals_CAN = dicsendsignal;

                ((ViewModelLocator)Application.Current.FindResource("Locator")).Product.LoadSendMessage();
                WindowClosedExecute(Win);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }


        }
        #endregion

        #region 保护方法
        protected override void WindowLoadedExecute(object obj)
        {
            IsOpen = true;

            if (!(obj is Window win))
            {
                return;
            }

            //获取当前窗口
            Win = win;
            WindowLeftDown_MoveEvent.LeftDown_MoveEventRegister(Win);

            SearchStr = string.Empty;

            SearchDBCConfigInfos.Clear();
            DBOperate.Default.Init();
            DBOperate.Default.SelectedDBCConfig_All().AttachIfSucceed(result =>
            {
                result.Data.ForEach(item => { LsDBCConfigInfos.Add(new DBCConfigInfo { Config = item }); });

                for (int i = 0; i < result.Data?.Count; i++)
                {
                    var config = new DBCConfigInfo { Config = result.Data[i] };
                    LsDBCConfigInfos.Add(config);
                    SearchDBCConfigInfos.Add(config);
                }
            })
               .AttachIfFailed(result =>
               {
                   MessageBox.Show($"{result.Message}");
                   return;
               });
            //base.WindowLoadedExecute(obj);
        }

        protected override void WindowClosedExecute(object obj)
        {
            IsOpen = false;

            WindowLeftDown_MoveEvent.LeftDown_MoveEventUnRegister(Win);

            base.WindowClosedExecute(obj);
        }
        #endregion


        #region 构造方法
        public SelectDBCViewModel() { }
        #endregion
    }
}
