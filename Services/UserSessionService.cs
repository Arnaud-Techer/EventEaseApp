namespace EventEaseApp.Services;

public class UserSessionService
{
    private readonly LocalStorageService _localStorage;
    public bool IsLoggedIn { get; private set; }
    public string? UserName { get; private set; }
    public string? PhotoUrl { get; private set; }
    public event Action? OnChange;

    public UserSessionService(LocalStorageService localStorage)
    {
        _localStorage = localStorage;
    }

    private void NotifyStateChanged()
    {
        OnChange?.Invoke();
    }
    public async Task InitializeAsync()
    {
        var storedUser = await _localStorage.GetItemAsync("username");
        if (!string.IsNullOrEmpty(storedUser))
        {
            IsLoggedIn = true;
            UserName = storedUser;
            NotifyStateChanged();
        }
    }

    public async Task LoginAsync(string name, string? photoUrl = null)
    {
        IsLoggedIn = true;
        UserName = name;
        PhotoUrl = photoUrl;
        await _localStorage.SetItemAsync("username", name);
        NotifyStateChanged();
    }

    public async Task LogoutAsync()
    {
        IsLoggedIn = false;
        Console.WriteLine("logout !");
        UserName = null;
        PhotoUrl = null;
        await _localStorage.RemoveItemAsync("username");
        NotifyStateChanged();
    }
}