// Maps Models to DTOs and vice versa
using AutoMapper;
using BookingApi.Dtos;
using BookingApi.Models;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<User, UserDto>().ReverseMap();
        CreateMap<CreateUserDto, User>();
        CreateMap<UpdateUserDto, User>();

        CreateMap<Resource, ResourceDto>().ReverseMap();
        CreateMap<CreateResourceDto, Resource>();
        CreateMap<UpdateResourceDto, Resource>();

        CreateMap<Booking, BookingDto>().ReverseMap();
        CreateMap<CreateBookingDto, Booking>();
        CreateMap<UpdateBookingDto, Booking>();
    }
}