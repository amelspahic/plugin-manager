using PluginManager.WebAPI.HelpPageExtensions;
using PluginManager.WebAPI.Models;
using PluginManager.WebAPI.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;
using System.Web.Http.Description;

namespace PluginManager.WebAPI.Controllers
{
    [RoutePrefix("api/plugins")]
    public class pluginsController : ApiController
    {
        /// <summary>
        /// Gets all available plugins
        /// </summary>
        /// <returns>List of available plugins info</returns>
        [Route("")]
        [HttpGet]
        [ResponseCodes(HttpStatusCode.OK, HttpStatusCode.NotFound)]
        [ResponseType(typeof(List<PluginLibInfo>))]
        public HttpResponseMessage Get()
        {
            HttpResponseMessage response = null;
            PluginService pluginSvc = new PluginService();
            List<PluginLibInfo> pluginsInfo = pluginSvc.GetPluginsInfo();

            response = Request.CreateResponse(HttpStatusCode.OK, pluginsInfo);
            return response;
        }

        /// <summary>
        /// Downloads plugin by name
        /// </summary>
        /// <param name="name">plugin name</param>
        /// <returns></returns>
        [Route("{name}/download")]
        [HttpGet]
        [ResponseCodes(HttpStatusCode.OK, HttpStatusCode.NotFound)]
        public HttpResponseMessage Download([FromUri]string name)
        {
            HttpResponseMessage resp = new HttpResponseMessage();
            PluginService pluginSvc = new PluginService();
            DownloadRequest downloadData = pluginSvc.Download(name);

            resp.Content = new PushStreamContent(async (responseStream, content, context) =>
            {
                await downloadData.PushStreamFunction(responseStream);
            });

            resp.Headers.AcceptRanges.Add("bytes");
            resp.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
            resp.Content.Headers.ContentDisposition.FileName = downloadData.FileName;
            resp.Content.Headers.ContentType = new MediaTypeHeaderValue(downloadData.ContentType);

            return resp;
        }
    }
}
