#region << 版 本 注 释 >>
/*----------------------------------------------------------------
 * 版权所有 (c) 2024   保留所有权利。
 * CLR版本：4.0.30319.42000
 * 机器名称：LAPTOP-9Q9TTD5V
 * 公司名称：
 * 命名空间：IMX.ATS.ProjectConfig
 * 唯一标识：dbe7e256-5cbf-42cc-bb41-e4d0ca98c5a5
 * 文件名：PCDBCFileChangeViewModel
 * 当前用户域：LAPTOP-9Q9TTD5V
 * 
 * 创建者：58274
 * 创建时间：2024/7/8 11:10:30
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

using GalaSoft.MvvmLight.Command;
using H.WPF.Framework;
using IMX.DB;
using IMX.DB.Model;
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
using System.Windows.Shapes;
using Path = System.IO.Path;

namespace IMX.ATS.DBCConfig
{
    public class DBCFileChangeViewModel : ExtendViewModelBase
    {


        #region 公共属性

        #region 界面绑定属性
        private ObservableCollection<Test_DBCFileInfo> dbcfileinfos = new ObservableCollection<Test_DBCFileInfo>();
        /// <summary>
        /// DBC文件检索列表
        /// </summary>
        public ObservableCollection<Test_DBCFileInfo> DBCFileInfos
        {
            get => dbcfileinfos;
            set => Set(nameof(DBCFileInfos), ref dbcfileinfos, value);
        }

        private int selecteddbcindex;
        /// <summary>
        /// 当前选择DBC文件Index
        /// </summary>
        public int SelectedDBCIndex
        {
            get => selecteddbcindex;
            set => Set(nameof(SelectedDBCIndex), ref selecteddbcindex, value);
        }

        private string searchstr = string.Empty;
        /// <summary>
        /// 检索字符串
        /// </summary>
        public string SearchStr
        {
            get => searchstr;
            set => Set(nameof(SearchStr), ref searchstr, value);
        }
        #endregion

        #region 界面绑定指令

        public RelayCommand UpLoad => new RelayCommand(() =>
        {
           // Win.Topmost = false;

            FileUpLoadViewModel viewmodel = ((ViewModelLocator)Application.Current.FindResource("Locator")).FileUpLoad;
            Window mainwindow = ContentControlManager.GetWindow<FileUpLoadView>(viewmodel);

            if (viewmodel.IsOpen) { MessageBox.Show($"界面已打，请勿重复操作！", "界面提示", MessageBoxButton.OK, MessageBoxImage.Information); return; }

            Application.Current.Dispatcher.Invoke(new Action(() =>
            {

                viewmodel.IsOpen = true;
                mainwindow.Show();
                mainwindow.Topmost = true;


                //try
                //{
                //    WindowClosedExecute(Win);
                //}
                //catch (Exception ex)
                //{
                //    MessageBox.Show(ex.GetMessage(), "当前窗口关闭异常，请手动关闭");
                //}
            }));

            Task.Run(() =>
            {
                Task.Delay(500);
                while (true)
                {
                    if (!viewmodel.IsOpen)
                    {
                        Application.Current.Dispatcher.Invoke(new Action(() =>
                        {
                            //Win.Topmost = true;
                            fileinfos.Clear();
                            DBCFileInfos.Clear();
                            SearchStr = string.Empty;

                            DBOperate.Default.GetFiles().AttachIfSucceed(result =>
                            {
                                fileinfos = result.Data;
                                for (int i = 0; i < result.Data.Count; i++)
                                {
                                    DBCFileInfos.Add(result.Data[i]);
                                }
                            });
                        }));
                        return;
                    }
                    Task.Delay(100);
                }
            });


        });

        public RelayCommand Submit => new RelayCommand(SubmitChange);

        public RelayCommand Delete => new RelayCommand(DeleteFile);

        /// <summary>
        /// 项目信息检索指令
        /// </summary>
        public RelayCommand Search => new RelayCommand(() =>
        {
            DBCFileInfos.Clear();
            List<Test_DBCFileInfo> data = new List<Test_DBCFileInfo>();

            if (string.IsNullOrEmpty(SearchStr))
            {
                data = fileinfos;
            }
            else
            {
                data = fileinfos.FindAll(x => x.FileName.Contains(SearchStr));
            }

            for (int i = 0; i < data.Count; i++)
            {
                DBCFileInfos.Add(data[i]);
            }
        });

        #endregion

        /// <summary>
        /// DBC信号配置ID
        /// </summary>
        public int ConfigID { get; set; } = -1;
        #endregion

        #region 公有变量
        /// <summary>
        /// 当前窗口
        /// </summary>
        private Window Win = null;

        public bool IsOpen = false;

        #endregion

        #region 私有变量
        private List<Test_DBCFileInfo> fileinfos = new List<Test_DBCFileInfo>();
        #endregion

        #region 私有方法

        private void SubmitChange()
        {
            if (DBCFileInfos.Count == 0)
            {
                MessageBox.Show("库中不存在DBC文件，无法进行变更操作", "变更提交异常");
                return;
            }

            if (SelectedDBCIndex == -1)
            {
                MessageBox.Show("请先选择DBC文件", "变更提交异常");
                return;
            }

            Test_DBCFileInfo info = DBCFileInfos[SelectedDBCIndex];

            string path;

            try
            {
                if (!Directory.Exists(SupportConfig.DBCFileDownPath))
                {
                    Directory.CreateDirectory(SupportConfig.DBCFileDownPath);
                }

                path = Path.Combine(SupportConfig.DBCFileDownPath, info.FileName + info.FileExtension);

                File.WriteAllBytes(path, info.FileContent);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.GetMessage(), "DBC文件变更异常");
                return;
            }

            GlobalModel.TestDBCFileInfo_Change = info;

            WindowClosedExecute(Win);
        }

        private void DeleteFile()
        {
            if (DBCFileInfos.Count == 0)
            {
                MessageBox.Show("库中不存在DBC文件，无法进行删除操作", "删除文件异常");
                return;
            }
            if (SelectedDBCIndex == -1)
            {
                MessageBox.Show("请先选择DBC文件", "删除文件异常");
                return;
            }

            Test_DBCFileInfo info = DBCFileInfos[SelectedDBCIndex];

            var dbcconfigmodel = ((ViewModelLocator)Application.Current.FindResource("Locator")).DBCConfig;

            if (MessageBox.Show("确认是否删除选择DBC文件", "删除文件", MessageBoxButton.OKCancel, MessageBoxImage.Question) == MessageBoxResult.OK)
            {
                DBOperate.Default.UpdateDBCEnableState_False(info.Id)
                    .And(DBOperate.Default.DeleteDBCFile(info.Id))
                   .AttachIfSucceed(result =>
                   {
                       if (info.Id == GlobalModel.Test_DBCFileInfo.Id)
                       {
                           GlobalModel.Test_DBCFileInfo.IsDeleted = true;
                       }

                       MessageBox.Show($"DBC文件【{info.FileName}】已删除\r\n", "DBC文件删除成功");
                   })
                   .AttachIfFailed(result => { MessageBox.Show(result.Message, "DBC文件删除失败"); });
                fileinfos.Clear();
                DBCFileInfos.Clear();
                DBOperate.Default.GetFiles().AttachIfSucceed(result =>
                {
                    fileinfos = result.Data;
                    for (int i = 0; i < result.Data.Count; i++)
                    {
                        DBCFileInfos.Add(result.Data[i]);
                    }
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

            WindowLeftDown_MoveEvent.LeftDown_MoveEventRegister(win);
            Win = win;

            //Window win = new MouseLeftButtonDown().MouseLeft(obj);
            //if (win != null) { Win = win; }

            fileinfos.Clear();
            DBCFileInfos.Clear();
            SearchStr = string.Empty;

            DBOperate.Default.GetFiles().AttachIfSucceed(result =>
            {
                fileinfos = result.Data;
                for (int i = 0; i < result.Data.Count; i++)
                {
                    DBCFileInfos.Add(result.Data[i]);
                }
            });

            IsOpen = true;
            //base.WindowLoadedExecute(obj);
        }

        protected override void WindowClosedExecute(object obj)
        {
            //if (GlobalModel.TestDBCFileInfo.Id == 0 && GlobalModel.Test_ProjectInfo.IsUseDDBC)
            //{
            //    MessageBox.Show("此项目未配置DBC文件，请选择变更的DBC文件");
            //    return;
            //}
            IsOpen = false;
            base.WindowClosedExecute(obj);
        }
        #endregion


        #region 构造方法
        public DBCFileChangeViewModel() { }
        #endregion

    }
}
