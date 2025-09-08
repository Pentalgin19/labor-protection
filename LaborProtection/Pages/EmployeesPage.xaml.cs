using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
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
using Microsoft.VisualBasic;
using Microsoft.Win32;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using Npgsql;

namespace LaborProtection.Pages
{
    /// <summary>
    /// Логика взаимодействия для EmployeesPage.xaml
    /// </summary>
    public partial class EmployeesPage : Page
    {
        private AppContext _context;
        //private Employee _selectedEmployee;


        public EmployeesPage()
        {
            InitializeComponent();
            _context = new AppContext();
            LoadEmployees();
        }

        private void LoadEmployees()
        {
            try
            {
                using (AppContext context = new AppContext())
                {
                    var employees = context.employees.ToList();
                    EmployeesDataGrid.ItemsSource = null;
                    EmployeesDataGrid.ItemsSource = employees;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке данных: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void AddEmployee_Click(object sender, RoutedEventArgs e) {
            var employeeWindow = new EmployeeWindow();
            if (employeeWindow.ShowDialog() == true)
            {
                try
                {
                    var newEmployee = employeeWindow.newEmp;
                    _context.employees.Add(newEmployee);
                    _context.SaveChanges();
                    LoadEmployees();
                    MessageBox.Show("Сотрудник успешно добавлен!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при добавлении сотрудника: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void EditEmployee_Click(object sender, RoutedEventArgs e) {
            if (EmployeesDataGrid.SelectedItems.Count != 1) { MessageBox.Show("Выберите один элемент"); return; }
            Employee selectEmp = (Employee)EmployeesDataGrid.SelectedItem;
            // Получаем актуальные данные из базы
            Employee selectEmpDb;
            using (var context = new AppContext())
            {
                selectEmpDb = context.employees.Find(selectEmp.id);
            }
            var empWindow = new EmployeeWindow(selectEmpDb);
            if (empWindow.ShowDialog() == true)
            {
                try
                {
                    using (var context = new AppContext())
                    {
                        var empToUpdate = context.employees.Find(selectEmp.id);
                        var emp = empWindow.newEmp;
                        if (empToUpdate != null && emp != null)
                        {
                            empToUpdate.surname = emp.surname;
                            empToUpdate.name = emp.name;
                            empToUpdate.patronymic = emp.patronymic;
                            empToUpdate.phone = emp.phone;
                            empToUpdate.email = emp.email;
                            empToUpdate.post = emp.post;
                            empToUpdate.department = emp.department;
                            empToUpdate.date_of_employment = emp.date_of_employment;
                            context.SaveChanges();
                        }
                    }
                    LoadEmployees();
                    MessageBox.Show("Сотрудник успешно обновлен!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при обновлении сотрудника: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
      
        private void DeleteEmployee_Click(object sender, RoutedEventArgs e) { 
            if (EmployeesDataGrid.SelectedItems.Count != 1) { MessageBox.Show("Выберите один элемент"); return; }
            Employee selectEmp = (Employee)EmployeesDataGrid.SelectedItem;
            Employee selectEmpDb = _context.employees.Find(selectEmp.id);
            try
            {
                _context.employees.Remove(selectEmpDb);
                _context.SaveChanges();
                LoadEmployees();
            }
            catch (Microsoft.EntityFrameworkCore.DbUpdateException ex)
            {
                var pgEx = ex.InnerException as Npgsql.PostgresException;   
                if (pgEx != null && pgEx.SqlState == "P0001" && pgEx.MessageText.Contains("Нельзя удалить сотрудника"))
                {
                    MessageBox.Show("Нельзя удалить сотрудника, у которого есть экзамены.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                {
                    string innerMsg = ex.InnerException != null ? ex.InnerException.ToString() : "";
                    MessageBox.Show($"Ошибка при удалении сотрудника: {ex.Message}\n\nInner: {innerMsg}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при удалении сотрудника: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ImportExcelButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog open = new OpenFileDialog();
            if (open.ShowDialog() == true) {
                string filePath = open.FileName;
                excel(filePath);
                MessageBox.Show(filePath);
            }
        }
        private void excel(string filePath)
        {
            List<Employee> employeesToAdd = new List<Employee>();
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
                            string surname = row.GetCell(0)?.ToString() ?? "";
                            string name = row.GetCell(1)?.ToString() ?? "";
                            string patronymic = row.GetCell(2)?.ToString() ?? "";
                            string phone = row.GetCell(3)?.ToString() ?? "";
                            string email = row.GetCell(4)?.ToString() ?? "";
                            int post = int.Parse(row.GetCell(5)?.ToString() ?? "0");
                            int department = int.Parse(row.GetCell(6)?.ToString() ?? "0");
                            DateOnly date_of_employment = DateOnly.Parse(row.GetCell(7)?.ToString() ?? DateOnly.FromDateTime(DateTime.Now).ToString());
                            var emp = new Employee(surname, name, patronymic, phone, post, department, email, date_of_employment);
                            employeesToAdd.Add(emp);
                        }
                        catch (Exception exRow)
                        {
                            MessageBox.Show($"Ошибка в строке {rowNum + 1}: {exRow.Message}");
                        }
                    }
                }
                if (employeesToAdd.Count > 0)
                {
                    using (AppContext context = new AppContext())
                    {
                        context.employees.AddRange(employeesToAdd);
                        context.SaveChanges();
                    }
                    LoadEmployees();
                    MessageBox.Show($"Импортировано сотрудников: {employeesToAdd.Count}");
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

        private void ExportExcelButton_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog save = new SaveFileDialog();
            save.Filter = "Excel Files|*.xlsx";
            if (save.ShowDialog() == true)
            {
                ExportEmployeesToExcel(save.FileName);
            }
        }

        private void ExportEmployeesToExcel(string filePath)
        {
            try
            {
                using (AppContext context = new AppContext())
                {
                    var employees = context.employees.ToList();
                    IWorkbook workbook = new XSSFWorkbook();
                    ISheet sheet = workbook.CreateSheet("Employees");
                    // Заголовки
                    IRow header = sheet.CreateRow(0);
                    header.CreateCell(0).SetCellValue("surname");
                    header.CreateCell(1).SetCellValue("name");
                    header.CreateCell(2).SetCellValue("patronymic");
                    header.CreateCell(3).SetCellValue("phone");
                    header.CreateCell(4).SetCellValue("email");
                    header.CreateCell(5).SetCellValue("post");
                    header.CreateCell(6).SetCellValue("department");
                    header.CreateCell(7).SetCellValue("date_of_employment");
                    // Данные
                    for (int i = 0; i < employees.Count; i++)
                    {
                        var emp = employees[i];
                        IRow row = sheet.CreateRow(i + 1);
                        row.CreateCell(0).SetCellValue(emp.surname);
                        row.CreateCell(1).SetCellValue(emp.name);
                        row.CreateCell(2).SetCellValue(emp.patronymic);
                        row.CreateCell(3).SetCellValue(emp.phone);
                        row.CreateCell(4).SetCellValue(emp.email);
                        row.CreateCell(5).SetCellValue(emp.post);
                        row.CreateCell(6).SetCellValue(emp.department);
                        row.CreateCell(7).SetCellValue(emp.date_of_employment.ToString());
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