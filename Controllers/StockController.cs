using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WebApi.Data;
using WebApi.DTOS.Stock;
using WebApi.Mappers;
using WebApi.Models;

namespace WebApi.Controllers
{
    [Route("api/stock")]
    [ApiController]
    public class StockController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public StockController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var stocks = _context.Stock.ToList().Select(s => s.ToStockDto());
            return Ok(stocks);
        }

        [HttpGet("{id}")]
        public IActionResult GetById([FromRoute] int id)
        {
            var stock = _context.Stock.Find(id);

            if (stock == null) return NotFound();

            return Ok(stock.ToStockDto());
        }

        [HttpPost]
        public IActionResult Create([FromBody] CreateStockRequestDto createStockRequest)
        {
            Stock stock = createStockRequest.ToStockFromCreateDTO();
            _context.Stock.Add(stock);
            _context.SaveChanges();
            return CreatedAtAction(nameof(GetById), new { id = stock.Id }, stock.ToStockDto());
        }

        [HttpPut]
        [Route("{id}")]
        public IActionResult Update([FromRoute] int id, [FromBody] UpdateStockRequestDto updateStockRequest)
        {
            Stock? stockModel = _context.Stock.FirstOrDefault(s => s.Id == id);

            if (stockModel == null) return NotFound();

            stockModel.Symbol = updateStockRequest.Symbol;
            stockModel.CompanyName = updateStockRequest.CompanyName;
            stockModel.Purchase = updateStockRequest.Purchase;
            stockModel.LastDiv = updateStockRequest.LastDiv;
            stockModel.Industry = updateStockRequest.Industry;
            stockModel.MarketCap = updateStockRequest.MarketCap;

            _context.SaveChanges();

            return Ok(stockModel.ToStockDto());
        }

        [HttpDelete]
        [Route("{id}")]
        public IActionResult Delete([FromRoute] int id)
        {
            Stock? stock = _context.Stock.FirstOrDefault(s => s.Id == id);

            if (stock == null) return NotFound();

            _context.Stock.Remove(stock);
            _context.SaveChanges();

            return NoContent();
        }
    }


}