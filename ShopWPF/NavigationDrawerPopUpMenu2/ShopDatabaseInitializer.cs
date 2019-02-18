using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NavigationDrawerPopUpMenu2
{
   public class ShopDatabaseInitializer : CreateDatabaseIfNotExists<ShopContext>
    {
        protected override void Seed(ShopContext context)
        {
            context.Clients.AddRange(new List<Client>()
            {
                new Client()
                {
                    Login = "XcenaX",
                    Name = "Vlad Lebedev",
                    Password = "Dagad582#",
                    Phone = "+7011242693",
                    Cart= new Cart(),
                    Rights = Rights.Admin
                },
                new Client()
                {
                    Login = "Olimeme",
                    Name = "Alibek Mombekov",
                    Password = "123",
                    Phone = "+77011007185",
                    Cart= new Cart(),
                    Rights = Rights.Admin
                }               
            });
            context.Products.AddRange(new List<Product>()
            {
                new Product()
                {
                     Category = Category.Baking,
                     Name = "Хлеб(нарезнойб черный, с семечками и кунжутом)",
                     Price = 200,                     
                },
                new Product()
                {
                     Category = Category.Baking,
                     Name = "Батон",
                     Price = 150,
                },
                new Product()
                {
                     Category = Category.Baking,
                     Name = "Беляши(шт)",
                     Price = 300,
                },
                new Product()
                {
                     Category = Category.Dairy,
                     Name = "Молоко 'Айналайын'",
                     Price = 350,
                },
                new Product()
                {
                     Category = Category.Dairy,
                     Name = "Йогурт 'Danon'",
                     Price = 110,
                },
                new Product()
                {
                     Category = Category.Fruits,
                     Name = "Яблоки(кг)",
                     Price = 250,
                },
                new Product()
                {
                     Category = Category.Fruits,
                     Name = "Груши(кг)",
                     Price = 200,
                },
                new Product()
                {
                     Category = Category.Meat,
                     Name = "Свинина(кг)",
                     Price = 2200,
                },
                new Product()
                {
                     Category = Category.Meat,
                     Name = "Баранина(кг)",
                     Price = 1900,
                },
                new Product()
                {
                     Category = Category.Snacks,
                     Name = "Чипсы Lays(в 2 раза больше воздуха)",
                     Price = 310,
                },
                new Product()
                {
                     Category = Category.Snacks,
                     Name = "Кириешки 'ХрусTeam'",
                     Price = 140,
                },
                new Product()
                {
                     Category = Category.Sweet,
                     Name = "Конфеты 'Арабская Ночь'(кг)",
                     Price = 1750,
                },
                new Product()
                {
                     Category = Category.Sweet,
                     Name = "Конфеты 'XXXDeluxe'",
                     Price = 2500,
                },
                new Product()
                {
                     Category = Category.Vegetables,
                     Name = "Картошка от Ашота(кг)",
                     Price = 556,
                }
            });
            base.Seed(context);
        }

    }
}
