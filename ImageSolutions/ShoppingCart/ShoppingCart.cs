using ImageSolutions.Address;
using ImageSolutions.Website;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalara.AvaTax.RestClient;

namespace ImageSolutions.ShoppingCart
{
    public class ShoppingCart : ISBase.BaseClass
    {
        public string ShoppingCartID { get; private set; }
        public bool IsNew { get { return string.IsNullOrEmpty(ShoppingCartID); } }
        public string GUID { get; set; }
        public string UserWebsiteID { get; set; }
        public string SalesOrderID { get; set; }
        public string CheckoutSession { get; set; }
        public string PunchoutSessionID { get; set; }
        public string WebsiteShippingServiceID { get; set; }
        public string PackageID { get; set; }
        public bool ExcludeOptional { get; set; }
        public AddressTrans ShippingAddress { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public DateTime CreatedOn { get; set; }
        private User.UserWebsite mUserWebsite = null;
        public User.UserWebsite UserWebsite
        {
            get
            {
                if (mUserWebsite == null && !string.IsNullOrEmpty(UserWebsiteID))
                {
                    mUserWebsite = new User.UserWebsite(UserWebsiteID);
                }
                return mUserWebsite;
            }
        }

        private WebsiteShippingService mWebsiteShippingService = null;
        public WebsiteShippingService WebsiteShippingService
        {
            get
            {
                if (mWebsiteShippingService == null && !string.IsNullOrEmpty(WebsiteShippingServiceID))
                {
                    mWebsiteShippingService = new WebsiteShippingService(WebsiteShippingServiceID);
                }
                return mWebsiteShippingService;
            }
        }
        private List<ShoppingCartPromotion> mShoppingCartPromotions = null;
        public List<ShoppingCartPromotion> ShoppingCartPromotions
        {
            get
            {
                if (!string.IsNullOrEmpty(ShoppingCartID))
                {
                    ShoppingCartPromotionFilter objFilter = null;

                    try
                    {
                        objFilter = new ShoppingCartPromotionFilter();
                        objFilter.ShoppingCartID = new Database.Filter.StringSearch.SearchFilter();
                        objFilter.ShoppingCartID.SearchString = ShoppingCartID;

                        mShoppingCartPromotions = ShoppingCartPromotion.GetShoppingCartPromotions(objFilter);
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }
                return mShoppingCartPromotions;
            }
        }
        private ImageSolutions.Package.Package mPackage = null;
        public ImageSolutions.Package.Package Package
        {
            get
            {
                if (mPackage == null && !string.IsNullOrEmpty(PackageID))
                {
                    mPackage = new ImageSolutions.Package.Package(PackageID);
                }
                return mPackage;
            }
        }
        public int TotalQuantity
        {
            get
            {
                int intReturn = 0;
                if (ShoppingCartLines != null)
                {
                    intReturn = ShoppingCartLines.Sum(m => m.Quantity);
                }
                return intReturn;
            }
        }

        public double LineTotal
        {
            get
            {
                double dcmReturn = 0;
                if (ShoppingCartLines != null)
                {
                    dcmReturn = ShoppingCartLines.Sum(m => m.LineTotal);
                    //dcmReturn = ShoppingCartLines.Sum(m => m.LineTotal) - ShoppingCartLines.FindAll(x => x.Item.IsCompanyInvoiced).Sum(m => m.LineTotal);
                }
                return Math.Round(dcmReturn, 2);
            }
        }

        private double? mSalesTaxAmount = null;
        public double SalesTaxAmount
        {
            get
            {
                if (ShippingAddress == null)
                {
                    return 0;
                }
                else
                {
                    try
                    {
                        ImageSolutions.Avalara.Avalara Avalara = new Avalara.Avalara();

                        TransactionModel TransactionModel = Avalara.GetAvalaraTax(this, ShippingAddress, UserWebsite.WebSite.DeliveryAddress);

                        return Convert.ToDouble(TransactionModel.totalTax);
                    }
                    catch (Exception ex)
                    {
                        return 0;
                    }


                    //ImageSolutions.ZipTax.ZipTax ZipTax = new ImageSolutions.ZipTax.ZipTax();
                    //ImageSolutions.ZipTax.ZipTaxResponse ZipTaxResponse = ZipTax.GetZipTax(ShippingAddress.City, ShippingAddress.State, ShippingAddress.PostalCode.Substring(0,5));

                    //if (ZipTaxResponse != null && ZipTaxResponse.results.Count > 0)
                    //{
                    //    double totalTax = 0;
                    //    ImageSolutions.ZipTax.ZipTaxResult ZipTaxResult = ZipTaxResponse.results[0];
                    //    totalTax += ZipTaxResult.taxUse;
                    //    totalTax += ZipTaxResult.stateUseTax;
                    //    //totalTax += ZipTaxResult.countyUseTax;
                    //    //totalTax += ZipTaxResult.districtUseTax;
                    //    //totalTax += ZipTaxResult.district1UseTax;
                    //    //totalTax += ZipTaxResult.district2UseTax;
                    //    //totalTax += ZipTaxResult.district3UseTax;
                    //    //totalTax += ZipTaxResult.district4UseTax;
                    //    //totalTax += ZipTaxResult.district5UseTax;

                    //    return LineTotal * totalTax;

                    //}
                    //else
                    //    return 0;
                }
            }
        }

        public double GetAvalaraTaxForBudget(decimal budgetamount)
        {
            if (ShippingAddress == null)
            {
                return 0;
            }
            else
            {
                if (ShoppingCartLines != null && ShoppingCartLines.Count > 0)
                {
                    ImageSolutions.Avalara.Avalara Avalara = new Avalara.Avalara();

                    try
                    {
                        double decTotal = ShoppingCartLines.Sum(x => x.UnitPrice * x.Quantity);

                        TransactionModel TransactionModel = Avalara.GetAvalaraTaxForBudget(ShippingAddress, UserWebsite.WebSite.DeliveryAddress, Convert.ToDecimal(decTotal) - budgetamount, ShoppingCartLines[0].Item.Description, ShoppingCartLines[0].Item.ItemNumber);

                        return Convert.ToDouble(TransactionModel.totalTax);
                    }
                    catch (Exception ex)
                    {
                        return 0;
                    }
                }
                else
                {
                    return 0;
                }
            }
        }

        public double TestTax
        {
            get
            {
                ImageSolutions.Avalara.Avalara Avalara = new Avalara.Avalara();

                AddressTrans AddressTrans = new AddressTrans();
                AddressTrans.AddressLine1 = "Rivadavia Avda 9825";
                AddressTrans.AddressLine2 = "";
                AddressTrans.AddressLine3 = "";
                AddressTrans.City = "Buenos Aires";
                AddressTrans.State = "Buenos Aires";
                AddressTrans.PostalCode = "";
                AddressTrans.CountryID = "AR";

                this.ShoppingCartLines = new List<ShoppingCartLine>();
                ShoppingCartLine ShoppingCartLine = new ShoppingCartLine();
                ShoppingCartLines.Add(ShoppingCartLine);

                TransactionModel TransactionModel = Avalara.GetAvalaraTax(this, AddressTrans, UserWebsite.WebSite.DeliveryAddress);

                return Convert.ToDouble(TransactionModel.totalTax);
            }
        }
        private double? mShippingAmount = null;
        public double ShippingAmount
        {
            get
            {
                if (mShippingAmount == null || mShippingAmount == 0)
                {
                    if (WebsiteShippingService == null || ShippingAddress == null)
                    {
                        mShippingAmount = 0;
                    }
                    else
                    {
                        double dbTotalWeightInLbs = ShoppingCartLines.Sum(m => m.Quantity);

                        ImageSolutions.Address.AddressTrans ToAddressTrans = new ImageSolutions.Address.AddressTrans();
                        ToAddressTrans.CompanyName = ShippingAddress.CompanyName;
                        ToAddressTrans.FirstName = ShippingAddress.FirstName;
                        ToAddressTrans.LastName = ShippingAddress.LastName;
                        ToAddressTrans.AddressLine1 = ShippingAddress.AddressLine1;
                        ToAddressTrans.AddressLine2 = ShippingAddress.AddressLine2;
                        ToAddressTrans.City = ShippingAddress.City;
                        ToAddressTrans.State = ShippingAddress.State;
                        ToAddressTrans.PostalCode = ShippingAddress.PostalCode;
                        ToAddressTrans.PhoneNumber = ShippingAddress.PhoneNumber;
                        ToAddressTrans.Email = ShippingAddress.Email;
                        ToAddressTrans.CountryCode = ShippingAddress.CountryCode;

                        //List<ImageSolutions.Shipping.ShippoRate> ShippoRates = ImageSolutions.Shipping.ShippoShipping.GetRate(UserWebsite.WebsiteID, this, ToAddressTrans);

                        ImageSolutions.Shipping.WebShipRate.WebShipRateResponse result =
                            ImageSolutions.Shipping.Restlet.getWebShipRate(UserWebsite.WebSite, this, ToAddressTrans, String.Empty);

                        if (result.shippingMethods != null && result.shippingMethods.Count > 0)
                        {
                            mShippingAmount = Convert.ToDouble(result.shippingMethods.Find(x => x.shipmethod == WebsiteShippingService.ShippingService.ServiceCode).rate);
                        }
                        else mShippingAmount = 0;
                    }
                }
                return Convert.ToDouble(mShippingAmount);

                //if (WebsiteShippingService == null || ShippingAddress == null)
                //{
                //    return 0;
                //}
                //else
                //{
                //    double dbTotalWeightInLbs = ShoppingCartLines.Sum(m => m.Quantity);
                //    //Shippo Shipping Method = WebsiteShippingService.ShippingService.ServiceCode
                //    //Ship To addreess =ShippingAddress
                //    //Ship From address = Torrance address

                //    //ImageSolutions.Address.AddressBook FromAddressBook = new ImageSolutions.Address.AddressBook();
                //    //FromAddressBook.CompanyName = "Image Solutions";
                //    //FromAddressBook.FirstName = "Steve";
                //    //FromAddressBook.LastName = "Park";
                //    //FromAddressBook.AddressLine1 = "19571 Magellan Dr";
                //    //FromAddressBook.AddressLine2 = "";
                //    //FromAddressBook.City = "Torrance";
                //    //FromAddressBook.State = "CA";
                //    //FromAddressBook.PostalCode = "90502";
                //    //FromAddressBook.PhoneNumber = "310-464-8991";
                //    //FromAddressBook.Email = "info@imageinc.com";

                //    ImageSolutions.Address.AddressTrans ToAddressTrans = new ImageSolutions.Address.AddressTrans();
                //    ToAddressTrans.CompanyName = ShippingAddress.CompanyName;
                //    ToAddressTrans.FirstName = ShippingAddress.FirstName;
                //    ToAddressTrans.LastName = ShippingAddress.LastName;
                //    ToAddressTrans.AddressLine1 = ShippingAddress.AddressLine1;
                //    ToAddressTrans.AddressLine2 = ShippingAddress.AddressLine2;
                //    ToAddressTrans.City = ShippingAddress.City;
                //    ToAddressTrans.State = ShippingAddress.State;
                //    ToAddressTrans.PostalCode = ShippingAddress.PostalCode;
                //    ToAddressTrans.CountryCode = ShippingAddress.CountryCode;
                //    ToAddressTrans.PhoneNumber = ShippingAddress.PhoneNumber;
                //    ToAddressTrans.Email = ShippingAddress.Email;

                //    //List<ImageSolutions.Shipping.ShippoRate> ShippoRates = ImageSolutions.Shipping.ShippoShipping.GetRate(UserWebsite.WebsiteID, this, ToAddressTrans, string.Empty);

                //    //if (ShippoRates != null && ShippoRates.Count > 0)
                //    //{
                //    //    if (ShippoRates.Exists(x => x.Token == WebsiteShippingService.ShippingService.ServiceCode))
                //    //    {
                //    //        return Convert.ToDouble(ShippoRates.Find(x => x.Token == WebsiteShippingService.ShippingService.ServiceCode).Amount);
                //    //    }
                //    //    else
                //    //        return 0;
                //    //}


                //    ImageSolutions.Shipping.WebShipRate.WebShipRateResponse result =
                //        ImageSolutions.Shipping.Restlet.getWebShipRate(UserWebsite.WebSite, this, ToAddressTrans, String.Empty);

                //    if (result.shippingMethods != null && result.shippingMethods.Count > 0)
                //    {
                //        return Convert.ToDouble(result.shippingMethods.Find(x => x.shipmethod == WebsiteShippingService.ShippingService.ServiceCode).rate);
                //    }

                //    else return 0;
                //}
            }
        }

        private double? mPartialShippingAmount = null;
        public double PartialShippingAmount
        {
            get
            {
                if (mPartialShippingAmount == null || mPartialShippingAmount == 0)
                {
                    if (WebsiteShippingService == null || ShippingAddress == null)
                    {
                        mPartialShippingAmount = 0;
                    }
                    else
                    {
                        //double dbTotalWeightInLbs = ShoppingCartLines.Sum(m => m.Quantity);
                        decimal dblPartialShippingAmount = 0;

                        ImageSolutions.Address.AddressTrans ToAddressTrans = new ImageSolutions.Address.AddressTrans();
                        ToAddressTrans.CompanyName = ShippingAddress.CompanyName;
                        ToAddressTrans.FirstName = ShippingAddress.FirstName;
                        ToAddressTrans.LastName = ShippingAddress.LastName;
                        ToAddressTrans.AddressLine1 = ShippingAddress.AddressLine1;
                        ToAddressTrans.AddressLine2 = ShippingAddress.AddressLine2;
                        ToAddressTrans.City = ShippingAddress.City;
                        ToAddressTrans.State = ShippingAddress.State;
                        ToAddressTrans.PostalCode = ShippingAddress.PostalCode;
                        ToAddressTrans.PhoneNumber = ShippingAddress.PhoneNumber;
                        ToAddressTrans.Email = ShippingAddress.Email;
                        ToAddressTrans.CountryCode = ShippingAddress.CountryCode;
                       
                        //Inventory 
                        int intInventoryQuantity = 0;
                        ImageSolutions.ShoppingCart.ShoppingCart InventoryShoppingCart = new ImageSolutions.ShoppingCart.ShoppingCart();
                        InventoryShoppingCart.ShoppingCartLines = new List<ImageSolutions.ShoppingCart.ShoppingCartLine>();
                        foreach (ImageSolutions.ShoppingCart.ShoppingCartLine _ShoppingCartLine in ShoppingCartLines.FindAll(x => x.Item.ItemType == "_inventoryItem"))
                        {
                            intInventoryQuantity += _ShoppingCartLine.Quantity;

                            ImageSolutions.ShoppingCart.ShoppingCartLine InventoryShoppingCartLine = new ImageSolutions.ShoppingCart.ShoppingCartLine();
                            InventoryShoppingCartLine.ItemID = _ShoppingCartLine.ItemID;
                            InventoryShoppingCartLine.Quantity = _ShoppingCartLine.Quantity;
                            InventoryShoppingCart.ShoppingCartLines.Add(InventoryShoppingCartLine);
                        }
                        if (intInventoryQuantity > 0)
                        {
                            //decimal decInventoryShippingAmount = 0;
                            ImageSolutions.Shipping.WebShipRate.WebShipRateResponse InventoryWebShipRateResponse = ImageSolutions.Shipping.Restlet.getWebShipRate(UserWebsite.WebSite, InventoryShoppingCart, ToAddressTrans, String.Empty);
                            if (InventoryWebShipRateResponse.shippingMethods != null && InventoryWebShipRateResponse.shippingMethods.Count > 0)
                            {
                                dblPartialShippingAmount += Convert.ToDecimal(InventoryWebShipRateResponse.shippingMethods.Find(x => x.shipmethod == WebsiteShippingService.ShippingService.ServiceCode).rate);
                            }
                        }

                        //Non-Inventory 
                        int intNonInventoryQuantity = 0;
                        ImageSolutions.ShoppingCart.ShoppingCart NonInventoryShoppingCart = new ImageSolutions.ShoppingCart.ShoppingCart();
                        NonInventoryShoppingCart.ShoppingCartLines = new List<ImageSolutions.ShoppingCart.ShoppingCartLine>();
                        foreach (ImageSolutions.ShoppingCart.ShoppingCartLine _ShoppingCartLine in ShoppingCartLines.FindAll(x => x.Item.ItemType != "_inventoryItem"))
                        {
                            intNonInventoryQuantity += _ShoppingCartLine.Quantity;

                            ImageSolutions.ShoppingCart.ShoppingCartLine NonInventoryShoppingCartLine = new ImageSolutions.ShoppingCart.ShoppingCartLine();
                            NonInventoryShoppingCartLine.ItemID = _ShoppingCartLine.ItemID;
                            NonInventoryShoppingCartLine.Quantity = _ShoppingCartLine.Quantity;
                            NonInventoryShoppingCart.ShoppingCartLines.Add(NonInventoryShoppingCartLine);
                        }
                        if (intNonInventoryQuantity > 0)
                        {
                            ImageSolutions.Shipping.WebShipRate.WebShipRateResponse NonInventoryWebShipRateResponse = ImageSolutions.Shipping.Restlet.getWebShipRate(UserWebsite.WebSite, NonInventoryShoppingCart, ToAddressTrans, String.Empty, true);
                            if (NonInventoryWebShipRateResponse.shippingMethods != null && NonInventoryWebShipRateResponse.shippingMethods.Count > 0)
                            {
                                dblPartialShippingAmount += Convert.ToDecimal(NonInventoryWebShipRateResponse.shippingMethods.Find(x => x.shipmethod == WebsiteShippingService.ShippingService.ServiceCode).rate);
                            }
                        }

                        //foreach (string _Group in ShoppingCartLines.FindAll(x => x.Item.ItemType != "_inventoryItem").Select(y => y.Item.VendorName).Distinct())
                        //{
                        //    ImageSolutions.ShoppingCart.ShoppingCart NonInventoryShoppingCart = new ImageSolutions.ShoppingCart.ShoppingCart();
                        //    NonInventoryShoppingCart.ShoppingCartLines = new List<ImageSolutions.ShoppingCart.ShoppingCartLine>();

                        //    int intNonInventoryQuantity = 0;

                        //    foreach (ImageSolutions.ShoppingCart.ShoppingCartLine _ShoppingCartLine in ShoppingCartLines.FindAll(x => x.Item.ItemType != "_inventoryItem" && x.Item.VendorName == _Group))
                        //    {
                        //        intNonInventoryQuantity += _ShoppingCartLine.Quantity;

                        //        ImageSolutions.ShoppingCart.ShoppingCartLine NonInventoryShoppingCartLine = new ImageSolutions.ShoppingCart.ShoppingCartLine();
                        //        NonInventoryShoppingCartLine.ItemID = _ShoppingCartLine.ItemID;
                        //        NonInventoryShoppingCartLine.Quantity = _ShoppingCartLine.Quantity;
                        //        NonInventoryShoppingCart.ShoppingCartLines.Add(NonInventoryShoppingCartLine);
                        //    }
                        //    decimal decNonInventoryShippingAmount = 0;
                        //    ImageSolutions.Shipping.WebShipRate.WebShipRateResponse NonInventoryWebShipRateResponse = ImageSolutions.Shipping.Restlet.getWebShipRate(UserWebsite.WebSite, NonInventoryShoppingCart, ToAddressTrans, String.Empty);
                        //    if (NonInventoryWebShipRateResponse.shippingMethods != null && NonInventoryWebShipRateResponse.shippingMethods.Count > 0)
                        //    {
                        //        dblPartialShippingAmount += Convert.ToDecimal(NonInventoryWebShipRateResponse.shippingMethods.Find(x => x.shipmethod == WebsiteShippingService.ShippingService.ServiceCode).rate);
                        //    }
                        //}

                        mPartialShippingAmount = Convert.ToDouble(dblPartialShippingAmount);
                    }
                }
                return Convert.ToDouble(mPartialShippingAmount);
            }
        }

        public double GetCompanyInvoicedAmount(string websiteid, bool istaxexempt)
        {            
            if (ShoppingCartLines.FindAll(x => x.Item.ItemWebsites.Find(y => y.WebsiteID == websiteid).IsCompanyInvoiced) != null 
                && ShoppingCartLines.FindAll(x => x.Item.ItemWebsites.Find(y => y.WebsiteID == websiteid).IsCompanyInvoiced).Count > 0)
            {
                if (ShoppingCartLines.Exists(x => !x.Item.ItemWebsites.Find(y => y.WebsiteID == websiteid).IsCompanyInvoiced))
                {
                    
                    return ShoppingCartLines.FindAll(x => x.Item.ItemWebsites.Find(y => y.WebsiteID == websiteid).IsCompanyInvoiced).Sum(x => x.LineTotal) * -1;
                }
                else
                {
                    if (istaxexempt)
                    {
                        return Math.Round(LineTotal + ShippingAmount + PromotionAmount - DiscountAmount, 2) * -1;
                    }
                    else
                    {
                        return Math.Round(LineTotal + ShippingAmount + SalesTaxAmount + PromotionAmount - DiscountAmount, 2) * -1;
                    }
                }
            }
            else
            {
                return 0;
            }
        }

        private double? mPromotionAmount = null;
        public double PromotionAmount
        {
            get
            {
                double dblCurrentTotal = LineTotal;
                double dblDiscountPercent = 0;
                double dblDiscountAnount = 0;
                double dblFreeShippingAmount = 0;

                foreach (ShoppingCartPromotion _ShoppingCartPromotion in ShoppingCartPromotions.FindAll(x => x.Promotion.DiscountPercent != null && x.Promotion.DiscountPercent != 0))
                {
                    dblDiscountPercent += Convert.ToDouble(_ShoppingCartPromotion.Promotion.DiscountPercent);
                }

                foreach (ShoppingCartPromotion _ShoppingCartPromotion in ShoppingCartPromotions.FindAll(x => x.Promotion.DiscountAmount != null && x.Promotion.DiscountAmount != 0))
                {
                    dblDiscountAnount += Convert.ToDouble(_ShoppingCartPromotion.Promotion.DiscountAmount);
                }

                foreach (ShoppingCartPromotion _ShoppingCartPromotion in ShoppingCartPromotions.FindAll(x => !string.IsNullOrEmpty(x.Promotion.FreeShippingServiceID)))
                {
                    if(WebsiteShippingService != null && WebsiteShippingService.ShippingService != null && WebsiteShippingService.ShippingService.ShippingServiceID == _ShoppingCartPromotion.Promotion.FreeShippingServiceID)
                    {
                        dblFreeShippingAmount = ShippingAmount;
                    }
                }

                double PromotionTotal = Convert.ToDouble((dblCurrentTotal * dblDiscountPercent) + dblDiscountAnount - dblFreeShippingAmount);

                if (PromotionTotal * -1 > LineTotal)
                {
                    double dblPromotionShipping = 0;
                    double dblPromotionTax = 0;
                    foreach (ImageSolutions.ShoppingCart.ShoppingCartPromotion _ShoppingCartPromotion in ShoppingCartPromotions.FindAll(x => x.Promotion.ExcludeShippingAndTax))
                    {
                        dblPromotionShipping = ShippingAmount;
                        dblPromotionTax = SalesTaxAmount;                        
                    }

                    if (dblPromotionShipping + dblPromotionTax > 0)
                    {
                        PromotionTotal = LineTotal * -1;
                    }
                }

                return PromotionTotal;
            }
        }
        public void DeletePromotion(string promotionid)
        {
            ShoppingCartPromotions.Find(x => x.PromotionID == promotionid).Delete();
        }

        private double? mDiscountAmount = null;
        public double DiscountAmount
        {
            get
            {
                double decDiscountAmount = 0;

                if (this.UserWebsite.WebSite.DiscountPerItem != null && Convert.ToDecimal(this.UserWebsite.WebSite.DiscountPerItem) > 0)
                {
                    foreach (ImageSolutions.ShoppingCart.ShoppingCartLine _ShoppingCartLine in ShoppingCartLines)
                    {
                        if (Convert.ToDecimal(_ShoppingCartLine.UnitPrice) > Convert.ToDecimal(this.UserWebsite.WebSite.DiscountPerItem))
                        {
                            decDiscountAmount += Convert.ToDouble(UserWebsite.WebSite.DiscountPerItem) * _ShoppingCartLine.Quantity;
                        }
                    }
                }

                foreach (ImageSolutions.ShoppingCart.ShoppingCartLine _ShoppingCartLine in ShoppingCartLines)
                {
                    if (_ShoppingCartLine.Item.DiscountAmount != null && _ShoppingCartLine.Item.DiscountAmount > 0)
                    {
                        decDiscountAmount += Convert.ToDouble(_ShoppingCartLine.Item.DiscountAmount) * _ShoppingCartLine.Quantity;
                    }
                }

                return decDiscountAmount;
            }
        }

        public bool AddGiftCertificateOrPromotionCode(string promotioncode)
        {
            Promotion.Promotion promotion = null;

            try
            {
                promotion = Promotion.Promotion.GetPromotionByPromotionCode(UserWebsite.WebsiteID, promotioncode);

                if (promotion == null) throw new Exception("This promotion code is not valid.");
                if(promotion.IsValid(this))
                {
                    ShoppingCartPromotion ExistsShoppingCartPromotion = new ShoppingCartPromotion();
                    ShoppingCartPromotionFilter ShoppingCartPromotionFilter = new ShoppingCartPromotionFilter();
                    ShoppingCartPromotionFilter.ShoppingCartID = new Database.Filter.StringSearch.SearchFilter();
                    ShoppingCartPromotionFilter.ShoppingCartID.SearchString = ShoppingCartID;
                    ShoppingCartPromotionFilter.PromotionID = new Database.Filter.StringSearch.SearchFilter();
                    ShoppingCartPromotionFilter.PromotionID.SearchString = promotion.PromotionID;
                    ExistsShoppingCartPromotion = ShoppingCartPromotion.GetShoppingCartPromotion(ShoppingCartPromotionFilter);

                    if(ExistsShoppingCartPromotion == null)
                    {
                        ShoppingCartPromotion ShoppingCartPromotion = new ShoppingCartPromotion();
                        ShoppingCartPromotion.ShoppingCartID = ShoppingCartID;
                        ShoppingCartPromotion.PromotionID = promotion.PromotionID;
                        ShoppingCartPromotion.CreatedBy = UserWebsite.UserInfoID;
                        ShoppingCartPromotion.Create();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                promotion = null;
            }
            return true;
        }

        //public double Total
        //{
        //    get
        //    {
        //        return Math.Round(LineTotal + ShippingAmount + SalesTaxAmount, 2);
        //    }
        //}
        public double GetTotal(string websiteid, bool istaxexempt)
        {
            double dblTotal = 0;

            if (istaxexempt)
            {
                dblTotal = Math.Round(LineTotal + ShippingAmount + PromotionAmount - DiscountAmount + GetCompanyInvoicedAmount(websiteid, istaxexempt), 2);
            }
            else
            {
                dblTotal = Math.Round(LineTotal + ShippingAmount + SalesTaxAmount + PromotionAmount - DiscountAmount + GetCompanyInvoicedAmount(websiteid, istaxexempt), 2);
            }

            return dblTotal; // dblTotal < 0 ? 0 : dblTotal;
        }

        public double GetTotalWithPartialShipping(string websiteid, bool istaxexempt)
        {
            double dblTotal = 0;

            if (istaxexempt)
            {
                dblTotal = Math.Round(LineTotal + PartialShippingAmount + PromotionAmount - DiscountAmount + GetCompanyInvoicedAmount(websiteid, istaxexempt), 2);
            }
            else
            {
                dblTotal = Math.Round(LineTotal + PartialShippingAmount + SalesTaxAmount + PromotionAmount - DiscountAmount + GetCompanyInvoicedAmount(websiteid, istaxexempt), 2);
            }

            return dblTotal; // dblTotal < 0 ? 0 : dblTotal;
        }

        public double LineTotalBeforeDiscount
        {
            get
            {
                return Math.Round(LineTotal + ShippingAmount, 2);
            }
        }

        private List<ShoppingCartLine> mShoppingCartLines = null;
        public List<ShoppingCartLine> ShoppingCartLines
        {
            get
            {
                if (mShoppingCartLines == null && !string.IsNullOrEmpty(ShoppingCartID))
                {
                    ShoppingCartLineFilter objFilter = null;

                    try
                    {
                        objFilter = new ShoppingCartLineFilter();
                        objFilter.ShoppingCartID = new Database.Filter.StringSearch.SearchFilter();
                        objFilter.ShoppingCartID.SearchString = ShoppingCartID;

                        mShoppingCartLines = ShoppingCartLine.GetShoppingCartLines(objFilter);

                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    finally
                    {
                        objFilter = null;
                    }
                }
                return mShoppingCartLines;
            }
            set
            {
                mShoppingCartLines = value;
            }
        }

        public ShoppingCart()
        {
        }
        public ShoppingCart(string ShoppingCartID)
        {
            this.ShoppingCartID = ShoppingCartID;
            Load();
        }
        public ShoppingCart(DataRow objRow)
        {
            Load(objRow);
        }

        protected override void Load()
        {
            base.Load();

            DataSet objData = null;
            string strSQL = string.Empty;

            try
            {
                strSQL = "SELECT * " +
                         "FROM ShoppingCart (NOLOCK) " +
                         "WHERE ShoppingCartID=" + Database.HandleQuote(ShoppingCartID);
                objData = Database.GetDataSet(strSQL);
                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    Load(objData.Tables[0].Rows[0]);
                }
                else
                {
                    throw new Exception("ShoppingCartID=" + ShoppingCartID + " is not found");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                objData = null;
            }
        }
        private void Load(DataRow objRow)
        {
            DataColumnCollection objColumns = null;

            try
            {
                objColumns = objRow.Table.Columns;

                if (objColumns.Contains("ShoppingCartID")) ShoppingCartID = Convert.ToString(objRow["ShoppingCartID"]);
                if (objColumns.Contains("GUID")) GUID = Convert.ToString(objRow["GUID"]);
                if (objColumns.Contains("UserWebsiteID")) UserWebsiteID = Convert.ToString(objRow["UserWebsiteID"]);
                if (objColumns.Contains("SalesOrderID")) SalesOrderID = Convert.ToString(objRow["SalesOrderID"]);
                if (objColumns.Contains("CheckoutSession")) CheckoutSession = Convert.ToString(objRow["CheckoutSession"]);
                if (objColumns.Contains("PunchoutSessionID")) PunchoutSessionID = Convert.ToString(objRow["PunchoutSessionID"]);
                if (objColumns.Contains("PackageID")) PackageID = Convert.ToString(objRow["PackageID"]);
                if (objColumns.Contains("ExcludeOptional")) ExcludeOptional = Convert.ToBoolean(objRow["ExcludeOptional"]);
                if (objColumns.Contains("UpdatedOn") && objRow["UpdatedOn"] != DBNull.Value) UpdatedOn = Convert.ToDateTime(objRow["UpdatedOn"]);
                if (objColumns.Contains("CreatedOn")) CreatedOn = Convert.ToDateTime(objRow["CreatedOn"]);

                if (string.IsNullOrEmpty(ShoppingCartID)) throw new Exception("Missing ShoppingCartID in the datarow");
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                objColumns = null;
            }
        }

        public override bool Create()
        {
            SqlConnection objConn = null;
            SqlTransaction objTran = null;

            try
            {
                objConn = new SqlConnection(Database.DefaultConnectionString);
                objConn.Open();
                objTran = objConn.BeginTransaction();
                Create(objConn, objTran);
                objTran.Commit();
            }
            catch (Exception ex)
            {
                if (objTran != null && objTran.Connection != null) objTran.Rollback();
                throw ex;
            }
            finally
            {
                if (objTran != null) objTran.Dispose();
                objTran = null;
                if (objConn != null) objConn.Dispose();
                objConn = null;
            }
            return true;
        }

        public override bool Create(SqlConnection objConn, SqlTransaction objTran)
        {
            base.Create();

            if (!IsActive) return true;

            Hashtable dicParam = new Hashtable();
            try
            {
                if (string.IsNullOrEmpty(UserWebsiteID)) throw new Exception("UserWebsiteID is required");
                if (!IsNew) throw new Exception("Create cannot be performed, ShoppingCartID already exists");

                dicParam["UserWebsiteID"] = UserWebsiteID;
                dicParam["SalesOrderID"] = SalesOrderID;
                ShoppingCartID = Database.ExecuteSQLWithIdentity(Database.GetInsertSQL(dicParam, "ShoppingCart"), objConn, objTran).ToString();

                foreach (ShoppingCartLine objShoppingCartLine in ShoppingCartLines)
                {
                    objShoppingCartLine.ShoppingCartID = ShoppingCartID;
                    objShoppingCartLine.Create(objConn, objTran);
                }

                Load(objConn, objTran);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                dicParam = null;
            }
            return true;
        }

        public override bool Update()
        {
            SqlConnection objConn = null;
            SqlTransaction objTran = null;

            try
            {
                objConn = new SqlConnection(Database.DefaultConnectionString);
                objConn.Open();
                objTran = objConn.BeginTransaction();
                Update(objConn, objTran);
                objTran.Commit();
            }
            catch (Exception ex)
            {
                if (objTran != null && objTran.Connection != null) objTran.Rollback();
                throw ex;
            }
            finally
            {
                if (objTran != null) objTran.Dispose();
                objTran = null;
                if (objConn != null) objConn.Dispose();
                objConn = null;
            }
            return true;
        }

        public override bool Update(SqlConnection objConn, SqlTransaction objTran)
        {
            base.Update();

            if (!IsActive) return Delete(objConn, objTran);

            Hashtable dicParam = new Hashtable();
            Hashtable dicWParam = new Hashtable();

            try
            {
                if (string.IsNullOrEmpty(UserWebsiteID)) throw new Exception("UserWebsiteID is required");
                if (IsNew) throw new Exception("Update cannot be performed, ShoppingCartID is missing");

                dicParam["UserWebsiteID"] = UserWebsiteID;
                dicParam["SalesOrderID"] = SalesOrderID;
                dicParam["CheckoutSession"] = CheckoutSession;
                dicParam["PunchoutSessionID"] = PunchoutSessionID;
                dicParam["PackageID"] = PackageID;
                dicParam["ExcludeOptional"] = ExcludeOptional;
                dicParam["UpdatedOn"] = DateTime.UtcNow;
                dicWParam["ShoppingCartID"] = ShoppingCartID;
                
                Database.ExecuteSQL(Database.GetUpdateSQL(dicParam, dicWParam, "ShoppingCart"), objConn, objTran);

                foreach (ShoppingCartLine objShoppingCartLine in ShoppingCartLines)
                {
                    objShoppingCartLine.Update(objConn, objTran);
                }

                Load(objConn, objTran);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                dicParam = null;
                dicWParam = null;
            }
            return true;
        }

        public override bool Delete()
        {
            SqlConnection objConn = null;
            SqlTransaction objTran = null;

            try
            {
                objConn = new SqlConnection(Database.DefaultConnectionString);
                objConn.Open();
                objTran = objConn.BeginTransaction();
                Delete(objConn, objTran);
                objTran.Commit();
            }
            catch (Exception ex)
            {
                if (objTran != null && objTran.Connection != null) objTran.Rollback();
                throw ex;
            }
            finally
            {
                if (objTran != null) objTran.Dispose();
                objTran = null;
                if (objConn != null) objConn.Dispose();
                objConn = null;
            }
            return true;
        }

        public override bool Delete(SqlConnection objConn, SqlTransaction objTran)
        {
            base.Delete();

            Hashtable dicDParam = new Hashtable();

            try
            {
                if (IsNew) throw new Exception("Delete cannot be performed, ShoppingCartID is missing");

                dicDParam["ShoppingCartID"] = ShoppingCartID;
                Database.ExecuteSQL(Database.GetDeleteSQL(dicDParam, "ShoppingCart"), objConn, objTran);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                dicDParam = null;
            }
            return true;
        }

        public static ShoppingCart GetShoppingCart(ShoppingCartFilter Filter)
        {
            List<ShoppingCart> objShoppingCarts = null;
            ShoppingCart objReturn = null;

            try
            {
                objShoppingCarts = GetShoppingCarts(Filter);
                if (objShoppingCarts != null && objShoppingCarts.Count >= 1) objReturn = objShoppingCarts[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                objShoppingCarts = null;
            }
            return objReturn;
        }

        public static List<ShoppingCart> GetShoppingCarts()
        {
            int intTotalCount = 0;
            return GetShoppingCarts(null, null, null, out intTotalCount);
        }

        public static List<ShoppingCart> GetShoppingCarts(ShoppingCartFilter Filter)
        {
            int intTotalCount = 0;
            return GetShoppingCarts(Filter, null, null, out intTotalCount);
        }

        public static List<ShoppingCart> GetShoppingCarts(ShoppingCartFilter Filter, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            return GetShoppingCarts(Filter, string.Empty, true, PageSize, PageNumber, out TotalRecord);
        }

        public static List<ShoppingCart> GetShoppingCarts(ShoppingCartFilter Filter, string SortExpression, bool SortAscending, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            List<ShoppingCart> objReturn = null;
            ShoppingCart objNew = null;
            DataSet objData = null;
            string strSQL = string.Empty;

            try
            {
                TotalRecord = 0;

                objReturn = new List<ShoppingCart>();

                strSQL = "SELECT * " +
                         "FROM ShoppingCart (NOLOCK) " +
                         "WHERE 1=1  ";

                if (Filter != null)
                {
                    if (Filter.ShoppingCartID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.ShoppingCartID, "ShoppingCartID");
                    if (Filter.UserWebsiteID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.UserWebsiteID, "UserWebsiteID");
                    if (Filter.SalesOrderID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.SalesOrderID, "SalesOrderID");
                    if (Filter.PunchoutSessionID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.PunchoutSessionID, "PunchoutSessionID");
                }

                if (PageSize != null && PageNumber != null) strSQL = Database.GetPagingSQL(strSQL, string.IsNullOrEmpty(SortExpression) ? "ShoppingCartID" : Utility.CustomSorting.GetSortExpression(typeof(ShoppingCart), SortExpression), string.IsNullOrEmpty(SortExpression) ? false : SortAscending, PageSize.Value, PageNumber.Value);
                objData = Database.GetDataSet(strSQL);

                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < objData.Tables[0].Rows.Count; i++)
                    {
                        objNew = new ShoppingCart(objData.Tables[0].Rows[i]);
                        objNew.IsLoaded = true;
                        objReturn.Add(objNew);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                objData = null;
            }
            return objReturn;
        }
    }
}
