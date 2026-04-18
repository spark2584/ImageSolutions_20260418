using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ImageSolutionsWebsite.MyAccount
{
    public partial class AddressBook : BasePageUserAccountAuth
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                BindAddressBook();
            }
        }

        protected void BindAddressBook()
        {
            this.gvAddressBook.DataSource = CurrentUser.AddressBooks;
            this.gvAddressBook.DataBind();
            //this.lblTotal.Text = CurrentUser.CurrentUserWebSite.ShoppingCart.Total.ToString("C");

            //if (this.gvShoppingCartLine.HeaderRow != null) this.gvShoppingCartLine.HeaderRow.TableSection = TableRowSection.TableHeader;
        }

        protected void btnAddNewAddressBook_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                if (string.IsNullOrEmpty(Request.QueryString.Get("target")))
                {
                    Response.Redirect(string.Format("/myaccount/AddressBookEditMode.aspx"));
                }
                else
                {
                    Response.Redirect(Request.QueryString.Get("target"));
                }
            }
        }

        protected void gvAddressBook_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            ImageSolutions.Address.AddressBook objAddressBook = null;
            string strAddressBookID = string.Empty;

            try
            {
                strAddressBookID = Convert.ToString(e.CommandArgument);
                objAddressBook = new ImageSolutions.Address.AddressBook(strAddressBookID);

                if (objAddressBook != null)
                {
                    if (e.CommandName == "EditAddress")
                    {
                        Response.Redirect(string.Format("/myaccount/AddressBookEditMode.aspx?AddressBookID={0}", strAddressBookID));
                        //objAddressBook.Update();
                    }
                    else if (e.CommandName == "DeleteAddress")
                    {
                        objAddressBook.Delete();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                objAddressBook = null;
            }
            Response.Redirect("/myaccount/AddressBook.aspx");
        }

        protected void gvAddressBook_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    string strAddressBookID = gvAddressBook.DataKeys[e.Row.RowIndex].Value.ToString();

                    CheckBox cbDefaultShipping = (CheckBox)e.Row.FindControl("cbDefaultShipping");
                    CheckBox cbDefaultBilling = (CheckBox)e.Row.FindControl("cbDefaultBilling");

                    cbDefaultShipping.Checked = Convert.ToString(CurrentUser.DefaultShippingAddressBookID) == strAddressBookID;
                    cbDefaultBilling.Checked = Convert.ToString(CurrentUser.DefaultBillingAddressBookID) == strAddressBookID;
                }
            }
            catch (Exception ex)
            {
                WebUtility.DisplayJavascriptMessage(this, ex.Message);
            }
        }
        protected void btnExport_Click(object sender, EventArgs e)
        {
            string strSQL = string.Empty;
            Hashtable dicParam = null;

            List<ImageSolutions.User.UserWebsite> objUserWebsite = null;
            ImageSolutions.User.UserWebsiteFilter objFilter = null;
            try
            {
                string strPath = Server.MapPath("\\Export\\AddressBook\\" + DateTime.UtcNow.ToString("yyyyMM") + "\\");
                if (!Directory.Exists(strPath))
                {
                    Directory.CreateDirectory(strPath);
                }

                string strFileExportPath = Server.MapPath(string.Format("\\Export\\AddressBook\\{0}\\AddressBook_{1}.csv", DateTime.UtcNow.ToString("yyyyMM"), DateTime.UtcNow.ToString("yyyyMMddHHmmssffff")));
                CreateExportCSV(strFileExportPath);

                Response.ContentType = "text/csv";
                //Response.ContentType = "application/vnd.ms-excel";
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
            SqlDataReader objRead = null;
            string strDBSQL = string.Empty;

            StringBuilder objReturn = new StringBuilder();

            int intCount = 0;

            try
            {
                //Header
                objReturn.Append(string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8}"
                    , "First Name"
                    , "Last Name"
                    , "Address Line 1"
                    , "Address Line 2"
                    , "City"
                    , "State"
                    , "Postal Code"
                    , "Country"
                    , "Phone"));
                objReturn.AppendLine();

                strDBSQL = string.Format(@"

SELECT REPLACE(ab.FirstName,',', ' ') as FirstName
	, REPLACE(ab.LastName,',', ' ') as LastName
	, REPLACE(ab.AddressLine1,',', ' ') as AddressLine1
	, REPLACE(ab.AddressLine2,',', ' ') as AddressLine2
	, REPLACE(ab.City,',', ' ') as City
	, REPLACE(ab.State,',', ' ') as State
	, REPLACE(ab.PostalCode,',', ' ') as PostalCode
	, REPLACE(ISNULL(ab.CountryCode,'US'),',', ' ') as CountryCode
	, REPLACE(ISNULL(ab.PhoneNumber,''),',', ' ') as Phone
FROM AddressBook (NOLOCK) ab
WHERE ab.UserInfoID = {0} "
                    , Database.HandleQuote(CurrentUser.UserInfoID) );


                objRead = Database.GetDataReader(strDBSQL);

                while (objRead.Read())
                {
                    objReturn.Append(string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8}"
                        , Convert.ToString(objRead["FirstName"])
                        , Convert.ToString(objRead["LastName"])
                        , Convert.ToString(objRead["AddressLine1"])
                        , Convert.ToString(objRead["AddressLine2"])
                        , Convert.ToString(objRead["City"])
                        , Convert.ToString(objRead["State"])
                        , Convert.ToString(objRead["PostalCode"])
                        , Convert.ToString(objRead["CountryCode"]) 
                        , Convert.ToString(objRead["Phone"]) )
                    );
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
                throw ex;
            }
            finally
            {
                if (objRead != null) objRead.Dispose();
                objRead = null;
            }
        }        
    }
}