using LaborProtection.Pages;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using LaborProtection.Entities;

namespace LaborProtection
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string _userName;
        private string _userSurname;
        private string _userRole;

        public MainWindow()
        {
            InitializeComponent();
            // profile.Text = Authorize.Name;
            var nameParts = (Authorize.Name ?? "").Split(' ');
            _userName = nameParts.Length > 0 ? nameParts[0] : "";
            _userSurname = nameParts.Length > 1 ? nameParts[1] : "";
            // Получаем название роли из справочника
            string roleTitle = "Пользователь";
            int roleId;
            if (int.TryParse(Authorize.Role, out roleId))
            {
                using (var context = new AppContext())
                {
                    var roleDict = context.dictionary.FirstOrDefault(d => d.category_id == 1 && d.id == roleId);
                    if (roleDict != null)
                        roleTitle = roleDict.short_text ?? roleDict.title;
                }
            }
            _userRole = roleTitle;
            UserInfoBlock.Text = $"{_userName} {_userSurname}\nРоль: {_userRole}";
        }

        private void OpenEmployees(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new EmployeesPage());
        }

        private void OpenExams(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new ExamsPage());
        }
        private void OpenReports(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new ReportsPage());
        }

        private void OpenSqlTools(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new SqlToolsPage());
        }

        private void OpenBackupRestore(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new BackupRestorePage());
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            // Открываем окно авторизации и закрываем главное окно
            var auth = new Authorize();
            auth.Show();
            this.Close();
        }
    }
}