using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.Remoting;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ImageSolutionsWebsite
{
    public partial class UserRegistration : BasePageUserNoAuth
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!CurrentWebsite.IsLoggedIn || !CurrentWebsite.UserRegistration) Response.Redirect("/login.aspx");

            if (!Page.IsPostBack)
            {
                Initialize();
            }
        }

        protected void Initialize()
        {

            switch (CurrentWebsite.WebsiteID)
            {
                case "2": //Burger King
                    lblRegistrationKey.Text = "RSI Member Number";
                    break;
                case "5": //Securitas
                    lblRegistrationKey.Text = "AVP Email";
                    break;
                default:
                    break;
            }

            BindCustomField();
            this.divRegistrationKey.Visible = CurrentWebsite.UserRegistrationKeyRequired;
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
        protected void btnRegister_Click(object sender, EventArgs e)
        {
            SqlConnection objConn = null;
            SqlTransaction objTran = null;
            ImageSolutions.Account.Account objAccount = null;
            ImageSolutions.Account.AccountFilter objFilter = null;
            bool blnReturn = false;

            try
            {
                objConn = new SqlConnection(Database.DefaultConnectionString);
                objConn.Open();
                objTran = objConn.BeginTransaction();

                objFilter = new ImageSolutions.Account.AccountFilter();
                objFilter.WebsiteID = new Database.Filter.StringSearch.SearchFilter();
                objFilter.WebsiteID.SearchString = CurrentWebsite.WebsiteID;
                if (CurrentWebsite.UserRegistrationKeyRequired)
                {
                    objFilter.RegistrationKey = new Database.Filter.StringSearch.SearchFilter();
                    objFilter.RegistrationKey.SearchString = this.txtRegistrationKey.Text.Trim();
                }
                objAccount = ImageSolutions.Account.Account.GetAccount(objFilter);
                if (objAccount == null) throw new Exception("Registration Key Not Found!");

                ImageSolutions.User.UserInfo UserInfo = new ImageSolutions.User.UserInfo();
                UserInfo.FirstName = txtFirstName.Text;
                UserInfo.LastName = txtLastName.Text;
                UserInfo.EmailAddress = txtEmail.Text;
                UserInfo.Password = txtPassword.Text.Trim();
                UserInfo.Create(objConn, objTran);

                ImageSolutions.User.UserWebsite objUserWebsite = new ImageSolutions.User.UserWebsite();
                objUserWebsite.UserInfoID = UserInfo.UserInfoID;
                objUserWebsite.WebsiteID = CurrentWebsite.WebsiteID;
                objUserWebsite.IsAdmin = false;
                objUserWebsite.IsPendingApproval = CurrentWebsite.UserApprovalRequired;
                objUserWebsite.OptInForNotification = true;

                if(!string.IsNullOrEmpty(CurrentWebsite.DefaultPaymentTermID))
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

                if (blnReturn && CurrentWebsite.UserApprovalRequired)
                {
                    //Send out email to account admin and parent account admin
                    SendUserRegistration(objAccount, objUserWebsite);
                }

                objTran.Commit();
            }
            catch (Exception ex)
            {
                objTran.Rollback();
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

            if (blnReturn) Response.Redirect("/RegistrationComplete.aspx");
        }
        protected bool SendUserRegistration(ImageSolutions.Account.Account account, ImageSolutions.User.UserWebsite userwebsite)
        {
            try
            {
                List<string> SentEmails = new List<string>();

                foreach (ImageSolutions.User.UserAccount _UserAccount in account.UserAccounts)
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
                                                    <p>A user account has been registered by ${AccountFullName}, please <a href='${ApprovalURL}'>login</a> to the portal to review and approve the subaccount registration.</p>
                                                </div>
                                            </div>
                                        </body>
                                    </html>";

                    strHTMLContent = strHTMLContent.Replace("${Logo}", string.IsNullOrEmpty(account.Website.EmailLogoPath) ? account.Website.LogoPath : account.Website.EmailLogoPath);
                    strHTMLContent = strHTMLContent.Replace("${FirstName}", _UserAccount.UserWebsite.UserInfo.FirstName);
                    strHTMLContent = strHTMLContent.Replace("${AccountFullName}", userwebsite.UserInfo.FullName);
                    strHTMLContent = strHTMLContent.Replace("${ApprovalURL}", WebUtility.PageSURL("/admin/userwebsite.aspx?id=" + userwebsite.UserWebsiteID));
                  
                    if (_UserAccount.UserWebsite.IsAdmin && _UserAccount.UserWebsite.UserManagement && _UserAccount.UserWebsite.OptInForNotification)
                    {
                        if (!SentEmails.Contains(_UserAccount.UserWebsite.UserInfo.EmailAddress))
                        {
                            SendEmail(_UserAccount.UserWebsite.UserInfo.EmailAddress, CurrentWebsite.Name + " User Account Registration", strHTMLContent);
                            SentEmails.Add(_UserAccount.UserWebsite.UserInfo.EmailAddress);
                        }
                    }
                }
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