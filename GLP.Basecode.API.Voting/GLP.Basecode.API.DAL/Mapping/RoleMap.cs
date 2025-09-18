using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GLP.Basecode.API.Model;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GLP.Basecode.API.DAL.Mapping
{
    public class RoleMap : IEntityTypeConfiguration<Role>
    {
        public void Configure(EntityTypeBuilder<Role> builder)
        {
            builder.ToTable("Roles");

            // Primary Key
            builder.HasKey(r => r.RoleId);

            // Properties
            builder.Property(r => r.RoleName)
                   .IsRequired()
                   .HasMaxLength(100);

            // FK
            builder.HasMany(r => r.Users)
                   .WithOne(u => u.Role)
                   .HasForeignKey(u => u.RoleId);
        }
    }

}
