using PluginManager.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PluginManager.FirstPlugin.Classes
{
    public class Second : ISimplePlugin
    {
        /// <summary>
        /// Outputs simple message
        /// </summary>
        public void Print()
        {
            Console.WriteLine("***Simple output from: FirstPlugin.Classes.Second***");
        }
    }
}
