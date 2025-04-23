using System;
using System.Collections.Generic;

namespace StoreLibrary.DbModels;

public partial class Favourite
{
    public int FkProduct { get; set; }

    public int FkUser { get; set; }

    public virtual Product FkProductNavigation { get; set; } = null!;
}
