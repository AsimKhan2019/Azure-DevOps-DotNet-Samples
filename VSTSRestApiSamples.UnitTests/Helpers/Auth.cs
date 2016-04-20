using System;
using VstsRestApiSamples.Client.Helpers;

namespace VstsRestApiSamples.Tests.Client.Helpers
{
    public class Auth : IAuth
    {
        private string _account = "https://accountname.visualstudio.com/DefaultCollection/";
        private string _login = Convert.ToBase64String(System.Text.ASCIIEncoding.ASCII.GetBytes(string.Format("{0}:{1}", "", "<personal access token>")));
        private string _project = "<project name>";
        private string _processId = "<process id>";
        private string _picklistId = "<picklist id>";
        private string _queryId = "<query id>";
        private string _workItemId = "<work item id>";

        public string Account
        {
            get
            {
                return _account;
            }
        }

        public string Login
        {
            get
            {
                return _login;
            }
        }

        public string Project
        {
            get
            {
                return _project;
            }
        }

        public string ProcessId
        {
            get
            {
                return _processId;
            }
        }

        public string PickListId
        {
            get
            {              
                return _picklistId;
            }
        }

        public string QueryId
        {
            get
            {               
                return _queryId;
            }
        }

        public string WorkItemId
        {
            get
            {
                return _workItemId;
            }
        }
    }
}
