using MindScribe.Models;
using MindScribe.ViewModels.EditViewModel;

namespace MindScribe.ViewModels.FromModel
{
    public static class CommentFromModel
    {
        public static Comment Convert(this Comment comment, CommentEditViewModel commenteditvm)
        {
            comment.Article_id= commenteditvm.Article_id;
            comment.Content_comment = commenteditvm.Content_comment;
            comment.Created_at = commenteditvm.Created_at;
            comment.Updated_at = commenteditvm.Updated_at;
            comment.User_id = commenteditvm.User_id;
            comment.Id = commenteditvm.Id;

            return comment;
        }
    }
}
