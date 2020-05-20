using CMS.EventLog;
using CMS.Scheduler;
using System.Diagnostics;
using System.Threading;

namespace DeleteBoilerplate.Common.Tasks
{
    public abstract class LockableTask : ITask
    {
        protected static object LockObj = new object();

        public abstract string ExecuteCriticalCode(TaskInfo task);

        public virtual string Execute(TaskInfo task)
        {
            var result = string.Empty;
            var lockTaken = false;
            var stopwatch = new Stopwatch();
            try
            {
                Monitor.TryEnter(LockObj, ref lockTaken);

                if (lockTaken)
                {
                    stopwatch.Start();
                    LogTaskStartEvent(task);
                    result = ExecuteCriticalCode(task);
                }
                else
                {
                    // The lock was not acquired.
                    EventLogProvider.LogInformation(task.TaskName, nameof(Execute), $"The lock was not acquired for the task '{task.TaskName}'");
                }
            }
            finally
            {
                // Ensure that the lock is released.
                if (lockTaken)
                {
                    LogTaskFinishEvent(task, stopwatch);
                    Monitor.Exit(LockObj);
                }

            }

            return result;
        }

        protected virtual void LogTaskFinishEvent(TaskInfo task, Stopwatch stopwatch)
        {
            EventLogProvider.LogInformation(task.TaskName, nameof(Execute), $"The task '{task.TaskName}' was finished [SiteID: {task.TaskSiteID}, Elapsed time: {stopwatch.Elapsed:c}]");
        }

        protected virtual void LogTaskStartEvent(TaskInfo task)
        {
            EventLogProvider.LogInformation(task.TaskName, nameof(Execute), $"The task '{task.TaskName}' was started [SiteID: {task.TaskSiteID}]");
        }
    }
}
