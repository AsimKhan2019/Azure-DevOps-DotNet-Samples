using System;
using System.Reflection;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.Net.Http;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json.Linq;
using System.IO;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;

namespace Microsoft.TeamServices.Samples.Client
{

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
                                runnableMethod.Area = char.ToUpper(runnableMethod.Area[0]) + runnableMethod.Area.Substring(1);
                                runnableMethod.Resource = char.ToUpper(runnableMethod.Resource[0]) + runnableMethod.Resource.Substring(1);

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
                    // Console.WriteLine(ex.Message);
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

            if (runnableMethodsBySample.Any())
            {
                ClientSampleContext context = new ClientSampleContext(connectionUrl, credentials);

                Console.WriteLine("Start running client samples...");
                Console.WriteLine("");
                Console.WriteLine("  URL     : {0}", connectionUrl);
                Console.WriteLine("  Area    : {0}", (area  == null ? "(all)" : area));
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

                            runnableMethod.MethodBase.Invoke(clientSample, null);
                        }
                        catch (Exception ex)
                        {
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
