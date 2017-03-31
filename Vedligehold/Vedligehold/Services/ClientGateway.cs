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
        readonly HttpClient _client = new HttpClient();

        public ClientGateway()
        {
            _client.DefaultRequestHeaders.Add("Accept", "application/json");
            //string baseAddress = "http://demo.biomass.eliteit.dk/";
            string baseAddress = "http://vedligehold.biomass.eliteit.dk/";
            _client.BaseAddress = new Uri(baseAddress);
        }

        public HttpClient GetHttpClient()
        {
            return _client;
        }
    }
}
