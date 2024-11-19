
using IMX.DB.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMX.ATS.UserManage
{
    public class GlobalModel
    {
        /// <summary>
        /// 用户信息
        /// </summary>
        public static UserInfo UserInfo { get; set; } = new UserInfo();
    }
}
