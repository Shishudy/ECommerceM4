using System;
using System.Collections.Generic;

namespace StoreLibrary.DbModels;

public partial class Cart
{
    public int PkCart { get; set; }

    public int? FkUser { get; set; }

    public int? FkPurchase { get; set; }

    public virtual ICollection<CartProdut> CartProduts { get; set; } = new List<CartProdut>();

    public virtual Purchase? FkPurchaseNavigation { get; set; }
}
