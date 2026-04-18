using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActiveWearIntegration.Task
{
    public class SyncFulfillment
    {
        public static bool Start()
        {
            ActiveWearAPI.Request request = new ActiveWearAPI.Request();
            request.GET_Orders();
            return true;
        }

        public static bool ExecuteByShipDate()
        {
            ImageSolutions.PurchaseOrder.PurchaseOrderFilter objFilter = null;
            ImageSolutions.PurchaseOrder.PurchaseOrder ISPurchaseOrder = null;
            ImageSolutions.Fulfillment.FulfillmentFilter objFulfillmentFilter = null;
            ImageSolutions.Fulfillment.Fulfillment ISFulfillment = null;
            SqlConnection objConn = null;
            SqlTransaction objTran = null;

            try
            {
                ActiveWearAPI.Request request = new ActiveWearAPI.Request();
                List<ActiveWearAPI.OrderHistory.Header> orders = request.GetOrdersByShipDate(DateTime.UtcNow);

                foreach (ActiveWearAPI.OrderHistory.Header _order in orders)
                {
                    try
                    {
                        objFilter = new ImageSolutions.PurchaseOrder.PurchaseOrderFilter();
                        objFilter.PurchaseOrderNumber = new Database.Filter.StringSearch.SearchFilter();
                        objFilter.PurchaseOrderNumber.SearchString = _order.poNumber;

                        ISPurchaseOrder = ImageSolutions.PurchaseOrder.PurchaseOrder.GetPurchaseOrder(objFilter);

                        if (ISPurchaseOrder == null)
                        {
                            throw new Exception("Purchase Order not in database");
                        }
                        if(string.IsNullOrEmpty(_order.trackingNumber))
                        {
                            throw new Exception("No tracking number from Vendor");
                        }

                        objFulfillmentFilter = new ImageSolutions.Fulfillment.FulfillmentFilter();
                        objFulfillmentFilter.TrackingNumber = new Database.Filter.StringSearch.SearchFilter();
                        objFulfillmentFilter.TrackingNumber.SearchString = _order.trackingNumber;
                       
                        ISFulfillment = ImageSolutions.Fulfillment.Fulfillment.GetFulfillment(objFulfillmentFilter);

                        if (ISFulfillment == null)
                        {
                            objConn = new SqlConnection(Database.DefaultConnectionString);
                            objConn.Open();
                            objTran = objConn.BeginTransaction();

                            ISFulfillment = new ImageSolutions.Fulfillment.Fulfillment();
                            ISFulfillment.TrackingNumber = _order.trackingNumber;
                            ISFulfillment.PurchaseOrderID = ISPurchaseOrder.PurchaseOrderID;
                            ISFulfillment.ShipDate = _order.shipDate;

                            ISFulfillment.FulfillmentLines = new List<ImageSolutions.Fulfillment.FulfillmentLine>();

                            //foreach (ActiveWearAPI.OrderHistory.Header.Box _box in _order.Boxes)
                            //{
                            //    ImageSolutions.Fulfillment.FulfillmentLine ISFulfillmentLine = new ImageSolutions.Fulfillment.FulfillmentLine();

                            //    ISFulfillmentLine.PurchaseOrderLineID = ISPurchaseOrder.PurchaseOrderLines.Find(m => m.Item.ExternalID == _box.lines.Find(b=>b.sku == m.Item.ExternalID).sku).PurchaseOrderLineID;
                            //    ISFulfillmentLine.ItemID = ISPurchaseOrder.PurchaseOrderLines.Find(m => m.Item.ExternalID == _box.lines.Find(b => b.sku == m.Item.ExternalID).sku).ItemID;
                            //    ISFulfillmentLine.Quantity = ISPurchaseOrder.PurchaseOrderLines.Find(m => m.Item.ExternalID == _box.lines.Find(b => b.sku == m.Item.ExternalID).sku).Quantity;
                            //    ISFulfillment.FulfillmentLines.Add(ISFulfillmentLine);
                            //}

                            ISFulfillment.Create(objConn, objTran);
                            objTran.Commit();
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.Write(string.Format("{0}", ex.Message));

                        if (ISPurchaseOrder != null)
                        {
                            ISPurchaseOrder.ErrorMessage = ex.Message;
                            ISPurchaseOrder.Update();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.Write(string.Format("{0}", ex.Message));
            }
            finally
            {
                objFilter = null;
                ISPurchaseOrder = null;
            }

            return true;
        }

        public static bool ExecuteByPONumber()
        {
            ImageSolutions.PurchaseOrder.PurchaseOrderFilter objFilter = null;
            ImageSolutions.PurchaseOrder.PurchaseOrder ISPurchaseOrder = null;
            ImageSolutions.Fulfillment.FulfillmentFilter objFulfillmentFilter = null;
            ImageSolutions.Fulfillment.Fulfillment ISFulfillment = null;
            SqlConnection objConn = null;
            SqlTransaction objTran = null;

            try
            {
                string strPONumber = "1786309";

                ActiveWearAPI.Request request = new ActiveWearAPI.Request();
                List<ActiveWearAPI.OrderHistory.Header> orders = request.GetOrdersByPoNumber(strPONumber, false);

                foreach (ActiveWearAPI.OrderHistory.Header _order in orders)
                {
                    try
                    {
                        objFilter = new ImageSolutions.PurchaseOrder.PurchaseOrderFilter();
                        objFilter.PurchaseOrderNumber = new Database.Filter.StringSearch.SearchFilter();
                        objFilter.PurchaseOrderNumber.SearchString = _order.poNumber;

                        ISPurchaseOrder = ImageSolutions.PurchaseOrder.PurchaseOrder.GetPurchaseOrder(objFilter);

                        if (ISPurchaseOrder == null)
                        {
                            throw new Exception("Purchase Order not in database");
                        }
                        if (string.IsNullOrEmpty(_order.trackingNumber))
                        {
                            throw new Exception("No tracking number from Vendor");
                        }

                        objFulfillmentFilter = new ImageSolutions.Fulfillment.FulfillmentFilter();
                        objFulfillmentFilter.TrackingNumber = new Database.Filter.StringSearch.SearchFilter();
                        objFulfillmentFilter.TrackingNumber.SearchString = _order.trackingNumber;

                        ISFulfillment = ImageSolutions.Fulfillment.Fulfillment.GetFulfillment(objFulfillmentFilter);

                        if (ISFulfillment == null)
                        {
                            objConn = new SqlConnection(Database.DefaultConnectionString);
                            objConn.Open();
                            objTran = objConn.BeginTransaction();

                            ISFulfillment = new ImageSolutions.Fulfillment.Fulfillment();
                            ISFulfillment.TrackingNumber = _order.trackingNumber;
                            ISFulfillment.PurchaseOrderID = ISPurchaseOrder.PurchaseOrderID;
                            ISFulfillment.ShipDate = _order.shipDate;

                            ISFulfillment.FulfillmentLines = new List<ImageSolutions.Fulfillment.FulfillmentLine>();

                            //    foreach (ActiveWearAPI.OrderHistory.Header.Box _box in _order.Boxes)
                            //    {
                            //        if (string.IsNullOrEmpty(_box.trackingNumber) || _box.trackingNumber == ISFulfillment.TrackingNumber)
                            //        {
                            //            foreach (ActiveWearAPI.OrderHistory.Header.Line _line in _box.lines)
                            //            {
                            //                ImageSolutions.Fulfillment.FulfillmentLine ISFulfillmentLine  = new ImageSolutions.Fulfillment.FulfillmentLine();

                            //                string ItemID = ISPurchaseOrder.PurchaseOrderLines.Find(m => m.Item.ExternalID == _line.sku).ItemID;

                            //                ISFulfillmentLine = ISFulfillment.FulfillmentLines.Find(m => m.ItemID == ItemID);

                            //                if(ISFulfillmentLine == null)
                            //                {
                            //                    ISFulfillmentLine = new ImageSolutions.Fulfillment.FulfillmentLine();
                            //                    ISFulfillmentLine.ItemID = ItemID;
                            //                    ISFulfillmentLine.PurchaseOrderLineID = ISPurchaseOrder.PurchaseOrderLines.Find(m => m.Item.ExternalID == _line.sku).PurchaseOrderLineID;
                            //                    ISFulfillmentLine.Quantity = Convert.ToDouble(_line.qtyShipped);

                            //                    //ISFulfillmentLine.Quantity = ISPurchaseOrder.PurchaseOrderLines.Find(m => m.Item.ExternalID == _line.sku).Quantity;
                            //                    ISFulfillment.FulfillmentLines.Add(ISFulfillmentLine);
                            //                }
                            //                else
                            //                {
                            //                    ISFulfillmentLine.Quantity += Convert.ToDouble(_line.qtyShipped);
                            //                }
                            //            }
                            //        }
                            //        else
                            //        {
                            //            throw new Exception("Needs multiple fulfillments, has multiple tracking number");
                            //        }
                            //    }

                            //    ISFulfillment.Create(objConn, objTran);
                            //    objTran.Commit();
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.Write(string.Format("{0}", ex.Message));

                        if (ISPurchaseOrder != null)
                        {
                            ISPurchaseOrder.ErrorMessage = ex.Message;
                            ISPurchaseOrder.Update();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.Write(string.Format("{0}", ex.Message));
            }
            finally
            {
                objFilter = null;
                ISPurchaseOrder = null;
            }

            return true;
        }
    }
}
