using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageSolutions.Paystand
{
    public class Hold
    {
        public string id { get; set; }
        public string amount { get; set; }
        public string currency { get; set; }
        public string holdTime { get; set; }
        public string releaseDate { get; set; }
        public string resourceId { get; set; }
        public string resourceType { get; set; }
        public string status { get; set; }
        public string created { get; set; }
        public string lastUpdated { get; set; }
    }
}
