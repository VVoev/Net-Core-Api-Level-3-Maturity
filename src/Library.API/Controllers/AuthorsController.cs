using AutoMapper;
using Library.API.Entities;
using Library.API.Enums;
using Library.API.Helpers;
using Library.API.Models;
using Library.API.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Library.API.Controllers
{
    [Route("api/authors")]
    public class AuthorsController : Controller
    {
        private ILibraryRepository libraryRepository;
        private IUrlHelper urlHelper;

        private string CreateAuthorsResourceUri(AuthorsResourceParameters authorsResourceParameters,ResourceUriType type)
        {
            switch (type)
            {
                case ResourceUriType.PreviousPage:
                    return urlHelper.Link("GetAuthors",
                        new
                        {
                            searchQuery= authorsResourceParameters.SearchQuery,
                            genre = authorsResourceParameters.Genre,
                            pageNumber = authorsResourceParameters.PageNumber - 1,
                            pageSize = authorsResourceParameters.PageSize
                        });
                case ResourceUriType.NextPage:
                    return urlHelper.Link("GetAuthors",
                       new
                       {
                           searchQuery = authorsResourceParameters.SearchQuery,
                           genre = authorsResourceParameters.Genre,
                           pageNumber = authorsResourceParameters.PageNumber + 1,
                           pageSize = authorsResourceParameters.PageSize
                       });
                default:
                    return urlHelper.Link("GetAuthors",
                       new
                       {
                           searchQuery = authorsResourceParameters.SearchQuery,
                           genre = authorsResourceParameters.Genre,
                           pageNumber = authorsResourceParameters.PageNumber,
                           pageSize = authorsResourceParameters.PageSize
                       });
            }
        }

        public AuthorsController(ILibraryRepository repository, IUrlHelper urlHelper)
        {
            this.libraryRepository = repository;
            this.urlHelper = urlHelper;
        }

        [HttpGet(Name ="GetAuthors")]
        public IActionResult GetAuthors(AuthorsResourceParameters authorsResourceParameters)
        {
            var authorsFromRepo = libraryRepository.GetAuthors(authorsResourceParameters);

            var previousPageLink = authorsFromRepo.HasPrevious ? CreateAuthorsResourceUri(authorsResourceParameters, ResourceUriType.PreviousPage) : null;
            var nextPageLink = authorsFromRepo.HasNext ? CreateAuthorsResourceUri(authorsResourceParameters, ResourceUriType.NextPage) : null;
            var paginationMetaData = new
            {
                totalcount = authorsFromRepo.TotalCount,
                pageSize = authorsFromRepo.PageSize,
                currentPage = authorsFromRepo.CurrentPage,
                totalPages = authorsFromRepo.TotalPages,
                previousPageLink = previousPageLink,
                nextPageLink = nextPageLink
            };

            Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(paginationMetaData));

            var authors = Mapper.Map<IEnumerable<AuthorDto>>(authorsFromRepo);
            return Ok(authors);
        }

        [HttpGet("{id}", Name = "GetAuthor")]
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
            if (author == null)
            {
                return BadRequest();
            }

            var authorEntity = Mapper.Map<Author>(author);
            libraryRepository.AddAuthor(authorEntity);
            if (libraryRepository.Save() == false)
            {
                throw new Exception("Creating an author failed");
            }

            var authorToReturn = Mapper.Map<AuthorDto>(authorEntity);
            return CreatedAtRoute("GetAuthor", new { id = authorToReturn.Id }, authorToReturn);
        }

        [HttpPost("{id}")]

        public IActionResult BlockAuthorCreation(Guid id)
        {
            if (libraryRepository.AuthorExists(id) == true)
            {
                return new StatusCodeResult(409);
            }
            return NotFound();
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteAuthor(Guid id)
        {
            if (id == null)
            {
                return BadRequest();
            }

            if (libraryRepository.AuthorExists(id) == false)
            {
                return NotFound();
            }

            var author = libraryRepository.GetAuthor(id);
            libraryRepository.DeleteAuthor(author);

            if (libraryRepository.Save() == false)
            {
                throw new Exception("Something bad happend");
            }

            return NoContent();
        }
    }
}