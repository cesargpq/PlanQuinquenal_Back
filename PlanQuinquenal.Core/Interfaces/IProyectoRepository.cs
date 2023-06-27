﻿using PlanQuinquenal.Core.DTOs.RequestDTO;
using PlanQuinquenal.Core.DTOs.ResponseDTO;
using PlanQuinquenal.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanQuinquenal.Core.Interfaces
{
    public interface IProyectoRepository
    {
        Task<ProyectoResponseDto> GetById(int id);
        Task<PaginacionResponseDto<ProyectoResponseDto>> GetAll(FiltersProyectos filterProyectos);

        Task<ResponseDTO> Add(ProyectoRequestDto proyectoRequestDto,int idUser);
    }
}
