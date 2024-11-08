#region << 版 本 注 释 >>
/*----------------------------------------------------------------
 * 版权所有 (c) 2024   保留所有权利。
 * CLR版本：4.0.30319.42000
 * 机器名称：LAPTOP-9Q9TTD5V
 * 公司名称：
 * 命名空间：IMX.ATS.ATEConfig.Function.ViewModel
 * 唯一标识：3d86fee1-623b-408f-b48c-e66a0593d909
 * 文件名：FunViewModelEquipmentResult
 * 当前用户域：LAPTOP-9Q9TTD5V
 * 
 * 创建者：58274
 * 创建时间：2024/11/7 16:16:12
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
using IMX.Function.Base;
using IMX.Function.ViewModel;
using IMX.Function;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using IMX.Common;
using System.Collections.ObjectModel;
using System.Windows;
using GalaSoft.MvvmLight.CommandWpf;
using IMX.Function.Base.Enumerations;
using System.Data.Entity.Core.Metadata.Edm;
using IMX.Device.Common;
using Super.Zoo.Framework;

namespace IMX.ATS.ATEConfig.Function
{
    public class FunViewModelEquipmentResult : FunViewModel
    {
        #region 公共属性
        private TestFunction func = TestFunction.Create(FuncitonType.EquipmentResult);
        public override TestFunction Func 
        {
            get=> func;
            set 
            {
                func = value;
                FunConfig_EquipmentResult config = value.Config as FunConfig_EquipmentResult;
                config.DatasName ??= SupportDeviceInfo.DeviceRecInfo["AN87330"]?.ToDictionary(x => x.DataInfo.Name)?.Keys.ToList();

                DataList.Clear();
                InDatas.Clear();

                for (int i = 0; i < config.DatasName?.Count; i++)
                {
                    DataList.Add(config.DatasName[i]);
                }

                for (int i = 0; i < config.Datas?.Count; i++)
                {
                    InDatas.Add(new ProtectProShow 
                    {
                        Function = value,
                        Index = i,
                        ResultData = config.Datas[i],
                        IsUse = config.Datas[i].IsUse,
                        ResultDatas = DataList,
                        ResultDataName = config.Datas[i].DataInfo.Name,
                        TrageCondition = config.Datas[i].Judgment,
                    });
                }
            }
        }

        public override FuncitonType SupportFuncitonType => FuncitonType.EquipmentResult;

        public override string SupportFuncitonString => "EquipmentResult";

        #region 界面绑定属性

        private int selecteditemindex;
        /// <summary>
        /// 当前选择参数
        /// </summary>
        public int SelectedItemIndex
        {
            get => selecteditemindex;
            set => Set(nameof(SelectedItemIndex), ref selecteditemindex, value);
        }

        #region 设置参数
        private ObservableCollection<string> datalist = new ObservableCollection<string>();
        /// <summary>
        /// 设备可读取参数列表
        /// </summary>
        public ObservableCollection<string> DataList
        {
            get => datalist;
            set => Set(nameof(DataList), ref datalist, value);
        }

        private ObservableCollection<ProtectProShow> indatas = new ObservableCollection<ProtectProShow>();
        /// <summary>
        /// 参与参数列表
        /// </summary>
        public ObservableCollection<ProtectProShow> InDatas
        {
            get => indatas;
            set => Set(nameof(InDatas), ref indatas, value);
        }

        /// <summary>
        /// 失败时处理方式列表
        /// </summary>
        public List<ResultOpereateType> ResultOpereates => [ResultOpereateType.OUTTEST, ResultOpereateType.OUTFUNCTION, ResultOpereateType.IGNORE];

        private ResultOpereateType resultopereate = ResultOpereateType.OUTTEST;
        /// <summary>
        /// 当前选择失败时处理方式
        /// </summary>
        public ResultOpereateType ResultOpereate
        {
            get => resultopereate;
            set 
            {
                if (Set(nameof(ResultOpereate), ref resultopereate, value))
                {
                    (Func.Config as IFunction_Result).ResultOpereate = value;
                }
            }
        }

        #endregion


        #endregion

        #region 界面绑定指令
        public RelayCommand AddNewData => new RelayCommand(Add);

        public RelayCommand DeleteData => new RelayCommand(Delete);
        #endregion

        #endregion

        #region 私有变量
        #endregion

        #region 私有方法

        /// <summary>
        /// 添加保护参数
        /// </summary>
        private void Add()
        {
            try
            {
                var data = new ProtectProShow
                {
                    ResultDatas = DataList,
                    Function = Func,
                    Index = InDatas.Count,
                    ResultData = new ModDeviceReadData { DataInfo = new ModTestDataInfo() },
                };

                InDatas.Add(data);

                if ((Func.Config as IFunction_Result).Datas == null)
                {
                    (Func.Config as IFunction_Result).Datas = new List<ModDeviceReadData>();
                }

                (Func.Config as IFunction_Result).Datas.Add(data.ResultData);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"参数添加异常:{ex.Message}");
            }
        }

        /// <summary>
        /// 删除保护参数
        /// </summary>
        private void Delete()
        {
            try
            {
                if (InDatas.Count < 1)
                {
                    MessageBox.Show($"参数列表为空，请先添加");
                    return;
                }

                int index = SelectedItemIndex;//记录数据，防止移除过程中数据变更
                
                (Func.Config as IFunction_Result).Datas.RemoveAt(index);
                InDatas.RemoveAt(index);
                

                for (int i = 0; i < InDatas?.Count; i++)
                {
                    InDatas[i].Index = i;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"参数删除异常:{ex.GetMessage()}");
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
        public FunViewModelEquipmentResult() { }
        #endregion

    }

    /// <summary>
    /// 保护参数展示界面
    /// </summary>
    public class ProtectProShow : ViewModelBase
    {
        #region 界面显示绑定

        //private bool istrage = false;
        ///// <summary>
        ///// 是否为触发值
        ///// </summary>
        //public bool IsTrage
        //{
        //    get => istrage;
        //    set
        //    {
        //        if (Set(nameof(IsTrage), ref istrage, value))
        //        {
        //            if (value)
        //            {
        //                ProtectType = "触发保护";
        //                TrageShow = Visibility.Visible;
        //                RangeShow = Visibility.Collapsed;

        //            }
        //            else
        //            {
        //                ProtectType = "范围保护";
        //                TrageShow = Visibility.Collapsed;
        //                RangeShow = Visibility.Visible;
        //            }

        //            config.ProtectDatas[Index].IsTrage = value;
        //            ProtectData.IsTrage = value;
        //        }
        //    }
        //}

        private bool isuse = true;
        /// <summary>
        /// 是否参与到结果判定
        /// </summary>
        public bool IsUse
        {
            get => isuse;
            set 
            {
                if (Set(nameof(IsUse), ref isuse, value)) 
                {
                    config.Datas[Index].IsUse = value;
                }
            }
        }


        /// <summary>
        /// 触发条件列表
        /// </summary>
        public List<ResultJudgment> TrageConditions { get; set; } = new List<ResultJudgment> {  ResultJudgment.RANGE, ResultJudgment.EQUALTO, ResultJudgment.NOTEQUALTO };

        private ResultJudgment tragecondition = ResultJudgment.RANGE;
        /// <summary>
        /// 当前选择条件
        /// </summary>
        public ResultJudgment TrageCondition
        {
            get => tragecondition;
            set
            {
                if (Set(nameof(TrageCondition), ref tragecondition, value))
                {
                    ResultData.Judgment = value;
                    config.Datas[Index].Judgment = value;
                    if (value == ResultJudgment.RANGE)
                    {
                        TrageShow = Visibility.Collapsed;
                        RangeShow = Visibility.Visible;
                    }
                    else
                    {
                        TrageShow = Visibility.Visible;
                        RangeShow = Visibility.Collapsed;
                    }
                }
            }
        }


        //private string protecttype = "范围保护";
        ///// <summary>
        ///// 保护类型显示
        ///// </summary>
        //public string ProtectType
        //{
        //    get => protecttype;
        //    set => Set(nameof(ProtectType), ref protecttype, value);
        //}

        private Visibility trageshow = Visibility.Hidden;
        /// <summary>
        /// 触发值显示状态
        /// </summary>
        public Visibility TrageShow
        {
            get => trageshow;
            set => Set(nameof(TrageShow), ref trageshow, value);
        }

        private Visibility rangeshow = Visibility.Visible;
        /// <summary>
        /// 范围值显示状态
        /// </summary>
        public Visibility RangeShow
        {
            get => rangeshow;
            set => Set(nameof(RangeShow), ref rangeshow, value);
        }

        #endregion

        #region 配置关联参数
        public int Index { get; set; }

        /// <summary>
        /// 新保护配置
        /// </summary>
        private IFunction_Result config = null;

        private TestFunction function = null;
        public TestFunction Function
        {
            get => function;
            set
            {
                function = value;
                config = function.Config as IFunction_Result;
            }
        }

        ///// <summary>
        ///// 参数对应设备类型
        ///// </summary>
        //public Dictionary<string, string> DicValueDeviceType { get; set; } = new Dictionary<string, string>();
        #endregion

        #region 设置值
        private ObservableCollection<string> resultdatas = new ObservableCollection<string>();
        /// <summary>
        /// 允许读取参数列表
        /// </summary>
        public ObservableCollection<string> ResultDatas
        {
            get => resultdatas;
            set => Set(nameof(ResultDatas), ref resultdatas, value);
        }

        private string resultdataname;
        /// <summary>
        /// 当前选择参数名称
        /// </summary>
        public string ResultDataName
        {
            get => resultdataname;
            set
            {
                if (Set(nameof(ResultDataName), ref resultdataname, value))
                {
                    //if (ProtectData.DataInfo == null)
                    //{
                    //    ProtectData.DataInfo = new ModTestDataInfo();
                    //}
                    ResultData.DataInfo.Name = value;
                }
            }
        }


        private ModDeviceReadData resultdata = new ModDeviceReadData { DataInfo = new ModTestDataInfo() };
        /// <summary>
        /// 参与结果读取参数
        /// </summary>
        public ModDeviceReadData ResultData
        {
            get => resultdata;
            set => Set(nameof(ResultData), ref resultdata, value);
        }
        #endregion
    }
}
