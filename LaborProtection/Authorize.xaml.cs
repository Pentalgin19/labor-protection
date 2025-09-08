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

namespace LaborProtection
{
    /// <summary>
    /// Логика взаимодействия для Authorize.xaml
    /// </summary>
    public partial class Authorize : Window
    {
        public static string Name;
        public static string Role;
        public Authorize()
        {
            //<!--DEC8A3 D2B48C-->
            InitializeComponent();
        }

        private void InputChanged(object sender, RoutedEventArgs e)
        {
            string login = loginBox.Text;
            string password = passwordBox.Password;

            bool bothFilled = !string.IsNullOrWhiteSpace(login) && !string.IsNullOrWhiteSpace(password);

            loginButton.IsEnabled = bothFilled;
            loginButton.Background = bothFilled ? new SolidColorBrush((Color)ColorConverter.ConvertFromString("#C89C6A")) : // A67C52 C89C6A
                new SolidColorBrush((Color)ColorConverter.ConvertFromString("#DEC8A3"));
        }


        private void Button_Click(object sender, RoutedEventArgs? e)
        {
            using (AppContext context = new AppContext())
            {
                var user = context.users.Where(d => (d.login == loginBox.Text && d.password == passwordBox.Password)).FirstOrDefault();

                if (user != null)
                {
                    Name = $"{user.name} {user.surname}";
                    Role = user.role.ToString();
                    MainWindow main = new MainWindow();
                    main.Show();
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Неверный логин или пароль");
                }
            }
        }

        private void loginBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Button_Click(sender, e);
                e.Handled = true; // Предотвращает дальнейшую обработку Enter (например, переход на новую строку)
            }
        }

        private void passwordBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Button_Click(sender, e);
                e.Handled = true; // Предотвращает дальнейшую обработку Enter (например, переход на новую строку)
            }   
        }
    }
}
