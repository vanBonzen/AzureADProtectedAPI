using ClientAzureAD.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Identity.Web;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace ClientAzureAD.Services
{
    public static class PeopleServiceExtensions
    {
        public static void AddPeopleService(this IServiceCollection services, IConfiguration configuration)
        {
            // https://docs.microsoft.com/en-us/dotnet/standard/microservices-architecture/implement-resilient-applications/use-httpclientfactory-to-implement-resilient-http-requests
            services.AddHttpClient<IPeopleService, PeopleService>();
        }
    }

    /// <summary></summary>
    /// <seealso cref="TodoListClient.Services.ITodoListService" />
    public class PeopleService : IPeopleService
    {
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly HttpClient _httpClient;
        private readonly string _PeopleScope = string.Empty;
        private readonly string _PeopleBaseAddress = string.Empty;
        private readonly ITokenAcquisition _tokenAcquisition;

        public PeopleService(ITokenAcquisition tokenAcquisition, HttpClient httpClient, IConfiguration configuration, IHttpContextAccessor contextAccessor)
        {
            this._httpClient = httpClient;
            this._tokenAcquisition = tokenAcquisition;
            this._contextAccessor = contextAccessor;
            this._PeopleScope = configuration["People:PeopleScope"];
            this._PeopleBaseAddress = configuration["People:PeopleBaseAdress"];
        }

        public async Task<Person> AddAsync(Person person)
        {
            await PrepareAuthenticatedClient();

            var jsonRequest = JsonConvert.SerializeObject(person);
            var jsoncontent = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

            var response = await this._httpClient.PostAsync($"{this._PeopleBaseAddress}/api/people", jsoncontent);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                var content = await response.Content.ReadAsStringAsync();
                person = JsonConvert.DeserializeObject<Person>(content);

                return person;
            }

            throw new HttpRequestException($"Invalid status code in the HttpResponseMessage: {response.StatusCode}.");
        }

        public async Task DeleteAsync(string guid)
        {
            await PrepareAuthenticatedClient();

            var response = await this._httpClient.DeleteAsync($"{this._PeopleBaseAddress}/api/people/{guid}");

            if (response.StatusCode == HttpStatusCode.OK)
            {
                return;
            }

            throw new HttpRequestException($"Invalid status code in the HttpResponseMessage: {response.StatusCode}.");
        }

        public async Task<Person> EditAsync(Person person)
        {
            await PrepareAuthenticatedClient();

            var jsonRequest = JsonConvert.SerializeObject(person);
            var jsoncontent = new StringContent(jsonRequest, Encoding.UTF8, "application/json-patch+json");

            var response = await this._httpClient.PatchAsync($"{this._PeopleBaseAddress}/api/people/{person.Id}", jsoncontent);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                var content = await response.Content.ReadAsStringAsync();
                person = JsonConvert.DeserializeObject<Person>(content);

                return person;
            }

            throw new HttpRequestException($"Invalid status code in the HttpResponseMessage: {response.StatusCode}.");
        }

        public async Task<IEnumerable<Person>> GetAsync()
        {
            await PrepareAuthenticatedClient();

            var response = await this._httpClient.GetAsync($"{this._PeopleBaseAddress}/api/People");
            if (response.StatusCode == HttpStatusCode.OK)
            {
                var content = await response.Content.ReadAsStringAsync();
                IEnumerable<Person> people = JsonConvert.DeserializeObject<IEnumerable<Person>>(content);

                return people;
            }

            throw new HttpRequestException($"Invalid status code in the HttpResponseMessage: {response.StatusCode}.");
        }

        private async Task PrepareAuthenticatedClient()
        {
            var accessToken = await this._tokenAcquisition.GetAccessTokenOnBehalfOfUserAsync(new[] { this._PeopleScope });
            Debug.WriteLine($"access token-{accessToken}");
            this._httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            this._httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public async Task<Person> GetAsync(string guid)
        {
            await PrepareAuthenticatedClient();

            var response = await this._httpClient.GetAsync($"{this._PeopleBaseAddress}/api/people/{guid}");
            if (response.StatusCode == HttpStatusCode.OK)
            {
                var content = await response.Content.ReadAsStringAsync();
                Person person = JsonConvert.DeserializeObject<Person>(content);

                return person;
            }

            throw new HttpRequestException($"Invalid status code in the HttpResponseMessage: {response.StatusCode}.");
        }
    }
}

