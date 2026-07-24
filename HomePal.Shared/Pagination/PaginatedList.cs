namespace HomePal.Shared.Pagination;

public class PaginatedList<T>
{
    public IReadOnlyCollection<T> Items { get; }
    public int PageNumber { get; }
    public int TotalPages { get; }
    public int TotalCount { get; }
    public bool HasPreviousPage => PageNumber > 1;
    public bool HasNextPage => PageNumber < TotalPages;

    public PaginatedList(IReadOnlyCollection<T> items, int count, int pageNumber, int pageSize)
    {
        PageNumber = pageNumber;
        TotalPages = (int)Math.Ceiling(count / (double)pageSize);
        TotalCount = count;
        Items = items;
    }

    public static PaginatedList<T> Create(IEnumerable<T> source, int count, int pageNumber, int pageSize)
    {
        var items = source.ToList();
        return new PaginatedList<T>(items, count, pageNumber, pageSize);
    }
}
