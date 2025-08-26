namespace EventEaseApp.Models;
using System;

public enum AttendanceStatus
{
    Unknown = 0,
    Present = 1,
    Late = 2,
    Excused = 3,
    Left = 4
}

public class Attendee
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public string? Email { get; set; }
}

public class AttendanceRecord
{
    public int EventId { get; set; }
    public Guid AttendeeId { get; set; }
    public AttendanceStatus Status { get; set; } = AttendanceStatus.Unknown;
    public DateTime? CheckInAt { get; set; }
    public DateTime? CheckOutAt { get; set; }
    public string? Notes { get; set; }
}

public class AttendanceEntry
{
    public Attendee Attendee { get; set; } = new Attendee();
    public AttendanceRecord Record { get; set; } = new AttendanceRecord();
}

public class AttendanceStats
{
    public int Total { get; set; }
    public int Present { get; set; }
    public int Late { get; set; }
    public int Excused { get; set; }
    public int Left { get; set; }
    public int Unknown { get; set; }
    public int Absent { get; set; }
    public double PercentPresent { get; set; }
}