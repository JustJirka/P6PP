using BookingService.API.Domain.Models;

namespace BookingService.API.Features.Rooms.Models;

public static class RoomsMappingExtensions
{
    public static Room Map(this RoomRequest input)
        => new()
        {
            Name = input.Name,
            Capacity = input.Capacity,
            Status = input.Status,
        };

    public static void MapTo(this RoomRequest input, Room output)
    {
        output.Name = input.Name;
        output.Capacity = input.Capacity;
        output.Status = input.Status;
    }

    public static RoomResponse Map(this Room input)
        => new(input.Id, input.Name, input.Capacity, input.Status);

    public static IList<RoomResponse> Map(this IList<Room> input)
        => [.. input.Select(Map)];
}
