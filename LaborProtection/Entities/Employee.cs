using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LaborProtection.Entities
{
    public class Employee
    {
        public int? id { get; set; }
        public string surname { get; set; }
        public string name{ get; set; }
        public string patronymic { get; set; }
        public string phone { get; set; }
        public int post { get; set; }
        public int department { get; set; }
        public string email { get; set; }
        public DateOnly date_of_employment { get; set; }

        public Employee(int Id, string Surname, string Name, string Patronymic, string Phone, int Post, int Department, string Email, DateOnly Date_of_employment)
        {
            id = Id;
            surname = Surname;
            name = Name;
            patronymic = Patronymic;
            phone = Phone;
            post = Post;
            department = Department;
            email = Email;
            date_of_employment = Date_of_employment;
        }
        public Employee(string Surname, string Name, string Patronymic, string Phone, int Post, int Department, string Email, DateOnly Date_of_employment)
        {
            surname = Surname;
            name = Name;
            patronymic = Patronymic;
            phone = Phone;
            post = Post;
            department = Department;
            email = Email;
            date_of_employment = Date_of_employment;
        }

        public Employee(int Id, string Surname, string Name, string Patronymic, string Post)
        {
            id = Id;
            surname = Surname;
            name = Name;
            patronymic = Patronymic;
        }
        public Employee() { }
    }
}
