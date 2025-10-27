using Application.Dtos.Products;

namespace Api.Controllers;

[ApiController]
[Route("api/products")]
public class ProductController : ControllerBase
{
    private readonly IProductService _productService;
    private readonly ILogger<ProductController> _logger;

    public ProductController(IProductService productService, ILogger<ProductController> logger)
    {
        _productService = productService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var products = await _productService.GetAllAsync();
        return Ok(products);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        var product = await _productService.GetByIdAsync(id);
        if (product == null)
        {
            return NotFound();
        }
        return Ok(product);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] List<ProductCreateDto> dto)
    {
        try
        {
            await _productService.InsertProductsBulkAsync(dto);
            return CreatedAtAction(nameof(GetAll), null);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error creating products");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, [FromBody] ProductUpdateDto dto)
    {
        var product = await _productService.UpdateAsync(id, dto);
        if (product == null)
        {
            return NotFound();
        }
        return Ok(product);
    }

    [HttpPatch("{id}")]
    public async Task<IActionResult> UpdatePartial(string id, [FromBody] ProductUpdateDto patchDto)
    {
        var product = await _productService.UpdatePartialAsync(id, patchDto);
        if (product == null)
        {
            return NotFound();
        }
        return Ok(product);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        var result = await _productService.DeleteAsync(id);
        if (!result)
        {
            return NotFound();
        }
        return NoContent();
    }
}
