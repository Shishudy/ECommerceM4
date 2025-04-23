using System;
using System.Collections.Generic;

namespace StoreLibrary.DbModels;

public partial class CartProdut
{
    public int FkCart { get; set; }

    public int FkEan { get; set; }

    public virtual CartHistory FkCart1 { get; set; } = null!;

    public virtual Cart FkCartNavigation { get; set; } = null!;

    public virtual Product FkEanNavigation { get; set; } = null!;
}
