using Microsoft.Extensions.DependencyInjection;
using Quartz;
using System.Reflection;

namespace AB.QuartzAdmin.WebApi
{
    /// <summary>
    /// Extension class for adding the api.
    /// </summary>
    public static class QuartzAdminStartup
    {
        /// <summary>
        /// Adds the QuartzAdmin Api to the middleware.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/></param>
        /// <param name="scheduler">The <see cref="IScheduler"/></param>
        public static void AddQuartzAdmin(this IServiceCollection services, IScheduler scheduler)
        {
            services.AddMvcCore()
                .AddApplicationPart(Assembly.GetExecutingAssembly());

            services.AddSingleton(scheduler);
        }
    }
}
