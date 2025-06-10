using BlazeGate.Model.WebApi.Response;
using BlazeGate.Services.Interface;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazeGate.Services.Implement.Remote
{
    public class SnowFlakeService : BaseWebApi, ISnowFlakeService
    {
        public SnowFlakeService(IHttpClientFactory httpClientFactory, IConfiguration configuration) : base(httpClientFactory, configuration)
        {
            WebApiAddress = BlazeGateAddress;
        }

        public Task<SnowFlakeInfo> GetSnowFlakeInfo()
        {
            throw new NotImplementedException();
        }

        public async Task<long> NextId()
        {
            return await HttpPostJsonAsync<string, long>("/api/SnowFlake/NextId", "");
        }

        public async Task<List<long>> NextIds(int count)
        {
            string result = await HttpPostJsonAsync<string, string>($"/api/SnowFlake/NextIds?count={count}", "");
            return result.Split(',').Select(long.Parse).ToList();
        }

        public Task SetId(long datacenterId, long workerId)
        {
            throw new NotImplementedException();
        }
    }
}