using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageSolutions.Shipping
{
    public class ShiphawkShipmentDocument
    {
        public string id { get; set; }
        public bool customer_uploaded { get; set; }
        public string type { get; set; }
        public object template_level { get; set; }
        public string extension { get; set; }
        public string code { get; set; }
        public string url { get; set; }
        public Meta_Data meta_data { get; set; }
        public DateTime created_at { get; set; }
        public string filename { get; set; }
    }
}
