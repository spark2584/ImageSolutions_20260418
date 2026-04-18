using Amazon.S3;
using Amazon.S3.Transfer;
using ImageSolutions.User;
using ImageSolutions.Website;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ImageSolutionsWebsite.Admin
{
    public partial class Tab : BasePageAdminUserWebSiteAuth
    {
        protected string mWebsiteTabID = string.Empty;
        protected string mParentID = string.Empty;

        private ImageSolutions.Website.WebsiteTab mWebsiteTab = null;
        protected ImageSolutions.Website.WebsiteTab _WebsiteTab
        {
            get
            {
                if (mWebsiteTab == null)
                {
                    if (string.IsNullOrEmpty(mWebsiteTabID))
                        mWebsiteTab = new ImageSolutions.Website.WebsiteTab();
                    else
                        mWebsiteTab = new ImageSolutions.Website.WebsiteTab(mWebsiteTabID);
                }
                return mWebsiteTab;
            }
            set
            {
                mWebsiteTab = value;
            }
        }

        protected string ReturnURL
        {
            get
            {
                if (ViewState["ReturnURL"] == null)
                {
                    if (!string.IsNullOrEmpty(mParentID))
                        return "/Admin/Tab.aspx?id=" + mParentID + "&tab=2";
                    else if (!_WebsiteTab.IsNew)
                        return "/Admin/Tab.aspx?id=" + _WebsiteTab.WebsiteTabID;
                    else
                        return "/Admin/TabOverview.aspx";
                }
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
            mWebsiteTabID = Request.QueryString.Get("id");
            mParentID = Request.QueryString.Get("parentid");

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
                InitializeTabs(top_1_tab, top_1, top_2_tab, top_2, top_3_tab, top_3, top_4_tab, top_4, top_5_tab, top_5);

                BindTabs();

                if (!_WebsiteTab.IsNew)
                {
                    txtName.Text = _WebsiteTab.TabName;
                    txtDisplayName.Text = _WebsiteTab.DisplayName;
                    txtTabSort.Text = Convert.ToString(_WebsiteTab.Sort);
                    imgTab.ImageUrl = _WebsiteTab.ImageURL;
                    cbAllowAllGroups.Checked = _WebsiteTab.AllowAllGroups;
                    cbInactive.Checked = _WebsiteTab.Inactive;
                    ddlParentTab.SelectedValue = _WebsiteTab.ParentID;
                    txtMessage.Text = _WebsiteTab.Message;

                    ddlDisplayUserPermission.SelectedValue = _WebsiteTab.DisplayUserPermission;

                    btnSave.Text = "Save";

                    InitializeImage();
                    BindWebsiteTab();
                    BindWebSiteTabItem();
                    BindWebsiteGroupTab();
                    BindUserWebsiteTab();
                }
                else
                {
                    this.ddlParentTab.SelectedIndex = this.ddlParentTab.Items.IndexOf(this.ddlParentTab.Items.FindByValue(mParentID));
                    btnSave.Text = "Create";
                    btnDelete.Visible = false;
                    top_2_tab.Visible = false;
                    top_3_tab.Visible = false;
                    top_4_tab.Visible = false;
                    top_5_tab.Visible = false;
                }
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
            btnResetImage.Visible = !string.IsNullOrEmpty(imgTab.ImageUrl) && imgTab.ImageUrl != _WebsiteTab.ImageURL;
            btnRemoveImage.Visible = !string.IsNullOrEmpty(imgTab.ImageUrl) && imgTab.ImageUrl == _WebsiteTab.ImageURL;
        }

        protected void BindTabs()
        {
            try
            {
                this.ddlParentTab.DataSource = CurrentUser.CurrentUserWebSite.WebSite.WebsiteTabs;
                this.ddlParentTab.DataBind();
                this.ddlParentTab.Items.Insert(0, new ListItem(String.Empty, string.Empty));
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally { }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            SqlConnection objConn = null;
            SqlTransaction objTran = null;
            bool blnReturn = false;

            try
            {
                objConn = new SqlConnection(Database.DefaultConnectionString);
                objConn.Open();
                objTran = objConn.BeginTransaction();

                _WebsiteTab.TabName = txtName.Text;
                _WebsiteTab.DisplayName = txtDisplayName.Text;
                _WebsiteTab.Sort = string.IsNullOrEmpty(txtTabSort.Text) ? 9999 : Convert.ToInt32(txtTabSort.Text);
                _WebsiteTab.ImageURL = imgTab.ImageUrl;
                _WebsiteTab.AllowAllGroups = cbAllowAllGroups.Checked;

                _WebsiteTab.DisplayUserPermission = ddlDisplayUserPermission.SelectedValue;
                _WebsiteTab.Message = txtMessage.Text;

                _WebsiteTab.Inactive = cbInactive.Checked;
                _WebsiteTab.ParentID = this.ddlParentTab.SelectedValue;

                //Update Sorting
                List<ImageSolutions.Website.WebsiteTab> WebsiteTabs = new List<ImageSolutions.Website.WebsiteTab>();
                ImageSolutions.Website.WebsiteTabFilter WebsiteTabFilter = new WebsiteTabFilter();
                WebsiteTabFilter.WebsiteID = new Database.Filter.StringSearch.SearchFilter();
                WebsiteTabFilter.WebsiteID.SearchString = CurrentUser.CurrentUserWebSite.WebSite.WebsiteID;

                WebsiteTabFilter.ParentID = new Database.Filter.StringSearch.SearchFilter();
                if (string.IsNullOrEmpty(ddlParentTab.SelectedValue))
                {
                    WebsiteTabFilter.ParentID.Operator = Database.Filter.StringSearch.SearchOperator.empty;
                }
                else
                {
                    WebsiteTabFilter.ParentID.SearchString = ddlParentTab.SelectedValue;
                }
                WebsiteTabs = ImageSolutions.Website.WebsiteTab.GetWebsiteTabs(WebsiteTabFilter);

                foreach (ImageSolutions.Website.WebsiteTab _websitetab in WebsiteTabs)
                {
                    if(_websitetab.WebsiteTabID == _WebsiteTab.WebsiteTabID)
                    {
                        _websitetab.Sort = _WebsiteTab.Sort;
                    }
                    else if (_websitetab.Sort != null 
                        && _websitetab.Sort >= _WebsiteTab.Sort)
                    {
                        _websitetab.Sort = _websitetab.Sort + 1;
                        _websitetab.Update(objConn, objTran);
                    }
                }

                if (_WebsiteTab.IsNew)
                {
                    _WebsiteTab.WebsiteID = CurrentUser.CurrentUserWebSite.WebSite.WebsiteID;
                    _WebsiteTab.CreatedBy = CurrentUser.UserInfoID;
                    blnReturn = _WebsiteTab.Create(objConn, objTran);

                    WebsiteTabs.Add(_WebsiteTab);
                }
                else
                {
                    blnReturn = _WebsiteTab.Update(objConn, objTran);
                }


                //Update Sorting
                WebsiteTabs = ImageSolutions.Website.WebsiteTab.GetWebsiteTabs(WebsiteTabFilter);
                WebsiteTabs = WebsiteTabs.OrderBy(x => x.Sort == null ? 9999 : x.Sort).ToList();
                int counter = 0;
                foreach (ImageSolutions.Website.WebsiteTab _websitetab in WebsiteTabs)
                {
                    counter++;
                    _websitetab.Sort = counter;
                    _websitetab.Update(objConn, objTran);
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

        protected void BindWebsiteTab()
        {
            try
            {
                gvWebsiteTabs.DataSource = _WebsiteTab.ChildWebsiteTabs;
                gvWebsiteTabs.DataBind();
            }
            catch (Exception ex)
            {
                WebUtility.DisplayJavascriptMessage(this, ex.Message);
            }
            finally { }
        }

        protected void gvWebsiteTabItem_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "MoveUp")
            {
                string strWebsiteTabItemID = Convert.ToString(e.CommandArgument);
                WebsiteTabItem WebsiteTabItem = new WebsiteTabItem(strWebsiteTabItemID);

                WebsiteTabItem UpWebsiteTabItem = new WebsiteTabItem();
                WebsiteTabItemFilter WebsiteTabItemFilter = new WebsiteTabItemFilter();
                WebsiteTabItemFilter.WebsiteTabID = new Database.Filter.StringSearch.SearchFilter();
                WebsiteTabItemFilter.WebsiteTabID.SearchString = _WebsiteTab.WebsiteTabID;
                WebsiteTabItemFilter.Sort = WebsiteTabItem.Sort - 1;
                UpWebsiteTabItem = WebsiteTabItem.GetWebsiteTabItem(WebsiteTabItemFilter);

                UpWebsiteTabItem.Sort = UpWebsiteTabItem.Sort + 1;
                UpWebsiteTabItem.Update();

                WebsiteTabItem.Sort = WebsiteTabItem.Sort - 1;
                WebsiteTabItem.Update();

                BindWebSiteTabItem();
            }
            else if (e.CommandName == "MoveDown")
            {
                string strWebsiteTabItemID = Convert.ToString(e.CommandArgument);
                WebsiteTabItem WebsiteTabItem = new WebsiteTabItem(strWebsiteTabItemID);

                WebsiteTabItem DownWebsiteTabItem = new WebsiteTabItem();
                WebsiteTabItemFilter WebsiteTabItemFilter = new WebsiteTabItemFilter();
                WebsiteTabItemFilter.WebsiteTabID = new Database.Filter.StringSearch.SearchFilter();
                WebsiteTabItemFilter.WebsiteTabID.SearchString = _WebsiteTab.WebsiteTabID;
                WebsiteTabItemFilter.Sort = WebsiteTabItem.Sort + 1;
                DownWebsiteTabItem = WebsiteTabItem.GetWebsiteTabItem(WebsiteTabItemFilter);

                DownWebsiteTabItem.Sort = DownWebsiteTabItem.Sort - 1;
                DownWebsiteTabItem.Update();

                WebsiteTabItem.Sort = WebsiteTabItem.Sort + 1;
                WebsiteTabItem.Update();

                BindWebSiteTabItem();
            }
        }
        protected void BindWebSiteTabItem()
        {
            try
            {
                List<ImageSolutions.Website.WebsiteTabItem> WebsiteTabItems = new List<ImageSolutions.Website.WebsiteTabItem>();
                ImageSolutions.Website.WebsiteTabItemFilter WebsiteTabItemFilter = new ImageSolutions.Website.WebsiteTabItemFilter();
                WebsiteTabItemFilter.WebsiteTabID = new Database.Filter.StringSearch.SearchFilter();
                WebsiteTabItemFilter.WebsiteTabID.SearchString = Convert.ToString(mWebsiteTabID);
                WebsiteTabItems = ImageSolutions.Website.WebsiteTabItem.GetWebsiteTabItems(WebsiteTabItemFilter);

                gvWebsiteTabItem.DataSource = WebsiteTabItems;
                gvWebsiteTabItem.DataBind();

                ddlTabItem.DataSource = WebsiteTabItems;
                ddlTabItem.DataBind();
                ddlTabItem.Items.Insert(0, new ListItem(string.Empty, string.Empty));

                btnUpdateSort.Visible = gvWebsiteTabItem.Rows.Count > 0;
            }
            catch (Exception ex)
            {
                WebUtility.DisplayJavascriptMessage(this, ex.Message);
            }
            finally { }
        }

        protected void btnCreateChildTab_Click(object sender, EventArgs e)
        {
            ImageSolutions.Website.WebsiteTab WebsiteTab = null;
            try
            {
                WebsiteTab = new ImageSolutions.Website.WebsiteTab(mWebsiteTabID);
            }
            catch (Exception ex)
            {
                WebUtility.DisplayJavascriptMessage(this, ex.Message);
                //Response.Redirect(string.Format("/Admin/TabOverview.aspx"));
            }
            finally { }

            Response.Redirect(String.Format("/Admin/CreateTab.aspx?websiteid={0}&parentid={1}", WebsiteTab.WebsiteID, WebsiteTab.WebsiteTabID));
        }

        protected void btnCreateTabItem_Click(object sender, EventArgs e)
        {
            Response.Redirect(String.Format("/Admin/CreateTabItem.aspx?websitetabid={0}", mWebsiteTabID));
        }

        protected void btnAddGroup_Click(object sender, EventArgs e)
        {
            Response.Redirect(String.Format("/Admin/CreateGroupTab.aspx?websitetabid={0}", mWebsiteTabID));
        }

        protected void gvWebsiteGroupTab_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "LineRemove")
            {
                try
                {
                    ImageSolutions.Website.WebsiteGroupTab WebsiteGroupTab = new ImageSolutions.Website.WebsiteGroupTab(Convert.ToString(e.CommandArgument));
                    WebsiteGroupTab.Delete();

                    BindWebsiteGroupTab();
                }
                catch (Exception ex)
                {
                    WebUtility.DisplayJavascriptMessage(this, ex.Message);
                }
                finally { }
            }
        }
        protected void BindWebsiteGroupTab()
        {
            try
            {
                List<ImageSolutions.Website.WebsiteGroupTab> WebsiteGroupTabs = new List<ImageSolutions.Website.WebsiteGroupTab>();
                ImageSolutions.Website.WebsiteGroupTabFilter WebsiteGroupTabFilter = new ImageSolutions.Website.WebsiteGroupTabFilter();
                WebsiteGroupTabFilter.WebsiteTabID = new Database.Filter.StringSearch.SearchFilter();
                WebsiteGroupTabFilter.WebsiteTabID.SearchString = Convert.ToString(mWebsiteTabID);
                WebsiteGroupTabs = ImageSolutions.Website.WebsiteGroupTab.GetWebsiteGroupTabs(WebsiteGroupTabFilter);

                gvWebsiteGroupTab.DataSource = WebsiteGroupTabs;
                gvWebsiteGroupTab.DataBind();
            }
            catch (Exception ex)
            {
                WebUtility.DisplayJavascriptMessage(this, ex.Message);
            }
            finally { }
        }
        protected void BindUserWebsiteTab()
        {
            try
            {
                List<ImageSolutions.User.UserWebsiteTab> UserWebsiteTabs = new List<ImageSolutions.User.UserWebsiteTab>();
                ImageSolutions.User.UserWebsiteTabFilter WebsiteGroupTabFilter = new ImageSolutions.User.UserWebsiteTabFilter();
                WebsiteGroupTabFilter.WebsiteTabID = new Database.Filter.StringSearch.SearchFilter();
                WebsiteGroupTabFilter.WebsiteTabID.SearchString = Convert.ToString(mWebsiteTabID);
                UserWebsiteTabs = ImageSolutions.User.UserWebsiteTab.GetUserWebsiteTabs(WebsiteGroupTabFilter);

                gvUserWebsiteTab.DataSource = UserWebsiteTabs.OrderBy(m => m.UserWebsite.Description);
                gvUserWebsiteTab.DataBind();
            }
            catch (Exception ex)
            {
                WebUtility.DisplayJavascriptMessage(this, ex.Message);
            }
            finally { }
        }

        protected void btnDelete_Click(object sender, EventArgs e)
        {
            SqlConnection objConn = null;
            SqlTransaction objTran = null;
            bool blnReturn = false;

            try
            {
                objConn = new SqlConnection(Database.DefaultConnectionString);
                objConn.Open();
                objTran = objConn.BeginTransaction();

                string strParentID = _WebsiteTab.ParentID;
                blnReturn = _WebsiteTab.Delete(objConn, objTran);

                //Update Sorting
                List<ImageSolutions.Website.WebsiteTab> WebsiteTabs = new List<ImageSolutions.Website.WebsiteTab>();
                ImageSolutions.Website.WebsiteTabFilter WebsiteTabFilter = new WebsiteTabFilter();
                WebsiteTabFilter.WebsiteID = new Database.Filter.StringSearch.SearchFilter();
                WebsiteTabFilter.WebsiteID.SearchString = CurrentUser.CurrentUserWebSite.WebSite.WebsiteID;
                WebsiteTabFilter.ParentID = new Database.Filter.StringSearch.SearchFilter();
                if (string.IsNullOrEmpty(strParentID))
                {
                    WebsiteTabFilter.ParentID.Operator = Database.Filter.StringSearch.SearchOperator.empty;
                }
                else
                {
                    WebsiteTabFilter.ParentID.SearchString = strParentID;
                }
                WebsiteTabs = ImageSolutions.Website.WebsiteTab.GetWebsiteTabs(WebsiteTabFilter);
                WebsiteTabs = WebsiteTabs.OrderBy(x => x.Sort == null ? 9999 : x.Sort).ToList();
                int counter = 0;
                foreach (ImageSolutions.Website.WebsiteTab _websitetab in WebsiteTabs)
                {
                    counter++;
                    _websitetab.Sort = counter;
                    _websitetab.Update(objConn, objTran);
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
                Response.Redirect("/Admin/TabOverview.aspx");
            }
        }
        protected void btnUpload_Click(object sender, EventArgs e)
        {
            if (fuTabImage.PostedFile.ContentLength > 0)
            {
                Stream objStream = fuTabImage.PostedFile.InputStream;
                string strExtension = Path.GetExtension(fuTabImage.PostedFile.FileName);

                string strFilename = string.Format("Item/{0}/{1}{2}", DateTime.UtcNow.ToString("yyyyMM"), DateTime.UtcNow.ToString("yyyyMMddHHmmssffffff"), strExtension);
                SaveFile(objStream, strFilename);

                string strImagePath = string.Format("{0}{1}", Convert.ToString(ConfigurationManager.AppSettings["ImageURL"]), strFilename);
                imgTab.ImageUrl = strImagePath;
            }
            InitializeImage();
        }

        protected void btnResetImage_Click(object sender, EventArgs e)
        {
            ImageSolutions.Website.WebsiteTab WebsiteTab = new ImageSolutions.Website.WebsiteTab(Convert.ToString(mWebsiteTabID));
            imgTab.ImageUrl = WebsiteTab.ImageURL;
            InitializeImage();
        }
        protected void btnRemoveImage_Click(object sender, EventArgs e)
        {
            imgTab.ImageUrl = string.Empty;
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

        protected void gvWebsiteTabItem_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    string strWebsiteTabItemID = gvWebsiteTabItem.DataKeys[e.Row.RowIndex].Value.ToString();

                    Button btnUp = (Button)e.Row.FindControl("btnUp");
                    Button btnDown = (Button)e.Row.FindControl("btnDown");
                    
                    WebsiteTabItem WebsiteTabItem = new WebsiteTabItem(strWebsiteTabItemID);

                    List<ImageSolutions.Website.WebsiteTabItem> WebsiteTabItems = new List<ImageSolutions.Website.WebsiteTabItem>();
                    ImageSolutions.Website.WebsiteTabItemFilter WebsiteTabItemFilter = new ImageSolutions.Website.WebsiteTabItemFilter();
                    WebsiteTabItemFilter.WebsiteTabID = new Database.Filter.StringSearch.SearchFilter();
                    WebsiteTabItemFilter.WebsiteTabID.SearchString = Convert.ToString(mWebsiteTabID);
                    WebsiteTabItems = ImageSolutions.Website.WebsiteTabItem.GetWebsiteTabItems(WebsiteTabItemFilter);

                    WebsiteTabItem.Sort = e.Row.RowIndex + 1;
                    WebsiteTabItem.Update();

                    btnUp.Visible = Convert.ToInt32(WebsiteTabItem.Sort) != 1;
                    btnDown.Visible = WebsiteTabItems.Count != Convert.ToInt32(WebsiteTabItem.Sort);

                    TextBox txtSort = (TextBox)e.Row.FindControl("txtSort");
                    txtSort.Text = Convert.ToString(WebsiteTabItem.Sort);
                }
            }
            catch (Exception ex)
            {
                WebUtility.DisplayJavascriptMessage(this, ex.Message);
            }
        }

        protected void btnUpdateSort_Click(object sender, EventArgs e)
        {
            foreach (GridViewRow _Row in gvWebsiteTabItem.Rows)
            {
                string strWebsiteTabItemID = gvWebsiteTabItem.DataKeys[_Row.RowIndex].Value.ToString();

                TextBox txtSort = (TextBox)_Row.FindControl("txtSort");

                WebsiteTabItem WebsiteTabItem = new WebsiteTabItem(strWebsiteTabItemID);
                WebsiteTabItem.Sort = Convert.ToInt32(txtSort.Text);
                WebsiteTabItem.Update();
            }

            BindWebSiteTabItem();
        }

        protected void gvUserWebsiteTab_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "LineRemove")
            {
                try
                {
                    ImageSolutions.User.UserWebsiteTab UserWebsiteTab = new ImageSolutions.User.UserWebsiteTab(Convert.ToString(e.CommandArgument));
                    UserWebsiteTab.Delete();

                    BindUserWebsiteTab();
                }
                catch (Exception ex)
                {
                    WebUtility.DisplayJavascriptMessage(this, ex.Message);
                }
                finally { }
            }
        }

        protected void ddlTabItem_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(!string.IsNullOrEmpty(ddlTabItem.SelectedValue))
            {
                ImageSolutions.Website.WebsiteTabItem WebsiteTabItem = new ImageSolutions.Website.WebsiteTabItem(ddlTabItem.SelectedValue);
                imgTab.ImageUrl = WebsiteTabItem.Item.ImageURL;
            }
            else
            {
                imgTab.ImageUrl = null;
            }
            InitializeImage();
        }
    }
}