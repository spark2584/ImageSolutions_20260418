using NetSuiteLibrary;
using NetSuiteLibrary.com.netsuite.webservices;
using Renci.SshNet;
using Renci.SshNet.Sftp;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace ScheduledTask.Task.DiscountTire
{
    public class DownloadDiscountTireEmployeeFile : NetSuiteBase
    {
        public bool Execute()
        {
            SyncDiscountTireEmployeeFile();

            //IEnumerable<ISftpFile> SftpFiles = null;

            //try
            //{
            //    string strLocalFolder = (ConfigurationManager.AppSettings["CustomerLocalPath"]) + DateTime.UtcNow.ToString("yyyyMM");

            //    if (!Directory.Exists(strLocalFolder))
            //    {
            //        Directory.CreateDirectory(strLocalFolder);
            //    }

            //    using (SftpClient _SftpClient = new SftpClient(Convert.ToString(ConfigurationManager.AppSettings["DiscountTireSFTPHost"])
            //        , Convert.ToString(ConfigurationManager.AppSettings["DiscountTireSFTPLogin"])
            //        , Convert.ToString(ConfigurationManager.AppSettings["DiscountTireSFTPPassword"])
            //        )
            //    )
            //    {
            //        _SftpClient.Connect();
            //        SftpFiles = _SftpClient.ListDirectory(Convert.ToString(ConfigurationManager.AppSettings["DiscountTireSFTPath"]));
            //        foreach (SftpFile _SftpFile in SftpFiles)
            //        {
            //            if (_SftpFile.FullName.Contains(".csv"))
            //            {
            //                ImageSolutions.Customer.CustomerImport CustomerImport = new ImageSolutions.Customer.CustomerImport();

            //                ImageSolutions.Customer.CustomerImportFilter CustomerImportFilter = new ImageSolutions.Customer.CustomerImportFilter();
            //                CustomerImportFilter.FileName = new Database.Filter.StringSearch.SearchFilter();
            //                CustomerImportFilter.FileName.SearchString = _SftpFile.Name;
            //                CustomerImport = ImageSolutions.Customer.CustomerImport.GetCustomerImport(CustomerImportFilter);

            //                if (CustomerImport == null)
            //                {
            //                    using (Stream _Stream = System.IO.File.OpenWrite(string.Format(@"{0}\{1}", strLocalFolder, _SftpFile.Name)))
            //                    {
            //                        _SftpClient.DownloadFile(_SftpFile.FullName, _Stream);
            //                        //Store in Database
            //                        ImageSolutions.Customer.CustomerImport NewCustomerImport = new ImageSolutions.Customer.CustomerImport();
            //                        NewCustomerImport.FilePath = string.Format(@"{0}\{1}", DateTime.UtcNow.ToString("yyyyMM"), _SftpFile.Name);
            //                        NewCustomerImport.FileName = _SftpFile.Name;
            //                        NewCustomerImport.IsStore = false;
            //                        NewCustomerImport.IsEncrypted = false;
            //                        NewCustomerImport.Create();

            //                        //Delete file in sFTP 
            //                        //if (!string.IsNullOrEmpty(NewCustomerImport.CustomerImportID))
            //                        //{
            //                        //    _SftpClient.DeleteFile(string.Format(@"{0}/{1}", Convert.ToString(ConfigurationManager.AppSettings["DiscountTireSFTPath"]), _SftpFile.Name));
            //                        //}
            //                    }
            //                }
            //            }
            //        }
            //    }
            //}
            //catch (Exception ex)
            //{
            //    Console.WriteLine(ex.Message);
            //}

            return true;
        }

        public bool SyncDiscountTireEmployeeFile()
        {
            NetSuiteLibrary.File.File File = null;
            NetSuiteLibrary.File.FileFilter NetSuiteFileFilter = null;
            //List<string> FileURLs = null;
            //List<string> FileInternalIds = null;

            try
            {
                //File = new NetSuiteLibrary.File.File();
                NetSuiteFileFilter = new NetSuiteLibrary.File.FileFilter();

                NetSuiteLibrary.File.FileFilter FileFilter = new NetSuiteLibrary.File.FileFilter();

                DateTime CreatedAfter = GetCreatedAfter() != null ? Convert.ToDateTime(GetCreatedAfter()) : Convert.ToDateTime("1/26/2024");
                FileFilter.CreatedAfter = CreatedAfter;
                FileFilter.Folder = "4345076";

                //FileURLs = NetSuiteLibrary.File.File.GetNetsuiteFileURLs(Service, FileFilter);
                //foreach (string _fileURL in FileURLs)
                //{
                //    Console.WriteLine(@"{0}", _fileURL);

                //    string strLocalFolder = (ConfigurationManager.AppSettings["CustomerLocalPath"]) + "DiscountTire\\" + DateTime.UtcNow.ToString("yyyyMM");
                //    string strFileName = string.Format("{0}\\{1}.csv", strLocalFolder, DateTime.UtcNow.ToString("yyyyMMddHHmmssffff")); //string.Format("{0}.csv", DateTime.UtcNow.ToString("yyyyMMddHHmmssffff")); //
                //    if (!Directory.Exists(strLocalFolder))
                //    {
                //        Directory.CreateDirectory(strLocalFolder);
                //    }

                //    using (WebClient _Client = new WebClient())
                //    {
                //        _Client.DownloadFile(_fileURL, strFileName);
                //    }

                //    string fromPath = Path.Combine(System.Reflection.Assembly.GetExecutingAssembly().Location, strFileName);
                //    string toPath = Path.Combine(strLocalFolder, string.Format("{0}\\{1}.csv", strLocalFolder, DateTime.UtcNow.ToString("yyyyMMddHHmmssffff")));

                //    // Move the file.
                //    System.IO.File.Move(fromPath, toPath);
                //}

                List<NetSuiteLibrary.File.File> Files = NetSuiteLibrary.File.File.GetFileSearch(Service, FileFilter);

                foreach (NetSuiteLibrary.File.File _File in Files)
                {
                    try
                    {
                        //Console.WriteLine(@"{0}", _File.NetsuiteFile.content);
                        byte[] bytes = _File.NetsuiteFile.content;

                        string strFolder = string.Format("DiscountTire\\{0}", DateTime.UtcNow.ToString("yyyyMM"));
                        string strFileName = string.Format("{0}.csv", DateTime.UtcNow.ToString("yyyyMMddHHmmssffff")); //_File.NetsuiteFile.name.Replace("\"", string.Empty).Trim();

                        string strLocalFolder = string.Format("{0}{1}", (ConfigurationManager.AppSettings["CustomerLocalPath"]), strFolder);
                        string strFullPath = string.Format("{0}\\{1}", strLocalFolder, strFileName); //string.Format("{0}.csv", DateTime.UtcNow.ToString("yyyyMMddHHmmssffff")); //
                        if (!Directory.Exists(strLocalFolder))
                        {
                            Directory.CreateDirectory(strLocalFolder);
                        }

                        if (strFileName.Contains(".csv"))
                        {
                            System.IO.File.WriteAllBytes(strFullPath, bytes);

                            ImageSolutions.Customer.CustomerImport CustomerImport = new ImageSolutions.Customer.CustomerImport();

                            ImageSolutions.Customer.CustomerImportFilter CustomerImportFilter = new ImageSolutions.Customer.CustomerImportFilter();
                            CustomerImportFilter.FileName = new Database.Filter.StringSearch.SearchFilter();
                            CustomerImportFilter.FileName.SearchString = _File.NetsuiteFile.name;
                            CustomerImport = ImageSolutions.Customer.CustomerImport.GetCustomerImport(CustomerImportFilter);

                            if (CustomerImport == null)
                            {
                                ImageSolutions.Customer.CustomerImport NewCustomerImport = new ImageSolutions.Customer.CustomerImport();
                                NewCustomerImport.WebsiteID = ConfigurationManager.AppSettings["DiscountTireWebsiteID"];
                                NewCustomerImport.FileDate = Convert.ToDateTime(_File.NetsuiteFile.createdDate);
                                NewCustomerImport.FilePath = string.Format("{0}\\{1}", strFolder, strFileName);
                                NewCustomerImport.FileName = _File.NetsuiteFile.name;
                                NewCustomerImport.IsStore = false;
                                NewCustomerImport.IsEncrypted = false;
                                NewCustomerImport.Create();
                            }
                        }
                    }
                    catch(Exception ex)
                    {
                        Console.WriteLine("Error: SyncDiscountTireEmployeeFie - Import File");
                    }                    
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {

            }
            return true;
        }

        protected DateTime? GetCreatedAfter()
        {
            DateTime? dtReturn = null;
            SqlDataReader objRead = null;
            string strSQL = string.Empty;

            try
            {
                strSQL = string.Format(@"SELECT TOP 1 FileDate FROM CustomerImport WHERE WebsiteID = {0} ORDER BY FileDate DESC"
                    , Database.HandleQuote(ConfigurationManager.AppSettings["DiscountTireWebsiteID"]));

                objRead = Database.GetDataReader(strSQL, Database.DefaultConnectionString);

                if (objRead.Read())
                {
                    dtReturn = Convert.ToDateTime(objRead["FileDate"]);
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

            return dtReturn;
        }
    }
}
