using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanQuinquenal.Core.Utilities
{
    public class Constantes
    {
        public  int CANTIDAD_INTENTOS = 3;
        public const string PathFinanciamientoTemplate = "\\\\Content\\";
        public const string DobleFactor = "\\doble.html";
        public const string ErrorImport = "Hubo un error en la carga del archivo";
        public const string SatisfactorioImport = "Se cargó el archivo correctamente";
        public const string ActualizacionSatisfactoria = "Se actualizó correctamente el registro";
        public const string NoExistePQNQ = "No existe el Plan Quinquenal seleccionado";
        public const string RegistroExiste = "Registro encontrado satisfactoriamente";
        public const string BaremoNoExiste = "No existe un baremo con el código solicitado";
        public const string ActualizacionError = "Hubo un error al actualizar el registro";
        public const string ErrorSistema = "Hubo un error en el sistema";
        public const string ExisteRegistro = "Ya existe el registro en el sistema";
        public const string CreacionExistosa = "Se creó el registro satisfactoriamente";
    }
}
