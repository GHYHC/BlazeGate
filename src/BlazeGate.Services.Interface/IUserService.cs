using BlazeGate.Model.EFCore;
using BlazeGate.Model.JwtBearer;
using BlazeGate.Model.WebApi;
using BlazeGate.Model.WebApi.Request;
using BlazeGate.Model.WebApi.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazeGate.Services.Interface
{
    public interface IUserService
    {
        public Task<ApiResult<PaginatedList<UserInfo>>> QueryByPage(string serviceName, int pageIndex, int pageSize, UserQuery userParam);

        public Task<ApiResult<int>> ChangeUserEnabled(string serviceName, long userId, bool enabled);

        public Task<ApiResult<int>> RemoveById(string serviceName, long userId);

        public Task<ApiResult<int>> SaveUser(string serviceName, UserSave userSave);
    }
}