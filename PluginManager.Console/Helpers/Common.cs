using PluginManager.Console.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace PluginManager.Console.Helpers
{
    public static class Common
    {
        public static List<Type> GetTypesByInterface<T>(Assembly assembly)
        {
            if (!typeof(T).IsInterface)
                throw new ArgumentException(UserMessages.NotInterfaceException);

            return assembly.GetTypes()
                .Where(x => x.GetInterface(typeof(T).Name) != null)
                .ToList();
        }

        public static string RemoveStringPart(string originalString, string partToRemove)
        {
            string newString = originalString.Replace(partToRemove, string.Empty);

            return newString;
        }
    }
}
