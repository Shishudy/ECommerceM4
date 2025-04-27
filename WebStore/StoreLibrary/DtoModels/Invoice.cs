using System;
using System.Collections.Generic;

namespace StoreLibrary.DbModels
{
    // DTOInvoice class
    public partial class DTOInvoice
    {
        public int PkInvoice { get; set; }
        public int FkAddressInvoice { get; set; }
        public string Name { get; set; } = null!;
        public int Nif { get; set; }
        public DateOnly DateInvoice { get; set; }
        public string? PaypallConfirmation { get; set; }
        public double? Amount { get; set; }
        public virtual Address FkAddressInvoiceNavigation { get; set; } = null!;
        public virtual ICollection<Purchase> Purchases { get; set; } = new List<Purchase>();
    }

    // Invoice class
    public partial class Invoice
    {
        public Invoice() { }

        // Copy constructor
        public Invoice(Invoice invoice)
        {
            PkInvoice = invoice.PkInvoice;
            FkAddressInvoice = invoice.FkAddressInvoice;
            Name = invoice.Name;
            Nif = invoice.Nif;
            DateInvoice = invoice.DateInvoice;
            PaypallConfirmation = invoice.PaypallConfirmation;
            Amount = invoice.Amount;
            FkAddressInvoiceNavigation = invoice.FkAddressInvoiceNavigation;
            Purchases = invoice.Purchases;
        }

        // Method to assign values from another Invoice object
        public void AssignFrom(Invoice invoice)
        {
            PkInvoice = invoice.PkInvoice;
            FkAddressInvoice = invoice.FkAddressInvoice;
            Name = invoice.Name;
            Nif = invoice.Nif;
            DateInvoice = invoice.DateInvoice;
            PaypallConfirmation = invoice.PaypallConfirmation;
            Amount = invoice.Amount;
            FkAddressInvoiceNavigation = invoice.FkAddressInvoiceNavigation;
            Purchases = invoice.Purchases;
        }

        // Explicit conversion from DTOInvoice to Invoice
        public static explicit operator Invoice(DTOInvoice dto)
        {
            return new Invoice
            {
                PkInvoice = dto.PkInvoice,
                //FkAddressInvoice = dto.FkAddressInvoice,
                Name = dto.Name,
                Nif = dto.Nif,
                DateInvoice = dto.DateInvoice,
                PaypallConfirmation = dto.PaypallConfirmation,
                Amount = dto.Amount,
                FkAddressInvoiceNavigation = dto.FkAddressInvoiceNavigation,
                Purchases = dto.Purchases
            };
        }

        // Explicit conversion from Invoice to DTOInvoice
        public static explicit operator DTOInvoice(Invoice invoice)
        {
            return new DTOInvoice
            {
                PkInvoice = invoice.PkInvoice,
                FkAddressInvoice = invoice.FkAddressInvoice,
                Name = invoice.Name,
                Nif = invoice.Nif,
                DateInvoice = invoice.DateInvoice,
                PaypallConfirmation = invoice.PaypallConfirmation,
                Amount = invoice.Amount,
                FkAddressInvoiceNavigation = invoice.FkAddressInvoiceNavigation,
                Purchases = invoice.Purchases
            };
        }
    }
}