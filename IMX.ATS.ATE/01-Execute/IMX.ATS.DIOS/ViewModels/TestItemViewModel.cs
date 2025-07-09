#region << 版 本 注 释 >>
/*----------------------------------------------------------------
 * 版权所有 (c) 2025   保留所有权利。
 * CLR版本：4.0.30319.42000
 * 机器名称：LAPTOP-9Q9TTD5V
 * 公司名称：
 * 命名空间：IMX.ATS.DIOS.ViewModels
 * 唯一标识：df0c705a-d408-45cd-b616-a63beed42c94
 * 文件名：TestItemViewModel
 * 当前用户域：LAPTOP-9Q9TTD5V
 * 
 * 创建者：58274
 * 创建时间：2025/6/12 19:34:24
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

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using H.WPF.Framework;
using IMX.Common;
using IMX.DB;
using IMX.DB.Model;
using Super.Zoo.Framework;
using Super.Zoo.Framework.Debugger;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using MessageBox = System.Windows.Forms.MessageBox;

namespace IMX.ATS.DIOS
{
    public class TestItemViewModel : ExtendViewModelBase
    {
        #region 公共属性

        #region 界面绑定属性

        #region 进度条属性
        private Visibility proBarVisily = Visibility.Hidden;
        /// <summary>
        /// 进度条显示状态
        /// </summary>
        public Visibility ProBarVisily
        {
            get => proBarVisily;
            set => Set(nameof(ProBarVisily), ref proBarVisily, value);
        }

        private int proBarValue;
        /// <summary>
        /// 当前进度
        /// </summary>
        public int ProBarValue
        {
            get => proBarValue;
            set => Set(nameof(ProBarValue), ref proBarValue, value);
        }

        private int proBarMaxValue;
        /// <summary>
        /// 进度完成值
        /// </summary>
        public int ProBarMaxValue
        {
            get => proBarMaxValue;
            set => Set(nameof(ProBarMaxValue), ref proBarMaxValue, value);
        }

        private string proBarText;
        /// <summary>
        /// 进度条显示文本
        /// </summary>
        public string ProBarText
        {
            get => proBarText;
            set => Set(nameof(ProBarText), ref proBarText, value);
        }
        #endregion

        #region 检索条件
        private string flowname = string.Empty;
        /// <summary>
        /// 试验项名称（模糊查询）
        /// </summary>
        public string FlowName
        {
            get => flowname;
            set => Set(nameof(FlowName), ref flowname, value);
        }

        private int resultindex = -1;
        /// <summary>
        /// 当前检索试验结果条件
        /// </summary>
        public int ResultIndex
        {
            get => resultindex;
            set => Set(nameof(ResultIndex), ref resultindex, value);
        }

        #endregion

        private ObservableCollection<TestItemModel> datas = new ObservableCollection<TestItemModel>();
        /// <summary>
        /// 测试项数据列表
        /// </summary>
        public ObservableCollection<TestItemModel> Datas
        {
            get => datas;
            set => Set(nameof(Datas), ref datas, value);
        }

        private int selectindex;
        /// <summary>
        /// 当前选择行
        /// </summary>
        public int SelectIndex
        {
            get => selectindex;
            set => Set(nameof(SelectIndex), ref selectindex, value);
        }
        #endregion

        #region 界面绑定指令
        public RelayCommand Search => new RelayCommand(SearchItem);

        public RelayCommand OpenDatas => new RelayCommand(DatasWindowOpen);

        /// <summary>
        /// 全选指令
        /// </summary>
        public RelayCommand<string> SelectAllCommand => new RelayCommand<string>(SelectAll);

        private void SelectAll(string obj)
        {
            if (obj == "C")
            {
                SelectAll();
            }
            else if (obj == "U")
            {
                UnSelectAll();
            }
        }
        #endregion

        #endregion

        #region 私有变量
        private long proid = -1;
        /// <summary>
        /// 数据源（原始数据库内容，为检索原始列表）
        /// </summary>
        private ObservableCollection<Test_ItemInfo> datasources = new ObservableCollection<Test_ItemInfo>();

        private List<int> selectindexs = new List<int>();
        #endregion

        #region 私有方法

        #region 全选
        private void SelectAll()
        {
            Datas.ToList().FindAll(model => model.IsSelect = true);
            for (int i = 0; i < Datas.Count; i++)
            { selectindexs.Add(i); }
        }

        private void UnSelectAll()
        {
            Datas.ToList().FindAll(model => model.IsSelect = false);
            selectindexs.Clear();
        }
        #endregion

        #region 导出方案
        private void MulExport() 
        {
            if (Datas == null || Datas.Count < 1)
            {
                return;
            }

            List<long> indexes = new List<long>();
            List<DateTime> Starttimes = new List<DateTime>();
            List<DateTime> stoptimes = new List<DateTime>();
            Datas.ToList().FindAll(x => x.IsSelect == true).ForEach(x =>
            {
                indexes.Add(x.Data.Id);
                Starttimes.Add(x.Data.CreateTime);
                stoptimes.Add(x.Data.UpdateTime);
            });

            if (indexes.Count == 0) return;

            DialogResult dr = System.Windows.Forms.MessageBox.Show("是否导出所选" + indexes.Count + "项的测试数据？", "询问", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (dr == DialogResult.No) return;

            //设置进度条控件属性
            proBarValue = 0;
            proBarMaxValue = indexes.Count;
        }
        #endregion
        /// <summary>
        /// 检索测试项
        /// </summary>
        private void SearchItem()
        {

            if (string.IsNullOrEmpty(FlowName) && ResultIndex == -1)
            {
                Datas.Clear();
                for (int i = 0; i < datasources.Count; i++)
                {
                    Datas.Add(new TestItemModel 
                    {
                        IsSelect = false,
                        Data = datasources[i]
                    });
                }
            }
            else if (string.IsNullOrEmpty(FlowName))
            {
                DBOperate.Default.SelectTestItems(proid, (ResultState)Enum.Parse(typeof(ResultState), ResultIndex.ToString()))
                    .AttachIfSucceed(result =>
                    {
                        Datas.Clear();
                        for (int i = 0; i < result.Data.Count; i++)
                        {
                            Datas.Add(new TestItemModel
                            {
                                IsSelect = false,
                                Data = result.Data[i]
                            });
                        }
                    })
                    .AttachIfFailed(result =>
                    {
                        MessageBox.Show($"按试验结果获取数据失败：\r\n{result.Message}", "条件检索失败");
                    });
            }
            else
            {
                DBOperate.Default.SelectTestItems(proid, FlowName)
                        .AttachIfSucceed(result =>
                        {
                            Datas.Clear();
                            for (int i = 0; i < result.Data.Count; i++)
                            {
                                Datas.Add(new TestItemModel
                                {
                                    IsSelect = false,
                                    Data = result.Data[i]
                                });
                            }
                        })
                        .AttachIfFailed(result =>
                        {
                            MessageBox.Show($"按测试项名称查询获取数据失败：\r\n{result.Message}", "条件检索失败");
                        });
            }
        }

        /// <summary>
        /// 打开数据展示界面窗口
        /// </summary>
        private void DatasWindowOpen()
        {
            if (SelectIndex == -1)
            {
                return;
            }
            int index = SelectIndex;

            Test_ItemInfo item = Datas[index].Data;

           var result =  DBOperate.Default.GetTestDataCount(item.Id, item.CreateTime, item.UpdateTime.AddSeconds(1));

            if (!result)
            {
                MessageBox.Show(result.Message,"实验数据查询异常");
                return;
            }

            var model = new TestDatasViewModel($"{item.ProjectName}-{item.FlowName}",item.Id,item.CreateTime, item.UpdateTime, result.Data);

            var view = new TestDatasView
            {
              DataContext = model,
            };
            view.Show();
        }
        #endregion

        #region 保护方法
        protected override void WindowLoadedExecute(object obj)
        {
            if (proid == GlobalModel.ProjectItemId)
            {
                return;
            }

            proid = GlobalModel.ProjectItemId;

            Datas.Clear();
            datasources.Clear();
            DBOperate.Default.SelectTestItems(proid)
                .AttachIfSucceed(result =>
                {
                    for (int i = 0; i < result.Data.Count; i++)
                    {
                        TestItemModel testinfomodel = new TestItemModel
                        {
                            IsSelect = false,
                            Data = result.Data[i],
                        };
                        Datas.Add(testinfomodel);
                        datasources.Add(result.Data[i]);
                    }
                })
                .AttachIfFailed(result =>
                {
                    MessageBox.Show(result.Message, "试验条目数据加载失败");
                });
            //base.WindowLoadedExecute(obj);
        }

        protected override void WindowClosedExecute(object obj)
        {
            base.WindowClosedExecute(obj);
        }
        #endregion


        #region 构造方法
        public TestItemViewModel() { }
        #endregion
    }

    /// <summary>
    /// 测试项页面展示类
    /// </summary>
    public class TestItemModel :ViewModelBase
    {
        private bool isselect = false;
        /// <summary>
        /// 当前选中状态
        /// </summary>
        public bool IsSelect
        {
            get => isselect;
            set => Set(nameof(IsSelect), ref isselect, value);
        }


        private Test_ItemInfo data;
        /// <summary>
        /// 测试项数据列表
        /// </summary>
        public Test_ItemInfo Data
        {
            get => data;
            set => Set(nameof(Data), ref data, value);
        }
    }
}
