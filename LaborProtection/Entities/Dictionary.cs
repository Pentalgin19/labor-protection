using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace LaborProtection.Entities
{
    class Dictionary
    {
        public int id { get; set; }
        public int category_id { get; set; }
        public string title { get; set; }
        public string? short_text { get; set; }
        public string? text { get; set; }

        public Dictionary(int Id, int Category_id, string Title, string Short_text, string Text)
        {
            id = Id;
            category_id = Category_id;
            title = Title;
            short_text = Short_text;
            text = Text;
        }
        public Dictionary(int Id, int Category_id, string Title, string Short_text)
        {
            id = Id;
            category_id = Category_id;
            title = Title;
            short_text = Short_text;
        }

        public Dictionary(int Id, string Title, int Category_id, string Text)
        {
            id = Id;
            category_id = Category_id;
            title = Title;
            text = Text;
        }

        public Dictionary() { }
    }
}
