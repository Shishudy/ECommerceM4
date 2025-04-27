using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StoreLibrary.DbModels;
using StoreLibrary.EfCoreMethods;


namespace StoreAPI.Controllers.InvoiceControllers
{
    [Route("api/cards")]
    [ApiController]
    public class CardController : ControllerBase
    {
        private readonly PurchaseMethods _purchaseMethods;

        public CardController(PurchaseMethods purchaseMethods)
        {
            _purchaseMethods = purchaseMethods;
        }

        // GET: api/cards/{fk_user}
        [HttpGet("{fk_user}")]
        public IActionResult GetCardsByUserID(string fk_user)
        {
            var cards = _purchaseMethods.GetCardsByUserID(fk_user);
            if (cards == null || !cards.Any())
                return NotFound("No cards found for the specified user.");
            return Ok(cards);
        }

        // POST: api/cards/{fk_user}
        [HttpPost("{fk_user}")]
        public IActionResult AddCardToPurchase(string fk_user, [FromBody] Card card)
        {
            if (card == null)
                return BadRequest("Card data is required.");

            _purchaseMethods.AddCardToPurchase(fk_user, card);
            return CreatedAtAction(nameof(GetCardsByUserID), new { fk_user }, card);
        }

        // DELETE: api/cards/{cardId}
        [HttpDelete("{cardId}")]
        public IActionResult DeleteCardById(int cardId)
        {
            try
            {
                _purchaseMethods.DeleteCardById(cardId);
                return NoContent();
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}