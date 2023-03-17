using System.Data;

namespace Application.Interfaces.Repositories
{
    public interface IUnitOfWork
    {
        Task SaveChangesAsync();
        void SaveChanges();

        IDbTransaction BeginTransaction();
    }
}
