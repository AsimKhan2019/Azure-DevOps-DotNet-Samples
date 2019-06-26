using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi;
using Microsoft.VisualStudio.Services.WebApi;

namespace Microsoft.Azure.DevOps.ClientSamples.Test
{
    [ClientSample(TestPlanResourceIds.AreaName, TestPlanResourceIds.TestSuiteByCaseString)]
    public class TestSuiteByCaseSample : ClientSample
    {
        [ClientSampleMethod]
        public List<TestSuite> GetTestSuitesByCase()
        {
            string projectName = ClientSampleHelpers.FindAnyProject(this.Context).Name;
            int testCaseId = this._getTestCaseId();

            // Get a testplan client instance
            VssConnection connection = Context.Connection;
            TestPlanHttpClient testPlanClient = connection.GetClient<TestPlanHttpClient>();
                        
            
            // Get Test Suites for a test case
            List<TestSuite> testSuites = testPlanClient.GetSuitesByTestCaseIdAsync(testCaseId).Result;


            foreach (TestSuite testSuite in testSuites)
            {
                Context.Log("{0} {1}", testSuite.Id.ToString().PadLeft(6), testSuite.Name);
            }
            return testSuites;
        }


        //Dummy data
        //Edit this for use.
        private int _getTestCaseId()
        {
            return 99999999;
        }
    }
}
