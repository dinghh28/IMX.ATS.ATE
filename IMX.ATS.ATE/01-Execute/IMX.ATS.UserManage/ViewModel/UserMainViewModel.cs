using GalaSoft.MvvmLight.Command;
using H.WPF.Framework;
using IMX.DB;
using IMX.DB.Model;
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

        private string userName;

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
        private string passWord;
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
        private Window Win=null;

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
                    TestLevel = (user.Privilege & 1) == 1,
                    ProjectLevel = (user.Privilege & 2) == 2,
                    DataLevel = (user.Privilege & 4) == 4,
                    UserLevel = (user.Privilege & 8) == 8,
                });
            }
        }
        #endregion

        #region 保护方法
        protected override void WindowLoadedExecute(object obj)
        {
            Window win = MouseLeft(obj);
            if (win != null) { Win = win; }
            
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

        //}

        #endregion

        public Window MouseLeft(object WindowsName)
        {
            if (!(WindowsName is Window win))
            {
                return null;
            }

            //获取当前窗口
            //Win = win;

            win.MouseLeftButtonDown += (s, e) =>
            {
                if (e.LeftButton == MouseButtonState.Pressed)
                    win.DragMove();
            };
            return win;
        }
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
                int privilege = (TestLevel ? 1 : 0) + (ProjectLevel ? 2 : 0) + (DataLevel ? 4 : 0) + (UserLevel ? 8 : 0);
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
            catch(Exception ex) 
            {
                MessageBox.Show(ex.Message );
            }
        }
    }
}
