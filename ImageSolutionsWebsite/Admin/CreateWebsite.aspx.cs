using Amazon.S3;
using Amazon.S3.Transfer;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ImageSolutionsWebsite.Admin
{
    public partial class CreateWebsite : BasePageAdminUserAuth
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }
        protected void btnCreate_Click(object sender, EventArgs e)
        {

            try
            {
                ImageSolutions.Website.Website Website = new ImageSolutions.Website.Website();
                Website.Name = txtName.Text;
                Website.Domain = txtDomain.Text;
                Website.InActive = cbInactive.Checked;
                Website.LogoPath = imgLogo.ImageUrl;
                Website.Create();

                //Response.Redirect(String.Format("/Admin/Website.aspx?id={0}", Website.WebsiteID));
                Response.Redirect("/Admin/WebsiteOverview.aspx");
            }
            catch (Exception ex)
            {
                WebUtility.DisplayJavascriptMessage(this, ex.Message);
            }
            finally { }

        }
        protected void btnCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect("/Admin/WebsiteOverview.aspx");
        }

        protected void btnUpload_Click(object sender, EventArgs e)
        {
            if (fuLogoImage.PostedFile.ContentLength > 0)
            {
                Stream objStream = fuLogoImage.PostedFile.InputStream;
                string strExtension = Path.GetExtension(fuLogoImage.PostedFile.FileName);

                string strFilename = string.Format("Website/{0}/{1}{2}", DateTime.UtcNow.ToString("yyyyMM"), DateTime.UtcNow.ToString("yyyyMMddHHmmssffffff"), strExtension);
                SaveFile(objStream, strFilename);

                string strImagePath = string.Format("{0}{1}", Convert.ToString(ConfigurationManager.AppSettings["ImageURL"]), strFilename);
                imgLogo.ImageUrl = strImagePath;
            }
        }
        protected void btnRemoveImage_Click(object sender, EventArgs e)
        {
            imgLogo.ImageUrl = String.Empty;
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
                if (Source.CanSeek)
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
            catch (Exception ex)
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