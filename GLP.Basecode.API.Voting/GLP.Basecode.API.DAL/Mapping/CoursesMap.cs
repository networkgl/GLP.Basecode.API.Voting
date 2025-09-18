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
    public class CoursesMap : IEntityTypeConfiguration<Course>
    {
        public void Configure(EntityTypeBuilder<Course> modelBuilder)
        {
            //Table
            modelBuilder.ToTable("Courses");

            modelBuilder.HasKey(c => c.CourseId);

            //Properties
            modelBuilder.Property(c => c.CourseName)
                .HasColumnName("courseName")
                .HasMaxLength(100)
                .IsRequired();

            modelBuilder.Property(c => c.CourseAbbreviation)
                .HasColumnName("courseAbbreviation")
                .HasMaxLength(100)
                .IsRequired();

            modelBuilder.Property(c => c.DepartmentName)
                .HasColumnName("departmentName")
                .HasMaxLength(100)
                .IsRequired();

            modelBuilder.Property(c => c.DepartmentAbbreviation)
                .HasColumnName("departmentAbbreviation")
                .HasMaxLength(100)
                .IsRequired();

            modelBuilder.HasMany(c => c.Students)
                .WithOne(c => c.Course);
        }
    }
}
