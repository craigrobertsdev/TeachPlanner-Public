using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TeachPlanner.Api.Database.Converters;
using TeachPlanner.Api.Domain.Curriculum;
using TeachPlanner.Api.Domain.LessonPlans;
using TeachPlanner.Api.Domain.Teachers;
using TeachPlanner.Api.Domain.YearDataRecords;

namespace TeachPlanner.Api.Database.Configurations;

public class LessonPlanConfiguration : IEntityTypeConfiguration<LessonPlan>
{
    public void Configure(EntityTypeBuilder<LessonPlan> builder)
    {
        builder.ToTable("lesson_plans");

        builder.HasKey(lp => lp.Id);

        builder.Property(lp => lp.Id)
            .HasColumnName("Id")
            .HasConversion(new StronglyTypedIdConverter.LessonPlanIdConverter());

        builder.HasOne<YearData>()
            .WithMany(yd => yd.LessonPlans)
            .HasForeignKey(lp => lp.YearDataId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<CurriculumSubject>()
            .WithMany()
            .HasForeignKey(lp => lp.SubjectId);

        builder.HasMany(lp => lp.Resources)
            .WithMany(r => r.LessonPlans)
            .UsingEntity(
                l => l.HasOne(typeof(LessonPlan)).WithMany().OnDelete(DeleteBehavior.Restrict)
                    .HasForeignKey("LessonPlansId"),
                r => r.HasOne(typeof(Resource)).WithMany().OnDelete(DeleteBehavior.Restrict)
                    .HasForeignKey("ResourcesId"));

        builder.Property(lp => lp.ContentDescriptionIds)
            .HasConversion(
                v => string.Join(',', v),
                v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(Guid.Parse).ToList());

        builder.OwnsMany(lp => lp.Comments, lcb =>
        {
            lcb.ToTable("lesson_comment");

            lcb.WithOwner().HasForeignKey("LessonPlanId");

            lcb.Property<Guid>("Id");

            lcb.HasKey("Id", "LessonPlanId");
        });
    }
}