using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazeGate.Model.WebApi.Request
{
    public class RolePageQuery
    {
        /// <summary>
        /// 角色ID
        /// </summary>
        public long? RoleId { get; set; }

        /// <summary>
        /// 角色名称
        /// </summary>
        public string? RoleName { get; set; }
    }
}