using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TeachPlanner.Api.Database.Converters;
using TeachPlanner.Api.Domain.Curriculum;
using TeachPlanner.Api.Domain.LessonPlans;
using TeachPlanner.Api.Domain.Teachers;
using TeachPlanner.Shared.Enums;

namespace TeachPlanner.Api.Database.Configurations;

public class ResourceConfiguration : IEntityTypeConfiguration<Resource>
{
    public void Configure(EntityTypeBuilder<Resource> builder)
    {
        builder.ToTable("resources");

        builder.HasKey(r => r.Id);

        builder.Property(r => r.Id)
            .HasColumnName("Id")
            .HasConversion(new StronglyTypedIdConverter.ResourceIdConverter());

        builder.Property(r => r.Name)
            .HasMaxLength(500);

        builder.Property(r => r.Url)
            .HasMaxLength(300);

        builder.Property(r => r.AssociatedStrands)
            .HasConversion(
                v => string.Join(',', v),
                v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList(),
                null);

        builder.HasOne<CurriculumSubject>()
            .WithMany()
            .HasForeignKey(r => r.SubjectId)
            .IsRequired();

        builder.HasMany(r => r.LessonPlans)
            .WithMany(lp => lp.Resources)
            .UsingEntity(
                l => l.HasOne(typeof(LessonPlan)).WithMany().OnDelete(DeleteBehavior.Restrict)
                    .HasForeignKey("LessonPlansId"),
                r => r.HasOne(typeof(Resource)).WithMany().OnDelete(DeleteBehavior.Restrict)
                    .HasForeignKey("ResourcesId"));

#pragma warning disable CS8600, CS8603, CS8604 // Converting null literal or possible null value to non-nullable type.
        builder.Property<List<YearLevelValue>>("_yearLevels")
            .HasColumnName("YearLevels")
            .HasMaxLength(100)
            .HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null),
                v => JsonSerializer.Deserialize<List<YearLevelValue>>(v, (JsonSerializerOptions)null),
                new ValueComparer<List<YearLevelValue>>(
                    (c1, c2) => c1.SequenceEqual(c2),
                    c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                    c => c.ToList()));
    }
}