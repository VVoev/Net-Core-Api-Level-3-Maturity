using Library.API.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Library.API.Controllers
{
    public class AuthorsController : Controller
    {
        private ILibraryRepository repository;

        public AuthorsController(ILibraryRepository repository)
        {
            this.repository = repository;
        }

        [HttpGet("api/authors")]
        public IActionResult GetResults()
        {
            var authors = repository.GetAuthors();
            return new JsonResult(authors);
        }
    }
}
