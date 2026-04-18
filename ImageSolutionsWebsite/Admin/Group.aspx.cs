using Amazon.S3;
using Amazon.S3.Transfer;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ImageSolutionsWebsite.Admin
{
    public partial class Group : BasePageAdminUserWebSiteAuth
    {
        private string mWebsiteGroupID = string.Empty;
        private string mPage = string.Empty;

        private ImageSolutions.Website.WebsiteGroup mWebsiteGroup = null;
        protected ImageSolutions.Website.WebsiteGroup WebsiteGroup
        {
            get
            {
                if (mWebsiteGroup == null)
                {
                    if (string.IsNullOrEmpty(mWebsiteGroupID))
                        mWebsiteGroup = new ImageSolutions.Website.WebsiteGroup();
                    else
                        mWebsiteGroup = new ImageSolutions.Website.WebsiteGroup(mWebsiteGroupID);
                }
                return mWebsiteGroup;
            }
            set
            {
                mWebsiteGroup = value;
            }
        }
        
        protected string ReturnURL
        {
            get
            {
                if (ViewState["ReturnURL"] == null)
                    return "/Admin/GroupOverview.aspx";
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
            mWebsiteGroupID = Request.QueryString.Get("id");
            mPage = Request.QueryString.Get("page");
           
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

                if (!string.IsNullOrEmpty(mPage))
                {
                    if (top_2_tab != null)
                    {
                        top_2_tab.Attributes.Remove("class");
                        top_2_tab.Attributes.Add("class", "nav-link active");
                        top_2.Attributes.Remove("class");
                        top_2.Attributes.Add("class", "tab-pane fade show active");
                    }

                    if (top_1_tab != null)
                    {
                        top_1_tab.Attributes.Remove("class");
                        top_1_tab.Attributes.Add("class", "nav-link");
                        top_1.Attributes.Remove("class");
                        top_1.Attributes.Add("class", "tab-pane fade");
                    }
                }

                if (!WebsiteGroup.IsNew)
                {
                    txtName.Text = WebsiteGroup.GroupName;
                    imgLogo.ImageUrl = WebsiteGroup.LogoPath;
                    imgHomePageImage.ImageUrl = WebsiteGroup.HomePageImagePath;
                    imgHomePageMobileImage.ImageUrl = WebsiteGroup.HomePageMobileImagePath;
                    btnSave.Text = "Save";

                    BindUserAccount();
                }
                else
                {
                    btnSave.Text = "Create";
                    btnDelete.Visible = false;
                    this.top_2_tab.Visible = false;
                }
                aCancel.HRef = ReturnURL;

                InitializeImage();
            }
            catch (Exception ex)
            {
                WebUtility.DisplayJavascriptMessage(this, ex.Message);
            }
            finally { }
        }

        protected void BindUserAccount()
        {
            List<ImageSolutions.User.UserAccount> UserAccounts = null;
            ImageSolutions.User.UserAccountFilter UserAccountFilter = null;
            int intTotalRecord = 0;

            try
            {
                UserAccountFilter = new ImageSolutions.User.UserAccountFilter();
                UserAccountFilter.WebsiteGroupID = new Database.Filter.StringSearch.SearchFilter();
                UserAccountFilter.WebsiteGroupID.SearchString = WebsiteGroup.WebsiteGroupID;
                UserAccounts = ImageSolutions.User.UserAccount.GetUserAccounts(UserAccountFilter, ucPager.PageSize, ucPager.CurrentPageNumber, out intTotalRecord);

                this.gvUserAccount.DataSource = UserAccounts; //WebsiteGroup.UserAccounts;
                this.gvUserAccount.DataBind();
                ucPager.TotalRecord = intTotalRecord;

                if (this.gvUserAccount.HeaderRow != null) this.gvUserAccount.HeaderRow.TableSection = TableRowSection.TableHeader;
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
                WebsiteGroup.GroupName = txtName.Text;

                if (WebsiteGroup.IsNew)
                {
                    WebsiteGroup.WebsiteID = CurrentWebsite.WebsiteID;

                    WebsiteGroup.LogoPath = imgLogo.ImageUrl;
                    WebsiteGroup.HomePageImagePath = imgHomePageImage.ImageUrl;
                    WebsiteGroup.HomePageMobileImagePath = imgHomePageMobileImage.ImageUrl;

                    WebsiteGroup.CreatedBy = CurrentUser.UserInfoID;
                    blnReturn =  WebsiteGroup.Create();
                }
                else
                {
                    WebsiteGroup.LogoPath = imgLogo.ImageUrl;
                    WebsiteGroup.HomePageImagePath = imgHomePageImage.ImageUrl;
                    WebsiteGroup.HomePageMobileImagePath = imgHomePageMobileImage.ImageUrl;

                    blnReturn = WebsiteGroup.Update();
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
                blnReturn = WebsiteGroup.Delete();
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
            imgLogo.ImageUrl = WebsiteGroup.LogoPath;
            InitializeImage();
        }

        protected void btnRemoveImage_Click(object sender, EventArgs e)
        {
            imgLogo.ImageUrl = string.Empty;
            InitializeImage();
        }
        protected void InitializeImage()
        {
            btnResetImage.Visible = !string.IsNullOrEmpty(imgLogo.ImageUrl) && imgLogo.ImageUrl != WebsiteGroup.LogoPath;
            btnRemoveImage.Visible = !string.IsNullOrEmpty(imgLogo.ImageUrl) && imgLogo.ImageUrl == WebsiteGroup.LogoPath;

            btnHomePageImageResetImage.Visible = !string.IsNullOrEmpty(imgHomePageImage.ImageUrl) && imgHomePageImage.ImageUrl != WebsiteGroup.HomePageImagePath;
            btnHomePageImageRemoveImage.Visible = !string.IsNullOrEmpty(imgHomePageImage.ImageUrl) && imgHomePageImage.ImageUrl == WebsiteGroup.HomePageImagePath;

            btnHomePageMobileImageResetImage.Visible = !string.IsNullOrEmpty(imgHomePageMobileImage.ImageUrl) && imgHomePageMobileImage.ImageUrl != WebsiteGroup.HomePageMobileImagePath;
            btnHomePageMobileImageRemoveImage.Visible = !string.IsNullOrEmpty(imgHomePageMobileImage.ImageUrl) && imgHomePageMobileImage.ImageUrl == WebsiteGroup.HomePageMobileImagePath;
        }

        protected void btnHomePageImageUpload_Click(object sender, EventArgs e)
        {
            if (fuHomePageImageImage.PostedFile.ContentLength > 0)
            {
                Stream objStream = fuHomePageImageImage.PostedFile.InputStream;
                string strExtension = Path.GetExtension(fuHomePageImageImage.PostedFile.FileName);

                string strFilename = string.Format("Website/{0}/{1}{2}", DateTime.UtcNow.ToString("yyyyMM"), DateTime.UtcNow.ToString("yyyyMMddHHmmssffffff"), strExtension);
                SaveFile(objStream, strFilename);

                string strImagePath = string.Format("{0}{1}", Convert.ToString(ConfigurationManager.AppSettings["ImageURL"]), strFilename);
                imgHomePageImage.ImageUrl = strImagePath;
            }
            InitializeImage();
        }

        protected void btnHomePageImageResetImage_Click(object sender, EventArgs e)
        {
            imgHomePageImage.ImageUrl = WebsiteGroup.HomePageImagePath;
            InitializeImage();
        }

        protected void btnHomePageImageRemoveImage_Click(object sender, EventArgs e)
        {
            imgHomePageImage.ImageUrl = string.Empty;
            InitializeImage();
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

        protected void btnHomePageMobileImageUpload_Click(object sender, EventArgs e)
        {
            if (fuHomePageMobileImageImage.PostedFile.ContentLength > 0)
            {
                Stream objStream = fuHomePageMobileImageImage.PostedFile.InputStream;
                string strExtension = Path.GetExtension(fuHomePageMobileImageImage.PostedFile.FileName);

                string strFilename = string.Format("Website/{0}/{1}{2}", DateTime.UtcNow.ToString("yyyyMM"), DateTime.UtcNow.ToString("yyyyMMddHHmmssffffff"), strExtension);
                SaveFile(objStream, strFilename);

                string strImagePath = string.Format("{0}{1}", Convert.ToString(ConfigurationManager.AppSettings["ImageURL"]), strFilename);
                imgHomePageMobileImage.ImageUrl = strImagePath;
            }
            InitializeImage();
        }

        protected void btnHomePageMobileImageResetImage_Click(object sender, EventArgs e)
        {
            imgHomePageMobileImage.ImageUrl = WebsiteGroup.HomePageMobileImagePath;
            InitializeImage();
        }

        protected void btnHomePageMobileImageRemoveImage_Click(object sender, EventArgs e)
        {
            imgHomePageMobileImage.ImageUrl = string.Empty;
            InitializeImage();
        }
    }
}