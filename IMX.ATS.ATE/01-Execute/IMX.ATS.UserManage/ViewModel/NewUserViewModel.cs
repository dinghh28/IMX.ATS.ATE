using GalaSoft.MvvmLight.Command;
using H.WPF.Framework;
using IMX.ATS.Common;
using IMX.DB;
using IMX.DB.Model;
using Super.Zoo.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace IMX.ATS.UserManage
{
    public class NewUserViewModel : ExtendViewModelBase
    {

        #region 公共属性

        #region 界面绑定属性

        private string userName;
        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName
        {
            get => userName;
            set => Set(nameof(UserName), ref userName, value);
        }

        private string passWord = "111111";
        /// <summary>
        /// 用户密码
        /// </summary>
        public string PassWord
        {
            get => passWord;
            set => Set(nameof(PassWord), ref passWord, value);
        }

        private bool testLevel;
        /// <summary>
        /// 测试权限
        /// </summary>
        public bool TestLevel
        {
            get => testLevel;
            set => Set(nameof(TestLevel), ref testLevel, value);
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
        private bool dataLevel;
        /// <summary>
        /// 数据查询权限
        /// </summary>
        public bool DataLevel
        {
            get => dataLevel;
            set => Set(nameof(DataLevel), ref dataLevel, value);
        }
        private bool projectLevel;
        /// <summary>
        /// 方案配置权限
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

        #endregion

        #region 界面绑定指令

        public RelayCommand AddNewUserCommond => new RelayCommand(AddnewUser);


        #endregion

        #endregion

        #region 私有变量

        #endregion
        #region 私有方法
        /// <summary>
        /// 增加用户
        /// </summary>
        private void AddnewUser()
        {
            try
            {
                int privilege = (TestLevel ? (int)UserPermissions.ATE : 0)
                            + (DataLevel ? (int)UserPermissions.DIOS : 0)
                            + (UserLevel ? (int)UserPermissions.UserManage : 0)
                            + (DBC ? (int)UserPermissions.DBCConfig : 0)
                            + (DeviceConfig ? (int)UserPermissions.DeviceConfig : 0)
                            + (Manual ? (int)UserPermissions.Manual : 0);
                UserInfo user = new UserInfo() { UserName = UserName, Password = "111111", Privilege = privilege };
                //OperateResult resu = DBOperate.Default.Init();
                OperateResult result = DBOperate.Default.AddNewUser(user);
                if (!result)
                {
                    MessageBox.Show($"{result.Message}");
                    return;
                }
                MessageBox.Show($"[{UserName}]用户增加成功！");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{ex.Message}");
            }
        }
        #endregion

        #region 保护方法
        protected override void WindowClosedExecute(object obj)
        {
            UserName = "";
            PassWord = "";
            TestLevel = false;
            ProjectLevel = false;
            UserLevel = false;
            DataLevel = false;
            DeviceConfig = false;
            Manual = false;
            DBC = false;
            base.WindowClosedExecute(obj);
        }
        #endregion
        #region 构造函数

        public NewUserViewModel() { }

        #endregion

    }
}
