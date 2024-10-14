using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TeachPlanner.Api.Domain.WeekPlanners;

namespace TeachPlanner.Api.Database.Configurations;

public class DayPlanConfiguration : IEntityTypeConfiguration<DayPlan>
{
    public void Configure(EntityTypeBuilder<DayPlan> builder)
    {
        builder.ToTable("day_plans");

        builder.HasKey(dp => dp.Id);
        builder.Property(dp => dp.Id)
            .HasColumnName("Id")
            .HasConversion(new DayPlanId.StronglyTypedIdEfValueConverter());

        builder.HasMany(dp => dp.LessonPlans)
            .WithOne()
            .HasForeignKey("DayPlanId")
            .IsRequired();

        builder.HasMany(dp => dp.SchoolEvents)
            .WithMany();
    }
}