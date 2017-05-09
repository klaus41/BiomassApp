using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Vedligehold.Services
{
    public class ClientGateway
    {
        readonly HttpClient _client;

        public ClientGateway()
        {
            if (_client == null)
            {
                _client = new HttpClient();
                _client.DefaultRequestHeaders.Add("Accept", "application/json");
                //string baseAddress = "http://demo.biomass.eliteit.dk/";
                string baseAddress = App.Database.GetConnectionSetting(0).Result.BaseAddress;
                _client.BaseAddress = new Uri(baseAddress);
            }
        }

        public HttpClient GetHttpClient()
        {
            return _client;
        }
    }
}
