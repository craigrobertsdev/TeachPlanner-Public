using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TeachPlanner.Shared.Domain.Common.Enums;
using TeachPlanner.Shared.Domain.Teachers;
using TeachPlanner.Shared.Domain.TermPlanners;
using TeachPlanner.Shared.Domain.YearDataRecords;
using TeachPlanner.Shared.Database.Converters;
using TeachPlanner.Shared.Domain.Curriculum;
using TeachPlanner.Shared.Domain.PlannerTemplates;

namespace TeachPlanner.Shared.Database.Configurations;

public class YearDataConfiguration : IEntityTypeConfiguration<YearData>
{
    public void Configure(EntityTypeBuilder<YearData> builder)
    {
        builder.ToTable("yeardata");
        builder.HasKey(yd => yd.Id);

        builder.Property(yd => yd.Id)
            .HasColumnName("Id")
            .HasConversion(new StronglyTypedIdConverter.YearDataIdConverter());

        builder.HasMany(yd => yd.Students)
            .WithOne();

        builder.HasOne<TermPlanner>()
            .WithOne()
            .HasForeignKey<TermPlanner>(tp => tp.YearDataId);

        builder.HasMany(yd => yd.LessonPlans)
            .WithOne()
            .HasForeignKey(lp => lp.YearDataId);

        builder.HasOne<Teacher>()
            .WithMany()
            .HasForeignKey(yd => yd.TeacherId);

        builder.HasMany(yd => yd.WeekPlanners)
            .WithOne()
            .HasForeignKey(wp => wp.YearDataId);

        builder.HasOne(yd => yd.WeekStructure)
            .WithOne()
            .HasForeignKey<WeekStructure>("YearDataId");

#pragma warning disable CS8600, CS8603, CS8604 // Converting null literal or possible null value to non-nullable type.
        builder.Property<List<YearLevelValue>>("_yearLevelsTaught")
            .HasColumnName("YearLevels")
            .HasMaxLength(100)
            .HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null),
                v => JsonSerializer.Deserialize<List<YearLevelValue>>(v, (JsonSerializerOptions)null),
                new ValueComparer<List<YearLevelValue>>(
                    (c1, c2) => c1.SequenceEqual(c2),
                    c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                    c => c.ToList()));


        builder.OwnsMany(yd => yd.Subjects, sb =>
        {
            sb.ToTable("subjects");
            sb.WithOwner().HasForeignKey("YearDataId");

            sb.Property<Guid>("Id");
            sb.HasKey("Id");

            sb.Property(s => s.Name)
                .HasColumnName("Name")
                .HasMaxLength(50);

            sb.HasOne<CurriculumSubject>()
                .WithMany()
                .HasForeignKey("CurriculumSubjectId")
                .IsRequired()
                .OnDelete(DeleteBehavior.NoAction);

            sb.OwnsMany(s => s.ContentDescriptions, cdb =>
            {
                cdb.ToTable("year_data_content_descriptions");
                cdb.WithOwner().HasForeignKey("SubjectId");

                cdb.Property<Guid>("Id");
                cdb.HasKey("Id");

                cdb.Property(cd => cd.CurriculumCode)
                    .HasMaxLength(15);
            });
        });
    }
}