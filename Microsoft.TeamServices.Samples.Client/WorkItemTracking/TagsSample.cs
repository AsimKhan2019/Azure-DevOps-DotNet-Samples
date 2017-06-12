using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamServices.Samples.Client.WorkItemTracking
{
    /// <summary>
    /// 
    /// Samples for accessing tag metadata
    /// 
    /// See https://www.visualstudio.com/en-us/docs/integrate/api/wit/tags for more details.
    /// 
    /// </summary> 
    [ClientSample(TeamFoundation.WorkItemTracking.WebApi.WitConstants.WorkItemTrackingWebConstants.RestAreaName, "tagging")]
    public class TagsSample : ClientSample
    {
        [ClientSampleMethod]
        public WebApiTagDefinitionList GetListOfTags()
        {
            System.Guid projectId = ClientSampleHelpers.FindAnyProject(this.Context).Id;

            VssConnection connection = Context.Connection;
            TaggingHttpClient taggingClient = connection.GetClient<TaggingHttpClient>();

            WebApiTagDefinitionList listofTags = taggingClient.GetTagsAsync(projectId).Result;

            Console.WriteLine("List of tags:");

            foreach (var tag in listofTags)
            {
                Console.WriteLine("  ({0}) - {1}", tag.Id.ToString(), tag.Name);
            }

            return listofTags;
        }

        [ClientSampleMethod]
        public List<WebApiTagDefinition> GetListOfTagsIncludeInactive()
        {
            System.Guid projectId = ClientSampleHelpers.FindAnyProject(this.Context).Id;

            VssConnection connection = Context.Connection;
            TaggingHttpClient taggingClient = connection.GetClient<TaggingHttpClient>();

            WebApiTagDefinitionList listofTags = taggingClient.GetTagsAsync(projectId, true).Result;
            List<WebApiTagDefinition> listofInactiveTags = listofTags.Where(x => x.Active == false).ToList<WebApiTagDefinition>();

            Console.WriteLine("List of inactive tags:");

            foreach (var tag in listofInactiveTags)
            {
                Console.WriteLine("  ({0}) - {1}", tag.Id.ToString(), tag.Name);
            }

            return listofInactiveTags;
        }

        [ClientSampleMethod]
        public WebApiTagDefinition GetTagByName()
        {
            System.Guid projectId = ClientSampleHelpers.FindAnyProject(this.Context).Id;
            string tagName = "test"; //TODO

            VssConnection connection = Context.Connection;
            TaggingHttpClient taggingClient = connection.GetClient<TaggingHttpClient>();

            WebApiTagDefinition tag = taggingClient.GetTagAsync(projectId, tagName).Result;

            if (tag == null)
            {
                Console.WriteLine("Tag '{0}' not found", tagName);
            }
            else
            {
                Console.WriteLine("Name:   {0}", tagName);
                Console.WriteLine("Id:     {0}", tag.Id.ToString());
                Console.WriteLine("Active: {0}", tag.Active.ToString());
            }

            return tag;
        }

        [ClientSampleMethod]
        public WebApiTagDefinition GetTagById()
        {
            System.Guid projectId = ClientSampleHelpers.FindAnyProject(this.Context).Id;
            System.Guid tagId = new System.Guid("C807AEE9-D3FA-468D-BFD5-66C2B3D42AD3"); //TODO

            VssConnection connection = Context.Connection;
            TaggingHttpClient taggingClient = connection.GetClient<TaggingHttpClient>();

            WebApiTagDefinition tag = taggingClient.GetTagAsync(projectId, tagId).Result;

            if (tag == null)
            {
                Console.WriteLine("Tag '{0}' not found", tagId);
            }
            else
            {
                Console.WriteLine("Name:   {0}", tag.Name);
                Console.WriteLine("Id:     {0}", tag.Id.ToString());
                Console.WriteLine("Active: {0}", tag.Active.ToString());
            }

            return tag;
        }

        [ClientSampleMethod]
        public WebApiTagDefinition CreateTag()
        {
            System.Guid projectId = ClientSampleHelpers.FindAnyProject(this.Context).Id;
            string tagName = "Hello World";

            VssConnection connection = Context.Connection;
            TaggingHttpClient taggingClient = connection.GetClient<TaggingHttpClient>();

            WebApiTagDefinition tag = taggingClient.CreateTagAsync(projectId, tagName).Result;

            Console.WriteLine("Tag '{0}' successfully created", tagName);

            return tag;
        }

        [ClientSampleMethod]
        public WebApiTagDefinition UpdateTag()
        {
            System.Guid projectId = ClientSampleHelpers.FindAnyProject(this.Context).Id;
            System.Guid tagId = new System.Guid("C807AEE9-D3FA-468D-BFD5-66C2B3D42AD3"); //TODO

            VssConnection connection = Context.Connection;
            TaggingHttpClient taggingClient = connection.GetClient<TaggingHttpClient>();

            WebApiTagDefinition tag = taggingClient.UpdateTagAsync(projectId, tagId, "Pretty Monkey", true).Result;

            if (tag == null)
            {
                Console.WriteLine("Error updating tag: ", tagId);
            }
            else
            {
                Console.WriteLine("Tag successfully updated");
                Console.WriteLine("Name:   {0}", tag.Name);
                Console.WriteLine("Id:     {0}", tag.Id.ToString());
                Console.WriteLine("Active: {0}", tag.Active.ToString());
            }

            return tag;
        }

        [ClientSampleMethod]
        public void DeleteTag()
        {
            System.Guid projectId = ClientSampleHelpers.FindAnyProject(this.Context).Id;
            System.Guid tagId = new System.Guid("C807AEE9-D3FA-468D-BFD5-66C2B3D42AD3"); //TODO

            VssConnection connection = Context.Connection;
            TaggingHttpClient taggingClient = connection.GetClient<TaggingHttpClient>();

            taggingClient.DeleteTagAsync(projectId, tagId).SyncResult();

            Console.WriteLine("Tag '{0}' deleted", tagId);
        }

        [ClientSampleMethod]
        public void DeleteAllInactiveTags()
        {
            System.Guid projectId = ClientSampleHelpers.FindAnyProject(this.Context).Id;

            VssConnection connection = Context.Connection;
            TaggingHttpClient taggingClient = connection.GetClient<TaggingHttpClient>();

            WebApiTagDefinitionList listofTags = taggingClient.GetTagsAsync(projectId, true).Result;
            List<WebApiTagDefinition> listofInactiveTags = listofTags.Where(x => x.Active == false).ToList<WebApiTagDefinition>();

            Console.WriteLine("Get list of inactive tags: Done");
            Console.WriteLine("Start deleting inactive tags...");

            foreach (var tag in listofInactiveTags)
            {
                Console.WriteLine("  Delete tag '{0}'", tag.Name);

                taggingClient.DeleteTagAsync(projectId, tag.Id).SyncResult();
            }

            Console.WriteLine("");
            Console.WriteLine("Completed");
        }
    }
}
