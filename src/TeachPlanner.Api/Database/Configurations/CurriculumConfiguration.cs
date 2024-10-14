using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TeachPlanner.Api.Database.Converters;
using TeachPlanner.Api.Domain.Curriculum;
using TeachPlanner.Shared.Enums;

namespace TeachPlanner.Api.Database.Configurations;

public class CurriculumConfiguration : IEntityTypeConfiguration<CurriculumSubject>
{
    public void Configure(EntityTypeBuilder<CurriculumSubject> builder)
    {
        builder.ToTable("curriculum_subjects");
        builder.HasKey(s => s.Id);

        builder.Property(s => s.Id)
            .HasColumnName("Id")
            .HasConversion(new StronglyTypedIdConverter.CurriculumSubjectIdConverter());

        builder.Property(s => s.Name)
            .HasMaxLength(50);

        builder.HasMany(s => s.YearLevels)
            .WithOne()
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
    }

    public class YearLevelConfiguration : IEntityTypeConfiguration<YearLevel>
    {
        public void Configure(EntityTypeBuilder<YearLevel> builder)
        {
            builder.ToTable("year_levels");
            builder.Property<Guid>("Id");
            builder.HasKey("Id");

            builder.Ignore(yl => yl.Name);

            builder.Property(yl => yl.YearLevelValue)
                .HasConversion(
                    v => (int)v,
                    v => (YearLevelValue)v);

            builder.OwnsMany(yl => yl.Capabilities, cb =>
            {
                cb.ToTable("capabilities");
                cb.Property<Guid>("Id");
                cb.HasKey("Id");
                cb.WithOwner().HasForeignKey("YearLevelId");
            });

            builder.OwnsMany(yl => yl.Dispositions, db =>
            {
                db.ToTable("dispositions");
                db.Property<Guid>("Id");
                db.HasKey("Id");
                db.WithOwner().HasForeignKey("YearLevelId");
            });

            builder.OwnsMany(yl => yl.ConceptualOrganisers, cb =>
            {
                cb.ToTable("conceptual_organisers");
                cb.Property<Guid>("Id");
                cb.HasKey("Id");
                cb.WithOwner().HasForeignKey("YearLevelId");

                cb.OwnsMany(cb => cb.ContentDescriptions, cdb =>
                {
                    cdb.ToTable("content_descriptions");
                    cdb.HasKey(cd => cd.Id);
                    cdb.WithOwner(cd => cd.ConceptualOrganiser).HasForeignKey("ConceptualOrganiserId");
                });
            });
        }
    }
}