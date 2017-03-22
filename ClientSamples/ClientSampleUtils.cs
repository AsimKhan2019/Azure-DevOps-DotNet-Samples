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

namespace Vsts.ClientSamples
{

    /// <summary>
    /// Utilities for discovering client samples.
    /// </summary>
    public static class ClientSampleUtils
    {
        public static Dictionary<ClientSample, IEnumerable<RunnableClientSampleMethod>> GetRunnableMethods(string area = null, string resource = null)
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
    }

    public class RunnableClientSampleMethod : ClientSampleMethodInfo
    {
        public MethodBase MethodBase { get; set; }
    }

  

}
