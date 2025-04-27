using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StoreLibrary.DbModels;
using StoreLibrary.EfCoreMethods;

namespace StoreAPI.Controllers.InvoiceControllers
{
    [Route("api/invoices")]
    [ApiController]
    public class InvoiceController : ControllerBase
    {
        private readonly PurchaseMethods _purchaseMethods;

        public InvoiceController(PurchaseMethods purchaseMethods)
        {
            _purchaseMethods = purchaseMethods;
        }

        // GET: api/invoices/{fk_user}
        [HttpGet("{fk_user}")]
        public IActionResult GetInvoicesByUserID(string fk_user)
        {
            var invoices = _purchaseMethods.GetInvoicesByUserID(fk_user);
            if (invoices == null || !invoices.Any())
                return NotFound("No invoices found for the specified user.");
            return Ok(invoices);
        }

        // GET: api/invoices/id/{id}
        [HttpGet("id/{id}")]
        public IActionResult GetInvoiceById(int id)
        {
            var invoice = _purchaseMethods.GetInvoiceById(id);
            if (invoice == null)
                return NotFound("Invoice not found.");
            return Ok(invoice);
        }

        // POST: api/invoices/{fk_user}
        [HttpPost("{fk_user}")]
        public IActionResult AddInvoiceToPurchase(string fk_user, [FromBody] Invoice invoice)
        {
            if (invoice == null)
                return BadRequest("Invoice data is required.");

            _purchaseMethods.AddInvoiceToPurchase(fk_user, invoice);
            return CreatedAtAction(nameof(GetInvoicesByUserID), new { fk_user }, invoice);
        }

        // DELETE: api/invoices/{fk_user}
        [HttpDelete("{fk_user}")]
        public IActionResult DeleteInvoice(string fk_user)
        {
            try
            {
                _purchaseMethods.DeleteInvoice(fk_user);
                return NoContent();
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}
