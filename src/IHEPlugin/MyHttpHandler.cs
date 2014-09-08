using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HttpServer.Sessions;
using HttpServer;
using Panasonic.KV_SS1100.API.WebConfig;

namespace PluginExample
{
    class MyHttpHandler : IHttpHandler
    {
        #region IHttpHandler Members

        public void Process(IHttpRequest request, IHttpResponse response, IHttpSession session)
        {
            WebConfigServer.SendPage(response, "Plugin Example HTTP Handler", "<h2>Hello World!</h2>");
        }

        #endregion
    }
}
