using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using TeachPlanner.Shared.Contracts;
using TeachPlanner.Shared.Interfaces;

namespace TeachPlanner.Shared.StronglyTypedIds;

[JsonConverter(typeof(StronglyTypedIdJsonConverter<TermPlannerId>))]
public record TermPlannerId : IStronglyTypedId
{
    public TermPlannerId(Guid value)
    {
        Value = value;
    }

    public Guid Value { get; set; }

    public class StronglyTypedIdEfValueConverter : ValueConverter<TermPlannerId, Guid>
    {
        public StronglyTypedIdEfValueConverter(ConverterMappingHints? mappingHints = null)
            : base(id => id.Value, value => new TermPlannerId(value), mappingHints)
        {
        }
    }
}