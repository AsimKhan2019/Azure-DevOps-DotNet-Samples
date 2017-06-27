using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System.Net.Http;
using Microsoft.VisualStudio.Services.Client;

namespace Microsoft.TeamServices.Samples.Client
{
    /// <summary>
    /// Configuration data for client samples. Includes the target URL, credentials, and any other properties.
    /// </summary>
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
}
