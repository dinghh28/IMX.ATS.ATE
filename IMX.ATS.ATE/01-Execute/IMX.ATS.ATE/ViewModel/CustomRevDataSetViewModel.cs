#region << 版 本 注 释 >>
/*----------------------------------------------------------------
 * 版权所有 (c) 2025   保留所有权利。
 * CLR版本：4.0.30319.42000
 * 机器名称：LAPTOP-9Q9TTD5V
 * 公司名称：
 * 命名空间：IMX.ATS.ATE.ViewModel
 * 唯一标识：9f0d43de-e01b-45e5-8fd7-e4fd2063242a
 * 文件名：CustomRevDataSetViewModel
 * 当前用户域：LAPTOP-9Q9TTD5V
 * 
 * 创建者：58274
 * 创建时间：2025/7/1 16:31:01
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
using IMX.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace IMX.ATS.ATE
{
    public class CustomRevDataSetViewModel : WindowViewModelBaseEx
    {

        #region 公共属性

        #region 界面绑定属性

        private ObservableCollection<ModTestDataInfo> datas = new ObservableCollection<ModTestDataInfo>();
        /// <summary>
        /// 输入数据列表
        /// </summary>
        public ObservableCollection<ModTestDataInfo> Datas
        {
            get => datas;
            set => Set(nameof(Datas), ref datas, value);
        }

        #endregion

        #region 界面绑定指令
        public RelayCommand Check => new RelayCommand(() => 
        {
            WindowClosedExecute(window);
        });
        #endregion

        public bool IsOpen { get; set; } = true;
        #endregion

        #region 私有变量
        private Window window;
        #endregion

        #region 私有方法
        #endregion

        #region 保护方法
        protected override void WindowLoadedExecute(object obj)
        {
            if (obj is not Window win)
            {
                return;
            }

            window = win;
            window.Topmost = true;
            //base.WindowLoadedExecute(obj);
        }

        protected override void WindowClosedExecute(object obj)
        {
            IsOpen = false;
            base.WindowClosedExecute(obj);
        }
        #endregion

        #region 构造方法
        public CustomRevDataSetViewModel() { }

        public CustomRevDataSetViewModel(List<ModTestDataInfo> infos) 
        {
            Datas.Clear();
            for (int i = 0; i < infos?.Count; i++)
            {
                Datas.Add(infos[i]);
            }

        }
        #endregion

    }
}
