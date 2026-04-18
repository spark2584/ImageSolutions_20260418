using Stripe;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ImageSolutions.StripeAPI
{
    public class StripeAPI
    {
        public StripeAPI()
        {
            //Publishable key
            //pk_test_51N6b7yL8BQnKd8oD9RuT2DGTWhemFQdX64MoMPakvxOS0yJmKMpZgkeC3DHRHzhbKNWV3MuzkkVPeS15n67QHv7l00AHHbMis7
            //Secret
            //sk_test_51N6b7yL8BQnKd8oDkz1EeYEf0DYA2d3Pl6JGZ9WFCnAK89kWJsohX4qi4BMet1R3gFKDE5TT3JbaPFnXTXsHeWVn00C2kAlx9S
            //account
            //acct_1N6b7yL8BQnKd8oD
            
            if(Convert.ToString(ConfigurationManager.AppSettings["Environment"]) == "production")
            {
                StripeConfiguration.ApiKey = Convert.ToString(ConfigurationManager.AppSettings["StripeAPI"]);
            }
            else
            {
                StripeConfiguration.ApiKey = Convert.ToString(ConfigurationManager.AppSettings["StagingStripeAPI"]);
            }
        }

        public Stripe.Customer SaveCustomer(string name, string address1, string address2, string city, string state, string postalcode, string country)
        {
            try
            {
                //Save Customer
                Stripe.CustomerService CustomerService = new Stripe.CustomerService();
                Stripe.CustomerCreateOptions CustomerCreateOptions = new Stripe.CustomerCreateOptions();
                CustomerCreateOptions.Name = name;
                
                CustomerCreateOptions.Address = new Stripe.AddressOptions();
                CustomerCreateOptions.Address.Line1 = address1;
                CustomerCreateOptions.Address.Line2 = address2;
                CustomerCreateOptions.Address.City = city;
                CustomerCreateOptions.Address.State = state;
                CustomerCreateOptions.Address.PostalCode = postalcode;
                CustomerCreateOptions.Address.Country = string.IsNullOrEmpty(country) ? "US" : country;

                Stripe.Customer Customer = CustomerService.Create(CustomerCreateOptions);

                if (Customer.StripeResponse.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    throw new Exception(string.Format("{0}: {1}", Customer.StripeResponse.StatusCode, Customer.StripeResponse.Content));
                }

                return Customer;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public Stripe.Source SaveSource(string customerid, string number,  long? expmonth, long? expyear, string cvc, string name, ImageSolutions.Address.AddressBook addressbook)
        {
            try
            {
                //SaveSource
                Stripe.SourceService SourceService = new SourceService();
                Stripe.SourceCreateOptions SourceCreateOptions = new SourceCreateOptions();

                Stripe.SourceCardOptions SourceCardOptions = new SourceCardOptions();
                SourceCardOptions.Number = number;
                SourceCardOptions.ExpMonth = expmonth;
                SourceCardOptions.ExpYear = expyear;
                SourceCardOptions.Cvc = cvc;
                SourceCreateOptions.Card = SourceCardOptions;
                SourceCreateOptions.Type = SourceType.Card;

                Stripe.SourceOwnerOptions SourceOwnerOptions = new SourceOwnerOptions();
                SourceOwnerOptions.Address = new Stripe.AddressOptions();
                SourceOwnerOptions.Address.Line1 = addressbook.AddressLine1;
                SourceOwnerOptions.Address.Line2 = addressbook.AddressLine2;
                SourceOwnerOptions.Address.City = addressbook.City;
                SourceOwnerOptions.Address.State = addressbook.State;
                SourceOwnerOptions.Address.PostalCode = addressbook.PostalCode;
                SourceOwnerOptions.Address.Country = string.IsNullOrEmpty(addressbook.CountryCode) ? "US" : addressbook.CountryCode;
                SourceOwnerOptions.Name = name;
                SourceCreateOptions.Owner = SourceOwnerOptions;

                Stripe.Source Source = SourceService.Create(SourceCreateOptions);

                if (Source.StripeResponse.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    throw new Exception(string.Format("{0}: {1}", Source.StripeResponse.StatusCode, Source.StripeResponse.Content));
                }

                //Attach Source to Customer
                Stripe.SourceAttachOptions SourceAttachOptions = new SourceAttachOptions();
                SourceAttachOptions.Source = Source.Id;
                SourceService.Attach(customerid, SourceAttachOptions);

                if (Source.StripeResponse.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    throw new Exception(string.Format("{0}: {1}", Source.StripeResponse.StatusCode, Source.StripeResponse.Content));
                }
                return Source;
            }
            catch (Exception ex)
            {
                throw ex;
            }            
        }

        public Stripe.Charge Charge(string customerid, string sourceid, long? amount, string currency, string orderid, string abbreviation = "SO")
        {
            try
            {
                LogCreditCardRequest(customerid, sourceid, string.Empty, string.Empty, amount, GetLocalIPAddress());

                Stripe.ChargeCreateOptions ChargeCreateOptions = new Stripe.ChargeCreateOptions();
                ChargeCreateOptions.Amount = amount;
                ChargeCreateOptions.Currency = currency;
                ChargeCreateOptions.Source = sourceid;
                ChargeCreateOptions.Customer = customerid;
                ChargeCreateOptions.Metadata = new Dictionary<string, string> { { "OrderId", orderid } };

                if (string.IsNullOrEmpty(abbreviation))
                    abbreviation = "SO";

                ChargeCreateOptions.StatementDescriptorSuffix = abbreviation + orderid;
               
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

        public void LogCreditCardRequest(string customerid, string sourceid, string lastfourdigit, string fullname, long? amount, string ipaddress)
        {
            List<ImageSolutions.CreditCard.CreditCardRequestLog> CreditCardRequestLogs = new List<CreditCard.CreditCardRequestLog>();
            ImageSolutions.CreditCard.CreditCardRequestLogFilter CreditCardRequestLogFilter = new CreditCard.CreditCardRequestLogFilter();
            if (!string.IsNullOrEmpty(customerid))
            {
                CreditCardRequestLogFilter.PayerExternalID = new Database.Filter.StringSearch.SearchFilter();
                CreditCardRequestLogFilter.PayerExternalID.SearchString = customerid;
            }
            else
            {
                CreditCardRequestLogFilter.PayerExternalID = new Database.Filter.StringSearch.SearchFilter();
                CreditCardRequestLogFilter.PayerExternalID.Operator = Database.Filter.StringSearch.SearchOperator.empty;
            }
            if (!string.IsNullOrEmpty(sourceid))
            {
                CreditCardRequestLogFilter.CardExternalID = new Database.Filter.StringSearch.SearchFilter();
                CreditCardRequestLogFilter.CardExternalID.SearchString = sourceid;
            }
            else
            {
                CreditCardRequestLogFilter.CardExternalID = new Database.Filter.StringSearch.SearchFilter();
                CreditCardRequestLogFilter.CardExternalID.Operator = Database.Filter.StringSearch.SearchOperator.empty;
            }
            if (!string.IsNullOrEmpty(lastfourdigit))
            {
                CreditCardRequestLogFilter.LastFourDigit = new Database.Filter.StringSearch.SearchFilter();
                CreditCardRequestLogFilter.LastFourDigit.SearchString = lastfourdigit;
            }
            else
            {
                CreditCardRequestLogFilter.LastFourDigit = new Database.Filter.StringSearch.SearchFilter();
                CreditCardRequestLogFilter.LastFourDigit.Operator = Database.Filter.StringSearch.SearchOperator.empty;
            }
            if (!string.IsNullOrEmpty(fullname))
            {
                CreditCardRequestLogFilter.FullName = new Database.Filter.StringSearch.SearchFilter();
                CreditCardRequestLogFilter.FullName.SearchString = fullname;
            }
            else
            {
                CreditCardRequestLogFilter.FullName = new Database.Filter.StringSearch.SearchFilter();
                CreditCardRequestLogFilter.FullName.Operator = Database.Filter.StringSearch.SearchOperator.empty;
            }
            if (!string.IsNullOrEmpty(ipaddress))
            {
                CreditCardRequestLogFilter.IPAddress = new Database.Filter.StringSearch.SearchFilter();
                CreditCardRequestLogFilter.IPAddress.SearchString = ipaddress;
            }
            else
            {
                CreditCardRequestLogFilter.IPAddress = new Database.Filter.StringSearch.SearchFilter();
                CreditCardRequestLogFilter.IPAddress.Operator = Database.Filter.StringSearch.SearchOperator.empty;
            }
            CreditCardRequestLogFilter.Amount = amount;

            CreditCardRequestLogs = ImageSolutions.CreditCard.CreditCardRequestLog.GetCreditCardRequestLogs(CreditCardRequestLogFilter);
            
            if(CreditCardRequestLogs != null && CreditCardRequestLogs.Count > 0)
            {
                DateTime LastUsed = CreditCardRequestLogs.Max(x => x.CreatedOn);
                if((DateTime.UtcNow - LastUsed).TotalMinutes < 3)
                {
                    //throw new Exception("Credit Card: charging in progress");
                }
            }

            ImageSolutions.CreditCard.CreditCardRequestLog CreditCardRequestLog = new CreditCard.CreditCardRequestLog();
            CreditCardRequestLog.LastFourDigit = lastfourdigit;
            CreditCardRequestLog.PayerExternalID = customerid;
            CreditCardRequestLog.CardExternalID = sourceid;
            CreditCardRequestLog.FullName = fullname;
            CreditCardRequestLog.Amount = Convert.ToDecimal(amount);
            CreditCardRequestLog.IPAddress = ipaddress;
            CreditCardRequestLog.Create();
        }

        public string GetLocalIPAddress()
        {
            string strReturn = string.Empty;

            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    strReturn = ip.ToString();
                }
            }
            return strReturn;
        }

        public Stripe.Charge ChargeNewCard(
            string name, string number, long? expmonth, long? expyear, string cvc, string cctype
            , long? amount, string orderid, string currency
            , ImageSolutions.Address.AddressBook address, ImageSolutions.User.UserInfo userinfo
            , bool savecard, string nickname, ImageSolutions.CreditCard.CreditCard creditcard = null
            , SqlConnection conn = null, SqlTransaction trans = null, string abbreviation = "SO")
        {
            try
            {
                string strLastFourDigit = number.Trim().Length >= 4 ? number.Trim().Substring(number.Trim().Length - 4) : number.Trim();
                LogCreditCardRequest(string.Empty, string.Empty, strLastFourDigit, name, amount, GetLocalIPAddress());

                //Save Customer
                Stripe.CustomerService CustomerService = new Stripe.CustomerService();
                Stripe.CustomerCreateOptions CustomerCreateOptions = new Stripe.CustomerCreateOptions();
                CustomerCreateOptions.Name = name;

                CustomerCreateOptions.Address = new Stripe.AddressOptions();
                CustomerCreateOptions.Address.Line1 = address.AddressLine1;
                CustomerCreateOptions.Address.Line2 = address.AddressLine2;
                CustomerCreateOptions.Address.City = address.City;
                CustomerCreateOptions.Address.State = address.State;
                CustomerCreateOptions.Address.PostalCode = address.PostalCode;
                CustomerCreateOptions.Address.Country = address.CountryCode;

                Stripe.Customer Customer = CustomerService.Create(CustomerCreateOptions);

                if (Customer.StripeResponse.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    throw new Exception(string.Format("{0}: {1}", Customer.StripeResponse.StatusCode, Customer.StripeResponse.Content));
                }

                //SaveSource
                Stripe.SourceService SourceService = new SourceService();
                Stripe.SourceCreateOptions SourceCreateOptions = new SourceCreateOptions();

                Stripe.SourceCardOptions SourceCardOptions = new SourceCardOptions();
                SourceCardOptions.Number = number;
                SourceCardOptions.ExpMonth = expmonth;
                SourceCardOptions.ExpYear = expyear;
                SourceCardOptions.Cvc = cvc;
                SourceCreateOptions.Card = SourceCardOptions;
                SourceCreateOptions.Type = SourceType.Card;

                Stripe.SourceOwnerOptions SourceOwnerOptions = new SourceOwnerOptions();
                SourceOwnerOptions.Address = new Stripe.AddressOptions();
                SourceOwnerOptions.Address.Line1 = address.AddressLine1;
                SourceOwnerOptions.Address.Line2 = address.AddressLine2;
                SourceOwnerOptions.Address.City = address.City;
                SourceOwnerOptions.Address.State = address.State;
                SourceOwnerOptions.Address.PostalCode = address.PostalCode;
                SourceOwnerOptions.Address.Country = string.IsNullOrEmpty(address.CountryCode) ? "US" : address.CountryCode;
                SourceOwnerOptions.Name = name;
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
                ChargeCreateOptions.Amount = amount;
                ChargeCreateOptions.Currency = currency;
                ChargeCreateOptions.Source = Source.Id;
                ChargeCreateOptions.Customer = Customer.Id;
                ChargeCreateOptions.Metadata = new Dictionary<string, string> { { "OrderId", orderid } };

                if (string.IsNullOrEmpty(abbreviation))
                    abbreviation = "SO";

                ChargeCreateOptions.StatementDescriptorSuffix = abbreviation + orderid;

                Stripe.ChargeService ChargeService = new Stripe.ChargeService();
                Stripe.Charge Charge = ChargeService.Create(ChargeCreateOptions);

                if (Charge.StripeResponse.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    throw new Exception(string.Format("{0}: {1}", Charge.StripeResponse.StatusCode, Charge.StripeResponse.Content));
                }

                if (savecard)
                {
                    //Store Payer and Card 
                    ImageSolutions.CreditCard.CreditCard CreditCard = new CreditCard.CreditCard();
                    Guid Guid = new Guid();
                    CreditCard.GUID = Convert.ToString(Guid.NewGuid());
                    CreditCard.Data = Encrypt(number.Trim(), CreditCard.GUID);
                    CreditCard.Nickname = nickname;
                    CreditCard.FullName = name;
                    CreditCard.LastFourDigit = number.Trim().Length >= 4 ? number.Trim().Substring(number.Trim().Length - 4) : number.Trim();
                    CreditCard.CreditCardType = cctype;
                    CreditCard.ExpirationDate = Convert.ToDateTime(String.Format("{0}/1/{1}", Convert.ToString(expmonth), Convert.ToString(expyear)));
                    CreditCard.CVV = cvc;
                    CreditCard.BillingAddressBookID = address.AddressBookID;
                    CreditCard.CreatedBy = userinfo.UserInfoID;
                    CreditCard.PayerExternalID = Customer.Id;
                    CreditCard.CardExternalID = Source.Id;

                    if(conn != null && trans != null)
                    {
                        CreditCard.Create(conn, trans);
                    }
                    else
                    {
                        CreditCard.Create();
                    }

                    ImageSolutions.User.UserCreditCard UserCreditCard = new ImageSolutions.User.UserCreditCard();
                    UserCreditCard.UserInfoID = userinfo.UserInfoID;
                    UserCreditCard.CreditCardID = CreditCard.CreditCardID;
                    UserCreditCard.CreatedBy = userinfo.UserInfoID;

                    if (conn != null && trans != null)
                    {
                        UserCreditCard.Create(conn, trans);
                    }
                    else
                    {
                        UserCreditCard.Create();
                    }

                    Charge.Description = CreditCard.CreditCardID;

                }
                else if (creditcard != null)
                {
                    creditcard.PayerExternalID = Customer.Id;
                    creditcard.CardExternalID = Source.Id;
                    if (conn != null && trans != null)
                    {
                        creditcard.Update(conn, trans);
                    }
                    else
                    {
                        creditcard.Update();
                    }

                    Charge.Description = creditcard.CreditCardID;
                }

                return Charge;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void Refund(ImageSolutions.Refund.Refund refund)
        {
            try
            {
                decimal amount = refund.Amount * 100;

                Stripe.RefundCreateOptions RefundCreateOptions = new RefundCreateOptions();

                if(refund.TransactionNumber.Substring(0,2) == "ch")
                {
                    RefundCreateOptions.Charge = refund.TransactionNumber;
                    RefundCreateOptions.Amount = (long)amount;
                }
                else if (refund.TransactionNumber.Substring(0, 2) == "pi")
                {
                    RefundCreateOptions.PaymentIntent = refund.TransactionNumber;
                    RefundCreateOptions.Amount = (long)amount;
                }
                else
                {
                    throw new Exception(String.Format("Invalid Transaction Number: {0}", refund.TransactionNumber));
                }

                Stripe.RefundService RefundService = new RefundService();
                Stripe.Refund Refund = RefundService.Create(RefundCreateOptions);

                if (Refund.StripeResponse.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    refund.ErrorMessage = string.Format("{0}: {1}", Refund.StripeResponse.StatusCode, Refund.StripeResponse.Content);
                    refund.Update();
                    throw new Exception(string.Format("{0}: {1}", Refund.StripeResponse.StatusCode, Refund.StripeResponse.Content));
                }
                else
                {
                    refund.ExternalID = Refund.Id;
                    refund.Status = "Processed";
                    refund.Response = Refund.StripeResponse.Content;
                    refund.ErrorMessage = String.Empty;
                    refund.Update();
                }
            }
            catch(Exception ex)
            {
                refund.ErrorMessage = string.Format("{0}", Convert.ToString(ex.Message));
                refund.Update();
            }           
        }
        public void Refund(string transactionnumber)
        {
            try
            {
                Stripe.RefundCreateOptions RefundCreateOptions = new RefundCreateOptions();
                RefundCreateOptions.Charge = transactionnumber;

                Stripe.RefundService RefundService = new RefundService();
                Stripe.Refund Refund = RefundService.Create(RefundCreateOptions);

                if (Refund.StripeResponse.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    //Error string.Format("{0}: {1}", Refund.StripeResponse.StatusCode, Refund.StripeResponse.Content);
                }
            }
            catch (Exception ex)
            {
                
            }
        }
        public void RefundPaymentIntent(string pyamentintentnumber)
        {
            try
            {
                Stripe.RefundCreateOptions RefundCreateOptions = new RefundCreateOptions();
                RefundCreateOptions.PaymentIntent = pyamentintentnumber;

                Stripe.RefundService RefundService = new RefundService();
                Stripe.Refund Refund = RefundService.Create(RefundCreateOptions);

                if (Refund.StripeResponse.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    //Error string.Format("{0}: {1}", Refund.StripeResponse.StatusCode, Refund.StripeResponse.Content);
                }
            }
            catch (Exception ex)
            {

            }
        }
        public void TestPayout()
        {
            try
            {
                Stripe.PayoutListOptions PayoutListOptions = new PayoutListOptions();
                //if (arrivaldate != null)
                //{
                //    DateRangeOptions DateRangeOptions = new DateRangeOptions();
                //    DateRangeOptions.GreaterThan = arrivaldate;
                //    PayoutListOptions.ArrivalDate = DateRangeOptions;
                //}
                PayoutListOptions.Status = "paid";

                Stripe.PayoutService PayoutService = new PayoutService();
                StripeList<Stripe.Payout> Payouts = PayoutService.List(PayoutListOptions);

                foreach (Stripe.Payout _Payout in Payouts)
                {
                    SqlConnection objConn = null;
                    SqlTransaction objTran = null;

                    try
                    {
                        Stripe.BalanceTransactionListOptions BalanceTransactionListOptions = new BalanceTransactionListOptions();
                        BalanceTransactionListOptions.Payout = _Payout.Id;
                        BalanceTransactionListOptions.Limit = 2;

                        BalanceTransactionService BalanceTransactionService = new BalanceTransactionService();
                        StripeList<BalanceTransaction> BalanceTransactions = BalanceTransactionService.List(BalanceTransactionListOptions);

                        List<ImageSolutions.Payout.PayoutLine> PayoutLines = new List<ImageSolutions.Payout.PayoutLine>();

                        foreach (BalanceTransaction _BalanceTransaction in BalanceTransactions)
                        {
                            ImageSolutions.Payout.PayoutLine PayoutLine = new ImageSolutions.Payout.PayoutLine();
                            PayoutLine.PayoutID = _Payout.Id;
                            PayoutLine.ExternalID = _BalanceTransaction.Id;
                            PayoutLine.SourceID = _BalanceTransaction.SourceId;
                            PayoutLine.Amount = Convert.ToDecimal(_BalanceTransaction.Amount) / Convert.ToDecimal(100.0);
                            PayoutLine.Type = _BalanceTransaction.Type;
                            PayoutLine.Description = _BalanceTransaction.Description;

                            PayoutLines.Add(PayoutLine);
                        }

                        while(BalanceTransactions.HasMore)
                        {
                            BalanceTransactionListOptions.StartingAfter = BalanceTransactions.Data.Last().Id;
                            BalanceTransactions = BalanceTransactionService.List(BalanceTransactionListOptions);
                            foreach (BalanceTransaction _BalanceTransaction in BalanceTransactions)
                            {
                                ImageSolutions.Payout.PayoutLine PayoutLine = new ImageSolutions.Payout.PayoutLine();
                                PayoutLine.PayoutID = _Payout.Id;
                                PayoutLine.ExternalID = _BalanceTransaction.Id;
                                PayoutLine.SourceID = _BalanceTransaction.SourceId;
                                PayoutLine.Amount = Convert.ToDecimal(_BalanceTransaction.Amount) / Convert.ToDecimal(100.0);
                                PayoutLine.Type = _BalanceTransaction.Type;
                                PayoutLine.Description = _BalanceTransaction.Description;

                                PayoutLines.Add(PayoutLine);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        objTran.Rollback();
                        throw ex;
                    }
                    finally
                    {
                        if (objTran != null) objTran.Dispose();
                        objTran = null;
                        if (objConn != null) objConn.Dispose();
                        objConn = null;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void GetPayout(DateTime? arrivaldate)
        {
            try
            {
                Stripe.PayoutListOptions PayoutListOptions = new PayoutListOptions();
                if (arrivaldate != null)
                {
                    DateRangeOptions DateRangeOptions = new DateRangeOptions();
                    DateRangeOptions.GreaterThan = arrivaldate;
                    PayoutListOptions.ArrivalDate = DateRangeOptions;
                }
                PayoutListOptions.Status = "paid";

                Stripe.PayoutService PayoutService = new PayoutService();
                StripeList<Stripe.Payout> Payouts = PayoutService.List(PayoutListOptions);

                foreach (Stripe.Payout _Payout in Payouts)
                {
                    SqlConnection objConn = null;
                    SqlTransaction objTran = null;

                    try
                    {
                        objConn = new SqlConnection(Database.DefaultConnectionString);
                        objConn.Open();
                        objTran = objConn.BeginTransaction();

                        ImageSolutions.Payout.Payout Payout = new ImageSolutions.Payout.Payout();
                        ImageSolutions.Payout.PayoutFilter PayoutFilter = new ImageSolutions.Payout.PayoutFilter();
                        PayoutFilter.ExternalID = new Database.Filter.StringSearch.SearchFilter();
                        PayoutFilter.ExternalID.SearchString = _Payout.Id;
                        Payout = ImageSolutions.Payout.Payout.GetPayout(PayoutFilter);

                        if (Payout == null)
                        {
                            Payout = new ImageSolutions.Payout.Payout();
                            Payout.ExternalID = _Payout.Id;
                            Payout.Amount = Convert.ToDecimal(_Payout.Amount) / Convert.ToDecimal(100.0);
                            Payout.Description = _Payout.Description;
                            Payout.Status = "Pending";
                            Payout.ArrivalDate = _Payout.ArrivalDate;
                            Payout.Create(objConn, objTran);

                            Stripe.BalanceTransactionListOptions BalanceTransactionListOptions = new BalanceTransactionListOptions();
                            BalanceTransactionListOptions.Payout = _Payout.Id;
                            BalanceTransactionListOptions.Limit = 100;

                            BalanceTransactionService BalanceTransactionService = new BalanceTransactionService();
                            StripeList<BalanceTransaction> BalanceTransactions = BalanceTransactionService.List(BalanceTransactionListOptions);

                            foreach (BalanceTransaction _BalanceTransaction in BalanceTransactions)
                            {
                                ImageSolutions.Payout.PayoutLine PayoutLine = new ImageSolutions.Payout.PayoutLine();
                                PayoutLine.PayoutID = Payout.PayoutID;
                                PayoutLine.ExternalID = _BalanceTransaction.Id;
                                PayoutLine.SourceID = _BalanceTransaction.SourceId;

                                if (!string.IsNullOrEmpty(PayoutLine.SourceID) && PayoutLine.SourceID.StartsWith("ch"))
                                {
                                    ChargeService ChargeService = new ChargeService();
                                    Charge Charge = ChargeService.Get(PayoutLine.SourceID);
                                    if (Charge != null && !string.IsNullOrEmpty(Charge.PaymentIntentId))
                                    {
                                        PayoutLine.IntentID = Charge.PaymentIntentId;
                                    }
                                }                           

                                PayoutLine.Amount = Convert.ToDecimal(_BalanceTransaction.Amount) / Convert.ToDecimal(100.0);
                                PayoutLine.Type = _BalanceTransaction.Type;
                                PayoutLine.Description = _BalanceTransaction.Description;
                                PayoutLine.Create(objConn, objTran);
                            }

                            while (BalanceTransactions.HasMore)
                            {
                                BalanceTransactionListOptions.StartingAfter = BalanceTransactions.Data.Last().Id;
                                BalanceTransactions = BalanceTransactionService.List(BalanceTransactionListOptions);
                                foreach (BalanceTransaction _BalanceTransaction in BalanceTransactions)
                                {
                                    ImageSolutions.Payout.PayoutLine PayoutLine = new ImageSolutions.Payout.PayoutLine();
                                    PayoutLine.PayoutID = Payout.PayoutID;
                                    PayoutLine.ExternalID = _BalanceTransaction.Id;
                                    PayoutLine.SourceID = _BalanceTransaction.SourceId;

                                    if (!string.IsNullOrEmpty(PayoutLine.SourceID) && PayoutLine.SourceID.StartsWith("ch"))
                                    {
                                        ChargeService ChargeService = new ChargeService();
                                        Charge Charge = ChargeService.Get(PayoutLine.SourceID);
                                        if (Charge != null && !string.IsNullOrEmpty(Charge.PaymentIntentId))
                                        {
                                            PayoutLine.IntentID = Charge.PaymentIntentId;
                                        }
                                    }

                                    PayoutLine.Amount = Convert.ToDecimal(_BalanceTransaction.Amount) / Convert.ToDecimal(100.0);
                                    PayoutLine.Type = _BalanceTransaction.Type;
                                    PayoutLine.Description = _BalanceTransaction.Description;
                                    PayoutLine.Create(objConn, objTran);
                                }
                            }
                        }

                        objTran.Commit();
                    }
                    catch (Exception ex)
                    {
                        objTran.Rollback();
                        throw ex;
                    }
                    finally
                    {
                        if (objTran != null) objTran.Dispose();
                        objTran = null;
                        if (objConn != null) objConn.Dispose();
                        objConn = null;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void UpdatePayoutLinePaymentIntent(ImageSolutions.Payout.PayoutLine payoutline)
        {
            try
            {
                if(payoutline != null && string.IsNullOrEmpty(payoutline.IntentID))
                {
                    if (!string.IsNullOrEmpty(payoutline.SourceID) && payoutline.SourceID.StartsWith("ch"))
                    {
                        ChargeService ChargeService = new ChargeService();
                        Charge Charge = ChargeService.Get(payoutline.SourceID);
                        if (Charge != null && !string.IsNullOrEmpty(Charge.PaymentIntentId))
                        {
                            payoutline.IntentID = Charge.PaymentIntentId;
                            payoutline.Update();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static DateTime UnixTimeStampToDateTime(double unixtimestamp)
        {
            // Unix timestamp is seconds past epoch
            DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dateTime = dateTime.AddSeconds(unixtimestamp).ToLocalTime();
            return dateTime;
        }
        public void Test()
        {            
            //Customer
            Stripe.CustomerService CustomerService = new Stripe.CustomerService();
            // Create a Customer:
            //Stripe.CustomerCreateOptions CustomerCreateOptions = new Stripe.CustomerCreateOptions
            //{
            //    Email = "steve@imageinc.com",
            //};
            //Stripe.Customer Customer = CustomerService.Create(CustomerCreateOptions);
            //Get Customer - cus_O2DzWuhwu0mQFd
            Stripe.Customer Customer = CustomerService.Get("cus_O2DzWuhwu0mQFd");

            //Test
            //4242424242424242 - Visa - 3 digit - US
            //4000056655665556 - Visa Debut - 3 digit
            //5555555555554444 - Master - 3 digit
            //5200828282828210 - Master Debut - 3 digit
            //378282246310005 - American Express - 4 digit
            //6011111111111117 - Discover

            //4242424242424242 - Visa - US
            //4000001240000000 - Visa - CA
           
//Stripe.TokenService TokenService = new Stripe.TokenService();
//Stripe.TokenCreateOptions TokenCreateOptions = new Stripe.TokenCreateOptions();
//Stripe.TokenCardOptions TokenCardOptions = new Stripe.TokenCardOptions();
//TokenCardOptions.Name = "Steve Il Soo Park";
//TokenCardOptions.Number = "4242424242424242";
//TokenCardOptions.ExpMonth = "6";
//TokenCardOptions.ExpYear = "2024";
//TokenCardOptions.Cvc = "314";
//TokenCardOptions.AddressLine1 = "12561 Camus Ln";
//TokenCardOptions.AddressLine2 = "#6";
//TokenCardOptions.AddressCity = "Garden Grove";
//TokenCardOptions.AddressState = "CA";
//TokenCardOptions.AddressZip = "92841";
//TokenCardOptions.AddressCountry = "US";
//TokenCardOptions.Currency = "usd";
//TokenCreateOptions.Card = TokenCardOptions;
//Stripe.Token Token = TokenService.Create(TokenCreateOptions);

//Stripe.CardService CardService = new CardService();
//Stripe.CardCreateNestedOptions CardCreateNestedOptions = new CardCreateNestedOptions();
//CardCreateNestedOptions.Name = "Steve Park";
//CardCreateNestedOptions.Number = "4242424242424242";
//CardCreateNestedOptions.AddressLine1 = "12561 Camus Ln";
//CardCreateNestedOptions.AddressLine2 = "#6";
//CardCreateNestedOptions.AddressCity = "Garden Grove";
//CardCreateNestedOptions.AddressState = "CA";
//CardCreateNestedOptions.AddressZip = "92841";
//CardCreateNestedOptions.AddressCountry = "US";
////CardCreateNestedOptions.Currency = "USD";
//CardCreateNestedOptions.Cvc = "123";
//CardCreateNestedOptions.ExpMonth = 8;
//CardCreateNestedOptions.ExpYear = 2024;
//Stripe.CardCreateOptions CardCreateOptions = new CardCreateOptions();
//CardCreateOptions.Source = CardCreateNestedOptions;
//Stripe.Card Card = CardService.Create(Customer.Id, CardCreateOptions);


            //Stripe.SourceService SourceService = new SourceService();
            //Stripe.SourceCreateOptions SourceCreateOptions = new SourceCreateOptions();

            //Stripe.SourceCardOptions SourceCardOptions = new SourceCardOptions();
            ////SourceCardOptions.Name = "Steve Il Soo Park";
            //SourceCardOptions.Number = "4242424242424242";
            //SourceCardOptions.ExpMonth = 6;
            //SourceCardOptions.ExpYear = 2024;
            //SourceCardOptions.Cvc = "314";
            ////SourceCardOptions.AddressLine1 = "12561 Camus Ln";
            ////SourceCardOptions.AddressLine2 = "#6";
            ////SourceCardOptions.AddressCity = "Garden Grove";
            ////SourceCardOptions.AddressState = "CA";
            ////SourceCardOptions.AddressZip = "92841";
            ////SourceCardOptions.AddressCountry = "US";
            ////SourceCardOptions.Currency = "usd";

            //SourceCreateOptions.Type = SourceType.Card;
            //SourceCreateOptions.Card = SourceCardOptions;
            ////SourceCreateOptions.Customer = "cus_O2DzWuhwu0mQFd";
            //Stripe.Source Source = SourceService.Create(SourceCreateOptions);

            //Attach Source to Customer
            //Stripe.SourceAttachOptions SourceAttachOptions = new SourceAttachOptions();
            //SourceAttachOptions.Source = "src_1NGpy2L8BQnKd8oDlpTEM1Wu";
            //SourceService.Attach(Customer.Id, SourceAttachOptions);

            //// Save Card ID
            // Save the customer ID and other info in a database for later.

            // When it's time to charge the customer again, retrieve the customer ID.
            Stripe.ChargeCreateOptions ChargeCreateOptions = new Stripe.ChargeCreateOptions();
            ChargeCreateOptions.Amount = 300;
            ChargeCreateOptions.Currency = "usd";
            ChargeCreateOptions.Source = "src_1NGpy2L8BQnKd8oDlpTEM1Wu";
            ChargeCreateOptions.Customer = "cus_O2DzWuhwu0mQFd";
            ChargeCreateOptions.Metadata = new Dictionary<string, string> { { "OrderId", "101" } };
            
            Stripe.ChargeService ChargeService = new Stripe.ChargeService();
            Stripe.Charge Charge = ChargeService.Create(ChargeCreateOptions);
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
