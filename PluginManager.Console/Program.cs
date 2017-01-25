using Microsoft.Practices.Unity;
using PluginManager.Console.Helpers;
using PluginManager.Console.Interfaces;
using PluginManager.Console.Properties;
using PluginManager.Console.Services;
using PluginManager.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace PluginManager.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            //Can be added somewhere else, but before PluginManagerService execution
            UnityContainer container = new UnityContainer();
            container.RegisterType<IAssemblyService, AssemblyService>(new HierarchicalLifetimeManager());
            container.RegisterType<IPluginService, PluginService>(new HierarchicalLifetimeManager());
            container.RegisterType<IPluginsManagerService, PluginsManagerService>(new HierarchicalLifetimeManager());

            PluginsManagerService PluginsManagerService = container.Resolve<PluginsManagerService>();

            PluginsManagerService.UpdateAndRunAssemblies();

            System.Console.ReadLine();
        }
    }
}
