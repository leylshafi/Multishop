using Microsoft.EntityFrameworkCore;
using Multishop.Data;
using Multishop.Services;
using System.Reflection;

namespace Multishop
{
    public static class DI
    {
        public static IServiceCollection AddDbConfig(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<AppDbContext>(o => o.UseSqlServer(configuration.GetConnectionString("Default")));
            services.AddAutoMapper(Assembly.GetExecutingAssembly());
            services.AddScoped<LayoutService>();
            return services;
        }
    }
}
