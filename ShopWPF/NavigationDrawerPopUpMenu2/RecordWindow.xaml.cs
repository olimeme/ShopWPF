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
using Excel = Microsoft.Office.Interop.Excel;
using Microsoft.Office.Interop.Excel;
using iTextSharp.text.pdf;
using iTextSharp.text;
using System.IO;

namespace NavigationDrawerPopUpMenu2
{
    /// <summary>
    /// Логика взаимодействия для RecordWindow.xaml
    /// </summary>
    public partial class RecordWindow : System.Windows.Window
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

        private void ExportToExcel(object sender, RoutedEventArgs e)
        {
            Excel.Application excel = new Excel.Application();
            excel.Visible = true;
            Workbook workbook = excel.Workbooks.Add(System.Reflection.Missing.Value);
            Worksheet sheet1 = (Worksheet)workbook.Sheets[1];

            for (int j = 0; j < grid.Columns.Count; j++)
            {
                Range myRange = (Range)sheet1.Cells[1, j + 1];
                myRange.Value2 = grid.Columns[j].Header;
            }
            for (int i = 0; i < grid.Columns.Count; i++)
            { 
                for (int j = 0; j < grid.Items.Count-1; j++)
                {
                    var rows = grid.Items.SourceCollection as List<SellingRecord>;                    
                    Microsoft.Office.Interop.Excel.Range myRange = (Microsoft.Office.Interop.Excel.Range)sheet1.Cells[j + 2, i + 1];
                    if(i == 0)myRange.Value2 = rows[j].CountOfProduct;
                    else if (i == 1) myRange.Value2 = rows[j].NameOfProduct;
                }
            }
        }

        private void ExportToPdf(object sender, RoutedEventArgs e)
        {
            Document document = new Document();
            var date = DateTime.Now.Day.ToString() + "." + DateTime.Now.Month.ToString() + "." + DateTime.Now.Year.ToString() + "(" + DateTime.Now.Hour + ";" + DateTime.Now.Minute + ";" + DateTime.Now.Second+")";
            PdfWriter writer = PdfWriter.GetInstance(document, new FileStream("e://Отчет" + date + ".pdf", FileMode.Create));
            document.Open();
            string ttf = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts), "ARIAL.TTF");
            var baseFont = BaseFont.CreateFont(ttf, BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);
            var font = new iTextSharp.text.Font(baseFont, iTextSharp.text.Font.DEFAULTSIZE, iTextSharp.text.Font.NORMAL);
            PdfPTable table;
            try
            {
                table = new PdfPTable(grid.Columns.Count);
            }
            catch(ArgumentException)
            {
                MessageBox.Show("Сначала выведите таблицу");
                return;
            }
            PdfPRow row = null;
            float[] widths = new float[grid.Columns.Count];
            for (int i = 0; i < grid.Columns.Count; i++)
            {
                widths[i] = 4f;
            }

            table.SetWidths(widths);

            table.WidthPercentage = 100;
            PdfPCell cell = new PdfPCell(new Phrase("Products"));

            cell.Colspan = grid.Columns.Count;

            foreach (DataGridColumn c in grid.Columns)
            {

                table.AddCell(new Phrase(c.Header.ToString(), font));
            }

            foreach (var r in grid.Items.SourceCollection as List<SellingRecord>)
            {
                if (grid.Items.Count > 0)
                {
                    table.AddCell(new Phrase(r.CountOfProduct.ToString(), font));
                    table.AddCell(new Phrase(r.NameOfProduct.ToString(), font));
                }
            }
            document.Add(table);
            document.Close();
        }
    }
}