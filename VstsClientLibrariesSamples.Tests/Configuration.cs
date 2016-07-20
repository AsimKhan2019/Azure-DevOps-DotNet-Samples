using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.VisualStudio.Services.Common;

namespace VstsClientLibrariesSamples.Tests
{
    public class Configuration : IConfiguration
    {
        private Uri _uri = new Uri("https://accountname.visualstudio.com/");
        private VssBasicCredential _vssBasicCredential = new VssBasicCredential("", "personal access token");
        private string _projectName = "project name";
        private string _identity = "artvandelay@hotmail.com";
        private string _queryName = "Shared Queries/Current Iteration/Open User Stories";

        public Uri Uri {
            get {
                return _uri;
            }            
        }

        public VssBasicCredential Credentials {
            get {
                return _vssBasicCredential;
            }           
        }

        public string ProjectName {
            get {
                return _projectName;
            } 
        }

        public string QueryName {
            get {
                return _queryName;
            }
        }

        public string Identity {
            get {
                return _identity;
            }
       }
    }
}
