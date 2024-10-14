using TeachPlanner.Shared.StronglyTypedIds;

namespace TeachPlanner.Shared.ValueObjects;

public record SchoolEvent
{
    private SchoolEvent(
        SchoolEventId id,
        Location location,
        string name,
        bool fullDay,
        DateTime eventStart,
        DateTime eventEnd)
    {
        Id = id;
        Location = location;
        Name = name;
        FullDay = fullDay;
        EventStart = eventStart;
        EventEnd = eventEnd;
        CreatedDateTime = DateTime.UtcNow;
        UpdatedDateTime = DateTime.UtcNow;
    }

    public SchoolEventId Id { get; private set; }
    public Location Location { get; private set; }
    public string Name { get; private set; }
    public bool FullDay { get; private set; }
    public DateTime EventStart { get; private set; }
    public DateTime EventEnd { get; private set; }
    public DateTime CreatedDateTime { get; private set; }
    public DateTime UpdatedDateTime { get; private set; }

    public static SchoolEvent Create(
        SchoolEventId id,
        Location location,
        string name,
        bool fullDay,
        DateTime eventStart,
        DateTime eventEnd)
    {
        return new SchoolEvent(
            id,
            location,
            name,
            fullDay,
            eventStart,
            eventEnd);
    }
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    private SchoolEvent()
    {
    }
}