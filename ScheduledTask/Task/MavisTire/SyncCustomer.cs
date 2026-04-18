using ImageSolutions.MavisTire;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace ScheduledTask.Task.MavisTire
{
    public class SyncCustomer
    {
        public bool Execute()
        {
            List<MavisTireCustomer> Customers = null;
            MavisTireCustomerFilter CustomerFilter = null;

            try
            {
                Customers = new List<MavisTireCustomer>();
                CustomerFilter = new MavisTireCustomerFilter();
                //CustomerFilter.IsUpdated = true;

                //Test
                CustomerFilter.EmployeeNumber = new Database.Filter.StringSearch.SearchFilter();
                CustomerFilter.EmployeeNumber.SearchString = "416738";

                Customers = MavisTireCustomer.GetMavisTireCustomers(CustomerFilter);

                int counter = 0;

                foreach (MavisTireCustomer _Customer in Customers)
                {
                    counter++;

                    SqlConnection objConn = null;
                    SqlTransaction objTran = null;

                    bool blnIsInActive = false;

                    try
                    {
                        objConn = new SqlConnection(Database.DefaultConnectionString);
                        objConn.Open();
                        objTran = objConn.BeginTransaction();

                        Console.WriteLine(String.Format("{0}. Syncing Customer: {1}", counter, _Customer.MavisTireCustomerID));

                        string strEmail = !string.IsNullOrEmpty(_Customer.Email) ? Convert.ToString(_Customer.Email) : Convert.ToString(_Customer.WorkEmail);
                        string strFirstName = string.Empty;

                        if (!string.IsNullOrEmpty(strEmail))
                        {
                            ImageSolutions.User.UserWebsite UserWebsite = new ImageSolutions.User.UserWebsite();
                            ImageSolutions.User.UserWebsiteFilter UserWebsiteFilter = new ImageSolutions.User.UserWebsiteFilter();
                            UserWebsiteFilter.EmployeeID = new Database.Filter.StringSearch.SearchFilter();
                            UserWebsiteFilter.EmployeeID.SearchString = _Customer.EmployeeNumber;
                            UserWebsiteFilter.WebsiteID = new Database.Filter.StringSearch.SearchFilter();
                            UserWebsiteFilter.WebsiteID.SearchString = ConfigurationManager.AppSettings["MavisTireWebsiteID"];
                            UserWebsite = ImageSolutions.User.UserWebsite.GetUserWebsite(UserWebsiteFilter);

                            ImageSolutions.User.UserInfo UserInfo = new ImageSolutions.User.UserInfo();
                            //ImageSolutions.User.UserInfoFilter UserInfoFilter = new ImageSolutions.User.UserInfoFilter();
                            //UserInfoFilter.EmailAddress = new Database.Filter.StringSearch.SearchFilter();
                            //UserInfoFilter.EmailAddress.SearchString = _Customer.Email;
                            //UserInfoFilter.IsGuest = false;
                            //UserInfo = ImageSolutions.User.UserInfo.GetUserInfo(UserInfoFilter);


                            //Create UserInfo
                            //if (UserInfo == null)
                            if (UserWebsite == null)
                            {
                                UserInfo = new ImageSolutions.User.UserInfo();
                                UserInfo.EmailAddress = strEmail; //_Customer.Email;

                                String[] NameValues = null;
                                NameValues = _Customer.EmployeeName.Split(',');

                                UserInfo.LastName = String.IsNullOrEmpty(NameValues[1]) ? "-" : Convert.ToString(NameValues[0]).Trim();
                                UserInfo.FirstName = String.IsNullOrEmpty(NameValues[0]) ? "-" : Convert.ToString(NameValues[1]).Trim();

                                UserInfo.Password = "Mavis$1";

                                UserInfo.RequirePasswordReset = true;
                                UserInfo.Create(objConn, objTran);

                                strFirstName = UserInfo.FirstName;
                            }
                            else
                            {
                                UserInfo = UserWebsite.UserInfo;
                                UserInfo.EmailAddress = strEmail; //_Customer.Email;

                                String[] NameValues = null;
                                NameValues = _Customer.EmployeeName.Split(',');

                                UserInfo.LastName = String.IsNullOrEmpty(NameValues[0]) ? "-" : Convert.ToString(NameValues[0]).Trim();
                                UserInfo.FirstName = String.IsNullOrEmpty(NameValues[1]) ? "-" : Convert.ToString(NameValues[1]).Trim();

                                UserInfo.Update(objConn, objTran);

                                strFirstName = UserInfo.FirstName;
                            }

                            //Create UserWebsite - Annual Package
                            //ImageSolutions.User.UserWebsite UserWebsite = new ImageSolutions.User.UserWebsite();
                            //ImageSolutions.User.UserWebsiteFilter UserWebsiteFilter = new ImageSolutions.User.UserWebsiteFilter();
                            //UserWebsiteFilter.UserInfoID = new Database.Filter.StringSearch.SearchFilter();
                            //UserWebsiteFilter.UserInfoID.SearchString = UserInfo.UserInfoID;
                            //UserWebsiteFilter.WebsiteID = new Database.Filter.StringSearch.SearchFilter();
                            //UserWebsiteFilter.WebsiteID.SearchString = ConfigurationManager.AppSettings["MavisTireWebsiteID"];
                            //UserWebsite = ImageSolutions.User.UserWebsite.GetUserWebsite(UserWebsiteFilter);

                            if (UserWebsite == null)
                            {
                                UserWebsite = new ImageSolutions.User.UserWebsite();
                                UserWebsite.UserInfoID = UserInfo.UserInfoID;
                                UserWebsite.WebsiteID = Convert.ToString(ConfigurationManager.AppSettings["MavisTireWebsiteID"]);
                                UserWebsite.EmployeeID = _Customer.EmployeeNumber;
                                if (_Customer.HireDate != null) UserWebsite.HiredDate = _Customer.HireDate;
                                UserWebsite.OptInForNotification = true;
                                UserWebsite.CreatedBy = Convert.ToString(ConfigurationManager.AppSettings["CreatedByUserInfoID"]);

                                //if(_Customer.TerminationDate != null)
                                //{
                                //    UserWebsite.InActive = true;
                                //}
                                //else if (_Customer.DaysEmployed != null && _Customer.DaysEmployed >= 10 )
                                //{
                                //    UserWebsite.InActive = false;
                                //}
                                //else
                                //{
                                //    UserWebsite.InActive = true;
                                //}

                                UserWebsite.InActive = _Customer.PositionStatus.ToLower() != "active";

                                UserWebsite.Create(objConn, objTran);

                                blnIsInActive = UserWebsite.InActive;
                            }
                            else
                            {
                                UserWebsite.EmployeeID = _Customer.EmployeeNumber;
                                if (_Customer.HireDate != null) UserWebsite.HiredDate = _Customer.HireDate;
                                UserWebsite.OptInForNotification = true;

                                //if (_Customer.TerminationDate != null)
                                //{
                                //    UserWebsite.InActive = true;
                                //}
                                //else if (_Customer.DaysEmployed != null && _Customer.DaysEmployed >= 10)
                                //{
                                //    UserWebsite.InActive = false;
                                //}
                                //else
                                //{
                                //    UserWebsite.InActive = true;
                                //}

                                UserWebsite.InActive = _Customer.PositionStatus.ToLower() != "active";

                                if (_Customer.ResetPackage)
                                {
                                    UserWebsite.PackageAvailableDate = (DateTime?)null;
                                }

                                UserWebsite.Update(objConn, objTran);

                                blnIsInActive = UserWebsite.InActive;
                            }

                            //Store
                            if (!string.IsNullOrEmpty(_Customer.StoreNumber))
                            {
                                ImageSolutions.Custom.CustomValue CustomValue = new ImageSolutions.Custom.CustomValue();
                                ImageSolutions.Custom.CustomValueFilter CustomValueFilter = new ImageSolutions.Custom.CustomValueFilter();
                                CustomValueFilter.CustomFieldID = new Database.Filter.StringSearch.SearchFilter();
                                CustomValueFilter.CustomFieldID.SearchString = Convert.ToString(ConfigurationManager.AppSettings["MavisStoreCustomFieldID"]);
                                CustomValueFilter.UserWebsiteID = new Database.Filter.StringSearch.SearchFilter();
                                CustomValueFilter.UserWebsiteID.SearchString = UserWebsite.UserWebsiteID;
                                CustomValue = ImageSolutions.Custom.CustomValue.GetCustomValue(CustomValueFilter);

                                if (CustomValue != null && !string.IsNullOrEmpty(CustomValue.CustomValueID))
                                {
                                    CustomValue.Value = _Customer.StoreNumber;
                                    CustomValue.Update(objConn, objTran);
                                }
                                else
                                {
                                    CustomValue = new ImageSolutions.Custom.CustomValue();
                                    CustomValue.CustomFieldID = Convert.ToString(ConfigurationManager.AppSettings["MavisStoreCustomFieldID"]);
                                    CustomValue.UserWebsiteID = UserWebsite.UserWebsiteID;
                                    CustomValue.Value = _Customer.StoreNumber;
                                    CustomValue.Create(objConn, objTran);
                                }
                            }

                            //string strUniformBrand = _Customer.UniformBrand;
                            //string strTerritory = _Customer.Territory;
                            //string strPositionArea = _Customer.PositionArea;

                            string strWebsiteGroupName = string.Format("{0} - {1} {2}"
                                , _Customer.UniformBrand
                                , _Customer.Territory
                                , _Customer.PositionArea);

                            ImageSolutions.Website.WebsiteGroup WebsiteGroup = new ImageSolutions.Website.WebsiteGroup();
                            ImageSolutions.Website.WebsiteGroupFilter WebsiteGroupFilter = new ImageSolutions.Website.WebsiteGroupFilter();
                            WebsiteGroupFilter.WebsiteID = new Database.Filter.StringSearch.SearchFilter();
                            WebsiteGroupFilter.WebsiteID.SearchString = Convert.ToString(ConfigurationManager.AppSettings["MavisTireWebsiteID"]);
                            WebsiteGroupFilter.GroupName = new Database.Filter.StringSearch.SearchFilter();
                            WebsiteGroupFilter.GroupName.SearchString = strWebsiteGroupName;
                            WebsiteGroup = ImageSolutions.Website.WebsiteGroup.GetWebsiteGroup(WebsiteGroupFilter);

                            if (WebsiteGroup != null)
                            {
                                List<ImageSolutions.User.UserAccount> UserAccounts = new List<ImageSolutions.User.UserAccount>();
                                ImageSolutions.User.UserAccountFilter UserAccountFilter = new ImageSolutions.User.UserAccountFilter();
                                UserAccountFilter.UserWebsiteID = new Database.Filter.StringSearch.SearchFilter();
                                UserAccountFilter.UserWebsiteID.SearchString = UserWebsite.UserWebsiteID;
                                UserAccountFilter.AccountID = new Database.Filter.StringSearch.SearchFilter();
                                UserAccountFilter.AccountID.SearchString = Convert.ToString(ConfigurationManager.AppSettings["MavisTireAccountID"]);
                                UserAccounts = ImageSolutions.User.UserAccount.GetUserAccounts(UserAccountFilter);

                                bool blnExist = false;
                                foreach (ImageSolutions.User.UserAccount _UserAccount in UserAccounts)
                                {
                                    if (_UserAccount.WebsiteGroupID != WebsiteGroup.WebsiteGroupID)
                                    {
                                        _UserAccount.Delete(objConn, objTran);
                                    }
                                    else
                                    {
                                        blnExist = true;
                                    }
                                }

                                if (!blnExist)
                                {
                                    //Create UserAccount 
                                    ImageSolutions.User.UserAccount UserAccount = new ImageSolutions.User.UserAccount();
                                    UserAccount.UserWebsiteID = UserWebsite.UserWebsiteID;
                                    UserAccount.AccountID = Convert.ToString(ConfigurationManager.AppSettings["MavisTireAccountID"]);
                                    UserAccount.WebsiteGroupID = WebsiteGroup.WebsiteGroupID;
                                    UserAccount.CreatedBy = Convert.ToString(ConfigurationManager.AppSettings["CreatedByUserInfoID"]);
                                    UserAccount.Create(objConn, objTran);
                                }
                            }
                            else
                            {
                                throw new Exception(String.Format("Website Group not available: {0}", strWebsiteGroupName));
                            }


                            ImageSolutions.Address.AddressBook ShipToAddressBook = null;
                            if (string.IsNullOrEmpty(UserInfo.DefaultShippingAddressBookID))
                            {
                                ShipToAddressBook = new ImageSolutions.Address.AddressBook();
                            }
                            else
                            {
                                ShipToAddressBook = new ImageSolutions.Address.AddressBook(UserInfo.DefaultShippingAddressBookID);
                            }

                            ShipToAddressBook.UserInfoID = UserInfo.UserInfoID;
                            //ShipToAddressBook.AddressLabel = string.Format("{0} Store # {1}"
                            //    , string.IsNullOrEmpty(_Customer.Brand) ? _Customer.UniformBrand : _Customer.Brand
                            //    , _Customer.StoreNumber);
                            ShipToAddressBook.AddressLabel = string.Format("{0} Store # {1}"
                                , string.IsNullOrEmpty(_Customer.MailingName) ? _Customer.UniformBrand : _Customer.MailingName
                                , _Customer.StoreNumber);
                            ShipToAddressBook.FirstName = UserInfo.FirstName;
                            ShipToAddressBook.LastName = UserInfo.LastName;
                            ShipToAddressBook.AddressLine1 = _Customer.LocationAddress;
                            ShipToAddressBook.City = _Customer.LocationCity;
                            ShipToAddressBook.State = _Customer.LocationState;
                            ShipToAddressBook.PostalCode = _Customer.LocationZip;
                            ShipToAddressBook.CountryCode = "US";
                            ShipToAddressBook.PhoneNumber = string.IsNullOrEmpty(_Customer.HomePhone) ? _Customer.MobilePhone : _Customer.HomePhone;
                            if (ShipToAddressBook.IsNew)
                            {
                                ShipToAddressBook.CreatedBy = Convert.ToString(ConfigurationManager.AppSettings["CreatedByUserInfoID"]);
                                ShipToAddressBook.Create(objConn, objTran);
                            }
                            else
                            {
                                ShipToAddressBook.Update(objConn, objTran);
                            }

                            UserInfo.DefaultShippingAddressBookID = ShipToAddressBook.AddressBookID;
                            UserInfo.DefaultBillingAddressBookID = ShipToAddressBook.AddressBookID;
                            UserInfo.Update(objConn, objTran);

                            UserWebsite.AddressPermission = "Default";
                            UserWebsite.DefaultShippingAddressID = ShipToAddressBook.AddressBookID;
                            UserWebsite.DefaultBillingAddressID = ShipToAddressBook.AddressBookID;
                            UserWebsite.Update(objConn, objTran);

                            //Create UserWebsite - Credit Card 
                            ImageSolutions.User.UserWebsite CCUserWebsite = new ImageSolutions.User.UserWebsite();
                            ImageSolutions.User.UserWebsiteFilter CCUserWebsiteFilter = new ImageSolutions.User.UserWebsiteFilter();
                            CCUserWebsiteFilter.UserInfoID = new Database.Filter.StringSearch.SearchFilter();
                            CCUserWebsiteFilter.UserInfoID.SearchString = UserInfo.UserInfoID;
                            CCUserWebsiteFilter.WebsiteID = new Database.Filter.StringSearch.SearchFilter();
                            CCUserWebsiteFilter.WebsiteID.SearchString = ConfigurationManager.AppSettings["MavisTireCCWebsiteID"];
                            CCUserWebsite = ImageSolutions.User.UserWebsite.GetUserWebsite(CCUserWebsiteFilter);

                            if (CCUserWebsite == null)
                            {
                                CCUserWebsite = new ImageSolutions.User.UserWebsite();
                                CCUserWebsite.UserInfoID = UserInfo.UserInfoID;
                                CCUserWebsite.WebsiteID = Convert.ToString(ConfigurationManager.AppSettings["MavisTireCCWebsiteID"]);
                                CCUserWebsite.EmployeeID = _Customer.EmployeeNumber;
                                if (_Customer.HireDate != null) CCUserWebsite.HiredDate = _Customer.HireDate;
                                CCUserWebsite.OptInForNotification = true;
                                CCUserWebsite.CreatedBy = Convert.ToString(ConfigurationManager.AppSettings["CreatedByUserInfoID"]);

                                //if (_Customer.TerminationDate != null)
                                //{
                                //    CCUserWebsite.InActive = true;
                                //}
                                //else if (_Customer.DaysEmployed != null && _Customer.DaysEmployed >= 10)
                                //{
                                //    CCUserWebsite.InActive = false;
                                //}
                                //else
                                //{
                                //    CCUserWebsite.InActive = true;
                                //}

                                CCUserWebsite.InActive = _Customer.PositionStatus.ToLower() != "active";

                                CCUserWebsite.Create(objConn, objTran);
                            }
                            else
                            {
                                CCUserWebsite.EmployeeID = _Customer.EmployeeNumber;
                                if (_Customer.HireDate != null) CCUserWebsite.HiredDate = _Customer.HireDate;
                                CCUserWebsite.OptInForNotification = true;

                                //if (_Customer.TerminationDate != null)
                                //{
                                //    CCUserWebsite.InActive = true;
                                //}
                                //else if (_Customer.DaysEmployed != null && _Customer.DaysEmployed >= 10)
                                //{
                                //    CCUserWebsite.InActive = false;
                                //}
                                //else
                                //{
                                //    CCUserWebsite.InActive = true;
                                //}

                                CCUserWebsite.InActive = _Customer.PositionStatus.ToLower() != "active";

                                CCUserWebsite.Update(objConn, objTran);
                            }

                            ImageSolutions.Website.WebsiteGroup CCWebsiteGroup = new ImageSolutions.Website.WebsiteGroup();
                            ImageSolutions.Website.WebsiteGroupFilter CCWebsiteGroupFilter = new ImageSolutions.Website.WebsiteGroupFilter();
                            CCWebsiteGroupFilter.WebsiteID = new Database.Filter.StringSearch.SearchFilter();
                            CCWebsiteGroupFilter.WebsiteID.SearchString = Convert.ToString(ConfigurationManager.AppSettings["MavisTireCCWebsiteID"]);
                            CCWebsiteGroupFilter.GroupName = new Database.Filter.StringSearch.SearchFilter();
                            CCWebsiteGroupFilter.GroupName.SearchString = strWebsiteGroupName;
                            CCWebsiteGroup = ImageSolutions.Website.WebsiteGroup.GetWebsiteGroup(CCWebsiteGroupFilter);

                            if (CCWebsiteGroup != null)
                            {
                                List<ImageSolutions.User.UserAccount> CCUserAccounts = new List<ImageSolutions.User.UserAccount>();
                                ImageSolutions.User.UserAccountFilter CCUserAccountFilter = new ImageSolutions.User.UserAccountFilter();
                                CCUserAccountFilter.UserWebsiteID = new Database.Filter.StringSearch.SearchFilter();
                                CCUserAccountFilter.UserWebsiteID.SearchString = CCUserWebsite.UserWebsiteID;
                                CCUserAccountFilter.AccountID = new Database.Filter.StringSearch.SearchFilter();
                                CCUserAccountFilter.AccountID.SearchString = Convert.ToString(ConfigurationManager.AppSettings["MavisTireCCAccountID"]);
                                CCUserAccounts = ImageSolutions.User.UserAccount.GetUserAccounts(CCUserAccountFilter);

                                bool blnExist = false;
                                foreach (ImageSolutions.User.UserAccount _UserAccount in CCUserAccounts)
                                {
                                    if (_UserAccount.WebsiteGroupID != CCWebsiteGroup.WebsiteGroupID)
                                    {
                                        _UserAccount.Delete(objConn, objTran);
                                    }
                                    else
                                    {
                                        blnExist = true;
                                    }
                                }

                                if (!blnExist)
                                {
                                    //Create UserAccount 
                                    ImageSolutions.User.UserAccount CCUserAccount = new ImageSolutions.User.UserAccount();
                                    CCUserAccount.UserWebsiteID = CCUserWebsite.UserWebsiteID;
                                    CCUserAccount.AccountID = Convert.ToString(ConfigurationManager.AppSettings["MavisTireCCAccountID"]);
                                    CCUserAccount.WebsiteGroupID = CCWebsiteGroup.WebsiteGroupID;
                                    CCUserAccount.CreatedBy = Convert.ToString(ConfigurationManager.AppSettings["CreatedByUserInfoID"]);
                                    CCUserAccount.Create(objConn, objTran);
                                }
                            }
                            else
                            {
                                throw new Exception(String.Format("Website Group not available: {0}", strWebsiteGroupName));
                            }

                            _Customer.ResetPackage = false;
                            _Customer.IsUpdated = false;
                            _Customer.ErrorMessage = String.Empty;
                            _Customer.Update(objConn, objTran);
                        }
                        else
                        {
                            if (_Customer.PositionStatus.ToLower() != "active")
                            {
                                ImageSolutions.User.UserWebsite UserWebsite = new ImageSolutions.User.UserWebsite();
                                ImageSolutions.User.UserWebsiteFilter UserWebsiteFilter = new ImageSolutions.User.UserWebsiteFilter();
                                UserWebsiteFilter.EmployeeID = new Database.Filter.StringSearch.SearchFilter();
                                UserWebsiteFilter.EmployeeID.SearchString = _Customer.EmployeeNumber;
                                UserWebsiteFilter.WebsiteID = new Database.Filter.StringSearch.SearchFilter();
                                UserWebsiteFilter.WebsiteID.SearchString = ConfigurationManager.AppSettings["MavisTireWebsiteID"];
                                UserWebsite = ImageSolutions.User.UserWebsite.GetUserWebsite(UserWebsiteFilter);

                                if (UserWebsite != null)
                                {
                                    UserWebsite.InActive = true;
                                    UserWebsite.Update(objConn, objTran);
                                }

                                ImageSolutions.User.UserWebsite CCUserWebsite = new ImageSolutions.User.UserWebsite();
                                ImageSolutions.User.UserWebsiteFilter CCUserWebsiteFilter = new ImageSolutions.User.UserWebsiteFilter();
                                CCUserWebsiteFilter.EmployeeID = new Database.Filter.StringSearch.SearchFilter();
                                CCUserWebsiteFilter.EmployeeID.SearchString = _Customer.EmployeeNumber;
                                CCUserWebsiteFilter.WebsiteID = new Database.Filter.StringSearch.SearchFilter();
                                CCUserWebsiteFilter.WebsiteID.SearchString = ConfigurationManager.AppSettings["MavisTireCCWebsiteID"];
                                CCUserWebsite = ImageSolutions.User.UserWebsite.GetUserWebsite(CCUserWebsiteFilter);

                                if (CCUserWebsite != null)
                                {
                                    CCUserWebsite.InActive = true;
                                    CCUserWebsite.Update(objConn, objTran);
                                }

                                _Customer.ResetPackage = false;
                                _Customer.IsUpdated = false;
                                _Customer.ErrorMessage = String.Empty;
                                _Customer.Update(objConn, objTran);

                                blnIsInActive = true;
                            }
                            else
                            {
                                throw new Exception("Missing Email");
                            }
                        }                        

                        objTran.Commit();

                        strEmail = "steve@imageinc.com";
                        strFirstName = "Steve";

                        if (!blnIsInActive)
                        {
                            //strFirstName = "Steve";
                            //strEmail = "steve@imageinc.com";

                            if (_Customer.SendNewHireEmail)
                            {
                                //Send Email
                                string strBody = string.Empty;

                                strBody = string.Format(@"
<!DOCTYPE html>
<html lang=""en"">

<head>
<meta http-equiv=Content-Type content=""text/html; charset=windows-1252"">
<meta name=Generator content=""Microsoft Word 15 (filtered)"">
<style>
<!--
/* Font Definitions */
@font-face
{{font-family:Wingdings;
panose-1:5 0 0 0 0 0 0 0 0 0;}}
@font-face
{{font-family:""Cambria Math"";
panose-1:2 4 5 3 5 4 6 3 2 4;}}
@font-face
{{font-family:Aptos;}}
/* Style Definitions */
p.MsoNormal, li.MsoNormal, div.MsoNormal
{{margin-top:0in;
margin-right:0in;
margin-bottom:8.0pt;
margin-left:0in;
line-height:115%;
font-size:12.0pt;
font-family:""Aptos"",sans-serif;}}

a:link, span.MsoHyperlink
{{color:#0563C1;
text-decoration:underline;}}
p.MsoListParagraph, li.MsoListParagraph, div.MsoListParagraph
{{margin-top:0in;
margin-right:0in;
margin-bottom:8.0pt;
margin-left:.5in;
line-height:115%;
font-size:12.0pt;
font-family:""Aptos"",sans-serif;}}
p.MsoListParagraphCxSpFirst, li.MsoListParagraphCxSpFirst, div.MsoListParagraphCxSpFirst
{{margin-top:0in;
margin-right:0in;
margin-bottom:0in;
margin-left:.5in;
line-height:115%;
font-size:12.0pt;
font-family:""Aptos"",sans-serif;}}
p.MsoListParagraphCxSpMiddle, li.MsoListParagraphCxSpMiddle, div.MsoListParagraphCxSpMiddle
{{margin-top:0in;
margin-right:0in;
margin-bottom:0in;
margin-left:.5in;
line-height:115%;
font-size:12.0pt;
font-family:""Aptos"",sans-serif;}}
p.MsoListParagraphCxSpLast, li.MsoListParagraphCxSpLast, div.MsoListParagraphCxSpLast
{{margin-top:0in;
margin-right:0in;
margin-bottom:8.0pt;
margin-left:.5in;
line-height:115%;
font-size:12.0pt;
font-family:""Aptos"",sans-serif;}}
.MsoChpDefault
{{font-size:12.0pt;
font-family:""Aptos"",sans-serif;}}
.MsoPapDefault
{{margin-bottom:8.0pt;
line-height:115%;}}
@page WordSection1
{{size:8.5in 11.0in;
margin:1.0in 1.0in 1.0in 1.0in;}}
div.WordSection1
{{page:WordSection1;}}
/* List Definitions */
ol
{{margin-bottom:0in;}}

ul
{{margin-bottom:0in;}}
-->
</style>
</head>

<body style='word-wrap:break-word'>

<div>

<p align=""center"">
<img height=125 src=""https://portal.imageinc.com/assets/company/mavis/2024_Mavis_Tire_Logo.png"">
</p>

<p align=""center""><b>Welcome to the Mavis Gear E-Store!</b></p>

<p>&nbsp;</p>

<p>Hello, {0}!</p>
<br />

<p>Welcome to the Mavis Team! Your <b>Mavis Uniform Allotment</b> is now available and ready to use.</p>

<br />
<p class=MsoNormal><b>What Should You Do Today? </b></p>

<p class=MsoListParagraphCxSpFirst style='text-indent:-.25in'>
<b>1.<span style='font:7.0pt ""Times New Roman""'>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; </span></b>
<b>Log in</b> to the <b> Mavis Gear E-Store </b> website via the Company Intranet on the store computer. 
</p>

<p class=MsoListParagraphCxSpMiddle style='margin-top:0in;margin-right:0in;margin-bottom:0in;margin-left:.75in;text-indent:-.25in;line-height:normal'>
<span style='font-family:Symbol'> <span style='font:7.0pt ""Times New Roman""'>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; </span></span>
<b>• Username</b>: Use your personal email address listed in ADP. 
</p>

<p class=MsoListParagraphCxSpMiddle style='margin-top:0in;margin-right:0in;margin-bottom:0in;margin-left:.75in;text-indent:-.25in;line-height:normal'>
<span style='font-family:Symbol'> <span style='font:7.0pt ""Times New Roman""'>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; </span></span>
<b>• Password:</b> Use the one-time initial password: <b>Mavis$1</b>
</p>

<p class=MsoListParagraphCxSpLast style='margin-top:0in;margin-right:0in;margin-bottom:0in;margin-left:1.0in;text-indent:-.25in;line-height:normal'>
<span style='font:7.0pt ""Times New Roman""'>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; </span>
<i>- Then create a new secure password on your first login. </i>
</p>

<p class=MsoNormal style='margin-bottom:0in;line-height:normal'><i>&nbsp;</i></p>

<p class=MsoListParagraphCxSpFirst style='margin-top:0in;margin-right:0in;margin-bottom:0in;text-indent:-.25in;line-height:normal'>
<b>2.<span style='font:7.0pt ""Times New Roman""'>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; </span></b>
<b>Select </b>your preferred uniform package. 
</p>

<p class=MsoListParagraphCxSpMiddle style='margin-bottom:0in;line-height:normal'>&nbsp;</p>

<p class=MsoListParagraphCxSpLast style='margin-bottom:0in;text-indent:-.25in;line-height:normal'>
<b>3.<span style='font:7.0pt ""Times New Roman""'>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; </span></b>
<b>Submit </b>your Order. </p>

<p class=MsoNormal style='margin-bottom:0in;line-height:normal'>&nbsp;</p>

<p class=MsoNormal style='margin-bottom:0in;line-height:normal'>Orders should be placed by Team Members during working hours. </p>

<br />
<p align=""center"">Thank you for being a valued member of the Mavis Team!</p>
<br />

<p align=""center"">
Image Solutions Customer Service is available at 
<a href=""mailto:cs@imageinc.com"">cs@imageinc.com</a> 
or (800)805-3090, Monday - Friday, 9:00 am - 6:00 pm EST.
</p>

</div>

</body>
</html>
", strFirstName);
                                SendEmail(strEmail, string.Format("Your Annual Mavis Uniform Allotment is Available!"), strBody).Wait();

                                _Customer.SendNewHireEmail = false;
                                _Customer.Update();
                            }

                            if (_Customer.SendRegionChangeEmail)
                            {
                                //Send Email
                                string strBody = string.Empty;

                                strBody = string.Format(@"
<!DOCTYPE html>
<html lang=""en"">

<head>
<meta http-equiv=Content-Type content=""text/html; charset=windows-1252"">
<meta name=Generator content=""Microsoft Word 15 (filtered)"">
<style>
<!--
 /* Font Definitions */
 @font-face
	{{font-family:Wingdings;
    panose-1:5 0 0 0 0 0 0 0 0 0;}}
@font-face
	{{font-family:""Cambria Math"";
	panose-1:2 4 5 3 5 4 6 3 2 4;}}
@font-face
	{{font-family:Aptos;}}
 /* Style Definitions */
 p.MsoNormal, li.MsoNormal, div.MsoNormal
	{{margin-top:0in;
	margin-right:0in;
	margin-bottom:8.0pt;
	margin-left:0in;
	line-height:115%;
	font-size:12.0pt;
	font-family:""Aptos"",sans-serif;}}

a:link, span.MsoHyperlink
	{{color:#0563C1;
	text-decoration:underline;}}
p.MsoListParagraph, li.MsoListParagraph, div.MsoListParagraph
	{{margin-top:0in;
	margin-right:0in;
	margin-bottom:8.0pt;
	margin-left:.5in;
	line-height:115%;
	font-size:12.0pt;
	font-family:""Aptos"",sans-serif;}}
p.MsoListParagraphCxSpFirst, li.MsoListParagraphCxSpFirst, div.MsoListParagraphCxSpFirst
	{{margin-top:0in;
	margin-right:0in;
	margin-bottom:0in;
	margin-left:.5in;
	line-height:115%;
	font-size:12.0pt;
	font-family:""Aptos"",sans-serif;}}
p.MsoListParagraphCxSpMiddle, li.MsoListParagraphCxSpMiddle, div.MsoListParagraphCxSpMiddle
	{{margin-top:0in;
	margin-right:0in;
	margin-bottom:0in;
	margin-left:.5in;
	line-height:115%;
	font-size:12.0pt;
	font-family:""Aptos"",sans-serif;}}
p.MsoListParagraphCxSpLast, li.MsoListParagraphCxSpLast, div.MsoListParagraphCxSpLast
	{{margin-top:0in;
	margin-right:0in;
	margin-bottom:8.0pt;
	margin-left:.5in;
	line-height:115%;
	font-size:12.0pt;
	font-family:""Aptos"",sans-serif;}}
.MsoChpDefault
	{{font-size:12.0pt;
	font-family:""Aptos"",sans-serif;}}
.MsoPapDefault
	{{margin-bottom:8.0pt;
	line-height:115%;}}
@page WordSection1
	{{size:8.5in 11.0in;
	margin:1.0in 1.0in 1.0in 1.0in;}}
div.WordSection1
	{{page:WordSection1;}}
 /* List Definitions */
 ol
	{{margin-bottom:0in;}}

ul
	{{margin-bottom:0in;}}
-->
</style>
</head>

<body style='word-wrap:break-word'>

<div>

<p align=""center"">
<img height=125 src=""https://portal.imageinc.com/assets/company/mavis/2024_Mavis_Tire_Logo.png"">
</p>

<p align=""center""><b>Welcome to the Mavis Gear E-Store!</b></p>

<p>&nbsp;</p>

<p>Hello, {0}!</p>
<br />

<p>Good News! With your recent change in geography, your <b>Mavis Uniform Allotment </b>has been reset and is now available for you to use. </p>

<br />
<p class=MsoNormal><b>What Should You Do Today?</b></p>

<p class=MsoListParagraphCxSpFirst style='text-indent:-.25in'>
<b>1.<span style='font:7.0pt ""Times New Roman""'>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; </span></b>
<b>Log in</b> to the <b> Mavis Gear E-Store </b> website via the Company Intranet on the store computer. 
</p>

<p class=MsoListParagraphCxSpMiddle style='margin-top:0in;margin-right:0in;margin-bottom:0in;margin-left:.75in;text-indent:-.25in;line-height:normal'>
<span style='font-family:Symbol'> <span style='font:7.0pt ""Times New Roman""'>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</span></span>
<b>• Username</b>: Use your personal email address listed in ADP. 
</p>

<p class=MsoListParagraphCxSpMiddle style='margin-top:0in;margin-right:0in; margin-bottom:0in;margin-left:.75in;text-indent:-.25in;line-height:normal'>
<span style='font-family:Symbol'> <span style='font:7.0pt ""Times New Roman""'>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</span></span>
<b>• Password:</b> Use the password you created during the initial login. <i>(If you forgot your password, password reset is an option.) </i>
</p>

<p class=MsoListParagraphCxSpMiddle style='margin-top:0in;margin-right:0in;margin-bottom:0in;margin-left:.75in;line-height:normal'><i>&nbsp;</i></p>

<p class=MsoListParagraphCxSpMiddle style='margin-top:0in;margin-right:0in;margin-bottom:0in;text-indent:-.25in;line-height:normal'>
<b><i>2.<span style='font:7.0pt ""Times New Roman""'>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; </span></i></b>
<b>Select </b>your preferred uniform package. 
</p>

<p class=MsoListParagraphCxSpMiddle style='margin-top:0in;margin-right:0in;margin-bottom:0in;margin-left:.75in;line-height:normal'><b>&nbsp;</b></p>

<p class=MsoListParagraphCxSpLast style='margin-top:0in;margin-right:0in;margin-bottom:0in;text-indent:-.25in;line-height:normal'>
<b><i>3.<span style='font:7.0pt ""Times New Roman""'>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; </span></i></b>
<b>Submit </b>your Order. 
</p>

<p class=MsoNormal style='margin-bottom:0in;line-height:normal'>&nbsp;</p>

<p class=MsoNormal style='margin-bottom:0in;line-height:normal'>Orders should be placed by Team Members during working hours. </p>

<br />
<p align=""center"">Thank you for being a valued member of the Mavis Team!</p>
<br />

<p align=""center"">
Image Solutions Customer Service is available at 
<a href=""mailto:cs@imageinc.com"">cs@imageinc.com</a> 
or (800)805-3090, Monday - Friday, 9:00 am - 6:00 pm EST.
</p>

</div>

</body>
</html>
", strFirstName);
                                SendEmail(strEmail, string.Format("Your New Mavis Uniform Allotment is Available!"), strBody).Wait();

                                _Customer.SendRegionChangeEmail = false;
                                _Customer.Update();
                            }

                            if (_Customer.SendPositionChangeEmail)
                            {
                                //Send Email
                                string strBody = string.Empty;

                                strBody = string.Format(@"
<!DOCTYPE html>
<html lang=""en"">

<head>
<meta http-equiv=Content-Type content=""text/html; charset=windows-1252"">
<meta name=Generator content=""Microsoft Word 15 (filtered)"">
<style>
<!--
 /* Font Definitions */
 @font-face
	{{font-family:Wingdings;
    panose-1:5 0 0 0 0 0 0 0 0 0;}}
@font-face
	{{font-family:""Cambria Math"";
	panose-1:2 4 5 3 5 4 6 3 2 4;}}
@font-face
	{{font-family:Aptos;}}
 /* Style Definitions */
 p.MsoNormal, li.MsoNormal, div.MsoNormal
	{{margin-top:0in;
	margin-right:0in;
	margin-bottom:8.0pt;
	margin-left:0in;
	line-height:115%;
	font-size:12.0pt;
	font-family:""Aptos"",sans-serif;}}

a:link, span.MsoHyperlink
	{{color:#0563C1;
	text-decoration:underline;}}
p.MsoListParagraph, li.MsoListParagraph, div.MsoListParagraph
	{{margin-top:0in;
	margin-right:0in;
	margin-bottom:8.0pt;
	margin-left:.5in;
	line-height:115%;
	font-size:12.0pt;
	font-family:""Aptos"",sans-serif;}}
p.MsoListParagraphCxSpFirst, li.MsoListParagraphCxSpFirst, div.MsoListParagraphCxSpFirst
	{{margin-top:0in;
	margin-right:0in;
	margin-bottom:0in;
	margin-left:.5in;
	line-height:115%;
	font-size:12.0pt;
	font-family:""Aptos"",sans-serif;}}
p.MsoListParagraphCxSpMiddle, li.MsoListParagraphCxSpMiddle, div.MsoListParagraphCxSpMiddle
	{{margin-top:0in;
	margin-right:0in;
	margin-bottom:0in;
	margin-left:.5in;
	line-height:115%;
	font-size:12.0pt;
	font-family:""Aptos"",sans-serif;}}
p.MsoListParagraphCxSpLast, li.MsoListParagraphCxSpLast, div.MsoListParagraphCxSpLast
	{{margin-top:0in;
	margin-right:0in;
	margin-bottom:8.0pt;
	margin-left:.5in;
	line-height:115%;
	font-size:12.0pt;
	font-family:""Aptos"",sans-serif;}}
.MsoChpDefault
	{{font-size:12.0pt;
	font-family:""Aptos"",sans-serif;}}
.MsoPapDefault
	{{margin-bottom:8.0pt;
	line-height:115%;}}
@page WordSection1
	{{size:8.5in 11.0in;
	margin:1.0in 1.0in 1.0in 1.0in;}}
div.WordSection1
	{{page:WordSection1;}}
 /* List Definitions */
 ol
	{{margin-bottom:0in;}}

ul
	{{margin-bottom:0in;}}
-->
</style>
</head>

<body style='word-wrap:break-word'>

<div>

<p align=""center"">
<img height=125 src=""https://portal.imageinc.com/assets/company/mavis/2024_Mavis_Tire_Logo.png"">
</p>

<p align=""center""><b>Welcome to the Mavis Gear E-Store!</b></p>

<p>&nbsp;</p>

<p>Hello, {0}!</p>
<br />

<p>Good News! With your recent change in position, your <b>Mavis Uniform Allotment </b>has been reset and is now available for you to use. </p>

<br />
<p class=MsoNormal><b>What Should You Do Today?</b></p>

<p class=MsoListParagraphCxSpFirst style='text-indent:-.25in'>
<b>1.<span style='font:7.0pt ""Times New Roman""'>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; </span></b>
<b>Log in</b> to the <b> Mavis Gear E-Store </b> website via the Company Intranet on the store computer. 
</p>

<p class=MsoListParagraphCxSpMiddle style='margin-top:0in;margin-right:0in;margin-bottom:0in;margin-left:.75in;text-indent:-.25in;line-height:normal'>
<span style='font-family:Symbol'> <span style='font:7.0pt ""Times New Roman""'>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</span></span>
<b>• Username</b>: Use your personal email address listed in ADP. 
</p>

<p class=MsoListParagraphCxSpMiddle style='margin-top:0in;margin-right:0in; margin-bottom:0in;margin-left:.75in;text-indent:-.25in;line-height:normal'>
<span style='font-family:Symbol'> <span style='font:7.0pt ""Times New Roman""'>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</span></span>
<b>• Password:</b> Use the password you created during the initial login. <i>(If you forgot your password, password reset is an option.) </i>
</p>

<p class=MsoListParagraphCxSpMiddle style='margin-top:0in;margin-right:0in;margin-bottom:0in;margin-left:.75in;line-height:normal'><i>&nbsp;</i></p>

<p class=MsoListParagraphCxSpMiddle style='margin-top:0in;margin-right:0in;margin-bottom:0in;text-indent:-.25in;line-height:normal'>
<b><i>2.<span style='font:7.0pt ""Times New Roman""'>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; </span></i></b>
<b>Select </b>your preferred uniform package. 
</p>

<p class=MsoListParagraphCxSpMiddle style='margin-top:0in;margin-right:0in;margin-bottom:0in;margin-left:.75in;line-height:normal'><b>&nbsp;</b></p>

<p class=MsoListParagraphCxSpLast style='margin-top:0in;margin-right:0in;margin-bottom:0in;text-indent:-.25in;line-height:normal'>
<b><i>3.<span style='font:7.0pt ""Times New Roman""'>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; </span></i>
</b><b>Submit </b>your Order. 
</p>

<p class=MsoNormal style='margin-bottom:0in;line-height:normal'>&nbsp;</p>

<p class=MsoNormal style='margin-bottom:0in;line-height:normal'>Orders should be placed by Team Members during working hours. </p>

<br />
<p align=""center"">Thank you for being a valued member of the Mavis Team!</p>
<br />

<p align=""center"">
Image Solutions Customer Service is available at 
<a href=""mailto:cs@imageinc.com"">cs@imageinc.com</a> 
or (800)805-3090, Monday - Friday, 9:00 am - 6:00 pm EST.
</p>

</div>

</body>
</html>
", strFirstName);
                                SendEmail(strEmail, string.Format("Your New Mavis Uniform Allotment is Available!"), strBody).Wait();

                                _Customer.SendPositionChangeEmail = false;
                                _Customer.Update();
                            }
                        }
                        
                    }
                    catch (Exception ex)
                    {
                        if (objTran != null && objTran.Connection != null)
                        {
                            objTran.Rollback();
                        }

                        _Customer.ErrorMessage = ex.Message;
                        _Customer.Update();
                    }
                    finally
                    {
                        if (objConn != null) objConn.Dispose();
                        objConn = null;

                        if (objTran != null) objTran.Dispose();
                        objTran = null;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                Customers = null;
                CustomerFilter = null;
            }
            return true;
        }

        static async Task<Response> SendEmail(string toemail, string subject, string htmlcontent, List<string> ccs = null)
        {
            try
            {
                if (String.IsNullOrEmpty(Convert.ToString(ConfigurationManager.AppSettings["Environment"]))
                    || Convert.ToString(ConfigurationManager.AppSettings["Environment"]).ToLower() != "production"
                )
                {
                    toemail = string.IsNullOrEmpty(Convert.ToString(ConfigurationManager.AppSettings["DevelopmentEmail"])) ? "steve@imageinc.com" : Convert.ToString(ConfigurationManager.AppSettings["DevelopmentEmail"]);
                }

                string strAPIKey = Convert.ToString(ConfigurationManager.AppSettings["SendGridApiKey"]);
                SendGridClient Client = new SendGridClient(strAPIKey);
                EmailAddress FromEmailAddress = new EmailAddress(Convert.ToString(ConfigurationManager.AppSettings["EmailFrom"]));
                EmailAddress ToEmailAddress = new EmailAddress(toemail);
                SendGridMessage SendGridMessage = MailHelper.CreateSingleEmail(FromEmailAddress, ToEmailAddress, subject, null, htmlcontent);

                if (ccs != null)
                {
                    foreach (string _cc in ccs)
                        SendGridMessage.AddCc(_cc);
                }

                return await Client.SendEmailAsync(SendGridMessage);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {

            }
        }
    }
}
