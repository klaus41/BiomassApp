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
    public class PictureService : ClientGateway
    {
        string endPoint = "api/Picture/";

        public async Task<PictureModel> PostPicture(PictureModel picture, string id)
        {
            HttpClient client = GetHttpClient();

            var data = JsonConvert.SerializeObject(picture);
            var content = new StringContent(data, Encoding.UTF8, "application/json");
            var response = await client.PostAsync(endPoint + id, content);
            return JsonConvert.DeserializeObject<PictureModel>(response.Content.ReadAsStringAsync().Result);

        }
    }
}
