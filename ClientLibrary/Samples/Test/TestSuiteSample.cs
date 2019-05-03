using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi;
using Microsoft.VisualStudio.Services.WebApi;

namespace Microsoft.Azure.DevOps.ClientSamples.Test
{
    [ClientSample(TestPlanResourceIds.AreaName, TestPlanResourceIds.TestSuiteProjectLocationIdString)]
    public class TestSuiteSample : ClientSample
    {
        [ClientSampleMethod]
        public List<TestSuite> GetTestSuitesForPlan()
        {
            string projectName = ClientSampleHelpers.FindAnyProject(this.Context).Name;
            int testPlanId = this._getTestPlanId();

            // Get a testplan client instance
            VssConnection connection = Context.Connection;
            TestPlanHttpClient testPlanClient = connection.GetClient<TestPlanHttpClient>();

            // Get Test Suites
            List<TestSuite> testSuites = testPlanClient.GetTestSuitesForPlanAsync(projectName, testPlanId, SuiteExpand.Children | SuiteExpand.DefaultTesters).Result;

            foreach (TestSuite testSuite in testSuites)
            {
                Context.Log("{0} {1}", testSuite.Id.ToString().PadLeft(6), testSuite.Name);
            }
            return testSuites;
        }

        [ClientSampleMethod]
        public List<TestSuite> GetTestSuitesAsTreeView()
        {
            string projectName = ClientSampleHelpers.FindAnyProject(this.Context).Name;
            int testPlanId = this._getTestPlanId();

            // Get a testplan client instance
            VssConnection connection = Context.Connection;
            TestPlanHttpClient testPlanClient = connection.GetClient<TestPlanHttpClient>();

            // Get Test Suites
            List<TestSuite> testSuites = testPlanClient.GetTestSuitesForPlanAsync(projectName, testPlanId, asTreeView: true).Result;

            return testSuites;
        }


        [ClientSampleMethod]
        public TestSuite CreateTestSuite()
        {
            string projectName = ClientSampleHelpers.FindAnyProject(this.Context).Name;
            int testPlanId = this._getTestPlanId();

            // Get a testplan client instance
            VssConnection connection = Context.Connection;
            TestPlanHttpClient testPlanClient = connection.GetClient<TestPlanHttpClient>();

            int parentSuiteId = this._getRootSuiteId();
            TestSuiteCreateParams testSuiteCreateParams = new TestSuiteCreateParams()
            {
                Name = "SubSuite 1.1.1",
                SuiteType = TestSuiteType.StaticTestSuite,
                ParentSuite = new TestSuiteReference()
                {
                    Id = parentSuiteId
                },
                InheritDefaultConfigurations = false,
                DefaultConfigurations = new List<TestConfigurationReference>()
                {
                    new TestConfigurationReference(){Id=1},
                    new TestConfigurationReference(){Id=2}
                }
            };

            // Create Test Suite
            TestSuite suite = testPlanClient.CreateTestSuiteAsync(testSuiteCreateParams, projectName, testPlanId).Result;

            Context.SetValue<TestSuite>("$newSuite", suite);
            Context.Log("{0} {1}", suite.Id.ToString().PadLeft(6), suite.Name);
            return suite;
        }

        [ClientSampleMethod]
        public TestSuite GetTestSuiteById()
        {
            string projectName = ClientSampleHelpers.FindAnyProject(this.Context).Name;
            // Get a testplan client instance
            VssConnection connection = Context.Connection;
            TestPlanHttpClient testPlanClient = connection.GetClient<TestPlanHttpClient>();

            
            int testPlanId = this._getTestPlanId();
            TestSuite newSuite;
            Context.TryGetValue<TestSuite>("$newSuite", out newSuite);
            int id = newSuite.Id;
            if (id != 0)
            {
                // Get Test Suite
                TestSuite suite = testPlanClient.GetTestSuiteByIdAsync(projectName, testPlanId, id, SuiteExpand.Children).Result;

                Context.Log("{0} {1}", suite.Id.ToString().PadLeft(6), suite.Name);
                return suite;
            }
            return null;
        }

        [ClientSampleMethod]
        public TestSuite UpdateTestSuiteParent()
        {
            string projectName = ClientSampleHelpers.FindAnyProject(this.Context).Name;
            // Get a testplan client instance
            VssConnection connection = Context.Connection;
            TestPlanHttpClient testPlanClient = connection.GetClient<TestPlanHttpClient>();


            TestSuiteUpdateParams testSuiteUpdateParams = new TestSuiteUpdateParams()
            {
                ParentSuite = new TestSuiteReference()
                {
                    Id = this._getUpdatedRootSuiteId()
                }
            };

            int testPlanId = this._getTestPlanId();
            TestSuite newSuite;
            Context.TryGetValue<TestSuite>("$newSuite", out newSuite);
            if (newSuite != null)
            {
                int id = newSuite.Id;

                // Update Test Suite
                TestSuite updtaetdTestSuite = testPlanClient.UpdateTestSuiteAsync(testSuiteUpdateParams, projectName, testPlanId, id).Result;

                Context.Log("{0} {1}", updtaetdTestSuite.Id.ToString().PadLeft(6), updtaetdTestSuite.Name);
                return updtaetdTestSuite;
            }
            return null;
        }

        [ClientSampleMethod]
        public TestSuite UpdateTestSuiteProperties()
        {
            string projectName = ClientSampleHelpers.FindAnyProject(this.Context).Name;
            // Get a testplan client instance
            VssConnection connection = Context.Connection;
            TestPlanHttpClient testPlanClient = connection.GetClient<TestPlanHttpClient>();

            TestSuiteUpdateParams testSuiteUpdateParams = new TestSuiteUpdateParams()
            {
                DefaultTesters = new List<IdentityRef>()
                {
                    new IdentityRef()
                    {
                        Id = this._getDefaultTesterIdentity()
                    }
                }
            };

            int testPlanId = this._getTestPlanId();
            TestSuite newSuite;
            Context.TryGetValue<TestSuite>("$newSuite", out newSuite);
            if (newSuite != null)
            {
                int id = newSuite.Id;
                TestSuite updtaetdTestSuite = testPlanClient.UpdateTestSuiteAsync(testSuiteUpdateParams, projectName, testPlanId, id).Result;
                Context.Log("{0} {1}", updtaetdTestSuite.Id.ToString().PadLeft(6), updtaetdTestSuite.Name);
                return updtaetdTestSuite;
            }
            return null;
        }

        [ClientSampleMethod]
        public void DeleteTestSuite()
        {
            string projectName = ClientSampleHelpers.FindAnyProject(this.Context).Name;
            // Get a testplan client instance
            VssConnection connection = Context.Connection;
            TestPlanHttpClient testPlanClient = connection.GetClient<TestPlanHttpClient>();

            int testPlanId = this._getTestPlanId();
            TestSuite newSuite;
            Context.TryGetValue<TestSuite>("$newSuite", out newSuite);
            if (newSuite != null)
            {
                int id = newSuite.Id;

                //Delete Test Suite
                testPlanClient.DeleteTestSuiteAsync(projectName, testPlanId, id).SyncResult();
            }
        }

        //Common dummy data
        //Edit this for use.
        private int _getTestPlanId()
        {
            return 99999999;
        }

        private int _getRootSuiteId()
        {
            return 99999999;
        }

        private int _getUpdatedRootSuiteId()
        {
            return 99999999;
        }

        private string _getDefaultTesterIdentity()
        {
            return "0fdbdad8-6afb-6149-9af9-c0a216137d1d";
        }
    }
}
