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
    /// Interface representing a client sample method. Provides a way to discover client samples for a particular area, resource, or operation.
    /// </summary>
    public interface IClientSampleMethodInfo
    {
        string Area { get; }

        string Resource { get; }

        string Operation { get; }
    }


    public class ClientSampleMethodInfo : IClientSampleMethodInfo
    {
        public string Area { get; set; }

        public string Resource { get; set; }

        public string Operation { get; set; }
    }

    /// <summary>
    /// Attribute applied to all client samples. Optionally indicates the API "area" and/or "resource" the sample is associatd with.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class ClientSampleAttribute : ExportAttribute
    {
        public string Area { get; private set; }

        public string Resource { get; private set; }

        public ClientSampleAttribute(String area = null, String resource = null) : base(typeof(ClientSample))
        {
            this.Area = area;
            this.Resource = resource;
        }
    }

    /// <summary>
    /// Attribute applied to methods within a client sample. Allow overriding the area or resource of the containing client sample.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class ClientSampleMethodAttribute : Attribute, IClientSampleMethodInfo
    {
        public string Area { get; internal set; }

        public string Resource { get; internal set; }

        public string Operation { get; internal set; }

        public ClientSampleMethodAttribute(String area = null, String resource = null, String operation = null)
        {
            this.Area = area;
            this.Resource = resource;
            this.Operation = operation;
        }
    }




}