using ImageSolutions.Sprouts;
using Renci.SshNet;
using Renci.SshNet.Sftp;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduledTask.Task.Sprouts
{
    public class DownloadCustomer
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

                using (SftpClient _SftpClient = new SftpClient(Convert.ToString(ConfigurationManager.AppSettings["SproutsCustomerSFTPHost"])
                    , Convert.ToString(ConfigurationManager.AppSettings["SproutsCustomerSFTPLogin"])
                    , Convert.ToString(ConfigurationManager.AppSettings["SproutsCustomerSFTPPassword"])
                    )
                )
                {
                    _SftpClient.Connect();
                    SftpFiles = _SftpClient.ListDirectory(Convert.ToString(ConfigurationManager.AppSettings["SproutsCustomerSFTPath"]));
                    foreach (SftpFile _SftpFile in SftpFiles)
                    {
                        if (_SftpFile.FullName.Contains(".csv"))
                        {
                            SproutsCustomerImport SproutsCustomerImport = new SproutsCustomerImport();

                            SproutsCustomerImportFilter SproutsCustomerImportFilter = new SproutsCustomerImportFilter();
                            SproutsCustomerImportFilter.FileName = new Database.Filter.StringSearch.SearchFilter();
                            SproutsCustomerImportFilter.FileName.SearchString = _SftpFile.Name;
                            SproutsCustomerImport = SproutsCustomerImport.GetSproutsCustomerImport(SproutsCustomerImportFilter);

                            if (SproutsCustomerImport == null)
                            {
                                using (Stream _Stream = File.OpenWrite(string.Format(@"{0}\{1}", strLocalFolder, _SftpFile.Name)))
                                {
                                    _SftpClient.DownloadFile(_SftpFile.FullName, _Stream);
                                    //Store in Database
                                    SproutsCustomerImport NewSproutsCustomerImport = new SproutsCustomerImport();
                                    NewSproutsCustomerImport.FilePath = string.Format(@"{0}\{1}", DateTime.UtcNow.ToString("yyyyMM"), _SftpFile.Name);
                                    NewSproutsCustomerImport.FileName = _SftpFile.Name;
                                    NewSproutsCustomerImport.Create();

                                    //Delete file in sFTP - need to test
                                    if (!string.IsNullOrEmpty(NewSproutsCustomerImport.SproutsCustomerImportID))
                                    {
                                        _SftpClient.DeleteFile(string.Format(@"{0}/{1}", Convert.ToString(ConfigurationManager.AppSettings["SproutsCustomerSFTPath"]), _SftpFile.Name));
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch
            {

            }

            return true;
        }
    }
}
