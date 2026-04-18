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

namespace NetSuiteLibrary.FBAInboundShipment
{
    public class FBAInboundShipment : NetSuiteBase
    {
        private ImageSolutions.AmazonMWSLibrary.AmazonMWSSyncInboundShipmentLog mImageSolutionsFBAInboundshipment = null;
        public ImageSolutions.AmazonMWSLibrary.AmazonMWSSyncInboundShipmentLog ImageSolutionsFBAInboundshipment
        {
            get
            {
                if (mImageSolutionsFBAInboundshipment == null && mNetSuiteFBAInboundShipment != null && !string.IsNullOrEmpty(mNetSuiteFBAInboundShipment.internalId))
                {
                    List<ImageSolutions.AmazonMWSLibrary.AmazonMWSSyncInboundShipmentLog> objAmazonMWSSyncInboundShipmentLogs = null;
                    ImageSolutions.AmazonMWSLibrary.AmazonMWSSyncInboundShipmentLogFilter objFilter = null;

                    try
                    {
                        objFilter = new ImageSolutions.AmazonMWSLibrary.AmazonMWSSyncInboundShipmentLogFilter();
                        objFilter.NetSuiteInternalID = new Database.Filter.StringSearch.SearchFilter();
                        objFilter.NetSuiteInternalID.SearchString = mNetSuiteFBAInboundShipment.internalId;
                        objAmazonMWSSyncInboundShipmentLogs = ImageSolutions.AmazonMWSLibrary.AmazonMWSSyncInboundShipmentLog.GetAmazonMWSSyncInboundShipmentLogs("2", objFilter);
                        if (objAmazonMWSSyncInboundShipmentLogs != null && objAmazonMWSSyncInboundShipmentLogs.Count > 0)
                        {
                            mImageSolutionsFBAInboundshipment = objAmazonMWSSyncInboundShipmentLogs[0];
                        }
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    finally
                    {
                        objAmazonMWSSyncInboundShipmentLogs = null;
                    }
                }
                return mImageSolutionsFBAInboundshipment;
            }
            private set
            {
                mImageSolutionsFBAInboundshipment = value;
            }
        }

        private NetSuiteLibrary.com.netsuite.webservices.CustomRecord mNetSuiteFBAInboundShipment = null;
        public NetSuiteLibrary.com.netsuite.webservices.CustomRecord NetSuiteFBAInboundShipment
        {
            get
            {
                if (mNetSuiteFBAInboundShipment == null && mImageSolutionsFBAInboundshipment != null && !string.IsNullOrEmpty(mImageSolutionsFBAInboundshipment.NetSuiteInternalID))
                {
                    mNetSuiteFBAInboundShipment = LoadNetSuiteFBAInboundShipment(mImageSolutionsFBAInboundshipment.NetSuiteInternalID);
                }
                return mNetSuiteFBAInboundShipment;
            }
            private set
            {
                mNetSuiteFBAInboundShipment = value;
            }
        }

        public FBAInboundShipment(ImageSolutions.AmazonMWSLibrary.AmazonMWSSyncInboundShipmentLog ImageSolutionsFBAInboundShipment)
        {
            mImageSolutionsFBAInboundshipment = ImageSolutionsFBAInboundShipment;
        }

        public FBAInboundShipment(NetSuiteLibrary.com.netsuite.webservices.CustomRecord NetSuiteFBAInboundShipment)
        {
            mNetSuiteFBAInboundShipment = NetSuiteFBAInboundShipment;
        }

        private NetSuiteLibrary.com.netsuite.webservices.CustomRecord LoadNetSuiteFBAInboundShipment(string NetSuiteInternalID)
        {
            CustomRecordRef objCustomRecordRef = null;
            ReadResponse objReadResult = null;
            NetSuiteLibrary.com.netsuite.webservices.CustomRecord objReturn = null;

            try
            {

                objCustomRecordRef = new CustomRecordRef();
                objCustomRecordRef.typeId = "448";
                objCustomRecordRef.internalId = NetSuiteInternalID;

                objReadResult = Service.get(objCustomRecordRef);

                if (objReadResult != null && objReadResult.record != null && (objReadResult.record is NetSuiteLibrary.com.netsuite.webservices.CustomRecord))
                {
                    objReturn = (NetSuiteLibrary.com.netsuite.webservices.CustomRecord)objReadResult.record;
                }
                else
                {
                    throw new Exception("Can not find FBAInboundShipment custom record with Internal ID : " + NetSuiteInternalID);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                objCustomRecordRef = null;
                objReadResult = null;
            }
            return objReturn;
        }

        public bool Create()
        {
            WriteResponse objWriteResponse = null;
            FBAInboundShipment objFBAInboundShipment = null;

            try
            {
                if (ImageSolutionsFBAInboundshipment == null) throw new Exception("ImageSolutionsFBAInboundshipment cannot be null");
                if (ImageSolutionsFBAInboundshipment.AmazonMWSSyncInboundShipmentLogItems == null || ImageSolutionsFBAInboundshipment.AmazonMWSSyncInboundShipmentLogItems.Count() == 0) throw new Exception("AmazonMWSSyncInboundShipmentLogItems are missing");
                if (NetSuiteFBAInboundShipment != null) throw new Exception("NetSuiteFBAInboundShipment already exists");

                objFBAInboundShipment = ObjectAlreadyExists();

                if (objFBAInboundShipment != null)
                {
                    //NetSuite InternalID did not get updated, auto fix
                    ImageSolutionsFBAInboundshipment.NetSuiteInternalID = objFBAInboundShipment.NetSuiteFBAInboundShipment.internalId;
                }
                else
                {
                    objWriteResponse = Service.add(CreateNetSuiteFBAInboundShipment());
                    if (objWriteResponse.status.isSuccess != true)
                    {
                        throw new Exception("Unable to create FBAInboundShipment: " + objWriteResponse.status.statusDetail[0].message);
                    }
                    else
                    {
                        ImageSolutionsFBAInboundshipment.NetSuiteInternalID = ((CustomRecordRef)objWriteResponse.baseRef).internalId;
                    }
                }

                ImageSolutionsFBAInboundshipment.ErrorMessage = string.Empty;
                ImageSolutionsFBAInboundshipment.Update();
            }
            catch (Exception ex)
            {
                ImageSolutionsFBAInboundshipment.ErrorMessage = "FBAInboundShipment.cs - Create() - " + ex.Message;
                ImageSolutionsFBAInboundshipment.Update();
            }
            finally
            {
                objWriteResponse = null;
            }
            return true;
        }

        private NetSuiteLibrary.com.netsuite.webservices.CustomRecord CreateNetSuiteFBAInboundShipment()
        {
            NetSuiteLibrary.com.netsuite.webservices.CustomRecord objReturn = null;
            RecordRef objRecordRef = null;
            int intCustomFieldIndex = 0;

            try
            {
                //if (ImageSolutionsTransferOrder.TransferOrder == null) throw new Exception("TransferOrder is missing");
                //if (string.IsNullOrEmpty(ImageSolutionsTransferOrder.TransferOrder.NetSuiteInternalID)) throw new Exception("TransferOrder.NetSuiteInternalID is missing");

                objRecordRef = new RecordRef();
                objRecordRef.internalId = "448";
                objRecordRef.type = RecordType.customRecordType;

                objReturn = new CustomRecord();
                objReturn.recType = objRecordRef;
                objReturn.internalId = ImageSolutionsFBAInboundshipment.NetSuiteInternalID;
                
                objReturn.customFieldList = new CustomFieldRef[99];
                objReturn.customFieldList[intCustomFieldIndex++] = NetSuiteHelper.CreateStringCustomField(ImageSolutionsFBAInboundshipment.ShipmentID, "custrecord_shipment_id");
                objReturn.customFieldList[intCustomFieldIndex++] = NetSuiteHelper.CreateStringCustomField(ImageSolutionsFBAInboundshipment.ShipmentStatus.Value.ToString(), "custrecord_shipment_status");
                objReturn.customFieldList[intCustomFieldIndex++] = NetSuiteHelper.CreateStringCustomField(ImageSolutionsFBAInboundshipment.DestinationFulfillmentCenterID, "custrecord_fulfillment_center_id");
                objReturn.customFieldList[intCustomFieldIndex++] = NetSuiteHelper.CreateStringCustomField(ImageSolutionsFBAInboundshipment.TransferOrder.NetSuiteInternalID, "custrecord_parent_internal_id");
                objReturn.customFieldList[intCustomFieldIndex++] = NetSuiteHelper.CreateStringCustomField(ImageSolutionsFBAInboundshipment.AmazonMWSSyncInboundShipmentLogID, "custrecord_api_external_id");
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally { }
            return objReturn;
        }

        public bool Update()
        {
            WriteResponse objWriteResult = null;

            try
            {
                if (ImageSolutionsFBAInboundshipment == null) throw new Exception("ImageSolutionsFBAInboundshipment cannot be null");
                if (NetSuiteFBAInboundShipment == null) throw new Exception("NetSuiteFBAInboundShipment record does not exists in NetSuite");

                objWriteResult = Service.update(UpdateNetSuiteFBAInboundShipment());

                if (objWriteResult.status.isSuccess != true)
                {
                    throw new Exception("FBAInboundShipment Update() : " + objWriteResult.status.statusDetail[0].message);
                }
                else
                {
                    mNetSuiteFBAInboundShipment = null;
                }
                ImageSolutionsFBAInboundshipment.IsNSUpdated = true;
                ImageSolutionsFBAInboundshipment.ErrorMessage = string.Empty;
                ImageSolutionsFBAInboundshipment.Update();
            }
            catch (Exception ex)
            {
                ImageSolutionsFBAInboundshipment.ErrorMessage = "FBAInboundShipment.cs - Update() - " + ex.Message;
                ImageSolutionsFBAInboundshipment.Update();
            }
            finally
            {
                objWriteResult = null;
            }
            return true;
        }

        private NetSuiteLibrary.com.netsuite.webservices.CustomRecord UpdateNetSuiteFBAInboundShipment()
        {
            NetSuiteLibrary.com.netsuite.webservices.CustomRecord objReturn = null;
            RecordRef objRecordRef = null;
            int intCustomFieldIndex = 0;

            try
            {
                //if (ImageSolutionsTransferOrder.TransferOrder == null) throw new Exception("TransferOrder is missing");
                //if (string.IsNullOrEmpty(ImageSolutionsTransferOrder.TransferOrder.NetSuiteInternalID)) throw new Exception("TransferOrder.NetSuiteInternalID is missing");

                objRecordRef = new RecordRef();
                objRecordRef.internalId = "448";
                objRecordRef.type = RecordType.customRecordType;

                objReturn = new CustomRecord();
                objReturn.recType = objRecordRef;
                objReturn.internalId = ImageSolutionsFBAInboundshipment.NetSuiteInternalID;

                objReturn.customFieldList = new CustomFieldRef[99];
                objReturn.customFieldList[intCustomFieldIndex++] = NetSuiteHelper.CreateStringCustomField(ImageSolutionsFBAInboundshipment.ShipmentStatus.Value.ToString(), "custrecord_shipment_status");

                if (!string.IsNullOrEmpty(ImageSolutionsFBAInboundshipment.UpdateInboundShipmentXML))
                {
                    objReturn.customFieldList[intCustomFieldIndex++] = NetSuiteHelper.CreateBooleanCustomField(true, "custrecord_fba_shipment_updated");
                    objReturn.customFieldList[intCustomFieldIndex++] = NetSuiteHelper.CreateDateCustomField(ImageSolutionsFBAInboundshipment.UpdatedOn.Value.AddHours(-7), "custrecord_fba_shipment_updated_on");
                }

                objReturn.customFieldList[intCustomFieldIndex++] = NetSuiteHelper.CreateStringCustomField(ImageSolutionsFBAInboundshipment.ErrorMessage, "custrecord_fba_shipment_error_message");
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally { }
            return objReturn;
        }

        public bool UpdateNetSuiteFBAShipmentUpdating()
        {
            WriteResponse objWriteResult = null;

            try
            {
                if (ImageSolutionsFBAInboundshipment == null) throw new Exception("ImageSolutionsFBAInboundshipment cannot be null");
                if (mNetSuiteFBAInboundShipment == null) throw new Exception("mNetSuiteFBAInboundShipment record does not exists in NetSuite");

                objWriteResult = Service.update(GetUpdateNetSuiteFBAShipmentUpdating());

                if (objWriteResult.status.isSuccess != true)
                {
                    throw new Exception("NetSuiteFBAInboundShipment Update : transfer order can not be updated " + objWriteResult.status.statusDetail[0].message);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("UpdateNetSuiteFBAShipmentUpdating() - " + ex.Message);
            }
            finally
            {
                objWriteResult = null;
            }
            return true;
        }

        private NetSuiteLibrary.com.netsuite.webservices.CustomRecord GetUpdateNetSuiteFBAShipmentUpdating()
        {
            NetSuiteLibrary.com.netsuite.webservices.CustomRecord objReturn = null;
            RecordRef objRecordRef = null;
            int intCustomFieldIndex = 0;

            try
            {
                objRecordRef = new RecordRef();
                objRecordRef.internalId = "448";
                objRecordRef.type = RecordType.customRecordType;

                objReturn = new CustomRecord();
                objReturn.recType = objRecordRef;
                objReturn.internalId = ImageSolutionsFBAInboundshipment.NetSuiteInternalID;

                objReturn.customFieldList = new CustomFieldRef[99];

                //if (!string.IsNullOrEmpty(ImageSolutionsFBAInboundshipment.UpdateInboundShipmentXML))
                //{
                objReturn.customFieldList[intCustomFieldIndex++] = NetSuiteHelper.CreateBooleanCustomField(true, "custrecord_fba_shipment_updating");
                //}
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally { }
            return objReturn;
        }

        public FBAInboundShipment ObjectAlreadyExists()
        {
            List<FBAInboundShipment> objFBAInboundShipments = null;
            FBAInboundShipmentFilter objFilter = null;
            FBAInboundShipment objReturn = null;

            try
            {
                objFilter = new FBAInboundShipmentFilter();
                objFilter.APIExternalID = new Database.Filter.StringSearch.SearchFilter();
                objFilter.APIExternalID.SearchString = ImageSolutionsFBAInboundshipment.AmazonMWSSyncInboundShipmentLogID;
                objFBAInboundShipments = GetFBAInboundShipments(Service, objFilter);
                if (objFBAInboundShipments != null && objFBAInboundShipments.Count() > 0)
                {
                    if (objFBAInboundShipments.Count > 1) throw new Exception("More than one FBAInboundShipments with API External ID:" + ImageSolutionsFBAInboundshipment.AmazonMWSSyncInboundShipmentLogID + " found in Netsuite with InternalIDs " + string.Join(", ", objFBAInboundShipments.Select(m => m.NetSuiteFBAInboundShipment.internalId)));
                    objReturn = objFBAInboundShipments[0];
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                objFBAInboundShipments = null;
                objFilter = null;
            }
            return objReturn;
        }

        private static FBAInboundShipment GetFBAInboundShipment(NetSuiteService Service, FBAInboundShipmentFilter Filter)
        {
            List<FBAInboundShipment> objFBAInboundShipments = null;
            FBAInboundShipment objReturn = null;

            try
            {
                objFBAInboundShipments = GetFBAInboundShipments(Service, Filter);
                if (objFBAInboundShipments != null && objFBAInboundShipments.Count >= 1) objReturn = objFBAInboundShipments[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                objFBAInboundShipments = null;
            }
            return objReturn;
        }

        public static List<FBAInboundShipment> GetFBAInboundShipments(FBAInboundShipmentFilter Filter)
        {
            return GetFBAInboundShipments(Service, Filter);
        }

        private static List<FBAInboundShipment> GetFBAInboundShipments(NetSuiteService Service, FBAInboundShipmentFilter Filter)
        {
            List<FBAInboundShipment> objReturn = null;
            SearchResult objSearchResult = null;

            try
            {
                objReturn = new List<FBAInboundShipment>();
                objSearchResult = GetNetSuiteTransactions(Service, Filter);
                if (objSearchResult != null && objSearchResult.totalRecords > 0)
                {
                    do
                    {
                        foreach (object objNetFBAInboundShipment in objSearchResult.recordList)
                        {
                            if (objNetFBAInboundShipment is NetSuiteLibrary.com.netsuite.webservices.CustomRecord)
                            {
                                objReturn.Add(new FBAInboundShipment((NetSuiteLibrary.com.netsuite.webservices.CustomRecord)objNetFBAInboundShipment));
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

        private static SearchResult GetNetSuiteTransactions(NetSuiteService Service, FBAInboundShipmentFilter Filter)
        {
            SearchResult objSearchResult = null;
            CustomRecordSearch objCustomRecordSearch = null;
            SearchPreferences objSearchPreferences = null;
            RecordRef objRecordRef = null;
            int intCutstomField = 0;

            try
            {
                objCustomRecordSearch = new CustomRecordSearch();
                objCustomRecordSearch.basic = new CustomRecordSearchBasic();
                objCustomRecordSearch.basic.customFieldList = new SearchCustomField[99];

                if (Filter != null)
                {
                    if (Filter.APIExternalID != null)
                    {
                        SearchStringCustomField objAPIExternalID = new SearchStringCustomField();
                        objAPIExternalID.scriptId = "custrecord_api_external_id";

                        objAPIExternalID.searchValue = Filter.APIExternalID.SearchString;
                        objAPIExternalID.@operator = SearchStringFieldOperator.@is;
                        objAPIExternalID.operatorSpecified = true;

                        switch (Filter.APIExternalID.Operator)
                        {
                            case Database.Filter.StringSearch.SearchOperator.empty:
                                objAPIExternalID.@operator = SearchStringFieldOperator.empty;
                                break;
                            case Database.Filter.StringSearch.SearchOperator.notEmpty:
                                objAPIExternalID.@operator = SearchStringFieldOperator.notEmpty;
                                break;
                        }
                        objCustomRecordSearch.basic.customFieldList[intCutstomField] = objAPIExternalID;
                        intCutstomField++;
                    }

                    if (Filter.RequestFBAShipmentUpdate != null)
                    {
                        SearchBooleanCustomField objRequestFBAShipmentUpdate = new SearchBooleanCustomField();
                        objRequestFBAShipmentUpdate.scriptId = "custrecord_request_fba_shipment_update";
                        objRequestFBAShipmentUpdate.searchValue = Filter.RequestFBAShipmentUpdate.Value;
                        objRequestFBAShipmentUpdate.searchValueSpecified = true;
                        objCustomRecordSearch.basic.customFieldList[intCutstomField] = objRequestFBAShipmentUpdate;
                        intCutstomField++;
                    }
                    if (Filter.FBAShipmentUpdating != null)
                    {
                        SearchBooleanCustomField objFBAShipmentUpdating = new SearchBooleanCustomField();
                        objFBAShipmentUpdating.scriptId = "custrecord_fba_shipment_updating";
                        objFBAShipmentUpdating.searchValue = Filter.FBAShipmentUpdating.Value;
                        objFBAShipmentUpdating.searchValueSpecified = true;
                        objCustomRecordSearch.basic.customFieldList[intCutstomField] = objFBAShipmentUpdating;
                        intCutstomField++;
                    }
                }

                objRecordRef = new RecordRef();
                objRecordRef.internalId = "448";
                objRecordRef.type = RecordType.customRecordType;

                objCustomRecordSearch.basic.recType = objRecordRef;

                objSearchPreferences = new SearchPreferences();
                objSearchPreferences.bodyFieldsOnly = false;
                objSearchPreferences.pageSize = 10;
                objSearchPreferences.pageSizeSpecified = true;

                Service.searchPreferences = objSearchPreferences;
                objSearchResult = Service.search(objCustomRecordSearch);

                if (objSearchResult.status.isSuccess != true) throw new Exception("Cannot find FBAInboundShipment - " + objSearchResult.status.statusDetail[0].message);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                objCustomRecordSearch = null;
                objSearchPreferences = null;
            }
            return objSearchResult;
        }
    }
}


