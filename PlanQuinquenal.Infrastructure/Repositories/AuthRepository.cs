using Microsoft.EntityFrameworkCore;
using PlanQuinquenal.Core.DTOs;
using PlanQuinquenal.Core.DTOs.RequestDTO;
using PlanQuinquenal.Core.Entities;
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

        public async Task<bool> GetToken(int idUsuario)
        {
            var token = await _context.TokenAuth.Where(x=> x.cod_usu == idUsuario).FirstOrDefaultAsync();
            if (token.Token != null || !token.Equals(""))
            {
                return true;
            }
            else
            {
                return false;
            }
           
        }
        public async Task<bool> CerrarSesion(LoginRequestDTO login)
        {


            var userUpdate = await _context.Usuario.FirstOrDefaultAsync( x=> x.correo_usu == login.user);
            if(userUpdate != null)
            {
                if(!userUpdate.Conectado)
                {
                    return false;
                }
                userUpdate.Conectado = false;
                _context.Update(userUpdate);
                await _context.SaveChangesAsync();
                return true;
            }
            else
            {
                return false;
            }
           
        }
        public async Task<JwtResponse> Autenticar(LoginRequestDTO usuario)
        {
            JwtResponse jwt = new JwtResponse();
            usuario.password = hashService.Encriptar(usuario.password);
            var userResult = await _context.Usuario.Where(u => u.correo_usu == usuario.user).Include(s => s.Perfil).FirstOrDefaultAsync();
           if(userResult != null)
            {
                //if (userResult.Conectado)
                //{
                //    jwt = LoggedUser();
                //    return jwt;
                //}
                if (userResult.Interno)
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

                        if (!jwt.state && userResult.Intentos < _constantes.CANTIDAD_INTENTOS)
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

                jwt = await hashService.ConstruirToken(userResult, jwt);
                jwt.DobleFactor = userResult.DobleFactor;
                userResult.LastSesion = DateTime.Now;
                userResult.Conectado = true;
                _context.Update(userResult);
                List<string> correos = new List<string>();
                correos.Add(userResult.correo_usu);
                DobleFactorDTO message = new DobleFactorDTO();
                message.Nombre = userResult.nombre_usu + userResult.apellido_usu;
                message.DobleFactor = await hashService.GeneraDobuleFactor();
                string templateKey = "templateKey_DobleFactor";
                if (userResult.DobleFactor)
                {
                    var obj = new EmailData<DobleFactorDTO>
                    {
                        EmailType = 2,
                        EmailList = correos,
                        Model = message,
                        HtmlTemplateName = Constantes.DobleFactor
                    };
                    var resultDobleFactror = await hashService.EnviarDobleFactor(obj,message, templateKey);
                    if (resultDobleFactror)
                    {
                        DobleFactor db = new DobleFactor();
                        db.cod_usu= userResult.cod_usu;
                        db.Codigo = message.DobleFactor;
                        
                        var existe = await _context.DobleFactor.Where(x => x.cod_usu == userResult.cod_usu).FirstOrDefaultAsync();
                        
                        if (existe!=null)
                        {
                            existe.Codigo = db.Codigo;
                            _context.Update(existe);
                            
                        }
                        else
                        {
                            _context.Add(db);
                        }
                        
                     
                    }
                    
                }
                
                await _context.SaveChangesAsync();

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

                    //if (resultUser.Conectado)
                    //{

                    //    jwt = LoggedUser();
                    //    return jwt;
                    //}
                    jwt = ExisteUsuario();
                    return jwt;
                }
                else
                {
                    jwt = DatosInvalidos();
                    return jwt;
                }
            }
            catch (Exception)
            {

                throw;
            }
    
        }
        public static JwtResponse LoggedUser()
        {
            JwtResponse jwt = new JwtResponse();
            jwt.state = false;
            jwt.Message = "Hay una sesión activa, cierre y vuelva a ingresar";
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
