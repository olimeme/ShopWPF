using System.Collections.Generic;

namespace NavigationDrawerPopUpMenu2
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Category Category { get; set; }
        public int Price { get; set; }
        public virtual ICollection<Cart> Carts { get; set; }
        public string ImageSource { get; set; }
    }
}