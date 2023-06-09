using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanQuinquenal.Core.Entities
{
    
    public class TokenAuth
    {
        public int Id { get; set; }
        public int cod_usu { get; set; }
        public string Token { get; set; }
    }
}
