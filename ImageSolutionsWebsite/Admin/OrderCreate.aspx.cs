using ImageSolutionsWebsite.Library;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Web;
using System.Web.Management;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ImageSolutionsWebsite.Admin
{
    public partial class OrderCreate : BasePage
    {
        public class SalesOrderLine
        {
            public int Quantity { get; set; }
            public string Gender { get; set; }
            public string Size { get; set; }
            public string Memo { get; set; }
            public string Reference { get; set; }
            public string ShippingAttention { get; set; }
            public string ShippingAddressee { get; set; }
            public string ShippingAddressLine1 { get; set; }
            public string ShippingAddressLine2 { get; set; }
            public string ShippingCity { get; set; }
            public string ShippingState { get; set; }
            public string ShippingZip { get; set; }
            public string SalesOrder { get; set; }
            public string OrderNumber { get; set; }
            public string Name { get; set; }
            public string InternalID { get; set; }
            public string ExternalID { get; set; }
            public string ItemCode { get; set; }
            public string Location { get; set; }
            public string ShippingCarrier { get; set; }
            public string ShipVia { get; set; }
            public string NetSuiteItem
            {
                get
                {
                    string strReturn = string.Empty;

                    if (Gender == "F")
                    {
                        strReturn = "200 : AL249-WH-";
                    }
                    else if (Gender == "M")
                    {
                        strReturn = "200 : AL248-WH-";
                    }

                    switch (Size)
                    {
                        case "Small":
                            strReturn += "S";
                            break;
                        case "Medium":
                            strReturn += "M";
                            break;
                        case "Large":
                            strReturn += "L";
                            break;
                        case "X-Large":
                            strReturn += "XL";
                            break;
                        default:
                            strReturn += "N/A";
                            break;
                    }
                    return strReturn;
                }
            }

            public string Address
            {
                get
                {
                    return (string.IsNullOrEmpty(ShippingAttention) ? string.Empty : ShippingAddressee + Environment.NewLine)
                           + (string.IsNullOrEmpty(ShippingAddressee) ? string.Empty : ShippingAddressee + Environment.NewLine)
                           + (string.IsNullOrEmpty(ShippingAddressLine1) ? string.Empty : ShippingAddressLine1 + Environment.NewLine)
                           + (string.IsNullOrEmpty(ShippingAddressLine2) ? string.Empty : ShippingAddressLine2 + Environment.NewLine)
                           + (string.IsNullOrEmpty(ShippingCity) ? string.Empty : ShippingCity + ", ")
                           + (string.IsNullOrEmpty(ShippingState) ? string.Empty : ShippingState + " ")
                           + (string.IsNullOrEmpty(ShippingZip) ? string.Empty : ShippingZip + Environment.NewLine);
                }
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                Initialize();
            }
        }

        protected void Initialize()
        {
            this.ddlWebsiteShippingServices.DataSource = CurrentWebsite.WebsiteShippingServices;
            this.ddlWebsiteShippingServices.DataBind();
            this.ddlWebsiteShippingServices.Items.Insert(0, new ListItem());
        }

        //protected void lbnDownload_Click(object sender, EventArgs e)
        //{
        //    List<SalesOrderLine> objSalesOrderLines = null;
        //    string strFilePath = string.Empty;

        //    try
        //    {
        //        strFilePath = "C:\\Users\\livin\\OneDrive\\GitHub\\ImageSolutions\\ImageSolutions\\ImageSolutionsWebsite\\Admin\\OrderFiles\\NetSuiteCSV.csv";

        //        //UploadFile();
        //        objSalesOrderLines = GetSalesOrderLines();
        //        GenerateCSV(objSalesOrderLines);

        //        System.Web.HttpResponse response = System.Web.HttpContext.Current.Response;
        //        response.ClearContent();
        //        response.Clear();
        //        response.ContentType = "text/plain";
        //        response.AddHeader("Content-Disposition", "attachment; filename=" + "NetSuiteCSV.csv" + ";");
        //        response.TransmitFile(strFilePath);
        //        response.Flush();
        //        response.End();
        //    }
        //    catch (Exception ex)
        //    {
        //        WebUtility.DisplayJavascriptMessage(this, ex.Message);
        //    }
        //    finally
        //    {
        //        objSalesOrderLines = null;
        //    }
        //}

        private bool GenerateCSV(List<SalesOrderLine> objSalesOrderLines)
        {
            StringBuilder objCSV = null;
            string strGUID = Guid.NewGuid().ToString();

            try
            {
                objCSV = new StringBuilder();

                if (objSalesOrderLines != null && objSalesOrderLines.Count > 0)
                {
                    objCSV.Append("BatchNumber,CustomerInternalID,CustomerName,LocationInternalID,LocationName,ItemName,Quantity,Memo,Reference,ShippingAttention,ShippingAddressee,ShippingAddressLine1,ShippingAddressLine2,City,State,Zip");
                    objCSV.AppendLine();

                    foreach (SalesOrderLine objSalesOrderLine in objSalesOrderLines)
                    {
                        objCSV.Append(strGUID).Append(",");
                        objCSV.Append(objSalesOrderLine.InternalID).Append(",");
                        objCSV.Append(objSalesOrderLine.Name).Append(",");
                        objCSV.Append("3").Append(",");
                        objCSV.Append(objSalesOrderLine.Location).Append(",");
                        objCSV.Append(objSalesOrderLine.NetSuiteItem).Append(",");
                        objCSV.Append(objSalesOrderLine.Quantity).Append(",");
                        objCSV.Append(objSalesOrderLine.Memo).Append(",");
                        objCSV.Append(objSalesOrderLine.Reference).Append(",");
                        objCSV.Append(objSalesOrderLine.ShippingAttention).Append(",");
                        objCSV.Append(objSalesOrderLine.ShippingAddressee).Append(",");
                        objCSV.Append(objSalesOrderLine.ShippingAddressLine1).Append(",");
                        objCSV.Append(objSalesOrderLine.ShippingAddressLine2).Append(",");
                        objCSV.Append(objSalesOrderLine.ShippingCity).Append(",");
                        objCSV.Append(objSalesOrderLine.ShippingState).Append(",");
                        objCSV.Append(objSalesOrderLine.ShippingZip).Append(",");
                        objCSV.AppendLine();
                    }
                }

                string strFilePath = "C:\\GitHub\\ImageSolutions\\ImageSolutions\\ImageSolutionsWebsite\\Admin\\OrderFiles\\NetSuiteCSV.csv";
                if (objCSV != null)
                {
                    using (StreamWriter objStreamWriter = new StreamWriter(strFilePath))
                    {
                        objStreamWriter.Write(objCSV.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                objCSV = null;
            }
            return true;
        }

        private void UploadFile()
        {
            HttpPostedFile postedFile = null;

            try
            {
                postedFile = fuOrderFile.PostedFile;
                string contentType = postedFile.ContentType;
                int contentLength = postedFile.ContentLength;
                postedFile.SaveAs(Server.MapPath("~/Admin/OrderFiles/") + Path.GetFileName(postedFile.FileName));
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                postedFile = null;
            }
        }

        protected void lbnUpload_Click(object sender, EventArgs e)
        {
            List<SalesOrderLine> objSalesOrderLines = null;

            try
            {
                litPostedFileName.Text = Path.GetFileName(fuOrderFile.PostedFile.FileName);
                UploadFile();
                objSalesOrderLines = GetSalesOrderLines();
                BindSalesOrderLines(objSalesOrderLines);
            }
            catch (Exception ex)
            {
                WebUtility.DisplayJavascriptMessage(this, ex.Message);
            }
            finally
            {
                objSalesOrderLines = null;
            }
        }

        protected void BindSalesOrderLines(List<SalesOrderLine> objSalesOrderLines)
        {
            try
            {
                this.gvSalesOrderLines.DataSource = objSalesOrderLines;
                this.gvSalesOrderLines.DataBind();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally { }
        }

        protected List<SalesOrderLine> GetSalesOrderLines()
        {
            List<SalesOrderLine> objReturn = null;
            SalesOrderLine objSalesOrderLine = null;

            try
            {
                objReturn = new List<SalesOrderLine>();

                string csvData = System.IO.File.ReadAllText(Server.MapPath("~/Admin/OrderFiles/" + litPostedFileName.Text));
                //string csvData = System.IO.File.ReadAllText("C:\\Users\\livin\\Desktop\\samples.csv");

                foreach (string row in csvData.Split('\n'))
                {
                    if (!string.IsNullOrEmpty(row))
                    {
                        string[] strRow = row.Split(',');
                        if (!string.IsNullOrEmpty(strRow[0]))
                        {
                            int intQuantity = 0;

                            if (Int32.TryParse(strRow[0], out intQuantity))
                            {
                                if (intQuantity != 0)
                                {
                                    objSalesOrderLine = new SalesOrderLine();
                                    objSalesOrderLine.Quantity = intQuantity;
                                    objSalesOrderLine.Gender = strRow[1];
                                    objSalesOrderLine.Size = strRow[2].Replace(" ", string.Empty);
                                    objSalesOrderLine.Memo = strRow[3];
                                    objSalesOrderLine.Reference = strRow[4];
                                    objSalesOrderLine.ShippingAttention = strRow[5];
                                    objSalesOrderLine.ShippingAddressee = strRow[6];
                                    objSalesOrderLine.ShippingAddressLine1 = strRow[7];
                                    objSalesOrderLine.ShippingAddressLine2 = strRow[8];
                                    objSalesOrderLine.ShippingCity = strRow[9];
                                    objSalesOrderLine.ShippingState = strRow[10];
                                    objSalesOrderLine.ShippingZip = strRow[11];
                                    objSalesOrderLine.SalesOrder = strRow[12];
                                    objSalesOrderLine.OrderNumber = strRow[13];
                                    objSalesOrderLine.Name = strRow[14];
                                    objSalesOrderLine.InternalID = strRow[15];
                                    objSalesOrderLine.ExternalID = strRow[16];
                                    objSalesOrderLine.ItemCode = strRow[17];
                                    objSalesOrderLine.Location = strRow[18];
                                    objSalesOrderLine.ShippingCarrier = strRow[19];
                                    objSalesOrderLine.ShipVia = strRow[20];
                                    objReturn.Add(objSalesOrderLine);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally { }

            return objReturn;
        }

        protected void lbnCreateOrder_Click(object sender, EventArgs e)
        {
            SqlConnection objConn = null;
            SqlTransaction objTran = null;
            List<SalesOrderLine> objSources = null;
            ImageSolutions.Address.AddressBook objShippingAddressBook = null;
            ImageSolutions.Address.AddressBook objBillingAddressBook = null;
            ImageSolutions.Address.AddressTrans objShippingAddressTrans = null;
            ImageSolutions.SalesOrder.SalesOrder objSalesOrder = null;
            ImageSolutions.Item.Item objItem = null;
            ImageSolutions.Item.ItemFilter objItemFilter = null;
            //ImageSolutions.User.UserInfo objUserInfo = null;
            //ImageSolutions.User.UserWebsite objUserWebsite = null;

            try
            {
                objSources = GetSalesOrderLines();
                if (objSources == null || objSources.Count == 0) throw new Exception("Please upload a file");

                objConn = new SqlConnection(Database.DefaultConnectionString);
                objConn.Open();
                objTran = objConn.BeginTransaction();

                foreach (SalesOrderLine objSource in objSources)
                {
                    //objUserInfo = new ImageSolutions.User.UserInfo();
                    //objUserInfo.CustomerInternalID = objSource.InternalID;
                    //objUserInfo.FirstName = objSource.Name;
                    ////objUserInfo.EmailAddress = CurrentUser.EmailAddress;
                    //objUserInfo.Password = "ImageSolutions$1";
                    //objUserInfo.IsGuest = true;
                    //objUserInfo.Create(objConn, objTran);

                    //objUserWebsite = new ImageSolutions.User.UserWebsite();
                    //objUserWebsite.UserInfoID = objUserInfo.UserInfoID;
                    //objUserWebsite.WebsiteID = CurrentWebsite.WebsiteID;
                    //objUserWebsite.CreatedBy = CurrentUser.UserInfoID;
                    //objUserWebsite.Create(objConn, objTran);

                    objSalesOrder = new ImageSolutions.SalesOrder.SalesOrder();
                    objShippingAddressTrans = new ImageSolutions.Address.AddressTrans();
                    objShippingAddressTrans.AddressLine1 = objSource.ShippingAddressLine1;
                    objShippingAddressTrans.AddressLine2 = objSource.ShippingAddressLine2;
                    objShippingAddressTrans.FirstName = objSource.ShippingAttention;
                    objShippingAddressTrans.LastName = objSource.ShippingAddressee;
                    objShippingAddressTrans.City = objSource.ShippingCity;
                    objShippingAddressTrans.State = objSource.ShippingState;
                    objShippingAddressTrans.PostalCode = objSource.ShippingZip;
                    objShippingAddressTrans.Email = CurrentUser.EmailAddress;
                    objShippingAddressTrans.Create(objConn, objTran);
                    objSalesOrder.DeliveryAddressTransID = objShippingAddressTrans.AddressTransID;
                    objSalesOrder.BillingAddressTransID = objShippingAddressTrans.AddressTransID;

                    objSalesOrder.ShippingAmount = 0;
                    objSalesOrder.TaxAmount = 0;
                    objSalesOrder.DiscountAmount = 0;
                    objSalesOrder.WebsiteShippingServiceID = ddlWebsiteShippingServices.SelectedValue;

                    objSalesOrder.IsPendingApproval = false;
                    objSalesOrder.InActive = true;
                    objSalesOrder.TransactionDate = DateTime.Now;
                    objSalesOrder.UserInfoID = CurrentUser.UserInfoID;
                    objSalesOrder.UserWebsiteID = CurrentUser.CurrentUserWebSite.UserWebsiteID;
                    objSalesOrder.WebsiteID = CurrentUser.CurrentUserWebSite.WebsiteID;
                    objSalesOrder.AccountID = CurrentUser.CurrentUserWebSite.CurrentUserAccount.AccountID;
                    objSalesOrder.IsTaxExempt = CurrentUser.CurrentUserWebSite.IsTaxExempt;

                    objSalesOrder.SalesOrderLines = new List<ImageSolutions.SalesOrder.SalesOrderLine>();
                    ImageSolutions.SalesOrder.SalesOrderLine objSalesOrderLine = new ImageSolutions.SalesOrder.SalesOrderLine();

                    objItemFilter = new ImageSolutions.Item.ItemFilter();
                    objItemFilter.ItemNumber = new Database.Filter.StringSearch.SearchFilter();
                    objItemFilter.ItemNumber.SearchString = objSource.NetSuiteItem.Replace("200 : ", string.Empty);
                    objItem = ImageSolutions.Item.Item.GetItem(objItemFilter);
                    if (objItem == null) throw new Exception("NetSuite Item: " + objSource.NetSuiteItem + " is not found");

                    objSalesOrderLine.ItemID = objItem.ItemID;
                    objSalesOrderLine.Quantity = objSource.Quantity;
                    objSalesOrderLine.UnitPrice = objItem.BasePrice.Value;
                    objSalesOrder.SalesOrderLines.Add(objSalesOrderLine);

                    objSalesOrder.Create(objConn, objTran);
                }

                objTran.Commit();


                if (chkReceivesConfirmation.Checked)
                {
                    SendEmail(CurrentUser.EmailAddress, "Order Batch Created Successfully", "Hi " + CurrentUser.DisplayName + ",<br/<br/>Your order batch has been created successfully, total orders created = " + objSources.Count.ToString());
                }

                WebUtility.DisplayJavascriptMessage(this, "Orders are submitted, you should be receiving the email confirmations once created in NetSuite");
            }
            catch (Exception ex)
            {
                if (objTran != null && objTran.Connection != null)
                {
                    objTran.Rollback();
                }
                WebUtility.DisplayJavascriptMessage(this, ex.Message);
            }
            finally
            {
                objSources = null;
                if (objTran != null) objTran.Dispose();
                objTran = null;
                if (objConn != null) objConn.Dispose();
                objConn = null;
            }
        }
    }
}