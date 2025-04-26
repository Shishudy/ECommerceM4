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
        // Purchase Management
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

		public void DeletePurchase(string fk_user)
		{
			var cart = GetCartByUserID(fk_user);
			if (cart != null)
			{
				DeleteAddressById(cart.FkAddressShipment);
				DeleteCardById(cart.FkCard);
				DeleteInvoice(cart.FkUser);
				_context.PurchaseProducts.RemoveRange(_context.PurchaseProducts.Where(p => p.FkPurchase == cart.PkPurchase));
				_context.Purchases.Remove(cart);
				_context.SaveChanges();
			}
			else
				throw new Exception("Cart not found");
		}

		// -------------------------------
        // Cart Management
        // -------------------------------
		// Get - return, creates if nonexistent
		// Post - create new #
		// Put 	- update, upsert: create or update
		// Delete - remove 
		// -------------------------------

        public List<PurchaseProduct> GetCartItemsByUserID(string fk_user)
        {
			Purchase cart = GetCartByUserID(fk_user);
            List<PurchaseProduct> items = _context.PurchaseProducts.Where(p => p.FkPurchase == cart.PkPurchase).ToList();
            return items;
        }

		public void AddItemToCart(string fk_user, int productId, int quantity)
        {
            var cart = GetCartByUserID(fk_user);

            var existingItem = _context.PurchaseProducts
                .FirstOrDefault(pp => pp.FkPurchase == cart.PkPurchase && pp.FkProduct == productId);

            if (existingItem == null)
            {
                var newItem = new PurchaseProduct
                {
                    FkPurchase = cart.PkPurchase,
                    FkProduct = productId,
                    Qtt = quantity
                };
                _context.PurchaseProducts.Add(newItem);
            }
            else
            {
                existingItem.Qtt += quantity;
				if (existingItem.Qtt < 1)
					_context.PurchaseProducts.Remove(existingItem);
				else
                	_context.PurchaseProducts.Update(existingItem);
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
        // Card Management
        // -------------------------------
		// Get - return #
		// Post - create new
		// Put 	- update #
		// Delete - remove
		// -------------------------------

		public List<Card> GetCardsByUserID(string fk_user)
		{
			var cards = _context.Cards.Where(c => c.FkUser == fk_user && c.Toogle == true).ToList();
			return cards;
		}

        public void AddCardToPurchase(string fk_user, Card card)
        {
            var cart = GetCartByUserID(fk_user);

            var existingCard = _context.Cards
                .FirstOrDefault(c => c.PkCard == cart.FkCard);

			card.FkUser = fk_user;

            if (existingCard == null)
            {
				cart.FkCardNavigation = card;
                _context.Cards.Add(card);
                // cart.FkCard = card.PkCard; // Uncomment if needed
            }
            else
            {
                existingCard.AssignFrom(card); // Copy values from the new card
                _context.Cards.Update(existingCard);
            }
            _context.SaveChanges();
        }

        public void DeleteCardById(int? cardId)
        {
			if (cardId == null)
				throw new Exception("Card ID is null");
            var card = _context.Cards.FirstOrDefault(c => c.PkCard == cardId);
            if (card != null)
            {
                card.Toogle = true; // Soft delete
                _context.SaveChanges();
            }
			else
				throw new Exception("Card not found");
        }


		// -------------------------------
        // Invoice Management
        // -------------------------------
		// Get - return #
		// Post - create new
		// Put 	- update #
		// Delete - remove
		// -------------------------------

		public List<Invoice>? GetInvoicesByUserID(string fk_user)
		{
			IQueryable<int?> fkInvoices = _context.Purchases.Where(p => p.FkUser == fk_user).Select(p => p.FkInvoice);
			var invoices = _context.Invoices.Where(i => fkInvoices.Contains(i.PkInvoice)).ToList();
			return invoices;
		}

		public Invoice? GetInvoiceById(int id)
		{
			return _context.Invoices.FirstOrDefault(i => i.PkInvoice == id);
		}

        public void AddInvoiceToPurchase(string fk_user, Invoice invoice)
        {
            var cart = GetCartByUserID(fk_user);

			cart.Status = "Pending Payment";
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
                existingInvoice.AssignFrom(invoice); // Copy values from the new invoice
                _context.Invoices.Update(existingInvoice);
            }

            _context.SaveChanges();
        }

		public void DeleteInvoice(string fk_user)
		{
			var cart = GetCartByUserID(fk_user);

            var existingInvoice = _context.Invoices
                .FirstOrDefault(i => i.PkInvoice == cart.FkInvoice);
			if (existingInvoice != null && cart.Status != "finalized")
			{
				DeleteAddressById(existingInvoice.FkAddressInvoice);
				_context.Invoices.Remove(existingInvoice);
				_context.SaveChanges();
			}
			else
				throw new Exception("Invoice not found or already finalized");
		}

        // -------------------------------
        // Address Shipment Management
        // -------------------------------

		public List<Address> GetAddressesByUserID(string fk_user)
		{
			var addresses = _context.Addresses.Where(a => a.FkUser == fk_user && a.Toggle == true).ToList();
			return addresses;
		}

		public void AddAddressToPurchase(string fk_user, Address address)
        {
            var cart = GetCartByUserID(fk_user);

            var existingAddress = _context.Addresses
                .FirstOrDefault(a => a.PkAddress == cart.FkAddressShipment);

			address.FkUser = fk_user;

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
                existingAddress.AssignFrom(address); // Copy values from the new address
				_context.Addresses.Update(existingAddress);
            }

            _context.SaveChanges();
        }


		// -------------------------------
        // Address Invoice Management
        // -------------------------------

		public List<Address> GetInvoiceAddressByUser(string fk_user)
		{
			var invoices = GetInvoicesByUserID(fk_user)
				.Where(i => i.FkAddressInvoice != null )
				.Select(i => i.FkAddressInvoiceNavigation);
			return invoices.Where(a => a.Toggle == true).ToList();
		}

		public void AddAddressToInvoice(string fk_user, Address address)
		{
			var cart = GetCartByUserID(fk_user);
			var existingInvoice = _context.Invoices
                .FirstOrDefault(i => i.PkInvoice == cart.FkInvoice);

			// address.FkUser = fk_user;

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
					existingAddress.AssignFrom(address); // Copy values from the new address
					_context.Addresses.Update(existingAddress);
				}
				else
					throw new Exception("Address not found");
			}
			_context.SaveChanges();
		}

        public void DeleteAddressById(int? addressId)
        {
			if (addressId == null)
				throw new Exception("Address ID is null");
            var address = _context.Addresses.FirstOrDefault(a => a.PkAddress == addressId);
            if (address != null)
            {
                address.Toggle = true; // Soft delete
                _context.SaveChanges();
            }
			else 
				new Exception("Address not found");
        }

		// -------------------------------
		// Invoice Management
		// -------------------------------

		// -------------------------------
		// Invoice Management
		// -------------------------------

		// -------------------------------
        // Cart Management
        // -------------------------------


    }
}