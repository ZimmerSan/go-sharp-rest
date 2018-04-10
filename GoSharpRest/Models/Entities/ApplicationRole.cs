using Microsoft.AspNet.Identity.EntityFramework;

namespace GoSharpRest.Models.Entities
{
    public class ApplicationRole : IdentityRole
    {
        public ApplicationRole(string name) : base(name)
        {
        }

        public ApplicationRole() : base()
        {
        }
    }
}