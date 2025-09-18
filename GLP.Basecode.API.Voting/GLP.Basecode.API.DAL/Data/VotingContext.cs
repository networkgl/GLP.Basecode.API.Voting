using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GLP.Basecode.API.Model;
using System.Data;

namespace GLP.Basecode.API.DAL.Data
{
    public class VotingContext : DbContext
    {
        public VotingContext(DbContextOptions<VotingContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Candidate> Candidates { get; set; }

        public virtual DbSet<CandidatePosition> CandidatePositions { get; set; }

        public virtual DbSet<Course> Courses { get; set; }

        public virtual DbSet<ElectedCandidate> ElectedCandidates { get; set; }

        public virtual DbSet<FilePath> FilePaths { get; set; }

        public virtual DbSet<Notification> Notifications { get; set; }

        public virtual DbSet<PartyList> PartyLists { get; set; }

        public virtual DbSet<Position> Positions { get; set; }

        public virtual DbSet<Role> Roles { get; set; }

        public virtual DbSet<SchoolYear> SchoolYears { get; set; }

        public virtual DbSet<Student> Students { get; set; }

        public virtual DbSet<User> Users { get; set; }

        public virtual DbSet<UserVote> UserVotes { get; set; }

        public virtual DbSet<VwGetAllCandidatesByCurrentSchoolYear> VwGetAllCandidatesByCurrentSchoolYears { get; set; }

        public virtual DbSet<VwGetCandidateOrPartylistDetail> VwGetCandidateOrPartylistDetails { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
             => optionsBuilder.UseSqlServer("Data Source=DESKTOP-67QU08T\\SQLEXPRESS;Initial Catalog=VotingAppDB;Integrated Security=True;Connect Timeout=30;Encrypt=True;Trust Server Certificate=True;Application Intent=ReadWrite;Multi Subnet Failover=False");

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Applies all IEntityTypeConfiguration<T> classes in the assembly
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(VotingContext).Assembly);

            base.OnModelCreating(modelBuilder);
        }
    }
}
