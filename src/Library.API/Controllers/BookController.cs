using AutoMapper;
using Library.API.Entities;
using Library.API.Models;
using Library.API.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace Library.API.Controllers
{
    [Route("api/authors/{authorId}/books")]
    public class BookController : Controller
    {
        private  readonly ILibraryRepository libraryRepository;

        public BookController(ILibraryRepository libraryRepository)
        {
            this.libraryRepository = libraryRepository;
        }

        [HttpGet()]
        public IActionResult GetBooksForAuthor(Guid authorId)
        {
            var author = libraryRepository.AuthorExists(authorId);
            if(author == false)
            {
                return NotFound();
            }

            var booksFromRepo = libraryRepository.GetBooksForAuthor(authorId);
            var booksFromAuthor = Mapper.Map<IEnumerable<BookDto>>(booksFromRepo);
            return Ok(booksFromAuthor);
        }

        [HttpGet("{bookId}",Name ="GetBookForAuthor")]
        public IActionResult GetBookForAuthor(Guid authorId,Guid bookId)
        {
            var authorIsExisting = libraryRepository.AuthorExists(authorId);
            var bookIsExisting = libraryRepository.BookExist(authorId, bookId);
            
            if(authorIsExisting == false || bookIsExisting == false)
            {
                return NotFound();
            }

            var bookFromRepo = libraryRepository.GetBookForAuthor(authorId, bookId);
            var book = Mapper.Map<BookDto>(bookFromRepo);
            return Ok(book);
        }

        [HttpPost]
        public IActionResult CreateBookForAuthor(Guid authorId, [FromBody]BookForCreationDto book)
        {
            if(book == null)
            {
                return BadRequest();
            }

            if (libraryRepository.AuthorExists(authorId) == false)
            {
                return NotFound();
            }

            var bookEntity = Mapper.Map<Book>(book);
            libraryRepository.AddBookForAuthor(authorId, bookEntity);

            if (libraryRepository.Save() == false)
            {
                throw new Exception($"Creating a book with {authorId} has been failed");
            }

            var bookToReturn = Mapper.Map<BookDto>(bookEntity);
            return CreatedAtRoute("GetBookForAuthor", new { authorId = authorId, bookId = bookEntity.Id },bookToReturn);
        }
    }
}
