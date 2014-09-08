using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HttpServer.Sessions;
using HttpServer;
using Panasonic.KV_SS1100.API.WebConfig;
using Panasonic.KV_SS1100.API.Config;

namespace PluginExample
{
    class MyConfigHttpHandler : IHttpHandler
    {
        #region IHttpHandler Members

        void IHttpHandler.Process(IHttpRequest request, IHttpResponse response, IHttpSession session)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("<h2>Welcome to MyPlugin configuration</h2>");

            if (request.Method == "POST")
            {
                string theValue = request.Form["field"].Value;

                sb.Append("<p>Text data was assigned to: " + theValue + "</p>");

                // Write some data in the config
                lock (ConfigManager.Global)
                {
                    ConfigManager.Global.Set("MyCompany.MyPlugin.SomeValue", theValue);
                }
            }
            else
            {
                // Retrieve data from config
                string theValue = ConfigManager.Global.Get<string>("MyCompany.MyPlugin.SomeValue", null);

                if (theValue != null)
                {
                    sb.Append("<p>Currently saved text data: " + theValue + "</p>");
                }
                else
                {
                    sb.Append("<p>No data was saved. Enter data below to save</p>");
                }
            }

            sb.Append("<label for=field>Data to save in configuration: </label>" +
                      "<input type=text name=field id=field placeholder='Put something here'/>" +
                      "<br><input type=submit>");

            WebConfigServer.SendPage(response,
                                     "MyWebConfigPage",
                                     sb.ToString());
        }

        #endregion
    }
}
