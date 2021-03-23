using System;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Linq;

namespace QAReportGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            var projects = GetProjects();
            projects = SonarqubeClient.GetProjectMetrics(projects).GetAwaiter().GetResult();
            string fileName = ExcelClient.GenerateReport(projects);
            EmailClient.SendMail(fileName);
        }

        static List<Project> GetProjects()
        {
            List<Project> projects = new List<Project>();
            XElement configFromFile = XElement.Load(@"ReportConfig.xml");
            IEnumerable<XElement> projectsElement = configFromFile.Element("Projects").Elements();
            foreach (var element in projectsElement)
            {
                var project = new Project()
                {
                    Team = element.Element("Team").Value,
                    Repository = element.Element("Repository").Value,
                    Branch = element.Element("Branch").Value

                };
                projects.Add(project);
            }
            return projects;
        }
    }
}
