using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi;
using Microsoft.VisualStudio.Services.WebApi;

namespace Microsoft.Azure.DevOps.ClientSamples.Test
{
    [ClientSample(TestPlanResourceIds.AreaName, TestPlanResourceIds.TestVariableLocationIdString)]
    public class TestVariableSample : ClientSample
    {
        [ClientSampleMethod]
        public List<TestVariable> GetTestVariables()
        {
            string projectName = ClientSampleHelpers.FindAnyProject(this.Context).Name;
            // Get a testplan client instance
            VssConnection connection = Context.Connection;
            TestPlanHttpClient testPlanClient = connection.GetClient<TestPlanHttpClient>();

            // Get Test Variables
            List<TestVariable> variables = testPlanClient.GetTestVariablesAsync(projectName).Result;

            foreach (TestVariable variable in variables)
            {
                Context.Log("{0} {1}", variable.Id.ToString().PadLeft(6), variable.Name);
            }
            return variables;
        }

        [ClientSampleMethod]
        public TestVariable CreateTestVariable()
        {
            string projectName = ClientSampleHelpers.FindAnyProject(this.Context).Name;
            // Get a testplan client instance
            VssConnection connection = Context.Connection;
            TestPlanHttpClient testPlanClient = connection.GetClient<TestPlanHttpClient>();

            TestVariableCreateUpdateParameters testVariableCreateUpdateParameters = new TestVariableCreateUpdateParameters()
            {
                Name = "SampleTestVariable1",
                Description = "Sample Test Variable",
                Values = new List<string>()
                {
                    "Test Value 1",
                    "Test Value 2"
                }
            };

            // Create Test Variable
            TestVariable variable = testPlanClient.CreateTestVariableAsync(testVariableCreateUpdateParameters, projectName).Result;

            Context.SetValue<TestVariable>("$newVariable", variable);
            Context.Log("{0} {1}", variable.Id.ToString().PadLeft(6), variable.Name);
            return variable;
        }

        [ClientSampleMethod]
        public TestVariable GetTestVariableById()
        {
            string projectName = ClientSampleHelpers.FindAnyProject(this.Context).Name;
            // Get a testplan client instance
            VssConnection connection = Context.Connection;
            TestPlanHttpClient testPlanClient = connection.GetClient<TestPlanHttpClient>();

            TestVariable newVariable;
            Context.TryGetValue<TestVariable>("$newVariable", out newVariable);
            if (newVariable != null)
            {
                int id = newVariable.Id;

                // Get Test Variable
                TestVariable variable = testPlanClient.GetTestVariableByIdAsync(projectName, id).Result;

                Context.Log("{0} {1}", variable.Id.ToString().PadLeft(6), variable.Name);
                return variable;
            }
            return null;
        }

        [ClientSampleMethod]
        public TestVariable UpdateTestVariable()
        {
            string projectName = ClientSampleHelpers.FindAnyProject(this.Context).Name;
            // Get a testplan client instance
            VssConnection connection = Context.Connection;
            TestPlanHttpClient testPlanClient = connection.GetClient<TestPlanHttpClient>();

            //Get the test variable first
            TestVariable newVariable;
            Context.TryGetValue<TestVariable>("$newVariable", out newVariable);
            if (newVariable != null)
            {
                int id = newVariable.Id;

                TestVariable variable = testPlanClient.GetTestVariableByIdAsync(projectName, id).Result;
                TestVariableCreateUpdateParameters testVariableCreateUpdateParameters = new TestVariableCreateUpdateParameters()
                {
                    Name = variable.Name,
                    Description = "Updated Description",
                    Values = variable.Values
                };
                testVariableCreateUpdateParameters.Values.Add("New Value");

                // Update Test Variable
                TestVariable updatedVariable = testPlanClient.UpdateTestVariableAsync(testVariableCreateUpdateParameters, projectName, variable.Id).Result;

                Context.Log("{0} {1}", updatedVariable.Id.ToString().PadLeft(6), updatedVariable.Name);
                return variable;
            }
            return null;
        }

        [ClientSampleMethod]
        public void DeleteTestVariable()
        {
            string projectName = ClientSampleHelpers.FindAnyProject(this.Context).Name;
            // Get a testplan client instance
            VssConnection connection = Context.Connection;
            TestPlanHttpClient testPlanClient = connection.GetClient<TestPlanHttpClient>();            

            TestVariable newVariable;
            Context.TryGetValue<TestVariable>("$newVariable", out newVariable);
            if (newVariable != null)
            {
                int id = newVariable.Id;

                //Delete the test variable
                testPlanClient.DeleteTestVariableAsync(projectName, id).SyncResult();
            }
        }
    }
}
