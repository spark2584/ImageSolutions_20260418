using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EnterpriseIntegration
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("UpdateLicenseeCustomer");
            Tasks.UpdateCorporateCustomer a = new Tasks.UpdateCorporateCustomer();
            a.Execute();

            try
            {
                foreach (string arg in args)
                {
                    try
                    {
                        switch (arg)
                        {
                            case "DownloadLicenseeCustomer":
                                Console.WriteLine("DownloadLicenseeCustomer");
                                Tasks.DownloadLicenseeCustomer DownloadLicenseeCustomer = new Tasks.DownloadLicenseeCustomer();
                                DownloadLicenseeCustomer.Execute();
                                break;
                            case "UpdateLicenseeCustomer":
                                Console.WriteLine("UpdateLicenseeCustomer");
                                Tasks.UpdateLicenseeCustomer UpdateLicenseeCustomer = new Tasks.UpdateLicenseeCustomer();
                                UpdateLicenseeCustomer.Execute();
                                break;
                            case "SyncLicenseeCustomer":
                                Console.WriteLine("SyncLicenseeCustomer");
                                Tasks.SyncLicenseeCustomer SyncLicenseeCustomer = new Tasks.SyncLicenseeCustomer();
                                SyncLicenseeCustomer.Execute();
                                break;

                            case "DownloadCorporateCustomer":
                                Console.WriteLine("DownloadCorporateCustomer");
                                Tasks.DownloadCorporateCustomer DownloadCorporateCustomer = new Tasks.DownloadCorporateCustomer();
                                DownloadCorporateCustomer.Execute();
                                break;
                            case "UpdateCorporateCustomer":
                                Console.WriteLine("UpdateLicenseeCustomer");
                                Tasks.UpdateCorporateCustomer UpdateCorporateCustomer = new Tasks.UpdateCorporateCustomer();
                                UpdateCorporateCustomer.Execute();
                                break;
                            case "SyncCorporateCustomer":
                                Console.WriteLine("SyncCorporateCustomer");
                                Tasks.SyncCorporateCustomer SyncCorporateCustomer = new Tasks.SyncCorporateCustomer();
                                SyncCorporateCustomer.Execute();
                                break;

                            case "ImportGroup":
                                Console.WriteLine("ImportGroup");
                                Tasks.ImportGroup ImportGroup = new Tasks.ImportGroup();
                                ImportGroup.Execute();
                                break;
                            case "DecryptFile":
                                Console.WriteLine("DecryptFile");
                                Tasks.DecryptFile DecryptFile = new Tasks.DecryptFile();
                                DecryptFile.Execute();
                                break;
                            case "SyncCustomerEntityGroup":
                                Console.WriteLine("SyncCustomerEntityGroup");
                                Tasks.SyncCustomerEntityGroup SyncCustomerEntityGroup = new Tasks.SyncCustomerEntityGroup();
                                SyncCustomerEntityGroup.Execute();
                                break;
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        Thread.Sleep(5000);
                    }
                }
                
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Thread.Sleep(5000);
            }
        }
    }
}
