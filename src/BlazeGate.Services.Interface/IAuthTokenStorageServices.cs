using BlazeGate.Model.JwtBearer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazeGate.Services.Interface
{
    public interface IAuthTokenStorageServices
    {
        public Task<AuthTokenDto> GetAuthToken();

        public Task SetAuthToken(AuthTokenDto authToken);
    }
}
