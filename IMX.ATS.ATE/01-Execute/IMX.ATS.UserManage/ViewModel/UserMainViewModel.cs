using GalaSoft.MvvmLight.Command;
using H.WPF.Framework;
using IMX.ATS.Common;
using IMX.DB;
using IMX.DB.Model;
using IMX.WPF.Resource;
using Super.Zoo.Framework;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace IMX.ATS.UserManage
{
    public class UserMainViewModel : ExtendViewModelBase
    {
        #region 公共属性

        #region 界面绑定属性

        private string userName = GlobalModel.UserInfo.UserName;

        public string UserName
        {
            get => userName;
            set => Set(nameof(UserName), ref userName, value);
        }
        private Visibility userMannage;

        public Visibility UserMannage
        {
            get => userMannage;
            set => Set(nameof(UserMannage), ref userMannage, value);
        }
        private string passWord=GlobalModel.UserInfo.Password;
        /// <summary>
        /// 用户原密码
        /// </summary>
        public string PassWord
        {
            get => passWord;
            set => Set(nameof(PassWord), ref passWord, value);
        }

        private string newPassword;

        /// <summary>
        /// 用户新密码
        /// </summary>
        public string NewPassword
        {
            get => newPassword;
            set => Set(nameof(NewPassword), ref newPassword, value);
        }


        private ObservableCollection<UsersModel> users = new ObservableCollection<UsersModel>();

        public ObservableCollection<UsersModel> Users
        {
            get => users;
            set => Set(nameof(Users), ref users, value);
        }

        #endregion

        #region 界面绑定指令
        /// <summary>
        /// 修改用户密码
        /// </summary>
        public RelayCommand ModifiyPWCommond => new RelayCommand(ModifiyUserpassword);

        public RelayCommand AddNewuserCommand => new RelayCommand(AddNewuserinfo);

        public RelayCommand RefreshCommand => new RelayCommand(RefreshUser);


        #endregion

        #endregion
        #region 私有变量

        /// <summary>
        /// 当前窗口
        /// </summary>
        private Window Win = null;

        #endregion

        #region 私有方法

        /// <summary>
        /// 修改用户密码
        /// </summary>
        /// <param name="obj"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void ModifiyUserpassword()
        {
            try
            {
                OperateResult result = DBOperate.Default.UpdateUserPassword(GlobalModel.UserInfo.Id, PassWord, NewPassword);
                if (!result)
                {
                    MessageBox.Show($"{result.Message}");
                    return;
                }
                MessageBox.Show("密码修改成功！");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{ex.Message}");
            }

        }

        /// <summary>
        /// 新增用户
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        private void AddNewuserinfo()
        {
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                Window mainwindow = ContentControlManager.GetWindow<NewUserView>(((ViewModelLocator)Application.Current.FindResource("Locator")).NewUser);
                mainwindow.Show();
            }));
        }

        /// <summary>
        /// 刷新用户数据
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        private void RefreshUser()
        {
            OperateResult<List<UserInfo>> result = DBOperate.Default.GetAllusers();
            if (!result)
            {
                MessageBox.Show("用户信息查询失败，请重新查询！");
                return;
            }
            Users.Clear();
            int num = 0;
            foreach (var user in result.Data)
            {

                Users.Add(new UsersModel
                {
                    Num = num++,
                    Id = user.Id,
                    UserName = user.UserName,
                    TestLevel = (user.Privilege & (int)UserPermissions.ATE) == (int)UserPermissions.ATE,
                    ProjectLevel = (user.Privilege & (int)UserPermissions.ATEConfig) == (int)UserPermissions.ATEConfig,
                    DataLevel = (user.Privilege & (int)UserPermissions.DIOS) == (int)UserPermissions.DIOS,
                    UserLevel = (user.Privilege & (int)UserPermissions.UserManage) == (int)UserPermissions.UserManage,
                    DBC = (user.Privilege & (int)UserPermissions.DBCConfig) == (int)UserPermissions.DBCConfig,
                    Manual = (user.Privilege & (int)UserPermissions.Manual) == (int)UserPermissions.Manual,
                    DeviceConfig = (user.Privilege & (int)UserPermissions.DeviceConfig) == (int)UserPermissions.DeviceConfig,
                });
            }
        }
        #endregion

        #region 保护方法
        protected override void WindowLoadedExecute(object obj)
        {
            if (!(obj is Window win))
            {
                return;
            }
            Win = win;
            WindowLeftDown_MoveEvent.LeftDown_MoveEventRegister(win);
            if (!DBOperate.Default.IsInitOK)
            {
                DBOperate.Default.Init();
            }

            UserName = GlobalModel.UserInfo.UserName;
            UserMannage = (GlobalModel.UserInfo.Privilege & 8) == 8 ? Visibility.Visible : Visibility.Collapsed;

            RefreshUser();
            //OperateResult result1 = DBOperate.Default.Init();
        }

        protected override void WindowClosedExecute(object obj)
        {
            base.WindowClosedExecute(obj);
            //((ViewModelLocator)Application.Current.FindResource("Locator")).Portal.dicViewopen["UserManage.UserMainView"] = false;
        }
        #endregion

        #region 构造方法

        public UserMainViewModel()
        {

            //UserName = GlobalModel.UserInfo.UserName;
            //OperateResult result1 = DBOperate.Default.Init();
            //OperateResult<List<UserInfo>> result= DBOperate.Default.GetAllusers();
            //if (!result)
            //{
            //    MessageBox.Show("用户信息查询失败，请重新查询！");
            //    return;
            //}
            //Users.Clear();
            //foreach (var user in result.Data)
            //{
            //    Users.Add(new UsersModel
            //    {
            //       Id = user.Id,
            //        UserName=user.UserName,
            //        TestLevel= (user.Privilege & 1) == 1 ? true : false,
            //        ProjectLevel = (user.Privilege & 2) == 2 ? true : false,
            //        DataLevel = (user.Privilege & 4) == 4 ? true : false,
            //    });
            //}
        }

        //public UserMainViewModel(string[] args)
        //{
        //    GlobalModel.UserInfo.UserName = args[0];
        //    GlobalModel.UserInfo.Password = args[1];
        //    GlobalModel.UserInfo.Privilege =Convert.ToInt16(args[2]);
        //}

        #endregion
    }
    public class UsersModel : ExtendViewModelBase
    {
        /// <summary>
        /// 序号
        /// </summary>
        public int Num { get; set; }
        /// <summary>
        /// 用户Id
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName { get; set; }

        private bool testLevel;
        /// <summary>
        /// 测试权限
        /// </summary>
        public bool TestLevel
        {
            get => testLevel;
            set => Set(nameof(TestLevel), ref testLevel, value);
        }
        private bool projectLevel;
        /// <summary>
        /// 项目配置权限
        /// </summary>
        public bool ProjectLevel
        {
            get => projectLevel;
            set => Set(nameof(ProjectLevel), ref projectLevel, value);
        }

        private bool dbc;
        /// <summary>
        /// DBC配置权限
        /// </summary>
        public bool DBC
        {
            get => dbc;
            set => Set(nameof(DBC), ref dbc, value);
        }
        
        private bool manual;
        /// <summary>
        /// 手动操作权限
        /// </summary>
        public bool Manual
        {
            get => manual;
            set => Set(nameof(Manual), ref manual, value);
        }

        
        private bool deviceconfig;
        /// <summary>
        /// 串口配置权限
        /// </summary>
        public bool DeviceConfig
        {
            get => deviceconfig;
            set => Set(nameof(DeviceConfig), ref deviceconfig, value);
        }

        private bool dataLevel;
        /// <summary>
        /// 数据查询权限
        /// </summary>
        public bool DataLevel
        {
            get => dataLevel;
            set => Set(nameof(DataLevel), ref dataLevel, value);
        }

        private bool userLevel;
        /// <summary>
        /// 用户管理权限
        /// </summary>
        public bool UserLevel
        {
            get => userLevel;
            set => Set(nameof(UserLevel), ref userLevel, value);
        }

        public RelayCommand<object> Delete => new RelayCommand<object>(DeleteUser);

        private void DeleteUser(object obj)
        {
            try
            {
                if (MessageBox.Show($"确认删除[{UserName}]用户", "提示", MessageBoxButton.OKCancel, MessageBoxImage.Information) == MessageBoxResult.OK)
                {
                    OperateResult result = DBOperate.Default.DeleteUserinfo(Convert.ToInt32(obj));
                    if (!result)
                    {
                        MessageBox.Show($"{result.Message}");
                        return;
                    }
                    MessageBox.Show($"[{UserName}]用户删除成功！");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        public RelayCommand<object> Modifiy => new RelayCommand<object>(ModifiyUserPrivilege);

        private void ModifiyUserPrivilege(object obj)
        {
            try
            {
                int privilege = (TestLevel ? (int)UserPermissions.ATE : 0)
                            + (DataLevel ? (int)UserPermissions.DIOS : 0)
                            + (UserLevel ? (int)UserPermissions.UserManage : 0)
                            + (DBC ? (int)UserPermissions.DBCConfig : 0)
                            + (DeviceConfig ? (int)UserPermissions.DeviceConfig : 0)
                            + (Manual ? (int)UserPermissions.Manual : 0);
                if (MessageBox.Show($"确认修改[{UserName}]用户权限", "提示", MessageBoxButton.OKCancel, MessageBoxImage.Information) == MessageBoxResult.OK)
                {
                    OperateResult result = DBOperate.Default.UpdateUserPrivilege(Convert.ToInt32(obj), privilege);
                    if (!result)
                    {
                        MessageBox.Show($"{result.Message}");
                        return;
                    }
                    MessageBox.Show($"[{UserName}]用户权限修改成功！");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
