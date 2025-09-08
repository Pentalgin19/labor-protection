using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LaborProtection.Entities
{
    internal class User
    {
        //private int _id;
        //private string _login;
        //private string _password;
        //private string _email;
        //private string _name;
        //private string _surname;

        public int id { get; set; }
        public string login{ get; set; }
        public string password{ get; set; }
        public string email { get; set; }
        public int id_emp { get; set; }
        public int role { get; set; }

        public User(int Id, string Login, string Password, string Email, int Emp, int Role)
        {
            id = Id;
            login = Login;
            password = Password;
            email = Email;
            id_emp = Emp;
            role = Role;
        }

        public User() { }
    }
}
