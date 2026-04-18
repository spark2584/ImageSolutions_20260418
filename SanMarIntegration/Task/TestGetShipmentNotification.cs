using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SanMarIntegration.Task
{
    public class TestGetShipmentNotification
    {
        //webservice: https://ws.sanmar.com:8080/promostandards/OrderShipmentNotificationServiceBinding?wsdl

        public static bool Start()
        {
            com.sanmar.ws_ordershipmentnotificationservicebinding.GetOrderShipmentNotificationRequest request = new com.sanmar.ws_ordershipmentnotificationservicebinding.GetOrderShipmentNotificationRequest();
            request.wsVersion = "1.0.0";
            request.id = "DANIELLEMOORE1";
            request.password = "danie71046";
            request.queryType = com.sanmar.ws_ordershipmentnotificationservicebinding.GetOrderShipmentNotificationRequestQueryType.Item1;
            request.referenceNumber = "1392128";
            com.sanmar.ws_ordershipmentnotificationservicebinding.OrderShipmentNotificationService service = new com.sanmar.ws_ordershipmentnotificationservicebinding.OrderShipmentNotificationService();

            com.sanmar.ws_ordershipmentnotificationservicebinding.GetOrderShipmentNotificationResponse response = service.getOrderShipmentNotification(request);
            return true;
        }
    }
}
