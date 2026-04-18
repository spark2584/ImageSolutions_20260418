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

namespace NetSuiteLibrary.BinTransfer
{
    public class BinTransfer
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

        //private Toolots.BinTransfer.BinTransfer mToolotsBinTransfer = null;
        //public Toolots.BinTransfer.BinTransfer ToolotsBinTransfer
        //{
        //    get
        //    {
        //        if (mToolotsBinTransfer == null && mNetSuiteBinTransfer != null && !string.IsNullOrEmpty(mNetSuiteBinTransfer.internalId))
        //        {
        //            List<Toolots.BinTransfer.BinTransfer> objBinTransfers = null;
        //            Toolots.BinTransfer.BinTransferFilter objFilter = null;

        //            try
        //            {
        //                objFilter = new Toolots.BinTransfer.BinTransferFilter();
        //                objFilter.NetSuiteInternalID = new Database.Filter.StringSearch.SearchFilter();
        //                objFilter.NetSuiteInternalID.SearchString = mNetSuiteBinTransfer.internalId;
        //                objBinTransfers = Toolots.BinTransfer.BinTransfer.GetBinTransfers(objFilter);
        //                if (objBinTransfers != null && objBinTransfers.Count > 0)
        //                {
        //                    mToolotsBinTransfer = objBinTransfers[0];
        //                }
        //            }
        //            catch (Exception ex)
        //            {
        //                throw ex;
        //            }
        //            finally
        //            {
        //                objBinTransfers = null;
        //            }
        //        }
        //        return mToolotsBinTransfer;
        //    }
        //    private set
        //    {
        //        mToolotsBinTransfer = value;
        //    }
        //}

        private NetSuiteLibrary.com.netsuite.webservices.BinTransfer mNetSuiteBinTransfer = null;
        public NetSuiteLibrary.com.netsuite.webservices.BinTransfer NetSuiteBinTransfer
        {
            get
            {
                return mNetSuiteBinTransfer;
            }
            private set
            {
                mNetSuiteBinTransfer = value;
            }
        }

        //public BinTransfer(Toolots.BinTransfer.BinTransfer ToolotsBinTransfer)
        //{
        //    mToolotsBinTransfer = ToolotsBinTransfer;
        //}

        public BinTransfer(NetSuiteLibrary.com.netsuite.webservices.BinTransfer NetSuiteBinTransfer)
        {
            mNetSuiteBinTransfer = NetSuiteBinTransfer;
        }

        //private NetSuiteLibrary.com.netsuite.webservices.BinTransfer LoadNetSuiteBinTransfer(string NetSuiteInternalID)
        //{
        //    RecordRef objSORef = null;
        //    ReadResponse objReadResult = null;
        //    NetSuiteLibrary.com.netsuite.webservices.BinTransfer objReturn = null;

        //    try
        //    {
        //        objSORef = new RecordRef();
        //        objSORef.type = RecordType.binTransfer;
        //        objSORef.typeSpecified = true;
        //        objSORef.internalId = NetSuiteInternalID;

        //        objReadResult = Service.get(objSORef);

        //        if (objReadResult != null && objReadResult.record != null && (objReadResult.record is NetSuiteLibrary.com.netsuite.webservices.BinTransfer))
        //        {
        //            objReturn = (NetSuiteLibrary.com.netsuite.webservices.BinTransfer)objReadResult.record;
        //        }
        //        else
        //        {
        //            throw new Exception("Can not find BinTransfer with Internal ID : " + NetSuiteInternalID);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //    finally
        //    {
        //        objSORef = null;
        //        objReadResult = null;
        //    }
        //    return objReturn;
        //}

        public static List<BinTransfer> GetBinTransfers(BinTransferFilter Filter)
        {
            return GetBinTransfers(Service, Filter);
        }

        public static BinTransfer GetBinTransfer(NetSuiteService Service, BinTransferFilter Filter)
        {
            List<BinTransfer> objBinTransfers = null;
            BinTransfer objReturn = null;

            try
            {
                objBinTransfers = GetBinTransfers(Service, Filter);
                if (objBinTransfers != null && objBinTransfers.Count >= 1) objReturn = objBinTransfers[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                objBinTransfers = null;
            }
            return objReturn;
        }

        public static List<BinTransfer> GetBinTransfers(NetSuiteService Service, BinTransferFilter Filter)
        {
            List<BinTransfer> objReturn = null;
            SearchResult objSearchResult = null;

            try
            {
                objReturn = new List<BinTransfer>();
                objSearchResult = GetNetSuiteTransactions(Service, Filter);
                if (objSearchResult != null && objSearchResult.totalRecords > 0)
                {
                    do
                    {
                        foreach (object objNetBinTransfer in objSearchResult.recordList)
                        {
                            if (objNetBinTransfer is NetSuiteLibrary.com.netsuite.webservices.BinTransfer)
                            {
                                objReturn.Add(new BinTransfer((NetSuiteLibrary.com.netsuite.webservices.BinTransfer)objNetBinTransfer));
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

        private static SearchResult GetNetSuiteTransactions(NetSuiteService Service, BinTransferFilter Filter)
        {
            SearchResult objSearchResult = null;
            TransactionSearch objTransacSearch = null;
            SearchPreferences objSearchPreferences = null;

            try
            {
                objTransacSearch = new TransactionSearch();
                objTransacSearch.basic = new TransactionSearchBasic();

                if (Filter != null)
                {
                    if (Filter.LastModified != null)
                    {
                        objTransacSearch.basic.lastModifiedDate = Filter.LastModified;
                    }

                    if (Filter.NetSuiteInternalIDs != null)
                    {
                        objTransacSearch.basic.internalId = NetSuiteHelper.LoadMultiSearchField(Filter.NetSuiteInternalIDs);
                    }
                }

                objTransacSearch.basic.type = new SearchEnumMultiSelectField();
                objTransacSearch.basic.type.@operator = SearchEnumMultiSelectFieldOperator.anyOf;
                objTransacSearch.basic.type.operatorSpecified = true;
                objTransacSearch.basic.type.searchValue = new string[] { "_binTransfer" };

                objSearchPreferences = new SearchPreferences();
                objSearchPreferences.bodyFieldsOnly = false;
                objSearchPreferences.pageSize = 10;
                objSearchPreferences.pageSizeSpecified = true;

                Service.searchPreferences = objSearchPreferences;
                objSearchResult = Service.search(objTransacSearch);

                if (objSearchResult.status.isSuccess != true)  throw new Exception("Cannot find BinTransfer - " + objSearchResult.status.statusDetail[0].message);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                objTransacSearch = null;
                objSearchPreferences = null;
            }
            return objSearchResult;
        }

        public static List<DeletedRecord> GetDeletedBinTransferAfter(DateTime Date)
        {
            List<DeletedRecord> objReturn = null;
            GetDeletedResult objDeletedResult = null;
            GetDeletedFilter objFilter = null;
            try
            {
                objReturn = new List<DeletedRecord>();

                objFilter = new GetDeletedFilter();
                objFilter.deletedDate = new SearchDateField();
                objFilter.deletedDate.@operator = SearchDateFieldOperator.after;
                objFilter.deletedDate.operatorSpecified = true;
                objFilter.deletedDate.searchValue = Date;
                objFilter.deletedDate.searchValueSpecified = true;

                objFilter.type = new SearchEnumMultiSelectField();
                objFilter.type.@operator = SearchEnumMultiSelectFieldOperator.anyOf;
                objFilter.type.operatorSpecified = true;
                objFilter.type.searchValue = new string[] { NetSuiteHelper.DeletedRecordType.binTransfer.ToString() };
                objDeletedResult = Service.getDeleted(objFilter, 1);

                if (objDeletedResult != null && objDeletedResult.totalRecords > 0)
                {
                    do
                    {
                        foreach (DeletedRecord objDeleteRecord in objDeletedResult.deletedRecordList)
                        {
                            objReturn.Add(objDeleteRecord);
                        }
                        objDeletedResult = Service.getDeleted(objFilter, objDeletedResult.pageIndex + 1);
                    }
                    while (objDeletedResult.pageSizeSpecified = true && objDeletedResult.totalPages > 0 && objDeletedResult.totalPages >= objDeletedResult.pageIndex);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                objFilter = null;
                objDeletedResult = null;
            }
            return objReturn;
        }
    }
}


