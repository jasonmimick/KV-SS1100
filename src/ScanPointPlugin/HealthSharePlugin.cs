using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using Panasonic.KV_SS1100.API;
using Panasonic.KV_SS1100.API.AppButton;
using Panasonic.KV_SS1100.API.WebConfig;
using System.Reflection;

namespace PluginExample.Properties
{
    public class MyPlugin : IPlugin
    {

        public void OnLoad()
        {

            AppDomain.CurrentDomain.AssemblyResolve += (sender, args) =>
            {

                String resourceName = "HealthSharePlugin." +

                   new AssemblyName(args.Name).Name + ".dll";

                using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName))
                {

                    Byte[] assemblyData = new Byte[stream.Length];

                    stream.Read(assemblyData, 0, assemblyData.Length);

                    return Assembly.Load(assemblyData);

                }

            };
            // Register configuration page for the web
            WebConfigServer.RegisterHttpHandler("HealthShare Plugin Config", "/HealthSharePlugin", new MyConfigHttpHandler());
            
            //foo = 4;

            // Register application button
            AppButtonManager.AddAppButton(new MyStyledButton("HealthShare Plugin",
                                                             "HealthShare Plugin",
                                                             "HealthShare to enable scanning documents directly into HealthShare"));
        }

        public void OnUnload()
        {
        }
    }
}
