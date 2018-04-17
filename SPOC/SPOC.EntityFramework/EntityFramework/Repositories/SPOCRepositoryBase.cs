using Abp.Domain.Entities;
using Abp.EntityFramework;
using Abp.EntityFramework.Repositories;

namespace SPOC.EntityFramework.Repositories
{
    public abstract class SPOCRepositoryBase<TEntity, TPrimaryKey> : EfRepositoryBase<SPOCDbContext, TEntity, TPrimaryKey>
        where TEntity : class, IEntity<TPrimaryKey>
    {
        protected SPOCRepositoryBase(IDbContextProvider<SPOCDbContext> dbContextProvider)
            : base(dbContextProvider)
        {

        }

        //add common methods for all repositories
    }

    public abstract class SPOCRepositoryBase<TEntity> : SPOCRepositoryBase<TEntity, int>
        where TEntity : class, IEntity<int>
    {
        protected SPOCRepositoryBase(IDbContextProvider<SPOCDbContext> dbContextProvider)
            : base(dbContextProvider)
        {

        }

        //do not add any method here, add to the class above (since this inherits it)
    }
}
