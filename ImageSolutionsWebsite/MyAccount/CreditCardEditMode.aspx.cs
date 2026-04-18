using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ImageSolutionsWebsite.MyAccount
{
    public partial class CreditCardEditMode : BasePageUserAccountAuth
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
                    return "/Admin/UserOverview.aspx";
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
                if (Request.UrlReferrer != null) ReturnURL = Request.UrlReferrer.PathAndQuery;

                InitializePage();
            }
        }
        public void InitializePage()
        {

            try
            {
                BindExpirationMonth();
                BindExpirationYear();

                BindCountry();

                if (!_CreditCard.IsNew)
                {
                    txtNickname.Text = _CreditCard.Nickname;
                    txtFullName.Text = _CreditCard.FullName;
                    txtCreditCardNumber.Text = _CreditCard.LastFourDigit;
                    txtCreditCardType.Text = _CreditCard.CreditCardType;
                    ddlExpirationYear.SelectedValue = Convert.ToString(_CreditCard.ExpirationDate.Year);
                    ddlExpirationMonth.SelectedValue = Convert.ToString(_CreditCard.ExpirationDate.Month);
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
                        this.ddlCountry.Text = _CreditCard.BillingAddressBook.CountryCode;
                    }

                    pnlCVV.Visible = false;
                    btnSave.Visible = false;
                }
                else
                {
                    btnSave.Text = "Create";
                    btnDelete.Visible = false;
                }
                aCancel.HRef = ReturnURL;
            }
            catch (Exception ex)
            {
                WebUtility.DisplayJavascriptMessage(this, ex.Message);
            }
            finally { }
        }

        protected void BindCountry()
        {
            try
            {
                List<ImageSolutions.Address.AddressCountryCode> AddressCountryCodes = new List<ImageSolutions.Address.AddressCountryCode>();
                AddressCountryCodes = ImageSolutions.Address.AddressCountryCode.GetAddressCountryCodes();

                ddlCountry.DataSource = AddressCountryCodes;
                ddlCountry.DataBind();
                ddlCountry.Items.Insert(0, new ListItem(String.Empty, string.Empty));
            }
            catch (Exception ex)
            {
                throw ex;
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

            for (int year = intYear; year <= intYear + 15; year++)
            {
                ddlExpirationYear.Items.Add(new ListItem(year.ToString(), year.ToString()));
            }
        }
        protected void btnSave_Click(object sender, EventArgs e)
        {
            bool blnReturn = false;

            try
            {
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
                _CreditCard.BillingAddressBook.CountryCode = ddlCountry.SelectedValue;
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

                    Stripe.Source StripeSource = StripeAPI.SaveSource(
                        _CreditCard.PayerExternalID
                        , txtCreditCardNumber.Text
                        , Convert.ToInt32(ddlExpirationMonth.SelectedValue)
                        , Convert.ToInt32(ddlExpirationYear.SelectedValue)
                        , _CreditCard.CVV
                        , _CreditCard.FullName
                        , _CreditCard.BillingAddressBook);

                    _CreditCard.CardExternalID = StripeSource.Id;

                    _CreditCard.CreatedBy = CurrentUser.UserInfoID;
                    blnReturn = _CreditCard.Create();

                    ImageSolutions.User.UserCreditCard UserCreditCard = new ImageSolutions.User.UserCreditCard();
                    UserCreditCard.UserInfoID = CurrentUser.UserInfoID;
                    UserCreditCard.CreditCardID = _CreditCard.CreditCardID;
                    UserCreditCard.CreatedBy = CurrentUser.UserInfoID;
                    UserCreditCard.Create();
                }
                else
                {
                    blnReturn = _CreditCard.Update();
                }
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
    }
}