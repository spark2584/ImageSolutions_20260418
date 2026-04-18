using Avalara.AvaTax.RestClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageSolutions.Avalara
{
    public class Avalara
    {
        public Avalara()
        {
        }
        public TransactionModel GetAvalaraTax(ImageSolutions.ShoppingCart.ShoppingCart shoppingcart, Address.AddressTrans shipto, Address.AddressTrans shipfrom) // decimal amount, decimal qty, string taxcode, string description, string itemcode, string linenumber, string entityusecode)
        {
            try
            {
                
                //AvaTaxClient AvaTaxClient = new AvaTaxClient("Image Solutions Website", "1.0", Environment.MachineName, AvaTaxEnvironment.Sandbox).WithSecurity(
                //    Convert.ToString(ConfigurationManager.AppSettings["AvalaraUsername"])
                //    , Convert.ToString(ConfigurationManager.AppSettings["AvalaraPassword"]));

                AvaTaxClient AvaTaxClient = new AvaTaxClient("Image Solutions Website", "1.0", Environment.MachineName, AvaTaxEnvironment.Production).WithSecurity(
                    Convert.ToString(ConfigurationManager.AppSettings["AvalaraUsername"])
                    , Convert.ToString(ConfigurationManager.AppSettings["AvalaraPassword"]));

                TransactionBuilder TransactionBuilder = new TransactionBuilder(AvaTaxClient, "DEFAULT", DocumentType.SalesOrder, "DEFAULT-Customer");//shoppingcart.UserWebsiteID);

                foreach (ShoppingCart.ShoppingCartLine _ShoppingCartLine in shoppingcart.ShoppingCartLines)
                {
                    TransactionBuilder.WithLineItem(
                        Convert.ToDecimal(_ShoppingCartLine.LineTotal) //Total amount
                        , Convert.ToDecimal(_ShoppingCartLine.Quantity) //Total Quantity
                        , string.IsNullOrEmpty(_ShoppingCartLine.Item.AvataxTaxCode) ?  "PC030153" : _ShoppingCartLine.Item.AvataxTaxCode //Tax Code needed it from NS - default to PC030153
                        , _ShoppingCartLine.Item.Description //Description
                        , _ShoppingCartLine.Item.ItemNumber //Item Code - need for international
                        , null //Line Number
                        , null //Entity Use Code
                    );
                }

                TransactionBuilder.WithLineItem(
                    Convert.ToDecimal(shoppingcart.ShippingAmount) //Total amount
                    , Convert.ToDecimal(1) //Total Quantity
                    , "FR020100"//Tax Code needed it from NS - default to PC030153
                    , "Shipping" //Description
                    , "Shipping" //Item Code - need for international
                    , null //Line Number
                    , null //Entity Use Code
                );

                TransactionBuilder.WithAddress(TransactionAddressType.ShipFrom
                    , shipfrom.AddressLine1 //"4692 Brate Dr" //Convert.ToString(ConfigurationManager.AppSettings["ShipFromAddress1"])
                    , shipfrom.AddressLine2 //"Ste 300"
                    , null
                    , shipfrom.City //Convert.ToString(ConfigurationManager.AppSettings["ShipFromCity"])
                    , shipfrom.State //Convert.ToString(ConfigurationManager.AppSettings["ShipFromState"])
                    , shipfrom.PostalCode //"45011-3558" //Convert.ToString(ConfigurationManager.AppSettings["ShipFromPostalCode"])
                    , shipfrom.CountryCode //"US" //Convert.ToString(ConfigurationManager.AppSettings["ShipFromCountry"])
                );

                TransactionBuilder.WithAddress(TransactionAddressType.ShipTo
                    , shipto.AddressLine1
                    , shipto.AddressLine2
                    , shipto.AddressLine3
                    , shipto.City
                    , shipto.State
                    , shipto.PostalCode
                    , shipto.CountryCode);

                TransactionModel TransactionModel = TransactionBuilder.Create();

                return TransactionModel;
            }
            catch(Exception e)
            {
                throw e;
            }            
        }

        public TransactionModel GetAvalaraTaxForBudget (Address.AddressTrans shipto, Address.AddressTrans shipfrom, decimal budgetamount, string description, string itemnumber)
        {
            try
            {
                AvaTaxClient AvaTaxClient = new AvaTaxClient("Image Solutions Website", "1.0", Environment.MachineName, AvaTaxEnvironment.Production).WithSecurity(
                    Convert.ToString(ConfigurationManager.AppSettings["AvalaraUsername"])
                    , Convert.ToString(ConfigurationManager.AppSettings["AvalaraPassword"]));

                TransactionBuilder TransactionBuilder = new TransactionBuilder(AvaTaxClient, "DEFAULT", DocumentType.SalesOrder, "DEFAULT-Customer");//shoppingcart.UserWebsiteID);

                TransactionBuilder.WithLineItem(
                    Convert.ToDecimal(budgetamount)
                    , Convert.ToDecimal(1)
                    , "PC030153"
                    , description
                    , itemnumber
                    , null 
                    , null
                );
      
                TransactionBuilder.WithAddress(TransactionAddressType.ShipFrom
                    , shipfrom.AddressLine1
                    , shipfrom.AddressLine2 
                    , null
                    , shipfrom.City 
                    , shipfrom.State 
                    , shipfrom.PostalCode 
                    , shipfrom.CountryCode 
                );

                TransactionBuilder.WithAddress(TransactionAddressType.ShipTo
                    , shipto.AddressLine1
                    , shipto.AddressLine2
                    , shipto.AddressLine3
                    , shipto.City
                    , shipto.State
                    , shipto.PostalCode
                    , shipto.CountryCode);

                TransactionModel TransactionModel = TransactionBuilder.Create();

                return TransactionModel;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}
