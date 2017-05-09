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
    public class ResourcesService : ClientGateway
    {
        string endPoint = "api/Resources/";

        public async Task<Resources[]> GetResourcesAsync()
        {
            try
            {
                HttpClient client = GetHttpClient();

                var response = await client.GetAsync(endPoint);

                var statsJson = response.Content.ReadAsStringAsync().Result;

                var rootObject = JsonConvert.DeserializeObject<Resources[]>(statsJson);
                
                return rootObject;
            }
            catch
            {
                Resources[] ml = null;
                return ml;
            }
        }

    }
}
