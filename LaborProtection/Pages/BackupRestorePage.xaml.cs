using System.Windows.Controls;
using System.Windows;
using Microsoft.Win32;
using System.Diagnostics;

namespace LaborProtection.Pages
{
    public partial class BackupRestorePage : Page
    {
        private string _dbName = "labor";
        private string _dbUser = "postgres";
        private string _dbHost = "localhost";
        private string _dbPort = "5432";
        private string _dbPassword = "797827";
        private string _pgDumpPath = @"C:\Program Files\PostgreSQL\15\bin\pg_dump.exe";
        private string _pgRestorePath = @"C:\Program Files\PostgreSQL\15\bin\pg_restore.exe";

        public BackupRestorePage()
        {
            InitializeComponent();
        }

        private void BrowseBackupPath_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new SaveFileDialog { Filter = "SQL файлы|*.sql", FileName = "labor_backup.sql" };
            if (dialog.ShowDialog() == true)
                BackupPathBox.Text = dialog.FileName;
        }

        private void BrowseRestorePath_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog { Filter = "SQL файлы|*.sql" };
            if (dialog.ShowDialog() == true)
                RestorePathBox.Text = dialog.FileName;
        }

        private void BackupButton_Click(object sender, RoutedEventArgs e)
        {
            BackupStatusText.Text = string.Empty;
            if (string.IsNullOrWhiteSpace(BackupPathBox.Text))
            {
                BackupStatusText.Text = "Укажите путь для копии.";
                return;
            }
            try
            {
                var psi = new ProcessStartInfo
                {
                    FileName = _pgDumpPath,
                    Arguments = $"-h {_dbHost} -p {_dbPort} -U {_dbUser} -F c -b -v -f \"{BackupPathBox.Text}\" {_dbName}",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    EnvironmentVariables = { ["PGPASSWORD"] = _dbPassword }
                };
                var process = Process.Start(psi);
                process.WaitForExit();
                if (process.ExitCode == 0)
                    BackupStatusText.Text = "Резервная копия успешно создана.";
                else
                    BackupStatusText.Text = "Ошибка при создании копии: " + process.StandardError.ReadToEnd();
            }
            catch (System.Exception ex)
            {
                BackupStatusText.Text = "Ошибка: " + ex.Message;
            }
        }

        private void RestoreButton_Click(object sender, RoutedEventArgs e)
        {
            RestoreStatusText.Text = string.Empty;
            if (string.IsNullOrWhiteSpace(RestorePathBox.Text))
            {
                RestoreStatusText.Text = "Укажите файл для восстановления.";
                return;
            }
            try
            {
                var psi = new ProcessStartInfo
                {
                    FileName = _pgRestorePath,
                    Arguments = $"-h {_dbHost} -p {_dbPort} -U {_dbUser} -d {_dbName} -c \"{RestorePathBox.Text}\"",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    EnvironmentVariables = { ["PGPASSWORD"] = _dbPassword }
                };
                var process = Process.Start(psi);
                process.WaitForExit();
                if (process.ExitCode == 0)
                    RestoreStatusText.Text = "Восстановление успешно завершено.";
                else
                    RestoreStatusText.Text = "Ошибка при восстановлении: " + process.StandardError.ReadToEnd();
            }
            catch (System.Exception ex)
            {
                RestoreStatusText.Text = "Ошибка: " + ex.Message;
            }
        }
    }
} 