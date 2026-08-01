using BoardApi.Dtos;
using BoardApi.Data;
using BoardApi.Models;

namespace BoardApi.Services;

public class CommentService : ICommentService
{
    private readonly BoardDbContext _db;
    private readonly ILogger<CommentService> _logger;

    public CommentService(BoardDbContext db, ILogger<CommentService> logger)
    {
        _db = db;
        _logger = logger;
    }

    public Task<CommentResponse> CreateAsync(CreateCommentRequest request)
    {
        Comment comment = new Comment
        {
            PostId = request.postId,
            Content = request.Content,
        };
        throw new NotImplementedException();
    }

    public Task<List<CommentResponse>> GetPostCommentListAsync(int postId)
    {
        throw new NotImplementedException();
    }

    public Task<List<CommentResponse>> GetUserCommentListAsync(int userId)
    {
        throw new NotImplementedException();
    }

    public Task<CommentResponse> UpdateAsync(int id, UpdateCommentRequest request)
    {
        throw new NotImplementedException();
    }

    public Task<bool> DeleteAsync(int id)
    {
        throw new NotImplementedException();
    }
}