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

                objWebsiteTabItems = ImageSolutions.Website.WebsiteTabItem.GetWebsiteTabItems(objFilter, "Sort", true, ucPager.PageSize, ucPager.CurrentPageNumber, out intTotalRecord); ;

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
                ucLeftPanelNavigation.Visible = CurrentWebsite.DisplayLeftNavigation;

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

                if (!Page.IsPostBack)
                {
                    this.lvSize.DataSource = objSizes.Select(m => new { AttributeValue = m });
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
    }
}