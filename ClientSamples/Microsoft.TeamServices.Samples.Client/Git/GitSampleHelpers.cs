using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Microsoft.TeamServices.Samples.Client.Git
{
    public class GitSampleHelpers
    {
        public static GitRepository FindAnyRepository(ClientSampleContext context, Guid projectId)
        {
            GitRepository repo;
            if (!FindAnyRepository(context, projectId, out repo))
            {
                throw new Exception("No repositories available. Create a repo in this project and run the sample again.");
            }

            return repo;
        }

        private static bool FindAnyRepository(ClientSampleContext context, Guid projectId, out GitRepository repo)
        {
            // Check if we already have a repo loaded
            if (!context.TryGetValue<GitRepository>("$someRepo", out repo))
            {
                VssConnection connection = context.Connection;
                GitHttpClient gitClient = connection.GetClient<GitHttpClient>();

                using (new ClientSampleHttpLoggerOutputSuppression())
                {
                    // Check if an ID was already set (this could have been provided by the caller)
                    Guid repoId;
                    if (!context.TryGetValue<Guid>("repositoryId", out repoId))
                    {
                        // Get the first repo
                        repo = gitClient.GetRepositoriesAsync(projectId).Result.FirstOrDefault();
                    }
                    else
                    {
                        // Get the details for this repo
                        repo = gitClient.GetRepositoryAsync(repoId.ToString()).Result;
                    }
                }

                if (repo != null)
                {
                    context.SetValue<GitRepository>("$someRepo", repo);
                }
                else
                {
                    // create a project here?
                    throw new Exception("No repos available for running the sample.");
                }
            }

            return repo != null;
        }

        public static string ChooseRefsafeName()
        {
            return $"{ChooseNamePart()}-{ChooseNamePart()}-{ChooseNamePart()}";
        }

        public static string ChooseItemsafeName()
        {
            return $"{ChooseNamePart()}.{ChooseNamePart()}.{ChooseNamePart()}";
        }

        private static string ChooseNamePart()
        {
            if (WordList == null)
            {
                LoadWordList();
            }
            return WordList[Rng.Next(WordList.Count)];
        }

        private static void LoadWordList()
        {
            List<string> words = new List<string>();

            string wordListName = "Microsoft.TeamServices.Samples.Client.Git.WordList.txt";
            using (Stream inputStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(wordListName))
            using (StreamReader reader = new StreamReader(inputStream))
            {
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine().Trim();
                    if (!string.IsNullOrEmpty(line))
                    {
                        words.Add(line);
                    }
                }
                
            }

            WordList = words;
        }

        public static string WithoutRefsPrefix(string refName)
        {
            if (!refName.StartsWith("refs/"))
            {
                throw new Exception("The ref name should have started with 'refs/' but it didn't.");
            }
            return refName.Remove(0, "refs/".Length);
        }

        private static List<string> WordList;
        private static Random Rng = new Random();
    }
}
