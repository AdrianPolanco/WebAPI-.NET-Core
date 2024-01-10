using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi.DTOS.Comments
{
    public class CreateCommentDto
    {
        //Doing data validation through data annotations in C#
        [Required]
        [MinLength(5, ErrorMessage = "Title must have at least 5 characters")]
        [MaxLength(70, ErrorMessage = "Title cannot be over 70 characters")]
        public string Title { get; set; } = String.Empty;

        [Required]
        [MinLength(5, ErrorMessage = "Content must have at least 100 characters")]
        [MaxLength(70, ErrorMessage = "Content cannot be over 1000 characters")]
        public string Content { get; set; } = String.Empty;
    }
}