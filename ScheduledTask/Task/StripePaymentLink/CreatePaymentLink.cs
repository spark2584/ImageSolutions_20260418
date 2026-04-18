using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduledTask.Task.StripePaymentLink
{
    public class CreatePaymentLink
    {
        public CreatePaymentLink()
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
                List<ImageSolutions.Invoice.Invoice> Invoices = new List<ImageSolutions.Invoice.Invoice>();
                ImageSolutions.Invoice.InvoiceFilter InvoiceFilter = new ImageSolutions.Invoice.InvoiceFilter();
                InvoiceFilter.PaymentLinkID = new Database.Filter.StringSearch.SearchFilter();
                InvoiceFilter.PaymentLinkID.Operator = Database.Filter.StringSearch.SearchOperator.empty;
                Invoices = ImageSolutions.Invoice.Invoice.GetInvoices(InvoiceFilter);

                foreach (ImageSolutions.Invoice.Invoice _Invoice in Invoices)
                {
                    try
                    {
                        Stripe.PaymentLink PaymentLink = Create(_Invoice);

                        _Invoice.PaymentLinkID = PaymentLink.Id;
                        _Invoice.PaymentLinkURL = PaymentLink.Url;
                        _Invoice.Update();                        
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

        public Stripe.PaymentLink Create(ImageSolutions.Invoice.Invoice invoice)
        {
            try
            {
                Stripe.ProductCreateOptions ProductCreateOptions = new Stripe.ProductCreateOptions();
                ProductCreateOptions.Name = invoice.InvoiceNumber;
                Stripe.ProductService ProductService = new Stripe.ProductService();
                Stripe.Product Product = ProductService.Create(ProductCreateOptions);

                Stripe.PriceCreateOptions PriceCreateOptions = new Stripe.PriceCreateOptions();
                PriceCreateOptions.Currency = "usd";
                decimal amount = Decimal.Multiply(Convert.ToDecimal(invoice.AmountRemaining), Convert.ToDecimal(100.0));
                PriceCreateOptions.UnitAmount = (long)amount;
                PriceCreateOptions.Product = Product.Id;

                Stripe.PriceService PriceService = new Stripe.PriceService();
                Stripe.Price Price = PriceService.Create(PriceCreateOptions);

                Stripe.PaymentLinkCreateOptions PaymentLinkCreateOptions = new Stripe.PaymentLinkCreateOptions();
                PaymentLinkCreateOptions.LineItems = new List<Stripe.PaymentLinkLineItemOptions>();

                Stripe.PaymentLinkLineItemOptions PaymentLinkLineItemOptions = new Stripe.PaymentLinkLineItemOptions();
                PaymentLinkLineItemOptions.Price = Price.Id;
                PaymentLinkLineItemOptions.Quantity = 1;
                PaymentLinkCreateOptions.LineItems.Add(PaymentLinkLineItemOptions);

                Stripe.PaymentLinkService PaymentLinkService = new Stripe.PaymentLinkService();
                Stripe.PaymentLink PaymentLink = PaymentLinkService.Create(PaymentLinkCreateOptions);

                if (PaymentLink.StripeResponse.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    throw new Exception(string.Format("{0}: {1}", PaymentLink.StripeResponse.StatusCode, PaymentLink.StripeResponse.Content));
                }

                return PaymentLink;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
