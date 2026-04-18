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

namespace NetSuiteLibrary.ItemReceipt
{
    public class ItemReceipt : NetSuiteBase
    {

        public NetSuiteLibrary.com.netsuite.webservices.InboundShipment LoadNetSuiteInboundShipment()
        {
            RecordRef objSORef = null;
            ReadResponse objReadResult = null;
            NetSuiteLibrary.com.netsuite.webservices.InboundShipment objReturn = null;

            try
            {
                objSORef = new RecordRef();
                objSORef.type = RecordType.itemReceipt;
                objSORef.typeSpecified = true;
                objSORef.internalId = "2";

                objReadResult = Service.get(objSORef);

                if (objReadResult != null && objReadResult.record != null && (objReadResult.record is NetSuiteLibrary.com.netsuite.webservices.InboundShipment))
                {
                    objReturn = (NetSuiteLibrary.com.netsuite.webservices.InboundShipment)objReadResult.record;
                }
                else
                {
                    throw new Exception("Can not find InboundShipment with Internal ID : " );
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

        public bool CreateItemReceipt()
        {

            NetSuiteLibrary.com.netsuite.webservices.ItemReceipt objReturn = null;
            WriteResponse objWriteResult = null;

            try
            {

                //NetSuiteLibrary.com.netsuite.webservices.InboundShipment objInboundShipment = new InboundShipment();
                objReturn = new com.netsuite.webservices.ItemReceipt();

                //objReturn.createdFrom = NetSuiteHelper.GetRecordRef("2181", RecordType.inboundShipment);

                objReturn.inboundShipment = NetSuiteHelper.GetRecordRef("2", RecordType.inboundShipment);
                objReturn.createdFrom = NetSuiteHelper.GetRecordRef("21176142", RecordType.purchaseOrder);

                //objReturn.itemList = new ItemReceiptItemList();
                //objReturn.itemList.item = new ItemReceiptItem[1];
                //objReturn.itemList.replaceAll = false;

                //objReturn.itemList.item[0] = new ItemReceiptItem();
                //objReturn.itemList.item[0].quantity = 1;
                //objReturn.itemList.item[0].quantitySpecified = true;
                //objReturn.itemList.item[0].orderLine = 0;
                //objReturn.itemList.item[0].orderLineSpecified = true;
                //objReturn.itemList.item[0].itemReceive = true;
                //objReturn.itemList.item[0].itemReceiveSpecified = true;
                objWriteResult = Service.add(objReturn);

                if (objWriteResult.status.isSuccess != true)
                {
                    throw new Exception("Unable to create item receipt : " + objWriteResult.status.statusDetail[0].message);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                objWriteResult = null;
            }
            return true;
        }


        private static string NetSuiteItemReceiptFormID
        {
            get
            {
                if (ConfigurationManager.AppSettings["NetSuiteItemReceiptFormID"] != null)
                    return ConfigurationManager.AppSettings["NetSuiteItemReceiptFormID"].ToString();
                else
                    return string.Empty;
            }
        }

        private ImageSolutions.ItemReceipt.ItemReceipt mImageSolutionsItemReceipt = null;
        public ImageSolutions.ItemReceipt.ItemReceipt ImageSolutionsItemReceipt
        {
            get
            {
                if (mImageSolutionsItemReceipt == null && mNetSuiteItemReceipt != null && !string.IsNullOrEmpty(mNetSuiteItemReceipt.internalId))
                {
                    List<ImageSolutions.ItemReceipt.ItemReceipt> objItemReceipts = null;
                    ImageSolutions.ItemReceipt.ItemReceiptFilter objFilter = null;

                    try
                    {
                        objFilter = new ImageSolutions.ItemReceipt.ItemReceiptFilter();
                        //objFilter.NetSuiteInternalID = new Database.Filter.StringSearch.SearchFilter();
                        //objFilter.NetSuiteInternalID.SearchString = mNetSuiteItemReceipt.internalId;
                        objItemReceipts = ImageSolutions.ItemReceipt.ItemReceipt.GetItemReceipts(objFilter);
                        if (objItemReceipts != null && objItemReceipts.Count > 0)
                        {
                            mImageSolutionsItemReceipt = objItemReceipts[0];
                        }
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    finally
                    {
                        objItemReceipts = null;
                    }
                }
                return mImageSolutionsItemReceipt;
            }
            private set
            {
                mImageSolutionsItemReceipt = value;
            }
        }

        private NetSuiteLibrary.com.netsuite.webservices.ItemReceipt mNetSuiteItemReceipt = null;
        public NetSuiteLibrary.com.netsuite.webservices.ItemReceipt NetSuiteItemReceipt
        {
            get
            {
                if (mNetSuiteItemReceipt == null && mImageSolutionsItemReceipt != null && !string.IsNullOrEmpty(mImageSolutionsItemReceipt.InternalID))
                {
                    mNetSuiteItemReceipt = LoadNetSuiteItemReceipt(mImageSolutionsItemReceipt.InternalID);
                }
                return mNetSuiteItemReceipt;
            }
            private set
            {
                mNetSuiteItemReceipt = value;
            }
        }
        public ItemReceipt()
        {
        }
        public ItemReceipt(ImageSolutions.ItemReceipt.ItemReceipt ImageSolutionsItemReceipt)
        {
            mImageSolutionsItemReceipt = ImageSolutionsItemReceipt;
        }

        public ItemReceipt(NetSuiteLibrary.com.netsuite.webservices.ItemReceipt NetSuiteItemReceipt)
        {
            mNetSuiteItemReceipt = NetSuiteItemReceipt;
        }

        private NetSuiteLibrary.com.netsuite.webservices.ItemReceipt LoadNetSuiteItemReceipt(string NetSuiteInternalID)
        {
            RecordRef objSORef = null;
            ReadResponse objReadResult = null;
            NetSuiteLibrary.com.netsuite.webservices.ItemReceipt objReturn = null;

            try
            {
                objSORef = new RecordRef();
                objSORef.type = RecordType.itemReceipt;
                objSORef.typeSpecified = true;
                objSORef.internalId = NetSuiteInternalID;

                objReadResult = Service.get(objSORef);

                if (objReadResult != null && objReadResult.record != null && (objReadResult.record is NetSuiteLibrary.com.netsuite.webservices.ItemReceipt))
                {
                    objReturn = (NetSuiteLibrary.com.netsuite.webservices.ItemReceipt)objReadResult.record;
                }
                else
                {
                    throw new Exception("Can not find ItemReceipt with Internal ID : " + NetSuiteInternalID);
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

        //public bool Create()
        //{
        //    WriteResponse objWriteResponse = null;
        //    ItemReceipt objItemReceipt = null;

        //    try
        //    {
        //        if (ImageSolutionsItemReceipt == null) throw new Exception("ImageSolutionsItemReceipt cannot be null");
        //        //if (NetSuiteItemReceipt != null) throw new Exception("ItemReceipt record already exists in NetSuite");
        //        if (string.IsNullOrEmpty(ImageSolutionsItemReceipt.TransferOrderID)) throw new Exception("TransferOrderID is missing");
        //        if (string.IsNullOrEmpty(ImageSolutionsItemReceipt.TransferOrder.NetSuiteInternalID)) throw new Exception("Transfer order has not yet been created in NetSuite");
        //        if (ImageSolutionsItemReceipt.ItemReceiptLines == null || ImageSolutionsItemReceipt.ItemReceiptLines.Count() == 0) throw new Exception("Item receipt lines is missing");
        //        foreach (ImageSolutions.ItemReceipt.ItemReceiptLine objItemReceiptLine in ImageSolutionsItemReceipt.ItemReceiptLines)
        //        {
        //            //PurchaseOrderline fulfilled qty (ItemReceipt created in netsuite)
        //            int intTotalReceivedQuantity = objItemReceiptLine.TransferOrderLine.ItemReceiptLines.FindAll(m => !string.IsNullOrEmpty(m.ItemReceipt.NetSuiteInternalID)).Sum(n => n.Quantity);
        //            int intTotalQuantity = objItemReceiptLine.TransferOrderLine.Quantity;

        //            if (intTotalReceivedQuantity >= intTotalQuantity)
        //            {
        //                throw new Exception("TransferOrderLineID: " + objItemReceiptLine.TransferOrderLine.TransferOrderLineID + " has already been fully received");
        //            }
        //        }

        //        objItemReceipt = ObjectAlreadyExists();
        //        if (objItemReceipt != null)
        //        {
        //            //NetSuite InternalID did not get updated, auto fix
        //            ImageSolutionsItemReceipt.NetSuiteInternalID = objItemReceipt.NetSuiteItemReceipt.internalId;
        //            ImageSolutionsItemReceipt.NetSuiteDocumentNumber = objItemReceipt.NetSuiteItemReceipt.tranId;
        //        }
        //        else
        //        {
        //            objWriteResponse = Service.add(CreateNetSuiteItemReceipt());
        //            if (objWriteResponse.status.isSuccess != true)
        //            {
        //                throw new Exception("Unable to create ItemReceipt: " + objWriteResponse.status.statusDetail[0].message);
        //            }
        //            else
        //            {
        //                ImageSolutionsItemReceipt.NetSuiteInternalID = ((RecordRef)objWriteResponse.baseRef).internalId;
        //                ImageSolutionsItemReceipt.NetSuiteDocumentNumber = NetSuiteItemReceipt.tranId;
        //                ImageSolutionsItemReceipt.IsNSUpdated = true;
        //            }
        //        }

        //        foreach (NetSuiteLibrary.com.netsuite.webservices.ItemReceiptItem objItemReceiptItem in NetSuiteItemReceipt.itemList.item)
        //        {
        //            string strAPIExternalID = NetSuiteLibrary.NetSuiteHelper.GetStringCustomFieldValue(objItemReceiptItem, "custcol_api_external_id");
        //            if (ImageSolutionsItemReceipt.ItemReceiptLines.Exists(m => m.ItemReceiptLineID == strAPIExternalID))
        //            {
        //                ImageSolutionsItemReceipt.ItemReceiptLines.Find(m => m.ItemReceiptLineID == strAPIExternalID).NetSuiteLineID = objItemReceiptItem.line;
        //            }
        //        }
        //        ImageSolutionsItemReceipt.ErrorMessage = string.Empty;
        //        ImageSolutionsItemReceipt.Update();
        //    }
        //    catch (Exception ex)
        //    {
        //        ImageSolutionsItemReceipt.ErrorMessage = "ItemReceipt.cs - Create() - " + ex.Message;
        //        ImageSolutionsItemReceipt.Update();
        //    }
        //    finally
        //    {
        //        objWriteResponse = null;

        //    }
        //    return true;
        //}

        //public bool Update()
        //{
        //    WriteResponse objWriteResponse = null;

        //    try
        //    {
        //        if (ImageSolutionsItemReceipt == null) throw new Exception("ImageSolutionsItemReceipt cannot be null");
        //        if (NetSuiteItemReceipt == null) throw new Exception("ItemReceipt record does not exists in NetSuite");

        //        objWriteResponse = Service.update(UpdateNetSuiteItemReceipt());
        //        if (objWriteResponse.status.isSuccess != true)
        //        {
        //            throw new Exception("Unable to create ItemReceipt: " + objWriteResponse.status.statusDetail[0].message);
        //        }
        //        else
        //        {
        //            mNetSuiteItemReceipt = null;
        //            ImageSolutionsItemReceipt.IsNSUpdated = true;
        //        }

        //        ImageSolutionsItemReceipt.ErrorMessage = string.Empty;
        //        ImageSolutionsItemReceipt.Update();
        //    }
        //    catch (Exception ex)
        //    {
        //        ImageSolutionsItemReceipt.ErrorMessage = "ItemReceipt.cs - Update() - " + ex.Message;
        //        ImageSolutionsItemReceipt.Update();
        //    }
        //    finally
        //    {
        //        objWriteResponse = null;
        //    }
        //    return true;
        //}

        //public bool Delete()
        //{
        //    RecordRef objItemReceiptRef = null;
        //    WriteResponse objDeleteResponse = null;

        //    try
        //    {
        //        if (ImageSolutionsItemReceipt == null) throw new Exception("ImageSolutionsItemReceipt cannot be null");
        //        if (ImageSolutionsItemReceipt.IsShipped) throw new Exception("Unable to delete ItemReceipt with shipped status, please make sure to void tracking number, delete invoice...etc");
        //        if (ImageSolutionsItemReceipt.Invoice != null) throw new Exception("Unabel to delete ItemReceipt, invoice has already been created");
        //        if (PurchaseOrder.NetSuitePurchaseOrder == null) throw new Exception("Unable to load NetSuite sales order");

        //        if (ImageSolutionsItemReceipt.PurchaseOrder.IsCompleted && ImageSolutionsItemReceipt.IsCancelled)
        //        {
        //            //closed sales order commcerhub PurchaseOrder qty is completed (either shipped or cancelled)
        //            PurchaseOrder.Update();
        //            PurchaseOrder = null;
        //        }

        //        if (NetSuiteItemReceipt != null)
        //        {
        //            objItemReceiptRef = new RecordRef();
        //            objItemReceiptRef.internalId = NetSuiteItemReceipt.internalId;
        //            objItemReceiptRef.type = RecordType.itemItemReceipt;
        //            objItemReceiptRef.typeSpecified = true;
        //            objDeleteResponse = Service.delete(objItemReceiptRef);

        //            if (objDeleteResponse.status.isSuccess != true)
        //            {
        //                throw new Exception("Unable to delete ItemReceipt: " + objDeleteResponse.status.statusDetail[0].message);
        //            }
        //            else
        //            {
        //                mNetSuiteItemReceipt = null;
        //            }
        //        }

        //        ImageSolutionsItemReceipt.ErrorMessage = string.Empty;
        //        ImageSolutionsItemReceipt.NetSuiteInternalID = string.Empty;
        //        ImageSolutionsItemReceipt.NetSuiteItemReceiptNumber = string.Empty;
        //        ImageSolutionsItemReceipt.Update();
        //    }
        //    catch (Exception ex)
        //    {
        //        if (ex.Message.Contains("Can not find ItemReceipt with Internal ID"))
        //        {
        //            ImageSolutionsItemReceipt.ErrorMessage = string.Empty;
        //            ImageSolutionsItemReceipt.NetSuiteInternalID = string.Empty;
        //            ImageSolutionsItemReceipt.NetSuiteItemReceiptNumber = string.Empty;
        //            ImageSolutionsItemReceipt.Update();
        //        }
        //        else
        //        {
        //            ImageSolutionsItemReceipt.ErrorMessage = "ItemReceipt.cs - Delete() - " + ex.Message;
        //            ImageSolutionsItemReceipt.Update();
        //        }
        //    }
        //    finally
        //    {
        //        objItemReceiptRef = null;
        //        objDeleteResponse = null;
        //    }
        //    return true;
        //}

        public ItemReceipt ObjectAlreadyExists()
        {
            List<ItemReceipt> objItemReceipts = null;
            ItemReceiptFilter objFilter = null;
            ItemReceipt objReturn = null;

            try
            {
                objFilter = new ItemReceiptFilter();
                objFilter.APIExternalID = ImageSolutionsItemReceipt.ItemReceiptID;

                //objFilter.CustomerInternalIDs = new List<string>();
                //objFilter.CustomerInternalIDs.Add(ImageSolutionsItemReceipt.PurchaseOrder.Customer.NetSuiteInternalID);

                //objFilter.PurchaseOrderInternalID = ImageSolutionsItemReceipt.PurchaseOrder.NetSuiteInternalID;
                //objFilter.PurchaseOrderInternalIDOperator = SearchMultiSelectFieldOperator.anyOf;

                //objFilter.PONumber = ImageSolutionsItemReceipt.PONumber;
                //objFilter.PONumberOperator = SearchTextNumberFieldOperator.equalTo;
                //objFilter.CustomerInternalIDs = new List<string>();
                //objFilter.CustomerInternalIDs.Add(ImageSolutionsItemReceipt.Retailer.NetSuiteCustomerInternalID);

                objItemReceipts = GetItemReceipts(Service, objFilter);
                if (objItemReceipts != null && objItemReceipts.Count() > 0)
                {
                    if (objItemReceipts.Count > 1) throw new Exception("More than one ItemReceipts with API External ID:" + ImageSolutionsItemReceipt.ItemReceiptID + " found in Netsuite with InternalIDs " + string.Join(", ", objItemReceipts.Select(m => m.NetSuiteItemReceipt.internalId)));
                    objReturn = objItemReceipts[0];
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                objItemReceipts = null;
                objFilter = null;
            }
            return objReturn;
        }

        //private NetSuiteLibrary.com.netsuite.webservices.ItemReceipt CreateNetSuiteItemReceipt()
        //{
        //    NetSuiteLibrary.com.netsuite.webservices.ItemReceipt objReturn = null;
        //    int intCustomFieldIndex = 0;
        //    int intCustomFieldLineIndex = 0;
        //    NetSuiteLibrary.TransferOrder.TransferOrder objNSTransferOrder = null;
        //    ImageSolutions.ItemReceipt.ItemReceiptLine objItemReceiptLine = null;

        //    try
        //    {
        //        if (ImageSolutionsItemReceipt.TransferOrder == null) throw new Exception("TransferOrder is missing");
        //        if (string.IsNullOrEmpty(ImageSolutionsItemReceipt.TransferOrder.NetSuiteInternalID)) throw new Exception("TransferOrder.NetSuiteInternalID is missing");
        //        //if (ImageSolutionsItemReceipt.ShippingMethod == null) throw new Exception("Shipping method is missing");
        //        //if (ImageSolutionsItemReceipt.TransactionDate == null) throw new Exception("TransactionDate is missing");
        //        if (ImageSolutionsItemReceipt.ItemReceiptLines == null || ImageSolutionsItemReceipt.ItemReceiptLines.Count == 0) throw new Exception("ItemReceipt lines is missing");
               
        //        objReturn = new NetSuiteLibrary.com.netsuite.webservices.ItemReceipt();
        //        //objReturn.internalId = ImageSolutionsItemReceipt.NetSuiteInternalID;
        //        objReturn.customForm = NetSuiteLibrary.NetSuiteHelper.GetRecordRef(NetSuiteItemReceiptFormID, RecordType.itemReceipt);
        //        objReturn.createdFrom = new com.netsuite.webservices.RecordRef();
        //        objReturn.createdFrom.internalId = ImageSolutionsItemReceipt.TransferOrder.NetSuiteInternalID;
        //        objReturn.createdFrom.type = RecordType.transferOrder;
        //        objReturn.createdFrom.typeSpecified = true;

        //        objReturn.tranDate = ImageSolutionsItemReceipt.CreatedOn;
        //        objReturn.tranDateSpecified = true;

        //        objReturn.memo = ImageSolutionsItemReceipt.AmazonMWSSyncInboundShipmentLog.ShipmentID;

        //        objReturn.customFieldList = new CustomFieldRef[99];
        //        objReturn.customFieldList[intCustomFieldIndex++] = NetSuiteHelper.CreateStringCustomField(ImageSolutionsItemReceipt.ItemReceiptID, "custbody_api_external_id");

        //        objNSTransferOrder = new TransferOrder.TransferOrder(ImageSolutionsItemReceipt.TransferOrder);

        //        objReturn.itemList = new ItemReceiptItemList();
        //        objReturn.itemList.item = new ItemReceiptItem[objNSTransferOrder.NetSuiteTransferOrder.itemList.item.Count()];
        //        objReturn.itemList.replaceAll = false;

        //        if (ImageSolutionsItemReceipt.TransferOrder.TransferOrderLines.Count != objNSTransferOrder.NetSuiteTransferOrder.itemList.item.Count())
        //        {
        //            throw new Exception("Transfer order lines got edited, database has " + ImageSolutionsItemReceipt.TransferOrder.TransferOrderLines.Count + " lines, NetSuite has " + objNSTransferOrder.NetSuiteTransferOrder.itemList.item.Count() + " lines");
        //        }

        //        for (int i = 0; i < objNSTransferOrder.NetSuiteTransferOrder.itemList.item.Count(); i++)
        //        {
        //            intCustomFieldLineIndex = 0;
                    
        //            if (objNSTransferOrder.NetSuiteTransferOrder.itemList.item[i].quantityReceived < objNSTransferOrder.NetSuiteTransferOrder.itemList.item[i].quantity)
        //            {
        //                objItemReceiptLine = ImageSolutionsItemReceipt.ItemReceiptLines.Find(m => m.TransferOrderLine.NetSuiteLineID.Value == objNSTransferOrder.NetSuiteTransferOrder.itemList.item[i].line);
                        
        //                if (objItemReceiptLine != null)
        //                {
        //                    objReturn.itemList.item[i] = new com.netsuite.webservices.ItemReceiptItem();
        //                    objReturn.itemList.item[i].itemReceive = true; //ImageSolutionsItemReceipt.ItemReceiptLines[i].TransferOrderLine.NetSuiteLineID.Value;
        //                    objReturn.itemList.item[i].itemReceiveSpecified = true;
        //                    objReturn.itemList.item[i].item = NetSuiteLibrary.NetSuiteHelper.GetRecordRef(objItemReceiptLine.TransferOrderLine.Item.NetSuiteInternalID, RecordType.inventoryItem);
        //                    objReturn.itemList.item[i].quantity = objItemReceiptLine.Quantity;
        //                    objReturn.itemList.item[i].quantitySpecified = true;
        //                    //https://netsuite.custhelp.com/app/answers/detail/a_id/44188/kw/suitetalk%20item%20receipt%20transfer%20order
        //                    objReturn.itemList.item[i].orderLine = objItemReceiptLine.TransferOrderLine.NetSuiteLineID.Value + 2;
        //                    objReturn.itemList.item[i].orderLineSpecified = true;

        //                    objReturn.itemList.item[i].customFieldList = new CustomFieldRef[99];
        //                    objReturn.itemList.item[i].customFieldList[intCustomFieldLineIndex++] = NetSuiteHelper.CreateStringCustomField(objItemReceiptLine.ItemReceiptLineID, "custcol_api_external_id");
        //                }
        //                else
        //                {
        //                    objReturn.itemList.item[i] = new com.netsuite.webservices.ItemReceiptItem();
        //                    objReturn.itemList.item[i].itemReceive = true; //ImageSolutionsItemReceipt.ItemReceiptLines[i].TransferOrderLine.NetSuiteLineID.Value;
        //                    objReturn.itemList.item[i].itemReceiveSpecified = true;
        //                    objReturn.itemList.item[i].item = NetSuiteLibrary.NetSuiteHelper.GetRecordRef(objNSTransferOrder.NetSuiteTransferOrder.itemList.item[i].item.internalId, RecordType.inventoryItem);
        //                    objReturn.itemList.item[i].quantity = 0;
        //                    objReturn.itemList.item[i].quantitySpecified = true;
        //                    //https://netsuite.custhelp.com/app/answers/detail/a_id/44188/kw/suitetalk%20item%20receipt%20transfer%20order
        //                    objReturn.itemList.item[i].orderLine = objNSTransferOrder.NetSuiteTransferOrder.itemList.item[i].line + 2;
        //                    objReturn.itemList.item[i].orderLineSpecified = true;

        //                    objReturn.itemList.item[i].customFieldList = new CustomFieldRef[99];
        //                    objReturn.itemList.item[i].customFieldList[intCustomFieldLineIndex++] = NetSuiteHelper.CreateStringCustomField(string.Empty, "custcol_api_external_id");
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //    finally
        //    {
        //        objItemReceiptLine = null;
        //    }
        //    return objReturn;
        //}

        //private NetSuiteLibrary.com.netsuite.webservices.ItemReceipt UpdateNetSuiteItemReceipt()
        //{
        //    NetSuiteLibrary.com.netsuite.webservices.ItemReceipt objReturn = null;
        //    int intCustomFieldIndex = 0;
        //    NetSuiteLibrary.TransferOrder.TransferOrder objNSTransferOrder = null;
        //    ImageSolutions.ItemReceipt.ItemReceiptLine objItemReceiptLine = null;

        //    try
        //    {
        //        if (ImageSolutionsItemReceipt.TransferOrder == null) throw new Exception("TransferOrder is missing");
        //        if (string.IsNullOrEmpty(ImageSolutionsItemReceipt.TransferOrder.NetSuiteInternalID)) throw new Exception("TransferOrder.NetSuiteInternalID is missing");
        //        //if (ImageSolutionsItemReceipt.ShippingMethod == null) throw new Exception("Shipping method is missing");
        //        //if (ImageSolutionsItemReceipt.TransactionDate == null) throw new Exception("TransactionDate is missing");
        //        if (ImageSolutionsItemReceipt.ItemReceiptLines == null || ImageSolutionsItemReceipt.ItemReceiptLines.Count == 0) throw new Exception("ItemReceipt lines is missing");

        //        objReturn = new NetSuiteLibrary.com.netsuite.webservices.ItemReceipt();
        //        objReturn.internalId = ImageSolutionsItemReceipt.NetSuiteInternalID;
        //        objReturn.customForm = NetSuiteLibrary.NetSuiteHelper.GetRecordRef(NetSuiteItemReceiptFormID, RecordType.itemReceipt);
        //        objReturn.createdFrom = new com.netsuite.webservices.RecordRef();
        //        objReturn.createdFrom.internalId = ImageSolutionsItemReceipt.TransferOrder.NetSuiteInternalID;
        //        objReturn.createdFrom.type = RecordType.transferOrder;
        //        objReturn.createdFrom.typeSpecified = true;

        //        objReturn.tranDate = ImageSolutionsItemReceipt.CreatedOn;
        //        objReturn.tranDateSpecified = true;

        //        objReturn.memo = ImageSolutionsItemReceipt.AmazonMWSSyncInboundShipmentLog.ShipmentID;

        //        objReturn.customFieldList = new CustomFieldRef[99];
        //        objReturn.customFieldList[intCustomFieldIndex++] = NetSuiteHelper.CreateStringCustomField(ImageSolutionsItemReceipt.ItemReceiptID, "custbody_api_external_id");

        //        objNSTransferOrder = new TransferOrder.TransferOrder(ImageSolutionsItemReceipt.TransferOrder);

        //        objReturn.itemList = new ItemReceiptItemList();
        //        objReturn.itemList.item = new ItemReceiptItem[ImageSolutionsItemReceipt.ItemReceiptLines.Count];
        //        objReturn.itemList.replaceAll = true;

        //        //if (ImageSolutionsItemReceipt.TransferOrder.TransferOrderLines.Count != objNSTransferOrder.NetSuiteTransferOrder.itemList.item.Count())
        //        //{
        //        //    throw new Exception("Transfer order lines got edited, database has " + ImageSolutionsItemReceipt.TransferOrder.TransferOrderLines.Count + " lines, NetSuite has " + objNSTransferOrder.NetSuiteTransferOrder.itemList.item.Count() + " lines");
        //        //}

        //        for (int i = 0; i < ImageSolutionsItemReceipt.ItemReceiptLines.Count; i++)
        //        {
        //            objItemReceiptLine = ImageSolutionsItemReceipt.ItemReceiptLines[i];

        //            objReturn.itemList.item[i] = new com.netsuite.webservices.ItemReceiptItem();
        //            objReturn.itemList.item[i].item = NetSuiteLibrary.NetSuiteHelper.GetRecordRef(objItemReceiptLine.TransferOrderLine.Item.NetSuiteInternalID, RecordType.inventoryItem);
        //            objReturn.itemList.item[i].quantity = objItemReceiptLine.Quantity;
        //            objReturn.itemList.item[i].quantitySpecified = true;
        //            objReturn.itemList.item[i].line = objItemReceiptLine.NetSuiteLineID.Value;
        //            objReturn.itemList.item[i].lineSpecified = true;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //    finally
        //    {
        //        objItemReceiptLine = null;
        //    }
        //    return objReturn;
        //}

        public List<ItemReceipt> GetItemReceipts(ItemReceiptFilter Filter)
        {
            return GetItemReceipts(Service, Filter);
        }

        public static ItemReceipt GetItemReceipt(NetSuiteService Service, ItemReceiptFilter Filter)
        {
            List<ItemReceipt> objItemReceipts = null;
            ItemReceipt objReturn = null;

            try
            {
                objItemReceipts = GetItemReceipts(Service, Filter);
                if (objItemReceipts != null && objItemReceipts.Count >= 1) objReturn = objItemReceipts[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                objItemReceipts = null;
            }
            return objReturn;
        }

        public static List<ItemReceipt> GetItemReceipts(NetSuiteService Service, ItemReceiptFilter Filter)
        {
            List<ItemReceipt> objReturn = null;
            SearchResult objSearchResult = null;

            try
            {
                objReturn = new List<ItemReceipt>();
                objSearchResult = GetNetSuiteTransactions(Service, Filter);
                if (objSearchResult != null && objSearchResult.totalRecords > 0)
                {
                    do
                    {
                        foreach (object objNetItemReceipt in objSearchResult.recordList)
                        {
                            if (objNetItemReceipt is NetSuiteLibrary.com.netsuite.webservices.ItemReceipt)
                            {
                                objReturn.Add(new ItemReceipt((NetSuiteLibrary.com.netsuite.webservices.ItemReceipt)objNetItemReceipt));
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

        private static SearchResult GetNetSuiteTransactions(NetSuiteService Service, ItemReceiptFilter Filter)
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
                    if (Filter.InternalIDs != null)
                    {
                        objTransacSearch.basic.internalId = NetSuiteHelper.LoadMultiSearchField(Filter.InternalIDs);
                    }
                    if (Filter.LastModified != null)
                    {
                        objTransacSearch.basic.lastModifiedDate = Filter.LastModified;
                    }

                    //objTransacSearch.itemJoin = new com.netsuite.webservices.ItemSearchBasic();
                    //objTransacSearch.itemJoin...line

                    if (!string.IsNullOrEmpty(Filter.APIExternalID))
                    {
                        objTransacSearch.basic.customFieldList = new SearchCustomField[1];

                        SearchStringCustomField objAPIExternalID = new SearchStringCustomField();
                        objAPIExternalID.scriptId = "custbody_api_external_id";
                        objAPIExternalID.searchValue = Filter.APIExternalID;
                        objAPIExternalID.@operator = SearchStringFieldOperator.@is;
                        objAPIExternalID.operatorSpecified = true;
                        objTransacSearch.basic.customFieldList[0] = objAPIExternalID;
                    }
                }

                objTransacSearch.basic.type = new SearchEnumMultiSelectField();
                objTransacSearch.basic.type.@operator = SearchEnumMultiSelectFieldOperator.anyOf;
                objTransacSearch.basic.type.operatorSpecified = true;
                objTransacSearch.basic.type.searchValue = new string[] { "_itemReceipt" };

                objSearchPreferences = new SearchPreferences();
                objSearchPreferences.bodyFieldsOnly = false;
                objSearchPreferences.pageSize = 10;
                objSearchPreferences.pageSizeSpecified = true;

                Service.searchPreferences = objSearchPreferences;
                objSearchResult = Service.search(objTransacSearch);

                if (objSearchResult.status.isSuccess != true)  throw new Exception("Cannot find ItemReceipt - " + objSearchResult.status.statusDetail[0].message);
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

        public List<DeletedRecord> GetDeletedItemReceiptAfter(DateTime Date)
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
                objFilter.type.searchValue = new string[] { NetSuiteHelper.DeletedRecordType.itemReceipt.ToString() };
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


