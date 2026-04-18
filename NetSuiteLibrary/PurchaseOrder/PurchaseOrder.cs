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

namespace NetSuiteLibrary.PurchaseOrder
{
    public class PurchaseOrder : NetSuiteBase
    {
        private static string NetSuitePurchaseOrderFormID
        {
            get
            {
                if (ConfigurationManager.AppSettings["NetSuitePurchaseOrderFormID"] != null)
                    return ConfigurationManager.AppSettings["NetSuitePurchaseOrderFormID"].ToString();
                else
                    return string.Empty;
            }
        }
        private ImageSolutions.PurchaseOrder.PurchaseOrder mImageSolutionsPurchaseOrder = null;
        public ImageSolutions.PurchaseOrder.PurchaseOrder ImageSolutionsPurchaseOrder
        {
            get
            {
                if (mImageSolutionsPurchaseOrder == null && mNetSuitePurchaseOrder != null && !string.IsNullOrEmpty(mNetSuitePurchaseOrder.internalId))
                {
                    List<ImageSolutions.PurchaseOrder.PurchaseOrder> objPurchaseOrders = null;
                    ImageSolutions.PurchaseOrder.PurchaseOrderFilter objFilter = null;

                    try
                    {
                        objFilter = new ImageSolutions.PurchaseOrder.PurchaseOrderFilter();
                        objFilter.InternalID = new Database.Filter.StringSearch.SearchFilter();
                        objFilter.InternalID.SearchString = mNetSuitePurchaseOrder.internalId;
                        objPurchaseOrders = ImageSolutions.PurchaseOrder.PurchaseOrder.GetPurchaseOrders(objFilter);
                        if (objPurchaseOrders != null && objPurchaseOrders.Count > 0)
                        {
                            mImageSolutionsPurchaseOrder = objPurchaseOrders[0];
                        }
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    finally
                    {
                        objPurchaseOrders = null;
                    }
                }
                return mImageSolutionsPurchaseOrder;
            }
            private set
            {
                mImageSolutionsPurchaseOrder = value;
            }
        }
        private ImageSolutions.TransferOrder.TransferOrder mImageSolutionsTransferOrder = null;
        public ImageSolutions.TransferOrder.TransferOrder ImageSolutionsTransferOrder
        {
            get
            {
                if (mImageSolutionsTransferOrder == null && mNetSuitePurchaseOrder != null && !string.IsNullOrEmpty(mNetSuitePurchaseOrder.internalId))
                {
                    List<ImageSolutions.TransferOrder.TransferOrder> objTransferOrders = null;
                    ImageSolutions.TransferOrder.TransferOrderFilter objFilter = null;

                    try
                    {
                        objFilter = new ImageSolutions.TransferOrder.TransferOrderFilter();
                        objFilter.NetSuiteInternalID = new Database.Filter.StringSearch.SearchFilter();
                        objFilter.NetSuiteInternalID.SearchString = mNetSuitePurchaseOrder.internalId;
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

        private NetSuiteLibrary.com.netsuite.webservices.PurchaseOrder mNetSuitePurchaseOrder = null;
        public NetSuiteLibrary.com.netsuite.webservices.PurchaseOrder NetSuitePurchaseOrder
        {
            get
            {
                if (mNetSuitePurchaseOrder == null || mNetSuitePurchaseOrder.itemList == null)
                {
                    mNetSuitePurchaseOrder = LoadNetSuitePurchaseOrder(mNetSuitePurchaseOrder.internalId);
                }
                return mNetSuitePurchaseOrder;
            }
            private set
            {
                mNetSuitePurchaseOrder = value;
            }
        }

        //public PurchaseOrder(ImageSolutions.TransferOrder.TransferOrder ImageSolutionsTransferOrder)
        //{
        //    mImageSolutionsTransferOrder = ImageSolutionsTransferOrder;
        //}
        public PurchaseOrder(ImageSolutions.PurchaseOrder.PurchaseOrder ImageSolutionsPurchaseOrder)
        {
            mImageSolutionsPurchaseOrder = ImageSolutionsPurchaseOrder;
        }

        public PurchaseOrder(NetSuiteLibrary.com.netsuite.webservices.PurchaseOrder NetSuitePurchaseOrder)
        {
            mNetSuitePurchaseOrder = NetSuitePurchaseOrder;
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
            PurchaseOrder objPurchaseOrder = null;

            try
            {
                if (ImageSolutionsTransferOrder == null) throw new Exception("ImageSolutionsTransferOrder cannot be null");
                //if (PurchaseOrder == null) throw new Exception("Sales order is missing");
                //if (NetSuitePurchaseOrder != null) throw new Exception("PurchaseOrder record already exists in NetSuite");
                //if (string.IsNullOrEmpty(ImageSolutionsPurchaseOrder.PurchaseOrderID)) throw new Exception("PurchaseOrderID is missing");
                if (ImageSolutionsTransferOrder.TransferOrderLines == null || ImageSolutionsTransferOrder.TransferOrderLines.Count() == 0) throw new Exception("Transfer order lines are missing");
                //if (!ImageSolutionsPurchaseOrder.PurchaseOrder.IsCompleted) throw new Exception("Sales order is not yet completed, cannot generate PurchaseOrder");
                //if (PurchaseOrder.NetSuitePurchaseOrder.orderStatus != PurchaseOrderOrderStatus._pendingBilling) throw new Exception("Unable to create NetSuite PurchaseOrder, sales order stauts is not 'PendingBilling'");

                objPurchaseOrder = ObjectAlreadyExists();

                if (objPurchaseOrder != null)
                {
                    //NetSuite InternalID did not get updated, auto fix
                    ImageSolutionsTransferOrder.NetSuiteInternalID = objPurchaseOrder.NetSuitePurchaseOrder.internalId;
                    ImageSolutionsTransferOrder.NetSuiteDocumentNumber = objPurchaseOrder.NetSuitePurchaseOrder.tranId;
                }
                else
                {
                    objWriteResponse = Service.add(CreateNetSuitePurchaseOrder());
                    if (objWriteResponse.status.isSuccess != true)
                    {
                        throw new Exception("Unable to create PurchaseOrder: " + objWriteResponse.status.statusDetail[0].message);
                    }
                    else
                    {
                        ImageSolutionsTransferOrder.NetSuiteInternalID = ((RecordRef)objWriteResponse.baseRef).internalId;
                        ImageSolutionsTransferOrder.NetSuiteDocumentNumber = NetSuitePurchaseOrder.tranId;
                    }
                }

                foreach (NetSuiteLibrary.com.netsuite.webservices.PurchaseOrderItem objPurchaseOrderItem in NetSuitePurchaseOrder.itemList.item)
                {
                    string strAPIExternalID = NetSuiteLibrary.NetSuiteHelper.GetStringCustomFieldValue(objPurchaseOrderItem, "custcol_api_external_id");
                    if (string.IsNullOrEmpty(strAPIExternalID) && (objPurchaseOrderItem.item.internalId == "0"))
                    {
                        //This is item group end of group item, or general discount, ignore
                    }
                    else
                    {
                        if (ImageSolutionsTransferOrder.TransferOrderLines.Exists(m => m.TransferOrderLineID == strAPIExternalID))
                        {
                            ImageSolutionsTransferOrder.TransferOrderLines.Find(m => m.TransferOrderLineID == strAPIExternalID).NetSuiteLineID = objPurchaseOrderItem.line;
                        }
                        else
                        {
                            ImageSolutionsTransferOrder.NetSuiteInternalID = string.Empty;
                            ImageSolutionsTransferOrder.NetSuiteDocumentNumber = string.Empty;
                            throw new Exception("APIExternalID for PurchaseOrder line " + strAPIExternalID + " did not get created, not found in NetSuite PurchaseOrder");
                        }
                    }
                }

                ImageSolutionsTransferOrder.ErrorMessage = string.Empty;
                ImageSolutionsTransferOrder.Update();
            }
            catch (Exception ex)
            {
                ImageSolutionsTransferOrder.ErrorMessage = "PurchaseOrder.cs - Create() - " + ex.Message;
                ImageSolutionsTransferOrder.Update();
            }
            finally
            {
                objWriteResponse = null;
            }
            return true;
        }

        private NetSuiteLibrary.com.netsuite.webservices.PurchaseOrder CreateNetSuitePurchaseOrder()
        {
            NetSuiteLibrary.com.netsuite.webservices.PurchaseOrder objReturn = null;
            int intCustomFieldIndex = 0;
            int intCustomFieldLineIndex = 0;
            NetSuiteLibrary.Item.Item objItem = null;
            string strItemType = string.Empty;
            string strItemVendorID = string.Empty;
            int intItemIndex = 0;
            string strBinNumberInternalID = string.Empty;

            try
            {
                //if (ImageSolutionsPurchaseOrder.PurchaseOrder == null) throw new Exception("PurchaseOrder is missing");
                //if (string.IsNullOrEmpty(ImageSolutionsPurchaseOrder.PurchaseOrder.NetSuiteInternalID)) throw new Exception("PurchaseOrder.NetSuiteInternalID is missing");

                objReturn = new NetSuiteLibrary.com.netsuite.webservices.PurchaseOrder();
                objReturn.internalId = ImageSolutionsTransferOrder.NetSuiteInternalID;
                objReturn.customForm = NetSuiteLibrary.NetSuiteHelper.GetRecordRef(NetSuitePurchaseOrderFormID, RecordType.purchaseOrder);

                objReturn.tranDate = ImageSolutionsTransferOrder.CreatedOn;
                objReturn.tranDateSpecified = true;
                objReturn.memo = ImageSolutionsTransferOrder.ShipmentID;
                //objReturn.location = NetSuiteHelper.GetRecordRef(ImageSolutionsTransferOrder.ToLocation.NetSuiteInternalID, RecordType.location);
                //objReturn.transferLocation = NetSuiteHelper.GetRecordRef(ImageSolutionsTransferOrder.ToLocation.NetSuiteInternalID, RecordType.location);

                objReturn.customFieldList = new CustomFieldRef[99];
                objReturn.customFieldList[intCustomFieldIndex++] = NetSuiteHelper.CreateStringCustomField(ImageSolutionsTransferOrder.TransferOrderID, "custbody_api_external_id");

                objReturn.itemList = new PurchaseOrderItemList();
                objReturn.itemList.item = new PurchaseOrderItem[ImageSolutionsTransferOrder.TransferOrderLines.Count()];

                for (int i = 0; i < ImageSolutionsTransferOrder.TransferOrderLines.Count; i++)
                {
                    intCustomFieldLineIndex = 0;
                    string strTaxRate = string.Empty;

                    objItem = new Item.Item(ImageSolutionsTransferOrder.TransferOrderLines[i].Item);

                    objReturn.itemList.item[intItemIndex] = new PurchaseOrderItem();

                    if (objItem.NetSuiteInventoryItem != null)
                    {
                        objReturn.itemList.item[intItemIndex].item = NetSuiteLibrary.NetSuiteHelper.GetRecordRef(ImageSolutionsTransferOrder.TransferOrderLines[i].Item.InternalID, RecordType.inventoryItem);
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
                if (ImageSolutionsPurchaseOrder == null) throw new Exception("ImageSolutionsPurchaseOrder cannot be null");
                if (NetSuitePurchaseOrder == null) throw new Exception("Purchase order record does not exists in NetSuite");

                objWriteResult = Service.update(UpdateNetSuitePurchaseOrder());

                if (objWriteResult.status.isSuccess != true)
                {
                    throw new Exception("PurchaseOrder Update() : " + objWriteResult.status.statusDetail[0].message);
                }
                else
                {
                    mNetSuitePurchaseOrder = null;
                }

                ImageSolutionsPurchaseOrder.ErrorMessage = string.Empty;
                ImageSolutionsPurchaseOrder.Update();
            }
            catch (Exception ex)
            {
                ImageSolutionsPurchaseOrder.ErrorMessage = "PurchaseOrder.cs - Update() - " + ex.Message;
                ImageSolutionsPurchaseOrder.Update();
            }
            finally
            {
                objWriteResult = null;
            }
            return true;
        }

        private NetSuiteLibrary.com.netsuite.webservices.PurchaseOrder UpdateNetSuitePurchaseOrder()
        {
            NetSuiteLibrary.com.netsuite.webservices.PurchaseOrder objReturn = null;
            int intCustomFieldIndex = 0;
            string strBinNumberInternalID = string.Empty;

            try
            {
                objReturn = new NetSuiteLibrary.com.netsuite.webservices.PurchaseOrder();
                objReturn.internalId = ImageSolutionsPurchaseOrder.InternalID;

                objReturn.customFieldList = new CustomFieldRef[99];

                if (ImageSolutionsPurchaseOrder != null)
                {
                    objReturn.customFieldList[intCustomFieldIndex++] = NetSuiteHelper.CreateBooleanCustomField(true, "custbody_is_sent_to_vendor");
                    objReturn.customFieldList[intCustomFieldIndex++] = NetSuiteHelper.CreateSelectCustomField(ImageSolutionsPurchaseOrder.ErrorMessage, "custbody_error_message");
                }


                //if (NetSuitePurchaseOrder.status == "Pending Receipt")
                //{
                //    objReturn.itemList = new PurchaseOrderItemList();
                //    objReturn.itemList.item = new PurchaseOrderItem[ImageSolutionsTransferOrder.TransferOrderLines.Count()];
                //    objReturn.itemList.replaceAll = false;

                //for (int i = 0; i < ImageSolutionsTransferOrder.TransferOrderLines.Count; i++)
                //{
                //    bool blnFound = false;

                //    if (ImageSolutionsTransferOrder.TransferOrderLines[i].NetSuiteLineID == null) throw new Exception("TransferOrderLine: " + ImageSolutionsTransferOrder.TransferOrderLines[i].TransferOrderLineID + " is missing LineItemID");

                //    objReturn.itemList.item[i] = new PurchaseOrderItem();
                //    objReturn.itemList.item[i].line = ImageSolutionsTransferOrder.TransferOrderLines[i].NetSuiteLineID.Value;
                //    objReturn.itemList.item[i].lineSpecified = true;

                //    foreach (NetSuiteLibrary.com.netsuite.webservices.PurchaseOrderItem objPurchaseOrderItem in NetSuitePurchaseOrder.itemList.item)
                //    {
                //        string strAPIExternalID = NetSuiteLibrary.NetSuiteHelper.GetStringCustomFieldValue(objPurchaseOrderItem, "custcol_api_external_id");
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
                //        //throw new Exception("PurchaseOrderLine " + ImageSolutionsPurchaseOrder.PurchaseOrderLines[i].PurchaseOrderLineID + " did not get created, not found in NetSuite transfer order");
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

        public bool Close()
        {
            bool ret = true;

            WriteResponse objWriteResult = null;

            try
            {
                //if (ImageSolutionsTransferOrder == null) throw new Exception("ImageSolutionsPurchaseOrder cannot be null");
                if (NetSuitePurchaseOrder == null) throw new Exception("Purchase order record does not exists in NetSuite");

                if(NetSuitePurchaseOrder.status == "Fully Billed" || NetSuitePurchaseOrder.status == "Closed" || NetSuitePurchaseOrder.status == "Pending Bill")
                {
                    ret = false;

                    Console.WriteLine(string.Format("NetSuite Purchase Order {0} already closed or billed", NetSuitePurchaseOrder.tranId));

                    return ret;
                }

                if (NetSuitePurchaseOrder.shipTo.name.Contains("Raising"))
                {
                    throw new Exception("Failing PO Closure for Raising cane's");
                }

                objWriteResult = Service.update(CloseNetSuitePurchaseOrder());
                
                if (objWriteResult.status.isSuccess != true)
                {
                    throw new Exception("PurchaseOrder Close() : " + objWriteResult.status.statusDetail[0].message);
                }
                else
                {
                    mNetSuitePurchaseOrder = null;
                }
                //ImageSolutionsTransferOrder.IsNSUpdated = true;
                //ImageSolutionsTransferOrder.ErrorMessage = string.Empty;
                //ImageSolutionsTransferOrder.Update();
            }
            catch (Exception ex)
            {
                ret = false;

                if(ex.Message.Contains("Raising"))
                {
                    throw new Exception(string.Format("{0}", ex.Message));
                }
                //ImageSolutionsTransferOrder.ErrorMessage = "PurchaseOrder.cs - Close() - " + ex.Message;
                //ImageSolutionsTransferOrder.Update();
            }
            finally
            {
                objWriteResult = null;
            }
            return ret;
        }

        public NetSuiteLibrary.com.netsuite.webservices.PurchaseOrder CloseNetSuitePurchaseOrder()
        {
            NetSuiteLibrary.com.netsuite.webservices.PurchaseOrder objReturn = null;
            int intCustomFieldIndex = 0;
            string strBinNumberInternalID = string.Empty;

            try
            {
                objReturn = new NetSuiteLibrary.com.netsuite.webservices.PurchaseOrder();
                objReturn.internalId = NetSuitePurchaseOrder.internalId;
                objReturn.itemList = NetSuitePurchaseOrder.itemList;

                foreach (NetSuiteLibrary.com.netsuite.webservices.PurchaseOrderItem objPurchaseOrderItem in objReturn.itemList.item)
                {
                    //if (objPurchaseOrderItem.quantityReceived == 0)
                    //{
                    objPurchaseOrderItem.isClosed = true;
                    objPurchaseOrderItem.isClosedSpecified = true;
                    //}

                    objPurchaseOrderItem.quantityAvailableSpecified = false;
                    objPurchaseOrderItem.quantityBilledSpecified = false;
                    objPurchaseOrderItem.quantityOnHandSpecified = false;
                    objPurchaseOrderItem.quantityOnShipmentsSpecified = false;
                    objPurchaseOrderItem.quantityReceivedSpecified = false;
                    objPurchaseOrderItem.amountSpecified = false;
                    objPurchaseOrderItem.billVarianceStatusSpecified = false;
                    objPurchaseOrderItem.expectedReceiptDateSpecified = false;
                    objPurchaseOrderItem.grossAmtSpecified = false;
                    objPurchaseOrderItem.isBillableSpecified = false;
                    objPurchaseOrderItem.matchBillToReceiptSpecified = false;
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

        public bool UpdateNetSuiteApiExternalID()
        {
            WriteResponse objWriteResult = null;

            try
            {
                if (ImageSolutionsTransferOrder == null) throw new Exception("ImageSolutionsTransferOrder cannot be null");
                if (mNetSuitePurchaseOrder == null) throw new Exception("Purchase order record does not exists in NetSuite");

                objWriteResult = Service.update(UpdateNetSuiteAPIExternalID());

                if (objWriteResult.status.isSuccess != true)
                {
                    throw new Exception("PurchaseOrder Update : purchase order can not be updated " + objWriteResult.status.statusDetail[0].message);
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

        private NetSuiteLibrary.com.netsuite.webservices.PurchaseOrder UpdateNetSuiteAPIExternalID()
        {
            if (mNetSuitePurchaseOrder == null) throw new Exception("NetSuitePurchaseOrder cannot be null");
            if (string.IsNullOrEmpty(mNetSuitePurchaseOrder.internalId)) throw new Exception("NetSuitePurchaseOrder internalid cannot be null");
            if (ImageSolutionsTransferOrder == null) throw new Exception("ImageSolutionsTransferOrder cannot be null");
            if (string.IsNullOrEmpty(ImageSolutionsTransferOrder.TransferOrderID)) throw new Exception("ImageSolutionsTransferOrder TransferOrderID can not be empty");

            NetSuiteLibrary.com.netsuite.webservices.PurchaseOrder objNetSuitePurchaseOrder = null;

            try
            {
                objNetSuitePurchaseOrder = new com.netsuite.webservices.PurchaseOrder();
                objNetSuitePurchaseOrder.customForm = NetSuiteLibrary.NetSuiteHelper.GetRecordRef(NetSuitePurchaseOrderFormID, RecordType.purchaseOrder);
                objNetSuitePurchaseOrder.internalId = ImageSolutionsTransferOrder.NetSuiteInternalID;
                objNetSuitePurchaseOrder.customFieldList = new CustomFieldRef[1];
                objNetSuitePurchaseOrder.customFieldList[0] = NetSuiteHelper.CreateStringCustomField(ImageSolutionsTransferOrder.TransferOrderID, "custbody_api_external_id");

                objNetSuitePurchaseOrder.itemList = new PurchaseOrderItemList();
                objNetSuitePurchaseOrder.itemList.item = new PurchaseOrderItem[ImageSolutionsTransferOrder.TransferOrderLines.Count()];

                for (int i = 0; i < ImageSolutionsTransferOrder.TransferOrderLines.Count; i++)
                {
                    int intCustomFieldLineIndex = 0;

                    if (ImageSolutionsTransferOrder.TransferOrderLines[i].NetSuiteLineID == null) throw new Exception("TransferOrderLine: " + ImageSolutionsTransferOrder.TransferOrderLines[i].TransferOrderLineID + " is missing LineItemID");

                    objNetSuitePurchaseOrder.itemList.item[i] = new PurchaseOrderItem();
                    objNetSuitePurchaseOrder.itemList.item[i].line = ImageSolutionsTransferOrder.TransferOrderLines[i].NetSuiteLineID.Value;
                    objNetSuitePurchaseOrder.itemList.item[i].lineSpecified = true;

                    objNetSuitePurchaseOrder.itemList.item[i].customFieldList = new CustomFieldRef[99];
                    objNetSuitePurchaseOrder.itemList.item[i].customFieldList[intCustomFieldLineIndex++] = NetSuiteHelper.CreateStringCustomField(ImageSolutionsTransferOrder.TransferOrderLines[i].TransferOrderLineID, "custcol_api_external_id");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return objNetSuitePurchaseOrder;
        }

        public bool Delete()
        {
            RecordRef objPurchaseOrderRef = null;
            WriteResponse objDeleteResponse = null;

            try
            {
                if (ImageSolutionsTransferOrder == null) throw new Exception("ImageSolutionsTransferOrder cannot be null");

                if (NetSuitePurchaseOrder != null)
                {
                    objPurchaseOrderRef = new RecordRef();
                    objPurchaseOrderRef.internalId = NetSuitePurchaseOrder.internalId;
                    objPurchaseOrderRef.type = RecordType.purchaseOrder;
                    objPurchaseOrderRef.typeSpecified = true;
                    objDeleteResponse = Service.delete(objPurchaseOrderRef, null);

                    if (objDeleteResponse.status.isSuccess != true)
                    {
                        throw new Exception("Unable to delete PurchaseOrder: " + objDeleteResponse.status.statusDetail[0].message);
                    }
                    else
                    {
                        mNetSuitePurchaseOrder = null;
                    }
                }

                ImageSolutionsTransferOrder.ErrorMessage = string.Empty;
                ImageSolutionsTransferOrder.NetSuiteInternalID = string.Empty;
                ImageSolutionsTransferOrder.NetSuiteDocumentNumber = string.Empty;
                ImageSolutionsTransferOrder.Update();
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("Can not find PurchaseOrder with Internal ID"))
                {
                    ImageSolutionsTransferOrder.ErrorMessage = string.Empty;
                    ImageSolutionsTransferOrder.NetSuiteInternalID = string.Empty;
                    ImageSolutionsTransferOrder.NetSuiteDocumentNumber = string.Empty;
                    ImageSolutionsTransferOrder.Update();
                }
                else
                {
                    ImageSolutionsTransferOrder.ErrorMessage = "PurchaseOrder.cs - Delete() - " + ex.Message;
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

        public PurchaseOrder ObjectAlreadyExists()
        {
            List<PurchaseOrder> objPurchaseOrders = null;
            PurchaseOrderFilter objFilter = null;
            PurchaseOrder objReturn = null;

            try
            {
                objFilter = new PurchaseOrderFilter();
                objFilter.APIExternalID = new Database.Filter.StringSearch.SearchFilter();
                objFilter.APIExternalID.SearchString = ImageSolutionsTransferOrder.TransferOrderID;
                objPurchaseOrders = GetPurchaseOrders(Service, objFilter);
                if (objPurchaseOrders != null && objPurchaseOrders.Count() > 0)
                {
                    if (objPurchaseOrders.Count > 1) throw new Exception("More than one PurchaseOrders with API External ID:" + ImageSolutionsTransferOrder.TransferOrderID + " found in Netsuite with InternalIDs " + string.Join(", ", objPurchaseOrders.Select(m => m.NetSuitePurchaseOrder.internalId)));
                    objReturn = objPurchaseOrders[0];
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                objPurchaseOrders = null;
                objFilter = null;
            }
            return objReturn;
        }

        public static PurchaseOrder GetPurchaseOrder(NetSuiteService Service, PurchaseOrderFilter Filter)
        {
            List<PurchaseOrder> objPurchaseOrders = null;
            PurchaseOrder objReturn = null;

            try
            {
                objPurchaseOrders = GetPurchaseOrders(Service, Filter);
                if (objPurchaseOrders != null && objPurchaseOrders.Count >= 1) objReturn = objPurchaseOrders[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                objPurchaseOrders = null;
            }
            return objReturn;
        }

        public List<PurchaseOrder> GetPurchaseOrders(PurchaseOrderFilter Filter)
        {
            return GetPurchaseOrders(Service, Filter);
        }

        public static List<PurchaseOrder> GetPurchaseOrders(NetSuiteService Service, PurchaseOrderFilter Filter)
        {
            List<PurchaseOrder> objReturn = null;
            SearchResult objSearchResult = null;

            try
            {
                objReturn = new List<PurchaseOrder>();
                objSearchResult = GetNetSuiteTransactions(Service, Filter);
                if (objSearchResult != null && objSearchResult.totalRecords > 0)
                {
                    do
                    {
                        foreach (object objNetPurchaseOrder in objSearchResult.recordList)
                        {
                            if (objNetPurchaseOrder is NetSuiteLibrary.com.netsuite.webservices.PurchaseOrder)
                            {
                                objReturn.Add(new PurchaseOrder((NetSuiteLibrary.com.netsuite.webservices.PurchaseOrder)objNetPurchaseOrder));
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

        private static SearchResult GetNetSuiteTransactions(NetSuiteService Service, PurchaseOrderFilter Filter)
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
                    if (Filter.InternalIDs != null)
                    {
                        objTransacSearch.basic.internalId = NetSuiteHelper.LoadMultiSearchField(Filter.InternalIDs);
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
                objTransacSearch.basic.type.searchValue = new string[] { "_purchaseOrder" };

                //objSearchPreferences = new SearchPreferences();
                //objSearchPreferences.bodyFieldsOnly = false;
                //objSearchPreferences.pageSize = 10;
                //objSearchPreferences.pageSizeSpecified = true;

                Service.searchPreferences = objSearchPreferences;
                objSearchResult = Service.search(objTransacSearch);

                if (objSearchResult.status.isSuccess != true) throw new Exception("Cannot find PurchaseOrder - " + objSearchResult.status.statusDetail[0].message);
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

        public static List<string> GetPurchaseOrderSavedSearch(NetSuiteService Service, string savedsearchid)
        {
            List<string> objReturn = null;
            SearchResult objSearchResult = null;
            Dictionary<string, string> dicInternalIds = null;

            try
            {
                objReturn = new List<string>();
                dicInternalIds = new Dictionary<string, string>();

                objSearchResult = GetPurchaseOrderSavedSearchTransaction(Service, savedsearchid);
                if (objSearchResult != null && objSearchResult.totalRecords > 0)
                {
                    do
                    {
                        foreach (TransactionSearchRow objNetPurchaseOrder in objSearchResult.searchRowList)
                        {
                            if (objNetPurchaseOrder is NetSuiteLibrary.com.netsuite.webservices.TransactionSearchRow && !dicInternalIds.ContainsKey(objNetPurchaseOrder.basic.internalId[0].searchValue.internalId))
                            {
                                dicInternalIds.Add(objNetPurchaseOrder.basic.internalId[0].searchValue.internalId, objNetPurchaseOrder.basic.internalId[0].searchValue.internalId);
                                objReturn.Add(objNetPurchaseOrder.basic.internalId[0].searchValue.internalId);
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

        public static SearchResult GetPurchaseOrderSavedSearchTransaction(NetSuiteService Service, string savedsearchid)
        {
            TransactionSearchAdvanced objTransactionSearchAdvanced = null;
            SearchResult objSearchResult = null;
            TransactionSearch objTransacSearch = null;
            SearchPreferences objSearchPreferences = null;
            int intCutstomField = 0;

            try
            {
                objTransactionSearchAdvanced = new TransactionSearchAdvanced();
                objTransactionSearchAdvanced.savedSearchId = savedsearchid;

                Service.searchPreferences = objSearchPreferences;
                objSearchResult = Service.search(objTransactionSearchAdvanced);

                if (objSearchResult.status.isSuccess != true) throw new Exception(string.Format("Cannot find PurchaseOrder Saved Search - {0}: {1}", savedsearchid, objSearchResult.status.statusDetail[0].message));
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


