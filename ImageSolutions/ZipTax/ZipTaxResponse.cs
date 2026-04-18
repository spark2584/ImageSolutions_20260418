using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageSolutions.ZipTax
{
    public class ZipTaxResponse
    {
        public string version { get; set; }
        public string rCode { get; set; }
        public List<ZipTaxResult> results { get; set; }

    }
}
