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
    public class StatisticService : ClientGateway
    {
        public async Task<Statistic[]> GetStatsAsync(string id)
        {
            try
            {
                HttpClient client = GetHttpClient();

                var response = await client.GetAsync("api/statistik/" + id);

                var statsJson = response.Content.ReadAsStringAsync().Result;

                var rootObject = JsonConvert.DeserializeObject<Statistic[]>(statsJson);

                return rootObject;
            }
            catch
            {
                Statistic[] jl = null;
                return jl;
            }
        }
    }
}
