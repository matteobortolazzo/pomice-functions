using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Flurl;
using Flurl.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;

namespace PomiceFunctions
{
    public static class QueuePrerendering
    {
        [FunctionName("queue-prerendering")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Function, "post", Route = null)]HttpRequestMessage req, TraceWriter log)
        {
            string GetEnvironmentVariable(string name)
            {
                return Environment.GetEnvironmentVariable(name, EnvironmentVariableTarget.Process);
            }
            string ConvertToBase64(string personalToken)
            {
                return Convert.ToBase64String(
                    Encoding.ASCII.GetBytes(
                        string.Format("{0}:{1}", "", personalToken)));
            }
            var organization = GetEnvironmentVariable("DEVOPS_ORGANIZATION");
            var project = GetEnvironmentVariable("DEVOPS_PROJECT");
            var buildDefinitionId = GetEnvironmentVariable("DEVOPS_BUILDDEFINITIONID");
            var token = GetEnvironmentVariable("DEVOPS_ACCESSTOKEN");
            var base64Token = ConvertToBase64(token);
           
            var result = await $"https://dev.azure.com/{organization}/{project}/_apis/build/builds?api-version=5.0-preview.5"
                .WithHeader("Authorization", $"Basic {base64Token}")
                .PostJsonAsync(new {
                    definition = new
                    {
                        id = int.Parse(buildDefinitionId)
                    }
                });
            if (result.IsSuccessStatusCode)
                return req.CreateResponse(HttpStatusCode.OK, "Prerendering build queued.");
            return req.CreateResponse(HttpStatusCode.BadRequest);
        }
    }
}
