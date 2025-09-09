namespace Tenders.Guru.Facade.Api.Exceptions;

public class TendersApiException : Exception
{
    public TendersApiException(string message) : base(message)
    {
    }

    public TendersApiException(string message, Exception innerException) : base(message, innerException)
    {
    }
}