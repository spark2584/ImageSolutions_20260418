using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduledTask.Task.StripePaymentLink
{
    public class ConfirmPaymentLink
    {
        public ConfirmPaymentLink()
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
                        string strPaymentIntentID = FindSession(_InvoicePayment.PaymentLinkID);
                        if(!string.IsNullOrEmpty(strPaymentIntentID))
                        {
                            _InvoicePayment.PaymentIntentID = strPaymentIntentID;
                            _InvoicePayment.Status = "Payment Link Paid";
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

        public string FindSession(string paymentlinkid)
        {
            string strPaymentIntentID = String.Empty;

            try
            {

                Stripe.Checkout.SessionListOptions SessionListOptions = new Stripe.Checkout.SessionListOptions();
                SessionListOptions.PaymentLink = paymentlinkid;
                Stripe.Checkout.SessionService SessionService = new Stripe.Checkout.SessionService();
                Stripe.StripeList<Stripe.Checkout.Session> Sessions = SessionService.List(SessionListOptions);

                if (Sessions.StripeResponse.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    throw new Exception(string.Format("{0}: {1}", Sessions.StripeResponse.StatusCode, Sessions.StripeResponse.Content));
                }

                foreach (Stripe.Checkout.Session _Session in Sessions.Data)
                {
                    if (_Session.PaymentIntentId != null)
                    {
                        strPaymentIntentID = Convert.ToString(_Session.PaymentIntentId);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return strPaymentIntentID;
        }
    }
}
