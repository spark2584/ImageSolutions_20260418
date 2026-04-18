using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using System.Net;
using System.Linq;
using System.Configuration;

namespace ImageSolutionsWebsite.MyAccount
{
    public partial class Orders : BasePageUserAccountAuth
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                BindSalesOrders();
            }
        }

        protected void BindSalesOrders()
        {
            this.gvSalesOrders.DataSource = CurrentUser.CurrentUserWebSite.SalesOrders.OrderByDescending(m => m.TransactionDate);
            this.gvSalesOrders.DataBind();
            //this.lblTotal.Text = CurrentUser.CurrentUserWebSite.ShoppingCart.Total.ToString("C");

            //if (this.gvShoppingCartLine.HeaderRow != null) this.gvShoppingCartLine.HeaderRow.TableSection = TableRowSection.TableHeader;

            gvSalesOrders.Columns[8].Visible = (ConfigurationManager.AppSettings["Environment"] == "staging" && CurrentWebsite.WebsiteID == "53")
                || (ConfigurationManager.AppSettings["Environment"] == "staging" && CurrentWebsite.WebsiteID == "26")
                || (ConfigurationManager.AppSettings["Environment"] == "production" && CurrentWebsite.WebsiteID == "20")
                || (ConfigurationManager.AppSettings["Environment"] == "production" && CurrentWebsite.WebsiteID == "2")
                || (ConfigurationManager.AppSettings["Environment"] == "production" && CurrentWebsite.WebsiteID == "24")
                || (ConfigurationManager.AppSettings["Environment"] == "production" && CurrentWebsite.WebsiteID == "27");

            gvSalesOrders.Columns[9].Visible = (ConfigurationManager.AppSettings["Environment"] == "staging" && CurrentWebsite.WebsiteID == "53")
                || (ConfigurationManager.AppSettings["Environment"] == "production" && CurrentWebsite.WebsiteID == "20");
        }

        protected void gvSalesOrders_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    HyperLink lnkOrderFile = (HyperLink)e.Row.FindControl("lnkOrderFile");
                    HyperLink lnkInvoiceFile = (HyperLink)e.Row.FindControl("lnkInvoiceFile");
                    HiddenField hfSalesOrderID = (HiddenField)e.Row.FindControl("hfSalesOrderID");

                    ImageSolutions.SalesOrder.SalesOrder SalesOrder = new ImageSolutions.SalesOrder.SalesOrder(Convert.ToString(hfSalesOrderID.Value));

                    lnkOrderFile.Visible = !string.IsNullOrEmpty(SalesOrder.OrderFilePath);
                    lnkInvoiceFile.Visible = !string.IsNullOrEmpty(SalesOrder.InvoiceFilePath);

                    HyperLink lnkReturnPath = (HyperLink)e.Row.FindControl("lnkReturnPath");

                    lnkReturnPath.NavigateUrl = String.Format("/ReturnAuthorization.aspx?SalesOrderID={0}", SalesOrder.SalesOrderID);


                    if (SalesOrder.DeliveryAddress.CountryCode == "US" && (DateTime.UtcNow - SalesOrder.CreatedOn).TotalDays <= 30)
                    {
                        lnkReturnPath.Visible = true;
                    }
                    else if (SalesOrder.DeliveryAddress.CountryCode == "CA" && (DateTime.UtcNow - SalesOrder.CreatedOn).TotalDays <= 45)
                    {
                        lnkReturnPath.Visible = true;
                    }
                    else
                    {
                        lnkReturnPath.Visible = false;
                    }

                    //Temp
                    if (CurrentWebsite.WebsiteID == "20")
                    {
                        if (SalesOrder.DeliveryAddress.CountryCode == "US" && (DateTime.UtcNow - SalesOrder.CreatedOn).TotalDays <= 120)
                        {
                            lnkReturnPath.Visible = true;
                        }
                        else if (SalesOrder.DeliveryAddress.CountryCode == "CA" && (DateTime.UtcNow - SalesOrder.CreatedOn).TotalDays <= 135)
                        {
                            lnkReturnPath.Visible = true;
                        }
                        else
                        {
                            lnkReturnPath.Visible = false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                WebUtility.DisplayJavascriptMessage(this, ex.Message);
            }
        }
    }
}