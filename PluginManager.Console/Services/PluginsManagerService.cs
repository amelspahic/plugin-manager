using log4net;
using Microsoft.Practices.Unity;
using PluginManager.Console.Entities;
using PluginManager.Console.Interfaces;
using PluginManager.Console.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PluginManager.Console.Services
{
    public class PluginsManagerService : IPluginsManagerService
    {
        [Dependency]
        public IAssemblyService AssemblyService { get; set; }

        [Dependency]
        public IPluginService PluginService { get; set; }

        private static ILog logger = LogManager.GetLogger(typeof(PluginsManagerService));

        public void UpdateAndRunAssemblies()
        {
            List<PluginLibInfo> serverPluginsInfo = PluginService.GetServerPluginsInfo().ToList();

            if (serverPluginsInfo.Count > 0)
            {
                List<PluginLibInfo> localPluginsInfo = PluginService.GetLocalPluginsInfo().ToList();

                if (localPluginsInfo.Count == 0)
                {
                    logger.Info(UserMessages.LocalAssembliesNotFound);
                    PluginService.DownloadPlugins(serverPluginsInfo.Select(x => x.Name).ToList());
                }
                else
                {
                    List<string> updates = PluginService.GetPluginsForUpdate(localPluginsInfo, serverPluginsInfo).ToList();

                    if (updates.Count > 0)
                    {
                        PluginService.DownloadPlugins(updates);
                    }
                }
            }
            else
            {
                logger.Info(UserMessages.PluginsNotAvailable);
            }

            AssemblyService.ExecuteAssemblies();
        }
    }
}
