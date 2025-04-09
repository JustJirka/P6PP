namespace BookingService.API.Common.Exceptions;

public sealed class ValidationException(string message) : Exception(message) { }
