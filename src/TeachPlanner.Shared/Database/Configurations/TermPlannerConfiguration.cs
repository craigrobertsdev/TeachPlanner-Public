using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TeachPlanner.Shared.Domain.Common.Enums;
using TeachPlanner.Shared.Domain.TermPlanners;
using TeachPlanner.Shared.Domain.YearDataRecords;
using TeachPlanner.Shared.Database.Converters;

namespace TeachPlanner.Shared.Database.Configurations;

public class TermPlannerConfiguration : IEntityTypeConfiguration<TermPlanner>
{
    public void Configure(EntityTypeBuilder<TermPlanner> builder)
    {
        builder.ToTable("term_planner");

        builder.HasKey(tp => tp.Id);

        builder.Property(tp => tp.Id)
            .HasColumnName("Id")
            .HasConversion(new StronglyTypedIdConverter.TermPlannerIdConverter());

        builder.Property(tp => tp.CalendarYear)
            .IsRequired();

        builder.HasOne<YearData>()
            .WithOne()
            .HasForeignKey<YearData>(yd => yd.TermPlannerId);

#pragma warning disable CS8600, CS8603, CS8604 // Converting null literal or possible null value to non-nullable type.
        builder.Property<List<YearLevelValue>>("_yearLevels")
            .HasColumnName("YearLevels")
            .HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null),
                v => JsonSerializer.Deserialize<List<YearLevelValue>>(v, (JsonSerializerOptions)null),
                new ValueComparer<List<YearLevelValue>>(
                    (c1, c2) => c1.SequenceEqual(c2),
                    c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                    c => c.ToList()));
    }
}

public class TermPlanConfiguration : IEntityTypeConfiguration<TermPlan>
{
    public void Configure(EntityTypeBuilder<TermPlan> builder)
    {
        builder.ToTable("term_plans");

        builder.Property<Guid>("Id");
        builder.HasKey("Id");

        builder.HasMany(tp => tp.Subjects)
            .WithMany();

        builder.HasOne(tp => tp.TermPlanner)
            .WithMany(tp => tp.TermPlans)
            .HasForeignKey("TermPlannerId");
    }
}