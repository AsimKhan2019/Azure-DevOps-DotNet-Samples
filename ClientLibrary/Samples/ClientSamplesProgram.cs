using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;

namespace Microsoft.Azure.DevOps.ClientSamples
{
    /// <summary>
    /// Simple program that uses reflection to discovery/run client samples.
    /// </summary>
    public class Program
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
            Console.WriteLine("Required arguments:");
            Console.WriteLine("");
            Console.WriteLine("  /url:{value}         URL of the account/collection to run the samples against.");
            Console.WriteLine("  /area:{value}        API area to run the client samples for. Use * to include all areas.");
            Console.WriteLine("  /resource:{value}    API resource to run the client samples for. Use * to include all resources.");
            Console.WriteLine("");
            Console.WriteLine("Optional arguments:");
            Console.WriteLine("  /outputPath:{value}  Path for saving HTTP request/response files. If not specified, files are not generated.");
            Console.WriteLine("");
            Console.WriteLine("Examples:");
            Console.WriteLine("");
            Console.WriteLine("  Microsoft.Azure.DevOps.ClientSamples.exe /url:https://dev.azure.com/fabrikam /area:* /resource:*");
            Console.WriteLine("  Microsoft.Azure.DevOps.ClientSamples.exe /url:https://dev.azure.com/fabrikam /area:* /resource:* /outputPath:\"c:\\temp\\output results\"");
            Console.WriteLine("  Microsoft.Azure.DevOps.ClientSamples.exe /url:https://dev.azure.com/fabrikam /area:wit /resource:*");            
            Console.WriteLine("  Microsoft.Azure.DevOps.ClientSamples.exe /url:https://dev.azure.com/fabrikam /area:git /resource:pullrequests /outputPath:.\\output");
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

       /// <summary>
    /// Utilities for discovering client samples.
    /// </summary>
    public static class ClientSampleUtils
    {
        public static Dictionary<ClientSample, IEnumerable<RunnableClientSampleMethod>> GetRunnableClientSampleMethods(string area = null, string resource = null)
        {
            Dictionary<ClientSample, IEnumerable<RunnableClientSampleMethod>> results = new Dictionary<ClientSample, IEnumerable<RunnableClientSampleMethod>>();

            CompositionContainer container = new CompositionContainer(new AssemblyCatalog(Assembly.GetExecutingAssembly()));

            IEnumerable<Lazy<ClientSample>> samples = container.GetExports<ClientSample>();
            foreach (Lazy<ClientSample> cs in samples)
            {
                try
                {
                    Type csType = cs.Value.GetType();
                    ClientSampleAttribute csAttr = csType.GetCustomAttribute<ClientSampleAttribute>();

                    List<RunnableClientSampleMethod> runnableMethods = new List<RunnableClientSampleMethod>();

                    foreach (MethodInfo m in csType.GetMethods())
                    {
                        ClientSampleMethodAttribute[] attrs = (ClientSampleMethodAttribute[])m.GetCustomAttributes(typeof(ClientSampleMethodAttribute), false);
                        foreach (var ma in attrs)
                        {
                            RunnableClientSampleMethod runnableMethod = new RunnableClientSampleMethod();

                            if (string.IsNullOrEmpty(ma.Area))
                            {
                                runnableMethod.Area = csAttr.Area;
                            }
                            else
                            {
                                runnableMethod.Area = ma.Area;
                            }

                            if (string.IsNullOrEmpty(ma.Resource))
                            {
                                runnableMethod.Resource = csAttr.Resource;
                            }
                            else
                            {
                                runnableMethod.Resource = ma.Resource;
                            }

                            if (!string.IsNullOrEmpty(runnableMethod.Area) && !string.IsNullOrEmpty(runnableMethod.Resource))
                            {
                                runnableMethod.MethodBase = m;
                                runnableMethods.Add(runnableMethod);
                            }
                        }
                    }

                    if (runnableMethods.Any())
                    {
                        if (!String.IsNullOrEmpty(area))
                        {
                            runnableMethods = runnableMethods.FindAll(
                                rcsm =>
                                {
                                    return string.Equals(area, rcsm.Area, StringComparison.InvariantCultureIgnoreCase) &&
                                        (resource == null || string.Equals(resource, rcsm.Resource, StringComparison.InvariantCultureIgnoreCase));
                                }
                            );
                        }

                        results.Add(cs.Value, runnableMethods);
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex.Message);
                }
            }

            return results;
        }

        public static void RunClientSampleMethods(Uri connectionUrl, VssCredentials credentials, string area = null, string resource = null, DirectoryInfo outputPath = null)
        {
            if (area == "*")
            {
                area = null;
            }

            if (resource == "*")
            {
                resource = null;
            }

            Dictionary<ClientSample, IEnumerable<RunnableClientSampleMethod>> runnableMethodsBySample = GetRunnableClientSampleMethods(area, resource);

            if (!runnableMethodsBySample.Any())
            {
                Console.WriteLine("No samples found to run.");
            }
            else
            {
                ClientSampleContext context = new ClientSampleContext(connectionUrl, credentials);

                Console.WriteLine("Start running client samples...");
                Console.WriteLine("");
                Console.WriteLine("  URL     : {0}", connectionUrl);
                Console.WriteLine("  Area    : {0}", (area == null ? "(all)" : area));
                Console.WriteLine("  Resource: {0}", (resource == null ? "(all)" : resource));
                Console.WriteLine("  Output  : {0}", (outputPath == null ? "(disabled)" : outputPath.FullName));
                Console.WriteLine("");

                // Make sure we can connect before running the samples
                context.Connection.ConnectAsync().SyncResult();

                context.SetValue<DirectoryInfo>(ClientSampleHttpLogger.PropertyOutputFilePath, outputPath);

                foreach (var item in runnableMethodsBySample)
                {
                    ClientSample clientSample = item.Key;
                    clientSample.Context = context;

                    foreach (var runnableMethod in item.Value)
                    {
                        try
                        {
                            context.Log("+------------------------------------------------------------------------------+");
                            context.Log("| {0} |", String.Format("{0}/{1}", runnableMethod.MethodBase.Name, runnableMethod.MethodBase.DeclaringType.Name).PadRight(76));
                            context.Log("|                                                                              |");
                            context.Log("| API: {0} |", String.Format("{0}/{1}", runnableMethod.Area, runnableMethod.Resource).PadRight(71));
                            context.Log("+------------------------------------------------------------------------------+");
                            context.Log("");

                            // Set these so the HTTP logger has access to them when it needs to write the output
                            ClientSampleContext.CurrentRunnableMethod = runnableMethod;
                            ClientSampleContext.CurrentContext = context;

                            // Reset suppression and operation name (don't want these values carrying over to the next sample method call)
                            ClientSampleHttpLogger.SetSuppressOutput(context, false);
                            ClientSampleHttpLogger.ResetOperationName(context);

                            object result = runnableMethod.MethodBase.Invoke(clientSample, null);
                            // if the runnable method is async, then wait for the completion
                            if (result is Task resultTask)
                            {
                                resultTask.SyncResult();
                            }
                        }
                        catch (Exception ex)
                        {
                            //the innermost exception is the interesting one
                            while (ex.InnerException != null) ex = ex.InnerException;
                            Console.WriteLine("FAILED! With exception: " + ex.Message);
                        }
                        finally
                        {
                            context.Log("");
                        }
                    }
                }
            }
        }
    }

    public class RunnableClientSampleMethod : ClientSampleMethodInfo
    {
        public MethodBase MethodBase { get; set; }
    }

}