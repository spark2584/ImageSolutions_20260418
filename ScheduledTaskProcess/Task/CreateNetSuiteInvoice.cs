using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduledTaskProcess.Task
{
    public class CreateNetSuiteInvoice
    {
        public void Execute()
        {
            List<ImageSolutions.Invoice.Invoice> ISInvoices = null;
            ImageSolutions.Invoice.InvoiceFilter ISInvoiceFilter = null;
            NetSuiteLibrary.Invoice.Invoice NetSuiteInvoice = null;
            NetSuiteLibrary.Invoice.InvoiceFilter objFilter = null;

            try
            {
                ISInvoiceFilter = new ImageSolutions.Invoice.InvoiceFilter();
                ISInvoiceFilter.InternalID = new Database.Filter.StringSearch.SearchFilter();
                ISInvoiceFilter.InternalID.Operator = Database.Filter.StringSearch.SearchOperator.empty;

                //ISInvoiceFilter.PurchaseOrderID.Operator = Database.Filter.StringSearch.SearchOperator.notEmpty;

                ISInvoices = ImageSolutions.Invoice.Invoice.GetInvoices(ISInvoiceFilter);

                foreach(ImageSolutions.Invoice.Invoice _invoice in ISInvoices)
                {
                    try
                    {
                        NetSuiteInvoice = new NetSuiteLibrary.Invoice.Invoice(_invoice);
                        NetSuiteInvoice.CreateInvoice();
                    }
                    catch(Exception ex)
                    {
                    }
                }

            }
            catch(Exception ex)
            {
                Console.Write(string.Format("{0}", ex.Message));


            }
            finally
            {

            }

        }
    }

}