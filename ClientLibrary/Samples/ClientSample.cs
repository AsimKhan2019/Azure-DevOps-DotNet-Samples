using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Net.Http;
using System.Runtime.Serialization;

using Microsoft.VisualStudio.Services.Client;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;

namespace Microsoft.Azure.DevOps.ClientSamples
{
    /// <summary>
    /// Base class that all client samples extend from.
    /// </summary>
    [InheritedExport]
    public abstract class ClientSample
    {
        public ClientSampleContext Context { get; set; }

    }

    public class ClientSampleContext
    {
        protected VssCredentials Credentials { get; private set; }

        protected Uri Url { get; private set; }

        protected Dictionary<String, Object> Properties { get; set; } = new Dictionary<String, Object>();

        private VssConnection _connection;

        public VssConnection Connection
        {
            get
            {
                if (_connection == null)
                {
                    ClientSampleHttpLogger loggerHandler = new ClientSampleHttpLogger();

                    VssHttpMessageHandler vssHandler = new VssHttpMessageHandler(
                        Credentials, 
                        VssClientHttpRequestSettings.Default.Clone());

                    _connection = new VssConnection(
                        Url, 
                        vssHandler, 
                        new DelegatingHandler[] { loggerHandler });
                }

                return this._connection;
            }
            private set
            {
                _connection = value;
            }
        }

        public ClientSampleContext(Uri url): this(url, null)
        {            
        }

        public ClientSampleContext(Uri url, VssCredentials credentials)
        {
            this.Url = url;

            if (credentials == null)
            {
                this.Credentials = new VssClientCredentials();
            }
            else
            {
                this.Credentials = credentials;
            }
        }

        public ClientSampleContext(VssConnection connection)
        {
            this.Connection = connection;
        }

        public T GetValue<T>(string name)
        {
            return (T)Properties[name];
        }

        public bool TryGetValue<T>(string name, out T result)
        {
            return Properties.TryGetValue<T>(name, out result);
        }

        public void SetValue<T>(string name, T value)
        {
            Properties[name] = value;
        }

        public void RemoveValue(string name)
        {
            Properties.Remove(name);
        }

        public void Log(String message)
        {
            this.Log(message, null);
        }

        public void Log(String message, params object[] args)
        {
            System.Console.WriteLine(message, args);
        }

        public static RunnableClientSampleMethod CurrentRunnableMethod { get; set; }

        public static ClientSampleContext CurrentContext { get; set; }

        /// <summary>
        /// Creates a new client sample configuration from the supplied Team Services account name and personal access token.
        /// </summary>
        /// <param name="accountName"></param>
        /// <param name="personalAccessToken"></param>
        /// <returns></returns>
        public static ClientSampleContext NewInstanceFromAccountName(string accountName, string personalAccessToken)
        {
            return new ClientSampleContext(
                new Uri(String.Format(s_accountUrlPattern, accountName)),
                new VssBasicCredential("pat", personalAccessToken));
        }

        private static readonly string s_accountUrlPattern = "http://{0}.visualstudio.com";
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


    [DataContract]
    public class ClientSampleMethodInfo : IClientSampleMethodInfo
    {
        [DataMember(EmitDefaultValue = false, Name = "x-ms-vss-area")]
        public string Area { get; set; }

        [DataMember(EmitDefaultValue = false, Name = "x-ms-vss-resource")]
        public string Resource { get; set; }

        [DataMember(EmitDefaultValue = false, Name = "x-ms-vss-operation")]
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