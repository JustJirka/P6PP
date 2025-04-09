namespace BookingService.API.Common.Exceptions;

public sealed class AuthException(string message) : Exception(message) { }
