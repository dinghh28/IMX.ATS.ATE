#region << 版 本 注 释 >>
/*----------------------------------------------------------------
 * 版权所有 (c) 2025   保留所有权利。
 * CLR版本：4.0.30319.42000
 * 机器名称：LAPTOP-9Q9TTD5V
 * 公司名称：
 * 命名空间：IMX.ATS.ATEConfig.Function.ViewModel
 * 唯一标识：34220df9-e6d6-46cb-af1b-eea865bfcfde
 * 文件名：FunViewModelCustomRevData
 * 当前用户域：LAPTOP-9Q9TTD5V
 * 
 * 创建者：58274
 * 创建时间：2025/6/30 18:14:36
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
using IMX.Common;
using IMX.Function.Base;
using IMX.Function;
using IMX.Function.ViewModel;
using IMX.Function.ViewModel.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using System.Windows.Markup;
using System.Windows;

namespace IMX.ATS.ATEConfig.Function
{
    public class FunViewModelCustomRevData : FunViewModel
    {


        #region 公共属性
        private TestFunction func = TestFunction.Create(FuncitonType.CustomRevData);
        public override TestFunction Func
        {
            get => func;
            set
            {
                func = value;
                FunConfig_CustomRevData config = value.Config as FunConfig_CustomRevData;

                config.Datas ??= [];

                if (dataInfos.Count<1)
                {
                    if (!SupportConfig.DicProcessConfig.TryGetValue(GlobalModel.NowProcessName, out ProcessConfig_EX processconfig))
                    {
                        return;
                    }

                    if (!processconfig.UseCustomData)
                    {
                        return;
                    }
                    dataInfosname.Clear();

                    for (int i = 0; i < processconfig.Test_CustomData.Count; i++)
                    {
                        var data = processconfig.Test_CustomData[i];
                        dataInfos.Add(new ModTestDataInfo
                        {
                            Name = data.Name,
                        });
                        dataInfosname.Add(data.Name);
                    }
                }
                for (int i = 0; i < config.Datas.Count; i++)
                {
                    var data = config.Datas[i];
                    int findindex = dataInfos.ToList().FindIndex(x => x.Name == data.Name);
                    //以当前项目为基础加载上报信息
                    if (findindex != -1)
                    {
                        DataConfigs.Add(new CustomRevDataConfig
                        {
                            DataInfosName = dataInfosname,
                            DataInfos = dataInfos,
                            Config = config,
                            Data = data,
                            SelectIndex = findindex,
                            
                        });
                    }
                }

                config.Datas.Clear();

                for (int i = 0; i < DataConfigs.Count; i++)
                {
                    config.Datas.Add(DataConfigs[i].Data);
                }
                
            }
        }

        public override FuncitonType SupportFuncitonType => FuncitonType.CustomRevData;

        public override string SupportFuncitonString => SupportFuncitonType.ToString();

        #region 界面绑定属性
        private ObservableCollection<CustomRevDataConfig> dataconfigs = new ObservableCollection<CustomRevDataConfig>();
        /// <summary>
        /// 上报列表
        /// </summary>
        public ObservableCollection<CustomRevDataConfig> DataConfigs
        {
            get => dataconfigs;
            set => Set(nameof(DataConfigs), ref dataconfigs, value);
        }

        private int selectedconfigindex = -1;
        /// <summary>
        /// 当前选择上报信息
        /// </summary>
        public int SelectedConfigIndex
        {
            get => selectedconfigindex;
            set => Set(nameof(SelectedConfigIndex), ref selectedconfigindex, value);
        }

        #endregion

        #region 界面绑定指令
        /// <summary>
        /// 添加自定义上报信息
        /// </summary>
        public RelayCommand Add => new RelayCommand(() => 
        {
            CustomRevDataConfig data = new CustomRevDataConfig 
            {
                DataInfosName = dataInfosname,
                DataInfos = dataInfos,
                Config = (Func.Config as FunConfig_CustomRevData),
                Data = new ModTestDataInfo(),
            };

            DataConfigs.Add(data);

            (Func.Config as FunConfig_CustomRevData).Datas.Add(data.Data);
        });

        /// <summary>
        /// 删除选中信息
        /// </summary>
        public RelayCommand Delet => new RelayCommand(() => 
        {
            if (SelectedConfigIndex == -1)
            {
                return;
            }

            if (MessageBox.Show($"是否删除当前【{dataInfosname[SelectedConfigIndex]}】消息") != MessageBoxResult.Yes)
            {
                return;
            }

            DataConfigs.RemoveAt(SelectedConfigIndex);
            (Func.Config as FunConfig_CustomRevData).Datas.RemoveAt(SelectedConfigIndex);
        });
        #endregion

        #endregion

        #region 私有变量
        private ObservableCollection<ModTestDataInfo> dataInfos = new ObservableCollection<ModTestDataInfo>();

        private ObservableCollection<string> dataInfosname = new ObservableCollection<string>();
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
        public FunViewModelCustomRevData() { }
        #endregion

    }

    public class CustomRevDataConfig :ViewModelBase
    {
        private ObservableCollection<ModTestDataInfo> datainfos;
        /// <summary>
        /// 上报信息列表
        /// </summary>
        public ObservableCollection<ModTestDataInfo> DataInfos
        {
            get => datainfos;
            set => Set(nameof(DataInfos), ref datainfos, value);
        }

        private int selectindex = -1;
        /// <summary>
        /// 当前选择信息序号
        /// </summary>
        public int SelectIndex
        {
            get => selectindex;
            set 
            {
                if (Set(nameof(SelectIndex), ref selectindex, value)) 
                {
                    if (value == -1)
                    {
                        return;
                    }

                    Data = DataInfos[value];
                    Config.Datas[value].Name = DataInfosName[value];
                }
            }
                
        }

        private ObservableCollection<string> datainfosname;
        /// <summary>
        /// 信息名称列表
        /// </summary>
        public ObservableCollection<string> DataInfosName
        {
            get => datainfosname;
            set => Set(nameof(DataInfosName), ref datainfosname, value);
        }

        /// <summary>
        /// 配置方案
        /// </summary>
        public FunConfig_CustomRevData Config { get; set; }

        /// <summary>
        /// 当前选择上报信息
        /// </summary>
        public ModTestDataInfo Data { get; set; } = new ModTestDataInfo();
    }
}
