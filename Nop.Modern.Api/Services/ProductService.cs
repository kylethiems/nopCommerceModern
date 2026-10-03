namespace Nop.Modern.Api.Services;

using Microsoft.EntityFrameworkCore;
using Nop.Modern.Api.Data;
using Nop.Modern.Api.Models;

public class ProductService : IProductService
{
    private readonly NopDbContext _context;

    public ProductService(NopDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<PagedListResult<ProductDto>> SearchProductsAsync(
        int pageIndex = 0,
        int pageSize = 10,
        string? keywords = null,
        bool showHidden = false,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Products.AsNoTracking().Where(p => !p.Deleted);

        if (!showHidden)
        {
            query = query.Where(p => p.Published);
        }

        if (!string.IsNullOrWhiteSpace(keywords))
        {
            var term = keywords.Trim().ToLower();
            query = query.Where(p => p.Name.ToLower().Contains(term) || (p.Sku != null && p.Sku.ToLower().Contains(term)));
        }

        var totalCount = await query.CountAsync(cancellationToken);
        var totalPages = pageSize > 0 ? (int)Math.Ceiling(totalCount / (double)pageSize) : 0;

        var items = await query
            .OrderBy(p => p.Name)
            .Skip(pageIndex * pageSize)
            .Take(pageSize)
            .Select(p => new ProductDto(
                p.Id,
                p.Name,
                p.Sku,
                p.ShortDescription,
                p.FullDescription,
                p.Price,
                p.OldPrice,
                p.StockQuantity,
                p.Published,
                p.CreatedOnUtc
            ))
            .ToListAsync(cancellationToken);

        return new PagedListResult<ProductDto>(
            pageIndex,
            pageSize,
            totalCount,
            totalPages,
            pageIndex > 0,
            pageIndex + 1 < totalPages,
            items
        );
    }

    public async Task<ProductDto?> GetProductByIdAsync(int productId, CancellationToken cancellationToken = default)
    {
        var entity = await _context.Products.AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == productId && !p.Deleted, cancellationToken);

        if (entity == null)
        {
            return null;
        }

        return new ProductDto(
            entity.Id,
            entity.Name,
            entity.Sku,
            entity.ShortDescription,
            entity.FullDescription,
            entity.Price,
            entity.OldPrice,
            entity.StockQuantity,
            entity.Published,
            entity.CreatedOnUtc
        );
    }

    public async Task<ProductDto> InsertProductAsync(ProductDto product, CancellationToken cancellationToken = default)
    {
        if (product == null)
        {
            throw new ArgumentNullException(nameof(product));
        }

        var entity = new ProductEntity
        {
            Name = product.Name,
            Sku = product.Sku,
            ShortDescription = product.ShortDescription,
            FullDescription = product.FullDescription,
            Price = product.Price,
            OldPrice = product.OldPrice,
            StockQuantity = product.StockQuantity,
            Published = product.Published,
            Deleted = false,
            CreatedOnUtc = DateTime.UtcNow,
            UpdatedOnUtc = DateTime.UtcNow
        };

        _context.Products.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);

        return product with { Id = entity.Id, CreatedOnUtc = entity.CreatedOnUtc };
    }

    public async Task<bool> UpdateProductAsync(ProductDto product, CancellationToken cancellationToken = default)
    {
        if (product == null)
        {
            throw new ArgumentNullException(nameof(product));
        }

        var entity = await _context.Products
            .FirstOrDefaultAsync(p => p.Id == product.Id && !p.Deleted, cancellationToken);

        if (entity == null)
        {
            return false;
        }

        entity.Name = product.Name;
        entity.Sku = product.Sku;
        entity.ShortDescription = product.ShortDescription;
        entity.FullDescription = product.FullDescription;
        entity.Price = product.Price;
        entity.OldPrice = product.OldPrice;
        entity.StockQuantity = product.StockQuantity;
        entity.Published = product.Published;
        entity.UpdatedOnUtc = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> DeleteProductAsync(int productId, CancellationToken cancellationToken = default)
    {
        var entity = await _context.Products
            .FirstOrDefaultAsync(p => p.Id == productId, cancellationToken);

        if (entity == null)
        {
            return false;
        }

        entity.Deleted = true;
        entity.UpdatedOnUtc = DateTime.UtcNow;
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}
