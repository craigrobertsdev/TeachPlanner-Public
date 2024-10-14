using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using TeachPlanner.Shared.Contracts;
using TeachPlanner.Shared.Interfaces;

namespace TeachPlanner.Shared.StronglyTypedIds;

[JsonConverter(typeof(StronglyTypedIdJsonConverter<LessonPlanId>))]
public record LessonPlanId : IStronglyTypedId
{
    public LessonPlanId(Guid value)
    {
        Value = value;
    }

    public Guid Value { get; set; }

    public class StronglyTypedIdEfValueConverter : ValueConverter<LessonPlanId, Guid>
    {
        public StronglyTypedIdEfValueConverter(ConverterMappingHints? mappingHints = null)
            : base(id => id.Value, value => new LessonPlanId(value), mappingHints)
        {
        }
    }
}