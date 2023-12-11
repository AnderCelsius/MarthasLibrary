namespace MarthasLibrary.BlazorApp.Services;

public class BookService
{
    public event Action<string>? OnSearchTermChanged;

    public void NotifySearchTermChanged(string searchTerm)
    {
        OnSearchTermChanged?.Invoke(searchTerm);
    }
}