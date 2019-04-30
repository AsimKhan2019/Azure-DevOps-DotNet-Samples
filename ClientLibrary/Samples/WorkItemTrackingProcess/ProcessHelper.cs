using Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.Models;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;

namespace Microsoft.Azure.DevOps.ClientSamples.WorkItemTrackingProcess
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
}
