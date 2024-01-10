using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WebApi.Data;
using WebApi.DTOS.Stock;
using WebApi.Interfaces;
using WebApi.Mappers;
using WebApi.Models;

namespace WebApi.Repository
{
    public class StockRepository : IStockRepository
    {

        private readonly ApplicationDbContext _context;
        public StockRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Stock>> GetAllAsync()
        {
            var stockList = await _context.Stock.Include(c => c.Comments).ToListAsync();
            return stockList;
        }

        public async Task<Stock?> GetByIdAsync(int id)
        {
            //Especificando que queremos obtener el Stock de la tabla STOCK INCLUYENDO sus comentarios relacionados guardados en la tabla Comments en una sola llamada a la BD, ya que EF Core por defecto obtendra null cuando trates de acceder a las propiedades que tienen entidades relacionadas
            var stock = await _context.Stock.Include(c => c.Comments).FirstOrDefaultAsync(s => s.Id == id);

            if (stock == null) return null;

            return stock;
        }

        public async Task<Stock> CreateAsync(Stock stockModel)
        {
            await _context.Stock.AddAsync(stockModel);
            await _context.SaveChangesAsync();
            return stockModel;
        }

        public async Task<Stock?> UpdateAsync(int id, UpdateStockRequestDto updateStockRequest)
        {
            Stock? stockModel = await _context.Stock.FirstOrDefaultAsync(s => s.Id == id);

            if (stockModel == null) return null;

            stockModel.Symbol = updateStockRequest.Symbol;
            stockModel.CompanyName = updateStockRequest.CompanyName;
            stockModel.Purchase = updateStockRequest.Purchase;
            stockModel.LastDiv = updateStockRequest.LastDiv;
            stockModel.Industry = updateStockRequest.Industry;
            stockModel.MarketCap = updateStockRequest.MarketCap;

            await _context.SaveChangesAsync();

            return stockModel;
        }

        public async Task<Stock?> DeleteAsync(int id)
        {
            Stock? stock = await _context.Stock.FirstOrDefaultAsync(s => s.Id == id);

            if (stock == null) return null;

            _context.Stock.Remove(stock);
            await _context.SaveChangesAsync();

            return stock;
        }

        public Task<bool> Exists(int id)
        {
            return _context.Stock.AnyAsync(s => s.Id == id);
        }
    }
}