using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ImageSolutionsWebsite.Admin
{
    public partial class CreditCard : BasePageAdminUserWebSiteAuth
    {
        protected string mCreditCardID = string.Empty;
        private ImageSolutions.CreditCard.CreditCard mCreditCard = null;
        protected ImageSolutions.CreditCard.CreditCard _CreditCard
        {
            get
            {
                if (mCreditCard == null)
                {
                    if (string.IsNullOrEmpty(mCreditCardID))
                        mCreditCard = new ImageSolutions.CreditCard.CreditCard();
                    else
                        mCreditCard = new ImageSolutions.CreditCard.CreditCard(mCreditCardID);
                }
                return mCreditCard;
            }
            set
            {
                mCreditCard = value;
            }
        }
        protected string ReturnURL
        {
            get
            {
                if (ViewState["ReturnURL"] == null)
                    return "/Admin/CreditCardOverview.aspx";
                else
                    return ViewState["ReturnURL"].ToString();
            }
            set
            {
                ViewState["ReturnURL"] = value;
            }
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            mCreditCardID = Request.QueryString.Get("id");

            if (!Page.IsPostBack)
            {
                //if (Request.UrlReferrer != null) ReturnURL = Request.UrlReferrer.PathAndQuery;

                InitializePage();
            }
        }
        public void InitializePage()
        {
            try
            {
                InitializeTabs(top_1_tab, top_1, top_2_tab, top_2, null, null, null, null);

                BindExpirationMonth();
                BindExpirationYear();
                BindCountry();

                if (!_CreditCard.IsNew)
                {
                    txtNickname.Text = _CreditCard.Nickname;
                    txtFullName.Text = _CreditCard.FullName;
                    txtCreditCardNumber.Text = "XXXXXXXXXXXX" + _CreditCard.LastFourDigit;
                    txtCreditCardType.Text = _CreditCard.CreditCardType;
                    ddlExpirationYear.SelectedValue = Convert.ToString(_CreditCard.ExpirationDate.Year);
                    ddlExpirationMonth.SelectedValue = _CreditCard.ExpirationDate.Month < 10 ? "0" + Convert.ToString(_CreditCard.ExpirationDate.Month) : Convert.ToString(_CreditCard.ExpirationDate.Month);
                    txtCVV.Text = _CreditCard.CVV;

                    txtNickname.Enabled = false;
                    txtFullName.Enabled = false;
                    txtCreditCardNumber.Enabled = false;
                    txtCreditCardType.Enabled = false;
                    ddlExpirationYear.Enabled = false;
                    ddlExpirationMonth.Enabled = false;

                    txtFirstName.Enabled = false;
                    txtLastName.Enabled = false;
                    txtAddress.Enabled = false;
                    txtAddress2.Enabled = false;
                    txtCity.Enabled = false;
                    txtState.Enabled = false;
                    txtZip.Enabled = false;
                    ddlCountry.Enabled = false;

                    if (_CreditCard.BillingAddressBook != null)
                    {
                        this.txtFirstName.Text = _CreditCard.BillingAddressBook.FirstName;
                        this.txtLastName.Text = _CreditCard.BillingAddressBook.LastName;
                        this.txtAddress.Text = _CreditCard.BillingAddressBook.AddressLine1;
                        this.txtAddress2.Text = _CreditCard.BillingAddressBook.AddressLine2;
                        this.txtCity.Text = _CreditCard.BillingAddressBook.City;
                        this.txtState.Text = _CreditCard.BillingAddressBook.State;
                        this.txtZip.Text = _CreditCard.BillingAddressBook.PostalCode;
                        this.ddlCountry.SelectedIndex = !string.IsNullOrEmpty(_CreditCard.BillingAddressBook.CountryCode) ? this.ddlCountry.Items.IndexOf(this.ddlCountry.Items.FindByValue(_CreditCard.BillingAddressBook.CountryCode)) : this.ddlCountry.Items.IndexOf(this.ddlCountry.Items.FindByValue("US"));
                    }

                    BindUserCreditCard();

                    pnlCVV.Visible = false;
                    btnSave.Text = "Save";
                    btnSave.Visible = false;
                }
                else
                {
                    btnSave.Text = "Create";
                    btnDelete.Visible = false;
                    top_2_tab.Visible = false;
                }
                aCancel.HRef = ReturnURL;

                btnImport.Visible = !string.IsNullOrEmpty(mCreditCardID);
            }
            catch (Exception ex)
            {
                WebUtility.DisplayJavascriptMessage(this, ex.Message);
            }
            finally { }
        }

        protected void BindExpirationMonth()
        {
            for (int month = 1; month <= 12; month++)
            {
                ddlExpirationMonth.Items.Add(new ListItem(month.ToString().PadLeft(2, '0'), month.ToString().PadLeft(2, '0')));
            }
        }

        protected void BindExpirationYear()
        {
            int intYear = DateTime.UtcNow.Year;

            for (int year = intYear; year <= intYear+20; year++)
            {
                ddlExpirationYear.Items.Add(new ListItem(year.ToString(), year.ToString()));
            }
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
                throw ex;
            }
            finally { }
        }
        protected void BindUserCreditCard()
        {
            int intTotalRecord = 0;

            try
            {
                List<ImageSolutions.User.UserCreditCard> UserCreditCards = new List<ImageSolutions.User.UserCreditCard>();
                ImageSolutions.User.UserCreditCardFilter UserCreditCardFilter  = new ImageSolutions.User.UserCreditCardFilter();
                UserCreditCardFilter.CreditCardID = new Database.Filter.StringSearch.SearchFilter();
                UserCreditCardFilter.CreditCardID.SearchString = _CreditCard.CreditCardID;
                UserCreditCards = ImageSolutions.User.UserCreditCard.GetUserCreditCards(UserCreditCardFilter, ucUserCreditCardPager.PageSize, ucUserCreditCardPager.CurrentPageNumber, out intTotalRecord);

                //gvUserCreditCard.DataSource = _CreditCard.UserCreditCards;
                gvUserCreditCard.DataSource = UserCreditCards;
                gvUserCreditCard.DataBind();

                ucUserCreditCardPager.TotalRecord = intTotalRecord;
            }
            catch (Exception ex)
            {
                WebUtility.DisplayJavascriptMessage(this, ex.Message);
            }
            finally { }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            SqlConnection objConn = null;
            SqlTransaction objTran = null;
            ImageSolutions.User.UserCreditCard UserCreditCard = null;
            bool blnReturn = false;

            try
            {
                objConn = new SqlConnection(Database.DefaultConnectionString);
                objConn.Open();
                objTran = objConn.BeginTransaction();

                _CreditCard.Nickname = txtNickname.Text;
                _CreditCard.FullName = txtFullName.Text;
                _CreditCard.LastFourDigit = txtCreditCardNumber.Text.Substring(txtCreditCardNumber.Text.Length - 4);
                _CreditCard.CreditCardType = txtCreditCardType.Text;
                _CreditCard.ExpirationDate = Convert.ToDateTime(String.Format("{0}/1/{1}", Convert.ToString(ddlExpirationMonth.SelectedValue), Convert.ToString(ddlExpirationYear.SelectedValue)));
                _CreditCard.CVV = txtCVV.Text;
                _CreditCard.CreatedBy = CurrentUser.UserInfoID;

                if (_CreditCard.BillingAddressBook == null) _CreditCard.BillingAddressBook = new ImageSolutions.Address.AddressBook();
                _CreditCard.BillingAddressBook.AddressLabel = txtFirstName.Text + " " + txtLastName.Text;
                _CreditCard.BillingAddressBook.FirstName = txtFirstName.Text;
                _CreditCard.BillingAddressBook.LastName = txtLastName.Text;
                _CreditCard.BillingAddressBook.AddressLine1 = txtAddress.Text;
                _CreditCard.BillingAddressBook.AddressLine2 = txtAddress2.Text;
                _CreditCard.BillingAddressBook.City = txtCity.Text;
                _CreditCard.BillingAddressBook.State = txtState.Text;
                _CreditCard.BillingAddressBook.PostalCode = txtZip.Text;
                _CreditCard.BillingAddressBook.CreatedBy = CurrentUser.UserInfoID;

                if (_CreditCard.IsNew)
                {
                    Guid Guid = new Guid();
                    _CreditCard.GUID = Convert.ToString(Guid.NewGuid());
                    _CreditCard.Data = Encrypt(txtCreditCardNumber.Text, _CreditCard.GUID);

                    ImageSolutions.StripeAPI.StripeAPI StripeAPI = new ImageSolutions.StripeAPI.StripeAPI();
                    Stripe.Customer StripeCustomer = StripeAPI.SaveCustomer(
                        _CreditCard.FullName
                        , _CreditCard.BillingAddressBook.AddressLine1
                        , _CreditCard.BillingAddressBook.AddressLine2
                        , _CreditCard.BillingAddressBook.City
                        , _CreditCard.BillingAddressBook.State
                        , _CreditCard.BillingAddressBook.PostalCode
                        , _CreditCard.BillingAddressBook.CountryCode);

                    _CreditCard.PayerExternalID = StripeCustomer.Id;

                    Stripe.Source StripeSource =  StripeAPI.SaveSource(
                        _CreditCard.PayerExternalID
                        , txtCreditCardNumber.Text
                        , Convert.ToInt32(ddlExpirationMonth.SelectedValue)
                        , Convert.ToInt32(ddlExpirationYear.SelectedValue)
                        , _CreditCard.CVV
                        , _CreditCard.FullName
                        , _CreditCard.BillingAddressBook);

                    _CreditCard.CardExternalID = StripeSource.Id;

                    _CreditCard.CreatedBy = CurrentUser.UserInfoID;
                    _CreditCard.Create(objConn, objTran);

                    UserCreditCard = new ImageSolutions.User.UserCreditCard();
                    UserCreditCard.UserInfoID = CurrentUser.UserInfoID;
                    UserCreditCard.CreditCardID = _CreditCard.CreditCardID;
                    UserCreditCard.CreatedBy = CurrentUser.UserInfoID;
                    blnReturn = UserCreditCard.Create(objConn, objTran);
                }
                else
                {
                    blnReturn = _CreditCard.Update(objConn, objTran);
                }
                objTran.Commit();
            }
            catch (Exception ex)
            {
                objTran.Rollback();
                WebUtility.DisplayJavascriptMessage(this, ex.Message);
            }
            finally
            {
                if (objTran != null) objTran.Dispose();
                objTran = null;
                if (objConn != null) objConn.Dispose();
                objConn = null;
            }

            if (blnReturn)
            {
                Response.Redirect(ReturnURL);
            }
        }

        protected void btnDelete_Click(object sender, EventArgs e)
        {
            bool blnReturn = false;

            try
            {
                blnReturn = _CreditCard.Delete();
            }
            catch (Exception ex)
            {
                WebUtility.DisplayJavascriptMessage(this, ex.Message);
            }
            finally { }

            if (blnReturn)
            {
                Response.Redirect(ReturnURL);
            }
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

        protected void txtCreditCardNumber_TextChanged(object sender, EventArgs e)
        {
            txtCreditCardType.Text = GetCardType(txtCreditCardNumber.Text);
        }
        protected void btnImport_Click(object sender, EventArgs e)
        {
            Response.Redirect(string.Format("/Admin/usercreditcardimport.aspx?creditcardid={0}", mCreditCardID));
        }

        protected void btnExport_Click(object sender, EventArgs e)
        {
            //System.Data.OleDb.OleDbConnection OleDbConnection = null;
            //System.Data.OleDb.OleDbCommand OleDbCommand = null;
            //string strSQL = string.Empty;
            //Hashtable dicParam = null;
            //try
            //{
            //    string strPath = Server.MapPath("\\Export\\UserCreditCard\\" + DateTime.UtcNow.ToString("yyyyMM") + "\\");
            //    if (!Directory.Exists(strPath))
            //    {
            //        Directory.CreateDirectory(strPath);
            //    }

            //    string strTemplateFileName = "UserCreditCardTemplate.xlsx";
            //    string strTemplateFilePath = Server.MapPath(string.Format("\\Import\\Template\\{0}", strTemplateFileName));
            //    string strFileExportPath = Server.MapPath(string.Format("\\Export\\UserCreditCard\\{0}\\CreditCardUser_{1}.xlsx", DateTime.UtcNow.ToString("yyyyMM"), DateTime.UtcNow.ToString("yyyyMMddHHmmssffff")));
            //    File.Copy(strTemplateFilePath, strFileExportPath, true);

            //    OleDbConnection = new System.Data.OleDb.OleDbConnection("Provider=Microsoft.ACE.OLEDB.12.0;Data Source='" + strFileExportPath + "';Extended Properties=Excel 12.0;");
            //    OleDbConnection.Open();
            //    OleDbCommand = new System.Data.OleDb.OleDbCommand();
            //    OleDbCommand.Connection = OleDbConnection;

            //    foreach(ImageSolutions.User.UserCreditCard _UserCreditCard in _CreditCard.UserCreditCards)
            //    {
            //        dicParam = new Hashtable();
            //        dicParam["id"] = _UserCreditCard.UserCreditCardID;
            //        dicParam["email"] = _UserCreditCard.UserInfo.EmailAddress;
            //        dicParam["reset_day_of_the_month"] = _UserCreditCard.ResetDayOfTheMonth;
            //        dicParam["set_limit"] = _UserCreditCard.Limit;
            //        dicParam["active"] = "yes";

            //        strSQL = Database.GetInsertSQL(dicParam, "[credit_card_users$]", false);
            //        OleDbCommand.CommandText = strSQL;
            //        OleDbCommand.ExecuteNonQuery();
            //    }
            //    OleDbConnection.Close();


            //    Response.ContentType = "application/vnd.ms-excel";
            //    Response.AppendHeader("Content-Disposition", "attachment; filename=" + System.IO.Path.GetFileName(strFileExportPath));
            //    Response.WriteFile(strFileExportPath);
            //    Response.Flush();
            //    Response.End();
            //}
            //catch(Exception ex)
            //{
            //    WebUtility.DisplayJavascriptMessage(this, ex.Message);
            //}

            string strSQL = string.Empty;
            Hashtable dicParam = null;

            List<ImageSolutions.User.UserCreditCard> UserCreditCards = null;
            ImageSolutions.User.UserCreditCardFilter UserCreditCardFilter = null;
            try
            {
                string strPath = Server.MapPath("\\Export\\UserCreditCard\\" + DateTime.UtcNow.ToString("yyyyMM") + "\\");
                if (!Directory.Exists(strPath))
                {
                    Directory.CreateDirectory(strPath);
                }

                string strFileExportPath = Server.MapPath(string.Format("\\Export\\UserCreditCard\\{0}\\UserCreditCard_{1}.csv", DateTime.UtcNow.ToString("yyyyMM"), DateTime.UtcNow.ToString("yyyyMMddHHmmssffff")));
                CreateExportCSV(strFileExportPath);

                Response.ContentType = "text/csv";
                Response.AppendHeader("Content-Disposition", "attachment; filename=" + System.IO.Path.GetFileName(strFileExportPath));
                Response.WriteFile(strFileExportPath);

                HttpContext.Current.Response.Flush();
                HttpContext.Current.Response.SuppressContent = true;
                HttpContext.Current.ApplicationInstance.CompleteRequest();
            }
            catch (Exception ex)
            {
                WebUtility.DisplayJavascriptMessage(this, ex.Message);
            }
        }
        public void CreateExportCSV(string filepath)
        {
            SqlDataReader objRead = null;
            string strDBSQL = string.Empty;

            StringBuilder objReturn = new StringBuilder();

            int intCount = 0;

            try
            {
                //Header
                objReturn.Append(string.Format("{0},{1},{2},{3},{4}"
                    , "id"
                    , "email"
                    , "reset_day_of_the_month"
                    , "set_limit"
                    , "active"));
                objReturn.AppendLine();

                strDBSQL = string.Format(@"

SELECT ucc.UserCreditCardID as id
	, u.EmailAddress as email
	, ucc.ResetDayOfTheMonth as reset_day_of_the_month
	, ucc.Limit as set_limit
	, 'yes' as active
FROM UserCreditCard ucc
Inner Join UserInfo u on u.UserInfoID = ucc.UserInfoID
WHERE ucc.CreditCardID = {0} "
                    , Database.HandleQuote(mCreditCardID));


                objRead = Database.GetDataReader(strDBSQL);

                while (objRead.Read())
                {
                    objReturn.Append(string.Format("{0},{1},{2},{3},{4}"
                        , Convert.ToString(objRead["id"])
                        , Convert.ToString(objRead["email"])
                        , Convert.ToString(objRead["reset_day_of_the_month"])
                        , Convert.ToString(objRead["set_limit"])
                        , Convert.ToString(objRead["active"])
                    ) );
                    objReturn.AppendLine();
                }

                if (objReturn != null)
                {
                    using (StreamWriter _streamwriter = new StreamWriter(filepath))
                    {
                        _streamwriter.Write(objReturn.ToString());
                    }
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
        }
        public string Encrypt(string value, string encryptionKey)
        {
            string encryptValue = string.Empty;

            byte[] clearBytes = Encoding.Unicode.GetBytes(value);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(encryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(clearBytes, 0, clearBytes.Length);
                        cs.Close();
                    }
                    encryptValue = Convert.ToBase64String(ms.ToArray());
                }
            }
            return encryptValue;
        }

        protected void ucUserCreditCardPager_PostBackPageIndexChanging(object sender, EventArgs e)
        {
            BindUserCreditCard();

            top_2_tab.Attributes.Remove("class");
            top_2_tab.Attributes.Add("class", "nav-link active");
            top_2.Attributes.Remove("class");
            top_2.Attributes.Add("class", "tab-pane fade show active");

            top_1_tab.Attributes.Remove("class");
            top_1_tab.Attributes.Add("class", "nav-link");
            top_1.Attributes.Remove("class");
            top_1.Attributes.Add("class", "tab-pane fade");
        }
    }
}