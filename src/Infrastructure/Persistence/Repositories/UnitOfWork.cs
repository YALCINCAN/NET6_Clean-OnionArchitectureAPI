using Application.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore.Storage;
using Persistence.Context;
using System.Data;

namespace Persistence.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly CAContext _context;

        public UnitOfWork(CAContext context)
        {
            _context = context;
        }

        public IDbTransaction BeginTransaction()
        {
            var transaction = _context.Database.BeginTransaction();
            return transaction.GetDbTransaction();
        }

        public void SaveChanges()
        {
            _context.SaveChanges();
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
