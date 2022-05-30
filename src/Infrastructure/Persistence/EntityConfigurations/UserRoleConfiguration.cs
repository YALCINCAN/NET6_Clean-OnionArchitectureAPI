using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persistence.EntityConfigurations
{
    public class UserRoleConfiguration : IEntityTypeConfiguration<UserRole>
    {
        public void Configure(EntityTypeBuilder<UserRole> builder)
        {
            const string AdminRoleId = "43db034a-98cc-42ee-8fff-c57016484f4d";
            const string DefaultAdminUserId = "6e5d8fa8-fa96-419f-9c07-3e05b96b087e";

            builder.HasKey(ur => new { ur.UserId, ur.RoleId });

            builder.HasData(
                new UserRole
                {
                    UserId = Guid.Parse(DefaultAdminUserId),
                    RoleId = Guid.Parse(AdminRoleId)
                });
        }
    }
}
