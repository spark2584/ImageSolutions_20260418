using NetSuiteLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetSuiteLibrary.com.netsuite.webservices;

namespace ScheduledTask.Task.StripePaymentLink
{
    public class UpdateNS : NetSuiteBase
    {
        public bool Execute()
        {

            try
            {
                List<ImageSolutions.Invoice.Invoice> Invoices = new List<ImageSolutions.Invoice.Invoice>();
                ImageSolutions.Invoice.InvoiceFilter InvoiceFilter = new ImageSolutions.Invoice.InvoiceFilter();
                InvoiceFilter.PaymentLinkID = new Database.Filter.StringSearch.SearchFilter();
                InvoiceFilter.PaymentLinkID.Operator = Database.Filter.StringSearch.SearchOperator.empty;
                Invoices = ImageSolutions.Invoice.Invoice.GetInvoices(InvoiceFilter);

                foreach (ImageSolutions.Invoice.Invoice _Invoice in Invoices)
                {
                    try
                    {
                        UpdateNetSuite(_Invoice);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(String.Format("{0}", ex.Message));
                        _Invoice.ErrorMessage = ex.Message;
                        _Invoice.Update();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(String.Format("{0}", ex.Message));
            }

          
            return true;
        }

        public bool UpdateNetSuite(ImageSolutions.Invoice.Invoice invoice)
        {
            try
            {
                //Update NS
                NetSuiteLibrary.com.netsuite.webservices.Invoice NSInvoice = new Invoice();
                NSInvoice.internalId = invoice.InternalID;
                int intCustomFieldIndex = 0;
                NSInvoice.customFieldList = new CustomFieldRef[99];
                NSInvoice.customFieldList[intCustomFieldIndex++] = NetSuiteHelper.CreateStringCustomField(invoice.PaymentLinkID, "custbody_stripe_payment_link_id");
                NSInvoice.customFieldList[intCustomFieldIndex++] = NetSuiteHelper.CreateStringCustomField(invoice.PaymentLinkURL, "custbody_stripe_payment_link_url");

                WriteResponse WriteResponse = UpdateNetSuiteInvoice(NSInvoice);

                if (WriteResponse.status.isSuccess != true)
                {
                    throw new Exception("Unable to update Invoice: " + WriteResponse.status.statusDetail[0].message);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return true;
        }

        private WriteResponse UpdateNetSuiteInvoice(NetSuiteLibrary.com.netsuite.webservices.Invoice invoice)
        {
            WriteResponse objWriteResponse = null;

            try
            {
                objWriteResponse = Service.update(invoice);

                return objWriteResponse;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
