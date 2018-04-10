using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GoSharpRest.Models.Entities;

namespace GoSharpRest.Models.DTO
{
    public class FullCartReturnModel
    {
        public string UserId { get; set; }
        public List<CartRecordReturnModel> CartRecords { get; set; }

        public int ItemsCount
        {
            get
            {
                var res = 0;
                CartRecords.ForEach(e => res += e.Count);
                return res;
            }
        }

        public decimal TotalPrice
        {
            get
            {
                var res = decimal.Zero;
                CartRecords.ForEach(e => res += e.Count * e.SiteTemplate.Price);
                return res;
            }
        }
    }

    public class CartRecordReturnModel
    {
        public int Id { get; set; }
        public int Count { get; set; }
        public SiteTemplateBindingModel SiteTemplate { get; set; }
    }

    public class CartAddItemBindingModel
    {
        public int ItemId { get; set; }
        public int Count { get; set; } = 1;
    }
}