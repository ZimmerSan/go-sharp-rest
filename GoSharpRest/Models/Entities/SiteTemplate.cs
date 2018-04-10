using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using System.Web;
using GoSharpRest.Models.Constants;

namespace GoSharpRest.Models.Entities
{
    public class SiteTemplate
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "An Item Name is required")]
        [StringLength(160)]
        public string Title { get; set; }

        public string ShortDescription { get; set; }

        public string Description { get; set; }

        [Required(ErrorMessage = "Price is required")]
        [Range(0.01, 999999.99, ErrorMessage = "Price must be between 0.01 and 999999.99")]
        public decimal Price { get; set; }

        public string Category { get; set; }

        public string ImageUrl { get; set; }

        public string EntityStatus { get; set; } 

    }

}