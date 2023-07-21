﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PlanQuinquenal.Core.DTOs;
using PlanQuinquenal.Core.Interfaces;

namespace PlanQuinquenal.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardRepository _dashboardRepository;

        public DashboardController(IDashboardRepository dashboardRepository)
        {
            this._dashboardRepository = dashboardRepository;
        }

        [HttpPost("ListarMaterial")]
        public async Task<IActionResult> ListarMaterial(RequestDashboradDTO p)
        {
            var resultado = await _dashboardRepository.ListarMaterial(p);
            return Ok(resultado);
        }
        [HttpPost("ListarPermisos")]
        public async Task<IActionResult> ListarPermisos(RequestDashboradDTO p)
        {
            var resultado = await _dashboardRepository.ListarPermisos(p);
            return Ok(resultado);
        }
        [HttpPost("ListarAvanceMensual")]
        public async Task<IActionResult> ListarAvanceMensual(RequestDashboradDTO p)
        {
            var resultado = await _dashboardRepository.ListarAvanceMensual(p);
            return Ok(resultado);
        }
    }
}
