using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CrocoManager
{
    public class ConfigLoader
    {
        private static IConfiguration? _configuration;

        public static IConfiguration Configuration
        {
            get
            {
                if (_configuration == null)
                {
                    var assembly = Assembly.GetExecutingAssembly();

                    var resourceNames = assembly.GetManifestResourceNames();

                    foreach (var name in resourceNames)
                    {
                        System.Diagnostics.Debug.WriteLine($"Resource: {name}");
                    }

                    var stream = assembly.GetManifestResourceStream("CrocoManager.appsettings.json");

                    if (stream == null)
                    {
                        throw new FileNotFoundException("appsettings.json not found as embedded resource");
                    }

                    _configuration = new ConfigurationBuilder()
                        .AddJsonStream(stream)
                        .Build();
                }
                return _configuration;
            }
        }
    }
}
