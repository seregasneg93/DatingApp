using AutoMapper;
using DatingApp.DTOs;
using DatingApp.Entities;
using DatingApp.Exstensions;

namespace DatingApp.Helpers
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<AppUser, MemberDto>()
                .ForMember(desc => desc.PhotoUrl,
                           opt => opt.MapFrom(src => src.Photos.FirstOrDefault(x => x.IsMain).Url))
                .ForMember(desc => desc.Age,
                           opt => opt.MapFrom(src => src.DateBurth.CalculateAge()));

            CreateMap<Photo, PhotoDto>();
        }
    }
}
