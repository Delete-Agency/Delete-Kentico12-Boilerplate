using CMS.EventLog;
using CMS.Scheduler;
using DeleteBoilerplate.Common.Extensions;
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
            catch (Exception e)
            {
                var message = $"Unable to parse '{task.TaskName}' task parameters. Please check what is in the task's Data field";
                EventLogProvider.LogException(task.TaskName, nameof(Execute), e, task.TaskSiteID, message);
                return message;
            }

            return base.Execute(task);
        }

        protected override void LogTaskStartEvent(TaskInfo task)
        {
            EventLogProvider.LogInformation(task.TaskName, nameof(Execute), $"The task '{task.TaskName}' was started [SiteID: {task.TaskSiteID}; Parameters: {task.TaskData}]");
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
