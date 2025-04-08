namespace RoleService.API.Exceptions;

public class DuplicateEntryException : Exception
{
    public DuplicateEntryException(string message, Exception innerException)
        : base(message, innerException) { }
}