namespace Catalog.Domain.Entities.Pagination;

public class PagedResult<T>
{
    public List<T> Itens { get; set; }
    public int CurrentPage { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public int TotalPage => (int)Math.Ceiling(TotalCount / (double)PageSize);

    public bool HasPrevious => CurrentPage > 1;
    public bool HasNext => CurrentPage < TotalPage;

    public PagedResult(int count, int pageNumber, int pageSize, List<T> items)
    {
        Itens = items;
        CurrentPage = pageNumber;
        PageSize = pageSize;
        TotalCount = count;
    }
}
