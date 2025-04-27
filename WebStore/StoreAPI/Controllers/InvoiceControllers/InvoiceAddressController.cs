using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StoreLibrary.DbModels;
using StoreLibrary.EfCoreMethods;

namespace StoreAPI.Controllers
{
    [Route("api/addresses")]
    [ApiController]
    public class AddressController : ControllerBase
    {
        private readonly PurchaseMethods _purchaseMethods;

        public AddressController(PurchaseMethods purchaseMethods)
        {
            _purchaseMethods = purchaseMethods;
        }

        // GET: api/addresses/invoices/{fk_user}
        [HttpGet("invoices/{fk_user}")]
        public IActionResult GetInvoiceAddressByUser(string fk_user)
        {
            try
            {
                var addresses = _purchaseMethods.GetInvoiceAddressByUser(fk_user);
                if (addresses == null || !addresses.Any())
                    return NotFound("No addresses found for the specified user's invoices.");
                return Ok(addresses);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // POST: api/addresses/invoices/{fk_user}
        [HttpPost("invoices/{fk_user}")]
        public IActionResult AddAddressToInvoice(string fk_user, [FromBody] Address address)
        {
            if (address == null)
                return BadRequest("Address data is required.");

            try
            {
                _purchaseMethods.AddAddressToInvoice(fk_user, address);
                return CreatedAtAction(nameof(GetInvoiceAddressByUser), new { fk_user }, address);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // DELETE: api/addresses/{addressId}
        [HttpDelete("{addressId}")]
        public IActionResult DeleteAddressById(int addressId)
        {
            try
            {
                _purchaseMethods.DeleteAddressById(addressId);
                return NoContent();
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}
