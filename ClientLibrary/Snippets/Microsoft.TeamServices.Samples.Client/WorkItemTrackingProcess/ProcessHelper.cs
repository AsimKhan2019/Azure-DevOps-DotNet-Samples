using Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.Models;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamServices.Samples.Client.WorkItemTrackingProcess
{
    public static class ProcessHelper
    {
        public static Page getPage(FormLayout layout, string pageName)
        {
            List<Page> pages = layout.Pages as List<Page>;
            Page page = pages.Find(x => x.Label == pageName);

            return page;
        }

        public static Section getSection(FormLayout layout, string pageName, string sectionName)
        {
            Page page = getPage(layout, pageName);

            List<Section> sections = page.Sections as List<Section>;
            Section section = sections.Find(x => x.Id == sectionName);

            return section;
        }

        public static Group getGroup(FormLayout layout, string pageName, string sectionName, string groupName)
        {
            Section section = getSection(layout, pageName, sectionName);

            List<Group> groups = section.Groups as List<Group>;
            Group group = groups.Find(x => x.Label == groupName);

            return group;
        }
    }
    static class OutOfBoxProcessTemplateTypeIds
    {
        public static readonly Guid Agile = new Guid("ADCC42AB-9882-485E-A3ED-7678F01F66BC");
        public static readonly Guid Scrum = new Guid("6B724908-EF14-45CF-84F8-768B5384DA45");
        public static readonly Guid Cmmi = new Guid("27450541-8E31-4150-9947-DC59F998FC01");
    }
}
