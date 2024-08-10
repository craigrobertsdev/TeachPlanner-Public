using TeachPlanner.Shared.Domain.Common.Enums;

namespace TeachPlanner.Shared.Domain.Curriculum;

public sealed class YearLevel
{
    public List<Capability> Capabilities { get; private set; } = [];
    public List<Disposition> Dispositions { get; private set; } = [];
    public List<ConceptualOrganiser> ConceptualOrganisers { get; private set; } = [];
    public string LearningStandard { get; } = string.Empty;
    public string Name => YearLevelValue.ToDisplayString();
    public YearLevelValue YearLevelValue { get; }
    public YearLevelValue[] GetYearLevels()
    {
        if ((int)YearLevelValue < 15)
        {
            return [YearLevelValue];
        }
        else
        {
            return YearLevelValue switch
            {
                YearLevelValue.Years1to2 => [YearLevelValue.Year1, YearLevelValue.Year2],
                YearLevelValue.Years3to4 => [YearLevelValue.Year3, YearLevelValue.Year4],
                YearLevelValue.Years5to6 => [YearLevelValue.Year5, YearLevelValue.Year6],
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }

    public List<ContentDescription> GetContentDescriptions()
    {
        var cds = new List<ContentDescription>();
        foreach (var co in ConceptualOrganisers)
        {
            cds.AddRange(co.ContentDescriptions);
        }

        return cds;
    }

    public void SetCapabilities(List<Capability> capabilities) => Capabilities = capabilities;
    public void SetDispositions(List<Disposition> dispositions) => Dispositions = dispositions;
    public void SetConceptualOrganisers(List<ConceptualOrganiser> conceptualOrganisers) => ConceptualOrganisers = conceptualOrganisers;

    public static YearLevel Create(YearLevelValue yearLevelValue, string learningStandard)
    {
        return new YearLevel(yearLevelValue, learningStandard);
    }

    public static YearLevel Create(
        YearLevelValue yearLevelValue,
        string learningStandard,
        List<Capability> capabilities,
        List<Disposition> dispositions,
        List<ConceptualOrganiser> conceptualOrganisers)
    {
        return new YearLevel(
            yearLevelValue,
            learningStandard,
            capabilities,
            dispositions,
            conceptualOrganisers);
    }

    private YearLevel(YearLevelValue yearLevelValue, string learningStandard)
    {
        YearLevelValue = yearLevelValue;
        LearningStandard = learningStandard;
    }

    private YearLevel(
        YearLevelValue yearLevelValue,
        string learningStandard,
        List<Capability> capabilities,
        List<Disposition> dispositions,
        List<ConceptualOrganiser> conceptualOrganisers)
    {
        YearLevelValue = yearLevelValue;
        LearningStandard = learningStandard;
        Capabilities = capabilities;
        Dispositions = dispositions;
        ConceptualOrganisers = conceptualOrganisers;
    }

    private YearLevel() { }
}
public static class YearLevelExtensions
{
    public static YearLevel GetFromYearLevelValue(this IEnumerable<YearLevel> yearLevels, YearLevelValue yearLevelValue) =>
        yearLevels.First(yl => (yl.YearLevelValue == yearLevelValue)
            || yl.GetYearLevels().Contains(yearLevelValue));

    public static List<YearLevel> FilterYearLevels(this IEnumerable<YearLevel> yearLevels, IEnumerable<YearLevelValue> yearLevelValues) =>
        yearLevels.Where(s =>
            yearLevelValues.Contains(s.YearLevelValue))
        .ToList();

    public static List<YearLevel> FilterYearLevels(this IEnumerable<YearLevel> yearLevels, YearLevelValue yearLevelValue)
    {
        return yearLevels.Where(yl =>
            yearLevelValue == (yl.YearLevelValue)
            || yearLevelValue == yl.GetYearLevels()[0]
            || yearLevelValue == yl.GetYearLevels()[1])
        .ToList();
    }

    public static void FilterContentDescriptions(this IEnumerable<YearLevel> yearLevels, IEnumerable<Guid> contentDescriptionIds)
    {
        foreach (var yl in yearLevels)
        {
            yl.FilterContentDescriptions(contentDescriptionIds);
        }
    }

    public static void FilterContentDescriptions(this YearLevel yl, IEnumerable<Guid> contentDescriptionIds) =>
            yl.SetConceptualOrganisers(yl.ConceptualOrganisers.FilterContentDescriptions(contentDescriptionIds));

}
