using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Library.API.Models
{
    public abstract class BookForManipulationDto
    {
        [Required(ErrorMessage = "You should fill out a title")]
        [MaxLength(100, ErrorMessage = "Title should be no more than 100 chars")]
        public virtual string Title { get; set; }

        [MaxLength(500,ErrorMessage = "Description Should be more than 500 chars")]
        public virtual string Description { get; set; }
    }
}
