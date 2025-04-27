using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StoreLibrary.DbModels;
using StoreLibrary.EfCoreMethods;

namespace StoreAPI.Controllers
{
    [Route("api/cart")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly PurchaseMethods _purchaseMethods;

        public CartController(PurchaseMethods purchaseMethods)
        {
            _purchaseMethods = purchaseMethods;
        }

        // GET: api/cart/{fk_user}/items
        [HttpGet("{fk_user}/items")]
        public IActionResult GetCartItemsByUserID(string fk_user)
        {
            try
            {
                var items = _purchaseMethods.GetCartItemsByUserID(fk_user);
                if (items == null || !items.Any())
                    return NotFound("No items found in the cart for the specified user.");
                return Ok(items);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // POST: api/cart/{fk_user}/items
        [HttpPost("{fk_user}/items")]
        public IActionResult AddItemToCart(string fk_user, [FromBody] CartItemDto cartItem)
        {
            if (cartItem == null || cartItem.ProductId <= 0 || cartItem.Quantity <= 0)
                return BadRequest("Invalid cart item data.");

            try
            {
                _purchaseMethods.AddItemToCart(fk_user, cartItem.ProductId, cartItem.Quantity);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // DELETE: api/cart/{fk_user}/items/{productId}
        [HttpDelete("{fk_user}/items/{productId}")]
        public IActionResult RemoveItemFromCart(string fk_user, int productId)
        {
            try
            {
                _purchaseMethods.RemoveItemFromCart(fk_user, productId);
                return NoContent();
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }
    }

    // DTO for adding items to the cart
    public class CartItemDto
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }
}