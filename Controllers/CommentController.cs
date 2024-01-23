using Microsoft.AspNetCore.Mvc;
using WebApi.DTOS.Comments;
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
        public CommentController(ICommentRepository commentRepository, IStockRepository stockRepository)
        {
            _commentRepo = commentRepository;
            _stockRepo = stockRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            //Performing the validations written in the data annotations of our DTOs
            if (!ModelState.IsValid) return BadRequest(ModelState);

            List<Comment> comments = await _commentRepo.GetAllAsync();
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

        [HttpPost("{id:int}")]
        public async Task<IActionResult> Create([FromRoute] int id, CreateCommentDto comment)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            if (!await _stockRepo.Exists(id)) return BadRequest("The stock does not exist");

            Comment _comment = comment.ToCommentFromCreate(id);
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