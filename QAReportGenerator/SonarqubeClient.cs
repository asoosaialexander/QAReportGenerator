using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.Collections.Generic;

namespace QAReportGenerator
{
    static class SonarqubeClient
    {

        private static string APIUrl = "http://localhost:9000/api/";
        private static readonly HttpClient httpClient;

        static SonarqubeClient()
        {
            httpClient = new HttpClient();
        }
        public static async Task<List<Project>> GetProjectMetrics(List<Project> projects)
        {
            var authCredential = Encoding.UTF8.GetBytes("81482182a8d7eeb9d86985e4b1e844a7eb83cc3e:");
            httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Basic", Convert.ToBase64String(authCredential));
            httpClient.BaseAddress = new Uri(APIUrl);

            foreach (var project in projects)
            {
                HttpResponseMessage response = await
                httpClient.GetAsync(APIUrl +
                "measures/search_history?component=asoosaialexander_"
                + project.Repository
                + "&metrics=coverage,code_smells,vulnerabilities&p=1&from=2021-02-01");
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
    }
}
