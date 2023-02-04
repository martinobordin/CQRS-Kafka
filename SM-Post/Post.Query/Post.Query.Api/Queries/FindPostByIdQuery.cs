using CQRS.Core.Queries;

namespace Post.Query.Api.Queries;

public class FindPostByIdQuery : BaseQuery 
{
	public FindPostByIdQuery(Guid id)
	{
        Id = id;
    }

    public Guid Id { get; }
}
