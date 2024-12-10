using Contents.Services;
using Contents.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Contents.UnitTest
{
    public class DIServices
    {
        public ServiceProvider GenerateDependencyInjection()
        {
            var services = new ServiceCollection();
            services.AddScoped(typeof(IPaginationService<>), typeof(PaginationService<>));

            return services
                .AddLogging()
                .BuildServiceProvider();
        }
    }
}