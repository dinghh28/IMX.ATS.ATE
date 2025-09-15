using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMX.DB
{
    /// <summary>
    /// 试验记录数据类型
    /// </summary>
    public enum RecordDataType
    {
        /// <summary>
        /// 常规操作
        /// </summary>
        [Description("常规操作")]
        NORMAL,
        /// <summary>
        /// 结果上报 
        /// </summary>
        [Description("结果上报")]
        RESULT,
        /// <summary>
        /// 步进记录
        /// </summary>
        [Description("步进记录")]
        STEPPING,
        /// <summary>
        /// 条件跳转
        /// </summary>
        [Description("条件跳转")]
        RETURN,
    }
}
