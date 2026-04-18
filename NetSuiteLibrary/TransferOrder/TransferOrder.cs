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

namespace NetSuiteLibrary.TransferOrder
{
    public class TransferOrder : NetSuiteBase
    {
        private static string NetSuiteTransferOrderFormID
        {
            get
            {
                if (ConfigurationManager.AppSettings["NetSuiteTransferOrderFormID"] != null)
                    return ConfigurationManager.AppSettings["NetSuiteTransferOrderFormID"].ToString();
                else
                    return string.Empty;
            }
        }
        private ImageSolutions.TransferOrder.TransferOrder mImageSolutionsTransferOrder = null;
        public ImageSolutions.TransferOrder.TransferOrder ImageSolutionsTransferOrder
        {
            get
            {
                if (mImageSolutionsTransferOrder == null && mNetSuiteTransferOrder != null && !string.IsNullOrEmpty(mNetSuiteTransferOrder.internalId))
                {
                    List<ImageSolutions.TransferOrder.TransferOrder> objTransferOrders = null;
                    ImageSolutions.TransferOrder.TransferOrderFilter objFilter = null;

                    try
                    {
                        objFilter = new ImageSolutions.TransferOrder.TransferOrderFilter();
                        objFilter.NetSuiteInternalID = new Database.Filter.StringSearch.SearchFilter();
                        objFilter.NetSuiteInternalID.SearchString = mNetSuiteTransferOrder.internalId;
                        objTransferOrders = ImageSolutions.TransferOrder.TransferOrder.GetTransferOrders(objFilter);
                        if (objTransferOrders != null && objTransferOrders.Count > 0)
                        {
                            mImageSolutionsTransferOrder = objTransferOrders[0];
                        }
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    finally
                    {
                        objTransferOrders = null;
                    }
                }
                return mImageSolutionsTransferOrder;
            }
            private set
            {
                mImageSolutionsTransferOrder = value;
            }
        }

        private NetSuiteLibrary.com.netsuite.webservices.TransferOrder mNetSuiteTransferOrder = null;
        public NetSuiteLibrary.com.netsuite.webservices.TransferOrder NetSuiteTransferOrder
        {
            get
            {
                if (mNetSuiteTransferOrder == null && mImageSolutionsTransferOrder != null && !string.IsNullOrEmpty(mImageSolutionsTransferOrder.NetSuiteInternalID))
                {
                    mNetSuiteTransferOrder = LoadNetSuiteTransferOrder(mImageSolutionsTransferOrder.NetSuiteInternalID);
                }
                return mNetSuiteTransferOrder;
            }
            private set
            {
                mNetSuiteTransferOrder = value;
            }
        }

        private NetSuiteLibrary.com.netsuite.webservices.PurchaseOrder mNetSuitePurchaseOrder = null;
        public NetSuiteLibrary.com.netsuite.webservices.PurchaseOrder NetSuitePurchaseOrder
        {
            get
            {
                if (mNetSuitePurchaseOrder == null && mImageSolutionsTransferOrder != null && !string.IsNullOrEmpty(mImageSolutionsTransferOrder.NetSuiteInternalID))
                {
                    mNetSuitePurchaseOrder = LoadNetSuitePurchaseOrder(mImageSolutionsTransferOrder.NetSuiteInternalID);
                }
                return mNetSuitePurchaseOrder;
            }
            private set
            {
                mNetSuitePurchaseOrder = value;
            }
        }

        public TransferOrder(ImageSolutions.TransferOrder.TransferOrder ImageSolutionsTransferOrder)
        {
            mImageSolutionsTransferOrder = ImageSolutionsTransferOrder;
        }

        public TransferOrder(NetSuiteLibrary.com.netsuite.webservices.TransferOrder NetSuiteTransferOrder)
        {
            mNetSuiteTransferOrder = NetSuiteTransferOrder;
        }

        private NetSuiteLibrary.com.netsuite.webservices.TransferOrder LoadNetSuiteTransferOrder(string NetSuiteInternalID)
        {
            RecordRef objSORef = null;
            ReadResponse objReadResult = null;
            NetSuiteLibrary.com.netsuite.webservices.TransferOrder objReturn = null;

            try
            {
                objSORef = new RecordRef();
                objSORef.type = RecordType.transferOrder;
                objSORef.typeSpecified = true;
                objSORef.internalId = NetSuiteInternalID;

                objReadResult = Service.get(objSORef);

                if (objReadResult != null && objReadResult.record != null && (objReadResult.record is NetSuiteLibrary.com.netsuite.webservices.TransferOrder))
                {
                    objReturn = (NetSuiteLibrary.com.netsuite.webservices.TransferOrder)objReadResult.record;
                }
                else
                {
                    throw new Exception("Can not find TransferOrder with Internal ID : " + NetSuiteInternalID);
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

        private NetSuiteLibrary.com.netsuite.webservices.PurchaseOrder LoadNetSuitePurchaseOrder(string NetSuiteInternalID)
        {
            RecordRef objSORef = null;
            ReadResponse objReadResult = null;
            NetSuiteLibrary.com.netsuite.webservices.PurchaseOrder objReturn = null;

            try
            {
                objSORef = new RecordRef();
                objSORef.type = RecordType.purchaseOrder;
                objSORef.typeSpecified = true;
                objSORef.internalId = NetSuiteInternalID;

                objReadResult = Service.get(objSORef);

                if (objReadResult != null && objReadResult.record != null && (objReadResult.record is NetSuiteLibrary.com.netsuite.webservices.PurchaseOrder))
                {
                    objReturn = (NetSuiteLibrary.com.netsuite.webservices.PurchaseOrder)objReadResult.record;
                }
                else
                {
                    throw new Exception("Can not find PurchaseOrder with Internal ID : " + NetSuiteInternalID);
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
            TransferOrder objTransferOrder = null;

            try
            {
                if (ImageSolutionsTransferOrder == null) throw new Exception("ImageSolutionsTransferOrder cannot be null");
                //if (TransferOrder == null) throw new Exception("Sales order is missing");
                //if (NetSuiteTransferOrder != null) throw new Exception("TransferOrder record already exists in NetSuite");
                //if (string.IsNullOrEmpty(ImageSolutionsTransferOrder.TransferOrderID)) throw new Exception("TransferOrderID is missing");
                if (ImageSolutionsTransferOrder.TransferOrderLines == null || ImageSolutionsTransferOrder.TransferOrderLines.Count() == 0) throw new Exception("TransferOrder lines are missing");
                //if (!ImageSolutionsTransferOrder.TransferOrder.IsCompleted) throw new Exception("Sales order is not yet completed, cannot generate TransferOrder");
                //if (TransferOrder.NetSuiteTransferOrder.orderStatus != TransferOrderOrderStatus._pendingBilling) throw new Exception("Unable to create NetSuite TransferOrder, sales order stauts is not 'PendingBilling'");

                objTransferOrder = ObjectAlreadyExists();

                if (objTransferOrder != null)
                {
                    //NetSuite InternalID did not get updated, auto fix
                    ImageSolutionsTransferOrder.NetSuiteInternalID = objTransferOrder.NetSuiteTransferOrder.internalId;
                    ImageSolutionsTransferOrder.NetSuiteDocumentNumber = objTransferOrder.NetSuiteTransferOrder.tranId;
                }
                else
                {
                    objWriteResponse = Service.add(CreateNetSuiteTransferOrder());
                    if (objWriteResponse.status.isSuccess != true)
                    {
                        throw new Exception("Unable to create TransferOrder: " + objWriteResponse.status.statusDetail[0].message);
                    }
                    else
                    {
                        ImageSolutionsTransferOrder.NetSuiteInternalID = ((RecordRef)objWriteResponse.baseRef).internalId;
                        ImageSolutionsTransferOrder.NetSuiteDocumentNumber = NetSuiteTransferOrder.tranId;
                    }
                }

                foreach (NetSuiteLibrary.com.netsuite.webservices.TransferOrderItem objTransferOrderItem in NetSuiteTransferOrder.itemList.item)
                {
                    string strAPIExternalID = NetSuiteLibrary.NetSuiteHelper.GetStringCustomFieldValue(objTransferOrderItem, "custcol_api_external_id");
                    if (string.IsNullOrEmpty(strAPIExternalID) && (objTransferOrderItem.item.internalId == "0"))
                    {
                        //This is item group end of group item, or general discount, ignore
                    }
                    else
                    {
                        if (ImageSolutionsTransferOrder.TransferOrderLines.Exists(m => m.TransferOrderLineID == strAPIExternalID))
                        {
                            ImageSolutionsTransferOrder.TransferOrderLines.Find(m => m.TransferOrderLineID == strAPIExternalID).NetSuiteLineID = objTransferOrderItem.line;
                        }
                        else
                        {
                            ImageSolutionsTransferOrder.NetSuiteInternalID = string.Empty;
                            ImageSolutionsTransferOrder.NetSuiteDocumentNumber = string.Empty;
                            throw new Exception("APIExternalID for TransferOrder line " + strAPIExternalID + " did not get created, not found in NetSuite TransferOrder");
                        }
                    }
                }

                ImageSolutionsTransferOrder.ErrorMessage = string.Empty;
                ImageSolutionsTransferOrder.Update();
            }
            catch (Exception ex)
            {
                ImageSolutionsTransferOrder.ErrorMessage = "TransferOrder.cs - Create() - " + ex.Message;
                ImageSolutionsTransferOrder.Update();
            }
            finally
            {
                objWriteResponse = null;
            }
            return true;
        }

        private NetSuiteLibrary.com.netsuite.webservices.TransferOrder CreateNetSuiteTransferOrder()
        {
            NetSuiteLibrary.com.netsuite.webservices.TransferOrder objReturn = null;
            int intCustomFieldIndex = 0;
            int intCustomFieldLineIndex = 0;
            NetSuiteLibrary.Item.Item objItem = null;
            string strItemType = string.Empty;
            string strItemVendorID = string.Empty;
            int intItemIndex = 0;
            string strBinNumberInternalID = string.Empty;

            try
            {
                //if (ImageSolutionsTransferOrder.TransferOrder == null) throw new Exception("TransferOrder is missing");
                //if (string.IsNullOrEmpty(ImageSolutionsTransferOrder.TransferOrder.NetSuiteInternalID)) throw new Exception("TransferOrder.NetSuiteInternalID is missing");

                objReturn = new NetSuiteLibrary.com.netsuite.webservices.TransferOrder();
                objReturn.internalId = ImageSolutionsTransferOrder.NetSuiteInternalID;
                objReturn.customForm = NetSuiteLibrary.NetSuiteHelper.GetRecordRef(NetSuiteTransferOrderFormID, RecordType.transferOrder);
                
                objReturn.tranDate = ImageSolutionsTransferOrder.CreatedOn;
                objReturn.tranDateSpecified = true;
                objReturn.memo = ImageSolutionsTransferOrder.ShipmentID;
                objReturn.location = NetSuiteHelper.GetRecordRef(ImageSolutionsTransferOrder.FromLocation.NetSuiteInternalID, RecordType.location);
                objReturn.transferLocation = NetSuiteHelper.GetRecordRef(ImageSolutionsTransferOrder.ToLocation.NetSuiteInternalID, RecordType.location);

                objReturn.customFieldList = new CustomFieldRef[99];
                objReturn.customFieldList[intCustomFieldIndex++] = NetSuiteHelper.CreateStringCustomField(ImageSolutionsTransferOrder.TransferOrderID, "custbody_api_external_id");
                
                objReturn.itemList = new TransferOrderItemList();
                objReturn.itemList.item = new TransferOrderItem[ImageSolutionsTransferOrder.TransferOrderLines.Count()];
                
                for (int i = 0; i < ImageSolutionsTransferOrder.TransferOrderLines.Count; i++)
                {
                    intCustomFieldLineIndex = 0;
                    string strTaxRate = string.Empty;

                    objItem = new Item.Item(ImageSolutionsTransferOrder.TransferOrderLines[i].Item);
                    
                    objReturn.itemList.item[intItemIndex] = new TransferOrderItem();

                    if (objItem.NetSuiteInventoryItem != null)
                    {
                        objReturn.itemList.item[intItemIndex].item = NetSuiteLibrary.NetSuiteHelper.GetRecordRef(ImageSolutionsTransferOrder.TransferOrderLines[i].Item.NetSuiteInternalID, RecordType.inventoryItem);
                    }
                    else if (objItem.NetSuiteKitItem != null)
                    {
                        objReturn.itemList.item[intItemIndex].item = NetSuiteLibrary.NetSuiteHelper.GetRecordRef(objItem.NetSuiteKitItem.internalId, RecordType.kitItem);
                    }
                    else
                    {
                        throw new Exception("NetSuite Item not handeled");
                    }
                    objReturn.itemList.item[intItemIndex].quantity = ImageSolutionsTransferOrder.TransferOrderLines[i].Quantity;
                    objReturn.itemList.item[intItemIndex].quantitySpecified = true;

                    objReturn.itemList.item[intItemIndex].customFieldList = new CustomFieldRef[99];
                    objReturn.itemList.item[intItemIndex].customFieldList[intCustomFieldLineIndex++] = NetSuiteHelper.CreateStringCustomField(ImageSolutionsTransferOrder.TransferOrderLines[i].TransferOrderLineID, "custcol_api_external_id");

                    intItemIndex++;
                }
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
                if (ImageSolutionsTransferOrder == null) throw new Exception("ImageSolutionsTransferOrder cannot be null");
                if (NetSuiteTransferOrder == null) throw new Exception("Transfer order record does not exists in NetSuite");
                
                objWriteResult = Service.update(UpdateNetSuiteTransferOrder());

                if (objWriteResult.status.isSuccess != true)
                {
                    throw new Exception("TransferOrder Update() : " + objWriteResult.status.statusDetail[0].message);
                }
                else
                {
                    mNetSuiteTransferOrder = null;
                }
                ImageSolutionsTransferOrder.IsNSUpdated = true;
                ImageSolutionsTransferOrder.ErrorMessage = string.Empty;
                ImageSolutionsTransferOrder.Update();
            }
            catch (Exception ex)
            {
                ImageSolutionsTransferOrder.ErrorMessage = "TransferOrder.cs - Update() - " + ex.Message;
                ImageSolutionsTransferOrder.Update();
            }
            finally
            {
                objWriteResult = null;
            }
            return true;
        }

        private NetSuiteLibrary.com.netsuite.webservices.TransferOrder UpdateNetSuiteTransferOrder()
        {
            NetSuiteLibrary.com.netsuite.webservices.TransferOrder objReturn = null;
            int intCustomFieldIndex = 0;
            string strBinNumberInternalID = string.Empty;

            try
            {
                objReturn = new NetSuiteLibrary.com.netsuite.webservices.TransferOrder();
                objReturn.internalId = ImageSolutionsTransferOrder.NetSuiteInternalID;

                objReturn.customFieldList = new CustomFieldRef[99];
                if (ImageSolutionsTransferOrder.AmazonMWSSyncInboundShipmentPlanLog != null)
                {
                    objReturn.customFieldList[intCustomFieldIndex++] = NetSuiteHelper.CreateBooleanCustomField(true, "custbody_fba_shipment_plan_created");
                    objReturn.customFieldList[intCustomFieldIndex++] = NetSuiteHelper.CreateDateCustomField(ImageSolutionsTransferOrder.AmazonMWSSyncInboundShipmentPlanLog.CreatedOn.AddHours(-7), "custbody_fba_shipment_plan_created_on");
                    
                    if (ImageSolutionsTransferOrder.AmazonMWSSyncInboundShipmentPlanLog.AmazonMWSSyncInboundShipmentLogs != null && !ImageSolutionsTransferOrder.AmazonMWSSyncInboundShipmentPlanLog.AmazonMWSSyncInboundShipmentLogs.Exists(m => string.IsNullOrEmpty(m.CreateInboundShipmentXML)))
                    {
                        objReturn.customFieldList[intCustomFieldIndex++] = NetSuiteHelper.CreateBooleanCustomField(true, "custbody_fba_shipment_created");
                        objReturn.customFieldList[intCustomFieldIndex++] = NetSuiteHelper.CreateDateCustomField(ImageSolutionsTransferOrder.AmazonMWSSyncInboundShipmentPlanLog.AmazonMWSSyncInboundShipmentLogs[0].CreatedOn.AddHours(-7), "custbody_fba_shipment_created_on");
                    }
                }

                objReturn.customFieldList[intCustomFieldIndex++] = NetSuiteHelper.CreateStringCustomField(ImageSolutionsTransferOrder.ErrorMessage, "custbody_fba_sync_error_message");

                if (NetSuiteTransferOrder.status == "Pending Fulfillment")
                {
                    objReturn.itemList = new TransferOrderItemList();
                    objReturn.itemList.item = new TransferOrderItem[ImageSolutionsTransferOrder.TransferOrderLines.Count()];

                    for (int i = 0; i < ImageSolutionsTransferOrder.TransferOrderLines.Count; i++)
                    {
                        bool blnFound = false;

                        if (ImageSolutionsTransferOrder.TransferOrderLines[i].NetSuiteLineID == null) throw new Exception("TransferOrderLine: " + ImageSolutionsTransferOrder.TransferOrderLines[i].TransferOrderLineID + " is missing LineItemID");

                        objReturn.itemList.item[i] = new TransferOrderItem();
                        objReturn.itemList.item[i].line = ImageSolutionsTransferOrder.TransferOrderLines[i].NetSuiteLineID.Value;
                        objReturn.itemList.item[i].lineSpecified = true;

                        foreach (NetSuiteLibrary.com.netsuite.webservices.TransferOrderItem objTransferOrderItem in NetSuiteTransferOrder.itemList.item)
                        {
                            string strAPIExternalID = NetSuiteLibrary.NetSuiteHelper.GetStringCustomFieldValue(objTransferOrderItem, "custcol_api_external_id");
                            if (ImageSolutionsTransferOrder.TransferOrderLines[i].TransferOrderLineID == strAPIExternalID)
                            {
                                objReturn.itemList.item[i].quantity = ImageSolutionsTransferOrder.TransferOrderLines[i].Quantity;
                                objReturn.itemList.item[i].quantitySpecified = true;
                                blnFound = true;
                                break;
                            }
                        }
                        if (!blnFound)
                        {
                            //Do not throw, delete from DB and retry next time
                            ImageSolutionsTransferOrder.TransferOrderLines[i].Delete();
                            objReturn.itemList.item[i] = null;
                            //break;
                            //throw new Exception("TransferOrderLine " + ImageSolutionsTransferOrder.TransferOrderLines[i].TransferOrderLineID + " did not get created, not found in NetSuite transfer order");
                        }
                    }
                }
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

        public bool Update_Test()
        {
            WriteResponse objWriteResult = null;

            try
            {
                //if (ImageSolutionsTransferOrder == null) throw new Exception("ImageSolutionsTransferOrder cannot be null");
                //if (NetSuiteTransferOrder == null) throw new Exception("Transfer order record does not exists in NetSuite");

                objWriteResult = Service.update(UpdateNetSuiteTransferOrder_Test());

                if (objWriteResult.status.isSuccess != true)
                {
                    throw new Exception("TransferOrder Update() : " + objWriteResult.status.statusDetail[0].message);
                }
                else
                {
                    mNetSuiteTransferOrder = null;
                }
                ImageSolutionsTransferOrder.IsNSUpdated = true;
                ImageSolutionsTransferOrder.ErrorMessage = string.Empty;
                ImageSolutionsTransferOrder.Update();
            }
            catch (Exception ex)
            {
                ImageSolutionsTransferOrder.ErrorMessage = "TransferOrder.cs - Update() - " + ex.Message;
                ImageSolutionsTransferOrder.Update();
            }
            finally
            {
                objWriteResult = null;
            }
            return true;
        }

        private NetSuiteLibrary.com.netsuite.webservices.TransferOrder UpdateNetSuiteTransferOrder_Test()
        {
            NetSuiteLibrary.com.netsuite.webservices.TransferOrder objReturn = null;
            string strBinNumberInternalID = string.Empty;

            try
            {
                objReturn = new NetSuiteLibrary.com.netsuite.webservices.TransferOrder();
                objReturn.internalId = "16475991"; //ImageSolutionsTransferOrder.NetSuiteInternalID;
                objReturn.itemList = new TransferOrderItemList();
                objReturn.itemList.replaceAll = false;
                objReturn.itemList.item = new TransferOrderItem[1];
                objReturn.itemList.item[0] = new TransferOrderItem();
                objReturn.itemList.item[0].line = 4;
                objReturn.itemList.item[0].lineSpecified = true;
                objReturn.itemList.item[0].quantity = 1;
                objReturn.itemList.item[0].quantitySpecified = false;
                objReturn.itemList.item[0].rate = 1.58;
                objReturn.itemList.item[0].rateSpecified = true;
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

        public bool UpdateNetSuiteApiExternalID()
        {
            WriteResponse objWriteResult = null;

            try
            {
                if (ImageSolutionsTransferOrder == null) throw new Exception("ImageSolutionsTransferOrder cannot be null");
                if (mNetSuiteTransferOrder == null) throw new Exception("Transfer order record does not exists in NetSuite");

                objWriteResult = Service.update(UpdateNetSuiteAPIExternalID());

                if (objWriteResult.status.isSuccess != true)
                {
                    throw new Exception("TransferOrder Update : transfer order can not be updated " + objWriteResult.status.statusDetail[0].message);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("UpdateNetSuiteApiExternalID() - " + ex.Message);
            }
            finally
            {
                objWriteResult = null;
            }
            return true;
        }

        private NetSuiteLibrary.com.netsuite.webservices.TransferOrder UpdateNetSuiteAPIExternalID()
        {
            if (mNetSuiteTransferOrder == null) throw new Exception("NetSuiteTransferOrder cannot be null");
            if (string.IsNullOrEmpty(mNetSuiteTransferOrder.internalId)) throw new Exception("NetSuiteTransferOrder internalid cannot be null");
            if (ImageSolutionsTransferOrder == null) throw new Exception("ImageSolutionsTransferOrder cannot be null");
            if (string.IsNullOrEmpty(ImageSolutionsTransferOrder.TransferOrderID)) throw new Exception("ImageSolutionsTransferOrder TransferOrderID can not be empty");

            NetSuiteLibrary.com.netsuite.webservices.TransferOrder objNetSuiteTransferOrder = null;

            try
            {
                objNetSuiteTransferOrder = new com.netsuite.webservices.TransferOrder();
                objNetSuiteTransferOrder.customForm = NetSuiteLibrary.NetSuiteHelper.GetRecordRef(NetSuiteTransferOrderFormID, RecordType.transferOrder);
                objNetSuiteTransferOrder.internalId = ImageSolutionsTransferOrder.NetSuiteInternalID;
                objNetSuiteTransferOrder.customFieldList = new CustomFieldRef[1];
                objNetSuiteTransferOrder.customFieldList[0] = NetSuiteHelper.CreateStringCustomField(ImageSolutionsTransferOrder.TransferOrderID, "custbody_api_external_id");

                objNetSuiteTransferOrder.itemList = new TransferOrderItemList();
                objNetSuiteTransferOrder.itemList.item = new TransferOrderItem[ImageSolutionsTransferOrder.TransferOrderLines.Count()];

                for (int i = 0; i < ImageSolutionsTransferOrder.TransferOrderLines.Count; i++)
                {
                    int intCustomFieldLineIndex = 0;

                    if (ImageSolutionsTransferOrder.TransferOrderLines[i].NetSuiteLineID == null) throw new Exception("TransferOrderLine: " + ImageSolutionsTransferOrder.TransferOrderLines[i].TransferOrderLineID + " is missing LineItemID");

                    objNetSuiteTransferOrder.itemList.item[i] = new TransferOrderItem();
                    objNetSuiteTransferOrder.itemList.item[i].line = ImageSolutionsTransferOrder.TransferOrderLines[i].NetSuiteLineID.Value;
                    objNetSuiteTransferOrder.itemList.item[i].lineSpecified = true;

                    objNetSuiteTransferOrder.itemList.item[i].customFieldList = new CustomFieldRef[99];
                    objNetSuiteTransferOrder.itemList.item[i].customFieldList[intCustomFieldLineIndex++] = NetSuiteHelper.CreateStringCustomField(ImageSolutionsTransferOrder.TransferOrderLines[i].TransferOrderLineID, "custcol_api_external_id");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return objNetSuiteTransferOrder;
        }

        public bool Delete()
        {
            RecordRef objPurchaseOrderRef = null;
            WriteResponse objDeleteResponse = null;

            try
            {
                if (ImageSolutionsTransferOrder == null) throw new Exception("ImageSolutionsTransferOrder cannot be null");

                if (NetSuiteTransferOrder != null)
                {
                    objPurchaseOrderRef = new RecordRef();
                    objPurchaseOrderRef.internalId = NetSuiteTransferOrder.internalId;
                    objPurchaseOrderRef.type = RecordType.transferOrder;
                    objPurchaseOrderRef.typeSpecified = true;
                    objDeleteResponse = Service.delete(objPurchaseOrderRef, null);

                    if (objDeleteResponse.status.isSuccess != true)
                    {
                        throw new Exception("Unable to delete TransferOrder: " + objDeleteResponse.status.statusDetail[0].message);
                    }
                    else
                    {
                        mNetSuiteTransferOrder = null;
                    }
                }

                ImageSolutionsTransferOrder.ErrorMessage = string.Empty;
                ImageSolutionsTransferOrder.NetSuiteInternalID = string.Empty;
                ImageSolutionsTransferOrder.NetSuiteDocumentNumber = string.Empty;
                ImageSolutionsTransferOrder.Update();
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("Can not find TransferOrder with Internal ID"))
                {
                    ImageSolutionsTransferOrder.ErrorMessage = string.Empty;
                    ImageSolutionsTransferOrder.NetSuiteInternalID = string.Empty;
                    ImageSolutionsTransferOrder.NetSuiteDocumentNumber = string.Empty;
                    ImageSolutionsTransferOrder.Update();
                }
                else
                {
                    ImageSolutionsTransferOrder.ErrorMessage = "TransferOrder.cs - Delete() - " + ex.Message;
                    ImageSolutionsTransferOrder.Update();
                }
            }
            finally
            {
                objPurchaseOrderRef = null;
                objDeleteResponse = null;
            }
            return true;
        }

        public TransferOrder ObjectAlreadyExists()
        {
            List<TransferOrder> objTransferOrders = null;
            TransferOrderFilter objFilter = null;
            TransferOrder objReturn = null;

            try
            {
                objFilter = new TransferOrderFilter();
                objFilter.APIExternalID = new Database.Filter.StringSearch.SearchFilter();
                objFilter.APIExternalID.SearchString = ImageSolutionsTransferOrder.TransferOrderID;
                objTransferOrders = GetTransferOrders(Service, objFilter);
                if (objTransferOrders != null && objTransferOrders.Count() > 0)
                {
                    if (objTransferOrders.Count > 1) throw new Exception("More than one TransferOrders with API External ID:" + ImageSolutionsTransferOrder.TransferOrderID + " found in Netsuite with InternalIDs " + string.Join(", ", objTransferOrders.Select(m => m.NetSuiteTransferOrder.internalId)));
                    objReturn = objTransferOrders[0];
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                objTransferOrders = null;
                objFilter = null;
            }
            return objReturn;
        }

        private static TransferOrder GetTransferOrder(NetSuiteService Service, TransferOrderFilter Filter)
        {
            List<TransferOrder> objTransferOrders = null;
            TransferOrder objReturn = null;

            try
            {
                objTransferOrders = GetTransferOrders(Service, Filter);
                if (objTransferOrders != null && objTransferOrders.Count >= 1) objReturn = objTransferOrders[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                objTransferOrders = null;
            }
            return objReturn;
        }

        public static List<TransferOrder> GetTransferOrders(TransferOrderFilter Filter)
        {
            return GetTransferOrders(Service, Filter);
        }
        
        private static List<TransferOrder> GetTransferOrders(NetSuiteService Service, TransferOrderFilter Filter)
        {
            List<TransferOrder> objReturn = null;
            SearchResult objSearchResult = null;

            try
            {
                objReturn = new List<TransferOrder>();
                objSearchResult = GetNetSuiteTransactions(Service, Filter);
                if (objSearchResult != null && objSearchResult.totalRecords > 0)
                {
                    do
                    {
                        foreach (object objNetTransferOrder in objSearchResult.recordList)
                        {
                            if (objNetTransferOrder is NetSuiteLibrary.com.netsuite.webservices.TransferOrder)
                            {
                                objReturn.Add(new TransferOrder((NetSuiteLibrary.com.netsuite.webservices.TransferOrder)objNetTransferOrder));
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

        private static SearchResult GetNetSuiteTransactions(NetSuiteService Service, TransferOrderFilter Filter)
        {
            SearchResult objSearchResult = null;
            TransactionSearch objTransacSearch = null;
            SearchPreferences objSearchPreferences = null;
            int intCutstomField = 0;

            try
            {
                objTransacSearch = new TransactionSearch();
                objTransacSearch.basic = new TransactionSearchBasic();
                objTransacSearch.basic.customFieldList = new SearchCustomField[99];

                if (Filter != null)
                {
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
                        objTransacSearch.basic.customFieldList[intCutstomField] = objAPIExternalID;
                        intCutstomField++;
                    }

                    if (Filter.RequestFBAShipment != null)
                    {
                        SearchBooleanCustomField objFulfillmentCompleted = new SearchBooleanCustomField();
                        objFulfillmentCompleted.scriptId = "custbody_request_fba_shipment";
                        objFulfillmentCompleted.searchValue = Filter.RequestFBAShipment.Value;
                        objFulfillmentCompleted.searchValueSpecified = true;
                        objTransacSearch.basic.customFieldList[intCutstomField] = objFulfillmentCompleted;
                        intCutstomField++;
                    }

                    if (!string.IsNullOrEmpty(Filter.Memo))
                    {
                        objTransacSearch.basic.memo = new SearchStringField();
                        objTransacSearch.basic.memo.@operator = SearchStringFieldOperator.@is;
                        objTransacSearch.basic.memo.operatorSpecified = true;
                        objTransacSearch.basic.memo.searchValue = Filter.Memo;
                    }
                }

                objTransacSearch.basic.type = new SearchEnumMultiSelectField();
                objTransacSearch.basic.type.@operator = SearchEnumMultiSelectFieldOperator.anyOf;
                objTransacSearch.basic.type.operatorSpecified = true;
                objTransacSearch.basic.type.searchValue = new string[] { "_transferOrder" };

                objSearchPreferences = new SearchPreferences();
                objSearchPreferences.bodyFieldsOnly = false;
                objSearchPreferences.pageSize = 10;
                objSearchPreferences.pageSizeSpecified = true;

                Service.searchPreferences = objSearchPreferences;
                objSearchResult = Service.search(objTransacSearch);

                if (objSearchResult.status.isSuccess != true)  throw new Exception("Cannot find TransferOrder - " + objSearchResult.status.statusDetail[0].message);
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
    }
}


