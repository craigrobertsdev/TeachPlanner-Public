using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using TeachPlanner.Shared.Contracts;
using TeachPlanner.Shared.Interfaces;

namespace TeachPlanner.Shared.StronglyTypedIds;

[JsonConverter(typeof(StronglyTypedIdJsonConverter<WeekPlannerId>))]
public record WeekPlannerId : IStronglyTypedId
{
    public WeekPlannerId(Guid value)
    {
        Value = value;
    }

    public Guid Value { get; set; }

    public class StronglyTypedIdEfValueConverter : ValueConverter<WeekPlannerId, Guid>
    {
        public StronglyTypedIdEfValueConverter(ConverterMappingHints? mappingHints = null)
            : base(id => id.Value, value => new WeekPlannerId(value), mappingHints)
        {
        }
    }
}