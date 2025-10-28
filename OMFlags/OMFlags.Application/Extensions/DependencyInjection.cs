using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace OMFlags.Application.Extensions
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            // Ensure you have at least one handler in this assembly so it can scan something.
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
            return services;
        }
    }
}
