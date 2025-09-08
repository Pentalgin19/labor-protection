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
using System.Windows.Navigation;
using System.Windows.Shapes;
using LaborProtection.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Win32;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System.IO;

namespace LaborProtection.Pages
{
    /// <summary>
    /// Логика взаимодействия для ExamsPage.xaml
    /// </summary>
    public partial class ExamsPage : Page
    {
        private AppContext context;
        private Exam _selectedExam;

        public ExamsPage()
        {
            InitializeComponent();
            context = new AppContext();
            LoadExams();
        }

        public void LoadExams()
        {
            var list = new List<Exam>();
            foreach (var ex in context.exams.ToList())
            {
                list.Add(new Exam(ex.id, ex.title, ex.id_emp, ex.result_mark));
            }
            ExamsDataGrid.ItemsSource = list;
        }
            //try
            //{
            //    var exams = _context.exams.ToList();
            //    ExamsDataGrid.ItemsSource = exams;
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show($"Ошибка при загрузке данных: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            //}
        

        private void AddExam_Click(object sender, RoutedEventArgs e)
        {
            var examWindow = new ExamWindow();
            if (examWindow.ShowDialog() == true)
            {
                try
                {
                    var newExam = examWindow.newExam;
                    if (newExam != null)
                    {
                        using (var context = new AppContext())
                        {
                            context.exams.Add(newExam);
                            context.SaveChanges();
                        }
                        LoadExams();
                        MessageBox.Show("Проверка успешно добавлена!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при добавлении проверки: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void EditExam_Click(object sender, RoutedEventArgs e)
        {
            var selectedExam = ExamsDataGrid.SelectedItem as Exam;
            if (selectedExam == null)
            {
                MessageBox.Show("Выберите проверку для редактирования", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            // Получаем актуальные данные из базы
            Exam examFromDb;
            using (var context = new AppContext())
            {
                examFromDb = context.exams.Find(selectedExam.id);
            }
            var examWindow = new ExamWindow(examFromDb);
            if (examWindow.ShowDialog() == true)
            {
                try
                {
                    using (var context = new AppContext())
                    {
                        var examToUpdate = context.exams.Find(selectedExam.id);
                        var updatedExam = examWindow.newExam;
                        if (examToUpdate != null && updatedExam != null)
                        {
                            examToUpdate.title = updatedExam.title;
                            examToUpdate.reason = updatedExam.reason;
                            examToUpdate.date = updatedExam.date;
                            examToUpdate.id_emp = updatedExam.id_emp;
                            examToUpdate.date_prev = updatedExam.date_prev;
                            examToUpdate.result_mark = updatedExam.result_mark;
                            examToUpdate.date_next = updatedExam.date_next;
                            examToUpdate.number = updatedExam.number;
                            context.SaveChanges();
                        }
                    }
                    LoadExams();
                    MessageBox.Show("Проверка успешно обновлена!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при обновлении проверки: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void DeleteExam_Click(object sender, RoutedEventArgs e)
        {
            var selectedExam = ExamsDataGrid.SelectedItem as Exam;
            if (selectedExam == null)
            {
                MessageBox.Show("Выберите проверку для удаления", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            var result = MessageBox.Show($"Вы уверены, что хотите удалить проверку '{selectedExam.title}'?", "Подтверждение удаления", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    using (var context = new AppContext())
                    {
                        var examToDelete = context.exams.Find(selectedExam.id);
                        if (examToDelete != null)
                        {
                            context.exams.Remove(examToDelete);
                            context.SaveChanges();
                        }
                    }
                    LoadExams();
                    MessageBox.Show("Проверка успешно удалена!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при удалении проверки: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void ExamsDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //_selectedExam = ExamsDataGrid.SelectedItem as Exam;
        }

        private void ImportExamExcelButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog open = new OpenFileDialog();
            if (open.ShowDialog() == true)
            {
                string filePath = open.FileName;
                ImportExamsFromExcel(filePath);
            }
        }

        private void ImportExamsFromExcel(string filePath)
        {
            List<Exam> examsToAdd = new List<Exam>();
            try
            {
                using (FileStream file = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                {
                    IWorkbook workbook = null;
                    if (filePath.ToLower().EndsWith(".xlsx"))
                        workbook = new XSSFWorkbook(file);
                    else if (filePath.ToLower().EndsWith(".xls"))
                        workbook = new HSSFWorkbook(file);
                    else
                        throw new Exception("Неподдерживаемый формат файла. Поддерживаются .xlsx и .xls");

                    ISheet sheet = workbook.GetSheetAt(0);
                    for (int rowNum = 1; rowNum <= sheet.LastRowNum; rowNum++) // rowNum = 1, т.к. 0 — заголовки
                    {
                        IRow row = sheet.GetRow(rowNum);
                        if (row == null) continue;
                        try
                        {
                            string title = row.GetCell(0)?.ToString() ?? "";
                            int reason = int.Parse(row.GetCell(1)?.ToString() ?? "0");
                            DateOnly date = DateOnly.Parse(row.GetCell(2)?.ToString() ?? DateOnly.FromDateTime(System.DateTime.Now).ToString());
                            int id_emp = int.Parse(row.GetCell(3)?.ToString() ?? "0");
                            DateOnly? date_prev = string.IsNullOrWhiteSpace(row.GetCell(4)?.ToString()) ? null : DateOnly.Parse(row.GetCell(4).ToString());
                            int? result_mark = string.IsNullOrWhiteSpace(row.GetCell(5)?.ToString()) ? null : int.Parse(row.GetCell(5).ToString());
                            DateOnly? date_next = string.IsNullOrWhiteSpace(row.GetCell(6)?.ToString()) ? null : DateOnly.Parse(row.GetCell(6).ToString());
                            string number = row.GetCell(7)?.ToString() ?? "";
                            var exam = new Exam {
                                title = title,
                                reason = reason,
                                date = date,
                                id_emp = id_emp,
                                date_prev = date_prev ?? default,
                                result_mark = result_mark ?? 0,
                                date_next = date_next ?? default,
                                number = number
                            };
                            examsToAdd.Add(exam);
                        }
                        catch (Exception exRow)
                        {
                            MessageBox.Show($"Ошибка в строке {rowNum + 1}: {exRow.Message}");
                        }
                    }
                }
                if (examsToAdd.Count > 0)
                {
                    using (AppContext context = new AppContext())
                    {
                        context.exams.AddRange(examsToAdd);
                        context.SaveChanges();
                    }
                    LoadExams();
                    MessageBox.Show($"Импортировано проверок: {examsToAdd.Count}");
                }
                else
                {
                    MessageBox.Show("Нет данных для импорта.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при импорте: {ex.Message}");
            }
        }

        private void ExportExamExcelButton_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog save = new SaveFileDialog();
            save.Filter = "Excel Files|*.xlsx";
            if (save.ShowDialog() == true)
            {
                ExportExamsToExcel(save.FileName);
            }
        }

        private void ExportExamsToExcel(string filePath)
        {
            try
            {
                using (AppContext context = new AppContext())
                {
                    var exams = context.exams.ToList();
                    IWorkbook workbook = new XSSFWorkbook();
                    ISheet sheet = workbook.CreateSheet("Exams");
                    // Заголовки
                    IRow header = sheet.CreateRow(0);
                    header.CreateCell(0).SetCellValue("title");
                    header.CreateCell(1).SetCellValue("reason");
                    header.CreateCell(2).SetCellValue("date");
                    header.CreateCell(3).SetCellValue("id_emp");
                    header.CreateCell(4).SetCellValue("date_prev");
                    header.CreateCell(5).SetCellValue("result_mark");
                    header.CreateCell(6).SetCellValue("date_next");
                    header.CreateCell(7).SetCellValue("number");
                    // Данные
                    for (int i = 0; i < exams.Count; i++)
                    {
                        var ex = exams[i];
                        IRow row = sheet.CreateRow(i + 1);
                        row.CreateCell(0).SetCellValue(ex.title);
                        row.CreateCell(1).SetCellValue(ex.reason);
                        row.CreateCell(2).SetCellValue(ex.date.ToString());
                        row.CreateCell(3).SetCellValue(ex.id_emp);
                        row.CreateCell(4).SetCellValue(ex.date_prev.ToString());
                        row.CreateCell(5).SetCellValue(ex.result_mark);
                        row.CreateCell(6).SetCellValue(ex.date_next.ToString());
                        row.CreateCell(7).SetCellValue(ex.number);
                    }
                    using (FileStream fs = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                    {
                        workbook.Write(fs);
                    }
                    MessageBox.Show("Экспорт завершён!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при экспорте: {ex.Message}");
            }
        }
    }
}
