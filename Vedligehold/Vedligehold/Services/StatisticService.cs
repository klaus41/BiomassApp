using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Vedligehold.Models;

namespace Vedligehold.Services
{
    public class StatisticService
    {
        public async Task<Statistic[]> GetStatsAsync(string id)
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Accept", "application/json");

            client.BaseAddress = new Uri("http://Bevtoft.biomass.eliteit.dk/");

            var response = await client.GetAsync("api/statistik/" + id);

            var statsJson = response.Content.ReadAsStringAsync().Result;

            var rootObject = JsonConvert.DeserializeObject<Statistic[]>(statsJson);

            return rootObject;
        }
    }
}
