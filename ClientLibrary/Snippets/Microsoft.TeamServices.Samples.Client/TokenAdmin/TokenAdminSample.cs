using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.Services.Graph.Client;
using Microsoft.VisualStudio.Services.TokenAdmin.Client;
using Microsoft.VisualStudio.Services.WebApi;

namespace Microsoft.TeamServices.Samples.Client.TokenAdmin
{
    /// <summary>
    /// This sample shows how you can use the VSTS REST APIs to find and revoke 
    /// personal access tokens (PATs) for users in your organization.
    /// It also shows how to create revocation rules that prevent access through other OAuth credentials,
    /// such as self-describing session tokens.
    /// The sample is written using our C# client libraries, 
    /// but is commented with the HTTP calls that you can make to perform these same operations directly over the wire.
    /// </summary>
    /// <remarks>
    /// - NOTE ON REQUIRED PERMISSIONS: To be able to call of all the endpoints in this sample, 
    /// you will need to be an administrator of the target organization.
    /// Non-administrators will receive an authorization error.
    /// - NOTE ON CONNECTION DETAILS: For this sample, we will be using the connection from our current client context, 
    /// which includes our base URL and our authentication credentials through the parameters passed in through the runner.
    /// If you are calling these endpoints directly over the wire,
    /// your base URL will be => https://{yourOrganization}.vssps.visualstudio.com
    /// and the recommended approach for authentication is to acquire an ADAL access token and pass it in the header => Authorization: Bearer {yourADALAccessToken}
    /// For further guidance on how to acquire an ADAL access token, see:
    /// https://github.com/Microsoft/vsts-auth-samples/tree/master/ManagedClientConsoleAppSample
    /// For further guidance on other authentication options, see:
    /// https://docs.microsoft.com/en-us/vsts/integrate/get-started/authentication/authentication-guidance?view=vsts
    /// </remarks>
    [ClientSample(TokenAdminResourceIds.AreaName)]
    public class TokenAdminSample : ClientSample
    {
        // The areas of the VSTS REST API / HTTP clients that we will be using for these administration tasks are:
        // - the Graph endpoints, corresponding to the path pattern: /_apis/graph/*
        // - the TokenAdmin endpoints, corresponding to the path pattern: /_apis/tokenAdmin/*
        //   The C# client for this area may not be available until the next release of our client libraries,
        //   but the REST endpoints themselves should be available at the time of publishing.

        [ClientSampleMethod(TokenAdminResourceIds.AreaName, TokenAdminResourceIds.PersonalAccessTokensResource)]
        public List<Guid> GetPersonalAccessTokenDetailsForUsersInYourOrganization()
        {
            List<Guid> authorizationIds = new List<Guid>();
            
            var graphHttpClient = Context.Connection.GetClient<GraphHttpClient>();
            var tokenAdminHttpClient = Context.Connection.GetClient<TokenAdminHttpClient>();

            // 1. RETRIEVING USERS

            // First, we will use the Graph endpoints to find all of the users in the organization.
            // This is a paginated API, so you may need to keep track of the continuation token
            // and send multiple requests depending on the number of users in your organization.

            string userContinuationToken = null;

            do
            {
                // Make a call to retrieve the next page of users:
                // HTTP: GET /_apis/graph/users[?continuationToken={userContinuationToken}]
                //       => 200 OK {
                //                   "count":n,
                //                   "value":[{
                //                      "descriptor":"{user.Descriptor}",
                //                        …
                //                 },…]
                var pageOfUsers = graphHttpClient.ListUsersAsync(continuationToken: userContinuationToken).SyncResult();

                // The continuation token, if any, is returned in the X-MS-ContinuationToken response header:
                userContinuationToken = pageOfUsers.ContinuationToken?.SingleOrDefault();

                // 2. RETRIEVING AUTHORIZATION IDS FOR A USER'S PERSONAL ACCESS TOKENS

                // For each user, you can retrieve the authorization IDs corresponding to their personal access tokens as shown below.
                // This is a paginated API, so you may need to keep track of the continuation token
                // and send multiple requests depending on the number of PATs associated with the user.
                // The page size is client-configurable up to a server-side limit.
                // If you pass too high a value, you will receive an error.
                var patPageSize = 20;

                foreach (var user in pageOfUsers.GraphUsers)
                {
                    Guid? patContinuationToken = null;

                    do
                    {
                        // Make a call to retrieve the next page of PATs:
                        // HTTP: GET /_apis/tokenAdmin/personalAccessTokens/{user.Descriptor}[&continuationToken={patContinuationToken}]
                        //       => 200 OK {
                        //                   "count":n,
                        //                   "value":[{
                        //                      "authorizationId":"{pat.AuthorizationId}",
                        //                        …
                        //                   },…],
                        //                   "continuationToken":"{patContinuationToken}"
                        //                 }
                        var pageOfPats = tokenAdminHttpClient.ListPersonalAccessTokensAsync(
                            user.Descriptor, pageSize: patPageSize, continuationToken: patContinuationToken?.ToString()).SyncResult();

                        // In this case, the continuation token is returned in the response body as shown above.
                        patContinuationToken = pageOfPats.ContinuationToken;

                        // Track the authorization IDs from the objects returned in the response:
                        authorizationIds.AddRange(pageOfPats.SessionTokens.Select(pat => pat.AuthorizationId));
                    }
                    // Repeat the above while we still have more pages of PATs:
                    while (patContinuationToken != null && patContinuationToken != Guid.Empty);
                }
            }
            // Repeat the above while we still have more pages of users:
            while (!string.IsNullOrEmpty(userContinuationToken));

            return authorizationIds;
        }

        [ClientSampleMethod(TokenAdminResourceIds.AreaName, TokenAdminResourceIds.RevocationsResource)]
        public void RevokePersonalAccessTokensForUsersInYourOrganization()
        {
            var tokenAdminHttpClient = Context.Connection.GetClient<TokenAdminHttpClient>();

            var authorizationIds = GetPersonalAccessTokenDetailsForUsersInYourOrganization();

            // 3. REVOKING THE AUTHORIZATIONS

            // Once you have the list of authorization IDs you want to revoke, 
            // you can use the following token administration endpoints to revoke them in batch, up to a server-side limit. 
            // Just as we've structure it here, you can revoke a list of authorizations that correspond to multiple users in one call,
            // but keep in mind that you will receive an error if you pass too many revocations at once.

            var revocations = authorizationIds.Select(id => new TokenAdminRevocation { AuthorizationId = id });

            // HTTP: POST /_apis/tokenAdmin/revocations
            //       [{
            //          "authorizationId":"{authorizationId}"
            //        },…]
            //       => 204 No Content
            tokenAdminHttpClient.RevokeAuthorizationsAsync(revocations).SyncResult();
        }

        [ClientSampleMethod(TokenAdminResourceIds.AreaName, TokenAdminResourceIds.RevocationRulesResource)]
        public void RevokeSelfDescribingSessionTokensForUsersInYourOrganization()
        {
            var tokenAdminHttpClient = Context.Connection.GetClient<TokenAdminHttpClient>();

            ////// CREATING OAUTH REVOCATION RULES

            // Not all kinds of OAuth authorizations can be revoked directly.
            // Some, such as self-describing session tokens, must instead by revoked by creating a rule
            // which will be evaluated and used to reject matching OAuth credentials at authentication time.
            // Revocation rules as shown here will apply to all credentials that were issued before the datetime
            // at which the rule was created, and which match a particular OAuth scope.
            // For a list of all OAuth scopes supported by VSTS, see: 
            // https://docs.microsoft.com/en-us/vsts/integrate/get-started/authentication/oauth?view=vsts#scopes

            var scopeToRevoke = "vso.code";
            var revocationRule = new TokenAdminRevocationRule { Scope = scopeToRevoke };

            // HTTP: POST /_apis/tokenAdmin/revocationRules
            //       {
            //         "scope":"{scopeToRevoke}"
            //       }
            //       => 204 No Content
            tokenAdminHttpClient.CreateRevocationRuleAsync(revocationRule).SyncResult();
        }
    }
}