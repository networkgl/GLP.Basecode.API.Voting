using System;
using System.Collections.Generic;
using GLP.Basecode.API.Voting.Models;
using Microsoft.EntityFrameworkCore;

namespace GLP.Basecode.API.Voting;

public partial class VotingAppDbContext : DbContext
{
    public VotingAppDbContext()
    {
    }

    public VotingAppDbContext(DbContextOptions<VotingAppDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Candidate> Candidates { get; set; }

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

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=DESKTOP-UDTRL17\\SQLEXPRESS;Initial Catalog=VotingAppDB;Integrated Security=True;Connect Timeout=30;Encrypt=True;Trust Server Certificate=True;Application Intent=ReadWrite;Multi Subnet Failover=False");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Candidate>(entity =>
        {
            entity.Property(e => e.CandidateId).HasColumnName("candidateId");
            entity.Property(e => e.FilePathId).HasColumnName("filePathId");
            entity.Property(e => e.PartyListId).HasColumnName("partyListId");

            entity.HasOne(d => d.FilePath).WithMany(p => p.Candidates)
                .HasForeignKey(d => d.FilePathId)
                .HasConstraintName("FK_Candidates_FilePaths");

            entity.HasOne(d => d.PartyList).WithMany(p => p.Candidates)
                .HasForeignKey(d => d.PartyListId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Candidates_PartyLists");
        });

        modelBuilder.Entity<Course>(entity =>
        {
            entity.Property(e => e.CourseId).HasColumnName("courseId");
            entity.Property(e => e.CourseAbbreviation)
                .HasMaxLength(50)
                .HasColumnName("courseAbbreviation");
            entity.Property(e => e.CourseName)
                .HasMaxLength(100)
                .HasColumnName("courseName");
            entity.Property(e => e.DepartmentAbbreviation)
                .HasMaxLength(50)
                .HasColumnName("departmentAbbreviation");
            entity.Property(e => e.DepartmentName)
                .HasMaxLength(50)
                .HasColumnName("departmentName");
        });

        modelBuilder.Entity<ElectedCandidate>(entity =>
        {
            entity.HasKey(e => e.ElecCanId);

            entity.Property(e => e.ElecCanId)
                .ValueGeneratedNever()
                .HasColumnName("elecCanId");
            entity.Property(e => e.CandidateName).HasColumnName("candidateName");
            entity.Property(e => e.CourseName)
                .HasMaxLength(50)
                .HasColumnName("courseName");
            entity.Property(e => e.CourseYear).HasColumnName("courseYear");
            entity.Property(e => e.FromSy).HasColumnName("fromSy");
            entity.Property(e => e.PositionName)
                .HasMaxLength(10)
                .IsFixedLength()
                .HasColumnName("positionName");
            entity.Property(e => e.ToSy).HasColumnName("toSy");
            entity.Property(e => e.VoteCount).HasColumnName("voteCount");
        });

        modelBuilder.Entity<FilePath>(entity =>
        {
            entity.Property(e => e.FilePathId).HasColumnName("filePathId");
            entity.Property(e => e.Path).HasColumnName("path");
        });

        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasKey(e => e.NotifId);

            entity.Property(e => e.NotifId).HasColumnName("notifId");
            entity.Property(e => e.Message)
                .IsUnicode(false)
                .HasColumnName("message");
            entity.Property(e => e.StudentId).HasColumnName("studentId");

            entity.HasOne(d => d.Student).WithMany(p => p.Notifications)
                .HasForeignKey(d => d.StudentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Notifications_Students");
        });

        modelBuilder.Entity<PartyList>(entity =>
        {
            entity.HasKey(e => e.PartyListId).HasName("PK_PartyList");

            entity.Property(e => e.PartyListId).HasColumnName("partyListId");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("datetime")
                .HasColumnName("createdAt");
            entity.Property(e => e.FilePathId).HasColumnName("filePathId");
            entity.Property(e => e.IsCompleted).HasColumnName("isCompleted");
            entity.Property(e => e.PartyListName)
                .HasMaxLength(100)
                .HasColumnName("partyListName");

            entity.HasOne(d => d.FilePath).WithMany(p => p.PartyLists)
                .HasForeignKey(d => d.FilePathId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PartyLists_FilePaths");
        });

        modelBuilder.Entity<Position>(entity =>
        {
            entity.Property(e => e.PositionId).HasColumnName("positionId");
            entity.Property(e => e.CandidateId).HasColumnName("candidateId");
            entity.Property(e => e.PositionName)
                .HasMaxLength(100)
                .HasColumnName("positionName");
            entity.Property(e => e.SequenceNumber).HasColumnName("sequenceNumber");
            entity.Property(e => e.SyId).HasColumnName("syId");

            entity.HasOne(d => d.Candidate).WithMany(p => p.Positions)
                .HasForeignKey(d => d.CandidateId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Positions_Candidates");

            entity.HasOne(d => d.Sy).WithMany(p => p.Positions)
                .HasForeignKey(d => d.SyId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Positions_SchoolYears1");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.Property(e => e.RoleId).HasColumnName("roleId");
            entity.Property(e => e.RoleName)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("roleName");
        });

        modelBuilder.Entity<SchoolYear>(entity =>
        {
            entity.HasKey(e => e.SyId);

            entity.Property(e => e.SyId).HasColumnName("syId");
            entity.Property(e => e.FromSy).HasColumnName("fromSy");
            entity.Property(e => e.ToSy).HasColumnName("toSy");
        });

        modelBuilder.Entity<Student>(entity =>
        {
            entity.Property(e => e.StudentId).HasColumnName("studentId");
            entity.Property(e => e.CandidateId).HasColumnName("candidateId");
            entity.Property(e => e.CourseId).HasColumnName("courseId");
            entity.Property(e => e.FirstName)
                .HasMaxLength(50)
                .HasColumnName("firstName");
            entity.Property(e => e.IdNumber).HasColumnName("idNumber");
            entity.Property(e => e.LastName)
                .HasMaxLength(50)
                .HasColumnName("lastName");
            entity.Property(e => e.MiddleName)
                .HasMaxLength(50)
                .HasColumnName("middleName");
            entity.Property(e => e.SyId).HasColumnName("syId");

            entity.HasOne(d => d.Candidate).WithMany(p => p.Students)
                .HasForeignKey(d => d.CandidateId)
                .HasConstraintName("FK_Students_Candidates");

            entity.HasOne(d => d.Course).WithMany(p => p.Students)
                .HasForeignKey(d => d.CourseId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Students_Courses");

            entity.HasOne(d => d.Sy).WithMany(p => p.Students)
                .HasForeignKey(d => d.SyId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Students_SchoolYears");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.Property(e => e.UserId).HasColumnName("userId");
            entity.Property(e => e.FilePathId).HasColumnName("filePathId");
            entity.Property(e => e.IsVoted).HasColumnName("isVoted");
            entity.Property(e => e.Password).HasColumnName("password");
            entity.Property(e => e.RoleId).HasColumnName("roleId");
            entity.Property(e => e.StudentId).HasColumnName("studentId");
            entity.Property(e => e.UserEmail)
                .HasMaxLength(100)
                .HasColumnName("userEmail");
            entity.Property(e => e.UserOtp).HasColumnName("userOTP");
            entity.Property(e => e.Username)
                .HasMaxLength(100)
                .HasColumnName("username");
            entity.Property(e => e.VerifiedAt)
                .HasColumnType("datetime")
                .HasColumnName("verifiedAt");

            entity.HasOne(d => d.FilePath).WithMany(p => p.Users)
                .HasForeignKey(d => d.FilePathId)
                .HasConstraintName("FK_Users_FilePaths");

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Users_Roles");

            entity.HasOne(d => d.Student).WithMany(p => p.Users)
                .HasForeignKey(d => d.StudentId)
                .HasConstraintName("FK_Users_Students");
        });

        modelBuilder.Entity<UserVote>(entity =>
        {
            entity.HasKey(e => e.VoteId);

            entity.Property(e => e.VoteId).HasColumnName("voteId");
            entity.Property(e => e.CandidateId).HasColumnName("candidateId");
            entity.Property(e => e.UserId).HasColumnName("userId");
            entity.Property(e => e.VotedAt)
                .HasColumnType("datetime")
                .HasColumnName("votedAt");

            entity.HasOne(d => d.Candidate).WithMany(p => p.UserVotes)
                .HasForeignKey(d => d.CandidateId)
                .HasConstraintName("FK_UserVotes_Candidates");

            entity.HasOne(d => d.User).WithMany(p => p.UserVotes)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_UserVotes_Users");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
