using Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using System.Collections.Generic;
using SuiteEntry = Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi.SuiteEntry;

namespace Microsoft.Azure.DevOps.ClientSamples.Test
{
    [ClientSample(TestPlanResourceIds.AreaName, TestPlanResourceIds.TestSuiteEntryProjectLocationIdString)]
    public class TestSuiteEntrySample : ClientSample
    {
        [ClientSampleMethod]
        public List<SuiteEntry> GetSuiteEntries()
        {
            string projectName = ClientSampleHelpers.FindAnyProject(this.Context).Name;
            int suiteId = this._getTestSuiteId();

            // Get a testplan client instance
            VssConnection connection = Context.Connection;
            TestPlanHttpClient testPlanClient = connection.GetClient<TestPlanHttpClient>();

            
            //get suite entries
            List<SuiteEntry> suiteEntries = testPlanClient.GetSuiteEntriesAsync(projectName, suiteId).Result;


            foreach (SuiteEntry suiteEntry in suiteEntries)
            {
                Context.Log("{0} {1}", suiteEntry.Id.ToString().PadLeft(6), suiteEntry.SequenceNumber);
            }
            return suiteEntries;
        }

        [ClientSampleMethod]
        public List<SuiteEntry> GetChildSuiteEntries()
        {
            string projectName = ClientSampleHelpers.FindAnyProject(this.Context).Name;
            int suiteId = this._getTestSuiteId();

            // Get a testplan client instance
            VssConnection connection = Context.Connection;
            TestPlanHttpClient testPlanClient = connection.GetClient<TestPlanHttpClient>();



            //get suite entries            
            List<SuiteEntry> suiteEntries = testPlanClient.GetSuiteEntriesAsync(projectName, suiteId, SuiteEntryTypes.Suite).Result;


            Context.SetValue<List<SuiteEntry>>("$childSuiteEntries", suiteEntries);
            foreach (SuiteEntry suiteEntry in suiteEntries)
            {
                Context.Log("{0} {1}", suiteEntry.Id.ToString().PadLeft(6), suiteEntry.SequenceNumber);
            }
            return suiteEntries;
        }

        [ClientSampleMethod]
        public List<SuiteEntry> GetTestCaseEntries()
        {
            string projectName = ClientSampleHelpers.FindAnyProject(this.Context).Name;
            int suiteId = this._getTestSuiteId();

            // Get a testplan client instance
            VssConnection connection = Context.Connection;
            TestPlanHttpClient testPlanClient = connection.GetClient<TestPlanHttpClient>();


            //get suite entries            
            List<SuiteEntry> suiteEntries = testPlanClient.GetSuiteEntriesAsync(projectName, suiteId, SuiteEntryTypes.TestCase).Result;


            Context.SetValue<List<SuiteEntry>>("$testCaseEntries", suiteEntries);
            foreach (SuiteEntry suiteEntry in suiteEntries)
            {
                Context.Log("{0} {1}", suiteEntry.Id.ToString().PadLeft(6), suiteEntry.SequenceNumber);
            }
            return suiteEntries;
        }

        [ClientSampleMethod]
        public List<SuiteEntry> UpdateTestCaseEntryOrder()
        {
            string projectName = ClientSampleHelpers.FindAnyProject(this.Context).Name;
            int suiteId = this._getTestSuiteId();

            // Get a testplan client instance
            VssConnection connection = Context.Connection;
            TestPlanHttpClient testPlanClient = connection.GetClient<TestPlanHttpClient>();

            List<SuiteEntry> testCases;
            Context.TryGetValue<List<SuiteEntry>>("$testCaseEntries", out testCases);

            if (testCases != null && testCases.Count >= 2)
            {
                SuiteEntry testCase1 = testCases[0];
                SuiteEntry testCase2 = testCases[1];

                SuiteEntryUpdateParams suiteEntry1 = new SuiteEntryUpdateParams()
                {
                    Id = testCase1.Id,
                    SuiteEntryType = SuiteEntryTypes.TestCase,
                    SequenceNumber = testCase2.SequenceNumber
                };

                SuiteEntryUpdateParams suiteEntry2 = new SuiteEntryUpdateParams()
                {
                    Id = testCase2.Id,
                    SuiteEntryType = SuiteEntryTypes.TestCase,
                    SequenceNumber = testCase1.SequenceNumber
                };

                List<SuiteEntryUpdateParams> updatedEntries = new List<SuiteEntryUpdateParams>()
                {
                    suiteEntry1,
                    suiteEntry2
                };



                //update suite entries
                List<SuiteEntry> suiteEntries = testPlanClient.ReorderSuiteEntriesAsync(updatedEntries, projectName, suiteId).Result;
                


                foreach (SuiteEntry suiteEntry in suiteEntries)
                {
                    Context.Log("{0} {1}", suiteEntry.Id.ToString().PadLeft(6), suiteEntry.SequenceNumber);
                }
                return suiteEntries;
            }

            return null;
        }

        [ClientSampleMethod]
        public List<SuiteEntry> UpdateChildSuiteEntryOrder()
        {
            string projectName = ClientSampleHelpers.FindAnyProject(this.Context).Name;
            int suiteId = this._getTestSuiteId();

            // Get a testplan client instance
            VssConnection connection = Context.Connection;
            TestPlanHttpClient testPlanClient = connection.GetClient<TestPlanHttpClient>();

            List<SuiteEntry> childSuites;
            Context.TryGetValue<List<SuiteEntry>>("$childSuiteEntries", out childSuites);

            if (childSuites != null && childSuites.Count >= 2)
            {
                SuiteEntry childSuite1 = childSuites[0];
                SuiteEntry childSuite2 = childSuites[1];

                SuiteEntryUpdateParams suiteEntry1 = new SuiteEntryUpdateParams()
                {
                    Id = childSuite1.Id,
                    SuiteEntryType = SuiteEntryTypes.Suite,
                    SequenceNumber = childSuite2.SequenceNumber
                };

                SuiteEntryUpdateParams suiteEntry2 = new SuiteEntryUpdateParams()
                {
                    Id = childSuite2.Id,
                    SuiteEntryType = SuiteEntryTypes.Suite,
                    SequenceNumber = childSuite1.SequenceNumber
                };

                List<SuiteEntryUpdateParams> updatedEntries = new List<SuiteEntryUpdateParams>()
                {
                    suiteEntry1,
                    suiteEntry2
                };



                //update suite entries
                List<SuiteEntry> suiteEntries = testPlanClient.ReorderSuiteEntriesAsync(updatedEntries, projectName, suiteId).Result;



                foreach (SuiteEntry suiteEntry in suiteEntries)
                {
                    Context.Log("{0} {1}", suiteEntry.Id.ToString().PadLeft(6), suiteEntry.SequenceNumber);
                }
                return suiteEntries;
            }

            return null;
        }

        [ClientSampleMethod]
        public List<SuiteEntry> UpdateTestCaseAndChildSuiteEntryOrder()
        {
            string projectName = ClientSampleHelpers.FindAnyProject(this.Context).Name;
            int suiteId = this._getTestSuiteId();

            // Get a testplan client instance
            VssConnection connection = Context.Connection;
            TestPlanHttpClient testPlanClient = connection.GetClient<TestPlanHttpClient>();

            List<SuiteEntry> childSuites;
            Context.TryGetValue<List<SuiteEntry>>("$childSuiteEntries", out childSuites);

            List<SuiteEntry> testCases;
            Context.TryGetValue<List<SuiteEntry>>("$testCaseEntries", out testCases);

            if (childSuites != null && childSuites.Count >= 2 && testCases != null && testCases.Count >= 2)
            {
                SuiteEntry testCase2 = testCases[0];
                SuiteEntry childSuite2 = childSuites[0];

                SuiteEntryUpdateParams suiteEntry1 = new SuiteEntryUpdateParams()
                {
                    Id = testCase2.Id,
                    SuiteEntryType = SuiteEntryTypes.TestCase,
                    SequenceNumber = 0
                };

                SuiteEntryUpdateParams suiteEntry2 = new SuiteEntryUpdateParams()
                {
                    Id = childSuite2.Id,
                    SuiteEntryType = SuiteEntryTypes.Suite,
                    SequenceNumber = 0
                };

                List<SuiteEntryUpdateParams> updatedEntries = new List<SuiteEntryUpdateParams>()
                {
                    suiteEntry1,
                    suiteEntry2
                };



                //update suite entries
                List<SuiteEntry> suiteEntries = testPlanClient.ReorderSuiteEntriesAsync(updatedEntries, projectName, suiteId).Result;



                foreach (SuiteEntry suiteEntry in suiteEntries)
                {
                    Context.Log("{0} {1}", suiteEntry.Id.ToString().PadLeft(6), suiteEntry.SequenceNumber);
                }
                return suiteEntries;
            }

            return null;
        }

        [ClientSampleMethod]
        public List<SuiteEntry> UpdateTestCaseEntryOrderWithConflcitingSequenceNumber()
        {
            string projectName = ClientSampleHelpers.FindAnyProject(this.Context).Name;
            int suiteId = this._getTestSuiteId();

            // Get a testplan client instance
            VssConnection connection = Context.Connection;
            TestPlanHttpClient testPlanClient = connection.GetClient<TestPlanHttpClient>();

            List<SuiteEntry> testCases;
            Context.TryGetValue<List<SuiteEntry>>("$testCaseEntries", out testCases);

            if (testCases != null && testCases.Count >= 2)
            {
                SuiteEntry testCase1 = testCases[0];
                SuiteEntry testCase2 = testCases[1];

                SuiteEntryUpdateParams suiteEntry1 = new SuiteEntryUpdateParams()
                {
                    Id = testCase1.Id,
                    SuiteEntryType = SuiteEntryTypes.TestCase,
                    SequenceNumber = 0
                };

                SuiteEntryUpdateParams suiteEntry2 = new SuiteEntryUpdateParams()
                {
                    Id = testCase2.Id,
                    SuiteEntryType = SuiteEntryTypes.TestCase,
                    SequenceNumber = 0
                };

                List<SuiteEntryUpdateParams> updatedEntries = new List<SuiteEntryUpdateParams>()
                {
                    suiteEntry1,
                    suiteEntry2
                };


                //update suite entries
                List<SuiteEntry> suiteEntries = testPlanClient.ReorderSuiteEntriesAsync(updatedEntries, projectName, suiteId).Result;



                foreach (SuiteEntry suiteEntry in suiteEntries)
                {
                    Context.Log("{0} {1}", suiteEntry.Id.ToString().PadLeft(6), suiteEntry.SequenceNumber);
                }
                return suiteEntries;
            }

            return null;
        }

        [ClientSampleMethod]
        public List<SuiteEntry> UpdateTestCaseEntryOrderWithOverflowingSequenceOrder()
        {
            string projectName = ClientSampleHelpers.FindAnyProject(this.Context).Name;
            int suiteId = this._getTestSuiteId();

            // Get a testplan client instance
            VssConnection connection = Context.Connection;
            TestPlanHttpClient testPlanClient = connection.GetClient<TestPlanHttpClient>();

            List<SuiteEntry> testCases;
            Context.TryGetValue<List<SuiteEntry>>("$testCaseEntries", out testCases);

            if (testCases != null && testCases.Count >= 1)
            {
                SuiteEntry testCase1 = testCases[0];

                SuiteEntryUpdateParams suiteEntry1 = new SuiteEntryUpdateParams()
                {
                    Id = testCase1.Id,
                    SuiteEntryType = SuiteEntryTypes.TestCase,
                    SequenceNumber = 9999999
                };

                List<SuiteEntryUpdateParams> updatedEntries = new List<SuiteEntryUpdateParams>()
                {
                    suiteEntry1
                };


                //update suite entries
                List<SuiteEntry> suiteEntries = testPlanClient.ReorderSuiteEntriesAsync(updatedEntries, projectName, suiteId).Result;


                foreach (SuiteEntry suiteEntry in suiteEntries)
                {
                    Context.Log("{0} {1}", suiteEntry.Id.ToString().PadLeft(6), suiteEntry.SequenceNumber);
                }
                return suiteEntries;
            }

            return null;
        }

        //Common dummy data
        //Edit this for use.
        private int _getTestSuiteId()
        {
            return 99999999;
        }
    }
}
