using GalaSoft.MvvmLight.Command;
using H.WPF.Framework;
using System;
using System.Windows.Controls;
using System.Windows;
using Super.Zoo.Framework;
using IMX.DB;
using IMX.DB.Model;

namespace IMX.ATS.Lander
{
    public class LoginViewModel : ExtendViewModelBase
    {
        #region 公共属性

        #region 界面绑定

        private string userName;
        /// <summary>
        /// 用户登录名称
        /// </summary>
        public string UserName
        {
            get => userName;
            set => Set(nameof(UserName), ref userName, value);
        }

        private string password;
        /// <summary>
        /// 用户登录密码
        /// </summary>
        public string Password
        {
            get => password;
            set => Set(nameof(Password), ref password, value);
        }

        private string errorMessage; 
        /// <summary>
        /// 用户登录故障信息
        /// </summary>
        public string ErrorMessage
        {
            get => errorMessage;
            set => Set(nameof(ErrorMessage), ref errorMessage, value);
        }


        #endregion

        #region 指令

        public RelayCommand<object> LoginCommand => new RelayCommand<object>(UserLogin);

        #endregion

        #endregion

        #region 私有变量

        #endregion

        #region 私有方法

        /// <summary>
        /// 用户登录
        /// 查询数据库：
        /// 1、验证登录用户是否存在
        /// 2、验证已存在用户名和密码是否一致
        /// 
        /// 登录成功打开门户界面
        /// </summary>
        private void UserLogin(object objWindow)
        {
            try
            {
#if DEBUG
                UserName = "admin";
                Password = "111111";
#endif
                ErrorMessage = "";
                if (string.IsNullOrEmpty(UserName))
                {
                    ErrorMessage = $"登录失败！请输入用户名！";
                    return;
                }
                if (string.IsNullOrEmpty(Password))
                {
                    ErrorMessage = $"登录失败！请输入密码！";
                    return;
                }


                //检查数据库初始化状态
                if (!DBOperate.Default.IsInitOK) { DBOperate.Default.Init(); }

                //GlobalModel.UserInfo.Password = Password;
                //GlobalModel.UserInfo.UserName = UserName;
                //GlobalModel.UserInfo.Privilege = 15;

                //OperateResult re = DBOperate.Default.AddNewUser(GlobalModel.UserInfo);
                //#if DEBUG
                //                Application.Current.Dispatcher.Invoke(new Action(() =>
                //                {
                //                    Window mainwindow = ContentControlManager.GetWindow<TODeviceInitView>(((ViewModelLocator)Application.Current.FindResource("Locator")).TOInit);
                //                    mainwindow.Show();
                //                    (objWindow as Window).Close();
                //                }));
                //#else

                //验证用户信息，并返回带有用户权限的完整用户信息
                OperateResult<UserInfo> result = DBOperate.Default.Login(UserName,Password);

                if (!result)
                {
                    ErrorMessage=$"登录失败！{result.Message}";
                    return;
                }

                //GlobalModel.UserInfo.Password = Password;
                GlobalModel.UserInfo.UserName = UserName;
                GlobalModel.UserInfo.Id = result.Data.Id;
                GlobalModel.UserInfo.Privilege = result.Data.Privilege;

                Application.Current.Dispatcher.Invoke(new Action(() =>
                {
                    Window mainwindow = ContentControlManager.GetWindow<PortalView>(((ViewModelLocator)Application.Current.FindResource("Locator")).Portal);
                    mainwindow.Show();
                    (objWindow as Window).Close();
                }));
//#endif
            }
            catch (Exception ex)
            {
                ErrorMessage = $"登录异常！{ex.Message}";
            }
        }

#endregion


        #region 保护方法

        protected override void WindowLoadedExecute(object obj)
        {
            //for (int i = 0; i < SupportConfig.CabinetIDs.Count; i++)
            //{
            //    string cabinetid = SupportConfig.CabinetIDs[i];
                
            //    CabinetDeviceIp CabinetDeviceIp = SupportConfig.DicCabinetDeviceIp[cabinetid];

            //    #region 继电器配置信息初始化
            //    SupportConfig.DicCabinetDeviceConfig[cabinetid].RelayConfig_1.Address = 1;
            //    SupportConfig.DicCabinetDeviceConfig[cabinetid].RelayConfig_1.Name = "ZS";
            //    SupportConfig.DicCabinetDeviceConfig[cabinetid].RelayConfig_1.DeviceType = EDeviceType.Relay;
            //    SupportConfig.DicCabinetDeviceConfig[cabinetid].RelayConfig_1.DriveConfig = new ModDriveConfig
            //    {
            //        CommunicationType = DriveType.TCPIP,
            //        TerminationCharacterEnabled = false,
            //        BeforeReadDelayMS = 200,
            //        TimeoutMS = 500,
            //        ResourceString = DeviceHelper.EncryptedResourceString(DriveType.TCPIP, CabinetDeviceIp.RelayDefaltIP, CabinetDeviceIp.RelayStartPort.ToString(), string.Empty),
            //    };

            //    SupportConfig.DicCabinetDeviceConfig[cabinetid].RelayConfig_2.Address = 1;
            //    SupportConfig.DicCabinetDeviceConfig[cabinetid].RelayConfig_2.Name = "ZS";
            //    SupportConfig.DicCabinetDeviceConfig[cabinetid].RelayConfig_2.DeviceType = EDeviceType.Relay;
            //    SupportConfig.DicCabinetDeviceConfig[cabinetid].RelayConfig_2.DriveConfig = new ModDriveConfig
            //    {
            //        CommunicationType = DriveType.TCPIP,
            //        TerminationCharacterEnabled = false,
            //        BeforeReadDelayMS = 200,
            //        TimeoutMS = 500,
            //        ResourceString = DeviceHelper.EncryptedResourceString(DriveType.TCPIP, CabinetDeviceIp.RelayDefaltIP, (CabinetDeviceIp.RelayStartPort + 1).ToString(), string.Empty),
            //    };
            //    #endregion

            //    #region 工位配置信息初始化
            //    SupportConfig.DicCabinetDeviceConfig[cabinetid].ProductDeviceConfigs.Clear();

            //    for (int j = 0; j < SupportConfig.StationNum; j++)
            //    {
            //        ProductDeviceConfig prodectconfig = new ProductDeviceConfig();

            //        #region CAN设备配置初始化
            //        prodectconfig.CANConfig = new ModDeviceConfig
            //        {
            //            DeviceType = EDeviceType.Product,
            //            Name = "CAN",
            //            DriveConfig = new ModDriveConfig
            //            {
            //                CommunicationType = DriveType.VehicleBus,
            //                TerminationCharacterEnabled = false,
            //                BeforeReadDelayMS = 200,
            //                TimeoutMS = 500,
            //                ConfigString = DeviceHelper.EncryptedConfigString("0", $"192.168.{CabinetDeviceIp.DefaltHost}.{CabinetDeviceIp.CanDefaltBroadcastAddress + j}", 4000, CabinetDeviceIp.CANDefalWorkProt, 0),
            //                ResourceString = DeviceHelper.EncryptedResourceString(DriveType.VehicleBus, CabinetDeviceIp.CANDefalType, CabinetDeviceIp.CANDefalDeviceType, (j + CabinetDeviceIp.CANDefalIndex).ToString()),
            //            },
            //        };
            //        //prodectconfig.CANConfig.DeviceType = EDeviceType.Product;
            //        //prodectconfig.CANConfig.DriveConfig = new ModDriveConfig
            //        //{
            //        //    CommunicationType = DriveType.VehicleBus,
            //        //    TerminationCharacterEnabled = false,
            //        //    BeforeReadDelayMS = 200,
            //        //    TimeoutMS = 500,
            //        //    ConfigString = DeviceHelper.EncryptedConfigString("0", $"192.168.{CabinetDeviceIp.DefaltHost}.{CabinetDeviceIp.CanDefaltBroadcastAddress + j}", 4000, CabinetDeviceIp.CANDefalWorkProt, 0),
            //        //    ResourceString = DeviceHelper.EncryptedResourceString(DriveType.VehicleBus, CabinetDeviceIp.CANDefalType, CabinetDeviceIp.CANDefalDeviceType, (j + CabinetDeviceIp.CANDefalIndex).ToString()),
            //        //};
            //        #endregion

            //        #region 其他设备配置初始化
            //        prodectconfig.LoadConfig_1 = new ModDeviceConfig 
            //        {
            //            DeviceType = EDeviceType.DCLoad,
            //            Name = "HSBT3K6",
            //            Address = 1,
            //            DriveConfig = new ModDriveConfig
            //            {
            //                CommunicationType = DriveType.TCPIP,
            //                TerminationCharacterEnabled = false,
            //                BeforeReadDelayMS = 200,
            //                TimeoutMS = 500,
            //                ResourceString = DeviceHelper.EncryptedResourceString(DriveType.TCPIP, CabinetDeviceIp.OtherDefaltIP, (CabinetDeviceIp.LoadStartPort + j).ToString(), string.Empty),
            //            },
            //        };
            //        //prodectconfig.LoadConfig_1.DeviceType = EDeviceType.DCLoad;
            //        //prodectconfig.LoadConfig_1.Address = 1;
            //        //prodectconfig.LoadConfig_1.DriveConfig = new ModDriveConfig
            //        //{
            //        //    CommunicationType = DriveType.TCPIP,
            //        //    TerminationCharacterEnabled = false,
            //        //    BeforeReadDelayMS = 200,
            //        //    TimeoutMS = 500,
            //        //    ResourceString = DeviceHelper.EncryptedResourceString(DriveType.TCPIP, CabinetDeviceIp.OtherDefaltIP, (CabinetDeviceIp.LoadStartPort + j).ToString(), string.Empty),
            //        //};

            //        prodectconfig.LoadConfig_2 = new ModDeviceConfig
            //        {
            //            DeviceType = EDeviceType.DCLoad,
            //            Name = "HSBT3K6",
            //            Address = 2,
            //            DriveConfig = new ModDriveConfig
            //            {
            //                CommunicationType = DriveType.TCPIP,
            //                TerminationCharacterEnabled = false,
            //                BeforeReadDelayMS = 200,
            //                TimeoutMS = 500,
            //                ResourceString = DeviceHelper.EncryptedResourceString(DriveType.TCPIP, CabinetDeviceIp.OtherDefaltIP, (CabinetDeviceIp.LoadStartPort + j).ToString(), string.Empty),
            //            },
            //        };

            //        //prodectconfig.LoadConfig_2.DeviceType = EDeviceType.DCLoad;
            //        //prodectconfig.LoadConfig_2.Address = 2;
            //        //prodectconfig.LoadConfig_2.DriveConfig = new ModDriveConfig
            //        //{
            //        //    CommunicationType = DriveType.TCPIP,
            //        //    TerminationCharacterEnabled = false,
            //        //    BeforeReadDelayMS = 200,
            //        //    TimeoutMS = 500,
            //        //    ResourceString = DeviceHelper.EncryptedResourceString(DriveType.TCPIP, CabinetDeviceIp.OtherDefaltIP, (CabinetDeviceIp.LoadStartPort + j).ToString(), string.Empty),
            //        //};
            //        prodectconfig.AcquisitionConfig = new ModDeviceConfig 
            //        {
            //            DeviceType = EDeviceType.Acquisition,
            //            Name = "DDSU6606",
            //            Address = j + 1,
            //            DriveConfig = new ModDriveConfig
            //            {
            //                CommunicationType = DriveType.TCPIP,
            //                TerminationCharacterEnabled = false,
            //                BeforeReadDelayMS = 200,
            //                TimeoutMS = 1000,
            //                ResourceString = DeviceHelper.EncryptedResourceString(DriveType.TCPIP, CabinetDeviceIp.OtherDefaltIP, (CabinetDeviceIp.AcquisitionStartPort + j / 3).ToString(), string.Empty),
            //            }
            //        };
            //        //prodectconfig.AcquisitionConfig.DeviceType = EDeviceType.Acquisition;
            //        //prodectconfig.AcquisitionConfig.Address = j % 3 + 1;
            //        //prodectconfig.AcquisitionConfig.DriveConfig = new ModDriveConfig
            //        //{
            //        //    CommunicationType = DriveType.TCPIP,
            //        //    TerminationCharacterEnabled = false,
            //        //    BeforeReadDelayMS = 200,
            //        //    TimeoutMS = 500,
            //        //    ResourceString = DeviceHelper.EncryptedResourceString(DriveType.TCPIP, CabinetDeviceIp.OtherDefaltIP, (CabinetDeviceIp.AcquisitionStartPort + j / 3).ToString(), string.Empty),
            //        //};
            //        #endregion

            //        SupportConfig.DicCabinetDeviceConfig[cabinetid].ProductDeviceConfigs.Add(prodectconfig);
            //    }
            //    #endregion
            //}
            //base.WindowLoadedExecute(obj);
        }

        #endregion


        #region 构造函数
        public LoginViewModel() { }
    }

    #endregion

    /// <summary>
    /// 密码帮助类
    /// </summary>
    public class PassWordBoxHelper
    {
        //包含两个依赖附加属性
        public static readonly DependencyProperty PasswordProperty = DependencyProperty.RegisterAttached(
           "Password",
           typeof(string),
           typeof(PassWordBoxHelper),
           new FrameworkPropertyMetadata(string.Empty, OnPasswordPropertyChanged));



        public static readonly DependencyProperty AttachProperty = DependencyProperty.RegisterAttached(
            "Attach",
            typeof(bool),
            typeof(PassWordBoxHelper),
            new PropertyMetadata(false, Attach));


        private static readonly DependencyProperty IsUpdatingProperty = DependencyProperty.RegisterAttached(
            "IsUpdating",
            typeof(bool),
            typeof(PassWordBoxHelper));

        public static void SetAttach(DependencyObject dp, bool value)
        {
            dp.SetValue(AttachProperty, value);
        }
        public static bool GetAttach(DependencyObject dp)
        {
            return (bool)dp.GetValue(AttachProperty);
        }
        public static string GetPassword(DependencyObject dp)
        {
            return (string)dp.GetValue(PasswordProperty);
        }
        public static void SetPassword(DependencyObject dp, string value)
        {
            dp.SetValue(PasswordProperty, value);
        }
        private static bool GetIsUpdating(DependencyObject dp)
        {
            return (bool)dp.GetValue(IsUpdatingProperty);
        }
        private static void SetIsUpdating(DependencyObject dp, bool value)
        {
            dp.SetValue(IsUpdatingProperty, value);
        }

        private static void OnPasswordPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            PasswordBox passwordBox = sender as PasswordBox;
            passwordBox.PasswordChanged -= PasswordChanged;
            if (!(bool)GetIsUpdating(passwordBox))
            {
                passwordBox.Password = (string)e.NewValue;
            }
            passwordBox.PasswordChanged += PasswordChanged;
        }

        private static void Attach(
            DependencyObject sender,
            DependencyPropertyChangedEventArgs e)
        {
            PasswordBox passwordBox = sender as PasswordBox;
            if (passwordBox == null)
                return;
            if ((bool)e.OldValue)
            {
                passwordBox.PasswordChanged -= PasswordChanged;
            }
            if ((bool)e.NewValue)
            {
                passwordBox.PasswordChanged += PasswordChanged;
            }
        }

        private static void PasswordChanged(object sender, RoutedEventArgs e)
        {
            PasswordBox passwordBox = sender as PasswordBox;
            SetIsUpdating(passwordBox, true);
            SetPassword(passwordBox, passwordBox.Password);
            SetIsUpdating(passwordBox, false);
        }
    }


    //public class UserModel
    //{
    //    /// <summary>
    //    /// 用户登录名称
    //    /// </summary>
    //    public string UserName { get; set; }

    //    /// <summary>
    //    /// 用户登录密码
    //    /// </summary>
    //    public string UserPassword { get; set; }

    //    /// <summary>
    //    /// 用户权限等级
    //    /// </summary>
    //    public UserPrivilegeLevel UserAuthor { get; set; }
    //}
}

