namespace AutoMapper.Client
{
    using System;

    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.DependencyInjection;

    using Core;
    using AutoMapper.Data;
    using AutoMapper.Services;
    using AutoMapper.Models;
    using AutoMapper.ModelDtos;

    public class StartUp
    {
        public static void Main()
        {
            IServiceProvider serviceProvider = ConfigureServices();

            Engine engine = new Engine(serviceProvider);
            engine.Run();
        }

        private static IServiceProvider ConfigureServices()
        {
            IServiceCollection serviceCollection = new ServiceCollection();

            serviceCollection.AddDbContext<EmployeeDbContext>(options =>
            options.UseSqlServer(Configuration.ConnectionString));

            serviceCollection.AddAutoMapper(cfg => cfg.AddProfile<AppProfiler>());

            serviceCollection.AddTransient<EmployeeService>();
            serviceCollection.AddTransient<CommandInterpreter>();
            
            IServiceProvider serviceProvider = serviceCollection.BuildServiceProvider();

            return serviceProvider;
        }
    }
}
