using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebApi.Extensions;
using WebApi.Interfaces;
using WebApi.Models;
using WebApi.Repository;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PortfolioController : ControllerBase
    {
        private readonly IStockRepository _stockRepository;
        private readonly UserManager<AppUser> _userManager;
        private readonly IPortfolioRepository _portfolioRepository;
        private readonly IFMPService _fmpService;
        public PortfolioController(IStockRepository stockRepository, UserManager<AppUser> userManager, IPortfolioRepository portfolioRepository, IFMPService fmpService)
        {
            _stockRepository = stockRepository;
            _userManager = userManager;
            _portfolioRepository = portfolioRepository;
            _fmpService = fmpService;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetUserPortfolio()
        {
            string? username = User.GetUsername();
            AppUser? appUser = await _userManager.FindByNameAsync(username);
            List<Stock>? userPortfolio = await _portfolioRepository.GetUserPortfolio(appUser);
            return Ok(userPortfolio);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddPortfolio(string symbol)
        {
            //Getting username
            string? username = User.GetUsername();
            //Getting user based on his username
            AppUser? appUser = await _userManager.FindByNameAsync(username);
            //Getting stock by its symbol
            Stock? stock = await _stockRepository.GetBySymbolAsync(symbol);

            //Checking if the stock is not in our database
            if (stock == null)
            {
                //Making a HTTP call to the Financial Modeling Prep API
                stock = await _fmpService.FindStockBySymbolAsync(symbol);

                //Checking the stock code also doesnt exists in the API and returning a 400 status code
                if (stock == null) return BadRequest("The stock does not exists");

                //If the code exists in the API we will store it in our database
                await _stockRepository.CreateAsync(stock);
            }

            //Checking if the portfolio exists
            if (stock == null) return BadRequest("Stock not found");

            List<Stock?> userPortfolio = await _portfolioRepository.GetUserPortfolio(appUser);

            //Checking if the stock is already in the portfolio
            if (userPortfolio.Any(s => s.Symbol.ToLower() == symbol.ToLower())) return BadRequest("Cannot add the same stock twice");

            Portfolio portfolio = new Portfolio
            {
                StockId = stock.Id,
                AppUserId = appUser.Id
            };

            //Saving the new portfolio inside the DB
            await _portfolioRepository.CreateAsync(portfolio);

            if (portfolio == null) return StatusCode(500, "Could not save the change");

            return Created();
        }

        [HttpDelete]
        [Authorize]
        public async Task<IActionResult> DeletePortfolio(string symbol)
        {
            string? username = User.GetUsername();
            AppUser? appUser = await _userManager.FindByNameAsync(username);

            //Getting al stocks associated with the user id
            List<Stock?> portfolio = await _portfolioRepository.GetUserPortfolio(appUser);

            List<Stock?> filteredStock = portfolio.Where(s => s.Symbol.ToLower() == symbol.ToLower()).ToList();

            if (filteredStock.Count() == 1) await _portfolioRepository.DeletePortfolio(appUser, symbol);
            else return BadRequest("This stock is not in your portfolio");

            return Ok();

        }
    }
}