using System;
using System.Collections.Generic;

namespace StoreLibrary.DbModels;

public partial class UserImage
{
    public int FkImage { get; set; }

    public int FkUser { get; set; }

    public virtual Image FkImageNavigation { get; set; } = null!;
}
