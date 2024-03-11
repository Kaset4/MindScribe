using MindScribe.Repositories;

namespace MindScribe.Extentions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddCustomRepository<TEntity, TRepository>(this IServiceCollection services)
            where TEntity : class
            where TRepository : class, IRepository<TEntity>
        {
            services.AddScoped<IRepository<TEntity>, TRepository>();
            return services;
        }
    }
}
