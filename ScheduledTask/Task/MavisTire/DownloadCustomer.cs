using ImageSolutions.MavisTire;
using Renci.SshNet;
using Renci.SshNet.Sftp;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduledTask.Task.MavisTire
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

                using (SftpClient _SftpClient = new SftpClient(Convert.ToString(ConfigurationManager.AppSettings["MavisTireCustomerSFTPHost"])
                    , Convert.ToString(ConfigurationManager.AppSettings["MavisTireCustomerSFTPLogin"])
                    , Convert.ToString(ConfigurationManager.AppSettings["MavisTireCustomerSFTPPassword"])
                    )
                )
                {
                    _SftpClient.Connect();
                    SftpFiles = _SftpClient.ListDirectory(Convert.ToString(ConfigurationManager.AppSettings["MavisTireCustomerSFTPath"]));
                    foreach (SftpFile _SftpFile in SftpFiles)
                    {
                        if (_SftpFile.FullName.Contains(".gpg"))
                        {
                            MavisTireCustomerImport MavisTireCustomerImport = new MavisTireCustomerImport();

                            MavisTireCustomerImportFilter MavisTireCustomerImportFilter = new MavisTireCustomerImportFilter();
                            MavisTireCustomerImportFilter.FileName = new Database.Filter.StringSearch.SearchFilter();
                            MavisTireCustomerImportFilter.FileName.SearchString = _SftpFile.Name;
                            MavisTireCustomerImport = MavisTireCustomerImport.GetMavisTireCustomerImport(MavisTireCustomerImportFilter);

                            if (MavisTireCustomerImport == null)
                            {
                                using (Stream _Stream = File.OpenWrite(string.Format(@"{0}\{1}", strLocalFolder, _SftpFile.Name)))
                                {
                                    _SftpClient.DownloadFile(_SftpFile.FullName, _Stream);
                                    //Store in Database
                                    MavisTireCustomerImport NewMavisTireCustomerImport = new MavisTireCustomerImport();
                                    NewMavisTireCustomerImport.FilePath = string.Format(@"{0}\{1}", DateTime.UtcNow.ToString("yyyyMM"), _SftpFile.Name);
                                    NewMavisTireCustomerImport.FileName = _SftpFile.Name;
                                    NewMavisTireCustomerImport.IsEncrypted = true;
                                    NewMavisTireCustomerImport.Create();

                                    //Delete file in sFTP - need to test
                                    if (!string.IsNullOrEmpty(NewMavisTireCustomerImport.MavisTireCustomerImportID))
                                    {
                                        _SftpClient.DeleteFile(string.Format(@"{0}/{1}", Convert.ToString(ConfigurationManager.AppSettings["MavisTireCustomerSFTPath"]), _SftpFile.Name));
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
