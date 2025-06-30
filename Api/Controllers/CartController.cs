
namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CartController : ControllerBase
{
    private readonly ICartService _cartService;

    public CartController(ICartService cartService)
    {
        _cartService = cartService;
    }

    [HttpGet]
    public async Task<IActionResult> GetCart(string userEmail)
    {
        var cart = await _cartService.GetCartAsync(userEmail);
        return Ok(cart);
    }

    [HttpPost("items")]
    public async Task<IActionResult> AddItemToCart(string userEmail, [FromBody] AddItemToCartDto dto)
    {
        await _cartService.AddItemToCartAsync(userEmail, dto);
        return Ok();
    }

    [HttpDelete("items")]
    public async Task<IActionResult> RemoveItemFromCart(string userEmail, [FromBody] RemoveItemFromCartDto dto)
    {
        await _cartService.RemoveItemFromCartAsync(userEmail, dto);
        return Ok();
    }
}