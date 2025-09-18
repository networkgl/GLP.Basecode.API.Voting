using GLP.Basecode.API.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class VwGetCandidateOrPartylistDetailMap
    : IEntityTypeConfiguration<VwGetCandidateOrPartylistDetail>
{
    public void Configure(EntityTypeBuilder<VwGetCandidateOrPartylistDetail> modelBuilder)
    {
        // Map to database view
        modelBuilder.ToView("VwGetCandidateOrPartylistDetail");

        // This entity has no primary key
        modelBuilder.HasNoKey();

        // Column mappings 
        modelBuilder.Property(v => v.PartyListId).HasColumnName("PartyListId");
        modelBuilder.Property(v => v.CandidateId).HasColumnName("CandidateId");
        modelBuilder.Property(v => v.PositionId).HasColumnName("PositionId");
        modelBuilder.Property(v => v.StudentId).HasColumnName("StudentId");
        modelBuilder.Property(v => v.CandidateName).HasColumnName("CandidateName");
        modelBuilder.Property(v => v.PartyListName).HasColumnName("PartyListName");
        modelBuilder.Property(v => v.PositionName).HasColumnName("PositionName");
        modelBuilder.Property(v => v.FilePathId).HasColumnName("FilePathId");
        modelBuilder.Property(v => v.CandidateImgPath).HasColumnName("CandidateImgPath");
        modelBuilder.Property(v => v.PartyListImgPath).HasColumnName("PartyListImgPath");




    }
}
