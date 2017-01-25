using PluginManager.Console.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PluginManager.Console.Entities;
using System.IO;
using System.Net.Http;
using Newtonsoft.Json;
using PluginManager.Console.Extensions;
using log4net;
using PluginManager.Console.Properties;
using PluginManager.Console.Resources;

namespace PluginManager.Console.Services
{
    public class PluginService : IPluginService
    {
        private static ILog logger = LogManager.GetLogger(typeof(PluginService));

        /// <summary>
        /// Downloads plugins based on plugin filename(s)
        /// </summary>
        /// <param name="filenames">Plugin filename</param>
        public void DownloadPlugins(IList<string> filenames)
        {
            try
            {
                string pluginServerUrl = Settings.Default.PluginsServerUrl;
                string localFolderPath = Settings.Default.LocalPluginsFolder;

                Download(filenames.ToList(), pluginServerUrl, localFolderPath).Wait();
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }

        /// <summary>
        /// Retrieves info about local plugins assemblies
        /// </summary>
        /// <returns>List of plugins info</returns>
        public IList<PluginLibInfo> GetLocalPluginsInfo()
        {
            List<PluginLibInfo> plugins = new List<PluginLibInfo>();

            string localPluginsPath = Settings.Default.LocalPluginsFolder;

            //Create directory if does not exist
            if (!Directory.Exists(localPluginsPath))
            {
                Directory.CreateDirectory(localPluginsPath);
            }

            DirectoryInfo directory = new DirectoryInfo(localPluginsPath);

            foreach (var file in directory.GetFiles("*.dll"))
            {
                plugins.Add(new PluginLibInfo { Name = file.Name, DateCreated = file.LastWriteTimeUtc });
            }

            return plugins;
        }

        /// <summary>
        /// Retrieves list of plugins for update comparing dates
        /// </summary>
        /// <param name="localPluginsInfo"></param>
        /// <param name="serverPluginsInfo"></param>
        /// <returns>List of plugins for update</returns>
        public IList<string> GetPluginsForUpdate(IList<PluginLibInfo> localPluginsInfo, IList<PluginLibInfo> serverPluginsInfo)
        {
            logger.Info(UserMessages.CheckForUpdates);

            List<PluginLibInfo> newPlugins = new List<PluginLibInfo>();

            //Will retrieve server plugins which are different from local ones
            foreach (var serverPlugin in serverPluginsInfo.ToList())
            {
                PluginLibInfo lInfo = new PluginLibInfo();
                lInfo = localPluginsInfo.ToList().Where(x => x.Name.Equals(serverPlugin.Name) && DateTime.Compare(x.DateCreated.Value, serverPlugin.DateCreated.Value) < 0).SingleOrDefault();

                if (lInfo != null)
                {
                    newPlugins.Add(lInfo);
                }
            }

            logger.Info(string.Format(UserMessages.UpdateFound, newPlugins.Count));
            return newPlugins.Select(x=>x.Name).ToList();
        }

        /// <summary>
        /// Retrieves server plugins info
        /// </summary>
        /// <returns>List of server plugins info</returns>
        public IList<PluginLibInfo> GetServerPluginsInfo()
        {
            List<PluginLibInfo> res = new List<PluginLibInfo>();
            string pluginServerUrl = Settings.Default.PluginsServerUrl;

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri(pluginServerUrl);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                    
                    // New code:
                    var task = client.GetAsync("api/plugins").ContinueWith((taskwithresponse) =>
                    {
                        var response = taskwithresponse.Result;
                        var jsonString = response.Content.ReadAsStringAsync();
                        jsonString.Wait();

                        res = JsonConvert.DeserializeObject<List<PluginLibInfo>>(jsonString.Result);
                    });

                    task.Wait();
                }

                return res;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #region Private methods

        /// <summary>
        /// Downloads plugins asynchronously
        /// </summary>
        /// <param name="filenames">Filenames of plugins which will be downloaded</param>
        /// <param name="serverPath">Plugin server host path</param>
        /// <param name="localFolderPath">Folder to be downloaded to</param>
        /// <returns></returns>
        private async Task Download(List<string> filenames, string serverPath, string localFolderPath)
        {
            logger.Info(UserMessages.DownloadStart);

            try
            {
                using (HttpClient httpClient = new HttpClient())
                {
                    httpClient.BaseAddress = new Uri(serverPath);

                    foreach (var pluginName in filenames)
                    {
                        string apiDownloadPath = string.Format("api/plugins/{0}/download", pluginName);
                        string fullServerPath = string.Format("{0}{1}", serverPath, apiDownloadPath);

                        logger.Info(string.Format(UserMessages.Downloading, pluginName, fullServerPath));
                        HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, apiDownloadPath);

                        await httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead).
                            ContinueWith((response)
                                =>
                            {
                                try
                                {
                                    ProcessResponse(string.Format("{0}\\{1}", localFolderPath, pluginName), response);
                                }
                                catch (AggregateException aggregateException)
                                {
                                    logger.Error(aggregateException.Message);
                                }
                            });

                        logger.Info(string.Format(UserMessages.DownloadFinished, pluginName));
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(string.Format(UserMessages.UnhandledException, ex.Message));
            }

            logger.Info(UserMessages.DownloadComplete);
        }

        private void ProcessResponse(string localFilePath, Task<HttpResponseMessage> response)
        {
            if (response.Result.IsSuccessStatusCode)
            {
                response.Result.Content.DownloadFile(localFilePath).
                    ContinueWith((message) =>
                    {
                        logger.Info(message.Result);
                    });
            }
            else
            {
                string failMessage = 
                    string.Format(UserMessages.ProcessFail,
                    response.Result.StatusCode,
                    response.Result.ReasonPhrase,
                    response.Result.Content.ReadAsStringAsync().Result);

                logger.Info(failMessage);
            }
        }

        #endregion
    }
}
