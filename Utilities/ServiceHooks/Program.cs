using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.TeamServices.Samples.ServiceHooks
{
    public class Program
    {
        static void Main(string[] args)
        {
            string command = (args.Length > 0 ? args[0] : null);

            if (command == null || args.Length < 2)
            {
                Console.WriteLine("Usage: Microsoft.TeamServices.Samples.ServiceHooks [command] [collection URL]");
            }
            else if (String.Equals(command, "restore", StringComparison.InvariantCultureIgnoreCase))
            {
                RestoreManagePermissionsToProjectAdminGroups r = new RestoreManagePermissionsToProjectAdminGroups();
                r.Run(new Uri(args[1]));
            }
            else if(String.Equals(command, "restrict", StringComparison.InvariantCultureIgnoreCase))
            {
                RestrictPermissionsToOneGroup r = new RestrictPermissionsToOneGroup();
                r.Run(new Uri(args[1]));
            }
        }
    }
}
