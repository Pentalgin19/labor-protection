using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LaborProtection.Entities
{
    class Results
    {
        private int _id;
        private int _id_exam;
        private int _id_sphere;
        private int _id_category;
        private int _result;

        public int id { get; set; }
        public int id_exam { get; set; }
        public int id_sphere { get; set; }
        public int id_category { get; set; }
        public int result { get; set; }

        public Results(int Id, int Id_exam, int Id_sphere, int Id_category, int Result)
        {
            id = Id;
            id_exam = Id_exam;
            id_sphere = Id_sphere;
            id_category = Id_category;
            result = Result;
        }
        public Results() { }
    }
}
