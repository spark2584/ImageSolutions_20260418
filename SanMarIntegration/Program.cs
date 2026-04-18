using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SanMarIntegration
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                //Task.TestGetInventory.Start();
                Task.TestInvoicing.Start();

                if (args.Length > 0)
                {
                    foreach (string arg in args)
                    {
                        try
                        {
                            switch (arg)
                            {
                                case "TestGetPricing":
                                    Console.WriteLine("Starting: TestGetPricing");
                                    Task.TestGetPricing.Start();
                                    break;
                            }
                        }
                        catch { }
                    }
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(string.Format("Task fail: {1} - ", args[0].ToString(), ex.Message));
            }
        }
    }
}
