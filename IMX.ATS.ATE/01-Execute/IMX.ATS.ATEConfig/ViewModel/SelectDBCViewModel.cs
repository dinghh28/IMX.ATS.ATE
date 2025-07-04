#region << 版 本 注 释 >>
/*----------------------------------------------------------------
 * 版权所有 (c) 2025   保留所有权利。
 * CLR版本：4.0.30319.42000
 * 机器名称：LAPTOP-9Q9TTD5V
 * 公司名称：
 * 命名空间：IMX.ATS.ATEConfig.ViewModel
 * 唯一标识：69502bee-fb86-407c-b808-2c43f62c3ffd
 * 文件名：SelectDBCViewModel
 * 当前用户域：LAPTOP-9Q9TTD5V
 * 
 * 创建者：58274
 * 创建时间：2025/4/1 17:16:42
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

using GalaSoft.MvvmLight.CommandWpf;
using H.WPF.Framework;
using IMX.ATE.Framework;
using IMX.DB;
using IMX.WPF.Resource;
using Super.Zoo.Framework;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace IMX.ATS.ATEConfig
{
    public class SelectDBCViewModel : WindowViewModelBaseEx
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

            GlobalModel.TestDBCconfig_Change = SearchDBCConfigInfos[SelectedIndex].Config;
            WindowClosedExecute(Win);
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

            DBOperate.Default.SelectedDBCConfig(GlobalModel.NowElectricity).AttachIfSucceed(result => 
            {
                result.Data.ForEach(item => { LsDBCConfigInfos.Add(new DBCConfigInfo { Config = item }); });

                for (int i = 0; i < result.Data?.Count; i++)
                {
                    var config = new DBCConfigInfo { Config = result.Data[i] };
                    LsDBCConfigInfos.Add(config);
                    SearchDBCConfigInfos.Add(config);
                }
            });
            //base.WindowLoadedExecute(obj);
        }

        protected override void WindowClosedExecute(object obj)
        {
            LsDBCConfigInfos.Clear();
            SearchDBCConfigInfos.Clear();

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
