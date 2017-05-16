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
    public class JobService : ClientGateway
    {
        string endPoint = "api/Jobs/";

        public async Task<Job[]> GetJobsAsync()
        {
            try
            {
                HttpClient client = GetHttpClient();

                var response = await client.GetAsync(endPoint);

                var statsJson = response.Content.ReadAsStringAsync().Result;

                var rootObject = JsonConvert.DeserializeObject<Job[]>(statsJson);

                return rootObject;
            }
            catch
            {
                Job[] jl = null;
                return jl;
            }
        }
    }
}
