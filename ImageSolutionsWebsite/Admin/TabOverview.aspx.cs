using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ImageSolutionsWebsite.Admin
{
    public partial class TabOverview : BasePageAdminUserWebSiteAuth
    {
        List<ImageSolutions.Website.WebsiteTab> WebsiteTabs = null;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                BindWebsiteTab();
            }
        }

        protected void BindWebsiteTab()
        {
            try
            {               
                tvWebsiteTab.Nodes.Clear();

                foreach (ImageSolutions.Website.WebsiteTab _WebsiteTab in CurrentUser.CurrentUserWebSite.WebSite.WebsiteTabs)
                {
                    if (string.IsNullOrEmpty(_WebsiteTab.ParentID))
                    {
                        TreeNode ParentTreeNode = new TreeNode();
                        ParentTreeNode.Text = string.Format(@"{0}{1}", _WebsiteTab.TabName, _WebsiteTab.Inactive ? " (inactive)" : String.Empty);
                        ParentTreeNode.Value = _WebsiteTab.WebsiteTabID;
                        AddNodes(ref ParentTreeNode);
                        tvWebsiteTab.Nodes.Add(ParentTreeNode);
                    }
                }
                tvWebsiteTab.CollapseAll();
                //tvWebsiteTab.ExpandAll();
            }
            catch (Exception ex)
            {
                WebUtility.DisplayJavascriptMessage(this, ex.Message);
            }
            finally { }
        }

        private void AddNodes(ref TreeNode treenode)
        {
            DataTable DataTable = GetChildData(Convert.ToString(treenode.Value));
            foreach (DataRow _DataRow in DataTable.Rows)
            {
                TreeNode childNode = new TreeNode();
                childNode.Text = _DataRow["TabName"].ToString();
                childNode.Value = _DataRow["WebsiteTabID"].ToString();
                AddNodes(ref childNode);
                treenode.ChildNodes.Add(childNode);
            }
        }

        public DataTable GetChildData(string parentid)
        {
            DataTable DataTable = new DataTable();
            DataTable.Columns.AddRange(new DataColumn[] {
                new DataColumn("WebsiteTabID"),
                new DataColumn("ParentId"),
                new DataColumn("TabName") });

            foreach (ImageSolutions.Website.WebsiteTab _WebsiteTab in CurrentUser.CurrentUserWebSite.WebSite.WebsiteTabs)
            {
                if (_WebsiteTab.ParentID == parentid)
                {
                    DataRow DataRow = DataTable.NewRow();
                    DataRow["WebsiteTabID"] = _WebsiteTab.WebsiteTabID;
                    DataRow["ParentId"] = _WebsiteTab.ParentID;
                    DataRow["TabName"] = string.Format(@"{0}{1}", _WebsiteTab.TabName, _WebsiteTab.Inactive ? " (inactive)" : String.Empty); 
                    DataTable.Rows.Add(DataRow);
                }
            }
            return DataTable;
        }

        protected void tvWebsiteTab_SelectedNodeChanged(object sender, EventArgs e)
        {
            string strWebsiteTabID = Convert.ToString(tvWebsiteTab.SelectedNode.Value);
            Response.Redirect(String.Format("/Admin/Tab.aspx?id={0}", strWebsiteTabID));
        }
    }
}