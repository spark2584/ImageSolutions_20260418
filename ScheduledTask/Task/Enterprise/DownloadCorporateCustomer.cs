using ImageSolutions.Enterprise;
using Renci.SshNet;
using Renci.SshNet.Sftp;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduledTask.Task.Enterprise
{
    public class DownloadCorporateCustomer
    {
        public bool Execute()
        {
            IEnumerable<ISftpFile> SftpFiles = null;

            try
            {
                string strLocalFolder = (ConfigurationManager.AppSettings["CustomerLocalPath"]) + DateTime.UtcNow.ToString("yyyyMM");

                if (!Directory.Exists(strLocalFolder))
                {
                    Directory.CreateDirectory(strLocalFolder);
                }

                using (SftpClient _SftpClient = new SftpClient(Convert.ToString(ConfigurationManager.AppSettings["CustomerCorporateSFTPHost"])
                    , Convert.ToString(ConfigurationManager.AppSettings["CustomerCorporateSFTPLogin"])
                    , Convert.ToString(ConfigurationManager.AppSettings["CustomerCorporateSFTPPassword"])
                    )
                )
                {
                    _SftpClient.Connect();
                    SftpFiles = _SftpClient.ListDirectory(Convert.ToString(ConfigurationManager.AppSettings["CustomerCorporateSFTPath"]));
                    foreach (SftpFile _SftpFile in SftpFiles)
                    {
                        if (_SftpFile.FullName.Contains(".pgp"))
                        {
                            EnterpriseCustomerImport CustomerImport = new EnterpriseCustomerImport();

                            EnterpriseCustomerImportFilter CustomerImportFilter = new EnterpriseCustomerImportFilter();
                            CustomerImportFilter.FileName = new Database.Filter.StringSearch.SearchFilter();
                            CustomerImportFilter.FileName.SearchString = _SftpFile.Name;
                            CustomerImport = EnterpriseCustomerImport.GetEnterpriseCustomerImport(CustomerImportFilter);

                            if (CustomerImport == null)
                            {
                                using (Stream _Stream = File.OpenWrite(string.Format(@"{0}\{1}", strLocalFolder, _SftpFile.Name)))
                                {
                                    _SftpClient.DownloadFile(_SftpFile.FullName, _Stream);
                                    //Store in Database
                                    EnterpriseCustomerImport NewCustomerImport = new EnterpriseCustomerImport();
                                    NewCustomerImport.FilePath = string.Format(@"{0}\{1}", DateTime.UtcNow.ToString("yyyyMM"), _SftpFile.Name);
                                    NewCustomerImport.FileName = _SftpFile.Name;
                                    NewCustomerImport.IsStore = false;
                                    NewCustomerImport.IsEncrypted = true;
                                    NewCustomerImport.IsPreEmployee = false;
                                    NewCustomerImport.Create();

                                    //Delete file in sFTP - need to test
                                    if (!string.IsNullOrEmpty(NewCustomerImport.EnterpriseCustomerImportID))
                                    {
                                        _SftpClient.DeleteFile(string.Format(@"{0}/{1}", Convert.ToString(ConfigurationManager.AppSettings["CustomerCorporateSFTPath"]), _SftpFile.Name));
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return true;
        }

        public bool TestUpload()
        {
            try
            {
                string strLocalFilePath = string.Format(@"C:\Import\Customer\202212\ehi_location_all20221208_test.xls");
                string strRemoteFilePath = string.Format(@"/Outbound/ehi_location_all20221208_test.xls");


                using (SftpClient _SftpClient = new SftpClient(Convert.ToString(ConfigurationManager.AppSettings["CustomerCorporateSFTPHost"])
                    , Convert.ToString(ConfigurationManager.AppSettings["CustomerCorporateSFTPLogin"])
                    , Convert.ToString(ConfigurationManager.AppSettings["CustomerCorporateSFTPPassword"])
                    )
                )
                {
                    _SftpClient.Connect();

                    using (Stream localFile = File.Create(strLocalFilePath))
                    {
                        _SftpClient.UploadFile(localFile, strRemoteFilePath);
                    }
                }
            }
            catch
            {

            }
            return true;
        }

        public bool TestDelete()
        {
            try
            {
                string strRemoteFilePath = string.Format(@"/Outbound/ehi_location_all20221208.xls");

                using (SftpClient _SftpClient = new SftpClient(Convert.ToString(ConfigurationManager.AppSettings["CustomerCorporateSFTPHost"])
                    , Convert.ToString(ConfigurationManager.AppSettings["CustomerCorporateSFTPLogin"])
                    , Convert.ToString(ConfigurationManager.AppSettings["CustomerCorporateSFTPPassword"])
                    )
                )
                {
                    _SftpClient.Connect();
                    _SftpClient.DeleteFile(strRemoteFilePath);
                    _SftpClient.Disconnect();
                }
            }
            catch
            {

            }
            return true;
        }
    }
}
