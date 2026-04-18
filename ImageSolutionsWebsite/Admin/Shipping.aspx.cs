using ImageSolutions.Website;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ImageSolutionsWebsite.Admin
{
    public partial class Shipping : BasePageAdminUserWebSiteAuth
    {
        protected string mWebsiteShippingServiceID = string.Empty;

        private ImageSolutions.Website.WebsiteShippingService mWebsiteShippingService = null;
        protected ImageSolutions.Website.WebsiteShippingService WebsiteShippingService
        {
            get
            {
                if (mWebsiteShippingService == null)
                {
                    if (string.IsNullOrEmpty(mWebsiteShippingServiceID))
                        mWebsiteShippingService = new ImageSolutions.Website.WebsiteShippingService();
                    else
                        mWebsiteShippingService = new ImageSolutions.Website.WebsiteShippingService(mWebsiteShippingServiceID);
                }
                return mWebsiteShippingService;
            }
            set
            {
                mWebsiteShippingService = value;
            }
        }

        protected string ReturnURL
        {
            get
            {
                if (ViewState["ReturnURL"] == null)
                    return "/Admin/ShippingOverview.aspx";
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
            mWebsiteShippingServiceID = Request.QueryString.Get("id");

            if (!Page.IsPostBack)
            {
                if (Request.UrlReferrer != null) ReturnURL = Request.UrlReferrer.PathAndQuery;

                InitializePage();
            }
        }

        protected void BindWebsiteShippingService()
        {
            List<ImageSolutions.Shipping.ShippingService> objShippingServices = null;

            try
            {
                objShippingServices = ImageSolutions.Shipping.ShippingService.GetShippingServices(null);

                this.ddlShippingService.DataSource = objShippingServices;
                this.ddlShippingService.DataBind();
                this.ddlShippingService.Items.Insert(0, new ListItem(String.Empty, string.Empty));
            }
            catch (Exception ex)
            {
                WebUtility.DisplayJavascriptMessage(this, ex.Message);
            }
            finally { objShippingServices = null; }
        }

        public void InitializePage()
        {
            try
            {
                InitializeTabs(top_1_tab, top_1, top_2_tab, top_2, null, null, null, null);

                BindWebsiteShippingService();

                if (!WebsiteShippingService.IsNew)
                {
                    this.ddlShippingService.SelectedIndex = this.ddlShippingService.Items.IndexOf(this.ddlShippingService.Items.FindByValue(WebsiteShippingService.ShippingServiceID));
                    btnSave.Text = "Save";

                    BindWebsiteShippingServiceGroup();
                }
                else
                {
                    btnSave.Text = "Create";
                    btnDelete.Visible = false;
                }
                aCancel.HRef = "/Admin/ShippingOverview.aspx"; //ReturnURL;
            }
            catch (Exception ex)
            {
                WebUtility.DisplayJavascriptMessage(this, ex.Message);
            }
            finally { }
        }

        protected void BindWebsiteShippingServiceGroup()
        {
            List<ImageSolutions.Website.WebsiteShippingServiceGroup> WebsiteShippingServiceGroups = null;
            ImageSolutions.Website.WebsiteShippingServiceGroupFilter WebsiteShippingServiceGroupFilter = null;
            int intTotalRecord = 0;

            try
            {
                WebsiteShippingServiceGroupFilter = new ImageSolutions.Website.WebsiteShippingServiceGroupFilter();
                WebsiteShippingServiceGroupFilter.WebsiteShippingServiceID = new Database.Filter.StringSearch.SearchFilter();
                WebsiteShippingServiceGroupFilter.WebsiteShippingServiceID.SearchString = WebsiteShippingService.WebsiteShippingServiceID;
                WebsiteShippingServiceGroups = ImageSolutions.Website.WebsiteShippingServiceGroup.GetWebsiteShippingServiceGroups(WebsiteShippingServiceGroupFilter);

                this.gvWebsiteShippingServiceGroup.DataSource = WebsiteShippingServiceGroups;
                this.gvWebsiteShippingServiceGroup.DataBind();

                if (this.gvWebsiteShippingServiceGroup.HeaderRow != null) this.gvWebsiteShippingServiceGroup.HeaderRow.TableSection = TableRowSection.TableHeader;
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
                WebsiteShippingService.ShippingServiceID = Convert.ToString(ddlShippingService.SelectedValue);

                if (WebsiteShippingService.IsNew)
                {
                    WebsiteShippingService.WebsiteID = CurrentUser.CurrentUserWebSite.WebSite.WebsiteID;
                    WebsiteShippingService.CreatedBy = CurrentUser.UserInfoID;
                    blnReturn = WebsiteShippingService.Create();
                }
                else
                {
                    blnReturn = WebsiteShippingService.Update();
                }
            }
            catch (Exception ex)
            {
                WebUtility.DisplayJavascriptMessage(this, ex.Message);
            }
            finally { }


            if (blnReturn)
            {
                Response.Redirect("/Admin/ShippingOverview.aspx");
                //Response.Redirect(ReturnURL);
            }
        }

        protected void btnDelete_Click(object sender, EventArgs e)
        {
            bool blnReturn = false;

            try
            {
                blnReturn = WebsiteShippingService.Delete();
            }
            catch (Exception ex)
            {
                WebUtility.DisplayJavascriptMessage(this, ex.Message);
            }
            finally { }

            if (blnReturn)
            {
                Response.Redirect("/Admin/ShippingOverview.aspx");
                //Response.Redirect(ReturnURL);
            }
        }
    }
}