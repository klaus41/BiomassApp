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
    public class CustomerService : ClientGateway
    {
        string endPoint = "api/Customer/";

        public async Task<Customer[]> GetCustomersAsync()
        {
            try
            {
                HttpClient client = GetHttpClient();

                var response = await client.GetAsync(endPoint);

                var statsJson = response.Content.ReadAsStringAsync().Result;

                var rootObject = JsonConvert.DeserializeObject<Customer[]>(statsJson);

                return rootObject;
            }
            catch
            {
                Customer[] jl = null;
                return jl;
            }
        }

    }
}
