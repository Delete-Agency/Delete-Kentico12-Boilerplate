using System;
using CMS.Core;
using CMS.EventLog;
using CMS.Helpers;
using CMS.IO;
using CMS.SiteProvider;
using DeleteBoilerplate.Domain.Helpers;

namespace DeleteBoilerplate.Domain.WebFarmTasks
{
    public class CustomFileSyncWebFarmTask : WebFarmTaskBase
    {
        public override void ExecuteTask()
        {
            try
            {
                CustomFileHelper.EnsureDirectoryExists(this.TaskTarget);

                var pathToSave = FileHelper.GetFullFilePhysicalPath(this.TaskTarget);
                
                using (var file = File.Create(pathToSave))
                {
                    file.Write(this.TaskBinaryData.Data, 0, this.TaskBinaryData.Data.Length);
                    file.Close();
                }
            }
            catch (Exception e)
            {
                EventLogProvider.LogException(nameof(CustomFileSyncWebFarmTask), "FILE_SYNC_ERROR", e, SiteContext.CurrentSiteID);
            }
        }
    }
}
