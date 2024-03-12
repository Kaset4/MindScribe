using MindScribe.Models;

namespace MindScribe.ViewModels
{
    public class UserViewModel
    {
        public User User { get; set; }

        public UserViewModel(User user)
        {
            User = user;
        }

        public List<Article>? Articles { get; set; }
    }
}
