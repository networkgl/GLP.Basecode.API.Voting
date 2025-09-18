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
    public class PartyListMap : IEntityTypeConfiguration<PartyList>
    {
        public void Configure(EntityTypeBuilder<PartyList> modelBuilder)
        {
            //Table
            modelBuilder.ToTable("PartyLists");

            //PK
            modelBuilder.HasKey(p => p.PartyListId);

            //Properties
            modelBuilder.Property(p => p.PartyListName)
                .HasColumnName("partyListName")
                .HasMaxLength(100)
                .IsRequired();

            modelBuilder.Property(p => p.CreatedAt)
                .HasColumnName("createdAt")
                .HasColumnType("datetime")
                .IsRequired();

            modelBuilder.Property(p => p.IsCompleted)
                .HasColumnName("isCompleted")
                .IsRequired(false);

            //FK
            modelBuilder.HasOne(p => p.FilePath)
                .WithMany(p => p.PartyLists)
                .HasForeignKey(p => p.FilePathId);

        }
    }
}
