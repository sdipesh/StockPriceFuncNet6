using System.Collections.Generic;
using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Function.Domain.Providers;
using Function.Domain.Helpers;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.OpenApi.Models;
using System.Threading.Tasks;

namespace ep.azcd.sp.func
{
    public class GetOpenStockPriceForSymbol
    {
        private readonly ILogger _logger;
        private readonly IStockDataProvider _stockDataProvider;
        private readonly IHttpHelper _httpHelper;

        public GetOpenStockPriceForSymbol(
                    IStockDataProvider stockDataProvider,
                    IHttpHelper httpHelper,
                    ILogger<GetOpenStockPriceForSymbol> logger){
                _stockDataProvider = stockDataProvider;
                _httpHelper = httpHelper;
                _logger = logger;
        }

        [Function("GetOpenStockPriceForSymbol")]
        [OpenApiOperation(operationId: "GetOpenStockPriceForSymbol", tags: new[] { "stock-price/symbol"})]
        [OpenApiParameter(name: "symbol", In = ParameterLocation.Path, Required = true, Type = typeof(string), Description = "Symbol to get stock data from")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(string), Description = "OK response")]
        public async Task<HttpResponseData> Run([HttpTrigger(
                                        AuthorizationLevel.Anonymous, 
                                        "get", 
                                        Route = "stock-price/symbol/{symbol:alpha}/open")] HttpRequestData req, string symbol)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            var openPrice = await GetOpenStockPriceForSymbolAsync(symbol);
            
            var response = await _httpHelper.CreateSuccessfulHttpResponse(req, openPrice);

            return response;
        }

        private async Task<decimal> GetOpenStockPriceForSymbolAsync(string symbol){
            var stockData = await _stockDataProvider.GetStockDataForSymbolAsync(symbol);
            var openPrice = stockData.Open;

            return openPrice;
        }
    }
}
