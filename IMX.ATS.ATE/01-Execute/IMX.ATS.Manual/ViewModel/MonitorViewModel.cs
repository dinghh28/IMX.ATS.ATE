using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using H.WPF.Framework;
using IMX.Common;
using IMX.Device.Common;
using IMX.Logger;
using IMX.WPF.Resource;
using Super.Zoo.Framework;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace IMX.ATS.Manual
{
    public class MonitorViewModel : WindowViewModelBaseEx
    {

        #region 公共属性

        #region 界面绑定属性
        private FrameworkElement devicemanual = ContentControlManager.GetControl(typeof(ManualView), ((ViewModelLocator)Application.Current.FindResource("Locator")).Manual);
        /// <summary>
        /// 设备手动操作界面
        /// </summary>
        public FrameworkElement DeviceManual
        {
            get => devicemanual;
            set => Set(nameof(DeviceManual), ref devicemanual, value);
        }

        private Visibility manualShow = Visibility.Collapsed;

        public Visibility ManualShow
        {
            get => manualShow;
            set => Set(nameof(ManualShow), ref manualShow, value);
        }

        private ObservableCollection<ModRealtimedata> realtimedatas = new ObservableCollection<ModRealtimedata>();
        /// <summary>
        /// 实时监控数据
        /// </summary>
        public ObservableCollection<ModRealtimedata> Realtimedatas
        {
            get => realtimedatas;
            set => Set(nameof(Realtimedatas), ref realtimedatas, value);
        }

        private string btnShow = "\ue66c";
        /// <summary>
        /// 按钮显示
        /// </summary>
        public string BtnShow
        {
            get => btnShow;
            set => Set(nameof(BtnShow), ref btnShow, value);
        }

        private Visibility closeStrShow = Visibility.Collapsed;

        //关闭蒙版
        public Visibility CloseStrShow
        {
            get => closeStrShow;
            set => Set(nameof(CloseStrShow), ref closeStrShow, value);
        }

        #endregion

        #region 界面绑定指令
        public RelayCommand ShowManual => new RelayCommand(() =>
        {
            if (ManualShow == Visibility.Visible)
            {
                BtnShow = "\ue66c";
                ManualShow = Visibility.Collapsed;
            }
            else
            {
                BtnShow = "\ue66f";
                ManualShow = Visibility.Visible;
            }
            //ManualShow = ManualShow == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
        });


        public RelayCommand ShowCanInit => new RelayCommand(() =>
        {
            var model = ((ViewModelLocator)Application.Current.FindResource("Locator")).CanInfo;
            Window view = ContentControlManager.GetWindow<CanInfoView>(model);
            if (model.IsOpen) { MessageBox.Show($"界面已打，请勿重复操作！", "界面提示", MessageBoxButton.OK, MessageBoxImage.Information); return; }


            view.Topmost = true;
            view.Show();

        });

        #endregion

        #endregion

        #region 私有变量

        /// <summary>
        /// 设备数据列表
        /// </summary>
        private Dictionary<string, ModRealtimedata> dicDeviceData = new Dictionary<string, ModRealtimedata>();

        /// <summary>
        /// 设备类型列表
        /// </summary>
        private Dictionary<string, string> dicDeviceType = new Dictionary<string, string>();

        /// <summary>
        /// 当前窗口
        /// </summary>
        private Window Win = null;

        #endregion

        #region 私有方法
        /// <summary>
        /// 设备监控线程
        /// </summary>
        /// <param name="sender"></param>
        private void RefreshDeviceData(object sender)
        {
            if (!(sender is DeviceThread device))
            {
                SuperDHHLoggerManager.Error(LoggerType.THREAD, nameof(MonitorViewModel), nameof(RefreshDeviceData), $"线程传参异常:{sender.GetType()}");
                return;
            }

            if (device.ThreadID == null)
            {
                SuperDHHLoggerManager.Error(LoggerType.THREAD, nameof(MonitorViewModel), nameof(RefreshDeviceData), $"监控线程【{device.ThreadName}】不存在");
                return;
            }

            if (device.DeviceOperate == null)
            {
                SuperDHHLoggerManager.Error(LoggerType.THREAD, nameof(MonitorViewModel), nameof(RefreshDeviceData), $"【{device.ThreadName}-{device.ThreadID}】线程打开失败:设备不存在");
                return;
            }

            if (!device.DeviceOperate.IsInitOK)
            {
                SuperDHHLoggerManager.Error(LoggerType.THREAD, nameof(MonitorViewModel), nameof(RefreshDeviceData), $"【{device.ThreadName}-{device.ThreadID}】线程打开失败:驱动未初始化");
                return;
            }

            device.IsRunning = true;
            SuperDHHLoggerManager.Info(LoggerType.THREAD, nameof(MonitorViewModel), nameof(RefreshDeviceData), $"【{device.ThreadName}-{device.ThreadID}】监控线程开启");
            while (device.IsStratCommunication)
            {
                try
                {
                    if (device.IsReceiveData)
                    {

                        var operateResult = device.DeviceOperate.Device_ReadAll()
                            .AttachIfFailed(result =>
                            {
                                dicDeviceData[device.DeviceAddress].Enableshow = Visibility.Visible ;
                                dicDeviceData[device.DeviceAddress].Messagestr = $"设备通讯异常！";
                                //SuperDHHLoggerManager.Error(LoggerType.THREAD, nameof(MonitorViewModel), nameof(RefreshDeviceData), $"【{device.ThreadName}-{device.ThreadID}】{result.Message}");
                            })
                            .AttachIfSucceed(result =>
                            {
                                var lists = dicDeviceData[device.DeviceAddress];
                                Application.Current.Dispatcher.Invoke(() =>
                                {
                                    lock (lists.Datas)
                                    {
                                        lists.Datas.Clear();
                                        for (int i = 0; i < result.Data?.Count; i++)
                                        {
                                            var data = result.Data[i];
                                            //lists.Datas.Add(result.Data[i]);
                                            lists.Datas.Add(new ModDeviceReadData
                                            {
                                                DataInfo = new ModTestDataInfo { Name = data.DataInfo.Name, Value = data.DataInfo.Value },
                                                BelongTo = data.BelongTo,
                                                DeviceTypename = data.DeviceTypename
                                            });
                                        }
                                    }

                                });
                            });
                    }

                    if (device.DelayTime > 0)
                    {
                        Thread.Sleep(device.DelayTime);
                    }
                    else
                    {
                        Thread.Sleep(100);
                    }
                }
                catch (Exception ex)
                {
                    SuperDHHLoggerManager.Exception(LoggerType.THREAD, nameof(MonitorViewModel), nameof(RefreshDeviceData), ex);
                }
            }

            dicDeviceData.Remove(device.DeviceAddress);

            SuperDHHLoggerManager.Info(LoggerType.THREAD, nameof(MonitorViewModel), nameof(RefreshDeviceData), $"【{device.ThreadName}-{device.ThreadID}】监控线程关闭");
            device.IsRunning = false;

        }

        #endregion

        #region 公共方法
        /// <summary>
        /// 开始设备上报线程
        /// </summary>
        /// <param name="thread">设备上报线程参数</param>
        public void StartRefresh(DeviceThread thread)
        {
            //if (!SupportConfig.DicSupportDevice.ContainsValue(thread.DeviceType))
            //{
            //    return;
            //}

            //if (thread.DeviceType == EDeviceType.Product)
            //{
            //    thread.IsReceiveData = false;
            //}
            try
            {
                if (thread.DeviceAddress != null)
                {
                    if (dicDeviceData.ContainsKey(thread.DeviceAddress))
                    {
                        dicDeviceData.Remove(thread.DeviceAddress);
                    }
                }

                var lists = Realtimedatas.ToList().Find(x => x.TypeName == thread.DeviceAddress);

                if (lists == null)
                {
                    lists = new ModRealtimedata
                    {
                        Enableshow = thread.IsEnableShow? Visibility.Visible : Visibility.Collapsed,
                        Messagestr=thread.MessageLog,
                        TypeName = thread.DeviceAddress,
                        DeviceType = thread.DeviceType.ToString(),
                        //Datas = result.Data
                        Datas = new ObservableCollection<ModDeviceReadData>(),
                    };
                    Realtimedatas.Add(lists);
                }
                dicDeviceData.Add(thread.DeviceAddress, lists);

                if (!dicDeviceType.ContainsKey(thread.DeviceAddress))
                {
                    dicDeviceType.Add(thread.DeviceAddress, thread.DeviceType.ToString().ToUpper());
                }
                //dicLastDeviceData.Add(thread.DeviceAddress, lists.Datas.ToList());

                thread.OprateThread = new Thread(RefreshDeviceData) { IsBackground = true };

                thread.ThreadID = Guid.NewGuid().ToString();

                thread.OprateThread.Start(thread);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }



        }
        #endregion

        #region 保护方法
        protected override void WindowLoadedExecute(object obj)
        {
            try
            {
                foreach (var item in GlobalModel.DicDeviceInfo)
                {
                    if (item.Value.DeviceOperate == null) { continue; }

                    if (!item.Value.Config.EnableMonitor)// || !item.Value.DeviceOperate.IsInitOK)
                    {
                        continue;
                    }
                   
                    //开启设备实时监控
                    if (!GlobalModel.DicDeviceThreads.TryGetValue(item.Key, out DeviceThread thread))
                    {
                        thread = new DeviceThread
                        {
                            ThreadName = item.Key,
                            IsStratCommunication = true,
                            DeviceOperate = item.Value.DeviceOperate,
                            DeviceAddress = $"{item.Value.Config.DeviceType.GetDescription()}[{item.Value.Config.DeviceModel}]",
                            DeviceType = item.Value.Args.DeviceType,
                            DelayTime = item.Value.Drive.DriveConfig.BeforeReadDelayMS,
                            IsProduct = item.Key.Contains("PRODUCT"),
                            ProductIndex = 0,//Convert.ToInt32(item.Key.Split('_')[1]),
                            MessageLog=item.Value.MessageStr,
                            IsEnableShow=item.Value.IsEnableShow,
                        };

                        GlobalModel.DicDeviceThreads.Add(item.Key, thread);
                    }

                    if (item.Value.Drive.DriveConfig.CommunicationType == Device.Common.DriveType.USB)
                    {
                        thread.DelayTime = 500;
                    }

                    thread.DeviceOperate = item.Value.DeviceOperate;
                    thread.IsStratCommunication = true;

                    if (!thread.IsRunning)
                    {
                        StartRefresh(thread);
                    }

                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            if (!(obj is Window win))
            {
                return;
            }

            //获取当前窗
            Win = win;
            WindowLeftDown_MoveEvent.LeftDown_MoveEventRegister(Win);

            //foreach (var thread in GlobalModel.DicDeviceThreads)
            //{
            //    if (!thread.Value.IsRunning)
            //    {
            //        StartRefresh(thread.Value);
            //    }
            //}
            //base.WindowLoadedExecute(obj);
        }

        protected override void WindowClosedExecute(object obj)
        {
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                CloseStrShow = Visibility.Visible;
            }));

            foreach (var thread in GlobalModel.DicDeviceThreads)
            {
                if (thread.Value.IsRunning)
                {
                    //thread.Value.IsRunning = false;
                    thread.Value.IsReceiveData = false;
                    thread.Value.IsStratCommunication = false;
                }
            }
            Thread.Sleep(5000);
            GlobalModel.DicDeviceThreads.Clear();

            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                //ContentControlManager.Show<DeviceInitView>(((ViewModelLocator)Application.Current.FindResource("Locator")).Init);
                Window mainwindow = ContentControlManager.GetWindow<DeviceInitView>(((ViewModelLocator)Application.Current.FindResource("Locator")).Init);
                mainwindow.Show();
                Thread.Sleep(100);
                //base.WindowClosedExecute(obj);
            }));

            WindowLeftDown_MoveEvent.LeftDown_MoveEventUnRegister(Win);

            base.WindowClosedExecute(obj);
        }
        #endregion


        #region 构造方法
        public MonitorViewModel()
        {
            //foreach (var thread in GlobalModel.DicDeviceThreads)
            //{
            //    if (!thread.Value.IsRunning)
            //    {
            //       StartRefresh(thread.Value);
            //    }
            //    //    var lists = Realtimedatas.ToList().Find(x => x.TypeName == thread.Value.DeviceAddress);
            //    //    if (lists == null)
            //    //    {
            //    //        lists = new ModRealtimedata
            //    //        {
            //    //            TypeName = thread.Value.DeviceAddress,
            //    //            DeviceType = thread.Value.DeviceType.ToString(),
            //    //            //Datas = result.Data
            //    //            Datas = new ObservableCollection<ModDeviceReadData>(),
            //    //        };
            //    //        Realtimedatas.Add(lists);
            //    //    }
            //    //    dicDeviceData.Add(thread.Value.DeviceAddress, lists);

            //    //    if (!dicDeviceType.ContainsKey(thread.Value.DeviceAddress))
            //    //    {
            //    //        dicDeviceType.Add(thread.Value.DeviceAddress, thread.Value.DeviceType.ToString().ToUpper());
            //    //    }
            //    //    //dicLastDeviceData.Add(thread.DeviceAddress, lists.Datas.ToList());

            //    //    thread.Value.OprateThread = new Thread(RefreshDeviceData) { IsBackground = true };

            //    //    thread.Value.ThreadID = Guid.NewGuid().ToString();

            //    //    thread.Value.OprateThread.Start(thread);
            //}

        }
        #endregion

    }

    public class ModRealtimedata : ViewModelBase
    {
        /// <summary>
        /// 设备名称
        /// </summary>
        public string TypeName { get; set; }

        /// <summary>
        /// 设备总称
        /// </summary>
        public string DeviceType { get; set; }

        /// <summary>
        /// 初始化异常显示蒙版
        /// </summary>
        public Visibility Enableshow { get; set; }= Visibility.Collapsed;

        /// <summary>
        /// 初始化异常信息
        /// </summary>
        public string Messagestr { get; set; }

        /// <summary>
        /// 读取数据
        /// </summary>
        public ObservableCollection<ModDeviceReadData> Datas { get; set; }


        /// <summary>
        /// 清除当前数据
        /// </summary>
        public RelayCommand Clear => new RelayCommand(() =>
        {
            lock (Datas)
            {
                Datas.Clear();
            }
        });
    }
}
