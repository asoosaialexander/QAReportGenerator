using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;

namespace QAReportGenerator
{
    static class SonarqubeClient
    {

        private static IConfigurationRoot config;
        private static readonly HttpClient httpClient;

        static SonarqubeClient()
        {
            config = Program.configurationRoot;
            httpClient = new HttpClient();

            var token = config["Token"];
            var authCredential = Encoding.UTF8.GetBytes(token + ":");
            httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Basic", Convert.ToBase64String(authCredential));
            httpClient.BaseAddress = new Uri(config["SonarqubeApi"]);
        }
        public static async Task<List<Project>> GetProjectMetrics(List<Project> projects)
        {
            foreach (var project in projects)
            {
                HttpResponseMessage response = await
                httpClient.GetAsync(config["SonarqubeApi"] +
                "measures/search_history?component="
                + project.Repository
                + "&metrics=coverage,code_smells,vulnerabilities");
                if (response.IsSuccessStatusCode)
                {
                    var readTask = response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    var rawResponse = readTask.GetAwaiter().GetResult();
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };
                    var obj = JsonSerializer.Deserialize<Metrics>(rawResponse, options);
                    project.Metrics = obj;
                }
            }

            return projects;
        }

        public static async Task<List<Author>> GetVulnerabilities(List<Author> authors)
        {
            foreach (var author in authors)
            {
                HttpResponseMessage response = await
                httpClient.GetAsync(config["SonarqubeApi"] +
                 "issues/search?ststuses=OPEN&author=" + author.EmailAddress);
                if (response.IsSuccessStatusCode)
                {
                    var readTask = response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    var rawResponse = readTask.GetAwaiter().GetResult();
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };
                    var obj = JsonSerializer.Deserialize<Vulnerabilities>(rawResponse, options);
                    author.Vulnerabilities = obj;
                }
            }

            return authors;
        }
    }
}
