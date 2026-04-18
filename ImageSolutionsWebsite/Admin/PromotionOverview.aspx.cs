using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Text;
using System.Collections;
using System.IO;

namespace ImageSolutionsWebsite.Admin
{
    public partial class PromotionOverview : BasePageAdminUserWebSiteAuth
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //base.PromotionPermission = true;

            if (!Page.IsPostBack)
            {
                BindPromotions();
            }
        }
        protected void BindPromotions()
        {
            BindPromotions(string.Empty);
        }
        protected void BindPromotions(string SortExpression)
        {
            ImageSolutions.Promotion.PromotionFilter objFilter = null;
            int intTotalRecord = 0;

            try
            {
                objFilter = new ImageSolutions.Promotion.PromotionFilter();
                //if (gvPromotions.HeaderRow != null)
                //{
                    //objFilter.PromotionCode = ((TextBox)this.gvPromotions.HeaderRow.FindControl("txtPromotionCode")).Text.Trim();
                    //objFilter.PromotionName = ((TextBox)this.gvPromotions.HeaderRow.FindControl("txtPromotionName")).Text.Trim();
                    //objFilter.CustomerEmail = ((TextBox)this.gvPromotions.HeaderRow.FindControl("txtCustomerEmail")).Text.Trim();
                    //objFilter.CustomerFirstName = ((TextBox)this.gvPromotions.HeaderRow.FindControl("txtCustomerFirstName")).Text.Trim();
                    //objFilter.CustomerLastName = ((TextBox)this.gvPromotions.HeaderRow.FindControl("txtCustomerLastName")).Text.Trim();
                //}

                if(!string.IsNullOrEmpty(txtPromotionCode.Text))
                {
                    objFilter.PromotionCode = txtPromotionCode.Text;
                }
                if(!string.IsNullOrEmpty(txtPromotionName.Text))
                {
                    objFilter.PromotionName = txtPromotionName.Text;
                }
                if(chkActiveOnly.Checked)
                {
                    objFilter.IsActive = true;
                    objFilter.FromDate = new Database.Filter.DateTimeSearch.SearchFilter();
                    objFilter.FromDate.Operator = Database.Filter.DateTimeSearch.SearchOperator.onOrBefore;
                    objFilter.FromDate.From = DateTime.UtcNow.Date;
                    objFilter.ToDate = new Database.Filter.DateTimeSearch.SearchFilter();
                    objFilter.ToDate.Operator = Database.Filter.DateTimeSearch.SearchOperator.onOrAfter;
                    //objFilter.ToDate.To = DateTime.UtcNow.Date;
                    objFilter.ToDate.From = DateTime.UtcNow.Date;
                }

                gvPromotions.DataSource = ImageSolutions.Promotion.Promotion.GetPromotion(CurrentWebsite.WebsiteID, objFilter, SortExpression, base.SortAscending(SortExpression), ucPagerBottom.PageSize, ucPagerBottom.CurrentPageNumber, out intTotalRecord);
                //gvPromotions.DataSource = ImageSolutions.Promotion.Promotion.GetPromotion(CurrentWebsite.WebsiteID, objFilter, SortExpression, SortAscending(SortExpression), ucPagerBottom.PageSize, ucPagerBottom.CurrentPageNumber, out intTotalRecord);
                gvPromotions.DataBind();
                if (this.gvPromotions.Rows.Count == 0) WebUtility.ShowNoResultFound(new List<ImageSolutions.Promotion.Promotion>(), gvPromotions);
                //if (gvPromotions.HeaderRow != null)
                //{
                    //((TextBox)this.gvPromotions.HeaderRow.FindControl("txtPromotionCode")).Text = objFilter.PromotionCode;
                    //((TextBox)this.gvPromotions.HeaderRow.FindControl("txtPromotionName")).Text = objFilter.PromotionName;
                    //((TextBox)this.gvPromotions.HeaderRow.FindControl("txtCustomerEmail")).Text = objFilter.CustomerEmail;
                    //((TextBox)this.gvPromotions.HeaderRow.FindControl("txtCustomerFirstName")).Text = objFilter.CustomerFirstName;
                    //((TextBox)this.gvPromotions.HeaderRow.FindControl("txtCustomerLastName")).Text = objFilter.CustomerLastName;
                //}
                ucPagerBottom.TotalRecord = intTotalRecord;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                objFilter = null;
            }
        }
        protected void PageIndexChanging(object sender, EventArgs e)
        {
            BindPromotions();
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            BindPromotions();
        }

        protected void gvPromotions_Sorting(object sender, GridViewSortEventArgs e)
        {
            BindPromotions(e.SortExpression);
        }

        protected void FilterSearchedChanged(object sender, EventArgs e)
        {
            BindPromotions();
        }

        protected void btnExport_Click(object sender, EventArgs e)
        {
            string strSQL = string.Empty;
            Hashtable dicParam = null;

            //List<ImageSolutions.Budget.Budget> Budgets = null;
            //ImageSolutions.Budget.BudgetFilter BudgetFilter = null;
            try
            {
                string strPath = Server.MapPath("\\Export\\Promotion\\" + DateTime.UtcNow.ToString("yyyyMM") + "\\");
                if (!Directory.Exists(strPath))
                {
                    Directory.CreateDirectory(strPath);
                }

                string strFileExportPath = Server.MapPath(string.Format("\\Export\\Promotion\\{0}\\Promotion_{1}.csv", DateTime.UtcNow.ToString("yyyyMM"), DateTime.UtcNow.ToString("yyyyMMddHHmmssffff")));
                CreateExportCSV(strFileExportPath);

                Response.ContentType = "text/csv";
                Response.AppendHeader("Content-Disposition", "attachment; filename=" + System.IO.Path.GetFileName(strFileExportPath));
                Response.WriteFile(strFileExportPath);

                HttpContext.Current.Response.Flush();
                HttpContext.Current.Response.SuppressContent = true;
                HttpContext.Current.ApplicationInstance.CompleteRequest();
            }
            catch (Exception ex)
            {
                WebUtility.DisplayJavascriptMessage(this, ex.Message);
            }
        }
        public void CreateExportCSV(string filepath)
        {
            StringBuilder objReturn = new StringBuilder();

            List<ImageSolutions.Promotion.Promotion> Promotions = null;
            ImageSolutions.Promotion.PromotionFilter PromotionFilter = null;

            try
            {
                objReturn.Append(string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11}"
                    , "id"
                    , "promotion_code"
                    , "name"
                    , "discount_percent"
                    , "discount_amount"
                    , "min_order_amount"
                    , "max_order_amount"
                    , "from_date"
                    , "to_date"
                    , "max_usage_count"
                    , "free_shipping_service"
                    , "is_active"
                    , "usage_count"
                    ));
                objReturn.AppendLine();


                PromotionFilter = new ImageSolutions.Promotion.PromotionFilter();

                if (!string.IsNullOrEmpty(txtPromotionCode.Text))
                {
                    PromotionFilter.PromotionCode = txtPromotionCode.Text;
                }
                if (!string.IsNullOrEmpty(txtPromotionName.Text))
                {
                    PromotionFilter.PromotionName = txtPromotionName.Text;
                }
                if (chkActiveOnly.Checked)
                {
                    PromotionFilter.IsActive = true;
                    PromotionFilter.FromDate = new Database.Filter.DateTimeSearch.SearchFilter();
                    PromotionFilter.FromDate.Operator = Database.Filter.DateTimeSearch.SearchOperator.onOrBefore;
                    PromotionFilter.FromDate.From = DateTime.UtcNow.Date;
                    PromotionFilter.ToDate = new Database.Filter.DateTimeSearch.SearchFilter();
                    PromotionFilter.ToDate.Operator = Database.Filter.DateTimeSearch.SearchOperator.onOrAfter;
                    PromotionFilter.ToDate.From = DateTime.UtcNow.Date;
                }

                int intTotalCount = 0;

                Promotions = ImageSolutions.Promotion.Promotion.GetPromotion(CurrentUser.CurrentUserWebSite.WebSite.WebsiteID, PromotionFilter, null, null, out intTotalCount);

                foreach (ImageSolutions.Promotion.Promotion _Promotion in Promotions)
                {
                    objReturn.Append(string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11}"
                        , Convert.ToString(_Promotion.PromotionID)
                        , Convert.ToString(_Promotion.PromotionCode)
                        , Convert.ToString(_Promotion.PromotionName)
                        , string.IsNullOrEmpty(Convert.ToString(_Promotion.DiscountPercent)) ? String.Empty : Convert.ToString(_Promotion.DiscountPercent * 100)
                        , Convert.ToString(_Promotion.DiscountAmount)
                        , Convert.ToString(_Promotion.MinOrderAmount)
                        , Convert.ToString(_Promotion.MaxOrderAmount)
                        , string.IsNullOrEmpty(Convert.ToString(_Promotion.FromDate)) ? String.Empty : Convert.ToDateTime(_Promotion.FromDate).ToString("MM/dd/yyyy")
                        , string.IsNullOrEmpty(Convert.ToString(_Promotion.ToDate)) ? String.Empty : Convert.ToDateTime(_Promotion.ToDate).ToString("MM/dd/yyyy")
                        , Convert.ToString(_Promotion.MaxUsageCount)
                        , _Promotion.FreeShippingService == null ? string.Empty : Convert.ToString(_Promotion.FreeShippingService.Description)
                        , _Promotion.IsActive ? "yes" : "no"
                        , Convert.ToString(_Promotion.UsageCount)
                    ));
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
                WebUtility.DisplayJavascriptMessage(this, ex.Message);
            }
            finally { }

        }
        protected void txtPromotionCode_TextChanged(object sender, EventArgs e)
        {
            BindPromotions();
        }

        protected void txtPromotionName_TextChanged(object sender, EventArgs e)
        {
            BindPromotions();
        }

        protected void chkActiveOnly_CheckedChanged(object sender, EventArgs e)
        {
            BindPromotions();
        }
        //protected void gvPromotions_RowCommand(object sender, GridViewCommandEventArgs e)
        //{
        //    GridViewRow objRow = (GridViewRow)((Control)e.CommandSource).NamingContainer;

        //    switch (e.CommandName.ToLower())
        //    {
        //        case "report":
        //            string strSQL = string.Empty;
        //            strSQL = "SELECT cil.ItemName, cil.UnitPriceBeforeDiscount, cil.UnitPrice, cil.Quantity, cil.Quantity*cil.UnitPrice AS SubTotal, cil.CreatedOn " +
        //                     "FROM CustInvoiceLine cil (NOLOCK) " +
        //                     "INNER JOIN CustInvoice ci (NOLOCK) ON cil.CustInvoiceID=ci.CustInvoiceID " +
        //                     "INNER JOIN PromotionTrans pt (NOLOCK) ON ci.CustInvoiceID=pt.CustInvoiceID " +
        //                     "INNER JOIN Promotion p (NOLOCK) ON pt.PromotionID=p.PromotionID " +
        //                     "WHERE ci.BusinessID=" + Database.HandleQuote(CurrentBusiness.BusinessID) +
        //                     "AND p.PromotionID=" + Database.HandleQuote(this.gvPromotions.DataKeys[objRow.RowIndex].Values["PromotionID"].ToString());
        //            ExportToSpreadsheet(Database.GetDataSet(strSQL).Tables[0], this.gvPromotions.DataKeys[objRow.RowIndex].Values["PromotionCode"].ToString());
        //            break;
        //        default:
        //            break;
        //    }
        //}
        //public void ExportToSpreadsheet(DataTable dt, string name)
        //{
        //    string attachment = "attachment; filename=" + name + ".xls";
        //    Response.ClearContent();
        //    Response.AddHeader("content-disposition", attachment);
        //    Response.ContentType = "application/vnd.ms-excel";

        //    string tab = "";
        //    foreach (DataColumn dc in dt.Columns)
        //    {
        //        Response.Write(tab + dc.ColumnName);
        //        tab = "\t";
        //    }

        //    Response.Write("\n");

        //    int i;

        //    foreach (DataRow dr in dt.Rows)
        //    {
        //        tab = "";
        //        for (i = 0; i < dt.Columns.Count; i++)
        //        {
        //            Response.Write(tab + dr[i].ToString());
        //            tab = "\t";
        //        }
        //        Response.Write("\n");
        //    }
        //    Response.End();
        //}
        //public override void VerifyRenderingInServerForm(Control control)
        //{
        //    /* Verifies that the control is rendered */
        //}
        //protected void ExportEventClicked(object sender, EventArgs e)
        //{
        //    int intPageSize = ucPagerBottom.PageSize;
        //    ucPagerBottom.PageSize = ucPagerBottom.TotalRecord;
        //    BindPromotions();
        //    ucPagerBottom.PageSize = intPageSize;

        //    Response.Clear();
        //    Response.Buffer = true;
        //    Response.AddHeader("content-disposition", "attachment;filename=" + ucPagerBottom.PagingRecordText + ".csv");
        //    Response.Charset = "";
        //    Response.ContentType = "application/text";
        //    //this.gvPromotions.AllowPaging = false;
        //    //this.gvPromotions.DataBind();
        //    StringBuilder sb = new StringBuilder();

        //    for (int k = 0; k < gvPromotions.Columns.Count; k++)
        //    {
        //        //add separator 
        //        sb.Append(gvPromotions.Columns[k].HeaderText + ',');
        //    }
        //    //append new line 
        //    sb.Append("\r\n");
        //    for (int i = 0; i < gvPromotions.Rows.Count; i++)
        //    {
        //        foreach (TableCell cell in gvPromotions.Rows[i].Cells)
        //        {
        //            string text = "";
        //            if (cell.Controls.Count > 0)
        //            {
        //                foreach (Control control in cell.Controls)
        //                {
        //                    string strType = control.GetType().Name;
        //                    switch (control.GetType().Name)
        //                    {
        //                        case "HyperLink":
        //                            text += (control as HyperLink).Text;
        //                            break;
        //                        case "TextBox":
        //                            text += (control as TextBox).Text;
        //                            break;
        //                        case "LinkButton":
        //                            text += (control as LinkButton).Text;
        //                            break;
        //                        case "Button":
        //                            text += (control as Button).Text;
        //                            break;
        //                        case "CheckBox":
        //                            text += (control as CheckBox).Text;
        //                            break;
        //                        case "RadioButton":
        //                            text += (control as RadioButton).Text;
        //                            break;
        //                        //case "LiteralControl":
        //                        //    if ((control as LiteralControl).Text.Trim() != Environment.NewLine) text += (control as LiteralControl).Text;
        //                        //    break;
        //                        //case "Literal":
        //                        //    if ((control as Literal).Text.Trim() != Environment.NewLine) text += (control as Literal).Text;
        //                        //    break;
        //                        case "DataBoundLiteralControl":
        //                            text += (control as DataBoundLiteralControl).Text.Trim();
        //                            break;
        //                    }
        //                }
        //            }
        //            else
        //            {
        //                text = cell.Text;
        //            }
        //            text = text.Replace(Environment.NewLine, "");
        //            sb.Append(text + ',');
        //        }
        //        sb.Append("\r\n");
        //    }
        //    Response.Output.Write(sb.ToString());
        //    Response.Flush();
        //    Response.End();
        //}
    }
}