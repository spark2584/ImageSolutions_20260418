using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetSuiteLibrary.com.netsuite.webservices;
using System.Net;
using System.Collections;

namespace NetSuiteLibrary.Report
{
    public class FulfillmentLinePendingVendorBill
    {
        private static NetSuiteService Service
        {
            get
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                NetSuiteService mNetSuiteService = new NetSuiteService();
                NetSuiteLibrary.User objUser = new NetSuiteLibrary.User();
                mNetSuiteService.tokenPassport = objUser.TokenPassport();
                mNetSuiteService.Url = new Uri(mNetSuiteService.getDataCenterUrls(objUser.Passport().account).dataCenterUrls.webservicesDomain + new Uri(mNetSuiteService.Url).PathAndQuery).ToString();

                return mNetSuiteService;
            }
        }

        private Toolots.Report.ReportScheduler mReportScheduler = null;
        public Toolots.Report.ReportScheduler ReportScheduler
        {
            get
            {
                Toolots.Report.ReportSchedulerFilter objFilter = null;

                try
                {
                    objFilter = new Toolots.Report.ReportSchedulerFilter();
                    objFilter.SavedSearchID = "69";

                    if (mReportScheduler == null)
                    {
                        mReportScheduler = Toolots.Report.ReportScheduler.GetReportScheduler(objFilter);
                        if (mReportScheduler == null) throw new Exception("Unable to find SavedSearchID:" + objFilter.SavedSearchID);
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    objFilter = null;
                }
                return mReportScheduler;
            }
        }

        public bool Execute()
        {
            TransactionSearchAdvanced objTransactionSearchAdvanced = null;
            SearchResult objSearchResult = null;
            InventoryItem objItem = null;
            Toolots.Report.ItemFulfllment.FulfillmentLinePendingVendorBill objFulfillmentLinePendingVendorBill = null;
            List<Toolots.Report.ItemFulfllment.FulfillmentLinePendingVendorBill> objFulfillmentLinePendingVendorBills = null;
            int intCount = 0;

            try
            {
                if (!ReportScheduler.IsCompleted)
                {
                    objTransactionSearchAdvanced = new TransactionSearchAdvanced();

                    objFulfillmentLinePendingVendorBills = new List<Toolots.Report.ItemFulfllment.FulfillmentLinePendingVendorBill>();

                    do
                    {
                        objTransactionSearchAdvanced.savedSearchId = ReportScheduler.SavedSearchID;

                        ReportScheduler.CurrentPageIndex++;

                        Service.searchPreferences = new SearchPreferences
                        {
                            bodyFieldsOnly = false,
                            returnSearchColumns = true
                        };

                        if (ReportScheduler.TotalPages == 0 || ReportScheduler.TotalPages < ReportScheduler.CurrentPageIndex)
                        {
                            objSearchResult = Service.search(objTransactionSearchAdvanced);
                        }
                        else
                        {
                            objSearchResult = Service.searchMoreWithId(ReportScheduler.SearchID, ReportScheduler.CurrentPageIndex);
                        }

                        if (objSearchResult.status.isSuccess != true) throw new Exception(objSearchResult.status.statusDetail[0].message);

                        ReportScheduler.SearchID = objSearchResult.searchId;

                        foreach (TransactionSearchRow irow in objSearchResult.searchRowList)
                        {
                            intCount++;
                            Console.WriteLine("Processing " + intCount + " of " + objSearchResult.totalRecords + " " + irow.basic.item[0].searchValue);

                            objFulfillmentLinePendingVendorBill = new Toolots.Report.ItemFulfllment.FulfillmentLinePendingVendorBill();
                            objFulfillmentLinePendingVendorBill.TransactionDate = irow.basic.tranDate[0].searchValue;
                            objFulfillmentLinePendingVendorBill.DocumentNumber = irow.basic.tranId[0].searchValue;
                            objFulfillmentLinePendingVendorBill.DocumentInternalID = Convert.ToString(irow.basic.internalId[0].searchValue.internalId);
                            objFulfillmentLinePendingVendorBill.ItemInternalID = Convert.ToString(irow.basic.item[0].searchValue.internalId);
                            objFulfillmentLinePendingVendorBill.Quantity = Convert.ToInt32(irow.basic.quantity[0].searchValue);
                            objFulfillmentLinePendingVendorBill.LocationInternalID = Convert.ToString(irow.basic.location[0].searchValue.internalId);

                            objItem = (InventoryItem)NetSuiteLibrary.NetSuiteHelper.GetRecord(irow.basic.item[0].searchValue.internalId, RecordType.inventoryItem, Service).record;

                            string strItemType = ((SelectCustomFieldRef)objItem.customFieldList.ToList().Find(m => m.scriptId == "custitem_item_type")).value.internalId;
                            objFulfillmentLinePendingVendorBill.ItemType = (Toolots.Report.ItemFulfllment.FulfillmentLinePendingVendorBill.enumItemType)Enum.ToObject(typeof(Toolots.Report.ItemFulfllment.FulfillmentLinePendingVendorBill.enumItemType), Convert.ToInt32(strItemType));

                            objFulfillmentLinePendingVendorBill.VendorInternalID = Convert.ToString(((SelectCustomFieldRef)objItem.customFieldList.ToList().Find(m => m.scriptId == "custitem_vendor")).value.internalId);
                            objFulfillmentLinePendingVendorBill.SourceDocumentNumber = irow.appliedToTransactionJoin.tranId[0].searchValue;
                            objFulfillmentLinePendingVendorBill.SourceDocumentInternalID = irow.appliedToTransactionJoin.internalId[0].searchValue.internalId;
                            objFulfillmentLinePendingVendorBill.SourceDocumentIs3PL = ((SearchColumnBooleanCustomField)irow.appliedToTransactionJoin.customFieldList.ToList().Find(m => m.scriptId == "custbody_is_3pl")).searchValue;
                            objFulfillmentLinePendingVendorBill.SourceUnitPrice = Convert.ToDecimal(irow.appliedToTransactionJoin.rate[0].searchValue);
                            objFulfillmentLinePendingVendorBill.SourceLineID = irow.appliedToTransactionJoin.line[0].searchValue.ToString();

                            objFulfillmentLinePendingVendorBills.Add(objFulfillmentLinePendingVendorBill);
                        }

                        Toolots.Report.ItemFulfllment.FulfillmentLinePendingVendorBill.Create(objFulfillmentLinePendingVendorBills);

                        ReportScheduler.ErrorMessage = string.Empty;
                        ReportScheduler.TotalRecords = objSearchResult.totalRecords;
                        ReportScheduler.TotalPages = objSearchResult.totalPages;
                        ReportScheduler.CurrentPageIndex = objSearchResult.pageIndex;
                        ReportScheduler.IsCompleted = ReportScheduler.TotalRecords == 0 || ReportScheduler.TotalPages == ReportScheduler.CurrentPageIndex;
                        if (ReportScheduler.IsCompleted) ReportScheduler.LastCompletedOn = DateTime.UtcNow;
                        ReportScheduler.Update();
                    }
                    while (objSearchResult.pageSizeSpecified && objSearchResult.totalPages > objSearchResult.pageIndex);
                }
            }
            catch (Exception ex)
            {
                ReportScheduler.ErrorMessage = ex.Message + " record " + intCount + " of " + ReportScheduler.CurrentPageIndex + " page index";
                ReportScheduler.Update();
                throw ex;
            }
            finally
            {
                objTransactionSearchAdvanced = null;
                objSearchResult = null;
                objFulfillmentLinePendingVendorBill = null;
                objFulfillmentLinePendingVendorBills = null;
            }

            return true;
        }
    }
}
