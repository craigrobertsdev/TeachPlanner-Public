using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using TeachPlanner.Shared.Contracts;
using TeachPlanner.Shared.Interfaces;

namespace TeachPlanner.Shared.StronglyTypedIds;

[JsonConverter(typeof(StronglyTypedIdJsonConverter<WeekStructureId>))]
public record WeekStructureId : IStronglyTypedId
{
    public WeekStructureId(Guid value)
    {
        Value = value;
    }

    public Guid Value { get; set; }

    public class StronglyTypedIdEfValueConverter : ValueConverter<WeekStructureId, Guid>
    {
        public StronglyTypedIdEfValueConverter(ConverterMappingHints? mappingHints = null)
            : base(id => id.Value, value => new WeekStructureId(value), mappingHints)
        {
        }
    }
}