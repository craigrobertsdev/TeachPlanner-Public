using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TeachPlanner.Api.Domain.PlannerTemplates;
using TeachPlanner.Api.Domain.Teachers;
using TeachPlanner.Shared.Enums;
using TeachPlanner.Shared.StronglyTypedIds;

namespace TeachPlanner.Api.Database.Configurations;

public class WeekStructureConfiguration : IEntityTypeConfiguration<WeekStructure>
{
    public void Configure(EntityTypeBuilder<WeekStructure> builder)
    {
        builder.ToTable("week_structures");
        builder.HasKey(dp => dp.Id);
        builder.Property(dp => dp.Id)
            .HasConversion(new WeekStructureId.StronglyTypedIdEfValueConverter());

        builder.HasOne<Teacher>()
            .WithMany()
            .HasForeignKey(dp => dp.TeacherId);

        builder.OwnsMany(dp => dp.Periods, pb =>
        {
            pb.ToTable("template_periods");
            pb.Property<Guid>("Id");
            pb.HasKey("Id");
            pb.Property(p => p.Name)
                .HasMaxLength(50);

            pb.Property(p => p.PeriodType)
                .HasConversion(
                    v => v.ToString(),
                    v => Enum.Parse<PeriodType>(v))
                .HasMaxLength(20);
        });

        builder.OwnsMany(ws => ws.DayTemplates, dtb =>
        {
            dtb.ToTable("day_templates");
            dtb.Property<Guid>("Id");
            dtb.HasKey("Id");
            dtb.WithOwner().HasForeignKey("WeekStructureId");

            dtb.OwnsMany(dt => dt.Lessons, ltb =>
            {
                ltb.ToTable("day_template_lessons");
                ltb.Property<Guid>("Id");
                ltb.HasKey("Id");
                ltb.WithOwner().HasForeignKey("DayTemplateId");

                ltb.Property(lt => lt.SubjectName)
                    .HasMaxLength(50);
            });
        });
    }
}