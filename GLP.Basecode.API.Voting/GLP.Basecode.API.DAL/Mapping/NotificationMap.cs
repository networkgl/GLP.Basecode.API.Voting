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
    public class NotificationMap : IEntityTypeConfiguration<Notification>
    {
        public void Configure(EntityTypeBuilder<Notification> modelBuilder)
        {
            //Table
            modelBuilder.ToTable("Notifications");

            //PK
            modelBuilder.HasKey(n => n.NotifId);

            //Properties
            modelBuilder.Property(n => n.Message)
                .HasColumnName("message")
                .HasColumnType("nvarchar(max)");
            
            //FK
            modelBuilder.HasOne(n => n.Student)
                .WithMany(n => n.Notifications)
                .HasForeignKey(n => n.StudentId);
        }
    }
}
