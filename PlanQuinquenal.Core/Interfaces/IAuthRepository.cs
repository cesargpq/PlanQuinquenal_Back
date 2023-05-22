﻿using PlanQuinquenal.Core.DTOs.RequestDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanQuinquenal.Core.Interfaces
{
    public interface IAuthRepository
    {
        Task<JwtResponse> Autenticar(LoginRequestDTO usuario);
        Task<bool> CerrarSesion(int id);

    }
}
