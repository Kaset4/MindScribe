using Microsoft.EntityFrameworkCore.Infrastructure;
using MindScribe.Repositories;
using System.Diagnostics.CodeAnalysis;

namespace MindScribe.Data.UoW
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _appContext;

        private Dictionary<Type, object> _repositories;

        public UnitOfWork(ApplicationDbContext app)
        {
            this._appContext = app;
            this._repositories = new Dictionary<Type, object>();
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1816:Dispose methods should call SuppressFinalize", Justification = "Не требуется для этого класса")]
        public void Dispose()
        {

        }

        [SuppressMessage("Style", "IDE0059:Nenushnoe prisvaivanie znacheniya", Justification = "Необходимо для использования переменной repository в блоке условия")]
        public IRepository<TEntity> GetRepository<TEntity>(bool hasCustomRepository = true) where TEntity : class
        {
            _repositories ??= new Dictionary<Type, object>();

            if (hasCustomRepository)
            {
                var customRepo = _appContext.GetService<IRepository<TEntity>>();
                if (customRepo != null)
                {
                    return customRepo;
                }
            }

            var type = typeof(TEntity);
            if (!_repositories.TryGetValue(type, out var repository))
            {
                _repositories[type] = new Repository<TEntity>(_appContext);
            }

            return (IRepository<TEntity>)_repositories[type];

        }
        public int SaveChanges(bool ensureAutoHistory = false)
        {
            throw new NotImplementedException();
        }
    }
}
