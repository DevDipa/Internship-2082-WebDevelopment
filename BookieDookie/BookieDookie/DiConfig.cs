using BookieDookie.Services;
using BookieDookie.Services.Interface;

namespace BookieDookie;

public static class DiConfig
{
    //EXTENSION METHOD huna ko lagi class and method STATIC hunu paryo
    public static IServiceCollection Configure( this IServiceCollection services)
    {
        return services.AddScoped<IUserService, UserService>();
    }
}