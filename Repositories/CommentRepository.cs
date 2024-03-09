using MindScribe.Data;
using MindScribe.Models;

namespace MindScribe.Repositories
{
    public class CommentRepository: Repository<Comment>
    {
        public CommentRepository(ApplicationDbContext db) : base(db)
        {

        }
        public Comment GetCommentById(int commentId)
        {
            return Get(commentId);
        }

        public List<Comment> GetAllComments()
        {
            return GetAll().ToList();
        }

        public void CreateComment(Comment comment)
        {
            Create(comment);
        }

        public void UpdateComment(Comment updatedComment)
        {
            Update(updatedComment);
        }

        public void DeleteComment(int commentId)
        {
            Delete(commentId);
        }
    }
}
