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
    public class TimeRegistrationService : ClientGateway
    {
        string endPoint = "api/timeregistration/";

        public async Task<TimeRegistrationModel[]> GetTimeRegsAsync()
        {
            HttpClient client = GetHttpClient();

            var response = await client.GetAsync(endPoint);

            var statsJson = response.Content.ReadAsStringAsync().Result;

            var rootObject = JsonConvert.DeserializeObject<TimeRegistrationModel[]>(statsJson);

            return rootObject;
        }

        public async Task<TimeRegistrationModel> UpdateTimeReg(TimeRegistrationModel timeReg)
        {
            string newendPoint = null;
            newendPoint = endPoint + "update/" + timeReg.No;
            HttpClient client = GetHttpClient();

            var data = JsonConvert.SerializeObject(timeReg);

            var content = new StringContent(data, Encoding.UTF8, "application/json");

            var response = await client.PutAsync(newendPoint, content);

            return JsonConvert.DeserializeObject<TimeRegistrationModel>(response.Content.ReadAsStringAsync().Result);
        }

        public async Task<MaintenanceTask> CreateTimeReg(TimeRegistrationModel timeReg)
        {
            HttpClient client = GetHttpClient();

            var data = JsonConvert.SerializeObject(timeReg);

            var content = new StringContent(data, Encoding.UTF8, "application/json");

            var response = await client.PostAsync(endPoint + "create", content);

            return JsonConvert.DeserializeObject<MaintenanceTask>(response.Content.ReadAsStringAsync().Result);
        }
    }
}
