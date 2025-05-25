using BlazeGate.Model.EFCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazeGate.Model.WebApi.Response
{
    public class ServiceDetails
    {
        public Service Service { get; set; }= new Service();

        public ServiceConfig ServiceConfig { get; set; } = new ServiceConfig();
    }
}