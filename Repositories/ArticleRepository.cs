using MindScribe.Data;
using MindScribe.Models;

namespace MindScribe.Repositories
{
    public class ArticleRepository: Repository<Article>
    {
        private readonly ApplicationDbContext _context;
        public ArticleRepository(ApplicationDbContext db) : base(db)
        {
            _context = db;
        }
        public Article GetArticleById(int articleId)
        {
            var article = Get(articleId);

            // Явная загрузка связанных комментариев
            _context.Entry(article)
                    .Collection(a => a.Comments)
                    .Load();
            _context.Entry(article)
                    .Collection(a => a.Tags)
                    .Load();

            return article;
        }

        public List<Article> GetAllArticles()
        {
            return GetAll().ToList();
        }

        public void CreateArticle(Article article)
        {
            Create(article);
        }

        public void UpdateArticle(Article updatedArticle)
        {
            Update(updatedArticle);
        }

        public void DeleteArticle(int articleId)
        {
            Delete(articleId);
        }

        public List<Article> GetArticlesByAuthorId(User user)
        {
            return Set.Where(article => article.User_id == user.Id).ToList();
        }
    }
}
