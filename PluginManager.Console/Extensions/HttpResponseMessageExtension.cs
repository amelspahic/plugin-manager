using Newtonsoft.Json;
using PluginManager.Console.Extensions;
using PluginManager.Console.Resources;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace PluginManager.Console.Extensions
{
    public static class HttpResponseMessageExtension
    {
        /// <summary>
        /// Async download extension
        /// </summary>
        /// <param name="content"></param>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static async Task<string> DownloadFile(this HttpContent content, string filePath)
        {
            FileStream fs = null;
            string result = string.Empty;

            try
            {
                return await
                    content.ReadContent(filePath).
                    ContinueWith((response) =>
                    {
                        try
                        {
                            if (!response.IsFaulted && response.Result)
                            {
                                result = string.Format(UserMessages.PluginDownloadSuccess,
                                    Path.GetFileName(filePath));
                            }
                            else
                            {
                                result = string.Format(UserMessages.PluginDownloadFail, Path.GetFileName(filePath),
                                    JsonConvert.SerializeObject(response.Exception, Formatting.Indented));
                            }
                        }
                        catch (AggregateException ex)
                        {
                            throw ex;
                        }

                        return result;
                    });
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format(UserMessages.UnhandledException, ex.Message));
            }
            finally
            {
                if (fs != null)
                    fs.Close();
            }
        }

        /// <summary>
        /// Read content asynchronously
        /// </summary>
        /// <param name="content"></param>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static async Task<bool> ReadContent(this HttpContent content, string filePath)
        {
            FileStream fileStream = null;
            bool result = false;

            try
            {
                fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None);

                await content.CopyToAsync(fileStream).
                   ContinueWith((copyTask) =>
                   {
                       fileStream.Close();
                       result = true;
                   });
            }
            catch (DirectoryNotFoundException dnfex)
            {
                throw dnfex;
            }
            catch (InvalidOperationException invOpEx)
            {
                throw invOpEx;
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format(UserMessages.UnhandledException, ex.Message));
            }
            finally
            {
                if (fileStream != null)
                    fileStream.Close();
            }

            return result;
        }
    }
}