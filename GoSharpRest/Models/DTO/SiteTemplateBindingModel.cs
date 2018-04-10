namespace GoSharpRest.Models.DTO
{
    public class SiteTemplateBindingModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public decimal Price { get; set; }
        public string Category { get; set; }

        public string ShortDescription { get; set; }
        public string Description { get; set; }

        public string ImageUrl { get; set; }
    }
}