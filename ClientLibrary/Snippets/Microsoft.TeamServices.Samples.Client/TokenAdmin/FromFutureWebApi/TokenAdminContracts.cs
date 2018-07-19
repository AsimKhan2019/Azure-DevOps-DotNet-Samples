using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Microsoft.VisualStudio.Services.DelegatedAuthorization;

namespace Microsoft.VisualStudio.Services.TokenAdmin.Client
{
    public static class TokenAdminResourceIds
    {
        public const string AreaName = "TokenAdmin";
        public const string AreaId = "af68438b-ed04-4407-9eb6-f1dbae3f922e";

        public const string PersonalAccessTokensResource = "PersonalAccessTokens";
        public static readonly Guid PersonalAccessTokensLocationId = new Guid("{af68438b-ed04-4407-9eb6-f1dbae3f922e}");

        public const string RevocationsResource = "Revocations";
        public static readonly Guid RevocationsLocationId = new Guid("{a9c08b2c-5466-4e22-8626-1ff304ffdf0f}");

        public const string RevocationRulesResource = "RevocationRules";
        public static readonly Guid RevocationRulesLocationId = new Guid("{ee4afb16-e7ab-4ed8-9d4b-4ef3e78f97e4}");
    }

    /// <summary>
    /// A paginatated list of session tokens.
    /// Session tokens correspond to OAuth credentials such as personal access tokens (PATs)
    /// and other OAuth authorizations.
    /// </summary>
    [DataContract]
    public class TokenAdminPagedSessionTokens
    {
        /// <summary>
        /// The list of all session tokens in the current page.
        /// </summary>
        [DataMember(Name = "Value")]
        public IEnumerable<SessionToken> SessionTokens { get; set; }

        /// <summary>
        /// The continuation token that can be used to retrieve the next page of session tokens,
        /// or <code>null</code> if there is no next page.
        /// </summary>
        [DataMember]
        public Guid? ContinuationToken { get; set; }
    }
    
    /// <summary>
    /// A rule which is applied to disable any incoming delegated authorization
    /// which matches the given properties.
    /// </summary>
    public class TokenAdminRevocationRule
    {
        /// <summary>
        /// The OAuth scope for which matching authorizations should be rejected.
        /// For a list of all OAuth scopes supported by VSTS, see:
        /// https://docs.microsoft.com/en-us/vsts/integrate/get-started/authentication/oauth?view=vsts#scopes
        /// </summary>
        public string Scope { get; set; }
    }

    /// <summary>
    /// A request to revoke a particular delegated authorization.
    /// </summary>
    public class TokenAdminRevocation
    {
        /// <summary>
        /// The authorization ID of the OAuth authorization to revoke.
        /// </summary>
        public Guid AuthorizationId { get; set; }
    }
}
