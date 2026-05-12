using Amazon.S3.Model;
using ImageSolutions.Attribute;
using ImageSolutions.Item;
using ImageSolutionsWebsite.Admin;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ImageSolutionsWebsite
{
    public partial class Items : BasePageUserAccountAuth
    {
        protected string mWebSiteTabID = string.Empty;
        protected string mSearch = string.Empty;

        protected void Page_Load(object sender, EventArgs e)
        {
            mWebSiteTabID = Request.QueryString.Get("WebSiteTabID");
            mSearch = Request.QueryString.Get("Search");

            if (!Page.IsPostBack)
            {
                // Restore selections from session (survive pager redirects)
                if (Session["Items_SortValue"] != null)
                {
                    string saved = Session["Items_SortValue"].ToString();
                    // Guard against stale values from old dropdown options
                    if (ddlSort.Items.FindByValue(saved) != null)
                        ddlSort.SelectedValue = saved;
                }
                if (Session["Items_PageSize"] != null)
                    ddlPageSize.SelectedValue = Session["Items_PageSize"].ToString();
                if (Session["Items_PriceMin"] != null)
                    hfPriceMin.Value = Session["Items_PriceMin"].ToString();
                if (Session["Items_PriceMax"] != null)
                    hfPriceMax.Value = Session["Items_PriceMax"].ToString();
                // Restore absolute max so slider range survives pager redirects
                string absMaxKey = "Items_AbsMaxPrice_" + mWebSiteTabID + "_" + mSearch;
                if (Session[absMaxKey] != null)
                    hfPriceAbsMax.Value = Session[absMaxKey].ToString();

                BindItems();

                if (!string.IsNullOrEmpty(mWebSiteTabID))
                {
                    ImageSolutions.Website.WebsiteTab WebsiteTab = new ImageSolutions.Website.WebsiteTab(mWebSiteTabID);
                    if (!string.IsNullOrEmpty(WebsiteTab.Message))
                    {
                        pnlMessage.Visible = true;
                        litMessage.Text = WebsiteTab.Message.Replace(Environment.NewLine, "<br>");
                    }
                    else
                    {
                        pnlMessage.Visible = false;
                    }
                }

                if (
                    (Convert.ToString(ConfigurationManager.AppSettings["Environment"]) == "staging" && CurrentWebsite.WebsiteID == "55")
                    || (Convert.ToString(ConfigurationManager.AppSettings["Environment"]) == "production" && CurrentWebsite.WebsiteID == "22")
                )
                {
                    litStyle.Text = String.Format(@"
<style>
    .product-pagination {{
        border: none;
    }}

    .page-link {{
        border:none;
    }}

    .product-search-count-bottom {{
        border: none !important;
    }}
</style>
");
                }
            }
        }

        protected void BindItems()
        {
            List<ImageSolutions.Website.WebsiteTabItem> objWebsiteTabItems = null;
            ImageSolutions.Website.WebsiteTabItemFilter objFilter = null;
            int intTotalRecord = 0;
            List<string> objColors = new List<string>();
            List<string> objSizes = new List<string>();

            try
            {
                objFilter = new ImageSolutions.Website.WebsiteTabItemFilter();
                objFilter.WebsiteTabID = new Database.Filter.StringSearch.SearchFilter();
                objFilter.WebsiteTabID.SearchString = mWebSiteTabID;

                objFilter.mAttributeValues = new List<string>();
                foreach (ListViewItem objRow in lvSize.Items)
                {
                    CheckBox objSize = (CheckBox)objRow.FindControl("chkSize");
                    if (objSize.Checked) objFilter.mAttributeValues.Add(lvSize.DataKeys[objRow.DataItemIndex]["AttributeValue"].ToString());
                }
                foreach (ListViewItem objRow in lvColor.Items)
                {
                    CheckBox objColor = (CheckBox)objRow.FindControl("chkColor");
                    if (objColor.Checked) objFilter.mAttributeValues.Add(lvColor.DataKeys[objRow.DataItemIndex]["AttributeValue"].ToString());
                }

                if (CurrentWebsite.DisplaySubCategory)
                {
                    List<ImageSolutions.Website.WebsiteTab> objWebsiteTabs = null;
                    ImageSolutions.Website.WebsiteTabFilter WebsiteTabFilter = new ImageSolutions.Website.WebsiteTabFilter();
                    WebsiteTabFilter.ParentID = new Database.Filter.StringSearch.SearchFilter();
                    WebsiteTabFilter.ParentID.SearchString = mWebSiteTabID;
                    objWebsiteTabs = ImageSolutions.Website.WebsiteTab.GetWebsiteTabs(WebsiteTabFilter);

                    if(objWebsiteTabs != null && objWebsiteTabs.Count > 0 && string.IsNullOrEmpty(mSearch))
                    {
                        if(CurrentWebsite.CombineWebsiteGroup)
                        {
                            rptCategory.DataSource = objWebsiteTabs.FindAll(x => x.AllowAllGroups
                                || x.WebsiteGroupTabs.Exists(y => CurrentUser.CurrentUserWebSite.UserAccounts.Exists(z => z.WebsiteGroupID == y.WebsiteGroupID))
                                || x.UserWebsiteTabs.Exists(y => y.UserWebsiteID == CurrentUser.CurrentUserWebSite.UserWebsiteID)
                            );
                        }
                        else
                        {
                            rptCategory.DataSource = objWebsiteTabs.FindAll(x => x.AllowAllGroups
                                || x.WebsiteGroupTabs.Exists(y => y.WebsiteGroupID == CurrentUser.CurrentUserWebSite.CurrentUserAccount.WebsiteGroupID)
                                || x.UserWebsiteTabs.Exists(y => y.UserWebsiteID == CurrentUser.CurrentUserWebSite.UserWebsiteID)
                            );
                        }
                        rptCategory.DataBind();
                        pnlSubCategories.Visible = true;
                    }
                    else
                    {
                        pnlSubCategories.Visible = false;
                    }

                }

                objFilter.IsOnline = true;
                objFilter.Inactive = false;

                // Apply selected page size
                if (!string.IsNullOrEmpty(ddlPageSize.SelectedValue))
                {
                    int selectedPageSize = Convert.ToInt32(ddlPageSize.SelectedValue);
                    ucPager.PageSize = selectedPageSize;
                    ucPagerTop.PageSize = selectedPageSize;
                }

                // Determine sort field and direction from dropdown
                string sortValue = string.IsNullOrEmpty(ddlSort.SelectedValue) ? "Relevance" : ddlSort.SelectedValue;
                bool priceSort = sortValue.StartsWith("Price");
                bool ascending = sortValue.EndsWith("_ASC");
                string sortField = "Sort";   // DB sort field (used for Relevance)

                // Pre-read price filter — when active, load all items so we can filter + page in memory
                double prePriceMax = 0;
                double.TryParse(hfPriceMax.Value, out prePriceMax);
                bool isPriceFilterActive = prePriceMax > 0;
                int qPageSize = isPriceFilterActive ? 100000 : ucPager.PageSize;
                int qPageNum  = isPriceFilterActive ? 1 : ucPager.CurrentPageNumber;

                objWebsiteTabItems = ImageSolutions.Website.WebsiteTabItem.GetWebsiteTabItems(objFilter, sortField, ascending, qPageSize, qPageNum, out intTotalRecord);

                List<ImageSolutions.Item.MyGroupItem> MyGroupItems = new List<ImageSolutions.Item.MyGroupItem>();

                if (!string.IsNullOrEmpty(mSearch))
                {
                    intTotalRecord = 0;

                    SqlDataReader objRead = null;
                    string strSQL = string.Empty;

                    try
                    {
                        string strSQLAttribute = string.Empty;
                        if (objFilter.mAttributeValues != null && objFilter.mAttributeValues.Count > 0)
                        {
                            strSQLAttribute = "AND i.ItemID IN (SELECT ItemID FROM Attribute a (NOLOCK) INNER JOIN AttributeValue av (NOLOCK) ON a.AttributeID=av.AttributeID WHERE av.Value IN (";

                            for (int i = 0; i < objFilter.mAttributeValues.Count; i++)
                            {
                                if (i > 0) strSQLAttribute += ", ";
                                strSQLAttribute += Database.HandleQuote(objFilter.mAttributeValues[i]);
                            }

                            strSQLAttribute += ")) ";
                        }

                        strSQL = string.Format(@"
SELECT i.ItemID
FROM Item (NOLOCK) i
inner join ItemWebsite (NOLOCK) iw on iw.ItemID = i.ItemID
WHERE i.InActive = 0
and i.IsOnline = 1
and isnull(i.ParentID,0) = 0
and iw.WebsiteID = {0}
and (
	i.ItemName like '%' + {1} + '%'
	or i.ItemNumber like '%' + {1} + '%'
	or i.StoreDisplayName like '%' + {1} + '%'
	or i.SalesDescription like '%' + {1} + '%'
) 
{2}
"
                        , Database.HandleQuote(CurrentWebsite.WebsiteID)
                        , IsNumberic(mSearch) ? String.Format("'{0}'", mSearch) : Database.HandleQuote(mSearch)
                        , !string.IsNullOrEmpty(strSQLAttribute)
                            ? strSQLAttribute
                            : string.Empty);

                        objRead = Database.GetDataReader(strSQL);

                        List<ImageSolutions.Item.Item> Items = new List<ImageSolutions.Item.Item>();
                        //foreach (ImageSolutions.Website.WebsiteTab _WebsiteTab in CurrentUser.CurrentUserWebSite.WebSite.WebsiteTabs.FindAll(m => m.AllowAllGroups == true && string.IsNullOrEmpty(m.ParentID)))
                        //{
                        //    AddItemFromWebsiteTab(_WebsiteTab, ref Items);
                        //}

                        //foreach (ImageSolutions.Website.WebsiteGroupTab _WebsiteGroupTab in CurrentUser.CurrentUserWebSite.CurrentUserAccount.WebsiteGroup.WebsiteGroupTabs)
                        //{
                        //    if(string.IsNullOrEmpty(_WebsiteGroupTab.WebsiteTab.ParentID))
                        //    {
                        //        AddItemFromWebsiteTab(_WebsiteGroupTab.WebsiteTab, ref Items);
                        //    }
                        //}
                        foreach(ImageSolutions.Website.WebsiteTab _WebsiteTab in CurrentUser.CurrentUserWebSite.WebSite.WebsiteTabs.FindAll(m => string.IsNullOrEmpty(m.ParentID)))
                        {
                            if(!_WebsiteTab.Inactive)
                            {
                                AddItemFromWebsiteTab(_WebsiteTab, ref Items);
                            }
                        }

                        while (objRead.Read())
                        {                               
                            intTotalRecord++; 

                            if(Items.Exists(x => x.ItemID == Convert.ToString(objRead["ItemID"])))
                            {
                                MyGroupItems.Add(new ImageSolutions.Item.MyGroupItem(CurrentUser.CurrentUserWebSite.CurrentUserAccount.WebsiteGroupID, Convert.ToString(objRead["ItemID"])));
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

                    pnlCategoryBreadCrumb.Visible = false;
                }
                else
                {
                    foreach (ImageSolutions.Website.WebsiteTabItem _WebsiteTabItem in objWebsiteTabItems)
                    {
                        MyGroupItems.Add(new ImageSolutions.Item.MyGroupItem(CurrentUser.CurrentUserWebSite.CurrentUserAccount.WebsiteGroupID, _WebsiteTabItem.ItemID));

                        //if (_WebsiteTabItem.Item.Variations != null)
                        //{
                        //    foreach (ImageSolutions.Item.Item objItem in _WebsiteTabItem.Item.Variations)
                        //    {
                        //        if (objItem.ItemAttributeValues != null)
                        //        {
                        //            foreach (ImageSolutions.Item.ItemAttributeValue objItemAttributeValue in objItem.ItemAttributeValues)
                        //            {
                        //                if (objItemAttributeValue.AttributeValue.Attribute.AttributeName.ToLower() == "color")
                        //                {
                        //                    if (objColors.Exists(m => m.ToLower().Equals(objItemAttributeValue.AttributeValue.Value.ToLower())))
                        //                    {
                        //                        objColors.Add(objItemAttributeValue.AttributeValue.Attribute.AttributeName);
                        //                    }
                        //                }

                        //                if (objItemAttributeValue.AttributeValue.Attribute.AttributeName.ToLower() == "size")
                        //                {
                        //                    if (objSizes.Exists(m => m.ToLower().Equals(objItemAttributeValue.AttributeValue.Value.ToLower())))
                        //                    {
                        //                        objSizes.Add(objItemAttributeValue.AttributeValue.Attribute.AttributeName);
                        //                    }
                        //                }
                        //            }
                        //        }
                        //    }
                        //}

                        //if (_WebsiteTabItem.Item.ItemAttributeValues != null)
                        //{
                        //    foreach (ImageSolutions.Item.ItemAttributeValue objItemAttributeValue in _WebsiteTabItem.Item.ItemAttributeValues)
                        //    {
                        //        if (objItemAttributeValue.AttributeValue.Attribute.AttributeName.ToLower() == "color")
                        //        {
                        //            if (objColors.Exists(m => m.ToLower().Equals(objItemAttributeValue.AttributeValue.Value.ToLower())))
                        //            {
                        //                objColors.Add(objItemAttributeValue.AttributeValue.Attribute.AttributeName);
                        //            }
                        //        }

                        //        if (objItemAttributeValue.AttributeValue.Attribute.AttributeName.ToLower() == "size")
                        //        {
                        //            if (objSizes.Exists(m => m.ToLower().Equals(objItemAttributeValue.AttributeValue.Value.ToLower())))
                        //            {
                        //                objSizes.Add(objItemAttributeValue.AttributeValue.Attribute.AttributeName);
                        //            }
                        //        }
                        //    }
                        //}

                        //if (_WebsiteTabItem.Item.Attributes != null)
                        //{
                        //    foreach (ImageSolutions.Attribute.Attribute objAttribute in _WebsiteTabItem.Item.Attributes)
                        //    {
                        //        if (objAttribute.AttributeValues != null)
                        //        {
                        //            foreach (ImageSolutions.Attribute.AttributeValue objAttributeValue in objAttribute.AttributeValues)
                        //            {
                        //                if (objAttribute.AttributeName.ToLower() == "color")
                        //                {
                        //                    if (!objColors.Exists(m => m.ToLower().Equals(objAttributeValue.Value.ToLower())))
                        //                    {
                        //                        objColors.Add(objAttributeValue.Value);
                        //                    }
                        //                }

                        //                if (objAttribute.AttributeName.ToLower() == "size")
                        //                {
                        //                    if (!objSizes.Exists(m => m.ToLower().Equals(objAttributeValue.Value.ToLower())))
                        //                    {
                        //                        objSizes.Add(objAttributeValue.Value);
                        //                    }
                        //                }
                        //            }
                        //        }
                        //    }
                        //}
                    }

                    pnlCategoryBreadCrumb.Visible = true;
                    ImageSolutions.Website.WebsiteTab WebsiteTab = new ImageSolutions.Website.WebsiteTab(mWebSiteTabID);
                    litCategoryBreadCrumb.Text = WebsiteTab.TabPathBreadCrumb;
                }



                //foreach (ImageSolutions.Item.Item _Item in Items)
                //{
                //    if (_Item.WebsiteGroupItems != null && _Item.WebsiteGroupItems.FindAll(x => x.WebsiteID == CurrentWebsite.WebsiteID).Count > 0)
                //    {
                //        bool blnValid = false;

                //        foreach (ImageSolutions.Website.WebsiteGroupItem _WebsiteGroupItem in _Item.WebsiteGroupItems)
                //        {
                //            if (CurrentWebsite.CombineWebsiteGroup)
                //            {
                //                if (CurrentUser.CurrentUserWebSite.UserAccounts.Exists(x => x.WebsiteGroupID == _WebsiteGroupItem.WebsiteGroupID))
                //                {
                //                    blnValid = true;
                //                }
                //            }
                //            else
                //            {
                //                if (CurrentUser.CurrentUserWebSite.CurrentUserAccount.WebsiteGroupID == _WebsiteGroupItem.WebsiteGroupID)
                //                {
                //                    blnValid = true;
                //                }
                //            }
                //        }

                //        if (!blnValid)
                //        {
                //            Items.Remove(_Item);
                //        }
                //    }
                //}


                List<ImageSolutions.Item.MyGroupItem> RemoveMyGroupItems = new List<ImageSolutions.Item.MyGroupItem>();

                foreach (ImageSolutions.Item.MyGroupItem _Item in MyGroupItems)
                {
                    bool blnValid = true;

                    if (_Item.Item.WebsiteGroupItems != null
                        && _Item.Item.WebsiteGroupItems.FindAll(x => x.WebsiteID == CurrentWebsite.WebsiteID) != null 
                        && _Item.Item.WebsiteGroupItems.FindAll(x => x.WebsiteID == CurrentWebsite.WebsiteID).Count > 0)
                    {
                        blnValid = false;

                        foreach (ImageSolutions.Website.WebsiteGroupItem _WebsiteGroupItem in _Item.Item.WebsiteGroupItems)
                        {
                            if (CurrentWebsite.CombineWebsiteGroup)
                            {
                                if (CurrentUser.CurrentUserWebSite.UserAccounts.Exists(x => x.WebsiteGroupID == _WebsiteGroupItem.WebsiteGroupID))
                                {
                                    blnValid = true;
                                }
                            }
                            else
                            {
                                if (CurrentUser.CurrentUserWebSite.CurrentUserAccount.WebsiteGroupID == _WebsiteGroupItem.WebsiteGroupID)
                                {
                                    blnValid = true;
                                }
                            }
                        }

                        if (!blnValid)
                        {
                            RemoveMyGroupItems.Add(_Item);
                            //MyGroupItems.Remove(_Item);
                        }
                    }

                    if (_Item.Item.WebsiteGroupItemExcludes != null 
                        && _Item.Item.WebsiteGroupItemExcludes.FindAll(x => x.WebsiteID == CurrentWebsite.WebsiteID) != null
                        && _Item.Item.WebsiteGroupItemExcludes.FindAll(x => x.WebsiteID == CurrentWebsite.WebsiteID).Count > 0)
                    {
                        blnValid = true;

                        foreach (ImageSolutions.Website.WebsiteGroupItemExclude _WebsiteGroupItemExclude in _Item.Item.WebsiteGroupItemExcludes)
                        {
                            if (CurrentWebsite.CombineWebsiteGroup)
                            {
                                if (CurrentUser.CurrentUserWebSite.UserAccounts.Exists(x => x.WebsiteGroupID == _WebsiteGroupItemExclude.WebsiteGroupID))
                                {
                                    blnValid = false;
                                }
                            }
                            else
                            {
                                if (CurrentUser.CurrentUserWebSite.CurrentUserAccount.WebsiteGroupID == _WebsiteGroupItemExclude.WebsiteGroupID)
                                {
                                    blnValid = false;
                                }
                            }
                        }

                        if (!blnValid)
                        {
                            RemoveMyGroupItems.Add(_Item);
                            //MyGroupItems.Remove(_Item);
                        }
                    }

                    if (blnValid)
                    {
                        if (_Item.Item.Attributes != null)
                        {
                            foreach (ImageSolutions.Attribute.Attribute objAttribute in _Item.Item.Attributes)
                            {
                                if (objAttribute.AttributeValues != null)
                                {
                                    foreach (ImageSolutions.Attribute.AttributeValue objAttributeValue in objAttribute.AttributeValues)
                                    {
                                        if (objAttribute.AttributeName.ToLower() == "color")
                                        {
                                            if (!objColors.Exists(m => m.ToLower().Equals(objAttributeValue.Value.ToLower())))
                                            {
                                                objColors.Add(objAttributeValue.Value);
                                            }
                                        }

                                        if (objAttribute.AttributeName.ToLower() == "size")
                                        {
                                            if (!objSizes.Exists(m => m.ToLower().Equals(objAttributeValue.Value.ToLower())))
                                            {
                                                objSizes.Add(objAttributeValue.Value);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }      
                
                if(RemoveMyGroupItems != null && RemoveMyGroupItems.Count > 0)
                {
                    foreach (ImageSolutions.Item.MyGroupItem _Item in RemoveMyGroupItems)
                    {
                        MyGroupItems.Remove(_Item);
                    }
                }

                // Calculate the absolute price max.
                // When the price filter is active we loaded ALL items, so we have the
                // true global max — save it to Session so it survives paged (non-full) loads.
                // When not all items are loaded, fall back to any cached Session value so
                // the slider maximum never shrinks just because the page is filtered.
                string absMaxSessionKey = "Items_AbsMaxPrice_" + mWebSiteTabID + "_" + mSearch;
                if (MyGroupItems.Count > 0)
                {
                    double absMaxPrice = MyGroupItems.Select(x => Convert.ToDouble(x.Price)).Max();
                    double newMax = Math.Ceiling(absMaxPrice);

                    if (isPriceFilterActive)
                    {
                        // Full item set — this is the authoritative max; cache it
                        Session[absMaxSessionKey] = newMax.ToString("0");
                        hfPriceAbsMax.Value = newMax.ToString("0");
                    }
                    else
                    {
                        // Only a page of items — use cached max if available (more accurate)
                        if (Session[absMaxSessionKey] != null)
                            hfPriceAbsMax.Value = Session[absMaxSessionKey].ToString();
                        else
                            hfPriceAbsMax.Value = newMax.ToString("0");
                    }
                }

                // Apply price filter (always filter when hfPriceMax has a value)
                double filterMin = 0, filterMax = 0;
                double.TryParse(hfPriceMin.Value, out filterMin);
                double.TryParse(hfPriceMax.Value, out filterMax);
                if (filterMax > 0)
                {
                    MyGroupItems = MyGroupItems
                        .Where(x => Convert.ToDouble(x.Price) >= filterMin && Convert.ToDouble(x.Price) <= filterMax)
                        .ToList();
                }

                // When price filter loaded all items, update total count and page in memory
                if (isPriceFilterActive)
                {
                    intTotalRecord = MyGroupItems.Count;
                    int startIdx = (ucPager.CurrentPageNumber - 1) * ucPager.PageSize;
                    MyGroupItems = MyGroupItems.Skip(Math.Max(0, startIdx)).Take(ucPager.PageSize).ToList();
                }

                // Apply price sort in memory after filtering
                // Uses the item's lowest available price so variants with multiple
                // price points sort by their cheapest option.
                if (priceSort)
                {
                    MyGroupItems = ascending
                        ? MyGroupItems.OrderBy(x => GetMinPrice(x)).ToList()
                        : MyGroupItems.OrderByDescending(x => GetMinPrice(x)).ToList();
                }

                this.rptItems.DataSource = MyGroupItems;
                this.rptItems.DataBind();
                if (!string.IsNullOrEmpty(mSearch))
                {
                    ucPager.Visible = false;
                }
                else
                {
                    ucPager.TotalRecord = intTotalRecord;
                    ucPagerTop.TotalRecord = intTotalRecord;
                }

                pnlAttributeFilter.Visible = CurrentWebsite.DisplayAttributeFilter;
                ucLeftPanelNavigation.Visible = false;

                if (MyGroupItems.Count == 0 && pnlSubCategories.Visible)
                {
                    pnlItems.Visible = false;
                    if(!CurrentWebsite.DisplayLeftNavigation)
                    {
                        divLeftPanel.Visible = false;
                    }
                }

                if (!CurrentWebsite.DisplayAttributeFilter && !CurrentWebsite.DisplayLeftNavigation)
                {
                    divLeftPanel.Visible = false;
                }

                // Always show attribute filters when there are attributes available
                if (objColors.Count > 0 || objSizes.Count > 0)
                {
                    pnlAttributeFilter.Visible = true;
                    divLeftPanel.Visible = true;
                }

                // Category sidebar section permanently hidden
                pnlSidebarCategories.Visible = false;

                if (!Page.IsPostBack)
                {
                    this.lvSize.DataSource = SortSizes(objSizes).Select(m => new { AttributeValue = m });
                    this.lvSize.DataBind();

                    this.lvColor.DataSource = objColors.Select(m => new { AttributeValue = m });
                    this.lvColor.DataBind();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                objWebsiteTabItems = null;
                objFilter = null;
            }
        }

        public void AddItemFromWebsiteTab(ImageSolutions.Website.WebsiteTab websitetab, ref List<ImageSolutions.Item.Item> items)
        {
            if (!websitetab.Inactive
                        && (
                            websitetab.AllowAllGroups
                            ||
                            (
                                (!CurrentWebsite.CombineWebsiteGroup && websitetab.WebsiteGroupTabs.Exists(x => x.WebsiteGroupID == CurrentUser.CurrentUserWebSite.CurrentUserAccount.WebsiteGroupID))
                                ||
                                (CurrentWebsite.CombineWebsiteGroup && websitetab.WebsiteGroupTabs.Exists(x => CurrentUser.CurrentUserWebSite.UserAccounts.Exists(y => y.WebsiteGroup.WebsiteGroupID == x.WebsiteGroupID)))
                            )
                        )
                    )
            {
                if (websitetab.ChildWebsiteTabs != null && websitetab.ChildWebsiteTabs.Count > 0)
                {
                    foreach (ImageSolutions.Website.WebsiteTab _WebsiteTab in websitetab.ChildWebsiteTabs)
                    {
                        //if (!_WebsiteTab.Inactive
                        //    && (
                        //        _WebsiteTab.AllowAllGroups
                        //        ||
                        //        (
                        //            (!CurrentWebsite.CombineWebsiteGroup && _WebsiteTab.WebsiteGroupTabs.Exists(x => x.WebsiteGroupID == CurrentUser.CurrentUserWebSite.CurrentUserAccount.WebsiteGroupID))
                        //            ||
                        //            (CurrentWebsite.CombineWebsiteGroup && _WebsiteTab.WebsiteGroupTabs.Exists(x => CurrentUser.CurrentUserWebSite.UserAccounts.Exists(y => y.WebsiteGroup.WebsiteGroupID == x.WebsiteGroupID)))
                        //        )
                        //    )
                        //)
                        //{
                            AddItemFromWebsiteTab(_WebsiteTab, ref items);
                        //}
                    }
                }
                else
                {
                    foreach (ImageSolutions.Website.WebsiteTabItem _WebsiteTabItem in websitetab.WebsiteTabItems)
                    {
                        if (!items.Exists(x => x.ItemID == _WebsiteTabItem.Item.ItemID))
                        {
                            items.Add(_WebsiteTabItem.Item);
                        }
                    }                    
                }
            }                
        }

        public bool IsNumberic(string stringValue)
        {
            var pattern = @"^-?\d+(?:\.\d+)?$";
            var regex = new Regex(pattern);
            return regex.IsMatch(stringValue);
        }
        protected void rptItems_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem) return;

            ImageSolutions.Item.MyGroupItem MyGroupItem = (ImageSolutions.Item.MyGroupItem)e.Item.DataItem;
            Literal litColorSwatches = (Literal)e.Item.FindControl("litColorSwatches");
            if (litColorSwatches == null) return;

            string strSwatches = string.Empty;

            // Use the already-loaded object model exactly like ProductDetail.aspx does —
            // BackgroundColor is stored without '#', so we prepend it.
            if (MyGroupItem.Item != null && MyGroupItem.Item.Attributes != null)
            {
                foreach (ImageSolutions.Attribute.Attribute objAttribute in MyGroupItem.Item.Attributes)
                {
                    if (objAttribute.AttributeName.ToLower() == "color" && objAttribute.AttributeValues != null)
                    {
                        foreach (ImageSolutions.Attribute.AttributeValue objAttributeValue in objAttribute.AttributeValues)
                        {
                            string bg  = Convert.ToString(objAttributeValue.BackgroundColor);
                            string val = Convert.ToString(objAttributeValue.Value);
                            string style = !string.IsNullOrEmpty(bg)
                                ? string.Format("background-color:#{0};", bg)
                                : "background-color:#ccc;";
                            strSwatches += string.Format(
                                "<span class='color-dot' title='{0}' style='{1}'></span>", val, style);
                        }
                        break; // only the first Color attribute block needed
                    }
                }
            }

            litColorSwatches.Text = strSwatches;
        }

        // ── Price helper ─────────────────────────────────────────────────────────
        // Returns the lowest available price for an item using already-loaded data
        // only — no additional DB calls.
        private double GetMinPrice(ImageSolutions.Item.MyGroupItem item)
        {
            double basePrice = Convert.ToDouble(item.Price);
            return basePrice;
        }

        // ── Size ordering ────────────────────────────────────────────────────────
        private static readonly List<string> _sizeOrder = new List<string>
        {
            "XXXXXXXS", "XXXXXXSS", "XXXXXXS", "XXXXXS", "XXXXS", "XXXS", "XXS", "XS",
            "S", "M", "L", "XL", "XXL", "2XL",
            "3XL", "4XL", "5XL", "6XL", "7XL", "8XL", "9XL", "10XL"
        };

        private List<string> SortSizes(List<string> sizes)
        {
            return sizes
                .OrderBy(s =>
                {
                    int idx = _sizeOrder.FindIndex(o => o.Equals(s.Trim(), StringComparison.OrdinalIgnoreCase));
                    return idx < 0 ? int.MaxValue : idx;   // unknown sizes go to the end
                })
                .ToList();
        }

        // ── Title-case helper (callable from .aspx via <%# ToTitleCase(...) %>) ─
        protected string ToTitleCase(object value)
        {
            if (value == null) return string.Empty;
            string s = value.ToString();
            if (string.IsNullOrWhiteSpace(s)) return s;
            return System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(s.ToLower());
        }

        protected void rptItems_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "AddItem")
            {
                string strItemID = Convert.ToString(e.CommandArgument);

                //ImageSolutions.Item.Item objItem = new ImageSolutions.Item.Item(strItemID);
                ImageSolutions.Item.MyGroupItem MyGroupItem = CurrentUser.CurrentUserWebSite.CurrentUserAccount.MyGroupItems.Find(x => x.ItemID == strItemID);

                if (MyGroupItem != null)
                {
                    ImageSolutions.ShoppingCart.ShoppingCartLine newShoppingCartLine = new ImageSolutions.ShoppingCart.ShoppingCartLine();
                    newShoppingCartLine.ShoppingCartID = CurrentUser.CurrentUserWebSite.ShoppingCart.ShoppingCartID;
                    newShoppingCartLine.ItemID = MyGroupItem.ItemID;
                    newShoppingCartLine.Quantity = 1;
                    newShoppingCartLine.UnitPrice = MyGroupItem.Price;
                    newShoppingCartLine.Create();

                    CurrentUser.CurrentUserWebSite.ShoppingCart.Update();
                }
                Response.Redirect("/ShoppingCart.aspx");
            }
        }

        protected void chkSize_CheckedChanged(object sender, EventArgs e)
        {
            BindItems();
        }

        protected void chkColor_CheckedChanged(object sender, EventArgs e)
        {
            BindItems();
        }

        protected void rptCategory_ItemCommand(object source, RepeaterCommandEventArgs e)
        {

        }

        protected void ddlSort_SelectedIndexChanged(object sender, EventArgs e)
        {
            Session["Items_SortValue"] = ddlSort.SelectedValue;
            BindItems();
        }

        protected void ddlPageSize_SelectedIndexChanged(object sender, EventArgs e)
        {
            Session["Items_PageSize"] = ddlPageSize.SelectedValue;
            BindItems();
        }

        protected void btnApplyPrice_Click(object sender, EventArgs e)
        {
            // Persist price range to session so it survives pager redirects
            Session["Items_PriceMin"] = hfPriceMin.Value;
            Session["Items_PriceMax"] = hfPriceMax.Value;
            BindItems();
        }
    }
}