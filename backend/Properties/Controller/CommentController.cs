using BoardApi.Dtos;
using BoardApi.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace BoardApi.Controllers;

[ApiController]
[Route("api/comments")]
public class CommentController : ControllerBase
{
    private readonly ICommentService _commentService;

    public CommentController(ICommentService commentService)
    {
        _commentService = commentService;
    }

    [HttpPost]
    public async Task<ActionResult<CommentResponse>> CreateComments(CreateCommentRequest request)
    {
        var result = await _commentService.CreateAsync(request);
        return CreatedAtAction("", new { id = result.CommentId}, result);
    }

    [HttpGet]
    public async Task<ActionResult<CommentResponse>> GetPostComments(int postId)
    {
        return Ok(_commentService.GetPostCommentListAsync(postId));
    }

    [HttpGet]
    public async Task<ActionResult<CommentResponse>> GetUserComments(int postId)
    {
        return Ok(_commentService.GetUserCommentListAsync(postId));
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<CommentResponse>> Update(int postId, UpdateCommentRequest request)
    {
        return NotFound();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int postId)
    {
        bool deleted = await _commentService.DeleteAsync(postId);
        return deleted ? NoContent() : NotFound();
    }
}