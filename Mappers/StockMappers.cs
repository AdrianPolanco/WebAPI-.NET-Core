using WebApi.DTOS.Stock;
using WebApi.Models;

namespace WebApi.Mappers
{
    public static class StockMappers
    {
        public static StockDto ToStockDto(this Stock stockModel)
        {
            StockDto stockDto = new StockDto()
            {
                Id = stockModel.Id,
                Symbol = stockModel.Symbol,
                CompanyName = stockModel.CompanyName,
                Purchase = stockModel.Purchase,
                LastDiv = stockModel.LastDiv,
                Industry = stockModel.Industry,
                MarketCap = stockModel.MarketCap,
                Comments = stockModel.Comments.Select(c => c.ToCommentDto()).ToList()
            };

            return stockDto;
        }

        public static Stock ToStockFromCreateDTO(this CreateStockRequestDto stockRequestDto)
        {
            Stock stockDto = new Stock()
            {
                Symbol = stockRequestDto.Symbol,
                CompanyName = stockRequestDto.CompanyName,
                Purchase = stockRequestDto.Purchase,
                LastDiv = stockRequestDto.LastDiv,
                Industry = stockRequestDto.Industry,
                MarketCap = stockRequestDto.MarketCap
            };

            return stockDto;
        }

        public static Stock ToStockFromFMP(this FMPStock fmpStock)
        {
            Stock stockDto = new Stock()
            {
                Symbol = fmpStock.symbol,
                CompanyName = fmpStock.companyName,
                Purchase = (decimal)fmpStock.price,
                LastDiv = (decimal)fmpStock.lastDiv,
                Industry = fmpStock.industry,
                MarketCap = fmpStock.mktCap
            };

            return stockDto;
        }
    }
}