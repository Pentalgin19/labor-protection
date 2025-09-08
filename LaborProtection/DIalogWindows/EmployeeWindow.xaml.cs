using System;
using System.Windows;
using LaborProtection.Entities;
using LaborProtection.Pages;
using Microsoft.EntityFrameworkCore;

namespace LaborProtection
{
    /// <summary>
    /// Логика взаимодействия для EmployeeWindow.xaml
    /// </summary>
    public partial class EmployeeWindow : Window
    {
        public Employee? newEmp;
        public bool isEdit = false;
        public List<String>? posts = new List<string>();
        public List<String>? deps = new List<string>();
        private int? editId = null;
        public EmployeeWindow()
        {
            isEdit = false;
            InitializeComponent();
            
            EmploymentDatePicker.SelectedDate = DateTime.Today;
            load();
        }

        public EmployeeWindow(Employee selectEmp) {
            isEdit = true;
            InitializeComponent();
            TitleTextBlock.Text = "Редактировать сотрудника";
            load();
            using (AppContext context = new AppContext())
            {
                Employee selectEmpDb = context.employees.Find(selectEmp.id);
                editId = selectEmpDb.id;
                SurnameTextBox.Text = selectEmpDb.surname;
                NameTextBox.Text = selectEmpDb.name;
                PatronymicTextBox.Text = selectEmpDb.patronymic;
                PhoneTextBox.Text = selectEmpDb.phone;
                EmailTextBox.Text = selectEmpDb.email;
                foreach (var post in posts)
                {
                    if (selectEmpDb.post == Int32.Parse(post.Substring(0, post.IndexOf(' '))))
                    {
                        PostComboBox.SelectedItem = post;
                    }
                }
                foreach (var dep in deps)
                {
                    if (selectEmpDb.department == Int32.Parse(dep.Substring(0, dep.IndexOf(' '))))
                    {
                        DepartmentComboBox.SelectedItem = dep;
                    }
                }
                var a = selectEmpDb.date_of_employment.ToDateTime(TimeOnly.MinValue);
                EmploymentDatePicker.SelectedDate = a;
            }
        }
        public void load()
        {
            using (AppContext context = new AppContext())
            {
                foreach (var post in context.dictionary.Where(d => d.category_id == 2).ToList())
                {
                    posts.Add($"{post.id} {post.short_text}");
                }
                PostComboBox.ItemsSource = posts;

                foreach (var dep in context.dictionary.Where(d => d.category_id == 3).ToList())
                {
                    deps.Add($"{dep.id} {dep.short_text}");
                }
                DepartmentComboBox.ItemsSource = deps;
            }
        }
       
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (isEmpty()) {
                try
                {
                    string selectedPost = PostComboBox.SelectedValue.ToString();
                    int indPost = Int32.Parse(selectedPost.Substring(0, selectedPost.IndexOf(' ')));
                    string selectedDep = DepartmentComboBox.SelectedValue.ToString();
                    int indDep = Int32.Parse(selectedDep.Substring(0, selectedDep.IndexOf(' ')));
                    if (isEdit && editId.HasValue)
                    {
                        newEmp = new Employee(
                            editId.Value,
                            SurnameTextBox.Text.Trim(),
                            NameTextBox.Text.Trim(),
                            PatronymicTextBox.Text.Trim(),
                            PhoneTextBox.Text.Trim(),
                            indPost,
                            indDep,
                            EmailTextBox.Text.Trim(),
                            DateOnly.FromDateTime(EmploymentDatePicker.SelectedDate.Value));
                    }
                    else
                    {
                        newEmp = new Employee(
                            SurnameTextBox.Text.Trim(),
                            NameTextBox.Text.Trim(),
                            PatronymicTextBox.Text.Trim(),
                            PhoneTextBox.Text.Trim(),
                            indPost,
                            indDep,
                            EmailTextBox.Text.Trim(),
                            DateOnly.FromDateTime(EmploymentDatePicker.SelectedDate.Value));
                    }
                    DialogResult = true;
                    this.Close();
                }
                catch (Exception ex) { MessageBox.Show($"Ошибка при сохранении ew: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error); };
            }
                
        }

        private bool isEmpty()
        {
            if (string.IsNullOrEmpty(SurnameTextBox.Text)) { MessageBox.Show("Введите фамилию" + SurnameTextBox.Text); return false; }
            if (string.IsNullOrEmpty(NameTextBox.Text)) { MessageBox.Show("Введите имя"); return false; }
            if (string.IsNullOrEmpty(PhoneTextBox.Text)) { MessageBox.Show("Введите номер телефона"); return false; }
            if (string.IsNullOrEmpty(EmailTextBox.Text)) { MessageBox.Show("Введите почту"); return false; }
            if (PostComboBox.SelectedValue == null) { MessageBox.Show("Выберите должность"); return false; }
            if (DepartmentComboBox.SelectedValue == null) { MessageBox.Show("Выберите отдел"); return false; }
            if (EmploymentDatePicker.SelectedDate == null) { MessageBox.Show("Выберите дату трудоустройства"); return false; }
            else if (EmploymentDatePicker.SelectedDate >= DateTime.Now) { MessageBox.Show("Выберите корректную дату трудоустройства"); return false; }
            return true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e) { 
            DialogResult = false;
            this.Close();
        }
    }
} 