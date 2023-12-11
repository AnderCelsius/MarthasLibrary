using MarthasLibrary.APIClient;

namespace MarthasLibrary.BlazorApp.Services;

public class BookService(IMarthasLibraryAPIClient marthasLibraryApiClient, ILogger<BookService> logger)
{
    private int _currentPage = 1;
    public int PageSize { get; private set; }
    private int _totalBooks;
    private List<BookDetails> _books = new();

    public string? ErrorMessage { get; set; }

    public event Action? OnBooksUpdated;

    // Method to update books from an external source, like the search result
    public void UpdateBooks(List<BookDetails> newBooks)
    {
        _books = newBooks;
        NotifyBooksUpdated();
    }

    public async Task LoadBooks(int page)
    {
        _currentPage = page;

        try
        {
            var response = await marthasLibraryApiClient.GetAllBooksAsync(_currentPage, PageSize);
            _books = response?.Books?.ToList() ?? new List<BookDetails>();
            _totalBooks = response?.Total ?? 0;

            NotifyBooksUpdated();
        }
        catch (Exception e)
        {
            logger.LogError(e.Message);
            ErrorMessage = "An error occurred while loading books.Please try again later.";
        }
    }

    public (List<BookDetails>? Books, int CurrentPage, int TotalBooks) GetCurrentPage()
    {
        return (_books, _currentPage, _totalBooks);
    }

    private void NotifyBooksUpdated() => OnBooksUpdated?.Invoke();
}