using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GoSharpRest.Models.Entities;

namespace GoSharpRest.Models.DTO
{
    public class ProjectReturnModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Budget { get; set; }

        public string ProjectStatus { get; set; }

        public DateTime CreatedDate { get; set; }
        public DateTime DueDate { get; set; }

        public virtual UserReturnModel ProjectManager { get; set; }
        public virtual List<UserReturnModel> Developers { get; set; }

        public virtual OrderReturnModel Order { get; set; }
        public virtual List<WorkItemReturnModel> WorkItems { get; set; }
    }

    public class WorkItemReturnModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public decimal EstimatedTime { get; set; }
        public DateTime DueDate { get; set; }

        public string Status { get; set; }

        public virtual UserReturnModel AssignedDeveloper { get; set; }
        public virtual int ProjectId { get; set; }
    }
}