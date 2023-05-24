using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanQuinquenal.Core.DTOs
{
    public class EmailData<T>
    {
        public int EmailType { get; set; }
        public T Model { get; set; }
        public List<string> SubjectData { get; set; }
        public List<string> EmailList { get; set; }
        public string HtmlTemplateName { get; set; }
        public List<string> AttachedFilePath { get; set; }
    }
}
