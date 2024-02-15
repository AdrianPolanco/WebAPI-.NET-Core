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
                    string content = await result.Content.ReadAsStringAsync();
                    FMPStock[]? tasks = JsonConvert.DeserializeObject<FMPStock[]>(content);
                    FMPStock stock = tasks[0];

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