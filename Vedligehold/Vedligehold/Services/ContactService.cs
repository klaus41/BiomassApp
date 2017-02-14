using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Vedligehold.Models;
using Vedligehold.Services;

namespace Vedligehold.Services
{
    public class ContactService
    {
        public async Task<Contact[]> GetContactsAsync()
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Accept", "application/json");

            client.BaseAddress = new Uri("http://demo.biomass.eliteit.dk/");

            var response = await client.GetAsync("api/contacts/");

            var statsJson = response.Content.ReadAsStringAsync().Result;

            var rootObject = JsonConvert.DeserializeObject<Contact[]>(statsJson);

            return rootObject;
        }
        
    }
}
