namespace Instagraph.App
{
    using AutoMapper;
    using Models;
    using DataProcessor.Dtos.Export;

    public class InstagraphProfile : Profile
    {
        public InstagraphProfile()
        {
            CreateMap<User, PopularUserDto>()
                .ForMember(dto => dto.Followers,
                    f => f.MapFrom(u => u.Followers.Count));
        }
    }
}
