using Amazon.Util;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Security.Cryptography;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ImageSolutionsWebsite
{
    public partial class DragAndDrop : System.Web.UI.Page
    {
        protected string mItemID = "1";

        protected void Page_Load(object sender, EventArgs e)
        {
            imgResult.ImageUrl = "/DragAndDrop/RaisingCane.jpg";
        }

        protected void btnGenerate_Click(object sender, EventArgs e)
        {
            int left = 0;
            int top = 0;

            string strPath = Server.MapPath("\\DragAndDrop\\");
            //System.Drawing.Image imgLogo = System.Drawing.Image.FromFile(strPath + filLogo.FileName);
            
            System.Drawing.Image imgLogo = Resize();

            Bitmap imgSource = new Bitmap(strPath + "RaisingCane.jpg");
            Graphics graphicFinal = Graphics.FromImage(imgSource);

            int intLeftMargin = Convert.ToInt32(imgLogo.Width * Convert.ToDouble(ddlLeftMargin.SelectedValue)) - imgLogo.Width;
            int intTopMargin = Convert.ToInt32(imgLogo.Height * Convert.ToDouble(ddlTopMargin.SelectedValue)) - imgLogo.Height;

            switch (ddlPosition.SelectedValue)
            {
                case "left-chest":
                    left = (imgSource.Width / 3) - ((imgLogo.Width + intLeftMargin) / 2);
                    top = (imgSource.Height / 5) - ((imgLogo.Height + intTopMargin) / 2);
                    break;
                case "right-chest":
                    left = (imgSource.Width / 3  * 2) - ((imgLogo.Width + intLeftMargin) / 2);
                    top = (imgSource.Height / 5) - ((imgLogo.Height + intTopMargin) / 2);
                    break;
                case "left-waist":
                    left = (imgSource.Width / 3) - ((imgLogo.Width + intLeftMargin) / 2);
                    top = (imgSource.Height / 5 * 4) - ((imgLogo.Height + intTopMargin) / 2);
                    break;
                case "right-waist":
                    left = (imgSource.Width / 3 * 2) - ((imgLogo.Width + intLeftMargin) / 2);
                    top = (imgSource.Height / 5 * 4) - ((imgLogo.Height + intTopMargin) / 2);
                    break;
                case "center":
                    left = (imgSource.Width / 2) - ((imgLogo.Width + intLeftMargin) / 2);
                    top = (imgSource.Height / 2) - ((imgLogo.Height + intTopMargin) / 2);
                    break;
                default:
                    break;
            }

            graphicFinal.DrawImage(imgLogo, new Point(left, top));

            imgSource.Save(strPath + "result.jpg", System.Drawing.Imaging.ImageFormat.Jpeg);

            imgResult.ImageUrl = "/DragAndDrop/result.jpg";
        }

        protected System.Drawing.Image Resize()
        {
            System.Drawing.Image imgReturn = null;

            if ((filLogo.PostedFile != null) && (filLogo.PostedFile.ContentLength > 0))
            {
                Guid uid = Guid.NewGuid();
                string fn = System.IO.Path.GetFileName(filLogo.PostedFile.FileName);
                string SaveLocation = Server.MapPath("\\DragAndDrop\\Logo\\") + "" + uid + fn;
                try
                {
                    string fileExtention = filLogo.PostedFile.ContentType;
                    int fileLenght = filLogo.PostedFile.ContentLength;
                    if (fileExtention == "image/png" || fileExtention == "image/jpeg" || fileExtention == "image/x-png")
                    {
                        if (fileLenght <= 3048576)
                        {
                            System.Drawing.Bitmap bmpPostedImage = new System.Drawing.Bitmap(filLogo.PostedFile.InputStream);
                            imgReturn = ScaleImage(bmpPostedImage, Convert.ToInt32(100 * Convert.ToDouble(this.ddlRatio.SelectedValue)));
                            // Saving image in png format
                            imgReturn.Save(SaveLocation, ImageFormat.Png);
                            imgUplodaedLogo.ImageUrl = "/DragAndDrop/logo/" + uid + fn;
                            //WebUtility.DisplayJavascriptMessage(this, "The file has been uploaded.");
                        }
                        else
                        {
                            throw new Exception("Image file cannot be more than 3MB");
                        }
                    }
                    else
                    {
                        throw new Exception("Invalid File Format");
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            else
            {
                imgReturn = System.Drawing.Image.FromFile(Server.MapPath(imgUplodaedLogo.ImageUrl));
                imgReturn = ScaleImage(imgReturn, Convert.ToInt32(100 * Convert.ToDouble(this.ddlRatio.SelectedValue)));
            }
            return imgReturn;
        }

        public static System.Drawing.Image ScaleImage(System.Drawing.Image image, int maxHeight)
        {
            var ratio = (double)maxHeight / image.Height;
            var newWidth = (int)(image.Width * ratio);
            var newHeight = (int)(image.Height * ratio);
            var newImage = new Bitmap(newWidth, newHeight);
            using (var g = Graphics.FromImage(newImage))
            {
                g.DrawImage(image, 0, 0, newWidth, newHeight);
            }
            return newImage;
        }

        protected void ddlPosition_SelectedIndexChanged(object sender, EventArgs e)
        {
            btnGenerate_Click(null, null);
        }

        protected void btnRatio_Click(object sender, EventArgs e)
        {
            if (((Button)sender).CommandArgument == "Minus")
            {
                this.ddlRatio.SelectedIndex = this.ddlRatio.SelectedIndex - 1;
            }
            else if (((Button)sender).CommandArgument == "Plus")
            {
                this.ddlRatio.SelectedIndex = this.ddlRatio.SelectedIndex + 1;
            }
            ddlRatio_SelectedIndexChanged(null, null);
        }

        protected void ddlRatio_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.btnRatioMinus.Enabled = this.ddlRatio.SelectedIndex != 0;
            this.btnRatioPlus.Enabled = this.ddlRatio.SelectedIndex < this.ddlRatio.Items.Count - 1;
            btnGenerate_Click(null, null);
        }

        protected void btnLeftMargin_Click(object sender, EventArgs e)
        {
            if (((Button)sender).CommandArgument == "Minus")
            {
                this.ddlLeftMargin.SelectedIndex = this.ddlLeftMargin.SelectedIndex - 1;
            }
            else if (((Button)sender).CommandArgument == "Plus")
            {
                this.ddlLeftMargin.SelectedIndex = this.ddlLeftMargin.SelectedIndex + 1;
            }
            ddlLeftMargin_SelectedIndexChanged(null, null);
        }

        protected void ddlLeftMargin_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.btnLeftMarginMinus.Enabled = this.ddlLeftMargin.SelectedIndex != 0;
            this.btnLeftMarginPlus.Enabled = this.ddlLeftMargin.SelectedIndex < this.ddlLeftMargin.Items.Count - 1;
            btnGenerate_Click(null, null);
        }

        protected void btnTopMargin_Click(object sender, EventArgs e)
        {
            if (((Button)sender).CommandArgument == "Minus")
            {
                this.ddlTopMargin.SelectedIndex = this.ddlTopMargin.SelectedIndex - 1;
            }
            else if (((Button)sender).CommandArgument == "Plus")
            {
                this.ddlTopMargin.SelectedIndex = this.ddlTopMargin.SelectedIndex + 1;
            }
            ddlTopMargin_SelectedIndexChanged(null, null);
        }

        protected void ddlTopMargin_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.btnTopMarginMinus.Enabled = this.ddlTopMargin.SelectedIndex != 0;
            this.btnTopMarginPlus.Enabled = this.ddlTopMargin.SelectedIndex < this.ddlTopMargin.Items.Count - 1;
            btnGenerate_Click(null, null);
        }
    }
}