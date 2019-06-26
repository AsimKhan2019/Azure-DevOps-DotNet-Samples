using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Graph;
using Microsoft.VisualStudio.Services.Graph.Client;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Microsoft.Azure.DevOps.ClientSamples.Graph
{
    [ClientSample(GraphResourceIds.AreaName, GraphResourceIds.SubjectLookup.SubjectLookupResourceName)]
    public class SubjectLookupSample : ClientSample
    {
        /// <summary>
        /// Resolve descriptors to users/groups in batch.
        /// </summary>
        /// <returns></returns>
        [ClientSampleMethod]
        public void LookupSubject()
        {
            // Get the client
            VssConnection connection = Context.Connection;
            GraphHttpClient graphClient = connection.GetClient<GraphHttpClient>();

            //
            // Part 1: add the AAD user
            // 
            ClientSampleHttpLogger.SetOperationName(this.Context, "MaterializeAADUserByOIDWithStorageKey");
            GraphUserCreationContext addAADUserContext = new GraphUserOriginIdCreationContext
            {
                OriginId = "e97b0e7f-0a61-41ad-860c-748ec5fcb20b",
                StorageKey = Guid.NewGuid()
            };

            GraphUser newUser = graphClient.CreateUserAsync(addAADUserContext).Result;
            string userDescriptor = newUser.Descriptor;

            Context.Log("New user added! ID: {0}", userDescriptor);

            //
            // Part 2: add the AAD group
            // 
            ClientSampleHttpLogger.SetOperationName(this.Context, "MaterializeAADGroupByOIDWithStorageKey");
            GraphGroupCreationContext addAADGroupContext = new GraphGroupOriginIdCreationContext
            {
                OriginId = "f0d20172-7b96-42f6-9436-941433654b48",
                StorageKey = Guid.NewGuid()
            };

            GraphGroup newGroup = graphClient.CreateGroupAsync(addAADGroupContext).Result;
            string groupDescriptor = newGroup.Descriptor;

            Context.Log("New group created! ID: {0}", groupDescriptor);


            //
            // Part 3: lookup subjects
            //
            GraphSubjectLookup subjectLookup = new GraphSubjectLookup(new[] {
                new GraphSubjectLookupKey(newGroup.Descriptor),
                new GraphSubjectLookupKey(newUser.Descriptor)
            });
            ClientSampleHttpLogger.SetOperationName(this.Context, "LookupSubjects");
            IReadOnlyDictionary<SubjectDescriptor, GraphSubject> lookups = graphClient.LookupSubjectsAsync(subjectLookup).Result;
        }
    }
}
