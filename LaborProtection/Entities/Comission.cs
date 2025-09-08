using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LaborProtection.Entities
{
    internal class Comission
    {
        private int _id;
        private int _id_emp;
        private int _id_exam;

        public int id { get; set; }
        public int id_emp { get; set; }
        public int id_exam { get; set; }

        public Comission(int Id, int Id_emp, int Id_exam)
        {
            id = Id;
            id_emp = Id_emp; 
            id_exam = Id_exam;
        }

        public Comission() { }
    }
}
