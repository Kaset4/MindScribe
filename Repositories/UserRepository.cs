using Microsoft.EntityFrameworkCore;
using MindScribe.Data;
using MindScribe.Models;

namespace MindScribe.Repositories
{
    public class UserRepository: Repository<User>
    {
        public UserRepository(ApplicationDbContext db) : base(db)
        {

        }

        public User GetUserById(User target)
        {
            var user = Set.FirstOrDefault(u => u.Id == target.Id);

            return user;
        }

        public List<User> GetAllUsers(User target)
        {
            var users = Set.ToList();
            return users;
        }

        public void DeleteUser(User target)
        {
            var user = Set
                .AsEnumerable()
                .FirstOrDefault(user => user.Id == target.Id);

            if (user != null)
            {
                Delete(user);
            }
        }
    }
}
