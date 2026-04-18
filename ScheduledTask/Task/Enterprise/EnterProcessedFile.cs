using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduledTask.Task.Enterprise
{
    public class EnterProcessedFile
    {
        public bool Execute()
        {
            SqlConnection objConn = null;
            SqlTransaction objTran = null;

            SqlDataReader objRead = null;
            string strSQL = string.Empty;

            try
            {
                string strConnectionStrig = "Data Source=image-solutions.cwfbgfgwdwwa.us-west-1.rds.amazonaws.com,1433;Initial Catalog=ImageSolutions;Persist Security Info=True;User ID=admin;Password=Password$1";

                objConn = new SqlConnection(strConnectionStrig);
                objConn.Open();

                strSQL = string.Format(@"
SELECT FilePath, FileName, IsStore , *
FROM CustomerImport c
WHERE c.IsProcessed = 1
and c.CreatedOn >= '4/9/2024'
ORDER BY c.CustomerImportID
");
                objRead = Database.GetDataReader(strSQL, objConn, objTran);               

                while (objRead.Read())
                {
                    ImageSolutions.Enterprise.EnterpriseCustomerImport EnterpriseCustomerImport = new ImageSolutions.Enterprise.EnterpriseCustomerImport();
                    ImageSolutions.Enterprise.EnterpriseCustomerImportFilter EnterpriseCustomerImportFilter = new ImageSolutions.Enterprise.EnterpriseCustomerImportFilter();
                    EnterpriseCustomerImportFilter.FileName = new Database.Filter.StringSearch.SearchFilter();
                    EnterpriseCustomerImportFilter.FileName.SearchString = Convert.ToString(objRead["FileName"]);
                    EnterpriseCustomerImport = ImageSolutions.Enterprise.EnterpriseCustomerImport.GetEnterpriseCustomerImport(EnterpriseCustomerImportFilter);

                    if(EnterpriseCustomerImport == null || string.IsNullOrEmpty(EnterpriseCustomerImport.EnterpriseCustomerImportID))
                    {
                        string strFilePath = Convert.ToString(objRead["FilePath"]);

                        string[] FilePathSplit = strFilePath.Split('\\');

                        string strNewFilePath = string.Empty;
                        int counter = 0;
                        foreach (string _split in FilePathSplit)
                        {
                            counter++;

                            if(counter == FilePathSplit.Length)
                            {
                                strNewFilePath = string.Format("{0}\\Archive\\{1}", strNewFilePath, _split);
                            }
                            else
                            {
                                strNewFilePath = string.Format("{0}\\{1}", strNewFilePath, _split);
                            }
                        }


                        EnterpriseCustomerImport = new ImageSolutions.Enterprise.EnterpriseCustomerImport();
                        EnterpriseCustomerImport.FilePath = strNewFilePath;
                        EnterpriseCustomerImport.FileName = Convert.ToString(objRead["FileName"]);
                        EnterpriseCustomerImport.IsStore = Convert.ToBoolean(objRead["IsStore"]);
                        EnterpriseCustomerImport.Create();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.Write(string.Format("{0}", ex.Message));
            }
            return true;
        }
    }
}
