using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazeGate.Services.Interface
{
    /// <summary>
    /// 数据库初始化接口
    /// </summary>
    public interface IDbInitializer
    {
        /// <summary>
        /// 初始化数据库
        /// </summary>
        /// <returns></returns>
        public Task Initialize();
    }
}