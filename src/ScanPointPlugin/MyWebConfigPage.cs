using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HttpServer.Sessions;
using HttpServer;
using Panasonic.KV_SS1100.API.WebConfig;
using Panasonic.KV_SS1100.API.Config;
using System.Diagnostics;
using HSPanasonic.ScanPoint;
using System.Globalization;

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
                "scan_registry", "healthshare_username", "healthshare_password", "healthshare_validate_ssl" };
            this.configDefaults = new Dictionary<string, string>();
            this.configDefaults["scan_registry"] = "http://192.168.1.130:20187";
            this.configDefaults["healthshare_username"] = "ScanPointTest1";
            this.configDefaults["healthshare_password"] = "password";
            this.configDefaults["healthshare_validate_ssl"] = "";
         }
        private string TestConfig()
        {
            var message = "Test Successful";
            var registryUrl =
              ConfigManager.Global.Get<string>("InterSystems.HealthSharePlugin.scan_registry");
            var username =
                ConfigManager.Global.Get<string>("InterSystems.HealthSharePlugin.healthshare_username");
            var password =
                ConfigManager.Global.Get<string>("InterSystems.HealthSharePlugin.healthshare_password");
            var validateSSL =
                   ConfigManager.Global.Get<string>("InterSystems.HealthSharePlugin.healthshare_validate_ssl");
            bool vssl = !string.IsNullOrEmpty(validateSSL);
            ScanPointClient scanPointClient = null;
            try
            {
                scanPointClient = new ScanPointClient(registryUrl, username, password,vssl);
            }
            catch (Exception e)
            {
                message = e.Message;
                {
                    if (!string.IsNullOrEmpty(ScanPointClient.SSLError))
                    {
                        message += " " + ScanPointClient.SSLError;
                    }
                }
            }
            return message;
        }
        void IHttpHandler.Process(IHttpRequest request, IHttpResponse response, IHttpSession session)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("<h2>HealthShare Plugin Configuration</h2>");
            sb.Append("<p>Welcome to the InterSystems Panasonic HealthShare Plugin&copy;</p>");
            sb.Append("<p>In order to scan documents directly into HealthShare" +
                    " you must complete the following configuration settings. For more " +
                    "information visit " +
                    "<a href=\"http://intersystems.com/ihe-scanners\">" +
                        "http://intersystems.com/ihe-scanners</a></p><br/>");

            if ((request.Method == "GET") && (request.UriPath == "/HealthSharePlugin?test"))
            {
                var message = TestConfig();
                sb.Append("<script type='text/javascript'>alert('" + message + "');</script>");
            }
            if (request.Method == "POST")
            {
              
                // Write some data in the config
                lock (ConfigManager.Global)
                {
                    // always set this to empty - if it's checked it will be in the form
                    // otherwise won't be in the form's items.
                    ConfigManager.Global.Set<string>("InterSystems.HealthSharePlugin.healthshare_validate_ssl", string.Empty);
                    System.Collections.IEnumerator enumerator = request.Form.GetEnumerator();  
                    while ( enumerator.MoveNext() )
                    {
                        HttpInputItem item = (HttpInputItem) enumerator.Current;
                        string field = item.Name;
                        //var value = request.Form[field];
                        string value = item.Value;
                        Debug.WriteLine(field + "=" + value);
                        ConfigManager.Global.Set<string>("InterSystems.HealthSharePlugin." + field, 
                            value);
                    }
                }
            }
            else
            {
                // Retrieve data from config
                /*
                foreach(var field in this.configFields) {

                    string value = ConfigManager.Global.Get<string>(
                        "InterSystems.HealthSharePlugin." + field, this.configDefaults[field]);

                    if (value != null)
                    {
                        sb.Append(field + "=" + value + "<br/>");
                    }
                    else {
                        sb.Append("<p>"+field+": <b>NOT DEFINED!</b></p>");
                    }
                }
                 * */
            }

            

            foreach (var field in configFields ) {
                string value = ConfigManager.Global.Get<string>(
                        "InterSystems.HealthSharePlugin." + field, this.configDefaults[field]);
                if (field.Equals("healthshare_validate_ssl"))
                {
                    continue;
                }
                var displayField = field.Replace('_', ' ');
                TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
                var title = textInfo.ToTitleCase(displayField); //War And Peace
                sb.Append("<label for="+field+">"+title+": </label>" +
                      "<input type=text name='"+field+"' id='"+field+
                      "' value='"+value+"' " +
                      "' placeholder='"+value+"'/>");
                sb.Append("<br/>");
            
            }
            string vssl = ConfigManager.Global.Get<string>(
                      "InterSystems.HealthSharePlugin.healthshare_validate_ssl", this.configDefaults["healthshare_validate_ssl"]);
            var checkedSSL = "checked";
            if (string.IsNullOrEmpty(vssl))
            {
                checkedSSL = "";        
            }
            
            sb.Append("<label for=healthshare_validate_ssl>Validate SSL Certificate:</label>");
            sb.Append("<input type=checkbox name='healthshare_validate_ssl' id='healthshare_validate_ssl' " +
                        "value='1' " + checkedSSL + " />");
            sb.Append("<br/><input type='submit'>");

            sb.Append("<br/><br/>");
            sb.Append("<h2><a href=\"?test\">Test Connection to HealthShare</a></h2>");
            WebConfigServer.SendPage(response,
                                     "MyWebConfigPage",
                                     sb.ToString());
        }

        #endregion
    }
}
