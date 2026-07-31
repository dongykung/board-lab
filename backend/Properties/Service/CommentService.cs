using BoardApi.Dtos;

namespace BoardApi.Services;

public class CommentService : ICommentService
{
    public Task<CommentResponse> CreateAsync(CreateCommentRequest request)
    {
        throw new NotImplementedException();
    }

    public Task<bool> DeleteAsync(int id)
    {
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
}