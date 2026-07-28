namespace BoardApi.Exceptions;

public abstract class BoardApiException : Exception
{
    public abstract int StatusCode { get; }
    public abstract string Title { get; }
    protected BoardApiException(string message) : base(message) { }
}