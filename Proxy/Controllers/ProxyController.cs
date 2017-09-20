using Proxy.Filters;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace Proxy.Controllers
{
    [CustomFilter, RequreHttpsFilter]
    public class ProxyController : ApiController
    {
        [HttpGet, HttpPost, HttpPut, HttpPatch, HttpDelete]
        [Route("{*remaining}")]
        public Task<HttpResponseMessage> SendMessage()
        {
            // Create a new Uri using the configured host.  This is where the request will be forwarded.
            // The host value is configured in the web.config file
            string targetHost = ConfigurationManager.AppSettings.Get("TargetHost");
            string forwardUri = targetHost + Request.RequestUri.PathAndQuery;

            // Update the request with the new Uri
            Request.Headers.Remove("Host");
            Request.RequestUri = new Uri(forwardUri);
            if (Request.Method == HttpMethod.Get)
            {
                Request.Content = null;
            }

            // Call the configured host and return the result of the request
            var client = new HttpClient();
            return client.SendAsync(Request, HttpCompletionOption.ResponseHeadersRead);
        }
    }
}
