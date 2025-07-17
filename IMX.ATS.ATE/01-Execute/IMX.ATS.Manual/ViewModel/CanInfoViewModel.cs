using GalaSoft.MvvmLight.Command;
using H.WPF.Framework;
using IMX.DB.Model;
using IMX.Device.Base.DriveOperate;
using IMX.Device.Base;
using IMX.Device.Common;
using Super.Zoo.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using IMX.Common;
using IMX.Device.Product;
using IMX.Logger;
using IMX.WPF.Resource;
using IMX.ATE.Framework;
using IMX.DB;
using System.Runtime.InteropServices;
using System.Windows.Media.TextFormatting;
using System.Windows.Media;
using System.Collections.ObjectModel;
using System.Xml.Linq;

namespace IMX.ATS.Manual
{
    public class CanInfoViewModel : ExtendViewModelBase
    {
        #region 界面绑定

        #region 属性绑定

        private bool usedbc;
        /// <summary>
        /// DBC通讯启用
        /// </summary>
        public bool UseDBC
        {
            get => usedbc;
            set => Set(nameof(UseDBC), ref usedbc, value);
        }

        private string dbcconfigname;
        /// <summary>
        /// dbc配置名称
        /// </summary>
        public string DBCConfigName
        {
            get => dbcconfigname;
            set => Set(nameof(DBCConfigName), ref dbcconfigname, value);
        }

        /// <summary>
        /// CAN支持波特率列表
        /// </summary>
        public List<string> BaudRates => new List<string> { "5Kbps", "10Kbps", "20Kbps", "50Kbps", "100Kbps", "125Kbps", "250Kbps", "500Kbps", "800Kbps", "1000Kbps" };

        private string baudrate;
        /// <summary>
        /// 仲裁波特率
        /// </summary>
        public string BaudRate
        {
            get => baudrate;
            set => Set(nameof(BaudRate), ref baudrate, value);
        }


        private string databaudrate;
        /// <summary>
        /// 数据域波特率
        /// </summary>
        public string DataBaudRate
        {
            get => databaudrate;
            set => Set(nameof(DataBaudRate), ref databaudrate, value);
        }


        private ObservableCollection<string> messagestrs = new ObservableCollection<string>();
        /// <summary>
        /// 操作信息
        /// </summary>
        public ObservableCollection<string> Messagestrs
        {
            get => messagestrs;
            set => Set(nameof(Messagestrs), ref messagestrs, value);
        }


        #endregion

        #region 指令绑定

        public RelayCommand ChangeDBC => new RelayCommand(() =>
        {
            var model = ((ViewModelLocator)Application.Current.FindResource("Locator")).SelectDBC;
            Window view = ContentControlManager.GetWindow<SelectDBCView>(model);
            if (model.IsOpen) { MessageBox.Show($"界面已打，请勿重复操作！", "界面提示", MessageBoxButton.OK, MessageBoxImage.Information); return; }

            //if (GlobalModel.CANThread.TestDBCconfig != null || GlobalModel.CANThread.TestDBCconfig.Id != 0)
            //{
            //    if (MessageBox.Show("是否切换当前配置DBC?\r\n若切换请核对试验项操作步骤，避免产品相关指令不符合预期", "DBC配置变更", MessageBoxButton.YesNo) == MessageBoxResult.No)
            //    {
            //        return;
            //    }
            //}

            view.Topmost = true;
            view.Show();

            Task.Run(() =>
            {
                Thread.Sleep(500);
                while (true)
                {
                    if (!model.IsOpen)
                    {
                        dbcconfig = GlobalModel.CANThread.TestDBCconfig;
                        if (dbcconfig != null)
                        {
                            DBCConfigName = dbcconfig.ConfigName;
                        }
                    }
                }
            });
        });

        /// <summary>
        /// 初始化CAN设备
        /// </summary>
        public RelayCommand CanInit => new RelayCommand(CanDeviceInit);

        /// <summary>
        /// 卸载CAN设备
        /// </summary>
        public RelayCommand CanUnInit => new RelayCommand(CanDeviceUnInit);



        #endregion
        #endregion

        #region 构造函数

        #endregion


        #region 私有变量

        private Test_DBCConfig dbcconfig = null;

        /// <summary>
        /// 当前窗口
        /// </summary>
        private Window Win = null;

        #endregion

        #region 公有变量
        /// <summary>
        /// 当前界面打开状态
        /// </summary>
        public bool IsOpen = false;
        #endregion

        #region 私有方法

        /// <summary>
        /// 初始化CAN设备
        /// </summary>
        private void CanDeviceInit()
        {
            try
            {
                if (GlobalModel.CANThread.TestDBCFile == null || GlobalModel.CANThread.TestDBCconfig == null)
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        Messagestrs.Add($"[{DateTime.Now}]--无DBC文件或未配置DBC，请确认配置信息");

                    });
                    //ErrorStr = $"Product设备初始化失败!\r\n【无DBC文件或未配置DBC，请确认配置信息】！\r\n";
                    return;
                }


                if (GlobalModel.DicDeviceInfo.TryGetValue("Product", out var candeviceInfo))
                {
                    if (candeviceInfo == null)
                    {
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            Messagestrs.Add($"[{DateTime.Now}]--CAN设备配置文件不存在");

                        });

                        //ErrorStr = $"CAN设备初始化失败!\r\nCAN设备不存在！\r\n";
                        return;
                    }

                    if (candeviceInfo.DeviceOperate != null && candeviceInfo.DeviceOperate.IsInitOK)
                    {
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            Messagestrs.Add($"[{DateTime.Now}]--CAN设备已初始化");

                        });

                        //ErrorStr = $"CAN设备已初始化!\r\n"; 
                        return;
                    }

                    string configString = string.Empty;
                    DriveType eType = DriveType.VehicleBus;
                    string strParam1 = string.Empty;
                    string strParam2 = string.Empty;
                    string strParam3 = string.Empty;

                    DriveHelper.DecodeConfigString(candeviceInfo.Args.DriveConfig.ConfigString, out string canfdspeed, out string databaudrate, out int channelindex).AttachIfSucceed(result =>
                    {
                        DriveHelper.EncryptedConfigString(canfdspeed, databaudrate, (uint)channelindex, ref configString).AttachIfSucceed(result =>
                        {
                            candeviceInfo.Args.DriveConfig.ConfigString = configString;
                            candeviceInfo.Args.DriveConfig.BaudRate = BaudRate;
                        });
                    })
                        .AttachIfFailed(result =>
                        {
                            Thread.Sleep(10);
                            Application.Current.Dispatcher.Invoke(() =>
                            {
                                Messagestrs.Add($"[{DateTime.Now}]--CAN设备配置文件加载失败:{result.Message}");

                                //ErrorStr = $"CAN设备初始化失败\r\n{result.Message}\r\n";

                            });
                            return;
                        });

                    Thread.Sleep(10);
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        Messagestrs.Add($"[{DateTime.Now}]--CAN设备配置文件加载成功");

                    });

                    var canresult = DeviceInit(candeviceInfo.Args);
                    if (!canresult)
                    {
                        Thread.Sleep(10);
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            Messagestrs.Add($"[{DateTime.Now}]--CAN设备初始化失败:{canresult.Message}");

                            //ErrorStr = $"CAN设备初始化失败\r\n{canresult.Message}\r\n";

                        });
                        return;
                    }
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        Messagestrs.Add($"[{DateTime.Now}]--CAN设备初始化成功");

                    });

                    GlobalModel.DicDeviceInfo["Product"].DeviceOperate = canresult.Data;
                    Product_CAN_Operate operate = (canresult.Data as Product_CAN_Operate);

                    var cansetrlt = operate.LoadMessageFile(GlobalModel.CANThread.TestDBCFile.FileContent, GlobalModel.CANThread.TestDBCFile.FileExtension)
                    .And(operate.SetReadSignal(GlobalModel.CANThread.LisReadSignals_CAN))
                       .And(operate.SetSendSignal(GlobalModel.CANThread.DicSendSignals_CAN.Keys.ToList()))
                       .And(operate.SetSendSignalInitValue(GlobalModel.CANThread.DicSendSignals_CAN))
                       .And(operate.SetMessageCycleTimeANDFrameFormat(GlobalModel.CANThread.DicMessageSet));

                    if (!cansetrlt)
                    {
                        Thread.Sleep(200);
                        Application.Current.Dispatcher.Invoke(new Action(() =>
                        {
                            Messagestrs.Add($"[{DateTime.Now}]--CAN配置参数初始化失败");

                            //ErrorStr = "CAN配置参数初始化失败";

                        }));

                        CanDeviceUnInit();
                        return;
                    }

                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        Messagestrs.Add($"[{DateTime.Now}]--CAN配置参数初始化成功");

                    });


                    candeviceInfo.Drive = GlobalModel.DicDeviceDrives[candeviceInfo.Args.DriveConfig.ResourceString];
                    Thread.Sleep(100);
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        Messagestrs.Add($"[{DateTime.Now}]--CAN设备初始化成功");
                    });

                    var canstart = operate.StartCommunication();
                    if (!canstart)
                    {
                        Thread.Sleep(200);
                        Application.Current.Dispatcher.Invoke(new Action(() =>
                        {
                            Messagestrs.Add($"[{DateTime.Now}]--CAN设备启动失败");

                        }));
                        CanDeviceUnInit();
                        return;
                    }

                    Thread.Sleep(100);
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        Messagestrs.Add($"[{DateTime.Now}]--CAN设备启动成功");
                    });

                    GlobalModel.DicDeviceThreads.Add("Product", new DeviceThread
                    {
                        ThreadName = "Product",
                        IsStratCommunication = true,
                        DeviceOperate = candeviceInfo.DeviceOperate,
                        DeviceAddress = $"{candeviceInfo.Config.DeviceType.GetDescription()}[{candeviceInfo.Config.DeviceModel}]",
                        DeviceType = candeviceInfo.Args.DeviceType,
                        DelayTime = candeviceInfo.Drive.DriveConfig.BeforeReadDelayMS,
                        IsProduct = true,
                        ProductIndex = 0,//Convert.ToInt32(item.Key.Split('_')[1]),
                        IsReceiveData = true,
                    });
                    GlobalModel.DicDeviceThreads["Product"].DelayTime = 1000;

                    //GlobalModel.DicDeviceThreads["Product"].IsStratCommunication = true;

                    //if (!GlobalModel.DicDeviceThreads["Product"].IsRunning)
                    //{
                    //    ((ViewModelLocator)Application.Current.FindResource("Locator")).Monitor.StartRefresh(GlobalModel.DicDeviceThreads["Product"]);
                    //    Thread.Sleep(100);
                    //    Application.Current.Dispatcher.Invoke(() =>
                    //    {
                    //        Messagestrs.Add($"[{DateTime.Now}]--CAN设备通讯线程加载成功");
                    //    });

                    //}

                    ((ViewModelLocator)Application.Current.FindResource("Locator")).Manual.AddOperateView("Product", GlobalModel.DicDeviceInfo["Product"]);


                    ((ViewModelLocator)Application.Current.FindResource("Locator")).Monitor.StartRefresh(GlobalModel.DicDeviceThreads["Product"]);
                }
            }
            catch (Exception ex)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    Messagestrs.Add($"[{DateTime.Now}]--CANt设备初始化失败:{ex.Message}");
                });
            }

        }
        /// <summary>
        /// 卸载CAN设备
        /// </summary>
        private void CanDeviceUnInit()
        {
            try
            {
                if (!GlobalModel.DicDeviceThreads.TryGetValue("Product", out DeviceThread thread))
                {
                    MessageBox.Show("CAN设备未初始化");
                    return;
                }

                thread.IsReceiveData = false;
                Thread.Sleep(100);

                thread.IsStratCommunication = false;
                //Thread.Sleep(200);

                int timeout = 1 * 60 * 10;

                while (timeout-- > 0)
                {
                    if (thread.IsRunning) 
                    {
                        break;
                    }

                    Thread.Sleep(100);
                }

                var product = GlobalModel.DicDeviceInfo["Product"];

                var operate = product.DeviceOperate as Product_CAN_Operate;
                OperateResult result = operate.StopCommunication()
                    .And(operate.Close())
                    .And(product.Drive.UnregisterDevice(operate));
                //OperateResult result = GlobalModel.DicDeviceOperate[operate].UnregisterDevice(operate);

                if (!result)
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        Messagestrs.Add($"[{DateTime.Now}]--CAN设备卸载失败:{result.Message}");
                    });
                    return;
                }

                if (product.Drive.CanRemove)
                {
                    GlobalModel.DicDeviceDrives.Remove(product.DeviceOperate.DeviceConfig.DriveConfig.ResourceString);
                }

                Thread.Sleep(100);



                GlobalModel.DicDeviceThreads.Remove("Product");

                //GlobalModel.DicDeviceInfo.Remove("Product");

                Application.Current.Dispatcher.Invoke(() =>
                {
                    Messagestrs.Add($"[{DateTime.Now}]--CAN设备卸载成功");
                });

                ((ViewModelLocator)Application.Current.FindResource("Locator")).Manual.RemoveOperateView("Product");

                //if (!GlobalModel.DicDeviceInfo.TryGetValue("Product", out var candeviceInfo))
                //{
                //    if (!candeviceInfo.DeviceOperate.IsInitOK)
                //    {
                //        return;
                //    }
                //    candeviceInfo.DeviceOperate.Close().And(candeviceInfo.DeviceOperate.UnInit())
                //       .And(candeviceInfo.Drive.UnregisterDevice(candeviceInfo.DeviceOperate))
                //       .AttachIfSucceed(result =>
                //       {
                //           GlobalModel.DicDeviceThreads.Remove("Product");
                //           Application.Current.Dispatcher.Invoke(() =>
                //           {
                //               ErrorStr = $"Product设备卸载成功\r\n{result.Message}\r\n";
                //           });
                //           Thread.Sleep(100);
                //       })
                //       .AttachIfFailed(result =>
                //       {
                //           Application.Current.Dispatcher.Invoke(() =>
                //           {
                //               ErrorStr = $"Product设备卸载成功\r\n{result.Message}\r\n";
                //           });
                //           Thread.Sleep(100);
                //       });
                //}
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                //SuperDHHLoggerManager.Exception(LoggerType.FROMLOG, nameof(DeviceInitViewModel), nameof(CanDeviceUnInit), ex);
            }
        }


        /// <summary>
        /// 设备初始化
        /// </summary>
        /// <param name="config">设备配置信息</param>
        /// <returns></returns>
        private OperateResult<IDeviceOperate> DeviceInit(DeviceArgs config)
        {
            if (!GlobalModel.DicDeviceDrives.TryGetValue(config.DriveConfig.ResourceString, out DriveOperate drive))
            {
                DriveOperate operate = DriveOperate.Creat();
                OperateResult result = operate.Open(config.DriveConfig);
                if (!result)
                {
                    return OperateResult<IDeviceOperate>.Failed(null, result.Message);
                }
                GlobalModel.DicDeviceDrives.Add(config.DriveConfig.ResourceString, operate);
                drive = operate;
                GlobalModel.DicDeviceInfo["Product"].Drive = drive;
            }

            return drive.RegisterDevice(config)
                     .ThenAnd(result => result.Data.Init(config, drive.Drive).ConvertTo(result.Data))
                     .AttachIfSucceed(result =>
                     {
                         GlobalModel.DicDeviceOperate.Add(result.Data, drive);

                     });

            //if (!GlobalModel.DicDeviceDrives.TryGetValue(config.DriveConfig.ResourceString, out DriveOperate drive))
            //{
            //    DriveOperate operate = DriveOperate.Creat();
            //    OperateResult result = operate.Open(config.DriveConfig);
            //    if (!result)
            //    {
            //        return OperateResult<IDeviceOperate>.Failed(null, result.Message);
            //    }
            //    GlobalModel.DicDeviceDrives.Add(config.DriveConfig.ResourceString, operate);
            //    drive = operate;
            //}

            //return drive.RegisterDevice(config)
            //     .ThenAnd(result => (result.Data as Product_CAN_Operate).Init(config, drive.Drive).ConvertTo(result.Data));

        }
        #endregion

        #region 公有方法

        #endregion

        #region 保护方法

        protected override void WindowLoadedExecute(object obj)
        {
            IsOpen = true;

            if (!(obj is Window win))
            {
                return;
            }

            Messagestrs.Clear();
            //获取当前窗
            Win = win;
            WindowLeftDown_MoveEvent.LeftDown_MoveEventRegister(Win);
        }

        protected override void WindowClosedExecute(object obj)
        {
            IsOpen = false;

            WindowLeftDown_MoveEvent.LeftDown_MoveEventUnRegister(Win);

            base.WindowClosedExecute(obj);

        }

        #endregion
    }
}
