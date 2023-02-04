using CQRS.Core.Queries;

namespace Post.Query.Api.Queries;

public class FindPostByAuthorQuery : BaseQuery
{
    public FindPostByAuthorQuery(string author)
    {
        Author = author;
    }
 
    public string Author { get; }
}
