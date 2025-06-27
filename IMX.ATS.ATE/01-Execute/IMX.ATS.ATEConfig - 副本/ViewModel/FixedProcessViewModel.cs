#region << 版 本 注 释 >>
/*----------------------------------------------------------------
 * 版权所有 (c) 2025   保留所有权利。
 * CLR版本：4.0.30319.42000
 * 机器名称：LAPTOP-9Q9TTD5V
 * 公司名称：
 * 命名空间：IMX.ATS.ATEConfig.ViewModel
 * 唯一标识：78543c7e-9e69-4662-b8fc-02ad5d05ef41
 * 文件名：FixedProcessViewModel
 * 当前用户域：LAPTOP-9Q9TTD5V
 * 
 * 创建者：58274
 * 创建时间：2025/4/7 15:20:13
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

using FastDeepCloner;
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
using System.Threading;
using System.Windows;
using System.Windows.Forms;
using MessageBox = System.Windows.Forms.MessageBox;

namespace IMX.ATS.ATEConfig
{
    /// <summary>
    /// 固定试验步骤配置界面类
    /// </summary>
    public class FixedProcessViewModel : ExtendViewModelBase
    {
        #region 公共属性

        #region 界面绑定属性

        private ObservableCollection<TestFlowItem> testFlowItems = new ObservableCollection<TestFlowItem>();
        /// <summary>
        /// 试验模板列表
        /// </summary>
        public ObservableCollection<TestFlowItem> TestFlowItems
        {
            get { return testFlowItems; }
            set { testFlowItems = value; }
        }

        private ObservableCollection<TestProcessModel> tsetProcesses = new ObservableCollection<TestProcessModel>();

        public ObservableCollection<TestProcessModel> TsetProcesses
        {
            get => tsetProcesses;
            set => Set(nameof(TsetProcesses), ref tsetProcesses, value);
        }

        private TestProcessModel tsetProcesse;

        /// <summary>
        /// 当前选中配置页面
        /// </summary>
        public TestProcessModel TsetProcesse
        {
            get => tsetProcesse;
            set => Set(nameof(TsetProcesse), ref tsetProcesse, value);
        }


        //#region 配置信息

        //private ObservableCollection<FunctionInfo> functionInfos = new ObservableCollection<FunctionInfo>();
        ///// <summary>
        ///// 操作步骤配置列表
        ///// </summary>
        //public ObservableCollection<FunctionInfo> FunctionInfos
        //{
        //    get => functionInfos;
        //    set => Set(nameof(FunctionInfos), ref functionInfos, value);
        //}

        //private int functionInfoIndex;
        ///// <summary>
        ///// 选中的试验步骤
        ///// </summary>
        //public int FunctionInfoIndex
        //{
        //    get => functionInfoIndex;
        //    set
        //    {
        //        if (Set(nameof(FunctionInfoIndex), ref functionInfoIndex, value))
        //        {
        //            Thread.Sleep(20);
        //            ShowFunction(value);
        //        }
        //    }
        //}


        //#endregion
        #endregion

        #region 界面绑定指令
        #endregion

        #endregion

        #region 私有变量

        /// <summary>
        /// 项目ID
        /// </summary>
        private int proid = -1;
        #endregion

        #region 私有方法

        //#region 试验步骤顺序变更
        ///// <summary>
        ///// 排序
        ///// </summary>
        //private void ReNumber()
        //{
        //    FunctionInfos = TsetProcesse.ProcessName == "开机流程" ? TsetProcesses[0].FunctionInfos : TsetProcesses[1].FunctionInfos;

        //    for (int i = 0; i < FunctionInfos.Count; i++)
        //    {
        //        FunctionInfos[i].Step = i + 1;
        //    }
        //}

        ///// <summary>
        ///// 变更试验步骤
        ///// </summary>
        ///// <param name="obj">上/下移、删除</param>
        //private void ChangedFunInfos(object obj)
        //{
        //    //FunctionInfos = TsetProcesse.ProcessName == "上电程序方案" ? TsetProcesses[0].FunctionInfos : TsetProcesses[1].FunctionInfos;
        //    TsetProcesse = TsetProcesse.ProcessName == "开机流程" ? TsetProcesses[0] : TsetProcesses[1];
        //    int index = TsetProcesse.FunctionInfoIndex;
        //    if (TsetProcesse.FunctionInfos.Count == 0) return;

        //    try
        //    {

        //        switch (obj.ToString().ToUpper())
        //        {
        //            case "UP":
        //            {
        //                if (index == 0)
        //                { MessageBox.Show("已到达最高点，无法上移！", "提示！", MessageBoxButtons.OK, MessageBoxIcon.Information); return; }
        //                //int count = SelectConditions.Count;

        //                //for (int i = 0; i < SelectConditions.Count; i++)
        //                //{
        //                //    FunctionInfos.Insert(index - 1 + i, SelectConditions[i]);
        //                //}
        //                //for (int i = 0; i < count; i++)
        //                //{
        //                //    FunctionInfos.RemoveAt(index + i + 1);
        //                //}
        //                TsetProcesse.FunctionInfos.Insert(index - 1, FunctionInfos[index]);
        //                TsetProcesse.FunctionInfos.RemoveAt(index + 1);
        //                TsetProcesse.FunctionInfoIndex = index - 1;

        //                ReNumber();
        //            }
        //            break;
        //            case "DOWN":
        //            {
        //                if (index == FunctionInfos.Count - 1)
        //                { MessageBox.Show("已到达最低点，无法下移!", "提示！", MessageBoxButtons.OK, MessageBoxIcon.Information); return; }
        //                TsetProcesse.FunctionInfos.Insert(index + 2, FunctionInfos[index]);
        //                TsetProcesse.FunctionInfos.RemoveAt(index);
        //                TsetProcesse.FunctionInfoIndex = index + 1;
        //                ReNumber();
        //            }
        //            break;
        //            case "DELETE":
        //            {
        //                TsetProcesse.FunctionInfos.RemoveAt(index);
        //                for (int i = index; i < FunctionInfos.Count; i++)
        //                {
        //                    TsetProcesse.FunctionInfos[i].Step = i + 1;
        //                }
        //            }
        //            break;
        //            default: break;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(ex.GetMessage());
        //        return;
        //    }
        //}
        //#endregion

        /// <summary>
        /// 展示试验步骤
        /// </summary>
        /// <param name="index"></param>
        private void ShowFunction(int index)
        {

            //TsetProcesse = TsetProcesse.ProcessName == "开机流程" ? TsetProcesses[0] : TsetProcesses[1];

            if (index == -1)
            {
                TsetProcesse.ConfigContent = null;
                return;
            }

            try
            {

                if (index > TsetProcesse.FunctionInfos.Count)
                {
                    MessageBox.Show($"选择步骤超方案已有步骤范围");
                    return;
                }

                //string winname = SupportConfig.DicTestFlowItems.First(x => x.Value == FunctionInfos[index].FunctionName).Key;
                string winname = TsetProcesse.FunctionInfos[index].FunctionName;

                Type win = Type.GetType($"{SupportConfig.SystemName}.Function.FunView{winname}");

                TsetProcesse.ConfigContent = ContentControlManager.GetControl(win, TsetProcesse.FunctionInfos[index].Model);
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
                if (!GlobalModel.TestDBCconfig.EnableUse)
                {
                    MessageBox.Show("当前项目DBC配置存在异常，请确认相关配置后再使用相关模板!", "产品相关模板添加失败");
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

            IFunViewModel funmodel;

            var viewmodel = TsetProcesse.FunctionInfos.LastOrDefault(x => x.ModType == rlt.Data.SupportFuncitonType);
            if (viewmodel != null)
            {
                //FastDeepCloner.DeepCloner.CloneTo(viewmodel.Model, funmodel);
                funmodel = viewmodel.Model.DeepClone();
            }
            //if (FunctionInfos.ToList().FindAll(x => x.ModType == rlt.Data.SupportFuncitonType).Count > 1)
            //{
            //    DeepCloner.CloneTo(FunctionInfos.Last(x => x.ModType == rlt.Data.SupportFuncitonType).Model, funmodel);
            //    //funmodel = FunctionInfos.Last(x => x.ModType == rlt.Data.SupportFuncitonType).Model.DeepClone();
            //}
            else
            {
                funmodel = rlt.Data;
            }

            TsetProcesse.FunctionInfos.Add(new FunctionInfo
            {
                Step = TsetProcesse.FunctionInfos.Count + 1,
                //CutomFuncName = SupportConfig.DicTestFlowItems[obj.ToString()],
                //FunctionName = obj.ToString(),
                CutomFuncName = type.GetDescription(),
                FunctionName = obj.ToString(),
                ModType = type,
                Model = funmodel
            });

            TsetProcesse.FunctionInfoIndex = TsetProcesse.FunctionInfos.Count - 1;
            if (TsetProcesse.FunctionInfos.Count > 0)
            {
                ShowFunction(TsetProcesse.FunctionInfos.Count - 1);
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
                //剔除开关机
                if (item.Key == FuncitonType.Startup || item.Key == FuncitonType.Shutdown)
                {
                    continue;
                }
                TestFlowItems.Add(new TestFlowItem { Tag = item.Key, Selcted = new RelayCommand<object>(AddFunction) });
            }

            TsetProcesses = [
                                new TestProcessModel()
            {
                ProcessName = "开机流程",
                TestFlowItems = TestFlowItems,
                //new ObservableCollection<TestFlowItem>() {
                //    new TestFlowItem{ Name="工装设备配置",Tag="工装设备配置",Selcted = new RelayCommand<object>(AddFunction) },
                //    new TestFlowItem{ Name="产品指令操作",Tag="产品指令操作",Selcted = new RelayCommand<object>(AddFunction) },
                //    //new TestFlowItem{ Name="用户自定义指令",Tag="用户自定义指令",Selcted = new RelayCommand<object>(AddFunction) },
                //},
                FunctionInfos=new ObservableCollection<FunctionInfo>(),
                //OperatDataGrid=new RelayCommand<object>(ChangedFunInfos),
                //UpdataTestFlow = new RelayCommand(UpdataedTestFlow),
                //Export = new RelayCommand(ExportFunctions),
                //Import = new RelayCommand(ImportFunctions),
                FunctionInfoIndex = -1,
            },
                                new()
            {
                ProcessName = "关机流程",
                TestFlowItems = TestFlowItems,
                //new ObservableCollection<TestFlowItem>() {
                //    new TestFlowItem{ Name="工装设备配置",Tag="工装设备配置",Selcted = new RelayCommand<object>(AddFunction) },
                //    new TestFlowItem{ Name="产品指令操作",Tag="产品指令操作",Selcted = new RelayCommand<object>(AddFunction) },
                //    //new TestFlowItem{ Name="用户自定义指令",Tag="用户自定义指令",Selcted = new RelayCommand<object>(AddFunction) },
                //},
                FunctionInfos = new ObservableCollection<FunctionInfo>(),
                //OperatDataGrid=new RelayCommand<object>(ChangedFunInfos),
                //UpdataTestFlow = new RelayCommand(UpdataedTestFlow),
                //Export = new RelayCommand(ExportFunctions),
                //Import = new RelayCommand(ImportFunctions),
                FunctionInfoIndex = -1,
            }
                            ];
        }
        #endregion


        #endregion

        #region 保护方法
        protected override void WindowLoadedExecute(object obj)
        {
            GlobalModel.NowProcessName = "开关机流程";
            if (proid == GlobalModel.Test_ProjectInfo.Id)
            {
                return;
            }

            proid = GlobalModel.Test_ProjectInfo.Id;
            //开机流程导入
            for (int i = 0; i < GlobalModel.Test_ProjectInfo.Test_OpenFlows?.Count; i++)
            {
                TsetProcesses[0].CreatFunction(GlobalModel.Test_ProjectInfo.Test_OpenFlows[i]);
            }
            //关机流程导入
            for (int i = 0; i < GlobalModel.Test_ProjectInfo.Test_ShutFlows?.Count; i++)
            {
                TsetProcesses[1].CreatFunction(GlobalModel.Test_ProjectInfo.Test_ShutFlows[i]);
            }
            //base.WindowLoadedExecute(obj);
        }

        protected override void WindowClosedExecute(object obj)
        {
            base.WindowClosedExecute(obj);
        }
        #endregion


        #region 构造方法
        public FixedProcessViewModel() { TestFlowItemsInit(); }
        #endregion
    }

    public class TestProcessModel : ExtendViewModelBase
    {
        /// <summary>
        /// 配置流程名称
        /// </summary>
        public string ProcessName { get; set; }

        private ObservableCollection<TestFlowItem> testFlowItems = new ObservableCollection<TestFlowItem>();
        /// <summary>
        /// 试验模板信息
        /// </summary>
        public ObservableCollection<TestFlowItem> TestFlowItems
        {
            get => testFlowItems;
            set => Set(nameof(TestFlowItem), ref testFlowItems, value);
        }

        private TestFlowItem selectedtestflowitem;
        /// <summary>
        /// 当前选中试验模板
        /// </summary>
        public TestFlowItem SelectedTestFlowItem
        {
            get => selectedtestflowitem;
            set => Set(nameof(SelectedTestFlowItem), ref selectedtestflowitem, value);
        }

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
                    ShowFunction(value);
                }
            }
        }

        private FrameworkElement configContent;
        /// <summary>
        /// 详细配置内容
        /// </summary>
        public FrameworkElement ConfigContent
        {
            get => configContent;
            set => Set(nameof(ConfigContent), ref configContent, value);
        }

        /// <summary>
        /// 步骤操作
        /// </summary>
        public RelayCommand<string> OperatDataGrid => new RelayCommand<string>(ChangedFunInfos);

        /// <summary>
        /// 插入步骤
        /// </summary>
        public RelayCommand Insert { get; set; }

        /// <summary>
        /// 试验方案更新指令
        /// </summary>
        public RelayCommand UpdataTestFlow => new RelayCommand(UpdataedTestFlow);

        /// <summary>
        /// 配置导入
        /// </summary>
        public RelayCommand Import => new RelayCommand(ImportFunctions);

        /// <summary>
        /// 导出配置
        /// </summary>
        public RelayCommand Export => new RelayCommand(ExportFunctions);

        /// <summary>
        /// 更新方案
        /// </summary>
        private void UpdataedTestFlow()
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

            if (System.Windows.MessageBox.Show($"是否更新当前{ProcessName}！", "提示", MessageBoxButton.OKCancel) == MessageBoxResult.Cancel) return;

            DBOperate.Default.UpdataedTestFlow(GlobalModel.Test_ProjectInfo.Id, mod, ProcessName == "开机流程")
                .AttachIfSucceed(result => MessageBox.Show($"{ProcessName}配置信息保存成功！"))
                .AttachIfFailed(result => MessageBox.Show($"{ProcessName}配置信息保存成功失败:{result.Message}", "开关机流程配置异常"));
        }

        /// <summary>
        /// 方案导入
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        private void ImportFunctions()
        {
            try
            {
                OpenFileDialog openFileDialog = new OpenFileDialog
                {
                    InitialDirectory = Environment.CurrentDirectory,
                    Filter = "ATE配置文件 (*.FIXP)|*.FIXP"
                };

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string souname = openFileDialog.FileName.Split('_')[1];

                    string filePath = openFileDialog.FileName;
                    string infos = System.IO.File.ReadAllText(filePath);
                    FunctionInfos.Clear();
                    var functions = JsonConvert.DeserializeObject<List<ModTestProcess>>(infos);
                    for (int i = 0; i < functions?.Count; i++)
                    {
                        CreatFunction(functions[i]);
                    }

                    MessageBox.Show($"ATE配置【{ProcessName}】文件导入成功");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"ATE配置文件本地导入异常:{ex.GetMessage()}", "导入配置异常");
            }
        }

        /// <summary>
        /// 方案导出
        /// </summary>
        private void ExportFunctions()
        {
            if (FunctionInfos.Count < 1)
            {
                MessageBox.Show($"{ProcessName} 暂无流程，请先配置相关流程再导出配置", "导出配置异常");
                return;
            }
            try
            {
                string defaultFileName = $"{GlobalModel.Test_ProjectInfo.ProjectName}_{ProcessName}_{DateTime.Now:yyyyMMddHHmm}.BIS";
                SaveFileDialog saveFileDialog = new SaveFileDialog
                {
                    InitialDirectory = Environment.CurrentDirectory,
                    FileName = defaultFileName,
                    Filter = "ATS配置文件 (*.FIXP)|*.FIXP"
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

                    MessageBox.Show($"{ProcessName} 已导出共{FunctionInfos?.Count}步配置", "配置导出完成");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"老化配置文件本地写入异常:{ex.GetMessage()}", "导出配置异常");
            }
        }

        #region 流程操作
        /// <summary>
        /// 选择流程导入配置步骤ADMINISTRATOR
        /// </summary>
        /// <param name="obj"></param>
        public void CreatFunction(ModTestProcess obj)
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
                string winname = FunctionInfos[index].FunctionName;

                Type win = Type.GetType($"{SupportConfig.SystemName}.Function.FunView{winname}");

                ConfigContent = ContentControlManager.GetControl(win, FunctionInfos[index].Model);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.GetMessage());
                return;
            }

        }
        #endregion

        #region 试验步骤顺序变更
        /// <summary>
        /// 排序
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
        private void ChangedFunInfos(string obj)
        {
            int index = FunctionInfoIndex;
            if (FunctionInfos.Count == 0) return;

            try
            {
                switch (obj.ToUpper())
                {
                    case "UP":
                    {
                        if (index == 0)
                        { MessageBox.Show("已到达最高点，无法上移！", "提示！", MessageBoxButtons.OK, MessageBoxIcon.Information); return; }

                        FunctionInfos.Insert(index - 1, FunctionInfos[index]);
                        FunctionInfos.RemoveAt(index + 1);
                        FunctionInfoIndex = index - 1;

                        ReNumber();
                    }
                    break;
                    case "DOWN":
                    {
                        if (index == FunctionInfos.Count - 1)
                        { MessageBox.Show("已到达最低点，无法下移!", "提示！", MessageBoxButtons.OK, MessageBoxIcon.Information); return; }
                        FunctionInfos.Insert(index + 2, FunctionInfos[index]);
                        FunctionInfos.RemoveAt(index);
                        FunctionInfoIndex = index + 1;
                        ReNumber();
                    }
                    break;
                    case "DELETE":
                    {
                        FunctionInfos.RemoveAt(index);
                        for (int i = index; i < FunctionInfos.Count; i++)
                        {
                            FunctionInfos[i].Step = i + 1;
                        }
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
                    default: break;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.GetMessage());
                return;
            }
        }
        #endregion
    }
}
