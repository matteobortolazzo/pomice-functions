using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Flurl;
using Flurl.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json;

namespace PomiceFunctions
{
    public static class UpdateAppVersion
    {
        [FunctionName("update-app-version")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Function, "post", Route = null)]HttpRequestMessage req, TraceWriter log)
        {
            dynamic releaseData = await req.Content.ReadAsAsync<object>();
            var buildName = (string)releaseData.resource.deployment.release.artifacts[0].definitionReference.version.name;

            string GetEnvironmentVariable(string name)
            {
                return Environment.GetEnvironmentVariable(name, EnvironmentVariableTarget.Process);
            }

            var spaceId = GetEnvironmentVariable("CONTENTFUL_SPACEID");
            var environment = GetEnvironmentVariable("CONTENTFUL_ENVIRONMENT");
            var contentManagementToken = GetEnvironmentVariable("CONTENTFUL_CONTENTMANAGEMENTTOKEN");
            var contentManagementUrl = $"https://api.contentful.com/spaces/{spaceId}/environments/{environment}";

            async Task<ContentfulEntity> GetLatestEntry()
            {
                var collection = await contentManagementUrl
                .AppendPathSegments("entries")
                .SetQueryParam("content_type", "appInfo")
                .WithOAuthBearerToken(contentManagementToken)
                .GetJsonAsync<CtfCollection>();
                return collection.Items.First();
            }

            var entry = await GetLatestEntry();
            entry.Fields.Version.Field = buildName;
            await contentManagementUrl
               .AppendPathSegments("entries", entry.Sys.Id)
               .WithOAuthBearerToken(contentManagementToken)
               .WithHeader("X-Contentful-Version", entry.Sys.Version)
               .PutJsonAsync(new
               {
                   fields = entry.Fields
               });

            entry = await GetLatestEntry();
            var result = await contentManagementUrl
               .AppendPathSegments("entries", entry.Sys.Id, "published")
               .WithOAuthBearerToken(contentManagementToken)
               .WithHeader("X-Contentful-Version", entry.Sys.Version)
               .PutJsonAsync(new { });

            if (result.IsSuccessStatusCode)
                return req.CreateResponse(HttpStatusCode.OK, "Version " + buildName + " online");
            return req.CreateResponse(HttpStatusCode.BadRequest);
        }

        class CtfCollection
        {
            [JsonProperty("items")]
            public IEnumerable<ContentfulEntity> Items { get; set; }
        }
        class ContentfulEntity
        {
            [JsonProperty("sys")]
            public ContentfulSys Sys { get; set; }
            [JsonProperty("fields")]
            public AppInfoFields Fields { get; set; }
        }
        class ContentfulSys
        {
            [JsonProperty("id")]
            public string Id { get; set; }
            [JsonProperty("version")]
            public string Version { get; set; }
        }
        class AppInfoFields
        {
            [JsonProperty("name")]
            public ContentfilField Name { get; set; }
            [JsonProperty("version")]
            public ContentfilField Version { get; set; }
        }
        class ContentfilField
        {
            [JsonProperty("en-US")]
            public string Field { get; set; }
        }
    }
}
