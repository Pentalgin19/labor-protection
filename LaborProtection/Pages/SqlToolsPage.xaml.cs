using System.Windows.Controls;
using System;
using Npgsql;

namespace LaborProtection.Pages
{
    public partial class SqlToolsPage : Page
    {
        private string _connStr = "Host=localhost;Port=5432;Database=labor;Username=postgres;Password=797827";
        public SqlToolsPage()
        {
            InitializeComponent();
        }

        private void AddExamResultProc_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            AddExamResultProcText.Text = string.Empty;
            try
            {
                using (var conn = new NpgsqlConnection(_connStr))
                {
                    conn.Open();
                    using (var cmd = new NpgsqlCommand("CALL add_exam_result(@exam_id, @result)", conn))
                    {
                        cmd.Parameters.AddWithValue("exam_id", int.Parse(ExamIdBox.Text));
                        cmd.Parameters.AddWithValue("result", int.Parse(ResultValueBox.Text));
                        cmd.ExecuteNonQuery();
                        AddExamResultProcText.Text = "Результат добавлен!";
                    }
                }
                // Обновляем список экзаменов, если страница ExamsPage открыта
                var mainWindow = System.Windows.Application.Current.MainWindow as MainWindow;
                if (mainWindow != null && mainWindow.MainFrame.Content is LaborProtection.Pages.ExamsPage examsPage)
                {
                    examsPage.LoadExams();
                }
            }
            catch (Exception ex)
            {
                AddExamResultProcText.Text = "Ошибка: " + ex.Message;
            }
        }

        private void ShowEmployeesWithoutExamsProc_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            NoExamEmployeesProcList.Items.Clear();
            using (var conn = new NpgsqlConnection(_connStr))
            {
                conn.Open();
                using (var tran = conn.BeginTransaction())
                {
                    using (var cmd = new NpgsqlCommand("CALL get_employees_without_exams_proc('mycursor')", conn, tran))
                        cmd.ExecuteNonQuery();

                    using (var cmd = new NpgsqlCommand("FETCH ALL FROM mycursor", conn, tran))
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.HasRows && reader.FieldCount > 0)
                        {
                            while (reader.Read())
                            {
                                NoExamEmployeesProcList.Items.Add($"{reader[0]}: {reader[1]} {reader[2]} ({reader[3]})");
                            }
                        }
                        else
                        {
                            NoExamEmployeesProcList.Items.Add("Нет данных");
                        }
                    }
                    tran.Commit();
                }
            }
        }

        //private void TestDefaultEmploymentDateTrigger_Click(object sender, System.Windows.RoutedEventArgs e)
        //{
        //    TestDefaultEmploymentDateResult.Text = string.Empty;
        //    try
        //    {
        //        using (var conn = new NpgsqlConnection(_connStr))
        //        {
        //            conn.Open();
        //            using (var cmd = new NpgsqlCommand("INSERT INTO employees (surname, name, patronymic, phone, email, post, department) VALUES ('Тест', 'Тест', 'Тест', '000', 'test@example.com', 1, 1) RETURNING id, date_of_employment", conn))
        //            {
        //                using (var reader = cmd.ExecuteReader())
        //                {
        //                    if (reader.Read())
        //                    {
        //                        var id = reader.GetInt32(0);
        //                        var date = reader.GetDateTime(1);
        //                        TestDefaultEmploymentDateResult.Text = $"Сотрудник добавлен (id={id}), дата приёма: {date:yyyy-MM-dd}";
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        TestDefaultEmploymentDateResult.Text = "Ошибка: " + ex.Message;
        //    }
        //}

        //private void TestPreventDeleteEmployeeWithExamsTrigger_Click(object sender, System.Windows.RoutedEventArgs e)
        //{
        //    TestPreventDeleteEmployeeResult.Text = string.Empty;
        //    try
        //    {
        //        using (var conn = new NpgsqlConnection(_connStr))
        //        {
        //            conn.Open();
        //            int empId = -1;
        //            using (var cmd = new NpgsqlCommand("SELECT e.id FROM employees e JOIN exams ex ON e.id = ex.id_emp LIMIT 1", conn))
        //            using (var reader = cmd.ExecuteReader())
        //            {
        //                if (reader.Read()) empId = reader.GetInt32(0);
        //            }
        //            if (empId == -1)
        //            {
        //                TestPreventDeleteEmployeeResult.Text = "Нет сотрудника с проверками для теста.";
        //                return;
        //            }
        //            using (var cmd = new NpgsqlCommand($"DELETE FROM employees WHERE id = {empId}", conn))
        //            {
        //                try
        //                {
        //                    cmd.ExecuteNonQuery();
        //                    TestPreventDeleteEmployeeResult.Text = "Удаление прошло (триггер не сработал, проверьте данные).";
        //                }
        //                catch (Exception exTrig)
        //                {
        //                    TestPreventDeleteEmployeeResult.Text = "Триггер сработал: " + exTrig.Message;
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        TestPreventDeleteEmployeeResult.Text = "Ошибка: " + ex.Message;
        //    }
       // }
    }
} 