using ImageSolutions.Website;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ImageSolutionsWebsite.Admin
{
    public partial class CustomList : BasePageAdminUserWebSiteAuth
    {
        protected string mCustomListID = string.Empty;

        private ImageSolutions.Custom.CustomList mCustomList = null;
        protected ImageSolutions.Custom.CustomList _CustomList
        {
            get
            {
                if (mCustomList == null)
                {
                    if (string.IsNullOrEmpty(mCustomListID))
                        mCustomList = new ImageSolutions.Custom.CustomList();
                    else
                        mCustomList = new ImageSolutions.Custom.CustomList(mCustomListID);
                }
                return mCustomList;
            }
            set
            {
                mCustomList = value;
            }
        }

        protected string ReturnURL
        {
            get
            {
                if (ViewState["ReturnURL"] == null)
                    return "/Admin/CustomListOverview.aspx";
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
            mCustomListID = Request.QueryString.Get("id");

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

                if (!_CustomList.IsNew)
                {
                    txtListName.Text = _CustomList.ListName;
                    btnSave.Text = "Save";

                    BindCustomListValue();
                }
                else
                {
                    btnSave.Text = "Create";
                    btnDelete.Visible = false;
                    top_2_tab.Visible = false;
                }
                aCancel.HRef = ReturnURL;
            }
            catch (Exception ex)
            {
                WebUtility.DisplayJavascriptMessage(this, ex.Message);
            }
            finally { }
        }

        protected void BindCustomListValue()
        {
            try
            {
                gvAttributeValue.DataSource = _CustomList.CustomListValues;
                gvAttributeValue.DataBind();
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
                _CustomList.ListName = txtListName.Text;

                if (_CustomList.IsNew)
                {
                    _CustomList.WebsiteID = CurrentWebsite.WebsiteID;
                    _CustomList.CreatedBy = CurrentUser.UserInfoID;
                    blnReturn = _CustomList.Create();
                    ReturnURL = "/admin/CustomList.aspx?id=" + _CustomList.CustomListID;
                }
                else
                {
                    blnReturn = _CustomList.Update();
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
                blnReturn = _CustomList.Delete();
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
            if (blnReturn)
            {
                Response.Redirect(ReturnURL);
            }
        }

        protected void gvAttributeValue_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "MoveUp")
            {
                //string strCustomListValueID = Convert.ToString(e.CommandArgument);
                //ImageSolutions.Custom.CustomListValue CustomListValue = new ImageSolutions.Custom.CustomListValue(strCustomListValueID);

                //WebsiteTabItem UpWebsiteTabItem = new WebsiteTabItem();
                //WebsiteTabItemFilter WebsiteTabItemFilter = new WebsiteTabItemFilter();
                //WebsiteTabItemFilter.WebsiteTabID = new Database.Filter.StringSearch.SearchFilter();
                //WebsiteTabItemFilter.WebsiteTabID.SearchString = _WebsiteTab.WebsiteTabID;
                //WebsiteTabItemFilter.Sort = WebsiteTabItem.Sort - 1;
                //UpWebsiteTabItem = WebsiteTabItem.GetWebsiteTabItem(WebsiteTabItemFilter);

                //UpWebsiteTabItem.Sort = UpWebsiteTabItem.Sort + 1;
                //UpWebsiteTabItem.Update();

                //WebsiteTabItem.Sort = WebsiteTabItem.Sort - 1;
                //WebsiteTabItem.Update();

                BindCustomListValue();
            }
            else if (e.CommandName == "MoveDown")
            {
                //string strWebsiteTabItemID = Convert.ToString(e.CommandArgument);
                //WebsiteTabItem WebsiteTabItem = new WebsiteTabItem(strWebsiteTabItemID);

                //WebsiteTabItem DownWebsiteTabItem = new WebsiteTabItem();
                //WebsiteTabItemFilter WebsiteTabItemFilter = new WebsiteTabItemFilter();
                //WebsiteTabItemFilter.WebsiteTabID = new Database.Filter.StringSearch.SearchFilter();
                //WebsiteTabItemFilter.WebsiteTabID.SearchString = _WebsiteTab.WebsiteTabID;
                //WebsiteTabItemFilter.Sort = WebsiteTabItem.Sort + 1;
                //DownWebsiteTabItem = WebsiteTabItem.GetWebsiteTabItem(WebsiteTabItemFilter);

                //DownWebsiteTabItem.Sort = DownWebsiteTabItem.Sort - 1;
                //DownWebsiteTabItem.Update();

                //WebsiteTabItem.Sort = WebsiteTabItem.Sort + 1;
                //WebsiteTabItem.Update();

                BindCustomListValue();
            }
        }
    }
}