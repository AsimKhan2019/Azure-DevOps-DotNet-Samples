using System;
using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Services.WebApi;
using System.Net.Http;
using Microsoft.VisualStudio.Services.Common;
using System.Runtime.Serialization;

namespace Microsoft.TeamServices.Samples.Client
{
    /// <summary>
    /// Base class that all client samples extend from.
    /// </summary>
    [InheritedExport]
    public abstract class ClientSample
    {
        public ClientSampleContext Context { get; set; }

    }

    /// <summary>
    /// Interface representing a client sample method. Provides a way to discover client samples for a particular area, resource, or operation.
    /// </summary>
    public interface IClientSampleMethodInfo
    {
        string Area { get; }

        string Resource { get; }

        string Operation { get; }
    }


    [DataContract]
    public class ClientSampleMethodInfo : IClientSampleMethodInfo
    {
        [DataMember(EmitDefaultValue = false, Name = "x-ms-vss-area")]
        public string Area { get; set; }

        [DataMember(EmitDefaultValue = false, Name = "x-ms-vss-resource")]
        public string Resource { get; set; }

        [DataMember(EmitDefaultValue = false, Name = "x-ms-vss-operation")]
        public string Operation { get; set; }
    }

    /// <summary>
    /// Attribute applied to all client samples. Optionally indicates the API "area" and/or "resource" the sample is associatd with.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class ClientSampleAttribute : ExportAttribute
    {
        public string Area { get; private set; }

        public string Resource { get; private set; }

        public ClientSampleAttribute(String area = null, String resource = null) : base(typeof(ClientSample))
        {
            this.Area = area;
            this.Resource = resource;
        }
    }

    /// <summary>
    /// Attribute applied to methods within a client sample. Allow overriding the area or resource of the containing client sample.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class ClientSampleMethodAttribute : Attribute, IClientSampleMethodInfo
    {
        public string Area { get; internal set; }

        public string Resource { get; internal set; }

        public string Operation { get; internal set; }

        public ClientSampleMethodAttribute(String area = null, String resource = null, String operation = null)
        {
            this.Area = area;
            this.Resource = resource;
            this.Operation = operation;
        }
    }
}