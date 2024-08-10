using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Text.Json.Serialization;
using TeachPlanner.Shared.Contracts;
using TeachPlanner.Shared.Domain.Common.Interfaces;

namespace TeachPlanner.Shared.Domain.PlannerTemplates;

[JsonConverter(typeof(StronglyTypedIdJsonConverter<DayTemplateId>))]
public record DayTemplateId : IStronglyTypedId
{
    public Guid Value { get; set; }

    public DayTemplateId(Guid value)
    {
        Value = value;
    }

    public class StronglyTypedIdEfValueConverter : ValueConverter<DayTemplateId, Guid>
    {
        public StronglyTypedIdEfValueConverter(ConverterMappingHints? mappingHints = null)
            : base(id => id.Value, value => new DayTemplateId(value), mappingHints)
        {
        }
    }
}
