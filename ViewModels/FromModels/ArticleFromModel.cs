using MindScribe.Models;
using MindScribe.ViewModels.EditViewModel;

namespace MindScribe.ViewModels.FromModels
{
    public static class ArticleFromModel
    {
        public static Article Convert(this Article article, ArticleEditViewModel articleeditvm)
        {
            article.Title = articleeditvm.Title;
            article.Content_article = articleeditvm.Content_article;
            article.Tags = articleeditvm.Tags;
            article.Comments = articleeditvm.Comments;
            article.Created_at = articleeditvm.Created_at;
            article.Updated_at = articleeditvm.Updated_at;

            return article;
        }
    }
}
