using Analytics.Application.Services.Interface;
using Analytics.Domain.Entities;
using Analytics.Domain.Interface;
using Analytics.Application.DTOs;
using Analytics.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Analytics.Application.Services
{
    public class BookingService : IBookingService
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly IServiceRepository _serviceRepository;
        private readonly IRoomRepository _roomRepository;

        public BookingService(
            IBookingRepository bookingRepository,
            IServiceRepository serviceRepository,
            IRoomRepository roomRepository)
        {
            _bookingRepository = bookingRepository;
            _serviceRepository = serviceRepository;
            _roomRepository = roomRepository;
        }

        public async Task<List<Booking>> GetAllBookings()
        {
            return await _bookingRepository.GetAll();
        }

        public async Task<Booking?> GetBookingById(int id)
        {
            return await _bookingRepository.GetById(id);
        }

        public async Task<Booking> CreateBooking(BookingDto bookingDto)
        {
            if (string.IsNullOrEmpty(bookingDto.bookingDate))
                throw new ArgumentNullException(nameof(bookingDto.bookingDate), "Booking date cannot be null or empty.");

            var booking = new Booking
            {
                Id = bookingDto.id,
                BookingDate = DateTime.Parse(bookingDto.bookingDate),
                Status = Enum.Parse<BookingStatus>(bookingDto.status, ignoreCase: true),
                UserId = bookingDto.userId,
                ServiceId = bookingDto.serviceId
            };

            if (bookingDto.service != null)
            {
                var existingService = await _serviceRepository.GetById(bookingDto.serviceId);
                
                if (existingService == null)
                {
                    var newService = new Service
                    {
                        Id = bookingDto.serviceId,
                        ServiceName = bookingDto.service.serviceName,
                        Start = DateTime.Parse(bookingDto.service.start),
                        End = DateTime.Parse(bookingDto.service.end),
                        TrainerId = bookingDto.service.trainerId,
                        RoomId = bookingDto.service.roomId,
                        Users = bookingDto.service.users
                    };

                    if (bookingDto.service.room != null)
                    {
                        var existingRoom = await _roomRepository.GetById(bookingDto.service.roomId);
                        if (existingRoom == null)
                        {
                            var newRoom = new Room
                            {
                                Id = bookingDto.service.roomId,
                                Name = bookingDto.service.room.name,
                                Capacity = bookingDto.service.room.capacity,
                                Status = "Available"
                            };
                            await _roomRepository.Create(newRoom);
                        }
                        else if (existingRoom.Name != bookingDto.service.room.name || 
                                existingRoom.Capacity != bookingDto.service.room.capacity)
                        {
                            existingRoom.Name = bookingDto.service.room.name;
                            existingRoom.Capacity = bookingDto.service.room.capacity;
                            await _roomRepository.Update(existingRoom);
                        }
                    }

                    await _serviceRepository.Create(newService);
                }
                else if (existingService.ServiceName != bookingDto.service.serviceName ||
                        existingService.TrainerId != bookingDto.service.trainerId ||
                        existingService.RoomId != bookingDto.service.roomId)
                {
                    existingService.ServiceName = bookingDto.service.serviceName;
                    existingService.Start = DateTime.Parse(bookingDto.service.start);
                    existingService.End = DateTime.Parse(bookingDto.service.end);
                    existingService.TrainerId = bookingDto.service.trainerId;
                    existingService.RoomId = bookingDto.service.roomId;
                    existingService.Users = bookingDto.service.users;
                    
                    await _serviceRepository.Update(existingService);
                    
                    if (bookingDto.service.room != null)
                    {
                        var existingRoom = await _roomRepository.GetById(bookingDto.service.roomId);
                        if (existingRoom != null && 
                            (existingRoom.Name != bookingDto.service.room.name || 
                            existingRoom.Capacity != bookingDto.service.room.capacity))
                        {
                            existingRoom.Name = bookingDto.service.room.name;
                            existingRoom.Capacity = bookingDto.service.room.capacity;
                            await _roomRepository.Update(existingRoom);
                        }
                    }
                }
                
                if (bookingDto.service.users != null && bookingDto.service.users.Count > 0)
                {
                    await _serviceRepository.UpdateUsers(bookingDto.serviceId, bookingDto.service.users);
                }
            }

            await _bookingRepository.Create(booking);
            return booking;
        }

        public async Task<Booking?> DeleteBooking(int id)
        {
            return await _bookingRepository.Delete(id);
        }
    }
}
