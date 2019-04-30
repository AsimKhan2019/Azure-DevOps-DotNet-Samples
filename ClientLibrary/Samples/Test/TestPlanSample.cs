using System;
using System.Collections.Generic;
using Microsoft.TeamFoundation.TestManagement.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using TestPlanWebApi = Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi;

namespace Microsoft.Azure.DevOps.ClientSamples.Test
{
    [ClientSample(TestPlanResourceIds.AreaName, TestPlanResourceIds.TestPlanLocationIdString)]
    public class TestPlanSample : ClientSample
    {
        [ClientSampleMethod]
        public List<TestPlanWebApi.TestPlan> GetTestPlans()
        {
            string projectName = ClientSampleHelpers.FindAnyProject(this.Context).Name;

            // Get a testplan client instance
            VssConnection connection = Context.Connection;
            TestPlanHttpClient testPlanClient = connection.GetClient<TestPlanHttpClient>();


            // Get test plans
            List<TestPlanWebApi.TestPlan> plans = testPlanClient.GetTestPlansAsync(projectName).Result;


            foreach (TestPlanWebApi.TestPlan plan in plans)
            {
                Context.Log("{0} {1}", plan.Id, plan.Name);
            }
            return plans;
        }

        [ClientSampleMethod]
        public List<TestPlanWebApi.TestPlan> GetActiveTestPlansFilteredByOwner()
        {
            string projectName = ClientSampleHelpers.FindAnyProject(this.Context).Name;

            // Get a testplan client instance
            VssConnection connection = Context.Connection;
            TestPlanHttpClient testPlanClient = connection.GetClient<TestPlanHttpClient>();


            // Get test plans
            List<TestPlanWebApi.TestPlan> plans = testPlanClient.GetTestPlansAsync(projectName, includePlanDetails: true, filterActivePlans: true, owner: "0fdbdad8-6afb-6149-9af9-c0a216137d1d").Result;


            foreach (TestPlanWebApi.TestPlan plan in plans)
            {
                Context.Log("{0} {1}", plan.Id, plan.Name);
            }
            return plans;
        }

        [ClientSampleMethod]
        public TestPlanWebApi.TestPlan CreateTestPlanWithAreaPathAndIteration()
        {
            string projectName = ClientSampleHelpers.FindAnyProject(this.Context).Name;

            // Get a testplan client instance
            VssConnection connection = Context.Connection;
            TestPlanHttpClient testPlanClient = connection.GetClient<TestPlanHttpClient>();
            
            TestPlanWebApi.TestPlanCreateParams testPlanCreateParams = new TestPlanWebApi.TestPlanCreateParams()
            {
                Name = "newCreatedPlan1",
                AreaPath = this._getArea(),
                Iteration = this._getIteration()
            };


            // create a test plan
            TestPlanWebApi.TestPlan plan = testPlanClient.CreateTestPlanAsync(testPlanCreateParams, projectName).Result;


            Context.SetValue<TestPlanWebApi.TestPlan>("$newPlan1", plan);
            Context.Log("{0} {1}", plan.Id, plan.Name);
            return plan;
        }

        [ClientSampleMethod]
        public TestPlanWebApi.TestPlan CreateTestPlan()
        {
            string projectName = ClientSampleHelpers.FindAnyProject(this.Context).Name;

            // Get a testplan client instance
            VssConnection connection = Context.Connection;
            TestPlanHttpClient testPlanClient = connection.GetClient<TestPlanHttpClient>();

            BuildDefinitionReference buildDefinition = this._getBuildReference();
            TestOutcomeSettings testOutcomeSettings = new TestOutcomeSettings()
            {
                SyncOutcomeAcrossSuites = true
            };
            ReleaseEnvironmentDefinitionReference releaseEnvironmentDefinition = this._getReleaseEnvironmentDefinitionReference();
            TestPlanWebApi.TestPlanCreateParams testPlanCreateParams = new TestPlanWebApi.TestPlanCreateParams()
            {
                Name = "newCreatedPlan2",
                AreaPath = this._getArea(),
                Iteration = this._getIteration(),
                Description = "description of the test plan",
                StartDate = DateTime.Now.AddDays(2),
                EndDate = DateTime.Now.AddDays(9),
                State = "Inactive",
                BuildId = this._getBuildId(),
                BuildDefinition = buildDefinition,
                ReleaseEnvironmentDefinition = releaseEnvironmentDefinition,
                TestOutcomeSettings = testOutcomeSettings
            };


            // create a test plan
            TestPlanWebApi.TestPlan plan = testPlanClient.CreateTestPlanAsync(testPlanCreateParams, projectName).Result;


            Context.SetValue<TestPlanWebApi.TestPlan>("$newPlan2", plan);
            Context.Log("{0} {1}", plan.Id, plan.Name);
            return plan;
        }

        [ClientSampleMethod]
        public TestPlanWebApi.TestPlan GetTestPlanById()
        {
            string projectName = ClientSampleHelpers.FindAnyProject(this.Context).Name;

            // Get a testplan client instance
            VssConnection connection = Context.Connection;
            TestPlanHttpClient testPlanClient = connection.GetClient<TestPlanHttpClient>();
            
            TestPlanWebApi.TestPlan newplan1;
            Context.TryGetValue<TestPlanWebApi.TestPlan>("$newPlan1", out newplan1);
            if (newplan1 != null)
            {
                int id = newplan1.Id;


                // get a test plan
                TestPlanWebApi.TestPlan plan = testPlanClient.GetTestPlanByIdAsync(projectName, id).Result;


                Context.Log("{0} {1}", plan.Id, plan.Name);
                return plan;
            }
            return null;
        }

        [ClientSampleMethod]
        public TestPlanWebApi.TestPlan UpdateTestPlan()
        {
            string projectName = ClientSampleHelpers.FindAnyProject(this.Context).Name;

            // Get a testplan client instance
            VssConnection connection = Context.Connection;
            TestPlanHttpClient testPlanClient = connection.GetClient<TestPlanHttpClient>();

            BuildDefinitionReference buildDefinition = this._getBuildReference();
            TestOutcomeSettings testOutcomeSettings = new TestOutcomeSettings()
            {
                SyncOutcomeAcrossSuites = true
            };
            ReleaseEnvironmentDefinitionReference releaseEnvironmentDefinition = this._getReleaseEnvironmentDefinitionReference();

            
            TestPlanWebApi.TestPlan newplan1;
            Context.TryGetValue<TestPlanWebApi.TestPlan>("$newPlan1", out newplan1);
            if (newplan1 != null)
            {
                int id = newplan1.Id;
                TestPlanWebApi.TestPlanUpdateParams testPlanUpdateParams = new TestPlanWebApi.TestPlanUpdateParams()
                {
                    Name = "updatedPlan1",
                    AreaPath = this._getArea(),
                    Iteration = this._getIteration(),
                    Description = "description of the test plan",
                    StartDate = DateTime.Now.AddDays(2),
                    EndDate = DateTime.Now.AddDays(9),
                    State = "Inactive",
                    BuildId = this._getBuildId(),
                    Revision = newplan1.Revision,
                    BuildDefinition = buildDefinition,
                    ReleaseEnvironmentDefinition = releaseEnvironmentDefinition,
                    TestOutcomeSettings = testOutcomeSettings
                };


                // update a test plan
                TestPlanWebApi.TestPlan plan = testPlanClient.UpdateTestPlanAsync(testPlanUpdateParams, projectName, id).Result;


                Context.Log("{0} {1}", plan.Id, plan.Name);
                return plan;
            }
            return null;
        }

        [ClientSampleMethod]
        public TestPlanWebApi.TestPlan UpdateTestPlanWithAreaPathAndIteration()
        {
            string projectName = ClientSampleHelpers.FindAnyProject(this.Context).Name;

            // Get a testplan client instance
            VssConnection connection = Context.Connection;
            TestPlanHttpClient testPlanClient = connection.GetClient<TestPlanHttpClient>();
            
            TestPlanWebApi.TestPlan newplan2;
            Context.TryGetValue<TestPlanWebApi.TestPlan>("$newPlan2", out newplan2);
            if (newplan2 != null)
            {
                int id = newplan2.Id;
                TestPlanWebApi.TestPlanUpdateParams testPlanUpdateParams = new TestPlanWebApi.TestPlanUpdateParams()
                {
                    Name = "updatedPlan2",
                    AreaPath = this._getArea(),
                    Iteration = this._getIteration()
                };


                // update a test plan
                TestPlanWebApi.TestPlan plan = testPlanClient.UpdateTestPlanAsync(testPlanUpdateParams, projectName, id).Result;


                Context.Log("{0} {1}", plan.Id, plan.Name);
                return plan;
            }
            return null;
        }

        [ClientSampleMethod]
        public void DeletePlanById_1()
        {
            string projectName = ClientSampleHelpers.FindAnyProject(this.Context).Name;

            // Get a testplan client instance
            VssConnection connection = Context.Connection;
            TestPlanHttpClient testPlanClient = connection.GetClient<TestPlanHttpClient>();
            
            TestPlanWebApi.TestPlan newplan1;
            Context.TryGetValue<TestPlanWebApi.TestPlan>("$newPlan1", out newplan1);
            if (newplan1 != null)
            {
                int id1 = newplan1.Id;


                // Delete Test plan
                testPlanClient.DeleteTestPlanAsync(projectName, id1).SyncResult();
            }
        }

        [ClientSampleMethod]
        public void DeletePlanById_2()
        {
            string projectName = ClientSampleHelpers.FindAnyProject(this.Context).Name;

            // Get a testplan client instance
            VssConnection connection = Context.Connection;
            TestPlanHttpClient testPlanClient = connection.GetClient<TestPlanHttpClient>();

            TestPlanWebApi.TestPlan newplan2;
            Context.TryGetValue<TestPlanWebApi.TestPlan>("$newPlan2", out newplan2);
            if (newplan2 != null)
            {
                int id2 = newplan2.Id;


                // Delete Test plan
                testPlanClient.DeleteTestPlanAsync(projectName, id2).SyncResult();
            }
        }

        //Common dummy data
        //Edit this for use.
        private string _getArea()
        {
            return @"sampleProject\Team1";
        }
        private string _getIteration()
        {
            return @"sampleProject\Iteration 2";
        }

        private BuildDefinitionReference _getBuildReference()
        {
            int buildDefinitionId = 99999999;
            return new BuildDefinitionReference(buildDefinitionId);
        }

        private int _getBuildId()
        {
            return 99999999;
        }

        private ReleaseEnvironmentDefinitionReference _getReleaseEnvironmentDefinitionReference()
        {
            int releaseDefiniteonId = 99999999;
            int environmentId = 99999999;

            return new ReleaseEnvironmentDefinitionReference()
            {
                DefinitionId = releaseDefiniteonId,
                EnvironmentDefinitionId = environmentId
            };
        }

    }
}
