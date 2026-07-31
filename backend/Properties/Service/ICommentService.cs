using BoardApi.Dtos;

namespace BoardApi.Services;

public interface ICommentService
{
    Task<CommentResponse> CreateAsync(CreateCommentRequest request);
    Task<CommentResponse> UpdateAsync(int id, UpdateCommentRequest request);
    Task<List<CommentResponse>> GetPostCommentListAsync(int postId);
    Task<List<CommentResponse>> GetUserCommentListAsync(int userId);
    Task<bool> DeleteAsync(int id);

}