
using StoreLibrary.DbModels;
using System.Collections.Generic;
using System.Linq;

namespace StoreLibrary.EfCoreMethods
{
    public class PurchaseMethods
    {
        private readonly StoreDbContext _context;

        public PurchaseMethods(StoreDbContext context)
        {
            _context = context;
        }
		
		public List<Purchase> GetPurchasesByCustomerId(string customerId)
		{
			return _context.Purchases.Where(p => p.FkUser == customerId).ToList();
		}

		public Purchase GetCartByUserID(string fk_user)
		{
			var cart =  _context.Purchases.FirstOrDefault(p => p.FkUser == fk_user && p.Status == "Cart");
			if (cart == null)
			{
				Purchase new_purchase = new Purchase
				{
					FkUser = fk_user,
					Status = "Cart",
				};
				AddPurchase(new_purchase); // .SaveChanges() updates new_purchase
				return new_purchase;
			}
			return cart;
		}

		// public List<PurchaseProduct> GetCartInfoByPurchaseId(int fk_purchase)
		// {
		// 	var cart = _context.PurchaseProducts.Where(p => p.FkPurchase == fk_purchase).ToList();
		// 	return cart;
		// }

		public List<PurchaseProduct> GetCartInfoByUserID(string fk_user)
		{
			var cart = _context.PurchaseProducts.Where(p => p.FkPurchase == GetCartByUserID(fk_user).PkPurchase).ToList();
			return cart;
		}

		
        public List<Purchase> GetAllPurchases()
        {
            return _context.Purchases.ToList();
        }

        public Purchase GetPurchaseById(int id)
        {
            return _context.Purchases.FirstOrDefault(p => p.PkPurchase == id);
        }

        public void AddPurchase(Purchase purchase)
        {
            _context.Purchases.Add(purchase);
            _context.SaveChanges();
        }

        public void UpdatePurchase(Purchase purchase)
        {
            _context.Purchases.Update(purchase);
            _context.SaveChanges();
        }

        public void DeletePurchase(int id)
        {
            var purchase = _context.Purchases.FirstOrDefault(p => p.PkPurchase == id);
            if (purchase != null)
            {
                _context.Purchases.Remove(purchase);
                _context.SaveChanges();
            }
        }

		public void AddItemToCart(string fk_user, int productId, int quantity)
		{
			// Get the user's cart
			var cart = GetCartByUserID(fk_user);

			// Check if the product is already in the cart
			var existingItem = _context.PurchaseProducts
				.FirstOrDefault(pp => pp.FkPurchase == cart.PkPurchase && pp.FkProduct == productId);

			if (existingItem != null)
			{
				// If the product is already in the cart, update the quantity
				existingItem.Qtt += quantity;
				_context.PurchaseProducts.Update(existingItem);
			}
			else
			{
				// If the product is not in the cart, add a new entry
				var newItem = new PurchaseProduct
				{
					FkPurchase = cart.PkPurchase,
					FkProduct = productId,
					Qtt = quantity
				};
				_context.PurchaseProducts.Add(newItem);
			}

			// Save changes to the database
			_context.SaveChanges();
		}

		public void RemoveItemFromCart(string fk_user, int productId)
		{
			// Get the user's cart
			var cart = GetCartByUserID(fk_user);
			// Find the item to remove
			var itemToRemove = _context.PurchaseProducts
				.FirstOrDefault(pp => pp.FkPurchase == cart.PkPurchase && pp.FkProduct == productId);
			if (itemToRemove != null)
			{
				_context.PurchaseProducts.Remove(itemToRemove);
				_context.SaveChanges();
			}
		}
		
		public void DeleteCardById (int cardId)
		{
			var card = _context.Cards.FirstOrDefault(c => c.PkCard == cardId);
			if (card != null)
			{
				card.Toogle = true; // Soft delete
				_context.SaveChanges();
			}
		}

		public void DeleteAddressById (int addressId)
		{
			var address = _context.Addresses.FirstOrDefault(a => a.PkAddress == addressId);
			if (address != null)
			{
				address.Toggle = true; // Soft delete
				_context.SaveChanges();
			}
		}

		public void AddCardToPurchase(string fk_user, Card card)
		{
			// Get the user's cart
			var cart = GetCartByUserID(fk_user);
			// Check if the card is already in the purchase
			var existingCard = _context.Cards
				.FirstOrDefault(c => c.PkCard == cart.FkCard);
			if (existingCard != null)
			{
				// If the card is already in the purchase, update the card details
				existingCard.CardType = card.CardType;
				existingCard.CardHolder = card.CardHolder;
				existingCard.CardNumber = card.CardNumber;
				existingCard.Cvv = card.Cvv;
				existingCard.ExpirationDate = card.ExpirationDate;
				_context.Cards.Update(existingCard);
			}
			else
			{
				// If the card is not in the purchase, add a new entry
				_context.Cards.Add(card);
				// cart.PkCard = card.PkCard;
			}
			// Save changes to the database
			_context.SaveChanges();
		}

		public void AddAddressToPurchase(string fk_user, Address address)
		{
			// Get the user's cart
			var cart = GetCartByUserID(fk_user);
			// Check if the address is already in the purchase
			var existingAddress = _context.Addresses
				.FirstOrDefault(a => a.PkAddress == cart.FkAddressShipment);
			if (existingAddress != null)
			{
				// If the address is already in the purchase, update the address details
				existingAddress.Street = address.Street;
				existingAddress.City = address.City;
				existingAddress.State = address.State;
				existingAddress.ZipCode = address.ZipCode;
				existingAddress.Country = address.Country;
				_context.Addresses.Update(existingAddress);
			}
			else
			{
				// If the address is not in the purchase, add a new entry
				_context.Addresses.Add(address);
				// cart.PkAddress = address.PkAddress;
			}
			// Save changes to the database
			_context.SaveChanges();
    	}

		public void AddInvoiceToPurchase(string fk_user, Address address, Invoice invoice)
		{
			// Get the user's cart
			var cart = GetCartByUserID(fk_user);
			// Check if the invoice is already in the purchase
			var existingInvoice = _context.Invoices
				.FirstOrDefault(i => i.PkInvoice == cart.FkInvoice);
			if (existingInvoice != null)
			{
				// If the invoice is already in the purchase, update the invoice details
				existingInvoice.InvoiceType = invoice.InvoiceType;
				existingInvoice.InvoiceStatus = invoice.InvoiceStatus;

				existingInvoice.InvoiceNumber = invoice.InvoiceNumber;
				existingInvoice.InvoiceDate = invoice.InvoiceDate;
				existingInvoice.TotalAmount = invoice.TotalAmount;

					// Check if the address is already associated with the invoice
				if (existingInvoice.FkAddressInvoice == null)
				{
					// Add the address and associate it with the invoice
					_context.Addresses.Add(address);
					_context.SaveChanges(); // Save to get the generated PkAddress
					existingInvoice.FkAddressInvoice = address.PkAddress;
				}
				else
				{
					// Update the existing address
					var existingAddress = _context.Addresses.FirstOrDefault(a => a.PkAddress == existingInvoice.FkAddressInvoice);
					if (existingAddress != null)
					{
						existingAddress.Street = address.Street;
						existingAddress.City = address.City;
						existingAddress.State = address.State;
						existingAddress.ZipCode = address.ZipCode;
						existingAddress.Country = address.Country;
						_context.Addresses.Update(existingAddress);
					}
				}
				
				_context.Invoices.Update(existingInvoice);
			}
			else
			{
				// If the invoice is not in the purchase, add a new entry
				_context.Invoices.Add(invoice);
				// cart.PkInvoice = invoice.PkInvoice;
			}
			// Save changes to the database
			_context.SaveChanges();
		}
	}
}