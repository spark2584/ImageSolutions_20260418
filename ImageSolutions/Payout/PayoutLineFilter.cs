using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageSolutions.Payout
{
    public class PayoutLineFilter
    {
        public Database.Filter.StringSearch.SearchFilter PayoutID { get; set; }
        public Database.Filter.StringSearch.SearchFilter InternalID { get; set; }
    }
}
