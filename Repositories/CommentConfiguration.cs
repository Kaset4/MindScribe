using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using MindScribe.Models;

namespace MindScribe.Repositories
{
    public class CommentConfiguration : IEntityTypeConfiguration<Comment>
    {
        public void Configure(EntityTypeBuilder<Comment> builder)
        {
            builder.ToTable("Comment").HasKey(p => p.Id);
            builder.Property(x => x.Id).UseIdentityColumn();
            builder.Property(x => x.Article_id).HasColumnName("Article_id");
        }
    }
}
