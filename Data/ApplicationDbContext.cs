using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MindScribe.Models;
using MindScribe.Repositories;

namespace MindScribe.Data
{
    public class ApplicationDbContext: IdentityDbContext<User> 
    { 
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
            Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.ApplyConfiguration<Article>(new ArticleConfiguration());
            builder.ApplyConfiguration<Comment>(new CommentConfiguration());
        }
    }
}
