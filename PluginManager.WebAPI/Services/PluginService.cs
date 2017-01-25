using PluginManager.WebAPI.HelpPageExtensions;
using PluginManager.WebAPI.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Web;

namespace PluginManager.WebAPI.Services
{
    public class PluginService
    {
        /// <summary>
        /// Retrieves hosted plugins info
        /// </summary>
        /// <returns>List of hosted plugins info</returns>
        public List<PluginLibInfo> GetPluginsInfo()
        {
            List<PluginLibInfo> pluginsInfo = new List<PluginLibInfo>();

            try
            {
                string pluginsPath = HttpContext.Current.Server.MapPath("~/Plugins");
                DirectoryInfo directory = new DirectoryInfo(pluginsPath);
                

                foreach (var file in directory.GetFiles("*.dll"))
                {
                    pluginsInfo.Add(new PluginLibInfo { Name = file.Name, DateCreated = file.LastWriteTimeUtc });
                }

                return pluginsInfo;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error occured. Details:", ex.InnerException);
            }
        }

        /// <summary>
        /// Creates stream for plugin download
        /// </summary>
        /// <param name="fileName">pluginName</param>
        /// <returns></returns>
        public DownloadRequest Download(string fileName)
        {
            DownloadRequest request = new DownloadRequest();

            string localFilePath = HttpContext.Current.Server.MapPath(string.Format("~/Plugins/{0}", fileName));
            FileInfo file = new FileInfo(localFilePath);

            if (!file.Exists)
            {
                throw new FileNotFoundException("File not found");
            }

            try
            {
                int bufferSize = 4096;

                request.FileName = file.Name;
                request.ContentType = "application/octet-stream";

                request.PushStreamFunction = async (stream) =>
                {
                    using (var reader = new StreamContent(new FileStream(file.FullName, FileMode.Open, FileAccess.Read, FileShare.Read, bufferSize)))
                    using (stream) //this has to be called in order to signal caller that we have finished
                    {
                        await reader.CopyToAsync(stream);
                    }
                };
            }
            catch (Exception ex)
            {
                throw ex;
            }
            

            return request;
        }
    }
}