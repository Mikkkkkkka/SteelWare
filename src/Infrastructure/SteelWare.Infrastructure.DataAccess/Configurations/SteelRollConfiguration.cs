using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SteelWare.Domain;

namespace SteelWare.Infrastructure.DataAccess.Configurations;

public class SteelRollConfiguration : IEntityTypeConfiguration<SteelRoll>
{
    public void Configure(EntityTypeBuilder<SteelRoll> builder)
    {
        builder.ToTable("steel_rolls");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.Length).HasColumnName("length");
        builder.Property(x => x.Weight).HasColumnName("weight");
        builder.Property(x => x.AddedAt).HasColumnName("added_at");
        builder.Property(x => x.DeletedAt).HasColumnName("deleted_at");
    }
}