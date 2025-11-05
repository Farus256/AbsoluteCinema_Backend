using AbsoluteCinema.Application.DTO.UsersDTO;
using AbsoluteCinema.Infrastructure.Identity.Data;
using AutoMapper;

namespace AbsoluteCinema.Infrastructure.Mappings.UserMapper;

public class UserMappingProfile : Profile
{
    public UserMappingProfile()
    {
        CreateMap<ApplicationUser, UserDto >().ReverseMap();
        
        CreateMap<GetAllUsersDto, ApplicationUser>();
        
        CreateMap<UpdateUserDto, ApplicationUser>()
            .ForMember(dest => dest.BirthDate,
                opt => {
                    opt.Condition(src => src.BirthDate.HasValue);
                    opt.MapFrom(src => DateTime.SpecifyKind(src.BirthDate!.Value, DateTimeKind.Utc));
                });
    }
}