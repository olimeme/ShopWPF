using System.Collections.Generic;

namespace NavigationDrawerPopUpMenu2
{
   public class Cart
    {
        public int Id { get; set; }                
        public virtual ICollection<Product> Products { get; set; }
    }
}
