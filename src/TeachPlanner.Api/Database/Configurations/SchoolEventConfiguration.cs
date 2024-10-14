using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TeachPlanner.Api.Database.Converters;
using TeachPlanner.Shared.ValueObjects;

namespace TeachPlanner.Api.Database.Configurations;

public class SchoolEventConfiguration : IEntityTypeConfiguration<SchoolEvent>
{
    public void Configure(EntityTypeBuilder<SchoolEvent> builder)
    {
        builder.ToTable("school_events");
        builder.HasKey(se => se.Id);

        builder.Property(se => se.Id)
            .HasColumnName("Id")
            .HasConversion(new StronglyTypedIdConverter.SchoolEventIdConverter());

        builder.Property(se => se.Name)
            .HasMaxLength(100)
            .IsRequired();

        builder.HasOne(se => se.Location)
            .WithMany();
    }
}

public class LocationConfiguration : IEntityTypeConfiguration<Location>
{
    public void Configure(EntityTypeBuilder<Location> builder)
    {
        builder.ToTable("locations");
        builder.Property<Guid>("Id");
        builder.HasKey("Id");
    }
}