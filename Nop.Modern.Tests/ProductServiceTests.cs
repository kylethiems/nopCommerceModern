namespace Nop.Modern.Tests;

using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Microsoft.EntityFrameworkCore;
using Nop.Modern.Api.Data;
using Nop.Modern.Api.Models;
using Nop.Modern.Api.Services;

public class ProductServiceTests
{
    private NopDbContext CreateContext(string dbName)
    {
        var options = new DbContextOptionsBuilder<NopDbContext>()
            .UseInMemoryDatabase(databaseName: dbName)
            .Options;
        return new NopDbContext(options);
    }

    [Fact]
    public async Task SearchProductsAsync_Default_ReturnsOnlyPublishedAndNonDeleted()
    {
        // Arrange
        using var context = CreateContext(nameof(SearchProductsAsync_Default_ReturnsOnlyPublishedAndNonDeleted));
        context.Products.AddRange(
            new ProductEntity { Id = 1, Name = "Laptop", Price = 999m, Published = true, Deleted = false },
            new ProductEntity { Id = 2, Name = "Draft Laptop", Price = 899m, Published = false, Deleted = false },
            new ProductEntity { Id = 3, Name = "Old Laptop", Price = 499m, Published = true, Deleted = true }
        );
        await context.SaveChangesAsync();

        var service = new ProductService(context);

        // Act
        var result = await service.SearchProductsAsync();

        // Assert
        Assert.Equal(1, result.TotalCount);
        Assert.Single(result.Items);
        Assert.Equal("Laptop", result.Items[0].Name);
    }

    [Fact]
    public async Task SearchProductsAsync_ShowHidden_ReturnsUnpublished()
    {
        // Arrange
        using var context = CreateContext(nameof(SearchProductsAsync_ShowHidden_ReturnsUnpublished));
        context.Products.AddRange(
            new ProductEntity { Id = 1, Name = "Public Item", Price = 10m, Published = true },
            new ProductEntity { Id = 2, Name = "Hidden Item", Price = 20m, Published = false }
        );
        await context.SaveChangesAsync();

        var service = new ProductService(context);

        // Act
        var result = await service.SearchProductsAsync(showHidden: true);

        // Assert
        Assert.Equal(2, result.TotalCount);
    }

    [Fact]
    public async Task SearchProductsAsync_Keywords_MatchesNameOrSku()
    {
        // Arrange
        using var context = CreateContext(nameof(SearchProductsAsync_Keywords_MatchesNameOrSku));
        context.Products.AddRange(
            new ProductEntity { Id = 1, Name = "Apple iPhone", Sku = "IPHONE_15", Published = true },
            new ProductEntity { Id = 2, Name = "Samsung Galaxy", Sku = "GALAXY_S24", Published = true }
        );
        await context.SaveChangesAsync();

        var service = new ProductService(context);

        // Act
        var result = await service.SearchProductsAsync(keywords: "iPhone");

        // Assert
        Assert.Single(result.Items);
        Assert.Equal("Apple iPhone", result.Items[0].Name);
    }

    [Fact]
    public async Task GetProductByIdAsync_ExistingId_ReturnsDto()
    {
        // Arrange
        using var context = CreateContext(nameof(GetProductByIdAsync_ExistingId_ReturnsDto));
        context.Products.Add(new ProductEntity { Id = 42, Name = "Tablet", Price = 299m, Published = true });
        await context.SaveChangesAsync();

        var service = new ProductService(context);

        // Act
        var product = await service.GetProductByIdAsync(42);

        // Assert
        Assert.NotNull(product);
        Assert.Equal(42, product.Id);
        Assert.Equal("Tablet", product.Name);
    }

    [Fact]
    public async Task GetProductByIdAsync_NonExistent_ReturnsNull()
    {
        // Arrange
        using var context = CreateContext(nameof(GetProductByIdAsync_NonExistent_ReturnsNull));
        var service = new ProductService(context);

        // Act
        var product = await service.GetProductByIdAsync(999);

        // Assert
        Assert.Null(product);
    }

    [Fact]
    public async Task InsertProductAsync_ValidDto_InsertsAndReturnsPopulated()
    {
        // Arrange
        using var context = CreateContext(nameof(InsertProductAsync_ValidDto_InsertsAndReturnsPopulated));
        var service = new ProductService(context);

        var newDto = new ProductDto(
            0,
            "Wireless Mouse",
            "MOU_01",
            "Ergonomic mouse",
            "Long description",
            29.99m,
            39.99m,
            100,
            true,
            DateTime.UtcNow
        );

        // Act
        var created = await service.InsertProductAsync(newDto);

        // Assert
        Assert.True(created.Id > 0);
        var inDb = await context.Products.FindAsync(created.Id);
        Assert.NotNull(inDb);
        Assert.Equal("Wireless Mouse", inDb.Name);
    }

    [Fact]
    public async Task UpdateProductAsync_ValidProduct_UpdatesProperties()
    {
        // Arrange
        using var context = CreateContext(nameof(UpdateProductAsync_ValidProduct_UpdatesProperties));
        context.Products.Add(new ProductEntity { Id = 10, Name = "Old Name", Price = 50m, Published = true });
        await context.SaveChangesAsync();

        var service = new ProductService(context);

        var updateDto = new ProductDto(
            10,
            "New Name",
            "SKU_NEW",
            "New desc",
            "New full desc",
            75m,
            90m,
            50,
            true,
            DateTime.UtcNow
        );

        // Act
        var success = await service.UpdateProductAsync(updateDto);

        // Assert
        Assert.True(success);
        var inDb = await context.Products.FindAsync(10);
        Assert.NotNull(inDb);
        Assert.Equal("New Name", inDb.Name);
        Assert.Equal(75m, inDb.Price);
    }

    [Fact]
    public async Task DeleteProductAsync_ExistingId_SoftDeletes()
    {
        // Arrange
        using var context = CreateContext(nameof(DeleteProductAsync_ExistingId_SoftDeletes));
        context.Products.Add(new ProductEntity { Id = 20, Name = "Item to Delete", Published = true, Deleted = false });
        await context.SaveChangesAsync();

        var service = new ProductService(context);

        // Act
        var success = await service.DeleteProductAsync(20);

        // Assert
        Assert.True(success);
        var inDb = await context.Products.FindAsync(20);
        Assert.NotNull(inDb);
        Assert.True(inDb.Deleted);
    }
}
