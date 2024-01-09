
using WebApi.DTOS.Comments;

namespace WebApi.DTOS.Stock
{
    /*DTOs (Data Transfer Object) are objects used to transfer a specific amount of data, they are used, for example, when we are interested in pass just a part of the data from a model, for example, in this StockDto I am omitting the Comments property since I determined that when making a request to the route GET "api/stock/{id}", the route will just show the Stock object itself, it wont show the comments associated, so I just got rid of that property through this DTO*/
    public class StockDto
    {
        public int Id { get; set; }
        public string Symbol { get; set; } = String.Empty;
        public string CompanyName { get; set; } = String.Empty;
        public decimal Purchase { get; set; }
        public decimal LastDiv { get; set; }
        public string Industry { get; set; } = String.Empty;
        public long MarketCap { get; set; }
        public List<CommentDto> Comments { get; set; }
    }
}