using MindScribe.Data;
using MindScribe.Models;

namespace MindScribe.Repositories
{
    public class ArticleRepository: Repository<Article>
    {
        public ArticleRepository(ApplicationDbContext db) : base(db)
        {

        }
        public Article GetArticleById(int articleId)
        {
            return Get(articleId);
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

        public List<Article> GetArticlesByAuthorId(string authorId)
        {
            return Set.Where(article => article.User_id == authorId).ToList();
        }
    }
}
