using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StoreLibrary.DbModels;
using StoreLibrary.EfCoreMethods;

namespace StoreAPI.Controllers
{
    [Route("api/shipment-addresses")]
    [ApiController]
    public class ShipmentAddressController : ControllerBase
    {
        private readonly PurchaseMethods _purchaseMethods;

        public ShipmentAddressController(PurchaseMethods purchaseMethods)
        {
            _purchaseMethods = purchaseMethods;
        }

        // GET: api/shipment-addresses/{fk_user}
        [HttpGet("{fk_user}")]
        public IActionResult GetAddressesByUserID(string fk_user)
        {
            try
            {
                var addresses = _purchaseMethods.GetAddressesByUserID(fk_user);
                if (addresses == null || !addresses.Any())
                    return NotFound("No shipment addresses found for the specified user.");
                return Ok(addresses);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // POST: api/shipment-addresses/{fk_user}
        [HttpPost("{fk_user}")]
        public IActionResult AddAddressToPurchase(string fk_user, [FromBody] Address address)
        {
            if (address == null)
                return BadRequest("Address data is required.");

            try
            {
                _purchaseMethods.AddAddressToPurchase(fk_user, address);
                return CreatedAtAction(nameof(GetAddressesByUserID), new { fk_user }, address);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}