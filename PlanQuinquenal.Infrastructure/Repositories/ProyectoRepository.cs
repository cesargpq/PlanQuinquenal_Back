using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Newtonsoft.Json;
using OfficeOpenXml;
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
    public class ProyectoRepository : IRepositoryProyecto
    {
        private readonly PlanQuinquenalContext _context;

        public ProyectoRepository(PlanQuinquenalContext context)
        {
            _context = context;
        }

        public async Task<Object> NuevoProyecto(Proyectos nvoProyecto)
        {
            try
            {
                var nvoProyect = new Proyectos
                {
                    des_pry = nvoProyecto.des_pry,
                    cod_anioPA = nvoProyecto.cod_anioPA,
                    
                };

                // Agregar la entidad al objeto DbSet y guardar los cambios en la base de datos
                _context.Proyectos.Add(nvoProyect);
                _context.SaveChanges();
                var resp = new
                {
                    idMensaje = "1",
                    mensaje = "Se creó el proyecto correctamente"
                };

                var json = JsonConvert.SerializeObject(resp);
                return json;
            }
            catch (Exception ex)
            {
                var resp = new
                {
                    idMensaje = "0",
                    mensaje = "Hubo un error al crear el proyecto"
                };

                var json = JsonConvert.SerializeObject(resp);
                return json;
            }

        }

        public async Task<object> NuevosProyectosMasivo(ProyectoRequest reqMasivo)
        {
            var base64Content = reqMasivo.base64; 
            var bytes = Convert.FromBase64String(base64Content);
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            try
            {
                using (var memoryStream = new MemoryStream(bytes))
                {
                    using (var package = new ExcelPackage(memoryStream))
                    {
                        var worksheet = package.Workbook.Worksheets["Proyectos"];

                        for (int row = 2; row <= worksheet.Dimension.Rows; row++)
                        {
                            if (worksheet.Cells[row, 1].Value?.ToString() == null) { break; }
                            var valor1 = worksheet.Cells[row, 1].Value?.ToString();
                            var valor2 = worksheet.Cells[row, 2].Value?.ToString();
                            var valor3 = worksheet.Cells[row, 3].Value?.ToString();

                            var entidad = new Proyectos
                            {
                                des_pry = valor2
                            };

                            _context.Proyectos.Add(entidad);
                        }

                        _context.SaveChanges();
                    }
                }
                var resp = new
                {
                    idMensaje = "1",
                    mensaje = "Se crearon los proyectos correctamente"
                };

                var json = JsonConvert.SerializeObject(resp);
                return json;
            }
            catch (Exception ex)
            {
                var resp = new
                {
                    idMensaje = "0",
                    mensaje = "Hubo un error al crear los proyectos"
                };

                var json = JsonConvert.SerializeObject(resp);
                return json;
            }

            
        }

        public async Task<List<Proyectos>> ObtenerProyectos(FiltersProyectos filterProyectos)
        {
            List<Proyectos> lstPro = new List<Proyectos>();
            var queryable = _context.Proyectos
                                     .Where(x => filterProyectos.des_pry != "" ? x.des_pry == filterProyectos.des_pry : true)
                                     .Where(x => filterProyectos.cod_pry != "" ? x.cod_pry == filterProyectos.cod_pry : true)
                                     //.Where(x => filterProyectos.nro_exp != null ? x.nro_exp == filterProyectos.nro_exp : true)
                                     //.Where(x => filterProyectos.estado_pry != "" ? x.estado_pry == filterProyectos.estado_pry : true)
                                     .Where(x => filterProyectos.cod_etapa != null ? x.cod_etapa == filterProyectos.cod_etapa : true)
                                     //.Where(x => filterProyectos.por_avance != null ? x.por_avance == filterProyectos.por_avance : true)
                                     .Where(x => filterProyectos.cod_material != null ? x.cod_material == filterProyectos.cod_material : true)
                                     .Where(x => filterProyectos.cod_dist != null ? x.cod_dist == filterProyectos.cod_dist : true)
                                     .Where(x => filterProyectos.tipo_pry != null ? x.tipo_pry == filterProyectos.tipo_pry : true)
                                     .Where(x => filterProyectos.cod_PQ != null ? x.cod_PQ == filterProyectos.cod_PQ : true)
                                     .Where(x => filterProyectos.anioPQ != null ? x.anioPQ == filterProyectos.anioPQ : true)
                                     .Where(x => filterProyectos.cod_anioPA != null ? x.cod_anioPA == filterProyectos.cod_anioPA : true)
                                     .Where(x => filterProyectos.cod_malla != null ? x.cod_malla == filterProyectos.cod_malla : true)
                                     .Where(x => filterProyectos.constructor != null ? x.constructor == filterProyectos.constructor : true)
                                     .Where(x => filterProyectos.ingRespon != null ? x.ingRespon == filterProyectos.ingRespon : true)
                                     .Where(x => filterProyectos.user_reg != null ? x.user_reg == filterProyectos.user_reg : true)
                                     .Where(x => filterProyectos.fecha_gas != null ? x.fecha_gas == filterProyectos.fecha_gas : true)
                                     .Where(x => filterProyectos.cod_pryReemp != null ? x.cod_pryReemp == filterProyectos.cod_pryReemp : true)
                                     .AsQueryable();

            var entidades = await queryable.ToListAsync();
            lstPro = entidades;
            return lstPro;

        }

        public async Task<Object> ActualizarProyecto(Proyectos proyecto)
        {
            try
            {
                var modPry = _context.Proyectos.FirstOrDefault(p => p.id == proyecto.id);
                modPry.des_pry = proyecto.des_pry;
                modPry.cod_pry = proyecto.cod_pry;
                _context.SaveChanges();

                var resp = new
                {
                    idMensaje = "1",
                    mensaje = "Se modifico el proyecto correctamente"
                };

                var json = JsonConvert.SerializeObject(resp);
                return json;
            }
            catch (Exception ex)
            {
                var resp = new
                {
                    idMensaje = "0",
                    mensaje = "Hubo un error al actualizar el proyecto"
                };

                var json = JsonConvert.SerializeObject(resp);
                return json;
            }

        }
        public async Task<Proyectos> ObtenerProyectoxNro(string nroProy)
        {
            Proyectos lstPro = new Proyectos();
            var queryable = _context.Proyectos
                                     .Where(x =>  x.cod_pry == nroProy)
                                     .AsQueryable();

            double cantidad = await queryable.CountAsync();
            if (cantidad == 1)
            {
                var entidades = await queryable.ToListAsync();
                lstPro = entidades[0];
            }
                
            return lstPro;

        }
    }
}
