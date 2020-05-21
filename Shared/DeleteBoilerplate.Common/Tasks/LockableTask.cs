using CMS.Scheduler;
using DeleteBoilerplate.Common.Helpers;
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
                    LogHelper.LogInformation(task.TaskName, "LOCK_NOT_ACQUIRED", $"The lock was not acquired for the task '{task.TaskName}'.");
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
            LogHelper.LogInformation(task.TaskName, "TASK_FINISH", $"The task '{task.TaskName}' was finished.", $"Elapsed time: {stopwatch.Elapsed:c}]", task.TaskSiteID);
        }

        protected virtual void LogTaskStartEvent(TaskInfo task)
        {
            LogHelper.LogInformation(task.TaskName, "TASK_START", $"The task '{task.TaskName}' was started.", siteId: task.TaskSiteID);
        }
    }
}
