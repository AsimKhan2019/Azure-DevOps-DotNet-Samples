using System;
using System.Collections.Generic;
using Microsoft.TeamFoundation.TestManagement.WebApi;
using Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using SuiteEntry = Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi.SuiteEntry;

namespace Microsoft.Azure.DevOps.ClientSamples.Test
{
    [ClientSample(TestPlanResourceIds.AreaName, TestPlanResourceIds.TestCaseProjectLocationIdString)]
    public class TestCaseSample : ClientSample
    {
        [ClientSampleMethod]
        public void DeleteTestCase()
        {
            string projectName = ClientSampleHelpers.FindAnyProject(this.Context).Name;
            int testCaseId = this._getTestCaseId();

            // Get a testplan client instance
            VssConnection connection = Context.Connection;
            TestPlanHttpClient testPlanClient = connection.GetClient<TestPlanHttpClient>();
            
            //Delete a test case
            testPlanClient.DeleteTestCaseAsync(projectName, testCaseId).SyncResult();
        }

        //Dummy data
        //Edit this for use.
        private int _getTestCaseId()
        {
            return 99999999;
        }
    }
}
