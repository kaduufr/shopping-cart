using Application.Dtos.Categories;
using Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/categories")]
public class CategoryController : ControllerBase
{
    private readonly ICategoryService _categoryService;
    private readonly ILogger<CategoryController> _logger;

    public CategoryController(ICategoryService categoryService, ILogger<CategoryController> logger)
    {
        _categoryService = categoryService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var categories = await _categoryService.GetAllAsync();
        return Ok(categories);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        var category = await _categoryService.GetByIdAsync(id);
        if (category == null)
        {
            return NotFound();
        }
        return Ok(category);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CategoryCreateDto dto)
    {
        try
        {
            var category = await _categoryService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = category.Id }, category);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error creating category");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, [FromBody] CategoryUpdateDto dto)
    {
        var category = await _categoryService.UpdateAsync(id, dto);
        if (category == null)
        {
            return NotFound();
        }
        return Ok(category);
    }

    [HttpPatch("{id}")]
    public async Task<IActionResult> UpdatePartial(string id, [FromBody] CategoryUpdateDto dto)
    {
        var category = await _categoryService.UpdatePartialAsync(id, dto);
        if (category == null)
        {
            return NotFound();
        }
        return Ok(category);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        var result = await _categoryService.DeleteAsync(id);
        if (!result)
        {
            return NotFound();
        }
        return NoContent();
    }
}