using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Vsts.ClientSamples.Tests.Integration;
using Vsts.ClientSamples.Notification;
using Microsoft.VisualStudio.Services.Notifications.WebApi;

namespace Microsoft.TeamServices.Samples.Client.Tests.Integration.Notification
{
    [TestClass]
    public class SubscriptionTests : TestBase<SubscriptionsSample>
    {
        public SubscriptionTests()
        {
        }

        [TestMethod]
        public void Test_QueryCreateUpdateDeleteSubscription()
        {
            NotificationSubscription sub = ClientSample.CreateUpdateDeleteSubscription();

            Assert.AreEqual("Someone is waiting on one of my pull requests", sub.Description);
            Assert.AreEqual(SubscriptionStatus.PendingDeletion, sub.Status);
            Assert.AreEqual(GetCurrentUserId(), Guid.Parse(sub.Subscriber.Id));            
        }
    }
}
