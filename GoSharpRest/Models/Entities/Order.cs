using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GoSharpRest.Models.Entities
{
    public class Order
    {
        [Key]
        public int Id { get; set; }

        public DateTime OrderDate { get; set; }
        public DateTime DueDate { get; set; }

        public string OrderStatus { get; set; }
        public string Description { get; set; }

        public virtual List<OrderDetail> OrderDetails { get; set; }
        public virtual ApplicationUser Customer { get; set; }
        public virtual Project Project { get; set; }

    }
}