namespace BookingService.API.Common.Exceptions;

public sealed class NotFoundException(string message) : Exception(message) { }
