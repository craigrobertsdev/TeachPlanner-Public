using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TeachPlanner.Shared.ValueObjects;

namespace TeachPlanner.Api.Database.Configurations;

public class TermDatesConfiguration : IEntityTypeConfiguration<TermDate>
{
    public void Configure(EntityTypeBuilder<TermDate> builder)
    {
        builder.ToTable("term_dates");
        builder.Property<Guid>("Id");
        builder.HasKey("Id");
    }
}