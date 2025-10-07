## EventEaseApp

A small Blazor WebAssembly app (net9.0) to manage events and attendance. It lets you create, edit, and delete events, register attendees, and view simple attendance stats – all locally in-memory.

### Tech Stack
- **Blazor WebAssembly** (ASP.NET Core, .NET 9)
- **Bootstrap** for styling (bundled under `wwwroot/lib/bootstrap`)

### Features
- **Events management**: create, edit, delete events
- **Date & time input**: date via `InputDate` and time via `InputText type="time"`; combined into a single `DateTime`
- **Validation**:
  - Data annotations on `AppEvent` model (`Required`, `StringLength`)
  - Custom `FutureDate` attribute ensures the event date is today or in the future
- **Attendance**: view simple stats and navigate to attendance management
- **Session-aware UI**: basic `UserSessionService` toggles actions depending on login status

### Project Structure
```
EventEaseApp/
  Pages/
    Events/
      Events.razor              # List + add new event card + add form
      EventCard.razor           # Single event display + edit form
      EventRegistrationPage.razor
      Attendance.razor
      AttendanceBadge.razor
    Home.razor
  Models/
    AppEvent.cs                 # Event model + FutureDate validation attribute
    AttendanceModel.cs
    EventRegistrationModel.cs
    LoginModel.cs
    RegistrationModel.cs
  Services/
    EventManagerService.cs      # In-memory events store and CRUD
    AttendanceService.cs        # In-memory attendance data and stats
    LocalStorageService.cs
    UserSessionService.cs
  Layout/
    MainLayout.razor, NavMenu.razor
  wwwroot/
    index.html, css/, lib/bootstrap/
  Program.cs
  EventEaseApp.csproj
```

### Prerequisites
- .NET SDK 9.0 or later (`dotnet --version`)

### Getting Started
From the project root (`EventEaseApp/`):

```powershell
# Restore dependencies
dotnet restore

# Build
dotnet build -c Debug

# Run (watch for convenient hot reload during development)
dotnet watch run
```

The terminal will print the application URL (typically `http://localhost:****`). Open it in your browser.

### Usage Notes
- **Adding an event**: On the Events page, click the “Add Event” card.
  - Enter Name, Description, Date, Time, and Location.
  - Date is bound to a `DateTime` (date part) via `InputDate`.
  - Time is handled with `InputText type="time"` bound to a string (`HH:mm`), then parsed and combined with the selected date.
- **Editing an event**: Click the three-dot menu on an event and choose Edit.
  - The edit form mirrors the add form and uses the same date/time combination logic.

### Known Limitations
- Data is stored in memory via services like `EventManagerService` and `AttendanceService`. Restarting the app resets the state.
- Authentication is minimal (session service only) and included for UI gating.

### License
This project is for learning and personal use. No explicit license provided.


