using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Text.RegularExpressions;
using System.Data.SqlClient;
using System.Net;
using System.IO;
using System.Collections.Generic;

public partial class WebUtility : Utility
{
    public static string PageSURL(string strParam)
    {
        return ConfigurationManager.AppSettings["HTTPS"] + "://" + ConfigurationManager.AppSettings["BaseURL"] + RemoveLeadingSlash(strParam);
    }

    public static string PageURL(string strParam)
    {
        return "http://" + ConfigurationManager.AppSettings["BaseURL"] + RemoveLeadingSlash(strParam);
    }

    public static string ImageSURL(string strParam)
    {
        return ConfigurationManager.AppSettings["HTTPS"] + "://" + GetImageBasePath() + RemoveLeadingSlash(strParam);
    }

    public static string ImageURL(string strParam)
    {
        if (IsHttps())
            return ConfigurationManager.AppSettings["HTTPS"] + "://" + GetImageBasePath() + RemoveLeadingSlash(strParam);
        else
            return "http://" + GetImageBasePath() + strParam;
    }

    private static string RemoveLeadingSlash(string strParam)
    {
        if (!string.IsNullOrEmpty(strParam) && strParam.StartsWith("/")) strParam = strParam.Substring(1, strParam.Length - 1);
        return strParam;
    }

    public static bool IsHttps()
    {
        if (System.Web.HttpContext.Current.Request.ServerVariables["HTTPS"] == "on")
            return true;
        else
            return false;
    }

    public static string GetBasePhysicalPath(string strParam)
    {
        return ConfigurationManager.AppSettings["BasePhysicalPath"] + strParam;
    }

    public static string GetImageBasePath()
    {
        return ConfigurationManager.AppSettings["BaseURL"] + "images/";
    }

    public static void SetSession(string strKey, object objValue)
    {
        System.Web.HttpContext.Current.Session[strKey] = objValue;
    }

    public static object GetSession(string strKey)
    {
        return System.Web.HttpContext.Current.Session[strKey];
    }

    public static void DisplayJavascriptMessage(System.Web.UI.Page Page, string Message)
    {
        ScriptManager.RegisterClientScriptBlock(Page, Page.GetType(), "ErrorMessage", "alert(\"" + Message.Replace(Environment.NewLine, "").Replace("\"", "\\\"") + "\");", true);
        //Page.ClientScript.RegisterClientScriptBlock(typeof(Page), "ErrorMessage", "alert(\"" + Message.Replace(Environment.NewLine, "").Replace("\"", "\\\"") + "\");", true);
    }

    //public static void DisplayJavascriptMessage(System.Web.UI.Page Page, string Message, bool AjaxPage)
    //{
    //    if (AjaxPage)
    //        ScriptManager.RegisterClientScriptBlock(Page, Page.GetType(), "ErrorMessage", "alert(\"" + Message.Replace(Environment.NewLine, "").Replace("\"", "\\\"") + "\");", true);
    //    else
    //        DisplayJavascriptMessage(Page, Message);
    //}

    public static string GetHtmlSourceCode(string strURL)
    {
        HttpWebRequest objRequest = null;
        WebResponse objResponse = null;
        string strReturn;

        try
        {

            objRequest = (HttpWebRequest)HttpWebRequest.Create(strURL);
            objRequest.UserAgent = "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.1; SV1; .NET CLR 1.1.4322; .NET CLR 2.0.50727)";
            objResponse = objRequest.GetResponse();
            using (StreamReader sr = new StreamReader(objResponse.GetResponseStream()))
            {
                strReturn = sr.ReadToEnd();
                sr.Close();
            }
        }
        catch (Exception ex)
        {
            throw ex;
        }
        finally
        {
            objRequest = null;
            if (objResponse != null)
            {
                objResponse.Close();
                objResponse = null;
            }
        }
        return strReturn;
    }

    public static bool UrlExists(string URL)
    {
        try
        {
            var request = WebRequest.Create(URL) as HttpWebRequest;
            if (request == null) return false;
            request.Method = "HEAD";
            using (var response = (HttpWebResponse)request.GetResponse())
            {
                return response.StatusCode == HttpStatusCode.OK;
            }
        }
        catch (UriFormatException)
        {
            //Invalid Url
            return false;
        }
        catch (WebException)
        {
            //Unable to access url
            return false;
        }
    }

    public static void BindDropDown(SqlDataReader objRead, DropDownList objDropDown, string strTextKey, string strValueKey, string strSelectedValue, ListItem DefaultItem)
    {
        ListItem objList = null;
        objDropDown.Items.Clear();
        if (DefaultItem != null) objDropDown.Items.Add(DefaultItem);

        try
        {
            while (objRead.Read())
            {
                objList = new ListItem(objRead[strTextKey].ToString(), objRead[strValueKey].ToString());

                if (strSelectedValue.ToLower() == objRead[strValueKey].ToString().ToLower()) objList.Selected = true;
                objDropDown.Items.Add(objList);
            }
        }
        catch (Exception ex)
        {
            throw ex;
        }
        finally
        {
            objRead.Close();
            objRead = null;
            DefaultItem = null;
        }
    }

    public static void BindDropDown(DataSet objData, DropDownList objDropDown, string strTextKey, string strValueKey, string strSelectedValue, ListItem DefaultItem)
    {
        ListItem objList = null;
        objDropDown.Items.Clear();
        if (DefaultItem != null) objDropDown.Items.Add(DefaultItem);

        try
        {
            if (objData != null && objData.Tables[0] != null)
            {
                for (int i = 0; i < objData.Tables[0].Rows.Count; i++)
                {
                    objList = new ListItem(objData.Tables[0].Rows[i][strTextKey].ToString(), objData.Tables[0].Rows[i][strValueKey].ToString());

                    if (strSelectedValue.ToLower() == objData.Tables[0].Rows[i][strValueKey].ToString().ToLower()) objList.Selected = true;
                    objDropDown.Items.Add(objList);
                }
            }
        }
        catch (Exception ex)
        {
            throw ex;
        }
        finally
        {
            objData = null;
            DefaultItem = null;
        }
    }

    public static bool SelectDropDownByValue(DropDownList objDropDown, string ValueToSelect)
    {
        bool blnReturn = false;

        objDropDown.ClearSelection();

        if (!string.IsNullOrEmpty(ValueToSelect))
        {
            foreach (ListItem objItem in objDropDown.Items)
            {
                if (string.Compare(ValueToSelect.Trim(), objItem.Value.Trim(), true) == 0)
                {
                    objItem.Selected = true;
                    blnReturn = true;
                    break;
                }
            }
        }
        return blnReturn;
    }

    public static bool SelectDropDownByText(DropDownList objDropDown, string TextToSelect)
    {
        bool blnReturn = false;

        objDropDown.ClearSelection();

        if (!string.IsNullOrEmpty(TextToSelect))
        {
            foreach (ListItem objItem in objDropDown.Items)
            {
                if (string.Compare(TextToSelect.Trim(), objItem.Text.Trim(), true) == 0)
                {
                    objItem.Selected = true;
                    blnReturn = true;
                    break;
                }
            }
        }
        return blnReturn;
    }

    public static void DefaultGridViewSortExpression(GridView GridView)
    {
        foreach (DataControlField objField in GridView.Columns)
        {
            if (objField is BoundField)
            {
                ((BoundField)objField).SortExpression = ((BoundField)objField).DataField;
            }
        }
    }

    /// <summary>
    /// This method only needs to be placed inside the !Page.IsPostback, ValidationGroup will persist on postback
    /// </summary>
    /// <param name="ParentControl"></param>
    /// <param name="ValidationGroupName"></param>
    public static void AssignValidationGroup(Control ParentControl, string ValidationGroupName)
    {
        if (ParentControl.Visible)
        {
            foreach (Control objControl in ParentControl.Controls)
            {
                if (objControl is ValidationSummary)
                    ((ValidationSummary)objControl).ValidationGroup += ValidationGroupName;
                else if (objControl is BaseValidator)
                    ((BaseValidator)objControl).ValidationGroup += ValidationGroupName;
                else if (objControl is IButtonControl)
                    ((IButtonControl)objControl).ValidationGroup += ValidationGroupName;

                if (objControl.HasControls())
                {
                    AssignValidationGroup(objControl, ValidationGroupName);
                }
            }
        }
    }

    public static void ClearForm(Control parent)
    {
        foreach (Control objControl in parent.Controls)
        {
            if (object.ReferenceEquals(objControl.GetType(), typeof(TextBox)))
            {
                ((TextBox)objControl).Text = string.Empty;
            }
            else if (object.ReferenceEquals(objControl.GetType(), typeof(CheckBox)))
            {
                ((CheckBox)objControl).Checked = false;
            }
            else if (object.ReferenceEquals(objControl.GetType(), typeof(RadioButton)))
            {
                ((RadioButton)objControl).Checked = false;
            }
            else if (object.ReferenceEquals(objControl.GetType(), typeof(DropDownList)))
            {
                ((DropDownList)objControl).SelectedIndex = -1;
            }
            else if (object.ReferenceEquals(objControl.GetType(), typeof(ListBox)))
            {
                ((ListBox)objControl).Items.Clear();
            }
            if (objControl.Controls.Count > 0)
            {
                ClearForm(objControl);
            }
        }
    }

    public static void SetDefaultButton(System.Web.UI.HtmlControls.HtmlForm ThisForm, Control[] StartingControls)
    {
        for (int i = 0; i < StartingControls.Length; i++)
        {
            if (SetDefaultButton(ThisForm, StartingControls[i])) break;
        }
    }

    public static bool SetDefaultButton(System.Web.UI.HtmlControls.HtmlForm ThisForm, Control StartingControl)
    {
        foreach (Control objControl in StartingControl.Controls)
        {
            if (object.ReferenceEquals(objControl.GetType(), typeof(Button)))
            {
                ThisForm.DefaultButton = ((Button)objControl).UniqueID;
                return true;
            }
            else if (object.ReferenceEquals(objControl.GetType(), typeof(ImageButton)))
            {
                ThisForm.DefaultButton = ((ImageButton)objControl).UniqueID;
                return true;
            }
            if (objControl.Controls.Count > 0)
            {
                return SetDefaultButton(ThisForm, objControl);
            }
        }
        return false;
    }

    public static string GetClientIPAddress()
    {
        string strReturn = string.Empty;

        try
        {
            if (IsWebApplication)
            {
                strReturn = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
                if (!string.IsNullOrEmpty(strReturn))
                {
                    string[] ipRange = strReturn.Split(',');
                    strReturn = ipRange[0];
                }
                else
                {
                    strReturn = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                }
            }
        }
        catch { }
        return strReturn;
    }

    public static void ShowNoResultFound<T>(List<T> source, GridView gv) where T : new()
    {
        if (source == null) return;
        source.Add(new T());
        gv.DataSource = source;
        gv.DataBind();
        int columnsCount = gv.Columns.Count;
        gv.Rows[0].Cells[0].ColumnSpan = columnsCount;
        for (int i = 1; i < columnsCount; i++)
        {
            gv.Rows[0].Cells[i].Visible = false;
        }
        gv.Rows[0].Cells[0].Text = gv.EmptyDataText; //this gets lost after postback
        gv.Rows[0].Visible = false;
    }

    public static void ClearAsyncFile(Control control)
    {
        for (var i = 0; i < HttpContext.Current.Session.Keys.Count; i++)
        {
            string strKey = HttpContext.Current.Session.Keys[0];
            if (HttpContext.Current.Session.Keys[i].Contains(control.ClientID))
            {
                HttpContext.Current.Session.Remove(HttpContext.Current.Session.Keys[i]);
                break;
            }
        }
    }
}