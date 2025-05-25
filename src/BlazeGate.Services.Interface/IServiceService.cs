using BlazeGate.Model.EFCore;
using BlazeGate.Model.WebApi;
using BlazeGate.Model.WebApi.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazeGate.Services.Interface
{
    public interface IServiceService
    {
        public Task<ApiResult<List<Service>>> LoadData(string serviceName);

        public Task<ApiResult<long>> AddService();

        public Task<ApiResult<long>> DeleteService(long serviceId);

        public Task<ApiResult<int>> EnabledChanged(long serviceId, bool enabled);

        public Task<ApiResult<ServiceDetails>> GetServiceDetailsByName(string serviceName);

        public Task<ApiResult<long>> UpdateServiceDetails(ServiceDetails serviceDetails);

        public Task<ApiResult<Service>> GetServiceById(long serviceId);

        public Task<ApiResult<string>> GetDestinationConfig(long serviceId);
    }
}