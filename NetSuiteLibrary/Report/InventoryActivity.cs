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
    public class InventoryActivity
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
                    objFilter.SavedSearchID = "58";

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
            Toolots.Report.Inventory.InventoryActivity objInventoryActivity = null;
            List<Toolots.Report.Inventory.InventoryActivity> objInventoryActivities = null;
            int intCount = 0;

            try
            {
                if (!ReportScheduler.IsCompleted)
                {
                    objTransactionSearchAdvanced = new TransactionSearchAdvanced();

                    objInventoryActivities = new List<Toolots.Report.Inventory.InventoryActivity>();

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

                            objInventoryActivity = new Toolots.Report.Inventory.InventoryActivity();
                            objInventoryActivity.TransactionDate = irow.basic.tranDate[0].searchValue;
                            objInventoryActivity.TransactionType = (Toolots.Report.Inventory.InventoryActivity.enumTranactionType)Enum.Parse(typeof(Toolots.Report.Inventory.InventoryActivity.enumTranactionType), irow.basic.type[0].searchValue);
                            objInventoryActivity.DocumentNumber = irow.basic.tranId[0].searchValue;
                            objInventoryActivity.DocumentInternalID = Convert.ToInt64(irow.basic.internalId[0].searchValue.internalId);
                            objInventoryActivity.ItemInternalID = Convert.ToInt64(irow.itemJoin.internalId[0].searchValue.internalId);
                            objInventoryActivity.Quantity = Convert.ToInt32(irow.basic.quantity[0].searchValue);
                            objInventoryActivity.LocationInternalID = Convert.ToInt64(irow.basic.location[0].searchValue.internalId);

                            objItem = (InventoryItem)NetSuiteLibrary.NetSuiteHelper.GetRecord(irow.itemJoin.internalId[0].searchValue.internalId, RecordType.inventoryItem, Service).record;
                            objInventoryActivity.VendorInternalID = Convert.ToInt64(((SelectCustomFieldRef)objItem.customFieldList.ToList().Find(m => m.scriptId == "custitem_consigned_vendor")).value.internalId);

                            if (irow.createdFromJoin != null)
                            {
                                objInventoryActivity.SourceTransactionDate = irow.createdFromJoin.tranDate[0].searchValue;
                                objInventoryActivity.SourceTransactionType = (Toolots.Report.Inventory.InventoryActivity.enumTranactionType)Enum.Parse(typeof(Toolots.Report.Inventory.InventoryActivity.enumTranactionType), irow.createdFromJoin.type[0].searchValue);
                                objInventoryActivity.SourceDocumentNumber = irow.createdFromJoin.tranId[0].searchValue;
                                objInventoryActivity.SourceDocumentInternalID = Convert.ToInt64(irow.createdFromJoin.internalId[0].searchValue.internalId);
                            }
                            
                            objInventoryActivities.Add(objInventoryActivity);
                        }

                        Toolots.Report.Inventory.InventoryActivity.Create(objInventoryActivities);

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
                objInventoryActivity = null;
                objInventoryActivities = null;
            }

            return true;
        }
    }
}
