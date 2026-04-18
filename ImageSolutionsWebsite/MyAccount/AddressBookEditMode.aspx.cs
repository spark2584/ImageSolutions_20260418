using ImageSolutions.Website;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ImageSolutionsWebsite.MyAccount
{
    public partial class AddressBookEditMode : BasePage
    {
        protected string mAddressBookID = string.Empty;

        private ImageSolutions.Address.AddressBook mAddressBook = null;
        protected ImageSolutions.Address.AddressBook AddressBook
        {
            get
            {
                if (mAddressBook == null)
                {
                    if (string.IsNullOrEmpty(mAddressBookID))
                        mAddressBook = new ImageSolutions.Address.AddressBook();
                    else
                        mAddressBook = new ImageSolutions.Address.AddressBook(mAddressBookID);
                }
                return mAddressBook;
            }
            set
            {
                mAddressBook = value;
            }
        }

        protected string ReturnURL
        {
            get
            {
                if (ViewState["ReturnURL"] == null)
                    return "/myaccount/dashboard.aspx";
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
            mAddressBookID = Request.QueryString.Get("AddressBookID");

            if (!Page.IsPostBack)
            {
                if (Request.UrlReferrer != null) ReturnURL = Request.UrlReferrer.PathAndQuery;

                InitializePage();
            }
        }

        protected void InitializePage()
        {
            try
            {
                BindCountry();

                if (!AddressBook.IsNew)
                {
                    this.txtFirstName.Text = AddressBook.FirstName;
                    this.txtLastName.Text = AddressBook.LastName;
                    this.txtAddressLabel.Text = AddressBook.AddressLabel;
                    this.txtAddress.Text = AddressBook.AddressLine1;
                    this.txtAddress2.Text = AddressBook.AddressLine2;
                    this.txtCity.Text = AddressBook.City;
                    this.txtState.Text = AddressBook.State;
                    this.txtZip.Text = AddressBook.PostalCode;
                    this.ddlCountry.SelectedIndex = !string.IsNullOrEmpty(AddressBook.CountryCode) ? this.ddlCountry.Items.IndexOf(this.ddlCountry.Items.FindByValue(AddressBook.CountryCode))
                        : this.ddlCountry.Items.IndexOf(this.ddlCountry.Items.FindByValue("US"));
                    this.txtPhone.Text = AddressBook.PhoneNumber;

                    this.cbDefaultShipping.Checked = CurrentUser.DefaultShippingAddressBookID == AddressBook.AddressBookID;
                    this.cbDefaultBilling.Checked = CurrentUser.DefaultBillingAddressBookID == AddressBook.AddressBookID;

                    btnSave.Text = "Save";
                }
                else
                {
                    this.ddlCountry.SelectedIndex = this.ddlCountry.Items.IndexOf(this.ddlCountry.Items.FindByValue("US"));
                    btnSave.Text = "Create";
                    btnDelete.Visible = false;
                }
                aCancel.HRef = ReturnURL;

                cbSkipAddressValidation.Visible = !CurrentWebsite.DisableAddressValidation;
            }
            catch (Exception ex)
            {
                WebUtility.DisplayJavascriptMessage(this, ex.Message);
            }
            finally
            {
            }
        }
        protected void BindCountry()
        {
            try
            {
                List<ImageSolutions.Address.AddressCountryCode> AddressCountryCodes = new List<ImageSolutions.Address.AddressCountryCode>();
                AddressCountryCodes = ImageSolutions.Address.AddressCountryCode.GetAddressCountryCodes();

                //List<ImageSolutions.Address.AddressCountryCode> FilterAddressCountryCodes = new List<ImageSolutions.Address.AddressCountryCode>();
                //if (CurrentWebsite.WebsiteCountries.Count > 0)
                //{
                //    if (CurrentWebsite.WebsiteCountries.FindAll(x => !Convert.ToBoolean(x.Exclude)).Count > 0)
                //    {
                //        foreach (ImageSolutions.Website.WebsiteCountry _WebsiteCountry in CurrentWebsite.WebsiteCountries.FindAll(x => !Convert.ToBoolean(x.Exclude)).ToList())
                //        {
                //            FilterAddressCountryCodes.Add(AddressCountryCodes.Find(x => x.Alpha2Code == _WebsiteCountry.CountryCode));
                //        }
                //    }

                //    FilterAddressCountryCodes = FilterAddressCountryCodes != null && FilterAddressCountryCodes.Count > 0 ? FilterAddressCountryCodes : AddressCountryCodes;

                //    if (CurrentWebsite.WebsiteCountries.FindAll(x => Convert.ToBoolean(x.Exclude)).Count > 0)
                //    {
                //        foreach (ImageSolutions.Website.WebsiteCountry _WebsiteCountry in CurrentWebsite.WebsiteCountries.FindAll(x => Convert.ToBoolean(x.Exclude)).ToList())
                //        {
                //            FilterAddressCountryCodes.Remove(FilterAddressCountryCodes.Find(x => x.Alpha2Code == _WebsiteCountry.CountryCode));
                //        }
                //    }
                //}

                //ddlCountry.DataSource = FilterAddressCountryCodes != null && FilterAddressCountryCodes.Count > 0 ? FilterAddressCountryCodes : AddressCountryCodes;
                ddlCountry.DataSource = AddressCountryCodes;
                ddlCountry.DataBind();
                ddlCountry.Items.Insert(0, new ListItem(String.Empty, string.Empty));
            }
            catch (Exception ex)
            {
                WebUtility.DisplayJavascriptMessage(this, ex.Message);
            }
            finally { }
        }
        protected void btnSave_Click(object sender, EventArgs e)
        {
            bool blnReturn = false;

            try
            {
                if ((ddlCountry.SelectedValue == "US" || ddlCountry.SelectedValue == "CA") && string.IsNullOrEmpty(txtState.Text))
                {
                    throw new Exception("State required");
                }

                //if (Page.IsValid && (cbSkipAddressValidation.Checked || AddressBook.CountryCode != "US" || ValidateAddress()))
                if (Page.IsValid && (CurrentWebsite.DisableAddressValidation || cbSkipAddressValidation.Checked || ValidateAddress()))
                {
                    //AddressBook.AddressLabel = txtFirstName.Text + " " + txtLastName.Text;
                    AddressBook.AddressLabel = txtAddressLabel.Text;
                    AddressBook.FirstName = txtFirstName.Text;
                    AddressBook.LastName = txtLastName.Text;
                    AddressBook.AddressLine1 = txtAddress.Text;
                    AddressBook.AddressLine2 = txtAddress2.Text;
                    AddressBook.City = txtCity.Text;
                    AddressBook.State = txtState.Text;
                    AddressBook.PostalCode = txtZip.Text;
                    AddressBook.CountryCode = Convert.ToString(ddlCountry.SelectedValue);
                    AddressBook.PhoneNumber = txtPhone.Text;
                    AddressBook.UserInfoID = CurrentUser.UserInfoID;

                    if (AddressBook.IsNew)
                    {
                        AddressBook.CreatedBy = CurrentUser.UserInfoID;
                        blnReturn = AddressBook.Create();
                    }
                    else
                    {
                        blnReturn = AddressBook.Update();
                    }

                    if (cbDefaultShipping.Checked)
                    {
                        CurrentUser.DefaultShippingAddressBookID = AddressBook.AddressBookID;
                        CurrentUser.Update();
                    }
                    else
                    {
                        if (CurrentUser.DefaultShippingAddressBookID == AddressBook.AddressBookID)
                        {
                            CurrentUser.DefaultShippingAddressBookID = null;
                            CurrentUser.Update();
                        }
                    }
                    if (cbDefaultBilling.Checked)
                    {
                        CurrentUser.DefaultBillingAddressBookID = AddressBook.AddressBookID;
                        CurrentUser.Update();
                    }
                    else
                    {
                        if (CurrentUser.DefaultBillingAddressBookID == AddressBook.AddressBookID)
                        {
                            CurrentUser.DefaultBillingAddressBookID = null;
                            CurrentUser.Update();
                        }
                    }

                    Response.Redirect(string.Format("/myaccount/AddressBook.aspx"));
                }
            }
            catch(Exception ex)
            {
                WebUtility.DisplayJavascriptMessage(this, ex.Message);
            }
            
        }

        protected bool ValidateAddress()
        {
            bool _ret = false;

            ImageSolutions.Address.AddressTrans objAddressTrans = new ImageSolutions.Address.AddressTrans();
            objAddressTrans.FirstName = txtFirstName.Text;
            objAddressTrans.LastName = txtLastName.Text;
            objAddressTrans.AddressLabel = txtAddressLabel.Text;
            objAddressTrans.AddressLine1 = txtAddress.Text;
            objAddressTrans.AddressLine2 = txtAddress2.Text;
            objAddressTrans.City = txtCity.Text;
            objAddressTrans.State = txtState.Text;
            objAddressTrans.PostalCode = txtZip.Text;
            objAddressTrans.CountryCode = ddlCountry.SelectedValue;
            //objAddressTrans.CountryName = ddlCountry.SelectedValue;

            Shippo.Address ShippoAddress = ImageSolutions.Shipping.ShippoShipping.GetAddress(objAddressTrans);

            if (Convert.ToBoolean(ShippoAddress.ValidationResults.IsValid))
            {
                if(txtAddress.Text != Convert.ToString(ShippoAddress.Street1)
                    || txtAddress2.Text != Convert.ToString(ShippoAddress.Street2)
                    || txtCity.Text != Convert.ToString(ShippoAddress.City)
                    || txtState.Text != Convert.ToString(ShippoAddress.State)
                    || txtZip.Text != Convert.ToString(ShippoAddress.Zip)  
                )
                {
                    txtAddress.Text = Convert.ToString(ShippoAddress.Street1);
                     txtAddress2.Text = Convert.ToString(ShippoAddress.Street2);
                    txtCity.Text = Convert.ToString(ShippoAddress.City);
                    txtState.Text = Convert.ToString(ShippoAddress.State);
                    txtZip.Text = Convert.ToString(ShippoAddress.Zip);

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
                //pnlSkipAddressValidation.Visible = true;                
                //this.Page.ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('Invalid Address')", true);
            }
            pnlSkipAddressValidation.Visible = true;

            return _ret;
        }

        protected void btnDelete_Click(object sender, EventArgs e)
        {
            bool blnReturn = false;

            try
            {
                blnReturn = AddressBook.Delete();
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
    }
}