namespace PetClinic.App
{
    using AutoMapper;
    using PetClinic.DataProcessor.DTOs.Import;
    using PetClinic.Models;

    public class PetClinicProfile : Profile
    {
        // Configure your AutoMapper here if you wish to use it. If not, DO NOT DELETE THIS CLASS
        public PetClinicProfile()
        {
            CreateMap<AnimalAid, AnimalAidDto>().ReverseMap();

            CreateMap<Animal, AnimalDto>().ReverseMap()
                .ForMember(dest => dest.PassportSerialNumber,
                           opt => opt.MapFrom(a => a.Passport.SerialNumber));

            CreateMap<Passport, PassportDto>().ReverseMap()
                .ForMember(dest => dest.RegistrationDate,
                            opt => opt.Ignore());

            CreateMap<Vet, VetDto>().ReverseMap();

            //CreateMap<Procedure, ProcedureDto>().ReverseMap();
        }
    }
}
