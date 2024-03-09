using MindScribe.Data;
using MindScribe.Models;

namespace MindScribe.Repositories
{
    public class ArticleTagRepository : Repository<ArticleTag>
    {
        public ArticleTagRepository(ApplicationDbContext db) : base(db)
        {

        }

        public ArticleTag GetArticleTagById(int tagId)
        {
            return Get(tagId);
        }

        public List<ArticleTag> GetAllArticleTags()
        {
            return GetAll().ToList();
        }

        public void CreateArticleTag(ArticleTag tag)
        {
            Create(tag);
        }

        public void UpdateArticleTag(ArticleTag updatedTag)
        {
            Update(updatedTag);
        }

        public void DeleteArticleTag(int tagId)
        {
            Delete(tagId);
        }
    }
}
