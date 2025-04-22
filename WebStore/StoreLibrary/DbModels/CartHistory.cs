using System;
using System.Collections.Generic;

namespace StoreLibrary.DbModels;

public partial class CartHistory
{
    public int PkCart { get; set; }

    public int? FkUser { get; set; }

    public int? FkPurchase { get; set; }

    public int? FkReview { get; set; }

    public virtual ICollection<CartProdut> CartProduts { get; set; } = new List<CartProdut>();

    public virtual Purchase? FkPurchaseNavigation { get; set; }

    public virtual Review? FkReviewNavigation { get; set; }
}
