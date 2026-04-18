using ImageSolutions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduledTaskProcess
{
    public class ExecuteTask
    {
        public void ExecuteTaskEntries()
        {
            try
            {

                //Task.CreateNetSuiteFulfillment createNetSuiteFulfillment = new Task.CreateNetSuiteFulfillment();
                //createNetSuiteFulfillment.Execute();

                //Task.CreateNetSuiteInvoice createNetSuiteInvoice = new Task.CreateNetSuiteInvoice();
                //createNetSuiteInvoice.Execute();

                //Task.GetSandSActiveWearPO getSandSActiveWearPO = new ScheduledTaskProcess.Task.GetSandSActiveWearPO();
                //getSandSActiveWearPO.Execute();

                //Task.ClosePO ClosePO = new Task.ClosePO();
                //ClosePO.SendEmail();

                //Task.CloseSOByPO CloseSOByPO = new Task.CloseSOByPO();
                //CloseSOByPO.Execute(new ImageSolutions.TaskEntry.TaskEntry());

                ImageSolutions.TaskEntry.TaskEntryFilter TaskEntryFilter = new ImageSolutions.TaskEntry.TaskEntryFilter();
                TaskEntryFilter.Status = new Database.Filter.StringSearch.SearchFilter();
                TaskEntryFilter.Status.SearchString = "Pending";
                TaskEntryFilter.ScheduleRunDateDelay = true;
                //TaskEntryFilter.ScheduleRunDateDelay = false;
                //TaskEntryFilter.TaskID = new Database.Filter.StringSearch.SearchFilter();
                //TaskEntryFilter.TaskID.SearchString = "5";

                //Get all Pending Tasks
                List<ImageSolutions.TaskEntry.TaskEntry> TaskEntries = ImageSolutions.TaskEntry.TaskEntry.GetTaskEntries(TaskEntryFilter);

                Parallel.ForEach(TaskEntries, new ParallelOptions { MaxDegreeOfParallelism = 1 },
                taskEntry =>
                {
                    try
                    {
                        //Validate Status
                        if (ValidatePendingStatus(taskEntry.TaskEntryID))
                        {
                            Console.WriteLine(string.Format("Executing TaskEntry {0}", taskEntry.TaskEntryID));

                            taskEntry.Status = "Processing";
                            taskEntry.Update();

                            //Execute Task
                            Execute(taskEntry);

                            //Update Status to "Completed"
                            taskEntry.Status = "Completed";
                            taskEntry.Update();
                        }
                    }
                    catch (Exception ex)
                    {
                        //Update Status to "Failed" and save error message to TaskError
                        Console.WriteLine(string.Format("TaskError: {0}, TaskID: {1}, Error: {2}", taskEntry.TaskEntryID, taskEntry.TaskID, ex.Message));
                        taskEntry.Status = "Failed";
                        taskEntry.Update();

                        InsertTaskError(taskEntry, ex.Message);
                    }
                
                });

                //Loop through pending tasks
                //Confirm if status is still Pending
                //Update Status to Processing
                //Execute Task
                //Update Status to Complete

            }
            catch (Exception ex)
            {

                //Catch Error
                //Update Status to Failed
                //Save Error Message

                throw ex;
            }
        }

        private bool ValidatePendingStatus(string taskentryid)
        {
            bool blnReturn = true;
            try
            {
                ImageSolutions.TaskEntry.TaskEntryFilter TaskEntryFilter = new ImageSolutions.TaskEntry.TaskEntryFilter();
                TaskEntryFilter.TaskEntryID = new Database.Filter.StringSearch.SearchFilter();
                TaskEntryFilter.TaskEntryID.SearchString = taskentryid;

                ImageSolutions.TaskEntry.TaskEntry TaskEntry = ImageSolutions.TaskEntry.TaskEntry.GetTaskEntry(TaskEntryFilter);
                if (TaskEntry.Status != "Pending")
                {
                    blnReturn = false;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return blnReturn;
        }

        protected void InsertTaskError(ImageSolutions.TaskEntry.TaskEntry taskentry , string errormessage)
        {
            try
            {
                ImageSolutions.TaskEntry.TaskError TaskError = new ImageSolutions.TaskEntry.TaskError();
                TaskError.TaskEntryID = taskentry.TaskEntryID;
                TaskError.ErrorMessage = errormessage;

                TaskError.Create();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void Execute(ImageSolutions.TaskEntry.TaskEntry taskentry)
        {
            switch (taskentry.TaskID)
            {
                case "1":
                    Task.CreateTaskEntryClosePOOverdueNonInventory CreateTaskEntryClosePOOverdueNonInventory = new Task.CreateTaskEntryClosePOOverdueNonInventory();
                    CreateTaskEntryClosePOOverdueNonInventory.Execute();
                    break;
                case "4":
                    Task.ClosePO ClosePO = new Task.ClosePO();
                    ClosePO.Execute(taskentry);
                    break;
                case "5":
                    Task.CloseSOByPO CloseSOByPO = new Task.CloseSOByPO();
                    CloseSOByPO.Execute(taskentry);
                    break;
                case "6":
                    Task.SendClosePOWarning SendClosePOWarning = new Task.SendClosePOWarning();
                    SendClosePOWarning.Execute(taskentry);
                    break;
                case "7":
                    Task.GetSandSActiveWearPO GetSandSActiveWearPO = new Task.GetSandSActiveWearPO();
                    GetSandSActiveWearPO.Execute();
                    break;
                case "8":
                    Task.CreateNetSuiteInvoice CreateNetSuiteInvoice = new Task.CreateNetSuiteInvoice();
                    CreateNetSuiteInvoice.Execute();
                    break;
                case "9":
                    Task.CreateNetSuiteFulfillment CreateNetSuiteFulfillment = new Task.CreateNetSuiteFulfillment();
                    CreateNetSuiteFulfillment.Execute();
                    break;
                case "10":
                    Task.SyncItemDatabaseToNetSuite SyncItemDatabaseToNetSuite = new Task.SyncItemDatabaseToNetSuite();
                    SyncItemDatabaseToNetSuite.SyncChildItems();
                    break;
                default:
                    throw new Exception(string.Format(@"Invalid task id: {0}", taskentry.TaskID));
            }
        }
    }
}
