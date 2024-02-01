using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WebApi.Data;
using WebApi.Interfaces;
using WebApi.Models;

namespace WebApi.Repository
{
    public class PortfolioRepository : IPortfolioRepository
    {
        private readonly ApplicationDbContext _context;
        public PortfolioRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Stock>> GetUserPortfolio(AppUser user)
        {
            //.Where devuelve un IQueryable<Portfolio> de todos los registros cuyo AppUserId sean iguales al Id del usuario, que a su vez tambien es de tipo AppUser

            //Luego de eso, los selecciona y con los datos de cada registro crea un objeto Stock, con las propieades de cada uno y lo convierte a lista
            return await _context.Portfolios.Where(u => u.AppUserId == user.Id)
                        .Select(portfolio => new Stock
                        {
                            Id = portfolio.StockId,
                            Symbol = portfolio.Stock.Symbol,
                            CompanyName = portfolio.Stock.CompanyName,
                            Purchase = portfolio.Stock.Purchase,
                            LastDiv = portfolio.Stock.LastDiv,
                            Industry = portfolio.Stock.Industry,
                            MarketCap = portfolio.Stock.MarketCap
                        }).ToListAsync();
        }
        public async Task<Portfolio> CreateAsync(Portfolio portfolio)
        {
            await _context.Portfolios.AddAsync(portfolio);
            await _context.SaveChangesAsync();
            return portfolio;
        }

    }
}