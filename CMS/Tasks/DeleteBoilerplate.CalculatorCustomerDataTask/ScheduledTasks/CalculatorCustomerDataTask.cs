using System;
using CMS;
using CMS.Scheduler;
using DeleteBoilerplate.Tasks.ScheduledTasks;
using DeleteBoilerplate.Common.Extensions;
using DeleteBoilerplate.Common.Helpers;
using DeleteBoilerplate.Common.Tasks;
using DeleteBoilerplate.Tasks.Models;
using DeleteBoilerplate.Tasks.Processor;

[assembly: RegisterCustomClass("CalculatorCustomerDataTask", typeof(CalculatorCustomerDataTask))]

namespace DeleteBoilerplate.Tasks.ScheduledTasks
{
    public class CalculatorCustomerDataTask : ParameterizedTask<CalculatorInfo>
    {
        public override string ExecuteCriticalCode(TaskInfo task)
        {
            try
            {
                var processor = new CalculatorProcessor();
                processor.Process(Parameters);

                LogHelper.LogInformation(task.TaskName, "SUCCESS");
                return "Task completed successfully";
            }
            catch (Exception ex)
            {
                LogHelper.LogException(task.TaskName, "ERROR", ex);

                var errorMessage = $"The task was failed. Error: {ex.GetAllMessages()}";
                return errorMessage;
            }
        }
    }
}
