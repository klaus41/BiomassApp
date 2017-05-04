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
    public class JobRecLineService : ClientGateway
    {
        string endPoint = "api/JobRecLine/";
        public async Task<JobRecLine[]> GetJobRecLines()
        {
            try
            {
                HttpClient client = GetHttpClient();

                var response = await client.GetAsync(endPoint);

                var statsJson = response.Content.ReadAsStringAsync().Result;

                var rootObject = JsonConvert.DeserializeObject<JobRecLine[]>(statsJson);

                return rootObject;
            }
            catch
            {
                JobRecLine[] jl = null;
                return jl;
            }
        }

        public async Task<JobRecLine> UpdateJobRecLine(JobRecLine task)
        {
            try
            {
                string newendPoint = null;
                newendPoint = endPoint + "update/";
                HttpClient client = GetHttpClient();

                var data = JsonConvert.SerializeObject(task);

                var content = new StringContent(data, Encoding.UTF8, "application/json");

                var response = await client.PutAsync(newendPoint, content);

                return JsonConvert.DeserializeObject<JobRecLine>(response.Content.ReadAsStringAsync().Result);
            }
            catch
            {
                JobRecLine jl = null;
                return jl;
            }
        }

        public async Task<JobRecLine> CreateJobRecLine(JobRecLine task)
        {
            try
            {
                HttpClient client = GetHttpClient();

                var data = JsonConvert.SerializeObject(task);

                var content = new StringContent(data, Encoding.UTF8, "application/json");

                var response = await client.PostAsync(endPoint + "create", content);

                return JsonConvert.DeserializeObject<JobRecLine>(response.Content.ReadAsStringAsync().Result);
            }
            catch
            {
                JobRecLine jl = null;
                return jl;
            }
        }
    }
}
