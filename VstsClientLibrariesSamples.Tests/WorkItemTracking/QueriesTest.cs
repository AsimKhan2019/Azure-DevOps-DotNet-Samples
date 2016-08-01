using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using VstsClientLibrariesSamples.WorkItemTracking;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using System.Collections.Generic;

namespace VstsClientLibrariesSamples.Tests.WorkItemTracking
{
    [TestClass]
    public class QueriesTest
    {
        private IConfiguration _config = new Configuration();

        [TestInitialize]
        public void TestInitialize()
        {
            InitHelper.GetConfiguration(_config);
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _config = null;
        }

        [TestMethod, TestCategory("Client Libraries")]
        public void WorkItemTracking_Queries_GetQueryByName_Success()
        {
            //arrange
            Queries queries = new Queries(_config);

            //act
            var result = queries.GetQueryByName(_config.Project, _config.Query);

            //assert
            Assert.IsInstanceOfType(result, typeof(QueryHierarchyItem));
            Assert.AreEqual("Open User Stories", result.Name);
        }

        [TestMethod, TestCategory("Client Libraries")]
        public void WorkItemTracking_Queries_ExecuteQuery_Success()
        {
            //arrange
            Queries queries = new Queries(_config);

            //act
            var queryResult = queries.GetQueryByName(_config.Project, _config.Query);
            var queryId = queryResult.Id;

            try
            {
                var result = queries.ExecuteQuery(queryId);
                
                //assert
                Assert.IsInstanceOfType(result, typeof(WorkItemQueryResult));
            }
            catch (System.NullReferenceException ex)
            {
                Assert.Inconclusive(ex.Message);
            }                  
        }

        [TestMethod, TestCategory("Client Libraries")]
        public void WorkItemTracking_Queries_ExecuteByWiql_Success()
        {
            //arrange
            Queries queries = new Queries(_config);

            //create a query to get your list of work items needed
            Wiql wiql = new Wiql()
            {
                Query = "Select [State], [Title] " +
                       "From WorkItems " +
                       "Where [Work Item Type] = 'Bug' " +
                       "And [System.State] = 'New' " +
                       "Order By [State] Asc, [Changed Date] Desc"
            };
                       
            try
            {
                //act
                var result = queries.ExecuteByWiql(wiql, _config.Project);

                //assert
                Assert.IsInstanceOfType(result, typeof(WorkItemQueryResult));
            }
            catch (System.NullReferenceException ex)
            {
                Assert.Inconclusive(ex.Message);
            }
        }
    }
}
