using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetSuiteLibrary;
using NetSuiteLibrary.com.netsuite.webservices;

namespace ScheduledTaskProcess.Task
{
    public class GetSandSActiveWearPO : NetSuiteBase
    {
        public void Execute()
        {
            NetSuiteLibrary.PurchaseOrder.PurchaseOrderFilter NetSuitePurchaseOrderFilter = null;
            List<NetSuiteLibrary.PurchaseOrder.PurchaseOrder> NetSuitePurchaseOrders = null;
            ImageSolutions.PurchaseOrder.PurchaseOrder ISPurchaseOrder = null;
            ImageSolutions.Vendor.VendorFilter ISVendorFilter = null;
            ImageSolutions.Vendor.Vendor ISVendor = null;
            SqlConnection objConn = null;
            SqlTransaction objTran = null;

            try
            {
                NetSuitePurchaseOrderFilter = new NetSuiteLibrary.PurchaseOrder.PurchaseOrderFilter();

                NetSuitePurchaseOrderFilter.InternalIDs = new List<string>();
                NetSuitePurchaseOrderFilter.InternalIDs.Add("36226898");


                //staging
                //NetSuitePurchaseOrderFilter.InternalIDs = NetSuiteLibrary.PurchaseOrder.PurchaseOrder.GetPurchaseOrderSavedSearch(Service, "23254");

                //production
                //NetSuitePurchaseOrderFilter.InternalIDs = NetSuiteLibrary.PurchaseOrder.PurchaseOrder.GetPurchaseOrderSavedSearch(Service, "23254");

                NetSuitePurchaseOrders = NetSuiteLibrary.PurchaseOrder.PurchaseOrder.GetPurchaseOrders(Service, NetSuitePurchaseOrderFilter);

                if (NetSuitePurchaseOrders != null)
                {
                    if (NetSuitePurchaseOrders != null && NetSuitePurchaseOrders.Count > 0)
                    {
                        //foreach (NetSuiteLibrary.PurchaseOrder.PurchaseOrder _purchaseorder in NetSuitePurchaseOrders)
                        //{ 
                        //}

                        if (NetSuitePurchaseOrders[0].ImageSolutionsPurchaseOrder == null && NetSuitePurchaseOrders[0].NetSuitePurchaseOrder != null)
                        {
                            if(NetSuiteHelper.GetBoolCustomFieldValue(NetSuitePurchaseOrders[0].NetSuitePurchaseOrder, "custbody_is_sent_to_vendor"))
                            {
                                throw new Exception("PO already sent to Vendor");
                            }

                            NetSuiteLibrary.com.netsuite.webservices.PurchaseOrder NSPurchaseOrder = NetSuitePurchaseOrders[0].NetSuitePurchaseOrder;

                            objConn = new SqlConnection(Database.DefaultConnectionString);
                            objConn.Open();
                            objTran = objConn.BeginTransaction();


                            ImageSolutions.Address.AddressTrans address = new ImageSolutions.Address.AddressTrans();

                            if (NSPurchaseOrder.shippingAddress != null)
                            {
                                address.FirstName = NSPurchaseOrder.shippingAddress.addressee;
                                address.AddressLine1 = string.Format("{0}{1}", NSPurchaseOrder.shippingAddress.addr1
                                    , string.IsNullOrEmpty(NSPurchaseOrder.shippingAddress.addr2) ? "" : string.Format(" {0}", NSPurchaseOrder.shippingAddress.addr2));
                                address.City = NSPurchaseOrder.shippingAddress.city;
                                address.State = NSPurchaseOrder.shippingAddress.state;
                                address.PostalCode = NSPurchaseOrder.shippingAddress.zip;
                                //address.IsResidential = NetSuiteHelper.GetBoolCustomFieldValue(NSPurchaseOrder, "custbody_residential_shipping_address");
                                address.Create(objConn, objTran);
                            }

                            ISPurchaseOrder = new ImageSolutions.PurchaseOrder.PurchaseOrder();
                            ISPurchaseOrder.PurchaseOrderNumber = NSPurchaseOrder.tranId;

                            if(NSPurchaseOrder.entity == null)
                            {
                                throw new Exception("NetSuite Purchase Order Invalid Entity");
                            }

                            ISVendorFilter = new ImageSolutions.Vendor.VendorFilter();
                            ISVendorFilter.InternalID = new Database.Filter.StringSearch.SearchFilter();
                            ISVendorFilter.InternalID.SearchString = NSPurchaseOrder.entity.internalId;
                            ISVendor = ImageSolutions.Vendor.Vendor.GetVendor(ISVendorFilter);

                            if(ISVendor == null)
                            {
                                throw new Exception(string.Format("Unable to Identify Vendor with InternalID {0}", NSPurchaseOrder.entity.internalId));
                            }

                            ISPurchaseOrder.VendorID = ISVendor.VendorID;

                            ISPurchaseOrder.TransactionDate = NSPurchaseOrder.tranDate;
                            ISPurchaseOrder.InternalID = NSPurchaseOrder.internalId;
                            ISPurchaseOrder.ShipToAddressID = address != null ? address.AddressTransID : string.Empty;

                            if (NSPurchaseOrder.shipMethod != null)
                            {
                                ISPurchaseOrder.ShippingMethod = NSPurchaseOrder.shipMethod.name;
                            }

                            ISPurchaseOrder.PurchaseOrderLines = new List<ImageSolutions.PurchaseOrder.PurchaseOrderLine>();

                            foreach (PurchaseOrderItem _item in NSPurchaseOrder.itemList.item)
                            {
                                ImageSolutions.PurchaseOrder.PurchaseOrderLine poline = new ImageSolutions.PurchaseOrder.PurchaseOrderLine();
                                poline.PurchaseOrderID = ISPurchaseOrder.PurchaseOrderID;

                                ImageSolutions.Item.ItemFilter ISItemFilter = new ImageSolutions.Item.ItemFilter();
                                ISItemFilter.InternalID = new Database.Filter.StringSearch.SearchFilter();
                                ISItemFilter.InternalID.SearchString = _item.item.internalId;

                                ImageSolutions.Item.Item ISItem = ImageSolutions.Item.Item.GetItem(ISItemFilter);
                                
                                if(ISItem == null)
                                {
                                    throw new Exception(string.Format("Invalid Item in database {0}", _item.item.internalId));
                                }

                                poline.ItemID = ISItem.ItemID;
                                poline.Quantity = Convert.ToInt32(_item.quantity);
                                poline.LineID = _item.line;

                                ISPurchaseOrder.PurchaseOrderLines.Add(poline);
                            }

                            ISPurchaseOrder.Create(objConn, objTran);

                            objTran.Commit();
                        }
                        else
                        {
                            //Purchase Order already exists in DB
                        }


                        NetSuitePurchaseOrders[0].Update();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                NetSuitePurchaseOrderFilter = null;
                NetSuitePurchaseOrders = null;
                ISPurchaseOrder = null;
                ISVendorFilter = null;
                ISVendor = null;
                objConn = null;
                objTran = null;
            }
        }
    }
}
