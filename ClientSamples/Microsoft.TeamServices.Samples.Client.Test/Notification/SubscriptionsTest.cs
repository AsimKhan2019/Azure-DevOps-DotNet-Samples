using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualStudio.Services.Notifications.WebApi;
using Microsoft.TeamServices.Samples.Client.Notification;

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
