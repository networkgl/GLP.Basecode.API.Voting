using Microsoft.EntityFrameworkCore.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GLP.Basecode.API.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GLP.Basecode.API.DAL.Mapping
{
    public class UserMap : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> modelBuilder)
        {
            //Table
            modelBuilder.ToTable("Users");

            //PK
            modelBuilder.HasKey(u => u.UserId);

            //Properties
            modelBuilder.Property(u => u.Username)
                .HasColumnName("userName")
                .HasMaxLength(100)
                .IsRequired();

            modelBuilder.Property(u => u.Password)
                .HasColumnName("passWord")
                .HasMaxLength(100)
                .IsRequired();

            modelBuilder.Property(u => u.UserEmail)
                .HasColumnName("userEmail")
                .HasMaxLength(100);

            modelBuilder.Property(u => u.UserOtp)
                .HasColumnName("userOTP")
                .IsRequired(false);

            modelBuilder.Property(u => u.IsVoted)
                .HasColumnName("isVoted")
                .IsRequired(false);

            modelBuilder.Property(u => u.VerifiedAt)
                .HasColumnName("verifiedAt")
                .IsRequired(false);

            modelBuilder.Property(u => u.StudentId)
                .HasColumnName("studentId")
                .IsRequired(false);

            modelBuilder.Property(u => u.FilePathId)
                .HasColumnName("filePathId")
                .IsRequired(false);

            modelBuilder.Property(u => u.RoleId)
                .HasColumnName("roleId")
                .IsRequired(true);

            //FK
            modelBuilder.HasMany(u => u.UserVotes)
                .WithOne(u => u.User)
                .HasForeignKey(u => u.UserId);

            //FK
            modelBuilder.HasOne(u => u.Role)
                .WithMany(u => u.Users)
                .HasForeignKey(u => u.RoleId);
        }
    }
}
