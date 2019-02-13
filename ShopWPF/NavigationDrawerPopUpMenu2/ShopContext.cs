namespace NavigationDrawerPopUpMenu2
{
    using System;
    using System.Data.Entity;
    using System.Linq;

    public class ShopContext : DbContext
    {
        public ShopContext()
            : base("name=ShopContext")
        {
            Database.SetInitializer(new ShopDatabaseInitializer());// базовые значения
        }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<Client> Clients { get; set; }
        public DbSet<Product> Products { get; set; }
    }
}