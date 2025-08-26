using EventEaseApp.Models;
namespace EventEaseApp.Services;

public class EventManagerService
{
    private List<AppEvent> events = new List<AppEvent>();
    private int nextId = 1;

    public IEnumerable<AppEvent> GetEvents()
    {
        return events;
    }
    public void AddEvent(string eventName, string eventDescription, DateTime eventDate, string eventLocation)
    {
        AppEvent newEvent = new AppEvent
        {
            Id = nextId++,
            EventName = eventName,
            EventDescription = eventDescription,
            EventDate = eventDate,
            EventLocation = eventLocation
        };
        events.Add(newEvent);

    }

    public void DeleteEvent(int eventId)
    {
        AppEvent? eventToRemove = events.FirstOrDefault(e => e.Id == eventId);
        if (eventToRemove != null)
        {
            events.Remove(eventToRemove);
        }
    }

    public void UpdateEvent(AppEvent updatedEvent)
    {
        var existing = events.FirstOrDefault(e => e.Id == updatedEvent.Id);
        if (existing != null)
        {
            existing.EventName = updatedEvent.EventName;
            existing.EventDescription = updatedEvent.EventDescription;
            existing.EventDate = updatedEvent.EventDate;
            existing.EventLocation = updatedEvent.EventLocation;
        }
    }

}