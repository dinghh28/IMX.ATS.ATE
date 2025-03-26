#region << 版 本 注 释 >>
/*----------------------------------------------------------------
 * 版权所有 (c) 2025   保留所有权利。
 * CLR版本：4.0.30319.42000
 * 机器名称：LAPTOP-9Q9TTD5V
 * 公司名称：
 * 命名空间：IMX.ATS.DBCConfig.ViewModel
 * 唯一标识：b09f59ec-051f-4fec-9e90-ef0e5ffdb779
 * 文件名：SelectViewModel
 * 当前用户域：LAPTOP-9Q9TTD5V
 * 
 * 创建者：58274
 * 创建时间：2025/3/4 17:12:33
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
using IMX.WPF.Resource;
using Super.Zoo.Framework;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Markup;
using Application = System.Windows.Application;
using MessageBox = System.Windows.Forms.MessageBox;

namespace IMX.ATS.DBCConfig
{
    public class SelectViewModel : ExtendViewModelBase
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
        /// 页面跳转指令
        /// </summary>
        public RelayCommand<object> ReturnConfig => new RelayCommand<object>((object obj) =>
        {
            try
            {
                if (obj.ToString().ToUpper() == "N")
                {
                    GlobalModel.IsNew = true;
                    GlobalModel.Test_DBC = new Test_DBCConfig();
                    GlobalModel.Test_DBCFileInfo = new Test_DBCFileInfo();
                }
                else
                {
                    if (SelectedIndex == -1)
                    {
                        MessageBox.Show($"请选择配置或新建配置！");
                        return;
                    }

                    GlobalModel.Test_DBC = SearchDBCConfigInfos[SelectedIndex].Config;

                    if (GlobalModel.Test_DBC.EnableUse)
                    {
                        DBOperate.Default.GetFile_ByID(GlobalModel.Test_DBC.DBCFileID)
                        .AttachIfSucceed(result =>
                        {
                            GlobalModel.Test_DBCFileInfo = result.Data ?? new Test_DBCFileInfo();
                            try
                            {
                                if (!Directory.Exists(SupportConfig.DBCFileDownPath))
                                {
                                    Directory.CreateDirectory(SupportConfig.DBCFileDownPath);
                                }

                                string path = Path.Combine(SupportConfig.DBCFileDownPath, GlobalModel.Test_DBCFileInfo.FileName + GlobalModel.Test_DBCFileInfo.FileExtension);

                                File.WriteAllBytes(path, GlobalModel.Test_DBCFileInfo.FileContent);
                            }
                            catch (Exception ex)
                            {
                                SuperDHHLoggerManager.Exception(LoggerType.FROMLOG, nameof(SelectViewModel), nameof(ReturnConfig), ex);
                            }
                        })
                        .AttachIfFailed(result => MessageBox.Show(result.Message, "获取DBC配置文件失败"));
                    }
                }

                Application.Current.Dispatcher.Invoke(new Action(() =>
                {
                    MainViewModel viewmodel = ((ViewModelLocator)Application.Current.FindResource("Locator")).Main;
                    Window mainwindow = ContentControlManager.GetWindow<MainView>(viewmodel);
                    mainwindow.Show();
                    try
                    {
                        WindowClosedExecute(Win);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.GetMessage(), "当前窗口关闭异常，请手动关闭");
                    }

                }));

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.GetMessage(), "DBC配置界面开启异常，请关闭上位机重试");
            }
        });

        public RelayCommand UpLoad => new RelayCommand(() => 
        {
            

            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                FileUpLoadViewModel viewmodel = ((ViewModelLocator)Application.Current.FindResource("Locator")).FileUpLoad;
                Window mainwindow = ContentControlManager.GetWindow<FileUpLoadView>(viewmodel);
                viewmodel.IsOpen = true;
                mainwindow.Show();
                
                //try
                //{
                //    WindowClosedExecute(Win);
                //}
                //catch (Exception ex)
                //{
                //    MessageBox.Show(ex.GetMessage(), "当前窗口关闭异常，请手动关闭");
                //}

            }));
        });

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
        #endregion

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
        #endregion

        #region 保护方法
        protected override void WindowLoadedExecute(object obj)
        {
            if (!(obj is Window win))
            {
                return;
            }

            //获取当前窗口
            Win = win;
            WindowLeftDown_MoveEvent.LeftDown_MoveEventRegister(Win);

            SearchStr = string.Empty;

            if (!DBOperate.Default.IsInitOK)
            {
                DBOperate.Default.Init();
                DBOperate.Default.UpdateOperator = GlobalModel.UserInfo.UserName;
            }

            DBOperate.Default.SelectedDBCConfig_All()
                .AttachIfSucceed(result =>
            {
                LsDBCConfigInfos.Clear();
                SearchDBCConfigInfos.Clear();
                result.Data.ForEach(item => { LsDBCConfigInfos.Add(new DBCConfigInfo { Config = item }); });

                for (int i = 0; i < result.Data?.Count; i++)
                {
                    var config = new DBCConfigInfo { Config = result.Data[i] };
                    LsDBCConfigInfos.Add(config);
                    SearchDBCConfigInfos.Add(config);
                }
            });


        }

        protected override void WindowClosedExecute(object obj)
        {
            WindowLeftDown_MoveEvent.LeftDown_MoveEventUnRegister(Win);

            base.WindowClosedExecute(obj);
        }
        #endregion


        #region 构造方法
        public SelectViewModel() { }
        #endregion
    }

    /// <summary>
    /// DBC配置信息
    /// </summary>
    public class DBCConfigInfo:ViewModelBase
    {
        /// <summary>
        /// DBC配置
        /// </summary>
        public Test_DBCConfig Config { get; set; }

        /// <summary>
        /// DBC文件配置
        /// </summary>
        public Test_DBCFileInfo FileInfo { get; set; }
    }
}
