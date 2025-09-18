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
    public class StudentMap : IEntityTypeConfiguration<Student>
    {
        public void Configure(EntityTypeBuilder<Student> modelBuilder)
        {
            //Table
            modelBuilder.ToTable("Students");

            //PK
            modelBuilder.HasKey(s => s.StudentId);

            //Properties
            modelBuilder.Property(s => s.IdNumber)
                .HasColumnName("idNumber")
                .HasMaxLength(50)
                .IsRequired();

            modelBuilder.Property(s => s.FirstName)
                .HasColumnName("firstName")
                .HasMaxLength(50)
                .IsRequired();

            modelBuilder.Property(s => s.LastName)
                .HasColumnName("lastName")
                .HasMaxLength(50)
                .IsRequired();

            modelBuilder.Property(s => s.MiddleName)
                .HasColumnName("middleName")
                .HasMaxLength(50)
                .IsRequired(false);

            //FK
            modelBuilder.HasMany(s => s.Candidates)
                .WithOne(s => s.Student)
                .HasForeignKey(s => s.CandidateId);

            //FK
            modelBuilder.HasMany(s => s.Notifications)
                .WithOne(s => s.Student)
                .HasForeignKey(s => s.StudentId);

            //FK
            modelBuilder.HasOne(s => s.Sy)
                .WithMany(s => s.Students)
                .HasForeignKey(s => s.SyId);

            //FK
            modelBuilder.HasOne(s => s.Course)
                .WithMany(s => s.Students)
                .HasForeignKey(s => s.CourseId);

        }
    }
}
