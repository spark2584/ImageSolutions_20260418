using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetSuiteLibrary;
using NetSuiteLibrary.com.netsuite.webservices;

namespace ScheduledTask.Task.StripePaymentLink
{
    public class ImportInvoice : NetSuiteBase
    {
        public bool Execute()
        {
            SyncInvoice();

            return true;
        }

        public bool SyncInvoice()
        {
            try
            {
                //customsearch_export_invoice_to_db - https://acct88641.app.netsuite.com/app/common/search/searchresults.nl?searchid=298843&whence=
                List<NetSuiteLibrary.com.netsuite.webservices.Invoice> NSInvoices = GetInvoiceSavedSearch("298843");

                foreach (NetSuiteLibrary.com.netsuite.webservices.Invoice _Invoice in NSInvoices)
                {
                    SqlConnection objConn = null;
                    SqlTransaction objTran = null;

                    try
                    {
                        ImageSolutions.Invoice.Invoice Invoice = new ImageSolutions.Invoice.Invoice();
                        ImageSolutions.Invoice.InvoiceFilter InvoiceFilter = new ImageSolutions.Invoice.InvoiceFilter();
                        InvoiceFilter.InternalID = new Database.Filter.StringSearch.SearchFilter();
                        InvoiceFilter.InternalID.SearchString = _Invoice.internalId;
                        Invoice = ImageSolutions.Invoice.Invoice.GetInvoice(InvoiceFilter);
                        if(Invoice != null && string.IsNullOrEmpty(Invoice.InvoiceID))
                        {
                            //throw new Exception("Invoice already exists");
                            Invoice.TransactionDate = _Invoice.tranDate;
                            Invoice.InvoiceNumber = _Invoice.tranId;
                            Invoice.AmountRemaining = Convert.ToDecimal(_Invoice.amountRemaining);
                            Invoice.Total = Convert.ToDecimal(_Invoice.total);
                            Invoice.Update(objConn, objTran);
                        }
                        else
                        {
                            objConn = new SqlConnection(Database.DefaultConnectionString);
                            objConn.Open();
                            objTran = objConn.BeginTransaction();

                            Invoice = new ImageSolutions.Invoice.Invoice();
                            Invoice.InternalID = _Invoice.internalId;
                            Invoice.TransactionDate = _Invoice.tranDate;
                            Invoice.InvoiceNumber = _Invoice.tranId;
                            Invoice.AmountRemaining = Convert.ToDecimal(_Invoice.amountRemaining);
                            Invoice.Total = Convert.ToDecimal(_Invoice.total);
                            Invoice.Create(objConn, objTran);

                            foreach (NetSuiteLibrary.com.netsuite.webservices.InvoiceItem _InvoiceItem in _Invoice.itemList.item)
                            {
                                ImageSolutions.Invoice.InvoiceLine InvoiceLine = new ImageSolutions.Invoice.InvoiceLine();
                                InvoiceLine.InvoiceID = Invoice.InvoiceID;
                                InvoiceLine.LineID = Convert.ToString(_InvoiceItem.orderLine);
                                InvoiceLine.ItemInternalID = _InvoiceItem.item.internalId;
                                InvoiceLine.Quantity = Convert.ToDecimal(_InvoiceItem.quantity);
                                InvoiceLine.Create(objConn, objTran);
                            }

                            objTran.Commit();
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(String.Format("{0}", ex.Message));

                        if (objTran != null && objTran.Connection != null) objTran.Rollback();
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
                Console.WriteLine(String.Format("{0}", ex.Message));
            }
            return true;
        }

        public List<Invoice> GetInvoiceSavedSearch(string savedsearchid)
        {
            List<Invoice> Invoices = null;
            SearchResult SearchResult = null;
            Dictionary<string, string> dicInternalIds = new Dictionary<string, string>();

            try
            {
                Invoices = new List<Invoice>();
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

                                if (_TransactionSearchRow is TransactionSearchRow && !dicInternalIds.ContainsKey(_TransactionSearchRow.basic.internalId[0].searchValue.internalId))
                                {
                                    string strInternalID = _TransactionSearchRow.basic.internalId[0].searchValue.internalId;
                                    //Customers.Add(LoadNetSuiteCustomer(strInternalID));

                                    dicInternalIds.Add(strInternalID, strInternalID);
                                    Invoice Invoice = LoadNetSuiteInvoice(strInternalID);
                                    Invoice.amountRemaining = _TransactionSearchRow.basic.amountRemaining[0].searchValue;
                                    Invoices.Add(Invoice);

                                }
                            }
                            Service.tokenPassport = new NetSuiteLibrary.User().TokenPassport();
                            SearchResult = Service.searchMoreWithId(SearchResult.searchId, SearchResult.pageIndex + 1);
                        }
                    }
                    while (SearchResult.searchRowList != null && SearchResult.pageSizeSpecified == true && SearchResult.totalPages >= SearchResult.pageIndex);
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
            TransactionSearch TransactionSearch = null;
            SearchPreferences SearchPreferences = null;

            try
            {
                TransactionSearchAdvanced = new TransactionSearchAdvanced();
                TransactionSearchAdvanced.savedSearchId = savedsearchid;

                //SearchMultiSelectField SearchMultiSelectField = new SearchMultiSelectField();
                //SearchMultiSelectField.searchValue = new RecordRef[1];
                //SearchMultiSelectField.searchValue[0] = new RecordRef();
                //SearchMultiSelectField.searchValue[0].internalId = "45011252";
                //SearchMultiSelectField.searchValue[0].type = RecordType.salesOrder;
                //SearchMultiSelectField.searchValue[0].typeSpecified = true;
                //SearchMultiSelectField.@operator = SearchMultiSelectFieldOperator.anyOf;
                //SearchMultiSelectField.operatorSpecified = true;
                //TransactionSearchAdvanced.criteria = new TransactionSearch();
                //TransactionSearchAdvanced.criteria.basic = new TransactionSearchBasic();
                //TransactionSearchAdvanced.criteria.basic.internalId = SearchMultiSelectField;

                //TransactionSearchAdvanced.criteria = new TransactionSearch();
                //TransactionSearchAdvanced.criteria.basic = new TransactionSearchBasic();
                //TransactionSearchAdvanced.criteria.customerJoin = new CustomerSearchBasic();
                //TransactionSearchAdvanced.criteria.customerJoin.internalId = new SearchMultiSelectField();
                //TransactionSearchAdvanced.criteria.customerJoin.internalId.searchValue = new RecordRef[1];
                //TransactionSearchAdvanced.criteria.customerJoin.internalId.searchValue[0].internalId = "3846713";

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
                    objReturn = (NetSuiteLibrary.com.netsuite.webservices.Invoice)objReadResult.record;
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


    }
}
