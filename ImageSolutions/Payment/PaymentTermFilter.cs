using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageSolutions.Payment
{
    public class PaymentTermFilter
    {
        public Database.Filter.StringSearch.SearchFilter InternalID { get; set; }
        public Database.Filter.StringSearch.SearchFilter Description { get; set; }
    }
}
