using System.Net;
using System.IO;
using System.Web.Script.Serialization;
using System.Collections.Generic;
using System;

namespace ActiveWearIntegration
{
    public class ActiveWearAPI
    {
        public class Request
        {
            public Request()
            {
                Method = "";
                Host = "https://api.ssactivewear.com";
                URLPath = "";
                CustomerNumber = "42972";
                APIKey = "bcaafe01-c522-46a6-b741-2340212914c3";
                ContentType = "application/json";

            }

            public string Method { get; set; }
            public string Host { get; set; }
            public string URLPath { get; set; }
            public string CustomerNumber { get; set; }
            public string APIKey { get; set; }
            public string ContentType { get; set; }

            JavaScriptSerializer serializer = new JavaScriptSerializer();
            

            //-------------------------Products-------------------------

            public void GET_Categories()
            {
                Method = "GET";
                URLPath = "/v2/categories/71";

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Host + URLPath);
                request.Method = Method;
                request.Credentials = new NetworkCredential(CustomerNumber, APIKey);

                string Result = "";
                string ErrorText = "";

                List<Category> Categories = new List<Category>();

                try
                {
                    HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                    StreamReader StreamReader = new StreamReader(response.GetResponseStream());
                    Result = StreamReader.ReadToEnd();

                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        Categories = serializer.Deserialize<List<Category>>(Result);

                    }
                    else
                    {
                        ErrorText = Result;
                    }
                }
                catch (Exception ex)
                {
                    ErrorText = ex.Message;
                }
                if (string.IsNullOrEmpty(ErrorText))
                {

                    for (int i = 0; i <= Categories.Count - 1; i++)
                    {
                        string CategoryName = Categories[i].name;
                    }
                }
            }

            public void GET_Styles()
            {
                Method = "GET";
                URLPath = "/v2/styles/4170";
                //URLPath = "/v2/styles";

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Host + URLPath);
                request.Method = Method;
                request.Credentials = new NetworkCredential(CustomerNumber, APIKey);

                string Result = "";
                string ErrorText = "";

                List<Style> Styles = new List<Style>();



                try
                {
                    HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                    StreamReader StreamReader = new StreamReader(response.GetResponseStream());
                    Result = StreamReader.ReadToEnd();

                    serializer.MaxJsonLength = Int32.MaxValue;

                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        Styles = serializer.Deserialize<List<Style>>(Result);



                    }
                    else
                    {
                        ErrorText = Result;
                    }




                }
                catch (Exception ex)
                {
                    ErrorText = ex.Message;
                }



                if (string.IsNullOrEmpty(ErrorText))
                {
                    for (int i = 0; i <= Styles.Count - 1; i++)
                    {

                        string StyleName = Styles[i].styleName;
                    }
                }
            }
            public void GetProductsByExternalID(ImageSolutions.Item.Item item)
            {
                Console.WriteLine(string.Format("GetProductsByExternalID {0}", item.ExternalID));

                Method = "GET";
                //URLPath = "/v2/products/?style=4170";

                URLPath = string.Format("/v2/products/{0}", item.ExternalID);

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Host + URLPath);
                request.Method = Method;
                request.Credentials = new NetworkCredential(CustomerNumber, APIKey);

                string Result = "";
                string ErrorText = "";

                List<Sku> Products = new List<Sku>();

                try
                {
                    HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                    StreamReader StreamReader = new StreamReader(response.GetResponseStream());
                    Result = StreamReader.ReadToEnd();

                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        Products = serializer.Deserialize<List<Sku>>(Result);

                        double? dcSalePrice = Convert.ToDouble(Products[0].salePrice);
                        int? intVendorInventory = 0;
                        int? styleID = Products[0].styleID;
                        string strColorCode = Products[0].colorName;
                        string strSizeCode = Products[0].sizeName;

                        if (item.Color == strColorCode && item.SizeCode == strSizeCode)
                        {

                            foreach (ActiveWearIntegration.ActiveWearAPI.Warehouse _warehouse in Products[0].warehouses)
                            {
                                intVendorInventory += _warehouse.qty;
                            }

                            item.PurchasePrice = dcSalePrice;
                            item.VendorInventory = intVendorInventory;
                            item.VendorInventoryLastUpdatedOn = DateTime.UtcNow;
                            item.StyleID = Convert.ToString(styleID);
                            item.StyleNumber = Products[0].styleName;
                            item.UnitWeight = Products[0].unitWeight;
                            item.ErrorMessage = string.Empty;

                            item.Update();

                            Console.WriteLine(string.Format("GetProductsByExternalID {0} Updated", item.ExternalID));

                        }
                        else
                        {
                            throw new Exception("Descrepancy with ExternalID to Vendor SKU");
                        }
                    }
                    else
                    {
                        throw new Exception(Result);
                    }
                }
                catch (Exception ex)
                {
                    item.ErrorMessage = ex.Message;
                    item.Update();

                    Console.WriteLine(string.Format("GetProductsByExternalID {0} Error", item.ErrorMessage));
                }
            }

            public void GET_ProductsBySKU()
            {
                Method = "GET";
                URLPath = "/v2/products/B00760003";

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Host + URLPath);
                request.Method = Method;
                request.Credentials = new NetworkCredential(CustomerNumber, APIKey);

                string Result = "";
                string ErrorText = "";

                List<Sku> Products = new List<Sku>();



                try
                {
                    HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                    StreamReader StreamReader = new StreamReader(response.GetResponseStream());
                    Result = StreamReader.ReadToEnd();



                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        Products = serializer.Deserialize<List<Sku>>(Result);



                    }
                    else
                    {
                        ErrorText = Result;
                    }

                }
                catch (Exception ex)
                {
                    ErrorText = ex.Message;
                }

                if (string.IsNullOrEmpty(ErrorText))
                {
                    for (int i = 0; i <= Products.Count - 1; i++)
                    {

                        string StyleName = Products[i].styleName;
                    }
                }
            }

            public void GET_Specs()
            {
                Method = "GET";
                URLPath = "/v2/specs/39";

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Host + URLPath);
                request.Method = Method;
                request.Credentials = new NetworkCredential(CustomerNumber, APIKey);

                string Result = "";
                string ErrorText = "";

                List<Spec> Specs = new List<Spec>();



                try
                {
                    HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                    StreamReader StreamReader = new StreamReader(response.GetResponseStream());
                    Result = StreamReader.ReadToEnd();



                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        Specs = serializer.Deserialize<List<Spec>>(Result);



                    }
                    else
                    {
                        ErrorText = Result;
                    }




                }
                catch (Exception ex)
                {
                    ErrorText = ex.Message;
                }



                if (string.IsNullOrEmpty(ErrorText))
                {
                    for (int i = 0; i <= Specs.Count - 1; i++)
                    {

                        string StyleName = Specs[i].styleName;
                    }
                }
            }

            //-------------------------Orders-------------------------

            public void GET_Orders()
            {
                Method = "GET";
                URLPath = "/v2/orders/";

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Host + URLPath);
                request.Method = Method;
                request.Credentials = new NetworkCredential(CustomerNumber, APIKey);

                string Result = "";
                string ErrorText = "";

                List<OrderHistory.Header> OrderHistory = new List<OrderHistory.Header>();



                try
                {
                    HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                    StreamReader StreamReader = new StreamReader(response.GetResponseStream());
                    Result = StreamReader.ReadToEnd();



                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        OrderHistory = serializer.Deserialize<List<OrderHistory.Header>>(Result);

                    }
                    else
                    {
                        ErrorText = Result;
                    }

                }
                catch (Exception ex)
                {
                    ErrorText = ex.Message;
                }
                if (string.IsNullOrEmpty(ErrorText))
                {
                    for (int i = 0; i <= OrderHistory.Count - 1; i++)
                    {

                        string OrderNumber = OrderHistory[i].orderNumber;
                    }
                }
            }

            public List<OrderHistory.Header> GetOrdersByShipDate(DateTime? Date)
            {
                Method = "GET";
                URLPath = string.Format("/v2/orders/?invoicedate={0}{1}{2}2022-09-14&boxes=true&boxxlines=true"
                    , Date != null ? string.Format("{0},", ((DateTime)Date).ToString("yyyy-MM-dd")) : string.Empty
                    , Date != null ? string.Format("{0},", ((DateTime)Date).AddDays(-1).ToString("yyyy-MM-dd")) : string.Empty
                    , Date != null ? string.Format("{0},", ((DateTime)Date).AddDays(-2).ToString("yyyy-MM-dd")) : string.Empty
                    , Date != null ? string.Format("{0}", ((DateTime)Date).AddDays(-3).ToString("yyyy-MM-dd")) : string.Empty);

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Host + URLPath);
                request.Method = Method;
                request.Credentials = new NetworkCredential(CustomerNumber, APIKey);

                string Result = "";
                string ErrorText = "";

                List<OrderHistory.Header> OrderHistory = new List<OrderHistory.Header>();

                try
                {
                    HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                    StreamReader StreamReader = new StreamReader(response.GetResponseStream());
                    Result = StreamReader.ReadToEnd();

                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        OrderHistory = serializer.Deserialize<List<OrderHistory.Header>>(Result);
                    }
                    else
                    {
                        ErrorText = Result;
                    }
                }
                catch (Exception ex)
                {
                    ErrorText = ex.Message;
                }

                return OrderHistory;
            }
            public List<OrderHistory.Header> GetOrdersByPoNumber(string ponumber, bool IsFulfillment)
            {
                Method = "GET";
                URLPath = string.Format("/v2/orders/PO,{0}?lines=true&boxes=true&boxlines=true"
                    , ponumber);
                //, IsFulfillment ? string.Format("?lines=true") : string.Format("?boxes=true";

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Host + URLPath);
                request.Method = Method;
                request.Credentials = new NetworkCredential(CustomerNumber, APIKey);

                string Result = "";
                string ErrorText = "";

                List<OrderHistory.Header> OrderHistory = new List<OrderHistory.Header>();

                try
                {
                    HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                    StreamReader StreamReader = new StreamReader(response.GetResponseStream());
                    Result = StreamReader.ReadToEnd();

                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        OrderHistory = serializer.Deserialize<List<OrderHistory.Header>>(Result);
                    }
                    else
                    {
                        ErrorText = Result;
                    }
                }
                catch (Exception ex)
                {
                    ErrorText = ex.Message;
                }

                return OrderHistory;
            }

            public void POST_Orders()
            {
                Method = "POST";
                URLPath = "/v2/Orders";

                Order Order = new Order();

                Order.shippingAddress = new Address();
                Order.shippingAddress.customer = "Company ABC";
                Order.shippingAddress.attn = "John Doe";
                Order.shippingAddress.address = "123 Main St";
                Order.shippingAddress.city = "Bollingbrook";
                Order.shippingAddress.state = "IL";
                Order.shippingAddress.zip = "60440";
                Order.shippingAddress.residential = true;

                Order.shippingMethod = "1";
                Order.shipBlind = false;
                Order.poNumber = "Test";
                Order.emailConfirmation = "";
                Order.testOrder = true;

                Order.lines = new List<Order.Line>();
                Order.lines.Add(new Order.Line());
                Order.lines[0].warehouseAbbr = "IL";
                Order.lines[0].identifier = "B00760003";
                Order.lines[0].qty = 2;


                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Host + URLPath);
                request.Method = Method;
                request.Credentials = new NetworkCredential(CustomerNumber, APIKey);
                request.ContentType = ContentType;











                try
                {
                    using (StreamWriter streamWriter = new StreamWriter(request.GetRequestStream()))
                    {
                        string json = System.Convert.ToString(serializer.Serialize(Order));
                        streamWriter.Write(json);
                        streamWriter.Flush();
                        streamWriter.Close();
                    }
                }
                catch (Exception ex)
                {
                }


                string Result = "";
                string ErrorText = "";

                List<OrderHistory.Header> OrderHistory = new List<OrderHistory.Header>();



                try
                {
                    HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                    StreamReader StreamReader = new StreamReader(response.GetResponseStream());
                    Result = StreamReader.ReadToEnd();



                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        OrderHistory = serializer.Deserialize<List<OrderHistory.Header>>(Result);



                    }
                    else
                    {
                        ErrorText = Result;
                    }




                }
                catch (Exception ex)
                {
                    ErrorText = ex.Message;
                }



                if (string.IsNullOrEmpty(ErrorText))
                {
                    string ConfirmationNumber = OrderHistory[0].orderNumber;
                }

            }

            //-------------------------Cross Ref-------------------------

            public void GET_CrossRef()
            {
                Method = "GET";
                URLPath = "/v2/crossref/";

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Host + URLPath);
                request.Method = Method;
                request.Credentials = new NetworkCredential(CustomerNumber, APIKey);

                string Result = "";
                string ErrorText = "";

                List<CrossRef> CrossRef = new List<CrossRef>();



                try
                {
                    HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                    StreamReader StreamReader = new StreamReader(response.GetResponseStream());
                    Result = StreamReader.ReadToEnd();



                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        CrossRef = serializer.Deserialize<List<CrossRef>>(Result);



                    }
                    else
                    {
                        ErrorText = Result;
                    }




                }
                catch (Exception ex)
                {
                    ErrorText = ex.Message;
                }



                if (string.IsNullOrEmpty(ErrorText))
                {
                    for (int i = 0; i <= CrossRef.Count - 1; i++)
                    {

                        string YourSku = CrossRef[i].yourSku;
                    }
                }
            }

            public void PUT_CrossRef()
            {
                Method = "PUT";

                string YourSku = "ABC";
                string Identifier = "B00760503";

                URLPath = "/v2/crossref/" + YourSku + "?Identifier=" + Identifier;

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Host + URLPath);
                request.Method = Method;
                request.Credentials = new NetworkCredential(CustomerNumber, APIKey);
                request.ContentLength = 0;

                string Result = "";
                string ErrorText = "";


                try
                {
                    HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                    StreamReader StreamReader = new StreamReader(response.GetResponseStream());


                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        //Record Updated



                    }
                    else if (response.StatusCode == HttpStatusCode.Created)
                    {
                        //Record Created
                    }





                }
                catch (Exception ex)
                {
                    ErrorText = ex.Message;
                }


            }

            public void DELETE_CrossRef()
            {
                Method = "DELETE";

                string YourSku = "ABC";

                URLPath = "/v2/crossref/" + YourSku;

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Host + URLPath);
                request.Method = Method;
                request.Credentials = new NetworkCredential(CustomerNumber, APIKey);

                string Result = "";
                string ErrorText = "";


                try
                {
                    HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                    StreamReader StreamReader = new StreamReader(response.GetResponseStream());


                    if (response.StatusCode == HttpStatusCode.NoContent)
                    {
                        //Record Deleted 
                    }




                }
                catch (Exception ex)
                {
                    ErrorText = ex.Message;
                }


            }

        }

        public class Order
        {
            public Order()
            {
                shippingMethod = null;
                shipBlind = null;
                poNumber = null;
                emailConfirmation = null;
                testOrder = null;
            }

            public Address shippingAddress { get; set; }
            public string shippingMethod { get; set; }
            public bool? shipBlind { get; set; }
            public string poNumber { get; set; }
            public string emailConfirmation { get; set; }
            public bool? testOrder { get; set; }

            public CC CreditCard { get; set; }
            public List<Line> lines { get; set; }

            public class Line
            {
                public string warehouseAbbr { get; set; }
                public string identifier { get; set; }
                public int? qty { get; set; }

            }

            public class CC
            {
                public string cardNumber { get; set; }
                public string expMonth { get; set; }
                public string expYear { get; set; }
                public string billingZipCode { get; set; }
                public string billingStreetNumber { get; set; }

            }

        }

        public class OrderHistory
        {

            public class Header
            {

                public int orderHeaderID { get; set; }

                public Guid guid { get; set; }
                public string companyName { get; set; }
                public string warehouseAbbr { get; set; }
                public string orderNumber { get; set; }
                public string invoiceNumber { get; set; }
                public string poNumber { get; set; }

                public string customerNumber { get; set; }

                public System.DateTime? orderDate { get; set; }
                public System.DateTime? shipDate { get; set; }
                public System.DateTime? invoiceDate { get; set; }
                public string orderType { get; set; }
                public string terms { get; set; }
                public string orderStatus { get; set; }

                public bool? dropship { get; set; }
                public string shippingCarrier { get; set; }
                public string shippingMethod { get; set; }
                public bool? shipBlind { get; set; }
                public string shippingCollectNumber { get; set; }
                public string trackingNumber { get; set; }

                public Address shippingAddress { get; set; }


                public decimal? subtotal { get; set; }
                public decimal? shipping { get; set; }
                public decimal? cod { get; set; }
                public decimal? tax { get; set; }
                public decimal? lostCashDiscount { get; set; }
                public decimal? smallOrderFee { get; set; }
                public decimal? cuponDiscount { get; set; }
                public decimal? sampleDiscount { get; set; }
                public decimal? setUpFee { get; set; }
                public decimal? restockFee { get; set; }
                public decimal? debitCredit { get; set; }
                public decimal? total { get; set; }

                public int? totalPieces { get; set; }
                public int? totalLines { get; set; }
                public decimal? totalWeight { get; set; }
                public int? totalBoxes { get; set; }

                public List<Line> lines { get; set; }

                public class Line
                {

                    public Line()
                    {
                    }


                    public int? lineNumber { get; set; }
                    public string type { get; set; }
                    public int? skuID { get; set; }
                    public string sku { get; set; }
                    public string gtin { get; set; }
                    public string yourSku { get; set; }

                    public int? qtyOrdered { get; set; }
                    public int? qtyShipped { get; set; }
                    public decimal? price { get; set; }

                    public string brandName { get; set; }
                    public string styleName { get; set; }
                    public string title { get; set; }
                    public string colorName { get; set; }
                    public string sizeName { get; set; }


                }

            }

        }

        public class Address
        {

            public Address()
            {
                customer = null;
                attn = null;
                residential = null;
            }

            public string customer { get; set; }
            public string attn { get; set; }
            public string address { get; set; }
            public string city { get; set; }
            public string state { get; set; }
            public string zip { get; set; }
            public bool? residential { get; set; }

        }

        public class Category
        {
            public int? categoryID { get; set; }
            public string name { get; set; }
            public string image { get; set; }

        }

        public class Spec
        {
            public int? specID { get; set; }
            public int? styleID { get; set; }
            public string partNumber { get; set; }
            public string brandName { get; set; }
            public string styleName { get; set; }
            public string sizeName { get; set; }
            public string sizeOrder { get; set; }
            public string specName { get; set; }
            public string value { get; set; }

        }

        public class Style
        {
            public int? styleID { get; set; }
            public string partNumber { get; set; }
            public string brandName { get; set; }
            public string styleName { get; set; }
            public string title { get; set; }
            public string description { get; set; }
            public string baseCategory { get; set; }
            public string categories { get; set; }
            public string catalogPageNumber { get; set; }
            public bool? newStyle { get; set; }
            public int? comparableGroup { get; set; }
            public int? companionGroup { get; set; }
            public string brandImage { get; set; }
            public string styleImage { get; set; }
            public List<Spec> specs { get; set; }

        }

        public class Sku
        {

            public string sku { get; set; }
            public string gtin { get; set; }
            public string yourSku { get; set; }

            public int? styleID { get; set; }
            public string brandName { get; set; }
            public string styleName { get; set; }

            public string colorName { get; set; }
            public string colorCode { get; set; }
            public string colorPriceCodeName { get; set; }
            public string colorGroup { get; set; }
            public string colorFamily { get; set; }
            public string colorSwatchImage { get; set; }
            public string colorSwatchTextColor { get; set; }
            public string colorFrontImage { get; set; }
            public string colorSideImage { get; set; }
            public string colorBackImage { get; set; }
            public string color1 { get; set; }
            public string color2 { get; set; }

            public string sizeName { get; set; }
            public string sieCode { get; set; }
            public string sizeOrder { get; set; }
            public string sizePriceCodeName { get; set; }

            public int? caseQty { get; set; }
            public decimal? unitWeight { get; set; }


            public decimal? piecePrice { get; set; }
            public decimal? dozenPrice { get; set; }
            public decimal? casePrice { get; set; }
            public decimal? salePrice { get; set; }
            public decimal? customerPrice { get; set; }
            public System.DateTime? saleExpiration { get; set; }

            public List<Warehouse> warehouses { get; set; }

        }

        public class Warehouse
        {
            public string warehouseAbbr { get; set; }
            public int? skuID { get; set; }
            public int? qty { get; set; }
            public bool? closeout { get; set; }
            public bool? dropship { get; set; }
            public bool? excludeFreeFreight { get; set; }

        }

        public class CrossRef
        {
            public int? customerID { get; set; }
            public string yourSku { get; set; }
            public int? skuID { get; set; }
            public string sku { get; set; }
            public string gtin { get; set; }
            public string brandName { get; set; }
            public string styleName { get; set; }
            public string colorName { get; set; }
            public string sizeName { get; set; }

        }
    }
}