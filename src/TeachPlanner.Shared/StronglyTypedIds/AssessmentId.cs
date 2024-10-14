using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using TeachPlanner.Shared.Contracts;
using TeachPlanner.Shared.Interfaces;

namespace TeachPlanner.Shared.StronglyTypedIds;

[JsonConverter(typeof(StronglyTypedIdJsonConverter<AssessmentId>))]
public record AssessmentId : IStronglyTypedId
{
    public AssessmentId(Guid value)
    {
        Value = value;
    }

    public Guid Value { get; set; }

    public class StronglyTypedIdEfValueConverter : ValueConverter<AssessmentId, Guid>
    {
        public StronglyTypedIdEfValueConverter(ConverterMappingHints? mappingHints = null)
            : base(id => id.Value, value => new AssessmentId(value), mappingHints)
        {
        }

        public StronglyTypedIdEfValueConverter()
            : base(id => id.Value, value => new AssessmentId(value))
        {
        }
    }
}