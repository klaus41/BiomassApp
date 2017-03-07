using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Vedligehold.Models;

namespace Vedligehold.Services
{
    public class MaintenanceService : ClientGateway
    {
        string endPoint = "api/maintenancelist/";
        public async Task<MaintenanceTask[]> GetMaintenanceTasksAsync()
        {
            HttpClient client = GetHttpClient();

            var response = await client.GetAsync(endPoint);

            var statsJson = response.Content.ReadAsStringAsync().Result;

            var rootObject = JsonConvert.DeserializeObject<MaintenanceTask[]>(statsJson);

            return rootObject;
        }

        public async Task<MaintenanceTask> UpdateTask(MaintenanceTask task)
        {
            string newendPoint = null; 
            newendPoint = endPoint + "update/" + task.no;
            HttpClient client = GetHttpClient();

            var data = JsonConvert.SerializeObject(task);

            var content = new StringContent(data, Encoding.UTF8, "application/json");

            var response = await client.PutAsync(newendPoint, content);

            return JsonConvert.DeserializeObject<MaintenanceTask>(response.Content.ReadAsStringAsync().Result);
        }

        public async Task<MaintenanceTask> CreateTask(MaintenanceTask task)
        {
            HttpClient client = GetHttpClient();

            var data = JsonConvert.SerializeObject(task);

            var content = new StringContent(data, Encoding.UTF8, "application/json");

            var response = await client.PostAsync(endPoint + "create", content);

            return JsonConvert.DeserializeObject<MaintenanceTask>(response.Content.ReadAsStringAsync().Result);
        }

    }
}
