using System;
using System.Threading.Tasks;

namespace GoSharpRest.Models.Entities
{
    public class WorkItem
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public decimal EstimatedTime { get; set; }
        public DateTime DueDate { get; set; }

        public string Status { get; set; }

        public virtual ApplicationUser AssignedDeveloper { get; set; }
        public virtual Project Project { get; set; }
    }
}