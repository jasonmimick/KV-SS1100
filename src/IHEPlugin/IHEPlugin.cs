using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using Panasonic.KV_SS1100.API;
using Panasonic.KV_SS1100.API.AppButton;
using Panasonic.KV_SS1100.API.WebConfig;

namespace PluginExample.Properties
{
    public class MyPlugin : IPlugin
    {
        public void OnLoad()
        {
            // Register configuration page for the web
            WebConfigServer.RegisterHttpHandler("My Plugin Config", "/MyPlugin", new MyConfigHttpHandler());

            // Register application button
            AppButtonManager.AddAppButton(new MyStyledButton("My Plugin",
                                                             "My Plugin",
                                                             "Description for My Plugin"));
        }

        public void OnUnload()
        {
        }
    }
}
