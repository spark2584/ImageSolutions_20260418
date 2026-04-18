using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SanMarIntegration.Task
{
    public class TestInvoicing
    {
        public static bool Start()
        {
            com.sanmar.ws_invoiceservicebind.GetInvoicesRequest request = new com.sanmar.ws_invoiceservicebind.GetInvoicesRequest();
            request.wsVersion = "1.0.0";
            request.id = "daniellesorge";
            request.password = "image11";
            request.referenceNumber = "1789032";
            request.queryType = 1;

            com.sanmar.ws_invoiceservicebind.InvoiceService service = new com.sanmar.ws_invoiceservicebind.InvoiceService();
            com.sanmar.ws_invoiceservicebind.GetInvoicesResponse response = service.getInvoices(request);

            if(response.ErrorMessage != null)
            {
                //response.InvoiceArray[0].invoiceAmount;
            }

            return true;
        }
    }
}

