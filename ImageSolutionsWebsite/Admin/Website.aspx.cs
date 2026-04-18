using Amazon.S3;
using Amazon.S3.Transfer;
using ImageSolutions.Website;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ImageSolutionsWebsite.Admin
{
    public partial class Website : BasePageAdminUserWebSiteAuth
    {
        protected string ReturnURL
        {
            get
            {
                if (ViewState["ReturnURL"] == null)
                    return "/Admin/WebsiteOverview.aspx";
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
                //BindUserWebsite();
                BindWebsiteGroup();
                BindPaymentTerm();
                BindItemPersonalizationApproved();

                txtName.Text = CurrentUser.CurrentUserWebSite.WebSite.Name;
                txtDomain.Text = CurrentUser.CurrentUserWebSite.WebSite.Domain;
                txtAbbreviation.Text = CurrentUser.CurrentUserWebSite.WebSite.Abbreviation;
                chkUserRegistration.Checked = CurrentUser.CurrentUserWebSite.WebSite.UserRegistration;
                chkUserRegistrationKeyRequired.Checked = CurrentUser.CurrentUserWebSite.WebSite.UserRegistrationKeyRequired;
                chkUserApprovalRequired.Checked = CurrentUser.CurrentUserWebSite.WebSite.UserApprovalRequired;
                chkAccountRegistration.Checked = CurrentUser.CurrentUserWebSite.WebSite.AccountRegistration;
                chkAccountRegistrationKeyRequired.Checked = CurrentUser.CurrentUserWebSite.WebSite.AccountRegistrationKeyRequired;
                chkAccountApprovalRequired.Checked = CurrentUser.CurrentUserWebSite.WebSite.AccountApprovalRequired;
                this.ddlWebsiteGroup.SelectedIndex = this.ddlWebsiteGroup.Items.IndexOf(this.ddlWebsiteGroup.Items.FindByValue(CurrentUser.CurrentUserWebSite.WebSite.DefaultWebsiteGroupID));
                chkShowAvailableInventory.Checked = CurrentUser.CurrentUserWebSite.WebSite.ShowAvailableInventory;
                chkShowSalesDescription.Checked = CurrentUser.CurrentUserWebSite.WebSite.ShowSalesDescription;
                chkShowDetailedDescription.Checked = CurrentUser.CurrentUserWebSite.WebSite.ShowDetailedDescription;
                chkOrderApprovalRequired.Checked = CurrentUser.CurrentUserWebSite.WebSite.OrderApprovalRequired;
                chkEnableEmployeeCredit.Checked = CurrentUser.CurrentUserWebSite.WebSite.EnableEmployeeCredit;
                chkMustUseExistingEmployeeCredit.Checked = CurrentUser.CurrentUserWebSite.WebSite.MustUseExistingEmployeeCredit;
                chkEnableCreditCard.Checked = CurrentUser.CurrentUserWebSite.WebSite.EnableCreditCard;
                chkEnablePaymentTerm.Checked = CurrentUser.CurrentUserWebSite.WebSite.EnablePaymentTerm;
                chkEnablePromoCode.Checked = CurrentUser.CurrentUserWebSite.WebSite.EnablePromoCode;
                chkDisplaySubCategory.Checked = CurrentUser.CurrentUserWebSite.WebSite.DisplaySubCategory;
                chkDisplayAttributeFilter.Checked = CurrentUser.CurrentUserWebSite.WebSite.DisplayAttributeFilter;

                chkDisplayLeftNavigation.Checked = CurrentUser.CurrentUserWebSite.WebSite.DisplayLeftNavigation;
                chkDisplayTariffCharge.Checked = CurrentUser.CurrentUserWebSite.WebSite.DisplayTariffCharge;

                chkDisplayDefaultGroupPerAccount.Checked = CurrentUser.CurrentUserWebSite.WebSite.DisplayDefaultGroupPerAccount;
                chkCombineWebsiteGroup.Checked = CurrentUser.CurrentUserWebSite.WebSite.CombineWebsiteGroup;
                ddlPaymentTerm.SelectedIndex = this.ddlPaymentTerm.Items.IndexOf(this.ddlPaymentTerm.Items.FindByValue(CurrentUser.CurrentUserWebSite.WebSite.DefaultPaymentTermID));
                this.ddlItemDisplayType.SelectedIndex = this.ddlItemDisplayType.Items.IndexOf(this.ddlItemDisplayType.Items.FindByValue(CurrentUser.CurrentUserWebSite.WebSite.ItemDisplayType));
                this.ddlProductDisplayType.SelectedIndex = this.ddlProductDisplayType.Items.IndexOf(this.ddlProductDisplayType.Items.FindByValue(CurrentUser.CurrentUserWebSite.WebSite.ProductDetailDisplayType));
                imgLogo.ImageUrl = CurrentUser.CurrentUserWebSite.WebSite.LogoPath;
                imgBanner.ImageUrl = CurrentUser.CurrentUserWebSite.WebSite.BannerPath;
                imgEmailLogo.ImageUrl = CurrentUser.CurrentUserWebSite.WebSite.EmailLogoPath;
                imgSizeChart.ImageUrl = CurrentUser.CurrentUserWebSite.WebSite.DefaultSizeChartPath;

                chkAllowNameChange.Checked = CurrentUser.CurrentUserWebSite.WebSite.AllowNameChange;
                chkEnablePasswordReset.Checked = CurrentUser.CurrentUserWebSite.WebSite.EnablePasswordReset;

                chkDisallowBackOrder.Checked = CurrentUser.CurrentUserWebSite.WebSite.DisallowBackOrder;
                chkAllowBackOrderForAllItems.Checked = CurrentUser.CurrentUserWebSite.WebSite.AllowBackOrderForAllItems;

                chkHideEmail.Checked = CurrentUser.CurrentUserWebSite.WebSite.HideEmail;
                chkEnableZendesk.Checked = CurrentUser.CurrentUserWebSite.WebSite.EnableZendesk;

                chkHideMyAccountOrderReport.Checked = CurrentUser.CurrentUserWebSite.WebSite.HideMyAccountOrderReport;
                chkHideMyAccountOrderApproval.Checked = CurrentUser.CurrentUserWebSite.WebSite.HideMyAccountOrderApproval;
                chkHideAdminOrderApproval.Checked = CurrentUser.CurrentUserWebSite.WebSite.HideAdminOrderApproval;

                chkIsPunchOut.Checked = CurrentUser.CurrentUserWebSite.WebSite.IsPunchout;

                ddlDisplayUserPermission.SelectedValue = CurrentUser.CurrentUserWebSite.WebSite.DisplayUserPermission;

                txtBudgetAlias.Text = CurrentUser.CurrentUserWebSite.WebSite.BudgetAlias;
                txtOverBudgetMessage.Text = CurrentUser.CurrentUserWebSite.WebSite.OverBudgetMessage;

                txtCreditCardLimitPerOrder.Text = Convert.ToString(CurrentUser.CurrentUserWebSite.WebSite.CreditCardLimitPerOrder);

                chkIsOneBudgetPerUser.Checked = CurrentUser.CurrentUserWebSite.WebSite.IsOneBudgetPerUser;

                chkEnableEmailOptIn.Checked = CurrentUser.CurrentUserWebSite.WebSite.EnableEmailOptIn;

                chkEnableSMSOptIn.Checked = CurrentUser.CurrentUserWebSite.WebSite.EnableSMSOptIn;

                chkEnableLocalize.Checked = CurrentUser.CurrentUserWebSite.WebSite.EnableLocalize;

                chkDisplayNewAccountSetupForm.Checked = CurrentUser.CurrentUserWebSite.WebSite.DisplayNewAccountSetupForm;

                chkSuggestedSelling.Checked = CurrentWebsite.SuggestedSelling;

                txtDiscountPerItem.Text = Convert.ToString(CurrentUser.CurrentUserWebSite.WebSite.DiscountPerItem);

                txtBannerHTML.Text = Convert.ToString(CurrentUser.CurrentUserWebSite.WebSite.BannerHTML);
                txtFeaturedProductHTML.Text = Convert.ToString(CurrentUser.CurrentUserWebSite.WebSite.FeaturedProductHTML);

                InitializeImage();
                InitializeBannerImage();
                InitializeEmailLogoImage();
                InitializeSizeChartImage();
                BindWebsiteTab();

                BindWebsiteCountry();

                aCancel.HRef = ReturnURL;
            }
            catch (Exception ex)
            {
                WebUtility.DisplayJavascriptMessage(this, ex.Message);
            }
            finally { }
        }

        protected void InitializeImage()
        {
            btnResetImage.Visible = !string.IsNullOrEmpty(imgLogo.ImageUrl) && imgLogo.ImageUrl != CurrentUser.CurrentUserWebSite.WebSite.LogoPath;
            btnRemoveImage.Visible = !string.IsNullOrEmpty(imgLogo.ImageUrl) && imgLogo.ImageUrl == CurrentUser.CurrentUserWebSite.WebSite.LogoPath;
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            bool blnReturn = false;

            try
            {
                CurrentUser.CurrentUserWebSite.WebSite.Name = txtName.Text;
                CurrentUser.CurrentUserWebSite.WebSite.Domain = txtDomain.Text;

                if(txtAbbreviation.Text.Length > 2)
                {
                    throw new Exception("Abbreviation cannot be more than 2 characters long");
                }

                CurrentUser.CurrentUserWebSite.WebSite.Abbreviation = txtAbbreviation.Text;
                CurrentUser.CurrentUserWebSite.WebSite.UserRegistration = chkUserRegistration.Checked;
                CurrentUser.CurrentUserWebSite.WebSite.UserRegistrationKeyRequired = chkUserRegistrationKeyRequired.Checked;
                CurrentUser.CurrentUserWebSite.WebSite.UserApprovalRequired = chkUserApprovalRequired.Checked;
                CurrentUser.CurrentUserWebSite.WebSite.AccountRegistration = chkAccountRegistration.Checked;
                CurrentUser.CurrentUserWebSite.WebSite.AccountRegistrationKeyRequired = chkAccountRegistrationKeyRequired.Checked;
                CurrentUser.CurrentUserWebSite.WebSite.AccountApprovalRequired = chkAccountApprovalRequired.Checked;
                CurrentUser.CurrentUserWebSite.WebSite.DefaultWebsiteGroupID = this.ddlWebsiteGroup.SelectedValue;
                CurrentUser.CurrentUserWebSite.WebSite.ShowAvailableInventory = chkShowAvailableInventory.Checked;
                CurrentUser.CurrentUserWebSite.WebSite.ShowSalesDescription = chkShowSalesDescription.Checked;
                CurrentUser.CurrentUserWebSite.WebSite.ShowDetailedDescription = chkShowDetailedDescription.Checked;
                CurrentUser.CurrentUserWebSite.WebSite.OrderApprovalRequired = chkOrderApprovalRequired.Checked;
                CurrentUser.CurrentUserWebSite.WebSite.EnableEmployeeCredit = chkEnableEmployeeCredit.Checked;
                CurrentUser.CurrentUserWebSite.WebSite.EnableCreditCard = chkEnableCreditCard.Checked;
                CurrentUser.CurrentUserWebSite.WebSite.MustUseExistingEmployeeCredit = chkMustUseExistingEmployeeCredit.Checked;
                CurrentUser.CurrentUserWebSite.WebSite.EnablePaymentTerm = chkEnablePaymentTerm.Checked;
                CurrentUser.CurrentUserWebSite.WebSite.EnablePromoCode = chkEnablePromoCode.Checked;
                CurrentUser.CurrentUserWebSite.WebSite.DefaultPaymentTermID = ddlPaymentTerm.SelectedValue;
                CurrentUser.CurrentUserWebSite.WebSite.DisplayDefaultGroupPerAccount = chkDisplayDefaultGroupPerAccount.Checked;
                CurrentUser.CurrentUserWebSite.WebSite.DisplaySubCategory = chkDisplaySubCategory.Checked;
                CurrentUser.CurrentUserWebSite.WebSite.DisplayAttributeFilter = chkDisplayAttributeFilter.Checked;
                CurrentUser.CurrentUserWebSite.WebSite.DisplayLeftNavigation = chkDisplayLeftNavigation.Checked;
                CurrentUser.CurrentUserWebSite.WebSite.CombineWebsiteGroup = chkCombineWebsiteGroup.Checked;
                CurrentUser.CurrentUserWebSite.WebSite.ItemDisplayType = ddlItemDisplayType.SelectedValue;
                CurrentUser.CurrentUserWebSite.WebSite.ProductDetailDisplayType = ddlProductDisplayType.SelectedValue;
                CurrentUser.CurrentUserWebSite.WebSite.LogoPath = Convert.ToString(imgLogo.ImageUrl);
                CurrentUser.CurrentUserWebSite.WebSite.EmailLogoPath = Convert.ToString(imgEmailLogo.ImageUrl);
                CurrentUser.CurrentUserWebSite.WebSite.BannerPath = Convert.ToString(imgBanner.ImageUrl);
                CurrentUser.CurrentUserWebSite.WebSite.DefaultSizeChartPath = Convert.ToString(imgSizeChart.ImageUrl);
                CurrentUser.CurrentUserWebSite.WebSite.AllowNameChange = chkAllowNameChange.Checked;
                CurrentUser.CurrentUserWebSite.WebSite.EnablePasswordReset = chkEnablePasswordReset.Checked;
                CurrentUser.CurrentUserWebSite.WebSite.DisallowBackOrder = chkDisallowBackOrder.Checked;
                CurrentUser.CurrentUserWebSite.WebSite.AllowBackOrderForAllItems = chkAllowBackOrderForAllItems.Checked;
                CurrentUser.CurrentUserWebSite.WebSite.HideEmail = chkHideEmail.Checked;
                CurrentUser.CurrentUserWebSite.WebSite.EnableZendesk = chkEnableZendesk.Checked;
                CurrentUser.CurrentUserWebSite.WebSite.HideMyAccountOrderReport = chkHideMyAccountOrderReport.Checked;
                CurrentUser.CurrentUserWebSite.WebSite.HideMyAccountOrderApproval = chkHideMyAccountOrderApproval.Checked;
                CurrentUser.CurrentUserWebSite.WebSite.HideAdminOrderApproval = chkHideAdminOrderApproval.Checked;
                CurrentUser.CurrentUserWebSite.WebSite.IsPunchout = chkIsPunchOut.Checked;
                CurrentUser.CurrentUserWebSite.WebSite.DisplayUserPermission = ddlDisplayUserPermission.SelectedValue;
                CurrentUser.CurrentUserWebSite.WebSite.DisplayTariffCharge = chkDisplayTariffCharge.Checked;

                CurrentUser.CurrentUserWebSite.WebSite.BudgetAlias = txtBudgetAlias.Text;
                CurrentUser.CurrentUserWebSite.WebSite.OverBudgetMessage = txtOverBudgetMessage.Text;

                CurrentUser.CurrentUserWebSite.WebSite.CreditCardLimitPerOrder = !string.IsNullOrEmpty(txtCreditCardLimitPerOrder.Text) ? Convert.ToDecimal(txtCreditCardLimitPerOrder.Text) : 0;

                CurrentUser.CurrentUserWebSite.WebSite.IsOneBudgetPerUser = chkIsOneBudgetPerUser.Checked;
                
                CurrentUser.CurrentUserWebSite.WebSite.EnableEmailOptIn = chkEnableEmailOptIn.Checked;
                CurrentUser.CurrentUserWebSite.WebSite.EnableSMSOptIn = chkEnableSMSOptIn.Checked;

                CurrentUser.CurrentUserWebSite.WebSite.EnableLocalize = chkEnableLocalize.Checked;

                CurrentUser.CurrentUserWebSite.WebSite.DisplayNewAccountSetupForm = chkDisplayNewAccountSetupForm.Checked;

                CurrentUser.CurrentUserWebSite.WebSite.SuggestedSelling = chkSuggestedSelling.Checked;

                CurrentUser.CurrentUserWebSite.WebSite.DiscountPerItem = !string.IsNullOrEmpty(txtDiscountPerItem.Text) ? Convert.ToDecimal(txtDiscountPerItem.Text) : 0;

                CurrentUser.CurrentUserWebSite.WebSite.BannerHTML = txtBannerHTML.Text;
                CurrentUser.CurrentUserWebSite.WebSite.FeaturedProductHTML = txtFeaturedProductHTML.Text;

                CurrentUser.CurrentUserWebSite.WebSite.Update();
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

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect("/admin/websiteoverview.aspx");
        }

        protected void BindUserWebsite()
        {
            try
            {
                gvUserWebsite.DataSource = CurrentUser.CurrentUserWebSite.WebSite.UserWebsites;
                gvUserWebsite.DataBind();
            }
            catch (Exception ex)
            {
                WebUtility.DisplayJavascriptMessage(this, ex.Message);
            }
            finally { }
        }

        protected void gvUserWebsite_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "LineRemove")
            {
                try
                {
                    ImageSolutions.User.UserWebsite UserWebsite = new ImageSolutions.User.UserWebsite(Convert.ToString(e.CommandArgument));
                    UserWebsite.Delete();

                    BindUserWebsite();
                }
                catch (Exception ex)
                {
                    WebUtility.DisplayJavascriptMessage(this, ex.Message);
                }
                finally { }
            }
        }

        protected void BindWebsiteGroup()
        {
            try
            {
                this.ddlWebsiteGroup.DataSource = CurrentUser.CurrentUserWebSite.WebSite.WebsiteGroups;
                this.ddlWebsiteGroup.DataBind();
                this.ddlWebsiteGroup.Items.Insert(0, new ListItem(String.Empty, string.Empty));

            }
            catch (Exception ex)
            {
                WebUtility.DisplayJavascriptMessage(this, ex.Message);
            }
            finally { }
        }

        protected void BindPaymentTerm()
        {
            try
            {
                this.ddlPaymentTerm.DataSource = ImageSolutions.Payment.PaymentTerm.GetPaymentTerms();
                this.ddlPaymentTerm.DataBind();
                this.ddlPaymentTerm.Items.Insert(0, new ListItem(String.Empty, string.Empty));
            }
            catch (Exception ex)
            {
                WebUtility.DisplayJavascriptMessage(this, ex.Message);
            }
            finally { }
        }

        protected void BindItemPersonalizationApproved()
        {
            List<ImageSolutions.Item.ItemPersonalizationValueApproved> ItemPersonalizationValueApproveds = null;
            ImageSolutions.Item.ItemPersonalizationValueApprovedFilter ItemPersonalizationValueApprovedFilter = null;
            int intTotalRecord = 0;

            try
            {
                ItemPersonalizationValueApprovedFilter = new ImageSolutions.Item.ItemPersonalizationValueApprovedFilter();
                ItemPersonalizationValueApprovedFilter.WebsiteID = new Database.Filter.StringSearch.SearchFilter();
                ItemPersonalizationValueApprovedFilter.WebsiteID.SearchString = CurrentWebsite.WebsiteID;
                ItemPersonalizationValueApproveds = ImageSolutions.Item.ItemPersonalizationValueApproved.GetItemPersonalizationValueApproveds(ItemPersonalizationValueApprovedFilter, ucPager.PageSize, ucPager.CurrentPageNumber, out intTotalRecord);

                gvItemPersonalizationApproved.DataSource = ItemPersonalizationValueApproveds;
                gvItemPersonalizationApproved.DataBind();
                ucPager.TotalRecord = intTotalRecord;
            }
            catch (Exception ex)
            {
                WebUtility.DisplayJavascriptMessage(this, ex.Message);
            }
            finally { }
        }

        //protected void BindWebsiteGroup()
        //{
        //    try
        //    {
        //        this.gvWebsiteGroup.DataSource = CurrentUser.CurrentUserWebSite.WebSite.WebsiteGroups;
        //        this.gvWebsiteGroup.DataBind();
        //        if (this.gvWebsiteGroup.HeaderRow != null) this.gvWebsiteGroup.HeaderRow.TableSection = TableRowSection.TableHeader;

        //    }
        //    catch (Exception ex)
        //    {
        //        WebUtility.DisplayJavascriptMessage(this, ex.Message);
        //    }
        //    finally { }
        //}

        protected void BindWebsiteTab()
        {
            try
            {
                tvWebsiteTab.Nodes.Clear();

                foreach (ImageSolutions.Website.WebsiteTab _WebsiteTab in CurrentUser.CurrentUserWebSite.WebSite.WebsiteTabs)
                {
                    if (string.IsNullOrEmpty(_WebsiteTab.ParentID))
                    {
                        TreeNode ParentTreeNode = new TreeNode();
                        ParentTreeNode.Text = string.Format(@"{0}{1}", _WebsiteTab.TabName, _WebsiteTab.Inactive ? " (inactive)" : String.Empty);
                        ParentTreeNode.Value = _WebsiteTab.WebsiteTabID;
                        AddNodes(ref ParentTreeNode);
                        tvWebsiteTab.Nodes.Add(ParentTreeNode);
                    }
                }
                tvWebsiteTab.ExpandAll();
            }
            catch (Exception ex)
            {
                WebUtility.DisplayJavascriptMessage(this, ex.Message);
            }
            finally { }
        }

        private void AddNodes(ref TreeNode treenode)
        {
            DataTable DataTable = GetChildData(Convert.ToString(treenode.Value));
            foreach (DataRow _DataRow in DataTable.Rows)
            {
                TreeNode childNode = new TreeNode();
                childNode.Text = _DataRow["TabName"].ToString();
                childNode.Value = _DataRow["WebsiteTabID"].ToString();
                AddNodes(ref childNode);
                treenode.ChildNodes.Add(childNode);
            }
        }

        public DataTable GetChildData(string parentid)
        {
            DataTable DataTable = new DataTable();
            DataTable.Columns.AddRange(new DataColumn[] {
                new DataColumn("WebsiteTabID"),
                new DataColumn("ParentId"),
                new DataColumn("TabName") });

            foreach (ImageSolutions.Website.WebsiteTab _WebsiteTab in CurrentUser.CurrentUserWebSite.WebSite.WebsiteTabs)
            {
                if (_WebsiteTab.ParentID == parentid)
                {
                    DataRow DataRow = DataTable.NewRow();
                    DataRow["WebsiteTabID"] = _WebsiteTab.WebsiteTabID;
                    DataRow["ParentId"] = _WebsiteTab.ParentID;
                    DataRow["TabName"] = string.Format(@"{0}{1}", _WebsiteTab.TabName, _WebsiteTab.Inactive ? " (inactive)" : String.Empty);
                    DataTable.Rows.Add(DataRow);
                }
            }

            return DataTable;
        }

        protected void tvWebsiteTab_SelectedNodeChanged(object sender, EventArgs e)
        {
            string strWebsiteTabID = Convert.ToString(tvWebsiteTab.SelectedNode.Value);
            Response.Redirect(String.Format("/admin/tab.aspx?id={0}", strWebsiteTabID));
        }

        protected void btnCreateNewTab_Click(object sender, EventArgs e)
        {
            Response.Redirect("/admin/createtab.aspx");
        }

        protected void btnUpload_Click(object sender, EventArgs e)
        {
            if (fuLogoImage.PostedFile.ContentLength > 0)
            {
                Stream objStream = fuLogoImage.PostedFile.InputStream;
                string strExtension = Path.GetExtension(fuLogoImage.PostedFile.FileName);

                string strFilename = string.Format("Website/{0}/{1}{2}", DateTime.UtcNow.ToString("yyyyMM"), DateTime.UtcNow.ToString("yyyyMMddHHmmssffffff"), strExtension);
                SaveFile(objStream, strFilename);

                string strImagePath = string.Format("{0}{1}", Convert.ToString(ConfigurationManager.AppSettings["ImageURL"]), strFilename);
                imgLogo.ImageUrl = strImagePath;
            }
            InitializeImage();
        }

        protected void btnResetImage_Click(object sender, EventArgs e)
        {
            imgLogo.ImageUrl = CurrentUser.CurrentUserWebSite.WebSite.LogoPath;
            InitializeImage();
        }

        protected void btnRemoveImage_Click(object sender, EventArgs e)
        {
            imgLogo.ImageUrl = string.Empty;
            InitializeImage();
        }
        

        protected void btnBannerUpload_Click(object sender, EventArgs e)
        {
            if (fuBannerImage.PostedFile.ContentLength > 0)
            {
                Stream objStream = fuBannerImage.PostedFile.InputStream;
                string strExtension = Path.GetExtension(fuBannerImage.PostedFile.FileName);

                string strFilename = string.Format("Website/{0}/{1}{2}", DateTime.UtcNow.ToString("yyyyMM"), DateTime.UtcNow.ToString("yyyyMMddHHmmssffffff"), strExtension);
                SaveFile(objStream, strFilename);

                string strImagePath = string.Format("{0}{1}", Convert.ToString(ConfigurationManager.AppSettings["ImageURL"]), strFilename);
                imgBanner.ImageUrl = strImagePath;
            }
            InitializeBannerImage();
        }
        protected void btnBannerResetImage_Click(object sender, EventArgs e)
        {
            imgBanner.ImageUrl = CurrentUser.CurrentUserWebSite.WebSite.BannerPath;
            InitializeBannerImage();
        }
        protected void btnBannerRemoveImage_Click(object sender, EventArgs e)
        {
            imgBanner.ImageUrl = string.Empty;
            InitializeBannerImage();
        }
        protected void InitializeBannerImage()
        {
            btnBannerResetImage.Visible = !string.IsNullOrEmpty(imgBanner.ImageUrl) && imgBanner.ImageUrl != CurrentUser.CurrentUserWebSite.WebSite.BannerPath;
            btnBannerRemoveImage.Visible = !string.IsNullOrEmpty(imgBanner.ImageUrl) && imgBanner.ImageUrl == CurrentUser.CurrentUserWebSite.WebSite.BannerPath;
        }
        private AmazonS3Client mAmazonClient = null;
        private AmazonS3Client AmazonS3Client
        {
            get
            {
                if (mAmazonClient == null)
                {
                    Amazon.RegionEndpoint RegionEndPoint = null;

                    switch (Convert.ToString(ConfigurationManager.AppSettings["AWSRegionEndpoint"]))
                    {
                        case "USWest1":
                            RegionEndPoint = Amazon.RegionEndpoint.USWest1;
                            break;
                        case "USWest2":
                            RegionEndPoint = Amazon.RegionEndpoint.USWest2;
                            break;
                        case "USEast1":
                            RegionEndPoint = Amazon.RegionEndpoint.USEast1;
                            break;
                        case "USEast2":
                            RegionEndPoint = Amazon.RegionEndpoint.USEast2;
                            break;
                        default:
                            RegionEndPoint = Amazon.RegionEndpoint.USWest1;
                            break;
                    }


                    mAmazonClient = new AmazonS3Client(Convert.ToString(ConfigurationManager.AppSettings["AWSAccessKeyID"])
                        , Convert.ToString(ConfigurationManager.AppSettings["AWSSecretAccessKey"])
                        , RegionEndPoint);
                }
                return mAmazonClient;
            }
            set
            {
                mAmazonClient = value;
            }
        }
        private string S3BucketName = Convert.ToString(ConfigurationManager.AppSettings["AWSBucketName"]);
        private Hashtable mTempSavedKey = new Hashtable();
        protected void SaveFile(Stream Source, string Key)
        {
            TransferUtility objTransfer = null;
            TransferUtilityUploadRequest objRequest = null;

            try
            {
                if (Source.CanSeek)
                {
                    Source.Seek(0, SeekOrigin.Begin);

                    objTransfer = new TransferUtility(AmazonS3Client);
                    objRequest = new TransferUtilityUploadRequest();
                    objRequest.InputStream = Source;
                    objRequest.BucketName = S3BucketName;
                    objRequest.Key = Key;
                    objRequest.CannedACL = S3CannedACL.PublicRead;
                    objTransfer.Upload(objRequest);
                    mTempSavedKey[objRequest.Key] = true;
                }
                else
                {
                    throw new Exception("Unable to seek the input stream");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                objTransfer = null;
                objRequest = null;
            }
        }

        protected void gvWebsiteCountry_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "LineRemove")
            {
                try
                {
                    ImageSolutions.Website.WebsiteCountry WebsiteCountry = new ImageSolutions.Website.WebsiteCountry(Convert.ToString(e.CommandArgument));
                    WebsiteCountry.Delete();

                    BindWebsiteCountry();
                }
                catch (Exception ex)
                {
                    WebUtility.DisplayJavascriptMessage(this, ex.Message);
                }
                finally { }
            }
        }
        protected void BindWebsiteCountry()
        {
            try
            {
                gvWebsiteCountry.DataSource = CurrentUser.CurrentUserWebSite.WebSite.WebsiteCountries;
                gvWebsiteCountry.DataBind();
            }
            catch (Exception ex)
            {
                WebUtility.DisplayJavascriptMessage(this, ex.Message);
            }
            finally { }
        }

        protected void btnEmailLogoUpload_Click(object sender, EventArgs e)
        {
            if (fuEmailLogoImage.PostedFile.ContentLength > 0)
            {
                Stream objStream = fuEmailLogoImage.PostedFile.InputStream;
                string strExtension = Path.GetExtension(fuEmailLogoImage.PostedFile.FileName);

                string strFilename = string.Format("Website/{0}/{1}{2}", DateTime.UtcNow.ToString("yyyyMM"), DateTime.UtcNow.ToString("yyyyMMddHHmmssffffff"), strExtension);
                SaveFile(objStream, strFilename);

                string strImagePath = string.Format("{0}{1}", Convert.ToString(ConfigurationManager.AppSettings["ImageURL"]), strFilename);
                imgEmailLogo.ImageUrl = strImagePath;
            }
            InitializeEmailLogoImage();
        }

        protected void btnEmailLogoResetImage_Click(object sender, EventArgs e)
        {
            imgEmailLogo.ImageUrl = CurrentUser.CurrentUserWebSite.WebSite.LogoPath;
            InitializeEmailLogoImage();
        }

        protected void btnEmailLogoRemoveImage_Click(object sender, EventArgs e)
        {
            imgEmailLogo.ImageUrl = string.Empty;
            InitializeEmailLogoImage();
        }
        protected void InitializeEmailLogoImage()
        {
            btnEmailLogoResetImage.Visible = !string.IsNullOrEmpty(imgEmailLogo.ImageUrl) && imgEmailLogo.ImageUrl != CurrentUser.CurrentUserWebSite.WebSite.EmailLogoPath;
            btnEmailLogoRemoveImage.Visible = !string.IsNullOrEmpty(imgEmailLogo.ImageUrl) && imgEmailLogo.ImageUrl == CurrentUser.CurrentUserWebSite.WebSite.EmailLogoPath;
        }

        protected void InitializeSizeChartImage()
        {
            btnSizeChartResetImage.Visible = !string.IsNullOrEmpty(imgSizeChart.ImageUrl) && imgSizeChart.ImageUrl != CurrentUser.CurrentUserWebSite.WebSite.DefaultSizeChartPath;
            btnSizeChartRemoveImage.Visible = !string.IsNullOrEmpty(imgSizeChart.ImageUrl) && imgSizeChart.ImageUrl == CurrentUser.CurrentUserWebSite.WebSite.DefaultSizeChartPath;
        }

        protected void gvItemPersonalizationApproved_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "LineRemove")
            {
                try
                {
                    ImageSolutions.Item.ItemPersonalizationValueApproved ItemPersonalizationValueApproved = new ImageSolutions.Item.ItemPersonalizationValueApproved(Convert.ToString(e.CommandArgument));
                    ItemPersonalizationValueApproved.Delete();

                    BindItemPersonalizationApproved();
                }
                catch (Exception ex)
                {
                    WebUtility.DisplayJavascriptMessage(this, ex.Message);
                }
                finally { }
            }
        }

        protected void btnSizeChartUpload_Click(object sender, EventArgs e)
        {
            if (fuSizeChartImage.PostedFile.ContentLength > 0)
            {
                Stream objStream = fuSizeChartImage.PostedFile.InputStream;
                string strExtension = Path.GetExtension(fuSizeChartImage.PostedFile.FileName);

                string strFilename = string.Format("Website/{0}/{1}{2}", DateTime.UtcNow.ToString("yyyyMM"), DateTime.UtcNow.ToString("yyyyMMddHHmmssffffff"), strExtension);
                SaveFile(objStream, strFilename);

                string strImagePath = string.Format("{0}{1}", Convert.ToString(ConfigurationManager.AppSettings["ImageURL"]), strFilename);
                imgSizeChart.ImageUrl = strImagePath;
            }
            InitializeSizeChartImage();
        }

        protected void btnSizeChartResetImage_Click(object sender, EventArgs e)
        {
            imgSizeChart.ImageUrl = CurrentUser.CurrentUserWebSite.WebSite.DefaultSizeChartPath;
            InitializeSizeChartImage();
        }

        protected void btnSizeChartRemoveImage_Click(object sender, EventArgs e)
        {
            imgSizeChart.ImageUrl = string.Empty;
            InitializeSizeChartImage();
        }

        protected void btnExportItemPersonalizationApproved_Click(object sender, EventArgs e)
        {
            string strSQL = string.Empty;

            try
            {
                string strPath = Server.MapPath("\\Export\\ApprovedPersonalization\\" + DateTime.UtcNow.ToString("yyyyMM") + "\\");
                if (!Directory.Exists(strPath))
                {
                    Directory.CreateDirectory(strPath);
                }

                string strFileExportPath = Server.MapPath(string.Format("\\Export\\ApprovedPersonalization\\{0}\\ApprovedPersonalization_{1}.csv", DateTime.UtcNow.ToString("yyyyMM"), DateTime.UtcNow.ToString("yyyyMMddHHmmssffff")));
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

            try
            {
                StringBuilder objCSV = null;

                objCSV = new StringBuilder();

                objCSV.Append(string.Format("{0},{1}"
    , "Label"
    , "Value"
));
                objCSV.AppendLine();

                List<ImageSolutions.Item.ItemPersonalizationValueApproved> ItemPersonalizationValueApproveds = null;
                ImageSolutions.Item.ItemPersonalizationValueApprovedFilter ItemPersonalizationValueApprovedFilter = null;

                ItemPersonalizationValueApprovedFilter = new ImageSolutions.Item.ItemPersonalizationValueApprovedFilter();
                ItemPersonalizationValueApprovedFilter.WebsiteID = new Database.Filter.StringSearch.SearchFilter();
                ItemPersonalizationValueApprovedFilter.WebsiteID.SearchString = CurrentWebsite.WebsiteID;
                ItemPersonalizationValueApproveds = ImageSolutions.Item.ItemPersonalizationValueApproved.GetItemPersonalizationValueApproveds(ItemPersonalizationValueApprovedFilter);

                foreach (ImageSolutions.Item.ItemPersonalizationValueApproved _ItemPersonalizationValueApproved in ItemPersonalizationValueApproveds)
                {
                    objCSV.Append(_ItemPersonalizationValueApproved.ItemPersonalizationName.Replace(",", "")).Append(",");
                    objCSV.Append(_ItemPersonalizationValueApproved.ItemPersonalizationApprovedValue.Replace(",", "")).Append(",");
                    objCSV.AppendLine();
                }

                if (objCSV != null)
                {
                    using (StreamWriter objWriter = new StreamWriter(filepath))
                    {
                        objWriter.Write(objCSV.ToString());
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

    }
}