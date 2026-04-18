using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using NetSuiteLibrary.com.netsuite.webservices;

namespace ScheduledTask
{
    public class Program
    {
        static void Main(string[] args)
        {
            //Task.MavisTire.SendGeneralPackageNotification a = new ScheduledTask.Task.MavisTire.SendGeneralPackageNotification();
            //a.Execute();

            //Task.MavisTire.SyncCustomer a = new ScheduledTask.Task.MavisTire.SyncCustomer();
            //a.Execute();

            //Task.MavisTire.SendAnnualOrderNotification a = new ScheduledTask.Task.MavisTire.SendAnnualOrderNotification();
            //a.Execute();

            Task.Enterprise.GenerateTaxPayEntry a = new ScheduledTask.Task.Enterprise.GenerateTaxPayEntry();
            a.Execute();

            Task.Enterprise.GenerateCreditPayroll b = new ScheduledTask.Task.Enterprise.GenerateCreditPayroll();
            b.Execute();

            //Task.Enterprise.UpdateCorporateCustomer b = new ScheduledTask.Task.Enterprise.UpdateCorporateCustomer();
            //b.Execute();

            //Task.Enterprise.SyncCorporateCustomer b = new ScheduledTask.Task.Enterprise.SyncCorporateCustomer();
            //b.Execute();

            //Task.Enterprise.SyncPreEmployeeCustomer b = new ScheduledTask.Task.Enterprise.SyncPreEmployeeCustomer();
            //b.Execute();

            //Task.MavisTire.DownloadCustomer a = new ScheduledTask.Task.MavisTire.DownloadCustomer();
            //a.Execute();

            //Task.MavisTire.DecryptFile a = new ScheduledTask.Task.MavisTire.DecryptFile();
            //a.Execute();

            //Task.MavisTire.UpdateCustomer a = new ScheduledTask.Task.MavisTire.UpdateCustomer();
            //a.Execute();

            //Task.MavisTire.SyncCustomer a = new ScheduledTask.Task.MavisTire.SyncCustomer();
            //a.Execute();

            //Task.Avalara Avalara = new ScheduledTask.Task.Avalara();
            //Avalara.Test();

            //Task.Enterprise.SyncLicenseeCustomer a = new ScheduledTask.Task.Enterprise.SyncLicenseeCustomer();
            //a.ManualCreate();

            //Task.SendDTBudgetReport a = new ScheduledTask.Task.SendDTBudgetReport();
            //a.Execute();

            //ImageSolutions.User.UserWebsite UserWebsite = new ImageSolutions.User.UserWebsite("5386");
            //Console.WriteLine(UserWebsite.ActiveBudgetExpirationDate);

            //Task.Sprouts.DownloadCustomer a = new ScheduledTask.Task.Sprouts.DownloadCustomer();
            //a.Execute();

            //Task.Sprouts.UpdateCustomer a = new ScheduledTask.Task.Sprouts.UpdateCustomer();
            //a.Execute();

            //Task.Sprouts.SyncCustomer a = new ScheduledTask.Task.Sprouts.SyncCustomer();
            //a.Execute();

            //Task.DiscountTire.ProcessDiscountTireEmployeeFile a = new ScheduledTask.Task.DiscountTire.ProcessDiscountTireEmployeeFile();
            //a.TestProcessEmployee("61617", "Discount Tire");

            //Task.Enterprise.SyncLicenseeCustomer a = new Task.Enterprise.SyncLicenseeCustomer();
            //a.Execute();


            if (args.Length > 0)
            {
                foreach (string arg in args)
                {
                    try
                    {
                        switch (arg)
                        {
                            case "EnterpriseEBAInvoiceSync":
                                //Run this at 6pm PST daily before the invoice script to generate the submission file
                                Task.Enterprise.SyncInvoice objSyncInvoice = new ScheduledTask.Task.Enterprise.SyncInvoice();
                                objSyncInvoice.GetEnterpriseEBAInvoices(Convert.ToDateTime("8/6/2024"));
                                break;
                            //copy data from stating to production, this is deactivated/not running
                            case "UpdateSalesOrderStatus":
                                Task.UpdateSalesOrderStatus UpdateSalesOrderStatus = new ScheduledTask.Task.UpdateSalesOrderStatus();
                                UpdateSalesOrderStatus.Execute();
                                break;
                            //Download Discount Tire Employee file from NetSuite
                            case "DownloadDiscountTireEmployeeFile":
                                Task.DiscountTire.DownloadDiscountTireEmployeeFile DownloadDiscountTireEmployeeFile = new ScheduledTask.Task.DiscountTire.DownloadDiscountTireEmployeeFile();
                                DownloadDiscountTireEmployeeFile.Execute();
                                break;
                            //create/delete users, update budgets, every year it's going to create budget in dynamic website
                            case "ProcessDiscountTireEmployeeFile":
                                Task.DiscountTire.ProcessDiscountTireEmployeeFile ProcessDiscountTireEmployeeFile = new ScheduledTask.Task.DiscountTire.ProcessDiscountTireEmployeeFile();
                                ProcessDiscountTireEmployeeFile.Execute();
                                break;
                            //This is NetSuite refunds gets synced into database (Brandon has the script), this scripts calls Stripe to process the refunds, brandon has another script to push the transactionID to NetSuite.
                            case "ProcessRefund":
                                Task.ProcessRefund ProcessRefund = new Task.ProcessRefund();
                                ProcessRefund.Execute();
                                break;
                            //Fetch the summary from Stipe to database, brandon has a script to create payouts in NetSuite
                            case "SyncPayout":
                                Task.SyncPayout SyncPayout = new Task.SyncPayout();
                                SyncPayout.Execute();
                                break;
                            //Accounting creates email from invoices or customer with payment link, it goes to portal.imageinc.com, select invoices they wish to pay,
                            //Redirect to Stripe to make payment, this process checks to see if the payment is made, if yes, Brandon creates customer payment in NetSuite and check the paid invoices.
                            case "ConfirmPaymentLink":
                                Task.StripePaymentLink.ConfirmPaymentLink ConfirmPaymentLink = new ScheduledTask.Task.StripePaymentLink.ConfirmPaymentLink();
                                ConfirmPaymentLink.Execute();
                                break;
                            //Expires the payment link to Stripe
                            case "DeactivatePaymentLink":
                                Task.StripePaymentLink.DeactivatePaymentLink DeactivatePaymentLink = new ScheduledTask.Task.StripePaymentLink.DeactivatePaymentLink();
                                DeactivatePaymentLink.Execute();
                                break;

                            //Expires the payment link to Stripe
                            case "SendPendingApprovalEmail":
                                Task.SendPendingApprovalEmail SendPendingApprovalEmail = new ScheduledTask.Task.SendPendingApprovalEmail();
                                SendPendingApprovalEmail.Execute();
                                break;


                            //Enterprise
                            case "DownloadLicenseeCustomer":
                                Console.WriteLine("DownloadLicenseeCustomer");
                                Task.Enterprise.DownloadLicenseeCustomer DownloadLicenseeCustomer = new Task.Enterprise.DownloadLicenseeCustomer();
                                DownloadLicenseeCustomer.Execute();
                                break;
                            case "UpdateLicenseeCustomer":
                                Console.WriteLine("UpdateLicenseeCustomer");
                                Task.Enterprise.UpdateLicenseeCustomer UpdateLicenseeCustomer = new Task.Enterprise.UpdateLicenseeCustomer();
                                UpdateLicenseeCustomer.Execute();
                                break;
                            case "SyncLicenseeCustomer":
                                Console.WriteLine("SyncLicenseeCustomer");
                                Task.Enterprise.SyncLicenseeCustomer SyncLicenseeCustomer = new Task.Enterprise.SyncLicenseeCustomer();
                                SyncLicenseeCustomer.Execute();
                                break;

                            case "DownloadCorporateCustomer":
                                Console.WriteLine("DownloadCorporateCustomer");
                                Task.Enterprise.DownloadCorporateCustomer DownloadCorporateCustomer = new Task.Enterprise.DownloadCorporateCustomer();
                                DownloadCorporateCustomer.Execute();
                                break;
                            case "UpdateCorporateCustomer":
                                Console.WriteLine("UpdateCorporateCustomer");
                                Task.Enterprise.UpdateCorporateCustomer UpdateCorporateCustomer = new Task.Enterprise.UpdateCorporateCustomer();
                                UpdateCorporateCustomer.Execute();
                                break;
                            case "SyncCorporateCustomer":
                                Console.WriteLine("SyncCorporateCustomer");
                                Task.Enterprise.SyncCorporateCustomer SyncCorporateCustomer = new Task.Enterprise.SyncCorporateCustomer();
                                SyncCorporateCustomer.Execute();
                                break;

                            case "DownloadPreEmployeeCustomer":
                                Console.WriteLine("DownloadPreEmployeeCustomer");
                                Task.Enterprise.DownloadPreEmployeeCustomer DownloadPreEmployeeCustomer = new Task.Enterprise.DownloadPreEmployeeCustomer();
                                DownloadPreEmployeeCustomer.Execute();
                                break;
                            case "UpdatePreEmloyeeCustomer":
                                Console.WriteLine("UpdatePreEmloyeeCustomer");
                                Task.Enterprise.UpdatePreEmloyeeCustomer UpdatePreEmloyeeCustomer = new Task.Enterprise.UpdatePreEmloyeeCustomer();
                                UpdatePreEmloyeeCustomer.Execute();
                                break;
                            case "SyncPreEmployeeCustomer":
                                Console.WriteLine("SyncPreEmployeeCustomer");
                                Task.Enterprise.SyncPreEmployeeCustomer SyncPreEmployeeCustomer = new Task.Enterprise.SyncPreEmployeeCustomer();
                                SyncPreEmployeeCustomer.Execute();
                                break;

                            case "RefreshAnniversaryBudget":
                                Console.WriteLine("RefreshAnniversaryBudget");
                                Task.Enterprise.RefreshAnniversaryBudget RefreshAnniversaryBudget = new Task.Enterprise.RefreshAnniversaryBudget();
                                RefreshAnniversaryBudget.Execute();
                                break;

                            case "DeactivateInactiveUserWithNoBudget":
                                Console.WriteLine("DeactivateInactiveUserWithNoBudget");
                                Task.Enterprise.DeactivateInactiveUserWithNoBudget DeactivateInactiveUserWithNoBudget = new Task.Enterprise.DeactivateInactiveUserWithNoBudget();
                                DeactivateInactiveUserWithNoBudget.Execute();
                                break;

                            case "DecryptFile":
                                Console.WriteLine("DecryptFile");
                                Task.DecryptFile DecryptFile = new Task.DecryptFile();
                                DecryptFile.Execute();
                                break;

                            case "EnterProcessedFile":
                                Console.WriteLine("EnterProcessedFile");
                                Task.Enterprise.EnterProcessedFile EnterProcessedFile = new ScheduledTask.Task.Enterprise.EnterProcessedFile();
                                EnterProcessedFile.Execute();
                                break;


                            case "EnterpriseSyncInvoice":
                                Console.WriteLine("EnterpriseSyncInvoice");
                                Task.Enterprise.SyncInvoice EnterpriseSyncInvoice = new ScheduledTask.Task.Enterprise.SyncInvoice();
                                EnterpriseSyncInvoice.Execute();
                                break;
                            case "EnterpriseUpdateInvoiceIsExported":
                                Console.WriteLine("EnterpriseUpdateInvoiceIsExported");
                                Task.Enterprise.UpdateInvoiceIsExported EnterpriseUpdateInvoiceIsExported = new ScheduledTask.Task.Enterprise.UpdateInvoiceIsExported();
                                EnterpriseUpdateInvoiceIsExported.Execute();
                                break;
                            case "EnterpriseGenerateInvoiceFile":
                                Console.WriteLine("EnterpriseGenerateInvoiceFile");
                                Task.Enterprise.GenerateInvoiceFile EnterpriseGenerateInvoiceFile = new ScheduledTask.Task.Enterprise.GenerateInvoiceFile();
                                EnterpriseGenerateInvoiceFile.Execute();
                                break;
                            case "EnterpriseGenerateCreditFile":
                                Console.WriteLine("EnterpriseGenerateCreditFile");
                                Task.Enterprise.GenerateCreditFile EnterpriseGenerateCreditFile = new ScheduledTask.Task.Enterprise.GenerateCreditFile();
                                EnterpriseGenerateCreditFile.Execute();
                                break;

                            case "GenerateTaxPayEntry":
                                Console.WriteLine("GenerateTaxPayEntry");
                                Task.Enterprise.GenerateTaxPayEntry GenerateTaxPayEntry = new ScheduledTask.Task.Enterprise.GenerateTaxPayEntry();
                                GenerateTaxPayEntry.Execute();
                                break;
                            case "GenerateCreditPayroll":
                                Console.WriteLine("GenerateCreditPayroll");
                                Task.Enterprise.GenerateCreditPayroll GenerateCreditPayroll = new ScheduledTask.Task.Enterprise.GenerateCreditPayroll();
                                GenerateCreditPayroll.Execute();
                                break;

                            //Brightspeed
                            case "BrightspeedDownloadCustomer":
                                Console.WriteLine("DownloadCustomer");
                                Task.Brightspeed.DownloadCustomer DownloadCustomer = new ScheduledTask.Task.Brightspeed.DownloadCustomer();
                                DownloadCustomer.Execute();
                                break;
                            case "BrightspeedUpdateCustomer":
                                Console.WriteLine("UpdateCustomer");
                                Task.Brightspeed.UpdateCustomer UpdateCustomer = new Task.Brightspeed.UpdateCustomer();
                                UpdateCustomer.Execute();
                                break;
                            case "BrightspeedSyncCustomer":
                                Console.WriteLine("SyncLicenseeCustomer");
                                Task.Brightspeed.SyncCustomer SyncCustomer = new Task.Brightspeed.SyncCustomer();
                                SyncCustomer.Execute();
                                break;


                            //Discount Tire
                            case "SendDTBudgetReport":
                                Console.WriteLine("SendDTBudgetReport");
                                Task.SendDTBudgetReport SendDTBudgetReport = new ScheduledTask.Task.SendDTBudgetReport();
                                SendDTBudgetReport.Execute();
                                break;


                            case "SendBudgetReport":
                                Console.WriteLine("SendBudgetReport");
                                Task.SendBudgetReport SendBudgetReport = new ScheduledTask.Task.SendBudgetReport();
                                SendBudgetReport.Execute();
                                break;

                            //Mavis Tire
                            case "MavisTireDownloadCustomer":
                                Console.WriteLine("MavisTireDownloadCustomer");
                                Task.MavisTire.DownloadCustomer MavisTireDownloadCustomer = new ScheduledTask.Task.MavisTire.DownloadCustomer();
                                MavisTireDownloadCustomer.Execute();
                                break;
                            case "MavisTireDecryptFile":
                                Console.WriteLine("MavisTireDecryptFile");
                                Task.MavisTire.DecryptFile MavisTireDecryptFile = new ScheduledTask.Task.MavisTire.DecryptFile();
                                MavisTireDecryptFile.Execute();
                                break;
                            case "MavisTireUpdateCustomer":
                                Console.WriteLine("MavisTireUpdateCustomer");
                                Task.MavisTire.UpdateCustomer MavisTireUpdateCustomer = new ScheduledTask.Task.MavisTire.UpdateCustomer();
                                MavisTireUpdateCustomer.Execute();
                                break;
                            case "MavisTireSyncCustomer":
                                Console.WriteLine("MavisTireSyncCustomer");
                                Task.MavisTire.SyncCustomer MavisTireSyncCustomer = new ScheduledTask.Task.MavisTire.SyncCustomer();
                                MavisTireSyncCustomer.Execute();
                                break;

                            //Sprouts
                            case "SproutsDownloadCustomer":
                                Console.WriteLine("SproutsDownloadCustomer");
                                Task.Sprouts.DownloadCustomer SproutsDownloadCustomer = new ScheduledTask.Task.Sprouts.DownloadCustomer();
                                SproutsDownloadCustomer.Execute();
                                break;
                            case "SproutsUpdateCustomer":
                                Console.WriteLine("SproutsUpdateCustomer");
                                Task.Sprouts.UpdateCustomer SproutsUpdateCustomer = new ScheduledTask.Task.Sprouts.UpdateCustomer();
                                SproutsUpdateCustomer.Execute();
                                break;
                            case "SproutsSyncCustomer":
                                Console.WriteLine("SproutsSyncCustomer");
                                Task.Sprouts.SyncCustomer SproutsSyncCustomer = new ScheduledTask.Task.Sprouts.SyncCustomer();
                                SproutsSyncCustomer.Execute();
                                break;
     
                            default:
                                break;
                        }
                    }
                    catch { }
                }
            }
        }
    }
}
