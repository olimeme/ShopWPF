using System;
using System.Collections.Generic;

namespace NavigationDrawerPopUpMenu2
{
    public class Purchase
    {
        public int Id { get; set; }
        public DateTime TimeOfPurchase { get; set; }
        public virtual ICollection<Product> PurchasedProducts { get; set; }
    }
}
