﻿#region << 版 本 注 释 >>
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

using Force.DeepCloner;
using GalaSoft.MvvmLight.CommandWpf;
using H.WPF.Framework;
using IMX.DB;
using IMX.DB.Model;
using IMX.Function;
using IMX.Function.Base;
using IMX.Function.ViewModel;
using IMX.Function.ViewModel.Model;
using Newtonsoft.Json;
using Super.Zoo.Framework;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using MessageBox = System.Windows.Forms.MessageBox;
using FastDeepCloner;

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
                    SelectSolution();
                }
            }
        }

        private ObservableCollection<string> solutionDescriptions = new ObservableCollection<string>();
        /// <summary>
        /// 试验方案描述
        /// </summary>
        public ObservableCollection<string> SolutionDescriptions
        {
            get => solutionDescriptions;
            set => Set(nameof(SolutionDescriptions), ref solutionDescriptions, value);
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

        #region 试验模板
        private TestFlowItem selectedtestflowitem;
        /// <summary>
        /// 当前选中试验模板
        /// </summary>
        public TestFlowItem SelectedTestFlowItem
        {
            get => selectedtestflowitem;
            set => Set(nameof(SelectedTestFlowItem), ref selectedtestflowitem, value);
        }

        private List<TestFlowItem> testFlowItems = new List<TestFlowItem>();
        /// <summary>
        /// 试验模板列表
        /// </summary>
        public List<TestFlowItem> TestFlowItems
        {
            get { return testFlowItems; }
            set { testFlowItems = value; }
        }
        #endregion

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

        private int functionInfoIndex;
        /// <summary>
        /// 选中的试验步骤
        /// </summary>
        public int FunctionInfoIndex
        {
            get => functionInfoIndex;
            set
            {
                if (Set(nameof(FunctionInfoIndex), ref functionInfoIndex, value))
                {
                    Thread.Sleep(20);
                    ShowFunction(value);
                }
            }
        }


        #endregion

        #endregion

        #region 界面绑定指令

        /// <summary>
        /// 试验步骤变更指令
        /// </summary>
        public RelayCommand<object> ChangeFunInfos => new RelayCommand<object>(ChangedFunInfos);

        /// <summary>
        /// 试验方案更新指令
        /// </summary>
        public RelayCommand UpdataTestFlow => new RelayCommand(UpdataedTestFlow);

        /// <summary>
        /// 试验方案另存为指令
        /// </summary>
        public RelayCommand SaveAsTestFlow => new RelayCommand(SaveAsedTestFlow);

        /// <summary>
        /// 试验方案到导入指令
        /// </summary>
        public RelayCommand Import => new RelayCommand(ImportTestFlow);


        /// <summary>
        /// 试验方案导出指令
        /// </summary>
        public RelayCommand Export => new RelayCommand(ExportTestFlow);

        /// <summary>
        /// 试验方案删除指令
        /// </summary>
        public RelayCommand Delete => new RelayCommand(DeleteTestFlow);




        #endregion

        #endregion

        #region 私有变量
        private int projectid = -1;
        private string projectname = "";
        #endregion

        #region 私有方法

        /// <summary>
        /// 获取试验方案名称
        /// </summary>
        private void GetTestSolutions()
        {
            try
            {
                SolutionNames.Clear();
                DBOperate.Default.GetProcessName(projectid).AttachIfSucceed(result =>
                {
                    if (result.Data.Count < 1)
                    {
                        return;
                    }
                    for (int i = 0; i < result.Data.Count; i++)
                    {
                        SolutionNames.Add(result.Data[i]);
                    }
                    SolutionName = SolutionNames[0];
                });

                DBOperate.Default.GetProcessDescription(projectid).AttachIfSucceed(result =>
                {
                    if (result.Data.Count < 1)
                    {
                        return;
                    }

                    for (int i = 0; i < result.Data.Count; i++)
                    {
                        SolutionDescriptions.Add(result.Data[i]);
                    }
                    SolutionDescription = SolutionDescriptions[0];
                });

                //OperateResult<DataTable> relt = DBOperate.Default.GetTestFunctions(GlobalModel.TestInfo.Test_ID)
                //    .AttachIfSucceed(result => {
                //        for (int i = 0; i < result.Data.Rows.Count; i++)
                //        {
                //            SolutionNames.Add(result.Data.Rows[i][0].ToString());
                //        }
                //        SolutionName = SolutionNames[0];
                //    }).AttachIfFailed(result =>
                //    {
                //        MessageBox.Show($"试验方案名称加载异常:{result.Message}");
                //    });

            }
            catch (Exception ex)
            {
                MessageBox.Show($"试验流程列表获取异常:{ex.GetMessage()}");
            }
        }

        #region 试验模板操作
        /// <summary>
        /// 试验模板信息初始化
        /// </summary>
        private void TestFlowItemsInit()
        {
            if (TestFlowItems.Count > 0)
            {
                return;
            }

            foreach (var item in SupportConfig.DicTestFlowItems)
            {
                TestFlowItems.Add(new TestFlowItem { Tag = item.Key, Selcted = new RelayCommand<object>(AddFunction) });
                Thread.Sleep(10);
            }
        }
        #endregion

        #region 试验步骤操作
        /// <summary>
        /// 步骤列表重新排序
        /// </summary>
        private void ReNumber()
        {
            for (int i = 0; i < FunctionInfos.Count; i++)
            {
                FunctionInfos[i].Step = i + 1;
            }
        }

        /// <summary>
        /// 变更试验步骤
        /// </summary>
        /// <param name="obj">上/下移、删除</param>
        private void ChangedFunInfos(object obj)
        {
            int index = FunctionInfoIndex;
            if (FunctionInfos.Count == 0) return;

            switch (obj.ToString().ToUpper())
            {
                case "UP":
                    {
                        if (index < 0)
                        {
                            return;
                        }
                        if (index == 0)
                        { MessageBox.Show("已到达最高点，无法上移！", "提示！", MessageBoxButtons.OK, MessageBoxIcon.Information); return; }

                        FunctionInfos.Insert(index - 1, FunctionInfos[index]);
                        FunctionInfos.RemoveAt(index + 1);
                        FunctionInfoIndex = index - 1;
                        ReNumber();
                        Thread.Sleep(10);
                    }
                    break;
                case "DOWN":
                    {
                        if (index < 0)
                        {
                            return;
                        }
                        if (index == FunctionInfos.Count - 1)
                        { MessageBox.Show("已到达最低点，无法下移!", "提示！", MessageBoxButtons.OK, MessageBoxIcon.Information); return; }
                        FunctionInfos.Insert(index + 2, FunctionInfos[index]);
                        FunctionInfos.RemoveAt(index);
                        FunctionInfoIndex = index + 1;
                        ReNumber();
                        Thread.Sleep(10);
                    }
                    break;
                case "DELET":
                    {
                        if (FunctionInfos.Count < 1)
                        {
                            return;
                        }
                        FunctionInfos.RemoveAt(index);
                        if (FunctionInfos.Count < 1)
                        {
                            return;
                        }
                        FunctionInfoIndex = index == 0 ? index : index - 1;
                        ReNumber();
                        Thread.Sleep(10);
                    }
                    break;
                case "INSERT":
                    {
                        if (SelectedTestFlowItem == null)
                        {
                            MessageBox.Show("请选择需要插入的试验操作模板!");
                            return;
                        }

                        if (FunctionInfoIndex == -1)
                        {
                            MessageBox.Show("请选择需要插入到的试验操作步骤!");
                            return;
                        }

                        FuncitonType flowitemtag = SelectedTestFlowItem.Tag;

                        var rlt = FunViewModel.Create(SupportConfig.DicTestFlowItems[flowitemtag]);

                        if (!rlt)
                        {
                            MessageBox.Show($"操作无法添加:{rlt.Message}");
                            return;
                        }

                        FunctionInfos.Insert(FunctionInfoIndex, new FunctionInfo
                        {
                            Step = FunctionInfos.Count + 1,
                            //CutomFuncName = SupportConfig.DicTestFlowItems[flowitemtag],
                            //FunctionName = obj.ToString(),
                            CutomFuncName = SelectedTestFlowItem.Name,
                            FunctionName = flowitemtag.ToString(),
                            ModType = rlt.Data.SupportFuncitonType,
                            Model = rlt.Data
                        });

                        ReNumber();
                        Thread.Sleep(10);
                    }
                    break;
            }
        }

        /// <summary>
        /// 展示试验步骤
        /// </summary>
        /// <param name="index"></param>
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

                Type win = Type.GetType($"IMX.ATS.ATEConfig.Function.FunView{winname}");

                ConfigContent = ContentControlManager.GetControl(win, FunctionInfos[index].Model);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.GetMessage());
                return;
            }

        }

        /// <summary>
        /// 手动添加配置步骤
        /// </summary>
        /// <param name="obj"></param>
        private void AddFunction(object obj)
        {
            //FuncitonType type = (FuncitonType)Enum.Parse(typeof(FuncitonType), value: obj.ToString());

            if (!Enum.TryParse(obj.ToString(), out FuncitonType type))
            {
                MessageBox.Show($"无法获取当前{obj}类型操作步骤", "步骤添加异常");
                return;
            }

            //CAN相关配置
            if (type == FuncitonType.Product || type == FuncitonType.ProductResult)
            {
                if (!GlobalModel.Test_ProjectInfo.IsUseDDBC)
                {
                    MessageBox.Show("当前项目无产品通讯，请勿使用相关模板！（如需使用，请先切至产品信息界面开启 DBC 通讯）","产品相关模板添加失败");
                    return;
                }
                else if (GlobalModel.TestDBCconfig == null || GlobalModel.TestDBCconfig.Id == 0)
                {
                    MessageBox.Show("当前项目未配置DBC信息，请配置后使用相关模板", "产品相关模板添加失败");
                    return;
                }
            }


            var rlt = FunViewModel.Create(SupportConfig.DicTestFlowItems[type]);

            if (!rlt)
            {
                MessageBox.Show($"操作无法添加:{rlt.Message}");
                return;
            }
            IFunViewModel funmodel = rlt.Data;

            if (FunctionInfos.ToList().FindAll(x => x.ModType == rlt.Data.SupportFuncitonType).Count > 1)
            {
                DeepCloner.CloneTo( FunctionInfos.Last(x => x.ModType == rlt.Data.SupportFuncitonType).Model, funmodel);
                //funmodel = FunctionInfos.Last(x => x.ModType == rlt.Data.SupportFuncitonType).Model.DeepClone();
            }

            FunctionInfos.Add(new FunctionInfo
            {
                Step = FunctionInfos.Count + 1,
                //CutomFuncName = SupportConfig.DicTestFlowItems[obj.ToString()],
                //FunctionName = obj.ToString(),
                CutomFuncName = type.GetDescription(),
                FunctionName = obj.ToString(),
                ModType = type,
                Model = funmodel
            });

            FunctionInfoIndex = FunctionInfos.Count - 1;
            if (FunctionInfos.Count < 2)
            {
                ShowFunction(FunctionInfos.Count - 1);
            }
        }
        #endregion

        #region 试验方法操作

        /// <summary>
        /// 获取试验步骤
        /// </summary>
        private void SelectSolution()
        {
            if (string.IsNullOrEmpty(SolutionName))
            {
                return;
            }

            //OperateResult<List<ModTestFunction>> result = DBOperate.Default.GetTestFunctionByFuncName(SolutionName);
            OperateResult<List<ModTestProcess>> result = DBOperate.Default.GetFlowsByNameID(SolutionName, projectid);
            if (!result)
            {
                MessageBox.Show($"获取操作失败：{result.Message}");
                return;
            }

            FunctionInfos = new ObservableCollection<FunctionInfo>();
            foreach (ModTestProcess item in result.Data)
            {
                CreatFunction(item);
                //FunctionInfos.Add(new FunctionInfo
                //{
                //    Step = item.Step,
                //    FunctionName = item.FuntionName,
                //    ModType = (FuncitonType)Enum.Parse(typeof( FuncitonType), item.Type),
                //    Model = rlt.Data
                //});
            }
        }

        /// <summary>
        /// 选择流程导入配置步骤ADMINISTRATOR
        /// </summary>
        /// <param name="obj"></param>
        private void CreatFunction(ModTestProcess obj)
        {

            if (!Enum.TryParse(obj.FuntionName, out FuncitonType type))
            {
                MessageBox.Show($"无法获取当前{obj}类型操作步骤", "步骤添加异常");
                return;
            }

            var jsonrlt = Function_Config.DeJson(type, obj.Funtion?.ToString());
            if (!jsonrlt)
            {
                MessageBox.Show($"JSON错误:{jsonrlt.Message}");
                return;
            }

            var rlt = FunViewModel.Create(SupportConfig.DicTestFlowItems[type]);

            if (!rlt)
            {
                MessageBox.Show($"操作无法添加:{rlt.Message}");
                return;
            }
            var model = rlt.Data;
            model.Func = TestFunction.Create(jsonrlt.Data);

            FunctionInfos.Add(new FunctionInfo
            {
                Step = obj.Step,
                CutomFuncName = type.GetDescription(),
                FunctionName = type.ToString(),
                Content = obj.Description,
                ModType = type,
                Model = model
            });

            FunctionInfoIndex = FunctionInfos.Count - 1;
        }

        /// <summary>
        /// 更新流程
        /// </summary>
        private void UpdataedTestFlow()
        {
            if (System.Windows.MessageBox.Show($"是否更新当前【{SolutionName}】测试流程！", "提示", MessageBoxButton.OKCancel) == MessageBoxResult.Cancel) return;

            List<ModTestProcess> mod = new List<ModTestProcess>();

            foreach (var item in FunctionInfos)
            {

                OperateResult<string> result = item.Model.Func.Config.ToJson();

                mod.Add(new ModTestProcess
                {
                    Step = item.Step,
                    CustomName = item.CutomFuncName,
                    FuntionName = item.FunctionName,
                    Description = item.Content,
                    Type = item.ModType.ToString(),
                    Funtion = result ? result.Data : string.Empty,
                });
            }

            DBOperate.Default.UpdateProcess(projectid, SolutionName, mod)
                             .AttachIfSucceed(result =>
                             {
                                 MessageBox.Show($"配置信息保存成功");
                             })
                             .AttachIfFailed(result =>
                             {
                                 MessageBox.Show($"配置信息存储失败:{result.Message}", "试验流程配置异常");
                             });
        }

        /// <summary>
        /// 试验流程另存为
        /// </summary>
        private void SaveAsedTestFlow()
        {
            System.Windows.Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                Window newwindow = ContentControlManager.GetWindow<NewTestProcessView>(((ViewModelLocator)System.Windows.Application.Current.FindResource("Locator")).NewTestProcess);
                newwindow.Show();
            }));
        }

        /// <summary>
        /// 导出流程
        /// </summary>
        private void ExportTestFlow()
        {
            //TsetProcesse = TsetProcesse.ProcessName == "上电程序方案" ? TsetProcesses[0] : TsetProcesses[1];

            if (FunctionInfos.Count < 1)
            {
                MessageBox.Show($"{SolutionName} 暂无流程，请先配置相关流程再导出配置", "导出配置异常");
                return;
            }
            try
            {
                string defaultFileName = $"{projectname}_{SolutionName}_{DateTime.Now:yyyyMMddHHmm}.ATE";
                SaveFileDialog saveFileDialog = new SaveFileDialog
                {
                    InitialDirectory = Environment.CurrentDirectory,
                    FileName = defaultFileName,
                    Filter = "ATE配置文件 (*.ATE)|*.ATE"
                };

                //saveFileDialog.FilterIndex = 1;

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    List<ModTestProcess> mod = new List<ModTestProcess>();
                    foreach (var item in FunctionInfos)
                    {
                        OperateResult<string> result = item.Model.Func.Config.ToJson();

                        mod.Add(new ModTestProcess
                        {
                            Step = item.Step,
                            Description = item.Content,
                            CustomName = item.CutomFuncName,
                            FuntionName = item.FunctionName,
                            Type = item.ModType.ToString(),
                            Funtion = result ? result.Data : string.Empty,
                        });
                    }

                    string filePath = saveFileDialog.FileName;
                    string infos = JsonConvert.SerializeObject(mod);
                    System.IO.File.WriteAllText(filePath, infos);

                    MessageBox.Show($"{SolutionName} 已导出共{FunctionInfos?.Count}步配置", "配置导出完成");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"ATE配置文件本地写入异常:{ex.GetMessage()}", "导出配置异常");
            }
        }

        /// <summary>
        /// 导入流程
        /// </summary>
        private void ImportTestFlow()
        {
            try
            {
                OpenFileDialog openFileDialog = new OpenFileDialog
                {
                    InitialDirectory = Environment.CurrentDirectory,
                    Filter = "ATE配置文件 (*.ATE)|*.ATE"
                };

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string souname = openFileDialog.FileName.Split('_')[1];

                    if (SolutionNames.Contains(souname))
                    {
                        MessageBox.Show($"项目中已有[{SolutionName}]流程，请修改名称后再导入", "导入失败", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }

                    string filePath = openFileDialog.FileName;
                    string infos = System.IO.File.ReadAllText(filePath);
                    FunctionInfos.Clear();
                    var functions = JsonConvert.DeserializeObject<List<ModTestProcess>>(infos);
                    for (int i = 0; i < functions?.Count; i++)
                    {
                        CreatFunction(functions[i]);
                    }

                    NewTestProcessViewModel newTestproce = ((ViewModelLocator)System.Windows.Application.Current.FindResource("Locator")).NewTestProcess;

                    newTestproce.SchemeName = souname;

                    newTestproce.SaveSchemeCommannd.Execute(ContentControlManager.GetWindow<NewTestProcessView>(((ViewModelLocator)System.Windows.Application.Current.FindResource("Locator")).NewTestProcess));

                    SolutionName = souname;

                    MessageBox.Show($"{souname} 已导入共{functions?.Count}步配置", "配置导入完成");

                    //TsetProcesse.FunctionInfos = JsonConvert.DeserializeObject<ObservableCollection<FunctionInfo>>(infos);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"ATE配置文件本地导入异常:{ex.GetMessage()}", "导入配置异常");
            }
        }

        /// <summary>
        /// 删除流程
        /// </summary>
        private void DeleteTestFlow()
        {
            if (MessageBox.Show($"确认是否删除【{SolutionName}】试验项", "删除试验项", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
            {
                DBOperate.Default.DeleteProcess(projectid, SolutionName)
                 .AttachIfSucceed(result =>
                 {
                     MessageBox.Show($"【{SolutionName}】试验项删除成功");
                     GetTestSolutions();
                 })
                 .AttachIfFailed(result =>
                 {
                     MessageBox.Show($"试验项删除成功:{result.Message}", "异常");
                 });
            }
        }
        #endregion

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
        public TestProcessViewModel()
        {
            projectid = GlobalModel.Test_ProjectInfo.Id;
            projectname = GlobalModel.Test_ProjectInfo.ProjectName;
            //projectid = ((ViewModelLocator)System.Windows.Application.Current.FindResource("Locator")).Main.ProjectInfo.Id;
            //projectname = ((ViewModelLocator)System.Windows.Application.Current.FindResource("Locator")).Main.ProjectInfo.ProjectName;
            GetTestSolutions();
            TestFlowItemsInit();
        }
        #endregion

    }
}
