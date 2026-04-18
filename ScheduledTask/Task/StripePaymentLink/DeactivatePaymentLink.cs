using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduledTask.Task.StripePaymentLink
{
    public class DeactivatePaymentLink
    {
        public DeactivatePaymentLink()
        {
            if (Convert.ToString(ConfigurationManager.AppSettings["Environment"]) == "production")
            {
                Stripe.StripeConfiguration.ApiKey = Convert.ToString(ConfigurationManager.AppSettings["StripeAPI"]);
            }
            else
            {
                Stripe.StripeConfiguration.ApiKey = Convert.ToString(ConfigurationManager.AppSettings["StagingStripeAPI"]);
            }
        }
        public bool Execute()
        {
            try
            {
                List<ImageSolutions.Invoice.InvoicePayment> InvoicePayments = new List<ImageSolutions.Invoice.InvoicePayment>();
                ImageSolutions.Invoice.InvoicePaymentFilter InvoicePaymentFilter = new ImageSolutions.Invoice.InvoicePaymentFilter();
                InvoicePaymentFilter.Status = new Database.Filter.StringSearch.SearchFilter();
                InvoicePaymentFilter.Status.SearchString = "Payment Link Created";
                InvoicePayments = ImageSolutions.Invoice.InvoicePayment.GetInvoicePayments(InvoicePaymentFilter);

                foreach (ImageSolutions.Invoice.InvoicePayment _InvoicePayment in InvoicePayments)
                {
                    try
                    {
                        if( (DateTime.UtcNow - _InvoicePayment.CreatedOn).TotalDays > 10)
                        {
                            Deactivate(_InvoicePayment.PaymentLinkID);

                            _InvoicePayment.Status = "Deactivated";
                            _InvoicePayment.Update();
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(String.Format("{0}", ex.Message));
                        _InvoicePayment.ErrorMessage = ex.Message;
                        _InvoicePayment.Update();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(String.Format("{0}", ex.Message));
            }


            return true;
        }
        public void Deactivate(string paymentlinkid)
        {
            try
            {
                Stripe.PaymentLinkUpdateOptions PaymentLinkUpdateOptions = new Stripe.PaymentLinkUpdateOptions();
                PaymentLinkUpdateOptions.Active = false;
                Stripe.PaymentLinkService PaymentLinkService = new Stripe.PaymentLinkService();
                PaymentLinkService.Update(paymentlinkid, PaymentLinkUpdateOptions);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}
