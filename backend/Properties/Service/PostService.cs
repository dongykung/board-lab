using BoardApi.Data;
using BoardApi.Dtos;
using BoardApi.Exceptions;
using BoardApi.Models;
using Microsoft.EntityFrameworkCore;

namespace BoardApi.Services;

public class PostService : IPostService
{
    private readonly BoardDbContext _db;
    private readonly ILogger<PostService> _logger;

    public PostService(BoardDbContext db, ILogger<PostService> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task<PostResponse> CreateAsync(CreatePostRequest request)
    {
        Post post = new Post
        {
            Title = request.Title,
            Content = request.Content,
            AuthorName = request.AuthorName,
            CreatedAt = DateTime.UtcNow,
        };

        await _db.Posts.AddAsync(post); 
        await _db.SaveChangesAsync();
        _logger.LogInformation("Post {Title} created by {AuthorName}", request.Title, request.AuthorName);

        return PostResponse.FromEntity(post);
    }


    public async Task<PostResponse> GetByIdAsync(int id)
    {
        Post post = await _db.Posts.FindAsync(id) ?? throw new PostNotFoundException(id);
        post.ViewCount++;
        await _db.SaveChangesAsync();
        _logger.LogInformation("Post{id} ViewCount increase: {ViewCount}", id, post.ViewCount);

        return PostResponse.FromEntity(post);
    }

    public async Task<List<PostResponse>> GetListAsync()
    {
        return await _db.Posts
            .OrderByDescending(p => p.CreatedAt)
            .Select(p => PostResponse.FromEntity(p))
            .ToListAsync();
    }

    public async Task<PostResponse> UpdateAsync(int id, UpdatePostRequest request)
    {
        Post post = await _db.Posts.FindAsync(id) ?? throw new PostNotFoundException(id);

        post.Title = request.Title;
        post.Content = request.Content;
        post.UpdatedAt = DateTime.UtcNow;
        
        await _db.SaveChangesAsync();
        _logger.LogInformation("Update Post id={id}", post.Id);

        return PostResponse.FromEntity(post);
    }

        public async Task<bool> DeleteAsync(int id)
    {
        var post = await _db.Posts.FindAsync(id) ?? throw new PostNotFoundException(id);
        _db.Posts.Remove(post);
        await _db.SaveChangesAsync();
        _logger.LogInformation("Delete Post id={id}", id);
        return true;
    }
}