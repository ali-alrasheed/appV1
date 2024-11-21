using Application.Domain.Madels;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.seedConfiguration
{
    public class RoleConfiguration : IEntityTypeConfiguration<Role>
    {
        public void Configure(EntityTypeBuilder<Role> builder)
        {
            builder.HasData(
                new Role
                {
                    Id = Guid.NewGuid(),
                    Name = "User",
                    NormalizedName = "USER",
                    Description = " the visitor role for the user "
                },
                new Role
                {
                    Id = Guid.NewGuid(),
                    Name = "Admin",
                    NormalizedName = "ADMIN",
                    Description = " the admin role for the user "
                },
                new Role
                {
                    Id = Guid.NewGuid(),
                    Name = "Owner",
                    NormalizedName = "OWNER",
                    Description = " the Owner role for the user "
                },
                new Role
                {
                    Id = Guid.NewGuid(),
                    Name = "Driver",
                    NormalizedName = "DRIVER",
                    Description = " the Driver role for the user "
                }

                );
        }
    }

}
