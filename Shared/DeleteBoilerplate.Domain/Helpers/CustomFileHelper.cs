using System.IO.Compression;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using CMS.Helpers;
using CMS.IO;
using DeleteBoilerplate.Domain.WebFarmTasks;

namespace DeleteBoilerplate.Domain.Helpers
{
    public static class CustomFileHelper
    {
        public static void SerializeXmlToFile<T>(string fileLocalPath, T obj)
        {
            var filePath = FileHelper.GetFullFilePhysicalPath(fileLocalPath);

            EnsureDirectoryExists(fileLocalPath);

            var serializer = new XmlSerializer(typeof(T));

            using (var writer = StreamWriter.New(filePath))
            using (var xmlWriter = XmlWriter.Create(writer, new XmlWriterSettings { Indent = true, Encoding = Encoding.UTF8 }))
            {
                serializer.Serialize(xmlWriter, obj);
            }
        }

        public static string CompressFile(string fileLocalPath)
        {
            var filePath = FileHelper.GetFullFilePhysicalPath(fileLocalPath);

            using (var file = File.Open(filePath, FileMode.Open, FileAccess.Read))
            {
                var outputFileLocalPath = fileLocalPath + ".gz";
                var outputFilePath = FileHelper.GetFullFilePhysicalPath(outputFileLocalPath);

                DeleteFile(outputFileLocalPath);

                using (var compressedFile = File.Create(outputFilePath))
                {
                    using (var gzipStream = new GZipStream(compressedFile, CompressionMode.Compress))
                    {
                        file.CopyTo(gzipStream);
                    }
                }

                return outputFileLocalPath;
            }
        }

        public static void EnsureDirectoryExists(string fileLocalPath)
        {
            var filePath = FileHelper.GetFullFilePhysicalPath(fileLocalPath);

            var directoryPath = Path.GetDirectoryName(filePath);

            if (!Directory.Exists(directoryPath))
                Directory.CreateDirectory(directoryPath);
        }

        public static void DeleteFile(string fileLocalPath)
        {
            var filePath = FileHelper.GetFullFilePhysicalPath(fileLocalPath);

            if (File.Exists(filePath))
                File.Delete(filePath);
        }

        // For usage in CMS solution only
        public static void DeployFileToContentDeliveryServers(string fileLocalPath, string destinationFileLocalPath)
        {
            var filePath = FileHelper.GetFullFilePhysicalPath(fileLocalPath);

            var fileBytes = File.ReadAllBytes(filePath);

            var syncTask = new CustomFileSyncWebFarmTask
            {
                TaskBinaryData = fileBytes,
                TaskTarget = destinationFileLocalPath
            };

            WebFarmHelper.CreateIOTask(syncTask);
        }
    }
}
