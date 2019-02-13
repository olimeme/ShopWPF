using PayPal.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Twilio;
using Twilio.Exceptions;
using Twilio.Rest.Api.V2010.Account;


namespace NavigationDrawerPopUpMenu2
{
    /// <summary>
    /// Interação lógica para MainWindow.xam
    /// </summary>
    public partial class MainWindow : Window
    {
        public string code = "";

        private string PasswordGenerate(int length)
        {
            Random _random = new Random(Environment.TickCount);

            string chars = "0123456789";
            StringBuilder builder = new StringBuilder(length);

            for (int i = 0; i < length; ++i)
                builder.Append(chars[_random.Next(chars.Length)]);

            return builder.ToString();
        }

        private void MessageSender(string number, string randNumber)
        {
            // Find your Account Sid and Token at twilio.com/console
            const string accountSid = "AC46d729f9bab280d217192471bb510f0e";
            const string authToken = "9a4d673663e8e6de021d0c5d8c0af348";

            TwilioClient.Init(accountSid, authToken);
            var message = MessageResource.Create(
            body: "Your confirmation code is " + randNumber,
            from: new Twilio.Types.PhoneNumber("+19413000318"),
            to: new Twilio.Types.PhoneNumber(number)
            );

            code = randNumber;
        }

        public MainWindow()
        {
            InitializeComponent();
            Logout.Visibility = Visibility.Collapsed;
        }

        private void ButtonOpenMenu_Click(object sender, RoutedEventArgs e)
        {
            ButtonCloseMenu.Visibility = Visibility.Visible;
            ButtonOpenMenu.Visibility = Visibility.Collapsed;
        }

        private void ButtonCloseMenu_Click(object sender, RoutedEventArgs e)
        {
            ButtonOpenMenu.Visibility = Visibility.Visible;
            ButtonCloseMenu.Visibility = Visibility.Collapsed;
        }

        private void OpenCart(object sender, RoutedEventArgs e)
        {
            Cart.Visibility = Visibility.Visible;
            ProductsList.Visibility = Visibility.Collapsed;
            RegistrateForm.Visibility = Visibility.Collapsed;
            LoginForm.Visibility = Visibility.Collapsed;


            if (LoginIfLogged.Text == "")
            {
                var bc = new BrushConverter();
                CartItems.Items.Clear();
                CartItems.Items.Add(new TextBlock()
                {
                    Text = "Вы еще не вошли в систему!\n Пожалуйста войдите!",
                    Margin = new Thickness(0, 50, 0, 0),
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    Foreground = (Brush)bc.ConvertFrom("#EBB129"),
                    FontSize = 30,
                    FontWeight = FontWeights.Bold,
                    Height = 100,
                    TextAlignment = TextAlignment.Center

                });
                buyButton.Visibility = Visibility.Collapsed;
            }
            else
            {
                using (var context = new ShopContext())
                {
                    var currentCart = context.Clients.Where(client => client.Id.ToString() == CurrentClientId.Text.ToString()).ToList()[0].Cart;

                    if (currentCart.Products.Count() == 0)
                    {
                        var bc = new BrushConverter();
                        CartItems.Items.Clear();
                        CartItems.Items.Add(new TextBlock()
                        {
                            Text = "Корзина пуста!",
                            Margin = new Thickness(0, 50, 0, 0),
                            HorizontalAlignment = HorizontalAlignment.Stretch,
                            Foreground = (Brush)bc.ConvertFrom("#EBB129"),
                            FontSize = 30,
                            FontWeight = FontWeights.Bold,
                            Height = 100,
                            TextAlignment = TextAlignment.Center,
                            Name = "EmptyText"
                        });
                        buyButton.Visibility = Visibility.Collapsed;
                    }
                    else
                    {
                        foreach (var child in CartItems.Items)
                        {

                            if (child.GetType() == typeof(TextBlock))
                            {
                                if ((child as TextBlock).Name == "EmptyText")
                                {
                                    CartItems.Items.Remove(child);
                                    break;
                                }
                            }
                        }
                        if (currentCart.Products.Count > 0) buyButton.Visibility = Visibility.Visible;
                        CartItems.Items.Clear();
                        foreach (var product in currentCart.Products)
                        {
                            var bc = new BrushConverter();
                            var stackPanel = new StackPanel()
                            {
                                Height = 90,
                                Width = CartItems.Width,
                                Background = (Brush)bc.ConvertFrom("#3C464F"),
                                Orientation = Orientation.Horizontal
                            };
                            var image = new System.Windows.Controls.Image()
                            {
                                Width = 70,
                                Height = 70,
                                Source = new BitmapImage(new Uri(System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + "/carrot.png")),
                                HorizontalAlignment = HorizontalAlignment.Left,
                                Margin = new Thickness(10, 10, 0, 0)

                            };
                            var productDescription = new TextBlock()
                            {
                                Text = product.Price + " тенге\n" + product.Name,
                                FontSize = 20,
                                Foreground = (Brush)bc.ConvertFrom("#EBB129"),
                                TextAlignment = TextAlignment.Center,
                                HorizontalAlignment = HorizontalAlignment.Center,
                                Margin = new Thickness(50, 10, 0, 0),
                                Width = 290,
                                Height = 90
                            };
                            var addToCartButton = new Button()
                            {
                                Content = "Удалить",
                                Height = 40,
                                Width = 90,
                                Margin = new Thickness(250, 10, 0, 0),
                                HorizontalAlignment = HorizontalAlignment.Right,
                                Foreground = (Brush)bc.ConvertFrom("#3C464F"),
                                Background = (Brush)bc.ConvertFrom("#EBB129"),
                                Name = "Button" + product.Id.ToString()
                            };
                            addToCartButton.Click += DeleteFromCart;
                            stackPanel.Children.Add(image);
                            stackPanel.Children.Add(productDescription);
                            stackPanel.Children.Add(addToCartButton);
                            CartItems.Items.Add(stackPanel);
                            //GridMain.Children.Add(CartItems);
                        }
                    }
                }
            }
        }

        private void ListViewMenu_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UserControl usc = null;
            GridMain.Children.Clear();

            switch (((ListViewItem)((ListView)sender).SelectedItem).Name)
            {
                case "ItemHome":
                    usc = new UserControlHome();
                    GridMain.Children.Add(usc);
                    break;
                case "ItemCreate":
                    usc = new UserControlCreate();
                    GridMain.Children.Add(usc);
                    break;
                default:
                    break;
            }
        }

        private void CloseLoginPanel(object sender, RoutedEventArgs e)
        {
            LoginForm.Visibility = Visibility.Collapsed;
        }

        private void OpenLoginPanel(object sender, RoutedEventArgs e)
        {
            Cart.Visibility = Visibility.Collapsed;
            RegistrateForm.Visibility = Visibility.Collapsed;
            ProductsList.Visibility = Visibility.Collapsed;
            LoginForm.Visibility = Visibility.Visible;
            if (RegistrateForm.Visibility == Visibility.Visible) RegistrateForm.Visibility = Visibility.Collapsed;
        }

        private void CloseRegistratePanel(object sender, RoutedEventArgs e)
        {
            RegistrateForm.Visibility = Visibility.Collapsed;
        }

        private void OpenRegistratePanel(object sender, RoutedEventArgs e)
        {
            Cart.Visibility = Visibility.Collapsed;
            ProductsList.Visibility = Visibility.Collapsed;
            LoginForm.Visibility = Visibility.Collapsed;
            RegistrateForm.Visibility = Visibility.Visible;
            if (LoginForm.Visibility == Visibility.Visible) LoginForm.Visibility = Visibility.Collapsed;
        }

        private void LoginUser(object sender, RoutedEventArgs e)
        {
            using (var context = new ShopContext())
            {
                var users = context.Clients.Where(client => client.Login == UserLogin.Text.ToString() && client.Password == UserPassword.Password.ToString()).ToList();
                //if (users.Count() > 1)
                //{
                //    MessageBox.Show("Пользователь с таким Логином и Паролем уже зарегистрирован!");
                //}
                //else
                if (!users.Any())
                {
                    MessageBox.Show("Направильно введен логин или пароль");
                }
                else
                {
                    LoginIfLogged.Text = users[0].Name;
                    CloseLoginPanel(sender, e);
                    Registrate.Visibility = Visibility.Collapsed;
                    Login.Visibility = Visibility.Collapsed;
                    Logout.Visibility = Visibility.Visible;
                    UserLogin.Text = "";
                    UserPassword.Password = "";
                    CurrentClientId.Text = users[0].Id.ToString();
                }
            }
        }

        private void LogOutUser(object sender, RoutedEventArgs e)
        {
            LoginIfLogged.Text = "";
            Registrate.Visibility = Visibility.Visible;
            Login.Visibility = Visibility.Visible;
            Logout.Visibility = Visibility.Collapsed;
            UserLogin.Text = "";
            UserPassword.Password = "";
            CurrentClientId.Text = "-1";
        }

        private void RegistrateUser(object sender, RoutedEventArgs e)
        {
            using (var context = new ShopContext())
            {
                var users = context.Clients.Where(client => client.Login == ClientLoginRegistrate.Text.ToString());
                if (users.Count() > 0)
                {
                    MessageBox.Show("Пользователь с такими логином уже зарегестрирован!");
                    return;
                }
                else if (ClientFullNameRegistrate.Text.ToString() == "")
                {
                    MessageBox.Show("Не заполнено поле ФИО");
                    return;
                }
                else if (ClientLoginRegistrate.Text.ToString() == "")
                {
                    MessageBox.Show("Не заполнено поле Логин");
                    return;
                }
                else if ((ClientPasswordRegistrate.Password.ToString() != ClientPasswordConfirm.Password.ToString()) || (ClientPasswordRegistrate.Password.ToString() == ""))
                {
                    MessageBox.Show("Пароли не совпадают");
                    return;
                }
                else if (ClientNumberRegistrate.Text.ToString() == "" || Regex.IsMatch(ClientNumberRegistrate.Text.ToString(), ".*?[a-zA-Z].*?"))
                {
                    MessageBox.Show("Неправильно заполнено поле Номер телефона");
                    return;
                }
                else
                {
                    var clients = context.Set<Client>();
                    clients.Add(new Client
                    {
                        Name = ClientFullNameRegistrate.Text.ToString(),
                        Login = ClientLoginRegistrate.Text.ToString(),
                        Password = ClientPasswordRegistrate.Password.ToString(),
                        Phone = ClientNumberRegistrate.Text.ToString(),
                    });
                    try
                    {

                        var number = PasswordGenerate(4);
                        MessageSender(ClientNumberRegistrate.Text.ToString(), number);
                    }
                    catch (TwilioException error)
                    {
                        MessageBox.Show("Неправильный формат строки ");
                        return;
                    }

                    CloseRegistratePanel(sender, e);

                    RegistrateFormConfirm.Visibility = Visibility.Visible;
                }
            }
        }

        private void Withdraw(object sender, RoutedEventArgs e)
        {

            var config = PayPal.Api.ConfigManager.Instance.GetProperties();
            var accessToken = new PayPal.Api.OAuthTokenCredential(config).GetAccessToken();
            var apiContext = new PayPal.Api.APIContext(accessToken);

            var payout = new Payout
            {
                sender_batch_header = new PayoutSenderBatchHeader
                {
                    sender_batch_id = "batch_" + Guid.NewGuid().ToString().Substring(0, 8),
                    email_subject = "You have payment",
                    recipient_type = PayoutRecipientType.EMAIL
                },

                items = new List<PayoutItem>
            {
                new PayoutItem
                {
                    recipient_type = PayoutRecipientType.EMAIL,
                    amount = new Currency { value="1.5", currency="USD" },
                    receiver = "vlad-057-buyer@mail.ru",
                    note="Thank you",
                    sender_item_id = "item_1"
                }
            }
            };

            var created = payout.Create(apiContext, false);
        }

        private void RegisterUserConfirm(object sender, RoutedEventArgs e)
        {
            while (true)
            {
                using (var context = new ShopContext())
                {
                    if (ClientRegisterConfirm.Text.ToString() == code)
                    {
                        var clients = context.Set<Client>();
                        clients.Add(new Client
                        {
                            Name = ClientFullNameRegistrate.Text.ToString(),
                            Login = ClientLoginRegistrate.Text.ToString(),
                            Password = ClientPasswordRegistrate.Password.ToString(),
                            Phone = ClientNumberRegistrate.Text.ToString()
                        });

                        context.SaveChanges();

                        ClientNumberRegistrate.Text = "+";
                        ClientLoginRegistrate.Text = "";
                        ClientPasswordRegistrate.Password = "";
                        ClientPasswordConfirm.Password = "";
                        ClientFullNameRegistrate.Text = "";

                        RegistrateFormConfirm.Visibility = Visibility.Collapsed;
                        CurrentClientId.Text = clients.ToList()[clients.Count() - 1].Id.ToString();
                        break;
                    }
                    else
                        continue;
                }
            }
        }

        private void SendCodeMessage(object sender, RoutedEventArgs e)
        {
            var number = PasswordGenerate(4);
            MessageSender(ClientNumberRegistrate.Text.ToString(), number);
        }

        private void ShowGoodsByCategory(object sender, RoutedEventArgs e)
        {
            ProductItems.Items.Clear();
            Category category = 0;
            switch ((sender as ListViewItem).Name)
            {
                case "Meat":
                    category = Category.Meat;
                    break;
                case "Fruits":
                    category = Category.Fruits;
                    break;
                case "Vegetables":
                    category = Category.Vegetables;
                    break;
                case "DairyProducts":
                    category = Category.Dairy;
                    break;
                case "Sweet":
                    category = Category.Sweet;
                    break;
                case "Baking":
                    category = Category.Baking;
                    break;
                case "Snacks":
                    category = Category.Snacks;
                    break;
                default:
                    break;
            }
            ProductsList.Visibility = Visibility.Visible;
            Cart.Visibility = Visibility.Collapsed;
            LoginForm.Visibility = Visibility.Collapsed;
            RegistrateForm.Visibility = Visibility.Collapsed;
            using (var context = new ShopContext())
            {
                var products = context.Products.Where(product => product.Category == category).ToList();
                foreach (var product in products)
                {

                    var bc = new BrushConverter();
                    var image = new System.Windows.Controls.Image()
                    {
                        Width = 70,
                        Height = 70,
                        Source = new BitmapImage(new Uri(System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + "/carrot.png")),
                        HorizontalAlignment = HorizontalAlignment.Left,
                        Margin = new Thickness(30, 30, 0, 0)
                    };
                    var productDescription = new TextBlock()
                    {
                        Text = product.Name,
                        FontSize = 20,
                        Foreground = (Brush)bc.ConvertFrom("#EBB129"),
                        Width = 500,
                        Height = 90,
                        Margin = new Thickness(-200, -50, 0, 0)

                    };
                    var productPrice = new TextBlock()
                    {
                        Text = product.Price + " тг",
                        FontSize = 24,
                        Foreground = (Brush)bc.ConvertFrom("#EBB129"),
                        Width = 500,
                        Height = 90,
                        FontWeight = FontWeights.Bold,
                        Margin = new Thickness(-200, -70, 0, 0)

                    };

                    ProductItems.Items.Add(image);
                    ProductItems.Items.Add(productDescription);
                    ProductItems.Items.Add(productPrice);
                    if (CurrentClientId.Text != "-1")
                    {
                        var addToCartButton = new Button()
                        {
                            Content = "Добавить в корзину",
                            Height = 40,
                            Width = 170,
                            HorizontalAlignment = HorizontalAlignment.Right,
                            Foreground = (Brush)bc.ConvertFrom("#3C464F"),
                            Background = (Brush)bc.ConvertFrom("#EBB129"),
                            Name = "Button" + product.Id.ToString(),
                            Margin = new Thickness(0, -100, 30, 0)
                        };
                        addToCartButton.Click += AddToCart;
                        ProductItems.Items.Add(addToCartButton);
                    }
                }
            }
        }

        private void AddToCart(object sender, RoutedEventArgs e)
        {
            int currentProductId = int.Parse(Regex.Replace((sender as Button).Name.ToString(), "[^0-9.]", ""));
            int clientId = int.Parse(CurrentClientId.Text);
            using (var context = new ShopContext())
            {
                var currentClient = context.Clients.Where(client => client.Id == clientId).ToList()[0];
                var currentCart = currentClient.Cart;
                var currentProduct = context.Products.Where(product => product.Id == currentProductId).ToList()[0];
                currentCart.Products.Add(currentProduct);
                context.SaveChanges();
            }
        }

        private void DeleteFromCart(object sender, RoutedEventArgs e)
        {
            int currentProductId = int.Parse(Regex.Replace((sender as Button).Name.ToString(), "[^0-9.]", ""));
            using (var context = new ShopContext())
            {
                int currentId = int.Parse(CurrentClientId.Text);
                var currentCart = context.Clients.Where(client => client.Id == currentId).ToList()[0].Cart;

                int i = 0;
                foreach (var product in currentCart.Products)
                {
                    if (product.Id == currentProductId)
                    {
                        currentCart.Products.Remove(product);
                        CartItems.Items.RemoveAt(i);
                        if (CartItems.Items.Count == 0)
                        {
                            buyButton.Visibility = Visibility.Collapsed;
                        }
                        break;
                    }
                    i++;
                }

                context.SaveChanges();
                OpenCart(sender, e);
            }
        }

        private void CloseWindow(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void HideWindow(object sender, RoutedEventArgs e)
        {
            if (WindowState == WindowState.Normal)
                WindowState = WindowState.Minimized;
            else if (WindowState == WindowState.Minimized)
                WindowState = WindowState.Normal;

        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }
    }
}
