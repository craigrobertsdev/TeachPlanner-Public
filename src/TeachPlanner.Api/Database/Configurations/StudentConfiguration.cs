using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TeachPlanner.Api.Database.Converters;
using TeachPlanner.Api.Domain.Students;
using TeachPlanner.Api.Domain.Teachers;

namespace TeachPlanner.Api.Database.Configurations;

public class StudentConfiguration : IEntityTypeConfiguration<Student>
{
    public void Configure(EntityTypeBuilder<Student> builder)
    {
        builder.ToTable("students");
        builder.HasKey(s => s.Id);
        builder.Property(s => s.Id)
            .HasColumnName("Id")
            .HasConversion(new StronglyTypedIdConverter.StudentIdConverter());

        builder.HasMany(s => s.Reports)
            .WithOne()
            .HasForeignKey(r => r.StudentId)
            .IsRequired();

        builder.HasMany(s => s.Assessments)
            .WithOne()
            .HasForeignKey(a => a.StudentId);

        builder.HasOne<Teacher>()
            .WithMany()
            .HasForeignKey(s => s.TeacherId)
            .IsRequired();
    }
}