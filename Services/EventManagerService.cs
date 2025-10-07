using EventEaseApp.Models;
namespace EventEaseApp.Services;

public class EventManagerService
{
    private List<AppEvent> events = new List<AppEvent>();
    private int nextId = 1;
    
    public event Action? OnEventsChanged;

    public EventManagerService()
    {
        // Add some sample events for testing
        AddEvent("Tech Conference 2024", "Annual technology conference featuring the latest innovations", DateTime.Now.AddDays(30), "Convention Center");
        AddEvent("Team Building Workshop", "Interactive workshop to improve team collaboration", DateTime.Now.AddDays(15), "Office Meeting Room");
        AddEvent("Product Launch Event", "Launch of our new product line with live demonstrations", DateTime.Now.AddDays(45), "Grand Hotel Ballroom");
    }

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
        OnEventsChanged?.Invoke();
    }

    public void DeleteEvent(int eventId)
    {
        AppEvent? eventToRemove = events.FirstOrDefault(e => e.Id == eventId);
        if (eventToRemove != null)
        {
            events.Remove(eventToRemove);
            OnEventsChanged?.Invoke();
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
            OnEventsChanged?.Invoke();
        }
    }

}