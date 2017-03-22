using System;

namespace VstsSamples.Client.Utils
{
    public class ClientSampleProgram
    {

        public static int Main(string[] args)
        {
            if (args.length == 0)
            {
                ShowUsage();
            } 
            else
            {
                Uri connectionUrl;
                string area, resource;

                try
                {
                    CheckArguments(out connectionUrl, out area, out resource);
                }
                catch (ArgumentException ex)
                {
                    Console.WriteLine(ex.Message);
                    return -1;
                }

                ClientSampleMethod samples = ClientSampleUtils.GetClientSampleMethods(area, resource);
                foreach (Clien)
            }
        }

        private void CheckArguments(our Uri connectionUrl, out string area, out string resource)
        {
            try 
            {
                connectionUrl = new Uri(args[0]);
            } 
            catch (Exception)
            {
                throw new ArgumentException("Invalid URL");
            }
            
            if (args.length > 1)
            {
                area = args[1];
                if (!IsValidArea(area))
                {
                    throw new ArgumentException("Invalid area. Supported areas: {0}.", String.Join(", ", GetSupportedAreas()));     
                }

                if (args.length > 2)
                {
                    resource = args[2];
                    if (!IsValidResource(area, resource)) 
                    {
                        throw new ArgumentException("Invalid resource. Supported resources for {0}: {1}.", area, String.Join(", ", GetSupportedAreas()));
                    }
                }
            }
            else
            {
                area = null;
                resource = null;
            }
        }

        private static void ShowUsage() {
            Console.WriteLine("Run the client samples on a Team Services account or Team Foundation Server instance.")
            Console.WriteLine("");
            Console.WriteLine("WARNING: Some samples are destructive. Always run on a test account or collection.");
            Console.WriteLine("");
            Console.WriteLine("Usage: ClientSampleProgram url [area [resource]]");
            Console.WriteLine("");
            Console.WriteLine(" url        URL for the account or collection to run the samples on");
            Console.WriteLine("            Example: https://fabrikam.visualstudio.com");
            Console.WriteLine(" area       Run only samples for this area, otherwise run the samples for all areas.");
            Console.WriteLine(" resource   Run only samples for this resource, otherwise run the samples for all resources under this area (or all areas).");
        }

    }

}