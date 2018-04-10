using System.ComponentModel.DataAnnotations;

namespace GoSharpRest.Models.Entities
{
    public class OrderDetail
    {
        [Key]
        public int Id { get; set; }
        public int Quantity { get; set; }
        public decimal ItemPrice { get; set; }

        public virtual SiteTemplate Item { get; set; }
        public virtual Order Order { get; set; }
    }
}