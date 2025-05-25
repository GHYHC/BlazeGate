using BlazeGate.Model.EFCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazeGate.Model.WebApi.Request
{
    public class PageDropSave
    {
        public Page Page { get; set; }

        public List<long> SortIds { get; set; }
    }
}
