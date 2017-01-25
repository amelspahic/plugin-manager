using PluginManager.Console.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PluginManager.Console.Interfaces
{
    public interface IPluginService
    {
        IList<PluginLibInfo> GetServerPluginsInfo();
        IList<PluginLibInfo> GetLocalPluginsInfo();
        void DownloadPlugins(IList<string> filenames);
        IList<string> GetPluginsForUpdate(IList<PluginLibInfo> localPluginsInfo, IList<PluginLibInfo> serverPluginsInfo);
    }
}
