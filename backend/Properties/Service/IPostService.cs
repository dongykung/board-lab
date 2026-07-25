using BoardApi.Dtos;

namespace BoardApi.Services;

public interface IPostService
{
    Task<PostResponse> CreateAsync(CreatePostRequest request);
    Task<PostResponse?> GetByIdAsync(int id);
    Task<List<PostResponse>> GetListAsync();
    Task<PostResponse?> UpdateAsync (int id, UpdatePostRequest request);
    Task<bool> DeleteAsync(int id);
}