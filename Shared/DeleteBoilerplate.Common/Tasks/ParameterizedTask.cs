using CMS.Scheduler;
using DeleteBoilerplate.Common.Extensions;
using DeleteBoilerplate.Common.Helpers;
using System;
using System.Diagnostics;

namespace DeleteBoilerplate.Common.Tasks
{
    public abstract class ParameterizedTask<TParams> : ITask where TParams : new()
    {
        public abstract string ExecuteCriticalCode(TaskInfo task);

        protected TParams Parameters { get; private set; }

        public virtual string Execute(TaskInfo task)
        {
            try
            {
                Parameters = this.ParseTaskParameters(task);
            }
            catch (Exception ex)
            {
                var message = $"Unable to parse '{task.TaskName}' task parameters. Please check what is in the task's Data field";
                LogHelper.LogException(task.TaskName, "INVALID_PARAMETERS", ex, message);

                return message;
            }

            var stopwatch = new Stopwatch();
            string result;
            try
            {
                stopwatch.Start();
                this.LogTaskStartEvent(task);

                result = this.ExecuteCriticalCode(task);
            }
            finally
            {
                this.LogTaskFinishEvent(task, stopwatch);
            }

            return result;
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

        protected virtual void LogTaskStartEvent(TaskInfo task)
        {
            LogHelper.LogInformation(task.TaskName, "TASK_START", $"The task '{task.TaskName}' was started.", $"Parameters: {task.TaskData}");
        }

        protected virtual void LogTaskFinishEvent(TaskInfo task, Stopwatch stopwatch)
        {
            LogHelper.LogInformation(task.TaskName, "TASK_FINISH", $"The task '{task.TaskName}' was finished.", $"Elapsed time: {stopwatch.Elapsed:c}]", task.TaskSiteID);
        }
    }
}
