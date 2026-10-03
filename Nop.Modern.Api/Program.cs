using Microsoft.EntityFrameworkCore;
using Nop.Modern.Api.Data;
using Nop.Modern.Api.Models;
using Nop.Modern.Api.Services;

var builder = WebApplication.CreateBuilder(args);

// Register EF Core DbContext
builder.Services.AddDbContext<NopDbContext>(options =>
{
    var conn = builder.Configuration.GetConnectionString("NopDb");
    if (!string.IsNullOrEmpty(conn))
    {
        options.UseSqlServer(conn);
    }
    else
    {
        options.UseInMemoryDatabase("NopCommerceDb");
    }
});

// Dependency Injection
builder.Services.AddScoped<IProductService, ProductService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Seed initial product catalog
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<NopDbContext>();
    if (db.Database.IsInMemory() && !db.Products.Any())
    {
        db.Products.AddRange(
            new ProductEntity
            {
                Id = 1,
                Name = "Build your own computer",
                Sku = "COMP_CUST",
                ShortDescription = "Customizable PC with modern GPU and NVMe storage.",
                FullDescription = "High performance enterprise workstation built for software development.",
                Price = 1200.00m,
                OldPrice = 1350.00m,
                ProductCost = 900.00m,
                StockQuantity = 45,
                Published = true
            },
            new ProductEntity
            {
                Id = 2,
                Name = "Digital Storm Gaming Desktop",
                Sku = "DS_DESK",
                ShortDescription = "Pre-configured desktop with liquid cooling.",
                FullDescription = "Pre-configured desktop ready for intensive machine learning inference.",
                Price = 2499.00m,
                OldPrice = 2799.00m,
                ProductCost = 1800.00m,
                StockQuantity = 12,
                Published = true
            },
            new ProductEntity
            {
                Id = 3,
                Name = "Nikon D5500 DSLR",
                Sku = "NIK_D5500",
                ShortDescription = "24.2 MP DX-format CMOS sensor with Wi-Fi.",
                FullDescription = "High resolution camera with vari-angle touchscreen LCD.",
                Price = 699.95m,
                OldPrice = 750.00m,
                ProductCost = 500.00m,
                StockQuantity = 28,
                Published = true
            }
        );
        db.SaveChanges();
    }
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
