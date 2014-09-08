using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HttpServer.Sessions;
using HttpServer;
using Panasonic.KV_SS1100.API.WebConfig;
using Panasonic.KV_SS1100.API.Config;
using System.Diagnostics;

namespace PluginExample
{
    class MyConfigHttpHandler : IHttpHandler
    {
        #region IHttpHandler Members
        private IEnumerable<string> configFields;
        private IDictionary<string, string> configDefaults;
        public MyConfigHttpHandler()
        {
            this.configFields = new List<String>() {
                "hie_pdq_endpoint", "hie_xdsb_endpoint",
                "http_username", "http_password", 
                "ws_security_username", "ws_security_password" };
            this.configDefaults = new Dictionary<string, string>();
            this.configDefaults["hie_pdq_endpoint"] = "http://exchange.healthshare.us:57772/csp/healthshare/hsregistry/HS.IHE.PDQv3.Supplier.Services.CLS";
            this.configDefaults["hie_xdsb_endpoint"] = "http://exchange.healthshare.us:57772/csp/healthshare/hsrepository/HS.IHE.XDSb.Repository.Services.CLS";
            this.configDefaults["http_username"] = "_system";
            this.configDefaults["http_password"] = "SYS";
            this.configDefaults["ws_security_username"] = "HS_Services";
            this.configDefaults["ws_security_password"] = "HS_Services";
        }
        void IHttpHandler.Process(IHttpRequest request, IHttpResponse response, IHttpSession session)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("<h2>IHE Plugin configuration</h2>");
            sb.Append("<p>Welcome to the InterSystems Panasonic IHE Plugin&copy;</p>");
            sb.Append("<p>In order to scan documents directly into a Health InformationExchange" +
                    " you must complete the following configuration settings. For more " +
                    "information visit " +
                    "<a href=\"http://intersystems.com/ihe-scanners\">" +
                        "http://intersystems.com/ihe-scanners</a></p><br/>");

            if (request.Method == "POST")
            {
              
                // Write some data in the config
                lock (ConfigManager.Global)
                {
                    System.Collections.IEnumerator enumerator = request.Form.GetEnumerator();  
                    while ( enumerator.MoveNext() )
                    {
                        HttpInputItem item = (HttpInputItem) enumerator.Current;
                        string field = item.Name;
                        //var value = request.Form[field];
                        string value = item.Value;
                        Debug.WriteLine(field + "=" + value);
                        ConfigManager.Global.Set<string>("InterSystems.IHEPlugin."+field, 
                            value);
                    }
                }
            }
            else
            {
                // Retrieve data from config
                foreach(var field in this.configFields) {

                    string value = ConfigManager.Global.Get<string>(
                        "InterSystems.IHEPlugin."+field, this.configDefaults[field]);

                    if (value != null)
                    {
                        sb.Append(field + "=" + value + "<br/>");
                    }
                    else {
                        sb.Append("<p>"+field+": <b>NOT DEFINED!</b></p>");
                    }
                }
            }

            

            foreach (var field in configFields ) {
                string value = ConfigManager.Global.Get<string>(
                        "InterSystems.IHEPlugin." + field, this.configDefaults[field]);

                sb.Append("<label for="+field+">"+field+": </label>" +
                      "<input type=text name='"+field+"' id='"+field+
                      "' value='"+value+"' " +
                      "' placeholder='"+value+"'/>");
                sb.Append("<br/>");
            
            }
            sb.Append("<br><input type='submit'>");

            sb.Append("<br/><br/>");
            sb.Append("<h2>TODO: <input type='button'>Test Configuration</input></h2>");
            WebConfigServer.SendPage(response,
                                     "MyWebConfigPage",
                                     sb.ToString());
        }

        #endregion
    }
}
