#region << 版 本 注 释 >>
/*----------------------------------------------------------------
 * 版权所有 (c) 2024   保留所有权利。
 * CLR版本：4.0.30319.42000
 * 机器名称：LAPTOP-9Q9TTD5V
 * 公司名称：
 * 命名空间：IMX.ATS.ATEConfig.Function.ViewModel
 * 唯一标识：27196f6c-e44a-4f57-90b5-69b06206c79c
 * 文件名：FunViewModelPOPUP
 * 当前用户域：LAPTOP-9Q9TTD5V
 * 
 * 创建者：58274
 * 创建时间：2024/10/23 15:12:08
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
using IMX.Function;
using IMX.Function.Base;
using IMX.Function.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMX.ATS.ATEConfig.Function
{
    /// <summary>
    /// 弹窗配置模板
    /// </summary>
    public class FunViewModelPOPUP : FunViewModel
    {
        #region 公共属性

        public override TestFunction Func { get; set; } = TestFunction.Create(FuncitonType.POPUP);

        public override FuncitonType SupportFuncitonType => FuncitonType.POPUP;

        public override string SupportFuncitonString => "POPUP";

        #region 界面绑定属性

        private string title="";
        /// <summary>
        /// 弹窗标题
        /// </summary>
        public string Title 
        {
            get => title = (Func.Config as FunConfig_POPUP).Title;
            set
            {
                if (Set(nameof(Title), ref title, value))
                {
                    (Func.Config as FunConfig_POPUP).Title = value;
                }
            }
        }

        private string description;
        /// <summary>
        /// 弹窗内容
        /// </summary>
        public string Description
        {
            get => description = (Func.Config as FunConfig_POPUP).Description;
            set
            {
                if (Set(nameof(Description), ref description, value))
                {
                    (Func.Config as FunConfig_POPUP).Description = value;
                }
            }
        }

        private bool isresult;
        /// <summary>
        /// 是否允许弹窗结果作为试验结果判定
        /// </summary>
        public bool IsResult
        {
            get => isresult = (Func.Config as FunConfig_POPUP).IsResult;
            set
            {
                if (Set(nameof(IsResult), ref isresult, value))
                {
                    (Func.Config as FunConfig_POPUP).IsResult = value;
                }
            }
        }

        #endregion

        #region 界面绑定指令
        #endregion

        #endregion

        #region 私有变量
        #endregion

        #region 私有方法
        #endregion

        #region 构造方法
        public FunViewModelPOPUP() { }


        #endregion
    }
}
