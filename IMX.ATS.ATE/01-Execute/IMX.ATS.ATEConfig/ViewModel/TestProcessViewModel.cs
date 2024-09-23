#region << 版 本 注 释 >>
/*----------------------------------------------------------------
 * 版权所有 (c) 2024   保留所有权利。
 * CLR版本：4.0.30319.42000
 * 机器名称：LAPTOP-9Q9TTD5V
 * 公司名称：
 * 命名空间：IMX.ATS.ATEConfig.ViewModel
 * 唯一标识：39d088c8-7d00-47b9-ac17-092f477a3e73
 * 文件名：TestProcessViewModel
 * 当前用户域：LAPTOP-9Q9TTD5V
 * 
 * 创建者：58274
 * 创建时间：2024/9/13 15:52:57
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
    public class TestProcessViewModel : ExtendViewModelBase
    {

        #region 公共属性

        #region 界面绑定属性

        #region 试验方法信息
        private ObservableCollection<string> solutionNames = new ObservableCollection<string>();
        /// <summary>
        /// 试验方案列表
        /// </summary>
        public ObservableCollection<string> SolutionNames
        {
            get => solutionNames;
            set => Set(nameof(SolutionNames), ref solutionNames, value);
        }

        private string solutionName;
        /// <summary>
        /// 当前试验方案名称
        /// </summary>
        public string SolutionName
        {
            get => solutionName;
            set
            {
                if (Set(nameof(SolutionName), ref solutionName, value))
                {
                    //SelectSolution();
                }
            }
        }

        private string solutionDescription;
        /// <summary>
        /// 当前试验方案描述
        /// </summary>
        public string SolutionDescription
        {
            get => solutionDescription;
            set => Set(nameof(SolutionDescription), ref solutionDescription, value);
        }
        #endregion

        private FrameworkElement configContent;
        /// <summary>
        /// 详细配置内容
        /// </summary>
        public FrameworkElement ConfigContent
        {
            get => configContent;
            set => Set(nameof(ConfigContent), ref configContent, value);
        }

        #region 配置信息

        private ObservableCollection<FunctionInfo> functionInfos = new ObservableCollection<FunctionInfo>();
        /// <summary>
        /// 操作步骤配置列表
        /// </summary>
        public ObservableCollection<FunctionInfo> FunctionInfos
        {
            get => functionInfos;
            set => Set(nameof(FunctionInfos), ref functionInfos, value);
        }
        #endregion
        #endregion

        #region 界面绑定指令
        #endregion

        #endregion

        #region 私有变量
        #endregion

        #region 私有方法

        private void ShowFunction(int index)
        {
            if (index == -1)
            {
                ConfigContent = null;
                return;
            }

            try
            {
                if (index > FunctionInfos.Count)
                {
                    MessageBox.Show($"选择步骤超方案已有步骤范围");
                    return;
                }

                //string winname = SupportConfig.DicTestFlowItems.First(x => x.Value == FunctionInfos[index].FunctionName).Key;
                string winname = functionInfos[index].FunctionName;

                Type win = Type.GetType($"IMX.ATS.MCU.Function.FunView{winname}");

                ConfigContent = ContentControlManager.GetControl(win, FunctionInfos[index].Model);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.GetMessage());
                return;
            }

        }
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
        public TestProcessViewModel() { }
        #endregion

    }
}
