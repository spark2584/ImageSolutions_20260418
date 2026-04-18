using NetSuiteLibrary.com.netsuite.webservices;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ImageSolutionsWebsite.Payment
{
    public partial class Invoice : BasePage
    {
        private NetSuiteService mService = null;

        public NetSuiteService Service
        {
            get
            {
                NetSuiteLibrary.User objUser = null;

                objUser = new NetSuiteLibrary.User();

                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                NetSuiteService mNetSuiteService = new NetSuiteService();
                mNetSuiteService.tokenPassport = objUser.TokenPassport();
                mNetSuiteService.Url = new Uri(mNetSuiteService.getDataCenterUrls(objUser.TokenPassport().account).dataCenterUrls.webservicesDomain + new Uri(mNetSuiteService.Url).PathAndQuery).ToString();

                return mNetSuiteService;
            }
        }

        protected string mInvoiceInternalID = string.Empty;
        protected string mCustomerInternalID = string.Empty;

        protected void Page_Load(object sender, EventArgs e)
        {
            mInvoiceInternalID = Request.QueryString.Get("invoice");
            mCustomerInternalID = Request.QueryString.Get("customer");

            if (!Page.IsPostBack)
            {
                if(!string.IsNullOrEmpty(mInvoiceInternalID) || !string.IsNullOrEmpty(mCustomerInternalID))
                {
                    try
                    {
                        BindInvoices();
                        CalculateTotal();
                    }
                    catch (Exception ex)
                    {
                        WebUtility.DisplayJavascriptMessage(this, ex.Message);
                    }
                }
            } 
        }

        protected void BindInvoices()
        {
            List<NetSuiteLibrary.com.netsuite.webservices.Invoice> Invoices = GetInvoiceSavedSearch("298843");

            gvInvoice.DataSource = Invoices;
            gvInvoice.DataBind();
        }

        public List<NetSuiteLibrary.com.netsuite.webservices.Invoice> GetInvoiceSavedSearch(string savedsearchid)
        {
            List<NetSuiteLibrary.com.netsuite.webservices.Invoice> Invoices = null;
            SearchResult SearchResult = null;
            Dictionary<string, string> dicInternalIds = new Dictionary<string, string>();


            SearchResult ChildSearchResult = null;

            try
            {
                Invoices = new List<NetSuiteLibrary.com.netsuite.webservices.Invoice>();
                SearchResult = GetSavedSearch(savedsearchid);

                int count = 0;

                if (SearchResult != null && SearchResult.totalRecords > 0)
                {
                    do
                    {
                        if (SearchResult.searchRowList != null)
                        {
                            foreach (TransactionSearchRow _TransactionSearchRow in SearchResult.searchRowList)
                            {
                                count++;
                                Console.WriteLine(string.Format("{0}/{1}", count, SearchResult.totalRecords));

                                NetSuiteLibrary.com.netsuite.webservices.Invoice Invoice = new NetSuiteLibrary.com.netsuite.webservices.Invoice();
                                Invoice.internalId = _TransactionSearchRow.basic.internalId[0].searchValue.internalId;
                                Invoice.tranDate = _TransactionSearchRow.basic.tranDate[0].searchValue;
                                Invoice.tranId = _TransactionSearchRow.basic.tranId[0].searchValue;
                                Invoice.entity = _TransactionSearchRow.basic.entity[0].searchValue;
                                Invoice.amountRemaining = _TransactionSearchRow.basic.amountRemaining[0].searchValue;
                                Invoice.total = _TransactionSearchRow.basic.amount[0].searchValue;
                                if (_TransactionSearchRow.basic.dueDate != null)
                                {
                                    Invoice.dueDate = Convert.ToDateTime(_TransactionSearchRow.basic.dueDate[0].searchValue);
                                }
                                if (_TransactionSearchRow.customerJoin != null && _TransactionSearchRow.customerJoin.entityId != null)
                                {
                                    string strName = Convert.ToString(_TransactionSearchRow.customerJoin.entityId[0].searchValue);

                                    strName = strName.Substring(strName.LastIndexOf(" : ") + 3);
                                    Invoice.entity.name = strName;
                                }

                                if(!Invoices.Exists(x => x.internalId == Invoice.internalId))
                                {
                                    Invoices.Add(Invoice);
                                }
                            }
                            Service.tokenPassport = new NetSuiteLibrary.User().TokenPassport();
                            SearchResult = Service.searchMoreWithId(SearchResult.searchId, SearchResult.pageIndex + 1);
                        }
                    }
                    while (SearchResult.searchRowList != null && SearchResult.pageSizeSpecified == true && SearchResult.totalPages >= SearchResult.pageIndex);
                }


                if (!string.IsNullOrEmpty(mCustomerInternalID))
                {
                    ChildSearchResult = GetChildSavedSearch(savedsearchid);
                }

                if (ChildSearchResult != null && ChildSearchResult.totalRecords > 0)
                {	
                    do
                    {
                        if (ChildSearchResult.searchRowList != null)
                        {
                            foreach (TransactionSearchRow _TransactionSearchRow in ChildSearchResult.searchRowList)
                            {
                                count++;
                                Console.WriteLine(string.Format("{0}/{1}", count, ChildSearchResult.totalRecords));

                                NetSuiteLibrary.com.netsuite.webservices.Invoice Invoice = new NetSuiteLibrary.com.netsuite.webservices.Invoice();
                                Invoice.internalId = _TransactionSearchRow.basic.internalId[0].searchValue.internalId;
                                Invoice.tranDate = _TransactionSearchRow.basic.tranDate[0].searchValue;
                                Invoice.tranId = _TransactionSearchRow.basic.tranId[0].searchValue;
                                Invoice.entity = _TransactionSearchRow.basic.entity[0].searchValue;
                                Invoice.amountRemaining = _TransactionSearchRow.basic.amountRemaining[0].searchValue;
                                Invoice.total = _TransactionSearchRow.basic.amount[0].searchValue;
                                if (_TransactionSearchRow.basic.dueDate != null)
                                {
                                    Invoice.dueDate = Convert.ToDateTime(_TransactionSearchRow.basic.dueDate[0].searchValue);
                                }
                                if (_TransactionSearchRow.customerJoin != null && _TransactionSearchRow.customerJoin.entityId != null)
                                {
                                    string strName = Convert.ToString(_TransactionSearchRow.customerJoin.entityId[0].searchValue);

                                    strName = strName.Substring(strName.LastIndexOf(" : ") + 3);
                                    Invoice.entity.name = strName;
                                }

                                if (!Invoices.Exists(x => x.internalId == Invoice.internalId))
                                {
                                    Invoices.Add(Invoice);
                                }
                            }
                            Service.tokenPassport = new NetSuiteLibrary.User().TokenPassport();
                            ChildSearchResult = Service.searchMoreWithId(ChildSearchResult.searchId, ChildSearchResult.pageIndex + 1);
                        }
                    }
                    while (ChildSearchResult.searchRowList != null && ChildSearchResult.pageSizeSpecified == true && ChildSearchResult.totalPages >= ChildSearchResult.pageIndex);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return Invoices;
        }

        public SearchResult GetSavedSearch(string savedsearchid)
        {
            TransactionSearchAdvanced TransactionSearchAdvanced = null;
            SearchResult SearchResult = null;
            //TransactionSearch TransactionSearch = null;
            SearchPreferences SearchPreferences = null;

            try
            {
                TransactionSearchAdvanced = new TransactionSearchAdvanced();
                TransactionSearchAdvanced.savedSearchId = savedsearchid;

                if(!string.IsNullOrEmpty(mInvoiceInternalID))
                {
                    SearchMultiSelectField SearchMultiSelectField = new SearchMultiSelectField();
                    SearchMultiSelectField.searchValue = new RecordRef[1];
                    SearchMultiSelectField.searchValue[0] = new RecordRef();
                    SearchMultiSelectField.searchValue[0].internalId = mInvoiceInternalID;
                    SearchMultiSelectField.searchValue[0].type = RecordType.salesOrder;
                    SearchMultiSelectField.searchValue[0].typeSpecified = true;
                    SearchMultiSelectField.@operator = SearchMultiSelectFieldOperator.anyOf;
                    SearchMultiSelectField.operatorSpecified = true;
                    TransactionSearchAdvanced.criteria = new TransactionSearch();
                    TransactionSearchAdvanced.criteria.basic = new TransactionSearchBasic();
                    TransactionSearchAdvanced.criteria.basic.internalId = SearchMultiSelectField;
                }

                if(!string.IsNullOrEmpty(mCustomerInternalID))
                {
                    TransactionSearchAdvanced.criteria = new TransactionSearch();
                    TransactionSearchAdvanced.criteria.basic = new TransactionSearchBasic();
                    TransactionSearchAdvanced.criteria.customerJoin = new CustomerSearchBasic();
                    TransactionSearchAdvanced.criteria.customerJoin.internalId = new SearchMultiSelectField();
                    TransactionSearchAdvanced.criteria.customerJoin.internalId.searchValue = new RecordRef[1];
                    TransactionSearchAdvanced.criteria.customerJoin.internalId.searchValue[0] = new RecordRef();
                    TransactionSearchAdvanced.criteria.customerJoin.internalId.searchValue[0].internalId = mCustomerInternalID;
                    TransactionSearchAdvanced.criteria.customerJoin.internalId.searchValue[0].type = RecordType.customer;
                    TransactionSearchAdvanced.criteria.customerJoin.internalId.searchValue[0].typeSpecified = true;
                    TransactionSearchAdvanced.criteria.customerJoin.internalId.@operator = SearchMultiSelectFieldOperator.anyOf;
                    TransactionSearchAdvanced.criteria.customerJoin.internalId.@operatorSpecified = true;
                }

                SearchPreferences = new SearchPreferences();
                SearchPreferences.returnSearchColumns = true;
                Service.searchPreferences = SearchPreferences;

                SearchResult = Service.search(TransactionSearchAdvanced);

                if (SearchResult.status.isSuccess != true) throw new Exception(string.Format("Cannot find Saved Search - {0}: {1}", savedsearchid, SearchResult.status.statusDetail[0].message));

            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {

            }
            return SearchResult;
        }

        public SearchResult GetChildSavedSearch(string savedsearchid)
        {
            TransactionSearchAdvanced TransactionSearchAdvanced = null;
            SearchResult SearchResult = null;
            //TransactionSearch TransactionSearch = null;
            SearchPreferences SearchPreferences = null;

            try
            {
                TransactionSearchAdvanced = new TransactionSearchAdvanced();
                TransactionSearchAdvanced.savedSearchId = savedsearchid;

                if (!string.IsNullOrEmpty(mCustomerInternalID))
                {
                    TransactionSearchAdvanced.criteria = new TransactionSearch();
                    TransactionSearchAdvanced.criteria.basic = new TransactionSearchBasic();
                    TransactionSearchAdvanced.criteria.customerJoin = new CustomerSearchBasic();
                    TransactionSearchAdvanced.criteria.customerJoin.parent = new SearchMultiSelectField();
                    TransactionSearchAdvanced.criteria.customerJoin.parent.searchValue = new RecordRef[1];
                    TransactionSearchAdvanced.criteria.customerJoin.parent.searchValue[0] = new RecordRef();
                    TransactionSearchAdvanced.criteria.customerJoin.parent.searchValue[0].internalId = mCustomerInternalID;
                    TransactionSearchAdvanced.criteria.customerJoin.parent.searchValue[0].type = RecordType.customer;
                    TransactionSearchAdvanced.criteria.customerJoin.parent.searchValue[0].typeSpecified = true;
                    TransactionSearchAdvanced.criteria.customerJoin.parent.@operator = SearchMultiSelectFieldOperator.anyOf;
                    TransactionSearchAdvanced.criteria.customerJoin.parent.@operatorSpecified = true;
                }

                SearchPreferences = new SearchPreferences();
                SearchPreferences.returnSearchColumns = true;
                Service.searchPreferences = SearchPreferences;

                SearchResult = Service.search(TransactionSearchAdvanced);

                if (SearchResult.status.isSuccess != true) throw new Exception(string.Format("Cannot find Saved Search - {0}: {1}", savedsearchid, SearchResult.status.statusDetail[0].message));

            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {

            }
            return SearchResult;
        }

        private NetSuiteLibrary.com.netsuite.webservices.Invoice LoadNetSuiteInvoice(string netsuiteinternalid)
        {
            RecordRef objInvoiceRef = null;
            ReadResponse objReadResult = null;
            NetSuiteLibrary.com.netsuite.webservices.Invoice objReturn = null;
            try
            {
                objInvoiceRef = new RecordRef();
                objInvoiceRef.internalId = netsuiteinternalid;
                objInvoiceRef.type = RecordType.invoice;
                objInvoiceRef.typeSpecified = true;

                objReadResult = Service.get(objInvoiceRef);

                if (objReadResult != null && objReadResult.record != null && (objReadResult.record is NetSuiteLibrary.com.netsuite.webservices.Invoice))
                {
                    objReturn = (NetSuiteLibrary.com.netsuite.webservices.Invoice) objReadResult.record;
                }
                else
                {
                    throw new Exception("Can not find Customer with Internal ID : " + netsuiteinternalid);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                objInvoiceRef = null;
                objReadResult = null;
            }
            return objReturn;
        }


        protected static string NetSuiteAccountNumber
        {
            get
            {
                if (ConfigurationManager.AppSettings["NetSuiteAccountNumber"] != null)
                    return ConfigurationManager.AppSettings["NetSuiteAccountNumber"].ToString();
                else
                    return string.Empty;
            }
        }

        protected static string AppConsumerKey
        {
            get
            {
                if (ConfigurationManager.AppSettings["AppConsumerKey"] != null)
                    return ConfigurationManager.AppSettings["AppConsumerKey"].ToString();
                else
                    throw new Exception("Missing AppConsumerKey");
            }
        }

        protected static string AppConsumerSecret
        {
            get
            {
                if (ConfigurationManager.AppSettings["AppConsumerSecret"] != null)
                    return ConfigurationManager.AppSettings["AppConsumerSecret"].ToString();
                else
                    throw new Exception("Missing AppConsumerSecret");
            }
        }

        protected static string TokenID
        {
            get
            {
                if (ConfigurationManager.AppSettings["TokenID"] != null)
                    return ConfigurationManager.AppSettings["TokenID"].ToString();
                else
                    throw new Exception("Missing TokenID");
            }
        }

        protected static string TokenSecret
        {
            get
            {
                if (ConfigurationManager.AppSettings["TokenSecret"] != null)
                    return ConfigurationManager.AppSettings["TokenSecret"].ToString();
                else
                    throw new Exception("Missing TokenSecret");
            }
        }

        protected void chkInclude_CheckedChanged(object sender, EventArgs e)
        {
            CalculateTotal();
        }

        protected void CalculateTotal()
        {
            decimal decTotal = 0;

            foreach (GridViewRow _GridViewRow in gvInvoice.Rows)
            {
                HiddenField hfAmountRemaining = (HiddenField)_GridViewRow.FindControl("hfAmountRemaining");
                CheckBox chkInclude = (CheckBox)_GridViewRow.FindControl("chkInclude");

                if (chkInclude.Checked)
                {
                    decTotal += Convert.ToDecimal(hfAmountRemaining.Value);
                }
            }
            lblTotal.Text = decTotal.ToString("C");
        }

        protected void btnRequestPayment_Click(object sender, EventArgs e)
        {


            SqlConnection objConn = null;
            SqlTransaction objTran = null;

            if (Convert.ToString(ConfigurationManager.AppSettings["Environment"]) == "production")
            {
                Stripe.StripeConfiguration.ApiKey = Convert.ToString(ConfigurationManager.AppSettings["StripeAPI"]);
            }
            else
            {
                Stripe.StripeConfiguration.ApiKey = Convert.ToString(ConfigurationManager.AppSettings["StagingStripeAPI"]);
            }

            try
            {
                if (lblTotal.Text == 0.ToString("C"))
                {
                    throw new Exception("No invoice selected");
                }

                objConn = new SqlConnection(Database.DefaultConnectionString);
                objConn.Open();
                objTran = objConn.BeginTransaction();

                decimal decTotal = 0;
                int counter = 0;

                ImageSolutions.Invoice.InvoicePayment InvoicePayment = null;

                foreach (GridViewRow _GridViewRow in gvInvoice.Rows)
                {
                    counter++;

                    HiddenField hfCustomerInteralID = (HiddenField)_GridViewRow.FindControl("hfCustomerInteralID");
                    HiddenField hfInvoiceInternalID = (HiddenField)_GridViewRow.FindControl("hfInvoiceInternalID");
                    HiddenField hfTranDate = (HiddenField)_GridViewRow.FindControl("hfTranDate");
                    HiddenField hfInvoiceNumber = (HiddenField)_GridViewRow.FindControl("hfInvoiceNumber");
                    HiddenField hfAmountTotal = (HiddenField)_GridViewRow.FindControl("hfAmountTotal");
                    HiddenField hfAmountRemaining = (HiddenField)_GridViewRow.FindControl("hfAmountRemaining");
                    CheckBox chkInclude = (CheckBox)_GridViewRow.FindControl("chkInclude");

                    if(string.IsNullOrEmpty(hfCustomerInteralID.Value))
                    {
                        throw new Exception("Missing Customer Internal ID");
                    }
                    if (string.IsNullOrEmpty(hfInvoiceInternalID.Value))
                    {
                        throw new Exception("Missing Invoice Internal ID");
                    }

                    if (counter == 1)
                    {
                        InvoicePayment = new ImageSolutions.Invoice.InvoicePayment();
                        InvoicePayment.CustomerInternalID = string.IsNullOrEmpty(mCustomerInternalID) ? Convert.ToString(hfCustomerInteralID.Value) : mCustomerInternalID;

                        InvoicePayment.InvoicePaymentLines = new List<ImageSolutions.Invoice.InvoicePaymentLine>();
                    }

                    if (chkInclude.Checked)
                    {
                        ImageSolutions.Invoice.InvoicePaymentLine InvoicePaymentLine = new ImageSolutions.Invoice.InvoicePaymentLine();
                        InvoicePaymentLine.InvoiceInternalID = Convert.ToString(hfInvoiceInternalID.Value);
                        InvoicePaymentLine.CustomerInternalID = Convert.ToString(hfCustomerInteralID.Value);
                        InvoicePaymentLine.TransactionDate = Convert.ToDateTime(hfTranDate.Value);
                        InvoicePaymentLine.InvoiceNumber = Convert.ToString(hfInvoiceNumber.Value);
                        InvoicePaymentLine.AmountTotal = Convert.ToDecimal(hfAmountTotal.Value);
                        InvoicePaymentLine.AmountRemaining = Convert.ToDecimal(hfAmountRemaining.Value);
                        InvoicePaymentLine.Status = "Pending";
                        InvoicePayment.InvoicePaymentLines.Add(InvoicePaymentLine);

                        decTotal += Convert.ToDecimal(hfAmountRemaining.Value);
                    }
                }
                
                InvoicePayment.Amount = decTotal;
                InvoicePayment.Create(objConn, objTran);

                Stripe.PaymentLink PaymentLink = CreatePaymentLink(String.Format("{0} - {1}", InvoicePayment.InvoicePaymentID, InvoicePayment.InvoicePaymentLines.First().InvoiceNumber), decTotal);

                InvoicePayment.PaymentLinkID = PaymentLink.Id;
                InvoicePayment.PaymentLinkURL = PaymentLink.Url;
                InvoicePayment.Status = "Payment Link Created";
                InvoicePayment.Update(objConn, objTran);

                objTran.Commit();

                if (PaymentLink != null && !string.IsNullOrEmpty(InvoicePayment.PaymentLinkURL))
                {
                    //lnkPaymentLink.NavigateUrl = InvoicePayment.PaymentLinkURL;
                    //lnkPaymentLink.Visible = true;

                    Response.Redirect(InvoicePayment.PaymentLinkURL);
                }
                else
                {
                    throw new Exception("Payment Link Failed.  Please try again");
                }
            }
            catch (Exception ex)
            {
                if (objTran != null && objTran.Connection != null) objTran.Rollback();
                WebUtility.DisplayJavascriptMessage(this, ex.Message);
            }
            finally
            {
                if (objTran != null) objTran.Dispose();
                objTran = null;
                if (objConn != null) objConn.Dispose();
                objConn = null;
            }
        }

        public Stripe.PaymentLink CreatePaymentLink(string invoicepaymentid, decimal reaminingamount)
        {
            try
            {
                Stripe.ProductCreateOptions ProductCreateOptions = new Stripe.ProductCreateOptions();
                ProductCreateOptions.Name = invoicepaymentid;
                Stripe.ProductService ProductService = new Stripe.ProductService();
                Stripe.Product Product = ProductService.Create(ProductCreateOptions);

                Stripe.PriceCreateOptions PriceCreateOptions = new Stripe.PriceCreateOptions();
                PriceCreateOptions.Currency = "usd";
                decimal amount = Decimal.Multiply(Convert.ToDecimal(reaminingamount), Convert.ToDecimal(100.0));
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