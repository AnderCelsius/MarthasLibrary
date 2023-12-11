using MarthasLibrary.API.Features.Books;
using MarthasLibrary.API.Features.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MarthasLibrary.API.Controllers
{
  /// <summary>
  /// Controller for managing book-related operations.
  /// </summary>
  /// <remarks>
  /// This controller handles endpoints for book management, including listing, searching, creating, updating, and deleting books.
  /// </remarks>
  [Route("api/books")]
  [ApiController]
  [Authorize]
  public class BookManagementController(IMediator mediator) : ControllerBase
  {

    /// <summary>
    /// Retrieves a paginated list of all books.
    /// </summary>
    /// <remarks>
    /// This endpoint allows anyone (including unauthenticated users) to retrieve a list of all books with pagination support. 
    /// It returns a paginated response containing book details.
    /// </remarks>
    /// <param name="pageNumber">The page number of the result set, with a default value of 1.</param>
    /// <param name="pageSize">The size of each result page, with a default value of 20.</param>
    /// <param name="cancellationToken">A token for cancelling the operation if necessary, with a default value.</param>
    /// <returns>A list of all books in a paginated format.</returns>
    /// <response code="200">Returns the paginated list of books.</response>
    /// <response code="403">Returned when the user is not authorized to access this endpoint.</response>
    [AllowAnonymous]
    [HttpGet(Name = "GetAllBooks")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<GetAll.Response>> GetAllBooks(
      [FromQuery] int pageNumber = 1,
      [FromQuery] int pageSize = 20,
      CancellationToken cancellationToken = default)
    {
      return await mediator.Send(new GetAll.Request(pageNumber, pageSize), cancellationToken);
    }

    /// <summary>
    /// Searches for books based on a query string.
    /// </summary>
    /// <remarks>
    /// This endpoint allows users to search for books by providing a query string that can match various attributes of the books, such as title or author.
    /// </remarks>
    /// <param name="query">The search query string used to find matching books.</param>
    /// <param name="pageNumber">The page number of the result set, with a default value of 1.</param>
    /// <param name="pageSize">The size of each result page, with a default value of 20.</param>
    /// <param name="cancellationToken">A token for cancelling the operation if necessary.</param>
    /// <returns>A list of books that match the search query.</returns>
    /// <response code="200">Returns the list of books matching the search query.</response>
    [AllowAnonymous]
    [HttpGet("search", Name = "Search")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<Search.Response>> Search(
      [FromQuery] string query,
      [FromQuery] int pageNumber = 1,
      [FromQuery] int pageSize = 20,
      CancellationToken cancellationToken = default)
    {
      return await mediator.Send(new Search.Request(query, pageNumber, pageSize), cancellationToken);
    }

    /// <summary>
    /// Creates a new book in the library system.
    /// </summary>
    /// <remarks>
    /// This endpoint allows users to add a new book to the library's collection. It expects details of the book in the request body.
    /// </remarks>
    /// <param name="request">The details of the book to be created, including title, author, ISBN, and publication date.</param>
    /// <param name="cancellationToken">A token for cancelling the operation if necessary.</param>
    /// <returns>The details of the created book, including its unique identifier.</returns>
    /// <response code="201">Returns the details of the newly created book.</response>
    /// <response code="400">Returned if a book with the same ISBN already exists in the system.</response>
    /// <response code="403">Returned if the user is not authorized to create a book.</response>
    [HttpPost(Name = "CreateBook")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<Create.Response>> CreateBook(
      [FromBody] Create.Request request,
      CancellationToken cancellationToken)
    {
      try
      {
        var response = await mediator.Send(request, cancellationToken);
        return Created(new Uri($"/books/{response.Id}", UriKind.Relative), response);
      }
      catch (BookWithIsbnAlreadyExistsException e)
      {
        return BadRequest(e.Message);
      }
    }

    /// <summary>
    /// Retrieves details of a specific book by its ID.
    /// </summary>
    /// <remarks>
    /// This endpoint fetches detailed information about a book based on its unique identifier. 
    /// It is used to get individual book details such as title, author, ISBN, and publication date.
    /// </remarks>
    /// <param name="bookId">The unique identifier of the book.</param>
    /// <param name="cancellationToken">A token for cancelling the operation if necessary.</param>
    /// <returns>The details of the requested book, if found.</returns>
    /// <response code="200">Returns the detailed information of the requested book.</response>
    /// <response code="403">Returned if the user is not authorized to access this book's details.</response>
    /// <response code="404">Returned if the book with the specified ID is not found.</response>
    [AllowAnonymous]
    [HttpGet("{bookId}", Name = "GetBookById")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<GetById.Response>> GetBookById(
      [FromRoute] Guid bookId,
      CancellationToken cancellationToken)
    {
      try
      {
        return await mediator.Send(new GetById.Request(bookId), cancellationToken);
      }
      catch (BookNotFoundException e)
      {
        return NotFound(e.Message);
      }
    }

    /// <summary>
    /// Updates the details of a specific book by its ID.
    /// </summary>
    /// <remarks>
    /// This endpoint allows authorized users to update the details of a book, such as its title, author, ISBN, and publication date,
    /// by providing the unique identifier of the book and the updated information.
    /// </remarks>
    /// <param name="bookId">The unique identifier of the book to be updated.</param>
    /// <param name="updatedDetails">The updated details of the book, including title, author, ISBN, and publication date.</param>
    /// <param name="cancellationToken">A token for cancelling the operation if necessary.</param>
    /// <returns>No content if the update is successful.</returns>
    /// <response code="204">Returned if the book is updated successfully.</response>
    /// <response code="400">Returned if the request contains invalid data or the ISBN is already in use by another book.</response>
    /// <response code="403">Returned if the user is not authorized to update the book.</response>
    /// <response code="404">Returned if the book with the specified ID is not found.</response>
    [HttpPut("{bookId}", Name = "UpdateBookById")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> UpdateBookById(
      [FromRoute] Guid bookId,
      [FromBody] UpdateById.Request.UpdatedDetails updatedDetails,
      CancellationToken cancellationToken)
    {
      try
      {
        await mediator.Send(new UpdateById.Request(bookId, updatedDetails), cancellationToken);
        return NoContent();
      }
      catch (BookWithIsbnAlreadyExistsException e)
      {
        return BadRequest(e.Message);
      }
      catch (BookNotFoundException e)
      {
        return NotFound(e.Message);
      }
    }

    /// <summary>
    /// Deletes a book by its ID.
    /// </summary>
    /// <remarks>
    /// This endpoint allows authorized users to delete a book from the library by providing the unique identifier of the book.
    /// </remarks>
    /// <param name="bookId">The unique identifier of the book to be deleted.</param>
    /// <param name="cancellationToken">A token for cancelling the operation if necessary.</param>
    /// <returns>No content if the deletion is successful.</returns>
    /// <response code="204">Returned if the book is deleted successfully.</response>
    /// <response code="404">Returned if the book with the specified ID is not found.</response>
    [HttpDelete("{bookId}", Name = "DeleteBook")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Unit>> DeleteBook(
      [FromRoute] Guid bookId,
      CancellationToken cancellationToken)
    {
      try
      {
        await mediator.Send(new DeleteById.Request(bookId), cancellationToken);
        return NoContent();
      }
      catch (BookNotFoundException ex)
      {
        return NotFound(ex.Message);
      }
    }
  }
}