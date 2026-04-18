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
    public class FBAInboundShipmentItem : NetSuiteBase
    {
        private ImageSolutions.AmazonMWSLibrary.AmazonMWSSyncInboundShipmentLogItem mImageSolutionsFBAInboundshipmentItem = null;
        public ImageSolutions.AmazonMWSLibrary.AmazonMWSSyncInboundShipmentLogItem ImageSolutionsFBAInboundshipmentItem
        {
            get
            {
                if (mImageSolutionsFBAInboundshipmentItem == null && mNetSuiteFBAInboundShipmentItem != null && !string.IsNullOrEmpty(mNetSuiteFBAInboundShipmentItem.internalId))
                {
                    List<ImageSolutions.AmazonMWSLibrary.AmazonMWSSyncInboundShipmentLogItem> objAmazonMWSSyncInboundShipmentLogItems = null;
                    ImageSolutions.AmazonMWSLibrary.AmazonMWSSyncInboundShipmentLogItemFilter objFilter = null;

                    try
                    {
                        objFilter = new ImageSolutions.AmazonMWSLibrary.AmazonMWSSyncInboundShipmentLogItemFilter();
                        objFilter.NetSuiteInternalID = new Database.Filter.StringSearch.SearchFilter();
                        objFilter.NetSuiteInternalID.SearchString = mNetSuiteFBAInboundShipmentItem.internalId;
                        objAmazonMWSSyncInboundShipmentLogItems = ImageSolutions.AmazonMWSLibrary.AmazonMWSSyncInboundShipmentLogItem.GetAmazonMWSSyncInboundShipmentLogItems("2", objFilter);
                        if (objAmazonMWSSyncInboundShipmentLogItems != null && objAmazonMWSSyncInboundShipmentLogItems.Count > 0)
                        {
                            mImageSolutionsFBAInboundshipmentItem = objAmazonMWSSyncInboundShipmentLogItems[0];
                        }
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    finally
                    {
                        objAmazonMWSSyncInboundShipmentLogItems = null;
                    }
                }
                return mImageSolutionsFBAInboundshipmentItem;
            }
            private set
            {
                mImageSolutionsFBAInboundshipmentItem = value;
            }
        }

        private NetSuiteLibrary.com.netsuite.webservices.CustomRecord mNetSuiteFBAInboundShipmentItem = null;
        public NetSuiteLibrary.com.netsuite.webservices.CustomRecord NetSuiteFBAInboundShipmentItem
        {
            get
            {
                if (mNetSuiteFBAInboundShipmentItem == null && mImageSolutionsFBAInboundshipmentItem != null && !string.IsNullOrEmpty(mImageSolutionsFBAInboundshipmentItem.NetSuiteInternalID))
                {
                    mNetSuiteFBAInboundShipmentItem = LoadNetSuiteFBAInboundShipmentItem(mImageSolutionsFBAInboundshipmentItem.NetSuiteInternalID);
                }
                return mNetSuiteFBAInboundShipmentItem;
            }
            private set
            {
                mNetSuiteFBAInboundShipmentItem = value;
            }
        }

        public FBAInboundShipmentItem(ImageSolutions.AmazonMWSLibrary.AmazonMWSSyncInboundShipmentLogItem ImageSolutionsFBAInboundshipmentItem)
        {
            mImageSolutionsFBAInboundshipmentItem = ImageSolutionsFBAInboundshipmentItem;
        }

        public FBAInboundShipmentItem(NetSuiteLibrary.com.netsuite.webservices.CustomRecord NetSuiteFBAInboundShipmentItem)
        {
            mNetSuiteFBAInboundShipmentItem = NetSuiteFBAInboundShipmentItem;
        }

        private NetSuiteLibrary.com.netsuite.webservices.CustomRecord LoadNetSuiteFBAInboundShipmentItem(string NetSuiteInternalID)
        {
            CustomRecordRef objCustomRecordRef = null;
            ReadResponse objReadResult = null;
            NetSuiteLibrary.com.netsuite.webservices.CustomRecord objReturn = null;

            try
            {

                objCustomRecordRef = new CustomRecordRef();
                objCustomRecordRef.typeId = "450";
                objCustomRecordRef.internalId = NetSuiteInternalID;

                objReadResult = Service.get(objCustomRecordRef);

                if (objReadResult != null && objReadResult.record != null && (objReadResult.record is NetSuiteLibrary.com.netsuite.webservices.CustomRecord))
                {
                    objReturn = (NetSuiteLibrary.com.netsuite.webservices.CustomRecord)objReadResult.record;
                }
                else
                {
                    throw new Exception("Can not find FBAInboundShipmentItem custom record with Internal ID : " + NetSuiteInternalID);
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
            FBAInboundShipmentItem objFBAInboundShipmentItem = null;

            try
            {
                if (ImageSolutionsFBAInboundshipmentItem == null) throw new Exception("ImageSolutionsFBAInboundshipmentItem cannot be null");
                if (ImageSolutionsFBAInboundshipmentItem.AmazonMWSSyncInboundShipmentLog == null || string.IsNullOrEmpty(ImageSolutionsFBAInboundshipmentItem.AmazonMWSSyncInboundShipmentLog.NetSuiteInternalID)) throw new Exception("AmazonMWSSyncInboundShipmentLog is not yet created");
                if (NetSuiteFBAInboundShipmentItem != null) throw new Exception("NetSuiteFBAInboundShipmentItem already exists");

                objFBAInboundShipmentItem = ObjectAlreadyExists();

                if (objFBAInboundShipmentItem != null)
                {
                    //NetSuite InternalID did not get updated, auto fix
                    ImageSolutionsFBAInboundshipmentItem.NetSuiteInternalID = objFBAInboundShipmentItem.NetSuiteFBAInboundShipmentItem.internalId;
                }
                else
                {
                    objWriteResponse = Service.add(CreateNetSuiteFBAInboundShipmentItem());
                    if (objWriteResponse.status.isSuccess != true)
                    {
                        throw new Exception("Unable to create FBAInboundShipmentItem: " + objWriteResponse.status.statusDetail[0].message);
                    }
                    else
                    {
                        ImageSolutionsFBAInboundshipmentItem.NetSuiteInternalID = ((CustomRecordRef)objWriteResponse.baseRef).internalId;
                    }
                }

                ImageSolutionsFBAInboundshipmentItem.IsNSUpdated = true;
                ImageSolutionsFBAInboundshipmentItem.ErrorMessage = string.Empty;
                ImageSolutionsFBAInboundshipmentItem.Update();
            }
            catch (Exception ex)
            {
                ImageSolutionsFBAInboundshipmentItem.ErrorMessage = "FBAInboundShipmentItem.cs - Create() - " + ex.Message;
                ImageSolutionsFBAInboundshipmentItem.Update();
            }
            finally
            {
                objWriteResponse = null;
            }
            return true;
        }

        private NetSuiteLibrary.com.netsuite.webservices.CustomRecord CreateNetSuiteFBAInboundShipmentItem()
        {
            NetSuiteLibrary.com.netsuite.webservices.CustomRecord objReturn = null;
            RecordRef objRecordRef = null;
            int intCustomFieldIndex = 0;

            try
            {
                //if (ImageSolutionsTransferOrder.TransferOrder == null) throw new Exception("TransferOrder is missing");
                //if (string.IsNullOrEmpty(ImageSolutionsTransferOrder.TransferOrder.NetSuiteInternalID)) throw new Exception("TransferOrder.NetSuiteInternalID is missing");

                objRecordRef = new RecordRef();
                objRecordRef.internalId = "450";
                objRecordRef.type = RecordType.customRecordType;

                objReturn = new CustomRecord();
                objReturn.recType = objRecordRef;
                objReturn.internalId = ImageSolutionsFBAInboundshipmentItem.NetSuiteInternalID;
                
                objReturn.customFieldList = new CustomFieldRef[99];
                objReturn.customFieldList[intCustomFieldIndex++] = NetSuiteHelper.CreateStringCustomField(ImageSolutionsFBAInboundshipmentItem.TransferOrderLine.Item.NetSuiteInternalID, "custrecord_fba_shipment_item");
                objReturn.customFieldList[intCustomFieldIndex++] = NetSuiteHelper.CreateLongCustomField(ImageSolutionsFBAInboundshipmentItem.QuantityShipped, "custrecord_fba_line_qty");
                objReturn.customFieldList[intCustomFieldIndex++] = NetSuiteHelper.CreateLongCustomField(ImageSolutionsFBAInboundshipmentItem.QuantityReceived, "custrecord_fba_inbound_qty_received");
                objReturn.customFieldList[intCustomFieldIndex++] = NetSuiteHelper.CreateStringCustomField(ImageSolutionsFBAInboundshipmentItem.AmazonMWSSyncInboundShipmentLog.NetSuiteInternalID, "custrecord_fba_inbound_shipment_number");
                objReturn.customFieldList[intCustomFieldIndex++] = NetSuiteHelper.CreateStringCustomField(ImageSolutionsFBAInboundshipmentItem.AmazonMWSSyncInboundShipmentLogItemID, "custrecord_api_external_id_line");
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
                if (ImageSolutionsFBAInboundshipmentItem == null) throw new Exception("ImageSolutionsFBAInboundshipmentItem cannot be null");
                if (NetSuiteFBAInboundShipmentItem == null) throw new Exception("NetSuiteFBAInboundShipmentItem record does not exists in NetSuite");

                objWriteResult = Service.update(UpdateNetSuiteFBAInboundShipmentItem());

                if (objWriteResult.status.isSuccess != true)
                {
                    throw new Exception("FBAInboundShipmentItem Update() : " + objWriteResult.status.statusDetail[0].message);
                }
                else
                {
                    mNetSuiteFBAInboundShipmentItem = null;
                }
                ImageSolutionsFBAInboundshipmentItem.IsNSUpdated = true;
                ImageSolutionsFBAInboundshipmentItem.ErrorMessage = string.Empty;
                ImageSolutionsFBAInboundshipmentItem.Update();
            }
            catch (Exception ex)
            {
                ImageSolutionsFBAInboundshipmentItem.ErrorMessage = "FBAInboundShipmentItem.cs - Update() - " + ex.Message;
                ImageSolutionsFBAInboundshipmentItem.Update();
            }
            finally
            {
                objWriteResult = null;
            }
            return true;
        }

        private NetSuiteLibrary.com.netsuite.webservices.CustomRecord UpdateNetSuiteFBAInboundShipmentItem()
        {
            NetSuiteLibrary.com.netsuite.webservices.CustomRecord objReturn = null;
            RecordRef objRecordRef = null;
            int intCustomFieldIndex = 0;

            try
            {
                //if (ImageSolutionsTransferOrder.TransferOrder == null) throw new Exception("TransferOrder is missing");
                //if (string.IsNullOrEmpty(ImageSolutionsTransferOrder.TransferOrder.NetSuiteInternalID)) throw new Exception("TransferOrder.NetSuiteInternalID is missing");

                objRecordRef = new RecordRef();
                objRecordRef.internalId = "450";
                objRecordRef.type = RecordType.customRecordType;

                objReturn = new CustomRecord();
                objReturn.recType = objRecordRef;
                objReturn.internalId = ImageSolutionsFBAInboundshipmentItem.NetSuiteInternalID;

                objReturn.customFieldList = new CustomFieldRef[99];
                objReturn.customFieldList[intCustomFieldIndex++] = NetSuiteHelper.CreateLongCustomField(ImageSolutionsFBAInboundshipmentItem.QuantityReceived, "custrecord_fba_inbound_qty_received");
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally { }
            return objReturn;
        }

        //public bool Update()
        //{
        //    WriteResponse objWriteResult = null;

        //    try
        //    {
        //        if (ImageSolutionsTransferOrder == null) throw new Exception("ImageSolutionsTransferOrder cannot be null");
        //        if (NetSuiteTransferOrder == null) throw new Exception("Transfer order record does not exists in NetSuite");

        //        objWriteResult = Service.update(UpdateNetSuiteTransferOrder());

        //        if (objWriteResult.status.isSuccess != true)
        //        {
        //            throw new Exception("TransferOrder Update() : " + objWriteResult.status.statusDetail[0].message);
        //        }
        //        else
        //        {
        //            mNetSuiteTransferOrder = null;
        //        }
        //        ImageSolutionsTransferOrder.IsNSUpdated = true;
        //        ImageSolutionsTransferOrder.ErrorMessage = string.Empty;
        //        ImageSolutionsTransferOrder.Update();
        //    }
        //    catch (Exception ex)
        //    {
        //        ImageSolutionsTransferOrder.ErrorMessage = "TransferOrder.cs - Update() - " + ex.Message;
        //        ImageSolutionsTransferOrder.Update();
        //    }
        //    finally
        //    {
        //        objWriteResult = null;
        //    }
        //    return true;
        //}

        //private NetSuiteLibrary.com.netsuite.webservices.TransferOrder UpdateNetSuiteTransferOrder()
        //{
        //    NetSuiteLibrary.com.netsuite.webservices.TransferOrder objReturn = null;
        //    int intCustomFieldIndex = 0;

        //    try
        //    {
        //        objReturn = new NetSuiteLibrary.com.netsuite.webservices.TransferOrder();
        //        objReturn.internalId = ImageSolutionsTransferOrder.NetSuiteInternalID;

        //        objReturn.customFieldList = new CustomFieldRef[99];

        //        if (ImageSolutionsTransferOrder.AmazonMWSSyncInboundShipmentPlanLog != null)
        //        {
        //            objReturn.customFieldList[intCustomFieldIndex++] = NetSuiteHelper.CreateBooleanCustomField(true, "custbody_fba_shipment_plan_created");
        //            objReturn.customFieldList[intCustomFieldIndex++] = NetSuiteHelper.CreateDateCustomField(ImageSolutionsTransferOrder.AmazonMWSSyncInboundShipmentPlanLog.CreatedOn.AddHours(-7), "custbody_fba_shipment_plan_created_on");

        //            if (ImageSolutionsTransferOrder.AmazonMWSSyncInboundShipmentPlanLog.AmazonMWSSyncInboundShipmentLogItems != null && !ImageSolutionsTransferOrder.AmazonMWSSyncInboundShipmentPlanLog.AmazonMWSSyncInboundShipmentLogItems.Exists(m => string.IsNullOrEmpty(m.CreateInboundShipmentXML)))
        //            {
        //                objReturn.customFieldList[intCustomFieldIndex++] = NetSuiteHelper.CreateBooleanCustomField(true, "custbody_fba_shipment_created");
        //                objReturn.customFieldList[intCustomFieldIndex++] = NetSuiteHelper.CreateDateCustomField(ImageSolutionsTransferOrder.AmazonMWSSyncInboundShipmentPlanLog.AmazonMWSSyncInboundShipmentLogItems[0].CreatedOn.AddHours(-7), "custbody_fba_shipment_created_on");
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //    finally
        //    {
        //    }
        //    return objReturn;
        //}

        //public bool UpdateNetSuiteApiExternalID()
        //{
        //    WriteResponse objWriteResult = null;

        //    try
        //    {
        //        if (ImageSolutionsTransferOrder == null) throw new Exception("ImageSolutionsTransferOrder cannot be null");
        //        if (mNetSuiteTransferOrder == null) throw new Exception("Transfer order record does not exists in NetSuite");

        //        objWriteResult = Service.update(UpdateNetSuiteAPIExternalID());

        //        if (objWriteResult.status.isSuccess != true)
        //        {
        //            throw new Exception("TransferOrder Update : transfer order can not be updated " + objWriteResult.status.statusDetail[0].message);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception("UpdateNetSuiteApiExternalID() - " + ex.Message);
        //    }
        //    finally
        //    {
        //        objWriteResult = null;
        //    }
        //    return true;
        //}

        //private NetSuiteLibrary.com.netsuite.webservices.TransferOrder UpdateNetSuiteAPIExternalID()
        //{
        //    if (mNetSuiteTransferOrder == null) throw new Exception("NetSuiteTransferOrder cannot be null");
        //    if (string.IsNullOrEmpty(mNetSuiteTransferOrder.internalId)) throw new Exception("NetSuiteTransferOrder internalid cannot be null");
        //    if (ImageSolutionsTransferOrder == null) throw new Exception("ImageSolutionsTransferOrder cannot be null");
        //    if (string.IsNullOrEmpty(ImageSolutionsTransferOrder.TransferOrderID)) throw new Exception("ImageSolutionsTransferOrder TransferOrderID can not be empty");

        //    NetSuiteLibrary.com.netsuite.webservices.TransferOrder objNetSuiteTransferOrder = null;
        //    try
        //    {
        //        objNetSuiteTransferOrder = new com.netsuite.webservices.TransferOrder();
        //        objNetSuiteTransferOrder.customForm = NetSuiteLibrary.NetSuiteHelper.GetRecordRef(NetSuiteTransferOrderFormID, RecordType.transferOrder);
        //        objNetSuiteTransferOrder.internalId = ImageSolutionsTransferOrder.NetSuiteInternalID;
        //        objNetSuiteTransferOrder.customFieldList = new CustomFieldRef[1];
        //        objNetSuiteTransferOrder.customFieldList[0] = NetSuiteHelper.CreateStringCustomField(ImageSolutionsTransferOrder.TransferOrderID, "custbody_api_external_id");
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //    return objNetSuiteTransferOrder;
        //}

        //public bool Delete()
        //{
        //    RecordRef objPurchaseOrderRef = null;
        //    WriteResponse objDeleteResponse = null;

        //    try
        //    {
        //        if (ImageSolutionsTransferOrder == null) throw new Exception("ImageSolutionsTransferOrder cannot be null");

        //        if (NetSuiteTransferOrder != null)
        //        {
        //            objPurchaseOrderRef = new RecordRef();
        //            objPurchaseOrderRef.internalId = NetSuiteTransferOrder.internalId;
        //            objPurchaseOrderRef.type = RecordType.transferOrder;
        //            objPurchaseOrderRef.typeSpecified = true;
        //            objDeleteResponse = Service.delete(objPurchaseOrderRef, null);

        //            if (objDeleteResponse.status.isSuccess != true)
        //            {
        //                throw new Exception("Unable to delete TransferOrder: " + objDeleteResponse.status.statusDetail[0].message);
        //            }
        //            else
        //            {
        //                mNetSuiteTransferOrder = null;
        //            }
        //        }

        //        ImageSolutionsTransferOrder.ErrorMessage = string.Empty;
        //        ImageSolutionsTransferOrder.NetSuiteInternalID = string.Empty;
        //        ImageSolutionsTransferOrder.NetSuiteDocumentNumber = string.Empty;
        //        ImageSolutionsTransferOrder.Update();
        //    }
        //    catch (Exception ex)
        //    {
        //        if (ex.Message.Contains("Can not find TransferOrder with Internal ID"))
        //        {
        //            ImageSolutionsTransferOrder.ErrorMessage = string.Empty;
        //            ImageSolutionsTransferOrder.NetSuiteInternalID = string.Empty;
        //            ImageSolutionsTransferOrder.NetSuiteDocumentNumber = string.Empty;
        //            ImageSolutionsTransferOrder.Update();
        //        }
        //        else
        //        {
        //            ImageSolutionsTransferOrder.ErrorMessage = "TransferOrder.cs - Delete() - " + ex.Message;
        //            ImageSolutionsTransferOrder.Update();
        //        }
        //    }
        //    finally
        //    {
        //        objPurchaseOrderRef = null;
        //        objDeleteResponse = null;
        //    }
        //    return true;
        //}

        public FBAInboundShipmentItem ObjectAlreadyExists()
        {
            List<FBAInboundShipmentItem> objFBAInboundShipmentItems = null;
            FBAInboundShipmentItemFilter objFilter = null;
            FBAInboundShipmentItem objReturn = null;

            try
            {
                objFilter = new FBAInboundShipmentItemFilter();
                objFilter.APIExternalID = new Database.Filter.StringSearch.SearchFilter();
                objFilter.APIExternalID.SearchString = ImageSolutionsFBAInboundshipmentItem.AmazonMWSSyncInboundShipmentLogItemID;
                objFBAInboundShipmentItems = GetFBAInboundShipmentItems(Service, objFilter);
                if (objFBAInboundShipmentItems != null && objFBAInboundShipmentItems.Count() > 0)
                {
                    if (objFBAInboundShipmentItems.Count > 1) throw new Exception("More than one FBAInboundShipmentItems with API External ID:" + ImageSolutionsFBAInboundshipmentItem.AmazonMWSSyncInboundShipmentLogItemID + " found in Netsuite with InternalIDs " + string.Join(", ", objFBAInboundShipmentItems.Select(m => m.NetSuiteFBAInboundShipmentItem.internalId)));
                    objReturn = objFBAInboundShipmentItems[0];
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                objFBAInboundShipmentItems = null;
                objFilter = null;
            }
            return objReturn;
        }

        private static FBAInboundShipmentItem GetFBAInboundShipmentItem(NetSuiteService Service, FBAInboundShipmentItemFilter Filter)
        {
            List<FBAInboundShipmentItem> objFBAInboundShipmentItems = null;
            FBAInboundShipmentItem objReturn = null;

            try
            {
                objFBAInboundShipmentItems = GetFBAInboundShipmentItems(Service, Filter);
                if (objFBAInboundShipmentItems != null && objFBAInboundShipmentItems.Count >= 1) objReturn = objFBAInboundShipmentItems[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                objFBAInboundShipmentItems = null;
            }
            return objReturn;
        }

        public static List<FBAInboundShipmentItem> GetFBAInboundShipmentItems(FBAInboundShipmentItemFilter Filter)
        {
            return GetFBAInboundShipmentItems(Service, Filter);
        }

        private static List<FBAInboundShipmentItem> GetFBAInboundShipmentItems(NetSuiteService Service, FBAInboundShipmentItemFilter Filter)
        {
            List<FBAInboundShipmentItem> objReturn = null;
            SearchResult objSearchResult = null;

            try
            {
                objReturn = new List<FBAInboundShipmentItem>();
                objSearchResult = GetNetSuiteTransactions(Service, Filter);
                if (objSearchResult != null && objSearchResult.totalRecords > 0)
                {
                    do
                    {
                        foreach (object objNetFBAInboundShipmentItem in objSearchResult.recordList)
                        {
                            if (objNetFBAInboundShipmentItem is NetSuiteLibrary.com.netsuite.webservices.CustomRecord)
                            {
                                objReturn.Add(new FBAInboundShipmentItem((NetSuiteLibrary.com.netsuite.webservices.CustomRecord)objNetFBAInboundShipmentItem));
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

        private static SearchResult GetNetSuiteTransactions(NetSuiteService Service, FBAInboundShipmentItemFilter Filter)
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
                        objAPIExternalID.scriptId = "custrecord_api_external_id_line";

                        objAPIExternalID.searchValue = Filter.APIExternalID.SearchString;
                        objAPIExternalID.@operator = SearchStringFieldOperator.@is;
                        objAPIExternalID.operatorSpecified = true;

                        if (Filter.APIExternalID.Operator != null)
                        {
                            switch (Filter.APIExternalID.Operator)
                            {
                                case Database.Filter.StringSearch.SearchOperator.empty:
                                    objAPIExternalID.@operator = SearchStringFieldOperator.empty;
                                    break;
                                case Database.Filter.StringSearch.SearchOperator.notEmpty:
                                    objAPIExternalID.@operator = SearchStringFieldOperator.notEmpty;
                                    break;
                            }
                        }
                        objCustomRecordSearch.basic.customFieldList[intCutstomField] = objAPIExternalID;
                        intCutstomField++;
                    }
                    if (Filter.FBAInboundShipmentInternalID != null)
                    {
                        SearchMultiSelectCustomField objFBAInboundShipmentInternalID = new SearchMultiSelectCustomField();
                        objFBAInboundShipmentInternalID.scriptId = "custrecord_fba_inbound_shipment_number";
                        objFBAInboundShipmentInternalID.searchValue = new ListOrRecordRef[1];
                        objFBAInboundShipmentInternalID.searchValue[0] = new ListOrRecordRef();
                        objFBAInboundShipmentInternalID.searchValue[0].internalId = Filter.FBAInboundShipmentInternalID.SearchString;
                        objFBAInboundShipmentInternalID.@operator = SearchMultiSelectFieldOperator.anyOf;
                        objFBAInboundShipmentInternalID.operatorSpecified = true;

                        objCustomRecordSearch.basic.customFieldList[intCutstomField] = objFBAInboundShipmentInternalID;
                        intCutstomField++;
                    }
                }

                objRecordRef = new RecordRef();
                objRecordRef.internalId = "450";
                objRecordRef.type = RecordType.customRecordType;

                objCustomRecordSearch.basic.recType = objRecordRef;

                objSearchPreferences = new SearchPreferences();
                objSearchPreferences.bodyFieldsOnly = false;
                objSearchPreferences.pageSize = 10;
                objSearchPreferences.pageSizeSpecified = true;

                Service.searchPreferences = objSearchPreferences;
                objSearchResult = Service.search(objCustomRecordSearch);

                if (objSearchResult.status.isSuccess != true) throw new Exception("Cannot find FBAInboundShipmentItem - " + objSearchResult.status.statusDetail[0].message);
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


