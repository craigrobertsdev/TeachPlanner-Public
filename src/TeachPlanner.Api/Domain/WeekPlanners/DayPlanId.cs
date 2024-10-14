using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using TeachPlanner.Shared.Contracts;
using TeachPlanner.Shared.Interfaces;

namespace TeachPlanner.Api.Domain.WeekPlanners;

[JsonConverter(typeof(StronglyTypedIdJsonConverter<DayPlanId>))]
public record DayPlanId : IStronglyTypedId
{
    public DayPlanId(Guid value)
    {
        Value = value;
    }

    public Guid Value { get; set; }

    public class StronglyTypedIdEfValueConverter : ValueConverter<DayPlanId, Guid>
    {
        public StronglyTypedIdEfValueConverter(ConverterMappingHints? mappingHints = null)
            : base(id => id.Value, value => new DayPlanId(value), mappingHints)
        {
        }
    }
}