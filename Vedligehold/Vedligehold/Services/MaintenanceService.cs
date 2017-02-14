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
    public class MaintenanceService
    {
        public async Task<MaintenanceTask[]> GetMaintenanceTasksAsync()
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Accept", "application/json");

            client.BaseAddress = new Uri("http://demo.biomass.eliteit.dk/");

            var response = await client.GetAsync("api/maintenancelist/");

            var statsJson = response.Content.ReadAsStringAsync().Result;

            var rootObject = JsonConvert.DeserializeObject<MaintenanceTask[]>(statsJson);

            return rootObject;
        }
    }
}
