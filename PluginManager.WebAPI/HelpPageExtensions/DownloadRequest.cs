using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace PluginManager.WebAPI.HelpPageExtensions
{
    /// <summary>
    /// Contains necessary download data
    /// </summary>
    public class DownloadRequest
    {
        public string FileName { get; set; }
        public string ContentType { get; set; }
        public Func<Stream, Task> PushStreamFunction { get; set; }
    }
}