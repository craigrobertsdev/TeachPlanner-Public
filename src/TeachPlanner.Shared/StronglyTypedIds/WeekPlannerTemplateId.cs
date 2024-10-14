using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using TeachPlanner.Shared.Contracts;
using TeachPlanner.Shared.Interfaces;

namespace TeachPlanner.Shared.StronglyTypedIds;

[JsonConverter(typeof(StronglyTypedIdJsonConverter<WeekPlannerTemplateId>))]
public record WeekPlannerTemplateId : IStronglyTypedId
{
    public WeekPlannerTemplateId(Guid value)
    {
        Value = value;
    }

    public Guid Value { get; set; }

    public class StronglyTypedIdEfValueConverter : ValueConverter<WeekPlannerTemplateId, Guid>
    {
        public StronglyTypedIdEfValueConverter(ConverterMappingHints? mappingHints = null)
            : base(id => id.Value, value => new WeekPlannerTemplateId(value), mappingHints)
        {
        }
    }
}