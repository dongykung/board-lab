namespace BoardApi.Exceptions;

public class PostNotFoundException : BoardApiException
{
    public int PostId { get; }
    public override int StatusCode => StatusCodes.Status404NotFound;
    public override string Title => "Not Found";
    public PostNotFoundException(int postId) : base($"Post {postId} was not found.")
    {
        PostId = postId;
    }
}