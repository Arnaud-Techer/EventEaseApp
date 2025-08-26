namespace EventEaseApp.Services;

using EventEaseApp.Models;
using System.Text;
using System.Text.Json;
using System.Linq;

public class AttendanceService
{
    private readonly LocalStorageService _localStorage;
    private readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false
    };
    public event Action? OnChange;

    public AttendanceService(LocalStorageService localStorage)
    {
        _localStorage = localStorage;
    }
    private string KeyFor(int eventId) => $"attendance:{eventId}";

    private void NotifyStateChange() => OnChange?.Invoke();

    private async Task<List<AttendanceEntry>> LoadEntriesAsync(int eventId)
    {
        var raw = await _localStorage.GetItemAsync(KeyFor(eventId));
        if (string.IsNullOrWhiteSpace(raw)) return new List<AttendanceEntry>();
        try
        {
            var list = JsonSerializer.Deserialize<List<AttendanceEntry>>(raw, _jsonOptions);
            return list ?? new List<AttendanceEntry>();
        }
        catch
        {
            return new List<AttendanceEntry>();
        }
    }

    private async Task SaveEntriesAsync(int eventId, List<AttendanceEntry> entries)
    {
        var json = JsonSerializer.Serialize(entries, _jsonOptions);
        await _localStorage.SetItemAsync(KeyFor(eventId), json);
        NotifyStateChange();
    }

    public async Task<List<AttendanceEntry>> GetRosterAsync(int eventId)
    {
        return await LoadEntriesAsync(eventId);
    }

    public async Task<AttendanceEntry> AddAttendeeAsync(int eventId, string name, string? email = null)
    {
        var entries = await LoadEntriesAsync(eventId);
        if (!string.IsNullOrWhiteSpace(email))
        {
            var existing = entries.FirstOrDefault(e => !string.IsNullOrWhiteSpace(e.Attendee.Email) &&
                                                    string.Equals(e.Attendee.Email, email, StringComparison.OrdinalIgnoreCase));
            if (existing != null) return existing;
        }
        var existingByName = entries.FirstOrDefault(e => string.Equals(e.Attendee.Name?.Trim(), name?.Trim(), StringComparison.Ordinal));
        if (existingByName != null && string.IsNullOrWhiteSpace(email))
        {
            return existingByName;
        }

        var attendee = new Attendee { Name = name?.Trim() ?? string.Empty, Email = string.IsNullOrWhiteSpace(email) ? null : email?.Trim() };
        var record = new AttendanceRecord
        {
            EventId = eventId,
            AttendeeId = attendee.Id,
            Status = AttendanceStatus.Unknown
        };
        var entry = new AttendanceEntry { Attendee = attendee, Record = record };
        entries.Add(entry);
        await SaveEntriesAsync(eventId, entries);
        return entry;
    }

    public async Task<bool> RemoveAttendeeAsync(int eventId, Guid attendeeId)
    {
        var entries = await LoadEntriesAsync(eventId);
        var removed = entries.RemoveAll(e => e.Attendee.Id == attendeeId) > 0;
        if (removed) await SaveEntriesAsync(eventId, entries);
        return removed;
    }

    public async Task<bool> CheckInAsync(int eventId, Guid attendeeId, bool markLate = false)
    {
        var entries = await LoadEntriesAsync(eventId);
        var entry = entries.FirstOrDefault(e => e.Attendee.Id == attendeeId);
        if (entry is null)
        {
            return false;
        }
        entry.Record.CheckInAt = DateTime.Now;
        entry.Record.Status = markLate ? AttendanceStatus.Late : AttendanceStatus.Present;
        entry.Record.CheckOutAt = null;
        await SaveEntriesAsync(eventId, entries);
        return true;
    }

    public async Task<bool> CheckOutAsync(int eventId, Guid attendeeId)
    {
        var entries = await LoadEntriesAsync(eventId);
        var entry = entries.FirstOrDefault(e => e.Attendee.Id == attendeeId);
        if (entry is null) return false;
        entry.Record.CheckOutAt = DateTime.Now;
        if (entry.Record.Status == AttendanceStatus.Unknown || entry.Record.Status == AttendanceStatus.Present || entry.Record.Status == AttendanceStatus.Late)
        {
            entry.Record.Status = AttendanceStatus.Left;
        }
        await SaveEntriesAsync(eventId, entries);
        return true;
    }

    public async Task<bool> SetStatusAsync(int eventId, Guid attendeeId, AttendanceStatus status, string? notes = null)
    {
        var entries = await LoadEntriesAsync(eventId);
        var entry = entries.FirstOrDefault(e => e.Attendee.Id == attendeeId);
        if (entry is null) return false;

        entry.Record.Status = status;
        if (!string.IsNullOrWhiteSpace(notes)) entry.Record.Notes = notes;
        await SaveEntriesAsync(eventId, entries);
        return true;
    }

    public async Task<AttendanceStats> GetStatsAsync(int eventId)
    {
        var entries = await LoadEntriesAsync(eventId);
        var total = entries.Count;
        var present = entries.Count(e => e.Record.Status == AttendanceStatus.Present);
        var late = entries.Count(e => e.Record.Status == AttendanceStatus.Late);
        var excused = entries.Count(e => e.Record.Status == AttendanceStatus.Excused);
        var left = entries.Count(e => e.Record.Status == AttendanceStatus.Left);
        var unknown = entries.Count(e => e.Record.Status == AttendanceStatus.Unknown);
        var absent = total - (present + late + excused + left);
        double percentPresent = total == 0 ? 0 : Math.Round((double)(present + late) / total * 100.0, 1);

        return new AttendanceStats
        {
            Total = total,
            Present = present,
            Late = late,
            Excused = excused,
            Left = left,
            Unknown = unknown,
            Absent = Math.Max(0, absent),
            PercentPresent = percentPresent
        };
    }

    public async Task<string> ExportJsonAsync(int eventId)
    {
        var entries = await LoadEntriesAsync(eventId);
        return JsonSerializer.Serialize(entries, _jsonOptions);
    }
    public async Task<string> ExportCsvAsync(int eventId)
    {
        var entries = await LoadEntriesAsync(eventId);
        var sb = new StringBuilder();
        sb.AppendLine("Name,Email,Status,CheckInAt,CheckOutAt,Notes");
        foreach (var e in entries)
        {
            var name = EscapeCsv(e.Attendee.Name);
            var email = EscapeCsv(e.Attendee.Email);
            var status = e.Record.Status.ToString();
            var checkIn = e.Record.CheckInAt?.ToString("o") ?? "";
            var checkOut = e.Record.CheckOutAt?.ToString("o") ?? "";
            var notes = EscapeCsv(e.Record.Notes);
            sb.AppendLine($"{name},{email},{status},{checkIn},{checkOut},{notes}");
        }
        return sb.ToString();
    }

    private static string EscapeCsv(string? s)
    {
        if (string.IsNullOrEmpty(s)) return "";
        var needsQuotes = s.Contains(",") || s.Contains("\"") || s.Contains("\n") || s.Contains("\r");
        var v = s.Replace("\"", "\"\"");
        return needsQuotes ? $"\"{v}\"" : v;
    }

    public async Task ImportJsonAsync(int eventId, string json, bool merge = true)
    {
        List<AttendanceEntry>? imported;
        try
        {
            imported = JsonSerializer.Deserialize<List<AttendanceEntry>>(json, _jsonOptions);
            if (imported == null) return;
        }
        catch
        {
            return;
        }
        var entries = await LoadEntriesAsync(eventId);

        if (!merge)
        {
            await SaveEntriesAsync(eventId, imported);
            return;
        }
        foreach (var item in imported)
        {
            bool merged = false;
            if (!string.IsNullOrWhiteSpace(item.Attendee.Email))
            {
                var existing = entries.FirstOrDefault(e => !string.IsNullOrWhiteSpace(e.Attendee.Email) &&
                                                            string.Equals(e.Attendee.Email, item.Attendee.Email, StringComparison.OrdinalIgnoreCase));

                if (existing != null)
                {
                    existing.Record = item.Record;
                    existing.Attendee = item.Attendee;
                    merged = true;
                }
            }
            if (!merged)
            {
                var existingByName = entries.FirstOrDefault(e => string.Equals(e.Attendee.Name?.Trim(), item.Attendee.Name?.Trim(), StringComparison.Ordinal));
                if (existingByName != null)
                {
                    existingByName.Record = item.Record;
                    existingByName.Attendee = item.Attendee;
                    merged = true;
                }
            }
            if (!merged)
            {
                entries.Add(item);
            }
        }
        await SaveEntriesAsync(eventId, entries);
    }
}
