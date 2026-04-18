using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CLRFramework
{
    public class BaseUserControl : System.Web.UI.UserControl
    {
        public bool SortAscending(string SortExpression)
        {
            if (!string.IsNullOrEmpty(SortExpression) && ViewState["SortExpression"] != null && ViewState["SortAscending"] != null && ViewState["SortExpression"].ToString() == SortExpression)
            {
                if (Convert.ToBoolean(ViewState["SortAscending"]) == true)
                {
                    ViewState["SortAscending"] = false;
                }
                else
                {
                    ViewState["SortAscending"] = true;
                }
            }
            else
            {
                ViewState["SortExpression"] = SortExpression;
                ViewState["SortAscending"] = true;
            }
            return Convert.ToBoolean(ViewState["SortAscending"]);
        }
    }
}
