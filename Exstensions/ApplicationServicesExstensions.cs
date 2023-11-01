using DatingApp.Data;
using DatingApp.Helpers;
using DatingApp.Interfaces;
using DatingApp.Services;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.Exstensions
{
    public static class ApplicationServicesExstensions
    {
        public static IServiceCollection AddAplicationServices(this IServiceCollection services,IConfiguration config)
        {
            // сервис для создания токена
            services.AddScoped<ITokenServices, TokenService>();
            services.AddScoped<IUserRepository,UserRepository>();
            services.AddAutoMapper(typeof(AutoMapperProfile).Assembly);
            // mssql
            services.AddDbContext<DataContext>(options =>
                                           options.UseSqlServer(config.GetConnectionString("ConnectKratek")));

            return services;
        }
    }
}
