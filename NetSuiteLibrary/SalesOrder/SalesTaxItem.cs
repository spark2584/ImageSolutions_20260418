using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using NetSuiteLibrary.com.netsuite.webservices;
using System.Net;
using System.Collections;
using System.ComponentModel;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Configuration;
using System.Data.SqlClient;
using System.Web.Script.Serialization;

namespace NetSuiteLibrary.SalesOrder
{
    public class SalesTaxItem : NetSuiteBase
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
        private static string ToolotsSubsidiaryID
        {
            get
            {
                if (ConfigurationManager.AppSettings["ToolotsSubsidiaryID"] != null)
                    return ConfigurationManager.AppSettings["ToolotsSubsidiaryID"].ToString();
                else
                    return string.Empty;
            }
        }

        private string mTaxRate { get; set; }

        private NetSuiteLibrary.com.netsuite.webservices.SalesTaxItem mNetSuiteSalesTaxItem = null;
        public NetSuiteLibrary.com.netsuite.webservices.SalesTaxItem NetSuiteSalesTaxItem
        {
            get
            {
                if (mNetSuiteSalesTaxItem == null && !string.IsNullOrEmpty(mTaxRate))
                {
                    mNetSuiteSalesTaxItem = LoadNetSuiteSalesTaxItem(mTaxRate);
                }
                return mNetSuiteSalesTaxItem;
            }
            set
            {
                mNetSuiteSalesTaxItem = value;
            }
        }

        public SalesTaxItem(NetSuiteLibrary.com.netsuite.webservices.SalesTaxItem NetSuiteSalesTaxItem)
        {
            mNetSuiteSalesTaxItem = NetSuiteSalesTaxItem;
        }

        public SalesTaxItem(string TaxRate)
        {
            mTaxRate = TaxRate;
        }

        private NetSuiteLibrary.com.netsuite.webservices.SalesTaxItem LoadNetSuiteSalesTaxItem(string TaxRate)
        {
            RecordRef objSalesTaxItemRef = null;
            ReadResponse objReadResult = null;
            NetSuiteLibrary.com.netsuite.webservices.SalesTaxItem objReturn = null;

            try
            {
                objSalesTaxItemRef = new RecordRef();
                objSalesTaxItemRef.name = TaxRate;
                objSalesTaxItemRef.type = RecordType.salesTaxItem;
                objSalesTaxItemRef.typeSpecified = true;

                objReadResult = Service.get(objSalesTaxItemRef);

                if (objReadResult != null && objReadResult.record != null && (objReadResult.record is com.netsuite.webservices.SalesTaxItem))
                {
                    objReturn = (com.netsuite.webservices.SalesTaxItem)objReadResult.record;
                }
                else
                {
                    throw new Exception("Can not find Vendor with TaxRate : " + TaxRate);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                objSalesTaxItemRef = null;
                objReadResult = null;
            }
            return objReturn;
        }

        public bool Create()
        {
            WriteResponse objWriteResponse = null;
            SalesTaxItem objSalesTaxItem = null;

            try
            {
                if (string.IsNullOrEmpty(mTaxRate)) throw new Exception("TaxRate cannot be empty");

                objSalesTaxItem = ObjectAlreadyExists();
                if (objSalesTaxItem != null)
                {
                    //NetSuite InternalID did not get updated, auto fix
                    //ToolotsSalesOrder.NetSuiteInternalID = objSalesOrder.NetSuiteSalesOrder.internalId;
                    //ToolotsSalesOrder.NetSuiteDocumentNumber = objSalesOrder.NetSuiteSalesOrder.tranId;
                }
                else
                {
                    objWriteResponse = Service.add(CreateNetSuiteSalesTaxItem());
                    if (objWriteResponse.status.isSuccess != true)
                    {
                        throw new Exception("Unable to create tax rate: " + objWriteResponse.status.statusDetail[0].message);
                    }
                    else
                    {
                        //ToolotsSalesOrder.NetSuiteInternalID = ((RecordRef)objWriteResponse.baseRef).internalId;
                        //ToolotsSalesOrder.NetSuiteDocumentNumber = NetSuiteSalesOrder.tranId;
                    }
                }
                Console.WriteLine("Tax Rate " + mTaxRate + " sync complete");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Tax Rate " + ex.Message);
            }
            finally
            {
                objWriteResponse = null;
            }
            return true;
        }

        public SalesTaxItem ObjectAlreadyExists()
        {
            List<SalesTaxItem> objSalesTaxItems = null;
            SalesTaxItemFilter objFilter = null;
            SalesTaxItem objReturn = null;

            try
            {
                objFilter = new SalesTaxItemFilter();
                objFilter.TaxRate = mTaxRate;

                objSalesTaxItems = GetSalesTaxItems(Service, objFilter);
                if (objSalesTaxItems != null && objSalesTaxItems.Count() > 0)
                {
                    if (objSalesTaxItems.Count > 1) throw new Exception("More than one tax rate:" + mTaxRate + " found in Netsuite");
                    objReturn = objSalesTaxItems[0];
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                objSalesTaxItems = null;
                objFilter = null;
            }
            return objReturn;
        }

        private NetSuiteLibrary.com.netsuite.webservices.SalesTaxItem CreateNetSuiteSalesTaxItem()
        {
            com.netsuite.webservices.SalesTaxItem objReturn = null;

            try
            {
                objReturn = new com.netsuite.webservices.SalesTaxItem();
                objReturn.itemId = mTaxRate;
                objReturn.rate = mTaxRate;

                objReturn.subsidiaryList = new RecordRef[1];
                objReturn.subsidiaryList[0] = NetSuiteHelper.GetRecordRef(ToolotsSubsidiaryID, RecordType.subsidiary);

                objReturn.taxAgency = NetSuiteHelper.GetRecordRef("2", RecordType.vendor); //Tax Agency CA
                objReturn.taxAccount = NetSuiteHelper.GetRecordRef("109", RecordType.taxAcct); //Sales Taxes Payable CA
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally { }
            return objReturn;
        }

        public static SalesTaxItem GetSalesTaxItem(NetSuiteService Service, SalesTaxItemFilter Filter)
        {
            List<SalesTaxItem> objSalesTaxItems = null;
            SalesTaxItem objReturn = null;

            try
            {
                objSalesTaxItems = GetSalesTaxItems(Service, Filter);
                if (objSalesTaxItems != null && objSalesTaxItems.Count >= 1) objReturn = objSalesTaxItems[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                objSalesTaxItems = null;
            }
            return objReturn;
        }

        public static List<SalesTaxItem> GetSalesTaxItems(SalesTaxItemFilter Filter)
        {
            return GetSalesTaxItems(Service, Filter);
        }

        public static List<SalesTaxItem> GetSalesTaxItems(NetSuiteService Service, SalesTaxItemFilter Filter)
        {
            List<SalesTaxItem> objReturn = null;
            SearchResult objSearchResult = null;

            try
            {
                objReturn = new List<SalesTaxItem>();
                objSearchResult = GetNetSuiteSalesTaxItems(Service, Filter);
                if (objSearchResult != null && objSearchResult.totalRecords > 0)
                {
                    do
                    {
                        foreach (object objSalesTaxItem in objSearchResult.recordList)
                        {
                            if (objSalesTaxItem is NetSuiteLibrary.com.netsuite.webservices.SalesTaxItem)
                            {
                                objReturn.Add(new SalesTaxItem((NetSuiteLibrary.com.netsuite.webservices.SalesTaxItem)objSalesTaxItem));
                            }
                        }
                        Service.tokenPassport = new NetSuiteLibrary.User().TokenPassport();
                        objSearchResult = Service.searchMoreWithId(objSearchResult.searchId, objSearchResult.pageIndex + 1);
                    }
                    while (objSearchResult.pageSizeSpecified = true && objSearchResult.totalPages >= objSearchResult.pageIndex);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                objSearchResult = null;
            }
            return objReturn;
        }

        private static SearchResult GetNetSuiteSalesTaxItems(NetSuiteService Service, SalesTaxItemFilter Filter)
        {
            SearchResult objSearchResult = null;
            SalesTaxItemSearch objSalesTaxItemSearch = null;
            SearchPreferences objSearchPreferences = null;

            try
            {
                objSalesTaxItemSearch = new com.netsuite.webservices.SalesTaxItemSearch();
                objSalesTaxItemSearch.basic = new com.netsuite.webservices.SalesTaxItemSearchBasic();

                if (Filter != null)
                {
                    if (Filter.salesTaxItemInternalIDs != null)
                    {
                        objSalesTaxItemSearch.basic.internalId = NetSuiteHelper.LoadMultiSearchField(Filter.salesTaxItemInternalIDs);
                    }

                    if (!string.IsNullOrEmpty(Filter.TaxRate))
                    {
                        objSalesTaxItemSearch.basic.itemId = new SearchStringField();
                        objSalesTaxItemSearch.basic.itemId.@operator = SearchStringFieldOperator.@is;
                        objSalesTaxItemSearch.basic.itemId.operatorSpecified = true;
                        objSalesTaxItemSearch.basic.itemId.searchValue = Filter.TaxRate;
                    }
                }

                objSearchPreferences = new SearchPreferences();
                objSearchPreferences.bodyFieldsOnly = false;
                objSearchPreferences.pageSize = 10;
                objSearchPreferences.pageSizeSpecified = true;

                Service.searchPreferences = objSearchPreferences;
                objSearchResult = Service.search(objSalesTaxItemSearch);

                if (objSearchResult.status.isSuccess != true) throw new Exception("Cannot find SalesTaxItem - " + objSearchResult.status.statusDetail[0].message);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                objSalesTaxItemSearch = null;
                objSearchPreferences = null;
            }
            return objSearchResult;
        }
    }
}


