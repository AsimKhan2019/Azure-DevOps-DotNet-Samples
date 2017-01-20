using System.Net;

namespace VstsRestApiSamples.ViewModels
{
    public class BaseViewModel
    {
        public HttpStatusCode HttpStatusCode { get; set; }
        public string Message { get; set; }
    }
}
