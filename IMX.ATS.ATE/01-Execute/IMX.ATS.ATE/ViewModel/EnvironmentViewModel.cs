#region << 版 本 注 释 >>
/*----------------------------------------------------------------
 * 版权所有 (c) 2025   保留所有权利。
 * CLR版本：4.0.30319.42000
 * 机器名称：LAPTOP-9Q9TTD5V
 * 公司名称：
 * 命名空间：IMX.ATS.ATE.ViewModel
 * 唯一标识：9fbf5b73-f8d8-42c3-b1b1-75a91e885cc3
 * 文件名：EnvironmentViewModel
 * 当前用户域：LAPTOP-9Q9TTD5V
 * 
 * 创建者：58274
 * 创建时间：2025/4/25 14:58:21
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

using H.WPF.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace IMX.ATS.ATE
{
    public class EnvironmentViewModel : ExtendViewModelBase
    {
        #region 公共属性

        #region 界面绑定属性
        private IncubatorViewModel incubator = new IncubatorViewModel();
        /// <summary>
        /// 温箱
        /// </summary>
        public IncubatorViewModel Incubator
        {
            get => incubator;
            set => Set(nameof(Incubator), ref incubator, value);
        }

        private WaterBathViewModel waterbath;
        /// <summary>
        /// 水浴
        /// </summary>
        public WaterBathViewModel WaterBath
        {
            get => waterbath;
            set => Set(nameof(WaterBath), ref waterbath, value);
        }

        #endregion

        #region 界面绑定指令
        #endregion

        #endregion

        #region 私有变量
        #endregion

        #region 公共方法
        /// <summary>
        /// 开始水浴温箱通讯
        /// </summary>
        public void StartCommunication() 
        {
            new Thread(Incubator.IncubatorThread) { IsBackground = true }.Start();
        }
        #endregion

        #region 私有方法

        #endregion

        #region 保护方法
        protected override void WindowLoadedExecute(object obj)
        {
            //base.WindowLoadedExecute(obj);
        }

        protected override void WindowClosedExecute(object obj)
        {
            base.WindowClosedExecute(obj);
        }
        #endregion


        #region 构造方法
        public EnvironmentViewModel() { }
        #endregion

    }
}
