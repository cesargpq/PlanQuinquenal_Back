using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanQuinquenal.Core.Entities
{
    public class ProyectosP
    {
        public int Id { get; set; }
        [ForeignKey("TablaLogicaDatos")]
        public int cod_material { get; set; }
        public int cod_proyecto { get; set; }
        public  TablaLogicaDatos TablaLogicaDatos { get; set; }
        
    }
}
