using GLP.Basecode.API.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class VwGetAllCandidatesByCurrentSchoolYearMap
    : IEntityTypeConfiguration<VwGetAllCandidatesByCurrentSchoolYear>
{
    public void Configure(EntityTypeBuilder<VwGetAllCandidatesByCurrentSchoolYear> modelBuilder)
    {
        // Map to database view
        modelBuilder.ToView("VwGetAllCandidatesByCurrentSchoolYear");

        // This entity has no primary key
        modelBuilder.HasNoKey();

        // Column mappings 
        modelBuilder.Property(v => v.CandidateId).HasColumnName("CandidateId");
        modelBuilder.Property(v => v.CandidateName).HasColumnName("CandidateName");
        modelBuilder.Property(v => v.Position).HasColumnName("Position");
        modelBuilder.Property(v => v.SyFrom).HasColumnName("SyFrom");
        modelBuilder.Property(v => v.SyTo).HasColumnName("SyTo");
    }
}
