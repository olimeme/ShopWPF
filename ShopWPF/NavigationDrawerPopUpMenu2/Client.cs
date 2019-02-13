using System.Collections.Generic;

namespace NavigationDrawerPopUpMenu2
{
    public class Client
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Login {get;set;}
        public string Password { get; set; }
        public virtual Cart Cart { get; set; }

        public Client()
        {
            Cart = new Cart()
            {
                Products = new List<Product>()
            };
        }
    }
}
