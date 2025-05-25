using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazeGate.Model.WebApi.Response
{
    public class SnowFlakeInfo
    {
        public string Guid { get; set; }

        public long WorkerId { get; set; }

        public long DatacenterId { get; set; }

        public string IdInfo
        { get { return $"{DatacenterId}:{WorkerId}"; } }
    }
}