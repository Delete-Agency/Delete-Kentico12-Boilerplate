using CMS.Scheduler;
using DeleteBoilerplate.Common.Extensions;
using DeleteBoilerplate.Common.Helpers;
using System;

namespace DeleteBoilerplate.Common.Tasks
{
    public abstract class ParameterizedTask<TParams> : LockableTask where TParams : new()
    {
        protected TParams Parameters { get; private set; }

        public override string Execute(TaskInfo task)
        {
            try
            {
                Parameters = ParseTaskParameters(task);
            }
            catch (Exception ex)
            {
                var message = $"Unable to parse '{task.TaskName}' task parameters. Please check what is in the task's Data field";
                LogHelper.LogException(task.TaskName, "INVALID_PARAMETERS", ex, message);

                return message;
            }

            return base.Execute(task);
        }

        protected override void LogTaskStartEvent(TaskInfo task)
        {
            LogHelper.LogInformation(task.TaskName, "TASK_EXECUTION", $"The task '{task.TaskName}' was started.", $"Parameters: {task.TaskData}");
        }

        protected virtual TParams ParseTaskParameters(TaskInfo task)
        {
            if (task.TaskData.IsEmpty())
            {
                throw new InvalidOperationException($"The '{task.TaskName}' Data field is empty");
            }

            var parameters = task.TaskData.DeserializeJson<TParams>();
            if (parameters == null)
            {
                throw new InvalidOperationException($"The '{task.TaskName}' parsed parameters object is null");
            }

            return parameters;
        }
    }
}
