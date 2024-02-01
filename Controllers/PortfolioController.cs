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
        public PortfolioController(IStockRepository stockRepository, UserManager<AppUser> userManager, IPortfolioRepository portfolioRepository)
        {
            _stockRepository = stockRepository;
            _userManager = userManager;
            _portfolioRepository = portfolioRepository;
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
    }
}