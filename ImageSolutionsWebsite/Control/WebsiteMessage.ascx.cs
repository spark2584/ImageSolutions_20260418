using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ImageSolutionsWebsite.Control
{
    public partial class WebsiteMessage : BaseControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            this.Visible = false;

            if (ThisPage.CurrentWebsite.IsLoggedIn && HttpContext.Current.Request.Cookies["WebsiteMessage"] == null && !Request.Url.ToString().ToLower().Contains("login.aspx"))
            {
                if (ThisPage.CurrentWebsite.WebsiteMessages != null && ThisPage.CurrentWebsite.WebsiteMessages.Count > 0)
                {
                    //ImageSolutions.Website.WebsiteMessage objWebsiteMessage = ThisPage.CurrentWebsite.WebsiteMessages.Find(m => m.StartDate <= DateTime.Now.Date && m.EndDate > DateTime.Now.Date);
                    //if (objWebsiteMessage != null)
                    //{
                    //    this.lblMessage.Text = objWebsiteMessage.Message;
                    //    this.Visible = true;
                    //    HttpContext.Current.Response.Cookies["WebsiteMessage"].Value = objWebsiteMessage.WebsiteMessageID;
                    //}

                    //string strMessage = string.Empty;
                    string strWebsiteMessageIDs = string.Empty;
                    List<ImageSolutions.Website.WebsiteMessage> WebsiteMessages = ThisPage.CurrentWebsite.WebsiteMessages.FindAll(m => m.IsNotification == true && m.StartDate <= DateTime.Now.Date && m.EndDate > DateTime.Now.Date).OrderBy(x => x.StartDate).ToList();
                    foreach(ImageSolutions.Website.WebsiteMessage _WebsiteMessage in WebsiteMessages)
                    {
                        //strMessage = string.Format(@"{0}<br>{1}<br>{2}<br>", strMessage, _WebsiteMessage.StartDate.ToString("MM/dd/yyyy"), _WebsiteMessage.Message);

                        strWebsiteMessageIDs = String.Format(@"{0} {1}", strWebsiteMessageIDs, _WebsiteMessage.WebsiteMessageID);
                    }

                    if (WebsiteMessages != null && WebsiteMessages.Count > 0)
                    {
                        this.Visible = true;

                        rptAnnoucnement.DataSource = WebsiteMessages;
                        rptAnnoucnement.DataBind();

                        HttpContext.Current.Response.Cookies["WebsiteMessage"].Value = strWebsiteMessageIDs;
                    }
                }
            }
        }

        protected void rptAnnoucnement_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            try
            {
                if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
                {
                    Label lblDate = (Label)e.Item.FindControl("lblDate");
                    Literal litMessage = (Literal)e.Item.FindControl("litMessage");

                    string strWebsiteMessageID = ((HiddenField)e.Item.FindControl("hfWebsiteMessageID")).Value;
                    ImageSolutions.Website.WebsiteMessage WebsiteMessage = new ImageSolutions.Website.WebsiteMessage(strWebsiteMessageID);
                    lblDate.Text = WebsiteMessage.StartDate.ToString("MM/dd/yyyy");
                    litMessage.Text = WebsiteMessage.Message;

                }
            }
            catch (Exception ex)
            {
                
            }
        }
    }
}