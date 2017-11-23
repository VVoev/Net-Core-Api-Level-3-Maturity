using AutoMapper;
using Library.API.Helpers;
using Library.API.Models;
using Library.API.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Library.API.Controllers
{
    [Route("api/authors")]
    public class AuthorsController : Controller
    {
        private ILibraryRepository repository;

        public AuthorsController(ILibraryRepository repository)
        {
            this.repository = repository;
        }

        [HttpGet]
        public IActionResult GetResults()
        {
            var authorsFromRepo = repository.GetAuthors();
            var authors = Mapper.Map<IEnumerable<AuthorDto>>(authorsFromRepo);
            return new JsonResult(authors);
        }

        [HttpGet("{id}")]
        public IActionResult GetAuthorById(Guid id)
        {
            var authorWithCorrectId = repository.GetAuthor(id);
            var mappedAuthor = Mapper.Map<AuthorDto>(authorWithCorrectId);
            return new JsonResult(mappedAuthor);
        }

    }
}
