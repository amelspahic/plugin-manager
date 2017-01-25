using log4net;
using Newtonsoft.Json;
using PluginManager.Console.Interfaces;
using PluginManager.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using PluginManager.Console.Properties;
using PluginManager.Console.Resources;
using PluginManager.Console.Helpers;

namespace PluginManager.Console.Services
{
    public class AssemblyService : IAssemblyService
    {
        private static ILog logger = LogManager.GetLogger(typeof(AssemblyService));

        /// <summary>
        /// Method will load and execute assemblies from path defined in .config
        /// </summary>
        public void ExecuteAssemblies()
        {
            string pluginsPath = Settings.Default.LocalPluginsFolder;
            string[] fileNames = Directory.GetFiles(pluginsPath, "*.dll", SearchOption.AllDirectories);

            if (fileNames.Length > 0)
            {
                logger.Info(string.Format(UserMessages.AssembliesFound, fileNames.Length, String.Join(" ", RemovePathFromAssemblyNames(fileNames))));
                
                foreach (string fileName in fileNames)
                {
                    try
                    {
                        System.Console.WriteLine(UserMessages.LineSeparator);

                        //We can use LoadFile since these assemblies are not loaded in current appDomain
                        Assembly loadedAssembly = Assembly.LoadFile(fileName);

                        logger.Info(string.Format(UserMessages.AssemblyLoaded, Path.GetFileName(fileName)));

                        List<Type> types = Common.GetTypesByInterface<ISimplePlugin>(loadedAssembly);

                        foreach (Type type in types)
                        {
                            logger.Info(string.Format(UserMessages.MethodExecuting, type.Name));

                            ISimplePlugin plugin = (ISimplePlugin)Activator.CreateInstance(type);
                            plugin.Print();

                            logger.Info(string.Format(UserMessages.MethodExecuted, type.Name));
                        }
                    }
                    catch (FileLoadException ex)
                    {
                        logger.Error(string.Format(UserMessages.FileLoadExeption, fileName, ex.Message));
                    }
                    catch (BadImageFormatException ex)
                    {
                        logger.Error(string.Format(UserMessages.BadImageException, fileName, ex.Message));
                    }
                    catch (Exception ex)
                    {
                        logger.Error(string.Format(UserMessages.UnhandledException, ex.Message));
                    }
                }
            }
        }

        /// <summary>
        /// Removes part of string
        /// </summary>
        /// <param name="fullPathName"></param>
        /// <returns></returns>
        private List<string> RemovePathFromAssemblyNames(string[] fullPathName)
        {
            List<string> newListStr = new List<string>();
            foreach (string str in fullPathName)
            {
                string output = str.Substring(str.IndexOf('\\') + 1);
                newListStr.Add(Common.RemoveStringPart(str, Settings.Default.LocalPluginsFolder));
            }

            return newListStr;
        }
    }
}
