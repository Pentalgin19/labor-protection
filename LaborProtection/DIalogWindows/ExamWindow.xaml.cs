using System;
using System.Windows;
using System.Windows.Media;
using LaborProtection.Entities;

namespace LaborProtection
{
    /// <summary>
    /// Логика взаимодействия для ExamWindow.xaml
    /// </summary>
    public partial class ExamWindow : Window
    {
        public Exam Exam { get; private set; }
        private bool _isEditMode;
        private AppContext context;
        private List<String> emps = new List<string>();
        private List<String> marks = new List<string>();
        private List<String> reasons = new List<string>();
        public Exam newExam { get; set; }

        public ExamWindow()
        {
            InitializeComponent();
            _isEditMode = false;
            context = new AppContext();
            load();
            MarkComboBox.SelectedItem = "удовлетворительно";
            //ExamDatePicker.SelectedDate = DateTime.Today;
            //PrevDatePicker.SelectedDate = DateTime.Today;
            //NextDatePicker.SelectedDate = DateTime.Today.AddYears(1);
        }

        public ExamWindow(Exam exam)
        {
            InitializeComponent();
            _isEditMode = true;
            context = new AppContext();
            load();
            // Заполняем поля формы значениями exam
            if (exam != null)
            {
                TitleTextBox.Text = exam.title;
                // ReasonComboBox и MarkComboBox — если нужно, можно выбрать по id или тексту
                ExamDatePicker.SelectedDate = exam.date.ToDateTime(TimeOnly.MinValue);
                EmpComboBox.SelectedItem = emps.FirstOrDefault(e => e.Contains(exam.id_emp.ToString()));
                PrevDatePicker.SelectedDate = exam.date_prev.ToDateTime(TimeOnly.MinValue);
                MarkComboBox.SelectedItem = marks.FirstOrDefault(m => m == exam.result_mark.ToString());
                NextDatePicker.SelectedDate = exam.date_next.ToDateTime(TimeOnly.MinValue);
                NumberTextBox.Text = exam.number;
            }
        }

        public void load()
        {
            using (AppContext context = new AppContext())
            {
                int indEmp;
                int indMark;
                int indReason;
                foreach (var emp in context.employees.ToList())
                {
                    indEmp = (int)emp.id;
                    emps.Add($"{emp.surname} {emp.name} {emp.patronymic}");
                }
                EmpComboBox.ItemsSource = emps;

                foreach (var m in context.dictionary.Where(d => d.category_id == 9).ToList())
                {
                    indMark = m.id;
                    marks.Add($"{m.short_text}");
                }
                MarkComboBox.ItemsSource = marks;

                foreach (var reason in context.dictionary.Where(d => d.category_id == 8).ToList())
                {
                    indReason = reason.id;
                    reasons.Add($"{reason.short_text}");
                }
                ReasonComboBox.ItemsSource = reasons;
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (isEmpty())
            {
                try
                {
                    // Получаем значения из формы
                    string title = TitleTextBox.Text.Trim();
                    int reason = ReasonComboBox.SelectedIndex >= 0 ? context.dictionary.Where(d => d.category_id == 8).ToList()[ReasonComboBox.SelectedIndex].id : 0;
                    DateOnly date = DateOnly.FromDateTime(ExamDatePicker.SelectedDate.Value);
                    int id_emp = EmpComboBox.SelectedIndex >= 0 ? context.employees.ToList()[EmpComboBox.SelectedIndex].id ?? 0 : 0;
                    DateOnly date_prev = DateOnly.FromDateTime(PrevDatePicker.SelectedDate.Value);
                    int result_mark = 0;
                    var markText = MarkComboBox.SelectedItem?.ToString() ?? "";
                    switch (markText.ToLower())
                    {
                        case "неудовлетворительно": result_mark = 2; break;
                        case "удовлетворительно": result_mark = 3; break;
                        case "хорошо": result_mark = 4; break;
                        case "отлично": result_mark = 5; break;
                        default: result_mark = 0; break;
                    }
                    DateOnly date_next = DateOnly.FromDateTime(NextDatePicker.SelectedDate.Value);
                    string number = NumberTextBox.Text.Trim();

                    newExam = new Exam
                    {
                        title = title,
                        reason = reason,
                        date = date,
                        id_emp = id_emp,
                        date_prev = date_prev,
                        result_mark = result_mark,
                        date_next = date_next,
                        number = number
                    };
                    DialogResult = true;
                    this.Close();
                }
                catch (Exception ex) { MessageBox.Show($"Ошибка при сохранении: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error); };
            }
        }

        private bool isEmpty()
        {
            if (string.IsNullOrEmpty(TitleTextBox.Text)) { MessageBox.Show("Введите название проверки"); return false; }
            //if (string.IsNullOrEmpty(ReasonTextBox.Text)) { MessageBox.Show("Введите причину проверки"); return false; }
            if (string.IsNullOrEmpty(ExamDatePicker.Text)) { MessageBox.Show("Заполните дату проверки"); return false; }
            if (EmpComboBox.SelectedValue == null) { MessageBox.Show("Выберите сотрудника"); return false; }
            if (PrevDatePicker.SelectedDate == null) { MessageBox.Show("Заполните дату предыдущей проверки"); return false; }
            if (MarkComboBox.SelectedValue == null) { MessageBox.Show("Введите итоговую оценку"); return false; }
            if (NextDatePicker.SelectedDate == null) { MessageBox.Show("Заполните дату следующей проверки"); return false; }
            //if (string.IsNullOrEmpty(ReasonTextBox.Text)) { MessageBox.Show("Заполните дату следующей проверки"); return false; }
           // else if (EmploymentDatePicker.SelectedDate >= DateTime.Now) { MessageBox.Show("Введите номер протокола"); return false; }
            return true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void ExamDatePicker_SelectedDateChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (ExamDatePicker.SelectedDate != null)
            {
                NextDatePicker.SelectedDate = DateOnly.FromDateTime(ExamDatePicker.SelectedDate.Value).AddYears(1).ToDateTime(TimeOnly.MinValue);
            }
            else NextDatePicker.SelectedDate = null;
        }
        //if (string.IsNullOrWhiteSpace(TitleTextBox.Text))
        //{
        //    MessageBox.Show("Введите название проверки", "Ошибка валидации", MessageBoxButton.OK, MessageBoxImage.Warning);
        //    TitleTextBox.Focus();
        //    return false;
        //}

        //if (string.IsNullOrWhiteSpace(ReasonTextBox.Text))
        //{
        //    MessageBox.Show("Введите причину проверки", "Ошибка валидации", MessageBoxButton.OK, MessageBoxImage.Warning);
        //    ReasonTextBox.Focus();
        //    return false;
        //}

        //if (!ExamDatePicker.SelectedDate.HasValue)
        //{
        //    MessageBox.Show("Выберите дату проверки", "Ошибка валидации", MessageBoxButton.OK, MessageBoxImage.Warning);
        //    ExamDatePicker.Focus();
        //    return false;
        //}

        //if (string.IsNullOrWhiteSpace(EmployeeIdTextBox.Text))
        //{
        //    MessageBox.Show("Введите ID сотрудника", "Ошибка валидации", MessageBoxButton.OK, MessageBoxImage.Warning);
        //    EmployeeIdTextBox.Focus();
        //    return false;
        //}

        //if (!int.TryParse(EmployeeIdTextBox.Text.Trim(), out _))
        //{
        //    MessageBox.Show("ID сотрудника должен быть числом", "Ошибка валидации", MessageBoxButton.OK, MessageBoxImage.Warning);
        //    EmployeeIdTextBox.Focus();
        //    return false;
        //}

        //if (!PrevDatePicker.SelectedDate.HasValue)
        //{
        //    MessageBox.Show("Выберите предыдущую дату", "Ошибка валидации", MessageBoxButton.OK, MessageBoxImage.Warning);
        //    PrevDatePicker.Focus();
        //    return false;
        //}

        //if (string.IsNullOrWhiteSpace(ResGroupTextBox.Text))
        //{
        //    MessageBox.Show("Введите результат группы", "Ошибка валидации", MessageBoxButton.OK, MessageBoxImage.Warning);
        //    ResGroupTextBox.Focus();
        //    return false;
        //}

        //if (string.IsNullOrWhiteSpace(ResultMarkTextBox.Text))
        //{
        //    MessageBox.Show("Введите оценку", "Ошибка валидации", MessageBoxButton.OK, MessageBoxImage.Warning);
        //    ResultMarkTextBox.Focus();
        //    return false;
        //}

        //if (!NextDatePicker.SelectedDate.HasValue)
        //{
        //    MessageBox.Show("Выберите следующую дату", "Ошибка валидации", MessageBoxButton.OK, MessageBoxImage.Warning);
        //    NextDatePicker.Focus();
        //    return false;
        //}

        //if (string.IsNullOrWhiteSpace(NumberTextBox.Text))
        //{
        //    MessageBox.Show("Введите номер", "Ошибка валидации", MessageBoxButton.OK, MessageBoxImage.Warning);
        //    NumberTextBox.Focus();
        //    return false;
        //}

        //return true;
    }
} 