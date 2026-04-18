using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ImageSolutionsWebsite.MyAccount
{
    public partial class Budgets : BasePageUserAccountAuth
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                BindBudgets();
            }
        }

        protected void BindBudgets()
        {
            this.gvBudgetAssignments.DataSource = CurrentUser.CurrentUserWebSite.MyBudgetAssignments.FindAll(x => !x.BudgetAssignment.InActive);
            this.gvBudgetAssignments.DataBind();
            //this.lblTotal.Text = CurrentUser.CurrentUserWebSite.ShoppingCart.Total.ToString("C");

            //if (this.gvShoppingCartLine.HeaderRow != null) this.gvShoppingCartLine.HeaderRow.TableSection = TableRowSection.TableHeader;
        }
    }
}