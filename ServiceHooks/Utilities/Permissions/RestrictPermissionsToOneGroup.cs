using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.Framework.Client;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Common;

namespace Microsoft.TeamServices.Samples.ServiceHooks
{
    public class RestrictPermissionsToOneGroup
    {
        private static readonly Guid ServiceHooksSecurityNamespaceId = new Guid("cb594ebe-87dd-4fc9-ac2c-6a10a4c92046");
        private static Int32 ManagePermissions = 7; // view, create, delete
        private static Int32 ViewPermissions = 1;   // view
        private static readonly string SpecialGroupName = "Service Hooks Administrators"; // assumed to be a collection-level group containing people that will have management permissions for SH in each project

        public void Run(Uri collectionUri)
        {
            Console.WriteLine("Utility to remove Service Hooks management permissions from the Project Administrators groups.");
            Console.WriteLine("");
            Console.WriteLine(" All projects in account/collection: " + collectionUri);
            Console.WriteLine("");

            Console.WriteLine("WARNING! This operation will remove the permissions.\n\n  Are you sure you want to continue (Y/N)?");
            int confirmChar = Console.In.Read();
            if (confirmChar != 'y' || confirmChar != 'Y')
            {
                return;
            }

            if (collectionUri != null)
            {
                TfsTeamProjectCollection connection = new TfsTeamProjectCollection(collectionUri);

                // Get Core, security, and identity services
                ISecurityService securityService = connection.GetService<ISecurityService>();
                SecurityNamespace hooksSecurity = securityService.GetSecurityNamespace(ServiceHooksSecurityNamespaceId);
                IIdentityManagementService2 identityService = connection.GetService<IIdentityManagementService2>();
                ProjectHttpClient projectClient = connection.GetClient<ProjectHttpClient>();

                IEnumerable<TeamProjectReference> projects = projectClient.GetProjects(stateFilter: Microsoft.TeamFoundation.Common.ProjectState.WellFormed).Result;

                // Iterate over each project, check SH permissions, and remove if the project administrators group has access
                foreach (var project in projects)
                {
                    // Remove manage permissions from the project's administrators group (but leave it "view" access)
                    Console.WriteLine(String.Format("Project {0} ({1})", project.Name, project.Id));

                    var groups = identityService.ListApplicationGroups(project.Id.ToString(), ReadIdentityOptions.None, null, Microsoft.TeamFoundation.Framework.Common.IdentityPropertyScope.Both);

                    String adminGroupName = String.Format("vstfs:///Classification/TeamProject/{0}\\Project Administrators", project.Id);

                    try
                    {
                        TeamFoundationIdentity adminGroup = groups.First(g => String.Equals(g.UniqueName, adminGroupName, StringComparison.InvariantCultureIgnoreCase));

                        Console.WriteLine(" - Checking Project Administrators group permissions");

                        String securityToken = "PublisherSecurity/" + project.Id;

                        bool hasPermission = hooksSecurity.HasPermission(securityToken, adminGroup.Descriptor, ManagePermissions, false);
                        
                        // Project admin group has "manage" permissions for SH in the project
                        if (hasPermission)
                        {
                            // Remove manage permissions from the project's administrators group (but leave it "view" access)
                            Console.WriteLine(" - Has permissions. Removing...");

                            // Give the admin group only view permissions
                            hooksSecurity.SetPermissions(securityToken, adminGroup.Descriptor, ViewPermissions, 0, false);

                            // check permission again after granting
                            hasPermission = hooksSecurity.HasPermission(securityToken, adminGroup.Descriptor, ManagePermissions, false);
                            if (!hasPermission)
                            {
                                Console.WriteLine(" - Verified permissions correctly removed.");
                            }
                            else
                            {
                                Console.WriteLine(" - Project Administrators Group still has manage permissions.");
                            }
                         
                        }
                        else
                        {
                            Console.WriteLine(" - Does not have permissions to manage service hook subscriptions.");
                        }                        
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(String.Format("Admin group: Not found! ({0})", ex.Message));
                    }
                    
                    Console.WriteLine("");
                }

                // Grant the group manage permissions across the entire collection
                TeamFoundationIdentity specialGroup = identityService.ReadIdentity(IdentitySearchFactor.DisplayName, SpecialGroupName, MembershipQuery.None, ReadIdentityOptions.None);

                if (specialGroup != null)
                {
                    Console.WriteLine("Granting full manage permissions to: {0}", specialGroup.UniqueName);

                    String rootSecurityToken = "PublisherSecurity/";
                    hooksSecurity.SetPermissions(rootSecurityToken, specialGroup.Descriptor, ManagePermissions, 0, false);
                }
                else
                {
                    Console.WriteLine("Could not find this group.");
                }
            }
        }
    }
}
