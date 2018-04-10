using System;
using System.Collections.Generic;

namespace GoSharpRest.Models.Entities
{
    public class Project
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Budget { get; set; }

        public string ProjectStatus { get; set; }

        public DateTime CreatedDate { get; set; }
        public DateTime DueDate { get; set; }

        public virtual ApplicationUser ProjectManager { get; set; }
        public virtual List<ApplicationUser> Developers { get; set; }

        public virtual Order Order { get; set; }
        public virtual List<WorkItem> WorkItems { get; set; }
    }
}