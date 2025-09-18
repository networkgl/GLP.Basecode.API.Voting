using GLP.Basecode.API.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GLP.Basecode.API.DAL.Mapping
{
    public class UserVoteMap : IEntityTypeConfiguration<UserVote>
    {
        public void Configure(EntityTypeBuilder<UserVote> modelBuilder)
        {
            //Table
            modelBuilder.ToTable("UserVotes");

            //PK
            modelBuilder.HasKey(u => u.VoteId);

            //Properties
            modelBuilder.Property(u => u.VotedAt)
                .HasColumnType("datetime")
                .IsRequired();

            //FK
            modelBuilder.HasOne(u => u.Candidate)
                .WithMany(u => u.UserVotes)
                .HasForeignKey(u => u.CandidateId);

            //FK
            modelBuilder.HasOne(u => u.User)
                .WithMany(u => u.UserVotes)
                .HasForeignKey(u => u.UserId);
        }
    }
}
