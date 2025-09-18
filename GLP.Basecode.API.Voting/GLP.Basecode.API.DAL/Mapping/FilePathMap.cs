using GLP.Basecode.API.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GLP.Basecode.API.DAL.Mapping
{
    public class FilePathMap : IEntityTypeConfiguration<FilePath>
    {
        public void Configure(EntityTypeBuilder<FilePath> modelBuilder)
        {
            //Table
            modelBuilder.ToTable("FilePaths");

            //PK
            modelBuilder.HasKey(f => f.FilePathId);

            //Properties
            modelBuilder.Property(f => f.Path)
                .HasColumnType("nvarchar(max)");

            modelBuilder.HasMany(f => f.Candidates)
                .WithOne(f => f.FilePath);

            modelBuilder.HasMany(f => f.Users)
                .WithOne(f => f.FilePath);

            modelBuilder.HasMany(f => f.PartyLists)
                .WithOne(f => f.FilePath);
        }
    }
}
