using System;
using System.ComponentModel.DataAnnotations;

namespace GoSharpRest.Models.Entities
{
    public class CartRecord
    {
        [Key]
        public int Id { get; set; }
        public int Count { get; set; }

        public virtual ShoppingCart ShoppingCart { get; set; }
        public virtual SiteTemplate SiteTemplate { get; set; }
    }
}