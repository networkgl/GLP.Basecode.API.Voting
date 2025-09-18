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
    public class ElectedCandidateMap : IEntityTypeConfiguration<ElectedCandidate>
    {
        public void Configure(EntityTypeBuilder<ElectedCandidate> modelBuilder)
        {
            //Table
            modelBuilder.ToTable("ElectedCandidates");

            //PK
            modelBuilder.HasKey(e => e.ElecCanId);

            //Properties
            modelBuilder.Property(e => e.CandidateName)
                .HasColumnName("candidateName")
                .HasMaxLength(100);

            modelBuilder.Property(e => e.PositionName)
                .HasColumnName("positionName")
                .HasMaxLength(100);

            modelBuilder.Property(e => e.CourseName)
                .HasColumnName("courseName")
                .HasMaxLength(50);

            modelBuilder.Property(e => e.CourseYear)
                .HasColumnName("CourseYear");

            modelBuilder.Property(e => e.VoteCount)
                .HasColumnName("voteCount");

            modelBuilder.Property(e => e.FromSy)
                .HasColumnName("fromSy");

            modelBuilder.Property(e => e.ToSy)
                .HasColumnName("ToSy");
        }
    }
}
