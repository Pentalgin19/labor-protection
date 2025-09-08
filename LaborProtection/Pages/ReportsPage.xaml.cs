using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using LaborProtection.Entities;

namespace LaborProtection.Pages
{
    public partial class ReportsPage : Page
    {
        private ReportsService _service = new ReportsService();
        private AppContext context;
        private int idDep, idEmp; 
        Dictionary<string, int> deps = new Dictionary<string, int>();
        Dictionary<string, int> emps = new Dictionary<string, int>();
        public ReportsPage()
        {
            InitializeComponent();
            context = new AppContext();
            load();
        }

        public void load()
        {
            List<String> listDeps = new List<String>();
            foreach (var d in context.dictionary.Where(d => d.category_id == 3).ToList())
            {
                idDep = d.id;
                deps[d.short_text] = idDep;
                listDeps.Add($"{d.short_text}");
            }
            DeptIdSqlBox.ItemsSource = listDeps;

            List<String> listEmps = new List<String>();
            foreach (var e in context.employees.ToList())
            {
                idEmp = (int)e.id;
                string s = $"{e.surname} {e.name.Substring(0, 1)}. {e.patronymic.Substring(0, 1)}.";
                emps[s] = idEmp;
                listEmps.Add(s);
            }
            EmpIdBox.ItemsSource = listEmps;
        }
        private void ShowEmployeesByDeptSql_Click(object sender, RoutedEventArgs e)
        {
            SqlEmployeesList.Items.Clear();
            var employees = _service.GetEmployeesByDepartmentSql(deps[DeptIdSqlBox.SelectedItem.ToString()]);
            foreach (var emp in employees)
                SqlEmployeesList.Items.Add($"{emp.id}: {emp.surname} {emp.name} ({emp.email})");
            
        }

        private void ShowEmpCountByDeptSql_Click(object sender, RoutedEventArgs e)
        {
            SqlEmpCountList.Items.Clear();
            var counts = _service.GetEmployeeCountByDepartmentSql();
            foreach (var item in counts)
                SqlEmpCountList.Items.Add($"Отдел {item.Department}: {item.Count} сотрудников");
        }

        private void ShowEmployeesHiredAfter_Click(object sender, RoutedEventArgs e)
        {
            LinqEmployeesList.Items.Clear();
            if (DateOnly.TryParse(HireDateBox.Text, out var date))
            {
                var employees = _service.GetEmployeesHiredAfter(date);
                foreach (var emp in employees)
                    LinqEmployeesList.Items.Add($"{emp.id}: {emp.surname} {emp.name} ({emp.date_of_employment})");
            }
            else
            {
                LinqEmployeesList.Items.Add("Некорректная дата");
            }
        }

        private void ShowExamsByEmployee_Click(object sender, RoutedEventArgs e)
        {
            LinqExamsList.Items.Clear();
            
            var exams = _service.GetExamsByEmployee(emps[EmpIdBox.SelectedItem.ToString()]);
            foreach (var ex in exams)
                LinqExamsList.Items.Add($"{ex.id}: {ex.title} ({ex.date:yyyy-MM-dd})");
           
        }
    }
} 