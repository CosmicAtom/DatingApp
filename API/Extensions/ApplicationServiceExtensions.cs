using API.Data;
using API.Helpers;
using API.Interfaces;
using API.Services;
using API.SingalR;
using Microsoft.EntityFrameworkCore;

namespace API.Extensions
{
    public static class ApplicationServiceExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration _config)
        {
            services.AddSingleton<PresenceTracker>();
            services.Configure<CloudinarySettings>(_config.GetSection("CloudinarySettings"));
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IPhotoService, PhotoService>();
            services.AddScoped<LogUserActivity>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<ILikesRepository, LikesRepository>();
            services.AddScoped<IMessageRepository, MessageRepository>();
            services.AddAutoMapper(typeof(AutoMapperProfiles).Assembly); 
            services.AddDbContext<DataContext>(options => 
            {
                options.UseSqlServer(_config.GetConnectionString("DefaultConnection"));
            });
            return services;
        }
    }
}
