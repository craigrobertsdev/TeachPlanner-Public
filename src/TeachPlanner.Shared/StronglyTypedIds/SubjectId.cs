using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using TeachPlanner.Shared.Contracts;
using TeachPlanner.Shared.Interfaces;

namespace TeachPlanner.Shared.StronglyTypedIds;

[JsonConverter(typeof(StronglyTypedIdJsonConverter<SubjectId>))]
public record SubjectId : IStronglyTypedId
{
    public SubjectId(Guid value)
    {
        Value = value;
    }

    public Guid Value { get; set; }
    public override string ToString() => Value.ToString();

    public class StronglyTypedIdEfValueConverter : ValueConverter<SubjectId, Guid>
    {
        public StronglyTypedIdEfValueConverter(ConverterMappingHints? mappingHints = null)
            : base(id => id.Value, value => new SubjectId(value), mappingHints)
        {
        }
    }
}