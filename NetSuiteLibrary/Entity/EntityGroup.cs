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

namespace NetSuiteLibrary.Entity
{
    public class EntityGroup : NetSuiteBase
    {
        private ImageSolutions.Entity.EntityGroup mImageSolutionsEntityGroup = null;
        public ImageSolutions.Entity.EntityGroup ImageSolutionsEntityGroup
        {
            get
            {
                if (mImageSolutionsEntityGroup == null && mNetSuiteEntityGroup != null && !string.IsNullOrEmpty(mNetSuiteEntityGroup.internalId))
                {
                    List<ImageSolutions.Entity.EntityGroup> objEntityGroups = null;
                    ImageSolutions.Entity.EntityGroupFilter objFilter = null;

                    try
                    {
                        objFilter = new ImageSolutions.Entity.EntityGroupFilter();
                        objFilter.InternalID = new Database.Filter.StringSearch.SearchFilter();
                        objFilter.InternalID.SearchString = mNetSuiteEntityGroup.internalId;
                        objEntityGroups = ImageSolutions.Entity.EntityGroup.GetEntityGroups(objFilter);
                        if (objEntityGroups != null && objEntityGroups.Count > 0)
                        {
                            mImageSolutionsEntityGroup = objEntityGroups[0];
                        }
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    finally
                    {
                        objEntityGroups = null;
                    }
                }
                return mImageSolutionsEntityGroup;
            }
            private set
            {
                mImageSolutionsEntityGroup = value;
            }
        }
        //private ImageSolutions.TransferOrder.TransferOrder mImageSolutionsTransferOrder = null;
        //public ImageSolutions.TransferOrder.TransferOrder ImageSolutionsTransferOrder
        //{
        //    get
        //    {
        //        if (mImageSolutionsTransferOrder == null && mNetSuiteEntityGroup != null && !string.IsNullOrEmpty(mNetSuiteEntityGroup.internalId))
        //        {
        //            List<ImageSolutions.TransferOrder.TransferOrder> objTransferOrders = null;
        //            ImageSolutions.TransferOrder.TransferOrderFilter objFilter = null;

        //            try
        //            {
        //                objFilter = new ImageSolutions.TransferOrder.TransferOrderFilter();
        //                objFilter.NetSuiteInternalID = new Database.Filter.StringSearch.SearchFilter();
        //                objFilter.NetSuiteInternalID.SearchString = mNetSuiteEntityGroup.internalId;
        //                objTransferOrders = ImageSolutions.TransferOrder.TransferOrder.GetTransferOrders(objFilter);
        //                if (objTransferOrders != null && objTransferOrders.Count > 0)
        //                {
        //                    mImageSolutionsTransferOrder = objTransferOrders[0];
        //                }
        //            }
        //            catch (Exception ex)
        //            {
        //                throw ex;
        //            }
        //            finally
        //            {
        //                objTransferOrders = null;
        //            }
        //        }
        //        return mImageSolutionsTransferOrder;
        //    }
        //    private set
        //    {
        //        mImageSolutionsTransferOrder = value;
        //    }
        //}

        private NetSuiteLibrary.com.netsuite.webservices.EntityGroup mNetSuiteEntityGroup = null;
        public NetSuiteLibrary.com.netsuite.webservices.EntityGroup NetSuiteEntityGroup
        {
            get
            {
                if (mNetSuiteEntityGroup == null) // || mNetSuiteEntityGroup.itemList == null)
                {
                    mNetSuiteEntityGroup = LoadNetSuiteEntityGroup(mNetSuiteEntityGroup.internalId);
                }
                return mNetSuiteEntityGroup;
            }
            private set
            {
                mNetSuiteEntityGroup = value;
            }
        }

        public EntityGroup(ImageSolutions.Entity.EntityGroup ImageSolutionsEntityGroup)
        {
            mImageSolutionsEntityGroup = ImageSolutionsEntityGroup;
        }

        public EntityGroup(NetSuiteLibrary.com.netsuite.webservices.EntityGroup NetSuiteEntityGroup)
        {
            mNetSuiteEntityGroup = NetSuiteEntityGroup;
        }
        private NetSuiteLibrary.com.netsuite.webservices.EntityGroup LoadNetSuiteEntityGroup(string NetSuiteInternalID)
        {
            RecordRef objSORef = null;
            ReadResponse objReadResult = null;
            NetSuiteLibrary.com.netsuite.webservices.EntityGroup objReturn = null;

            try
            {
                objSORef = new RecordRef();
                objSORef.type = RecordType.entityGroup;
                objSORef.typeSpecified = true;
                objSORef.internalId = NetSuiteInternalID;

                objReadResult = Service.get(objSORef);

                if (objReadResult != null && objReadResult.record != null && (objReadResult.record is NetSuiteLibrary.com.netsuite.webservices.EntityGroup))
                {
                    objReturn = (NetSuiteLibrary.com.netsuite.webservices.EntityGroup)objReadResult.record;
                }
                else
                {
                    throw new Exception("Can not find EntityGroup with Internal ID : " + NetSuiteInternalID);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                objSORef = null;
                objReadResult = null;
            }
            return objReturn;
        }

        public bool Create()
        {
            WriteResponse objWriteResponse = null;
            EntityGroup objEntityGroup = null;

            try
            {
                if (ImageSolutionsEntityGroup == null) throw new Exception("ImageSolutionsEntityGroup cannot be null");

                objEntityGroup = ObjectAlreadyExists();

                if (objEntityGroup != null)
                {
                    //NetSuite InternalID did not get updated, auto fix
                    ImageSolutionsEntityGroup.InternalID = objEntityGroup.NetSuiteEntityGroup.internalId;
                    //ImageSolutionsEntityGroup.NetSuiteDocumentNumber = objEntityGroup.NetSuiteEntityGroup.tranId;
                }
                else
                {
                    objWriteResponse = Service.add(CreateNetSuiteEntityGroup());
                    if (objWriteResponse.status.isSuccess != true)
                    {
                        throw new Exception("Unable to create EntityGroup: " + objWriteResponse.status.statusDetail[0].message);
                    }
                    else
                    {
                        ImageSolutionsEntityGroup.InternalID = ((RecordRef)objWriteResponse.baseRef).internalId;
                        //ImageSolutionsEntityGroup.NetSuiteDocumentNumber = NetSuiteEntityGroup.tranId;
                    }
                }

                //foreach (NetSuiteLibrary.com.netsuite.webservices.EntityGroupItem objEntityGroupItem in NetSuiteEntityGroup.itemList.item)
                //{
                //    string strAPIExternalID = NetSuiteLibrary.NetSuiteHelper.GetStringCustomFieldValue(objEntityGroupItem, "custcol_api_external_id");
                //    if (string.IsNullOrEmpty(strAPIExternalID) && (objEntityGroupItem.item.internalId == "0"))
                //    {
                //        //This is item group end of group item, or general discount, ignore
                //    }
                //    else
                //    {
                //        if (ImageSolutionsEntityGroup.EntityGroupLines.Exists(m => m.EntityGroupLineID == strAPIExternalID))
                //        {
                //            ImageSolutionsEntityGroup.EntityGroupLines.Find(m => m.EntityGroupLineID == strAPIExternalID).NetSuiteLineID = objEntityGroupItem.line;
                //        }
                //        else
                //        {
                //            ImageSolutionsEntityGroup.NetSuiteInternalID = string.Empty;
                //            ImageSolutionsEntityGroup.NetSuiteDocumentNumber = string.Empty;
                //            throw new Exception("APIExternalID for EntityGroup line " + strAPIExternalID + " did not get created, not found in NetSuite EntityGroup");
                //        }
                //    }
                //}

                //ImageSolutionsEntityGroup.ErrorMessage = string.Empty;
                ImageSolutionsEntityGroup.Update();
            }
            catch (Exception ex)
            {
                //ImageSolutionsEntityGroup.ErrorMessage = "EntityGroup.cs - Create() - " + ex.Message;
                ImageSolutionsEntityGroup.Update();
            }
            finally
            {
                objWriteResponse = null;
            }
            return true;
        }

        private NetSuiteLibrary.com.netsuite.webservices.EntityGroup CreateNetSuiteEntityGroup()
        {
            NetSuiteLibrary.com.netsuite.webservices.EntityGroup objReturn = null;
            int intCustomFieldIndex = 0;
            int intCustomFieldLineIndex = 0;
            NetSuiteLibrary.Item.Item objItem = null;
            string strItemType = string.Empty;
            string strItemVendorID = string.Empty;
            int intItemIndex = 0;
            string strBinNumberInternalID = string.Empty;

            try
            {
                //if (ImageSolutionsEntityGroup.EntityGroup == null) throw new Exception("EntityGroup is missing");
                //if (string.IsNullOrEmpty(ImageSolutionsEntityGroup.EntityGroup.NetSuiteInternalID)) throw new Exception("EntityGroup.NetSuiteInternalID is missing");

                objReturn = new NetSuiteLibrary.com.netsuite.webservices.EntityGroup();
                objReturn.internalId = ImageSolutionsEntityGroup.InternalID;
                //objReturn.customForm = NetSuiteLibrary.NetSuiteHelper.GetRecordRef(NetSuiteEntityGroupFormID, RecordType.EntityGroup);

                //objReturn.tranDate = ImageSolutionsEntityGroup.CreatedOn;
                //objReturn.tranDateSpecified = true;
                //objReturn.memo = ImageSolutionsEntityGroup.ShipmentID;
                ////objReturn.location = NetSuiteHelper.GetRecordRef(ImageSolutionsEntityGroup.ToLocation.NetSuiteInternalID, RecordType.location);
                //objReturn.transferLocation = NetSuiteHelper.GetRecordRef(ImageSolutionsEntityGroup.ToLocation.NetSuiteInternalID, RecordType.location);

                //objReturn.customFieldList = new CustomFieldRef[99];
                //objReturn.customFieldList[intCustomFieldIndex++] = NetSuiteHelper.CreateStringCustomField(ImageSolutionsEntityGroup.EntityGroupID, "custbody_api_external_id");

                //objReturn.itemList = new EntityGroupItemList();
                //objReturn.itemList.item = new EntityGroupItem[ImageSolutionsEntityGroup.EntityGroupLines.Count()];

                //for (int i = 0; i < ImageSolutionsEntityGroup.EntityGroupLines.Count; i++)
                //{
                //    intCustomFieldLineIndex = 0;
                //    string strTaxRate = string.Empty;

                //    objItem = new Item.Item(ImageSolutionsEntityGroup.EntityGroupLines[i].Item);

                //    objReturn.itemList.item[intItemIndex] = new EntityGroupItem();

                //    if (objItem.NetSuiteInventoryItem != null)
                //    {
                //        objReturn.itemList.item[intItemIndex].item = NetSuiteLibrary.NetSuiteHelper.GetRecordRef(ImageSolutionsEntityGroup.EntityGroupLines[i].Item.InternalID, RecordType.inventoryItem);
                //    }
                //    else if (objItem.NetSuiteKitItem != null)
                //    {
                //        objReturn.itemList.item[intItemIndex].item = NetSuiteLibrary.NetSuiteHelper.GetRecordRef(objItem.NetSuiteKitItem.internalId, RecordType.kitItem);
                //    }
                //    else
                //    {
                //        throw new Exception("NetSuite Item not handeled");
                //    }
                //    objReturn.itemList.item[intItemIndex].quantity = ImageSolutionsEntityGroup.EntityGroupLines[i].Quantity;
                //    objReturn.itemList.item[intItemIndex].quantitySpecified = true;

                //    objReturn.itemList.item[intItemIndex].customFieldList = new CustomFieldRef[99];
                //    objReturn.itemList.item[intItemIndex].customFieldList[intCustomFieldLineIndex++] = NetSuiteHelper.CreateStringCustomField(ImageSolutionsEntityGroup.EntityGroupLines[i].EntityGroupLineID, "custcol_api_external_id");

                //    intItemIndex++;
                //}
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
                if (ImageSolutionsEntityGroup == null) throw new Exception("ImageSolutionsEntityGroup cannot be null");
                if (NetSuiteEntityGroup == null) throw new Exception("Purchase order record does not exists in NetSuite");

                objWriteResult = Service.update(UpdateNetSuiteEntityGroup());

                if (objWriteResult.status.isSuccess != true)
                {
                    throw new Exception("EntityGroup Update() : " + objWriteResult.status.statusDetail[0].message);
                }
                else
                {
                    mNetSuiteEntityGroup = null;
                }

                //ImageSolutionsEntityGroup.ErrorMessage = string.Empty;
                ImageSolutionsEntityGroup.Update();
            }
            catch (Exception ex)
            {
                //ImageSolutionsEntityGroup.ErrorMessage = "EntityGroup.cs - Update() - " + ex.Message;
                ImageSolutionsEntityGroup.Update();
            }
            finally
            {
                objWriteResult = null;
            }
            return true;
        }

        private NetSuiteLibrary.com.netsuite.webservices.EntityGroup UpdateNetSuiteEntityGroup()
        {
            NetSuiteLibrary.com.netsuite.webservices.EntityGroup objReturn = null;
            int intCustomFieldIndex = 0;
            string strBinNumberInternalID = string.Empty;

            try
            {
                objReturn = new NetSuiteLibrary.com.netsuite.webservices.EntityGroup();
                objReturn.internalId = ImageSolutionsEntityGroup.InternalID;

                objReturn.customFieldList = new CustomFieldRef[99];

                if (ImageSolutionsEntityGroup != null)
                {
                    objReturn.customFieldList[intCustomFieldIndex++] = NetSuiteHelper.CreateBooleanCustomField(true, "custbody_is_sent_to_vendor");
                    //objReturn.customFieldList[intCustomFieldIndex++] = NetSuiteHelper.CreateSelectCustomField(ImageSolutionsEntityGroup.ErrorMessage, "custbody_error_message");
                }


                //if (NetSuiteEntityGroup.status == "Pending Receipt")
                //{
                //    objReturn.itemList = new EntityGroupItemList();
                //    objReturn.itemList.item = new EntityGroupItem[ImageSolutionsTransferOrder.TransferOrderLines.Count()];
                //    objReturn.itemList.replaceAll = false;

                //for (int i = 0; i < ImageSolutionsTransferOrder.TransferOrderLines.Count; i++)
                //{
                //    bool blnFound = false;

                //    if (ImageSolutionsTransferOrder.TransferOrderLines[i].NetSuiteLineID == null) throw new Exception("TransferOrderLine: " + ImageSolutionsTransferOrder.TransferOrderLines[i].TransferOrderLineID + " is missing LineItemID");

                //    objReturn.itemList.item[i] = new EntityGroupItem();
                //    objReturn.itemList.item[i].line = ImageSolutionsTransferOrder.TransferOrderLines[i].NetSuiteLineID.Value;
                //    objReturn.itemList.item[i].lineSpecified = true;

                //    foreach (NetSuiteLibrary.com.netsuite.webservices.EntityGroupItem objEntityGroupItem in NetSuiteEntityGroup.itemList.item)
                //    {
                //        string strAPIExternalID = NetSuiteLibrary.NetSuiteHelper.GetStringCustomFieldValue(objEntityGroupItem, "custcol_api_external_id");
                //        if (ImageSolutionsTransferOrder.TransferOrderLines[i].TransferOrderLineID == strAPIExternalID)
                //        {
                //            objReturn.itemList.item[i].quantity = ImageSolutionsTransferOrder.TransferOrderLines[i].Quantity;
                //            objReturn.itemList.item[i].quantitySpecified = true;
                //            blnFound = true;
                //            break;
                //        }
                //    }
                //    if (!blnFound)
                //    {
                //        //Do not throw, delete from DB and retry next time
                //        ImageSolutionsTransferOrder.TransferOrderLines[i].Delete();
                //        objReturn.itemList.item[i] = null;
                //        //break;
                //        //throw new Exception("EntityGroupLine " + ImageSolutionsEntityGroup.EntityGroupLines[i].EntityGroupLineID + " did not get created, not found in NetSuite transfer order");
                //    }
                //}
                //}
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
            }
            return objReturn;
        }

        //public bool Close()
        //{
        //    WriteResponse objWriteResult = null;

        //    try
        //    {
        //        //if (ImageSolutionsTransferOrder == null) throw new Exception("ImageSolutionsEntityGroup cannot be null");
        //        if (NetSuiteEntityGroup == null) throw new Exception("Purchase order record does not exists in NetSuite");

        //        objWriteResult = Service.update(CloseNetSuiteEntityGroup());

        //        if (objWriteResult.status.isSuccess != true)
        //        {
        //            throw new Exception("EntityGroup Close() : " + objWriteResult.status.statusDetail[0].message);
        //        }
        //        else
        //        {
        //            mNetSuiteEntityGroup = null;
        //        }
        //        //ImageSolutionsTransferOrder.IsNSUpdated = true;
        //        //ImageSolutionsTransferOrder.ErrorMessage = string.Empty;
        //        //ImageSolutionsTransferOrder.Update();
        //    }
        //    catch (Exception ex)
        //    {
        //        //ImageSolutionsTransferOrder.ErrorMessage = "EntityGroup.cs - Close() - " + ex.Message;
        //        //ImageSolutionsTransferOrder.Update();
        //    }
        //    finally
        //    {
        //        objWriteResult = null;
        //    }
        //    return true;
        //}

        public bool Delete()
        {
            RecordRef objEntityGroupRef = null;
            WriteResponse objDeleteResponse = null;

            try
            {
                if (ImageSolutionsEntityGroup == null) throw new Exception("ImageSolutionsEntityGroup cannot be null");

                if (NetSuiteEntityGroup != null)
                {
                    objEntityGroupRef = new RecordRef();
                    objEntityGroupRef.internalId = NetSuiteEntityGroup.internalId;
                    objEntityGroupRef.type = RecordType.entityGroup;
                    objEntityGroupRef.typeSpecified = true;
                    objDeleteResponse = Service.delete(objEntityGroupRef, null);

                    if (objDeleteResponse.status.isSuccess != true)
                    {
                        throw new Exception("Unable to delete EntityGroup: " + objDeleteResponse.status.statusDetail[0].message);
                    }
                    else
                    {
                        mNetSuiteEntityGroup = null;
                    }
                }

                //ImageSolutionsEntityGroup.ErrorMessage = string.Empty;
                ImageSolutionsEntityGroup.InternalID = string.Empty;
                //ImageSolutionsEntityGroup.NetSuiteDocumentNumber = string.Empty;
                ImageSolutionsEntityGroup.Update();
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("Can not find EntityGroup with Internal ID"))
                {
                    //ImageSolutionsEntityGroup.ErrorMessage = string.Empty;
                    ImageSolutionsEntityGroup.InternalID = string.Empty;
                    //ImageSolutionsEntityGroup.NetSuiteDocumentNumber = string.Empty;
                    ImageSolutionsEntityGroup.Update();
                }
                else
                {
                    //ImageSolutionsEntityGroup.ErrorMessage = "EntityGroup.cs - Delete() - " + ex.Message;
                    ImageSolutionsEntityGroup.Update();
                }
            }
            finally
            {
                objEntityGroupRef = null;
                objDeleteResponse = null;
            }
            return true;
        }

        public EntityGroup ObjectAlreadyExists()
        {
            List<EntityGroup> objEntityGroups = null;
            EntityGroupFilter objFilter = null;
            EntityGroup objReturn = null;

            try
            {
                objFilter = new EntityGroupFilter();
                objFilter.APIExternalID = new Database.Filter.StringSearch.SearchFilter();
                objFilter.APIExternalID.SearchString = ImageSolutionsEntityGroup.EntityGroupID;
                objEntityGroups = GetEntityGroups(Service, objFilter);
                if (objEntityGroups != null && objEntityGroups.Count() > 0)
                {
                    if (objEntityGroups.Count > 1) throw new Exception("More than one EntityGroups with API External ID:" + ImageSolutionsEntityGroup.EntityGroupID + " found in Netsuite with InternalIDs " + string.Join(", ", objEntityGroups.Select(m => m.NetSuiteEntityGroup.internalId)));
                    objReturn = objEntityGroups[0];
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                objEntityGroups = null;
                objFilter = null;
            }
            return objReturn;
        }

        public static EntityGroup GetEntityGroup(NetSuiteService Service, EntityGroupFilter Filter)
        {
            List<EntityGroup> objEntityGroups = null;
            EntityGroup objReturn = null;

            try
            {
                objEntityGroups = GetEntityGroups(Service, Filter);
                if (objEntityGroups != null && objEntityGroups.Count >= 1) objReturn = objEntityGroups[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                objEntityGroups = null;
            }
            return objReturn;
        }

        public List<EntityGroup> GetEntityGroups(EntityGroupFilter Filter)
        {
            return GetEntityGroups(Service, Filter);
        }

        public static List<EntityGroup> GetEntityGroups(NetSuiteService Service, EntityGroupFilter Filter)
        {
            List<EntityGroup> objReturn = null;
            SearchResult objSearchResult = null;

            try
            {
                objReturn = new List<EntityGroup>();
                objSearchResult = GetNetSuiteEntityGroups(Service, Filter);
                if (objSearchResult != null && objSearchResult.totalRecords > 0)
                {
                    do
                    {
                        foreach (object objNetEntityGroup in objSearchResult.recordList)
                        {
                            if (objNetEntityGroup is NetSuiteLibrary.com.netsuite.webservices.EntityGroup)
                            {
                                objReturn.Add(new EntityGroup((NetSuiteLibrary.com.netsuite.webservices.EntityGroup)objNetEntityGroup));
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

        private static SearchResult GetNetSuiteEntityGroups(NetSuiteService Service, EntityGroupFilter Filter)
        {
            SearchResult objSearchResult = null;
            EntityGroupSearch objEntityGroupSearch = null;
            SearchPreferences objSearchPreferences = null;
            int intCutstomField = 0;

            try
            {
                objEntityGroupSearch = new EntityGroupSearch();
                objEntityGroupSearch.basic = new EntityGroupSearchBasic();
                objEntityGroupSearch.basic.customFieldList = new SearchCustomField[99];

                if (Filter != null)
                {
                    if (Filter.InternalIDs != null)
                    {
                        objEntityGroupSearch.basic.internalId = NetSuiteHelper.LoadMultiSearchField(Filter.InternalIDs);
                    }

                    if (Filter.APIExternalID != null)
                    {
                        SearchStringCustomField objAPIExternalID = new SearchStringCustomField();
                        objAPIExternalID.scriptId = "custbody_api_external_id";

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
                        objEntityGroupSearch.basic.customFieldList[intCutstomField] = objAPIExternalID;
                        intCutstomField++;
                    }
                }

                Service.searchPreferences = objSearchPreferences;
                objSearchResult = Service.search(objEntityGroupSearch);

                if (objSearchResult.status.isSuccess != true) throw new Exception("Cannot find EntityGroup - " + objSearchResult.status.statusDetail[0].message);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                objEntityGroupSearch = null;
                objSearchPreferences = null;
            }
            return objSearchResult;
        }

        public static List<string> GetEntityGroupSavedSearch(NetSuiteService Service, string savedsearchid)
        {
            List<string> objReturn = null;
            SearchResult objSearchResult = null;
            Dictionary<String, String> dicInternalIds = new Dictionary<string, string>();

            try
            {
                objReturn = new List<string>();
                dicInternalIds = new Dictionary<string, string>();

                objSearchResult = GetEntityGroupSavedSearchEntityGroup(Service, savedsearchid);
                if (objSearchResult != null && objSearchResult.totalRecords > 0)
                {
                    do
                    {
                        foreach (EntityGroupSearchRow objNetEntityGroup in objSearchResult.searchRowList)
                        {
                            if (objNetEntityGroup is NetSuiteLibrary.com.netsuite.webservices.EntityGroupSearchRow && !dicInternalIds.ContainsKey(objNetEntityGroup.basic.internalId[0].searchValue.internalId))
                            {
                                dicInternalIds.Add(objNetEntityGroup.basic.internalId[0].searchValue.internalId, objNetEntityGroup.basic.internalId[0].searchValue.internalId);
                                objReturn.Add(objNetEntityGroup.basic.internalId[0].searchValue.internalId);
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

        public static SearchResult GetEntityGroupSavedSearchEntityGroup(NetSuiteService Service, string savedsearchid)
        {
            EntityGroupSearchAdvanced objEntityGroupSearchAdvanced = null;
            SearchResult objSearchResult = null;
            EntityGroupSearch objTransacSearch = null;
            SearchPreferences objSearchPreferences = null;
            int intCutstomField = 0;

            try
            {
                objEntityGroupSearchAdvanced = new EntityGroupSearchAdvanced();
                objEntityGroupSearchAdvanced.savedSearchId = savedsearchid;

                Service.searchPreferences = objSearchPreferences;
                objSearchResult = Service.search(objEntityGroupSearchAdvanced);

                if (objSearchResult.status.isSuccess != true) throw new Exception(string.Format("Cannot find EntityGroup Saved Search - {0}: {1}", savedsearchid, objSearchResult.status.statusDetail[0].message));
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

        public void Attach(string customerinternalid)
        {
            RecordRef EntityGroup = new RecordRef();
            EntityGroup.internalId = ImageSolutionsEntityGroup.InternalID;
            EntityGroup.type = RecordType.entityGroup;
            EntityGroup.typeSpecified = true;

            RecordRef Customer = new RecordRef();
            Customer.internalId = customerinternalid;
            Customer.type = RecordType.customer;
            Customer.typeSpecified = true;

            AttachContactReference AttachContactReference = new AttachContactReference();
            AttachContactReference.contact = Customer;
            AttachContactReference.attachTo = EntityGroup;

            WriteResponse objWriteResponse = Service.attach(AttachContactReference);

            if (objWriteResponse.status.isSuccess != true)
            {
                throw new Exception("Unable to Attach: " + objWriteResponse.status.statusDetail[0].message);
            }
        }

        public void Detach(string customerinternalid)
        {
            RecordRef EntityGroup = new RecordRef();
            EntityGroup.internalId = ImageSolutionsEntityGroup.InternalID;
            EntityGroup.type = RecordType.entityGroup;
            EntityGroup.typeSpecified = true;

            RecordRef Customer = new RecordRef();
            Customer.internalId = customerinternalid;
            Customer.type = RecordType.customer;
            Customer.typeSpecified = true;

            DetachBasicReference DetachBasicReference = new DetachBasicReference();
            DetachBasicReference.detachFrom = EntityGroup;
            DetachBasicReference.detachedRecord = Customer;

            WriteResponse objWriteResponse = Service.detach(DetachBasicReference);

            if (objWriteResponse.status.isSuccess != true)
            {
                throw new Exception("Unable to Detach: " + objWriteResponse.status.statusDetail[0].message);
            }
        }
    }
}
