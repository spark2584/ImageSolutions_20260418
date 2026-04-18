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
    public class LeadsPendingConversion
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
                    objFilter.SavedSearchID = "74";

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
            CustomerSearchAdvanced objCustomerSearchAdvanced = null;
            SearchResult objSearchResult = null;
            Toolots.Vendor.Vendor objVendor = null;
            List<Toolots.Vendor.Vendor> objVendors = null;
            Toolots.Vendor.VendorFilter objFilter = null;
            Customer.Customer objCustomer = null;
            Customer.CustomerFilter objCustomerFilter = null;
            int intCount = 0;

            try
            {
                if (!ReportScheduler.IsCompleted)
                {
                    objCustomerSearchAdvanced = new CustomerSearchAdvanced();

                    objVendors = new List<Toolots.Vendor.Vendor>();

                    do
                    {
                        objCustomerSearchAdvanced.savedSearchId = ReportScheduler.SavedSearchID;

                        ReportScheduler.CurrentPageIndex++;

                        Service.searchPreferences = new SearchPreferences
                        {
                            bodyFieldsOnly = false,
                            returnSearchColumns = true
                        };

                        if (ReportScheduler.TotalPages == 0 || ReportScheduler.TotalPages < ReportScheduler.CurrentPageIndex)
                        {
                            objSearchResult = Service.search(objCustomerSearchAdvanced);
                        }
                        else
                        {
                            objSearchResult = Service.searchMoreWithId(ReportScheduler.SearchID, ReportScheduler.CurrentPageIndex);
                        }

                        if (objSearchResult.status.isSuccess != true) throw new Exception(objSearchResult.status.statusDetail[0].message);

                        ReportScheduler.SearchID = objSearchResult.searchId;

                        foreach (CustomerSearchRow irow in objSearchResult.searchRowList)
                        {
                            intCount++;
                            Console.WriteLine("Processing " + intCount + " of " + objSearchResult.totalRecords + " " + irow.basic.entityId);

                            objFilter = new Toolots.Vendor.VendorFilter();
                            objFilter.NetSuiteLeadInternalID = new Database.Filter.StringSearch.SearchFilter();
                            objFilter.NetSuiteLeadInternalID.SearchString = Convert.ToString(irow.basic.internalId[0].searchValue.internalId);
                            
                            objVendor = Toolots.Vendor.Vendor.GetVendor(objFilter);
                            if (objVendor == null)
                            {
                                objVendor = new Toolots.Vendor.Vendor();
                                objVendor.NetSuiteLeadInternalID = Convert.ToString(irow.basic.internalId[0].searchValue.internalId);

                                objCustomerFilter = new Customer.CustomerFilter();
                                objCustomerFilter.CustomerInternalIDs = new List<string>();
                                objCustomerFilter.CustomerInternalIDs.Add(objVendor.NetSuiteLeadInternalID);
                                objCustomer = Customer.Customer.GetCustomer(Service, objCustomerFilter);
                                if (objCustomer == null) throw new Exception("Unable to find lead with InternalID:" + objVendor.NetSuiteLeadInternalID);

                                objVendor.NetSuiteLeadEntityID = objCustomer.NetSuiteCustomer.entityId;
                                objVendor.CompanyName = irow.basic.companyName[0].searchValue;
                                if (irow.basic.email != null) objVendor.Email = irow.basic.email[0].searchValue;
                                if (irow.basic.phone != null) objVendor.Phone = irow.basic.phone[0].searchValue;

                                objVendor.MerchantID = objVendor.NetSuiteLeadEntityID;

                                long? lgFreeStorageMonths = NetSuiteLibrary.NetSuiteHelper.GetLongSearchColumnFieldValue(irow.basic, "custentity_merchant_free_storage_months");
                                objVendor.FreeStorageMonths = lgFreeStorageMonths == null ? (Int32?)null : Convert.ToInt32(lgFreeStorageMonths.Value);

                                long? lgCommissionPercentage = NetSuiteLibrary.NetSuiteHelper.GetLongSearchColumnFieldValue(irow.basic, "custentity_merchant_commission_percent");
                                objVendor.CommissionPercentage = lgCommissionPercentage == null ? (Int32?)null : Convert.ToInt32(lgCommissionPercentage.Value);

                                DateTime? dtServiceStartDate = NetSuiteLibrary.NetSuiteHelper.GetDateSearchColumnFieldValue(irow.basic, "custentity_merchant_service_start_date");
                                objVendor.ServiceStartDate = dtServiceStartDate == null ? (DateTime?)null : dtServiceStartDate.Value;


                                objVendors.Add(objVendor);
                            }
                        }

                        Toolots.Vendor.Vendor.Create(objVendors);

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
                objCustomerSearchAdvanced = null;
                objSearchResult = null;
                objVendor = null;
                objVendors = null;
                objFilter = null;
            }

            return true;
        }
    }
}
