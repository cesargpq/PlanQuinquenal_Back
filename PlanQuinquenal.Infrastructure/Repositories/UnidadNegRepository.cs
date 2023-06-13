using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using PlanQuinquenal.Core.DTOs.RequestDTO;
using PlanQuinquenal.Core.Entities;
using PlanQuinquenal.Core.Interfaces;
using PlanQuinquenal.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanQuinquenal.Infrastructure.Repositories
{
    public class UnidadNegRepository :IRepositoryUnidadNeg
    {
        private readonly PlanQuinquenalContext _context;

        public UnidadNegRepository(PlanQuinquenalContext context)
        {
            _context = context;
        }

        public async Task<Object> NuevoUnidadNeg(Unidad_negocio nvoUnidNeeg)
        {
            try
            {
                // Crear una nueva instancia de la entidad
                var nuevoUnidNeg = new Unidad_negocio
                {
                    cod_und = nvoUnidNeeg.cod_und,
                    nom_und = nvoUnidNeeg.nom_und,
                    estado_und = nvoUnidNeeg.estado_und
                };

                // Agregar la entidad al objeto DbSet y guardar los cambios en la base de datos
                _context.Unidad_negocio.Add(nuevoUnidNeg);
                _context.SaveChanges();

                var resp = new
                {
                    idMensaje = "1",
                    mensaje = "Se creo la unidad de negocio correctamente"
                };

                var json = JsonConvert.SerializeObject(resp);
                return json;
            }
            catch (Exception ex)
            {
                var resp = new
                {
                    idMensaje = "0",
                    mensaje = "Hubo un error al crear la unidad de negocio"
                };

                var json = JsonConvert.SerializeObject(resp);
                return json;
            }

        }

        public async Task<List<Unidad_negocio>> ObtenerUnidadNeg(string buscador)
        {
            List<Unidad_negocio> lstUnidadNeg = new List<Unidad_negocio>();
            var queryable = new List<Unidad_negocio>();
            if (buscador == "") {
                queryable = await _context.Unidad_negocio.ToListAsync();
            } else
            {
                queryable = await _context.Unidad_negocio.Where(x => x.nom_und == buscador).ToListAsync();
            }

            foreach (var unidNeg in queryable)
            {
                Unidad_negocio objUnidNeg = new Unidad_negocio();
                objUnidNeg.cod_und = unidNeg.cod_und;
                objUnidNeg.nom_und = unidNeg.nom_und;
                objUnidNeg.estado_und = unidNeg.estado_und;

                lstUnidadNeg.Add(objUnidNeg);

            }

            return lstUnidadNeg;

        }

        public async Task<Object> EliminarUnidadNeg(int cod_uniNeg)
        {
            // Obtener la entidad a eliminar por su clave primaria
            var codUnidNegElim = _context.Unidad_negocio.Find(cod_uniNeg);

            // Verificar si se encontró la entidad
            if (codUnidNegElim != null)
            {
                // Eliminar la entidad y guardar los cambios en la base de datos
                _context.Unidad_negocio.Remove(codUnidNegElim);
                _context.SaveChanges();

                var resp = new
                {
                    idMensaje = "1",
                    mensaje = "Se eliminó la unidad de negocio correctamente"
                };

                var json = JsonConvert.SerializeObject(resp);
                return json;
            }
            else
            {
                var resp = new
                {
                    idMensaje = "0",
                    mensaje = "Hubo el problema al eliminar la unidad de negocio"
                };

                var json = JsonConvert.SerializeObject(resp);
                return json;
            }

        }

        public async Task<Object> ActualizarUnidadNeg(Unidad_negocio uniNeg)
        {
            try
            {
                var modUnidNeg = _context.Unidad_negocio.FirstOrDefault(p => p.cod_und == uniNeg.cod_und);
                modUnidNeg.nom_und = uniNeg.nom_und;
                modUnidNeg.estado_und = uniNeg.estado_und;
                _context.SaveChanges();

                var resp = new
                {
                    idMensaje = "1",
                    mensaje = "Se modifico la unidad de negocio correctamente"
                };

                var json = JsonConvert.SerializeObject(resp);
                return json;
            }
            catch (Exception ex)
            {
                var resp = new
                {
                    idMensaje = "0",
                    mensaje = "Hubo un error al actualizar la unidad de negocio"
                };

                var json = JsonConvert.SerializeObject(resp);
                return json;
            }

        }
    }
}
