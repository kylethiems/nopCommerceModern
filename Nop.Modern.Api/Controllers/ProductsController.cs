namespace Nop.Modern.Api.Controllers;

using Microsoft.AspNetCore.Mvc;
using Nop.Modern.Api.Models;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;

    public ProductsController(IProductService productService)
    {
        _productService = productService ?? throw new ArgumentNullException(nameof(productService));
    }

    [HttpGet]
    public async Task<ActionResult<PagedListResult<ProductDto>>> Get(
        [FromQuery] int pageIndex = 0,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? keywords = null,
        [FromQuery] bool showHidden = false,
        CancellationToken cancellationToken = default)
    {
        var result = await _productService.SearchProductsAsync(pageIndex, pageSize, keywords, showHidden, cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ProductDto>> GetById(int id, CancellationToken cancellationToken = default)
    {
        var product = await _productService.GetProductByIdAsync(id, cancellationToken);
        if (product == null)
        {
            return NotFound();
        }
        return Ok(product);
    }

    [HttpPost]
    public async Task<ActionResult<ProductDto>> Create([FromBody] ProductDto product, CancellationToken cancellationToken = default)
    {
        if (product == null)
        {
            return BadRequest();
        }

        var created = await _productService.InsertProductAsync(product, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] ProductDto product, CancellationToken cancellationToken = default)
    {
        if (product == null)
        {
            return BadRequest();
        }

        var success = await _productService.UpdateProductAsync(product with { Id = id }, cancellationToken);
        if (!success)
        {
            return NotFound();
        }

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken = default)
    {
        var success = await _productService.DeleteProductAsync(id, cancellationToken);
        if (!success)
        {
            return NotFound();
        }

        return NoContent();
    }
}
