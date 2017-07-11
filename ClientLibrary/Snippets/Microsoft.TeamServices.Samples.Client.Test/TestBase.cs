using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Profile.Client;
using Microsoft.VisualStudio.Services.WebApi;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.TeamServices.Samples.Client.Tests.Integration
{
    public class TestBase<T> where T : ClientSample, new()
    {
        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        internal T ClientSample { get; private set; }

        internal VssConnection Connection
        {
            get
            {
                return ClientSample.Context.Connection;
            }
            private set
            {
                Connection = value;
            }
        }

        [TestInitialize]
        public void Initialize()
        {
            string connectionUrl = TestContext.Properties["connectionUrl"] as string;
            string userName = TestContext.Properties["password"] as string;
            string password = TestContext.Properties["password"] as string;

            ClientSampleContext context = new ClientSampleContext(new Uri(connectionUrl), new VssBasicCredential(userName, password));

            ClientSample = new T();
            ClientSample.Context = context;
        }

        protected Guid GetCurrentUserId()
        {
            return Connection.AuthorizedIdentity.Id;
        }
    }
}
