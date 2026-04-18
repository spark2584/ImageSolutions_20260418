using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ImageSolutionsWebsiteConsole
{
    internal class Program
    {
        static void Main(string[] args)
        {
            while (1 == 1)
            {
                //Task.SendFulfillmentEmail a = new Task.SendFulfillmentEmail();
                //a.Execute();

                foreach (string arg in args)
                {
                    try
                    {
                        switch (arg)
                        {
                            case "SendFulfillmentEmail":
                                Console.WriteLine("SendFulfillmentEmail");
                                Task.SendFulfillmentEmail SendFulfillmentEmail = new Task.SendFulfillmentEmail();
                                SendFulfillmentEmail.Execute();
                                break;

                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
                Thread.Sleep(5000);
            }
        }
    }    
}
