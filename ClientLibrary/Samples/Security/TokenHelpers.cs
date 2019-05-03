using System;
using System.Diagnostics;
using System.Text;

namespace Microsoft.Azure.DevOps.ClientSamples.Security
{
    public static class TokenHelpers
    {
        public static string CalculateGitAllReposToken(Guid projectId)
        {
            return CalculateGitToken(projectId, Guid.Empty, null, null);
        }

        public static string CalculateGitRepoToken(Guid projectId, Guid repositoryId)
        {
            return CalculateGitToken(projectId, repositoryId, null, null);
        }

        public static string CalculateGitBranchToken(Guid projectId, Guid repositoryId, string refName)
        {
            return CalculateGitToken(projectId, repositoryId, "refs/heads/", refName);
        }

        private static string CalculateGitToken(Guid projectId, Guid repositoryId, string refFirstTwoParts, string refName)
        {
            ValidateGitTokenInputs(projectId, repositoryId, refName);

            StringBuilder securable = new StringBuilder(GitTokenRoot);

            // Append the team project GUID
            if (projectId != Guid.Empty)
            {
                securable.Append(projectId);
                securable.Append("/");

                // Append the repository GUID if applicable.
                if (repositoryId != Guid.Empty)
                {
                    securable.Append(repositoryId.ToString());
                    securable.Append("/");

                    // Append the ref name if one is provided.
                    // The security namespace is case insensitive; Git ref names are case sensitive.
                    // Encode the ref name into a case-insensitive format.
                    // To save space, the first two components of the ref (refs/heads/, refs/tags/, etc.) are not hashed.
                    if (!String.IsNullOrEmpty(refName))
                    {
                        refName = refName.TrimEnd('/');

                        // Append the first two parts as-is.
                        securable.Append(refFirstTwoParts);

                        // Translate the ref name.
                        string[] nameParts = refName.Split('/');

                        // Append each encoded section and cap it off with a slash.
                        foreach (string namePart in nameParts)
                        {
                            securable.Append(StringFromByteArray(Encoding.Unicode.GetBytes(namePart)));
                            securable.Append('/');
                        }
                    }
                }
            }

            return securable.ToString();
        }

        private static void ValidateGitTokenInputs(Guid projectId, Guid repositoryId, string refName)
        {
            // If you pass in a repositoryId, you must pass in a team project
            Debug.Assert(projectId != Guid.Empty || repositoryId == Guid.Empty);

            // If you pass in a ref name, then you must pass in a repository id
            Debug.Assert(string.IsNullOrEmpty(refName) || repositoryId != Guid.Empty);

            // Total ref name length must be under a certain size
            Debug.Assert(refName.Length <= GitTokenMaxRefLength);
        }

        private static string StringFromByteArray(byte[] byteArray)
        {
            if (null == byteArray)
            {
                throw new ArgumentNullException("byteArray");
            }

            StringBuilder sb = new StringBuilder(byteArray.Length * 2);

            for (int i = 0; i < byteArray.Length; i++)
            {
                byte b = byteArray[i];

                char first = (char)(((b >> 4) & 0x0F) + 0x30);
                char second = (char)((b & 0x0F) + 0x30);

                sb.Append(first >= 0x3A ? (char)(first + 0x27) : first);
                sb.Append(second >= 0x3A ? (char)(second + 0x27) : second);
            }

            return sb.ToString();
        }

        private const string GitTokenRoot = "repoV2/";
        private const int GitTokenMaxRefLength = 400;
    }
}
