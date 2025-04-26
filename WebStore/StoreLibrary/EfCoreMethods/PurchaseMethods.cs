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

        // -------------------------------
        // CRUD Operations for Purchases
        // -------------------------------
        public List<Purchase> GetAllPurchases()
        {
            return _context.Purchases.ToList();
        }

        // public Purchase GetPurchaseById(int id)
        // {
        //     return _context.Purchases.FirstOrDefault(p => p.PkPurchase == id);
        // }

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

        // -------------------------------
        // Cart Management
        // -------------------------------
        public Purchase GetCartByUserID(string fk_user)
        {
            var cart = _context.Purchases.FirstOrDefault(p => p.FkUser == fk_user && p.Status == "Cart");
            if (cart == null)
            {
                Purchase new_purchase = new Purchase
                {
                    FkUser = fk_user,
                    Status = "Cart",
                };
                AddPurchase(new_purchase); // SaveChanges updates new_purchase
                return new_purchase;
            }
            return cart;
        }

        public List<PurchaseProduct> GetCartInfoByUserID(string fk_user)
        {
            var cart = _context.PurchaseProducts.Where(p => p.FkPurchase == GetCartByUserID(fk_user).PkPurchase).ToList();
            return cart;
        }

        public void AddItemToCart(string fk_user, int productId, int quantity)
        {
            var cart = GetCartByUserID(fk_user);

            var existingItem = _context.PurchaseProducts
                .FirstOrDefault(pp => pp.FkPurchase == cart.PkPurchase && pp.FkProduct == productId);

            if (existingItem != null)
            {
                existingItem.Qtt += quantity;
                _context.PurchaseProducts.Update(existingItem);
            }
            else
            {
                var newItem = new PurchaseProduct
                {
                    FkPurchase = cart.PkPurchase,
                    FkProduct = productId,
                    Qtt = quantity
                };
                _context.PurchaseProducts.Add(newItem);
            }

            _context.SaveChanges();
        }

        public void RemoveItemFromCart(string fk_user, int productId)
        {//TODO if iteam count goes bellow 1, remove it from cart
            var cart = GetCartByUserID(fk_user);

            var itemToRemove = _context.PurchaseProducts
                .FirstOrDefault(pp => pp.FkPurchase == cart.PkPurchase && pp.FkProduct == productId);

            if (itemToRemove != null)
            {
                _context.PurchaseProducts.Remove(itemToRemove);
                _context.SaveChanges();
            }
        }

        // -------------------------------
        // Address Management
        // -------------------------------
        public void AddAddressToPurchase(string fk_user, Address address)
        {
            var cart = GetCartByUserID(fk_user);

            var existingAddress = _context.Addresses
                .FirstOrDefault(a => a.PkAddress == cart.FkAddressShipment);

            if (existingAddress == null)
            {
				// if address in cart is null, add new address, creates an copy
				// cart.FkAddressShipment = address.PkAddress; 
				cart.FkAddressShipmentNavigation = address;
                _context.Addresses.Add(address);
                // cart.FkAddressShipment = address.PkAddress; // Uncomment if needed
            }
            else
            {
				// if address in cart is not null, update existing address
                existingAddress.Street = address.Street;
                existingAddress.City = address.City;
                existingAddress.State = address.State;
                existingAddress.ZipCode = address.ZipCode;
                existingAddress.Country = address.Country;
                _context.Addresses.Update(existingAddress);
            }

            _context.SaveChanges();
        }

        public void DeleteAddressById(int addressId)
        {
            var address = _context.Addresses.FirstOrDefault(a => a.PkAddress == addressId);
            if (address != null)
            {
                address.Toggle = true; // Soft delete
                _context.SaveChanges();
            }
			else 
				new Exception("Address not found");
        }



1

		// 	// Save changes to persist the updates
		// 	_context.SaveChanges();
		// }

        // -------------------------------
        // Card Management
        // -------------------------------
        public void AddCardToPurchase(string fk_user, Card card)
        {
            var cart = GetCartByUserID(fk_user);

            var existingCard = _context.Cards
                .FirstOrDefault(c => c.PkCard == cart.FkCard);

            if (existingCard == null)
            {
				cart.FkCardNavigation = card;
                _context.Cards.Add(card);
                // cart.FkCard = card.PkCard; // Uncomment if needed
            }
            else
            {
                existingCard.CardType = card.CardType;
                existingCard.CardHolder = card.CardHolder;
                existingCard.CardNumber = card.CardNumber;
                existingCard.Cvv = card.Cvv;
                existingCard.ExpirationDate = card.ExpirationDate;
                _context.Cards.Update(existingCard);
            }

            _context.SaveChanges();
        }

        public void DeleteCardById(int cardId)
        {
            var card = _context.Cards.FirstOrDefault(c => c.PkCard == cardId);
            if (card != null)
            {
                card.Toogle = true; // Soft delete
                _context.SaveChanges();
            }
        }

        // -------------------------------
        // Invoice Management
        // -------------------------------

		public void AddAddressToInvoice(string fk_user, Address address)
		{
			var cart = GetCartByUserID(fk_user);
			var existingInvoice = _context.Invoices
                .FirstOrDefault(i => i.PkInvoice == cart.FkInvoice);

			if (existingInvoice.FkAddressInvoice == null)
			{
				existingInvoice.FkAddressInvoiceNavigation = address;
				_context.Addresses.Add(address);
				_context.SaveChanges();
				// existingInvoice.FkAddressInvoice = address.PkAddress;
			}
			else
			{
				var existingAddress = _context.Addresses
					.FirstOrDefault(a => a.PkAddress == existingInvoice.FkAddressInvoice);

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
			_context.SaveChanges();
		}

		// public void DeleteAddressFromInvoice(string fk_user, int addressId)

        public void AddInvoiceToPurchase(string fk_user, Address address, Invoice invoice)
        {
            var cart = GetCartByUserID(fk_user);

            var existingInvoice = _context.Invoices
                .FirstOrDefault(i => i.PkInvoice == cart.FkInvoice);

            if (existingInvoice == null)
            {
				cart.FkInvoiceNavigation = invoice;
                _context.Invoices.Add(invoice);
                // cart.FkInvoice = invoice.PkInvoice; // Uncomment if needed
            }
            else
            {
                existingInvoice.InvoiceType = invoice.InvoiceType;
                existingInvoice.InvoiceStatus = invoice.InvoiceStatus;
                existingInvoice.InvoiceNumber = invoice.InvoiceNumber;
                existingInvoice.InvoiceDate = invoice.InvoiceDate;
                existingInvoice.TotalAmount = invoice.TotalAmount;


                _context.Invoices.Update(existingInvoice);
            }

            _context.SaveChanges();
        }
    }
}