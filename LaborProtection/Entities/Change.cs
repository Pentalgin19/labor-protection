using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LaborProtection.Entities
{
    internal class Change
    {
        private int _id;
        private int _id_user;
        private int _description;

        public int id { get; set; }
        public int id_user { get; set; }
        public int description  { get; set; }

        public Change(int Id, int Id_user, int Description)
        {
            id = Id;
            id_user = Id_user;
            description = Description;
        }

        public Change() { }
    }
}
