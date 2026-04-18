using ImageSolutions;
using ImageSolutions.Address;
using ImageSolutions.Avalara;
using ImageSolutions.Marketing;
using ImageSolutions.SalesOrder;
using ImageSolutions.User;
using ImageSolutionsWebsite.Library;
using ImageSolutionsWebsite.MyAccount;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace ImageSolutionsWebsite
{
    public partial class CheckOut : BasePageUserAccountAuth
    {
        //Test Credit Card: 4242424242424242 - Visa - 3 digit CVC

        protected void Page_Load(object sender, EventArgs e)
        {
            if (CurrentWebsite.IsPunchout)
            {
                Response.Redirect("/ShoppingCart.aspx");
            }

            if (CurrentUser.CurrentUserWebSite.EnableShipToAccount && !string.IsNullOrEmpty(CurrentUser.CurrentUserWebSite.CurrentUserAccount.Account.DefaultShippingAddressBookID))
            {
                pnlShipToAccount.Visible = true;
                UpdateAccount();
            }

            if (!IsPostBack)
            {
                Initalize();
            }

            if (CurrentUser.CurrentUserWebSite.ShoppingCart == null
                || CurrentUser.CurrentUserWebSite.ShoppingCart.ShoppingCartLines == null
                || CurrentUser.CurrentUserWebSite.ShoppingCart.ShoppingCartLines.Count == 0)
            {
                Response.Redirect("/shoppingcart.aspx");
                //throw new Exception("Cart is Empty");
            }
        }

        protected void UpdateAccount()
        {
            ucAccountSearchModal.SendMessageToThePage += delegate (string message)
            {
                try
                {
                    hfAccountID.Value = message;

                    if (!string.IsNullOrEmpty(hfAccountID.Value))
                    {
                        ImageSolutions.Account.Account Account = new ImageSolutions.Account.Account(hfAccountID.Value);

                        if (Account != null && !string.IsNullOrEmpty(Account.AccountID))
                        {
                            txtAccount.Text = Account.AccountName;

                            if (ddlShippingAddress.Items.FindByValue(Account.DefaultShippingAddressBookID) == null)
                            {
                                ddlShippingAddress.Items.Add(new ListItem(Account.AccountName, Account.DefaultShippingAddressBookID));
                            }

                            ddlShippingAddress.SelectedValue = Account.DefaultShippingAddressBookID;
                            SetShippingAddress();
                            pnlShippingAddressButton.Visible = false;
                        }
                    }
                    else
                    {
                        txtAccount.Text = string.Empty;
                    }
                }
                catch (Exception ex)
                {
                    WebUtility.DisplayJavascriptMessage(this, ex.Message);
                }
            };
        }

        protected void btnAccountSearch_Click(object sender, EventArgs e)
        {
            ucAccountSearchModal.WebsiteID = CurrentWebsite.WebsiteID;
            ucAccountSearchModal.Show();
        }

        protected void Initalize()
        {
            //ucLoading.Visible = true;

            string strErrorMessage = string.Empty;

            try
            {

                if (CurrentWebsite.EnablePackagePayment)
                {
                    if (!ValidatePackage())
                    {
                        if (CurrentUser.CurrentUserWebSite.ShoppingCart.ShoppingCartLines != null && CurrentUser.CurrentUserWebSite.ShoppingCart.ShoppingCartLines.Count > 0)
                        {
                            foreach (ImageSolutions.ShoppingCart.ShoppingCartLine _ShoppingCartLine in CurrentUser.CurrentUserWebSite.ShoppingCart.ShoppingCartLines)
                            {
                                _ShoppingCartLine.Delete();
                            }
                        }

                        Response.Redirect("/myaccount/dashboard.aspx?error=invaild-package-quantity");
                        //throw new Exception("Invalid Package Items");
                    }
                }

                hfUserGUID.Value = CurrentUser.GUID;
                hfUserWebsiteID.Value = CurrentUser.CurrentUserWebSite.UserWebsiteID;

                btnChooseExistingShippingAddress.Visible = !CurrentUser.IsGuest;
                btnChooseExistingBillingAddress.Visible = !CurrentUser.IsGuest;
                pnlCreateAccount.Visible = CurrentUser.IsGuest
                    && !(Convert.ToString(ConfigurationManager.AppSettings["Environment"]) == "staging" && CurrentWebsite.WebsiteID == "65")
                    && !(Convert.ToString(ConfigurationManager.AppSettings["Environment"]) == "production" && CurrentWebsite.WebsiteID == "31");

                //this.pnlSMSOptIn.Visible = CurrentWebsite.EnableSMSOptIn;
                //this.txtMobileNumber.Text = CurrentUser.CurrentUserWebSite.SMSMobileNumber;
                //this.chkSMSOptIn.Checked = CurrentUser.CurrentUserWebSite.SMSOptIn;

                BindCustomField();

                if (CurrentUser.CurrentUserWebSite.DisablePlaceOrder)
                {
                    btnPlaceOrder.Enabled = false;
                }

                foreach (ImageSolutions.ShoppingCart.ShoppingCartLine objShoppingCartLine in CurrentUser.CurrentUserWebSite.ShoppingCart.ShoppingCartLines)
                {

                    ImageSolutions.Item.Item Item = new ImageSolutions.Item.Item(objShoppingCartLine.ItemID);

                    //Reset Unit Prices
                    ImageSolutions.Item.MyGroupItem MyGroupItemWebsite = CurrentUser.CurrentUserWebSite.CurrentUserAccount.MyGroupItems.Find(x => x.ItemID == Item.ItemID);
                    if (MyGroupItemWebsite != null && objShoppingCartLine.UnitPrice != MyGroupItemWebsite.Price)
                    {
                        objShoppingCartLine.UnitPrice = MyGroupItemWebsite.Price;
                        objShoppingCartLine.Update();
                    }

                    if (
                        (!Item.AllowBackOrder || CurrentWebsite.DisallowBackOrder)
                        &&
                        (
                            (Item.IsNonInventory && objShoppingCartLine.Quantity > Item.VendorInventory)
                            ||
                            (!Item.IsNonInventory && objShoppingCartLine.Quantity > Item.QuantityAvailable)
                        )
                    )
                    {
                        strErrorMessage = string.Format(@"{0}\r\n{1}"
                            , strErrorMessage
                            , String.Format("Not enough inventory available {0} ({1})", Item.SalesDescription, Item.IsNonInventory ? Item.VendorInventory : Item.QuantityAvailable));
                        //throw new Exception(String.Format("Not enough inventory available {0} ({1})", Item.ItemName, Item.QuantityAvailable));
                    }

                    if (Item.InternalID.ToLower().Contains("inactive"))
                    {
                        throw new Exception(String.Format("Invalid Item: {0} ({1})", Item.SalesDescription, Item.QuantityAvailable));
                    }
                }


                if (CurrentWebsite.AllowPartialShipping)
                {
                    pnlParitalShippingOption.Visible = true;
                    chkPartialShipping.Checked = true;
                }
                else
                {
                    pnlParitalShippingOption.Visible = false;
                    chkPartialShipping.Checked = false;
                }

                BindShippingAddress();
                BindBillingAddress();

                BindCountry();

                for (int i = DateTime.Now.Year; i < DateTime.Now.Year + 20; i++)
                {
                    this.ddlCCYear.Items.Add(new ListItem(i.ToString(), i.ToString()));
                }

                //DT - Exclude Shipping Item
                foreach (ImageSolutions.ShoppingCart.ShoppingCartLine _ShoppingCartLine in CurrentUser.CurrentUserWebSite.ShoppingCart.ShoppingCartLines)
                {
                    if (_ShoppingCartLine.Item.ParentItem != null
                        && _ShoppingCartLine.Item.ParentItem.WebsiteTabItems != null
                        && _ShoppingCartLine.Item.ParentItem.WebsiteTabItems.Count > 0
                        && _ShoppingCartLine.Item.ParentItem.WebsiteTabItems[0].WebsiteTab != null
                        && _ShoppingCartLine.Item.ParentItem.WebsiteTabItems[0].WebsiteTab.ExcludeShipping)
                    {
                        pnlShippingAddressMain.Visible = false;
                        pnlShippingMethod.Visible = false;

                        //ddlShippingAddress.SelectedValue = String.Empty;
                        //ddlShippingAddress.SelectedValue = CurrentUser.CurrentUserWebSite.CurrentUserAccount.Account.DefaultShippingAddressBookID;
                        //SetShippingAddress();
                        //ddlWebsiteShippingService.SelectedValue = String.Empty;
                        //CurrentUser.CurrentUserWebSite.ShoppingCart.WebsiteShippingServiceID = CurrentWebsite.WebsiteShippingServices.Find(x => x.ShippingService.ServiceName == "Customer Pick Up").WebsiteShippingServiceID;
                        //ddlWebsiteShippingService.Items.Clear();

                        ImageSolutions.Address.AddressBook AddressBook = new ImageSolutions.Address.AddressBook(CurrentWebsite.CorporateAddressBookID);
                        if (AddressBook != null && !string.IsNullOrEmpty(AddressBook.AddressBookID))
                        {
                            ddlShippingAddress.Items.Add(new ListItem(AddressBook.GetDisplayFormat(false), AddressBook.AddressBookID));
                            ddlShippingAddress.SelectedValue = CurrentWebsite.CorporateAddressBookID;
                        }
                        UpdateShoppingCart();

                        if (!string.IsNullOrEmpty(CurrentUser.DefaultBillingAddressBookID))
                        {
                            ddlBillingAddress.SelectedValue = CurrentUser.DefaultBillingAddressBookID;
                            SetBillingAddress();
                        }
                        //else if (!string.IsNullOrEmpty(CurrentUser.CurrentUserWebSite.CurrentUserAccount.Account.DefaultShippingAddressBookID) 
                        //    && !CurrentUser.IsGuest)
                        //{
                        //    ddlBillingAddress.SelectedValue = CurrentUser.CurrentUserWebSite.CurrentUserAccount.Account.DefaultShippingAddressBookID;
                        //    SetBillingAddress();
                        //}
                        else
                        {
                            btnEditBillingAddress.Visible = !string.IsNullOrEmpty(ddlBillingAddress.SelectedValue);
                        }
                        cbSameAsShippingAddress.Enabled = false;
                    }
                }

                if (pnlShippingAddressMain.Visible)
                {

                    if (CurrentUser.CurrentUserWebSite.EnableShipToAccount && !string.IsNullOrEmpty(CurrentUser.CurrentUserWebSite.CurrentUserAccount.Account.DefaultShippingAddressBookID))
                    {
                        txtAccount.Text = CurrentUser.CurrentUserWebSite.CurrentUserAccount.Account.AccountName;
                        ddlShippingAddress.SelectedValue = CurrentUser.CurrentUserWebSite.CurrentUserAccount.Account.DefaultShippingAddressBookID;
                        SetShippingAddress();
                        pnlShippingAddressButton.Visible = false;
                    }
                    else

                    if (!string.IsNullOrEmpty(CurrentUser.CurrentUserWebSite.AddressPermission))
                    {
                        if (CurrentUser.CurrentUserWebSite.AddressPermission == "Account")
                        {
                            ddlShippingAddress.SelectedValue = CurrentUser.CurrentUserWebSite.CurrentUserAccount.Account.DefaultShippingAddressBookID;
                            SetShippingAddress();
                            pnlShippingAddressButton.Visible = false;

                            ddlBillingAddress.SelectedValue = CurrentUser.CurrentUserWebSite.CurrentUserAccount.Account.DefaultShippingAddressBookID;
                            SetBillingAddress();

                            //pnlBillingAddressButton.Visible = false;
                            cbSameAsShippingAddress.Enabled = false;
                            if (CurrentWebsite.EnableCreditCard && !CurrentUser.CurrentUserWebSite.CurrentUserAccount.Account.DoNotAllowCreditCard)
                            {
                                pnlBillingAddressButton.Visible = true;
                                //cbSameAsShippingAddress.Enabled = true;
                            }
                            else
                            {

                                pnlBillingAddressButton.Visible = false;
                                //cbSameAsShippingAddress.Enabled = false;
                            }
                        }
                        else if (CurrentUser.CurrentUserWebSite.AddressPermission == "Default")
                        {
                            ddlShippingAddress.SelectedValue = CurrentUser.CurrentUserWebSite.DefaultShippingAddressID;
                            SetShippingAddress();
                            pnlShippingAddressButton.Visible = false;


                            ddlBillingAddress.SelectedValue = CurrentUser.CurrentUserWebSite.DefaultBillingAddressID;
                            SetBillingAddress();

                            if (CurrentWebsite.EnableCreditCard && !CurrentUser.CurrentUserWebSite.CurrentUserAccount.Account.DoNotAllowCreditCard)
                            {
                                pnlBillingAddressButton.Visible = true;
                                cbSameAsShippingAddress.Enabled = true;
                            }
                            else
                            {

                                pnlBillingAddressButton.Visible = false;
                                cbSameAsShippingAddress.Enabled = false;
                            }
                        }
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(CurrentUser.DefaultShippingAddressBookID))
                        {
                            ddlShippingAddress.SelectedValue = CurrentUser.DefaultShippingAddressBookID;
                            SetShippingAddress();
                        }
                        else if (!string.IsNullOrEmpty(CurrentUser.CurrentUserWebSite.CurrentUserAccount.Account.DefaultShippingAddressBookID) && !CurrentUser.IsGuest)
                        {
                            ddlShippingAddress.SelectedValue = CurrentUser.CurrentUserWebSite.CurrentUserAccount.Account.DefaultShippingAddressBookID;
                            SetShippingAddress();
                        }
                        else
                        {
                            btnEditShippingAddress.Visible = !string.IsNullOrEmpty(ddlShippingAddress.SelectedValue);
                        }

                        if (!string.IsNullOrEmpty(CurrentUser.DefaultBillingAddressBookID))
                        {
                            ddlBillingAddress.SelectedValue = CurrentUser.DefaultBillingAddressBookID;
                            SetBillingAddress();
                        }
                        else if (!string.IsNullOrEmpty(CurrentUser.CurrentUserWebSite.CurrentUserAccount.Account.DefaultShippingAddressBookID) && !CurrentUser.IsGuest)
                        {
                            ddlBillingAddress.SelectedValue = CurrentUser.CurrentUserWebSite.CurrentUserAccount.Account.DefaultShippingAddressBookID;
                            SetBillingAddress();
                        }
                        else
                        {
                            btnEditBillingAddress.Visible = !string.IsNullOrEmpty(ddlBillingAddress.SelectedValue);
                        }
                    }
                }


                if (CurrentUser.CurrentUserWebSite.MyBudgetAssignments != null
                    && CurrentUser.CurrentUserWebSite.MyBudgetAssignments.FindAll(x =>
                        !x.BudgetAssignment.InActive
                        && x.BudgetAssignment.Budget.StartDate <= DateTime.UtcNow
                        && x.BudgetAssignment.Budget.EndDate >= DateTime.UtcNow.AddDays(-1)
                        && (x.Balance > 0 || (x.BudgetAssignment.Budget.AllowOverBudget && !x.BudgetAssignment.Budget.ExcludeNoAmountBudget))
                    ).Count > 0)
                {
                    List<ImageSolutions.Budget.MyBudgetAssignment> MyBudgetAssignments = CurrentUser.CurrentUserWebSite.MyBudgetAssignments.FindAll(x =>
                        !x.BudgetAssignment.InActive
                        && x.BudgetAssignment.Budget.StartDate <= DateTime.UtcNow
                        && x.BudgetAssignment.Budget.EndDate >= DateTime.UtcNow.AddDays(-1)
                        && (x.Balance > 0 || (x.BudgetAssignment.Budget.AllowOverBudget && !x.BudgetAssignment.Budget.ExcludeNoAmountBudget))
                    //&& (
                    //    x.BudgetAssignment.Budget.AllowOverBudget ||
                    //    x.Balance >= CurrentUser.CurrentUserWebSite.ShoppingCart.GetTotal(CurrentUser.CurrentUserWebSite.IsTaxExempt)
                    //)
                    );

                    this.ddlMyBudgetAssignment.DataSource = MyBudgetAssignments;
                    this.ddlMyBudgetAssignment.DataBind();
                    phBudgetOption.Visible = true;

                    if (CurrentUser.CurrentUserWebSite.ShoppingCart.ShoppingCartLines.Exists(x =>
                        x.Item.ParentItem != null
                        && x.Item.ParentItem.WebsiteTabItems != null
                        && x.Item.ParentItem.WebsiteTabItems.Exists(y =>
                            y.WebsiteTab.WebsiteID == CurrentWebsite.WebsiteID && y.WebsiteTab.DoNotAllowMixCart
                            )
                        )
                    )
                    {
                        phBudgetOption.Visible = false;
                    }
                }
                else
                {
                    phBudgetOption.Visible = false;
                }

                SetInvoicePayment();

                //this.ddlUserCreditCard.DataSource = CurrentUser.UserCreditCards.FindAll(x => x.RemainingBalance == null || x.RemainingBalance > CurrentUser.CurrentUserWebSite.ShoppingCart.Total);

                this.phEnableCreditCard.Visible = CurrentWebsite.EnableCreditCard && !CurrentUser.CurrentUserWebSite.CurrentUserAccount.Account.DoNotAllowCreditCard;
                if (CurrentUser.UserCreditCards != null && CurrentUser.UserCreditCards.Count > 0)
                {
                    this.ddlUserCreditCard.DataSource = CurrentUser.UserCreditCards;
                    this.ddlUserCreditCard.DataBind();
                    phUserCreditCardOption.Visible = true;
                }
                else
                {
                    phUserCreditCardOption.Visible = false;
                }

                this.rptShoppingCart.DataSource = CurrentUser.CurrentUserWebSite.ShoppingCart.ShoppingCartLines.FindAll(x => !x.Item.ItemWebsites.Find(y => y.WebsiteID == CurrentWebsite.WebsiteID).IsCompanyInvoiced);
                this.rptShoppingCart.DataBind();

                if (CurrentUser.CurrentUserWebSite.ShoppingCart.ShoppingCartLines.FindAll(x => x.Item.ItemWebsites.Find(y => y.WebsiteID == CurrentWebsite.WebsiteID).IsCompanyInvoiced) != null
                    && CurrentUser.CurrentUserWebSite.ShoppingCart.ShoppingCartLines.FindAll(x => x.Item.ItemWebsites.Find(y => y.WebsiteID == CurrentWebsite.WebsiteID).IsCompanyInvoiced).Count > 0)
                {
                    pnlCompanyInvoice.Visible = true;
                    this.rptShoppingCartCompanyInvoice.DataSource = CurrentUser.CurrentUserWebSite.ShoppingCart.ShoppingCartLines.FindAll(x => x.Item.ItemWebsites.Find(y => y.WebsiteID == CurrentWebsite.WebsiteID).IsCompanyInvoiced);
                    this.rptShoppingCartCompanyInvoice.DataBind();
                }


                if (CurrentUser.CurrentUserWebSite.ShoppingCart.DiscountAmount > 0)
                {
                    pnlDiscountAmount.Visible = true;
                    litDiscountAmount.Text = string.Format("{0:c}", CurrentUser.CurrentUserWebSite.ShoppingCart.DiscountAmount * -1);
                }
                else
                {
                    pnlDiscountAmount.Visible = false;
                }

                if (CurrentWebsite.EnablePromoCode)
                {
                    pnlPromotion.Visible = true;
                    this.gvPromotions.DataSource = CurrentUser.CurrentUserWebSite.ShoppingCart.ShoppingCartPromotions;
                    this.gvPromotions.DataBind();
                    pnlPromotionForm.Visible = CurrentUser.CurrentUserWebSite.ShoppingCart.ShoppingCartPromotions == null || CurrentUser.CurrentUserWebSite.ShoppingCart.ShoppingCartPromotions.Count < 1;
                    liPromo.Visible = true;
                }
                else
                {
                    foreach (ImageSolutions.ShoppingCart.ShoppingCartPromotion _ShoppingCartPromotion in CurrentUser.CurrentUserWebSite.ShoppingCart.ShoppingCartPromotions)
                    {
                        _ShoppingCartPromotion.Delete();
                    }

                    pnlPromotion.Visible = false;
                    liPromo.Visible = false;
                }

                //this.ddlWebsiteShippingService.DataSource = CurrentUser.CurrentUserWebSite.WebSite.WebsiteShippingServices;
                //this.ddlWebsiteShippingService.DataBind();
                //this.ddlWebsiteShippingService.Items.Insert(0, new ListItem());

                this.litSubtotal.Text = string.Format("{0:c}", CurrentUser.CurrentUserWebSite.ShoppingCart.LineTotal);

                double dblShoppingCartTotal = CurrentUser.CurrentUserWebSite.ShoppingCart.GetTotal(CurrentWebsite.WebsiteID, CurrentUser.CurrentUserWebSite.IsTaxExempt);
                if (CurrentWebsite.EnableIPD && IsInternational())
                {
                    dblShoppingCartTotal = dblShoppingCartTotal + double.Parse(litIPDAmount.Text, NumberStyles.AllowCurrencySymbol | NumberStyles.Currency);
                }
                this.litTotal.Text = string.Format("{0:c}", dblShoppingCartTotal);


                if (!string.IsNullOrEmpty(CurrentWebsite.CurrencyConvert)
                    && CurrentWebsite.CurrentyConvertPercentage != null
                    && CurrentWebsite.CurrentyConvertPercentage > 0)
                {
                    double dblTotal = double.Parse(litTotal.Text, NumberStyles.AllowCurrencySymbol | NumberStyles.Currency);
                    this.litTotalCurrencyConvert.Text = string.Format(@" USD <span style=""font-size: small; "" title=""est. {0:c} {1}""><i class=""ti-info-alt"" ></i> </span>"
                        , Convert.ToDecimal(dblTotal) * CurrentWebsite.CurrentyConvertPercentage
                        , CurrentWebsite.CurrencyConvert
                        );
                }

                //btnPlaceOrderPaystand.Visible = false;

                this.pnlUserCreditCardMessage.Visible = Convert.ToString(ConfigurationManager.AppSettings["Environment"]) != "production";
                this.pnlNewCreditCardMessage.Visible = Convert.ToString(ConfigurationManager.AppSettings["Environment"]) != "production";

                if (phInvoicePayment.Visible && CurrentWebsite.DefaultCheckoutPaymentMethod == "invoice")
                {
                    rbnInvoicePayment.Checked = true;
                    ChangePaymentOption();
                }

                if (!string.IsNullOrEmpty(ddlShippingAddress.SelectedValue) && pnlShippingMethod.Visible
                    && (ddlWebsiteShippingService.Items == null || ddlWebsiteShippingService.Items.Count == 0)
                )
                {
                    WebUtility.DisplayJavascriptMessage(this, "Shipping service not found for the provided shipping address. Please verify the shipping address.");
                }

                if (phBudgetOption.Visible && CurrentWebsite.MustUseExistingEmployeeCredit)
                {
                    phEnableCreditCard.Visible = false;
                    phInvoicePayment.Visible = false;
                    rbnInvoicePayment.Checked = false;
                }

                //phBudgetOption
                //phEnableCreditCard
                //phInvoicePayment
                if (phBudgetOption.Visible && !phEnableCreditCard.Visible && !phInvoicePayment.Visible)
                {
                    chkApplyBudget.Checked = true;
                    UpdateApplyBudget();
                }
                if (phInvoicePayment.Visible && !phEnableCreditCard.Visible && !phBudgetOption.Visible)
                {
                    rbnInvoicePayment.Checked = true;
                    ChangePaymentOption();
                }

                if (
                    (ConfigurationManager.AppSettings["Environment"] == "production" && (CurrentWebsite.WebsiteID == "7" || CurrentWebsite.WebsiteID == "8"))
                    ||
                    (ConfigurationManager.AppSettings["Environment"] != "production" && (CurrentWebsite.WebsiteID == "43" || CurrentWebsite.WebsiteID == "45"))
                )
                {
                    litCompanyInvoicedTotalList.Text = "Safety Items Total:";
                    litCompanyInvoicedTotal.Text = "Safety Items Total:";
                    litOrderTotal.Text = "Uniform Total";
                }

                if (!string.IsNullOrEmpty(strErrorMessage))
                {
                    throw new Exception(strErrorMessage);
                }


                if (CurrentUser.CurrentUserWebSite.DisplayNotificaitonEmailAtCheckout && !CurrentUser.CurrentUserWebSite.DisableNotificationEmail)
                {
                    pnlNotificationEmail.Visible = true;
                    txtNotificationEmail.Text = CurrentUser.CurrentUserWebSite.NotificationEmail;
                }

                this.pnlOptIn.Visible = CurrentWebsite.EnableEmailOptIn || CurrentWebsite.EnableSMSOptIn || CurrentUser.CurrentUserWebSite.EnableEmailOptIn || CurrentUser.CurrentUserWebSite.EnableSMSOptIn;
                this.pnlEmailOptIn.Visible = CurrentWebsite.EnableEmailOptIn || CurrentUser.CurrentUserWebSite.EnableEmailOptIn;
                this.pnlSMSOptIn.Visible = CurrentWebsite.EnableSMSOptIn || CurrentUser.CurrentUserWebSite.EnableSMSOptIn;
                this.txtNotificationEmail2.Text = CurrentUser.CurrentUserWebSite.NotificationEmail;
                this.txtMobileNumber.Text = CurrentUser.CurrentUserWebSite.SMSMobileNumber;

                if (pnlBillingAddressWrap.Visible)
                {
                    if (chkApplyBudget.Checked && !rbnNewCreditCard.Checked)
                    {
                        pnlBillingAddressWrap.Visible = false;
                    }
                }

                phInvoicePaymentNumber.Visible = CurrentWebsite.WebsiteID == "21"; //DT Corp
                phInvoiceStoreNumber.Visible = CurrentWebsite.WebsiteID == "26"; //Mavis CC

                if (CurrentWebsite.WebsiteID == "15")
                {
                    lblMyEmployeeCredit.Text = " Use Terms (Monthly Budget)  ";
                }


                if (CurrentWebsite.EnablePackagePayment)
                {
                    litPackage.Text = string.Format(@" - {0}", string.IsNullOrEmpty(CurrentUser.CurrentUserWebSite.ShoppingCart.Package.DisplayName) ? CurrentUser.CurrentUserWebSite.ShoppingCart.Package.Name : CurrentUser.CurrentUserWebSite.ShoppingCart.Package.DisplayName);
                    spanOrderTotal.Visible = false;

                    foreach (RepeaterItem _Item in rptShoppingCart.Items)
                    {
                        _Item.FindControl("spanLineTotal").Visible = false;
                    }

                    ulSubTotal.Style.Add("display", "none");
                    ulTotal.Style.Add("display", "none");

                    if (CurrentUser.CurrentUserWebSite.PackageAvailableDate != null
                        && CurrentUser.CurrentUserWebSite.PackageAvailableDate >= DateTime.UtcNow)
                    {
                        btnPlaceOrder.Enabled = false;
                    }
                }



                ValidateShoppingCartItems();

                UpdateShoppingCart();

                DisplayMultipleShipmentMessage();

                DisplayCutoffMessage();

                ValidateMinimumRequirementCanada();

            }
            catch (Exception ex)
            {
                WebUtility.DisplayJavascriptMessage(this, ex.Message);
            }
        }

        protected void DisplayMultipleShipmentMessage()
        {
            if (chkPartialShipping.Checked)
            {
                if (!string.IsNullOrEmpty(ddlWebsiteShippingService.SelectedValue))
                {
                    ImageSolutions.Website.WebsiteShippingService WebsiteShippingService = new ImageSolutions.Website.WebsiteShippingService(ddlWebsiteShippingService.SelectedValue);

                    if (ddlWebsiteShippingService != null && WebsiteShippingService.ShippingService != null
                        && WebsiteShippingService.ShippingService.Carrier != "Will Call"
                        && CurrentUser.CurrentUserWebSite.ShoppingCart.ShoppingCartLines.Exists(x => x.Item.ItemType == "_inventoryItem")
                        && CurrentUser.CurrentUserWebSite.ShoppingCart.ShoppingCartLines.Exists(x => x.Item.ItemType != "_inventoryItem")
                    )
                    {
                        WebUtility.DisplayJavascriptMessage(this, "The items will be shipped in multiple shipments");
                    }
                }
            }
        }

        protected void SetAddressPermission()
        {
            if (!string.IsNullOrEmpty(CurrentUser.CurrentUserWebSite.AddressPermission))
            {
                if (CurrentUser.CurrentUserWebSite.AddressPermission == "Account")
                {
                    ddlShippingAddress.SelectedValue = CurrentUser.CurrentUserWebSite.CurrentUserAccount.Account.DefaultShippingAddressBookID;
                    SetShippingAddress();
                    pnlShippingAddressButton.Visible = false;

                    ddlBillingAddress.SelectedValue = CurrentUser.CurrentUserWebSite.CurrentUserAccount.Account.DefaultShippingAddressBookID;
                    SetBillingAddress();

                    //pnlBillingAddressButton.Visible = false;
                    //cbSameAsShippingAddress.Enabled = false;
                    if (CurrentWebsite.EnableCreditCard && !CurrentUser.CurrentUserWebSite.CurrentUserAccount.Account.DoNotAllowCreditCard)
                    {
                        pnlBillingAddressButton.Visible = true;
                        cbSameAsShippingAddress.Enabled = true;
                    }
                    else
                    {

                        pnlBillingAddressButton.Visible = false;
                        cbSameAsShippingAddress.Enabled = false;
                    }
                }
                else if (CurrentUser.CurrentUserWebSite.AddressPermission == "Default")
                {
                    ddlShippingAddress.SelectedValue = CurrentUser.CurrentUserWebSite.DefaultShippingAddressID;
                    SetShippingAddress();
                    pnlShippingAddressButton.Visible = false;


                    ddlBillingAddress.SelectedValue = CurrentUser.CurrentUserWebSite.DefaultBillingAddressID;
                    SetBillingAddress();

                    if (CurrentWebsite.EnableCreditCard && !CurrentUser.CurrentUserWebSite.CurrentUserAccount.Account.DoNotAllowCreditCard)
                    {
                        pnlBillingAddressButton.Visible = true;
                        cbSameAsShippingAddress.Enabled = true;
                    }
                    else
                    {

                        pnlBillingAddressButton.Visible = false;
                        cbSameAsShippingAddress.Enabled = false;
                    }
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(CurrentUser.DefaultShippingAddressBookID))
                {
                    ddlShippingAddress.SelectedValue = CurrentUser.DefaultShippingAddressBookID;
                    SetShippingAddress();
                }
                else if (!string.IsNullOrEmpty(CurrentUser.CurrentUserWebSite.CurrentUserAccount.Account.DefaultShippingAddressBookID))
                {
                    ddlShippingAddress.SelectedValue = CurrentUser.CurrentUserWebSite.CurrentUserAccount.Account.DefaultShippingAddressBookID;
                    SetShippingAddress();
                }
                else
                {
                    btnEditShippingAddress.Visible = !string.IsNullOrEmpty(ddlShippingAddress.SelectedValue);
                }

                if (!string.IsNullOrEmpty(CurrentUser.DefaultBillingAddressBookID))
                {
                    ddlBillingAddress.SelectedValue = CurrentUser.DefaultBillingAddressBookID;
                    SetBillingAddress();
                }
                else if (!string.IsNullOrEmpty(CurrentUser.CurrentUserWebSite.CurrentUserAccount.Account.DefaultShippingAddressBookID))
                {
                    ddlBillingAddress.SelectedValue = CurrentUser.CurrentUserWebSite.CurrentUserAccount.Account.DefaultShippingAddressBookID;
                    SetBillingAddress();
                }
                else
                {
                    btnEditBillingAddress.Visible = !string.IsNullOrEmpty(ddlBillingAddress.SelectedValue);
                }
            }
        }

        protected void SetInvoicePayment()
        {
            phInvoicePayment.Visible = CurrentWebsite.EnablePaymentTerm && !string.IsNullOrEmpty(CurrentUser.CurrentUserWebSite.PaymentTermID);
            if (!string.IsNullOrEmpty(CurrentUser.CurrentUserWebSite.PaymentTermID)
                && CurrentUser.CurrentUserWebSite.PaymentTerm != null)
            {
                double dblTotal = 0;
                if (!string.IsNullOrEmpty(litTotal.Text))
                {
                    dblTotal = double.Parse(litTotal.Text, NumberStyles.AllowCurrencySymbol | NumberStyles.Currency);
                }

                if (CurrentUser.CurrentUserWebSite.PaymentTermAmount == null)
                {
                    this.lblInvoicePayment.Text = String.Format("Terms: {0}", Convert.ToString(CurrentUser.CurrentUserWebSite.PaymentTerm.Description));
                }
                else
                {
                    this.lblInvoicePayment.Text = String.Format("Terms: {0} (Balance {1})"
                        , Convert.ToString(CurrentUser.CurrentUserWebSite.PaymentTerm.Description)
                        , Convert.ToString(CurrentUser.CurrentUserWebSite.PaymentTermBalance));
                }
            }
        }
        protected void BindCustomField()
        {
            try
            {
                List<ImageSolutions.Custom.CustomField> CustomFields = new List<ImageSolutions.Custom.CustomField>();
                ImageSolutions.Custom.CustomFieldFilter CustomFieldFilter = new ImageSolutions.Custom.CustomFieldFilter();
                CustomFieldFilter.WebsiteID = new Database.Filter.StringSearch.SearchFilter();
                CustomFieldFilter.WebsiteID.SearchString = CurrentWebsite.WebsiteID;
                CustomFieldFilter.Location = new Database.Filter.StringSearch.SearchFilter();
                CustomFieldFilter.Location.SearchString = "checkout";
                CustomFieldFilter.Inactive = false;
                CustomFields = ImageSolutions.Custom.CustomField.GetCustomFields(CustomFieldFilter);

                List<ImageSolutions.Custom.CustomField> DisplayCustomFields = new List<ImageSolutions.Custom.CustomField>();
                foreach (ImageSolutions.Custom.CustomField _CustomField in CustomFields)
                {
                    //DT - hard code
                    if (
                        (Convert.ToString(ConfigurationManager.AppSettings["Environment"]) == "staging" && CurrentWebsite.WebsiteID == "54")
                        ||
                        (Convert.ToString(ConfigurationManager.AppSettings["Environment"]) == "production" && CurrentWebsite.WebsiteID == "21")
                    )
                    {
                        if (_CustomField.Name == "Department")
                        {
                            if (!CurrentUser.CurrentUserWebSite.ShoppingCart.ShoppingCartLines.Exists(x => x.Item.ParentItem != null && x.Item.ParentItem.WebsiteTabItems != null && x.Item.ParentItem.WebsiteTabItems.Exists(y => y.WebsiteTab.DoNotAllowMixCart && y.WebsiteTab.ExcludeShipping)))
                            {
                                break;
                            }
                        }
                    }

                    ImageSolutions.Custom.CustomFieldSignature CustomFieldSignature = null;

                    if (_CustomField.IsSignature)
                    {
                        CustomFieldSignature = new ImageSolutions.Custom.CustomFieldSignature();
                        ImageSolutions.Custom.CustomFieldSignatureFilter CustomFieldSignatureFilter = new ImageSolutions.Custom.CustomFieldSignatureFilter();
                        CustomFieldSignatureFilter.CustomFieldID = new Database.Filter.StringSearch.SearchFilter();
                        CustomFieldSignatureFilter.CustomFieldID.SearchString = _CustomField.CustomFieldID;
                        CustomFieldSignatureFilter.UserWebsiteID = new Database.Filter.StringSearch.SearchFilter();
                        CustomFieldSignatureFilter.UserWebsiteID.SearchString = CurrentUser.CurrentUserWebSite.UserWebsiteID;
                        CustomFieldSignature = ImageSolutions.Custom.CustomFieldSignature.GetCustomFieldSignature(CustomFieldSignatureFilter);
                    }

                    if (CustomFieldSignature == null || string.IsNullOrEmpty(CustomFieldSignature.CustomFieldSignatureID))
                    {
                        if (_CustomField.UserWebsitePermission == "Store Only")
                        {
                            if (CurrentUser.CurrentUserWebSite.IsStore)
                            {
                                DisplayCustomFields.Add(_CustomField);
                            }
                        }
                        else
                        {
                            DisplayCustomFields.Add(_CustomField);
                        }
                    }
                }

                rptCustomField.DataSource = DisplayCustomFields;
                rptCustomField.DataBind();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally { }
        }
        protected void BindCountry()
        {
            try
            {
                List<ImageSolutions.Address.AddressCountryCode> AddressCountryCodes = new List<ImageSolutions.Address.AddressCountryCode>();
                AddressCountryCodes = ImageSolutions.Address.AddressCountryCode.GetAddressCountryCodes();

                List<ImageSolutions.Address.AddressCountryCode> FilterAddressCountryCodes = new List<ImageSolutions.Address.AddressCountryCode>();
                if (CurrentWebsite.WebsiteCountries != null && CurrentWebsite.WebsiteCountries.Count > 0)
                {
                    if (CurrentWebsite.WebsiteCountries.FindAll(x => !Convert.ToBoolean(x.Exclude)).Count > 0)
                    {
                        foreach (ImageSolutions.Website.WebsiteCountry _WebsiteCountry in CurrentWebsite.WebsiteCountries.FindAll(x => !Convert.ToBoolean(x.Exclude)).ToList())
                        {
                            FilterAddressCountryCodes.Add(AddressCountryCodes.Find(x => x.Alpha2Code == _WebsiteCountry.CountryCode));
                        }
                    }

                    FilterAddressCountryCodes = FilterAddressCountryCodes != null && FilterAddressCountryCodes.Count > 0 ? FilterAddressCountryCodes : AddressCountryCodes;

                    if (CurrentWebsite.WebsiteCountries.FindAll(x => Convert.ToBoolean(x.Exclude)).Count > 0)
                    {
                        foreach (ImageSolutions.Website.WebsiteCountry _WebsiteCountry in CurrentWebsite.WebsiteCountries.FindAll(x => Convert.ToBoolean(x.Exclude)).ToList())
                        {
                            FilterAddressCountryCodes.Remove(FilterAddressCountryCodes.Find(x => x.Alpha2Code == _WebsiteCountry.CountryCode));
                        }
                    }
                }

                ddlCountry.DataSource = FilterAddressCountryCodes != null && FilterAddressCountryCodes.Count > 0 ? FilterAddressCountryCodes : AddressCountryCodes;
                ddlCountry.DataBind();
                ddlCountry.Items.Insert(0, new ListItem(String.Empty, string.Empty));

                //ddlBillingCountry.DataSource = FilterAddressCountryCodes != null && FilterAddressCountryCodes.Count > 0 ? FilterAddressCountryCodes : AddressCountryCodes;
                ddlBillingCountry.DataSource = AddressCountryCodes;
                ddlBillingCountry.DataBind();
                ddlBillingCountry.Items.Insert(0, new ListItem(String.Empty, string.Empty));
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally { }
        }
        protected void BindShippingAddress()
        {
            try
            {
                List<ImageSolutions.Address.AddressBook> AddressBooks = new List<ImageSolutions.Address.AddressBook>();
                ImageSolutions.Address.AddressBookFilter AddressBookFilter = new ImageSolutions.Address.AddressBookFilter();
                AddressBookFilter.UserInfoID = CurrentUser.UserInfoID;
                AddressBooks = ImageSolutions.Address.AddressBook.GetAddressBooks(AddressBookFilter);

                List<ImageSolutions.Address.AddressCountryCode> AddressCountryCodes = new List<ImageSolutions.Address.AddressCountryCode>();
                AddressCountryCodes = ImageSolutions.Address.AddressCountryCode.GetAddressCountryCodes();
                List<ImageSolutions.Address.AddressCountryCode> FilterAddressCountryCodes = new List<ImageSolutions.Address.AddressCountryCode>();
                if (CurrentWebsite.WebsiteCountries != null && CurrentWebsite.WebsiteCountries.Count > 0)
                {
                    if (CurrentWebsite.WebsiteCountries.FindAll(x => !Convert.ToBoolean(x.Exclude)).Count > 0)
                    {
                        foreach (ImageSolutions.Website.WebsiteCountry _WebsiteCountry in CurrentWebsite.WebsiteCountries.FindAll(x => !Convert.ToBoolean(x.Exclude)).ToList())
                        {
                            FilterAddressCountryCodes.Add(AddressCountryCodes.Find(x => x.Alpha2Code == _WebsiteCountry.CountryCode));
                        }
                    }

                    FilterAddressCountryCodes = FilterAddressCountryCodes != null && FilterAddressCountryCodes.Count > 0 ? FilterAddressCountryCodes : AddressCountryCodes;

                    if (CurrentWebsite.WebsiteCountries.FindAll(x => Convert.ToBoolean(x.Exclude)).Count > 0)
                    {
                        foreach (ImageSolutions.Website.WebsiteCountry _WebsiteCountry in CurrentWebsite.WebsiteCountries.FindAll(x => Convert.ToBoolean(x.Exclude)).ToList())
                        {
                            FilterAddressCountryCodes.Remove(FilterAddressCountryCodes.Find(x => x.Alpha2Code == _WebsiteCountry.CountryCode));
                        }
                    }
                }

                ddlShippingAddress.Items.Clear();
                ddlShippingAddress.Items.Add(new ListItem(string.Empty, string.Empty));
                foreach (ImageSolutions.Address.AddressBook _AddressBook in AddressBooks)
                {
                    if (FilterAddressCountryCodes != null && FilterAddressCountryCodes.Count > 0)
                    {
                        if (FilterAddressCountryCodes.FindAll(x => x.Alpha2Code == _AddressBook.CountryCode).Count > 0)
                        {
                            ddlShippingAddress.Items.Add(new ListItem(_AddressBook.GetDisplayFormat(false), _AddressBook.AddressBookID));
                        }
                    }
                    else
                    {
                        ddlShippingAddress.Items.Add(new ListItem(_AddressBook.GetDisplayFormat(false), _AddressBook.AddressBookID));
                    }
                }
                if (CurrentUser.CurrentUserWebSite.CurrentUserAccount.Account.DefaultShippingAddressBook != null)
                {
                    if (FilterAddressCountryCodes != null && FilterAddressCountryCodes.Count > 0)
                    {
                        if (FilterAddressCountryCodes.FindAll(x => x.Alpha2Code == CurrentUser.CurrentUserWebSite.CurrentUserAccount.Account.DefaultShippingAddressBook.CountryCode).Count > 0)
                        {
                            ddlShippingAddress.Items.Add(new ListItem("(Store Address) " + CurrentUser.CurrentUserWebSite.CurrentUserAccount.Account.DefaultShippingAddressBook.GetDisplayFormat(false), CurrentUser.CurrentUserWebSite.CurrentUserAccount.Account.DefaultShippingAddressBook.AddressBookID));
                        }
                    }
                    else
                    {
                        ddlShippingAddress.Items.Add(new ListItem("(Store Address) " + CurrentUser.CurrentUserWebSite.CurrentUserAccount.Account.DefaultShippingAddressBook.GetDisplayFormat(false), CurrentUser.CurrentUserWebSite.CurrentUserAccount.Account.DefaultShippingAddressBook.AddressBookID));
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally { }
        }
        protected void BindBillingAddress()
        {
            try
            {
                List<ImageSolutions.Address.AddressBook> AddressBooks = new List<ImageSolutions.Address.AddressBook>();
                ImageSolutions.Address.AddressBookFilter AddressBookFilter = new ImageSolutions.Address.AddressBookFilter();
                AddressBookFilter.UserInfoID = CurrentUser.UserInfoID;
                AddressBooks = ImageSolutions.Address.AddressBook.GetAddressBooks(AddressBookFilter);

                ddlBillingAddress.Items.Clear();
                ddlBillingAddress.Items.Add(new ListItem(string.Empty, string.Empty));
                foreach (ImageSolutions.Address.AddressBook _AddressBook in AddressBooks)
                {
                    ddlBillingAddress.Items.Add(new ListItem(_AddressBook.GetDisplayFormat(false), _AddressBook.AddressBookID));
                }

                if (CurrentUser.CurrentUserWebSite.CurrentUserAccount.Account.DefaultShippingAddressBook != null)
                {
                    ddlBillingAddress.Items.Add(new ListItem("(Store Address) " + CurrentUser.CurrentUserWebSite.CurrentUserAccount.Account.DefaultShippingAddressBook.GetDisplayFormat(false), CurrentUser.CurrentUserWebSite.CurrentUserAccount.Account.DefaultShippingAddressBook.AddressBookID));
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally { }
        }

        protected bool ValidateShippingPOBox()
        {
            bool blnReturn = true;

            ImageSolutions.Address.AddressBook objShippingAddressBook = new ImageSolutions.Address.AddressBook(Convert.ToString(ddlShippingAddress.SelectedValue));

            if (objShippingAddressBook.AddressLine1.ToLower().Contains("po box") || objShippingAddressBook.AddressLine2.ToLower().Contains("po box")
                || objShippingAddressBook.AddressLine1.ToLower().StartsWith("po ") || objShippingAddressBook.AddressLine2.ToLower().StartsWith("po ")
                || objShippingAddressBook.AddressLine1.ToLower().StartsWith("p.o.") || objShippingAddressBook.AddressLine2.ToLower().StartsWith("po. ")
            )
            {
                blnReturn = false;
            }

            return blnReturn;
        }

        protected void btnPlaceOrder_Click(object sender, EventArgs e)
        {
            if (CurrentWebsite.IsPunchout)
            {
                Response.Redirect("/ShoppingCart.aspx");
            }

            try
            {
                CurrentUser.CurrentUserWebSite.SMSMobileNumber = this.txtMobileNumber.Text.Trim();
                //CurrentUser.CurrentUserWebSite.SMSOptIn = this.chkSMSOptIn.Checked;
                CurrentUser.CurrentUserWebSite.Update();
            }
            catch { }

            PlaceOrder();
        }

        protected void PlaceOrder()
        {
            bool blnReturn = false;
            SqlConnection objConn = null;
            SqlTransaction objTran = null;
            ImageSolutions.Address.AddressBook objShippingAddressBook = null;
            ImageSolutions.Address.AddressBook objBillingAddressBook = null;
            ImageSolutions.Address.AddressTrans objShippingAddressTrans = null;
            ImageSolutions.Address.AddressTrans objBillingAddressTrans = null;
            ImageSolutions.SalesOrder.SalesOrder objSalesOrder = null;
            ImageSolutions.Payment.Payment objPayment = null;
            ImageSolutions.Payment.Payment objPromoPayment = null;
            ImageSolutions.Payment.Payment objCompanyInvoicePayment = null;
            ImageSolutions.Payment.Payment objBudgetPayment = null;
            ImageSolutions.CreditCard.CreditCardTransactionLog objCreditCardTransactionLog = null;

            string strBudgetPaymentTermID = string.Empty;
            string strCreditCardTransactionID = string.Empty;

            bool blnRequireItemPersonalizationApproval = false;
            bool blnRequireItemApproval = false;

            try
            {
                //ucLoading.Visible = true;
                //ucLoading.Show();

                //ValidateMinimumRequirementCanada();

                if (CurrentWebsite.EnablePackagePayment)
                {
                    if (CurrentUser.CurrentUserWebSite.PackageAvailableDate != null
                        && CurrentUser.CurrentUserWebSite.PackageAvailableDate >= DateTime.UtcNow)
                    {
                        throw new Exception("Order for selected package cannot be placed at this time.");
                    }

                    if (!ValidatePackage())
                    {
                        if (CurrentUser.CurrentUserWebSite.ShoppingCart.ShoppingCartLines != null && CurrentUser.CurrentUserWebSite.ShoppingCart.ShoppingCartLines.Count > 0)
                        {
                            foreach (ImageSolutions.ShoppingCart.ShoppingCartLine _ShoppingCartLine in CurrentUser.CurrentUserWebSite.ShoppingCart.ShoppingCartLines)
                            {
                                _ShoppingCartLine.Delete();
                            }
                        }

                        Response.Redirect("/myaccount/dashboard.aspx?error=invaild-package-quantity");
                        //throw new Exception("Invalid Package Items");
                    }
                }

                if (hfUserGUID.Value != CurrentUser.GUID)
                {
                    throw new Exception("Cannot identify the user.  Please refresh the page.");
                }

                if (hfUserWebsiteID.Value != CurrentUser.CurrentUserWebSite.UserWebsiteID)
                {
                    throw new Exception("Cannot identify the user.  Please refresh the page.");
                }

                if (this.litSubtotal.Text != string.Format("{0:c}", CurrentUser.CurrentUserWebSite.ShoppingCart.LineTotal))
                {
                    throw new Exception("Cart has been updated.");
                }

                //Enterprise
                //if ( (CurrentWebsite.WebsiteID == "53" || CurrentWebsite.WebsiteID == "20") && string.IsNullOrEmpty(hfEnterpriseMessage.Value))
                //{
                //    hfEnterpriseMessage.Value = "Message Displayed";
                //    throw new Exception("Please review your order carefully before hitting transfer AND be sure you have included at least one outerwear item.");
                //}

                if (chkApplyBudget.Checked && !string.IsNullOrEmpty(Convert.ToString(CurrentWebsite.OverBudgetMessage).Trim()) && string.IsNullOrEmpty(hfBudgetMessage.Value))
                {
                    double dblRemainingTotal = double.Parse(litRemainingTotal.Text, NumberStyles.AllowCurrencySymbol | NumberStyles.Currency);
                    if (dblRemainingTotal > 0)
                    {
                        hfBudgetMessage.Value = "Message Displayed";
                        throw new Exception(CurrentWebsite.OverBudgetMessage);
                    }
                }

                if (string.IsNullOrEmpty(ddlShippingAddress.SelectedValue))
                {
                    throw new Exception("Shipping Address is required.");
                }

                if (string.IsNullOrEmpty(hfShippingPOBoxMessage.Value) && !ValidateShippingPOBox())
                {
                    hfShippingPOBoxMessage.Value = "Message Displayed";
                    throw new Exception("We can't ship to PO boxes, please enter a valid address");
                }

                if (phInvoicePayment.Visible && phInvoicePaymentMessage.Visible && phInvoicePaymentNumber.Visible && string.IsNullOrEmpty(txtInvoicePaymentNumber.Text))
                {
                    throw new Exception("PO Number required");
                }
                if (phInvoicePayment.Visible && phInvoicePaymentMessage.Visible && phInvoiceStoreNumber.Visible && string.IsNullOrEmpty(txtInvoiceStoreNumber.Text))
                {
                    throw new Exception("Store Number required");
                }

                ValidateShoppingCartItems();

                Guid CheckoutSesssion = Guid.NewGuid();
                CurrentUser.CurrentUserWebSite.ShoppingCart.CheckoutSession = Convert.ToString(CheckoutSesssion);
                CurrentUser.CurrentUserWebSite.ShoppingCart.Update();
                Thread.Sleep(500);

                if (pnlShippingAddressMain.Visible && string.IsNullOrEmpty(ddlWebsiteShippingService.SelectedValue))
                {
                    throw new Exception("Shipping Method required");
                }

                //if (string.IsNullOrEmpty(ddlShippingAddress.SelectedValue)
                //    || (!rbnUserCreditCard.Checked && string.IsNullOrEmpty(ddlBillingAddress.SelectedValue))
                //)
                //{
                //    if (rbnUserCreditCard.Checked)
                //    {
                //        WebUtility.DisplayJavascriptMessage(this, "Shipping Address is required.");
                //    }
                //    else
                //    {
                //        WebUtility.DisplayJavascriptMessage(this, "Shipping Address and Billing Address are required.");
                //    }
                //}

                if (rbnNewCreditCard.Checked && string.IsNullOrEmpty(ddlBillingAddress.SelectedValue))
                {
                    WebUtility.DisplayJavascriptMessage(this, "Billing Address is required.");
                }
                else
                {
                    CurrentUser.CurrentUserWebSite.ShoppingCart.WebsiteShippingServiceID = this.ddlWebsiteShippingService.SelectedValue;

                    objConn = new SqlConnection(Database.DefaultConnectionString);
                    objConn.Open();
                    objTran = objConn.BeginTransaction();

                    //SP
                    //if (rbnBudget.Checked)
                    //{
                    //    ImageSolutions.Budget.MyBudgetAssignment MyBudgetAssignment = new ImageSolutions.Budget.MyBudgetAssignment(CurrentUser.UserInfoID, ddlMyBudgetAssignment.SelectedValue);
                    //    if (CurrentUser.CurrentUserWebSite.ShoppingCart.Total > MyBudgetAssignment.Balance)
                    //    {
                    //        throw new Exception("Not enough available amount to use this budget");
                    //    }
                    //}

                    objSalesOrder = new ImageSolutions.SalesOrder.SalesOrder();

                    objShippingAddressBook = new ImageSolutions.Address.AddressBook(Convert.ToString(ddlShippingAddress.SelectedValue));
                    objShippingAddressTrans = new ImageSolutions.Address.AddressTrans();
                    objShippingAddressTrans.AddressLabel = objShippingAddressBook.AddressLabel;
                    objShippingAddressTrans.AddressLine1 = objShippingAddressBook.AddressLine1;
                    objShippingAddressTrans.AddressLine2 = objShippingAddressBook.AddressLine2;
                    objShippingAddressTrans.FirstName = objShippingAddressBook.FirstName;
                    objShippingAddressTrans.LastName = objShippingAddressBook.LastName;
                    objShippingAddressTrans.City = objShippingAddressBook.City;
                    objShippingAddressTrans.State = objShippingAddressBook.State;
                    if (objShippingAddressTrans.State.Length != 2)
                    {
                        ImageSolutions.Address.AddressState AddressState = ImageSolutions.Address.AddressState.GetState(Convert.ToString(objShippingAddressBook.CountryCode), Convert.ToString(objShippingAddressBook.State));

                        if (AddressState != null)
                        {
                            objShippingAddressTrans.State = AddressState.StateID;
                        }
                    }

                    objShippingAddressTrans.PostalCode = objShippingAddressBook.PostalCode;
                    objShippingAddressTrans.CountryCode = objShippingAddressBook.CountryCode;

                    if (!string.IsNullOrEmpty(objShippingAddressBook.PhoneNumber) && objShippingAddressBook.PhoneNumber.Length >= 7)
                    {
                        objShippingAddressTrans.PhoneNumber = objShippingAddressBook.PhoneNumber;
                    }

                    objShippingAddressTrans.Email = objShippingAddressBook.Email;
                    objShippingAddressTrans.Create(objConn, objTran);

                    objSalesOrder.DeliveryAddressTransID = objShippingAddressTrans.AddressTransID;

                    //Create an Account
                    if (CurrentUser.IsGuest && chkRegister.Checked)
                    {
                        if (string.IsNullOrEmpty(this.txtEmailAddress.Text.Trim())) throw new Exception("Email address is required if you'd like to create an account");
                        if (string.IsNullOrEmpty(this.txtPassword.Text.Trim())) throw new Exception("Password is required if you'd like to create an account");

                        CurrentUser.FirstName = objShippingAddressBook.FirstName;
                        CurrentUser.LastName = objShippingAddressBook.LastName;
                        CurrentUser.Password = this.txtPassword.Text.Trim();
                        CurrentUser.EmailAddress = this.txtEmailAddress.Text.Trim();
                        CurrentUser.IsGuest = false;
                        CurrentUser.Update(objConn, objTran);
                    }

                    if (pnlNotificationEmail.Visible)
                    {
                        CurrentUser.CurrentUserWebSite.NotificationEmail = txtNotificationEmail.Text;
                        CurrentUser.CurrentUserWebSite.Update(objConn, objTran);
                    }
                    else if (pnlOptIn.Visible)
                    {
                        if (CurrentWebsite.EnableEmailOptIn || CurrentUser.CurrentUserWebSite.EnableEmailOptIn) CurrentUser.CurrentUserWebSite.NotificationEmail = txtNotificationEmail2.Text.Trim();
                        if (CurrentWebsite.EnableSMSOptIn || CurrentUser.CurrentUserWebSite.EnableSMSOptIn) CurrentUser.CurrentUserWebSite.SMSMobileNumber = this.txtMobileNumber.Text.Trim();
                        CurrentUser.CurrentUserWebSite.Update(objConn, objTran);
                    }

                    CurrentUser.CurrentUserWebSite.ShoppingCart.ShippingAddress = objShippingAddressTrans;

                    if (!string.IsNullOrEmpty(ddlBillingAddress.SelectedValue))
                    {
                        objBillingAddressBook = new ImageSolutions.Address.AddressBook(Convert.ToString(ddlBillingAddress.SelectedValue));
                        objBillingAddressTrans = new ImageSolutions.Address.AddressTrans();
                        objBillingAddressTrans.AddressLabel = objBillingAddressBook.AddressLabel;
                        objBillingAddressTrans.AddressLine1 = objBillingAddressBook.AddressLine1;
                        objBillingAddressTrans.AddressLine2 = objBillingAddressBook.AddressLine2;
                        objBillingAddressTrans.FirstName = objBillingAddressBook.FirstName;
                        objBillingAddressTrans.LastName = objBillingAddressBook.LastName;
                        objBillingAddressTrans.City = objBillingAddressBook.City;
                        objBillingAddressTrans.State = objBillingAddressBook.State;
                        objBillingAddressTrans.PostalCode = objBillingAddressBook.PostalCode;
                        objBillingAddressTrans.CountryCode = objBillingAddressBook.CountryCode;
                        if (objBillingAddressTrans.State.Length != 2)
                        {
                            ImageSolutions.Address.AddressState AddressState = ImageSolutions.Address.AddressState.GetState(Convert.ToString(objBillingAddressBook.CountryCode), Convert.ToString(objBillingAddressBook.State));

                            if (AddressState != null)
                            {
                                objBillingAddressTrans.State = AddressState.StateID;
                            }
                        }


                        if (!string.IsNullOrEmpty(objBillingAddressBook.PhoneNumber) && objBillingAddressBook.PhoneNumber.Length >= 7)
                        {
                            objBillingAddressTrans.PhoneNumber = objBillingAddressBook.PhoneNumber;
                        }

                        objBillingAddressTrans.Email = objBillingAddressBook.Email;
                        objBillingAddressTrans.Create(objConn, objTran);

                        objSalesOrder.BillingAddressTransID = objBillingAddressTrans.AddressTransID;
                    }
                    else
                    {
                        objSalesOrder.BillingAddressTransID = objShippingAddressTrans.AddressTransID;
                    }


                    if (pnlShippingAndTaxes.Visible)
                    {
                        objSalesOrder.ShippingAmount = CurrentUser.CurrentUserWebSite.ShoppingCart.ShippingAmount;

                        if (chkPartialShipping.Checked)
                        {
                            objSalesOrder.ShippingAmount = CurrentUser.CurrentUserWebSite.ShoppingCart.PartialShippingAmount;
                            //string[] splitString = ddlWebsiteShippingService.SelectedItem.Text.Split('-');
                            //string strShippingAmount = splitString[splitString.Count() - 1].Trim();
                            //litShippingAmount.Text = string.Format("{0:c}", Convert.ToDouble(strShippingAmount));

                            objSalesOrder.IsPartialShipping = true;
                        }

                        objSalesOrder.TaxAmount = CurrentUser.CurrentUserWebSite.IsTaxExempt ? 0 : CurrentUser.CurrentUserWebSite.ShoppingCart.SalesTaxAmount;

                        if (CurrentWebsite.EnableIPD && IsInternational())
                        {
                            objSalesOrder.IPDDutiesAndTaxesAmount = CurrentUser.CurrentUserWebSite.ShoppingCart.LineTotal * (Convert.ToDouble(CurrentWebsite.IPDTaxAdjustPercent) / 100.00);
                        }

                        objSalesOrder.WebsiteShippingServiceID = ddlWebsiteShippingService.SelectedValue;

                        if (!string.IsNullOrEmpty(litBudgetShipping.Text))
                        {
                            objSalesOrder.BudgetShippingAmount = double.Parse(litBudgetShipping.Text, NumberStyles.AllowCurrencySymbol | NumberStyles.Currency);
                        }
                        if (!string.IsNullOrEmpty(litBudgetTax.Text))
                        {
                            objSalesOrder.BudgetTaxAmount = double.Parse(litBudgetTax.Text, NumberStyles.AllowCurrencySymbol | NumberStyles.Currency);
                        }


                    }
                    else
                    {
                        objSalesOrder.ShippingAmount = 0;
                        objSalesOrder.TaxAmount = 0;
                        ImageSolutions.Budget.MyBudgetAssignment MyBudgetAssignment = new ImageSolutions.Budget.MyBudgetAssignment(CurrentUser.UserInfoID, ddlMyBudgetAssignment.SelectedValue);
                        objSalesOrder.WebsiteShippingServiceID = MyBudgetAssignment.BudgetAssignment.Budget.WebsiteShippingServiceID;

                    }

                    objSalesOrder.DiscountAmount = CurrentUser.CurrentUserWebSite.ShoppingCart.DiscountAmount;

                    if (!pnlShippingAddressMain.Visible)
                    {
                        //Customer Pick UP from NS
                        if (CurrentWebsite.WebsiteShippingServices != null && CurrentWebsite.WebsiteShippingServices.Find(x => x.ShippingService.ServiceCode == "643") != null)
                        {
                            objSalesOrder.WebsiteShippingServiceID = CurrentWebsite.WebsiteShippingServices.Find(x => x.ShippingService.ServiceCode == "643").WebsiteShippingServiceID;
                        }
                        else
                        {
                            throw new Exception("Service not available");
                        }
                    }

                    objSalesOrder.IsPendingApproval = CurrentUser.CurrentUserWebSite.WebSite.OrderApprovalRequired;


                    objSalesOrder.InActive = true;
                    objSalesOrder.TransactionDate = DateTime.Now;
                    objSalesOrder.UserInfoID = CurrentUser.UserInfoID;
                    objSalesOrder.UserWebsiteID = CurrentUser.CurrentUserWebSite.UserWebsiteID;
                    objSalesOrder.WebsiteID = CurrentUser.CurrentUserWebSite.WebsiteID;
                    objSalesOrder.AccountID = CurrentUser.CurrentUserWebSite.CurrentUserAccount.AccountID;
                    objSalesOrder.IsTaxExempt = CurrentUser.CurrentUserWebSite.IsTaxExempt;

                    objSalesOrder.SalesOrderLines = new List<ImageSolutions.SalesOrder.SalesOrderLine>();
                    foreach (ImageSolutions.ShoppingCart.ShoppingCartLine objShoppingCartLine in CurrentUser.CurrentUserWebSite.ShoppingCart.ShoppingCartLines)
                    {
                        ImageSolutions.Item.Item Item = new ImageSolutions.Item.Item(objShoppingCartLine.ItemID);
                        if (
                            (!Item.AllowBackOrder || CurrentWebsite.DisallowBackOrder)
                            &&
                            (
                                (Item.IsNonInventory && objShoppingCartLine.Quantity > Item.VendorInventory)
                                ||
                                (!Item.IsNonInventory && objShoppingCartLine.Quantity > Item.QuantityAvailable)
                            )
                        )
                        {
                            throw new Exception(String.Format("Not enough inventory available {0} ({1})", Item.SalesDescription, Item.QuantityAvailable));
                        }

                        if (Item.InternalID.ToLower().Contains("inactive"))
                        {
                            throw new Exception(String.Format("Invalid Item: {0} ({1})", Item.SalesDescription, Item.QuantityAvailable));
                        }

                        ImageSolutions.SalesOrder.SalesOrderLine objSalesOrderLine = new ImageSolutions.SalesOrder.SalesOrderLine();
                        objSalesOrderLine.ItemID = objShoppingCartLine.ItemID;
                        objSalesOrderLine.Quantity = objShoppingCartLine.Quantity;
                        objSalesOrderLine.UnitPrice = objShoppingCartLine.UnitTotal; // objShoppingCartLine.UnitPrice;
                        objSalesOrderLine.DiscountAmount = objShoppingCartLine.Item.DiscountAmount * objShoppingCartLine.Quantity; // objShoppingCartLine.UnitPrice;
                        objSalesOrderLine.OnlinePrice = objShoppingCartLine.OnlinePrice;
                        objSalesOrderLine.TariffCharge = objShoppingCartLine.TariffCharge;
                        objSalesOrderLine.UserInfoID = objShoppingCartLine.UserInfoID;
                        objSalesOrderLine.CustomListID_1 = objShoppingCartLine.CustomListID_1;
                        objSalesOrderLine.CustomListID_2 = objShoppingCartLine.CustomListID_2;
                        objSalesOrderLine.CustomListValueID_1 = objShoppingCartLine.CustomListValueID_1;
                        objSalesOrderLine.CustomListValueID_2 = objShoppingCartLine.CustomListValueID_2;
                        objSalesOrderLine.CustomDesignImagePath = objShoppingCartLine.CustomDesignImagePath;
                        objSalesOrderLine.CustomDesignName = objShoppingCartLine.CustomDesignName;

                        if (objShoppingCartLine.ShoppingCartLineSelectableLogos != null && objShoppingCartLine.ShoppingCartLineSelectableLogos.Count > 0)
                        {
                            objSalesOrderLine.SalesOrderLineSelectableLogos = new List<SalesOrderLineSelectableLogo>();
                            foreach (ImageSolutions.ShoppingCart.ShoppingCartLineSelectableLogo _ShoppingCartLineSelectableLogo in objShoppingCartLine.ShoppingCartLineSelectableLogos)
                            {
                                ImageSolutions.SalesOrder.SalesOrderLineSelectableLogo SalesOrderLineSelectableLogo = new ImageSolutions.SalesOrder.SalesOrderLineSelectableLogo();
                                SalesOrderLineSelectableLogo.SelectableLogoID = _ShoppingCartLineSelectableLogo.SelectableLogoID;
                                SalesOrderLineSelectableLogo.HasNoLogo = _ShoppingCartLineSelectableLogo.HasNoLogo;
                                SalesOrderLineSelectableLogo.BasePrice = _ShoppingCartLineSelectableLogo.BasePrice;
                                SalesOrderLineSelectableLogo.SelectYear = _ShoppingCartLineSelectableLogo.SelectYear;
                                objSalesOrderLine.SalesOrderLineSelectableLogos.Add(SalesOrderLineSelectableLogo);
                            }
                        }

                        if (objShoppingCartLine.ItemPersonalizationValues != null && objShoppingCartLine.ItemPersonalizationValues.Count > 0)
                        {
                            objSalesOrderLine.ItemPersonalizationValues = new List<ImageSolutions.Item.ItemPersonalizationValue>();
                            foreach (ImageSolutions.Item.ItemPersonalizationValue _ItemPersonalizationValue in objShoppingCartLine.ItemPersonalizationValues)
                            {
                                objSalesOrderLine.ItemPersonalizationValues.Add(_ItemPersonalizationValue);

                                if (_ItemPersonalizationValue.ItemPersonalization.RequireApproval)
                                {
                                    if ((CurrentWebsite.WebsiteID == "9" || CurrentWebsite.Name == "Discount Tire")
                                        && (_ItemPersonalizationValue.TextOption == "Name" || _ItemPersonalizationValue.TextOption == "Preferred Name" || _ItemPersonalizationValue.TextOption == "No Embroidery")
                                    )
                                    {
                                        //Do nothing for Discount Tire with Text Option set as Name
                                    }
                                    else
                                    if (string.IsNullOrEmpty(_ItemPersonalizationValue.Value)
                                        && _ItemPersonalizationValue.ItemPersonalization.BypassApprovalForBlank)
                                    {
                                        //Do nothing if there is no personalization selected and by pass approval for blank
                                    }
                                    else
                                    {
                                        ImageSolutions.Item.ItemPersonalizationValueApproved ItemPersonalizationValueApproved = new ImageSolutions.Item.ItemPersonalizationValueApproved();
                                        ImageSolutions.Item.ItemPersonalizationValueApprovedFilter ItemPersonalizationValueApprovedFilter = new ImageSolutions.Item.ItemPersonalizationValueApprovedFilter();
                                        ItemPersonalizationValueApprovedFilter.WebsiteID = new Database.Filter.StringSearch.SearchFilter();
                                        ItemPersonalizationValueApprovedFilter.WebsiteID.SearchString = CurrentWebsite.WebsiteID;
                                        ItemPersonalizationValueApprovedFilter.ItemPersonalizationName = new Database.Filter.StringSearch.SearchFilter();
                                        ItemPersonalizationValueApprovedFilter.ItemPersonalizationName.SearchString = _ItemPersonalizationValue.ItemPersonalization.Name;
                                        ItemPersonalizationValueApprovedFilter.ItemPersonalizationApprovedValue = new Database.Filter.StringSearch.SearchFilter();
                                        ItemPersonalizationValueApprovedFilter.ItemPersonalizationApprovedValue.SearchString = _ItemPersonalizationValue.Value;
                                        ItemPersonalizationValueApproved = ImageSolutions.Item.ItemPersonalizationValueApproved.GetItemPersonalizationValueApproved(ItemPersonalizationValueApprovedFilter);

                                        if (ItemPersonalizationValueApproved == null)
                                        {
                                            blnRequireItemPersonalizationApproval = true;
                                        }


                                        if ((CurrentWebsite.WebsiteID == "9" || CurrentWebsite.Name == "Discount Tire")
                                            && _ItemPersonalizationValue.TextOption == "Preferred Name"
                                        )
                                        {
                                            blnRequireItemPersonalizationApproval = false;
                                        }
                                    }
                                }
                            }
                        }

                        if (!string.IsNullOrEmpty(objSalesOrderLine.UserInfoID) && CurrentWebsite.WebsiteID != "9" && CurrentWebsite.Name != "Discount Tire")
                        {
                            blnRequireItemPersonalizationApproval = true;
                        }

                        if (objSalesOrderLine.Item.ItemWebsites.Find(x => x.WebsiteID == CurrentWebsite.WebsiteID).RequireApproval)
                        {
                            blnRequireItemApproval = true;
                        }

                        objSalesOrder.SalesOrderLines.Add(objSalesOrderLine);
                    }

                    if (phInvoicePaymentNumber.Visible && !string.IsNullOrEmpty(txtInvoicePaymentNumber.Text))
                    {
                        objSalesOrder.TermPaymentPONumber = txtInvoicePaymentNumber.Text;
                    }
                    if (phInvoiceStoreNumber.Visible && !string.IsNullOrEmpty(txtInvoiceStoreNumber.Text))
                    {
                        objSalesOrder.TermPaymentStoreNumber = txtInvoiceStoreNumber.Text;
                    }

                    if (!string.IsNullOrEmpty(CurrentUser.CurrentUserWebSite.ShoppingCart.PackageID))
                    {
                        objSalesOrder.PackageID = CurrentUser.CurrentUserWebSite.ShoppingCart.PackageID;
                        objSalesOrder.ExcludeOptional = CurrentUser.CurrentUserWebSite.ShoppingCart.ExcludeOptional;
                    }

                    objSalesOrder.Create(objConn, objTran);

                    foreach (RepeaterItem _Item in this.rptCustomField.Items)
                    {
                        string strCustomFieldID = ((HiddenField)_Item.FindControl("hfCustomFieldID")).Value;

                        ImageSolutions.Custom.CustomValue CustomValue = new ImageSolutions.Custom.CustomValue();
                        CustomValue.CustomFieldID = strCustomFieldID;
                        CustomValue.SalesOrderID = objSalesOrder.SalesOrderID;

                        ImageSolutions.Custom.CustomField CustomField = new ImageSolutions.Custom.CustomField(strCustomFieldID);

                        if (CustomField.Type == "dropdown")
                        {
                            DropDownList ddlCustomValueList = (DropDownList)_Item.FindControl("ddlCustomValueList");
                            CustomValue.Value = ddlCustomValueList.SelectedValue;
                        }
                        else
                        {
                            TextBox txtCustomValue = (TextBox)_Item.FindControl("txtCustomValue");
                            CustomValue.Value = txtCustomValue.Text;


                            if (CustomField.IsNumeric && !int.TryParse(CustomValue.Value, out int n))
                            {
                                throw new Exception(String.Format("'{0}' must be a valid number", CustomField.Name));
                            }
                        }

                        if (CustomField.IsRequired && string.IsNullOrEmpty(CustomValue.Value))
                        {
                            throw new Exception(String.Format("'{0}' is required", CustomField.Name));
                        }


                        CustomValue.Create(objConn, objTran);

                        if (CustomField.IsSignature)
                        {
                            ImageSolutions.Custom.CustomFieldSignature CustomFieldSignature = new ImageSolutions.Custom.CustomFieldSignature();
                            CustomFieldSignature.CustomFieldID = CustomField.CustomFieldID;
                            CustomFieldSignature.UserWebsiteID = CurrentUser.CurrentUserWebSite.UserWebsiteID;
                            CustomFieldSignature.Create(objConn, objTran);
                        }
                    }

                    objPayment = new ImageSolutions.Payment.Payment();
                    objPayment.UserInfoID = CurrentUser.UserInfoID;
                    objPayment.SalesOrderID = objSalesOrder.SalesOrderID;

                    if (CurrentUser.CurrentUserWebSite.ShoppingCart.PromotionAmount != 0)
                    {
                        foreach (ImageSolutions.ShoppingCart.ShoppingCartPromotion _ShoppingCartPromotion in CurrentUser.CurrentUserWebSite.ShoppingCart.ShoppingCartPromotions)
                        {
                            double dblPromoTotal = double.Parse(litPromo.Text, NumberStyles.AllowCurrencySymbol | NumberStyles.Currency);

                            objPromoPayment = new ImageSolutions.Payment.Payment();
                            objPromoPayment.UserInfoID = CurrentUser.UserInfoID;
                            objPromoPayment.SalesOrderID = objSalesOrder.SalesOrderID;
                            objPromoPayment.AmountPaid = dblPromoTotal * -1;
                            objPromoPayment.PaymentSource = ImageSolutions.Payment.Payment.enumPaymentSource.Promotion;
                            objPromoPayment.PromotionID = _ShoppingCartPromotion.PromotionID;
                            objPromoPayment.CreatedBy = CurrentUser.UserInfoID;
                            objPromoPayment.Create(objConn, objTran);

                            ImageSolutions.Promotion.PromotionTrans PromotionTrans = new ImageSolutions.Promotion.PromotionTrans(CurrentWebsite.WebsiteID);
                            PromotionTrans.PromotionID = _ShoppingCartPromotion.PromotionID;
                            PromotionTrans.SalesOrderID = objSalesOrder.SalesOrderID;
                            PromotionTrans.PromotionCode = _ShoppingCartPromotion.Promotion.PromotionCode;
                            PromotionTrans.PromotionName = _ShoppingCartPromotion.Promotion.PromotionName;
                            PromotionTrans.DiscountAmount = Convert.ToDecimal(dblPromoTotal); //_ShoppingCartPromotion.Promotion.DiscountAmount;
                            PromotionTrans.DiscountPercent = _ShoppingCartPromotion.Promotion.DiscountPercent;
                            PromotionTrans.MinOrderAmount = _ShoppingCartPromotion.Promotion.MinOrderAmount;
                            PromotionTrans.MaxOrderAmount = _ShoppingCartPromotion.Promotion.MaxOrderAmount;
                            PromotionTrans.FromDate = _ShoppingCartPromotion.Promotion.FromDate;
                            PromotionTrans.ToDate = _ShoppingCartPromotion.Promotion.ToDate;
                            PromotionTrans.MaxUsageCount = _ShoppingCartPromotion.Promotion.MaxUsageCount;
                            PromotionTrans.CanBeCombined = _ShoppingCartPromotion.Promotion.CanBeCombined;
                            PromotionTrans.IsSalesTaxExempt = _ShoppingCartPromotion.Promotion.IsSalesTaxExempt;
                            PromotionTrans.IsOrPromotionBuy = _ShoppingCartPromotion.Promotion.IsOrPromotionBuy;
                            PromotionTrans.IsOrPromotionGet = _ShoppingCartPromotion.Promotion.IsOrPromotionGet;
                            PromotionTrans.CreatedBy = CurrentUser.UserInfoID;
                            PromotionTrans.Create(objConn, objTran);
                        }
                    }

                    if (CurrentUser.CurrentUserWebSite.ShoppingCart.GetCompanyInvoicedAmount(CurrentWebsite.WebsiteID, CurrentUser.CurrentUserWebSite.IsTaxExempt) != 0)
                    {
                        objCompanyInvoicePayment = new ImageSolutions.Payment.Payment();
                        objCompanyInvoicePayment.UserInfoID = CurrentUser.UserInfoID;
                        objCompanyInvoicePayment.SalesOrderID = objSalesOrder.SalesOrderID;
                        objCompanyInvoicePayment.AmountPaid = CurrentUser.CurrentUserWebSite.ShoppingCart.GetCompanyInvoicedAmount(CurrentWebsite.WebsiteID, CurrentUser.CurrentUserWebSite.IsTaxExempt) * -1;
                        objCompanyInvoicePayment.PaymentSource = ImageSolutions.Payment.Payment.enumPaymentSource.Invoice;
                        objCompanyInvoicePayment.PaymentTermID = string.IsNullOrEmpty(CurrentUser.CurrentUserWebSite.WebSite.DefaultPaymentTermID) ? "3" : CurrentUser.CurrentUserWebSite.WebSite.DefaultPaymentTermID;
                        objCompanyInvoicePayment.CreatedBy = CurrentUser.UserInfoID;
                        objCompanyInvoicePayment.Create(objConn, objTran);
                    }


                    //SP
                    //if (rbnBudget.Checked)
                    //{
                    //    objPayment.PaymentSource = ImageSolutions.Payment.Payment.enumPaymentSource.Budget;
                    //    objPayment.BudgetAssignmentID = this.ddlMyBudgetAssignment.SelectedValue;
                    //}
                    //else
                    if (chkApplyBudget.Checked)
                    {
                        ImageSolutions.Budget.MyBudgetAssignment MyBudgetAssignment = new ImageSolutions.Budget.MyBudgetAssignment(CurrentUser.UserInfoID, ddlMyBudgetAssignment.SelectedValue);

                        double dblTotal = double.Parse(litTotal.Text, NumberStyles.AllowCurrencySymbol | NumberStyles.Currency);
                        double dblRemainingTotal = double.Parse(litRemainingTotal.Text, NumberStyles.AllowCurrencySymbol | NumberStyles.Currency);
                        double AmountBudgetApplied = MyBudgetAssignment.Balance > dblTotal ? dblTotal : MyBudgetAssignment.Balance;

                        if (Convert.ToDecimal(dblTotal) - Convert.ToDecimal(AmountBudgetApplied) != Convert.ToDecimal(dblRemainingTotal))
                        {
                            this.ddlMyBudgetAssignment.DataSource = CurrentUser.CurrentUserWebSite.MyBudgetAssignments.FindAll(x => x.BudgetAssignment.Budget.StartDate <= DateTime.UtcNow && x.BudgetAssignment.Budget.EndDate >= DateTime.UtcNow.AddDays(-1));
                            this.ddlMyBudgetAssignment.DataBind();
                            UpdateShoppingCart();

                            UpdateApplyBudget();

                            throw new Exception(String.Format("Budget updated"));
                        }

                        //if (AmountBudgetApplied > 0)
                        //{
                        objBudgetPayment = new ImageSolutions.Payment.Payment();
                        objBudgetPayment.UserInfoID = CurrentUser.UserInfoID;
                        objBudgetPayment.SalesOrderID = objSalesOrder.SalesOrderID;
                        objBudgetPayment.AmountPaid = AmountBudgetApplied;
                        objBudgetPayment.PaymentSource = ImageSolutions.Payment.Payment.enumPaymentSource.Budget;
                        objBudgetPayment.BudgetAssignmentID = this.ddlMyBudgetAssignment.SelectedValue;
                        objBudgetPayment.CreatedBy = CurrentUser.UserInfoID;
                        objBudgetPayment.Create(objConn, objTran);
                        //}

                        if (dblRemainingTotal > 0)
                        {
                            ImageSolutions.Budget.BudgetAssignment BudgetAssignment = new ImageSolutions.Budget.BudgetAssignment(MyBudgetAssignment.BudgetAssignmentID);
                            if (BudgetAssignment.Budget.AllowOverBudget)
                            {
                                ImageSolutions.SalesOrder.SalesOrder Salesorder = new SalesOrder(objSalesOrder.SalesOrderID);
                                Salesorder.IsPendingApproval = !string.IsNullOrEmpty(BudgetAssignment.Budget.ApproverUserWebsiteID);
                                Salesorder.Update(objConn, objTran);

                                if (!string.IsNullOrEmpty(BudgetAssignment.Budget.PaymentTermID))
                                {
                                    strBudgetPaymentTermID = BudgetAssignment.Budget.PaymentTermID;
                                    //objPayment.PaymentSource = ImageSolutions.Payment.Payment.enumPaymentSource.Invoice;
                                    //objPayment.PaymentTermID = BudgetAssignment.Budget.PaymentTermID;
                                }
                            }
                        }

                        objPayment.AmountPaid = dblRemainingTotal;

                    }
                    else
                    {
                        if (pnlShippingAndTaxes.Visible)
                        {
                            if (chkPartialShipping.Checked)
                            {
                                objPayment.AmountPaid = CurrentUser.CurrentUserWebSite.ShoppingCart.GetTotalWithPartialShipping(CurrentWebsite.WebsiteID, CurrentUser.CurrentUserWebSite.IsTaxExempt);
                            }
                            else
                            {
                                objPayment.AmountPaid = CurrentUser.CurrentUserWebSite.ShoppingCart.GetTotal(CurrentWebsite.WebsiteID, CurrentUser.CurrentUserWebSite.IsTaxExempt);
                            }

                            if (CurrentWebsite.EnableIPD && IsInternational())
                            {
                                objPayment.AmountPaid = objPayment.AmountPaid + double.Parse(litIPDAmount.Text, NumberStyles.AllowCurrencySymbol | NumberStyles.Currency);
                            }
                        }
                        else
                        {
                            objPayment.AmountPaid = CurrentUser.CurrentUserWebSite.ShoppingCart.GetTotal(CurrentWebsite.WebsiteID, true) - CurrentUser.CurrentUserWebSite.ShoppingCart.ShippingAmount;
                        }
                    }

                    if (objSalesOrder.UserInfoID != objSalesOrder.UserWebsite.UserInfoID)
                    {
                        throw new Exception("Cannot identify the user.  Please refresh the page.");
                    }

                    CurrentUser.CurrentUserWebSite.ShoppingCart.SalesOrderID = objSalesOrder.SalesOrderID;
                    CurrentUser.CurrentUserWebSite.ShoppingCart.Update(objConn, objTran);

                    //Dedut Inventory
                    foreach (SalesOrderLine _SalesOrderLine in objSalesOrder.SalesOrderLines)
                    {
                        if (_SalesOrderLine.Item.ItemType == "_inventoryItem")
                        {
                            _SalesOrderLine.Item.QuantityAvailable = _SalesOrderLine.Item.QuantityAvailable - _SalesOrderLine.Quantity;
                            if (_SalesOrderLine.Item.QuantityAvailable < 0) _SalesOrderLine.Item.QuantityAvailable = 0;
                            _SalesOrderLine.Item.Update(objConn, objTran);
                        }
                        else
                        {
                            _SalesOrderLine.Item.VendorInventory = _SalesOrderLine.Item.VendorInventory - _SalesOrderLine.Quantity;
                            if (_SalesOrderLine.Item.VendorInventory < 0) _SalesOrderLine.Item.VendorInventory = 0;
                            _SalesOrderLine.Item.Update(objConn, objTran);
                        }
                    }


                    if (CurrentWebsite.EnablePackagePayment)
                    {
                        CurrentUser.CurrentUserWebSite.PackageAvailableDate = DateTime.UtcNow.AddYears(1);
                        CurrentUser.CurrentUserWebSite.Update(objConn, objTran);
                    }

                    if (objPayment.AmountPaid > 0)
                    {
                        ImageSolutions.ShoppingCart.ShoppingCart ShoppingCart = new ImageSolutions.ShoppingCart.ShoppingCart(CurrentUser.CurrentUserWebSite.ShoppingCart.ShoppingCartID);
                        if (Convert.ToString(ShoppingCart.CheckoutSession) != Convert.ToString(CheckoutSesssion))
                        {
                            throw new Exception("Session Updated");
                        }

                        if (rbnInvoicePayment.Checked)
                        {
                            objPayment.PaymentSource = ImageSolutions.Payment.Payment.enumPaymentSource.Invoice;
                            //objPayment.PaymentTerm = CurrentUser.CurrentUserWebSite.PaymentTerm.Description;

                            if (CurrentUser.CurrentUserWebSite.PaymentTermAmount != null && CurrentUser.CurrentUserWebSite.PaymentTermBalance < Convert.ToDecimal(objPayment.AmountPaid))
                            {
                                throw new Exception("Not enough balance available for invoice payment");
                            }

                            objPayment.PaymentTermID = !string.IsNullOrEmpty(strBudgetPaymentTermID) ? strBudgetPaymentTermID : CurrentUser.CurrentUserWebSite.PaymentTermID;
                        }
                        else if (rbnUserCreditCard.Checked)
                        {
                            if (CurrentWebsite.CreditCardLimitPerOrder != null && CurrentWebsite.CreditCardLimitPerOrder != 0 && Convert.ToDecimal(objPayment.AmountPaid) > Convert.ToDecimal(CurrentWebsite.CreditCardLimitPerOrder))
                            {
                                throw new Exception(String.Format("Credit card amount exceeded the purchase limit", CurrentWebsite.CreditCardLimitPerOrder));
                            }


                            ImageSolutions.CreditCard.CreditCard objCreditCard = new ImageSolutions.CreditCard.CreditCard(this.ddlUserCreditCard.SelectedValue);

                            ImageSolutions.User.UserCreditCard objUserCreditCard = new ImageSolutions.User.UserCreditCard();
                            ImageSolutions.User.UserCreditCardFilter objUserCreditCardFilter = new ImageSolutions.User.UserCreditCardFilter();
                            objUserCreditCardFilter.CreditCardID = new Database.Filter.StringSearch.SearchFilter();
                            objUserCreditCardFilter.CreditCardID.SearchString = objCreditCard.CreditCardID;
                            objUserCreditCardFilter.UserInfoID = new Database.Filter.StringSearch.SearchFilter();
                            objUserCreditCardFilter.UserInfoID.SearchString = CurrentUser.UserInfoID;
                            objUserCreditCard = ImageSolutions.User.UserCreditCard.GetUserCreditCard(objUserCreditCardFilter);
                            if (objUserCreditCard.RemainingBalance != null && objUserCreditCard.RemainingBalance < objPayment.AmountPaid) //CurrentUser.CurrentUserWebSite.ShoppingCart.GetTotal(CurrentWebsite.WebsiteID, CurrentUser.CurrentUserWebSite.IsTaxExempt))
                            {
                                throw new Exception("Not enough balance remaining on the selected credit card");
                            }

                            //Charge Card

                            //Paystand
                            //ImageSolutions.Paystand.Payment Payment = null;
                            //string strPaymentData = String.Empty;
                            //ImageSolutions.Paystand.Paystand Paystand = new ImageSolutions.Paystand.Paystand();
                            //if (!string.IsNullOrEmpty(objCreditCard.PayerExternalID) && !string.IsNullOrEmpty(objCreditCard.CardExternalID))
                            //{
                            //    Payment = Paystand.ChargeExistingCard(objCreditCard
                            //        , Convert.ToString(CurrentUser.CurrentUserWebSite.ShoppingCart.Total), "USD");
                            //    strPaymentData = JsonSerializer.Serialize(Payment);
                            //}
                            //else
                            //{
                            //    Payment = Paystand.ChargeNewCard(objCreditCard.Nickname.Trim()
                            //        , objCreditCard.FullName.Trim()
                            //        , Decrypt(objCreditCard.Data, objCreditCard.GUID)
                            //        , objCreditCard.CreditCardType
                            //        , Convert.ToInt32(objCreditCard.ExpirationMonth) < 10 ? "0" + Convert.ToString(objCreditCard.ExpirationMonth) : Convert.ToString(objCreditCard.ExpirationMonth)
                            //        , objCreditCard.ExpirationYear
                            //        , objCreditCard.CVV
                            //        , CurrentUser.EmailAddress
                            //        , objCreditCard.BillingAddressBook
                            //        , Convert.ToString(CurrentUser.CurrentUserWebSite.ShoppingCart.Total)
                            //        , "USD"
                            //        , CurrentUser.CurrentUserWebSite.UserInfo
                            //        , false
                            //        , objCreditCard);

                            //    strPaymentData = JsonSerializer.Serialize(Payment);
                            //}
                            Stripe.Charge StripeCharge = null;
                            string strPaymentData = String.Empty;
                            ImageSolutions.StripeAPI.StripeAPI StripeAPI = new ImageSolutions.StripeAPI.StripeAPI();
                            decimal amount = Decimal.Multiply(Convert.ToDecimal(objPayment.AmountPaid), Convert.ToDecimal(100.0));
                            if (!string.IsNullOrEmpty(objCreditCard.PayerExternalID) && !string.IsNullOrEmpty(objCreditCard.CardExternalID))
                            {
                                StripeCharge = StripeAPI.Charge(objCreditCard.PayerExternalID
                                    , objCreditCard.CardExternalID
                                    , (long)amount
                                    , "usd"
                                    , objSalesOrder.SalesOrderID
                                    , CurrentWebsite.Abbreviation);
                                strPaymentData = StripeCharge.StripeResponse.Content;
                                strCreditCardTransactionID = StripeCharge.Id;
                            }
                            else
                            {
                                StripeCharge = StripeAPI.ChargeNewCard(
                                    objCreditCard.FullName.Trim()
                                    , Decrypt(objCreditCard.Data, objCreditCard.GUID)
                                    , Convert.ToInt32(objCreditCard.ExpirationMonth)
                                    , Convert.ToInt32(objCreditCard.ExpirationYear)
                                    , objCreditCard.CVV
                                    , objCreditCard.CreditCardType
                                    , (long)amount
                                    , objSalesOrder.SalesOrderID
                                    , "usd"
                                    , objCreditCard.BillingAddressBook
                                    , CurrentUser.CurrentUserWebSite.UserInfo
                                    , false
                                    , objCreditCard.Nickname.Trim()
                                    , objCreditCard
                                    , objConn
                                    , objTran
                                    , CurrentWebsite.Abbreviation);
                                strPaymentData = StripeCharge.StripeResponse.Content;
                                strCreditCardTransactionID = StripeCharge.Id;
                            }

                            objPayment.PaymentSource = ImageSolutions.Payment.Payment.enumPaymentSource.CreditCard;
                            objCreditCardTransactionLog = new ImageSolutions.CreditCard.CreditCardTransactionLog();
                            objCreditCardTransactionLog.UserInfoID = CurrentUser.UserInfoID;
                            objCreditCardTransactionLog.TransactionType = ImageSolutions.CreditCard.CreditCard.enumTransactionType.AUTH_CAPTURE;
                            objCreditCardTransactionLog.Amount = CurrentUser.CurrentUserWebSite.ShoppingCart.GetTotal(CurrentWebsite.WebsiteID, CurrentUser.CurrentUserWebSite.IsTaxExempt);
                            objCreditCardTransactionLog.CCName = objCreditCard.FullName;
                            objCreditCardTransactionLog.CCLastFourNumber = objCreditCard.LastFourDigit;
                            objCreditCardTransactionLog.CCExpiration = objCreditCard.ExpirationDate.Month.ToString("00") + objCreditCard.ExpirationDate.Year;
                            objCreditCardTransactionLog.CCCVV = objCreditCard.CVV;

                            objCreditCardTransactionLog.ResponseReasonText = Convert.ToString(strPaymentData);

                            objCreditCardTransactionLog.TransactionID = StripeCharge.Id;
                            objCreditCardTransactionLog.Status = StripeCharge.Status;

                            objCreditCardTransactionLog.Create(objConn, objTran);
                            objPayment.CreditCardTransactionLogID = objCreditCardTransactionLog.CreditCardTransactionLogID;
                            objPayment.CreditCardID = ddlUserCreditCard.SelectedValue;

                            //Discount Tire - Temp approve cc orders
                            if (objSalesOrder.WebsiteID == "21" || objSalesOrder.WebsiteID == "54")
                            {
                                objSalesOrder.IsPendingApproval = false;
                                objSalesOrder.Update(objConn, objTran);
                            }
                        }
                        else if (rbnNewCreditCard.Checked)
                        {
                            if (CurrentWebsite.CreditCardLimitPerOrder != null && CurrentWebsite.CreditCardLimitPerOrder != 0 && Convert.ToDecimal(objPayment.AmountPaid) > Convert.ToDecimal(CurrentWebsite.CreditCardLimitPerOrder))
                            {
                                throw new Exception(String.Format("Credit card amount exceeded the purchase limit of {0:c}", CurrentWebsite.CreditCardLimitPerOrder));
                            }

                            //ImageSolutions.Paystand.Paystand Paystand = new ImageSolutions.Paystand.Paystand();
                            //ImageSolutions.Paystand.Payment Payment = Paystand.ChargeNewCard( txtNickname.Text.Trim()
                            //    , txtCCFullName.Text.Trim()
                            //    , txtCCCardNumber.Text.Trim()
                            //    , txtCCType.Text.Trim()
                            //    , ddlCCMonth.SelectedValue
                            //    , ddlCCYear.SelectedValue
                            //    , txtCCCVV.Text
                            //    , CurrentUser.EmailAddress
                            //    , objBillingAddressBook
                            //    , Convert.ToString(CurrentUser.CurrentUserWebSite.ShoppingCart.Total)
                            //    , "USD"
                            //    , CurrentUser.CurrentUserWebSite.UserInfo
                            //    , cbSaveCard.Checked);
                            //string strPaymentData = JsonSerializer.Serialize(Payment);
                            Stripe.Charge StripeCharge = null;
                            string strPaymentData = String.Empty;
                            ImageSolutions.StripeAPI.StripeAPI StripeAPI = new ImageSolutions.StripeAPI.StripeAPI();
                            //double amount = CurrentUser.CurrentUserWebSite.ShoppingCart.Total * 100;
                            //double amount = objPayment.AmountPaid * 100;
                            decimal amount = Decimal.Multiply(Convert.ToDecimal(objPayment.AmountPaid), Convert.ToDecimal(100.0));
                            StripeCharge = StripeAPI.ChargeNewCard(
                                    txtCCFullName.Text.Trim()
                                    , txtCCCardNumber.Text.Trim()
                                    , Convert.ToInt32(ddlCCMonth.SelectedValue)
                                    , Convert.ToInt32(ddlCCYear.SelectedValue)
                                    , txtCCCVV.Text
                                    , txtCCType.Text.Trim()
                                    , (long)amount
                                    , objSalesOrder.SalesOrderID //CurrentUser.CurrentUserWebSite.ShoppingCart.ShoppingCartID
                                    , "usd"
                                    , objBillingAddressBook
                                    , CurrentUser.CurrentUserWebSite.UserInfo
                                    , cbSaveCard.Checked
                                    , txtNickname.Text.Trim()
                                    , null
                                    , objConn
                                    , objTran
                                    , CurrentWebsite.Abbreviation);
                            strPaymentData = StripeCharge.StripeResponse.Content;
                            strCreditCardTransactionID = StripeCharge.Id;

                            objPayment.PaymentSource = ImageSolutions.Payment.Payment.enumPaymentSource.CreditCard;
                            objCreditCardTransactionLog = new ImageSolutions.CreditCard.CreditCardTransactionLog();
                            objCreditCardTransactionLog.UserInfoID = CurrentUser.UserInfoID;
                            objCreditCardTransactionLog.TransactionType = ImageSolutions.CreditCard.CreditCard.enumTransactionType.AUTH_CAPTURE;
                            objCreditCardTransactionLog.Amount = CurrentUser.CurrentUserWebSite.ShoppingCart.GetTotal(CurrentWebsite.WebsiteID, CurrentUser.CurrentUserWebSite.IsTaxExempt);
                            objCreditCardTransactionLog.CCName = this.txtCCFullName.Text.Trim();
                            objCreditCardTransactionLog.CCLastFourNumber = this.txtCCCardNumber.Text.Trim().Length >= 4 ? this.txtCCCardNumber.Text.Trim().Substring(this.txtCCCardNumber.Text.Trim().Length - 4) : this.txtCCCardNumber.Text.Trim();
                            objCreditCardTransactionLog.CCExpiration = this.ddlCCMonth.SelectedValue + this.ddlCCYear.SelectedValue;
                            objCreditCardTransactionLog.CCCVV = this.txtCCCVV.Text.Trim();

                            objCreditCardTransactionLog.ResponseReasonText = Convert.ToString(strPaymentData);

                            objCreditCardTransactionLog.TransactionID = StripeCharge.Id;
                            objCreditCardTransactionLog.Status = StripeCharge.Status;

                            objCreditCardTransactionLog.Create(objConn, objTran);
                            objPayment.CreditCardTransactionLogID = objCreditCardTransactionLog.CreditCardTransactionLogID;
                            objPayment.CreditCardID = StripeCharge.Description;

                            //Discount Tire - Temp approve approve cc orders
                            if (objSalesOrder.WebsiteID == "21" || objSalesOrder.WebsiteID == "54")
                            {
                                objSalesOrder.IsPendingApproval = false;
                                objSalesOrder.Update(objConn, objTran);
                            }
                        }

                        else if (CurrentWebsite.EnablePackagePayment)
                        {
                            objPayment.PaymentSource = ImageSolutions.Payment.Payment.enumPaymentSource.Invoice;

                            //if (CurrentUser.CurrentUserWebSite.PaymentTermAmount != null && CurrentUser.CurrentUserWebSite.PaymentTermBalance < Convert.ToDecimal(objPayment.AmountPaid))
                            //{
                            //    throw new Exception("Not enough balance available for invoice payment");
                            //}

                            objPayment.PaymentTermID = CurrentWebsite.DefaultPaymentTermID;
                        }

                        objPayment.CreatedBy = CurrentUser.UserInfoID;
                        objPayment.Create(objConn, objTran);
                    }

                    objTran.Commit();

                    if (CurrentWebsite.EnableSMSOptIn || CurrentUser.CurrentUserWebSite.EnableSMSOptIn)
                    {
                        MarketingTemplate objMarketingTemplate = null;
                        MarketingTemplateFilter objMarketingTemplateFilter = null;
                        SMSOutbox objSMSOutbox = null;

                        try
                        {
                            objMarketingTemplateFilter = new MarketingTemplateFilter();
                            objMarketingTemplateFilter.WebsiteID = new Database.Filter.StringSearch.SearchFilter();
                            objMarketingTemplateFilter.WebsiteID.SearchString = CurrentWebsite.WebsiteID;
                            objMarketingTemplateFilter.MarketingCampaigns = new List<MarketingTemplate.enumMarketingCampaign>();
                            objMarketingTemplateFilter.MarketingCampaigns.Add(MarketingTemplate.enumMarketingCampaign.OrderConfirmation);

                            objMarketingTemplateFilter.IsEnabled = true;

                            objMarketingTemplate = MarketingTemplate.GetMarketingTemplate(objMarketingTemplateFilter);

                            if (objMarketingTemplate != null)
                            {
                                objSMSOutbox = new SMSOutbox();
                                objSMSOutbox.MarketingTemplateID = objMarketingTemplate.MarketingTemplateID;
                                objSMSOutbox.UserWebsiteID = objSalesOrder.UserWebsiteID;
                                objSMSOutbox.Message = objMarketingTemplate.SMSContent.Replace("{$FirstName}", CurrentUser.FirstName)
                                                                                      .Replace("{$Website}", CurrentWebsite.Name)
                                                                                      .Replace("{$OrderNumber}", objSalesOrder.SalesOrderID);
                                objSMSOutbox.SMSMobileNumber = objSalesOrder.UserWebsite.SMSMobileNumber;
                                objSMSOutbox.Create();
                            }
                        }
                        catch (Exception ex)
                        {
                            //do not throw error
                        }
                        finally
                        {
                            objMarketingTemplate = null;
                            objMarketingTemplateFilter = null;
                            objSMSOutbox = null;
                        }
                    }

                    if (CurrentWebsite.EnableEmailOptIn && CurrentUser.CurrentUserWebSite.EmailOptIn)
                    {
                        MarketingTemplate objMarketingTemplate = null;
                        MarketingTemplateFilter objMarketingTemplateFilter = null;
                        EmailOutbox objEmailOutbox = null;
                        string strOrderSummary = string.Empty;

                        try
                        {
                            objMarketingTemplateFilter = new MarketingTemplateFilter();
                            objMarketingTemplateFilter.WebsiteID = new Database.Filter.StringSearch.SearchFilter();
                            objMarketingTemplateFilter.WebsiteID.SearchString = CurrentWebsite.WebsiteID;
                            objMarketingTemplateFilter.MarketingCampaigns = new List<MarketingTemplate.enumMarketingCampaign>();
                            objMarketingTemplateFilter.MarketingCampaigns.Add(MarketingTemplate.enumMarketingCampaign.OrderConfirmation);
                            objMarketingTemplate = MarketingTemplate.GetMarketingTemplate(objMarketingTemplateFilter);

                            if (objMarketingTemplate != null)
                            {
                                objEmailOutbox = new EmailOutbox();
                                objEmailOutbox.MarketingTemplateID = objMarketingTemplate.MarketingTemplateID;
                                objEmailOutbox.UserWebsiteID = objSalesOrder.UserWebsiteID;
                                objEmailOutbox.Subject = objMarketingTemplate.EmailSubject.Replace("{$FirstName}", CurrentUser.FirstName)
                                                                                          .Replace("{$Website}", CurrentWebsite.Name);
                                objEmailOutbox.ToEmail = CurrentUser.CurrentUserWebSite.NotificationEmail;

                                foreach (SalesOrderLine objSalesOrderLine in objSalesOrder.SalesOrderLines)
                                {
                                    strOrderSummary += "<li>" + objSalesOrderLine.Quantity + " x " + (!string.IsNullOrEmpty(objSalesOrderLine.Description) ? objSalesOrderLine.Description : objSalesOrderLine.Item.SalesDescription) + "</li>";
                                    //strOrderSummary += "<li>Qty: " + objSalesOrderLine.Quantity + " <strong>" + (!string.IsNullOrEmpty(objSalesOrderLine.Description) ? objSalesOrderLine.Description : objSalesOrderLine.Item.SalesDescription) + "</strong></li>";
                                }

                                objEmailOutbox.HTMLContent = objMarketingTemplate.EmailContent.Replace("{$FirstName}", CurrentUser.FirstName)
                                                                                              .Replace("{$OrderNumber}", objSalesOrder.SalesOrderID)
                                                                                              .Replace("{$OrderSummary}", strOrderSummary)
                                                                                              .Replace("{$Website}", CurrentWebsite.Name);
                                objEmailOutbox.IsApproved = false;
                                objEmailOutbox.Create();
                            }
                        }
                        catch (Exception ex)
                        {
                            //do not throw error
                        }
                        finally
                        {
                            objMarketingTemplate = null;
                            objMarketingTemplateFilter = null;
                            objEmailOutbox = null;
                        }
                    }
                    else if (objSalesOrder.UserWebsite.OptInForNotification)
                    {
                        //Send Order Confirmation to existing user
                        SendOrderConfirmation(objSalesOrder, string.IsNullOrEmpty(objSalesOrder.UserWebsite.NotificationEmail) ? objSalesOrder.UserInfo.EmailAddress : objSalesOrder.UserWebsite.NotificationEmail);
                    }

                    if (objPayment.AmountPaid > 0
                        || (objSalesOrder.AccountID == "4953"
                            ||
                            objSalesOrder.AccountID == "4954"
                            ||
                            objSalesOrder.AccountID == "4955"
                            ||
                            objSalesOrder.AccountID == "6606"
                        )
                    )
                    {
                        bool blnAccountApproval = SendOrderAccountApprovalEmail(objSalesOrder);
                        if (!objSalesOrder.IsPendingApproval && blnAccountApproval)
                        {
                            ImageSolutions.SalesOrder.SalesOrder Salesorder = new SalesOrder(objSalesOrder.SalesOrderID);
                            Salesorder.IsPendingApproval = blnAccountApproval;
                            Salesorder.Update();
                        }
                    }

                    if (blnRequireItemPersonalizationApproval || blnRequireItemApproval)
                    {
                        bool blnAccountPersonalizationApproval = SendOrderPersonalizationApprovalEmail(objSalesOrder);

                        ImageSolutions.SalesOrder.SalesOrder Salesorder = new SalesOrder(objSalesOrder.SalesOrderID);
                        Salesorder.IsPendingItemPersonalizationApproval = blnAccountPersonalizationApproval;
                        Salesorder.Update();
                    }



                    //Send Approval Email to Parent Account
                    //if (CurrentUser.CurrentUserWebSite.WebSite.OrderApprovalRequired)
                    //{
                    //    bool blnApproverFound = false;

                    //    //Look for users assigned to the same account with Order Management = True
                    //    if (CurrentUser.CurrentUserWebSite.CurrentUserAccount.Account != null)
                    //    {                           
                    //        if (!blnApproverFound)
                    //        {
                    //            foreach (ImageSolutions.User.UserAccount objUserAccount in CurrentUser.CurrentUserWebSite.CurrentUserAccount.Account.UserAccounts.FindAll(m => m.UserWebsite.OrderManagement && m.UserWebsite.OptInForNotification))
                    //            {
                    //                if (!string.IsNullOrEmpty(objUserAccount.UserWebsite.UserInfo.EmailAddress))
                    //                {
                    //                    OrderApprovalNotification(objSalesOrder, string.IsNullOrEmpty(objUserAccount.UserWebsite.NotificationEmail) ? objUserAccount.UserWebsite.UserInfo.EmailAddress : objUserAccount.UserWebsite.NotificationEmail);
                    //                    blnApproverFound = true;
                    //                }
                    //            }
                    //        }
                    //    }

                    //    if (!blnApproverFound)
                    //    {
                    //        //Or look for the parent account with Order Management = True
                    //        if (CurrentUser.CurrentUserWebSite.CurrentUserAccount.Account.ParentAccount != null)
                    //        {
                    //            foreach (ImageSolutions.User.UserAccount objParentUserAccount in CurrentUser.CurrentUserWebSite.CurrentUserAccount.Account.ParentAccount.UserAccounts.FindAll(m => m.UserWebsite.OrderManagement && m.UserWebsite.OptInForNotification))
                    //            {
                    //                if (!string.IsNullOrEmpty(objParentUserAccount.UserWebsite.UserInfo.EmailAddress))
                    //                {
                    //                    OrderApprovalNotification(objSalesOrder, string.IsNullOrEmpty(objParentUserAccount.UserWebsite.NotificationEmail) ? objParentUserAccount.UserWebsite.UserInfo.EmailAddress : objParentUserAccount.UserWebsite.NotificationEmail);
                    //                }
                    //            }
                    //        }
                    //    }
                    //}

                    blnReturn = true;
                }
            }
            catch (Exception ex)
            {
                //ucLoading.Visible = false;

                if (objTran != null && objTran.Connection != null)
                {
                    objTran.Rollback();

                    SaveErrorLog(ex.Message);
                    if (!string.IsNullOrEmpty(strCreditCardTransactionID))
                    {
                        SaveErrorLog(String.Format(@"Credit Card Transaction Created: {0}", strCreditCardTransactionID));
                        //refund cc
                        //ImageSolutions.StripeAPI.StripeAPI StripeAPI = new ImageSolutions.StripeAPI.StripeAPI();
                        //StripeAPI.Refund(strCreditCardTransactionID);
                    }
                }

                WebUtility.DisplayJavascriptMessage(this, string.Format(@"{0}", ex.Message));
            }
            finally
            {
                //ucLoading.Visible = false;

                if (objTran != null) objTran.Dispose();
                objTran = null;
                if (objConn != null) objConn.Dispose();
                objConn = null;
            }

            if (blnReturn) Response.Redirect(string.Format("/OrderConfirmation.aspx?SalesOrderID={0}", objSalesOrder.SalesOrderID));
        }

        protected bool SendOrderAccountApprovalEmail(SalesOrder salesorder)
        {
            bool blnReturn = false;

            List<ImageSolutions.Account.AccountOrderApproval> AccountOrderApprovals = new List<ImageSolutions.Account.AccountOrderApproval>();
            ImageSolutions.Account.AccountOrderApprovalFilter AccountOrderApprovalFilter = new ImageSolutions.Account.AccountOrderApprovalFilter();
            AccountOrderApprovalFilter.AccountID = new Database.Filter.StringSearch.SearchFilter();
            AccountOrderApprovalFilter.AccountID.SearchString = CurrentUser.CurrentUserWebSite.CurrentUserAccount.Account.AccountID;
            AccountOrderApprovals = ImageSolutions.Account.AccountOrderApproval.GetAccountOrderApprovals(AccountOrderApprovalFilter);

            if (AccountOrderApprovals != null && AccountOrderApprovals.Count > 0)
            {
                AccountOrderApprovals = AccountOrderApprovals.Where(m => m.Amount <= Convert.ToDecimal(salesorder.Total)).ToList();

                if (AccountOrderApprovals != null && AccountOrderApprovals.Count > 0)
                {
                    //ImageSolutions.Account.AccountOrderApproval AccountOrderApproval = new ImageSolutions.Account.AccountOrderApproval();
                    //AccountOrderApproval = AccountOrderApprovals.OrderByDescending(m => m.Amount).First();

                    //if (AccountOrderApproval.UserWebsite.UserInfoID != CurrentUser.UserInfoID)
                    //{
                    //    if (AccountOrderApproval.UserWebsite.OptInForNotification)
                    //    {
                    //        OrderApprovalNotification(salesorder, AccountOrderApproval.UserWebsite); //AccountOrderApproval.UserWebsite.UserInfo.EmailAddress);
                    //    }
                    //    blnReturn = true;
                    //}                    

                    foreach (ImageSolutions.Account.AccountOrderApproval _AccountOrderApproval in AccountOrderApprovals.Where(x => x.Amount == AccountOrderApprovals.Max(y => y.Amount)))
                    {
                        if (_AccountOrderApproval.UserWebsite.UserInfoID != CurrentUser.UserInfoID)
                        {
                            if (_AccountOrderApproval.UserWebsite.OptInForNotification)
                            {
                                OrderApprovalNotification(salesorder, _AccountOrderApproval.UserWebsite);
                            }
                            blnReturn = true;
                        }
                    }
                }
            }

            if (!blnReturn)
            {
                if (CurrentUser.CurrentUserWebSite.CurrentUserAccount.Account.ParentAccount != null)
                {
                    List<ImageSolutions.Account.AccountOrderApproval> ParentAccountOrderApprovals = new List<ImageSolutions.Account.AccountOrderApproval>();
                    ImageSolutions.Account.AccountOrderApprovalFilter ParentAccountOrderApprovalFilter = new ImageSolutions.Account.AccountOrderApprovalFilter();
                    ParentAccountOrderApprovalFilter.AccountID = new Database.Filter.StringSearch.SearchFilter();
                    ParentAccountOrderApprovalFilter.AccountID.SearchString = CurrentUser.CurrentUserWebSite.CurrentUserAccount.Account.ParentAccount.AccountID;
                    ParentAccountOrderApprovals = ImageSolutions.Account.AccountOrderApproval.GetAccountOrderApprovals(ParentAccountOrderApprovalFilter);

                    if (ParentAccountOrderApprovals != null && ParentAccountOrderApprovals.Count > 0)
                    {
                        ParentAccountOrderApprovals = ParentAccountOrderApprovals.Where(m => m.Amount <= Convert.ToDecimal(salesorder.Total)).ToList();

                        if (ParentAccountOrderApprovals != null && ParentAccountOrderApprovals.Count > 0)
                        {
                            //ImageSolutions.Account.AccountOrderApproval AccountOrderApproval = new ImageSolutions.Account.AccountOrderApproval();
                            //AccountOrderApproval = AccountOrderApprovals.OrderByDescending(m => m.Amount).First();

                            //if (AccountOrderApproval.UserWebsite.UserInfoID != CurrentUser.UserInfoID)
                            //{
                            //    if (AccountOrderApproval.UserWebsite.OptInForNotification)
                            //    {
                            //        OrderApprovalNotification(salesorder, AccountOrderApproval.UserWebsite); //AccountOrderApproval.UserWebsite.UserInfo.EmailAddress);
                            //    }y
                            //    blnReturn = true;
                            //}

                            foreach (ImageSolutions.Account.AccountOrderApproval _AccountOrderApproval in AccountOrderApprovals.Where(x => x.Amount == ParentAccountOrderApprovals.Max(y => y.Amount)))
                            {
                                if (_AccountOrderApproval.UserWebsite.UserInfoID != CurrentUser.UserInfoID)
                                {
                                    if (_AccountOrderApproval.UserWebsite.OptInForNotification)
                                    {
                                        OrderApprovalNotification(salesorder, _AccountOrderApproval.UserWebsite);
                                    }
                                    blnReturn = true;
                                }
                            }
                        }
                    }
                }
            }

            return blnReturn;
        }

        protected ImageSolutions.User.UserWebsite GetPersonalizationAppover(string accountid)
        {
            ImageSolutions.User.UserWebsite UserWebsite = null;
            ImageSolutions.Account.Account Account = null;

            try
            {
                Account = new ImageSolutions.Account.Account(accountid);

                if (!string.IsNullOrEmpty(Account.PersonalizationApproverUserWebsiteID))
                {
                    UserWebsite = new UserWebsite(Account.PersonalizationApproverUserWebsiteID);
                }
                else if (!string.IsNullOrEmpty(Account.ParentID))
                {
                    UserWebsite = GetPersonalizationAppover(Account.ParentID);
                }
            }
            catch (Exception ex)
            {

            }

            return UserWebsite;
        }

        protected ImageSolutions.User.UserWebsite GetPersonalizationAppover2(string accountid)
        {
            ImageSolutions.User.UserWebsite UserWebsite = null;
            ImageSolutions.Account.Account Account = null;

            try
            {
                Account = new ImageSolutions.Account.Account(accountid);

                if (!string.IsNullOrEmpty(Account.PersonalizationApprover2UserWebsiteID))
                {
                    UserWebsite = new ImageSolutions.User.UserWebsite(Account.PersonalizationApprover2UserWebsiteID);
                }
                else if (string.IsNullOrEmpty(Account.PersonalizationApproverUserWebsiteID) && !string.IsNullOrEmpty(Account.ParentID))
                {
                    UserWebsite = GetPersonalizationAppover2(Account.ParentID);
                }
            }
            catch (Exception ex)
            {

            }

            return UserWebsite;
        }

        protected bool SendOrderPersonalizationApprovalEmail(SalesOrder salesorder)
        {
            bool blnReturn = false;
            ImageSolutions.User.UserWebsite UserWebsite = new ImageSolutions.User.UserWebsite();
            ImageSolutions.User.UserWebsite UserWebsite2 = new ImageSolutions.User.UserWebsite();
            //List<string> objEmails = null;
            List<ImageSolutions.User.UserWebsite> UserWebsites = null;

            try
            {
                //objEmails = new List<string>();
                UserWebsites = new List<UserWebsite>();
                if (CurrentWebsite.CombineWebsiteGroup)
                {
                    foreach (ImageSolutions.User.UserAccount _UserAccount in CurrentUser.CurrentUserWebSite.UserAccounts)
                    {
                        UserWebsite = GetPersonalizationAppover(CurrentUser.CurrentUserWebSite.CurrentUserAccount.Account.AccountID);

                        if (UserWebsite != null && !string.IsNullOrEmpty(UserWebsite.UserWebsiteID))
                        {
                            //if(UserWebsite.UserInfoID != CurrentUser.UserInfoID && !objEmails.Exists(x => x == UserWebsite.UserInfo.EmailAddress))
                            if (UserWebsite.UserInfoID != CurrentUser.UserInfoID && !UserWebsites.Exists(x => x.UserWebsiteID == UserWebsite.UserWebsiteID))
                            {
                                if (UserWebsite.OptInForNotification)
                                {
                                    //objEmails.Add(UserWebsite.UserInfo.EmailAddress);
                                    UserWebsites.Add(UserWebsite);
                                }
                                blnReturn = true;
                            }
                        }

                        UserWebsite2 = GetPersonalizationAppover2(CurrentUser.CurrentUserWebSite.CurrentUserAccount.Account.AccountID);

                        if (UserWebsite2 != null && !string.IsNullOrEmpty(UserWebsite2.UserWebsiteID))
                        {
                            if (UserWebsite2.UserInfoID != CurrentUser.UserInfoID && !UserWebsites.Exists(x => x.UserWebsiteID == UserWebsite2.UserWebsiteID))
                            {
                                if (UserWebsite2.OptInForNotification)
                                {
                                    //objEmails.Add(UserWebsite.UserInfo.EmailAddress);
                                    UserWebsites.Add(UserWebsite2);
                                }
                                blnReturn = true;
                            }
                        }
                    }
                }
                else
                {
                    UserWebsite = GetPersonalizationAppover(CurrentUser.CurrentUserWebSite.CurrentUserAccount.Account.AccountID);

                    if (UserWebsite != null && !string.IsNullOrEmpty(UserWebsite.UserWebsiteID) && UserWebsite.UserInfoID != CurrentUser.UserInfoID)
                    {
                        if (UserWebsite.OptInForNotification)
                        {
                            //objEmails.Add(UserWebsite.UserInfo.EmailAddress);
                            UserWebsites.Add(UserWebsite);
                        }
                        blnReturn = true;
                    }

                    UserWebsite2 = GetPersonalizationAppover2(CurrentUser.CurrentUserWebSite.CurrentUserAccount.Account.AccountID);

                    if (UserWebsite2 != null && !string.IsNullOrEmpty(UserWebsite2.UserWebsiteID))
                    {
                        if (UserWebsite2.UserInfoID != CurrentUser.UserInfoID && !UserWebsites.Exists(x => x.UserWebsiteID == UserWebsite2.UserWebsiteID))
                        {
                            if (UserWebsite2.OptInForNotification)
                            {
                                UserWebsites.Add(UserWebsite2);
                            }
                            blnReturn = true;
                        }
                    }
                }

                //foreach(string _email in objEmails)
                //{
                //    PersonalizationApprovalNotification(salesorder, UserWebsite.UserInfo.EmailAddress);
                //}
                foreach (ImageSolutions.User.UserWebsite _UserWebsite in UserWebsites)
                {
                    PersonalizationApprovalNotification(salesorder, _UserWebsite); //UserWebsite.UserInfo.EmailAddress);
                }

            }
            catch (Exception ex)
            {

            }
            finally
            {
                UserWebsite = null;
                //objEmails = null;
            }

            return blnReturn;
        }

        protected bool PersonalizationApprovalNotification(SalesOrder salesorder, ImageSolutions.User.UserWebsite userwebsite) //string ToEmailAddress)
        {
            string strToEmailAddress = string.IsNullOrEmpty(userwebsite.NotificationEmail) ? userwebsite.UserInfo.EmailAddress : userwebsite.NotificationEmail;

            string strHTMLContent = @"<!DOCTYPE html>
                                        <html>
                                            <head></head>
                                            <body>
                                                <div style='font-size:14px;padding-bottom:30px;height:100%;position:relative;'>
                                                    <div style='float:right;width:45%; font-weight:bold; font-size:20px; text-align:right; padding-top:20px;'>Thank you for your order!</div>

                                                    <div style='float:left; width:45%;'><img src='${Logo}' style='width:2.5in; height:0.76in ' /></div>

                                                    <div style='clear:left;padding-top:40px;'>
                                                        <div style='font-size:16px; font-weight:bold;'>Hi ${FirstName},</div>
                                                        <p>A new order #<span style='color:blue; font-size:14px;'>${OrderNumber}</span> has been submitted for approval, please <a href='https://portal.imageinc.com/admin/Order.aspx?id=${OrderNumber}'>login</a> to the portal to approve this order.</p>
                                                    </div>
                                                    <table style='font-size:0.9em;width:100%;border-collapse:collapse;margin:10px 0;'>
                                                        <thead>
                                                            ${SalesOrderLineHeader}
                                                        </thead>
                                                        <tbody>
                                                            ${SalesOrderLine}
                                                        </tbody>
                                                    </table>
                                                    <table style='width:100%;border-collapse:collapse;margin:10px 0;'>
                                                        <tbody>
                                                            <tr>
                                                                <td>
                                                                    <table style='width:100%;border-collapse:collapse;margin:10px 0;'>
                                                                        <tbody>
                                                                            <tr>
                                                                                <td>
                                                                                    <b>Ship To:</b>
                                                                                    <br />
                                                                                    ${ShipTo}
                                                                                    <br />
                                                                                </td>
                                                                            </tr>
                                                                        </tbody>
                                                                    </table>
                                                                </td>
                                                                <td align='right'>
                                                                    <table>
                                                                        <tr>
                                                                            <td>
                                                                                <table style='width:100%;border-collapse:collapse;margin:10px 0;'>
                                                                                    <tbody>
                                                                                        <tr>
                                                                                            <td align='right'><b>Subtotal</b></td>
                                                                                            <td align='right'>${Subtotal}</td>
                                                                                        </tr>
                                                                                        <tr>
                                                                                            <td align='right'><b>Shipping & Handling</b></td>
                                                                                            <td align='right'>${Shipping}</td>
                                                                                        </tr>
                                                                                        <tr>
                                                                                            <td align='right'><b>Tax</b></td>
                                                                                            <td align='right'>${Tax}</td>
                                                                                        </tr>
                                                                                        <tr>
                                                                                            <td align='right' style='font-weight:bold;'><b>TOTAL</b></td>
                                                                                            <td align='right' style='font-weight:bold;'>${Total}</td>
                                                                                        </tr>
                                                                                    </tbody>
                                                                                </table>
                                                                            </td>
                                                                        </tr>
                                                                    </table>
                                                                </td>
                                                            </tr>
                                                        </tbody>
                                                    </table>

                                                    <div style='margin-top:30px;text-align:center;'>
                                                        We appreciate your business! Providing you with a great experience is very important to us.<br />
                                                        - Image Solutions Team
                                                    </div>
                                                </div>
                                            </body>
                                        </html>";

            try
            {
                string strSalesOrderLineHeader = string.Empty;
                string strSalesOrderLine = string.Empty;

                if (CurrentWebsite.DisplayTariffCharge)
                {
                    strSalesOrderLineHeader = @"<tr>
                                                    <th align='left' colspan='12' style='background-color:#ddd;text-transform:uppercase;padding:10px 6px;border:1px solid #333;border-left:none;border-right:none;'>Item</th>
                                                    <th colspan='3' align='center' style='background-color:#ddd;text-transform:uppercase;padding:10px 6px;border:1px solid #333;border-left:none;border-right:none;'>Options</th>
                                                    <th align='center' colspan='4' style='background-color:#ddd;text-transform:uppercase;padding:10px 6px;border:1px solid #333;border-left:none;border-right:none;'>Rate</th>
                                                    <th align='center' colspan='4' style='background-color:#ddd;text-transform:uppercase;padding:10px 6px;border:1px solid #333;border-left:none;border-right:none;'>Tariff Surcharge</th>
                                                    <th align='center' colspan='4' style='background-color:#ddd;text-transform:uppercase;padding:10px 6px;border:1px solid #333;border-left:none;border-right:none;'>Qty</th>
                                                    <th align='center' colspan='4' style='background-color:#ddd;text-transform:uppercase;padding:10px 6px;border:1px solid #333;border-left:none;border-right:none;'>Amount</th>
                                                </tr>";
                }
                else
                {
                    strSalesOrderLineHeader = @"<tr>
                                                    <th align='left' colspan='12' style='background-color:#ddd;text-transform:uppercase;padding:10px 6px;border:1px solid #333;border-left:none;border-right:none;'>Item</th>
                                                    <th colspan='3' align='center' style='background-color:#ddd;text-transform:uppercase;padding:10px 6px;border:1px solid #333;border-left:none;border-right:none;'>Options</th>
                                                    <th align='center' colspan='4' style='background-color:#ddd;text-transform:uppercase;padding:10px 6px;border:1px solid #333;border-left:none;border-right:none;'>Rate</th>
                                                    <th align='center' colspan='4' style='background-color:#ddd;text-transform:uppercase;padding:10px 6px;border:1px solid #333;border-left:none;border-right:none;'>Qty</th>
                                                    <th align='center' colspan='4' style='background-color:#ddd;text-transform:uppercase;padding:10px 6px;border:1px solid #333;border-left:none;border-right:none;'>Amount</th>
                                                </tr>";
                }

                foreach (ImageSolutions.SalesOrder.SalesOrderLine objSalesOrderLine in salesorder.SalesOrderLines)
                {
                    string strOptions = string.Empty;

                    strOptions += objSalesOrderLine.UserInfo == null ? string.Empty : "User: " + objSalesOrderLine.UserInfo.FullName + "<br />";
                    strOptions += objSalesOrderLine.CustomListValue_1 == null ? string.Empty : objSalesOrderLine.CustomListValue_1.CustomList.ListName + ": " + objSalesOrderLine.CustomListValue_1.ListValue + "<br />";
                    strOptions += objSalesOrderLine.CustomListValue_2 == null ? string.Empty : objSalesOrderLine.CustomListValue_2.CustomList.ListName + ": " + objSalesOrderLine.CustomListValue_2.ListValue + "<br />";

                    if (objSalesOrderLine.ItemPersonalizationValues != null && objSalesOrderLine.ItemPersonalizationValues.Count > 0)
                    {
                        foreach (ImageSolutions.Item.ItemPersonalizationValue _ItemPersonalizationValue in objSalesOrderLine.ItemPersonalizationValues)
                        {
                            strOptions += _ItemPersonalizationValue.ItemPersonalization.Name + ": " + _ItemPersonalizationValue.Value + "<br />";
                        }
                    }

                    if (objSalesOrderLine.SalesOrderLineSelectableLogos != null && objSalesOrderLine.SalesOrderLineSelectableLogos.Count > 0)
                    {
                        foreach (ImageSolutions.SalesOrder.SalesOrderLineSelectableLogo _SalesOrderLineSelectableLogo in objSalesOrderLine.SalesOrderLineSelectableLogos)
                        {
                            if (_SalesOrderLineSelectableLogo.SelectableLogo != null)
                            {
                                strOptions += _SalesOrderLineSelectableLogo.SelectableLogo.Name + "<br />";
                            }
                            else if (_SalesOrderLineSelectableLogo.HasNoLogo)
                            {
                                strOptions += "No Logo" + "<br />";
                            }
                        }
                    }

                    if (objSalesOrderLine.Item.IsNonInventory || !string.IsNullOrEmpty(objSalesOrderLine.CustomListValueID_1) || !string.IsNullOrEmpty(objSalesOrderLine.CustomListValueID_2))
                    {
                        strOptions += "<font style='color:red;'>(This is a custom item. You will receive it in 12 business days.)</font>";
                    }

                    if (CurrentWebsite.DisplayTariffCharge)
                    {
                        strSalesOrderLine += string.Format(@"<tr>
                                                        <td colspan='12' style='padding:6px 2px;border-bottom:1px solid #333;' valign='top'>
                                                            <span style='font-weight:bold; color:#333;'>{0}</span><br />
                                                            <span style='font-style:italic;'>Item #{1}</span>
                                                        </td>
                                                        <td colspan='3' align='center' style='padding:6px 2px;border-bottom:1px solid #333;' valign='top'>{2}</td>
                                                        <td align='center' colspan='4' style='padding:6px 2px;border-bottom:1px solid #333;' valign='top'>{3}</td>
                                                        <td align='center' colspan='4' style='padding:6px 2px;border-bottom:1px solid #333;' valign='top'>{4}</td>
                                                        <td align='center' colspan='4' style='padding:6px 2px;border-bottom:1px solid #333;' valign='top'>{5}</td>
                                                        <td align='center' colspan='4' style='padding:6px 2px;border-bottom:1px solid #333;' valign='top'>{6}</td>
                                                    </tr>", objSalesOrderLine.Item.StoreDisplayName, objSalesOrderLine.Item.ItemNumber, strOptions, string.Format("{0:c}", objSalesOrderLine.OnlinePrice), string.Format("{0:c}", objSalesOrderLine.TariffCharge), objSalesOrderLine.Quantity, string.Format("{0:c}", objSalesOrderLine.LineSubTotal));
                    }
                    else
                    {
                        strSalesOrderLine += string.Format(@"<tr>
                                                        <td colspan='12' style='padding:6px 2px;border-bottom:1px solid #333;' valign='top'>
                                                            <span style='font-weight:bold; color:#333;'>{0}</span><br />
                                                            <span style='font-style:italic;'>Item #{1}</span>
                                                        </td>
                                                        <td colspan='3' align='center' style='padding:6px 2px;border-bottom:1px solid #333;' valign='top'>{2}</td>
                                                        <td align='center' colspan='4' style='padding:6px 2px;border-bottom:1px solid #333;' valign='top'>{3}</td>
                                                        <td align='center' colspan='4' style='padding:6px 2px;border-bottom:1px solid #333;' valign='top'>{4}</td>
                                                        <td align='center' colspan='4' style='padding:6px 2px;border-bottom:1px solid #333;' valign='top'>{5}</td>
                                                    </tr>", objSalesOrderLine.Item.StoreDisplayName, objSalesOrderLine.Item.ItemNumber, strOptions, string.Format("{0:c}", objSalesOrderLine.UnitPrice), objSalesOrderLine.Quantity, string.Format("{0:c}", objSalesOrderLine.LineSubTotal));
                    }
                }

                strHTMLContent = strHTMLContent.Replace("${Logo}", string.IsNullOrEmpty(salesorder.Website.EmailLogoPath) ? salesorder.Website.LogoPath : salesorder.Website.EmailLogoPath);
                strHTMLContent = strHTMLContent.Replace("${FirstName}", userwebsite.UserInfo.FirstName); //salesorder.UserInfo.FirstName);
                strHTMLContent = strHTMLContent.Replace("${OrderNumber}", salesorder.SalesOrderID);
                strHTMLContent = strHTMLContent.Replace("${ShipTo}", salesorder.DisplayDeliveryAddress());
                strHTMLContent = strHTMLContent.Replace("${Subtotal}", string.Format("{0:c}", salesorder.LineTotal));
                strHTMLContent = strHTMLContent.Replace("${Shipping}", string.Format("{0:c}", salesorder.ShippingAmount));
                strHTMLContent = strHTMLContent.Replace("${Tax}", string.Format("{0:c}", salesorder.TaxAmount));
                strHTMLContent = strHTMLContent.Replace("${Total}", string.Format("{0:c}", salesorder.Total));
                strHTMLContent = strHTMLContent.Replace("${SalesOrderLineHeader}", strSalesOrderLineHeader);
                strHTMLContent = strHTMLContent.Replace("${SalesOrderLine}", string.Format("{0:c}", strSalesOrderLine));


                SendEmail(strToEmailAddress, CurrentWebsite.Name + " Personalization Approval Request #" + salesorder.SalesOrderID, strHTMLContent);
            }
            catch { }
            finally { }
            return true;
        }


        protected void SaveErrorLog(string errormessage)
        {
            try
            {
                ImageSolutions.ErrorLog.ErrorLog ErrorLog = new ImageSolutions.ErrorLog.ErrorLog();
                ErrorLog.UserInfoID = CurrentUser.UserInfoID;
                ErrorLog.UserWebsiteID = CurrentUser.CurrentUserWebSite.UserWebsiteID;
                ErrorLog.Path = "/checkout.aspx";
                ErrorLog.ErrorMessage = errormessage;
                ErrorLog.Create();
            }
            catch (Exception ex)
            {

            }
        }

        protected bool SendOrderConfirmation(SalesOrder SalesOrder, string ToEmailAddress)
        {
            string strHTMLContent = @"<!DOCTYPE html>
                                        <html>
                                            <head></head>
                                            <body>
                                                <div style='font-size:14px;padding-bottom:30px;height:100%;position:relative;'>
                                                    <div style='float:right;width:45%; font-weight:bold; font-size:20px; text-align:right; padding-top:20px;'>Thank you for your order!</div>

                                                    <div style='float:left; width:45%;'><img src='${Logo}' style='width:2.5in; height:0.76in ' /></div>

                                                    <div style='clear:left;padding-top:40px;'>
                                                        <div style='font-size:16px; font-weight:bold;'>Hi ${FirstName},</div>
                                                        <p>Your order #<span style='color:blue; font-size:14px;'>${OrderNumber}</span> has been processed${Approval}</p>
                                                        <p>Items stocked in our warehouse will ship same day if your order is placed by 2:30pm PST and next business day if after. Custom items will ship within 10-12 business days.</p>
                                                    </div>
                                                    <table style='font-size:0.9em;width:100%;border-collapse:collapse;margin:10px 0;'>
                                                        <thead>
                                                            ${SalesOrderLineHeader}
                                                        </thead>
                                                        <tbody>
                                                            ${SalesOrderLine}
                                                        </tbody>
                                                    </table>
                                                    <table style='width:100%;border-collapse:collapse;margin:10px 0;'>
                                                        <tbody>
                                                            <tr>
                                                                <td>
                                                                    <table style='width:100%;border-collapse:collapse;margin:10px 0;'>
                                                                        <tbody>
                                                                            <tr>
                                                                                <td>
                                                                                    <b>Ship To:</b>
                                                                                    <br />
                                                                                    ${ShipTo}
                                                                                    <br />
                                                                                </td>
                                                                            </tr>
                                                                        </tbody>
                                                                    </table>
                                                                </td>
                                                                <td align='right'>
                                                                    <table>
                                                                        <tr>
                                                                            <td>
                                                                                <table style='width:100%;border-collapse:collapse;margin:10px 0;${displaynone}'>
                                                                                    <tbody>
                                                                                        <tr>
                                                                                            <td align='right'><b>Subtotal</b></td>
                                                                                            <td align='right'>${Subtotal}</td>
                                                                                        </tr>
                                                                                        <tr>
                                                                                            <td align='right'><b>Shipping & Handling</b></td>
                                                                                            <td align='right'>${Shipping}</td>
                                                                                        </tr>
                                                                                        <tr>
                                                                                            <td align='right'><b>Tax</b></td>
                                                                                            <td align='right'>${Tax}</td>
                                                                                        </tr>
                                                                                        <tr>
                                                                                            <td align='right' style='font-weight:bold;'><b>TOTAL</b></td>
                                                                                            <td align='right' style='font-weight:bold;'>${Total}</td>
                                                                                        </tr>
                                                                                    </tbody>
                                                                                </table>
                                                                            </td>
                                                                        </tr>
                                                                    </table>
                                                                </td>
                                                            </tr>
                                                        </tbody>
                                                    </table>

                                                    <div style='margin-top:30px;text-align:center;'>
                                                        We appreciate your business! Providing you with a great experience is very important to us. <br />
                                                        - Image Solutions Team
                                                    </div>
                                                </div>
                                            </body>
                                        </html>";


            try
            {
                strHTMLContent = strHTMLContent.Replace("${displaynone}", CurrentWebsite.EnablePackagePayment ? "display: none;" : string.Empty);

                string strSalesOrderLineHeader = string.Empty;
                string strSalesOrderLine = string.Empty;

                if (CurrentWebsite.DisplayTariffCharge)
                {
                    strSalesOrderLineHeader = string.Format(@"<tr>
                                                    <th align='left' colspan='12' style='background-color:#ddd;text-transform:uppercase;padding:10px 6px;border:1px solid #333;border-left:none;border-right:none;'>Item</th>
                                                    <th colspan='3' align='center' style='background-color:#ddd;text-transform:uppercase;padding:10px 6px;border:1px solid #333;border-left:none;border-right:none;'>Options</th>
                                                    <th align='center' colspan='4' style='background-color:#ddd;text-transform:uppercase;padding:10px 6px;border:1px solid #333;border-left:none;border-right:none;{0}'>Rate</th>
                                                    <th align='center' colspan='4' style='background-color:#ddd;text-transform:uppercase;padding:10px 6px;border:1px solid #333;border-left:none;border-right:none;{0}'>Tariff Surcharge</th>
                                                    <th align='center' colspan='4' style='background-color:#ddd;text-transform:uppercase;padding:10px 6px;border:1px solid #333;border-left:none;border-right:none;'>Qty</th>
                                                    <th align='center' colspan='4' style='background-color:#ddd;text-transform:uppercase;padding:10px 6px;border:1px solid #333;border-left:none;border-right:none;{0}'>Amount</th>
                                                </tr>", CurrentWebsite.EnablePackagePayment ? "display: none;" : string.Empty);
                }
                else
                {
                    strSalesOrderLineHeader = string.Format(@"<tr>
                                                    <th align='left' colspan='12' style='background-color:#ddd;text-transform:uppercase;padding:10px 6px;border:1px solid #333;border-left:none;border-right:none;'>Item</th>
                                                    <th colspan='3' align='center' style='background-color:#ddd;text-transform:uppercase;padding:10px 6px;border:1px solid #333;border-left:none;border-right:none;'>Options</th>
                                                    <th align='center' colspan='4' style='background-color:#ddd;text-transform:uppercase;padding:10px 6px;border:1px solid #333;border-left:none;border-right:none;{0}'>Rate</th>
                                                    <th align='center' colspan='4' style='background-color:#ddd;text-transform:uppercase;padding:10px 6px;border:1px solid #333;border-left:none;border-right:none;'>Qty</th>
                                                    <th align='center' colspan='4' style='background-color:#ddd;text-transform:uppercase;padding:10px 6px;border:1px solid #333;border-left:none;border-right:none;{0}'>Amount</th>
                                                </tr>", CurrentWebsite.EnablePackagePayment ? "display: none;" : string.Empty);
                }

                foreach (ImageSolutions.SalesOrder.SalesOrderLine objSalesOrderLine in SalesOrder.SalesOrderLines)
                {
                    string strOptions = string.Empty;

                    strOptions += objSalesOrderLine.UserInfo == null ? string.Empty : "User: " + objSalesOrderLine.UserInfo.FullName + "<br />";
                    strOptions += objSalesOrderLine.CustomListValue_1 == null ? string.Empty : objSalesOrderLine.CustomListValue_1.CustomList.ListName + ": " + objSalesOrderLine.CustomListValue_1.ListValue + "<br />";
                    strOptions += objSalesOrderLine.CustomListValue_2 == null ? string.Empty : objSalesOrderLine.CustomListValue_2.CustomList.ListName + ": " + objSalesOrderLine.CustomListValue_2.ListValue + "<br />";

                    if (objSalesOrderLine.ItemPersonalizationValues != null && objSalesOrderLine.ItemPersonalizationValues.Count > 0)
                    {
                        foreach (ImageSolutions.Item.ItemPersonalizationValue _ItemPersonalizationValue in objSalesOrderLine.ItemPersonalizationValues)
                        {
                            strOptions += _ItemPersonalizationValue.ItemPersonalization.Name + ": " + _ItemPersonalizationValue.Value + "<br />";
                        }
                    }

                    if (objSalesOrderLine.SalesOrderLineSelectableLogos != null && objSalesOrderLine.SalesOrderLineSelectableLogos.Count > 0)
                    {
                        foreach (ImageSolutions.SalesOrder.SalesOrderLineSelectableLogo _SalesOrderLineSelectableLogo in objSalesOrderLine.SalesOrderLineSelectableLogos)
                        {
                            if (_SalesOrderLineSelectableLogo.SelectableLogo != null)
                            {
                                strOptions += _SalesOrderLineSelectableLogo.SelectableLogo.Name + "<br />";
                            }
                        }
                    }

                    if (objSalesOrderLine.Item.IsNonInventory || !string.IsNullOrEmpty(objSalesOrderLine.CustomListValueID_1) || !string.IsNullOrEmpty(objSalesOrderLine.CustomListValueID_2))
                    {
                        strOptions += "<font style='color:red;'>(This is a custom item. You will receive it in 12 business days.)</font>";
                    }

                    if (CurrentWebsite.DisplayTariffCharge)
                    {
                        strSalesOrderLine += string.Format(@"<tr>
                                                        <td colspan='12' style='padding:6px 2px;border-bottom:1px solid #333;' valign='top'>
                                                            <span style='font-weight:bold; color:#333;'>{0}</span><br />
                                                            <span style='font-style:italic;'>Item #{1}</span>
                                                        </td>
                                                        <td align='center' colspan='3' style='padding:6px 2px;border-bottom:1px solid #333;' valign='top'>{2}</td>>
                                                        <td align='center' colspan='4' style='padding:6px 2px;border-bottom:1px solid #333;{7}' valign='top'>{3}</td>
                                                        <td align='center' colspan='4' style='padding:6px 2px;border-bottom:1px solid #333;{7}' valign='top'>{4}</td>
                                                        <td align='center' colspan='4' style='padding:6px 2px;border-bottom:1px solid #333;' valign='top'>{5}</td>
                                                        <td align='center' colspan='4' style='padding:6px 2px;border-bottom:1px solid #333;{7}' valign='top'>{6}</td>
                                                    </tr>"
                                                    , objSalesOrderLine.Item.StoreDisplayName
                                                    , objSalesOrderLine.Item.ItemNumber
                                                    , strOptions
                                                    , string.Format("{0:c}", objSalesOrderLine.OnlinePrice == null ? Convert.ToDouble(objSalesOrderLine.UnitPrice) : Convert.ToDouble(objSalesOrderLine.OnlinePrice))
                                                    , string.Format("{0:c}", objSalesOrderLine.TariffCharge == null ? 0 : Convert.ToDouble(objSalesOrderLine.TariffCharge))
                                                    , objSalesOrderLine.Quantity
                                                    , string.Format("{0:c}", objSalesOrderLine.LineSubTotal)
                                                    , CurrentWebsite.EnablePackagePayment ? "display: none;" : string.Empty);
                    }
                    else
                    {
                        strSalesOrderLine += string.Format(@"<tr>
                                                        <td colspan='12' style='padding:6px 2px;border-bottom:1px solid #333;' valign='top'>
                                                            <span style='font-weight:bold; color:#333;'>{0}</span><br />
                                                            <span style='font-style:italic;'>Item #{1}</span>
                                                        </td>
                                                        <td colspan='3' align='center' style='padding:6px 2px;border-bottom:1px solid #333;' valign='top'>{2}</td>
                                                        <td align='center' colspan='4' style='padding:6px 2px;border-bottom:1px solid #333;{6}' valign='top'>{3}</td>
                                                        <td align='center' colspan='4' style='padding:6px 2px;border-bottom:1px solid #333;' valign='top'>{4}</td>
                                                        <td align='center' colspan='4' style='padding:6px 2px;border-bottom:1px solid #333;{6}' valign='top'>{5}</td>
                                                    </tr>"
                                                    , objSalesOrderLine.Item.StoreDisplayName
                                                    , objSalesOrderLine.Item.ItemNumber
                                                    , strOptions
                                                    , string.Format("{0:c}", objSalesOrderLine.UnitPrice)
                                                    , objSalesOrderLine.Quantity
                                                    , string.Format("{0:c}"
                                                    , objSalesOrderLine.LineSubTotal)
                                                    , CurrentWebsite.EnablePackagePayment ? "display: none;" : string.Empty);
                    }

                }

                strHTMLContent = strHTMLContent.Replace("${Logo}", string.IsNullOrEmpty(SalesOrder.Website.EmailLogoPath) ? SalesOrder.Website.LogoPath : SalesOrder.Website.EmailLogoPath);
                strHTMLContent = strHTMLContent.Replace("${FirstName}", SalesOrder.UserInfo.FirstName);
                strHTMLContent = strHTMLContent.Replace("${OrderNumber}", SalesOrder.SalesOrderID);

                if (CurrentUser.CurrentUserWebSite.WebSite.OrderApprovalRequired)
                {
                    strHTMLContent = strHTMLContent.Replace("${Approval}", " and is currently under review, you will receive a confirmation email once your order is approved.");
                }
                else
                {
                    strHTMLContent = strHTMLContent.Replace("${Approval}", ".");
                }

                strHTMLContent = strHTMLContent.Replace("${ShipTo}", SalesOrder.DisplayDeliveryAddress());
                strHTMLContent = strHTMLContent.Replace("${Subtotal}", string.Format("{0:c}", SalesOrder.LineTotal));
                strHTMLContent = strHTMLContent.Replace("${Shipping}", string.Format("{0:c}", SalesOrder.ShippingAmount));
                strHTMLContent = strHTMLContent.Replace("${Tax}", string.Format("{0:c}", SalesOrder.TaxAmount));
                strHTMLContent = strHTMLContent.Replace("${Total}", string.Format("{0:c}", SalesOrder.Total));
                strHTMLContent = strHTMLContent.Replace("${SalesOrderLineHeader}", strSalesOrderLineHeader);
                strHTMLContent = strHTMLContent.Replace("${SalesOrderLine}", string.Format("{0:c}", strSalesOrderLine));

                SendEmail(ToEmailAddress, CurrentWebsite.Name + " Order Confirmation #" + SalesOrder.SalesOrderID, strHTMLContent);
            }
            catch { }
            finally { }
            return true;
        }

        protected bool OrderApprovalNotification(SalesOrder salesorder, UserWebsite userwebsite) //string ToEmailAddress)
        {
            string strToEmailAddress = string.IsNullOrEmpty(userwebsite.NotificationEmail) ? userwebsite.UserInfo.EmailAddress : userwebsite.NotificationEmail;

            string strHTMLContent = @"<!DOCTYPE html>
                                        <html>
                                            <head></head>
                                            <body>
                                                <div style='font-size:14px;padding-bottom:30px;height:100%;position:relative;'>
                                                    <div style='float:right;width:45%; font-weight:bold; font-size:20px; text-align:right; padding-top:20px;'>Thank you for your order!</div>

                                                    <div style='float:left; width:45%;'><img src='${Logo}' style='width:2.5in; height:0.76in ' /></div>

                                                    <div style='clear:left;padding-top:40px;'>
                                                        <div style='font-size:16px; font-weight:bold;'>Hi ${FirstName},</div>
                                                        <p>A new order #<span style='color:blue; font-size:14px;'>${OrderNumber}</span> has been submitted for approval, please <a href='https://portal.imageinc.com/admin/Order.aspx?id=${OrderNumber}'>login</a> to the portal to approve this order.</p>
                                                    </div>
                                                    <table style='font-size:0.9em;width:100%;border-collapse:collapse;margin:10px 0;'>
                                                        <thead>
                                                            <tr>
                                                                <th align='left' colspan='12' style='background-color:#ddd;text-transform:uppercase;padding:10px 6px;border:1px solid #333;border-left:none;border-right:none;'>Item</th>
                                                                <th colspan='3' align='center' align='center' style='background-color:#ddd;text-transform:uppercase;padding:10px 6px;border:1px solid #333;border-left:none;border-right:none;'>Options</th>
                                                                <th align='center' colspan='4' style='background-color:#ddd;text-transform:uppercase;padding:10px 6px;border:1px solid #333;border-left:none;border-right:none;${displaynone}'>Rate</th>
                                                                <th align='center' colspan='4' style='background-color:#ddd;text-transform:uppercase;padding:10px 6px;border:1px solid #333;border-left:none;border-right:none;'>Qty</th>
                                                                <th align='center' colspan='4' style='background-color:#ddd;text-transform:uppercase;padding:10px 6px;border:1px solid #333;border-left:none;border-right:none;${displaynone}'>Amount</th>
                                                            </tr>
                                                        </thead>
                                                        <tbody>
                                                            ${SalesOrderLine}
                                                        </tbody>
                                                    </table>
                                                    <table style='width:100%;border-collapse:collapse;margin:10px 0;'>
                                                        <tbody>
                                                            <tr>
                                                                <td>
                                                                    <table style='width:100%;border-collapse:collapse;margin:10px 0;'>
                                                                        <tbody>
                                                                            <tr>
                                                                                <td>
                                                                                    <b>Ship To:</b>
                                                                                    <br />
                                                                                    ${ShipTo}
                                                                                    <br />
                                                                                </td>
                                                                            </tr>
                                                                        </tbody>
                                                                    </table>
                                                                </td>
                                                                <td align='right'>
                                                                    <table>
                                                                        <tr>
                                                                            <td>
                                                                                <table style='width:100%;border-collapse:collapse;margin:10px 0;${displaynone}'>
                                                                                    <tbody>
                                                                                        <tr>
                                                                                            <td align='right'><b>Subtotal</b></td>
                                                                                            <td align='right'>${Subtotal}</td>
                                                                                        </tr>
                                                                                        <tr>
                                                                                            <td align='right'><b>Shipping & Handling</b></td>
                                                                                            <td align='right'>${Shipping}</td>
                                                                                        </tr>
                                                                                        <tr>
                                                                                            <td align='right'><b>Tax</b></td>
                                                                                            <td align='right'>${Tax}</td>
                                                                                        </tr>
                                                                                        <tr>
                                                                                            <td align='right' style='font-weight:bold;'><b>TOTAL</b></td>
                                                                                            <td align='right' style='font-weight:bold;'>${Total}</td>
                                                                                        </tr>
                                                                                    </tbody>
                                                                                </table>
                                                                            </td>
                                                                        </tr>
                                                                    </table>
                                                                </td>
                                                            </tr>
                                                        </tbody>
                                                    </table>

                                                    <div style='margin-top:30px;text-align:center;'>
                                                        We appreciate your business! Providing you with a great experience is very important to us. <br />
                                                        - Image Solutions Team
                                                    </div>
                                                </div>
                                            </body>
                                        </html>";

            try
            {
                strHTMLContent = strHTMLContent.Replace("${displaynone}", CurrentWebsite.EnablePackagePayment ? "display: none;" : string.Empty);

                string strSalesOrderLine = string.Empty;

                foreach (ImageSolutions.SalesOrder.SalesOrderLine objSalesOrderLine in salesorder.SalesOrderLines)
                {
                    string strOptions = string.Empty;

                    strOptions += objSalesOrderLine.UserInfo == null ? string.Empty : "User: " + objSalesOrderLine.UserInfo.FullName + "<br />";
                    strOptions += objSalesOrderLine.CustomListValue_1 == null ? string.Empty : objSalesOrderLine.CustomListValue_1.CustomList.ListName + ": " + objSalesOrderLine.CustomListValue_1.ListValue + "<br />";
                    strOptions += objSalesOrderLine.CustomListValue_2 == null ? string.Empty : objSalesOrderLine.CustomListValue_2.CustomList.ListName + ": " + objSalesOrderLine.CustomListValue_2.ListValue + "<br />";
                    if (objSalesOrderLine.ItemPersonalizationValues != null && objSalesOrderLine.ItemPersonalizationValues.Count > 0)
                    {
                        foreach (ImageSolutions.Item.ItemPersonalizationValue _ItemPersonalizationValue in objSalesOrderLine.ItemPersonalizationValues)
                        {
                            strOptions += _ItemPersonalizationValue.ItemPersonalization.Name + ": " + _ItemPersonalizationValue.Value + "<br />";
                        }
                    }

                    if (objSalesOrderLine.SalesOrderLineSelectableLogos != null && objSalesOrderLine.SalesOrderLineSelectableLogos.Count > 0)
                    {
                        foreach (ImageSolutions.SalesOrder.SalesOrderLineSelectableLogo _SalesOrderLineSelectableLogo in objSalesOrderLine.SalesOrderLineSelectableLogos)
                        {
                            if (_SalesOrderLineSelectableLogo.SelectableLogo != null)
                            {
                                strOptions += _SalesOrderLineSelectableLogo.SelectableLogo.Name + "<br />";
                            }
                        }
                    }
                    if (objSalesOrderLine.Item.IsNonInventory || !string.IsNullOrEmpty(objSalesOrderLine.CustomListValueID_1) || !string.IsNullOrEmpty(objSalesOrderLine.CustomListValueID_2))
                    {
                        strOptions += "<font style='color:red;'>(This is a custom item. You will receive it in 12 business days.)</font>";
                    }

                    strSalesOrderLine += string.Format(@"<tr>
                                                        <td colspan='12' style='padding:6px 2px;border-bottom:1px solid #333;' valign='top'>
                                                            <span style='font-weight:bold; color:#333;'>{0}</span><br />
                                                            <span style='font-style:italic;'>Item #{1}</span>
                                                        </td>
                                                        <td colspan='3' align='center' style='padding:6px 2px;border-bottom:1px solid #333;' valign='top'>{2}</td>
                                                        <td align='center' colspan='4' style='padding:6px 2px;border-bottom:1px solid #333;{6}' valign='top'>{3}</td>
                                                        <td align='center' colspan='4' style='padding:6px 2px;border-bottom:1px solid #333;' valign='top'>{4}</td>
                                                        <td align='center' colspan='4' style='padding:6px 2px;border-bottom:1px solid #333;{6}' valign='top'>{5}</td>
                                                    </tr>"
                                                    , objSalesOrderLine.Item.StoreDisplayName
                                                    , objSalesOrderLine.Item.ItemNumber
                                                    , strOptions
                                                    , string.Format("{0:c}", objSalesOrderLine.UnitPrice)
                                                    , objSalesOrderLine.Quantity
                                                    , string.Format("{0:c}", objSalesOrderLine.LineSubTotal)
                                                    , CurrentWebsite.EnablePackagePayment ? "display: none;" : string.Empty);
                }

                strHTMLContent = strHTMLContent.Replace("${Logo}", string.IsNullOrEmpty(salesorder.Website.EmailLogoPath) ? salesorder.Website.LogoPath : salesorder.Website.EmailLogoPath);
                strHTMLContent = strHTMLContent.Replace("${FirstName}", userwebsite.UserInfo.FirstName);
                strHTMLContent = strHTMLContent.Replace("${OrderNumber}", salesorder.SalesOrderID);
                strHTMLContent = strHTMLContent.Replace("${ShipTo}", salesorder.DisplayDeliveryAddress());
                strHTMLContent = strHTMLContent.Replace("${Subtotal}", string.Format("{0:c}", salesorder.LineTotal));
                strHTMLContent = strHTMLContent.Replace("${Shipping}", string.Format("{0:c}", salesorder.ShippingAmount));
                strHTMLContent = strHTMLContent.Replace("${Tax}", string.Format("{0:c}", salesorder.TaxAmount));
                strHTMLContent = strHTMLContent.Replace("${Total}", string.Format("{0:c}", salesorder.Total));
                strHTMLContent = strHTMLContent.Replace("${SalesOrderLine}", string.Format("{0:c}", strSalesOrderLine));


                SendEmail(strToEmailAddress, CurrentWebsite.Name + " New Order Approval Request #" + salesorder.SalesOrderID, strHTMLContent);
            }
            catch { }
            finally { }
            return true;
        }

        protected void ddlShippingAddress_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(ddlShippingAddress.SelectedValue))
                SetShippingAddress();
        }

        protected void SetShippingAddress()
        {
            pnlExistingShippingAddress.Visible = false;
            pnlShippingAddress.Visible = true;
            pnlEditShippingAddress.Visible = false;
            pnlShippingAddressButton.Visible = true;
            btnEditShippingAddress.Visible = !string.IsNullOrEmpty(ddlShippingAddress.SelectedValue);

            btnEditShippingAddress.Visible = ddlBillingAddress.SelectedValue != ddlShippingAddress.SelectedValue;
            btnEditBillingAddress.Visible = ddlBillingAddress.SelectedValue != ddlShippingAddress.SelectedValue;

            if (!string.IsNullOrEmpty(ddlShippingAddress.SelectedValue))
            {
                ImageSolutions.Address.AddressBook AddressBook = new ImageSolutions.Address.AddressBook(ddlShippingAddress.SelectedValue);
                litShippingAddress.Text = AddressBook.GetDisplayFormat(true);

                btnEditShippingAddress.Enabled = string.IsNullOrEmpty(AddressBook.AccountID);
            }

            if (cbSameAsShippingAddress.Checked)
            {
                BindBillingAddress();

                ddlBillingAddress.SelectedValue = ddlShippingAddress.SelectedValue;
                ImageSolutions.Address.AddressBook BillingAddressBook = new ImageSolutions.Address.AddressBook(ddlBillingAddress.SelectedValue);
                litBillingAddress.Text = BillingAddressBook.GetDisplayFormat(true);

                btnEditBillingAddress.Enabled = string.IsNullOrEmpty(BillingAddressBook.AccountID);
            }

            UpdateShippingMethod();
            UpdateShoppingCart();

            if (chkApplyBudget.Checked && !string.IsNullOrEmpty(ddlMyBudgetAssignment.SelectedValue))
            {
                UpdateApplyBudget();
            }

            this.ddlUserCreditCard.DataSource = CurrentUser.UserCreditCards.FindAll(x => x.RemainingBalance == null || x.RemainingBalance > CurrentUser.CurrentUserWebSite.ShoppingCart.GetTotal(CurrentWebsite.WebsiteID, CurrentUser.CurrentUserWebSite.IsTaxExempt));
            this.ddlUserCreditCard.DataBind();

            if (!string.IsNullOrEmpty(ddlShippingAddress.SelectedValue)
                    && (ddlWebsiteShippingService.Items == null || ddlWebsiteShippingService.Items.Count == 0)
                )
            {
                WebUtility.DisplayJavascriptMessage(this, "Shipping service not found for the provided shipping address. Please verify the shipping address.");
            }

            if (!string.IsNullOrEmpty(CurrentUser.CurrentUserWebSite.AddressPermission))
            {
                if (CurrentUser.CurrentUserWebSite.AddressPermission == "Account")
                {
                    pnlShippingAddressButton.Visible = false;
                }
                else if (CurrentUser.CurrentUserWebSite.AddressPermission == "Default")
                {
                    pnlShippingAddressButton.Visible = false;
                }
            }

            ValidateMinimumRequirementCanada();
        }

        protected void UpdateShippingMethod()
        {
            liIPDCharge.Visible = CurrentWebsite.EnableIPD && IsInternational();

            if (!string.IsNullOrEmpty(ddlShippingAddress.SelectedValue))
            {
                ImageSolutions.Address.AddressBook ToAddressBook = new ImageSolutions.Address.AddressBook(ddlShippingAddress.SelectedValue);

                ImageSolutions.Address.AddressTrans ToAddressTrans = new ImageSolutions.Address.AddressTrans();
                ToAddressTrans.CompanyName = ToAddressBook.CompanyName;
                ToAddressTrans.FirstName = ToAddressBook.FirstName;
                ToAddressTrans.LastName = ToAddressBook.LastName;
                ToAddressTrans.AddressLine1 = ToAddressBook.AddressLine1;
                ToAddressTrans.AddressLine2 = ToAddressBook.AddressLine2;
                ToAddressTrans.City = ToAddressBook.City;
                ToAddressTrans.State = ToAddressBook.State;
                ToAddressTrans.PostalCode = ToAddressBook.PostalCode;
                ToAddressTrans.CountryCode = ToAddressBook.CountryCode;
                ToAddressTrans.PhoneNumber = ToAddressBook.PhoneNumber;
                ToAddressTrans.Email = ToAddressBook.Email;

                //List<ImageSolutions.Shipping.ShippoRate> ShippoRates = ImageSolutions.Shipping.ShippoShipping.GetRate(CurrentUser.CurrentUserWebSite.WebsiteID, CurrentUser.CurrentUserWebSite.ShoppingCart, ToAddressTrans, CurrentUser.CurrentUserWebSite.CurrentUserAccount.WebsiteGroupID);

                ImageSolutions.Shipping.WebShipRate.WebShipRateResponse result = null;

                if (CurrentUser.CurrentUserWebSite.ShoppingCart.ShoppingCartLines != null && CurrentUser.CurrentUserWebSite.ShoppingCart.ShoppingCartLines.Count > 0)
                {
                    result = ImageSolutions.Shipping.Restlet.getWebShipRate(CurrentUser.CurrentUserWebSite.WebSite, CurrentUser.CurrentUserWebSite.ShoppingCart, ToAddressTrans, CurrentUser.CurrentUserWebSite.CurrentUserAccount.WebsiteGroupID);
                }

                ddlWebsiteShippingService.Items.Clear();

                //if(ShippoRates != null && ShippoRates.Count > 0)
                //{
                //    foreach (ImageSolutions.Shipping.ShippoRate _ShippoRate in ShippoRates)
                //    {
                //        ImageSolutions.Shipping.ShippingService ShippingService = new ImageSolutions.Shipping.ShippingService();
                //        ImageSolutions.Shipping.ShippingServiceFilter ShippingServiceFilter = new ImageSolutions.Shipping.ShippingServiceFilter();
                //        ShippingServiceFilter.ServiceCode = new Database.Filter.StringSearch.SearchFilter();
                //        ShippingServiceFilter.ServiceCode.SearchString = _ShippoRate.Token;
                //        ShippingService = ImageSolutions.Shipping.ShippingService.GetShippingService(ShippingServiceFilter);

                //        ImageSolutions.Website.WebsiteShippingService WebsiteShippingService = new ImageSolutions.Website.WebsiteShippingService();
                //        ImageSolutions.Website.WebsiteShippingServiceFilter WebsiteShippingServiceFilter = new ImageSolutions.Website.WebsiteShippingServiceFilter();
                //        WebsiteShippingServiceFilter.ShippingServiceID = new Database.Filter.StringSearch.SearchFilter();
                //        WebsiteShippingServiceFilter.ShippingServiceID.SearchString = ShippingService.ShippingServiceID;
                //        WebsiteShippingServiceFilter.WebsiteID = new Database.Filter.StringSearch.SearchFilter();
                //        WebsiteShippingServiceFilter.WebsiteID.SearchString = CurrentUser.CurrentUserWebSite.WebsiteID;
                //        WebsiteShippingService = ImageSolutions.Website.WebsiteShippingService.GetWebsiteShippingService(WebsiteShippingServiceFilter);

                //        ddlWebsiteShippingService.Items.Add(new ListItem(string.Format("{0} - {1} - {2}", _ShippoRate.Carrier, _ShippoRate.ServiceName, _ShippoRate.Amount), WebsiteShippingService.WebsiteShippingServiceID)); //_ShippoRate.RateResponseID));                
                //    }
                //}


                if (result != null)
                {
                    if (CurrentWebsite.AllowPartialShipping)
                    //if (Convert.ToString(ConfigurationManager.AppSettings["Environment"]) == "staging" && CurrentWebsite.WebsiteID == "33")
                    {
                        if (!chkPartialShipping.Checked)
                        {
                            foreach (ImageSolutions.Shipping.WebShipRate.ShippingMethod _shippingmethod in result.shippingMethods)
                            {
                                //if (_shippingmethod.rate != null)
                                //{
                                ImageSolutions.Shipping.ShippingService ShippingService = new ImageSolutions.Shipping.ShippingService();
                                ImageSolutions.Shipping.ShippingServiceFilter ShippingServiceFilter = new ImageSolutions.Shipping.ShippingServiceFilter();
                                ShippingServiceFilter.ServiceCode = new Database.Filter.StringSearch.SearchFilter();
                                ShippingServiceFilter.ServiceCode.SearchString = _shippingmethod.shipmethod;
                                ShippingService = ImageSolutions.Shipping.ShippingService.GetShippingService(ShippingServiceFilter);

                                ImageSolutions.Website.WebsiteShippingService WebsiteShippingService = new ImageSolutions.Website.WebsiteShippingService();
                                ImageSolutions.Website.WebsiteShippingServiceFilter WebsiteShippingServiceFilter = new ImageSolutions.Website.WebsiteShippingServiceFilter();
                                WebsiteShippingServiceFilter.ShippingServiceID = new Database.Filter.StringSearch.SearchFilter();
                                WebsiteShippingServiceFilter.ShippingServiceID.SearchString = ShippingService.ShippingServiceID;
                                WebsiteShippingServiceFilter.WebsiteID = new Database.Filter.StringSearch.SearchFilter();
                                WebsiteShippingServiceFilter.WebsiteID.SearchString = CurrentUser.CurrentUserWebSite.WebsiteID;
                                WebsiteShippingService = ImageSolutions.Website.WebsiteShippingService.GetWebsiteShippingService(WebsiteShippingServiceFilter);

                                ddlWebsiteShippingService.Items.Add(new ListItem(string.Format("{0} - {1}", _shippingmethod.name, string.IsNullOrEmpty(_shippingmethod.rate_formatted) ? "$0" : _shippingmethod.rate_formatted)
                                    , WebsiteShippingService.WebsiteShippingServiceID)); //_ShippoRate.RateResponseID));  
                                                                                         //}
                            }
                        }
                        else
                        {
                            //Partial
                            List<ListItem> PartialListItems = new List<ListItem>();
                            List<ListItem> PartialShippingMethods = new List<ListItem>();

                            //Inventory 
                            int intInventoryQuantity = 0;
                            ImageSolutions.ShoppingCart.ShoppingCart InventoryShoppingCart = new ImageSolutions.ShoppingCart.ShoppingCart();
                            InventoryShoppingCart.ShoppingCartLines = new List<ImageSolutions.ShoppingCart.ShoppingCartLine>();
                            foreach (ImageSolutions.ShoppingCart.ShoppingCartLine _ShoppingCartLine in CurrentUser.CurrentUserWebSite.ShoppingCart.ShoppingCartLines.FindAll(x => x.Item.ItemType == "_inventoryItem"))
                            {
                                intInventoryQuantity += _ShoppingCartLine.Quantity;

                                ImageSolutions.ShoppingCart.ShoppingCartLine InventoryShoppingCartLine = new ImageSolutions.ShoppingCart.ShoppingCartLine();
                                InventoryShoppingCartLine.ItemID = _ShoppingCartLine.ItemID;
                                InventoryShoppingCartLine.Quantity = _ShoppingCartLine.Quantity;
                                InventoryShoppingCart.ShoppingCartLines.Add(InventoryShoppingCartLine);
                            }
                            if (intInventoryQuantity > 0)
                            {
                                ImageSolutions.Shipping.WebShipRate.WebShipRateResponse InventoryWebShipRateResponse = ImageSolutions.Shipping.Restlet.getWebShipRate(CurrentWebsite, InventoryShoppingCart, ToAddressTrans, CurrentUser.CurrentUserWebSite.CurrentUserAccount.WebsiteGroupID);

                                if (InventoryWebShipRateResponse.shippingMethods != null && InventoryWebShipRateResponse.shippingMethods.Count > 0)
                                {
                                    foreach (ImageSolutions.Shipping.WebShipRate.ShippingMethod _shippingmethod in InventoryWebShipRateResponse.shippingMethods)
                                    {
                                        ImageSolutions.Shipping.ShippingService ShippingService = new ImageSolutions.Shipping.ShippingService();
                                        ImageSolutions.Shipping.ShippingServiceFilter ShippingServiceFilter = new ImageSolutions.Shipping.ShippingServiceFilter();
                                        ShippingServiceFilter.ServiceCode = new Database.Filter.StringSearch.SearchFilter();
                                        ShippingServiceFilter.ServiceCode.SearchString = _shippingmethod.shipmethod;
                                        ShippingService = ImageSolutions.Shipping.ShippingService.GetShippingService(ShippingServiceFilter);

                                        ImageSolutions.Website.WebsiteShippingService WebsiteShippingService = new ImageSolutions.Website.WebsiteShippingService();
                                        ImageSolutions.Website.WebsiteShippingServiceFilter WebsiteShippingServiceFilter = new ImageSolutions.Website.WebsiteShippingServiceFilter();
                                        WebsiteShippingServiceFilter.ShippingServiceID = new Database.Filter.StringSearch.SearchFilter();
                                        WebsiteShippingServiceFilter.ShippingServiceID.SearchString = ShippingService.ShippingServiceID;
                                        WebsiteShippingServiceFilter.WebsiteID = new Database.Filter.StringSearch.SearchFilter();
                                        WebsiteShippingServiceFilter.WebsiteID.SearchString = CurrentUser.CurrentUserWebSite.WebsiteID;
                                        WebsiteShippingService = ImageSolutions.Website.WebsiteShippingService.GetWebsiteShippingService(WebsiteShippingServiceFilter);

                                        if (ddlWebsiteShippingService.Items.FindByValue(WebsiteShippingService.WebsiteShippingServiceID) != null)
                                        {
                                            ListItem ListItem = new ListItem(Convert.ToString(_shippingmethod.rate == null ? 0 : _shippingmethod.rate), WebsiteShippingService.WebsiteShippingServiceID);
                                            PartialListItems.Add(ListItem);
                                            PartialShippingMethods.Add(new ListItem(_shippingmethod.name, WebsiteShippingService.WebsiteShippingServiceID));
                                        }
                                        else
                                        {
                                            ListItem ListItem = new ListItem(Convert.ToString(_shippingmethod.rate == null ? 0 : _shippingmethod.rate), WebsiteShippingService.WebsiteShippingServiceID);
                                            PartialListItems.Add(ListItem);
                                            PartialShippingMethods.Add(new ListItem(_shippingmethod.name, WebsiteShippingService.WebsiteShippingServiceID));
                                        }
                                    }
                                }
                            }

                            //Non-Inventory 
                            int intNonInventoryQuantity = 0;
                            ImageSolutions.ShoppingCart.ShoppingCart NonInventoryShoppingCart = new ImageSolutions.ShoppingCart.ShoppingCart();
                            NonInventoryShoppingCart.ShoppingCartLines = new List<ImageSolutions.ShoppingCart.ShoppingCartLine>();
                            foreach (ImageSolutions.ShoppingCart.ShoppingCartLine _ShoppingCartLine in CurrentUser.CurrentUserWebSite.ShoppingCart.ShoppingCartLines.FindAll(x => x.Item.ItemType != "_inventoryItem"))
                            {
                                intNonInventoryQuantity += _ShoppingCartLine.Quantity;

                                ImageSolutions.ShoppingCart.ShoppingCartLine NonInventoryShoppingCartLine = new ImageSolutions.ShoppingCart.ShoppingCartLine();
                                NonInventoryShoppingCartLine.ItemID = _ShoppingCartLine.ItemID;
                                NonInventoryShoppingCartLine.Quantity = _ShoppingCartLine.Quantity;
                                NonInventoryShoppingCart.ShoppingCartLines.Add(NonInventoryShoppingCartLine);
                            }
                            if (intNonInventoryQuantity > 0)
                            {
                                ImageSolutions.Shipping.WebShipRate.WebShipRateResponse NonInventoryWebShipRateResponse = ImageSolutions.Shipping.Restlet.getWebShipRate(CurrentWebsite, NonInventoryShoppingCart, ToAddressTrans, CurrentUser.CurrentUserWebSite.CurrentUserAccount.WebsiteGroupID, true);

                                if (NonInventoryWebShipRateResponse.shippingMethods != null && NonInventoryWebShipRateResponse.shippingMethods.Count > 0)
                                {
                                    foreach (ImageSolutions.Shipping.WebShipRate.ShippingMethod _shippingmethod in NonInventoryWebShipRateResponse.shippingMethods)
                                    {
                                        ImageSolutions.Shipping.ShippingService ShippingService = new ImageSolutions.Shipping.ShippingService();
                                        ImageSolutions.Shipping.ShippingServiceFilter ShippingServiceFilter = new ImageSolutions.Shipping.ShippingServiceFilter();
                                        ShippingServiceFilter.ServiceCode = new Database.Filter.StringSearch.SearchFilter();
                                        ShippingServiceFilter.ServiceCode.SearchString = _shippingmethod.shipmethod;
                                        ShippingService = ImageSolutions.Shipping.ShippingService.GetShippingService(ShippingServiceFilter);

                                        ImageSolutions.Website.WebsiteShippingService WebsiteShippingService = new ImageSolutions.Website.WebsiteShippingService();
                                        ImageSolutions.Website.WebsiteShippingServiceFilter WebsiteShippingServiceFilter = new ImageSolutions.Website.WebsiteShippingServiceFilter();
                                        WebsiteShippingServiceFilter.ShippingServiceID = new Database.Filter.StringSearch.SearchFilter();
                                        WebsiteShippingServiceFilter.ShippingServiceID.SearchString = ShippingService.ShippingServiceID;
                                        WebsiteShippingServiceFilter.WebsiteID = new Database.Filter.StringSearch.SearchFilter();
                                        WebsiteShippingServiceFilter.WebsiteID.SearchString = CurrentUser.CurrentUserWebSite.WebsiteID;
                                        WebsiteShippingService = ImageSolutions.Website.WebsiteShippingService.GetWebsiteShippingService(WebsiteShippingServiceFilter);

                                        if (PartialListItems.Exists(x => x.Value == WebsiteShippingService.WebsiteShippingServiceID))
                                        {
                                            double dblUpdateRate = Convert.ToDouble(PartialListItems.Find(x => x.Value == WebsiteShippingService.WebsiteShippingServiceID).Text) + Convert.ToDouble(_shippingmethod.rate == null ? 0 : _shippingmethod.rate);

                                            PartialListItems.Find(x => x.Value == WebsiteShippingService.WebsiteShippingServiceID).Text = Convert.ToString(dblUpdateRate);
                                        }
                                        else
                                        {
                                            ListItem ListItem = new ListItem(Convert.ToString(_shippingmethod.rate == null ? 0 : _shippingmethod.rate), WebsiteShippingService.WebsiteShippingServiceID);
                                            PartialListItems.Add(ListItem);
                                            PartialShippingMethods.Add(new ListItem(_shippingmethod.name, WebsiteShippingService.WebsiteShippingServiceID));
                                        }
                                    }
                                }
                            }

                            //SP
                            //foreach (string _Group in CurrentUser.CurrentUserWebSite.ShoppingCart.ShoppingCartLines.FindAll(x => x.Item.ItemType != "_inventoryItem").Select(y => y.Item.VendorName).Distinct())
                            //{
                            //    ImageSolutions.ShoppingCart.ShoppingCart NonInventoryShoppingCart = new ImageSolutions.ShoppingCart.ShoppingCart();
                            //    NonInventoryShoppingCart.ShoppingCartLines = new List<ImageSolutions.ShoppingCart.ShoppingCartLine>();

                            //    foreach (ImageSolutions.ShoppingCart.ShoppingCartLine _ShoppingCartLine in CurrentUser.CurrentUserWebSite.ShoppingCart.ShoppingCartLines.FindAll(x => x.Item.ItemType != "_inventoryItem" && x.Item.VendorName == _Group))
                            //    {
                            //        ImageSolutions.ShoppingCart.ShoppingCartLine NonInventoryShoppingCartLine = new ImageSolutions.ShoppingCart.ShoppingCartLine();
                            //        NonInventoryShoppingCartLine.ItemID = _ShoppingCartLine.ItemID;
                            //        NonInventoryShoppingCartLine.Quantity = _ShoppingCartLine.Quantity;
                            //        NonInventoryShoppingCart.ShoppingCartLines.Add(NonInventoryShoppingCartLine);
                            //    }
                            //    ImageSolutions.Shipping.WebShipRate.WebShipRateResponse NonInventoryWebShipRateResponse = ImageSolutions.Shipping.Restlet.getWebShipRate(CurrentWebsite, NonInventoryShoppingCart, ToAddressTrans, CurrentUser.CurrentUserWebSite.CurrentUserAccount.WebsiteGroupID);
                            //    if (NonInventoryWebShipRateResponse.shippingMethods != null && NonInventoryWebShipRateResponse.shippingMethods.Count > 0)
                            //    {
                            //        foreach (ImageSolutions.Shipping.WebShipRate.ShippingMethod _shippingmethod in NonInventoryWebShipRateResponse.shippingMethods)
                            //        {
                            //            ImageSolutions.Shipping.ShippingService ShippingService = new ImageSolutions.Shipping.ShippingService();
                            //            ImageSolutions.Shipping.ShippingServiceFilter ShippingServiceFilter = new ImageSolutions.Shipping.ShippingServiceFilter();
                            //            ShippingServiceFilter.ServiceCode = new Database.Filter.StringSearch.SearchFilter();
                            //            ShippingServiceFilter.ServiceCode.SearchString = _shippingmethod.shipmethod;
                            //            ShippingService = ImageSolutions.Shipping.ShippingService.GetShippingService(ShippingServiceFilter);

                            //            ImageSolutions.Website.WebsiteShippingService WebsiteShippingService = new ImageSolutions.Website.WebsiteShippingService();
                            //            ImageSolutions.Website.WebsiteShippingServiceFilter WebsiteShippingServiceFilter = new ImageSolutions.Website.WebsiteShippingServiceFilter();
                            //            WebsiteShippingServiceFilter.ShippingServiceID = new Database.Filter.StringSearch.SearchFilter();
                            //            WebsiteShippingServiceFilter.ShippingServiceID.SearchString = ShippingService.ShippingServiceID;
                            //            WebsiteShippingServiceFilter.WebsiteID = new Database.Filter.StringSearch.SearchFilter();
                            //            WebsiteShippingServiceFilter.WebsiteID.SearchString = CurrentUser.CurrentUserWebSite.WebsiteID;
                            //            WebsiteShippingService = ImageSolutions.Website.WebsiteShippingService.GetWebsiteShippingService(WebsiteShippingServiceFilter);

                            //            if (PartialListItems.Exists(x => x.Value == WebsiteShippingService.WebsiteShippingServiceID))
                            //            {
                            //                double dblUpdateRate = Convert.ToDouble(PartialListItems.Find(x => x.Value == WebsiteShippingService.WebsiteShippingServiceID).Text) + Convert.ToDouble(_shippingmethod.rate == null ? 0 : _shippingmethod.rate);

                            //                PartialListItems.Find(x => x.Value == WebsiteShippingService.WebsiteShippingServiceID).Text = Convert.ToString(dblUpdateRate);
                            //            }
                            //            else
                            //            {
                            //                ListItem ListItem = new ListItem(Convert.ToString(_shippingmethod.rate == null ? 0 : _shippingmethod.rate), WebsiteShippingService.WebsiteShippingServiceID);
                            //                PartialListItems.Add(ListItem);
                            //                PartialShippingMethods.Add(new ListItem(_shippingmethod.name, WebsiteShippingService.WebsiteShippingServiceID));
                            //            }
                            //        }
                            //    }
                            //}

                            foreach (ListItem _ListItem in PartialListItems)
                            {
                                ddlWebsiteShippingService.Items.Add(new ListItem(string.Format("{0} - {1}", PartialShippingMethods.Find(x => x.Value == _ListItem.Value).Text, _ListItem.Text), _ListItem.Value));
                            }

                            //List<ListItem> CopyShippingService = new List<ListItem>();
                            //foreach (ListItem item in ddlWebsiteShippingService.Items)
                            //{
                            //    CopyShippingService.Add(item);
                            //}
                            //ddlWebsiteShippingService.Items.Clear();
                            //foreach (ListItem item in CopyShippingService.OrderBy(item => item.Text))
                            //{
                            //    ddlWebsiteShippingService.Items.Add(item);
                            //}
                        }
                    }
                    else
                    {
                        foreach (ImageSolutions.Shipping.WebShipRate.ShippingMethod _shippingmethod in result.shippingMethods)
                        {
                            //if (_shippingmethod.rate != null)
                            //{
                            ImageSolutions.Shipping.ShippingService ShippingService = new ImageSolutions.Shipping.ShippingService();
                            ImageSolutions.Shipping.ShippingServiceFilter ShippingServiceFilter = new ImageSolutions.Shipping.ShippingServiceFilter();
                            ShippingServiceFilter.ServiceCode = new Database.Filter.StringSearch.SearchFilter();
                            ShippingServiceFilter.ServiceCode.SearchString = _shippingmethod.shipmethod;
                            ShippingService = ImageSolutions.Shipping.ShippingService.GetShippingService(ShippingServiceFilter);

                            ImageSolutions.Website.WebsiteShippingService WebsiteShippingService = new ImageSolutions.Website.WebsiteShippingService();
                            ImageSolutions.Website.WebsiteShippingServiceFilter WebsiteShippingServiceFilter = new ImageSolutions.Website.WebsiteShippingServiceFilter();
                            WebsiteShippingServiceFilter.ShippingServiceID = new Database.Filter.StringSearch.SearchFilter();
                            WebsiteShippingServiceFilter.ShippingServiceID.SearchString = ShippingService.ShippingServiceID;
                            WebsiteShippingServiceFilter.WebsiteID = new Database.Filter.StringSearch.SearchFilter();
                            WebsiteShippingServiceFilter.WebsiteID.SearchString = CurrentUser.CurrentUserWebSite.WebsiteID;
                            WebsiteShippingService = ImageSolutions.Website.WebsiteShippingService.GetWebsiteShippingService(WebsiteShippingServiceFilter);

                            ddlWebsiteShippingService.Items.Add(new ListItem(string.Format("{0} - {1}", _shippingmethod.name, string.IsNullOrEmpty(_shippingmethod.rate_formatted) ? "$0" : _shippingmethod.rate_formatted)
                                , WebsiteShippingService.WebsiteShippingServiceID)); //_ShippoRate.RateResponseID));  
                                                                                     //}
                        }
                    }
                }

                hfShippingPOBoxMessage.Value = String.Empty;
            }
            else
            {
                ddlWebsiteShippingService.Items.Clear();
            }
        }

        protected void ddlBillingAddress_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(ddlBillingAddress.SelectedValue))
                SetBillingAddress();
        }
        protected void SetBillingAddress()
        {
            pnlExistingBillingAddress.Visible = false;
            pnlBillingAddress.Visible = true;
            pnlEditBillingAddress.Visible = false;
            pnlBillingAddressButton.Visible = true;
            btnEditBillingAddress.Visible = !string.IsNullOrEmpty(ddlBillingAddress.SelectedValue);

            btnEditShippingAddress.Visible = ddlBillingAddress.SelectedValue != ddlShippingAddress.SelectedValue;
            btnEditBillingAddress.Visible = ddlBillingAddress.SelectedValue != ddlShippingAddress.SelectedValue;

            if (!string.IsNullOrEmpty(ddlBillingAddress.SelectedValue))
            {
                ImageSolutions.Address.AddressBook AddressBook = new ImageSolutions.Address.AddressBook(ddlBillingAddress.SelectedValue);
                litBillingAddress.Text = AddressBook.GetDisplayFormat(true);

                btnEditBillingAddress.Enabled = string.IsNullOrEmpty(AddressBook.AccountID);
            }

            //UpdatePaystand();
        }

        protected void btnEditShippingAddress_Click(object sender, EventArgs e)
        {
            ImageSolutions.Address.AddressBook AddressBook = new ImageSolutions.Address.AddressBook(ddlShippingAddress.SelectedValue);
            txtFirstName.Value = AddressBook.FirstName;
            txtLastName.Value = AddressBook.LastName;
            txtAddressLabel.Value = AddressBook.AddressLabel;
            txtPhone.Value = AddressBook.PhoneNumber;
            //txtEmailAddress.Value = AddressBook.Email;
            txtAddress.Value = AddressBook.AddressLine1;
            txtAddress2.Value = AddressBook.AddressLine2;
            txtCity.Value = AddressBook.City;
            txtState.Value = AddressBook.State;
            txtPostalCode.Value = AddressBook.PostalCode;
            ddlCountry.SelectedIndex = !string.IsNullOrEmpty(AddressBook.CountryCode) ? this.ddlCountry.Items.IndexOf(this.ddlCountry.Items.FindByValue(AddressBook.CountryCode))
                        : this.ddlCountry.Items.IndexOf(this.ddlCountry.Items.FindByValue("US"));

            pnlExistingShippingAddress.Visible = false;
            pnlShippingAddress.Visible = false;
            pnlEditShippingAddress.Visible = true;
            pnlShippingAddressButton.Visible = false;

            pnlSkipShippingAddressValidation.Visible = false;
        }

        protected void btnEditBillingAddress_Click(object sender, EventArgs e)
        {
            ImageSolutions.Address.AddressBook AddressBook = new ImageSolutions.Address.AddressBook(ddlBillingAddress.SelectedValue);
            txtBillingFirstName.Value = AddressBook.FirstName;
            txtBillingLastName.Value = AddressBook.LastName;
            txtBillingAddressLabel.Value = AddressBook.AddressLabel;
            txtBillingPhone.Value = AddressBook.PhoneNumber;
            //txtBillingEmailAddress.Value = AddressBook.Email;
            txtBillingAddress.Value = AddressBook.AddressLine1;
            txtBillingAddress2.Value = AddressBook.AddressLine2;
            txtBillingCity.Value = AddressBook.City;
            txtBillingState.Value = AddressBook.State;
            txtBillingPostalCode.Value = AddressBook.PostalCode;
            ddlBillingCountry.SelectedIndex = !string.IsNullOrEmpty(AddressBook.CountryCode) ? this.ddlCountry.Items.IndexOf(this.ddlCountry.Items.FindByValue(AddressBook.CountryCode))
            : this.ddlCountry.Items.IndexOf(this.ddlCountry.Items.FindByValue("US"));

            pnlExistingBillingAddress.Visible = false;
            pnlBillingAddress.Visible = false;
            pnlEditBillingAddress.Visible = true;
            pnlBillingAddressButton.Visible = false;

            pnlSkipBillingAddressValidation.Visible = false;

            if (!string.IsNullOrEmpty(CurrentUser.CurrentUserWebSite.AddressPermission))
            {
                if (CurrentUser.CurrentUserWebSite.AddressPermission == "Account")
                {
                    pnlShippingAddressButton.Visible = false;
                }
                else if (CurrentUser.CurrentUserWebSite.AddressPermission == "Default")
                {
                    pnlShippingAddressButton.Visible = false;
                }
            }
        }

        protected void btnNewShippingAddress_Click(object sender, EventArgs e)
        {
            ddlShippingAddress.SelectedValue = string.Empty;

            txtFirstName.Value = String.Empty;
            txtLastName.Value = String.Empty;
            txtAddressLabel.Value = String.Empty;
            txtPhone.Value = String.Empty;
            //txtEmailAddress.Value = String.Empty; ;
            txtAddress.Value = String.Empty;
            txtAddress2.Value = String.Empty;
            txtCity.Value = String.Empty;
            txtState.Value = String.Empty;
            txtPostalCode.Value = String.Empty;

            pnlExistingShippingAddress.Visible = false;
            pnlShippingAddress.Visible = false;
            pnlEditShippingAddress.Visible = true;
            pnlShippingAddressButton.Visible = false;
        }

        protected void btnNewBillingAddress_Click(object sender, EventArgs e)
        {
            ddlBillingAddress.SelectedValue = string.Empty;

            txtBillingFirstName.Value = String.Empty;
            txtBillingLastName.Value = String.Empty;
            txtBillingAddressLabel.Value = String.Empty;
            txtBillingPhone.Value = String.Empty;
            //txtBillingEmailAddress.Value = String.Empty;
            txtBillingAddress.Value = String.Empty;
            txtBillingAddress2.Value = String.Empty;
            txtBillingCity.Value = String.Empty;
            txtBillingState.Value = String.Empty;
            txtBillingPostalCode.Value = String.Empty;

            pnlExistingBillingAddress.Visible = false;
            pnlBillingAddress.Visible = false;
            pnlEditBillingAddress.Visible = true;
            pnlBillingAddressButton.Visible = false;
        }

        protected void btnChooseExistingShippingAddress_Click(object sender, EventArgs e)
        {
            pnlExistingShippingAddress.Visible = true;
            pnlShippingAddress.Visible = false;
            pnlEditShippingAddress.Visible = false;
            pnlShippingAddressButton.Visible = false;
        }
        protected void btnChooseExistingBillingAddress_Click(object sender, EventArgs e)
        {
            pnlExistingBillingAddress.Visible = true;
            pnlBillingAddress.Visible = false;
            pnlEditBillingAddress.Visible = false;
            pnlBillingAddressButton.Visible = false;
        }
        protected void btnSaveShippingAddress_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(txtAddress.Value))
                {
                    throw new Exception("Address required");
                }
                if (string.IsNullOrEmpty(txtCity.Value))
                {
                    throw new Exception("City required");
                }
                if ((ddlCountry.SelectedValue == "US" || ddlCountry.SelectedValue == "CA") && string.IsNullOrEmpty(txtState.Value))
                {
                    throw new Exception("State required");
                }
                if (string.IsNullOrEmpty(txtPostalCode.Value))
                {
                    throw new Exception("Postal Code required");
                }
                if (string.IsNullOrEmpty(ddlCountry.SelectedValue))
                {
                    throw new Exception("Country required");
                }

                if (txtPhone.Value.Length < 7)
                {
                    throw new Exception("Invalid phone number");
                }

                ImageSolutions.Address.AddressTrans AddressTrans = new ImageSolutions.Address.AddressTrans();
                AddressTrans.FirstName = txtFirstName.Value;
                AddressTrans.LastName = txtLastName.Value;
                AddressTrans.AddressLabel = txtAddressLabel.Value;
                AddressTrans.AddressLine1 = txtAddress.Value;
                AddressTrans.AddressLine2 = txtAddress2.Value;
                AddressTrans.City = txtCity.Value;
                AddressTrans.State = txtState.Value;
                AddressTrans.PostalCode = txtPostalCode.Value;
                //AddressTrans.CountryName = "USA";
                AddressTrans.CountryCode = ddlCountry.SelectedValue;

                //if (cbSkipShippingAddressValidation.Checked || AddressTrans.CountryCode != "US" || ValidateAddress(AddressTrans))
                if (CurrentWebsite.DisableAddressValidation || cbSkipShippingAddressValidation.Checked || ValidateAddress(AddressTrans))
                {
                    ImageSolutions.Address.AddressBook AddressBook = null;
                    if (!string.IsNullOrEmpty(ddlShippingAddress.SelectedValue))
                    {
                        AddressBook = new ImageSolutions.Address.AddressBook(ddlShippingAddress.SelectedValue);
                    }
                    else
                    {
                        AddressBook = new ImageSolutions.Address.AddressBook();
                    }

                    AddressBook.FirstName = txtFirstName.Value;
                    AddressBook.LastName = txtLastName.Value;
                    AddressBook.AddressLabel = txtAddressLabel.Value; //txtFirstName.Value + "  " + txtLastName.Value;
                    AddressBook.AddressLine1 = txtAddress.Value;
                    AddressBook.AddressLine2 = txtAddress2.Value;
                    AddressBook.City = txtCity.Value;
                    AddressBook.State = txtState.Value;
                    AddressBook.PostalCode = txtPostalCode.Value;
                    AddressBook.PhoneNumber = txtPhone.Value;
                    AddressBook.CountryCode = ddlCountry.SelectedValue;
                    //AddressBook.Email = txtEmailAddress.Value;
                    AddressBook.UserInfoID = CurrentUser.UserInfoID;

                    if (AddressBook.IsNew)
                    {
                        AddressBook.CreatedBy = CurrentUser.UserInfoID;
                        AddressBook.Create();
                    }
                    else
                    {
                        AddressBook.Update();
                    }

                    BindShippingAddress();
                    ddlShippingAddress.SelectedValue = AddressBook.AddressBookID;
                    SetShippingAddress();

                    string strBillingAdress = Convert.ToString(ddlBillingAddress.SelectedValue);
                    BindBillingAddress();
                    if (!string.IsNullOrEmpty(strBillingAdress)) ddlBillingAddress.SelectedValue = strBillingAdress;
                    SetBillingAddress();
                }
            }
            catch (Exception ex)
            {
                if (!CurrentWebsite.DisableAddressValidation)
                {
                    pnlSkipBillingAddressValidation.Visible = true;
                    pnlSkipShippingAddressValidation.Visible = true;
                }

                WebUtility.DisplayJavascriptMessage(this, ex.Message);
            }
        }
        protected void btnSaveBillingAddress_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(txtBillingAddress.Value))
                {
                    throw new Exception("Address required");
                }
                if (string.IsNullOrEmpty(txtBillingCity.Value))
                {
                    throw new Exception("City required");
                }
                if ((ddlCountry.SelectedValue == "US" || ddlCountry.SelectedValue == "CA") && string.IsNullOrEmpty(txtBillingState.Value))
                {
                    throw new Exception("State required");
                }
                if (string.IsNullOrEmpty(txtBillingPostalCode.Value))
                {
                    throw new Exception("Postal Code required");
                }
                if (string.IsNullOrEmpty(ddlBillingCountry.SelectedValue))
                {
                    throw new Exception("Country required");
                }

                if (txtBillingPhone.Value.Length < 7)
                {
                    throw new Exception("Invalid phone number");
                }

                ImageSolutions.Address.AddressTrans AddressTrans = new ImageSolutions.Address.AddressTrans();
                AddressTrans.FirstName = txtBillingFirstName.Value;
                AddressTrans.LastName = txtBillingLastName.Value;
                AddressTrans.AddressLabel = txtBillingAddressLabel.Value;
                AddressTrans.AddressLine1 = txtBillingAddress.Value;
                AddressTrans.AddressLine2 = txtBillingAddress2.Value;
                AddressTrans.City = txtBillingCity.Value;
                AddressTrans.State = txtBillingState.Value;
                AddressTrans.PostalCode = txtBillingPostalCode.Value;
                //AddressTrans.CountryName = "USA";
                AddressTrans.CountryCode = ddlBillingCountry.SelectedValue;

                //if (cbSkipBillingAddressValidation.Checked || AddressTrans.CountryCode != "US" || ValidateBillingAddress(AddressTrans))
                if (CurrentWebsite.DisableAddressValidation || cbSkipBillingAddressValidation.Checked || ValidateBillingAddress(AddressTrans))
                {
                    ImageSolutions.Address.AddressBook AddressBook = null;
                    if (!string.IsNullOrEmpty(ddlBillingAddress.SelectedValue))
                    {
                        AddressBook = new ImageSolutions.Address.AddressBook(ddlBillingAddress.SelectedValue);
                    }
                    else
                    {
                        AddressBook = new ImageSolutions.Address.AddressBook();
                    }

                    //AddressBook.AddressLabel = txtBillingFirstName.Value + "  " + txtBillingLastName.Value;
                    AddressBook.FirstName = txtBillingFirstName.Value;
                    AddressBook.LastName = txtBillingLastName.Value;
                    AddressBook.AddressLabel = txtAddressLabel.Value;
                    AddressBook.AddressLine1 = txtBillingAddress.Value;
                    AddressBook.AddressLine2 = txtBillingAddress2.Value;
                    AddressBook.City = txtBillingCity.Value;
                    AddressBook.State = txtBillingState.Value;
                    AddressBook.PostalCode = txtBillingPostalCode.Value;
                    AddressBook.PhoneNumber = txtBillingPhone.Value;
                    AddressBook.CountryCode = ddlBillingCountry.SelectedValue;
                    //AddressBook.Email = txtBillingEmailAddress.Value;
                    AddressBook.UserInfoID = CurrentUser.UserInfoID;

                    if (AddressBook.IsNew)
                    {
                        AddressBook.CreatedBy = CurrentUser.UserInfoID;
                        AddressBook.Create();
                    }
                    else
                    {
                        AddressBook.Update();
                    }

                    BindBillingAddress();
                    ddlBillingAddress.SelectedValue = AddressBook.AddressBookID;
                    SetBillingAddress();

                    string strShippingAdress = Convert.ToString(ddlShippingAddress.SelectedValue);
                    BindShippingAddress();

                    if (!pnlShippingAddressMain.Visible)
                    {
                        ImageSolutions.Address.AddressBook CorporateAddressBook = new ImageSolutions.Address.AddressBook(CurrentWebsite.CorporateAddressBookID);
                        if (CorporateAddressBook != null && !string.IsNullOrEmpty(CorporateAddressBook.AddressBookID))
                        {
                            ddlShippingAddress.Items.Add(new ListItem(AddressBook.GetDisplayFormat(false), CorporateAddressBook.AddressBookID));
                            ddlShippingAddress.SelectedValue = CurrentWebsite.CorporateAddressBookID;
                        }
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(strShippingAdress)) ddlShippingAddress.SelectedValue = strShippingAdress;
                        SetShippingAddress();
                    }
                }
            }
            catch (Exception ex)
            {
                WebUtility.DisplayJavascriptMessage(this, ex.Message);
            }
        }
        protected bool ValidateAddress(ImageSolutions.Address.AddressTrans addresstrans)
        {
            bool _ret = false;

            Shippo.Address ShippoAddress = ImageSolutions.Shipping.ShippoShipping.GetAddress(addresstrans);

            if (Convert.ToBoolean(ShippoAddress.ValidationResults.IsValid))
            {
                if (Convert.ToString(txtAddress.Value) != Convert.ToString(ShippoAddress.Street1)
                    || Convert.ToString(txtAddress2.Value) != Convert.ToString(ShippoAddress.Street2)
                    || Convert.ToString(txtCity.Value) != Convert.ToString(ShippoAddress.City)
                    || Convert.ToString(txtState.Value) != Convert.ToString(ShippoAddress.State)
                    || Convert.ToString(txtPostalCode.Value) != Convert.ToString(ShippoAddress.Zip)
                )
                {
                    txtAddress.Value = Convert.ToString(ShippoAddress.Street1);
                    txtAddress2.Value = Convert.ToString(ShippoAddress.Street2);
                    txtCity.Value = Convert.ToString(ShippoAddress.City);
                    txtState.Value = Convert.ToString(ShippoAddress.State);
                    txtPostalCode.Value = Convert.ToString(ShippoAddress.Zip);

                    _ret = false;
                    this.Page.ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('Our address validator applied an adjustment to your address.  Please review and save again')", true);
                }
                else
                {
                    _ret = true;
                }
            }
            else
            {
                if (ShippoAddress.ValidationResults.Messages != null && ShippoAddress.ValidationResults.Messages.Count > 0)
                {
                    string strMessage = string.Empty;
                    foreach (Shippo.ValidationMessage _ValidationMessage in ShippoAddress.ValidationResults.Messages)
                    {
                        strMessage = string.Format(@"{0}\n\n{1}"
                            , strMessage
                            , Convert.ToString(_ValidationMessage.Text));
                    }
                    this.Page.ClientScript.RegisterStartupScript(this.GetType(), "alert", string.Format(@"alert('{0}')", strMessage), true);
                }
                else
                {
                    this.Page.ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('Invalid Address')", true);
                }
            }
            pnlSkipBillingAddressValidation.Visible = true;
            pnlSkipShippingAddressValidation.Visible = true;

            return _ret;
        }
        protected bool ValidateBillingAddress(ImageSolutions.Address.AddressTrans addresstrans)
        {
            bool _ret = false;

            Shippo.Address ShippoAddress = ImageSolutions.Shipping.ShippoShipping.GetAddress(addresstrans);

            if (Convert.ToBoolean(ShippoAddress.ValidationResults.IsValid))
            {
                if (txtBillingAddress.Value != Convert.ToString(ShippoAddress.Street1)
                    || txtBillingAddress2.Value != Convert.ToString(ShippoAddress.Street2)
                    || txtBillingCity.Value != Convert.ToString(ShippoAddress.City)
                    || txtBillingState.Value != Convert.ToString(ShippoAddress.State)
                    || txtBillingPostalCode.Value != Convert.ToString(ShippoAddress.Zip)
                )
                {
                    txtBillingAddress.Value = Convert.ToString(ShippoAddress.Street1);
                    txtBillingAddress2.Value = Convert.ToString(ShippoAddress.Street2);
                    txtBillingCity.Value = Convert.ToString(ShippoAddress.City);
                    txtBillingState.Value = Convert.ToString(ShippoAddress.State);
                    txtBillingPostalCode.Value = Convert.ToString(ShippoAddress.Zip);

                    _ret = false;
                    this.Page.ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('Our address validator applied an adjustment to your address.  Please review and save again')", true);
                }
                else
                {
                    _ret = true;
                }
            }
            else
            {
                if (ShippoAddress.ValidationResults.Messages != null && ShippoAddress.ValidationResults.Messages.Count > 0)
                {
                    string strMessage = string.Empty;
                    foreach (Shippo.ValidationMessage _ValidationMessage in ShippoAddress.ValidationResults.Messages)
                    {
                        strMessage = string.Format(@"{0}\n\n{1}"
                            , strMessage
                            , Convert.ToString(_ValidationMessage.Text));
                    }
                    this.Page.ClientScript.RegisterStartupScript(this.GetType(), "alert", string.Format(@"alert('{0}')", strMessage), true);
                }
                else
                {
                    this.Page.ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('Invalid Address')", true);
                }
            }
            pnlSkipBillingAddressValidation.Visible = true;
            pnlSkipShippingAddressValidation.Visible = true;

            return _ret;
        }
        protected void btnCancelShippingAddressSelect_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(ddlShippingAddress.SelectedValue))
            {
                SetShippingAddress();
            }
            else
            {
                ddlWebsiteShippingService.Items.Clear();

                pnlExistingShippingAddress.Visible = false;
                pnlShippingAddress.Visible = false;
                pnlEditShippingAddress.Visible = false;
                pnlShippingAddressButton.Visible = true;
                btnEditShippingAddress.Visible = !string.IsNullOrEmpty(ddlShippingAddress.SelectedValue);
            }
        }
        protected void btnCancelBillingAddressSelect_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(ddlBillingAddress.SelectedValue))
            {
                SetBillingAddress();
            }
            else
            {
                pnlExistingBillingAddress.Visible = false;
                pnlBillingAddress.Visible = false;
                pnlEditBillingAddress.Visible = false;
                pnlBillingAddressButton.Visible = true;
                btnEditBillingAddress.Visible = !string.IsNullOrEmpty(ddlBillingAddress.SelectedValue);
            }
        }
        protected void btnCancelShippingAddressSave_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(ddlShippingAddress.SelectedValue))
            {
                SetShippingAddress();
            }
            else
            {
                pnlExistingShippingAddress.Visible = false;
                pnlShippingAddress.Visible = false;
                pnlEditShippingAddress.Visible = false;
                pnlShippingAddressButton.Visible = true;
                btnEditShippingAddress.Visible = !string.IsNullOrEmpty(ddlShippingAddress.SelectedValue);
            }
        }
        protected void btnCancelBillingAddressSave_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(ddlBillingAddress.SelectedValue))
            {
                SetBillingAddress();
            }
            else
            {
                pnlExistingBillingAddress.Visible = false;
                pnlBillingAddress.Visible = false;
                pnlEditBillingAddress.Visible = false;
                pnlBillingAddressButton.Visible = true;
                btnEditBillingAddress.Visible = !string.IsNullOrEmpty(ddlBillingAddress.SelectedValue);
            }
        }

        protected void cbSameAsShippingAddress_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (cbSameAsShippingAddress.Checked)
                {
                    pnlExistingBillingAddress.Visible = false;
                    pnlBillingAddress.Visible = true;
                    pnlEditBillingAddress.Visible = false;
                    pnlBillingAddressButton.Visible = false;

                    if (!string.IsNullOrEmpty(ddlShippingAddress.SelectedValue))
                    {
                        SetShippingAddress();
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(ddlBillingAddress.SelectedValue))
                    {
                        SetBillingAddress();
                    }
                    else
                    {
                        pnlExistingBillingAddress.Visible = false;
                        pnlBillingAddress.Visible = false;
                        pnlEditBillingAddress.Visible = false;
                        pnlBillingAddressButton.Visible = true;
                        btnEditBillingAddress.Visible = !string.IsNullOrEmpty(ddlBillingAddress.SelectedValue);
                    }
                }

                if (!string.IsNullOrEmpty(CurrentUser.CurrentUserWebSite.AddressPermission))
                {
                    if (CurrentUser.CurrentUserWebSite.AddressPermission == "Account")
                    {
                        pnlShippingAddressButton.Visible = false;
                    }
                    else if (CurrentUser.CurrentUserWebSite.AddressPermission == "Default")
                    {
                        pnlShippingAddressButton.Visible = false;
                    }
                }
            }
            catch (Exception ex)
            {
                WebUtility.DisplayJavascriptMessage(this, ex.Message);
            }
        }

        protected void ddlWebsiteShippingService_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                bool blnUserCreditCardChecked = false;

                if (rbnUserCreditCard.Checked)
                {
                    blnUserCreditCardChecked = true;
                }

                UpdateShoppingCart();
                UpdateApplyBudget();

                rbnUserCreditCard.Checked = blnUserCreditCardChecked;

                DisplayMultipleShipmentMessage();

                DisplayCutoffMessage();
            }
            catch (Exception ex)
            {
                WebUtility.DisplayJavascriptMessage(this, ex.Message);
            }
        }

        protected void DisplayCutoffMessage()
        {
            if (
                (Convert.ToString(ConfigurationManager.AppSettings["Environment"]) == "staging" && CurrentWebsite.WebsiteID == "26")
                || (Convert.ToString(ConfigurationManager.AppSettings["Environment"]) == "production" && CurrentWebsite.WebsiteID == "2")
            )
            {
                if (!string.IsNullOrEmpty(ddlWebsiteShippingService.SelectedValue))
                {
                    ImageSolutions.Website.WebsiteShippingService WebsiteShippingService = new ImageSolutions.Website.WebsiteShippingService(Convert.ToString(ddlWebsiteShippingService.SelectedValue));

                    if (WebsiteShippingService.ShippingService.ServiceName.ToLower().Contains("overnight"))
                    {
                        TimeZoneInfo objEasternZone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
                        DateTime objEasternTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, objEasternZone);

                        if (objEasternTime.TimeOfDay > new TimeSpan(12, 0, 0))
                        {
                            WebUtility.DisplayJavascriptMessage(this, "It's past our 12:00 PM ET cutoff for Overnight Shipping. Your order will ship on the next business day.");
                        }
                    }
                }
            }
        }

        protected void ValidateMinimumRequirementCanada()
        {
            if (
                !string.IsNullOrEmpty(ddlShippingAddress.SelectedValue)
                && (
                    (Convert.ToString(ConfigurationManager.AppSettings["Environment"]) == "staging" && CurrentWebsite.WebsiteID == "53")
                    || (Convert.ToString(ConfigurationManager.AppSettings["Environment"]) == "production" && CurrentWebsite.WebsiteID == "20")
                )
            )
            {
                ImageSolutions.Address.AddressBook objShippingAddressBook = new ImageSolutions.Address.AddressBook(Convert.ToString(ddlShippingAddress.SelectedValue));

                if (CurrentUser.CurrentUserWebSite.ShoppingCart.LineTotal < 50 && objShippingAddressBook.CountryCode == "CA")
                {
                    //throw new Exception("Orders shipping to Canada must total a minimum of $50 USD (before taxes and shipping fees).");
                    //throw new Exception("Due to shipping costs, the minimum order amount is $50. Please review your cart and add additional items to proceed with your order.");
                    //WebUtility.DisplayJavascriptMessage(this, "Please note: Orders shipping to Canada must total a minimum of $50 USD (before taxes and shipping fees).");

                    lblMinimumRequirementMessage.Text = "Due to shipping costs, the minimum order amount is $50. Please review your cart and add additional items to proceed with your order.";
                    btnPlaceOrder.Visible = false;
                }
                else
                {
                    lblMinimumRequirementMessage.Text = String.Empty;
                    btnPlaceOrder.Visible = true;
                }
            }
        }

        protected void rbnUserCreditCard_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                ChangePaymentOption();
            }
            catch (Exception ex)
            {
                WebUtility.DisplayJavascriptMessage(this, ex.Message);
            }
        }

        protected void ChangePaymentOption()
        {
            //this.phBudget.Visible = this.rbnBudget.Checked;
            this.phUserCreditCard.Visible = this.rbnUserCreditCard.Checked;
            this.phNewCreditCard.Visible = this.rbnNewCreditCard.Checked;
            this.phInvoicePaymentMessage.Visible = this.rbnInvoicePayment.Checked;
            this.phNewCreditCardMessage.Visible = this.rbnNewCreditCard.Checked;

            //btnPlaceOrderPaystand.Visible = rbnNewCreditCard.Checked;
            //btnPlaceOrder.Visible = !btnPlaceOrderPaystand.Visible;

            pnlBillingAddressWrap.Visible = !rbnUserCreditCard.Checked;

            if (pnlBillingAddressWrap.Visible)
            {
                if (chkApplyBudget.Checked && !rbnNewCreditCard.Checked)
                {
                    pnlBillingAddressWrap.Visible = false;
                }
            }
        }

        protected bool UpdateShoppingCart()
        {
            if (!string.IsNullOrEmpty(this.ddlWebsiteShippingService.SelectedValue))
            {
                CurrentUser.CurrentUserWebSite.ShoppingCart.WebsiteShippingServiceID = this.ddlWebsiteShippingService.SelectedValue;
            }

            if (!string.IsNullOrEmpty(ddlShippingAddress.SelectedValue))
            {
                ImageSolutions.Address.AddressBook objShippingAddressBook = new ImageSolutions.Address.AddressBook(Convert.ToString(ddlShippingAddress.SelectedValue));
                AddressTrans objShippingAddressTrans = new ImageSolutions.Address.AddressTrans();
                objShippingAddressTrans.AddressLine1 = objShippingAddressBook.AddressLine1;
                objShippingAddressTrans.AddressLine2 = objShippingAddressBook.AddressLine2;
                objShippingAddressTrans.FirstName = objShippingAddressBook.FirstName;
                objShippingAddressTrans.LastName = objShippingAddressBook.LastName;
                objShippingAddressTrans.City = objShippingAddressBook.City;
                objShippingAddressTrans.State = objShippingAddressBook.State;
                objShippingAddressTrans.PostalCode = objShippingAddressBook.PostalCode;
                objShippingAddressTrans.CountryCode = objShippingAddressBook.CountryCode;
                objShippingAddressTrans.PhoneNumber = objShippingAddressBook.PhoneNumber;
                objShippingAddressTrans.Email = objShippingAddressBook.Email;
                CurrentUser.CurrentUserWebSite.ShoppingCart.ShippingAddress = objShippingAddressTrans;
            }

            //if (!pnlShippingAddressMain.Visible 
            //    && (Convert.ToString(ConfigurationManager.AppSettings["Environment"]) == "staging" && CurrentWebsite.WebsiteID == "54")
            //    ||
            //    (Convert.ToString(ConfigurationManager.AppSettings["Environment"]) == "production" && CurrentWebsite.WebsiteID == "21")
            //)
            //{
            //    AddressTrans objShippingAddressTrans = new ImageSolutions.Address.AddressTrans();
            //    objShippingAddressTrans.AddressLabel = "Discount Tire HQ";
            //    objShippingAddressTrans.FirstName = "Discount Tire";
            //    objShippingAddressTrans.LastName = "HQ";
            //    objShippingAddressTrans.AddressLine1 = "20225 N Scottsdale Rd";
            //    objShippingAddressTrans.City = "Scottsdale";
            //    objShippingAddressTrans.State = "AZ";
            //    objShippingAddressTrans.PostalCode = "85255";
            //    objShippingAddressTrans.CountryCode = "US";
            //    objShippingAddressTrans.PhoneNumber = "480.606.6000";
            //    CurrentUser.CurrentUserWebSite.ShoppingCart.ShippingAddress = objShippingAddressTrans;
            //}

            double dblPartialShipping = 0;

            if (!string.IsNullOrEmpty(this.ddlWebsiteShippingService.SelectedValue))
            {
                this.litShippingAmount.Text = string.Format("{0:c}", CurrentUser.CurrentUserWebSite.ShoppingCart.ShippingAmount);

                if (chkPartialShipping.Checked)
                {
                    string[] splitString = ddlWebsiteShippingService.SelectedItem.Text.Split('-');
                    string strShippingAmount = splitString[splitString.Count() - 1].Trim();
                    dblPartialShipping = double.Parse(strShippingAmount, NumberStyles.AllowCurrencySymbol | NumberStyles.Currency);//Convert.ToDouble(strShippingAmount);
                    litShippingAmount.Text = string.Format("{0:c}", dblPartialShipping);
                }

                if (CurrentWebsite.AllowPartialShipping && pnlShippingBreakdown.Visible)
                //if (Convert.ToString(ConfigurationManager.AppSettings["Environment"]) == "staging" && CurrentWebsite.WebsiteID == "33")
                {
                    pnlParitalShippingOption.Visible = true;
                    pnlShippingBreakdown.Visible = true;

                    List<ShippingBreakdown> ShippingBreakdowns = new List<ShippingBreakdown>();

                    //Inventory 
                    int intInventoryQuantity = 0;
                    ImageSolutions.ShoppingCart.ShoppingCart InventoryShoppingCart = new ImageSolutions.ShoppingCart.ShoppingCart();
                    InventoryShoppingCart.ShoppingCartLines = new List<ImageSolutions.ShoppingCart.ShoppingCartLine>();
                    foreach (ImageSolutions.ShoppingCart.ShoppingCartLine _ShoppingCartLine in CurrentUser.CurrentUserWebSite.ShoppingCart.ShoppingCartLines.FindAll(x => x.Item.ItemType == "_inventoryItem"))
                    {
                        intInventoryQuantity += _ShoppingCartLine.Quantity;

                        ImageSolutions.ShoppingCart.ShoppingCartLine InventoryShoppingCartLine = new ImageSolutions.ShoppingCart.ShoppingCartLine();
                        InventoryShoppingCartLine.ItemID = _ShoppingCartLine.ItemID;
                        InventoryShoppingCartLine.Quantity = _ShoppingCartLine.Quantity;
                        InventoryShoppingCart.ShoppingCartLines.Add(InventoryShoppingCartLine);
                    }
                    if (intInventoryQuantity > 0)
                    {
                        decimal decInventoryShippingAmount = 0;
                        ImageSolutions.Shipping.WebShipRate.WebShipRateResponse InventoryWebShipRateResponse = ImageSolutions.Shipping.Restlet.getWebShipRate(CurrentWebsite, InventoryShoppingCart, CurrentUser.CurrentUserWebSite.ShoppingCart.ShippingAddress, String.Empty);
                        if (InventoryWebShipRateResponse.shippingMethods != null && InventoryWebShipRateResponse.shippingMethods.Count > 0)
                        {
                            decInventoryShippingAmount = Convert.ToDecimal(InventoryWebShipRateResponse.shippingMethods.Find(x => x.shipmethod == CurrentUser.CurrentUserWebSite.ShoppingCart.WebsiteShippingService.ShippingService.ServiceCode).rate);
                        }
                        ShippingBreakdown InventoryShippingBreakdown = new ShippingBreakdown();
                        InventoryShippingBreakdown.Label = string.Format("Inventory ({0} pieces)", intInventoryQuantity);
                        InventoryShippingBreakdown.Amount = decInventoryShippingAmount;
                        ShippingBreakdowns.Add(InventoryShippingBreakdown);
                    }

                    //Non-Inventory 
                    int intNonInventoryQuantity = 0;
                    ImageSolutions.ShoppingCart.ShoppingCart NonInventoryShoppingCart = new ImageSolutions.ShoppingCart.ShoppingCart();
                    NonInventoryShoppingCart.ShoppingCartLines = new List<ImageSolutions.ShoppingCart.ShoppingCartLine>();
                    foreach (ImageSolutions.ShoppingCart.ShoppingCartLine _ShoppingCartLine in CurrentUser.CurrentUserWebSite.ShoppingCart.ShoppingCartLines.FindAll(x => x.Item.ItemType != "_inventoryItem"))
                    {
                        intNonInventoryQuantity += _ShoppingCartLine.Quantity;

                        ImageSolutions.ShoppingCart.ShoppingCartLine NonInventoryShoppingCartLine = new ImageSolutions.ShoppingCart.ShoppingCartLine();
                        NonInventoryShoppingCartLine.ItemID = _ShoppingCartLine.ItemID;
                        NonInventoryShoppingCartLine.Quantity = _ShoppingCartLine.Quantity;
                        NonInventoryShoppingCart.ShoppingCartLines.Add(NonInventoryShoppingCartLine);
                    }
                    if (intNonInventoryQuantity > 0)
                    {
                        decimal decNonInventoryShippingAmount = 0;
                        ImageSolutions.Shipping.WebShipRate.WebShipRateResponse NonInventoryWebShipRateResponse = ImageSolutions.Shipping.Restlet.getWebShipRate(CurrentWebsite, NonInventoryShoppingCart, CurrentUser.CurrentUserWebSite.ShoppingCart.ShippingAddress, String.Empty, true);
                        if (NonInventoryWebShipRateResponse.shippingMethods != null && NonInventoryWebShipRateResponse.shippingMethods.Count > 0)
                        {
                            decNonInventoryShippingAmount = Convert.ToDecimal(NonInventoryWebShipRateResponse.shippingMethods.Find(x => x.shipmethod == CurrentUser.CurrentUserWebSite.ShoppingCart.WebsiteShippingService.ShippingService.ServiceCode).rate);
                        }
                        ShippingBreakdown NonInventoryShippingBreakdown = new ShippingBreakdown();
                        NonInventoryShippingBreakdown.Label = string.Format("Non-Inventory ({0} pieces)", intInventoryQuantity);
                        NonInventoryShippingBreakdown.Amount = decNonInventoryShippingAmount;
                        ShippingBreakdowns.Add(NonInventoryShippingBreakdown);
                    }

                    //foreach (string _Group in CurrentUser.CurrentUserWebSite.ShoppingCart.ShoppingCartLines.FindAll(x => x.Item.ItemType != "_inventoryItem").Select(y => y.Item.VendorName).Distinct())
                    //{
                    //    ImageSolutions.ShoppingCart.ShoppingCart NonInventoryShoppingCart = new ImageSolutions.ShoppingCart.ShoppingCart();
                    //    NonInventoryShoppingCart.ShoppingCartLines = new List<ImageSolutions.ShoppingCart.ShoppingCartLine>();

                    //    int intNonInventoryQuantity = 0;

                    //    foreach (ImageSolutions.ShoppingCart.ShoppingCartLine _ShoppingCartLine in CurrentUser.CurrentUserWebSite.ShoppingCart.ShoppingCartLines.FindAll(x => x.Item.ItemType != "_inventoryItem" && x.Item.VendorName == _Group) )
                    //    {
                    //        intNonInventoryQuantity += _ShoppingCartLine.Quantity;

                    //        ImageSolutions.ShoppingCart.ShoppingCartLine NonInventoryShoppingCartLine = new ImageSolutions.ShoppingCart.ShoppingCartLine();
                    //        NonInventoryShoppingCartLine.ItemID = _ShoppingCartLine.ItemID;
                    //        NonInventoryShoppingCartLine.Quantity = _ShoppingCartLine.Quantity;
                    //        NonInventoryShoppingCart.ShoppingCartLines.Add(NonInventoryShoppingCartLine);
                    //    }
                    //    decimal decNonInventoryShippingAmount = 0;
                    //    ImageSolutions.Shipping.WebShipRate.WebShipRateResponse NonInventoryWebShipRateResponse = ImageSolutions.Shipping.Restlet.getWebShipRate(CurrentWebsite, NonInventoryShoppingCart, CurrentUser.CurrentUserWebSite.ShoppingCart.ShippingAddress, String.Empty);
                    //    if (NonInventoryWebShipRateResponse.shippingMethods != null && NonInventoryWebShipRateResponse.shippingMethods.Count > 0)
                    //    {
                    //        decNonInventoryShippingAmount = Convert.ToDecimal(NonInventoryWebShipRateResponse.shippingMethods.Find(x => x.shipmethod == CurrentUser.CurrentUserWebSite.ShoppingCart.WebsiteShippingService.ShippingService.ServiceCode).rate);
                    //    }
                    //    ShippingBreakdown NonInventoryShippingBreakdown = new ShippingBreakdown();
                    //    NonInventoryShippingBreakdown.Label = string.Format("NonInventory - {0} ({1} pieces)", _Group, intNonInventoryQuantity);
                    //    NonInventoryShippingBreakdown.Amount = decNonInventoryShippingAmount;
                    //    ShippingBreakdowns.Add(NonInventoryShippingBreakdown);
                    //}

                    rptShippingBreakdown.DataSource = ShippingBreakdowns;
                    rptShippingBreakdown.DataBind();
                }
            }
            else
            {
                this.litShippingAmount.Text = string.Empty;

                pnlShippingBreakdown.Visible = false;
            }

            this.litTaxAmount.Text = string.Format("{0:c}", CurrentUser.CurrentUserWebSite.IsTaxExempt ? 0 : CurrentUser.CurrentUserWebSite.ShoppingCart.SalesTaxAmount);
            litIPDAmount.Text = string.Format("{0:c}", CurrentWebsite.EnableIPD && IsInternational() ? CurrentUser.CurrentUserWebSite.ShoppingCart.LineTotal * (Convert.ToDouble(CurrentWebsite.IPDTaxAdjustPercent) / 100.00) : 0);
            if (CurrentWebsite.EnableIPD && IsInternational())
            {
                litTaxAmount.Text = string.Format("{0:c}", CurrentUser.CurrentUserWebSite.IsTaxExempt ? 0 : CurrentUser.CurrentUserWebSite.ShoppingCart.SalesTaxAmount + (CurrentUser.CurrentUserWebSite.ShoppingCart.LineTotal * Convert.ToDouble(CurrentWebsite.IPDTaxAdjustPercent) / 100.00));
            }

            if (CurrentUser.CurrentUserWebSite.ShoppingCart.GetCompanyInvoicedAmount(CurrentWebsite.WebsiteID, CurrentUser.CurrentUserWebSite.IsTaxExempt) != 0)
            {
                pnlCompanyInvoicedAmount.Visible = true;
                this.litCompanyInvoiced.Text = Math.Abs(CurrentUser.CurrentUserWebSite.ShoppingCart.GetCompanyInvoicedAmount(CurrentWebsite.WebsiteID, CurrentUser.CurrentUserWebSite.IsTaxExempt)).ToString("C");
                //this.litCompanyInvoiced.Text = string.Format("{0:0.00}", Math.Abs(CurrentUser.CurrentUserWebSite.ShoppingCart.GetCompanyInvoicedAmount(CurrentWebsite.WebsiteID, CurrentUser.CurrentUserWebSite.IsTaxExempt)));
            }
            else
            {
                pnlCompanyInvoicedAmount.Visible = false;
            }

            double dblTotal = 0; //CurrentUser.CurrentUserWebSite.ShoppingCart.GetTotal(CurrentWebsite.WebsiteID, CurrentUser.CurrentUserWebSite.IsTaxExempt);
            double dblPromotionAmount = CurrentUser.CurrentUserWebSite.ShoppingCart.PromotionAmount;

            if (chkPartialShipping.Checked)
            {
                dblTotal = CurrentUser.CurrentUserWebSite.ShoppingCart.GetTotalWithPartialShipping(CurrentWebsite.WebsiteID, CurrentUser.CurrentUserWebSite.IsTaxExempt);
            }
            else
            {
                dblTotal = CurrentUser.CurrentUserWebSite.ShoppingCart.GetTotal(CurrentWebsite.WebsiteID, CurrentUser.CurrentUserWebSite.IsTaxExempt);
            }

            //SP
            if (CurrentWebsite.EnableIPD && IsInternational())
            {
                dblTotal = dblTotal + double.Parse(litIPDAmount.Text, NumberStyles.AllowCurrencySymbol | NumberStyles.Currency);
                dblTotal = Math.Round(dblTotal, 2);
            }

            if (dblTotal < 0)
            {
                this.litTotal.Text = string.Format("{0:c}", 0);

                if (!string.IsNullOrEmpty(CurrentWebsite.CurrencyConvert)
                    && CurrentWebsite.CurrentyConvertPercentage != null
                    && CurrentWebsite.CurrentyConvertPercentage > 0)
                {
                    this.litTotalCurrencyConvert.Text = string.Format(@" USD <span style=""font-size: small; "" title=""est. {0:c} {1}""><i class=""ti-info-alt"" ></i> </span>"
                        , 0
                        , CurrentWebsite.CurrencyConvert
                        );
                }

                this.litPromo.Text = string.Format("{0:c}", dblPromotionAmount - dblTotal);
            }
            else
            {
                this.litTotal.Text = string.Format("{0:c}", dblTotal);

                if (!string.IsNullOrEmpty(CurrentWebsite.CurrencyConvert)
                    && CurrentWebsite.CurrentyConvertPercentage != null
                    && CurrentWebsite.CurrentyConvertPercentage > 0)
                {
                    double dblTotalCurrencyConvert = double.Parse(litTotal.Text, NumberStyles.AllowCurrencySymbol | NumberStyles.Currency);
                    this.litTotalCurrencyConvert.Text = string.Format(@" USD <span style=""font-size: small; "" title=""{0:c} {1}""><i class=""ti-info-alt"" ></i> </span>"
                        , Convert.ToDecimal(dblTotalCurrencyConvert) * CurrentWebsite.CurrentyConvertPercentage
                        , CurrentWebsite.CurrencyConvert
                        );
                }

                this.litPromo.Text = string.Format("{0:c}", dblPromotionAmount);
            }

            this.gvPromotions.DataSource = CurrentUser.CurrentUserWebSite.ShoppingCart.ShoppingCartPromotions;
            this.gvPromotions.DataBind();
            pnlPromotionForm.Visible = CurrentUser.CurrentUserWebSite.ShoppingCart.ShoppingCartPromotions == null || CurrentUser.CurrentUserWebSite.ShoppingCart.ShoppingCartPromotions.Count < 1;

            //UpdatePaystand();            
            UpdateApplyBudget();

            return true;
        }

        public class ShippingBreakdown
        {
            public string Label { get; set; }
            public decimal Amount { get; set; }

        }


//        protected void UpdatePaystand()
//        {
//            if (!string.IsNullOrEmpty(ddlBillingAddress.SelectedValue))
//            {
//                ImageSolutions.Address.AddressBook objBillingAddressBook = new ImageSolutions.Address.AddressBook(Convert.ToString(ddlBillingAddress.SelectedValue));

//                litPaystandScript.Text = string.Format(@"
//<script
//    type='text/javascript'
//    id = 'ps_checkout'
//    src = 'https://checkout.paystand.co/v4/js/paystand.checkout.js'
//    ps-env = 'sandbox'
//    ps-publishableKey = 't4f464u6sbjtljj2w4wrrm1a'
//    ps-containerId = 'ps_container_id'
//    ps-mode = 'modal'
//    ps-viewFunds = 'card'
//    ps-checkoutType = 'checkout_payment'
//    ps-paymentAmount = '{0}'
//    ps-fixedAmount = 'true'
//    ps-payerName = '{1}'
//    ps-payerEmail = '{2}'
//    ps-payerAddressStreet = '{3}'
//    ps-payerAddressCity = '{4}'
//    ps-payerAddressPostal = '{5}'
//    ps-payerAddressCountry = '{6}'
//    ps-payerAddressState = '{7}'
//    ps-customReceipt = 'Thank you for your order!<br>Your order confirmation number is {8}'
//    ps-checkoutEvent = psEvent => {{console.log('here')}}
//    >
//</script> 
//"
//                , Convert.ToString(litTotal.Text)
//                , CurrentUser.FirstName + ' ' + CurrentUser.LastName
//                , CurrentUser.EmailAddress
//                , objBillingAddressBook.AddressLine1 + ' ' + objBillingAddressBook.AddressLine2
//                , objBillingAddressBook.City
//                , objBillingAddressBook.PostalCode
//                , objBillingAddressBook.CountryID
//                , objBillingAddressBook.State
//                , CurrentUser.CurrentUserWebSite.ShoppingCart.ShoppingCartID);
//            }
//        }
      
        protected void btnTest_Click(object sender, EventArgs e)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.Append("$(document).ready(function () {");
            sb.Append("console.log('here5', document);");
            sb.Append("$('#ps_checkout').ready(function() {");
            sb.Append("console.log('here8', psCheckout);");
            sb.Append("})});");

            ScriptManager.RegisterStartupScript(this, typeof(Page), "Test", sb.ToString(), true);
        }

        protected void txtCCCardNumber_TextChanged(object sender, EventArgs e)
        {
            txtCCType.Text = GetCardType(txtCCCardNumber.Text);
        }
        protected string GetCardType(string cardnumber)
        {
            if (Regex.IsMatch(cardnumber, "^(34|37)") && cardnumber.Length == 15)
                return "AMEX";
            else if (Regex.IsMatch(cardnumber, "^(51|52|53|54|55)") && cardnumber.Length == 16)
                return "MasterCard ";
            else if (Regex.IsMatch(cardnumber, "^(4)") && (cardnumber.Length == 13 || cardnumber.Length == 16))
                return "Visa ";
            else if (Regex.IsMatch(cardnumber, "^(6011)") && cardnumber.Length == 16)
                return "Discover ";
            else
                return string.Empty;
        }
        //protected void btnPlaceOrderPaystand_Click(object sender, EventArgs e)
        //{         
        //    string strPaystandData = Convert.ToString(hfPaystandData.Value);
        //    PlaceOrder();
        //}

        public string Decrypt(string value, string encryptionKey)
        {
            string decryptValue = string.Empty;

            byte[] cipherBytes = Convert.FromBase64String(value);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(encryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(cipherBytes, 0, cipherBytes.Length);
                        cs.Close();
                    }
                    decryptValue = Encoding.Unicode.GetString(ms.ToArray());
                }
            }
            return decryptValue;
        }

        protected void cbSaveCard_CheckedChanged(object sender, EventArgs e)
        {
            phCreditCardNickname.Visible = cbSaveCard.Checked;
        }

        protected void chkApplyBudget_CheckedChanged(object sender, EventArgs e)
        {
            UpdateShoppingCart();

            UpdateApplyBudget();
        }

        protected void UpdateApplyBudget()
        {
            phBudget.Visible = chkApplyBudget.Checked;
            ddlWebsiteShippingService.Enabled = true;

            bool DisplayBudgetShippingAndTaxes = false;

            ImageSolutions.Budget.MyBudgetAssignment MyBudgetAssignment = new ImageSolutions.Budget.MyBudgetAssignment(CurrentUser.UserInfoID, ddlMyBudgetAssignment.SelectedValue);

            double dblTotal = double.Parse(litTotal.Text, NumberStyles.AllowCurrencySymbol | NumberStyles.Currency);

            if (phBudget.Visible && MyBudgetAssignment != null && !MyBudgetAssignment.BudgetAssignment.Budget.IncludeShippingAndTaxes)
            {
                ddlWebsiteShippingService.Enabled = false;

                if (MyBudgetAssignment.BudgetAssignment.Budget.DisplayShippingAndTaxes)
                {
                    pnlShippingAndTaxes.Visible = true;
                    pnlBudgetShippingAndTaxes.Visible = true;

                    DisplayBudgetShippingAndTaxes = true;
                }
                else
                {
                    pnlShippingAndTaxes.Visible = false;
                    pnlBudgetShippingAndTaxes.Visible = false;
                }

                decimal decTotal = Convert.ToDecimal(CurrentUser.CurrentUserWebSite.ShoppingCart.GetTotal(CurrentWebsite.WebsiteID, true)) - Convert.ToDecimal(CurrentUser.CurrentUserWebSite.ShoppingCart.ShippingAmount);
                //if (CurrentWebsite.EnableIPD && IsInternational())
                //{
                //    decTotal = decTotal + decimal.Parse(litIPDAmount.Text, NumberStyles.AllowCurrencySymbol | NumberStyles.Currency);
                //}

                string strTotal = Convert.ToString(decTotal);
                litTotal.Text = string.Format("{0:c}", strTotal);

                if (!string.IsNullOrEmpty(CurrentWebsite.CurrencyConvert)
                    && CurrentWebsite.CurrentyConvertPercentage != null
                    && CurrentWebsite.CurrentyConvertPercentage > 0)
                {
                    double dblTotalCurrencyConvert = double.Parse(litTotal.Text, NumberStyles.AllowCurrencySymbol | NumberStyles.Currency);
                    this.litTotalCurrencyConvert.Text = string.Format(@" USD <span style=""font-size: small; "" title=""est. {0:c} {1}""><i class=""ti-info-alt"" ></i> </span>"
                        , Convert.ToDecimal(dblTotalCurrencyConvert) * CurrentWebsite.CurrentyConvertPercentage
                        , CurrentWebsite.CurrencyConvert
                        );
                }

                dblTotal = Convert.ToDouble(decTotal);
            }
            else
            {
                pnlShippingAndTaxes.Visible = true;
                pnlBudgetShippingAndTaxes.Visible = false;
            }

            if (chkApplyBudget.Checked)
            {

                if (DisplayBudgetShippingAndTaxes)
                {
                    decimal decBudgetShipping = Convert.ToDecimal(CurrentUser.CurrentUserWebSite.ShoppingCart.ShippingAmount) * -1;
                    litBudgetShipping.Text = string.Format("{0:c}", decBudgetShipping);

                    if (MyBudgetAssignment.BudgetAssignment.Budget.TaxNonBudgetAmount && CurrentUser.CurrentUserWebSite.ShoppingCart.LineTotal > MyBudgetAssignment.Balance)
                    {
                        //double dblNonBudgetTax = CurrentUser.CurrentUserWebSite.ShoppingCart.GetAvalaraTaxForBudget(Convert.ToDecimal(CurrentUser.CurrentUserWebSite.ShoppingCart.LineTotal - MyBudgetAssignment.Balance));
                        double dblNonBudgetTax = CurrentUser.CurrentUserWebSite.ShoppingCart.GetAvalaraTaxForBudget(Convert.ToDecimal(MyBudgetAssignment.Balance));
                        dblTotal = dblTotal + dblNonBudgetTax;

                        decimal decBudgetTax = (Convert.ToDecimal(CurrentUser.CurrentUserWebSite.ShoppingCart.SalesTaxAmount) - Convert.ToDecimal(dblNonBudgetTax)) * -1;
                        if (CurrentWebsite.EnableIPD && IsInternational())
                        {
                            decBudgetTax = decBudgetTax - decimal.Parse(litIPDAmount.Text, NumberStyles.AllowCurrencySymbol | NumberStyles.Currency);
                        }

                        litBudgetTax.Text = string.Format("{0:c}", decBudgetTax);
                        
                        litTotal.Text = string.Format("{0:c}", Convert.ToString(dblTotal));

                        if (!string.IsNullOrEmpty(CurrentWebsite.CurrencyConvert)
                            && CurrentWebsite.CurrentyConvertPercentage != null
                            && CurrentWebsite.CurrentyConvertPercentage > 0)
                        {
                            double dblTotalCurrencyConvert = double.Parse(litTotal.Text, NumberStyles.AllowCurrencySymbol | NumberStyles.Currency);
                            this.litTotalCurrencyConvert.Text = string.Format(@" USD <span style=""font-size: small; "" title=""est. {0:c} {1}""><i class=""ti-info-alt"" ></i> </span>"
                                , Convert.ToDecimal(dblTotalCurrencyConvert) * CurrentWebsite.CurrentyConvertPercentage
                                , CurrentWebsite.CurrencyConvert
                                );
                        }
                    }
                    else
                    {
                        decimal decBudgetTax = Convert.ToDecimal(CurrentUser.CurrentUserWebSite.IsTaxExempt ? 0 : CurrentUser.CurrentUserWebSite.ShoppingCart.SalesTaxAmount) * -1;
                        if (CurrentWebsite.EnableIPD && IsInternational())
                        {
                            decBudgetTax = decBudgetTax - decimal.Parse(litIPDAmount.Text, NumberStyles.AllowCurrencySymbol | NumberStyles.Currency);
                        }

                        litBudgetTax.Text = string.Format("{0:c}", decBudgetTax);
                    }
                }

                if (dblTotal > MyBudgetAssignment.Balance)
                {
                    double dblRemainingTotal = dblTotal - MyBudgetAssignment.Balance;
                    litRemainingTotal.Text = string.Format("{0:c}", dblRemainingTotal);

                    ImageSolutions.Budget.Budget Budget = new ImageSolutions.Budget.Budget(MyBudgetAssignment.BudgetAssignment.Budget.BudgetID);

                    if (Budget.AllowOverBudget)
                    {
                        pnlPaymentOption.Visible = true;

                        if (!string.IsNullOrEmpty(Budget.PaymentTermID) && Budget.OverBudgetPaymentTerm != null)
                        {
                            phInvoicePayment.Visible = true;
                            lblInvoicePayment.Text = String.Format("Terms: {0}", Convert.ToString(Budget.OverBudgetPaymentTerm.Description));
                            rbnUserCreditCard.Checked = false;
                            rbnNewCreditCard.Checked = false;
                            rbnInvoicePayment.Checked = true;
                        }
                        else
                        {
                            SetInvoicePayment();
                            //rbnUserCreditCard.Checked = false;
                            //rbnNewCreditCard.Checked = false;
                            //rbnInvoicePayment.Checked = false;
                            phInvoicePaymentMessage.Visible = rbnInvoicePayment.Checked;

                            if (CurrentWebsite.MustUseExistingEmployeeCredit && !phEnableCreditCard.Visible)
                            {
                                this.phEnableCreditCard.Visible = CurrentWebsite.EnableCreditCard && !CurrentUser.CurrentUserWebSite.CurrentUserAccount.Account.DoNotAllowCreditCard;
                            }
                        }
                    }
                    else
                    {
                        SetInvoicePayment();
                        rbnUserCreditCard.Checked = false;
                        rbnNewCreditCard.Checked = false;
                        rbnInvoicePayment.Checked = false;
                        phInvoicePaymentMessage.Visible = rbnInvoicePayment.Checked;
                        pnlPaymentOption.Visible = false;
                    }
                    ChangePaymentOption();

                    litBudgetAppliedAmount.Text = string.Format("{0:c}", MyBudgetAssignment.Balance * -1);
                    
                }
                else
                {
                    litRemainingTotal.Text = string.Format("{0:c}", 0);
                    pnlPaymentOption.Visible = false;
                    litBudgetAppliedAmount.Text = string.Format("{0:c}", dblTotal * -1);
                }

                pnlBudgetAppliedSummary.Visible = true;
                litTotal.Visible = false;
                litTotalWithBudget.Visible = true;
                litTotalWithBudget.Text = string.Format("{0:c}", litRemainingTotal.Text);

                if (!string.IsNullOrEmpty(CurrentWebsite.CurrencyConvert)
                    && CurrentWebsite.CurrentyConvertPercentage != null
                    && CurrentWebsite.CurrentyConvertPercentage > 0)
                {
                    double dblTotalCurrencyConvert = double.Parse(litTotalWithBudget.Text, NumberStyles.AllowCurrencySymbol | NumberStyles.Currency);
                    this.litTotalWithBudgetCurrencyeConvert.Text = string.Format(@" USD <span style=""font-size: small; "" title=""est. {0:c} {1}""><i class=""ti-info-alt"" ></i> </span>"
                        , Convert.ToDecimal(dblTotalCurrencyConvert) * CurrentWebsite.CurrentyConvertPercentage
                        , CurrentWebsite.CurrencyConvert
                        );
                }

                if (dblTotal < 0)
                { 
                    chkApplyBudget.Checked = false;
                    UpdateShoppingCart();
                }
            }
            else
            {
                SetInvoicePayment();
                rbnInvoicePayment.Checked = false;
                phInvoicePaymentMessage.Visible = rbnInvoicePayment.Checked;

                rbnUserCreditCard.Checked = false;
                //rbnNewCreditCard.Checked = false;
 
                litRemainingTotal.Text = litTotal.Text;
                pnlPaymentOption.Visible = true;

                pnlBudgetAppliedSummary.Visible = false;
                litTotal.Visible = true;
                litTotalWithBudget.Visible = false;

                if (phBudgetOption.Visible && CurrentWebsite.MustUseExistingEmployeeCredit)
                {
                    phEnableCreditCard.Visible = false;
                    phInvoicePayment.Visible = false;
                    rbnInvoicePayment.Checked = false;
                }
            }
        }

        protected void rptCustomField_ItemDataBound(object sender, RepeaterItemEventArgs e) 
        {
            try
            {
                if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
                {
                    ImageSolutions.Custom.CustomField CustomField = (ImageSolutions.Custom.CustomField)e.Item.DataItem;

                    TextBox txtCustomValue = (TextBox)e.Item.FindControl("txtCustomValue");
                    DropDownList ddlCustomValueList = (DropDownList)e.Item.FindControl("ddlCustomValueList");
                    RequiredFieldValidator valCustomField = (RequiredFieldValidator)e.Item.FindControl("valCustomField");
                    Label lblHint = (Label)e.Item.FindControl("lblHint");

                    Panel pnlLabel = (Panel)e.Item.FindControl("pnlLabel");
                    Literal litLabelHTML = (Literal)e.Item.FindControl("litLabelHTML");
                    if (!string.IsNullOrEmpty(CustomField.LabelHTML))
                    {
                        pnlLabel.Visible = false;
                        litLabelHTML.Visible = true;
                        litLabelHTML.Text = CustomField.LabelHTML;
                    }
                    else
                    {
                        litLabelHTML.Visible = false;
                    }

                    if (CustomField.Type == "dropdown")
                    {
                        txtCustomValue.Visible = false;
                        ddlCustomValueList.Visible = true;

                        List<ImageSolutions.Custom.CustomValueList> CustomValueLists = new List<ImageSolutions.Custom.CustomValueList>();
                        ImageSolutions.Custom.CustomValueListFilter CustomValueListFilter = new ImageSolutions.Custom.CustomValueListFilter();

                        if (!string.IsNullOrEmpty(CustomField.AccountCustomFieldID))
                        {
                            CustomValueListFilter.FilterAccountID = new Database.Filter.StringSearch.SearchFilter();
                            CustomValueListFilter.FilterAccountID.SearchString = CurrentUser.CurrentUserWebSite.CurrentUserAccount.AccountID;
                            CustomValueListFilter.CustomFieldID = new Database.Filter.StringSearch.SearchFilter();
                            CustomValueListFilter.CustomFieldID.SearchString = CustomField.AccountCustomFieldID;
                        }
                        else
                        {
                            CustomValueListFilter.CustomFieldID = new Database.Filter.StringSearch.SearchFilter();
                            CustomValueListFilter.CustomFieldID.SearchString = CustomField.CustomFieldID;
                        }
                        CustomValueListFilter.Inactive = false;
                        CustomValueLists = ImageSolutions.Custom.CustomValueList.GetCustomValueLists(CustomValueListFilter);

                        ddlCustomValueList.DataSource = CustomValueLists;
                        ddlCustomValueList.DataBind();
                        ddlCustomValueList.Items.Insert(0, new ListItem(String.Empty, string.Empty));

                        if (!string.IsNullOrEmpty(CustomField.DefaultUserWebsiteCustomFieldID))
                        {
                            ImageSolutions.Custom.CustomValue CustomValue = new ImageSolutions.Custom.CustomValue();
                            ImageSolutions.Custom.CustomValueFilter CustomValueFilter = new ImageSolutions.Custom.CustomValueFilter();
                            CustomValueFilter.CustomFieldID = new Database.Filter.StringSearch.SearchFilter();
                            CustomValueFilter.CustomFieldID.SearchString = CustomField.DefaultUserWebsiteCustomFieldID;
                            CustomValueFilter.UserWebsiteID = new Database.Filter.StringSearch.SearchFilter();
                            CustomValueFilter.UserWebsiteID.SearchString = CurrentUser.CurrentUserWebSite.UserWebsiteID;
                            CustomValue = ImageSolutions.Custom.CustomValue.GetCustomValue(CustomValueFilter);
                            if (CustomValue != null) ddlCustomValueList.SelectedValue = Convert.ToString(CustomValue.Value);
                        }

                    }
                    else
                    {
                        txtCustomValue.Visible = true;
                        ddlCustomValueList.Visible = false;

                        if (!string.IsNullOrEmpty(CustomField.DefaultUserWebsiteCustomFieldID))
                        {
                            ImageSolutions.Custom.CustomValue CustomValue = new ImageSolutions.Custom.CustomValue();
                            ImageSolutions.Custom.CustomValueFilter CustomValueFilter = new ImageSolutions.Custom.CustomValueFilter();
                            CustomValueFilter.CustomFieldID = new Database.Filter.StringSearch.SearchFilter();
                            CustomValueFilter.CustomFieldID.SearchString = CustomField.DefaultUserWebsiteCustomFieldID;
                            CustomValueFilter.UserWebsiteID = new Database.Filter.StringSearch.SearchFilter();
                            CustomValueFilter.UserWebsiteID.SearchString = CurrentUser.CurrentUserWebSite.UserWebsiteID;
                            CustomValue = ImageSolutions.Custom.CustomValue.GetCustomValue(CustomValueFilter);
                            if (CustomValue != null) txtCustomValue.Text = Convert.ToString(CustomValue.Value);
                        }
                    }

                    if (string.IsNullOrEmpty(CustomField.Hint))
                    {
                        lblHint.Visible = false;
                    }
                    else
                    {
                        lblHint.Visible = true;
                        lblHint.Text = CustomField.Hint;
                    }
                }
            }
            catch (Exception ex)
            {
                WebUtility.DisplayJavascriptMessage(this, ex.Message);
            }
        }
        protected void gvPromotions_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            CurrentUser.CurrentUserWebSite.ShoppingCart.DeletePromotion(this.gvPromotions.DataKeys[e.RowIndex]["PromotionID"].ToString());
            UpdateShoppingCart();
        }
        protected void custValidPromotion_ServerValidate(object source, ServerValidateEventArgs args)
        {
            try
            {
                CurrentUser.CurrentUserWebSite.ShoppingCart.AddGiftCertificateOrPromotionCode(this.txtPromotionCode.Text.Trim());
                this.txtPromotionCode.Text = string.Empty;
                args.IsValid = true;
            }
            catch (Exception ex)
            {
                WebUtility.DisplayJavascriptMessage(this, ex.Message);
                args.IsValid = false;
            }
        }
        protected void btnApplyPromotion_Click(object sender, EventArgs e)
        {
            UpdateShoppingCart();
        }

        protected void ValidateShoppingCartItems()
        {
            foreach (ImageSolutions.ShoppingCart.ShoppingCartLine _ShoppingCartLine in CurrentUser.CurrentUserWebSite.ShoppingCart.ShoppingCartLines)
            {
                if (string.IsNullOrEmpty(_ShoppingCartLine.Item.ParentID) || !_ShoppingCartLine.Item.ItemWebsites.Exists(x => x.WebsiteID == CurrentWebsite.WebsiteID))
                {
                    throw new Exception(String.Format("Invalid Item: {0}", _ShoppingCartLine.Item.Description));
                }
            }
        }

        protected void chkRegister_CheckedChanged(object sender, EventArgs e)
        {
            this.divCreateAccountForm.Visible = chkRegister.Checked;
        }

        protected void chkPartialShipping_CheckedChanged(object sender, EventArgs e)
        {
            UpdateShippingMethod();
            UpdateShoppingCart();
        }

        protected bool IsInternational()
        {
            bool blnReturn = false;
            if (!string.IsNullOrEmpty(ddlShippingAddress.SelectedValue))
            {
                ImageSolutions.Address.AddressBook objShippingAddressBook = new ImageSolutions.Address.AddressBook(Convert.ToString(ddlShippingAddress.SelectedValue));
                blnReturn = objShippingAddressBook.CountryCode != "US";
            }
            return blnReturn;
        }

        protected bool ValidatePackage()
        {
            bool blnReturn = true;
            List<PackageCount> PackageCounts = new List<PackageCount>();
            try
            {
                foreach (ImageSolutions.ShoppingCart.ShoppingCartLine _ShoppingCartLine in CurrentUser.CurrentUserWebSite.ShoppingCart.ShoppingCartLines)
                {
                    string strItemID = _ShoppingCartLine.ItemID;
                    int Quantity = 0;
                    ImageSolutions.Item.Item Item = new ImageSolutions.Item.Item(strItemID);
                    ImageSolutions.Item.Item ParentItem = new ImageSolutions.Item.Item(Item.ParentID);

                    string strSize = string.Empty;
                    foreach (ImageSolutions.Item.ItemAttributeValue _ItemAttributeValue in Item.ItemAttributeValues)
                    {
                        if (_ItemAttributeValue.AttributeValue.Attribute.AttributeName == "Size")
                        {
                            strSize = _ItemAttributeValue.AttributeValue.Value;
                        }
                    }

                    AddPackageCount(ParentItem, _ShoppingCartLine.Quantity, ref PackageCounts, strSize);
                }
            }
            catch (Exception ex)
            {
                throw ex;
                //WebUtility.DisplayJavascriptMessage(this, ex.Message);
                //blnReturn = false;
            }

            if (blnReturn)
            {
                string strPackageMessage = string.Empty;

                ImageSolutions.Package.Package Package = new ImageSolutions.Package.Package(Convert.ToString(CurrentUser.CurrentUserWebSite.ShoppingCart.PackageID));
                foreach (ImageSolutions.Package.PackageLine _PacakgeLine in Package.PackageLines)
                {
                    if (PackageCounts.Exists(x => x.PackageGroupID == _PacakgeLine.PackageGroupID))
                    {
                        PackageCount PackageCount = PackageCounts.Find(x => x.PackageGroupID == _PacakgeLine.PackageGroupID);
                        ImageSolutions.Package.PackageGroup PackageGroup = new ImageSolutions.Package.PackageGroup(_PacakgeLine.PackageGroupID);

                        if (_PacakgeLine.ValidateSingleSize && !PackageCount.IsSingleAttribute)
                        {
                            strPackageMessage = string.Format(@"{0}\nMultiple sizes are not allowed for '{1}'", strPackageMessage, PackageGroup.Name);

                            blnReturn = false;
                        }

                        if (PackageCount.Quantity != _PacakgeLine.Quantity)
                        {
                            if (PackageCount.Quantity > _PacakgeLine.Quantity)
                            {
                                strPackageMessage = string.Format(@"{0}\nToo Many '{1}' - Quantity To Remove {2}", strPackageMessage, PackageGroup.Name, PackageCount.Quantity - _PacakgeLine.Quantity);
                            }
                            else
                            {
                                strPackageMessage = string.Format(@"{0}\nMissing '{1}' - Quantity To Add {2}", strPackageMessage, PackageGroup.Name, _PacakgeLine.Quantity - PackageCount.Quantity);
                            }

                            blnReturn = false;
                        }
                    }
                    else
                    {
                        if (!_PacakgeLine.IsOptional)
                        {
                            ImageSolutions.Package.PackageGroup PackageGroup = new ImageSolutions.Package.PackageGroup(_PacakgeLine.PackageGroupID);

                            strPackageMessage = string.Format(@"{0}\nMissing '{1}' - Quantity To Add: {2}", strPackageMessage, PackageGroup.Name, _PacakgeLine.Quantity);

                            blnReturn = false;
                        }
                    }
                }

                //if (!string.IsNullOrEmpty(strPackageMessage))
                //{
                //    //throw new Exception(strPackageMessage);
                //    //WebUtility.DisplayJavascriptMessage(this, strPackageMessage);
                //}
            }

            return blnReturn;
        }

        protected void AddPackageCount(ImageSolutions.Item.Item item, int quantity, ref List<PackageCount> packagecounts, string attribute = "")
        {
            if (!string.IsNullOrEmpty(item.PackageGroupID))
            {
                if (packagecounts.Exists(x => x.PackageGroupID == item.PackageGroupID))
                {
                    PackageCount PackageCount = packagecounts.Find(x => x.PackageGroupID == item.PackageGroupID);
                    PackageCount.Quantity += quantity;
                    if (PackageCount.IsSingleAttribute)
                    {
                        PackageCount.IsSingleAttribute = PackageCount.Attribute == attribute;
                    }
                }
                else
                {
                    PackageCount PackageCount = new PackageCount();
                    PackageCount.PackageGroupID = item.PackageGroupID;
                    PackageCount.Quantity = quantity;
                    PackageCount.Attribute = attribute;
                    PackageCount.IsSingleAttribute = true;
                    packagecounts.Add(PackageCount);
                }
            }
            else
            {
                throw new Exception(string.Format("Item {0} not available for package", item.ItemName));
            }
        }

        protected void rbnPackage_CheckedChanged(object sender, EventArgs e)
        {

        }

        protected void ddlPackage_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        public class PackageCount
        {
            public string PackageGroupID { get; set; }
            public int Quantity { get; set; }
            public string Attribute { get; set; }
            public bool IsSingleAttribute { get; set; }
        }
    }
}