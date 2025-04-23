using System;
using System.Collections.Generic;

namespace StoreLibrary.DbModels;

public partial class PurchaseProduct
{
    public int PkPurchase { get; set; }

    public int PkProduct { get; set; }

    public string Status { get; set; } = null!;

    public int Qtt { get; set; }

    public int? FkUser { get; set; }

    public int? FkReview { get; set; }

    public virtual Review? FkReviewNavigation { get; set; }

    public virtual Product PkProductNavigation { get; set; } = null!;

    public virtual Purchase PkPurchaseNavigation { get; set; } = null!;
}
