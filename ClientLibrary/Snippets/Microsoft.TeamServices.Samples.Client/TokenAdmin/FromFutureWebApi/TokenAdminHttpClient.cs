/*
 * ---------------------------------------------------------
 * Copyright(C) Microsoft Corporation. All rights reserved.
 * ---------------------------------------------------------
 *
 * ---------------------------------------------------------
 * Generated file, DO NOT EDIT
 * ---------------------------------------------------------
 *
 * See following wiki page for instructions on how to regenerate:
 *   https://vsowiki.com/index.php?title=Rest_Client_Generation
 *
 * Configuration file:
 *   vssf\client\webapi\httpclients\clientgeneratorconfigs\tokenadmin.genclient.json
 */

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;

namespace Microsoft.VisualStudio.Services.TokenAdmin.Client
{
    [ResourceArea(TokenAdminResourceIds.AreaId)]
    public class TokenAdminHttpClient : VssHttpClientBase
    {
        public TokenAdminHttpClient(Uri baseUrl, VssCredentials credentials)
            : base(baseUrl, credentials)
        {
        }

        public TokenAdminHttpClient(Uri baseUrl, VssCredentials credentials, VssHttpRequestSettings settings)
            : base(baseUrl, credentials, settings)
        {
        }

        public TokenAdminHttpClient(Uri baseUrl, VssCredentials credentials, params DelegatingHandler[] handlers)
            : base(baseUrl, credentials, handlers)
        {
        }

        public TokenAdminHttpClient(Uri baseUrl, VssCredentials credentials, VssHttpRequestSettings settings, params DelegatingHandler[] handlers)
            : base(baseUrl, credentials, settings, handlers)
        {
        }

        public TokenAdminHttpClient(Uri baseUrl, HttpMessageHandler pipeline, bool disposeHandler)
            : base(baseUrl, pipeline, disposeHandler)
        {
        }

        /// <summary>
        /// [Preview API] Lists of all the session token details of the personal access tokens (PATs) for a particular user.
        /// </summary>
        /// <param name="subjectDescriptor">The descriptor of the target user.</param>
        /// <param name="pageSize">The maximum number of results to return on each page.</param>
        /// <param name="continuationToken">An opaque data blob that allows the next page of data to resume immediately after where the previous page ended. The only reliable way to know if there is more data left is the presence of a continuation token.</param>
        /// <param name="userState"></param>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        public Task<TokenAdminPagedSessionTokens> ListPersonalAccessTokensAsync(
            SubjectDescriptor subjectDescriptor,
            int? pageSize = null,
            string continuationToken = null,
            object userState = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            HttpMethod httpMethod = new HttpMethod("GET");
            Guid locationId = new Guid("af68438b-ed04-4407-9eb6-f1dbae3f922e");
            object routeValues = new { subjectDescriptor = subjectDescriptor };

            List<KeyValuePair<string, string>> queryParams = new List<KeyValuePair<string, string>>();
            if (pageSize != null)
            {
                queryParams.Add("pageSize", pageSize.Value.ToString(CultureInfo.InvariantCulture));
            }
            if (!string.IsNullOrEmpty(continuationToken))
            {
                queryParams.Add("continuationToken", continuationToken);
            }

            return SendAsync<TokenAdminPagedSessionTokens>(
                httpMethod,
                locationId,
                routeValues: routeValues,
                version: new ApiResourceVersion("5.0-preview.1"),
                queryParameters: queryParams,
                userState: userState,
                cancellationToken: cancellationToken);
        }

        /// <summary>
        /// [Preview API] Creates a revocation rule to prevent the further usage of any OAuth authorizations that were created before the current point in time and which match the conditions in the rule.
        /// </summary>
        /// <param name="revocationRule">The revocation rule to create. The rule must specify a scope, after which preexisting OAuth authorizations that match that scope will be rejected. For a list of all OAuth scopes supported by VSTS, see: https://docs.microsoft.com/en-us/vsts/integrate/get-started/authentication/oauth?view=vsts#scopes</param>
        /// <param name="userState"></param>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        public async Task CreateRevocationRuleAsync(
            TokenAdminRevocationRule revocationRule,
            object userState = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            HttpMethod httpMethod = new HttpMethod("POST");
            Guid locationId = new Guid("ee4afb16-e7ab-4ed8-9d4b-4ef3e78f97e4");
            HttpContent content = new ObjectContent<TokenAdminRevocationRule>(revocationRule, new VssJsonMediaTypeFormatter(true));

            using (HttpResponseMessage response = await SendAsync(
                httpMethod,
                locationId,
                version: new ApiResourceVersion("5.0-preview.1"),
                userState: userState,
                cancellationToken: cancellationToken,
                content: content).ConfigureAwait(false))
            {
                return;
            }
        }

        /// <summary>
        /// [Preview API] Revokes the listed OAuth authorizations.
        /// </summary>
        /// <param name="revocations">The list of objects containing the authorization IDs of the OAuth authorizations, such as session tokens retrieved by listed a users PATs, that should be revoked.</param>
        /// <param name="userState"></param>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        public async Task RevokeAuthorizationsAsync(
            IEnumerable<TokenAdminRevocation> revocations,
            object userState = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            HttpMethod httpMethod = new HttpMethod("POST");
            Guid locationId = new Guid("a9c08b2c-5466-4e22-8626-1ff304ffdf0f");
            HttpContent content = new ObjectContent<IEnumerable<TokenAdminRevocation>>(revocations, new VssJsonMediaTypeFormatter(true));

            using (HttpResponseMessage response = await SendAsync(
                httpMethod,
                locationId,
                version: new ApiResourceVersion("5.0-preview.1"),
                userState: userState,
                cancellationToken: cancellationToken,
                content: content).ConfigureAwait(false))
            {
                return;
            }
        }
    }
}
