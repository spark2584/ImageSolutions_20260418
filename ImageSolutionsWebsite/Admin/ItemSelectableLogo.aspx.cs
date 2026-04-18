using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ImageSolutionsWebsite.Admin
{
    public partial class ItemSelectableLogo : BasePageAdminUserWebSiteAuth
    {
        protected string mItemID = string.Empty;
        protected string mItemSelectableLogoID = string.Empty;
        private ImageSolutions.Item.ItemSelectableLogo mItemSelectableLogo = null;
        protected ImageSolutions.Item.ItemSelectableLogo _ItemSelectableLogo
        {
            get
            {
                if (mItemSelectableLogo == null)
                {
                    if (string.IsNullOrEmpty(mItemSelectableLogoID))
                        mItemSelectableLogo = new ImageSolutions.Item.ItemSelectableLogo();
                    else
                        mItemSelectableLogo = new ImageSolutions.Item.ItemSelectableLogo(mItemSelectableLogoID);
                }
                return mItemSelectableLogo;
            }
        }
        protected string ReturnURL
        {
            get
            {
                if (ViewState["ReturnURL"] == null)
                    return "/Admin/Item.aspx?id=" + mItemID + "&tab=7";
                else
                    return ViewState["ReturnURL"].ToString();
            }
            set
            {
                ViewState["ReturnURL"] = value;
            }
        }

        private ImageSolutions.Item.Item mItem = null;
        protected ImageSolutions.Item.Item _Item
        {
            get
            {
                if (mItem == null)
                {
                    mItem = new ImageSolutions.Item.Item(mItemID);
                }
                return mItem;
            }
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            mItemID = Request.QueryString.Get("itemid");
            mItemSelectableLogoID = Request.QueryString.Get("id");

            if (!Page.IsPostBack)
            {
                InitializePage();
            }
        }

        public void InitializePage()
        {
            try
            {
                BindSelectableLogo();
                BindItemSelectableLogoExcludeAttribute();
                if (!_ItemSelectableLogo.IsNew)
                {
                    ddlSelectableLogo.SelectedValue = _ItemSelectableLogo.SelectableLogoID;
                    txtPositionXPercent.Text = Convert.ToString(_ItemSelectableLogo.PositionXPercent);
                    txtPositionYPercent.Text = Convert.ToString(_ItemSelectableLogo.PositionYPercent);
                    txtWidth.Text = Convert.ToString(_ItemSelectableLogo.Width);
                    imgItem.ImageUrl = Convert.ToString(_ItemSelectableLogo.ImageURL);
                    btnSave.Text = "Save";
                }
                else
                {
                    btnSave.Text = "Create";
                    btnDelete.Visible = false;
                }

                pnlPreview.Visible = !string.IsNullOrEmpty(_ItemSelectableLogo.ImageURL);
                GenerateImage();

                aCancel.HRef = ReturnURL;

                top_2_tab.Visible = !_ItemSelectableLogo.IsNew;
                top_3_tab.Visible = !_ItemSelectableLogo.IsNew;

                BindWebsiteGroupItemSelectableLogo();
            }
            catch (Exception ex)
            {
                WebUtility.DisplayJavascriptMessage(this, ex.Message);
            }
            finally { }
        }
        protected void BindSelectableLogo()
        {
            try
            {
                List<ImageSolutions.SelectableLogo.SelectableLogo> SelectableLogos = new List<ImageSolutions.SelectableLogo.SelectableLogo>();
                SelectableLogos = ImageSolutions.SelectableLogo.SelectableLogo.GetSelectableLogos();

                ddlSelectableLogo.DataSource = SelectableLogos.OrderBy(x => x.Name);
                ddlSelectableLogo.DataBind();
                ddlSelectableLogo.Items.Insert(0, new ListItem(String.Empty, string.Empty));

            }
            catch (Exception ex)
            {
                WebUtility.DisplayJavascriptMessage(this, ex.Message);
            }
            finally { }
        }

        protected void BindItemSelectableLogoExcludeAttribute()
        {
            try
            {
                List<ImageSolutions.Item.ItemSelectableLogoExcludeAttribute> ItemSelectableLogoExcludeAttributes = new List<ImageSolutions.Item.ItemSelectableLogoExcludeAttribute>();
                ImageSolutions.Item.ItemSelectableLogoExcludeAttributeFilter ItemSelectableLogoExcludeAttributeFilter = new ImageSolutions.Item.ItemSelectableLogoExcludeAttributeFilter();
                ItemSelectableLogoExcludeAttributeFilter.ItemSelectableLogoID = new Database.Filter.StringSearch.SearchFilter();
                ItemSelectableLogoExcludeAttributeFilter.ItemSelectableLogoID.SearchString = _ItemSelectableLogo.ItemSelectableLogoID;
                ItemSelectableLogoExcludeAttributes = ImageSolutions.Item.ItemSelectableLogoExcludeAttribute.GetItemSelectableLogoExcludeAttributes(ItemSelectableLogoExcludeAttributeFilter);

                gvItemSelectableLogoExcludeAttribute.DataSource = ItemSelectableLogoExcludeAttributes.OrderBy(x => x.AttributeValue.Value);
                gvItemSelectableLogoExcludeAttribute.DataBind();
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
                _ItemSelectableLogo.SelectableLogoID = Convert.ToString(ddlSelectableLogo.SelectedValue);
                _ItemSelectableLogo.PositionXPercent = string.IsNullOrEmpty(txtPositionXPercent.Text) ? 0 : Convert.ToInt32(txtPositionXPercent.Text);
                _ItemSelectableLogo.PositionYPercent = string.IsNullOrEmpty(txtPositionYPercent.Text) ? 0 : Convert.ToInt32(txtPositionYPercent.Text);
                if(string.IsNullOrEmpty(txtWidth.Text))
                {
                    _ItemSelectableLogo.Width = null; 
                }
                else
                {
                    _ItemSelectableLogo.Width = Convert.ToInt32(txtWidth.Text);
                }

                _ItemSelectableLogo.ImageURL = imgItem.ImageUrl;

                if (_ItemSelectableLogo.IsNew)
                {
                    GenerateImage();
                    _ItemSelectableLogo.ImageURL = imgItem.ImageUrl;

                    _ItemSelectableLogo.ItemID = mItemID;
                    _ItemSelectableLogo.CreatedBy = CurrentUser.UserInfoID;
                    blnReturn = _ItemSelectableLogo.Create();
                }
                else
                {
                    blnReturn = _ItemSelectableLogo.Update();
                }

                pnlPreview.Visible = !string.IsNullOrEmpty(_ItemSelectableLogo.ImageURL);
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
                blnReturn = _ItemSelectableLogo.Delete();
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

        protected void ddlSelectableLogo_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(ddlSelectableLogo.SelectedValue))
            {
                ImageSolutions.SelectableLogo.SelectableLogo SelectableLogo = new ImageSolutions.SelectableLogo.SelectableLogo(ddlSelectableLogo.SelectedValue);

                int intXPercent = 0;
                int intYPercent = 0;
                if (!string.IsNullOrEmpty(SelectableLogo.LogoPosition))
                {
                    string[] arrPosition = SelectableLogo.LogoPosition.Split(',');
                    if (arrPosition.Length == 2)
                    {
                        string strXPercent = arrPosition[0];
                        string strYPercent = arrPosition[1];

                        intXPercent = Convert.ToInt32(Convert.ToDecimal(strXPercent.Substring(0, strXPercent.IndexOf("%"))));
                        intYPercent = Convert.ToInt32(Convert.ToDecimal(strYPercent.Substring(0, strYPercent.IndexOf("%"))));
                    }
                }

                txtPositionXPercent.Text = Convert.ToString(intXPercent);
                txtPositionYPercent.Text = Convert.ToString(intYPercent);
            }

            GenerateImage();
        }

        protected void GenerateImage()
        {
            int left = 0;
            int top = 0;

            string strMonth = DateTime.UtcNow.ToString("yyyyMM");
            string strPath = Server.MapPath("\\DragAndDrop\\" + strMonth + "\\");
            if (!Directory.Exists(strPath))
            {
                Directory.CreateDirectory(strPath);
            }
            
            if(!string.IsNullOrEmpty(ddlSelectableLogo.SelectedValue))
            {
                ImageSolutions.SelectableLogo.SelectableLogo SelectableLogo = new ImageSolutions.SelectableLogo.SelectableLogo(ddlSelectableLogo.SelectedValue);

                if (_ItemSelectableLogo != null && SelectableLogo != null)
                {
                    if (CheckUrlStatus(SelectableLogo.ImagePath))
                    {
                        imgUploadedLogo.ImageUrl = SelectableLogo.ImagePath;
                        System.Drawing.Image imgLogo = GetImageFromURL(SelectableLogo.ImagePath);

                        int intWidth = string.IsNullOrEmpty(txtWidth.Text) ? 0 : Convert.ToInt32(txtWidth.Text);
                        if(intWidth > 0)
                        {
                            imgLogo = ScaleImage(imgLogo, Convert.ToInt32(intWidth));
                        }

                        if (imgLogo != null)
                        {
                            string strBackImageURL = !string.IsNullOrEmpty(_Item.LogoImageURL) ? _Item.LogoImageURL : _Item.ImageURL;
                            System.Drawing.Image imgSource = GetImageFromURL(strBackImageURL);

                            Graphics graphicFinal = Graphics.FromImage(imgSource);

                            int intXPercent = string.IsNullOrEmpty(txtPositionXPercent.Text) ? 0 : Convert.ToInt32(txtPositionXPercent.Text);
                            int intYPercent = string.IsNullOrEmpty(txtPositionYPercent.Text) ? 0 : Convert.ToInt32(txtPositionYPercent.Text);

                            left = (imgSource.Width * intXPercent / 100) - (imgLogo.Width / 2);
                            top = (imgSource.Height * intYPercent / 100) - (imgLogo.Height / 2);

                            graphicFinal.DrawImage(imgLogo, new Point(left, top));

                            string strCustomDesignImageName = Guid.NewGuid().ToString() + ".jpg";
                            string strCustomDesignImagePath = strPath + strCustomDesignImageName;

                            imgSource.Save(strCustomDesignImagePath, System.Drawing.Imaging.ImageFormat.Jpeg);

                            imgItem.ImageUrl = "/DragAndDrop/" + strMonth + "/" + strCustomDesignImageName;

                            _ItemSelectableLogo.ImageURL = imgItem.ImageUrl;
                        }
                    }
                    else
                    {
                        imgItem.ImageUrl = "../assets/images/pro3/2.jpg";
                    }
                }
                else
                {
                    imgItem.ImageUrl = "../assets/images/pro3/2.jpg";
                }
            }
            
            pnlPreview.Visible = !string.IsNullOrEmpty(_ItemSelectableLogo.ImageURL);
        }
        public static System.Drawing.Image ScaleImage(System.Drawing.Image image, int width)
        {
            var ratio = (double)width / image.Width;
            var newWidth = (int)(image.Width * ratio);
            var newHeight = (int)(image.Height * ratio);
            var newImage = new Bitmap(newWidth, newHeight);
            using (var g = Graphics.FromImage(newImage))
            {
                g.DrawImage(image, 0, 0, newWidth, newHeight);
            }
            return newImage;
        }
        protected bool CheckUrlStatus(string Website)
        {
            try
            {
                var request = WebRequest.Create(Website) as HttpWebRequest;
                request.Method = "GET";
                using (var response = (HttpWebResponse)request.GetResponse())
                {
                    return response.StatusCode == HttpStatusCode.OK;
                }
            }
            catch
            {
                return false;
            }
        }
        public System.Drawing.Image GetImageFromURL(string url)
        {
            WebClient WebClient = new WebClient();
            byte[] bytes = WebClient.DownloadData(url);
            MemoryStream ms = new MemoryStream(bytes);
            return System.Drawing.Image.FromStream(ms);
        }

        protected void txtPositionXPercent_TextChanged(object sender, EventArgs e)
        {
            GenerateImage();
        }

        protected void txtPositionYPercent_TextChanged(object sender, EventArgs e)
        {
            GenerateImage();
        }

        protected void txtWidth_TextChanged(object sender, EventArgs e)
        {
            GenerateImage();
        }

        protected void btnApplyToAll_Click(object sender, EventArgs e)
        {
            try
            {
                List<ImageSolutions.Item.ItemSelectableLogo> ItemSelectableLogos = new List<ImageSolutions.Item.ItemSelectableLogo>();
                ImageSolutions.Item.ItemSelectableLogoFilter ItemSelectableLogoFilter = new ImageSolutions.Item.ItemSelectableLogoFilter();
                ItemSelectableLogoFilter.ItemID = new Database.Filter.StringSearch.SearchFilter();
                ItemSelectableLogoFilter.ItemID.SearchString = mItemID;
                ItemSelectableLogos = ImageSolutions.Item.ItemSelectableLogo.GetItemSelectableLogos(ItemSelectableLogoFilter);

                foreach(ImageSolutions.Item.ItemSelectableLogo _ItemSelectableLogo in ItemSelectableLogos)
                {
                    _ItemSelectableLogo.PositionXPercent = string.IsNullOrEmpty(txtPositionXPercent.Text) ? 0 : Convert.ToInt32(txtPositionXPercent.Text);
                    _ItemSelectableLogo.PositionYPercent = string.IsNullOrEmpty(txtPositionYPercent.Text) ? 0 : Convert.ToInt32(txtPositionYPercent.Text);
                    if (string.IsNullOrEmpty(txtWidth.Text))
                    {
                        _ItemSelectableLogo.Width = null;
                    }
                    else
                    {
                        _ItemSelectableLogo.Width = Convert.ToInt32(txtWidth.Text);
                    }
                    _ItemSelectableLogo.Update();
                }

                Response.Redirect(ReturnURL);
            }
            catch (Exception ex)
            {
                WebUtility.DisplayJavascriptMessage(this, ex.Message);
            }
            finally { }
        }

        protected void BindWebsiteGroupItemSelectableLogo()
        {
            try
            {
                List<ImageSolutions.Website.WebsiteGroupItemSelectableLogo> WebsiteGroupItemSelectableLogos = new List<ImageSolutions.Website.WebsiteGroupItemSelectableLogo>();
                ImageSolutions.Website.WebsiteGroupItemSelectableLogoFilter WebsiteGroupItemSelectableLogoFilter = new ImageSolutions.Website.WebsiteGroupItemSelectableLogoFilter();
                WebsiteGroupItemSelectableLogoFilter.WebsiteID = new Database.Filter.StringSearch.SearchFilter();
                WebsiteGroupItemSelectableLogoFilter.WebsiteID.SearchString = CurrentWebsite.WebsiteID;
                WebsiteGroupItemSelectableLogoFilter.ItemSelectableLogoID = new Database.Filter.StringSearch.SearchFilter();
                WebsiteGroupItemSelectableLogoFilter.ItemSelectableLogoID.SearchString = _ItemSelectableLogo.ItemSelectableLogoID;
                WebsiteGroupItemSelectableLogos = ImageSolutions.Website.WebsiteGroupItemSelectableLogo.GetWebsiteGroupItemSelectableLogos(WebsiteGroupItemSelectableLogoFilter);
                gvWebsiteGroupItemSelectableLogo.DataSource = WebsiteGroupItemSelectableLogos;
                gvWebsiteGroupItemSelectableLogo.DataBind();
            }
            catch (Exception ex)
            {
                WebUtility.DisplayJavascriptMessage(this, ex.Message);
            }
            finally { }
        }

        protected void btnExcludeAttributeApplyToAllItems_Click(object sender, EventArgs e)
        {
            try
            {
                List<ImageSolutions.Item.ItemSelectableLogo> ItemSelectableLogos = new List<ImageSolutions.Item.ItemSelectableLogo>();
                ImageSolutions.Item.ItemSelectableLogoFilter ItemSelectableLogoFilter = new ImageSolutions.Item.ItemSelectableLogoFilter();
                ItemSelectableLogoFilter.SelectableLogoID = new Database.Filter.StringSearch.SearchFilter();
                ItemSelectableLogoFilter.SelectableLogoID.SearchString = _ItemSelectableLogo.SelectableLogoID;
                ItemSelectableLogos = ImageSolutions.Item.ItemSelectableLogo.GetItemSelectableLogos(ItemSelectableLogoFilter);

                foreach(ImageSolutions.Item.ItemSelectableLogo _ItemSelectableLogo2 in ItemSelectableLogos)
                {
                    List<ImageSolutions.Item.ItemSelectableLogoExcludeAttribute> ItemSelectableLogoExcludeAttributes = new List<ImageSolutions.Item.ItemSelectableLogoExcludeAttribute>();
                    ImageSolutions.Item.ItemSelectableLogoExcludeAttributeFilter ItemSelectableLogoExcludeAttributeFilter = new ImageSolutions.Item.ItemSelectableLogoExcludeAttributeFilter();
                    ItemSelectableLogoExcludeAttributeFilter.ItemSelectableLogoID = new Database.Filter.StringSearch.SearchFilter();
                    ItemSelectableLogoExcludeAttributeFilter.ItemSelectableLogoID.SearchString = _ItemSelectableLogo.ItemSelectableLogoID;
                    ItemSelectableLogoExcludeAttributes = ImageSolutions.Item.ItemSelectableLogoExcludeAttribute.GetItemSelectableLogoExcludeAttributes(ItemSelectableLogoExcludeAttributeFilter);

                    foreach (ImageSolutions.Item.ItemSelectableLogoExcludeAttribute _ItemSelectableLogoExcludeAttribute in ItemSelectableLogoExcludeAttributes)
                    {
                        ImageSolutions.Attribute.Attribute Attribute = new ImageSolutions.Attribute.Attribute();
                        ImageSolutions.Attribute.AttributeFilter AttributeFilter = new ImageSolutions.Attribute.AttributeFilter();
                        AttributeFilter.ItemID = new Database.Filter.StringSearch.SearchFilter();
                        AttributeFilter.ItemID.SearchString = _ItemSelectableLogo2.ItemID;
                        AttributeFilter.AttributeName = new Database.Filter.StringSearch.SearchFilter();
                        AttributeFilter.AttributeName.SearchString = _ItemSelectableLogoExcludeAttribute.AttributeValue.Attribute.AttributeName;
                        Attribute = ImageSolutions.Attribute.Attribute.GetAttribute(AttributeFilter);

                        if (Attribute != null)
                        {
                            ImageSolutions.Attribute.AttributeValue AttributeValue = new ImageSolutions.Attribute.AttributeValue();
                            ImageSolutions.Attribute.AttributeValueFilter AttributeValueFilter = new ImageSolutions.Attribute.AttributeValueFilter();
                            AttributeValueFilter.AttributeID = new Database.Filter.StringSearch.SearchFilter();
                            AttributeValueFilter.AttributeID.SearchString = Attribute.AttributeID;
                            AttributeValueFilter.Value = new Database.Filter.StringSearch.SearchFilter();
                            AttributeValueFilter.Value.SearchString = _ItemSelectableLogoExcludeAttribute.AttributeValue.Value;
                            AttributeValue = ImageSolutions.Attribute.AttributeValue.GetAttributeValue(AttributeValueFilter);

                            if (AttributeValue != null)
                            {
                                ImageSolutions.Item.ItemSelectableLogoExcludeAttribute ItemSelectableLogoExcludeAttribute2 = new ImageSolutions.Item.ItemSelectableLogoExcludeAttribute();
                                ImageSolutions.Item.ItemSelectableLogoExcludeAttributeFilter ItemSelectableLogoExcludeAttributeFilter2 = new ImageSolutions.Item.ItemSelectableLogoExcludeAttributeFilter();
                                ItemSelectableLogoExcludeAttributeFilter2.ItemSelectableLogoID = new Database.Filter.StringSearch.SearchFilter();
                                ItemSelectableLogoExcludeAttributeFilter2.ItemSelectableLogoID.SearchString = _ItemSelectableLogo2.ItemSelectableLogoID;
                                ItemSelectableLogoExcludeAttributeFilter2.AttributeValueID = new Database.Filter.StringSearch.SearchFilter();
                                ItemSelectableLogoExcludeAttributeFilter2.AttributeValueID.SearchString = AttributeValue.AttributeValueID;
                                ItemSelectableLogoExcludeAttribute2 = ImageSolutions.Item.ItemSelectableLogoExcludeAttribute.GetItemSelectableLogoExcludeAttribute(ItemSelectableLogoExcludeAttributeFilter2);

                                if (ItemSelectableLogoExcludeAttribute2 == null)
                                {
                                    ItemSelectableLogoExcludeAttribute2 = new ImageSolutions.Item.ItemSelectableLogoExcludeAttribute();
                                    ItemSelectableLogoExcludeAttribute2.ItemSelectableLogoID = _ItemSelectableLogo2.ItemSelectableLogoID;
                                    ItemSelectableLogoExcludeAttribute2.AttributeValueID = AttributeValue.AttributeValueID;
                                    ItemSelectableLogoExcludeAttribute2.CreatedBy = CurrentUser.UserInfoID;
                                    ItemSelectableLogoExcludeAttribute2.Create();
                                }
                            }
                        }                        
                    }
                }
            }
            catch (Exception ex)
            {
                WebUtility.DisplayJavascriptMessage(this, ex.Message);
            }
            finally { }
        }

        protected void btnGroupApplyToAllItems_Click(object sender, EventArgs e)
        {
            try
            {
                List<ImageSolutions.Item.ItemSelectableLogo> ItemSelectableLogos = new List<ImageSolutions.Item.ItemSelectableLogo>();
                ImageSolutions.Item.ItemSelectableLogoFilter ItemSelectableLogoFilter = new ImageSolutions.Item.ItemSelectableLogoFilter();
                ItemSelectableLogoFilter.SelectableLogoID = new Database.Filter.StringSearch.SearchFilter();
                ItemSelectableLogoFilter.SelectableLogoID.SearchString = _ItemSelectableLogo.SelectableLogoID;
                ItemSelectableLogos = ImageSolutions.Item.ItemSelectableLogo.GetItemSelectableLogos(ItemSelectableLogoFilter);

                foreach (ImageSolutions.Item.ItemSelectableLogo _ItemSelectableLogo2 in ItemSelectableLogos)
                {
                    List<ImageSolutions.Website.WebsiteGroupItemSelectableLogo> WebsiteGroupItemSelectableLogos = new List<ImageSolutions.Website.WebsiteGroupItemSelectableLogo>();
                    ImageSolutions.Website.WebsiteGroupItemSelectableLogoFilter WebsiteGroupItemSelectableLogoFilter = new ImageSolutions.Website.WebsiteGroupItemSelectableLogoFilter();
                    WebsiteGroupItemSelectableLogoFilter.WebsiteID = new Database.Filter.StringSearch.SearchFilter();
                    WebsiteGroupItemSelectableLogoFilter.WebsiteID.SearchString = CurrentWebsite.WebsiteID;
                    WebsiteGroupItemSelectableLogoFilter.ItemSelectableLogoID = new Database.Filter.StringSearch.SearchFilter();
                    WebsiteGroupItemSelectableLogoFilter.ItemSelectableLogoID.SearchString = _ItemSelectableLogo.ItemSelectableLogoID;
                    WebsiteGroupItemSelectableLogos = ImageSolutions.Website.WebsiteGroupItemSelectableLogo.GetWebsiteGroupItemSelectableLogos(WebsiteGroupItemSelectableLogoFilter);

                    foreach (ImageSolutions.Website.WebsiteGroupItemSelectableLogo _WebsiteGroupItemSelectableLogo in WebsiteGroupItemSelectableLogos)
                    {
                        ImageSolutions.Website.WebsiteGroupItemSelectableLogo WebsiteGroupItemSelectableLogo2 = new ImageSolutions.Website.WebsiteGroupItemSelectableLogo();
                        ImageSolutions.Website.WebsiteGroupItemSelectableLogoFilter WebsiteGroupItemSelectableLogoFilter2 = new ImageSolutions.Website.WebsiteGroupItemSelectableLogoFilter();
                        WebsiteGroupItemSelectableLogoFilter2.ItemSelectableLogoID = new Database.Filter.StringSearch.SearchFilter();
                        WebsiteGroupItemSelectableLogoFilter2.ItemSelectableLogoID.SearchString = _ItemSelectableLogo2.ItemSelectableLogoID;
                        WebsiteGroupItemSelectableLogoFilter2.WebsiteGroupID = new Database.Filter.StringSearch.SearchFilter();
                        WebsiteGroupItemSelectableLogoFilter2.WebsiteGroupID.SearchString = _WebsiteGroupItemSelectableLogo.WebsiteGroupID;
                        WebsiteGroupItemSelectableLogo2 = ImageSolutions.Website.WebsiteGroupItemSelectableLogo.GetWebsiteGroupItemSelectableLogo(WebsiteGroupItemSelectableLogoFilter2);

                        if (WebsiteGroupItemSelectableLogo2 == null)
                        {
                            WebsiteGroupItemSelectableLogo2 = new ImageSolutions.Website.WebsiteGroupItemSelectableLogo();
                            WebsiteGroupItemSelectableLogo2.WebsiteID = CurrentWebsite.WebsiteID;
                            WebsiteGroupItemSelectableLogo2.ItemSelectableLogoID = _ItemSelectableLogo2.ItemSelectableLogoID;
                            WebsiteGroupItemSelectableLogo2.WebsiteGroupID = _WebsiteGroupItemSelectableLogo.WebsiteGroupID;
                            WebsiteGroupItemSelectableLogo2.CreatedBy = CurrentUser.UserInfoID;
                            WebsiteGroupItemSelectableLogo2.Create();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                WebUtility.DisplayJavascriptMessage(this, ex.Message);
            }
            finally { }
        }
    }
}