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
        public virtual ICollection<Purchase> Purchases { get; set; }
        public int Count { get; set; } = 1;
        public int TotalPrice { get { return Price * Count; } set { } }
        public string SorucePath
        {
            get
            {
                switch (Category)
                {
                    case Category.Baking:
                        return "croissant.png";
                    case Category.Dairy:
                        return "dairy.png";
                    case Category.Fruits:
                        return "orange.png";
                    case Category.Meat:
                        return "steak.png";
                    case Category.Snacks:
                        return "sandwich.png";
                    case Category.Sweet:
                        return "toffee.png";
                    case Category.Vegetables:
                        return "carrot.png";
                    default: return "";
                }
            }
            set { }
        }
    }
}
