using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageSolutions.ZipTax
{
    public class ZipTaxResult
    {
        public string geoPostalCode { get; set; }
        public string geoCity { get; set; }
        public string geoCounty { get; set; }
        public string geoState { get; set; }
        public double taxSales { get; set; }
        public double taxUse { get; set; }
        public string txbService { get; set; }
        public string txbFreight { get; set; }
        public double stateSalesTax { get; set; }
        public double stateUseTax { get; set; }
        public double citySalesTax { get; set; }
        public double cityUseTax { get; set; }
        public string cityTaxCode { get; set; }
        public double countySalesTax { get; set; }
        public double countyUseTax { get; set; }
        public string countyTaxCode { get; set; }
        public double districtSalesTax { get; set; }
        public double districtUseTax { get; set; }
        public string district1Code { get; set; }
        public double district1SalesTax { get; set; }
        public double district1UseTax { get; set; }
        public string district2Code { get; set; }
        public double district2SalesTax { get; set; }
        public double district2UseTax { get; set; }
        public string district3Code { get; set; }
        public double district3SalesTax { get; set; }
        public double district3UseTax { get; set; }
        public string district4Code { get; set; }
        public double district4SalesTax { get; set; }
        public double district4UseTax { get; set; }
        public string district5Code { get; set; }
        public double district5SalesTax { get; set; }
        public double district5UseTax { get; set; }
        public string originDestination { get; set; }
    }
}
