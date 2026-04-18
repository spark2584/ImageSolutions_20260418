using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ImageSolutionsWebsite.Control
{
    public partial class Pager : System.Web.UI.UserControl
    {
        public enum PagingModes { PostBack, Redirect }
        public enum RedirectUrlModes { QueryString, LeadingPageFolder }
        private string LeadingPageFolderUrl = null;
        private string QueryStringPageNumberUrl = null;

        public PagingModes PagingMode
        {
            get { return ViewState["PagingMode"] == null ? PagingModes.PostBack : (PagingModes)(ViewState["PagingMode"]); }
            set { ViewState["PagingMode"] = value; }
        }

        public RedirectUrlModes RedirectUrlMode
        {
            get { return ViewState["RedirectUrlMode"] == null ? RedirectUrlModes.QueryString : (RedirectUrlModes)(ViewState["RedirectUrlMode"]); }
            set { ViewState["RedirectUrlMode"] = value; }
        }

        public event EventHandler PostBackPageIndexChanging;
        public event EventHandler ExportEventClicked;

        public string PagingRecordText
        {
            get { return ViewState["PagingRecordText"] == null ? string.Empty : ViewState["PagingRecordText"].ToString(); }
            set { ViewState["PagingRecordText"] = value; }
        }

        public string NoResultsText
        {
            get { return ViewState["NoResultsText"] == null ? "No results found" : ViewState["NoResultsText"].ToString(); }
            set { ViewState["NoResultsText"] = value; }
        }

        public int TotalRecord
        {
            get
            {
                return ViewState["TotalRecord"] == null ? 0 : Convert.ToInt32(ViewState["TotalRecord"]);
            }
            set
            {
                ViewState["TotalRecord"] = value;
            }
        }

        public int PageSize
        {
            get { return ViewState["PageSize"] == null ? 10 : Convert.ToInt32(ViewState["PageSize"]); }
            set { ViewState["PageSize"] = value; }
        }

        public int NumberOfPagesToShow
        {
            get { return ViewState["NumberOfPagesToShow"] == null ? 5 : Convert.ToInt32(ViewState["NumberOfPagesToShow"]); }
            set { ViewState["NumberOfPagesToShow"] = value; }
        }

        public int CurrentPageNumber
        {
            get
            {
                if (PagingMode == PagingModes.Redirect)
                {
                    if (!string.IsNullOrEmpty(Request.QueryString.Get("page")) && Utility.IsInteger(Request.QueryString.Get("page")))
                        return Convert.ToInt32(Request.QueryString.Get("page"));
                    else
                        return 1;
                }
                else
                {
                    return ViewState["CurrentPageNumber"] == null ? 1 : Convert.ToInt32(ViewState["CurrentPageNumber"]);
                }
            }
            set
            {
                ViewState["CurrentPageNumber"] = value;
            }
        }

        protected int TotalPages
        {
            get
            {
                if (PageSize == 0)
                    return 0;
                else
                    return Convert.ToInt32(Math.Ceiling((double)TotalRecord / PageSize));
            }
        }

        protected int StartPageNumber
        {
            get { return (CurrentPageNumber - 1) / NumberOfPagesToShow * NumberOfPagesToShow + 1; }
        }

        protected int EndPageNumber
        {
            get { return (StartPageNumber + NumberOfPagesToShow - 1 > TotalPages) ? TotalPages : StartPageNumber + NumberOfPagesToShow - 1; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            this.lbnExport.Visible = ExportEventClicked != null;
        }

        protected override void OnPreRender(EventArgs e)
        {
            if (PagingMode == PagingModes.PostBack && PostBackPageIndexChanging == null) throw new Exception("OnPostBackPageIndexChanging event handler must be set for PostBack Paging mode");
            GeneratePaging();
            base.OnPreRender(e);
        }

        protected void GeneratePaging()
        {
            ArrayList arylPaging = new ArrayList();

            try
            {
                //We can move this to CurrentPageNumber GET Property but the comparison will be done too many times
                if ((CurrentPageNumber - 1) * PageSize + 1 > TotalRecord) CurrentPageNumber = (TotalRecord - 1) / PageSize + 1;

                for (int i = StartPageNumber; i <= EndPageNumber; i++) arylPaging.Add(i);

                if (PagingMode == PagingModes.Redirect)
                {
                    if (CurrentPageNumber > 0 && CurrentPageNumber > TotalPages && TotalPages > 0) Response.Redirect(RedirectUrl(TotalPages));
                    this.rptRedirect.DataSource = arylPaging;
                    this.rptRedirect.DataBind();
                }
                else
                {
                    this.rptPostback.DataSource = arylPaging;
                    this.rptPostback.DataBind();
                }
                if (arylPaging.Count == 0)
                {
                    this.lblPagingRecordText.Text = NoResultsText;
                    this.rptRedirect.Visible = false;
                    this.rptPostback.Visible = false;
                }
                else
                {
                    this.rptRedirect.Visible = true;
                    this.rptPostback.Visible = true;
                    int intCurrentPageFirstRecord = ((CurrentPageNumber - 1) * PageSize + 1);
                    int intCurrentPageLastRecord = TotalRecord > intCurrentPageFirstRecord + PageSize - 1 ? intCurrentPageFirstRecord + PageSize - 1 : TotalRecord;
                    this.lblPagingRecordText.Text = "Showing " + PagingRecordText + " " + intCurrentPageFirstRecord + "-" + intCurrentPageLastRecord + " of " + TotalRecord;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                arylPaging = null;
            }
        }

        protected string RedirectUrl(object PageNumber)
        {
            if (RedirectUrlMode == RedirectUrlModes.LeadingPageFolder)
            {
                return GetLeadingPageFolderUrl(PageNumber);
            }
            else
            {
                return GetQueryStringPageNumberUrl(PageNumber);
            }
        }

        private string GetLeadingPageFolderUrl(object PageNumber)
        {
            if (LeadingPageFolderUrl == null)
            {
                LeadingPageFolderUrl = string.Empty;

                Regex objReg = null;
                string strPath = string.Empty;
                string strReturn = string.Empty;

                try
                {
                    strPath = Request.RawUrl.ToLower().ToString();
                    objReg = new Regex(@"(.*)/page/(\d+)/(.+)");
                    if (objReg.IsMatch(strPath))
                    {
                        LeadingPageFolderUrl = objReg.Replace(strPath, "$1/page/{0}/$3");
                    }
                    else
                    {
                        LeadingPageFolderUrl = strPath.Substring(0, strPath.LastIndexOf("/") + 1) + "page/{0}" + strPath.Substring(strPath.LastIndexOf("/"), strPath.Length - strPath.LastIndexOf("/"));
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    objReg = null;
                }
            }
            if (PageNumber.ToString() == "1")
                return LeadingPageFolderUrl.Replace("/page/{0}/", "/");
            else
                return string.Format(LeadingPageFolderUrl, PageNumber.ToString());
        }

        private string GetQueryStringPageNumberUrl(object PageNumber)
        {
            if (QueryStringPageNumberUrl == null)
            {
                NameValueCollection objQueryString = WebUtility.URL.QueryStringRawAndPhysical;
                objQueryString.Remove("page");

                string strNewQueryString = string.Empty;
                if (objQueryString.Count > 0)
                {
                    foreach (string key in objQueryString.AllKeys)
                    {
                        strNewQueryString += key.Trim() + "=" + objQueryString[key].Trim() + "&";
                    }
                }
                QueryStringPageNumberUrl = WebUtility.URL.RawUrlNoQuery + "?" + strNewQueryString + "page={0}";
            }
            //if (PageNumber.ToString() == "1")
            //    return QueryStringPageNumberUrl.Replace("&page={0}", "").Replace("page={0}", "");
            //else
            //    return string.Format(QueryStringPageNumberUrl, PageNumber.ToString());

            return string.Format(QueryStringPageNumberUrl, PageNumber.ToString());
        }

        protected void rptPostback_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            CurrentPageNumber = Convert.ToInt32(e.CommandArgument);
            if (PostBackPageIndexChanging != null) PostBackPageIndexChanging(this, EventArgs.Empty);
            GeneratePaging();
        }

        protected void lbnExport_Click(object sender, EventArgs e)
        {
            if (ExportEventClicked != null) ExportEventClicked(this, e);
        }
    }
}