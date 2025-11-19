using AbsoluteCinema.Application.DTO.AuthDTO.SessionsDTO;
using AbsoluteCinema.Application.DTO.Entities;
using AbsoluteCinema.Application.DTO.EntityDTO;
using AbsoluteCinema.Domain.Entities;
using AutoMapper;

namespace AbsoluteCinema.Infrastructure.Mappings.EntityMapper;

public class EntityMappingProfile : Profile
{
    public EntityMappingProfile()
    {
        CreateMap<Actor, ActorDto>().ReverseMap();
        CreateMap<Genre, GenreDto>().ReverseMap();
        CreateMap<Hall, HallDto>().ReverseMap();
        CreateMap<Movie, MovieDto>().ReverseMap();
        CreateMap<Session, SessionDto>().ReverseMap();
        CreateMap<Ticket, TicketDto>().ReverseMap();
        CreateMap<TicketStatus, TicketStatusDto>().ReverseMap();
        CreateMap<MovieGenre, MovieGenreDto>().ReverseMap();
        CreateMap<MovieActor, MovieActorDto>().ReverseMap();
    }
}