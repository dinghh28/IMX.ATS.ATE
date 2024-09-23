using FreeSql;
using FreeSql.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMX.DB.Model
{
    /// <summary>
    /// 用户信息库表
    /// </summary>
    public class UserInfo : BaseEntity<UserInfo, int>
    {
        /// <summary>
        /// 用户名称
        /// </summary>
        [Column(IsNullable = false)]
        public string UserName { get; set; }

        /// <summary>
        /// 登陆密码
        /// </summary>
        [Column(IsNullable = false)]
        public string Password { get; set; }

        /// <summary>
        /// 用户权限
        /// </summary>
        public int Privilege { get; set; }
    }
}
