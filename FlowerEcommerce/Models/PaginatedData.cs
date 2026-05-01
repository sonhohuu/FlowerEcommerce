namespace FlowerEcommerce.View.Models;

public class PaginatedData<T>
{
    public int Size { get; set; }
    public int Page { get; set; }
    public int Total { get; set; }
    public int TotalPages { get; set; }
    public List<T> Items { get; set; } = new();
}
