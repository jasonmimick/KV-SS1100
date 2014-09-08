using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InterSystems.IHE.Client;
using Panasonic.KV_SS1100.API.Config;
namespace IHEPlugin
{
    class IHEClientUtils
    {
        /// <summary>
        /// Decorate an IHE request with configuration (user/password)
        /// stuff
        /// </summary>
        /// <param name="request"></param>
        internal static void ConfigureRequest(IHERequest request)
        {
            request.HttpUserName = ConfigManager.Global.Get<string>("InterSystems.IHEPlugin.http_username");
            request.HttpPassword = ConfigManager.Global.Get<string>("InterSystems.IHEPlugin.http_password");
            request.WSSecurityUserName = ConfigManager.Global.Get<string>("InterSystems.IHEPlugin.ws_security_username");
            request.WSSecurityPassword = ConfigManager.Global.Get<string>("InterSystems.IHEPlugin.ws_security_password");

        }

        internal static string PDQEndpoint()
        {
            var p= ConfigManager.Global.Get<String>("InterSystems.IHEPlugin.hie_pdq_endpoint");
            return p;
        }
        internal static string XDSbEndpoint()
        {
            return ConfigManager.Global.Get<String>("InterSystems.IHEPlugin.hie_xdsb_endpoint");
        }
    }
}
