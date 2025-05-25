using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazeGate.Model.JwtBearer
{
    public class UserDto
    {
        /// <summary>
        /// 用户Id
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// 用户名称
        /// </summary>
        public string UserName { get; set; } = string.Empty;

        /// <summary>
        /// 账号
        /// </summary>
        public string Account { get; set; } = string.Empty;

        /// <summary>
        /// 电话
        /// </summary>
        public string Phone { get; set; } = string.Empty;

        /// <summary>
        /// 角色
        /// </summary>
        public List<string> Roles { get; set; } = new List<string>();

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; } = string.Empty;

        /// <summary>
        /// 用户数据
        /// </summary>
        public string UserData { get; set; } = string.Empty;

        /// <summary>
        /// 用户数据字典
        /// </summary>
        public Dictionary<string, string> UserDataDict { get; set; } = new Dictionary<string, string>();
    }
}