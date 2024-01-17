using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WebApi.Data;
using WebApi.DTOS.Stock;
using WebApi.Helpers;
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

        public async Task<List<Stock>> GetAllAsync(QueryObject query)
        {

            //Al usar .AsQueryable "diferimos" la ejecucion de la consulta, retornando este metodo un IQueryable<Tabla>, de modo que, hasta que, entre el uso de .AsQueryable() y .ToListAsync() podemos entonces, construir como sera nuestra consulta, en este caso, la estamos "configurando" para que en caso de que el usuario use los query parameters de la ruta para filtrar, entonces se ejecuten los filtrados
            var stockList = _context.Stock.Include(c => c.Comments).AsQueryable();

            //Si el query parameter (capturado en las propiedad CompanyName de la clase QueryObject) no es nulo o espacio en blanco entonces se ejecutara el codigo dentro del if
            if (!string.IsNullOrWhiteSpace(query.CompanyName))
            {
                //Filtrara los registros cuya columna CompanyName contenga la palabra que este en el CompanyName del query
                stockList = stockList.Where(s => s.CompanyName.Contains(query.CompanyName));
            }

            if (!string.IsNullOrWhiteSpace(query.Symbol))
            {
                stockList = stockList.Where(s => s.Symbol.Contains(query.Symbol));
            }

            //.ToList() y .ToListAsync() son los que disparan a EF Core a generar las consultas para obtener datos de la tabla, osea, hasta que no invocas uno de esos metodos, la consulta aun no se ha ejecutado
            return await stockList.ToListAsync();
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