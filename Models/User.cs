using Microsoft.AspNetCore.Identity;

namespace MindScribe.Models
{
    public class User: IdentityUser
    {
        public string FirstName { get; set; } = "";
        public string LastName { get; set; } = "";
        public string? Image { get; set; } = "https://via.placeholder.com/500x500/";
        public string? About { get; set; }  
        public string GetFullName()
        {
            return FirstName + " " + LastName;
        }
    }
}
