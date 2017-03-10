using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Vedligehold.Services
{
    public class PDFService : ClientGateway
    {
        string endPoint = "api/asset/";

        public async Task<string> GetPDF(string id)
        {
            HttpClient client = GetHttpClient();
            
            var response = await client.GetAsync(endPoint + id);

            string result = response.Content.ReadAsStringAsync().Result;

            //var rootObject = JsonConvert.DeserializeObject<MaintenanceTask[]>(statsJson);
            

            return result;
        }
    }
}
