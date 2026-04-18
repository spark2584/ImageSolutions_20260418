using NetSuiteLibrary.com.netsuite.webservices;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.Remoting.Channels;
using System.Text;
using System.Threading.Tasks;
using NetSuiteLibrary;
using SanMarIntegration.com.sanmar.ws_invoiceservicebind;

namespace ScheduledTask.Task.Enterprise
{
    internal class SyncInvoice : NetSuiteBase
    {
        public bool Execute()
        {
            ImageSolutions.Task.Task Task = null;
            ImageSolutions.Task.TaskFilter TaskFilter = null;

            try
            {
                //Get Task for Enterprise Inventory Sync
                Task = new ImageSolutions.Task.Task();
                TaskFilter = new ImageSolutions.Task.TaskFilter();
                TaskFilter.TaskName = new Database.Filter.StringSearch.SearchFilter();
                TaskFilter.TaskName.SearchString = "Sync Enterprise Invoice";
                Task = ImageSolutions.Task.Task.GetTask(TaskFilter);
                DateTime LastInvoiceCreatedOn = GetEnterpriseEBAInvoices(Convert.ToDateTime(Task.LastExecutedOn));

                Task.LastExecutedOn = LastInvoiceCreatedOn;
                Task.Update();
            }
            catch (Exception ex)
            {
                if (Task != null)
                {
                    Task.ErrorMessage = ex.Message;
                    Task.Update();
                }

                throw ex;
            }
            finally
            {
                Task = null;
                TaskFilter = null;
            }
            return true;
        }

        public DateTime GetEnterpriseEBAInvoices(DateTime CreatedOnAndAfter)
        {
            DateTime dtLastInvoiceCreatedOn = CreatedOnAndAfter;

            ArrayList arylQuery = null;
            TransactionSearchAdvanced objTransactionSearchAdvanced = null;
            SearchResult objSearchResult = null;

            try
            {
                objTransactionSearchAdvanced = new TransactionSearchAdvanced();

                do
                {
                    objTransactionSearchAdvanced.savedSearchId = "390831";
                    //objTransactionSearchAdvanced.criteria = new TransactionSearch();
                    //objTransactionSearchAdvanced.criteria.basic = new TransactionSearchBasic();
                    //objTransactionSearchAdvanced.criteria.basic.dateCreated = new SearchDateField();
                    //objTransactionSearchAdvanced.criteria.basic.dateCreated.searchValue = CreatedOnAndAfter;
                    //objTransactionSearchAdvanced.criteria.basic.dateCreated.searchValueSpecified = true;
                    //objTransactionSearchAdvanced.criteria.basic.dateCreated.@operator = SearchDateFieldOperator.onOrAfter;
                    //objTransactionSearchAdvanced.criteria.basic.dateCreated.operatorSpecified = true;

                    if (objSearchResult == null)
                    {
                        objSearchResult = Service.search(objTransactionSearchAdvanced);
                    }
                    else
                    {
                        objSearchResult = Service.searchMoreWithId(objSearchResult.searchId, objSearchResult.pageIndex + 1);
                    }

                    if (objSearchResult.status.isSuccess != true) throw new Exception(objSearchResult.status.statusDetail[0].message);

                    arylQuery = new ArrayList();

                    Console.WriteLine("Page: " + objSearchResult.pageIndex + " of " + objSearchResult.totalPages);

                    foreach (TransactionSearchRow objRow in objSearchResult.searchRowList)
                    {
                        string strInvoiceInternalID = objRow.basic.internalId[0].searchValue.internalId;
                        string strSalesOrderInternalID = objRow.basic.createdFrom[0].searchValue.internalId;
                        DateTime dtTransactionDate = objRow.basic.tranDate[0].searchValue;
                        DateTime dtCreatedOn = objRow.basic.dateCreated[0].searchValue;
                        double dbTotalAmount = objRow.basic.amount == null ? 0 : objRow.basic.amount[0].searchValue;
                        double dbDiscountAmount = objRow.basic.discountAmount == null ? 0 : objRow.basic.discountAmount[0].searchValue;
                        double dbShippingAmount = objRow.basic.shippingAmount == null ? 0 : objRow.basic.shippingAmount[0].searchValue;
                        double dbTaxAmount = objRow.basic.taxTotal == null ? 0 : objRow.basic.taxTotal[0].searchValue;
                        double dbSubtotal = dbTotalAmount - dbDiscountAmount - dbShippingAmount - dbTaxAmount;
                        double? dbBudgetShippingAmount = NetSuiteLibrary.NetSuiteHelper.GetSearchColumnDoubleCustomFieldValue(objRow.basic, "custbody_budget_shipping_amount");
                        double? dbBudgetTaxAmount = NetSuiteLibrary.NetSuiteHelper.GetSearchColumnDoubleCustomFieldValue(objRow.basic, "custbody_budget_tax_amount");

                        //Check DB Invoice Table to see if Invoice Internal ID exists
                        //If not, insert, if yes, bypass

                        ImageSolutions.Enterprise.EnterpriseInvoice EnterpriseInvoice = new ImageSolutions.Enterprise.EnterpriseInvoice();
                        ImageSolutions.Enterprise.EnterpriseInvoiceFilter EnterpriseInvoiceFilter = new ImageSolutions.Enterprise.EnterpriseInvoiceFilter();
                        EnterpriseInvoiceFilter.InvoiceInternalID = new Database.Filter.StringSearch.SearchFilter();
                        EnterpriseInvoiceFilter.InvoiceInternalID.SearchString = strInvoiceInternalID;
                        EnterpriseInvoice = ImageSolutions.Enterprise.EnterpriseInvoice.GetEnterpriseInvoice(EnterpriseInvoiceFilter);

                        if (EnterpriseInvoice == null)
                        {
                            EnterpriseInvoice = new ImageSolutions.Enterprise.EnterpriseInvoice();

                            EnterpriseInvoice.InvoiceInternalID = strInvoiceInternalID;
                            EnterpriseInvoice.SalesOrderInternalID = strSalesOrderInternalID;
                            EnterpriseInvoice.TransactionDate = dtTransactionDate;
                            EnterpriseInvoice.InvoiceCreatedOn = dtCreatedOn;
                            EnterpriseInvoice.TotalAmount = Convert.ToDecimal(dbTotalAmount);
                            EnterpriseInvoice.DiscountAmount = Convert.ToDecimal(dbDiscountAmount);
                            EnterpriseInvoice.ShippingAmount = Convert.ToDecimal(dbShippingAmount);
                            EnterpriseInvoice.TaxAmount = Convert.ToDecimal(dbTaxAmount);
                            EnterpriseInvoice.SubTotal = Convert.ToDecimal(dbSubtotal);
                            EnterpriseInvoice.BudgetShippingAmount = Convert.ToDecimal(dbBudgetShippingAmount);
                            EnterpriseInvoice.BudgetTaxAmount = Convert.ToDecimal(dbBudgetTaxAmount);
                            EnterpriseInvoice.Create();
                        }

                        if (dtCreatedOn > dtLastInvoiceCreatedOn)
                        {
                            dtLastInvoiceCreatedOn = dtCreatedOn;
                        }
                    }
                }
                while (objSearchResult.pageSizeSpecified && objSearchResult.totalPages > objSearchResult.pageIndex);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                objTransactionSearchAdvanced = null;
                objSearchResult = null;
                arylQuery = null;
            }

            return dtLastInvoiceCreatedOn;
        }

        //public static bool GetInvoice()
        //{
        //    NetSuiteLibrary.Invoice.Invoice objNSInvoice = null;
        //    NetSuiteLibrary.Invoice.InvoiceFilter objFilter = null;

        //    objFilter = new NetSuiteLibrary.Invoice.InvoiceFilter();
        //    objFilter.SalesOrderInternalID = "50115891";
        //    objFilter.SalesOrderInternalIDOperator = SearchMultiSelectFieldOperator.anyOf;
        //    objNSInvoice = NetSuiteLibrary.Invoice.Invoice.GetInvoice(Service, objFilter);

        //    if (objNSInvoice != null)
        //    {
        //        double dbSubtotal = objNSInvoice.NetSuiteInvoice.subTotal;
        //        double dbDiscountAmount = objNSInvoice.NetSuiteInvoice.discountAmount;
        //        double dbShippingAmount = objNSInvoice.NetSuiteInvoice.shippingCost;
        //        double dbTotalAmount = objNSInvoice.NetSuiteInvoice.total;
        //        double? dbBudgetShippingAmount = NetSuiteLibrary.NetSuiteHelper.GetDoubleCustomFieldValue(objNSInvoice.NetSuiteInvoice, "custbody_budget_shipping_amount");
        //        double? dbBudgetTaxAmount = NetSuiteLibrary.NetSuiteHelper.GetDoubleCustomFieldValue(objNSInvoice.NetSuiteInvoice, "custbody_budget_tax_amount");
        //    }
        //    return true;
        //}
    }
}
