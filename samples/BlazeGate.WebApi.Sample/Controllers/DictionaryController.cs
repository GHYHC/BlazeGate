using BlazeGate.Model.Sample;
using BlazeGate.Model.Sample.EFCore;
using BlazeGate.Model.WebApi;
using BlazeGate.Services.Interface;
using BlazeGate.WebApi.Sample.Resources;
using LinqKit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;

namespace BlazeGate.WebApi.Sample.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class DictionaryController : ControllerBase
    {
        private readonly BlazeGateSampleContext context;
        private readonly ISnowFlakeService snowFlake;
        private readonly IStringLocalizer<I18n> l;

        public DictionaryController(BlazeGateSampleContext context, ISnowFlakeService snowFlake, IStringLocalizer<I18n> l)
        {
            this.context = context;
            this.snowFlake = snowFlake;
            this.l = l;
        }

        [HttpPost]
        public async Task<ApiResult<PaginatedList<TB_Dictionary>>> QueryByPage(int pageIndex, int pageSize, DictionaryQuery query)
        {
            var where = PredicateBuilder.New<TB_Dictionary>(true);
            if (query.Id != null)
            {
                where.And(x => x.Id == query.Id);
            }
            if (!string.IsNullOrWhiteSpace(query.Type))
            {
                where.And(x => x.Type.Contains(query.Type));
            }
            if (!string.IsNullOrWhiteSpace(query.Key))
            {
                where.And(x => x.Key.Contains(query.Key));
            }
            if (!string.IsNullOrWhiteSpace(query.Value))
            {
                where.And(x => x.Value.Contains(query.Value));
            }
            //根据类型和序号排序
            var source = context.TB_Dictionaries.AsNoTracking().Where(where).OrderBy(x => x.Type).ThenBy(x => x.NumberIndex);
            var list = await PaginatedList<TB_Dictionary>.CreateAsync(source, pageIndex, pageSize);
            return ApiResult<PaginatedList<TB_Dictionary>>.SuccessResult(list);
        }

        [HttpPost]
        public async Task<ApiResult<long>> Save(TB_Dictionary dictionary)
        {
            //判断相同的type下是否有相同的key（注意修改时排除当前这条数据）
            var where = PredicateBuilder.New<TB_Dictionary>(true);
            where.And(x => x.Type == dictionary.Type && x.Key == dictionary.Key);
            if (dictionary.Id > 0)
            {
                where.And(x => x.Id != dictionary.Id);
            }

            var sameKey = await context.TB_Dictionaries.AsNoTracking().Where(where).FirstOrDefaultAsync();
            if (sameKey != null)
            {
                return ApiResult<long>.FailResult("相同的类型下不能有相同的键！");
            }

            //如果Id>0则更新 否则添加
            if (dictionary.Id > 0)
            {
                context.TB_Dictionaries.Update(dictionary);
            }
            else
            {
                dictionary.Id = await snowFlake.NextId();
                dictionary.CreateTime = DateTime.Now;
                dictionary.UpdateTime = DateTime.Now;
                context.TB_Dictionaries.Add(dictionary);
            }

            int result = await context.SaveChangesAsync();
            return ApiResult<long>.Result(result > 0, dictionary.Id);
        }

        [HttpPost]
        public async Task<ApiResult<int>> RemoveById(long id)
        {
            int result = await context.TB_Dictionaries.Where(x => x.Id == id).ExecuteDeleteAsync();
            return ApiResult<int>.Result(result > 0, result);
        }

        [HttpPost]
        public async Task<ApiResult<List<string>>> GetType()
        {
            var list = await context.TB_Dictionaries.AsNoTracking().Select(x => x.Type).Distinct().ToListAsync();
            return ApiResult<List<string>>.SuccessResult(list);
        }

        [HttpPost]
        public async Task<ApiResult<int>> ChangeEnabled(long id, bool enabled)
        {
            var result = await context.TB_Dictionaries.Where(b => b.Id == id).ExecuteUpdateAsync(s => s
                    .SetProperty(t => t.Enabled, t => enabled)
                    .SetProperty(t => t.UpdateTime, t => DateTime.Now));
            return ApiResult<int>.Result(result > 0, result, result > 0 ? l["ApiResult.SuccessMsg"] : l["ApiResult.FailureMsg"]);
        }

        [HttpPost]
        public async Task<ApiResult<int>> GetMaxNumberIndexByType(string type)
        {
            var result = context.TB_Dictionaries.AsNoTracking().Where(x => x.Type == type).OrderByDescending(x => x).Select(x => x.NumberIndex).FirstOrDefault();

            return ApiResult<int>.SuccessResult(result);
        }
    }
}