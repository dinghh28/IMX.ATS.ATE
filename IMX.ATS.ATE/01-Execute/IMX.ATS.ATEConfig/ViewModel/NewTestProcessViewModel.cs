#region << 版 本 注 释 >>
/*----------------------------------------------------------------
 * 版权所有 (c) 2024   保留所有权利。
 * CLR版本：4.0.30319.42000
 * 机器名称：LAPTOP-9Q9TTD5V
 * 公司名称：
 * 命名空间：IMX.ATS.ATEConfig
 * 唯一标识：14afe029-4cdd-40f2-ae31-3177295bc569
 * 文件名：NewTestProcessViewModel
 * 当前用户域：LAPTOP-9Q9TTD5V
 * 
 * 创建者：58274
 * 创建时间：2024/10/17 16:14:48
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
using IMX.DB.Model;
using IMX.DB;
using Super.Zoo.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using IMX.Function.ViewModel.Model;
using System.Collections.ObjectModel;
using IMX.Common;
using System.Runtime.Remoting.Metadata.W3cXsd2001;

namespace IMX.ATS.ATEConfig
{
    public class NewTestProcessViewModel : ExtendViewModelBase
    {
        #region 公共属性

        #region 界面绑定属性
        private string schemeName = "";
        /// <summary>
        /// 试验流程名称
        /// </summary>
        public string SchemeName
        {
            get => schemeName;
            set => Set(nameof(SchemeName), ref schemeName, value);
        }

        private ObservableCollection<string> schemenames = new ObservableCollection<string>();
        /// <summary>
        /// 当前可新增试验项列表
        /// </summary>
        public ObservableCollection<string> SchemeNames
        {
            get => schemenames;
            set => Set(nameof(SchemeNames), ref schemenames, value);
        }


        private string schemeDescribe = "";
        /// <summary>
        /// 试验流程描述
        /// </summary>
        public string SchemeDescribe
        {
            get => schemeDescribe;
            set => Set(nameof(SchemeDescribe), ref schemeDescribe, value);
        }
        #endregion

        #region 界面绑定指令
        public RelayCommand<object> SaveSchemeCommannd => new RelayCommand<object>(SaveScheme);
        #endregion

        public bool IsOpen { get; set; } = false;

        #endregion

        #region 私有变量
        #endregion

        #region 私有方法
        private void SaveScheme(object obj)
        {
            try
            {
                int id = GlobalModel.Test_ProjectInfo.Id;
                //int id = ((ViewModelLocator)Application.Current.FindResource("Locator")).Main.ProjectInfo.Id;
                var testprocess = ((ViewModelLocator)Application.Current.FindResource("Locator")).TestProcess;
                var funtion = testprocess.FunctionInfos;
                List<string> SchemeNames = testprocess.SolutionNames.ToList();



                //foreach (var item in funtion)
                //{
                //    OperateResult<string> result = item.Model.Func.Config.ToJson();

                //    mod.Add(new ModTestProcess
                //    {

                //        Step = item.Step,
                //        CustomName = item.CutomFuncName,
                //        Description = item.Content,
                //        FuntionName = item.FunctionName,
                //        Type = item.ModType.ToString(),
                //        Funtion = result ? result.Data : string.Empty,
                //    });
                //}

                if (SchemeName == null)
                { MessageBox.Show("请填写【试验流程方案名称】！"); return; }
                if (SchemeNames.Contains(SchemeName))
                { MessageBox.Show($"【{SchemeName}】已存在！"); return; }


                List<ModTestProcess> mod = new List<ModTestProcess>();
                var config = SupportConfig.DicProcessConfig[SchemeName];

                if (MessageBox.Show($"是否引用测试项【{testprocess.SolutionName}】的测试步骤?","操作步骤复制提醒", MessageBoxButton.YesNo)== MessageBoxResult.Yes)
                {
                    for (int i = 0; i < funtion?.Count; i++)
                    {
                        FunctionInfo item = funtion[i];
                        string data = string.Empty;
                        if (item.ModType is not IMX.Function.Base.FuncitonType.Startup and not IMX.Function.Base.FuncitonType.Shutdown)
                        {
                            item.Model.Func.Config.ToJson()
                                .AttachIfSucceed(result => data = result.Data);
                        }

                        if (!config.UseCustomData && item.ModType== IMX.Function.Base.FuncitonType.CustomRevData)
                        {
                            continue;
                        }

                        mod.Add(new ModTestProcess
                        {

                            Step = item.Step,
                            CustomName = item.CutomFuncName,
                            Description = item.Content,
                            FuntionName = item.FunctionName,
                            Type = item.ModType.ToString(),
                            Funtion = data,
                        });
                    }
                }
                
                #region 试验存储数据加载
                List<ModTestDataInfo> eupreaddata = config.Test_ReadData_Euq;
                List<ModTestDataInfo> eupsetdata = config.Test_SetData_Euq;
                List<ModTestDataInfo> proreaddata = new List<ModTestDataInfo>();
                List<ModTestDataInfo> prosetdata = new List<ModTestDataInfo>();
                List<ModTestDataInfo> costomreaddata = new List<ModTestDataInfo>();
                List<ModTestDataInfo> calculatedata = new List<ModTestDataInfo>();

                for (int i = 0; i < GlobalModel.TestDBCconfig.Test_DBCReceiveSignals.Count; i++)
                {
                    proreaddata.Add(new ModTestDataInfo { Name = GlobalModel.TestDBCconfig.Test_DBCReceiveSignals[i].Custom_Name });
                }

                for (int i = 0; i < GlobalModel.TestDBCconfig.Test_DBCSendSignals.Count; i++)
                {
                    var signal = GlobalModel.TestDBCconfig.Test_DBCSendSignals[i];
                    if (signal.Custom_Name != signal.Signal_Name)
                    {
                        prosetdata.Add(new ModTestDataInfo { Name = signal.Custom_Name });
                    }
                }

                if (config.UseCustomData)
                {
                    costomreaddata.AddRange(config.Test_CustomData);
                }

                if (config.UseCalculate)
                {
                    calculatedata.AddRange(config.Test_CalculateData);
                    if (GlobalModel.NowElectricity == ATE.Common.Electricity.Three)
                    {
                        calculatedata.AddRange(config.Test_CalculateDataEX);
                    }
                }

                if (GlobalModel.NowElectricity == ATE.Common.Electricity.Three
                || GlobalModel.NowElectricity == ATE.Common.Electricity.ThreeANDInversion)
                {
                    eupreaddata.AddRange(config.Test_ReadData_EX);
                    eupsetdata.AddRange(config.Test_SetData_Ex);
                }
                #endregion

                OperateResult DbRtl = DBOperate.Default.InsertTestProccess(new Test_Process
                {
                    ProjectID = id,
                    FunctionName = SchemeName,
                    Description = SchemeDescribe,
                    Test_Flows = mod,
                    Test_ReadData_Euq = eupreaddata,
                    Test_ReadData_Pro = proreaddata,
                    Test_SetData_Euq = eupsetdata,
                    Test_SetData_Pro = prosetdata,
                    Test_CustomData = costomreaddata,
                    UseCustomData = config.UseCustomData,
                    UseCalculateData = config.UseCalculate,
                    Test_CalculateData = calculatedata,
                });

                if (!DbRtl)
                {
                    MessageBox.Show($"配置信息存储失败:{DbRtl.Message}");
                    return;
                }

                //添加方案名称至SolutionTestFlow
                testprocess.SolutionNames.Add(SchemeName);
                testprocess.DicDescription.Add(SchemeName, SchemeDescribe);
                Thread.Sleep(10);

                //修改方案名称至SolutionTestFlow
                testprocess.SolutionName = SchemeName;
                ////修改方案描述至SolutionTestFlow
                //testprocess.SolutionDescription = SchemeDescribe;

                MessageBox.Show($"【{SchemeName}】保存成功！");

                (obj as Window).Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"【{SchemeName}】保存失败：{ex.Message}");
            }
        }
        #endregion

        #region 保护方法
        protected override void WindowLoadedExecute(object obj)
        {
            SchemeName = "";
            SchemeDescribe = "";
            IsOpen = true;
            //base.WindowLoadedExecute(obj);
        }

        protected override void WindowClosedExecute(object obj)
        {
            IsOpen = false;
            base.WindowClosedExecute(obj);
        }
        #endregion

        #region 构造方法
        public NewTestProcessViewModel() { }
        #endregion

    }
}
