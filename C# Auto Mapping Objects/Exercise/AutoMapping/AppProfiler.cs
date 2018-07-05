using AutoMapper.ModelDtos;
using AutoMapper.Models;

namespace AutoMapper.Client
{
    public class AppProfiler : Profile
    {
        public AppProfiler()
        {
            CreateMap<Employee, EmployeeDto>();
            CreateMap<Employee, ManagerDto>()
                .ForMember(dest => dest.EmployeesCount,
                           opt => opt.MapFrom(src => src.Employees.Count))
                .ForMember(dest => dest.Employees,
                           opt => opt.MapFrom(src => src.Employees));
            CreateMap<Employee, EmployeeWithManagerDto>()
                .ForMember(dest => dest.Manager,
                           opt => opt.MapFrom(src => src.Manager));
        }
    }
}
