using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.Remoting;
using System.Web;
using System.Web.Hosting;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ImageSolutionsWebsite
{
    public partial class AccountRegistration : BasePageUserNoAuth
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!CurrentWebsite.IsLoggedIn || !CurrentWebsite.AccountRegistration) Response.Redirect("/login.aspx");

            if (!Page.IsPostBack)
            {
                Initialize();
            }
        }

        protected void Initialize()
        {
            BindCustomField();
            BindCountry();

            switch (CurrentWebsite.WebsiteID)
            {
                case "2": //Burger King
                    lblMemberNumber.Text = "RSI Member Number";
                    break;
                case "5": //Securitas
                    lblMemberNumber.Text = "Controller Email";
                    break;
                default:
                    break;
            }
            this.divRegistrationKey.Visible = CurrentWebsite.AccountRegistrationKeyRequired;
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
                CustomFieldFilter.Location.SearchString = "user";
                CustomFieldFilter.Inactive = false;
                CustomFields = ImageSolutions.Custom.CustomField.GetCustomFields(CustomFieldFilter);

                rptCustomField.DataSource = CustomFields;
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
                if (CurrentWebsite.WebsiteCountries.Count > 0)
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
            }
            catch (Exception ex)
            {
                WebUtility.DisplayJavascriptMessage(this, ex.Message);
            }
            finally { }
        }
        protected void btnRegister_Click(object sender, EventArgs e)
        {
            SqlConnection objConn = null;
            SqlTransaction objTran = null;
            ImageSolutions.Account.Account objParentAccount = null;
            ImageSolutions.Account.AccountFilter objFilter = null;
            ImageSolutions.Account.Account objAccount = null;
            bool blnReturn = false;

            try
            {
                objConn = new SqlConnection(Database.DefaultConnectionString);
                objConn.Open();
                objTran = objConn.BeginTransaction();

                objFilter = new ImageSolutions.Account.AccountFilter();
                objFilter.WebsiteID = new Database.Filter.StringSearch.SearchFilter();
                objFilter.WebsiteID.SearchString = CurrentWebsite.WebsiteID;
                if (CurrentWebsite.AccountRegistrationKeyRequired)
                {
                    objFilter.RegistrationKey = new Database.Filter.StringSearch.SearchFilter();
                    objFilter.RegistrationKey.SearchString = this.txtRegistrationKey.Text.Trim();
                }
                objParentAccount = ImageSolutions.Account.Account.GetAccount(objFilter);
                if (objParentAccount == null) throw new Exception("Registration Key Not Found!");

                ImageSolutions.User.UserInfo UserInfo = new ImageSolutions.User.UserInfo();
                UserInfo.FirstName = txtFirstName.Text;
                UserInfo.LastName = txtLastName.Text;
                UserInfo.EmailAddress = txtEmail.Text;
                UserInfo.Password = txtPassword.Text.Trim();
                UserInfo.Create(objConn, objTran);

                objAccount = new ImageSolutions.Account.Account();

                int intAccountNumber = 1;
                string strAccountName = string.Format("{0}-{1}", this.txtRegistrationKey.Text.Trim(), intAccountNumber);

                while (ExistsAccount(strAccountName))
                {
                    intAccountNumber++;
                    strAccountName = string.Format("{0}-{1}", this.txtRegistrationKey.Text.Trim(), intAccountNumber);
                }
                objAccount.AccountName = strAccountName;

                //objAccount.AccountName = this.txtRegistrationKey.Text.Trim() + "-" + DateTime.Now.Second.ToString();
                objAccount.ParentID = objParentAccount.AccountID;
                objAccount.DefaultWebsiteGroupID = objParentAccount.DefaultWebsiteGroupID;
                if (objAccount.DefaultShippingAddressBook == null) objAccount.DefaultShippingAddressBook = new ImageSolutions.Address.AddressBook();
                objAccount.DefaultShippingAddressBook.AddressLabel = txtFirstName.Text + " " + txtLastName.Text;
                objAccount.DefaultShippingAddressBook.FirstName = txtFirstName.Text;
                objAccount.DefaultShippingAddressBook.LastName = txtLastName.Text;
                objAccount.DefaultShippingAddressBook.AddressLine1 = txtAddress.Text;
                objAccount.DefaultShippingAddressBook.AddressLine2 = txtAddress2.Text;
                objAccount.DefaultShippingAddressBook.City = txtCity.Text;
                objAccount.DefaultShippingAddressBook.State = txtState.Text;
                objAccount.DefaultShippingAddressBook.PostalCode = txtZip.Text;
                objAccount.DefaultShippingAddressBook.CountryCode = Convert.ToString(ddlCountry.SelectedValue);
                objAccount.DefaultShippingAddressBook.PhoneNumber = txtPhone.Text;
                objAccount.DefaultShippingAddressBook.CreatedBy = CurrentUser.UserInfoID;
                objAccount.WebsiteID = CurrentWebsite.WebsiteID;
                objAccount.IsPendingApproval = CurrentWebsite.AccountApprovalRequired;
                objAccount.CreatedBy = UserInfo.UserInfoID;

                if (CurrentWebsite.WebsiteID == "5")
                {
                    objAccount.AccountName = txtEmail.Text;
                    objAccount.RegistrationKey = txtEmail.Text;
                }

                objAccount.Create(objConn, objTran);

                ImageSolutions.User.UserWebsite objUserWebsite = new ImageSolutions.User.UserWebsite();
                objUserWebsite.UserInfoID = UserInfo.UserInfoID;
                objUserWebsite.WebsiteID = CurrentWebsite.WebsiteID;
                objUserWebsite.IsAdmin = false;
                objUserWebsite.IsPendingApproval = false; //set this to false because once account is approved, user should be able to login automatically

                if (!string.IsNullOrEmpty(CurrentWebsite.DefaultPaymentTermID))
                {
                    objUserWebsite.PaymentTermID = CurrentWebsite.DefaultPaymentTermID;
                }

                objUserWebsite.CreatedBy = UserInfo.UserInfoID;
                blnReturn = objUserWebsite.Create(objConn, objTran);

                foreach (RepeaterItem _Item in this.rptCustomField.Items)
                {
                    string strCustomFieldID = ((HiddenField)_Item.FindControl("hfCustomFieldID")).Value;

                    ImageSolutions.Custom.CustomValue CustomValue = new ImageSolutions.Custom.CustomValue();
                    CustomValue.CustomFieldID = strCustomFieldID;
                    CustomValue.UserWebsiteID = objUserWebsite.UserWebsiteID;

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
                    }

                    if (CustomField.IsRequired && string.IsNullOrEmpty(CustomValue.Value))
                    {
                        throw new Exception(String.Format("{0} is required", CustomField.Name));
                    }

                    blnReturn = CustomValue.Create(objConn, objTran);
                }


                if (!string.IsNullOrEmpty(objAccount.DefaultWebsiteGroupID) || CurrentWebsite.DefaultWebsiteGroup != null)
                {
                    ImageSolutions.User.UserAccount objUserAccount = new ImageSolutions.User.UserAccount();
                    objUserAccount.UserWebsiteID = objUserWebsite.UserWebsiteID;
                    objUserAccount.AccountID = objAccount.AccountID;
                    objUserAccount.WebsiteGroupID = String.IsNullOrEmpty(objAccount.DefaultWebsiteGroupID) ? CurrentWebsite.DefaultWebsiteGroup.WebsiteGroupID : objAccount.DefaultWebsiteGroupID;
                    objUserAccount.CreatedBy = UserInfo.UserInfoID;
                    blnReturn = objUserAccount.Create(objConn, objTran);
                }

                if (blnReturn && CurrentWebsite.AccountApprovalRequired)
                {
                    if (CurrentWebsite.WebsiteID == "2") //BK
                    {
                        //Send out email to parent account for sub account approval
                        SendSubAccountRegistration(objParentAccount, objAccount, objUserWebsite);
                    }
                    else
                    {
                        SendAccountRegistration(objParentAccount, objAccount, objUserWebsite);
                    }
                }

                objTran.Commit();

            }
            catch (Exception ex)
            {
                if (objTran != null) objTran.Rollback();
                blnReturn = false;
                WebUtility.DisplayJavascriptMessage(this, ex.Message);
            }
            finally
            {
                if (objTran != null) objTran.Dispose();
                objTran = null;
                if (objConn != null) objConn.Dispose();
                objConn = null;
            }

            if (blnReturn) Response.Redirect("/AccountRegistrationComplete.aspx");

        }

        protected bool ExistsAccount(string accountname)
        {
            ImageSolutions.Account.Account Account = null;
            ImageSolutions.Account.AccountFilter AccountFilter = null;

            try
            {
                Account = new ImageSolutions.Account.Account();
                AccountFilter = new ImageSolutions.Account.AccountFilter();
                AccountFilter.AccountName = new Database.Filter.StringSearch.SearchFilter();
                AccountFilter.AccountName.SearchString = accountname;
                Account = ImageSolutions.Account.Account.GetAccount(AccountFilter);

                return Account != null && !string.IsNullOrEmpty(Account.AccountID);
            }
            catch (Exception ex)
            {
                throw ex;
            }           
            finally
            {
                Account = null;
                AccountFilter = null;
            }
        }

        protected bool SendSubAccountRegistration(ImageSolutions.Account.Account parentaccount, ImageSolutions.Account.Account subaccount, ImageSolutions.User.UserWebsite userwebsite)
        {
            try
            {
                List<string> SentEmails = new List<string>();

                foreach (ImageSolutions.User.UserAccount _UserAccount in parentaccount.UserAccounts)
                {
                    string strHTMLContent = @"<!DOCTYPE html>
                                        <html>
                                            <head></head>
                                            <body>
                                                <div style='font-size:14px;padding-bottom:30px;height:100%;position:relative;'>
                                                    <div style='float:right;width:45%; font-weight:bold; font-size:20px; text-align:right; padding-top:20px;'></div>

                                                    <div style='float:left; width:45%;'><img src='${Logo}' style='width:2.5in; height:0.76in ' /></div>

                                                    <div style='clear:left;padding-top:40px;'>
                                                        <div style='font-size:16px; font-weight:bold;'>Hi ${FirstName},</div>
                                                        <p>A subaccount has been registered by ${SubAccountFullName}, please <a href='${ApprovalURL}'>login</a> to the portal to review and approve the subaccount registration.</p>
                                                    </div>
                                                </div>
                                            </body>
                                        </html>";

                    strHTMLContent = strHTMLContent.Replace("${Logo}", string.IsNullOrEmpty(parentaccount.Website.EmailLogoPath) ? parentaccount.Website.LogoPath : parentaccount.Website.EmailLogoPath);
                    strHTMLContent = strHTMLContent.Replace("${FirstName}", _UserAccount.UserWebsite.UserInfo.FirstName);
                    strHTMLContent = strHTMLContent.Replace("${SubAccountFullName}", userwebsite.UserInfo.FullName);
                    strHTMLContent = strHTMLContent.Replace("${ApprovalURL}", WebUtility.PageSURL("/admin/account.aspx?id=" + subaccount.AccountID));

                    if(_UserAccount.UserWebsite.IsAdmin && _UserAccount.UserWebsite.StoreManagement && _UserAccount.UserWebsite.OptInForNotification)
                    {
                        string strEmaillAddress = string.IsNullOrEmpty(_UserAccount.UserWebsite.NotificationEmail) ? _UserAccount.UserWebsite.UserInfo.EmailAddress : _UserAccount.UserWebsite.NotificationEmail;

                        if (!SentEmails.Contains(strEmaillAddress))
                        {
                            SendEmail(strEmaillAddress, CurrentWebsite.Name + " Sub-Account Registration", strHTMLContent);
                            SentEmails.Add(_UserAccount.UserWebsite.UserInfo.EmailAddress);

                            if(_UserAccount.UserWebsite.WebsiteID == "2") // If Burger King - temporarily hard coded
                            {
                                SendEmail("hfebres@rsilink.com", CurrentWebsite.Name + " Sub-Account Registration", strHTMLContent);
                                SendEmail("scaballero@rsilink.com", CurrentWebsite.Name + " Sub-Account Registration", strHTMLContent);
                                SendEmail("smccarthy@rsilink.com", CurrentWebsite.Name + " Sub-Account Registration", strHTMLContent);
                                SendEmail("jprat@rsilink.com", CurrentWebsite.Name + " Sub-Account Registration", strHTMLContent);                                
                            }
                        }
                    }
                }
                //SendEmail(ParentAccount.UserAccounts[0].UserWebsite.UserInfo.EmailAddress, CurrentWebsite.Name + " Sub-Account Registration", strHTMLContent);
            }
            catch { }
            finally { }
            return true;
        }
        protected bool SendAccountRegistration(ImageSolutions.Account.Account parentaccount, ImageSolutions.Account.Account account, ImageSolutions.User.UserWebsite userwebsite)
        {
            try
            {
                List<string> SentEmails = new List<string>();

                foreach (ImageSolutions.User.UserAccount _UserAccount in parentaccount.UserAccounts)
                {
                    string strHTMLContent = @"<!DOCTYPE html>
                                        <html>
                                            <head></head>
                                            <body>
                                                <div style='font-size:14px;padding-bottom:30px;height:100%;position:relative;'>
                                                    <div style='float:right;width:45%; font-weight:bold; font-size:20px; text-align:right; padding-top:20px;'></div>

                                                    <div style='float:left; width:45%;'><img src='${Logo}' style='width:2.5in; height:0.76in ' /></div>

                                                    <div style='clear:left;padding-top:40px;'>
                                                        <div style='font-size:16px; font-weight:bold;'>Hi ${FirstName},</div>
                                                        <p>A account has been registered by ${AccountFullName}, please <a href='${ApprovalURL}'>login</a> to the portal to review and approve the account registration.</p>
                                                    </div>
                                                </div>
                                            </body>
                                        </html>";

                    strHTMLContent = strHTMLContent.Replace("${Logo}", string.IsNullOrEmpty(parentaccount.Website.EmailLogoPath) ? parentaccount.Website.LogoPath : parentaccount.Website.EmailLogoPath);
                    strHTMLContent = strHTMLContent.Replace("${FirstName}", _UserAccount.UserWebsite.UserInfo.FirstName);
                    strHTMLContent = strHTMLContent.Replace("${AccountFullName}", userwebsite.UserInfo.FullName);
                    strHTMLContent = strHTMLContent.Replace("${ApprovalURL}", WebUtility.PageSURL("/admin/account.aspx?id=" + account.AccountID));

                    if (_UserAccount.UserWebsite.IsAdmin && _UserAccount.UserWebsite.StoreManagement && _UserAccount.UserWebsite.OptInForNotification)
                    {
                        if (!SentEmails.Contains(_UserAccount.UserWebsite.UserInfo.EmailAddress))
                        {
                            SendEmail(_UserAccount.UserWebsite.UserInfo.EmailAddress, CurrentWebsite.Name + " Account Registration", strHTMLContent);
                            SentEmails.Add(_UserAccount.UserWebsite.UserInfo.EmailAddress);
                        }
                    }
                }
                //SendEmail(ParentAccount.UserAccounts[0].UserWebsite.UserInfo.EmailAddress, CurrentWebsite.Name + " Sub-Account Registration", strHTMLContent);
            }
            catch { }
            finally { }
            return true;
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

                    if (CustomField.Type == "dropdown")
                    {
                        txtCustomValue.Visible = false;
                        ddlCustomValueList.Visible = true;

                        List<ImageSolutions.Custom.CustomValueList> CustomValueLists = new List<ImageSolutions.Custom.CustomValueList>();
                        ImageSolutions.Custom.CustomValueListFilter CustomValueListFilter = new ImageSolutions.Custom.CustomValueListFilter();
                        CustomValueListFilter.CustomFieldID = new Database.Filter.StringSearch.SearchFilter();
                        CustomValueListFilter.CustomFieldID.SearchString = CustomField.CustomFieldID;
                        CustomValueListFilter.Inactive = false;
                        CustomValueLists = ImageSolutions.Custom.CustomValueList.GetCustomValueLists(CustomValueListFilter);

                        ddlCustomValueList.DataSource = CustomValueLists;
                        ddlCustomValueList.DataBind();
                        ddlCustomValueList.Items.Insert(0, new ListItem(String.Empty, string.Empty));
                    }
                    else
                    {
                        txtCustomValue.Visible = true;
                        ddlCustomValueList.Visible = false;
                    }
                }
            }
            catch (Exception ex)
            {
                WebUtility.DisplayJavascriptMessage(this, ex.Message);
            }
        }
    }
}