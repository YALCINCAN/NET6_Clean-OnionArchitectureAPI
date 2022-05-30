using Application.Interfaces.Repositories;
using Domain.Entities;
using Persistence.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persistence.Repositories
{
    public class RefreshTokenRepository : EfEntityRepository<RefreshToken, CAContext, int>, IRefreshTokenRepository
    {
        public RefreshTokenRepository(CAContext context) : base(context)
        {
            
        }
    }
}
