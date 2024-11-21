#region << 版 本 注 释 >>
/*----------------------------------------------------------------
 * 版权所有 (c) 2024   保留所有权利。
 * CLR版本：4.0.30319.42000
 * 机器名称：LAPTOP-9Q9TTD5V
 * 公司名称：
 * 命名空间：IMX.ATS.ATEConfig.Function
 * 唯一标识：852e3c70-d359-44c5-b72e-e2f4326d5308
 * 文件名：FunViewModelProductResult
 * 当前用户域：LAPTOP-9Q9TTD5V
 * 
 * 创建者：58274
 * 创建时间：2024/11/7 16:16:51
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
using IMX.Common;
using IMX.Device.Common;
using IMX.Function;
using IMX.Function.Base;
using IMX.Function.Base.Enumerations;
using IMX.Function.ViewModel;
using Super.Zoo.Framework;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IMX.ATS.ATEConfig.Function
{
    public class FunViewModelProductResult : FunViewModel
    {
        #region 公共属性
        private TestFunction func = TestFunction.Create(FuncitonType.ProductResult);
        public override TestFunction Func
        {
            get => func;
            set
            {
                func = value;
                FunConfig_ProductResult config = value.Config as FunConfig_ProductResult;
                config.DatasName ??= GlobalModel.TestDBCconfig.Test_DBCReceiveSignals?.ToDictionary(x=>x.Custom_Name)?.Keys.ToList();

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

        public override FuncitonType SupportFuncitonType => FuncitonType.ProductResult;

        public override string SupportFuncitonString => "ProductResult";

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
            get => resultopereate = (Func.Config as IFunction_Result).ResultOpereate;
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
                MessageBox.Show($"参数添加异常:{ex.GetMessage()}");
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
        public FunViewModelProductResult() 
        {

        }
        #endregion

    }
}
