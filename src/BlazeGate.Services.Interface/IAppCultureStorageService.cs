using BlazeGate.Model.Culture;
using BlazeGate.Model.JwtBearer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazeGate.Services.Interface
{
    public interface IAppCultureStorageService
    {
        public Task<AppCultureInfo> GetAppCulture();

        public Task SetAppCulture(AppCultureInfo appCultureInfo);
    }
}