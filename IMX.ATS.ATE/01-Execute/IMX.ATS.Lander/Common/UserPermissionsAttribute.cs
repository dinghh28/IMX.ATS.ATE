using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMX.ATS.Lander.Common
{
    /// <summary>
    /// 权限属性类
    /// </summary>
    /// <param name="description">权限描述</param>
    /// <param name="key">AES加密盐</param>
    [AttributeUsage(AttributeTargets.All , AllowMultiple = true)]
    internal class UserPermissionsAttribute(string description, string key) : Attribute
    {
        /// <summary>
        /// 权限描述
        /// </summary>
        public string Description { get; set; } = description;
        /// <summary>
        /// 加密盐
        /// </summary>
        public string AESKey { get; set; } = key;
    }
}
