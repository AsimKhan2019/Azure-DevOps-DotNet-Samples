using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Microsoft.TeamServices.Samples.Client.Runner
{
    /// <summary>
    /// Simple program that uses reflection to discovery/run client samples.
    /// </summary>
    public class Runner
    {

        public static int Main(string[] args)
        {
            if (args.Length == 0)
            {
                ShowUsage();
                return 0;
            } 

            Uri connectionUrl;
            string area, resource;
            DirectoryInfo outputPath;

            try
            {
                CheckArguments(args, out connectionUrl, out area, out resource, out outputPath);
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine(ex.Message);

                ShowUsage();

                return -1;
            }

            try
            {
                ClientSampleUtils.RunClientSampleMethods(connectionUrl, null, area: area, resource: resource, outputPath: outputPath);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed to run the sample: " + ex.Message);
                return 1;
            }
                        
            return 0;
        }
 
        private static void CheckArguments(string[] args, out Uri connectionUrl, out string area, out string resource, out DirectoryInfo outputPath)
        {
            connectionUrl = null;
            area = null;
            resource = null;
            outputPath = null;

            Dictionary<string, string> argsMap = new Dictionary<string, string>();
            foreach (var arg in args)
            {
                if (arg[0] == '/' && arg.IndexOf(':') > 1)
                {
                    string key = arg.Substring(1, arg.IndexOf(':') - 1);
                    string value = arg.Substring(arg.IndexOf(':') + 1);
                    
                    switch (key)
                    {
                        case "url":
                            connectionUrl = new Uri(value);
                            break;
                        case "area":
                            area = value;
                            // TODO validate supplied area
                            break;
                        case "resource":
                            resource = value;
                            // TODO validate supplied resource
                            break;
                        case "outputPath":
                            outputPath = new DirectoryInfo(value);
                            break;
                        default:
                            throw new ArgumentException("Unknown argument", key);
                    }
                }
            }

            if (connectionUrl == null || area == null || resource == null)
            {
                throw new ArgumentException("Missing required arguments");
            }                     
        }

        private static void ShowUsage() {
            Console.WriteLine("Runs the client samples on a Team Services account or Team Foundation Server instance.");
            Console.WriteLine("");
            Console.WriteLine("!!WARNING!! Some samples are destructive. Always run on a test account or collection.");
            Console.WriteLine("");
            Console.WriteLine("Arguments:");
            Console.WriteLine("");
            Console.WriteLine("  /url:{value}         URL of the account/collection to run the samples against.");
            Console.WriteLine("  /area:{value}        API area to run the client samples for. Use * to include all areas.");
            Console.WriteLine("  /resource:{value}    API resource to run the client samples for. Use * to include all resources.");
            Console.WriteLine("  /outputPath:{value}  Path for saving HTTP request/response files. If not specified, files are not generated.");
            Console.WriteLine("");
            Console.WriteLine("Examples:");
            Console.WriteLine("");
            Console.WriteLine("  Runner.exe /url:https://fabrikam.visualstudio.com /area:* /resource:*");
            Console.WriteLine("  Runner.exe /url:https://fabrikam.visualstudio.com /area:* /resource:* /outputPath:\"c:\\temp\\output results\"");
            Console.WriteLine("  Runner.exe /url:https://fabrikam.visualstudio.com /area:wit /resource:*");            
            Console.WriteLine("  Runner.exe /url:https://fabrikam.visualstudio.com /area:git /resource:pullrequests /outputPath:.\\output");
            Console.WriteLine("");

            Dictionary<ClientSample, IEnumerable<RunnableClientSampleMethod>> runnableMethodsBySample = ClientSampleUtils.GetRunnableClientSampleMethods();

            HashSet<string> areas = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase);

            foreach(var kvp in runnableMethodsBySample)
            {
                foreach (var rcsm in kvp.Value)
                {
                    areas.Add(rcsm.Area.ToLower());
                }
            }

            Console.WriteLine("Available areas: " + String.Join(",", areas.ToArray<string>()));
        }

    }

}