using System.Linq;
using AutoMapper;
using DatingApp.API.Dtos;
using DatingApp.API.Models;

namespace DatingApp.API.Helpers
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<User, UserForListDto>()
            .ForMember(c=> c.PhotoUrl, opt => {
                opt.MapFrom(src => src.Photos.FirstOrDefault(p=> p.IsMain).Url);
            })
            .ForMember(c=> c.Age, opt => {
                opt.ResolveUsing(d => d.DateOfBirth.CalculatesAge());                    
            });

            CreateMap<User, UserForDetailedDto>()
            .ForMember(c=> c.PhotoUrl, opt => {
                opt.MapFrom(src => src.Photos.FirstOrDefault(p=> p.IsMain).Url);
            })
                        .ForMember(c=> c.Age, opt => {
                opt.ResolveUsing(d => d.DateOfBirth.CalculatesAge());                    
            });
            
            CreateMap<Photo, PhotosForDetailsDTo>();
            CreateMap<UserForUpdateDto, User>();
            CreateMap<Photo, PhotoForReturnDto>();
            CreateMap<PhotoForCreationDTo, Photo>();
            CreateMap<UserForRegisterDto, User>();
        }
    }
}