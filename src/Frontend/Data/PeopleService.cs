using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Entities;
using System.Net.Http;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using Microsoft.Extensions.Caching.Distributed;

namespace Frontend.Data
{
    public class PeopleService : IPeopleService
    {
        private string _baseUrl;
        private IDistributedCache _cache;

        public PeopleService(string baseUrl, IDistributedCache cache)
        {
            _baseUrl = baseUrl;
            _cache = cache;
        }

        public async Task<Person> CreateAsync(Person person)
        {
            var client = new HttpClient();

            var content = new StringContent(JsonConvert.SerializeObject(person));
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            var response = await client.PostAsync(_baseUrl, content);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadAsStringAsync();
            var createdPerson = JsonConvert.DeserializeObject<Person>(result);

            await _cache.RemoveAsync("person.all");
            await _cache.SetStringAsync($"person.{createdPerson.Id}", result);

            return createdPerson;
        }

        public async Task DeleteAsync(string id)
        {
            var client = new HttpClient();

            var response = await client.DeleteAsync(_baseUrl + "/" + id);
            response.EnsureSuccessStatusCode();

            await _cache.RemoveAsync("person.all");
            await _cache.RemoveAsync($"person.{id}");
        }

        public async Task<Person> GetByIdAsync(string id)
        {
            string result = await _cache.GetStringAsync($"person.{id}");

            if (result == null)
            {
                var client = new HttpClient();

                var response = await client.GetAsync(_baseUrl + "/" + id);
                response.EnsureSuccessStatusCode();

                result = await response.Content.ReadAsStringAsync();

                await _cache.SetStringAsync($"person.{id}", result);
            }

            return JsonConvert.DeserializeObject<Person>(result);
        }

        public async Task<IEnumerable<Person>> GetListAsync()
        {
            string result = await _cache.GetStringAsync("person.all");

            if (result == null)
            {
                var client = new HttpClient();

                var response = await client.GetAsync(_baseUrl);
                response.EnsureSuccessStatusCode();

                result = await response.Content.ReadAsStringAsync();

                await _cache.SetStringAsync("person.all", result);
            }

            return JsonConvert.DeserializeObject<IEnumerable<Person>>(result);
        }

        public async Task UpdateAsync(Person person)
        {
            var client = new HttpClient();

            var value = JsonConvert.SerializeObject(person);

            var content = new StringContent(value);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            var response = await client.PutAsync(_baseUrl + "/" + person.Id.ToString(), content);
            response.EnsureSuccessStatusCode();

            await _cache.RemoveAsync("person.all");
            await _cache.SetStringAsync($"person.{person.Id}", value);
        }
    }
}
