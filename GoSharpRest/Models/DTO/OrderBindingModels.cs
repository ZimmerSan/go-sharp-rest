using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GoSharpRest.Models.Entities;

namespace GoSharpRest.Models.DTO
{
    public class OrderReturnModel
    {
        public int Id { get; set; }

        public DateTime OrderDate { get; set; }
        public DateTime DueDate { get; set; }

        public string OrderStatus { get; set; }
        public string Description { get; set; }

        public List<OrderDetailReturnModel> OrderDetails { get; set; }
        public UserReturnModel Customer { get; set; }

        public int ItemsCount
        {
            get
            {
                var res = 0;
                OrderDetails.ForEach(e => res += e.Quantity);
                return res;
            }
        }
        public decimal TotalPrice
        {
            get
            {
                var res = decimal.Zero;
                OrderDetails.ForEach(e => res += e.Quantity * e.ItemPrice);
                return res;
            }
        }
    }

    public class OrderDetailReturnModel
    {
        public int Id { get; set; }
        public int Quantity { get; set; }
        public decimal ItemPrice { get; set; }

        public SiteTemplateBindingModel Item { get; set; }
    }
}