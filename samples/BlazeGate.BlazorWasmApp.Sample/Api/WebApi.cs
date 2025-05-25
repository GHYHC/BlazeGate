using BlazeGate.Model.Sample;
using BlazeGate.Model.Sample.EFCore;
using BlazeGate.Model.WebApi;
using BlazeGate.Services.Implement.Remote;
using BlazeGate.Services.Interface;

namespace BlazeGate.BlazorWasmApp.Sample.Api
{
    public class WebApi : AuthWebApi
    {
        public WebApi(IHttpClientFactory httpClientFactory, IAuthTokenStorageServices authTokenStorage, IConfiguration configuration) : base(httpClientFactory, authTokenStorage, configuration)
        {
        }

        /// <summary>
        /// 查询字典数据
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        public async Task<ApiResult<PaginatedList<TB_Dictionary>>> Dictionary_QueryByPage(int pageIndex, int pageSize, DictionaryQuery query)
        {
            return await HttpPostJsonAsync<DictionaryQuery, ApiResult<PaginatedList<TB_Dictionary>>>($"/api/Dictionary/QueryByPage?pageIndex={pageIndex}&pageSize={pageSize}", query);
        }

        /// <summary>
        /// 保存字典数据
        /// </summary>
        /// <param name="dictionary"></param>
        /// <returns></returns>
        public async Task<ApiResult<long>> Dictionary_Save(TB_Dictionary dictionary)
        {
            return await HttpPostJsonAsync<TB_Dictionary, ApiResult<long>>("/api/Dictionary/Save", dictionary);
        }

        /// <summary>
        /// 删除字典数据
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<ApiResult<int>> Dictionary_RemoveById(long id)
        {
            return await HttpPostJsonAsync<string, ApiResult<int>>($"/api/Dictionary/RemoveById?id={id}", "");
        }

        /// <summary>
        /// 获取字典类型
        /// </summary>
        /// <returns></returns>
        public async Task<ApiResult<List<string>>> Dictionary_GetType()
        {
            return await HttpPostJsonAsync<string, ApiResult<List<string>>>("/api/Dictionary/GetType", "");
        }

        /// <summary>
        /// 修改字典启用状态
        /// </summary>
        /// <param name="id"></param>
        /// <param name="enabled"></param>
        /// <returns></returns>
        public async Task<ApiResult<int>> Dictionary_ChangeEnabled(long id, bool enabled)
        {
            return await HttpPostJsonAsync<string, ApiResult<int>>($"/api/Dictionary/ChangeEnabled?id={id}&enabled={enabled}", "");
        }

        /// <summary>
        /// 获取最大排序号
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public async Task<ApiResult<int>> Dictionary_GetMaxNumberIndexByType(string type)
        {
            return await HttpPostJsonAsync<string, ApiResult<int>>($"/api/Dictionary/GetMaxNumberIndexByType?type={type}", "");
        }
    }
}