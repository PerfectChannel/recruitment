using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace PerfectChannel.WebApi.IntegrationTest
{
    public class Tests : IClassFixture<WebApplicationFactory<Startup>>
    {
        private readonly WebApplicationFactory<Startup> _factory;

        private const string AddCommand = "add";
        private const string ChangeStatusCommand = "changeStatus";

        private const string FakeTask1 = "Fake Task 1";
        private const string FakeTask2 = "Fake Task 2";
        private const string FakeTask3 = "Fake Task 3";
        private const string FakeTask4 = "Fake Task 4";

        private const string UrlBase = "api/Task";

        public Tests(WebApplicationFactory<Startup> factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task PostGet_PendingList()
        {
            HttpClient client = _factory.CreateClient();

            // PUT a task
            await AddTask(client, FakeTask1);

            // GET the list
            var lists = await GetList(client);

            // Assert 
            Assert.NotEmpty(lists[0]); // Pending

            Assert.Single(lists[0].Where(q => q.Value == FakeTask1));
        }

        [Fact]
        public async Task PostGet_CompletedList()
        {
            HttpClient client = _factory.CreateClient();

            // PUT a task
            await AddTask(client, FakeTask2);

            // GET the list
            var lists = await GetList(client);

            // Change the status
            await ChangeStatusTask(client, lists[0].Where(q => q.Value == FakeTask2).First().Key);

            // Assert
            lists = await GetList(client);

            Assert.NotEmpty(lists[1]); // Completed

            Assert.Single(lists[1].Where(q => q.Value == FakeTask2));
        }

        [Fact]
        public async Task PostGet_PendingAndCompletedList()
        {
            HttpClient client = _factory.CreateClient();

            // PUT a task
            await AddTask(client, FakeTask3);
            await AddTask(client, FakeTask4);

            // GET the list
            var lists = await GetList(client);

            // Change the status
            await ChangeStatusTask(client, lists[0].Where(q => q.Value == FakeTask3).First().Key);

            // Assert
            lists = await GetList(client);

            Assert.NotEmpty(lists[0]); // Pending
            Assert.NotEmpty(lists[1]); // Completed

            Assert.Single(lists[0].Where(q => q.Value == FakeTask4));
            Assert.Single(lists[1].Where(q => q.Value == FakeTask3));
        }

        private static async Task AddTask(HttpClient client, string taskDescription)
        {
            await PostAsync(client, $"{UrlBase}/{AddCommand}/{taskDescription}");
        }

        private static async Task ChangeStatusTask(HttpClient client, string taskId)
        {
            await PostAsync(client, $"{UrlBase}/{ChangeStatusCommand}/{taskId}");
        }

        /// <summary>
        /// Put an object and check the response headers.
        /// </summary>
        private static async Task PostAsync(HttpClient client, string url)
        {
            HttpResponseMessage response = await client.PostAsync(url, null);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        /// <summary>
        /// Get list calling client.GetAsync
        /// </summary>
        private static async Task<List<List<KeyValuePair<string, string>>>> GetList(HttpClient client)
        {
            HttpResponseMessage response = await client.GetAsync($"{UrlBase}/list");
            response.EnsureSuccessStatusCode();
            Assert.Equal("text/plain", response.Content.Headers.ContentType.MediaType);
            var list = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<List<List<KeyValuePair<string, string>>>>(list);
        }
    }
}
