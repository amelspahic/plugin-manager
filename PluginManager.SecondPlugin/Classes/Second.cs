using PluginManager.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PluginManager.SecondPlugin.Classes
{
    public class Second : ISimplePlugin
    {
        //Outputs simple message
        public void Print()
        {
            Console.WriteLine("***Simple output from: SecondPlugin.Classes.Second***");
        }
    }
}
