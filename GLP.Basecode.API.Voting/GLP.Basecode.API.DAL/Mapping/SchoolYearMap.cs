using GLP.Basecode.API.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GLP.Basecode.API.DAL.Mapping
{
    public class SchoolYearMap : IEntityTypeConfiguration<SchoolYear>
    {
        public void Configure(EntityTypeBuilder<SchoolYear> modelBuilder)
        {
            //Table
            modelBuilder.ToTable("SchoolYears");

            //PK
            modelBuilder.HasKey(s => s.SyId);

            //Properties
            modelBuilder.Property(s => s.FromSy)
                .HasColumnName("fromSy")
                .IsRequired();

            modelBuilder.Property(s => s.ToSy)
               .HasColumnName("toSy")
               .IsRequired();

            modelBuilder.HasMany(s => s.Students)
                .WithOne(s => s.Sy);

            modelBuilder.HasMany(s => s.Positions)
                .WithOne(s => s.Sy);
        }
    }
}
