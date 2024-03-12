using Microsoft.AspNetCore.Identity;

namespace MindScribe.Models
{
    public class User: IdentityUser
    {
        public string FirstName { get; set; } = "";
        public string LastName { get; set; } = "";
        public string? Image { get; set; }
        public string? About { get; set; }  
        public string GetFullName()
        {
            return FirstName + " " + LastName;
        }
    }
}
