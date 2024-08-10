using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Text.Json.Serialization;
using TeachPlanner.Shared.Contracts;
using TeachPlanner.Shared.Domain.Common.Interfaces;

namespace TeachPlanner.Shared.Domain.PlannerTemplates;

[JsonConverter(typeof(StronglyTypedIdJsonConverter<WeekStructureId>))]
public record WeekStructureId : IStronglyTypedId
{
    public Guid Value { get; set; }

    public WeekStructureId(Guid value)
    {
        Value = value;
    }

    public class StronglyTypedIdEfValueConverter : ValueConverter<WeekStructureId, Guid>
    {
        public StronglyTypedIdEfValueConverter(ConverterMappingHints? mappingHints = null)
            : base(id => id.Value, value => new WeekStructureId(value), mappingHints)
        {
        }
    }
}
