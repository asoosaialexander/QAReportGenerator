using System;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;

namespace QAReportGenerator
{
    class Program
    {
        public static IConfigurationRoot configurationRoot;

        static async Task Main(string[] args)
        {
            using IHost host = CreateHostBuilder(args).Build();

            // Application code should start here.
            var projects = GetProjects();
            projects = SonarqubeClient.GetProjectMetrics(projects).GetAwaiter().GetResult();
            string fileName = ExcelClient.GenerateReport(projects);
            EmailClient.SendMail(fileName);

            await host.RunAsync();
        }

        static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((hostingContext, configuration) =>
            {
                configuration.Sources.Clear();
                configuration.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
                configuration.AddEnvironmentVariables();

                configurationRoot = configuration.Build();
            });

        static List<Project> GetProjects()
        {
            List<Project> projects = new List<Project>();
            XElement projectsXml = XElement.Load(@"Projects.xml");
            IEnumerable<XElement> projectsElement = projectsXml.Elements();
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
