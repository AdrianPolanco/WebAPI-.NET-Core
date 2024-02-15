using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using WebApi.DTOS.Stock;
using WebApi.Interfaces;
using WebApi.Mappers;
using WebApi.Models;

namespace WebApi.Services
{
    public class FMPService : IFMPService
    {

        private readonly HttpClient _httpClient;
        private IConfiguration _configuration;
        public FMPService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }
        public async Task<Stock> FindStockBySymbolAsync(string symbol)
        {
            try
            {
                HttpResponseMessage? result = await _httpClient.GetAsync($"https://financialmodelingprep.com/api/v3/profile/{symbol}?apikey={_configuration["FMPKey"]}");

                if (result.IsSuccessStatusCode)
                {
                    //Serializing the HTTP call as string
                    string content = await result.Content.ReadAsStringAsync();
                    //Turning the string json to FMPStock instance
                    FMPStock[]? tasks = JsonConvert.DeserializeObject<FMPStock[]>(content);
                    //Getting into the content, since the response is in an array item
                    FMPStock stock = tasks[0];

                    //If the response has content, we return it converted to Stock
                    if (stock != null) return stock.ToStockFromFMP();
                }

                return null;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}