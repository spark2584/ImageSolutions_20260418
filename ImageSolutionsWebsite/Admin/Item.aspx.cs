using Amazon.S3;
using Amazon.S3.Transfer;
using ImageSolutions.Item;
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
using System.Threading;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ImageSolutionsWebsite.Admin
{
    public partial class Item : BasePageAdminUserWebSiteAuth
    {
        protected string mParentID = string.Empty;
        protected string mItemID = string.Empty;

        private ImageSolutions.Item.Item mItem = null;
        protected ImageSolutions.Item.Item _Item
        {
            get
            {
                if (mItem == null)
                {
                    if (string.IsNullOrEmpty(mItemID))
                        mItem = new ImageSolutions.Item.Item();
                    else
                        mItem = new ImageSolutions.Item.Item(mItemID);
                }
                return mItem;
            }
            set
            {
                mItem = value;
            }
        }

        protected string ReturnURL
        {
            get
            {
                if (ViewState["ReturnURL"] == null)
                {
                    if (!string.IsNullOrEmpty(mParentID))
                        return "/Admin/Item.aspx?id=" + mParentID + "&tab=3";
                    else
                        return "/Admin/ItemOverview.aspx";
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
            mItemID = Request.QueryString.Get("id");
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
                InitializeTabs(top_1_tab, top_1, top_2_tab, top_2, top_3_tab, top_3, top_4_tab, top_4, top_5_tab, top_5, top_6_tab, top_6, top_7_tab, top_7);

                BindGroupByAttribute();
                BindUnitItem();

                if (!_Item.IsNew)
                {
                    txtItemNumber.Text = _Item.ItemNumber;
                    txtItemName.Text = _Item.ItemName;
                    txtQuantityAvailable.Text = _Item.QuantityAvailable.ToString();
                    imgItem.ImageUrl = _Item.ImageURL;
                    imgItemLogo.ImageUrl = _Item.LogoImageURL;
                    imgSizeChart.ImageUrl = _Item.SizeChartURL;
                    chkHideSizeChart.Checked = _Item.HideSizeChart;
                    ddlItemType.SelectedValue = _Item.ItemType;
                    txtStoreDisplayName.Text = _Item.StoreDisplayName;
                    txtSalesDescription.Text = _Item.SalesDescription;
                    txtDetailedDescription.Text = _Item.DetailedDescription;
                    txtBasePrice.Text = Convert.ToString(_Item.BasePrice);
                    txtPurchasePrice.Text = Convert.ToString(_Item.PurchasePrice);
                    ddlAttributeDisplayType.SelectedValue = _Item.AttributeDisplayType;
                    ddlGroupByAttribute.SelectedValue = _Item.GroupByAttributeID;
                    chkIsNonInventory.Checked = _Item.IsNonInventory;
                    ddlUnitItem.SelectedValue = _Item.UnitItemID;
                    chkEnablePersonalization.Checked = _Item.EnablePersonalization;
                    chkEnableSelectableLogo.Checked = _Item.EnableSelectableLogo;
                    cbCompanyInvoiced.Checked = _Item.ItemWebsites.Find(x => x.WebsiteID == CurrentWebsite.WebsiteID) != null ? _Item.ItemWebsites.Find(x => x.WebsiteID == CurrentWebsite.WebsiteID).IsCompanyInvoiced : false;
                    chkRequireApproval.Checked = _Item.ItemWebsites.Find(x => x.WebsiteID == CurrentWebsite.WebsiteID) != null ? _Item.ItemWebsites.Find(x => x.WebsiteID == CurrentWebsite.WebsiteID).RequireApproval : false;

                    txtQuantityPerPerson.Text = _Item.QuantityPerPerson != null && _Item.QuantityPerPerson > 0 ? Convert.ToString(_Item.QuantityPerPerson) : String.Empty;
                    chkHideDetailedDescription.Checked = _Item.HideDetailedDescription;
                    chkExcludeDisplayUser.Checked = _Item.ExcludeDisplayUser;
                    chkRequireLogoSelection.Checked = _Item.RequireLogoSelection;
                    chkDoNotDisplayNIMessage.Checked = _Item.DoNotDisplayNIMessage;

                    cbAllowBackorder.Checked = _Item.AllowBackOrder;
                    cbIsOnline.Checked = _Item.IsOnline;
                    cbInactive.Checked = _Item.InActive;

                    InitializeImage();
                    InitializeLogoImage();
                    InitializeSizeChartImage();
                    BindItemAttribute();
                    BindItemDetail();
                    BindVariations();
                    BindItemPricing();
                    BindSuperceedingItem();
                    BindItemPersonalization();
                    BindItemSelectableLogo();
                    BindTabs();
                    BindWebsiteGroupItem();
                    BindWebsiteGroupItemExclude();

                    btnSave.Text = "Save";
                }
                else
                {
                    btnSave.Text = "Create";
                    btnDelete.Visible = false;
                    top_2_tab.Visible = false;
                    top_3_tab.Visible = false;
                    top_4_tab.Visible = false;
                    top_5_tab.Visible = false;
                    top_6_tab.Visible = false;
                    top_7_tab.Visible = false;
                }
                aCancel.HRef = ReturnURL;

                pnlGroupByAttribute.Visible = ddlAttributeDisplayType.SelectedValue == "list";
                navAttribute.Visible = string.IsNullOrEmpty(_Item.ParentID);
                navVariations.Visible = string.IsNullOrEmpty(_Item.ParentID);
                navSuperceedingItems.Visible = _Item.ParentItem != null || !_Item.IsVariation;
                navItemPricing.Visible = !_Item.IsNew;
                navTabs.Visible = string.IsNullOrEmpty(_Item.ParentID);
            }
            catch (Exception ex)
            {
                WebUtility.DisplayJavascriptMessage(this, ex.Message);
            }
            finally { }
        }

        protected void InitializeImage()
        {
            btnResetImage.Visible = !string.IsNullOrEmpty(imgItem.ImageUrl) && imgItem.ImageUrl != _Item.ImageURL;
            btnRemoveImage.Visible = !string.IsNullOrEmpty(imgItem.ImageUrl) && imgItem.ImageUrl == _Item.ImageURL;
        }

        protected void BindGroupByAttribute()
        {
            try
            {
                ddlGroupByAttribute.DataSource = _Item.Attributes;
                ddlGroupByAttribute.DataBind();
                ddlGroupByAttribute.Items.Insert(0, new ListItem(String.Empty, string.Empty));
            }
            catch (Exception ex)
            {
                WebUtility.DisplayJavascriptMessage(this, ex.Message);
            }
            finally { }
        }

        protected void BindUnitItem()
        {
            try
            {
                List<ImageSolutions.Item.ItemWebsite> ItemWebsites = new List<ImageSolutions.Item.ItemWebsite>();
                ImageSolutions.Item.ItemWebsiteFilter ItemWebsiteFilter = new ImageSolutions.Item.ItemWebsiteFilter();
                ItemWebsiteFilter.WebsiteID = new Database.Filter.StringSearch.SearchFilter();
                ItemWebsiteFilter.WebsiteID.SearchString = CurrentUser.CurrentUserWebSite.WebSite.WebsiteID;
                ItemWebsiteFilter.ParentID = new Database.Filter.StringSearch.SearchFilter();
                ItemWebsiteFilter.ParentID.Operator = Database.Filter.StringSearch.SearchOperator.empty;
                ItemWebsites = ImageSolutions.Item.ItemWebsite.GetItemWebsites(ItemWebsiteFilter);

                ddlUnitItem.DataSource = ItemWebsites.OrderBy(x => x.ItemNumber);
                ddlUnitItem.DataTextField = "ItemNumber";
                ddlUnitItem.DataValueField = "ItemID";
                ddlUnitItem.DataBind();
                ddlUnitItem.Items.Insert(0, new ListItem(String.Empty, string.Empty));
            }
            catch (Exception ex)
            {
                WebUtility.DisplayJavascriptMessage(this, ex.Message);
            }
            finally { }
        }

        protected void BindItemAttribute()
        {
            try
            {
                gvAttributes.DataSource = _Item.Attributes;
                gvAttributes.DataBind();
            }
            catch (Exception ex)
            {
                WebUtility.DisplayJavascriptMessage(this, ex.Message);
            }
            finally { }
        }

        protected void BindTabs()
        {
            try
            {
                gvItemTab.DataSource = _Item.WebsiteTabItems;
                gvItemTab.DataBind();
            }
            catch (Exception ex)
            {
                WebUtility.DisplayJavascriptMessage(this, ex.Message);
            }
            finally { }
        }

        protected void BindItemDetail()
        {
            try
            {
                gvItemDetails.DataSource = _Item.ItemDetails;
                gvItemDetails.DataBind();
            }
            catch (Exception ex)
            {
                WebUtility.DisplayJavascriptMessage(this, ex.Message);
            }
            finally { }
        }

        protected void BindWebsiteGroupItem()
        {
            try
            {
                List<ImageSolutions.Website.WebsiteGroupItem> WebsiteGroupItems = new List<ImageSolutions.Website.WebsiteGroupItem>();
                ImageSolutions.Website.WebsiteGroupItemFilter WebsiteGroupItemFilter = new WebsiteGroupItemFilter();
                WebsiteGroupItemFilter.WebsiteID = new Database.Filter.StringSearch.SearchFilter();
                WebsiteGroupItemFilter.WebsiteID.SearchString = CurrentWebsite.WebsiteID;
                WebsiteGroupItemFilter.ItemID = new Database.Filter.StringSearch.SearchFilter();
                WebsiteGroupItemFilter.ItemID.SearchString = _Item.ItemID;
                WebsiteGroupItems = ImageSolutions.Website.WebsiteGroupItem.GetWebsiteGroupItems(WebsiteGroupItemFilter);
                gvWebsiteGroupItem.DataSource = WebsiteGroupItems;
                gvWebsiteGroupItem.DataBind();
            }
            catch (Exception ex)
            {
                WebUtility.DisplayJavascriptMessage(this, ex.Message);
            }
            finally { }
        }

        protected void BindWebsiteGroupItemExclude()
        {
            try
            {
                List<ImageSolutions.Website.WebsiteGroupItemExclude> WebsiteGroupItemExcludes = new List<ImageSolutions.Website.WebsiteGroupItemExclude>();
                ImageSolutions.Website.WebsiteGroupItemExcludeFilter WebsiteGroupItemExcludeFilter = new WebsiteGroupItemExcludeFilter();
                WebsiteGroupItemExcludeFilter.WebsiteID = new Database.Filter.StringSearch.SearchFilter();
                WebsiteGroupItemExcludeFilter.WebsiteID.SearchString = CurrentWebsite.WebsiteID;
                WebsiteGroupItemExcludeFilter.ItemID = new Database.Filter.StringSearch.SearchFilter();
                WebsiteGroupItemExcludeFilter.ItemID.SearchString = _Item.ItemID;
                WebsiteGroupItemExcludes = ImageSolutions.Website.WebsiteGroupItemExclude.GetWebsiteGroupItemExcludes(WebsiteGroupItemExcludeFilter);
                gvWebsiteGroupItemExclude.DataSource = WebsiteGroupItemExcludes;
                gvWebsiteGroupItemExclude.DataBind();
            }
            catch (Exception ex)
            {
                WebUtility.DisplayJavascriptMessage(this, ex.Message);
            }
            finally { }
        }

        protected void BindItemPricing()
        {
            gvItemPricing.DataSource = _Item.ItemPricings;
            gvItemPricing.DataBind();
        }

        protected void BindVariations()
        {
            try
            {
                this.gvVariations.DataSource = _Item.Variations;
                this.gvVariations.DataBind();
            }
            catch (Exception ex)
            {
                WebUtility.DisplayJavascriptMessage(this, ex.Message);
            }
            finally { }
        }

        protected void BindSuperceedingItem()
        {
            try
            {
                gvSuperceedingItem.DataSource = _Item.SuperceedingItems;
                gvSuperceedingItem.DataBind();

                btnUpdateSort.Visible = gvSuperceedingItem.Rows.Count > 0;
            }
            catch (Exception ex)
            {
                WebUtility.DisplayJavascriptMessage(this, ex.Message);
            }
            finally { }
        }
        protected void BindItemPersonalization()
        {
            try
            {
                gvItemPersonalization.DataSource = _Item.ItemPersonalizations;
                gvItemPersonalization.DataBind();

                btnUpdateSortPersonalization.Visible = gvItemPersonalization.Rows.Count > 0;
            }
            catch (Exception ex)
            {
                WebUtility.DisplayJavascriptMessage(this, ex.Message);
            }
            finally { }
        }


        protected void BindItemSelectableLogo()
        {
            try
            {
                gvItemSelectableLogo.DataSource = _Item.ItemSelectableLogos;
                gvItemSelectableLogo.DataBind();
            }
            catch (Exception ex)
            {
                WebUtility.DisplayJavascriptMessage(this, ex.Message);
            }
            finally { }
        }
        protected void gvSuperceedingItem_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    string strSuperceedingItemID = gvSuperceedingItem.DataKeys[e.Row.RowIndex].Value.ToString();

                    Button btnUp = (Button)e.Row.FindControl("btnUp");
                    Button btnDown = (Button)e.Row.FindControl("btnDown");

                    ImageSolutions.Item.SuperceedingItem objSuperceedingItem = new ImageSolutions.Item.SuperceedingItem(strSuperceedingItemID);

                    List<ImageSolutions.Item.SuperceedingItem> objSuperceedingItems = new List<ImageSolutions.Item.SuperceedingItem>();
                    ImageSolutions.Item.SuperceedingItemFilter objSuperceedingItemFilter = new SuperceedingItemFilter();
                    objSuperceedingItemFilter.ItemID = new Database.Filter.StringSearch.SearchFilter();
                    objSuperceedingItemFilter.ItemID.SearchString = Convert.ToString(mItemID);
                    objSuperceedingItems = ImageSolutions.Item.SuperceedingItem.GetSuperceedingItems(objSuperceedingItemFilter);

                    objSuperceedingItem.Sort = e.Row.RowIndex + 1;
                    objSuperceedingItem.Update();

                    btnUp.Visible = Convert.ToInt32(objSuperceedingItem.Sort) != 1;
                    btnDown.Visible = objSuperceedingItems.Count != Convert.ToInt32(objSuperceedingItem.Sort);

                    TextBox txtSort = (TextBox)e.Row.FindControl("txtSort");
                    txtSort.Text = Convert.ToString(objSuperceedingItem.Sort);
                }
            }
            catch (Exception ex)
            {
                WebUtility.DisplayJavascriptMessage(this, ex.Message);
            }
        }

        protected void gvSuperceedingItem_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "MoveUp")
            {
                string strSuperceedingItemID = Convert.ToString(e.CommandArgument);
                ImageSolutions.Item.SuperceedingItem objSuperceedingItem = new ImageSolutions.Item.SuperceedingItem(strSuperceedingItemID);

                ImageSolutions.Item.SuperceedingItem objSuperceedingItemMoveUp = new ImageSolutions.Item.SuperceedingItem();
                ImageSolutions.Item.SuperceedingItemFilter objSuperceedingItemFilter = new ImageSolutions.Item.SuperceedingItemFilter();
                objSuperceedingItemFilter.ItemID = new Database.Filter.StringSearch.SearchFilter();
                objSuperceedingItemFilter.ItemID.SearchString = mItemID;
                objSuperceedingItemFilter.Sort = objSuperceedingItem.Sort - 1;
                objSuperceedingItemMoveUp = ImageSolutions.Item.SuperceedingItem.GetSuperceedingItem(objSuperceedingItemFilter);

                objSuperceedingItemMoveUp.Sort = objSuperceedingItemMoveUp.Sort + 1;
                objSuperceedingItemMoveUp.Update();

                objSuperceedingItem.Sort = objSuperceedingItem.Sort - 1;
                objSuperceedingItem.Update();

                BindSuperceedingItem();
            }
            else if (e.CommandName == "MoveDown")
            {
                string strSuperceedingItemID = Convert.ToString(e.CommandArgument);
                ImageSolutions.Item.SuperceedingItem objSuperceedingItem = new ImageSolutions.Item.SuperceedingItem(strSuperceedingItemID);

                ImageSolutions.Item.SuperceedingItem objSuperceedingItemMoveDown = new ImageSolutions.Item.SuperceedingItem();
                ImageSolutions.Item.SuperceedingItemFilter objSuperceedingItemFilter = new ImageSolutions.Item.SuperceedingItemFilter();
                objSuperceedingItemFilter.ItemID = new Database.Filter.StringSearch.SearchFilter();
                objSuperceedingItemFilter.ItemID.SearchString = mItemID;
                objSuperceedingItemFilter.Sort = objSuperceedingItem.Sort + 1;
                objSuperceedingItemMoveDown = ImageSolutions.Item.SuperceedingItem.GetSuperceedingItem(objSuperceedingItemFilter);

                objSuperceedingItemMoveDown.Sort = objSuperceedingItemMoveDown.Sort - 1;
                objSuperceedingItemMoveDown.Update();

                objSuperceedingItem.Sort = objSuperceedingItem.Sort + 1;
                objSuperceedingItem.Update();

                BindSuperceedingItem();
            }
        }

        protected void btnUpdateSort_Click(object sender, EventArgs e)
        {
            foreach (GridViewRow _Row in this.gvSuperceedingItem.Rows)
            {
                string strSuperceedingItemID = gvSuperceedingItem.DataKeys[_Row.RowIndex].Value.ToString();

                TextBox txtSort = (TextBox)_Row.FindControl("txtSort");

                ImageSolutions.Item.SuperceedingItem objSuperceedingItem = new ImageSolutions.Item.SuperceedingItem(strSuperceedingItemID);
                objSuperceedingItem.Sort = Convert.ToInt32(txtSort.Text);
                objSuperceedingItem.Update();
            }

            BindSuperceedingItem();
        }

        protected void ddlAttributeDisplayType_SelectedIndexChanged(object sender, EventArgs e)
        {
            pnlGroupByAttribute.Visible = ddlAttributeDisplayType.SelectedValue == "list";
        }
        
        protected void btnSave_Click(object sender, EventArgs e)
        {
            SqlConnection objConn = null;
            SqlTransaction objTran = null;
            ImageSolutions.Item.ItemWebsite objItemWebsite = null;
            bool blnReturn = false;

            try
            {
                objConn = new SqlConnection(Database.DefaultConnectionString);
                objConn.Open();
                objTran = objConn.BeginTransaction();

                _Item.ItemNumber = txtItemNumber.Text;
                _Item.ItemName = txtItemName.Text;
                _Item.QuantityAvailable = Convert.ToInt32(txtQuantityAvailable.Text.Trim());
                _Item.ImageURL = imgItem.ImageUrl;
                _Item.LogoImageURL = imgItemLogo.ImageUrl;
                _Item.SizeChartURL = imgSizeChart.ImageUrl;
                _Item.HideSizeChart = chkHideSizeChart.Checked;
                _Item.ItemType = ddlItemType.SelectedValue;
                _Item.StoreDisplayName = txtStoreDisplayName.Text;
                _Item.SalesDescription = txtSalesDescription.Text;
                _Item.DetailedDescription = txtDetailedDescription.Text;
                if (!string.IsNullOrEmpty(txtBasePrice.Text)) _Item.BasePrice = Convert.ToDouble(txtBasePrice.Text);
                if (!string.IsNullOrEmpty(txtPurchasePrice.Text)) _Item.PurchasePrice = Convert.ToDouble(txtPurchasePrice.Text);
                _Item.IsNonInventory = chkIsNonInventory.Checked;
                //_Item.IsCompanyInvoiced = cbCompanyInvoiced.Checked;
                _Item.UnitItemID = Convert.ToString(ddlUnitItem.SelectedValue);
                _Item.AllowBackOrder = cbAllowBackorder.Checked;
                _Item.IsOnline = cbIsOnline.Checked;
                _Item.InActive = cbInactive.Checked;

                _Item.QuantityPerPerson = !string.IsNullOrEmpty(txtQuantityPerPerson.Text) && Convert.ToDecimal(txtQuantityPerPerson.Text) > 0 ? Convert.ToDecimal(txtQuantityPerPerson.Text.Trim()) : (decimal?)null;
                _Item.HideDetailedDescription = chkHideDetailedDescription.Checked;
                _Item.ExcludeDisplayUser = chkExcludeDisplayUser.Checked;
                _Item.RequireLogoSelection = chkRequireLogoSelection.Checked;
                _Item.DoNotDisplayNIMessage = chkDoNotDisplayNIMessage.Checked;

                _Item.GroupByAttributeID = ddlGroupByAttribute.SelectedValue;
                _Item.AttributeDisplayType = ddlAttributeDisplayType.SelectedValue;
                _Item.EnablePersonalization = chkEnablePersonalization.Checked;
                _Item.EnableSelectableLogo= chkEnableSelectableLogo.Checked;

                if (_Item.IsNew)
                {
                    _Item.CreatedBy = CurrentUser.UserInfoID;
                    _Item.VendorInventory = 0;
                    _Item.Create(objConn, objTran);

                    objItemWebsite = new ImageSolutions.Item.ItemWebsite();
                    objItemWebsite.ItemID = _Item.ItemID;
                    objItemWebsite.WebsiteID = CurrentUser.CurrentUserWebSite.WebSite.WebsiteID;
                    objItemWebsite.IsCompanyInvoiced = cbCompanyInvoiced.Checked;
                    objItemWebsite.RequireApproval = chkRequireApproval.Checked;
                    objItemWebsite.CreatedBy = CurrentUser.UserInfoID;
                    blnReturn = objItemWebsite.Create(objConn, objTran);
                    ReturnURL = "/admin/item.aspx?id=" + _Item.ItemID + "&tab=2";
                }
                else
                {
                    blnReturn = _Item.Update(objConn, objTran);

                    ItemWebsite ItemWebsite = new ItemWebsite();
                    ItemWebsiteFilter ItemWebsiteFilter = new ItemWebsiteFilter();
                    ItemWebsiteFilter.ItemID = new Database.Filter.StringSearch.SearchFilter();
                    ItemWebsiteFilter.ItemID.SearchString = _Item.ItemID;
                    ItemWebsiteFilter.WebsiteID = new Database.Filter.StringSearch.SearchFilter();
                    ItemWebsiteFilter.WebsiteID.SearchString = CurrentWebsite.WebsiteID;
                    ItemWebsite = ItemWebsite.GetItemWebsite(ItemWebsiteFilter);

                    ItemWebsite.IsCompanyInvoiced = cbCompanyInvoiced.Checked;
                    ItemWebsite.RequireApproval = chkRequireApproval.Checked;
                    ItemWebsite.Update(objConn, objTran);
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

                objItemWebsite = null;
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
                blnReturn = _Item.Delete();
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
            if(fuItemImage.PostedFile.ContentLength > 0)
            {
                Stream objStream = fuItemImage.PostedFile.InputStream;
                string strExtension = Path.GetExtension(fuItemImage.PostedFile.FileName);

                string strFilename = string.Format("Item/{0}/{1}{2}", DateTime.UtcNow.ToString("yyyyMM"), DateTime.UtcNow.ToString("yyyyMMddHHmmssffffff"), strExtension);
                SaveFile(objStream, strFilename);

                string strImagePath = string.Format("{0}{1}", Convert.ToString(ConfigurationManager.AppSettings["ImageURL"]), strFilename);
                imgItem.ImageUrl = strImagePath;
            }
            InitializeImage();
        }

        protected void btnResetImage_Click(object sender, EventArgs e)
        {
            ImageSolutions.Item.Item Item = new ImageSolutions.Item.Item(Convert.ToString(mItemID));
            imgItem.ImageUrl = Item.ImageURL;
            InitializeImage();
        }
        protected void btnRemoveImage_Click(object sender, EventArgs e)
        {
            imgItem.ImageUrl = string.Empty;
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

        protected void gvItemPersonalization_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "MoveUp")
            {
                string strItemPersonalizationID = Convert.ToString(e.CommandArgument);
                ImageSolutions.Item.ItemPersonalization objItemPersonalization = new ImageSolutions.Item.ItemPersonalization(strItemPersonalizationID);

                ImageSolutions.Item.ItemPersonalization objItemPersonalizationMoveUp = new ImageSolutions.Item.ItemPersonalization();
                ImageSolutions.Item.ItemPersonalizationFilter objItemPersonalizationFilter = new ImageSolutions.Item.ItemPersonalizationFilter();
                objItemPersonalizationFilter.ItemID = new Database.Filter.StringSearch.SearchFilter();
                objItemPersonalizationFilter.ItemID.SearchString = mItemID;
                objItemPersonalizationFilter.Sort = objItemPersonalization.Sort - 1;
                objItemPersonalizationMoveUp = ImageSolutions.Item.ItemPersonalization.GetItemPersonalization(objItemPersonalizationFilter);

                objItemPersonalizationMoveUp.Sort = objItemPersonalizationMoveUp.Sort + 1;
                objItemPersonalizationMoveUp.Update();

                objItemPersonalization.Sort = objItemPersonalization.Sort - 1;
                objItemPersonalization.Update();

                BindItemPersonalization();
            }
            else if (e.CommandName == "MoveDown")
            {
                string strItemPersonalizationID = Convert.ToString(e.CommandArgument);
                ImageSolutions.Item.ItemPersonalization objItemPersonalization = new ImageSolutions.Item.ItemPersonalization(strItemPersonalizationID);

                ImageSolutions.Item.ItemPersonalization objItemPersonalizationMoveDown = new ImageSolutions.Item.ItemPersonalization();
                ImageSolutions.Item.ItemPersonalizationFilter objItemPersonalizationFilter = new ImageSolutions.Item.ItemPersonalizationFilter();
                objItemPersonalizationFilter.ItemID = new Database.Filter.StringSearch.SearchFilter();
                objItemPersonalizationFilter.ItemID.SearchString = mItemID;
                objItemPersonalizationFilter.Sort = objItemPersonalization.Sort + 1;
                objItemPersonalizationMoveDown = ImageSolutions.Item.ItemPersonalization.GetItemPersonalization(objItemPersonalizationFilter);

                objItemPersonalizationMoveDown.Sort = objItemPersonalizationMoveDown.Sort - 1;
                objItemPersonalizationMoveDown.Update();

                objItemPersonalization.Sort = objItemPersonalization.Sort + 1;
                objItemPersonalization.Update();

                BindItemPersonalization();
            }
        }

        protected void gvItemPersonalization_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    string strItemPersonalizationID = gvItemPersonalization.DataKeys[e.Row.RowIndex].Value.ToString();

                    Button btnUp = (Button)e.Row.FindControl("btnUp");
                    Button btnDown = (Button)e.Row.FindControl("btnDown");

                    ImageSolutions.Item.ItemPersonalization objItemPersonalization = new ImageSolutions.Item.ItemPersonalization(strItemPersonalizationID);

                    List<ImageSolutions.Item.ItemPersonalization> objItemPersonalizations = new List<ImageSolutions.Item.ItemPersonalization>();
                    ImageSolutions.Item.ItemPersonalizationFilter objItemPersonalizationFilter = new ItemPersonalizationFilter();
                    objItemPersonalizationFilter.ItemID = new Database.Filter.StringSearch.SearchFilter();
                    objItemPersonalizationFilter.ItemID.SearchString = Convert.ToString(mItemID);
                    objItemPersonalizations = ImageSolutions.Item.ItemPersonalization.GetItemPersonalizations(objItemPersonalizationFilter);

                    objItemPersonalization.Sort = e.Row.RowIndex + 1;
                    objItemPersonalization.Update();

                    btnUp.Visible = Convert.ToInt32(objItemPersonalization.Sort) != 1;
                    btnDown.Visible = objItemPersonalizations.Count != Convert.ToInt32(objItemPersonalization.Sort);

                    TextBox txtSortPersonalization = (TextBox)e.Row.FindControl("txtSortPersonalization");
                    txtSortPersonalization.Text = Convert.ToString(objItemPersonalization.Sort);
                }
            }
            catch (Exception ex)
            {
                WebUtility.DisplayJavascriptMessage(this, ex.Message);
            }
        }

        protected void btnUpdateSortPersonalization_Click(object sender, EventArgs e)
        {
            foreach (GridViewRow _Row in this.gvItemPersonalization.Rows)
            {
                string strItemPersonalizationID = gvItemPersonalization.DataKeys[_Row.RowIndex].Value.ToString();

                TextBox txtSortPersonalization = (TextBox)_Row.FindControl("txtSortPersonalization");

                ImageSolutions.Item.ItemPersonalization objItemPersonalization = new ImageSolutions.Item.ItemPersonalization(strItemPersonalizationID);
                objItemPersonalization.Sort = Convert.ToInt32(txtSortPersonalization.Text);
                objItemPersonalization.Update();
            }

            BindItemPersonalization();
        }

        protected void gvItemSelectableLogo_RowCommand(object sender, GridViewCommandEventArgs e)
        {

        }

        protected void gvItemSelectableLogo_RowDataBound(object sender, GridViewRowEventArgs e)
        {

        }

        protected void btnResetLogoImage_Click(object sender, EventArgs e)
        {
            ImageSolutions.Item.Item Item = new ImageSolutions.Item.Item(Convert.ToString(mItemID));
            imgItem.ImageUrl = Item.ImageURL;
            InitializeImage();
        }

        protected void btnRemoveLogoImage_Click(object sender, EventArgs e)
        {
            imgItemLogo.ImageUrl = string.Empty;
            InitializeImage();
        }

        protected void btnUploadLogoImage_Click(object sender, EventArgs e)
        {
            if (fuLogoItemImage.PostedFile.ContentLength > 0)
            {
                Stream objStream = fuLogoItemImage.PostedFile.InputStream;
                string strExtension = Path.GetExtension(fuLogoItemImage.PostedFile.FileName);

                string strFilename = string.Format("Item/{0}/{1}{2}", DateTime.UtcNow.ToString("yyyyMM"), DateTime.UtcNow.ToString("yyyyMMddHHmmssffffff"), strExtension);
                SaveFile(objStream, strFilename);

                string strImagePath = string.Format("{0}{1}", Convert.ToString(ConfigurationManager.AppSettings["ImageURL"]), strFilename);
                imgItemLogo.ImageUrl = strImagePath;
            }
            InitializeImage();
        }

        protected void InitializeLogoImage()
        {
            btnResetLogoImage.Visible = !string.IsNullOrEmpty(imgItemLogo.ImageUrl) && imgItemLogo.ImageUrl != _Item.LogoImageURL;
            btnRemoveLogoImage.Visible = !string.IsNullOrEmpty(imgItemLogo.ImageUrl) && imgItemLogo.ImageUrl == _Item.LogoImageURL;
        }

        protected void InitializeSizeChartImage()
        {
            btnResetSizeChartImage.Visible = !string.IsNullOrEmpty(imgSizeChart.ImageUrl) && imgSizeChart.ImageUrl != _Item.SizeChartURL;
            btnRemoveSizeChartImage.Visible = !string.IsNullOrEmpty(imgSizeChart.ImageUrl) && imgSizeChart.ImageUrl == _Item.SizeChartURL;
        }
        protected void btnUploadSizeChartImage_Click(object sender, EventArgs e)
        {
            if (fuSizeChartImage.PostedFile.ContentLength > 0)
            {
                Stream objStream = fuSizeChartImage.PostedFile.InputStream;
                string strExtension = Path.GetExtension(fuSizeChartImage.PostedFile.FileName);

                string strFilename = string.Format("Item/{0}/{1}{2}", DateTime.UtcNow.ToString("yyyyMM"), DateTime.UtcNow.ToString("yyyyMMddHHmmssffffff"), strExtension);
                SaveFile(objStream, strFilename);

                string strImagePath = string.Format("{0}{1}", Convert.ToString(ConfigurationManager.AppSettings["ImageURL"]), strFilename);
                imgSizeChart.ImageUrl = strImagePath;
            }
            InitializeSizeChartImage();
        }

        protected void btnResetSizeChartImage_Click(object sender, EventArgs e)
        {
            ImageSolutions.Item.Item Item = new ImageSolutions.Item.Item(Convert.ToString(mItemID));
            imgSizeChart.ImageUrl = Item.SizeChartURL;
            InitializeSizeChartImage();
        }

        protected void btnRemoveSizeChartImage_Click(object sender, EventArgs e)
        {
            imgSizeChart.ImageUrl = string.Empty;
            InitializeSizeChartImage();
        }

        protected void btnExportItemDetail_Click(object sender, EventArgs e)
        {
            string strSQL = string.Empty;

            try
            {
                CreateExportExcel();

                //string strPath = Server.MapPath("\\Export\\ProductDetail\\" + DateTime.UtcNow.ToString("yyyyMM") + "\\");
                //if (!Directory.Exists(strPath))
                //{
                //    Directory.CreateDirectory(strPath);
                //}

                //string strFileExportPath = Server.MapPath(string.Format("\\Export\\ProductDetail\\{0}\\ProductDetail_{1}.csv", DateTime.UtcNow.ToString("yyyyMM"), DateTime.UtcNow.ToString("yyyyMMddHHmmssffff")));
                //CreateExportCSV(strFileExportPath);

                //Response.ContentType = "text/csv";
                //Response.AppendHeader("Content-Disposition", "attachment; filename=" + System.IO.Path.GetFileName(strFileExportPath));
                //Response.WriteFile(strFileExportPath);

                //HttpContext.Current.Response.Flush();
                //HttpContext.Current.Response.SuppressContent = true;
                //HttpContext.Current.ApplicationInstance.CompleteRequest();
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
                objReturn.Append(string.Format("{0},{1},{2},{3},{4},{5}"
                    , "ID"
                    , "ItemNumber"
                    , "AttributeHeader"
                    , "HeaderSort"
                    , "AttributeLine"
                    , "LineSort"));
                objReturn.AppendLine();

                strDBSQL = string.Format(@"
--SELECT idv.ItemDetailValueID as ID
--	, i.ItemNumber as ItemNumber
--	, id.Attribute as AttributeHeader
--	, id.Sort as HeaderSort
--	, idv.Value as AttributeLine
--	, idv.Sort as LineSort
--FROM ItemDetail (NOLOCK) id
--Inner Join ItemDetailValue (NOLOCK) idv on idv.ItemDetailID = id.ItemDetailID
--Inner Join Item (NOLOCK) i on i.ItemID = id.ItemID
--WHERE i.ItemID = {0} 

SELECT idv.ItemDetailValueID as ID
	, i.ItemNumber as ItemNumber
	, id.Attribute as AttributeHeader
	, id.Sort as HeaderSort
	, idv.Value as AttributeLine
	, idv.Sort as LineSort
	, id.ItemDetailID
	, idv.ItemDetailValueID
FROM (
	SELECT Attribute, ItemID, ItemDetailID, ROW_NUMBER() OVER(ORDER BY CASE WHEN Sort = 0 THEN 99 ELSE ISNULL(Sort,99) END, ItemDetailID) AS Sort
	FROM ItemDetail (NOLOCK) d2
	WHERE d2.ItemID = {0}
) id
Cross Apply (
	SELECT ItemDetailValueID, Value, ROW_NUMBER() OVER(ORDER BY CASE WHEN Sort = 0 THEN 99 ELSE ISNULL(Sort,99) END, ItemDetailValueID) AS Sort
	FROM ItemDetailValue (NOLOCK) idv2
	WHERE idv2.ItemDetailID = id.ItemDetailID
) idv 
Inner Join Item (NOLOCK) i on i.ItemID = id.ItemID
"
                    , Database.HandleQuote(_Item.ItemID));


                objRead = Database.GetDataReader(strDBSQL);

                while (objRead.Read())
                {
                    objReturn.Append(string.Format("{0},{1},{2},{3},{4},{5}"
                        , Convert.ToString(objRead["ID"])
                        , Convert.ToString(objRead["ItemNumber"])
                        , Convert.ToString(objRead["AttributeHeader"])
                        , Convert.ToString(objRead["HeaderSort"])
                        , Convert.ToString(objRead["AttributeLine"])
                        , Convert.ToString(objRead["LineSort"]))
                    );
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
        protected void CreateExportExcel()
        {
            System.Data.OleDb.OleDbConnection OleDbConnection = null;
            System.Data.OleDb.OleDbCommand OleDbCommand = null;
            string strSQL = string.Empty;
            Hashtable dicParam = null;

            SqlDataReader objRead = null;
            string strDBSQL = string.Empty;

            //StringBuilder objReturn = new StringBuilder();

            try
            {
                string strPath = Server.MapPath("\\Export\\ProductDetail\\" + DateTime.UtcNow.ToString("yyyyMM") + "\\");
                if (!Directory.Exists(strPath))
                {
                    Directory.CreateDirectory(strPath);
                }

                string strTemplateFileName = "ProductDetailTemplate.xlsx";
                string strTemplateFilePath = Server.MapPath(string.Format("\\Import\\Template\\{0}", strTemplateFileName));
                string strFileExportPath = Server.MapPath(string.Format("\\Export\\ProductDetail\\{0}\\ProductDetail_{1}.xlsx", DateTime.UtcNow.ToString("yyyyMM"), DateTime.UtcNow.ToString("yyyyMMddHHmmssffff")));
                File.Copy(strTemplateFilePath, strFileExportPath, true);

                OleDbConnection = new System.Data.OleDb.OleDbConnection("Provider=Microsoft.ACE.OLEDB.12.0;Data Source='" + strFileExportPath + "';Extended Properties=Excel 12.0;");
                OleDbConnection.Open();
                OleDbCommand = new System.Data.OleDb.OleDbCommand();
                OleDbCommand.Connection = OleDbConnection;

                //Header
                //objReturn.Append(string.Format("{0},{1},{2},{3},{4},{5}"
                //    , "ID"
                //    , "Item Number"
                //    , "Attribute Header"
                //    , "Header Sort"
                //    , "Attribute Line"
                //    , "Line Sort"));
                //objReturn.AppendLine();

                //DataTable dt = new DataTable();

                //dt.Columns.Add("ID");
                //dt.Columns.Add("Item Number");
                //dt.Columns.Add("Attribute Header");
                //dt.Columns.Add("Header Sort");
                //dt.Columns.Add("Attribute Line");
                //dt.Columns.Add("Line Sort");

                strDBSQL = string.Format(@"
--SELECT idv.ItemDetailValueID as ID
--	, i.ItemNumber as ItemNumber
--	, id.Attribute as AttributeHeader
--	, id.Sort as HeaderSort
--	, idv.Value as AttributeLine
--	, idv.Sort as LineSort
--FROM ItemDetail (NOLOCK) id
--Inner Join ItemDetailValue (NOLOCK) idv on idv.ItemDetailID = id.ItemDetailID
--Inner Join Item (NOLOCK) i on i.ItemID = id.ItemID
--WHERE i.ItemID = {0} 

SELECT idv.ItemDetailValueID as ID
	, i.ItemNumber as ItemNumber
	, id.Attribute as AttributeHeader
	, id.Sort as HeaderSort
	, idv.Value as AttributeLine
	, idv.Sort as LineSort
	, id.ItemDetailID
	, idv.ItemDetailValueID
FROM (
	SELECT Attribute, ItemID, ItemDetailID, ROW_NUMBER() OVER(ORDER BY CASE WHEN Sort = 0 THEN 99 ELSE ISNULL(Sort,99) END, ItemDetailID) AS Sort
	FROM ItemDetail (NOLOCK) d2
	WHERE d2.ItemID = {0}
) id
Cross Apply (
	SELECT ItemDetailValueID, Value, ROW_NUMBER() OVER(ORDER BY CASE WHEN Sort = 0 THEN 99 ELSE ISNULL(Sort,99) END, ItemDetailValueID) AS Sort
	FROM ItemDetailValue (NOLOCK) idv2
	WHERE idv2.ItemDetailID = id.ItemDetailID
) idv 
Inner Join Item (NOLOCK) i on i.ItemID = id.ItemID
"
                    , Database.HandleQuote(_Item.ItemID));


                objRead = Database.GetDataReader(strDBSQL);

                while (objRead.Read())
                {
                    dicParam = new Hashtable();
                    dicParam["ID"] = Convert.ToString(objRead["ID"]);
                    dicParam["Item_Number"] = Convert.ToString(objRead["ItemNumber"]);
                    dicParam["Attribute_Header"] = Convert.ToString(objRead["AttributeHeader"]);
                    dicParam["Header_Sort"] = Convert.ToString(objRead["HeaderSort"]);
                    dicParam["Attribute_Line"] = Convert.ToString(objRead["AttributeLine"]);
                    dicParam["Line_Sort"] = Convert.ToString(objRead["LineSort"]);

                    //DataRow objRow = dt.NewRow();
                    //objRow["ID"] = Convert.ToString(objRead["ID"]);
                    //objRow["Item Number"] = Convert.ToString(objRead["ItemNumber"]);
                    //objRow["Attribute Header"] = Convert.ToString(objRead["AttributeHeader"]);
                    //objRow["Header Sort"] = Convert.ToString(objRead["HeaderSort"]);
                    //objRow["Attribute Line"] = Convert.ToString(objRead["AttributeLine"]);
                    //objRow["Line Sort"] = Convert.ToString(objRead["LineSort"]);
                    //dt.Rows.Add(objRow);

                    strSQL = Database.GetInsertSQL(dicParam, "[product_detail$]", false);
                    OleDbCommand.CommandText = strSQL;
                    OleDbCommand.ExecuteNonQuery();
                }

                OleDbConnection.Close();
                Thread.Sleep(4000);

                Response.ContentType = "application/vnd.ms-excel";
                Response.AppendHeader("Content-Disposition", "attachment; filename=" + System.IO.Path.GetFileName(strFileExportPath));
                Response.WriteFile(strFileExportPath);
                Response.Flush();
                Response.End();


                ////Double dimensional array to keep style name and style
                //string[,] styles = { { "text", "\\@;" },
                //               { "text", "\\@;" },
                //               { "text", "\\@;" },
                //               { "text", "\\@;" },
                //               { "text", "\\@;" },
                //               { "text", "\\@;" } };

                ////Dummy GridView to hold data to be exported in excel
                //System.Web.UI.WebControls.GridView gvExport = new System.Web.UI.WebControls.GridView();
                //gvExport.AllowPaging = false;
                //gvExport.DataSource = dt;
                //gvExport.DataBind();

                //StringWriter sw = new StringWriter();
                //HtmlTextWriter hw = new HtmlTextWriter(sw);
                //int cnt = styles.Length / 2;

                //for (int i = 0; i < gvExport.Rows.Count; i++)
                //{
                //    for (int j = 0; j < cnt; j++)
                //    {
                //        //Apply style to each cell
                //        gvExport.Rows[i].Cells[j].Attributes.Add("class", styles[j, 0]);
                //    }
                //}

                //gvExport.RenderControl(hw);
                //StringBuilder style = new StringBuilder();
                //style.Append("<style>");
                //for (int j = 0; j < cnt; j++)
                //{
                //    style.Append("." + styles[j, 0] + " { mso-number-format:" + styles[j, 1] + " }");
                //}

                //style.Append("</style>");
                //Response.Clear();
                //Response.Buffer = true;
                //Response.AddHeader("content-disposition", "attachment;filename=order_export.xls"); Response.Charset = "";
                //Response.ContentType = "application/vnd.ms-excel";
                //Response.Write(style.ToString());
                ////string headerTable = @"<Table><tr><td></td><td>Report Header</td></tr><tr><td>second</td></tr></Table>";
                ////Response.Output.Write(headerTable);
                //Response.Output.Write(sw.ToString());
                //Response.Flush();
                //Response.End();

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