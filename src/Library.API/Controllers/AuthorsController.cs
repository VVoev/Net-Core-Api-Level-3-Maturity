using AutoMapper;
using Library.API.Entities;
using Library.API.Helpers;
using Library.API.Models;
using Library.API.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Library.API.Controllers
{
    [Route("api/authors")]
    public class AuthorsController : Controller
    {
        private ILibraryRepository libraryRepository;

        public AuthorsController(ILibraryRepository repository)
        {
            this.libraryRepository = repository;
        }

        [HttpGet]
        public IActionResult GetAuthors()
        {
                var authorsFromRepo = libraryRepository.GetAuthors();
                var authors = Mapper.Map<IEnumerable<AuthorDto>>(authorsFromRepo);
                return Ok(authors);
        }

        [HttpGet("{id}",Name = "GetAuthor")]
        public IActionResult GetAuthorById(Guid id)
        {
            if (!libraryRepository.AuthorExists(id))
            {
                return NotFound();
            }

            var authorWithCorrectId = libraryRepository.GetAuthor(id);
            var mappedAuthor = Mapper.Map<AuthorDto>(authorWithCorrectId);
            return Ok(mappedAuthor);
        }

        [HttpPost]
        public IActionResult CreateAuthor([FromBody] AuthorForCreationDto author)
        {
            if(author == null)
            {
                return BadRequest();
            }

            var authorEntity = Mapper.Map<Author>(author);
            libraryRepository.AddAuthor(authorEntity);
            if(libraryRepository.Save() == false)
            {
                throw new Exception("Creating an author failed");
            }

            var authorToReturn = Mapper.Map<AuthorDto>(authorEntity);
            return CreatedAtRoute("GetAuthor", new { id = authorToReturn.Id },authorToReturn);
        }

        [HttpPost("{id}")]

        public IActionResult BlockAuthorCreation(Guid id)
        {
            if(libraryRepository.AuthorExists(id) == true)
            {
                return new StatusCodeResult(409);
            }
            return NotFound();
        }
    }
}