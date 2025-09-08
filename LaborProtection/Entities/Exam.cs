using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LaborProtection.Entities
{
    public class Exam
    {
        public int id { get; set; }
        public string title { get; set; }
        public int reason { get; set; }
        public DateOnly date { get; set; }
        public int id_emp { get; set; }
        public DateOnly date_prev { get; set; }
        public int result_mark { get; set; }
        public DateOnly date_next { get; set; }

        public Exam(int Id, string Title, int Reason, DateOnly Date, int Id_emp, DateOnly Date_prev, int Result_mark, DateOnly Date_next)
        {
            id = Id;
            title = Title;
            reason = Reason;
            date = Date;
            date_prev = Date_prev;
            date_next = Date_next;
            result_mark = Result_mark;
            id_emp = Id_emp;
        }
        public Exam(int Id, string Title, int Id_emp, int Result_mark)
        {
            id = Id;
            title = Title;
            result_mark = Result_mark;
            id_emp = Id_emp;
        }
        public Exam() { }
    }
}
