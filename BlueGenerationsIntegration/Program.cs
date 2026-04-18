using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlueGenerationsIntegration
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Task.TestGetInventory.Start();

                if (args.Length > 0)
                {
                    foreach (string arg in args)
                    {
                        try
                        {
                            switch (arg)
                            {
                                case "TestGetInventory":
                                    Console.WriteLine("Starting: TestGetInventory");
                                    Task.TestGetInventory.Start();
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
