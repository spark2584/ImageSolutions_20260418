using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActiveWearIntegration
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                //Task.SyncItems.SyncByExternalID();
                //Task.GetInventory.SyncByStyleNumber();
                //Task.CreateOrder.Execute();
                //Task.SyncInvoice.Start();

                Task.SyncInvoice.ExecuteByInvoiceDate();
                Task.SyncInvoice.ExecuteByPONumber();
                Task.SyncFulfillment.ExecuteByShipDate();
                Task.SyncFulfillment.ExecuteByPONumber();

                if (args.Length > 0)
                {
                    foreach (string arg in args)
                    {
                        try
                        {
                            switch (arg)
                            {
                                case "GetInventory":
                                    Console.WriteLine("Starting: GetInventory");
                                    Task.GetInventory.Start();
                                    break;
                            }
                        }
                        catch { }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(string.Format("Task fail: {1} - ", args[0].ToString(), ex.Message));
            }
        }
    }
}
