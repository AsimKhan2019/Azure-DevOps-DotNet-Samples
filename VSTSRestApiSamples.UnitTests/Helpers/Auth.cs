using System;
using VstsRestApiSamples.Client.Helpers;

namespace VstsRestApiSamples.Tests.Client.Helpers
{
    public class Auth : IAuth
    {
        private string _account = "https://danhellem.visualstudio.com/DefaultCollection/";
        private string _login = Convert.ToBase64String(System.Text.ASCIIEncoding.ASCII.GetBytes(string.Format("{0}:{1}", "", "4tjp7stxlnjm6pd2tcyj2re2pzqawei45eiqziywf6b3sspeeeiq")));
        private string _project = "MedIntake";
        private string _processId = "f11f0070-2d42-41c4-a01b-892cd0183dd3";
        private string _picklistId = "710e528c-54e1-4591-8d7f-51b3e5862bfe";
        private string _queryId = "c5699578-595d-409c-a5ea-e4cf549e0ba3";
        private string _workItemId = "2768";

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
