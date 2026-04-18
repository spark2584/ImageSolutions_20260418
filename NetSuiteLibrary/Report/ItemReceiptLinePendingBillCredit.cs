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
    public class ItemReceiptLinePendingBillCredit
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
                    objFilter.SavedSearchID = "71";

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
            NetSuiteLibrary.com.netsuite.webservices.ItemReceipt objItemReceipt = null;
            NetSuiteLibrary.com.netsuite.webservices.ItemReceiptItem objItemReceiptItem = null;
            Toolots.Report.ItemReceipt.ItemReceiptLinePendingBillCredit objItemReceiptLinePendingBillCredit = null;
            List<Toolots.Report.ItemReceipt.ItemReceiptLinePendingBillCredit> objItemReceiptLinePendingBillCredits = null;
            int intCount = 0;

            try
            {
                if (!ReportScheduler.IsCompleted)
                {
                    objTransactionSearchAdvanced = new TransactionSearchAdvanced();

                    objItemReceiptLinePendingBillCredits = new List<Toolots.Report.ItemReceipt.ItemReceiptLinePendingBillCredit>();

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

                            objItemReceiptLinePendingBillCredit = new Toolots.Report.ItemReceipt.ItemReceiptLinePendingBillCredit();
                            objItemReceiptLinePendingBillCredit.TransactionDate = irow.basic.tranDate[0].searchValue;
                            objItemReceiptLinePendingBillCredit.DocumentNumber = irow.basic.tranId[0].searchValue;
                            objItemReceiptLinePendingBillCredit.DocumentInternalID = Convert.ToInt64(irow.basic.internalId[0].searchValue.internalId);
                            objItemReceiptLinePendingBillCredit.ItemInternalID = Convert.ToInt64(irow.basic.item[0].searchValue.internalId);
                            objItemReceiptLinePendingBillCredit.Quantity = Convert.ToInt32(irow.basic.quantity[0].searchValue);
                            objItemReceiptLinePendingBillCredit.IsConsignedTransaction = ((SearchColumnBooleanCustomField)irow.basic.customFieldList.ToList().Find(m => m.scriptId == "custcol_is_consigned_transaction")).searchValue;

                            objItemReceipt = (NetSuiteLibrary.com.netsuite.webservices.ItemReceipt)NetSuiteLibrary.NetSuiteHelper.GetRecord(objItemReceiptLinePendingBillCredit.DocumentInternalID.ToString(), RecordType.itemReceipt, Service).record;

                            objItemReceiptItem = objItemReceipt.itemList.item.ToList().Find(m => m.item.internalId == objItemReceiptLinePendingBillCredit.ItemInternalID.ToString());
                            if (objItemReceiptItem == null) throw new Exception("Item Receipt# " + objItemReceiptLinePendingBillCredit.DocumentNumber + " does not contain ItemInternalID: " + objItemReceiptLinePendingBillCredit.ItemInternalID);
                            long? intMissingLabelQuantity = NetSuiteLibrary.NetSuiteHelper.GetLongCustomFieldValue(objItemReceiptItem, "custcol_missing_label_qty");
                            objItemReceiptLinePendingBillCredit.MissingLabelQuantity = intMissingLabelQuantity == null ? 0 : Convert.ToInt32(intMissingLabelQuantity);

                            if (objItemReceiptLinePendingBillCredit.TotalPenaltyQuantity == 0) throw new Exception("Total penaity quantity is 0");

                            objItem = (InventoryItem)NetSuiteLibrary.NetSuiteHelper.GetRecord(irow.basic.item[0].searchValue.internalId, RecordType.inventoryItem, Service).record;
                            objItemReceiptLinePendingBillCredit.IsItemConsigned = Convert.ToBoolean(((BooleanCustomFieldRef)objItem.customFieldList.ToList().Find(m => m.scriptId == "custitem_is_consigned")).value);
                            objItemReceiptLinePendingBillCredit.ConsignedVendorInternalID = Convert.ToInt64(((SelectCustomFieldRef)objItem.customFieldList.ToList().Find(m => m.scriptId == "custitem_consigned_vendor")).value.internalId);

                            objItemReceiptLinePendingBillCredits.Add(objItemReceiptLinePendingBillCredit);
                        }

                        Toolots.Report.ItemReceipt.ItemReceiptLinePendingBillCredit.Create(objItemReceiptLinePendingBillCredits);

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
                ReportScheduler.ErrorMessage = ex.Message;
                ReportScheduler.Update();
                throw ex;
            }
            finally
            {
                objTransactionSearchAdvanced = null;
                objSearchResult = null;
                objItemReceiptLinePendingBillCredit = null;
                objItemReceiptLinePendingBillCredits = null;
            }

            return true;
        }
    }
}
