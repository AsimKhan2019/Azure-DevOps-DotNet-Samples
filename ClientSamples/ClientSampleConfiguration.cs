using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.Services.Common;

namespace VstsSamples.Client
{
    /// <summary>
    /// Configuration data for client samples. Includes the target URL, credentials, and any other properties.
    /// </summary>
    public class ClientSampleConfiguration
    {
        public VssCredentials Credentials { get; private set; }

        public Uri Url { get; private set; }

        protected Dictionary<String, Object> Properties { get; set; } = new Dictionary<String, Object>();

        public ClientSampleConfiguration(Uri url): this(url, new VssCredentials())
        {
            
        }

        public ClientSampleConfiguration(Uri url, VssCredentials credentials)
        {
            this.Url = url;
            this.Credentials = credentials;
        }

        public T Get<T>(string name, T defaultValueIfMissing)
        {
            T result;
            if (Properties.TryGetValue<T>(name, out result))
            {
                return result;
            }
            else
            {
                return defaultValueIfMissing;
            }
        }

        public void Set<T>(string name, T value)
        {
            Properties[name] = value;
        }

        /// <summary>
        /// Creates a new client sample configuration from the supplied Team Services account name and personal access token.
        /// </summary>
        /// <param name="accountName"></param>
        /// <param name="personalAccessToken"></param>
        /// <returns></returns>
        public static ClientSampleConfiguration NewInstanceFromAccountName(string accountName, string personalAccessToken)
        {
            return new ClientSampleConfiguration(
                new Uri(String.Format(s_accountUrlPattern, accountName)),
                new VssBasicCredential("pat", personalAccessToken));
        }

        private static readonly string s_accountUrlPattern = "http://{0}.visualstudio.com";
    }
}
