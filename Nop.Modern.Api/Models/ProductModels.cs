namespace Nop.Modern.Api.Models;

public record ProductDto(
    int Id,
    string Name,
    string Sku,
    string? ShortDescription,
    string? FullDescription,
    decimal Price,
    decimal OldPrice,
    int StockQuantity,
    bool Published,
    DateTime CreatedOnUtc
);

public record PagedListResult<T>(
    int PageIndex,
    int PageSize,
    int TotalCount,
    int TotalPages,
    bool HasPreviousPage,
    bool HasNextPage,
    IReadOnlyList<T> Items
);

public class ProductEntity
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Sku { get; set; } = string.Empty;
    public string? ShortDescription { get; set; }
    public string? FullDescription { get; set; }
    public decimal Price { get; set; }
    public decimal OldPrice { get; set; }
    public decimal ProductCost { get; set; }
    public int StockQuantity { get; set; }
    public bool Published { get; set; } = true;
    public bool Deleted { get; set; }
    public DateTime CreatedOnUtc { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedOnUtc { get; set; } = DateTime.UtcNow;
}

public interface IProductService
{
    Task<PagedListResult<ProductDto>> SearchProductsAsync(
        int pageIndex = 0,
        int pageSize = 10,
        string? keywords = null,
        bool showHidden = false,
        CancellationToken cancellationToken = default);

    Task<ProductDto?> GetProductByIdAsync(int productId, CancellationToken cancellationToken = default);

    Task<ProductDto> InsertProductAsync(ProductDto product, CancellationToken cancellationToken = default);

    Task<bool> UpdateProductAsync(ProductDto product, CancellationToken cancellationToken = default);

    Task<bool> DeleteProductAsync(int productId, CancellationToken cancellationToken = default);
}
