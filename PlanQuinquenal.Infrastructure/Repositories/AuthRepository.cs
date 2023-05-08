using Microsoft.EntityFrameworkCore;
using PlanQuinquenal.Core.DTOs.RequestDTO;
using PlanQuinquenal.Core.Interfaces;
using PlanQuinquenal.Core.Utilities;
using PlanQuinquenal.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanQuinquenal.Infrastructure.Repositories
{
    public class AuthRepository : IAuthRepository
    {
        private readonly PlanQuinquenalContext _context;
        private readonly HashService hashService;
        private readonly Constantes _constantes;

        public AuthRepository(PlanQuinquenalContext context, HashService hashService, Constantes constantes)
        {
            this._context = context;
            this.hashService = hashService;
            this._constantes = constantes;
        }

        public async Task<JwtResponse> Autenticar(LoginRequestDTO usuario)
        {
            JwtResponse jwt = new JwtResponse();
            usuario.password = hashService.Encriptar(usuario.password);
            var userResult = await _context.Usuario.Where(u => u.correo_usu == usuario.user).FirstOrDefaultAsync();
           if(userResult != null)
            {
                if (userResult.Interno )
                {
                    jwt = ActiveDirectory(usuario);
                    if(!jwt.state && userResult.Intentos <_constantes.CANTIDAD_INTENTOS)
                    {
                        userResult.Intentos = userResult.Intentos + 1;
                        _context.Update(userResult);
                        await _context.SaveChangesAsync();
                        return DatosInvalidos();
                    }
                    else if(!jwt.state && (userResult.Intentos == _constantes.CANTIDAD_INTENTOS || !userResult.Estado))
                    {
                        userResult.Estado = false;
                        _context.Update(userResult);
                        await _context.SaveChangesAsync();
                        return BloqueadoUser();
                    }
                }
                else
                {
                    jwt = await LoginInterno(usuario,_context);

                    if(!jwt.state && userResult.Intentos < _constantes.CANTIDAD_INTENTOS)
                    {
                        userResult.Intentos = userResult.Intentos + 1;
                        _context.Update(userResult);
                        await _context.SaveChangesAsync();
                        return DatosInvalidos();
                    }
                    else if (!jwt.state && (userResult.Intentos == _constantes.CANTIDAD_INTENTOS || !userResult.Estado))
                    {
                        userResult.Estado = false;
                        _context.Update(userResult);
                        await _context.SaveChangesAsync();
                        return BloqueadoUser();
                    }
                }
            }
            else
            {
                jwt = new JwtResponse();
                jwt = NoExisteUsuario();
                return jwt;
            }
            if(jwt.state)
            {
                jwt = await hashService.ConstruirToken(userResult,jwt);
            }
            return jwt;
        }
        
        public static JwtResponse ActiveDirectory(LoginRequestDTO usuario)
        {

            JwtResponse jwt = new JwtResponse();
            jwt.state = true;
            return jwt;
        }
        public static async Task<JwtResponse>  LoginInterno(LoginRequestDTO usuario, PlanQuinquenalContext context)
        {
            JwtResponse jwt = new JwtResponse();
            try
            {
                var resultUser = await context.Usuario.Where(u => u.correo_usu == usuario.user && u.passw_user == usuario.password).FirstOrDefaultAsync();
                if (resultUser != null)
                {
                    jwt = ExisteUsuario();
                    return jwt;

                }
                else
                {
                    jwt = NoExisteUsuario();
                    return jwt;
                }
            }
            catch (Exception)
            {

                throw;
            }
         
          return jwt;
        }
        public static JwtResponse BloqueadoUser()
        {
            JwtResponse jwt = new JwtResponse();
            jwt.state = false;
            jwt.Message = "El usuario se encuentra bloqueado";
            return jwt;
        }
        public static JwtResponse DatosInvalidos()
        {
            JwtResponse jwt = new JwtResponse();
            
            jwt.state = false;
            jwt.Message = "Usuario o contraseña incorrectos";
            return jwt;
        }
        public static JwtResponse NoExisteUsuario()
        {
            JwtResponse jwt = new JwtResponse();
            jwt.state = false;
            jwt.Message = "El usuario no existe";
            return jwt;
        }
        public static JwtResponse ExisteUsuario()
        {
            JwtResponse jwt = new JwtResponse();
            jwt.state = true;
            jwt.Message = "Ingreso satisfactorio";
            return jwt;
        }

    }
}
