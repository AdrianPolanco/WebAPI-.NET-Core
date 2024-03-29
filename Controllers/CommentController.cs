using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebApi.DTOS.Comments;
using WebApi.Extensions;
using WebApi.Helpers;
using WebApi.Interfaces;
using WebApi.Mappers;
using WebApi.Models;

namespace WebApi.Controllers
{
    [Route("api/comment")]
    [ApiController]
    public class CommentController : ControllerBase
    {
        private readonly ICommentRepository _commentRepo;
        private readonly IStockRepository _stockRepo;
        private readonly UserManager<AppUser> _userManager;
        private readonly IFMPService _fmpService;
        public CommentController(ICommentRepository commentRepository, IStockRepository stockRepository, UserManager<AppUser> userManager, IFMPService fmpService)
        {
            _commentRepo = commentRepository;
            _stockRepo = stockRepository;
            _userManager = userManager;
            _fmpService = fmpService;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAll([FromQuery] CommentQueryObject commentQueryObject)
        {
            //Performing the validations written in the data annotations of our DTOs
            if (!ModelState.IsValid) return BadRequest(ModelState);

            List<Comment> comments = await _commentRepo.GetAllAsync(commentQueryObject);
            var commentDtos = comments.Select(c => c.ToCommentDto());

            return Ok(commentDtos);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            Comment? comment = await _commentRepo.GetByIdAsync(id);

            if (comment == null) return NotFound();

            return Ok(comment.ToCommentDto());
        }

        [HttpPost("{symbol:alpha}")]
        public async Task<IActionResult> Create([FromRoute] string symbol, CreateCommentDto comment)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var stock = await _stockRepo.GetBySymbolAsync(symbol);

            if (stock == null)
            {
                stock = await _fmpService.FindStockBySymbolAsync(symbol);

                if (stock == null) return BadRequest("The stock does not exists");

                await _stockRepo.CreateAsync(stock);
            }

            string? username = User.GetUsername();
            AppUser? appUser = await _userManager.FindByNameAsync(username);

            Comment _comment = comment.ToCommentFromCreate(stock.Id);
            _comment.AppUserId = appUser.Id;
            await _commentRepo.CreateAsync(_comment);

            return CreatedAtAction(nameof(GetById), new { id = _comment.Id }, _comment.ToCommentDto());
        }

        [HttpPut]
        [Route("{id}")]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdateCommentRequestDto requestDto)
        {
            Comment comment = requestDto.ToCommentFromUpdate();
            Comment? updatedComment = await _commentRepo.UpdateAsync(id, comment);

            if (updatedComment == null) return NotFound("Comment not found");

            CommentDto commentDto = comment.ToCommentDto();

            return Ok(commentDto);
        }

        //Making validation of types with id:int we are checking that the id parameter sent by the link is a number
        [HttpDelete]
        [Route("{id:int}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            Comment? _comment = await _commentRepo.DeleteAsync(id);

            if (_comment == null) return NotFound("This comments does not exist.");

            return Ok(_comment);
        }
    }
}