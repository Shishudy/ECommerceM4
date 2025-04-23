using System;
using System.Collections.Generic;

namespace StoreLibrary.DbModels;

public partial class Product
{
    public int PkProduct { get; set; }

    public int FkImage { get; set; }

    public string Ean { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string Description { get; set; } = null!;

    public double Price { get; set; }

    public int Stock { get; set; }

    public bool Toggle { get; set; }

    public virtual ICollection<CartProdut> CartProduts { get; set; } = new List<CartProdut>();

    public virtual ICollection<Favourite> Favourites { get; set; } = new List<Favourite>();

    public virtual Image FkImageNavigation { get; set; } = null!;

    public virtual ICollection<PurchaseProduct> PurchaseProducts { get; set; } = new List<PurchaseProduct>();

    public virtual ICollection<Campaign> FkCampaigns { get; set; } = new List<Campaign>();

    public virtual ICollection<Category> FkCategories { get; set; } = new List<Category>();
}
