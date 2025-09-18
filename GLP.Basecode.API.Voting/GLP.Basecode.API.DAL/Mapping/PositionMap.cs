using GLP.Basecode.API.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GLP.Basecode.API.DAL.Mapping
{
    public class PositionMap : IEntityTypeConfiguration<Position>
    {
        public void Configure(EntityTypeBuilder<Position> modelBuilder)
        {
            //Table
            modelBuilder.ToTable("Positions");

            modelBuilder.HasKey(p => p.PositionId);

            //Properties
            modelBuilder.Property(p => p.PositionName)
                .HasColumnName("positionName")
                .IsRequired();

            modelBuilder.Property(p => p.SequenceNumber)
                .HasColumnName("sequenceNumber")
                .IsRequired();
            //FK
            modelBuilder.HasOne(p => p.Sy)
                .WithMany(p => p.Positions)
                .HasForeignKey(p => p.SyId);

            modelBuilder.HasMany(p => p.CandidatePositions)
                .WithOne(p => p.Position);
        }
    }
}
