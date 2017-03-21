using System;
using System.Reflection;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Services.WebApi;
using System.Net.Http;
using Microsoft.VisualStudio.Services.Common;

namespace VstsSamples.Client
{
    /// <summary>
    /// Base class that all client samples extend from.
    /// </summary>
    public abstract class ClientSample
    {
        public ClientSampleConfiguration Configuration { get; set; }

        private VssConnection _connection;

        public VssConnection Connection
        {
            get
            {
                if (_connection == null)
                {
                    ClientSampleHttpLogger loggerHandler = new ClientSampleHttpLogger();

                    VssHttpMessageHandler vssHandler = new VssHttpMessageHandler(Configuration.Credentials, VssClientHttpRequestSettings.Default.Clone());

                    _connection = new VssConnection(Configuration.Url, vssHandler, new DelegatingHandler[] { loggerHandler });
                }

                return this._connection;
            }
            private set
            {
                _connection = value;
            }
        }

        [ImportingConstructor]
        public ClientSample()
        {
        }

        public ClientSample(ClientSampleConfiguration configuration)
        {
            this.Configuration = configuration;
        }

        protected void Log(String message)
        {
            this.Log(message, null);
        }

        protected void Log(String message, params object[] args)
        {
            System.Console.WriteLine(message, args);
        }

    }

    /// <summary>
    /// Utilities for discovering client samples.
    /// </summary>
    public static class ClientSampleMetadataUtils
    {
        public static IEnumerable<IClientSampleMethod> GetClientSampleMethods(string area = null)
        {
            List<IClientSampleMethod> methods = new List<IClientSampleMethod>();

            CompositionContainer container = new CompositionContainer(new AssemblyCatalog(Assembly.GetExecutingAssembly()));
            IEnumerable<Lazy<ClientSample>> samples = container.GetExports<ClientSample>();

            foreach (Lazy<ClientSample> cs in samples)
            {
                Type csType = cs.Value.GetType();

                ClientSampleAttribute csAttr = csType.GetCustomAttribute<ClientSampleAttribute>();

                foreach (MethodInfo m in csType.GetMethods())
                {
                    ClientSampleMethodAttribute[] attrs = (ClientSampleMethodAttribute[])m.GetCustomAttributes(typeof(ClientSampleMethodAttribute), false);
                    foreach (var ma in attrs)
                    {
                        if (string.IsNullOrEmpty(ma.Area))
                        {
                            ma.Area = csAttr.Area;
                        }

                        if (string.IsNullOrEmpty(ma.Resource))
                        {
                            ma.Resource = csAttr.Resource;
                        }

                        if (!string.IsNullOrEmpty(ma.Area) && !string.IsNullOrEmpty(ma.Resource) && !string.IsNullOrEmpty(ma.Operation))
                        {
                            methods.Add(ma);
                        }
                    }
                }
            }

            if (!String.IsNullOrEmpty(area))
            {
                methods = methods.FindAll(csm => { return String.Equals(area, csm.Area, StringComparison.OrdinalIgnoreCase); });
            }

            return methods;
        }

    }


}