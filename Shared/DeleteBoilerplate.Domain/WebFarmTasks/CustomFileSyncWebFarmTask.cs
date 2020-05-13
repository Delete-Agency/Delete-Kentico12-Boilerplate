using CMS.Core;
using CMS.Helpers;
using CMS.IO;
using CMS.SiteProvider;
using DeleteBoilerplate.Common.Helpers;
using DeleteBoilerplate.Domain.Helpers;
using System;

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
            catch (Exception ex)
            {
                LogHelper.LogException("FILE_SYNC_ERROR", ex, SiteContext.CurrentSiteID);
            }
        }
    }
}
