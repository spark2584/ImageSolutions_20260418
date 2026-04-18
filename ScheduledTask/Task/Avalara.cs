using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalara.AvaTax.RestClient;
using ImageSolutions.Address;
using ImageSolutions.ShoppingCart;

namespace ScheduledTask.Task
{
    public class Avalara
    {
        public bool Test()
        {
            ImageSolutions.Avalara.Avalara Avalara = new ImageSolutions.Avalara.Avalara();

            double decTotal = 150.45; //ShoppingCartLines.Sum(x => x.UnitPrice * x.Quantity);

            AddressTrans ShippingAddress = new AddressTrans();
            AddressTrans DeliveryAddress = new AddressTrans();
            ShoppingCart ShoppingCart = new ShoppingCart();

            ShoppingCart.ShoppingCartLines = new List<ShoppingCartLine>();

            ShoppingCartLine ShoppingCartLine1 = new ShoppingCartLine();
            ShoppingCartLine1.ItemID = "949877";
            ShoppingCartLine1.Quantity = 2;
            ShoppingCartLine1.UnitPrice = 6.75;
            ShoppingCart.ShoppingCartLines.Add(ShoppingCartLine1);

            ShoppingCartLine ShoppingCartLine2 = new ShoppingCartLine();
            ShoppingCartLine2.ItemID = "949878";
            ShoppingCartLine2.Quantity = 4;
            ShoppingCartLine2.UnitPrice = 6.75;
            ShoppingCart.ShoppingCartLines.Add(ShoppingCartLine2);

            ShoppingCartLine ShoppingCartLine3 = new ShoppingCartLine();
            ShoppingCartLine3.ItemID = "949879";
            ShoppingCartLine3.Quantity = 4;
            ShoppingCartLine3.UnitPrice = 6.75;
            ShoppingCart.ShoppingCartLines.Add(ShoppingCartLine3);

            ShoppingCartLine ShoppingCartLine4 = new ShoppingCartLine();
            ShoppingCartLine4.ItemID = "949880";
            ShoppingCartLine4.Quantity = 3;
            ShoppingCartLine4.UnitPrice = 6.75;
            ShoppingCart.ShoppingCartLines.Add(ShoppingCartLine4);

            ShoppingCartLine ShoppingCartLine5 = new ShoppingCartLine();
            ShoppingCartLine5.ItemID = "1746";
            ShoppingCartLine5.Quantity = 1;
            ShoppingCartLine5.UnitPrice = 10.08;
            ShoppingCart.ShoppingCartLines.Add(ShoppingCartLine5);
            
            ShoppingCartLine ShoppingCartLine6 = new ShoppingCartLine();
            ShoppingCartLine6.ItemID = "1753";
            ShoppingCartLine6.Quantity = 1;
            ShoppingCartLine6.UnitPrice = 10.08;
            ShoppingCart.ShoppingCartLines.Add(ShoppingCartLine6);

            ShoppingCartLine ShoppingCartLine7 = new ShoppingCartLine();
            ShoppingCartLine7.ItemID = "1760";
            ShoppingCartLine7.Quantity = 1;
            ShoppingCartLine7.UnitPrice = 10.08;
            ShoppingCart.ShoppingCartLines.Add(ShoppingCartLine7);

            ShoppingCartLine ShoppingCartLine8 = new ShoppingCartLine();
            ShoppingCartLine8.ItemID = "1765";
            ShoppingCartLine8.Quantity = 1;
            ShoppingCartLine8.UnitPrice = 10.08;
            ShoppingCart.ShoppingCartLines.Add(ShoppingCartLine8);

            ShoppingCartLine ShoppingCartLine9 = new ShoppingCartLine();
            ShoppingCartLine9.ItemID = "1343";
            ShoppingCartLine9.Quantity = 2;
            ShoppingCartLine9.UnitPrice = 11.19;
            ShoppingCart.ShoppingCartLines.Add(ShoppingCartLine9);

      //      <add key="AvalaraUsername" value="vincent@imageinc.com" />
		    //<add key="AvalaraPassword" value="ImageSolutions$1" />

            AvaTaxClient AvaTaxClient = new AvaTaxClient("Image Solutions Website", "1.0", Environment.MachineName, AvaTaxEnvironment.Production).WithSecurity(
                    Convert.ToString("vincent@imageinc.com")
                    , Convert.ToString("ImageSolutions$1"));

            TransactionBuilder TransactionBuilder = new TransactionBuilder(AvaTaxClient, "DEFAULT", DocumentType.SalesOrder, "DEFAULT-Customer");//shoppingcart.UserWebsiteID);

            foreach (ShoppingCartLine _ShoppingCartLine in ShoppingCart.ShoppingCartLines)
            {
                TransactionBuilder.WithLineItem(
                    Convert.ToDecimal(Math.Round((_ShoppingCartLine.UnitPrice) * _ShoppingCartLine.Quantity, 2)) //Total amount
                    , Convert.ToDecimal(_ShoppingCartLine.Quantity) //Total Quantity
                    , string.IsNullOrEmpty(_ShoppingCartLine.Item.AvataxTaxCode) ? "PC030153" : _ShoppingCartLine.Item.AvataxTaxCode //Tax Code needed it from NS - default to PC030153
                    , _ShoppingCartLine.Item.Description //Description
                    , _ShoppingCartLine.Item.ItemNumber //Item Code - need for international
                    , null //Line Number
                    , null //Entity Use Code
                );
            }

            TransactionBuilder.WithLineItem(
                Convert.ToDecimal(27.83) //Convert.ToDecimal(ShoppingCart.ShippingAmount) //Total amount
                , Convert.ToDecimal(1) //Total Quantity
                , "FR020100"//Tax Code needed it from NS - default to PC030153
                , "Shipping" //Description
                , "Shipping" //Item Code - need for international
                , null //Line Number
                , null //Entity Use Code
            );

            TransactionBuilder.WithAddress(TransactionAddressType.ShipFrom
                , "4692 Brate Dr" //Convert.ToString(ConfigurationManager.AppSettings["ShipFromAddress1"])
                , "Ste 300"
                , null
                , "West Chester"//Convert.ToString(ConfigurationManager.AppSettings["ShipFromCity"])
                , "OH"//Convert.ToString(ConfigurationManager.AppSettings["ShipFromState"])
                , "45011-3558" //Convert.ToString(ConfigurationManager.AppSettings["ShipFromPostalCode"])
                , "US" //Convert.ToString(ConfigurationManager.AppSettings["ShipFromCountry"])
            );

            TransactionBuilder.WithAddress(TransactionAddressType.ShipTo
                , "2571 Lawrence Ave E" //Convert.ToString(ConfigurationManager.AppSettings["ShipFromAddress1"])
                , ""
                , null
                , "Scarborough"//Convert.ToString(ConfigurationManager.AppSettings["ShipFromCity"])
                , "ON"//Convert.ToString(ConfigurationManager.AppSettings["ShipFromState"])
                , "M1P 4W5" //Convert.ToString(ConfigurationManager.AppSettings["ShipFromPostalCode"])
                , "CA" //Convert.ToString(ConfigurationManager.AppSettings["ShipFromCountry"])
            );

            TransactionModel TransactionModel = TransactionBuilder.Create();

            return true;
        }
    }
}
