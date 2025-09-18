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
    public class CandidatePositionMap : IEntityTypeConfiguration<CandidatePosition>
    {
        public void Configure(EntityTypeBuilder<CandidatePosition> modelBuilder)
        {
            //Table
            modelBuilder.ToTable("CandidatePositions");

            //PK
            modelBuilder.HasKey(c => c.CanposId);
          
            modelBuilder.HasOne(c => c.Candidate)
                .WithMany(c => c.CandidatePositions);

            modelBuilder.HasOne(c => c.Position)
                .WithMany(c => c.CandidatePositions);
        }
    }
}
