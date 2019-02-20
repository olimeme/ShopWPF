using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace NavigationDrawerPopUpMenu2
{
    /// <summary>
    /// Логика взаимодействия для RecordWindow.xaml
    /// </summary>
    public partial class RecordWindow : Window
    {
        public RecordWindow()
        {
            InitializeComponent();
        }

        private List<Purchase> GetListOfPurchasesByDate(DateTime? begin, DateTime? end, ShopContext context)
        {
            return context.Purchases.Where(purchase => purchase.TimeOfPurchase <= end && purchase.TimeOfPurchase >= begin).ToList();
        }

        private void ShowTable(object sender, RoutedEventArgs e)
        {
            if (datePicker1.SelectedDate == null || datePicker2.SelectedDate == null)
            {
                MessageBox.Show("Пожалуйста, укажите время для отчета!");
                return;
            }
            using (var context = new ShopContext())
            {

                var purchases = GetListOfPurchasesByDate(datePicker1.SelectedDate, datePicker2.SelectedDate, context);
                List<SellingRecord> rows = new List<SellingRecord>(purchases.Count);
                List<Product> allProducts = new List<Product>();

                foreach (var purchase in purchases)
                {
                    foreach (var product in purchase.PurchasedProducts)
                    {
                        allProducts.Add(product);
                    }
                }

                var products = context.Products.ToList();
                for (int i = 0; i < products.Count(); i++)
                {
                    int count = 0;

                    foreach (var product in allProducts)
                    {
                        if (product.Name == products[i].Name)
                        {
                            count += product.Count;
                        }
                    }
                    rows.Add(new SellingRecord()
                    {
                        CountOfProduct = count,
                        NameOfProduct = products[i].Name
                    });

                }

                grid.ItemsSource = rows;

            }
        }
    }
}