using Amazon.S3;
using Amazon.S3.Transfer;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ImageSolutionsWebsite.Admin
{
    public partial class CreateItem : BasePageAdminUserWebSiteAuth
    {
        private string mWebsiteID = string.Empty;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(Convert.ToString(HttpContext.Current.Request.Cookies["AdminWebsiteID"].Value)))
            {
                mWebsiteID = Convert.ToString(HttpContext.Current.Request.Cookies["AdminWebsiteID"].Value);
                GetWebsite();
            }
            //if (!string.IsNullOrEmpty(Request.QueryString.Get("websiteid")))
            //{
            //    mWebsiteID = Request.QueryString.Get("websiteid");
            //}
            else
            {
                Response.Redirect("/Admin/ItemOverview.aspx");
            }
        }
        protected void GetWebsite()
        {
            ImageSolutions.Website.Website Website = new ImageSolutions.Website.Website(mWebsiteID);
            lblWebsite.Text = Website.Name;
        }
        protected void btnCreate_Click(object sender, EventArgs e)
        {
            SqlConnection objConn = null;
            SqlTransaction objTran = null;

            try
            {
                objConn = new SqlConnection(Database.DefaultConnectionString);
                objConn.Open();
                objTran = objConn.BeginTransaction();



                ImageSolutions.Item.Item Item = new ImageSolutions.Item.Item();
                Item.ItemNumber = txtItemNumber.Text;
                Item.ItemName = txtItemName.Text;
                Item.ItemType = ddlItemType.SelectedValue;
                Item.StoreDisplayName = txtStoreDisplayName.Text;
                Item.SalesDescription = txtSalesDescription.Text;
                if (!string.IsNullOrEmpty(txtBasePrice.Text))
                {
                    Item.BasePrice = Convert.ToDouble(txtBasePrice.Text);
                }
                if (!string.IsNullOrEmpty(txtPurchasePrice.Text))
                {
                    Item.PurchasePrice = Convert.ToDouble(txtPurchasePrice.Text);
                }
                Item.ImageURL = imgItem.ImageUrl;
                Item.IsOnline = cbIsOnline.Checked;
                Item.InActive = cbInactive.Checked;
                Item.Create(objConn, objTran);

                ImageSolutions.Item.ItemWebsite ItemWebsite = new ImageSolutions.Item.ItemWebsite();
                ItemWebsite.WebsiteID = mWebsiteID;
                ItemWebsite.ItemID = Item.ItemID;
                ItemWebsite.Create(objConn, objTran);

                //Save Image
                Stream objStream = fuItemImage.PostedFile.InputStream;
                string strExtension = Path.GetExtension(fuItemImage.PostedFile.FileName);
                string strFilename = string.Format("Item/{0}/{1}{2}", DateTime.UtcNow.ToString("yyyyMM"), DateTime.UtcNow.ToString("yyyyMMddHHmmssffffff"), strExtension);
                SaveFile(objStream, strFilename);

                objTran.Commit();

                Response.Redirect(String.Format("/Admin/ItemOverview.aspx"));
            }
            catch (Exception ex)
            {
                if (objTran != null && objTran.Connection != null) objTran.Rollback();
                WebUtility.DisplayJavascriptMessage(this, ex.Message);
            }
            finally {
                if (objTran != null) objTran.Dispose();
                objTran = null;
                if (objConn != null) objConn.Dispose();
                objConn = null;
            }
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect(String.Format("/Admin/ItemOverview.aspx"));
        }
        protected void btnUpload_Click(object sender, EventArgs e)
        {
            if (fuItemImage.PostedFile.ContentLength > 0)
            {
                Stream objStream = fuItemImage.PostedFile.InputStream;
                string strExtension = Path.GetExtension(fuItemImage.PostedFile.FileName);

                string strFilename = string.Format("Item/{0}/{1}{2}", DateTime.UtcNow.ToString("yyyyMM"), DateTime.UtcNow.ToString("yyyyMMddHHmmssffffff"), strExtension);
                SaveFile(objStream, strFilename);

                string strImagePath = string.Format("{0}{1}", Convert.ToString(ConfigurationManager.AppSettings["ImageURL"]), strFilename);
                imgItem.ImageUrl = strImagePath;
            }
        }
        protected void btnRemoveImage_Click(object sender, EventArgs e)
        {
            imgItem.ImageUrl = String.Empty;
        }

        private AmazonS3Client mAmazonClient = null;
        private AmazonS3Client AmazonS3Client
        {
            get
            {
                if (mAmazonClient == null)
                {
                    Amazon.RegionEndpoint RegionEndPoint = null;

                    switch (Convert.ToString(ConfigurationManager.AppSettings["AWSRegionEndpoint"]))
                    {
                        case "USWest1":
                            RegionEndPoint = Amazon.RegionEndpoint.USWest1;
                            break;
                        case "USWest2":
                            RegionEndPoint = Amazon.RegionEndpoint.USWest2;
                            break;
                        case "USEast1":
                            RegionEndPoint = Amazon.RegionEndpoint.USEast1;
                            break;
                        case "USEast2":
                            RegionEndPoint = Amazon.RegionEndpoint.USEast2;
                            break;
                        default:
                            RegionEndPoint = Amazon.RegionEndpoint.USWest1;
                            break;
                    }


                    mAmazonClient = new AmazonS3Client(Convert.ToString(ConfigurationManager.AppSettings["AWSAccessKeyID"])
                        , Convert.ToString(ConfigurationManager.AppSettings["AWSSecretAccessKey"])
                        , RegionEndPoint);
                }
                return mAmazonClient;
            }
            set
            {
                mAmazonClient = value;
            }
        }
        private string S3BucketName = Convert.ToString(ConfigurationManager.AppSettings["AWSBucketName"]);
        private Hashtable mTempSavedKey = new Hashtable();
        protected void SaveFile(Stream Source, string Key)
        {
            TransferUtility objTransfer = null;
            TransferUtilityUploadRequest objRequest = null;

            try
            {
                if(Source.CanSeek)
                {
                    Source.Seek(0, SeekOrigin.Begin);

                    objTransfer = new TransferUtility(AmazonS3Client);
                    objRequest = new TransferUtilityUploadRequest();
                    objRequest.InputStream = Source;
                    objRequest.BucketName = S3BucketName;
                    objRequest.Key = Key;
                    objRequest.CannedACL = S3CannedACL.PublicRead;
                    objTransfer.Upload(objRequest);
                    mTempSavedKey[objRequest.Key] = true;
                }
                else
                {
                    throw new Exception("Unable to seek the input stream");
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }
            finally
            {
                objTransfer = null;
                objRequest = null;
            }
        }
    }
}