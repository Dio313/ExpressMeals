namespace ExpressMeals.Contracts.Wrappers;

public class PaginatedList<T>
{
    public PaginatedList()
    {

    }
    public int PageIndex { get; set; }
    public int TotalPages { get; set; }
    public List<T> Items { get; set; } = null!;

    public PaginatedList(List<T> items, int count, int pageIndex, int pageSize)
    {
        PageIndex = pageIndex;
        TotalPages = (int)Math.Ceiling(count / (double)pageSize);

        Items = new List<T>();
        Items.AddRange(items);
    }

    public bool HasPreviousPage
    {
        get
        {
            return (PageIndex > 1);
        }
        set { }
    }

    public bool HasNextPage
    {
        get
        {
            return (PageIndex < TotalPages);
        }
        set { }
    }

    public static Task<PaginatedList<T>> CreateAsync(IEnumerable<T> source, int pageIndex, int pageSize)
    {
        var count = source.Count();
        var items = source.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
        return Task.FromResult(new PaginatedList<T>(items, count, pageIndex, pageSize));
    }
}