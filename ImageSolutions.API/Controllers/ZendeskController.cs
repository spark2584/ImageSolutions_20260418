using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;

namespace ImageSolutions.API.Controllers
{
    [RoutePrefix("zendesk")]
    public class ZendeskController : ApiController
    {
        [HttpGet]
        [Route("tracking/search/{search}")]
        public HttpResponseMessage GetTrackingSearch(string search)
        {
            //List<OrderTracking> Return = new List<OrderTracking>();

            //OrderTracking OrderTracking1 = new OrderTracking();
            //OrderTracking1.OrderNumber = "Order_101";
            //OrderTracking1.Tracking = "XXXXXXXXXXXXXXX1";
            //Return.Add(OrderTracking1);

            //OrderTracking OrderTracking2 = new OrderTracking();
            //OrderTracking2.OrderNumber = "Order_101";
            //OrderTracking2.Tracking = "XXXXXXXXXXXXXXX1";
            //Return.Add(OrderTracking2);


            search = DecodeBase64(search);

            List<OrderTracking> Return = new List<OrderTracking>();
            Return = FindOrderBySearch(search);

            return Request.CreateResponse(HttpStatusCode.OK, Return);
        }

        [HttpGet]
        [Route("tracking/ordernumber/{ordernumber}")]
        public HttpResponseMessage GetTrackingOrderNumber(string ordernumber)
        {
            //ordernumber = DecodeBase64(ordernumber);

            OrderTracking Return = new OrderTracking();
            Return = FindOrderByOrderNumber(ordernumber);

            return Request.CreateResponse(HttpStatusCode.OK, Return);
            //return Request.CreateResponse(HttpStatusCode.OK, string.Format(@"Tracking for Order# {0}: {1}", ordernumber, "XXXXXXXXXXXXXXX3"));
        }

        [HttpPost]
        [Route("tracking/search")]
        public HttpResponseMessage CreateTrackingSearch([FromBody] OrderTrackingSearch ordertrackingsearch)
        {
            List<OrderTracking> Return = new List<OrderTracking>();
            Return = FindOrderBySearch(ordertrackingsearch.Search);

            return Request.CreateResponse(HttpStatusCode.OK, Return);
        }

        public List<OrderTracking> FindOrderBySearch(string search)
        {
            SqlDataReader objRead = null;
            string strSQL = string.Empty;

            List<OrderTracking> OrderTrackings = null;
            try
            {
                strSQL = string.Format(@"
SELECT TOP 3 s.SalesOrderID, CONVERT(VARCHAR(10), s.TransactionDate , 101) as OrderDate, s.SalesOrderNumber, s.NetSuiteInternalID, s.TransactionDate, u.EmailAddress, CONVERT(VARCHAR(10), f.ShipDate , 101) as ShipDate, f.TrackingNumber
FROM SalesOrder (NOLOCK) s
Inner Join UserInfo (NOLOCK) u on u.UserInfoID = s.UserInfoID
Left Outer Join Fulfillment (NOLOCK) f on f.SalesOrderID = s.SalesOrderID
WHERE ( u.EmailAddress = {0} or s.SalesOrderNumber = {0} or CAST(s.SalesOrderID as varchar) = {0} )
ORDER BY s.TransactionDate DESC
"
                , Database.HandleQuote(search));

                objRead = Database.GetDataReader(strSQL);

                OrderTrackings = new List<OrderTracking>();

                while (objRead.Read())
                {
                    OrderTracking OrderTracking = new OrderTracking();
                    OrderTracking.SalesOrderID = Convert.ToString(objRead["SalesOrderID"]);
                    OrderTracking.OrderDate = Convert.ToString(objRead["OrderDate"]);
                    OrderTracking.OrderNumber = Convert.ToString(objRead["SalesOrderNumber"]);
                    OrderTracking.Email = Convert.ToString(objRead["EmailAddress"]);
                    OrderTracking.Tracking = Convert.ToString(objRead["TrackingNumber"]);
                    OrderTracking.ShipDate = Convert.ToString(objRead["ShipDate"]);
                    OrderTrackings.Add(OrderTracking);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (objRead != null) objRead.Dispose();
                objRead = null;
            }

            return OrderTrackings;
        }

        public OrderTracking FindOrderByOrderNumber(string ordernumber)
        {
            SqlDataReader objRead = null;
            string strSQL = string.Empty;

            OrderTracking OrderTracking = null;
            try
            {
                strSQL = string.Format(@"
SELECT s.SalesOrderID, CONVERT(VARCHAR(10), s.TransactionDate , 101) as OrderDate, s.SalesOrderNumber, s.NetSuiteInternalID, s.TransactionDate, u.EmailAddress, CONVERT(VARCHAR(10), f.ShipDate , 101) as ShipDate, f.TrackingNumber
FROM SalesOrder (NOLOCK) s
Inner Join UserInfo (NOLOCK) u on u.UserInfoID = s.UserInfoID
Left Outer Join Fulfillment (NOLOCK) f on f.SalesOrderID = s.SalesOrderID
WHERE ( s.SalesOrderNumber = {0} or CAST(s.SalesOrderID as varchar) = {0} ) 
ORDER BY s.SalesOrderID DESC
"
                , Database.HandleQuote(ordernumber));

                objRead = Database.GetDataReader(strSQL);

                OrderTracking = new OrderTracking();

                if (objRead.Read())
                {
                    OrderTracking.SalesOrderID = Convert.ToString(objRead["SalesOrderID"]);
                    OrderTracking.OrderDate = Convert.ToString(objRead["OrderDate"]);
                    OrderTracking.OrderNumber = Convert.ToString(objRead["SalesOrderNumber"]);
                    OrderTracking.Email = Convert.ToString(objRead["EmailAddress"]);
                    OrderTracking.Tracking = Convert.ToString(objRead["TrackingNumber"]);
                    OrderTracking.ShipDate = Convert.ToString(objRead["ShipDate"]);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (objRead != null) objRead.Dispose();
                objRead = null;
            }

            return OrderTracking;
        }

        public string EncodeBase64(string value)
        {
            var valueBytes = Encoding.UTF8.GetBytes(value);
            return Convert.ToBase64String(valueBytes);
        }

        public string DecodeBase64(string value)
        {
            var valueBytes = System.Convert.FromBase64String(value);
            return Encoding.UTF8.GetString(valueBytes);
        }

        public class OrderTracking
        {
            public string SalesOrderID { get; set; }
            public string OrderDate { get; set; }
            public string OrderNumber { get; set; }
            public string Email { get; set; }
            public string Tracking { get; set; }
            public string ShipDate { get; set; }
        }

        public class OrderTrackingSearch
        {
            public string Search { get; set; }
        }
    }
}