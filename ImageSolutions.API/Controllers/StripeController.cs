using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using Stripe;

namespace ImageSolutions.API.Controllers
{
    [RoutePrefix("stripe")]
    public class StripeController : ApiController
    {
        [HttpPost]
        [Route("charge")]
        public HttpResponseMessage Charge([FromBody] CreditCard creditcard)
        {            
            try
            {
                string strCreditCardTransactionID = string.Empty;
                string strPaymentData = string.Empty;

                Stripe.Charge StripeCharge = null;

                StripeConfiguration.ApiKey = Convert.ToString(ConfigurationManager.AppSettings["StripeAPI"]);
                StripeCharge = ChargeCard(creditcard);
                //strPaymentData = StripeCharge.StripeResponse.Content;
                strCreditCardTransactionID = StripeCharge.Id;

                return Request.CreateResponse(HttpStatusCode.OK, strCreditCardTransactionID);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        public Stripe.Charge ChargeCard(CreditCard creditcard)
        {
            try
            {
                creditcard.Amount = Decimal.Multiply(Convert.ToDecimal(creditcard.Amount), Convert.ToDecimal(100.0));

                //Save Customer
                Stripe.CustomerService CustomerService = new Stripe.CustomerService();
                Stripe.CustomerCreateOptions CustomerCreateOptions = new Stripe.CustomerCreateOptions();
                CustomerCreateOptions.Name = creditcard.FullName;

                CustomerCreateOptions.Address = new Stripe.AddressOptions();
                //CustomerCreateOptions.Address.Line1 = address.AddressLine1;
                //CustomerCreateOptions.Address.Line2 = address.AddressLine2;
                //CustomerCreateOptions.Address.City = address.City;
                //CustomerCreateOptions.Address.State = address.State;
                CustomerCreateOptions.Address.PostalCode = creditcard.PostalCode;
                CustomerCreateOptions.Address.Country = creditcard.CountryCode;

                Stripe.Customer Customer = CustomerService.Create(CustomerCreateOptions);

                if (Customer.StripeResponse.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    throw new Exception(string.Format("{0}: {1}", Customer.StripeResponse.StatusCode, Customer.StripeResponse.Content));
                }

                //SaveSource
                Stripe.SourceService SourceService = new SourceService();
                Stripe.SourceCreateOptions SourceCreateOptions = new SourceCreateOptions();

                Stripe.SourceCardOptions SourceCardOptions = new SourceCardOptions();
                SourceCardOptions.Number = creditcard.CardNumber;
                SourceCardOptions.ExpMonth = creditcard.ExpirationMonth;
                SourceCardOptions.ExpYear = creditcard.ExpirationYear;
                SourceCardOptions.Cvc = creditcard.CVV;
                SourceCreateOptions.Card = SourceCardOptions;
                SourceCreateOptions.Type = SourceType.Card;

                Stripe.SourceOwnerOptions SourceOwnerOptions = new SourceOwnerOptions();
                SourceOwnerOptions.Address = new Stripe.AddressOptions();
                //SourceOwnerOptions.Address.Line1 = address.AddressLine1;
                //SourceOwnerOptions.Address.Line2 = address.AddressLine2;
                //SourceOwnerOptions.Address.City = address.City;
                //SourceOwnerOptions.Address.State = address.State;
                SourceOwnerOptions.Address.PostalCode = creditcard.PostalCode;
                SourceOwnerOptions.Address.Country = string.IsNullOrEmpty(creditcard.CountryCode) ? "US" : creditcard.CountryCode;
                SourceOwnerOptions.Name = creditcard.FullName;
                SourceCreateOptions.Owner = SourceOwnerOptions;

                Stripe.Source Source = SourceService.Create(SourceCreateOptions);

                if (Source.StripeResponse.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    throw new Exception(string.Format("{0}: {1}", Source.StripeResponse.StatusCode, Source.StripeResponse.Content));
                }

                //Attach Source to Customer
                Stripe.SourceAttachOptions SourceAttachOptions = new SourceAttachOptions();
                SourceAttachOptions.Source = Source.Id;
                SourceService.Attach(Customer.Id, SourceAttachOptions);

                if (Source.StripeResponse.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    throw new Exception(string.Format("{0}: {1}", Source.StripeResponse.StatusCode, Source.StripeResponse.Content));
                }

                Stripe.ChargeCreateOptions ChargeCreateOptions = new Stripe.ChargeCreateOptions();
                ChargeCreateOptions.Amount = (long)creditcard.Amount;
                ChargeCreateOptions.Currency = creditcard.Currency;
                ChargeCreateOptions.Source = Source.Id;
                ChargeCreateOptions.Customer = Customer.Id;
                //ChargeCreateOptions.Metadata = new Dictionary<string, string> { { "OrderId", orderid } };

                //ChargeCreateOptions.StatementDescriptorSuffix = abbreviation + orderid;

                Stripe.ChargeService ChargeService = new Stripe.ChargeService();
                Stripe.Charge Charge = ChargeService.Create(ChargeCreateOptions);

                if (Charge.StripeResponse.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    throw new Exception(string.Format("{0}: {1}", Charge.StripeResponse.StatusCode, Charge.StripeResponse.Content));
                }

                return Charge;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public class CreditCard
        {
            public string FullName { get; set; }
            public string CardNumber { get; set; }
            public int ExpirationMonth { get; set; }
            public int ExpirationYear { get; set; }
            public string CVV { get; set; }
            public string PostalCode { get; set; }
            public string CountryCode { get; set; }
            public decimal Amount { get; set; }
            public string Currency { get; set; }
        }
    }
}