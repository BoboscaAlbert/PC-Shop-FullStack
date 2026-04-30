using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace PCShop.API.Controllers;

public class CartItemRequest
{
    public int ProductId { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }
    public int Quantity { get; set; }

    public string ImageUrl { get; set; }
}

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CartController : ControllerBase
{
    private static List<CartItemRequest> _tempCart = new List<CartItemRequest>();

    [HttpPost]
    public IActionResult AddToCart([FromBody] CartItemRequest dto)
    {
        if (dto == null) return BadRequest();
        _tempCart.Add(dto);
        return Ok(new { message = "Success" });
    }

    [HttpGet]
    public IActionResult GetCartItems()
    {
        return Ok(_tempCart);
    }

    [HttpDelete("{index}")]
    public IActionResult RemoveItem(int index)
    {
        if (index >= 0 && index < _tempCart.Count)
        {
            _tempCart.RemoveAt(index);
            return Ok();
        }
        return BadRequest();
    }

    [HttpDelete]
    public IActionResult ClearCart()
    {
        _tempCart.Clear();
        return Ok();
    }
}