using Microsoft.VisualStudio.Services.Graph.Client;
using System;
using System.Collections.Generic;

namespace GraphQuickStarts
{
    class Program
    {
        public static int Main(string[] args)
        {
            if (args.Length == 0)
            {
                ShowUsage();
                return 0;
            }

            string connectionUrl, token, groupName, clientId, redirectURL = "";

            try
            {
                CheckArguments(args, out connectionUrl, out token, out groupName, out clientId, out redirectURL);
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine(ex.Message);

                ShowUsage();
                return -1;
            }

            try
            {
                Console.WriteLine("Executing Graph quick start samples...");
                Console.WriteLine("");

                // Enumerate Users sample
                Samples.EnumerateUsers objUsers = new Samples.EnumerateUsers(connectionUrl, token);
                List<GraphUser> users = objUsers.RunEnumerateUsersUsingClientLib();

                // Enumber Group Memberships sample
                objUsers.RunEnumerateEnumerateGroupMembershipsUsingClientLib(users);

                // Enumerate Members of Groups sample
                Samples.EnumerateMembersOfGroups objMembers = new Samples.EnumerateMembersOfGroups(connectionUrl, clientId, redirectURL);
                objMembers.RunEnumerateMembersOfGroupsUsingClientLib(groupName);

                Console.ReadKey();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed to run the sample: " + ex.Message);
                return 1;
            }

            return 0;
        }

        private static void ShowUsage()
        {
            Console.WriteLine("Runs the Graph Quick Start samples on a Team Services account or Team Foundation Server instance.");
            Console.WriteLine("");
            Console.WriteLine("These samples are to provide you the building blocks of using the REST API's in Identity.");
            Console.WriteLine("Examples are written using the .NET client library and using direct HTTP calls. We recommend, that");
            Console.WriteLine("whenever possible, you use the .NET client library.");
            Console.WriteLine("");
            Console.WriteLine("Arguments:");
            Console.WriteLine("");
            Console.WriteLine("  /url:http://fabrikam.vssps.visualstudio.com /token:personalaccesstoken /group:Developers /clientId:7e2fa445-a7c0-48b2-b1b2-be805e7a2fdf /redirectUrl:http://sampleUrl");
            Console.WriteLine("");

            Console.ReadKey();
        }

        private static void CheckArguments(string[] args, out string connectionUrl, out string token, out string groupName, out string clientId, out string redirectUrl)
        {
            connectionUrl = null;
            token = null;
            groupName = null;
            clientId = null;
            redirectUrl = null;

            Dictionary<string, string> argsMap = new Dictionary<string, string>();
            foreach (var arg in args)
            {
                if (arg[0] == '/' && arg.IndexOf(':') > 1)
                {
                    string key = arg.Substring(1, arg.IndexOf(':') - 1);
                    string value = arg.Substring(arg.IndexOf(':') + 1);

                    switch (key)
                    {
                        case "url":
                            connectionUrl = value;
                            break;

                        case "token":
                            token = value;
                            break;

                        case "group":
                            groupName = value;
                            break;

                        case "clientId":
                            clientId = value;
                            break;

                        case "redirectUrl":
                            redirectUrl = value;
                            break;

                        default:
                            throw new ArgumentException("Unknown argument", key);
                    }
                }
            }

            if (connectionUrl == null || token == null)
            {
                throw new ArgumentException("Missing required arguments");
            }
        }
    }
}
