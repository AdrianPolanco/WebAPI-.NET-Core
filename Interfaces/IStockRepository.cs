using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApi.DTOS.Stock;
using WebApi.Helpers;
using WebApi.Models;

namespace WebApi.Interfaces
{
    public interface IStockRepository
    {
        Task<List<Stock>> GetAllAsync(QueryObject query);
        Task<Stock?> GetByIdAsync(int id);
        Task<Stock> CreateAsync(Stock stockModel);
        Task<Stock?> UpdateAsync(int id, UpdateStockRequestDto updateStockRequest);
        Task<Stock?> DeleteAsync(int id);
        Task<bool> Exists(int id);
    }
}