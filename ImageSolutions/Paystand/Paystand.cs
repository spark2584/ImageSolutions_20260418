using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RestSharp;
using System.Configuration;
using System.Net.Http;
using System.Net;
using System.IO;
using System.Security.Cryptography;

namespace ImageSolutions.Paystand
{
    public class Paystand
    {
        public Paystand()
        {

        }
        public ImageSolutions.Paystand.Payment ChargeExistingCard(ImageSolutions.CreditCard.CreditCard creditcard, string amount, string currency)
        {
            //Get Access Token
            AccessTokenRequest AccessTokenRequest = new AccessTokenRequest();
            AccessTokenRequest.grant_type = "client_credentials";
            AccessTokenRequest.client_id = Convert.ToString(ConfigurationManager.AppSettings["PaystandClientID"]);
            AccessTokenRequest.client_secret = Convert.ToString(ConfigurationManager.AppSettings["PaystandSecret"]);
            AccessTokenRequest.scope = "auth";
            string strAccessTokenData = JsonConvert.SerializeObject(AccessTokenRequest);
            RestClient AccessTokenRestClient = new RestClient("https://api.paystand.co/v3/oauth/token");
            RestRequest AccessTokenRestRequest = new RestRequest("/", Method.Post);
            AccessTokenRestRequest.AddHeader("accept", "application/json");
            AccessTokenRestRequest.AddHeader("Content-Type", "application/json");
            AccessTokenRestRequest.AddHeader("Accept", "application/json");
            AccessTokenRestRequest.AddParameter("application/json", strAccessTokenData, ParameterType.RequestBody);
            RestResponse AccessTokenResponse = AccessTokenRestClient.Execute(AccessTokenRestRequest);
            AccessToken AccessToken = JsonConvert.DeserializeObject<AccessToken>(AccessTokenResponse.Content);

            //Payment
            PaymentRequest PaymentRequest = new PaymentRequest();
            PaymentRequest.amount = amount;
            PaymentRequest.currency = currency;
            PaymentRequest.payerId = creditcard.PayerExternalID;
            PaymentRequest.cardId = creditcard.CardExternalID;
            string strPaymentData = JsonConvert.SerializeObject(PaymentRequest);
            RestClient PaymentRestClient = new RestClient("https://api.paystand.co/v3/payments/secure");
            RestRequest PaymentRestRequest = new RestRequest("/", Method.Post);
            PaymentRestRequest.AddHeader("accept", "application/json");
            PaymentRestRequest.AddHeader("content-type", "application/json");
            PaymentRestRequest.AddHeader("X-CUSTOMER-ID", Convert.ToString(ConfigurationManager.AppSettings["PaystandCustomerID"]));
            PaymentRestRequest.AddHeader("Authorization", String.Format(@"Bearer {0}", AccessToken.access_token));
            PaymentRestRequest.AddHeader("Accept", "application/json");
            PaymentRestRequest.AddHeader("Content-Type", "application/json");
            PaymentRestRequest.AddParameter("application/json", strPaymentData, ParameterType.RequestBody);
            RestResponse PaymentResponse = PaymentRestClient.Execute(PaymentRestRequest);
            ImageSolutions.Paystand.Payment Payment = JsonConvert.DeserializeObject<Payment>(PaymentResponse.Content);

            //Validate Payment - throw error if not processed
            if (PaymentResponse.StatusCode != HttpStatusCode.OK)
            {
                if (!string.IsNullOrEmpty(PaymentResponse.ErrorMessage))
                {
                    throw new Exception(PaymentResponse.ErrorMessage);
                }
                else
                {
                    throw new Exception("Payment process failed");
                }
            }
            return Payment;
        }

        public ImageSolutions.Paystand.Payment ChargeNewCard(string nickname, string fullname, string ccnumber, string cctype, string ccmonth, string ccyear, string cccvv, string email, ImageSolutions.Address.AddressBook address
            , string amount, string currency, ImageSolutions.User.UserInfo userinfo, bool savecard, ImageSolutions.CreditCard.CreditCard creditcard = null)
        {
            //Get Access Token
            AccessTokenRequest AccessTokenRequest = new AccessTokenRequest();
            AccessTokenRequest.grant_type = "client_credentials";
            AccessTokenRequest.client_id = Convert.ToString(ConfigurationManager.AppSettings["PaystandClientID"]);
            AccessTokenRequest.client_secret = Convert.ToString(ConfigurationManager.AppSettings["PaystandSecret"]);
            AccessTokenRequest.scope = "auth";
            string strAccessTokenData = JsonConvert.SerializeObject(AccessTokenRequest);
            RestClient AccessTokenRestClient = new RestClient("https://api.paystand.co/v3/oauth/token");
            RestRequest AccessTokenRestRequest = new RestRequest("/", Method.Post);
            AccessTokenRestRequest.AddHeader("accept", "application/json");
            AccessTokenRestRequest.AddHeader("Content-Type", "application/json");
            AccessTokenRestRequest.AddHeader("Accept", "application/json");
            AccessTokenRestRequest.AddParameter("application/json", strAccessTokenData, ParameterType.RequestBody);
            RestResponse AccessTokenResponse = AccessTokenRestClient.Execute(AccessTokenRestRequest);
            AccessToken AccessToken = JsonConvert.DeserializeObject<AccessToken>(AccessTokenResponse.Content);

            //Create Payer
            PayerRequest PayerRequest = new PayerRequest();
            PayerRequest.name = fullname;
            //PayerRequest.email = email;
            AddressRequest Address = new AddressRequest();
            Address.street1 = address.AddressLine1;
            Address.street2 = address.AddressLine2;
            Address.city = address.City;
            Address.state = address.State;
            Address.postalCode = address.PostalCode;
            Address.country = string.IsNullOrEmpty(address.CountryID) ? "US" : address.CountryID;
            PayerRequest.address = Address;
            string strPayerData = JsonConvert.SerializeObject(PayerRequest);
            RestClient PayerRestClient = new RestClient("https://api.paystand.co/v3/payers");
            RestRequest PayerRestRequest = new RestRequest("/", Method.Post);
            PayerRestRequest.AddHeader("accept", "application/json");
            PayerRestRequest.AddHeader("content-type", "application/json");
            PayerRestRequest.AddHeader("X-CUSTOMER-ID", Convert.ToString(ConfigurationManager.AppSettings["PaystandCustomerID"]));
            PayerRestRequest.AddHeader("Authorization", String.Format(@"Bearer {0}", AccessToken.access_token));
            PayerRestRequest.AddHeader("Accept", "application/json");
            PayerRestRequest.AddHeader("Content-Type", "application/json");
            PayerRestRequest.AddParameter("application/json", strPayerData, ParameterType.RequestBody);
            RestResponse PayerResponse = PayerRestClient.Execute(PayerRestRequest);
            Payer Payer = JsonConvert.DeserializeObject<Payer>(PayerResponse.Content);

            //Payment
            PaymentRequest PaymentRequest = new PaymentRequest();
            PaymentRequest.amount = amount;
            PaymentRequest.currency = currency;
            CardRequest CardRequest = new CardRequest();
            CardRequest.nameOnCard = fullname;
            CardRequest.cardNumber = ccnumber;
            CardRequest.expirationMonth = ccmonth;
            CardRequest.expirationYear = ccyear;
            CardRequest.securityCode = cccvv;
            //Billing Address
            AddressRequest BillingAddress = new AddressRequest();
            BillingAddress.street1 = address.AddressLine1;
            BillingAddress.street2 = address.AddressLine2;
            BillingAddress.city = address.City;
            BillingAddress.state = address.State;
            BillingAddress.postalCode = address.PostalCode;
            BillingAddress.country = string.IsNullOrEmpty(address.CountryID) ? "US" : address.CountryID;
            CardRequest.billingAddress = BillingAddress;

            PaymentRequest.card = CardRequest;
            PaymentRequest.payerId = Payer.id;

            //Personal Contact 
            //PersonalContact PersonalContact = new PersonalContact();
            //PersonalContact.email = email;
            //PaymentRequest.personalContact = PersonalContact;
            //PaymentRequest.description = "";
            string strPaymentData = JsonConvert.SerializeObject(PaymentRequest);
            //Create Payment
            RestClient PaymentRestClient = new RestClient("https://api.paystand.co/v3/payments/secure");
            RestRequest PaymentRestRequest = new RestRequest("/", Method.Post);
            PaymentRestRequest.AddHeader("accept", "application/json");
            PaymentRestRequest.AddHeader("content-type", "application/json");
            PaymentRestRequest.AddHeader("X-CUSTOMER-ID", Convert.ToString(ConfigurationManager.AppSettings["PaystandCustomerID"]));
            PaymentRestRequest.AddHeader("Authorization", String.Format(@"Bearer {0}", AccessToken.access_token));
            PaymentRestRequest.AddHeader("Accept", "application/json");
            PaymentRestRequest.AddHeader("Content-Type", "application/json");
            PaymentRestRequest.AddParameter("application/json", strPaymentData, ParameterType.RequestBody);
            RestResponse PaymentResponse = PaymentRestClient.Execute(PaymentRestRequest);
            ImageSolutions.Paystand.Payment Payment = JsonConvert.DeserializeObject<Payment>(PaymentResponse.Content);

            //Validate Payment - throw error if not processed
            if(PaymentResponse.StatusCode != HttpStatusCode.OK)
            {
                if(!string.IsNullOrEmpty(PaymentResponse.ErrorMessage))
                {
                    throw new Exception(PaymentResponse.ErrorMessage);
                }
                else
                {
                    throw new Exception("Payment process failed");
                }
            }

            if (savecard)
            {
                //Store Payer and Card 
                ImageSolutions.CreditCard.CreditCard CreditCard = new CreditCard.CreditCard();
                Guid Guid = new Guid();
                CreditCard.GUID = Convert.ToString(Guid.NewGuid());
                CreditCard.Data = Encrypt(ccnumber.Trim(), CreditCard.GUID);
                CreditCard.Nickname = nickname;
                CreditCard.FullName = fullname;
                CreditCard.LastFourDigit = ccnumber.Trim().Length >= 4 ? ccnumber.Trim().Substring(ccnumber.Trim().Length - 4) : ccnumber.Trim();
                CreditCard.CreditCardType = cctype;
                CreditCard.ExpirationDate = Convert.ToDateTime(String.Format("{0}/1/{1}", Convert.ToString(ccmonth), Convert.ToString(ccyear)));
                CreditCard.CVV = cccvv;
                CreditCard.CreatedBy = userinfo.UserInfoID;
                CreditCard.PayerExternalID = Payment.payerId;
                CreditCard.CardExternalID = Payment.sourceId;
                CreditCard.Create();

                ImageSolutions.User.UserCreditCard UserCreditCard = new ImageSolutions.User.UserCreditCard();
                UserCreditCard.UserInfoID = userinfo.UserInfoID;
                UserCreditCard.CreditCardID = CreditCard.CreditCardID;
                UserCreditCard.CreatedBy = userinfo.UserInfoID;
                UserCreditCard.Create();
            }

            else if(creditcard != null)
            {
                creditcard.PayerExternalID = Payment.payerId;
                creditcard.CardExternalID = Payment.sourceId;
                creditcard.Update();
            }

            return Payment;
        }
        public void ApplyPayment()
        {
            ApplyPayment ApplyPayment = new ApplyPayment();

            ApplyPayment.paymentId = "6up6yomagj0cl7cmfocpuyic";
            ApplyPayment.transactionId = "326";
            ApplyPayment.transactionType = "salesOrder";

            string strApplyPaymentData = JsonConvert.SerializeObject(ApplyPayment);

            RestClient ApplyPaymentRestClient = new RestClient("https://api.paystand.co/v3/netsuites/apply-payment/public");
            RestRequest ApplyPaymentRestRequest = new RestRequest("/", Method.Post);
            ApplyPaymentRestRequest.AddHeader("Accept", "application/json");
            ApplyPaymentRestRequest.AddHeader("Content-Type", "application/json");
            ApplyPaymentRestRequest.AddHeader("Cache-Control", "no-cache");
            ApplyPaymentRestRequest.AddHeader("x-publishable-key", Convert.ToString(ConfigurationManager.AppSettings["PaystandPublishableKey"]));
            //PaymentRestRequest.AddHeader("Cookie", "");

            ApplyPaymentRestRequest.AddParameter("application/json", strApplyPaymentData, ParameterType.RequestBody);
            RestResponse ApplyPaymentResponse = ApplyPaymentRestClient.Execute(ApplyPaymentRestRequest);
            Payment Payment = JsonConvert.DeserializeObject<Payment>(ApplyPaymentResponse.Content);
        }

        public void Test()
        {
            //Access Token
            AccessTokenRequest AccessTokenRequest = new AccessTokenRequest();
            AccessTokenRequest.grant_type = "client_credentials";
            AccessTokenRequest.client_id = Convert.ToString(ConfigurationManager.AppSettings["PaystandClientID"]);
            AccessTokenRequest.client_secret = Convert.ToString(ConfigurationManager.AppSettings["PaystandSecret"]);
            AccessTokenRequest.scope = "auth";
            string strAccessTokenData = JsonConvert.SerializeObject(AccessTokenRequest);

            RestClient AccessTokenRestClient = new RestClient("https://api.paystand.co/v3/oauth/token");
            RestRequest AccessTokenRestRequest = new RestRequest("/", Method.Post);
            AccessTokenRestRequest.AddHeader("accept", "application/json");
            AccessTokenRestRequest.AddHeader("Content-Type", "application/json");
            AccessTokenRestRequest.AddHeader("Accept", "application/json");
            //AccessTokenRestRequest.AddHeader("X-PUBLISHABLE-KEY", Convert.ToString(ConfigurationManager.AppSettings["PaystandPublishableKey"]));
            //AccessTokenRestRequest.AddHeader("X-CUSTOMER-ID", Convert.ToString(ConfigurationManager.AppSettings["PaystandCustomerID"]));
            AccessTokenRestRequest.AddParameter("application/json", strAccessTokenData, ParameterType.RequestBody);
            RestResponse AccessTokenResponse = AccessTokenRestClient.Execute(AccessTokenRestRequest);
            AccessToken AccessToken = JsonConvert.DeserializeObject<AccessToken>(AccessTokenResponse.Content);


            //GetPaymentData
            //        var client = new HttpClient();
            //        var request = new HttpRequestMessage
            //        {
            //            Method = HttpMethod.Get,
            //            RequestUri = new Uri("https://api.paystand.co/v3/payments/all?startDate=2023-01-01&endDate=2023-02-28&format=json&offset=0&limit=50"),
            //            Headers =
            //{
            //    { "accept", "application/json" },
            //    { "X-CUSTOMER-ID", Convert.ToString(ConfigurationManager.AppSettings["PaystandCustomerID"]) },
            //    { "Authorization", String.Format(@"Bearer {0}", AccessToken.access_token) },
            //    { "Accept", "application/json" },
            //},
            //        };
            //        using (var response = await client.SendAsync(request))
            //        {
            //            string strResponse = Convert.ToString(response);
            //        }
            //var client = new RestClient("https://api.paystand.co/v3/payments/rurl2jp2p31djxm6tg8f7ezh");
            //var request = new RestRequest("/", Method.Get);
            //request.AddHeader("accept", "application/json");
            //request.AddHeader("X-CUSTOMER-ID", Convert.ToString(ConfigurationManager.AppSettings["PaystandCustomerID"]));
            //request.AddHeader("Authorization", String.Format(@"Bearer {0}", AccessToken.access_token));
            //request.AddHeader("Accept", "application/json");
            //request.AddHeader("Content-Type", "application/json");
            //var response = client.Execute(request);

            //RestClient GetPaymentRestClient = new RestClient("https://api.paystand.co/v3/payments/rv9ri7glzdzepvz3gb1humf3");
            ////RestClient GetPaymentRestClient = new RestClient("https://api.paystand.co/v3/payments/all?startDate=2023-01-01&endDate=2023-02-28&format=json&offset=0&limit=50");
            //RestRequest GetPaymentRestRequest = new RestRequest("/", Method.Get);
            //GetPaymentRestRequest.AddHeader("accept", "application/json");
            //GetPaymentRestRequest.AddHeader("content-type", "application/json");
            //GetPaymentRestRequest.AddHeader("X-CUSTOMER-ID", Convert.ToString(ConfigurationManager.AppSettings["PaystandCustomerID"]));
            //GetPaymentRestRequest.AddHeader("Authorization", String.Format(@"Bearer {0}", AccessToken.access_token));
            //GetPaymentRestRequest.AddHeader("Accept", "application/json");
            //GetPaymentRestRequest.AddHeader("Content-Type", "application/json");
            //RestResponse GetPaymentResponse = GetPaymentRestClient.Execute(GetPaymentRestRequest);
            //Payment GetPayment = JsonConvert.DeserializeObject<Payment>(GetPaymentResponse.Content);


            //Payer
            //PayerRequest PayerRequest = new PayerRequest();
            //PayerRequest.name = "Shalin Shaun";
            //PayerRequest.email = "shalin+test@paystand.com";
            //Address Address = new Address();
            //Address.street1 = "123 First St";
            //Address.street2 = "#2";
            //Address.city = "Santa Cruz";
            //Address.state = "CA";
            //Address.postalCode = "95060";
            //Address.country = "USA";
            //PayerRequest.address = Address;
            //string strPayerData = JsonConvert.SerializeObject(PayerRequest);

            //RestClient PayerRestClient = new RestClient("https://api.paystand.co/v3/payers");
            //RestRequest PayerRestRequest = new RestRequest("/", Method.Post);
            //PayerRestRequest.AddHeader("accept", "application/json");
            //PayerRestRequest.AddHeader("content-type", "application/json");
            //PayerRestRequest.AddHeader("X-CUSTOMER-ID", Convert.ToString(ConfigurationManager.AppSettings["PaystandCustomerID"]));
            //PayerRestRequest.AddHeader("Authorization", String.Format(@"Bearer {0}", AccessToken.access_token));
            //PayerRestRequest.AddHeader("Accept", "application/json");
            //PayerRestRequest.AddHeader("Content-Type", "application/json");
            //PayerRestRequest.AddParameter("application/json", strPayerData, ParameterType.RequestBody);
            //RestResponse PayerResponse = PayerRestClient.Execute(PayerRestRequest);
            //Payer Payer = JsonConvert.DeserializeObject<Payer>(PayerResponse.Content);

            //Payment
            //One of payerId or payer values are required in the request, but not both. - Payer
            //Only one of bank, cardId, bankId, or tokenId are required in the request. - cardid
            //The accountKey field defaults to the customer's default account.
            PaymentRequest PaymentRequest = new PaymentRequest();
            PaymentRequest.amount = "1.00";
            PaymentRequest.currency = "USD";

            PaymentRequest.payerId = "78f4g453ic8nl3iyvfy40dh1";
            PaymentRequest.cardId = "qfszevd3zp6iqjmr4iqy04kw";  //source.id

            //CardRequest CardRequest = new CardRequest();
            //CardRequest.nameOnCard = "Christina Chan";
            //CardRequest.cardNumber = "4000000000000077";
            //CardRequest.expirationMonth = "03";
            //CardRequest.expirationYear = "2025";
            //CardRequest.securityCode = "123";

            //AddressRequest BillingAddress = new AddressRequest();
            //BillingAddress.street1 = "123 First St";
            //BillingAddress.street2 = "#2";
            //BillingAddress.city = "Santa Cruz";
            //BillingAddress.state = "CA";
            //BillingAddress.postalCode = "95060";
            //BillingAddress.country = "USA";
            //CardRequest.billingAddress = BillingAddress;

            //PaymentRequest.card = CardRequest;

            PersonalContact PersonalContact = new PersonalContact();
            PersonalContact.email = "steve@imageinc.com";
            PaymentRequest.personalContact = PersonalContact;

            PaymentRequest.description = "test2";
            string strPaymentData = JsonConvert.SerializeObject(PaymentRequest);

            RestClient PaymentRestClient = new RestClient("https://api.paystand.co/v3/payments/secure");
            RestRequest PaymentRestRequest = new RestRequest("/", Method.Post);
            PaymentRestRequest.AddHeader("accept", "application/json");
            PaymentRestRequest.AddHeader("content-type", "application/json");
            PaymentRestRequest.AddHeader("X-CUSTOMER-ID", Convert.ToString(ConfigurationManager.AppSettings["PaystandCustomerID"]));
            PaymentRestRequest.AddHeader("Authorization", String.Format(@"Bearer {0}", AccessToken.access_token));
            PaymentRestRequest.AddHeader("Accept", "application/json");
            PaymentRestRequest.AddHeader("Content-Type", "application/json");
            PaymentRestRequest.AddParameter("application/json", strPaymentData, ParameterType.RequestBody);
            RestResponse PaymentResponse = PaymentRestClient.Execute(PaymentRestRequest);
            Payment Payment = JsonConvert.DeserializeObject<Payment>(PaymentResponse.Content);
     
        }
        public string Encrypt(string value, string encryptionKey)
        {
            string encryptValue = string.Empty;

            byte[] clearBytes = Encoding.Unicode.GetBytes(value);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(encryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(clearBytes, 0, clearBytes.Length);
                        cs.Close();
                    }
                    encryptValue = Convert.ToBase64String(ms.ToArray());
                }
            }
            return encryptValue;
        }
    }
}
