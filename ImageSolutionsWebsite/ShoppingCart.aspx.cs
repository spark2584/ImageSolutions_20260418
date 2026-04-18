using ImageSolutions.ShoppingCart;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace ImageSolutionsWebsite
{
    public partial class ShoppingCart : BasePageUserAccountAuth
    {
        public bool IsBatchOrdering
        {
            get
            {
                if (Request.QueryString.Get("batch") == "t" && CurrentUser.CurrentUserWebSite.OrderManagement)
                {
                    Response.Cookies["IsBatchOrdering"].Value = "true";
                }
                else if (Request.Cookies["IsBatchOrdering"] == null)
                {
                    Response.Cookies["IsBatchOrdering"].Value = "false";
                }
                return Request.Cookies["IsBatchOrdering"].Value == "true";
            }
            set
            {
                Response.Cookies["IsBatchOrdering"].Value = value ? "true" : "false";
            }
        }

        public bool BatchOrderingValue
        {
            get
            {
                if (Request.Cookies["BatchOrderingValue"] == null)
                {
                    Response.Cookies["BatchOrderingValue"].Value = IsBatchOrdering ? "true" : "false";
                }
                return Request.Cookies["BatchOrderingValue"].Value == "true";
            }
            set
            {
                Response.Cookies["BatchOrderingValue"].Value = value ? "true" : "false";
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (CurrentWebsite.EnablePackagePayment)
            {
                Response.Redirect("/myaccount/dashboard.aspx");
            }

            if (!Page.IsPostBack)
            {

                Initalize();

                string strMessage = string.Empty;

                //if (
                    //(Convert.ToString(ConfigurationManager.AppSettings["Environment"]).ToLower() == "production" && CurrentWebsite.WebsiteID == "2") //BK Prod
                    //||
                //    (Convert.ToString(ConfigurationManager.AppSettings["Environment"]).ToLower() == "staging" && CurrentWebsite.WebsiteID == "26") //BK Staging
                //)

                if (CurrentWebsite.SuggestedSelling)
                {
                    string strRateIncreaseSuggestions = GetNoRateIncreaseSuggestions(CurrentUser.CurrentUserWebSite.ShoppingCart.ShoppingCartLines);
                    if (!string.IsNullOrEmpty(strRateIncreaseSuggestions))
                    {
                        strMessage = string.Format(@"{0}\n", strRateIncreaseSuggestions);
                    }
                    //if (!string.IsNullOrEmpty(strSuggestions))
                    //{
                    //    WebUtility.DisplayJavascriptMessage(this, strSuggestions);
                    //}
                }

                if (
                        (Convert.ToString(ConfigurationManager.AppSettings["Environment"]).ToLower() == "production" && CurrentWebsite.WebsiteID == "2")
                        ||
                        (Convert.ToString(ConfigurationManager.AppSettings["Environment"]).ToLower() == "staging" && CurrentWebsite.WebsiteID == "26")
                    )
                {
                    if (!CurrentWebsite.DisallowBackOrder)
                    {
                        foreach (ImageSolutions.ShoppingCart.ShoppingCartLine _ShoppingCartLine in CurrentUser.CurrentUserWebSite.ShoppingCart.ShoppingCartLines)
                        {
                            if (_ShoppingCartLine.Item.AllowBackOrder 
                                &&
                                (
                                    _ShoppingCartLine.Item.IsNonInventory && _ShoppingCartLine.Item.VendorInventory < _ShoppingCartLine.Quantity
                                    ||
                                    !_ShoppingCartLine.Item.IsNonInventory && _ShoppingCartLine.Item.QuantityAvailable < _ShoppingCartLine.Quantity
                                )
                            )
                            {
                                strMessage = string.Format(@"{0}\n{1} is temporarily out of stock but can be backordered. You will be charged at the time of purchase for both the product and shipping. Please check the home page announcement for the estimated restock date.\n"
                                    , strMessage, _ShoppingCartLine.Item.SalesDescription);
                            }
                        }
                    }
                }

                if (!string.IsNullOrEmpty(strMessage))
                {
                    WebUtility.DisplayJavascriptMessage(this, strMessage);
                }
            }
        }

        public static string GetNoRateIncreaseSuggestions(IEnumerable<ShoppingCartLine> cartLines, int maxCombos = 2)
        {
            if (cartLines == null) return string.Empty;

            var lines = cartLines
                .Where(l => l?.Item != null)
                .GroupBy(l => l.Item.StoreDisplayName)
                .Select(g => new
                {
                    Name = g.Key,
                    Weight = g.First().Item.UnitWeight.Value,
                    Qty = g.Sum(x => x.Quantity)
                })
                .Where(x => x.Weight > 0m)
                .ToList();

            if (lines.Count == 0) return string.Empty;

            decimal currentTotal = lines.Sum(l => l.Weight * l.Qty);
            decimal currentBand = (decimal)Math.Ceiling((double)currentTotal);
            decimal capacity = currentBand - currentTotal;

            if (capacity <= 0m) return string.Empty;

            var names = lines.Select(l => l.Name).ToArray();
            var weights = lines.Select(l => l.Weight).ToArray();

            // Precompute the maximum single-item adds (used to prune redundant single-item combos).
            var maxSingleAdds = new Dictionary<string, int>();
            for (int i = 0; i < names.Length; i++)
            {
                int maxAdd = (int)Math.Floor((double)(capacity / weights[i]));
                maxSingleAdds[names[i]] = Math.Max(0, maxAdd);
            }

            var combos = new HashSet<string>(); // store formatted strings
            var picked = new int[names.Length];

            void Recurse(int idx, decimal used)
            {
                if (combos.Count >= maxCombos) return;

                if (idx == names.Length)
                {
                    if (used > 0m && used <= capacity + 1e-9m)
                    {
                        // Check if this is a single-item combo and prune if not the max for that item
                        int nonZero = 0, nzIndex = -1, qty = 0;
                        for (int i = 0; i < picked.Length; i++)
                        {
                            if (picked[i] > 0)
                            {
                                nonZero++;
                                nzIndex = i;
                                qty = picked[i];
                                if (nonZero > 1) break;
                            }
                        }

                        if (nonZero == 1)
                        {
                            // Only keep the MAX quantity for that single item
                            string itemName = names[nzIndex];
                            if (qty < maxSingleAdds[itemName]) return;
                        }

                        // Format: "2 more A + 1 more B"
                        var parts = new List<string>();
                        for (int i = 0; i < names.Length; i++)
                        {
                            if (picked[i] > 0)
                            {
                                parts.Add($"{picked[i]} more {names[i]}");
                            }
                        }

                        if (parts.Count > 0)
                            combos.Add(string.Join(" + ", parts));
                    }
                    return;
                }

                int maxAddThis = (int)Math.Floor((double)((capacity - used) / weights[idx]));
                for (int add = 0; add <= maxAddThis; add++)
                {
                    picked[idx] = add;
                    decimal newUsed = used + add * weights[idx];
                    if (newUsed <= capacity + 1e-9m)
                    {
                        Recurse(idx + 1, newUsed);
                        if (combos.Count >= maxCombos) return;
                    }
                    else break;
                }
                picked[idx] = 0;
            }

            Recurse(0, 0m);

            if (combos.Count == 0) return string.Empty;

            var ordered = combos
                .OrderBy(s => s.Count(c => c == '+')) // prefer fewer distinct items
                .ThenBy(s => s.Length)
                .ToList();

            string joined = ordered.Count == 1
                ? ordered[0]
                : string.Join(", ", ordered.Take(ordered.Count - 1)) + ", or " + ordered.Last();

            return $"You can add {joined} and it will not affect your shipping cost";
        }

        protected void Initalize()
        {        
            try
            {
                if (CurrentWebsite.WebsiteID == "13" || CurrentWebsite.Name == "Enterprise Uniforms")
                {
                    btnTransfer.Text = "Proceed to Checkout";
                }

                HttpContext.Current.Response.Cookies["IsBatchOrdering"].Expires = System.DateTime.Now.AddDays(-1);
                HttpContext.Current.Response.Cookies["BatchOrderingValue"].Expires = System.DateTime.Now.AddDays(-1);

                this.chkAdvanced.Visible = false; //IsBatchOrdering;
                this.chkAdvanced.Checked = false; //BatchOrderingValue;

                if (
                    (ConfigurationManager.AppSettings["Environment"] == "production" && (CurrentWebsite.WebsiteID == "7" || CurrentWebsite.WebsiteID == "8"))
                    ||
                    (ConfigurationManager.AppSettings["Environment"] != "production" && (CurrentWebsite.WebsiteID == "43" || CurrentWebsite.WebsiteID == "45"))
                )
                {
                    litCompanyInvoiced.Text = "Safety Items Total :";
                    litOrderTotal.Text = "Uniform Total :";
                }

                if (CurrentUser.CurrentUserWebSite.ShoppingCart != null && CurrentUser.CurrentUserWebSite.ShoppingCart.DiscountAmount != null && Convert.ToDecimal(CurrentUser.CurrentUserWebSite.ShoppingCart.DiscountAmount) > 0)
                {
                    pnlDiscount.Visible = true;
                    lblDisocuntAmount.Text = string.Format("{0:c}", Convert.ToDecimal(CurrentUser.CurrentUserWebSite.ShoppingCart.DiscountAmount * -1));
                }
                else
                {
                    pnlDiscount.Visible = false;
                }
                

                BindShoppingCart();

                btnCheckOut.Visible = !CurrentWebsite.IsPunchout;
                btnTransfer.Visible = CurrentWebsite.IsPunchout;

                ValidateShoppingCartItems();

                pnlEstimatedShipping.Visible = false;
                if (CurrentWebsite.IsPunchout)
                {
                    if (CurrentUser.DefaultShippingAddressBook != null)
                    {
                        ImageSolutions.Address.AddressTrans ToAddressTrans = new ImageSolutions.Address.AddressTrans();
                        //ToAddressTrans.FirstName = ToAddressBook.FirstName;
                        //ToAddressTrans.LastName = ToAddressBook.LastName;
                        ToAddressTrans.AddressLine1 = CurrentUser.DefaultShippingAddressBook.AddressLine1;
                        ToAddressTrans.AddressLine2 = CurrentUser.DefaultShippingAddressBook.AddressLine2;
                        ToAddressTrans.City = CurrentUser.DefaultShippingAddressBook.City;
                        ToAddressTrans.State = CurrentUser.DefaultShippingAddressBook.State;
                        ToAddressTrans.PostalCode = CurrentUser.DefaultShippingAddressBook.PostalCode;
                        ToAddressTrans.CountryCode = CurrentUser.DefaultShippingAddressBook.CountryCode;
                        //ToAddressTrans.PhoneNumber = CurrentUser.DefaultShippingAddressBook.PhoneNumber;
                        //ToAddressTrans.Email = CurrentUser.DefaultShippingAddressBook.Email;

                        ImageSolutions.Shipping.WebShipRate.WebShipRateResponse result = null;

                        if (CurrentUser.CurrentUserWebSite.ShoppingCart.ShoppingCartLines != null && CurrentUser.CurrentUserWebSite.ShoppingCart.ShoppingCartLines.Count > 0)
                        {
                            double? dblEstimatedShipping = 0;

                            result = ImageSolutions.Shipping.Restlet.getWebShipRate(CurrentUser.CurrentUserWebSite.WebSite
                                , CurrentUser.CurrentUserWebSite.ShoppingCart
                                , ToAddressTrans
                                , CurrentUser.CurrentUserWebSite.CurrentUserAccount.WebsiteGroupID);

                            if (result != null && result.shippingMethods != null && result.shippingMethods.Count > 0)
                            {
                                dblEstimatedShipping = result.shippingMethods.FindAll(x => x.rate != null && x.rate > 0).Min(x => x.rate);

                                if (dblEstimatedShipping != null && Convert.ToDouble(dblEstimatedShipping) > 0)
                                {
                                    pnlEstimatedShipping.Visible = true;
                                    litEstimatedShipping.Text = string.Format("Estimated Shipping: {0:c}<br />Estimated Total: {1:c}", dblEstimatedShipping, dblEstimatedShipping + CurrentUser.CurrentUserWebSite.ShoppingCart.GetTotal(CurrentWebsite.WebsiteID, CurrentUser.CurrentUserWebSite.IsTaxExempt));
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
        }

        protected void BindShoppingCart()
        {
            bool blnShowOptions = false;

            CurrentUser.CurrentUserWebSite.ShoppingCart.ShoppingCartLines = null;

            //blnShowOptions = (CurrentUser.CurrentUserWebSite.WebSite.CustomLists != null && CurrentUser.CurrentUserWebSite.WebSite.CustomLists.Count > 0) || this.chkAdvanced.Checked;

            this.gvShoppingCartLine.DataSource = CurrentUser.CurrentUserWebSite.ShoppingCart.ShoppingCartLines.OrderBy(m => m.Item.ItemName);
            this.gvShoppingCartLine.DataBind();

            blnShowOptions = false;
            foreach (ImageSolutions.ShoppingCart.ShoppingCartLine _ShoppingCartLine in CurrentUser.CurrentUserWebSite.ShoppingCart.ShoppingCartLines)
            {
                if(!blnShowOptions && IsDisplayAccountUserItem(_ShoppingCartLine.Item.ParentID))
                {
                    blnShowOptions = true;
                }
            }

            this.gvShoppingCartLine.Columns[3].Visible = CurrentWebsite.DisplayTariffCharge;
            this.gvShoppingCartLine.Columns[6].Visible = blnShowOptions;
            this.gvShoppingCartLine.Columns[8].Visible = true; // !this.chkAdvanced.Checked;
            this.gvShoppingCartLine.Columns[9].Visible = false; // this.chkAdvanced.Checked;

            if (CurrentUser.CurrentUserWebSite.ShoppingCart.GetCompanyInvoicedAmount(CurrentWebsite.WebsiteID, CurrentUser.CurrentUserWebSite.IsTaxExempt) != 0)
            {
                pnlCompanyInvoicedAmount.Visible = true;
                this.lblCompanyInvoicedAmount.Text = Math.Abs(CurrentUser.CurrentUserWebSite.ShoppingCart.GetCompanyInvoicedAmount(CurrentWebsite.WebsiteID, CurrentUser.CurrentUserWebSite.IsTaxExempt)).ToString("C");
                //this.lblCompanyInvoicedAmount.Text = string.Format("{0:0.00}", Math.Abs(CurrentUser.CurrentUserWebSite.ShoppingCart.GetCompanyInvoicedAmount(CurrentWebsite.WebsiteID, CurrentUser.CurrentUserWebSite.IsTaxExempt)));
            }
            else
            {
                pnlCompanyInvoicedAmount.Visible = false;
            }

            this.lblTotal.Text = CurrentUser.CurrentUserWebSite.ShoppingCart.GetTotal(CurrentWebsite.WebsiteID, CurrentUser.CurrentUserWebSite.IsTaxExempt).ToString("C");

            if (this.gvShoppingCartLine.HeaderRow != null) this.gvShoppingCartLine.HeaderRow.TableSection = TableRowSection.TableHeader;
        }

        protected void UpdateShoppingCart()
        {
            ImageSolutions.ShoppingCart.ShoppingCartLine objShoppingCartLine = null;
            string strQuantity = string.Empty;
            string strUserInfoID = string.Empty;
            string strCustomListValue_1 = string.Empty;
            string strCustomListValue_2 = string.Empty;

            try
            {
                foreach (GridViewRow objRow in this.gvShoppingCartLine.Rows)
                {
                    HtmlControl divEmployee = (HtmlControl)objRow.FindControl("divEmployee");                    
                    objShoppingCartLine = CurrentUser.CurrentUserWebSite.ShoppingCart.ShoppingCartLines.Find(m => m.ShoppingCartLineID == this.gvShoppingCartLine.DataKeys[objRow.RowIndex].Values["ShoppingCartLineID"].ToString());
                    strQuantity = ((TextBox)objRow.FindControl("txtQuantity")).Text;

                    if(divEmployee.Visible)
                    {
                        strUserInfoID = ((DropDownList)objRow.FindControl("ddlUserInfo")).SelectedValue;
                        //strCustomListValue_1 = ((DropDownList)objRow.FindControl("ddlCustomListValue_1")).SelectedValue;
                        //strCustomListValue_2 = ((DropDownList)objRow.FindControl("ddlCustomListValue_2")).SelectedValue;
                    }

                    if (objShoppingCartLine != null && Utility.IsInteger(strQuantity) && (objShoppingCartLine.Quantity != Convert.ToInt32(strQuantity) || objShoppingCartLine.UserInfoID != strUserInfoID || objShoppingCartLine.CustomListValueID_1 != strCustomListValue_1 || objShoppingCartLine.CustomListValueID_2 != strCustomListValue_2))
                    {
                        objShoppingCartLine.Quantity = Convert.ToInt32(strQuantity);

                        if (divEmployee.Visible)
                        {
                            objShoppingCartLine.UserInfoID = strUserInfoID;
                        }

                        objShoppingCartLine.CustomListValueID_1 = strCustomListValue_1;
                        objShoppingCartLine.CustomListValueID_2 = strCustomListValue_2;
                        objShoppingCartLine.Update();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                objShoppingCartLine = null;
            }
        }

        protected bool IsDisplayAccountUserItem(string itemid)
        {
            bool blnReturn = false;

            if (CurrentWebsite.DisplayUserPermission == "All")
            {
                blnReturn = true;
            }
            else if (CurrentWebsite.DisplayUserPermission == "Store Only")
            {
                if (CurrentUser.CurrentUserWebSite.IsStore)
                {
                    blnReturn = true;
                }
            }

            if (!blnReturn)
            {
                List<ImageSolutions.Website.WebsiteTabItem> WebsiteTabItems = new List<ImageSolutions.Website.WebsiteTabItem>();
                ImageSolutions.Website.WebsiteTabItemFilter WebsiteTabItemFilter = new ImageSolutions.Website.WebsiteTabItemFilter();
                WebsiteTabItemFilter.ItemID = new Database.Filter.StringSearch.SearchFilter();
                WebsiteTabItemFilter.ItemID.SearchString = itemid;
                WebsiteTabItems = ImageSolutions.Website.WebsiteTabItem.GetWebsiteTabItems(WebsiteTabItemFilter);

                foreach (ImageSolutions.Website.WebsiteTabItem _WebsiteTabItem in WebsiteTabItems)
                {
                    if (!string.IsNullOrEmpty(_WebsiteTabItem.WebsiteTab.DisplayUserPermission))
                    {
                        if (_WebsiteTabItem.WebsiteTab.DisplayUserPermission == "All")
                        {
                            blnReturn = true;
                        }
                        else if (_WebsiteTabItem.WebsiteTab.DisplayUserPermission == "Store Only")
                        {
                            if (CurrentUser.CurrentUserWebSite.IsStore)
                            {
                                blnReturn = true;
                            }
                        }
                    }
                }
            }

            return blnReturn;
        }

        protected void gvShoppingCartLine_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    ImageSolutions.ShoppingCart.ShoppingCartLine objShoppingCartLine = (ImageSolutions.ShoppingCart.ShoppingCartLine)e.Row.DataItem;

                    HtmlControl rowOptions = (HtmlControl)e.Row.FindControl("rowOptions");
                    //{
                    //    if (objShoppingCartLine.Item.WebsiteTabItems != null && objShoppingCartLine.Item.ParentItem == null)
                    //    {
                    //        if (objShoppingCartLine.Item.WebsiteTabItems.Exists(m => m.WebsiteTab.TabName.ToLower() == "polo") || objShoppingCartLine.Item.WebsiteTabItems.Exists(m => m.WebsiteTab.TabName.ToLower() == "hoodie") || objShoppingCartLine.Item.WebsiteTabItems.Exists(m => m.WebsiteTab.TabName.ToLower() == "manager"))
                    //        {
                    //            rowOptions.Visible = true;
                    //        }
                    //    }
                    //    else if (objShoppingCartLine.Item.WebsiteTabItems != null && objShoppingCartLine.Item.ParentItem != null)
                    //    {
                    //        if (objShoppingCartLine.Item.ParentItem.WebsiteTabItems.Exists(m => m.WebsiteTab.TabName.ToLower() == "polo") || objShoppingCartLine.Item.ParentItem.WebsiteTabItems.Exists(m => m.WebsiteTab.TabName.ToLower() == "hoodie") || objShoppingCartLine.Item.ParentItem.WebsiteTabItems.Exists(m => m.WebsiteTab.TabName.ToLower() == "manager"))
                    //        {
                    //            rowOptions.Visible = true;
                    //        }
                    //    }                     
                    //}

                    // Display Employee List Option
                    if (!rowOptions.Visible && objShoppingCartLine.Item.ParentItem != null && IsDisplayAccountUserItem(objShoppingCartLine.Item.ParentItem.ItemID))
                    {
                        this.chkAdvanced.Checked = true;
                        this.gvShoppingCartLine.Columns[5].Visible = true;
                        rowOptions.Visible = true;
                    }


                    Image objItemImage = (Image)e.Row.FindControl("imgItemImage");
                    if (!string.IsNullOrEmpty(objShoppingCartLine.CustomDesignImagePath))
                        objItemImage.ImageUrl = objShoppingCartLine.CustomDesignImagePath;
                    else if (!string.IsNullOrEmpty(objShoppingCartLine.Item.ImageURL))
                        objItemImage.ImageUrl = objShoppingCartLine.Item.ImageURL;
                    else if (objShoppingCartLine.Item.ParentItem != null)
                        objItemImage.ImageUrl = objShoppingCartLine.Item.ParentItem.ImageURL;

                    if (this.chkAdvanced.Checked)
                    {
                        if (CurrentWebsite.DisplayUserPermission != "All With Account Change")
                        {
                            HtmlControl divEmployee = (HtmlControl)e.Row.FindControl("divEmployee");
                            divEmployee.Visible = true;
                            string strUserInfoID = this.gvShoppingCartLine.DataKeys[e.Row.RowIndex].Values["UserInfoID"].ToString();
                            DropDownList ddlUserInfo = (DropDownList)e.Row.FindControl("ddlUserInfo");
                            ddlUserInfo.DataSource = CurrentUser.CurrentUserWebSite.CurrentUserAccount.Account.UserInfos;
                            ddlUserInfo.DataBind();
                            //ddlUserInfo.Items.Insert(0, new ListItem("", ""));
                            if (!string.IsNullOrEmpty(strUserInfoID))
                            {
                                ddlUserInfo.SelectedIndex = ddlUserInfo.Items.IndexOf(ddlUserInfo.Items.FindByValue(strUserInfoID));
                            }
                            else
                            {
                                ddlUserInfo.SelectedIndex = ddlUserInfo.Items.IndexOf(ddlUserInfo.Items.FindByValue(CurrentUser.UserInfoID));
                            }
                        }                        
                    }

                    //if (CurrentUser.CurrentUserWebSite.WebSite.CustomLists != null)
                    //{
                    //    if (CurrentUser.CurrentUserWebSite.WebSite.CustomLists.Count >= 1)
                    //    {
                    //        HtmlControl divCustomList_1 = (HtmlControl)e.Row.FindControl("divCustomList_1");
                    //        divCustomList_1.Visible = true;

                    //        string strCustomListValueID_1 = this.gvShoppingCartLine.DataKeys[e.Row.RowIndex].Values["CustomListValueID_1"].ToString();
                    //        DropDownList ddlCustomListValue_1 = (DropDownList)e.Row.FindControl("ddlCustomListValue_1");
                    //        ddlCustomListValue_1.DataSource = CurrentUser.CurrentUserWebSite.WebSite.CustomLists[0].CustomListValues;
                    //        ddlCustomListValue_1.DataBind();
                    //        ddlCustomListValue_1.Items.Insert(0, new ListItem("", ""));
                    //        ddlCustomListValue_1.SelectedIndex = ddlCustomListValue_1.Items.IndexOf(ddlCustomListValue_1.Items.FindByValue(strCustomListValueID_1));
                    //    }

                    //    if (CurrentUser.CurrentUserWebSite.WebSite.CustomLists.Count >= 2)
                    //    {
                    //        HtmlControl divCustomList_2 = (HtmlControl)e.Row.FindControl("divCustomList_2");
                    //        divCustomList_2.Visible = true;

                    //        string strCustomListValueID_2 = this.gvShoppingCartLine.DataKeys[e.Row.RowIndex].Values["CustomListValueID_2"].ToString();
                    //        DropDownList ddlCustomListValue_2 = (DropDownList)e.Row.FindControl("ddlCustomListValue_2");
                    //        ddlCustomListValue_2.DataSource = CurrentUser.CurrentUserWebSite.WebSite.CustomLists[1].CustomListValues;
                    //        ddlCustomListValue_2.DataBind();
                    //        ddlCustomListValue_2.Items.Insert(0, new ListItem("", ""));
                    //        ddlCustomListValue_2.SelectedIndex = ddlCustomListValue_2.Items.IndexOf(ddlCustomListValue_2.Items.FindByValue(strCustomListValueID_2));
                    //    }
                    //}
                }
            }
            catch (Exception ex)
            {
                WebUtility.DisplayJavascriptMessage(this, ex.Message);
            }
        }

        protected void gvShoppingCart_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            ImageSolutions.ShoppingCart.ShoppingCartLine objShoppingCartLine = null;
            string strShoppingCartLineID = string.Empty;

            try
            {
                strShoppingCartLineID = Convert.ToString(e.CommandArgument);
                objShoppingCartLine = new ImageSolutions.ShoppingCart.ShoppingCartLine(strShoppingCartLineID);

                if (objShoppingCartLine != null)
                {
                    if (e.CommandName == "AddQty")
                    {
                        objShoppingCartLine.Quantity++;
                        objShoppingCartLine.Update();
                    }
                    else if (e.CommandName == "SubQty")
                    {
                        objShoppingCartLine.Quantity--;
                        objShoppingCartLine.Update();
                    }
                    else if (e.CommandName == "DeleteLine")
                    {
                        objShoppingCartLine.Delete();
                    }
                    else if (e.CommandName == "CopyLine")
                    {
                        objShoppingCartLine.Copy();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                objShoppingCartLine = null;
            }
            Response.Redirect("/ShoppingCart.aspx");
        }

        protected void btnContinueShopping_Click(object sender, EventArgs e)
        {
            Response.Redirect("/Default.aspx");
        }

        protected void btnUpdateCart_Click(object sender, EventArgs e)
        {
            try
            {
                UpdateShoppingCart();
                Response.Redirect("/ShoppingCart.aspx");
            }
            catch (Exception ex)
            {
                WebUtility.DisplayJavascriptMessage(this, ex.Message);
                BindShoppingCart();
            }
        }

        protected void btnCheckOut_Click(object sender, EventArgs e)
        {
            try
            {
                UpdateShoppingCart();
                Response.Redirect("/CheckOut.aspx");
            }
            catch (Exception ex)
            {
                WebUtility.DisplayJavascriptMessage(this, ex.Message);
                BindShoppingCart();
            }
        }

        protected void chkAdvanced_CheckedChanged(object sender, EventArgs e)
        {
            BatchOrderingValue = this.chkAdvanced.Checked;
            BindShoppingCart();
        }

        protected void ddlOptions_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                UpdateShoppingCart();
                BindShoppingCart();
            }
            catch (Exception ex)
            {
                WebUtility.DisplayJavascriptMessage(this, ex.Message);
                BindShoppingCart();
            }
        }

        protected void btnTransfer_Click(object sender, EventArgs e)
        {
            try
            {
                ImageSolutions.ShoppingCart.ShoppingCartTransfer.ReturnCart ReturnCart = new ShoppingCartTransfer.ReturnCart();

                ImageSolutions.ShoppingCart.ShoppingCartTransfer.Body Body = new ShoppingCartTransfer.Body();
                Body.edit_mode = 0;

                ImageSolutions.ShoppingCart.ShoppingCartTransfer.Items Items = new ShoppingCartTransfer.Items();
                Items.items = new List<ShoppingCartTransfer.Item>();

                foreach (ImageSolutions.ShoppingCart.ShoppingCartLine _ShoppingCartLine in CurrentUser.CurrentUserWebSite.ShoppingCart.ShoppingCartLines)
                {
                    if (_ShoppingCartLine.Quantity == 0)
                    {
                        throw new Exception("Quantity cannot be 0");
                    }

                    ImageSolutions.ShoppingCart.ShoppingCartTransfer.Item Item = new ShoppingCartTransfer.Item();

                    Item.quantity = _ShoppingCartLine.Quantity;
                    Item.supplierid = _ShoppingCartLine.Item.ItemNumber;

                    if (!string.IsNullOrEmpty(_ShoppingCartLine.Item.InternalID) && !string.IsNullOrEmpty(CurrentUser.CurrentUserWebSite.CustomerInternalID))
                    {
                        Item.supplierauxid = String.Format("{0}/{1}", _ShoppingCartLine.Item.InternalID, CurrentUser.CurrentUserWebSite.CustomerInternalID);
                    }
                    else
                    {
                        throw new Exception("Item Internal ID and Customer Internal ID are required");
                    }

                    Item.unitprice = Convert.ToString(_ShoppingCartLine.UnitTotal); //Convert.ToString(_ShoppingCartLine.UnitPrice);
                    
                    if (_ShoppingCartLine.TariffCharge != null)
                    {
                        Item.unitprice = Convert.ToString(Convert.ToDouble(Item.unitprice) + Convert.ToDouble(_ShoppingCartLine.TariffCharge));
                    }

                    Item.currency = "USD";
                    Item.uom = "EA";

                    string strOptionValue = string.Empty;

                    if (
                        (_ShoppingCartLine.ShoppingCartLineSelectableLogos != null && _ShoppingCartLine.ShoppingCartLineSelectableLogos.Count > 0)
                        ||
                        (_ShoppingCartLine.ItemPersonalizationValues != null && _ShoppingCartLine.ItemPersonalizationValues.Count > 0)
                        ||
                        !string.IsNullOrEmpty(_ShoppingCartLine.UserInfoID)
                    )
                    {
                        foreach (ImageSolutions.ShoppingCart.ShoppingCartLineSelectableLogo _ShoppingCartLineSelectableLogo in _ShoppingCartLine.ShoppingCartLineSelectableLogos)
                        {
                            if (!_ShoppingCartLineSelectableLogo.HasNoLogo && !string.IsNullOrEmpty(_ShoppingCartLineSelectableLogo.SelectableLogoID))
                            {
                                if (!string.IsNullOrEmpty(_ShoppingCartLineSelectableLogo.SelectableLogo.Placement))
                                {
                                    string strPlacement = _ShoppingCartLineSelectableLogo.SelectableLogo.Placement;
                                    strOptionValue = string.IsNullOrEmpty(strOptionValue)
                                        ? string.Format(@"{0}: ", strPlacement)
                                        : string.Format(@"{0}
{1}: "
                                            , strOptionValue, strPlacement);
                                }

                                string strLogoName = _ShoppingCartLineSelectableLogo.SelectableLogo.Name;
                                strOptionValue = string.IsNullOrEmpty(strOptionValue)
            ? strLogoName
            : string.Format(@"{0}
{1} "
                , strOptionValue, strLogoName);

                                if (!string.IsNullOrEmpty(_ShoppingCartLineSelectableLogo.SelectYear))
                                {
                                    strOptionValue = string.Format(@"{0}
{1} "
                                        , strOptionValue, string.Format("Years : {0}", Convert.ToString(_ShoppingCartLineSelectableLogo.SelectYear))
                                    );

                                }
                            }
                        }

                        if (string.IsNullOrEmpty(strOptionValue))
                        {
                            strOptionValue = string.IsNullOrEmpty(strOptionValue)
            ? string.Format(@"{0} ", "no patch")
            : string.Format(@"{0}
{1} "
                , strOptionValue, "no patch");
                        }

                        foreach (ImageSolutions.Item.ItemPersonalizationValue _ItemPersonalizationValue in _ShoppingCartLine.ItemPersonalizationValues)
                        {
                            if (!string.IsNullOrEmpty(_ItemPersonalizationValue.Value))
                            {
                                List<ImageSolutions.Item.ItemSelectableLogo> ItemSelectableLogos = new List<ImageSolutions.Item.ItemSelectableLogo>();
                                ImageSolutions.Item.ItemSelectableLogoFilter ItemSelectableLogoFilter = new ImageSolutions.Item.ItemSelectableLogoFilter();
                                ItemSelectableLogoFilter.ItemID = new Database.Filter.StringSearch.SearchFilter();
                                ItemSelectableLogoFilter.ItemID.SearchString = _ShoppingCartLine.Item.ParentID;
                                ItemSelectableLogos = ImageSolutions.Item.ItemSelectableLogo.GetItemSelectableLogos(ItemSelectableLogoFilter);

                                ImageSolutions.Item.ItemSelectableLogo ItemSelectableLogo = ItemSelectableLogos.FindLast(x => x.SelectableLogo.IsPersonalization);

                                if (ItemSelectableLogo != null && ItemSelectableLogo.SelectableLogo != null && !string.IsNullOrEmpty(ItemSelectableLogo.SelectableLogo.Placement))
                                {
                                    string strPlacement = ItemSelectableLogo.SelectableLogo.Placement;
                                    strOptionValue = string.IsNullOrEmpty(strOptionValue)
                                        ? string.Format(@"{0}: ", strPlacement)
                                        : string.Format(@"{0}
{1}: "
                                            , strOptionValue, strPlacement);
                                }

                                string strPersonalizationName = _ItemPersonalizationValue.ItemPersonalization.Name;
                                strOptionValue = string.IsNullOrEmpty(strOptionValue)
            ? String.Format("{0}: {1}", _ItemPersonalizationValue.ItemPersonalization.Name, _ItemPersonalizationValue.Value)
            : string.Format(@"{0}
{1}"
                , strOptionValue, String.Format("{0}: {1}", _ItemPersonalizationValue.ItemPersonalization.Name, _ItemPersonalizationValue.Value));

                                //                        if (string.IsNullOrEmpty(strOptionValue))
                                //                        {
                                //                            strOptionValue = string.Format("{0}: {1}"
                                //                                , _ItemPersonalizationValue.ItemPersonalization.Name
                                //                                , _ItemPersonalizationValue.Value);
                                //                        }
                                //                        else
                                //                        {
                                //                            strOptionValue = string.Format(@"{0} 
                                //{1}: {2}"
                                //                                , strOptionValue
                                //                                , _ItemPersonalizationValue.ItemPersonalization.Name
                                //                                , _ItemPersonalizationValue.Value);
                                //                        }
                            }
                        }
                    }

                    ImageSolutions.ShoppingCart.ShoppingCartTransfer.ItemOptions ItemOptions = null;
                    ImageSolutions.ShoppingCart.ShoppingCartTransfer.Option Option = null;

                    string strOptions = string.Empty;
                    if (
                        ((_ShoppingCartLine.ShoppingCartLineSelectableLogos != null && _ShoppingCartLine.ShoppingCartLineSelectableLogos.Count > 0)
                            ||
                            (_ShoppingCartLine.ItemPersonalizationValues != null && _ShoppingCartLine.ItemPersonalizationValues.Count > 0)
                        )
                        && !string.IsNullOrEmpty(strOptionValue))
                    {
                        ItemOptions = new ShoppingCartTransfer.ItemOptions();
                        ItemOptions.Opts = new List<ShoppingCartTransfer.Option>();

                        Option = new ShoppingCartTransfer.Option();
                        Option.id = "_itemembroidery";

                        Option.value = strOptionValue;
                        ItemOptions.Opts.Add(Option);
                    }

                    if (!string.IsNullOrEmpty(_ShoppingCartLine.UserInfoID))
                    {
                        string strOptionValueUserInfo = string.Empty;

                        ImageSolutions.User.UserWebsite UserWebsite = new ImageSolutions.User.UserWebsite();
                        ImageSolutions.User.UserWebsiteFilter UserWebsiteFilter = new ImageSolutions.User.UserWebsiteFilter();
                        UserWebsiteFilter.WebsiteID = new Database.Filter.StringSearch.SearchFilter();
                        UserWebsiteFilter.WebsiteID.SearchString = CurrentWebsite.WebsiteID;
                        UserWebsiteFilter.UserInfoID = new Database.Filter.StringSearch.SearchFilter();
                        UserWebsiteFilter.UserInfoID.SearchString = _ShoppingCartLine.UserInfoID;
                        UserWebsite = ImageSolutions.User.UserWebsite.GetUserWebsite(UserWebsiteFilter);

                        if (UserWebsite != null)
                        {
                            strOptionValueUserInfo = String.Format("{0} - {1}", Convert.ToString(UserWebsite.UserInfo.FullName), Convert.ToString(UserWebsite.EmployeeID));
                        }

                        if (ItemOptions == null)
                        {
                            ItemOptions = new ShoppingCartTransfer.ItemOptions();
                            ItemOptions.Opts = new List<ShoppingCartTransfer.Option>();
                        }

                        Option = new ShoppingCartTransfer.Option();
                        Option.id = "__toemployee";

                        Option.value = strOptionValueUserInfo;
                        ItemOptions.Opts.Add(Option);
                    }

                    if (ItemOptions != null && ItemOptions.Opts != null && ItemOptions.Opts.Count > 0)
                    {
                        strOptions = JsonConvert.SerializeObject(ItemOptions.Opts);
                    }

                    //strEmbroideryOptions = strEmbroideryOptions.Replace("\\","\\\\").Replace("\"", "\\\"");

                    //Item.description = String.Format("{0} {1}", _ShoppingCartLine.Item.SalesDescription, string.IsNullOrEmpty(strEmbroideryOptions) ? string.Empty : string.Format("\\nOpts:{0}", strEmbroideryOptions));
                    Item.description = String.Format("{0} {1}", _ShoppingCartLine.Item.SalesDescription, string.IsNullOrEmpty(strOptions) ? string.Empty : string.Format("Opts:{0}", strOptions));
                    Item.language = "en";
                    Items.items.Add(Item);
                }

                Body.items = Items;
                Body.total = CurrentUser.CurrentUserWebSite.ShoppingCart.LineTotal;
                Body.currency = "USD";
                ReturnCart.body = Body;

                string strCartContent = JsonConvert.SerializeObject(ReturnCart).Replace("\\\\r", string.Empty);
                byte[] byteCartContent = System.Text.Encoding.UTF8.GetBytes(strCartContent);
                string encodedCartContent = System.Convert.ToBase64String(byteCartContent);

                PostBackToPunchOut(encodedCartContent);
            }
            catch (Exception ex)
            {
                WebUtility.DisplayJavascriptMessage(this, ex.Message);
                BindShoppingCart();
            }            
        }

        protected void PostBackToPunchOut(string encodedcartcontent)
        {
            try
            {
                if (string.IsNullOrEmpty(CurrentUser.CurrentUserWebSite.PunchoutSessionID))
                {
                     throw new Exception("Missing Punchout Session");
                }

                string strPunchInURL = string.Format("{0}{1}", Convert.ToString(ConfigurationManager.AppSettings["TradeCentricPunchInURL"]), CurrentUser.CurrentUserWebSite.PunchoutSessionID);
                HttpWebRequest HttpWebRequest = (HttpWebRequest)WebRequest.Create(strPunchInURL);
                
                string strData = string.Format("apikey={0}&params={1}&version={2}"
                    , HttpUtility.UrlEncode(Convert.ToString(ConfigurationManager.AppSettings["TradeCentricAPIKEY"]))
                    , HttpUtility.UrlEncode(encodedcartcontent)
                    , HttpUtility.UrlEncode("1.0"));

                HttpWebRequest.Method = "POST";

                HttpWebRequest.Accept = "*/*";
                HttpWebRequest.KeepAlive = true;
                HttpWebRequest.ContentType = "application/x-www-form-urlencoded";
                HttpWebRequest.Host = "connect.punchout2go.com";
                //HttpWebRequest.ContentLength = byteArray.Length;
                //HttpWebRequest.UserAgent = "";

                HttpWebRequest.Headers.Add("accept-encoding", "gzip, deflate, br");
                //HttpWebRequest.Headers.Add("connection", "keep-alive");
                //HttpWebRequest.Headers.Add("host", "connect.punchout2go.com");
                //HttpWebRequest.Headers.Add("content-type", "application/x-www-form-urlencoded; charset=UTF-8");
                //HttpWebRequest.Headers.Add("accept", "text/*");

                //strData = HttpUtility.UrlEncode(strData);

                //Stream dataStream = HttpWebRequest.GetRequestStream();
                //dataStream.Write(byteArray, 0, byteArray.Length);
                //dataStream.Close();

                //HttpWebResponse response = (HttpWebResponse)HttpWebRequest.GetResponse();
                //dataStream = response.GetResponseStream();
                //StreamReader reader = new StreamReader(dataStream);
                //string strResponse = reader.ReadToEnd();
                //response.Close();

                string strResponse = string.Empty;

                using (StreamWriter _StreamWriter = new StreamWriter(HttpWebRequest.GetRequestStream()))
                {
                    _StreamWriter.Write(strData);
                    _StreamWriter.Close();
                    //_StreamWriter.Flush();

                    HttpWebResponse HttpWebResponse = (HttpWebResponse)HttpWebRequest.GetResponse();

                    //using (HttpWebResponse _HttpWebResponse = (HttpWebResponse)HttpWebRequest.GetResponse())
                    //{
                    //    using (StreamReader _StreamReader = new StreamReader(_HttpWebResponse.GetResponseStream()))
                    //    {
                    //        strResponse = _StreamReader.ReadToEnd();

                    //    }
                    //}
                }

                CurrentUser.CurrentUserWebSite.ShoppingCart.PunchoutSessionID = Convert.ToString(CurrentUser.CurrentUserWebSite.PunchoutSessionID);
                CurrentUser.CurrentUserWebSite.ShoppingCart.Update();

                string strReturnURL = string.Format("{0}{1}", CurrentUser.CurrentUserWebSite.PunchoutReturnURL, "/?redirect=1");

                Response.Redirect(strReturnURL);
            }
             catch (Exception ex)
            {
                WebUtility.DisplayJavascriptMessage(this, ex.Message);
            }
        }

        protected void ValidateShoppingCartItems()
        {
            foreach (ImageSolutions.ShoppingCart.ShoppingCartLine _ShoppingCartLine in CurrentUser.CurrentUserWebSite.ShoppingCart.ShoppingCartLines)
            {
                if (string.IsNullOrEmpty(_ShoppingCartLine.Item.ParentID) || !_ShoppingCartLine.Item.ItemWebsites.Exists(x => x.WebsiteID == CurrentWebsite.WebsiteID))
                {
                    throw new Exception(String.Format("Invalid Item: {0}", _ShoppingCartLine.Item.Description));
                }
            }
        }
    }
}