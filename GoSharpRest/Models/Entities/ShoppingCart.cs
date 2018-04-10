using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GoSharpRest.Models.Entities
{
    public class ShoppingCart
    {
        [Key]
        public int Id { get; set; }
        public List<CartRecord> Records { get; set; }

        public virtual ApplicationUser User { get; set; }
    }
}