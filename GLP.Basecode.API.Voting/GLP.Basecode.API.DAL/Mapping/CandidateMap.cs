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
    public class CandidateMap : IEntityTypeConfiguration<Candidate>
    {
        public void Configure(EntityTypeBuilder<Candidate> modelBuilder)
        {
            //Table
            modelBuilder.ToTable("Candidates");

            //PK
            modelBuilder.HasKey(c => c.CandidateId);

            //FK
            modelBuilder.HasOne(c => c.PartyList)
                .WithMany(c => c.Candidates)
                .HasForeignKey(c => c.PartyListId);

            //FK
            modelBuilder.HasOne(c => c.FilePath)
                .WithMany(c => c.Candidates)
                .HasForeignKey(c => c.FilePathId);

            //FK
            modelBuilder.HasMany(c => c.UserVotes)
                .WithOne(c => c.Candidate)
                .HasForeignKey(c => c.CandidateId);
        }
    }
}
