using System;
using VstsRestApiSamples.Client.Helpers;

namespace VstsRestApiSamples.Tests.Client.Helpers
{
    public class Auth : IAuth
    {
        private string _account;
        private string _login;
        private string _project;
        private string _processId;
        private string _picklistId;

        public string Account
        {
            get
            {
                _account = "https://fabrikam.visualstudio.com/defaultCollection/";

                return _account;
            }
        }

        public string Login
        {
            get
            {
                _login = Convert.ToBase64String(System.Text.ASCIIEncoding.ASCII.GetBytes(string.Format("{0}:{1}", "", "<personal access token string>")));


                return _login;
            }           
        } 

        public string Project
        {
            get
            {
                _project = "Project Name";

                return _project;
            }
        }

        public string ProcessId
        {
            get
            {
                _processId = "f11f0070-2d42-41c4-a01b-892cd0183dd3";
                return _processId;
            }            
        }

        public string PickListId
        {
            get
            {
                _picklistId = "710e528c-54e1-4591-8d7f-51b3e5862bfe";
                return _picklistId;
            }
        }
    }
}
