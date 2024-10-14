using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using TeachPlanner.Shared.Contracts;
using TeachPlanner.Shared.Interfaces;

namespace TeachPlanner.Shared.StronglyTypedIds;

[JsonConverter(typeof(StronglyTypedIdJsonConverter<UserId>))]
public record UserId : IStronglyTypedId
{
    public UserId(Guid value)
    {
        Value = value;
    }

    public Guid Value { get; set; }

    public class StronglyTypedIdEfValueConverter : ValueConverter<UserId, Guid>
    {
        public StronglyTypedIdEfValueConverter(ConverterMappingHints? mappingHints = null)
            : base(id => id.Value, value => new UserId(value), mappingHints)
        {
        }
    }

    public class IdToStringConverter : ValueConverter<UserId, string>
    {
        public IdToStringConverter(ConverterMappingHints? mappingHints = null)
            : base(id => id.Value.ToString(), value => new UserId(Guid.Parse(value)), mappingHints)
        {
        }
    }
}